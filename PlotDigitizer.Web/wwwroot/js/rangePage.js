// Range page: mirrors each OCR selection box into the hidden form fields that carry its
// rectangle back to the server.
(function () {
	"use strict";

	function fieldFor(boxName, field) {
		return document.querySelector("input[data-box=\"" + boxName + "\"][data-field=\"" + field + "\"]");
	}

	function publish(boxName, rect) {
		fieldFor(boxName, "X").value = Math.round(rect.x);
		fieldFor(boxName, "Y").value = Math.round(rect.y);
		fieldFor(boxName, "Width").value = Math.round(rect.width);
		fieldFor(boxName, "Height").value = Math.round(rect.height);
	}

	document.querySelectorAll(".selection-box[data-target]").forEach(function (element) {
		var name = element.dataset.target;
		var box = window.createSelectionBox(element);
		box.onChange(function (rect) { publish(name, rect); });
		publish(name, box.get());
	});
})();
