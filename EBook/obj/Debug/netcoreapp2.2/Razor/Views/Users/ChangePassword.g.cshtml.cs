#pragma checksum "D:\Projects\E-book\EBook\EBook\Views\Users\ChangePassword.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "f970ffadc3ef9a50aa9371922710ac69c0f78456"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Users_ChangePassword), @"mvc.1.0.view", @"/Views/Users/ChangePassword.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Users/ChangePassword.cshtml", typeof(AspNetCore.Views_Users_ChangePassword))]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#line 1 "D:\Projects\E-book\EBook\EBook\Views\_ViewImports.cshtml"
using EBook;

#line default
#line hidden
#line 2 "D:\Projects\E-book\EBook\EBook\Views\_ViewImports.cshtml"
using EBook.Models;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"f970ffadc3ef9a50aa9371922710ac69c0f78456", @"/Views/Users/ChangePassword.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"8481bd3f05b9d736cdc733cab532ac816037a14f", @"/Views/_ViewImports.cshtml")]
    public class Views_Users_ChangePassword : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/lib/jquery/dist/jquery.min.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            BeginContext(0, 2, true);
            WriteLiteral("\r\n");
            EndContext();
#line 2 "D:\Projects\E-book\EBook\EBook\Views\Users\ChangePassword.cshtml"
  
    ViewData["Title"] = "ChangePassword";

#line default
#line hidden
            BeginContext(52, 96, true);
            WriteLiteral("\r\n<h1>ChangePassword</h1>\r\n\r\n<div class=\"row\">\r\n\t<div class=\"col-sm-12\">\r\n\t\t<input type=\"hidden\"");
            EndContext();
            BeginWriteAttribute("value", " value=\"", 148, "\"", 167, 1);
#line 10 "D:\Projects\E-book\EBook\EBook\Views\Users\ChangePassword.cshtml"
WriteAttributeValue("", 156, ViewBag.Id, 156, 11, false);

#line default
#line hidden
            EndWriteAttribute();
            BeginContext(168, 594, true);
            WriteLiteral(@" id=""hiddenId"" />
		<label class=""control-label"">New password</label>
		<input type=""password"" name=""password"" id=""firstPassword"" class=""form-control"" />
		<label class=""control-label"" id=""firstLabel""></label>
	</div>
	<div class=""col-sm-12"">
		<label class=""control-label"">Repeat password</label>
		<input type=""password"" name=""password"" id=""secondPassword"" class=""form-control"" />
		<label class=""control-label"" id=""secondLabel""></label>
	</div>
	<div class=""col-sm-12"">
		<input type=""button"" value=""Change"" class=""btn btn-primary"" onclick=""ChangePassowrd()"" />
	</div>
</div>
");
            EndContext();
            BeginContext(762, 55, false);
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "f970ffadc3ef9a50aa9371922710ac69c0f784564934", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            EndContext();
            BeginContext(817, 1048, true);
            WriteLiteral(@"

<script>
	function ChangePassowrd() {
		console.log(""1"");
		let id = $(""#hiddenId"").val();
		let firstPassowrd = $(""#firstPassword"").val();
		let secondPassword = $(""#secondPassword"").val();
		let firstLabel = $(""#firstLabel"");
		let secondLabel = $(""#secondLabel"");

		if (firstPassowrd == """")
		{
			firstLabel.empty();
			firstLabel.append(""Password can't be empty"");
			return;
		}
		if (!(firstPassowrd === secondPassword))
		{
			console.log(firstPassowrd);
			console.log(secondPassword);
			secondLabel.empty();
			secondLabel.append(""Password isn't same"");
			return;
		}
		$.ajax({
			url: '/Users/ChangePassword/' + id,
			type: 'Post',
			data: {
				""newPassword"": firstPassowrd
			},
			dataType: 'json',
			success: function (response) {
				alert(""password successfully changed"")
				if (response == ""Success"") {
					window.location.href = ""https://localhost:44325/Users/Edit/"" + id;
				}
			},
			error: function (response) {
				alert(""Something went wrong"");
		");
            WriteLiteral("\t}\r\n\t\t});\r\n\t}\r\n</script>");
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
