// Data page: switching between continuous and discrete extraction re-runs the point detection on
// the server and refreshes the preview panel.
(function () {
	"use strict";

	var message = document.getElementById("data-message");

	document.querySelectorAll("input[name=\"data-type\"]").forEach(function (input) {
		input.addEventListener("change", function () {
			message.textContent = "Extracting\u2026";
			$.post({ url: "?handler=DataType", data: { dataType: input.value } })
				.done(function (html) {
					$("#view").html(html);
					message.textContent = "";
				})
				.fail(function () {
					message.textContent = "The points could not be re-extracted.";
				});
		});
	});
})();
