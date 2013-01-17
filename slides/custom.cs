$(document).ready ->
	paper = new Raphael(document.getElementById('svg_canvas_1'), 500, 500)
	#circle = paper.circle(100, 100, 80)
	tetronimo = paper.path("M 250 250 l 0 -50 l -50 0 l 0 -50 l -50 0 l 0 50 l -50 0 l 0 50 z")
	tetronimo.attr( gradient: '90-#526c7a-#64a0c1',  stroke: '#3b4449',  'stroke-width': 10,  'stroke-linejoin': 'round',  rotation: -90 )
	tetronimo.animate({transform: "r360"}, 2000, 'ease-out')
