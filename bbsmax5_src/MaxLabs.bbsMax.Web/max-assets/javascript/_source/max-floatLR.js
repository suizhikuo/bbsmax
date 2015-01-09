var delta=0.15;
var collection;
var isClosed =false;
var closeB=false;
function floaters() {
	this.items = [];
	this.addItem = function(id,x,y,content)	{
		document.write('<div id=' + id + ' style="z-index: 10; position: absolute; width:' + (document.body.clientWidth - (typeof(x) == 'string' ? eval(x) : x)*2) + 'px; left:' + (typeof(x) == 'string' ? eval(x) : x) + ';top:' + (typeof(y) == 'string'? eval(y) : y) + '">' + content + '</div>');
		var newItem = {};
		newItem.object = $(id);
		newItem.x = x;
		newItem.y = y;
		this.items[this.items.length] = newItem;
	}
	this.play = function() {
		collection = this.items;
		setInterval('play()',30);
	}
}
function play() {
    if(isClosed) return;
	if(screen.width <= 800 || closeB) {
		for(var i = 0;i < collection.length;i++) {
			collection[i].object.style.display = 'none';
		}
		isClosed=true;
		return;
	}
	for(var i = 0;i < collection.length;i++) {
		var followObj = collection[i].object;
		var followObj_x = (typeof(collection[i].x) == 'string' ? eval(collection[i].x) : collection[i].x);
		var followObj_y = (typeof(collection[i].y) == 'string' ? eval(collection[i].y) : collection[i].y);
		if(followObj.offsetLeft != (document.documentElement.scrollLeft + followObj_x)) {
			var dx = (document.documentElement.scrollLeft + followObj_x - followObj.offsetLeft) * delta;
			dx = (dx > 0 ? 1 : -1) * Math.ceil(Math.abs(dx));
			followObj.style.left = (followObj.offsetLeft + dx) + 'px';
		}
		if(followObj.offsetTop != (document.documentElement.scrollTop + followObj_y)) {
			var dy = (document.documentElement.scrollTop + followObj_y - followObj.offsetTop) * delta;
			dy = (dy > 0 ? 1 : -1) * Math.ceil(Math.abs(dy));
			followObj.style.top = (followObj.offsetTop + dy) + 'px';

		}
		followObj.style.display = '';
	}
}
function closeBanner() {
	closeB = true;
	return;
}

var theFloaters = new floaters();