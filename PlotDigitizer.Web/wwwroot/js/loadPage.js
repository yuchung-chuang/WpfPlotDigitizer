// Browser equivalents of the desktop paste and drag-and-drop image sources.
(function () {
	"use strict";

	var dropZone = document.getElementById("drop-zone");
	var fileInput = document.getElementById("UploadedFile");
	var uploadForm = document.getElementById("upload-form");
	var pasteForm = document.getElementById("paste-form");
	var pastedImage = document.getElementById("pasted-image");

	if (!dropZone || !fileInput || !uploadForm || !pasteForm) {
		return;
	}

	function submitFile(file) {
		if (!file) {
			return;
		}
		// Hand the file to the normal multipart upload handler rather than inventing a
		// second code path for it.
		var transfer = new DataTransfer();
		transfer.items.add(file);
		fileInput.files = transfer.files;
		uploadForm.submit();
	}

	function submitDataUrl(dataUrl) {
		pastedImage.value = dataUrl;
		pasteForm.submit();
	}

	["dragenter", "dragover"].forEach(function (name) {
		dropZone.addEventListener(name, function (event) {
			event.preventDefault();
			dropZone.classList.add("drop-zone-active");
		});
	});

	["dragleave", "drop"].forEach(function (name) {
		dropZone.addEventListener(name, function (event) {
			event.preventDefault();
			dropZone.classList.remove("drop-zone-active");
		});
	});

	dropZone.addEventListener("drop", function (event) {
		var data = event.dataTransfer;
		if (data.files && data.files.length) {
			submitFile(data.files[0]);
			return;
		}

		var url = data.getData("text/uri-list") || data.getData("text/plain");
		if (url) {
			var address = document.getElementById("ImageAddress");
			address.value = url.trim();
			address.form.submit();
		}
	});

	dropZone.addEventListener("click", function () {
		fileInput.click();
	});

	document.addEventListener("paste", function (event) {
		var items = (event.clipboardData || {}).items || [];
		for (var i = 0; i < items.length; i++) {
			if (items[i].type.indexOf("image/") !== 0) {
				continue;
			}
			var file = items[i].getAsFile();
			var reader = new FileReader();
			reader.onload = function () { submitDataUrl(reader.result); };
			reader.readAsDataURL(file);
			event.preventDefault();
			return;
		}
	});
})();
