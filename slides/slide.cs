GenHandler = {}

class Slide
	_events: null
	_obj: null
	_frame: 1
	_num_frame_cache: null
	_transform: null
	deck: null
	expected_duration:60


	get_transform: ->
		" translate(#{@_transform.x}px, #{@_transform.y}px) scale(#{@_transform.scale}) rotate(#{@_transform.rot}deg)"

	get_camera_translation: ->
		" translate(#{-@_transform.x}px, #{-@_transform.y}px)"

	get_camera_rotation: ->
		"rotate(#{-@_transform.rot}deg) scale(#{1/@_transform.scale})"

	apply_transform: ->
		trans = @get_transform()
		@_obj.css("transform", trans )

	set_pos: (x,y) ->
		@_transform.x=x
		@_transform.y=y
		@apply_transform()
		return @

	set_rot: (r) ->
		@_transform.rot = r
		@apply_transform()
		return @

	set_scale: (s) ->
		@_transform.scale = s
		@apply_transform()
		return @

	init:() ->
		@trigger_events(1)
		return @

	goto_frame: (frame) ->
		@_frame=frame
		@trigger_events(@_frame)
		return @

	prev: () ->
		@_frame -= 1
		if @_frame < 1
			@_frame = 1
			return false 
		@trigger_events(@_frame)
		return true

	next: () ->
		@_frame += 1 
		if @_frame > @get_num_frames()
			@_frame = @get_num_frames()
			return false 
		@trigger_events(@_frame)
		return true

	add_event: (evnt) ->
		@_num_frames = Math.max @_num_frames, evnt.last_frame
		evnt.deck = @deck
		@event_list.push(evnt)
		return @

	trigger_events: (frame) ->
		for evnt in @event_list
			evnt.trigger(frame)
		return @


	get_num_frames: () ->
		return @_num_frames

	constructor: (domobj) ->
		@_obj = domobj
		@event_list= []
		@_num_frames = 0
		@_transform =
			x: 0
			y: 0
			scale: 1
			rot: 0
