// A draggable and resizable box drawn over an image, the web equivalent of the WPF
// SelectionBox control. Coordinates are always kept in *image* pixels so the value posted to the
// server does not depend on how large the image happens to be rendered.
(function (global) {
	"use strict";

	function clamp(value, min, max) {
		return Math.min(Math.max(value, min), max);
	}

	function createSelectionBox(element) {
		var container = element.closest(".image-canvas");
		var imageWidth = parseFloat(container.dataset.imageWidth);
		var imageHeight = parseFloat(container.dataset.imageHeight);
		var minSize = 4;
		var listeners = [];

		function scale() {
			return container.clientWidth / imageWidth;
		}

		function get() {
			return {
				x: parseFloat(element.dataset.x) || 0,
				y: parseFloat(element.dataset.y) || 0,
				width: parseFloat(element.dataset.width) || 0,
				height: parseFloat(element.dataset.height) || 0
			};
		}

		function set(rect, silent) {
			var width = clamp(rect.width, minSize, imageWidth);
			var height = clamp(rect.height, minSize, imageHeight);
			var x = clamp(rect.x, 0, imageWidth - width);
			var y = clamp(rect.y, 0, imageHeight - height);

			element.dataset.x = x;
			element.dataset.y = y;
			element.dataset.width = width;
			element.dataset.height = height;
			render();

			if (!silent) {
				listeners.forEach(function (listener) { listener(get()); });
			}
		}

		function render() {
			var s = scale();
			var rect = get();
			element.style.left = (rect.x * s) + "px";
			element.style.top = (rect.y * s) + "px";
			element.style.width = (rect.width * s) + "px";
			element.style.height = (rect.height * s) + "px";
		}

		interact(element)
			.draggable({
				listeners: {
					move: function (event) {
						var s = scale();
						var rect = get();
						rect.x += event.dx / s;
						rect.y += event.dy / s;
						set(rect);
					}
				}
			})
			.resizable({
				edges: { left: true, right: true, bottom: true, top: true },
				listeners: {
					move: function (event) {
						var s = scale();
						var rect = get();
						set({
							x: rect.x + event.deltaRect.left / s,
							y: rect.y + event.deltaRect.top / s,
							width: event.rect.width / s,
							height: event.rect.height / s
						});
					}
				}
			});

		global.addEventListener("resize", render);
		render();

		return {
			element: element,
			name: element.dataset.name,
			get: get,
			set: set,
			render: render,
			onChange: function (listener) { listeners.push(listener); }
		};
	}

	global.createSelectionBox = createSelectionBox;
})(window);
