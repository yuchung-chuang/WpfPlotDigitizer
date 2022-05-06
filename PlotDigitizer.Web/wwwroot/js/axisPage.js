$('#view').load('/AxisPage/View');
interact('#axis')
	.draggable({
		onmove: event => {
			var target = event.target,
				// keep the dragged position in the data-x/data-y attributes
				x = (parseFloat(target.getAttribute('data-x')) || 0) + event.dx,
				y = (parseFloat(target.getAttribute('data-y')) || 0) + event.dy;

			// translate the element
			target.style.webkitTransform =
				target.style.transform =
				'translate(' + x + 'px, ' + y + 'px)';

			// update the posiion attributes
			target.setAttribute('data-x', x);
			target.setAttribute('data-y', y);
		},
		// keep the element within the area of it's parent
		modifiers: [
			interact.modifiers.restrictRect({
				restriction: 'parent',
				endOnly: true,
			})
		],
	})
	.resizable({
		onmove: event => {
			var target = event.target,
				x = (parseFloat(target.getAttribute('data-x')) || 0),
				y = (parseFloat(target.getAttribute('data-y')) || 0);

			// update the element's style
			target.style.width = event.rect.width + 'px';
			target.style.height = event.rect.height + 'px';
			// translate when resizing from top or left edges
			x += event.deltaRect.left;
			y += event.deltaRect.top;

			target.style.webkitTransform = target.style.transform =
				'translate(' + x + 'px,' + y + 'px)';

			target.setAttribute('data-x', x);
			target.setAttribute('data-y', y);
		},
		edges: { left: true, right: true, bottom: true, top: true },
		invert: 'reposition',
		modifiers: [
			interact.modifiers.restrictEdges({
				outer: 'parent'
			}),
		],
	});
$('#submit').click(() => {
	var svg = document.getElementById('svg'),
		axis = document.getElementById('axis'),
		rectSvg = svg.getBoundingClientRect(),
		rectAxis = axis.getBoundingClientRect(),
		x = parseInt(rectAxis.left - rectSvg.left ?? 0),
		y = parseInt(rectAxis.top - rectSvg.top ?? 0),
		width = parseInt(rectAxis.width ?? 0),
		height = parseInt(rectAxis.height ?? 0);
	$('#x').val(x);
	$('#y').val(y);
	$('#width').val(width);
	$('#height').val(height);
});
