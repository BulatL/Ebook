$(function () {
	let BooleanDiv = $("#BooleanDiv");
	let PhrazeDiv = $("#PhrazeDiv");
	let FuzzyDiv = $("#FuzzyDiv");

	BooleanDiv.hide();
	FuzzyDiv.hide();

	$("#querySelect").change(function () {
		let query = this.value;

		if (query == "Boolean") {
			BooleanDiv.show();
			FuzzyDiv.hide();
			PhrazeDiv.hide();
		}
		else if (query == "Phraze query") {
			PhrazeDiv.show();
			BooleanDiv.hide();
			FuzzyDiv.hide();
		}
		else if (query == "Fuzzy query") {
			FuzzyDiv.show();
			BooleanDiv.hide();
			PhrazeDiv.hide();
		}
	});
});

function Search() {
	let searchInput = $("#searchInput").val();
	let querySelect = $('#querySelect').find(":selected").text();

	let andOr = $('input[name=AndOr]:checked').val()

	let slop = $("#slop").val();

	let fuzziness = $("#fuzziness").val();
	let prefixLenght = $("#prefixLenght").val();
	let transpositionsboxes = $('input[name=transpositions]:checked').val();
	let maxExpansions = $("#maxExpansions").val();

	let transposition = false;

	if (transpositionsboxes == "on")
		transposition = true;

	if ($("#querySelect").val() == "Boolean") {
		$.ajax({
			url: 'https://localhost:44325/Elasticsearch/phrase/' + searchInput + "/" + slop,
			type: 'GET',
			dataType: 'json',
			success: function (response) {
				console.table(response);
				fillTable(response);
				hideColumns();
			},
			error: function (response, jqXHR) {
				console.table(response);
			}
		});
	}
	else if ($("#querySelect").val() == "Phraze query") {
		$.ajax({
			url: 'https://localhost:44325/Elasticsearch/phrase/' + searchInput + "/" + slop,
			type: 'GET',
			dataType: 'json',
			success: function (response) {
				console.table(response);
				fillTable(response);
				hideColumns();
			},
			error: function (response, jqXHR) {
				console.table(response);
			}
		});
	}
	else if ($("#querySelect").val() == "Fuzzy query") {
		$.ajax({
			url: 'https://localhost:44325/Elasticsearch/fuzzy/' + searchInput + "/" + fuzziness + "/" + prefixLenght
				+ "/" + transposition + "/" + maxExpansions,
			type: 'GET',
			dataType: 'json',
			success: function (response) {
				console.table(response);
				fillTable(response);
				hideColumns();
			},
			error: function (response, jqXHR) {
				console.table(response);
			}
		});
	}

	/*console.log("--------------------- \n");
	console.log(searchInput);
	console.log(querySelect);
	console.log("--------------------- \n Boolean \n");
	console.log(andOr);
	console.log("--------------------- \n phraze \n");
	console.log(slop);
	console.log("--------------------- \n fuzzy \n");
	console.log(fuzziness);
	console.log(prefixLenght);
	console.log(transpositionsboxes);
	console.log(maxExpansions);*/
}

function fillTable(data) {
	let tableBody = $("#tableBody");
	let hightlights = "";
	console.log(data);

	tableBody.empty();
	for (var i = 0; i < data.length; i++) {
		console.log(data[i]);
		for (var j = 0; j < data[i].hightlights.length; j++) {
			hightlights += data[i].hightlights[j] + "<br />";
		}
		console.log(hightlights);
		tableBody.append(`
		<tr>
			<td>${data[i].bookTitle}</td>
			<td>${hightlights}</td>
			<td class="download"><a class="download" href="/Books/Download?filename=${data[i].bookFilename}" style="display: none;">Download</a></td>
			<td class="register"><a class="register" href="">Register</a></td>
		</tr>
		`);
	}
}

function hideColumns() {
	$.ajax({
		url: 'https://localhost:44325/Users/GetLoggedInUser',
		type: 'GET',
		dataType: 'json',
		success: function (response) {
			if (typeof response === 'undefined') {
				$(".edit").hide();
				$(".delete").hide();
				$(".download").hide();
				$("#logout").hide();
				$("#createNew").hide();
				$("#editUser").hide();
			}
			else {
				let pathname = window.location.pathname.toLowerCase();
				let pathId = "/users/details/" + response.id;
				let navUl = $("#navUl");
				navUl.append
					(`<li class="nav-item">
						 		<a id="myProfile" class="nav-link text-dark" asp-area="" href="/users/details/${response.id}">My profile</a>
						 	</li>
							`);
				$("#RegisterHeader").hide()
				$(".register").hide()
				if (response.role != 0) {
					$(".edit").hide();
					$(".delete").hide();
					$(".download").hide();
					if (pathname != pathId) {
						$("#editUser").hide();
					}
				}
				$("#login").hide();
				$("#register").hide();
			}
		},
		error: function (response, jqXHR) {
			$("#logout").hide()
		}
	});
}