$(document).ready(function () {
	console.log("ready");
	$("#categorySelect").change(function () {
		let category = this.value;

		window.location.href = "https://localhost:44325/Books/" + category;
	});
});
