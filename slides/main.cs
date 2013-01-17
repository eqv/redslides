getParams = () ->
	strGET=document.location.search.substr(1,document.location.search.length);
	get_vars = {}
	if strGET!=''
			gArr=strGET.split('&');
			for param in gArr
				v='';
				vArr=param.split('=');
				v=vArr[1] if vArr.length>1
				get_vars[unescape(vArr[0])]=unescape(v);
	return get_vars


$(document).ready ->
	deck = new Deck()
	sync = new SyncSock(deck)
	ctr = new CountDownTimer()

	deck.mainframe=$("#mainframe")
	$("div.slide").each ->
		dom = $(@)
		slide = deck.add_slide(dom)
		slide.init()
		for name, handler of GenHandler
			$("[#{name}]",dom).each ->
				frame_obj = $(@)
				args = frame_obj.attr name
				evnt = new handler(args, frame_obj, slide)
	deck.init()

	$(document).keydown (e) ->
		if e.keyCode == 37 #left
			deck.prev()
			return false
		if e.keyCode == 39 #right
			deck.next()
			return false

	$("#mainframe").click ->
		deck.next()
