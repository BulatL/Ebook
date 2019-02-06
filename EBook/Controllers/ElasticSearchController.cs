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
using System.Dynamic;
using EBook.ViewModels;

namespace EBook.Controllers
{
	public class ElasticsearchController : Controller
	{
		private IHostingEnvironment _hostingEnvironment;
		private IBook _bookManager;

		public ElasticsearchController(IHostingEnvironment hostingEnvironment, IBook bookManager)
		{
			_hostingEnvironment = hostingEnvironment;
			_bookManager = bookManager;
		}

		public IActionResult Index()
		{
			List<SearchViewModel> viewModels = new List<SearchViewModel>();
			return View(viewModels);
		}

		public IActionResult IndexExist()
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
						||
						q.MatchPhrasePrefix(m => m
							.Field(f => f.Body)
							.Query("")
						)
				)
			);
			var book = searchResponse.Documents;
			if (book.Count == 0)
			{
				IndexFromDB();
			}
			var datasend = (from hits in searchResponse.Hits
								 select hits.Source).OrderBy(e => e.Title).ToList();
			return Json(datasend);
		}

		public async Task<IActionResult> IndexBook(Book book)
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

				var nesto = new { index = new { _index = "proba123", _type = "book", _id = books[i].Id } };
				var nesto2 = new
				{
					id = books[i].Id.ToString(),
					author = books[i].Author,
					categoryId = books[i].CategoryId.ToString(),
					category = books[i].Category,
					fileName = books[i].FileName,
					keywords = books[i].Keywords,
					languageId = books[i].LanguageId.ToString(),
					language = books[i].Language,
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
		//https://localhost:44325/Elasticsearch/phrase/отворене коридора/2
		[Route("[controller]/phrase/{search}/{slop}")]
		public JsonResult ElasticsearchPhrase(string search, int slop)
		{
			if (String.IsNullOrEmpty(search))
			{
				search = "";
			}
			var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
			.DefaultIndex("book");

			var client = new ElasticClient(settings);

			ISearchResponse<Book> responsedata = client.Search<Book>(s => s
											.Index("proba123")
											.AllTypes()
											.From(0)
											.Size(50)
											.Query(q => q
												 .MatchPhrase(m => m
													  .Field(f => f.Title)
													  .Query(search)
													  .Slop(slop)
												 )
												 ||
												 q.MatchPhrase(m => m
													  .Field(f => f.Author)
													  .Query(search)
													  .Slop(slop)
												 )
												 ||
												 q.MatchPhrase(m => m
													  .Field(f => f.Keywords)
													  .Query(search)
													  .Slop(slop)
												 )
												 ||
												 q.MatchPhrase(m => m
													  .Field(f => f.Body)
													  .Query(search)
													  .Slop(slop)
												 )
												 ||
												 q.MatchPhrase(m => m
													  .Field(f => f.Language.Name)
													  .Query(search)
													  .Slop(slop)
												 )
											)
											.Highlight(h => h
												 .PreTags("<strong>")
												 .PostTags("</strong>")
												 .Encoder(HighlighterEncoder.Html)
												 .Fields( fs => fs
													.Field(f => f.Title)
													.Type(HighlighterType.Plain)
													.PreTags("<strong>")
													.PostTags("</strong>")
													.ForceSource()
													.Fragmenter(HighlighterFragmenter.Span)
													.HighlightQuery(q => q
														.Match(m => m
														  .Field(p => p.Title)
														  .Query(search)
														)
													),

													fs => fs
													.Field(f => f.Author)
													.RequireFieldMatch(true)
													.Type(HighlighterType.Plain)
													.PreTags("<strong>")
													.PostTags("</strong>")
													.ForceSource()
													.Fragmenter(HighlighterFragmenter.Span)
													.HighlightQuery(q => q
														.Match(m => m
														  .Field(p => p.Author)
														  .Query(search)
														)
													),

													fs => fs
													.Field(f => f.Keywords)
													.Type(HighlighterType.Plain)
													.PreTags("<strong>")
													.PostTags("</strong>")
													.ForceSource()
													.Fragmenter(HighlighterFragmenter.Span)
													.HighlightQuery(q => q
														.Match(m => m
														  .Field(p => p.Keywords)
														  .Query(search)
														)
													),

													fs => fs
													.Field(f => f.Body)
													.Type(HighlighterType.Plain)
													.PreTags("<strong>")
													.PostTags("</strong>")
													.ForceSource()
													.Fragmenter(HighlighterFragmenter.Span)
													.HighlightQuery(q => q
														.Match(m => m
														  .Field(p => p.Body)
														  .Query(search)
														)
													),

													fs => fs
													.Field(f => f.Language.Name)
													.Type(HighlighterType.Plain)
													.PreTags("<strong>")
													.PostTags("</strong>")
													.ForceSource()
													.Fragmenter(HighlighterFragmenter.Span)
													.HighlightQuery(q => q
														.Match(m => m
														  .Field(p => p.Language.Name)
														  .Query(search)
														)
													)
												 )
											)
									  );

			List<object> returnList = new List<object>();

			foreach (var item in responsedata.Hits)
			{
				List<string> highlightsList = new List<string>();
				foreach (var h in item.Highlights.Values)
				{
					string highl = "<strong>";
					highl = h.Field + "</strong> : ";
					var highlight = h.Highlights.ToList();
					foreach (var hl in highlight)
					{
						highl += hl;
					}
					highlightsList.Add(highl);
				}
				var returnObj = new { hightlights = highlightsList, bookId = item.Source.Id, bookFilename = item.Source.FileName, bookTitle = item.Source.Title };
				returnList.Add(returnObj);
			}
			return Json(returnList);
		}

		/*
		 fuzziness The maximum edit distance. Defaults to AUTO. See Fuzzinessedit.

		prefix_length The number of initial characters which will not be “fuzzified”. 
		This helps to reduce the number of terms which must be examined. Defaults to 0.

		max_expansions The maximum number of terms that the fuzzy query will expand to. Defaults to 50.

		transpositions Whether fuzzy transpositions (ab → ba) are supported. Default is false.

		Warning This query can be very heavy if prefix_length is set to 0 and if max_expansions
		is set to a high number. It could result in every term in the index being examined!
					
			 * 
			 https://localhost:44325/Elasticsearch/fuzzy/бугке/2/0/true/100
			 */

		[Route("[controller]/fuzzy/{search}/{fuzziness}/{prefixLenght?}/{transpositions}/{maxExpansions?}")]
		public JsonResult ElasticsearchFuzzy(string search, int fuzziness, int? prefixLenght, bool transpositions, int? maxExpansions)
		{
			if (String.IsNullOrEmpty(search))
				search = "";

			if(maxExpansions == null)
				maxExpansions = 50;

			if (prefixLenght == null)
				prefixLenght = 0;

			var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
			.DefaultIndex("book");

			var client = new ElasticClient(settings);

			var responsedata = client.Search<Book>(s => s
											.Index("proba123")
											.AllTypes()
											.From(0)
											.Size(50)
											.Query(q => q
												 .Fuzzy(f => f
													.Field(fi => fi.Title)
													.Fuzziness(Fuzziness.EditDistance(fuzziness))
													.PrefixLength(prefixLenght)
													.Transpositions(transpositions)
													.MaxExpansions(maxExpansions)
													.Value(search)
												)
												|| q
												.Fuzzy(f => f
													.Field(fi => fi.Author)
													.Fuzziness(Fuzziness.EditDistance(fuzziness))
													.PrefixLength(prefixLenght)
													.Transpositions(transpositions)
													.MaxExpansions(maxExpansions)
													.Value(search)
												)
												|| q
												.Fuzzy(f => f
													.Field(fi => fi.Keywords)
													.Fuzziness(Fuzziness.EditDistance(fuzziness))
													.PrefixLength(prefixLenght)
													.Transpositions(transpositions)
													.MaxExpansions(maxExpansions)
													.Value(search)
												)
												|| q
												.Fuzzy(f => f
													.Field(fi => fi.Body)
													.Fuzziness(Fuzziness.EditDistance(fuzziness))
													.PrefixLength(prefixLenght)
													.Transpositions(transpositions)
													.MaxExpansions(maxExpansions)
													.Value(search)
												)
												|| q
												.Fuzzy(f => f
													.Field(fi => fi.Language.Name)
													.Fuzziness(Fuzziness.EditDistance(fuzziness))
													.PrefixLength(prefixLenght)
													.Transpositions(transpositions)
													.MaxExpansions(maxExpansions)
													.Value(search)
												)
											)
											.Highlight(h => h
												 .PreTags("<strong>")
												 .PostTags("</strong>")
												 .Encoder(HighlighterEncoder.Html)
												 .Fields(fs => fs
												  .Field(f => f.Title)
												  .Type(HighlighterType.Plain)
												  .PreTags("<strong>")
												  .PostTags("</strong>")
												  .ForceSource()
												  .Fragmenter(HighlighterFragmenter.Span)
												  .HighlightQuery(q => q
													 .Fuzzy(m => m
														.Field(fi => fi.Title)
														.Fuzziness(Fuzziness.EditDistance(fuzziness))
														.PrefixLength(prefixLenght)
														.Transpositions(transpositions)
														.MaxExpansions(maxExpansions)
														.Value(search)
														)
												  ),

													fs => fs
													.Field(f => f.Author)
													.RequireFieldMatch(true)
													.Type(HighlighterType.Plain)
													.PreTags("<strong>")
													.PostTags("</strong>")
													.ForceSource()
													.Fragmenter(HighlighterFragmenter.Span)
													.HighlightQuery(q => q
														.Fuzzy(m => m
														   .Field(p => p.Author)
															.Fuzziness(Fuzziness.EditDistance(fuzziness))
															.PrefixLength(prefixLenght)
															.Transpositions(transpositions)
															.MaxExpansions(maxExpansions)
															.Value(search)
														)
													),

													fs => fs
													.Field(f => f.Keywords)
													.Type(HighlighterType.Plain)
													.PreTags("<strong>")
													.PostTags("</strong>")
													.ForceSource()
													.Fragmenter(HighlighterFragmenter.Span)
													.HighlightQuery(q => q
														.Fuzzy(m => m
														   .Field(p => p.Keywords)
															.Fuzziness(Fuzziness.EditDistance(fuzziness))
															.PrefixLength(prefixLenght)
															.Transpositions(transpositions)
															.MaxExpansions(maxExpansions)
															.Value(search)
														)
													),

													fs => fs
													.Field(f => f.Body)
													.Type(HighlighterType.Plain)
													.PreTags("<strong>")
													.PostTags("</strong>")
													.ForceSource()
													.Fragmenter(HighlighterFragmenter.Span)
													.HighlightQuery(q => q
														.Fuzzy(m => m
														   .Field(p => p.Body)
															.Fuzziness(Fuzziness.EditDistance(fuzziness))
															.PrefixLength(prefixLenght)
															.Transpositions(transpositions)
															.MaxExpansions(maxExpansions)
															.Value(search)
														)
													),

													fs => fs
													.Field(f => f.Language.Name)
													.Type(HighlighterType.Plain)
													.PreTags("<strong>")
													.PostTags("</strong>")
													.ForceSource()
													.Fragmenter(HighlighterFragmenter.Span)
													.HighlightQuery(q => q
														.Fuzzy(m => m
														   .Field(p => p.Language.Name)
															.Fuzziness(Fuzziness.EditDistance(fuzziness))
															.PrefixLength(prefixLenght)
															.Transpositions(transpositions)
															.MaxExpansions(maxExpansions)
															.Value(search)
														)
													)
												 )
											)
									  );

			List<object> returnList = new List<object>();

			foreach (var item in responsedata.Hits)
			{
				List<string> highlightsList = new List<string>();
				foreach (var h in item.Highlights.Values)
				{
					string highl = "<strong>";
					highl = h.Field + "</strong> : ";
					var highlight = h.Highlights.ToList();
					foreach (var hl in highlight)
					{
						highl += hl;
					}
					highlightsList.Add(highl);
				}
				var returnObj = new { hightlights = highlightsList, bookId = item.Source.Id, bookFilename = item.Source.FileName, bookTitle = item.Source.Title };
				returnList.Add(returnObj);
			}
			return Json(returnList);
		}

		//https://localhost:44325/Elasticsearch/phrase/отворене коридора/2
		[Route("[controller]/bool/{search}/{andOrNot}")]
		public JsonResult ElasticsearchBool(string search, string andOrNot)
		{
			if (String.IsNullOrEmpty(search))
			{
				search = "";
			}
			var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
			.DefaultIndex("book");

			var client = new ElasticClient(settings);

			ISearchResponse<Book> responsedata = client.Search<Book>();

			switch (andOrNot)
			{
				case "and":
					responsedata = client.Search<Book>(s => s
											.Index("proba123")
											.AllTypes()
											.From(0)
											.Size(50)
											.Query(q => q
												.Bool(b => b
													.Must(m => m
														.Match(ma => ma
															.Field(f => f.Title)
															.Query(search)
														) || m
														.Match(ma => ma
															.Field(f => f.Author)
															.Query(search)
														) || m
														.Match(ma => ma
															.Field(f => f.Keywords)
															.Query(search)
														) || m
														.Match(ma => ma
															.Field(f => f.Language.Name)
															.Query(search)
														) || m
														.Match(ma => ma
															.Field(f => f.Body)
															.Query(search)
														)
													)
												)
											)
											.Highlight(h => h
												 .PreTags("<strong>")
												 .PostTags("</strong>")
												 .Encoder(HighlighterEncoder.Html)
												 .Fields(fs => fs
												  .Field(f => f.Title)
												  .Type(HighlighterType.Plain)
												  .PreTags("<strong>")
												  .PostTags("</strong>")
												  .ForceSource()
												  .Fragmenter(HighlighterFragmenter.Span)
												  .HighlightQuery(q => q
													  .Match(m => m
														 .Field(p => p.Title)
														 .Query(search)
													  )
												  ),

													fs => fs
													.Field(f => f.Author)
													.RequireFieldMatch(true)
													.Type(HighlighterType.Plain)
													.PreTags("<strong>")
													.PostTags("</strong>")
													.ForceSource()
													.Fragmenter(HighlighterFragmenter.Span)
													.HighlightQuery(q => q
														.Match(m => m
														  .Field(p => p.Author)
														  .Query(search)
														)
													),

													fs => fs
													.Field(f => f.Keywords)
													.Type(HighlighterType.Plain)
													.PreTags("<strong>")
													.PostTags("</strong>")
													.ForceSource()
													.Fragmenter(HighlighterFragmenter.Span)
													.HighlightQuery(q => q
														.Match(m => m
														  .Field(p => p.Keywords)
														  .Query(search)
														)
													),

													fs => fs
													.Field(f => f.Body)
													.Type(HighlighterType.Plain)
													.PreTags("<strong>")
													.PostTags("</strong>")
													.ForceSource()
													.Fragmenter(HighlighterFragmenter.Span)
													.HighlightQuery(q => q
														.Match(m => m
														  .Field(p => p.Body)
														  .Query(search)
														)
													),

													fs => fs
													.Field(f => f.Language.Name)
													.Type(HighlighterType.Plain)
													.PreTags("<strong>")
													.PostTags("</strong>")
													.ForceSource()
													.Fragmenter(HighlighterFragmenter.Span)
													.HighlightQuery(q => q
														.Match(m => m
														  .Field(p => p.Language.Name)
														  .Query(search)
														)
													)
												 )
											)
									  );
					break;
				case "or":
					responsedata = client.Search<Book>(s => s
											.Index("proba123")
											.AllTypes()
											.From(0)
											.Size(50)
											.Query(q => q
												.Bool(b => b
													.Should(m => m
														.Match(ma => ma
															.Field(f => f.Title)
															.Query(search)
														) || m
														.Match(ma => ma
															.Field(f => f.Author)
															.Query(search)
														) || m
														.Match(ma => ma
															.Field(f => f.Keywords)
															.Query(search)
														) || m
														.Match(ma => ma
															.Field(f => f.Language.Name)
															.Query(search)
														) || m
														.Match(ma => ma
															.Field(f => f.Body)
															.Query(search)
														)
													)
												)
											)
											.Highlight(h => h
												 .PreTags("<strong>")
												 .PostTags("</strong>")
												 .Encoder(HighlighterEncoder.Html)
												 .Fields(fs => fs
												  .Field(f => f.Title)
												  .Type(HighlighterType.Plain)
												  .PreTags("<strong>")
												  .PostTags("</strong>")
												  .ForceSource()
												  .Fragmenter(HighlighterFragmenter.Span)
												  .HighlightQuery(q => q
													  .Match(m => m
														 .Field(p => p.Title)
														 .Query(search)
													  )
												  ),

													fs => fs
													.Field(f => f.Author)
													.RequireFieldMatch(true)
													.Type(HighlighterType.Plain)
													.PreTags("<strong>")
													.PostTags("</strong>")
													.ForceSource()
													.Fragmenter(HighlighterFragmenter.Span)
													.HighlightQuery(q => q
														.Match(m => m
														  .Field(p => p.Author)
														  .Query(search)
														)
													),

													fs => fs
													.Field(f => f.Keywords)
													.Type(HighlighterType.Plain)
													.PreTags("<strong>")
													.PostTags("</strong>")
													.ForceSource()
													.Fragmenter(HighlighterFragmenter.Span)
													.HighlightQuery(q => q
														.Match(m => m
														  .Field(p => p.Keywords)
														  .Query(search)
														)
													),

													fs => fs
													.Field(f => f.Body)
													.Type(HighlighterType.Plain)
													.PreTags("<strong>")
													.PostTags("</strong>")
													.ForceSource()
													.Fragmenter(HighlighterFragmenter.Span)
													.HighlightQuery(q => q
														.Match(m => m
														  .Field(p => p.Body)
														  .Query(search)
														)
													),

													fs => fs
													.Field(f => f.Language.Name)
													.Type(HighlighterType.Plain)
													.PreTags("<strong>")
													.PostTags("</strong>")
													.ForceSource()
													.Fragmenter(HighlighterFragmenter.Span)
													.HighlightQuery(q => q
														.Match(m => m
														  .Field(p => p.Language.Name)
														  .Query(search)
														)
													)
												 )
											)
									  );
					break;
				case "not":
					responsedata = client.Search<Book>(s => s
											.Index("proba123")
											.AllTypes()
											.From(0)
											.Size(50)
											.Query(q => q
												.Bool(b => b
													.MustNot(m => m
														.Match(ma => ma
															.Field(f => f.Title)
															.Query(search)
														) || m
														.Match(ma => ma
															.Field(f => f.Author)
															.Query(search)
														) || m
														.Match(ma => ma
															.Field(f => f.Keywords)
															.Query(search)
														) || m
														.Match(ma => ma
															.Field(f => f.Language.Name)
															.Query(search)
														) || m
														.Match(ma => ma
															.Field(f => f.Body)
															.Query(search)
														)
													)
												)
											)
									  );
					break;
			}
			
			List<object> returnList = new List<object>();

			foreach (var item in responsedata.Hits)
			{
				List<string> highlightsList = new List<string>();
				foreach (var h in item.Highlights.Values)
				{
					string highl = "<strong>";
					highl = h.Field + "</strong> : "; 
					var highlight = h.Highlights.ToList();
					foreach (var hl in highlight)
					{
						highl += hl;
					}
					highlightsList.Add(highl);
				}
				var returnObj = new { hightlights = highlightsList, bookId = item.Source.Id, bookFilename = item.Source.FileName, bookTitle = item.Source.Title };
				returnList.Add(returnObj);
			}
			return Json(returnList);
		}

		//pronalazi sve indexe
		[Route("ElasticsearchMatchAll")]
		public JsonResult ElasticsearchMatchAll()
		{
			var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
			.DefaultIndex("book");

			var client = new ElasticClient(settings);

			var responsedata = client.Search<Book>(s => s
											.Index("proba123")
											.AllTypes()
											.From(0)
											.Size(50)
											.Query(q => q
												 .MatchAll()
											)
									  );

			var datasend = (from hits in responsedata.Hits
								 select hits.Source).OrderBy(e => e.Title).ToList();
			
			return Json(datasend);
		}
		[HttpDelete]
		public JsonResult DeleteIndex()
		{
			var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
			.DefaultIndex("book");

			var client = new ElasticClient(settings);
			var response = client.DeleteIndex("proba123");

			return Json(response);
		}

		[HttpDelete]
		public JsonResult DeleteDocument(int id)
		{
			var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
			.DefaultIndex("book");

			var client = new ElasticClient(settings);
			var response = client.Delete<Book>(id, d => d
															.Index("proba123")
															.Type("book")
														);

			return Json(response);
		}
		[HttpPut]
		public JsonResult UpdateIndex(int id)
		{
			var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
			.DefaultIndex("book");

			var client = new ElasticClient(settings);

			Book book = _bookManager.GetById(id);

			var filePath = System.IO.Path.Combine(_hostingEnvironment.WebRootPath, "uploads/" + book.FileName);
			PdfReader reader = new PdfReader(filePath);
			var bodyFromPdf = string.Empty;
			for (int c = 1; c <= reader.NumberOfPages; c++)
			{
				ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
				bodyFromPdf += PdfTextExtractor.GetTextFromPage(reader, c, strategy);
			}
			reader.Close();
			
			dynamic updateFields = new ExpandoObject();
			updateFields.Id = book.Id;
			updateFields.Author = book.Author;
			updateFields.Body = bodyFromPdf;
			updateFields.Category = book.Category;
			updateFields.FileName = book.FileName;
			updateFields.Keywords = book.Keywords;
			updateFields.Language = book.Language;
			updateFields.MIME = book.MIME;
			updateFields.PublicationYear = book.PublicationYear;
			updateFields.Title = book.Title;


			var response = client.UpdateAsync<Book, dynamic>(new DocumentPath<Book>(id), u => u
			.Index("proba123").Doc(updateFields));
			
			return Json(response);
		}

		public IActionResult CreateNewIndex()
		{
			var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
			.DefaultIndex("book");

			var client = new ElasticClient(settings);

			var createIndexResponse = client.CreateIndex("proba123", c => c
				 .Settings(s => s
					  .Analysis(a => a
							.TokenFilters(t => t
								.Stop("serbian_stopwords", stw => stw
									.StopWords("i", "a", "ili", "ali", "pa", "te", "da", "u", "po", "na")))
							.CharFilters(cf => cf
								 .Mapping("serbian_analyzer", mca => mca
									  .Mappings(new[]
									  {
											"a => \u0430",
											"b => \u0431",
											"c => \u0446",
											"d => \u0434",
											"e => \u0435",
											"f => \u0444",
											"g => \u0433",
											"h => \u0445",
											"i => \u0438",
											"j => \u0458",
											"k => \u043A",
											"l => \u043B",
											"m => \u043C",
											"n => \u043D",
											"o => \u043E",
											"p => \u043F",
											"r => \u0440",
											"s => \u0441",
											"t => \u0442",
											"u => \u0443",
											"v => \u0432",
											"z => \u0437",
											"A => \u0410",
											"B => \u0411",
											"C => \u0426",
											"D => \u0414",
											"E => \u0415",
											"F => \u0424",
											"G => \u0413",
											"H => \u0425",
											"I => \u0418",
											"J => \u0408",
											"K => \u041A",
											"L => \u041B",
											"M => \u041C",
											"N => \u041D",
											"O => \u041E",
											"P => \u041F",
											"R => \u0420",
											"S => \u0421",
											"T => \u0422",
											"U => \u0423",
											"V => \u0412",
											"Z => \u0417",
											"\u0107 => \u045B", //ć  => ћ
											"\u010D => \u0447", //č  => ч
											"\u0111 => \u0452", //đ  => ђ
											"\u0161 => \u0448", //š  => ш
											"\u017E => \u0436", //ž  => ж
											"\u0106 => \u040B", //Ć  => Ћ
											"\u010C => \u0427", //Č  => Ч
											"\u0110 => \u0402", //Đ  => Ђ
											"\u0160 => \u0428", //Š  => Ш
											"\u017D => \u0416", //Ž  => Ж   
											"\u01C6 => \u045F", //dž  => џ
											"\u01C4 => \u040F", //DŽ  => Џ
											"lj => \u0459",
											"nj => \u045A",
											"Lj => \u0409",
											"Nj => \u040A"
									  })
								 )
							)
							.Analyzers(an => an
								 .Custom("test1234Analyzer", ca => ca
									  .CharFilters("serbian_analyzer")
									  .Tokenizer("standard")
									  .Filters("standard", "lowercase", "serbian_stopwords")
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
									  .Analyzer("test1234Analyzer")
								 ).Text(t => t
									  .Name(n => n.Keywords)
									  .Analyzer("test1234Analyzer")
								 ).Text(t => t
									  .Name(n => n.Language.Name)
									  .Analyzer("test1234Analyzer")
								 ).Text(t => t
									  .Name(n => n.MIME)
									  .Analyzer("test1234Analyzer")
								 ).Date(t => t
									  .Name(n => n.PublicationYear)
								 ).Text(t => t
									  .Name(n => n.FileName)
									  .Analyzer("test1234Analyzer")
								 ).Text(t => t
									  .Name(n => n.Category.Name)
									  .Analyzer("test1234Analyzer")
								 ).Text(t => t
									  .Name(n => n.Author)
									  .Analyzer("test1234Analyzer")
								 ).Text(t => t
									  .Name(n => n.Body)
									  .Analyzer("test1234Analyzer")
								 )
							)
					  )
				 )
			);
			return Json(createIndexResponse); 
		}
	}
}