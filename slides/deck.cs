class Deck
	slides: []
	slide_index: 0
	camera: null
	rotcam: null
	height: 600
	width: 800
	master: false
	mainframe: null
	config: Config


	register_plugins: () ->
		for plugin in Plugins.plugins
			plugin(@)

	register_trigger: (type,func) ->
		@callbacks[type] = [] unless type of @callbacks
		@callbacks[type].push func

	trigger_callbacks: (type, args...) ->
		if @callbacks[type]
			for callback in @callbacks[type]
				callback(args...)

	add_slide: (jnode) ->
		sld = new Slide(jnode)
		sld.deck = @
		sld.set_pos( (@width+10)*@slides.length, 0 )
		sld.expected_duration = @config.timings[@slides.length] if @config.timings[@slides.length]
		#sld.set_rot(30+30*@slides.length)
		#sld.set_scale(1/(@slides.length+1))
		@slides.push sld
		return sld

	current_slide: () ->
		return @slides[@slide_index]

	#use this function to trigger a transfer to another slide (will spawn events)
	jmp: (index, frame = null) ->
		@trigger_callbacks("slide","jmp")
		@trigger_callbacks("frame","jmp")
		@_goto_slide(index,frame)

	#internal function for moving to a slide/frame WITHOUT triggering events
	_goto_slide: (index, frame = null) ->
		@slide_index = index
		@camera.css( "transform", @slides[index].get_camera_translation() )
		@rotcam.css( "transform", @slides[index].get_camera_rotation() )
		@slides[index].goto_frame(frame) if frame

	update_urlanchor: ->
		document.location.hash = "#{@slide_index}.#{@slides[@slide_index]._frame}"
		@

	read_urlanchor: ->
		hash_anchor = document.location.hash.substring(1).split(".")
		if hash_anchor.length == 2
			slidenr = parseInt(hash_anchor[0])
			framenr =	parseInt(hash_anchor[1])
			if !isNaN(slidenr) and !isNaN(slidenr)
				@_goto_slide(slidenr,framenr)
				return true
		return false

	next: ->
		if @slides[@slide_index].next()
			@trigger_callbacks("frame","next")
			return false
		if @slide_index+1 < @slides.length
			@slide_index+=1 
		else 
			@trigger_callbacks("slide","end")
		@_goto_slide(@slide_index)
		@trigger_callbacks("slide","next")
		return true

	prev: ->
		if @slides[@slide_index].prev()
			@trigger_callbacks("frame","prev")
			return false
		if @slide_index > 0
			@slide_index-=1 
		else
			@trigger_callbacks("slides","start")
		@_goto_slide(@slide_index)
		@trigger_callbacks("slide","prev")
		return true

	init: ->
		@master = getParams()["master"]=="new"
		@register_plugins()
		for slide in @slides
			slide.init()
			@trigger_callbacks("init_slide",slide)
		@_goto_slide(0) unless @read_urlanchor()
		@trigger_callbacks("slide","init")
		@trigger_callbacks("frame","init")

	constructor: () ->
		@camera = $("#camera")
		@rotcam = $("#rotcam")
		@callbacks = {}
