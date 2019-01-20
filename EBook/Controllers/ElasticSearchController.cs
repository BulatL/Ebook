using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EBook.Models;
using EBook.Services;
using Elasticsearch.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nest;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace EBook.Controllers
{
	public class ElasticSearchController : Controller
	{

		private IHostingEnvironment _hostingEnvironment;
		private IBook _bookManager;

		public ElasticSearchController(IHostingEnvironment hostingEnvironment, IBook bookManager)
		{
			_hostingEnvironment = hostingEnvironment;
			_bookManager = bookManager;
		}
		public IActionResult CheckIndex()
		{
			var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
			.DefaultIndex("book");

			var client = new ElasticClient(settings);

			var searchResponse = client.Search<Book>(s => s
				 .AllTypes()
				 .From(0)
				 .Size(10)
				 .Query(q => q
						.MatchPhrasePrefix(m => m
							.Field(f => f.FileName)
							.Query("")
						)
						||
						q.MatchPhrasePrefix(m => m
							.Field(f => f.Author)
							.Query("")
						)
						||
						q.MatchPhrasePrefix(m => m
							.Field(f => f.Title)
							.Query("")
						)
				)
			);
			var book = searchResponse.Documents;
			if (book.Count == 0)
			{
				IndexFromDB();
			}
			return Json(book);
		}
		
		public async Task<IActionResult> Index(Book book)
		{
			var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
			.DefaultIndex("book");

			var client = new ElasticClient(settings);

			var asyncIndexResponse = await client.IndexDocumentAsync(book);
			return Json(asyncIndexResponse);
		}
		
		public ActionResult IndexFromDB()
		{

			var node = new Uri("http://localhost:9200");
			var settings = new ConnectionSettings(node);
			settings.DefaultIndex("book");
			var lowlevelClient = new ElasticLowLevelClient();

			List<Book> books = _bookManager.GetAllBooks().ToList();
			List<Object> listObj = new List<object>();
			for (int i = 0; i < books.Count(); i++)
			{
				var filePath = System.IO.Path.Combine(_hostingEnvironment.WebRootPath, "uploads/" + books[i].FileName);
				PdfReader reader = new PdfReader(filePath);
				var bodyFromPdf = string.Empty;
				for (int c = 1; c <= reader.NumberOfPages; c++)
				{
					ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
					bodyFromPdf += PdfTextExtractor.GetTextFromPage(reader, c, strategy);
				}
				reader.Close();

				var nesto = new { index = new { _index = "book", _type = "book", _id = books[i].Id } };
				var nesto2 = new
				{
					id = books[i].Id.ToString(),
					author = books[i].Author,
					categoryId = books[i].CategoryId.ToString(),
					fileName = books[i].FileName,
					keywords = books[i].Keywords,
					languageId = books[i].LanguageId.ToString(),
					mIME = books[i].MIME,
					publicationYear = books[i].PublicationYear.ToString("yyyy-MM-dd"),
					title = books[i].Title,
					body = bodyFromPdf
				};
				listObj.Add(nesto);
				listObj.Add(nesto2);
			}
			var indexResponseList = lowlevelClient.Bulk<StringResponse>(PostData.MultiJson(listObj));
			string responseStream = indexResponseList.Body;

			return Json(responseStream);
		}

		[Route("ElasticSearch/{name?}")]
		public JsonResult ElasticSearch(string name)
		{
			if (String.IsNullOrEmpty(name))
			{
				name = "";
			}
			var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
			.DefaultIndex("book");

			var client = new ElasticClient(settings);
			var responsedata = client.Search<Book>(s => s
											.Index("book")
											.AllTypes()
											.From(0)
											.Size(50)
											.Query(q => q
												 .MatchPhrasePrefix(m => m
													  .Field(f => f.FileName)
													  .Query(name)
												 )
												 ||
												 q.MatchPhrasePrefix(m => m
													  .Field(f => f.Author)
													  .Query(name)
												 )
												 ||
												 q.MatchPhrasePrefix(m => m
													  .Field(f => f.Title)
													  .Query(name)
												 )
											)
									  );

			var datasend = (from hits in responsedata.Hits
								 select hits.Source).OrderBy(e => e.Title).ToList();
			return Json(datasend);
		}

		public IActionResult Index2()
		{
			var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
			.DefaultIndex("book");

			var client = new ElasticClient(settings);

			var createIndexResponse = client.CreateIndex("questions", c => c
				 .Settings(s => s
					  .Analysis(a => a
							.CharFilters(cf => cf
								 .Mapping("programming_language", mca => mca
									  .Mappings(new[]
									  {
											"c# => csharp",
											"C# => Csharp"
									  })
								 )
							)
							.Analyzers(an => an
								 .Custom("question", ca => ca
									  .CharFilters("html_strip", "programming_language")
									  .Tokenizer("standard")
									  .Filters("standard", "lowercase", "stop")
								 )
							)
					  )
				 )
				 .Mappings(m => m
					  .Map<Book>(mm => mm
							.AutoMap()
							.Properties(p => p
								 .Text(t => t
									  .Name(n => n.Title)
									  .Analyzer("question")
								 )
							)
					  )
				 )
			);
			return Json(createIndexResponse); 
		}
	}
}