$(function () {
	let hiddenId = $("#hiddenId").val();
	if (hiddenId == 1) {
		$("#btnDelete").hide();
		$("#h3Delete").hide();
		$("#h1Delete").text("U can't delete this item")
	}
});