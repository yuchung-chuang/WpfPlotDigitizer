// Editor page: the browser counterpart of the WPF Editor control.
//
// Drawing happens on a canvas, but the undo history lives on the server in the shared
// IEditService. Every completed gesture posts the resulting image back and the server returns the
// refreshed panel, so the two frontends share one notion of "an edit".
(function () {
	"use strict";

	var tool = "none";
	var editor, base, overlay, baseCtx, overlayCtx;
	var imageWidth = 0;
	var imageHeight = 0;

	var stroke = null;      // in-progress pencil or eraser gesture
	var rect = null;        // committed rectangle selection
	var polygon = [];       // polygon vertices, image coordinates
	var polygonClosed = false;
	var busy = false;

	var message = document.getElementById("editor-message");

	function notify(text) {
		message.textContent = text || "";
	}

	function pencilRadius() {
		return Math.max(1, Math.round(imageWidth * 0.005));
	}

	function eraserSize() {
		return Math.max(2, Math.round(imageWidth * 0.05));
	}

	function toImagePoint(event) {
		var bounds = overlay.getBoundingClientRect();
		return {
			x: (event.clientX - bounds.left) / bounds.width * imageWidth,
			y: (event.clientY - bounds.top) / bounds.height * imageHeight
		};
	}

	function clearOverlay() {
		overlayCtx.clearRect(0, 0, imageWidth, imageHeight);
	}

	function drawOverlayRect(r) {
		clearOverlay();
		overlayCtx.save();
		overlayCtx.strokeStyle = "#dc3545";
		overlayCtx.lineWidth = Math.max(1, imageWidth / 400);
		overlayCtx.setLineDash([6, 4]);
		overlayCtx.strokeRect(r.x, r.y, r.width, r.height);
		overlayCtx.restore();
	}

	function drawOverlayPolygon(preview) {
		clearOverlay();
		if (!polygon.length) {
			return;
		}
		overlayCtx.save();
		overlayCtx.strokeStyle = "#dc3545";
		overlayCtx.lineWidth = Math.max(1, imageWidth / 400);
		overlayCtx.setLineDash([6, 4]);
		overlayCtx.beginPath();
		overlayCtx.moveTo(polygon[0].x, polygon[0].y);
		polygon.slice(1).forEach(function (point) { overlayCtx.lineTo(point.x, point.y); });
		if (preview) {
			overlayCtx.lineTo(preview.x, preview.y);
		}
		if (polygonClosed) {
			overlayCtx.closePath();
		}
		overlayCtx.stroke();
		overlayCtx.restore();
	}

	function normalise(a, b) {
		return {
			x: Math.min(a.x, b.x),
			y: Math.min(a.y, b.y),
			width: Math.abs(a.x - b.x),
			height: Math.abs(a.y - b.y)
		};
	}

	function eraseRect(r) {
		baseCtx.clearRect(Math.round(r.x), Math.round(r.y), Math.round(r.width), Math.round(r.height));
	}

	function erasePolygon(points) {
		baseCtx.save();
		baseCtx.globalCompositeOperation = "destination-out";
		baseCtx.beginPath();
		baseCtx.moveTo(points[0].x, points[0].y);
		points.slice(1).forEach(function (point) { baseCtx.lineTo(point.x, point.y); });
		baseCtx.closePath();
		baseCtx.fill();
		baseCtx.restore();
	}

	function drawDot(point) {
		baseCtx.save();
		baseCtx.fillStyle = "rgba(0, 0, 0, 1)";
		baseCtx.beginPath();
		baseCtx.arc(point.x, point.y, pencilRadius(), 0, Math.PI * 2);
		baseCtx.fill();
		baseCtx.restore();
	}

	function drawSegment(from, to) {
		// Sample along the segment so a fast drag still leaves a continuous line.
		var distance = Math.hypot(to.x - from.x, to.y - from.y);
		var steps = Math.max(1, Math.ceil(distance / Math.max(1, pencilRadius() / 2)));
		for (var i = 0; i <= steps; i++) {
			drawDot({
				x: from.x + (to.x - from.x) * i / steps,
				y: from.y + (to.y - from.y) * i / steps
			});
		}
	}

	function eraseSegment(from, to) {
		var size = eraserSize();
		var distance = Math.hypot(to.x - from.x, to.y - from.y);
		var steps = Math.max(1, Math.ceil(distance / Math.max(1, size / 2)));
		for (var i = 0; i <= steps; i++) {
			var x = from.x + (to.x - from.x) * i / steps;
			var y = from.y + (to.y - from.y) * i / steps;
			eraseRect({ x: x - size / 2, y: y - size / 2, width: size, height: size });
		}
	}

	function post(handler, data) {
		if (busy) {
			return;
		}
		busy = true;
		notify("Saving\u2026");

		$.post({ url: "?handler=" + handler, data: data || {} })
			.done(function (html) {
				$("#view").html(html);
				notify("");
				initialise();
			})
			.fail(function () {
				notify("The edit could not be saved.");
			})
			.always(function () {
				busy = false;
			});
	}

	function commit(tag) {
		post("Commit", { image: base.toDataURL("image/png"), tag: tag });
	}

	function resetSelection() {
		rect = null;
		polygon = [];
		polygonClosed = false;
		clearOverlay();
	}

	function onPointerDown(event) {
		if (tool === "none" || busy) {
			return;
		}
		event.preventDefault();
		var point = toImagePoint(event);

		if (tool === "pencil" || tool === "eraser") {
			overlay.setPointerCapture(event.pointerId);
			stroke = { last: point };
			if (tool === "pencil") {
				drawDot(point);
			}
			else {
				eraseSegment(point, point);
			}
			return;
		}

		if (tool === "rect") {
			overlay.setPointerCapture(event.pointerId);
			stroke = { origin: point };
			rect = null;
			return;
		}

		if (tool === "poly") {
			if (polygonClosed) {
				resetSelection();
			}
			polygon.push(point);
			drawOverlayPolygon();
		}
	}

	function onPointerMove(event) {
		if (busy) {
			return;
		}
		var point = toImagePoint(event);

		if (tool === "poly" && polygon.length && !polygonClosed) {
			drawOverlayPolygon(point);
			return;
		}

		if (!stroke) {
			return;
		}

		if (tool === "pencil") {
			drawSegment(stroke.last, point);
			stroke.last = point;
		}
		else if (tool === "eraser") {
			eraseSegment(stroke.last, point);
			stroke.last = point;
		}
		else if (tool === "rect") {
			rect = normalise(stroke.origin, point);
			drawOverlayRect(rect);
		}
	}

	function onPointerUp(event) {
		if (!stroke || busy) {
			return;
		}
		if (overlay.hasPointerCapture(event.pointerId)) {
			overlay.releasePointerCapture(event.pointerId);
		}

		var finished = tool;
		stroke = null;

		if (finished === "pencil") {
			commit("draw image");
		}
		else if (finished === "eraser") {
			commit("erase image");
		}
		else if (finished === "rect" && rect && rect.width >= 1 && rect.height >= 1) {
			notify("Press Delete to erase the selected region.");
		}
	}

	function onDoubleClick(event) {
		if (tool !== "poly" || polygon.length < 3) {
			return;
		}
		event.preventDefault();
		polygonClosed = true;
		drawOverlayPolygon();
		notify("Press Delete to erase the selected region.");
	}

	function isTextEntry(element) {
		if (!element) {
			return false;
		}
		if (element.isContentEditable) {
			return true;
		}
		var tag = (element.tagName || "").toUpperCase();
		if (tag === "TEXTAREA" || tag === "SELECT") {
			return true;
		}
		// Only text-entry inputs should swallow Delete; the toolbar radio buttons must not.
		return tag === "INPUT"
			&& !/^(radio|checkbox|button|submit|reset|file|range)$/i.test(element.type || "text");
	}

	function onKeyDown(event) {
		if (event.key !== "Delete" && event.key !== "Backspace") {
			return;
		}
		if (isTextEntry(event.target)) {
			return;
		}

		if (tool === "rect" && rect && rect.width >= 1 && rect.height >= 1) {
			event.preventDefault();
			eraseRect(rect);
			resetSelection();
			commit("Delete rectangle region");
			return;
		}

		if (tool === "poly" && polygonClosed && polygon.length >= 3) {
			event.preventDefault();
			erasePolygon(polygon);
			resetSelection();
			commit("Delete polygon region");
		}
	}

	function initialise() {
		editor = document.getElementById("editor");
		if (!editor) {
			return;
		}

		base = document.getElementById("editor-base");
		overlay = document.getElementById("editor-overlay");
		baseCtx = base.getContext("2d");
		overlayCtx = overlay.getContext("2d");
		imageWidth = parseInt(editor.dataset.imageWidth, 10);
		imageHeight = parseInt(editor.dataset.imageHeight, 10);
		resetSelection();

		var image = new Image();
		image.onload = function () {
			baseCtx.clearRect(0, 0, imageWidth, imageHeight);
			baseCtx.drawImage(image, 0, 0, imageWidth, imageHeight);
		};
		image.onerror = function () {
			notify("The image being edited could not be loaded.");
		};
		image.src = editor.dataset.imageUrl;

		overlay.addEventListener("pointerdown", onPointerDown);
		overlay.addEventListener("pointermove", onPointerMove);
		overlay.addEventListener("pointerup", onPointerUp);
		overlay.addEventListener("pointercancel", onPointerUp);
		overlay.addEventListener("dblclick", onDoubleClick);
	}

	document.querySelectorAll("input[name=\"editor-tool\"]").forEach(function (input) {
		input.addEventListener("change", function () {
			tool = input.value;
			resetSelection();
			notify("");
		});
	});

	// The history controls and the "clear border" button live inside the refreshed panel or the
	// toolbar, so bind them by delegation and re-use one handler for all of them.
	$(document).on("click", "[data-editor-action]", function () {
		post(this.dataset.editorAction);
	});

	$(document).on("change", "[data-editor-history]", function () {
		var steps = parseInt(this.value, 10);
		if (steps > 0) {
			post(this.dataset.editorHistory, { steps: steps });
		}
	});

	document.addEventListener("keydown", onKeyDown);

	initialise();
})();
