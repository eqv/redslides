class Plugins
	@plugins = []
	@add = (plugin) ->
		@plugins.push plugin

#updates and shows/hides the page counter
Plugins.add (deck) =>
	if deck.config.plugins.slidecount
		deck.mainframe.append("<div class='footer-right' id='pagecounter'> -/- </div>")
		ctr = deck.mainframe.find("#pagecounter")
		deck.register_trigger "slide", =>
			if deck.slide_index > 0
				ctr.show().text("#{deck.slide_index}/#{deck.slides.length-1}")
			else
				ctr.hide()

#updates the url anchor for bookmarks
Plugins.add (deck) =>
	if deck.config.plugins.anchor
		deck.register_trigger "slide", =>
			deck.update_urlanchor()
		deck.register_trigger "frame", =>
			deck.update_urlanchor()

#speaker's notes presentation
Plugins.add (deck) =>
	if deck.master and deck.config.plugins.notes
		deck.mainframe.parent().append("<div id='notes_container' class = 'note_container'></div>")
		container=deck.mainframe.parent().find("#notes_container")
		deck.register_trigger "slide", =>
			container.html("")
			notes = deck.current_slide()._obj.find(".notes").hide().each (i,note) =>
				container.append($(note).html())


#dynamically loads master.css or slave.css depending on wether this slide is in master mode
Plugins.add (deck) ->
	if deck.config.plugins.mastercss
		if deck.master
			$("head").append($("<link rel='stylesheet' href='master.css' type='text/css' />"));
		else
			$("head").append($("<link rel='stylesheet' href='slave.css' type='text/css' />"));

#records the slide timings to be used in config.timings and prints them to console after pressing next on the last slide
Plugins.add (deck) =>
	if deck.config.plugins.recorder
		start_time = new Date().getTime() / 1000
		slide_durations={}
		deck.register_trigger "slide", (dir) =>
			console.log slide_durations
			now = new Date().getTime() / 1000
			if not slide_durations[deck.slide_index-1] and dir== "next"
				slide_durations[deck.slide_index-1] = now - start_time
				start_time = now
			if dir == "end"
				slide_durations[deck.slide_index] = now - start_time
				console.log slide_durations
