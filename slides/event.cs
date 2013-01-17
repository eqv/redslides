class BinaryEvent

	is_active: false
	last_frame: 0
	deck:null

	init: ->

	trigger: (frame) ->
		if @is_active and !@active_at_frame(frame)
			@disable()
			@is_active=false
			return @
		if !@is_active and @active_at_frame(frame)
			@enable()
			@is_active=true
			return @

	active_at_frame: (frame) ->
		for range in @ranges
			return true if range(frame)
		return false

	parse_ranges: (range) ->
		for elem in range.split(",")
			upper = Number.POSITIVE_INFINITY
			lower = Number.NEGATIVE_INFINITY
			if /(\d+)-(\d+)/.exec(elem)
				lower = parseInt(RegExp.$1)
				upper = parseInt(RegExp.$2)
			else 
				if /-(\d+)/.exec(elem)
					upper = parseInt(RegExp.$1)
				else
					if /(\d+)-/.exec(elem)
						lower = parseInt(RegExp.$1)
					else
						if /(\d+)/.exec(elem)
							lower = parseInt(RegExp.$1)
							upper = lower
			@last_frame = Math.max @last_frame,lower if upper != Number.NEGATIVE_INFINITY
			@last_frame = Math.max @last_frame,upper if upper != Number.POSITIVE_INFINITY
			@ranges.push (x) ->
					lower <= x and x <= upper
		return @

	constructor: (ranges, @jnode, slide) ->
		@ranges = []
		@is_active = @default_enabled
		@parse_ranges(ranges)
		slide.add_event(@)
		@init()

register_event = (name, evnt) ->
	GenHandler["gen-#{name}"] = evnt

add_class = (jnode,str) ->
	jnode.attr("class",jnode.attr("class")+" "+str)

rem_class = (jnode,str) ->
	if jnode.attr("class")
		classes = str.split(" ")
		jnode.attr("class",jnode.attr("class").split(" ").filter (c) -> ! c in classes)

class Visibility extends BinaryEvent
	default_enabled: true
	enable: -> @jnode.animate opacity:1
	disable: -> @jnode.animate opacity:0
register_event("vis",Visibility)

class Uncover extends BinaryEvent
	default_enabled: true
	enable: -> @jnode.show()
	disable: -> @jnode.hide()
register_event("unc",Uncover)

class Highlight extends BinaryEvent
	default_enabled: false
	enable: -> add_class(@jnode,"hl_red")
	disable: -> rem_class(@jnode, "hl_red")
register_event("hl",Highlight)

class RadialMove extends BinaryEvent
	default_enabled: true
	timeout_id = null
	init: ->
		add_class(@jnode, "animated_move")

	clear_timeout: ->
		if @timeoutid != null
			clearTimeout(@timeoutid)
			@timeout_id = null

	enable: -> 
		@jnode.show()
		@clear_timeout()
		console.log("from: #{@jnode.position().top} #{@jnode.position().left}") #DO NOT REMOVE, SRSLY, TIMING BUG AHEAD
		@jnode.css("transform": "translate(0px,0px)")

	disable: -> 
		pos=@jnode.position()
		dir_x = pos.left-@deck.width/2
		dir_y = pos.top-@deck.height/2
		len = Math.sqrt(dir_x*dir_x+dir_y*dir_y)
		dir_x=(dir_x/len)*(@deck.width+@deck.height)
		dir_y=(dir_y/len)*(@deck.width+@deck.height)
		@clear_timeout()
		@timeoutid = setTimeout(=> 
				@jnode.hide() 
			,500)
		@jnode.css("transform": "translate(#{dir_x}px,#{dir_y}px)")
register_event("rad",RadialMove)
