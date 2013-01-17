arc = (val, total, radius) ->
	val = total if val > total
	alpha = 360 / total * val
	a = (90 - alpha) * Math.PI / 180
	x = 120 + radius * Math.cos(a)
	y = 120 - radius * Math.sin(a)
	path=[];
	if total == val
		path = [["M", 120, 120 - radius], ["A", radius, radius, 0, 1, 1, 119.99, 120 - radius]];
	else
		path = [["M", 120, 120 - radius], ["A", radius, radius, 0, +(alpha > 180), 1, x, y]]
	return {path: path}

class CountDownTimer
	slide_dur: 0
	compound_dur:15*60
	first_slide_transition: 0
	last_slide_transition: 0
	@started: false

	delta_to_s: (delta) ->
			neg = delta < 0
			delta = Math.floor(Math.abs(delta))
			minutes = Math.floor(delta/60) 
			seconds = delta%60
			seconds ="0#{seconds}" if seconds < 10
			return "#{if neg then "-" else ""}#{minutes}:#{seconds}"

	set_transition: (expectation) ->
			@start()
			seconds = new Date().getTime() / 1000
			slide_delta = (@slide_dur+@last_slide_transition) - seconds
			console.log "exp: #{expectation} delta: #{slide_delta} dur: #{@slide_dur} last: #{@last_slide_transition}"
			@slide_dur=slide_delta+expectation
			@last_slide_transition = seconds
			@outer.attr {arc: [seconds-@first_slide_transition+1, @compound_dur, 100]}

	start: ->
		return if @started
		@started = true
		seconds = new Date().getTime() / 1000
		@first_slide_transition = seconds
		@last_slide_transition = seconds
		paper = new Raphael(document.getElementById('clock_container'), 240, 240);
		paper.customAttributes.arc = arc
		slide_color = "#ff6f2b"
		compound_color = "#75ff2b"
		text_attr = {"font-size": 40}
		@outer = paper.path().attr({stroke: compound_color, "stroke-width": 20}).attr({arc: [0, 5*60, 100]})
		@inner = paper.path().attr({stroke: slide_color, "stroke-width": 20}).attr({arc: [0, 1*60, 75]})
		@slide_text = paper.text(120,100,"00:00").attr( fill: slide_color).attr(text_attr)
		@compound_text = paper.text(120,140,"00:00").attr( fill: compound_color).attr(text_attr)
		setInterval =>
			seconds = new Date().getTime() / 1000
			@outer.animate {arc: [seconds-@first_slide_transition+1, @compound_dur, 100]}, 300, "bounce"
			@inner.animate {arc: [seconds-@last_slide_transition+1, @slide_dur, 75]}, 300, "bounce"
			slide_delta = (@slide_dur+@last_slide_transition) - seconds
			@slide_text.attr text: @delta_to_s(slide_delta)
			compound_delta =(@compound_dur+@first_slide_transition) - seconds
			@compound_text.attr text: @delta_to_s(compound_delta)
		,1000

	constructor: ->
			Plugins.add (deck) =>
				if deck.master
					$("head").append($("<link rel='stylesheet' href='master.css' type='text/css' />"));
					deck.mainframe.parent().append("<div id='clock_container' class = 'clock'></div>")
					max_seen = 0
					deck.register_trigger "slide", (dir) =>
						if dir = "next" && deck.slide_index > max_seen
							max_seen = deck.slide_index
							@set_transition(deck.current_slide().expected_duration)
