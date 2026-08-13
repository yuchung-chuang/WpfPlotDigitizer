// Filter page: three RGB range sliders, the web stand-in for the WPF RangeSlider control.
(function () {
	"use strict";

	var status = document.getElementById("filter-status");
	var sliders = ["filterR", "filterG", "filterB"].map(function (id) {
		return document.getElementById(id);
	});

	if (sliders.some(function (element) { return !element; })) {
		return;
	}

	sliders.forEach(function (element) {
		noUiSlider.create(element, {
			start: [parseInt(element.dataset.min, 10), parseInt(element.dataset.max, 10)],
			step: 1,
			connect: true,
			tooltips: true,
			behaviour: "tap-drag",
			range: { min: 0, max: 255 },
			format: {
				to: function (value) { return Math.round(value).toString(); },
				from: function (value) { return parseInt(value, 10); }
			}
		});
	});

	var pending = false;
	var queued = false;
	var timer = null;

	function currentBounds() {
		var values = sliders.map(function (element) { return element.noUiSlider.get(); });
		return {
			minR: parseInt(values[0][0], 10),
			maxR: parseInt(values[0][1], 10),
			minG: parseInt(values[1][0], 10),
			maxG: parseInt(values[1][1], 10),
			minB: parseInt(values[2][0], 10),
			maxB: parseInt(values[2][1], 10)
		};
	}

	// Filtering re-runs over the whole image, so coalesce slider movement and never let two
	// requests overlap.
	function apply() {
		if (pending) {
			queued = true;
			return;
		}

		pending = true;
		status.textContent = "Filtering\u2026";

		$.post({ url: "?handler=Filter", data: currentBounds() })
			.done(function (html) {
				$("#view").html(html);
				status.textContent = "";
			})
			.fail(function () {
				status.textContent = "The preview could not be updated.";
			})
			.always(function () {
				pending = false;
				if (queued) {
					queued = false;
					apply();
				}
			});
	}

	function schedule() {
		window.clearTimeout(timer);
		timer = window.setTimeout(apply, 200);
	}

	sliders.forEach(function (element) {
		element.noUiSlider.on("update", schedule);
	});
})();
