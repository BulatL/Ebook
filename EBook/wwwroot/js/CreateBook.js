$(document).ready(function () {
	console.log("ready");

	$("#PdfUpload").change(function () {
		ajaxCall(this.files);
	});
});

function ajaxCall(pdf) {

	var formData = new FormData();
	formData.append("pdf", pdf[0]);

	var jqxhr = $.ajax({
		url: "ReadMetadata",
		type: "POST",
		contentType: false,
		data: formData,
		dataType: "json",
		cache: false,
		processData: false,
		async: false,
		xhr: function () {
			var xhr = new window.XMLHttpRequest();
			xhr.upload.addEventListener("progress",
				function (evt) {
					if (evt.lengthComputable) {
						var progress = Math.round((evt.loaded / evt.total) * 100);

						// Do something with the progress
					}
				},
				false);
			return xhr;
		}
	})
		.done(function (data, textStatus, jqXhr) {
			let titleInput = $("#title");
			let authorInput = $("#author");
			let keywordsInput = $("#keywords");
			let publicationYearInput = $("#publicationYear");
			let filenameInput = $("#filename");
			let mimeInput = $("#mime");
			let oldfFilename = $("#oldfFilename");


			var res = data.split("^");
			for (var i = 0; i < res.length; i++) {
				console.log(res[i]);
				console.log(i);
				console.log("///////////////////////////////////////////////////////")
				if (i == 0) {
					titleInput.val(res[i]);
				}
				else if (i == 1) {
					authorInput.val(res[i]);
				}
				else if (i == 2) {
					keywordsInput.val(res[i]);
				}
				else if (i == 3) {
					publicationYearInput.val(res[i]);
				}
				else if (i == 4) {
					filenameInput.val(res[i]);
				}
				else if (i == 5) {
					mimeInput.val(res[i]);
					mimeInput.attr("readonly", true);
				}
				else {
					console.log("usao");
					console.log(res[i]);
					oldfFilename.val(res[i]);
				}
			}
		})
		.fail(function (jqXhr, textStatus, errorThrown) {
			if (errorThrown === "abort") {
				alert("Uploading was aborted");
			} else {
				alert("Uploading failed");
			}
		})
		.always(function (data, textStatus, jqXhr) { });
}