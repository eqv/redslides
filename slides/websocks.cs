class SyncSock
	socket: null
	master: false

	on_msg: (evnt) ->
		json = JSON.parse(evnt.data)
		if !@master and json.type == "goto"
			@deck.jmp(json.slide,json.frame)
			@deck.update_urlanchor()
		if json.type == "session"
			url = document.location.protocol+"//"+document.location.pathname+"?master="+json.id
			$("body").prepend("<div class='sessionlink'><a href='#{url}'>#{url}</a> <a><div class='closebutton'>X</div></div>")
			$("body").find(".sessionlink").each (i,link) ->
				$(link).find(".closebutton").click =>
					$(link).remove()

	send_goto_slide: () ->
		@send( {"type":"goto",  "slide": @deck.slide_index,  "frame": @deck.slides[@deck.slide_index]._frame} )

	send: (json) ->
		@socket.send(JSON.stringify(json)) if @socket

	constructor: (@deck) ->
		socket = new WebSocket 'ws://localhost:8080'
		socket.onopen = =>
			@socket = socket
			console.log "connection estab."
			@send type: "session", "master": getParams()["master"]

		if getParams()["master"] == "new"
			@master = true
			Plugins.add (deck) =>
				deck.register_trigger "slide", (dir) =>
					@send_goto_slide()
				deck.register_trigger "frame", (dir) =>
					@send_goto_slide()
		socket.onmessage = (evnt) =>
			@on_msg(evnt) if @on_msg

