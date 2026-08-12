// Axis page: keeps the hidden form fields in step with the dragged selection box.
(function () {
	"use strict";

	var box;

	function round(value) {
		return Math.round(value);
	}

	function publish(rect) {
		document.getElementById("axis-x").value = round(rect.x);
		document.getElementById("axis-y").value = round(rect.y);
		document.getElementById("axis-width").value = round(rect.width);
		document.getElementById("axis-height").value = round(rect.height);
		document.getElementById("axis-readout").textContent =
			"x " + round(rect.x) + ", y " + round(rect.y) +
			", " + round(rect.width) + " \u00d7 " + round(rect.height) + " px";
	}

	function initialise() {
		var element = document.getElementById("axis-box");
		if (!element) {
			return;
		}
		box = window.createSelectionBox(element);
		box.onChange(publish);
		publish(box.get());
	}

	document.getElementById("detect-axis").addEventListener("click", function () {
		window.plotDigitizer.refreshPanel("#view", "?handler=Detect").done(initialise);
	});

	initialise();
})();
