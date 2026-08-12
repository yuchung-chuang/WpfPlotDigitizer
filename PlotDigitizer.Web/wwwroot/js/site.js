// Shared behaviour for every PlotDigitizer page.
(function () {
	"use strict";

	// The page handlers are protected by antiforgery validation, so every non-safe ajax request
	// has to carry the token that the layout renders.
	$(document).ajaxSend(function (event, jqxhr, settings) {
		if (/^(GET|HEAD|OPTIONS|TRACE)$/i.test(settings.type || "GET")) {
			return;
		}
		var token = $("input[name=\"__RequestVerificationToken\"]").first().val();
		if (token) {
			jqxhr.setRequestHeader("RequestVerificationToken", token);
		}
	});

	var plotDigitizer = {
		/**
		 * Replaces the contents of a panel with a server rendered partial. This is the single
		 * refresh mechanism used by every page, mirroring how the desktop app re-renders one
		 * control when a model property changes.
		 */
		refreshPanel: function (selector, url) {
			return $.get(url).done(function (html) {
				$(selector).html(html);
			});
		},

		/** Shows a dismissible inline message; the web replacement for a modal message box. */
		notify: function (selector, message, level) {
			if (!message) {
				$(selector).empty();
				return;
			}
			$(selector).html(
				$("<div>")
					.addClass("alert alert-" + (level || "warning"))
					.attr("role", "alert")
					.text(message));
		}
	};

	window.plotDigitizer = plotDigitizer;
})();
