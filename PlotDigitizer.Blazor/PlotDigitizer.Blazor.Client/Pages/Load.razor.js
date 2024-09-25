window.createObjectURL = function (inputFile) {
	const url = URL.createObjectURL(inputFile.files[0]);
	window.addEventListener('unload', () => URL.revokeObjectURL(url), { once: true });
	return url;
};