if (!window.root) root = "";
if(!window.maxdoc) window.maxdoc = document;
var keyEnum = { "enter": "13", "ctrl": "17", "space": "32", "backspace": "8", "shift": "16", "esc": "27" }
String.prototype.contains   = function(str) { return (this.indexOf(str) > -1);};
String.prototype.trim       = function(s){if(s) return this.trimEnd(s).trimStart(s);else return this.replace( /(^[ \t\n\r]*)|([ \t\n\r]*$)/g, '' ) ;};
String.prototype.trimEnd    = function(s){ if(this.endsWith(s)) { return this.substring(0,this.length-s.length);} return this;};
String.prototype.trimStart  = function(s){ if(this.startsWith(s)){ return this.slice(s.length);}return this;};
String.prototype.startsWith = function(str){return (this.indexOf(str) == 0);};
String.prototype.endsWith   = function(str){ return (str.length <= this.length && this.substr(this.length - str.length, str.length) == str); };
String.prototype.remove     = function(start, l){ var str1 = this.substring(0, start); var str2 = this.substring(start + l, this.length); return str1 + str2; }
String.prototype.insert     = function(index, str) { var str1 = this.substring(0, index); var str2 = this.substring(index, this.length); return str1 + str + str2; }
String.prototype.getHashCode= function() {var h = 31; var i = 0; var l = this.length; while (i < l) h ^= (h << 5) + (h >> 2) + this.charCodeAt(i++); return h;}
String.isNullOrEmpty        = function(str){ return str;};
String.format               = function() { var str = arguments[0]; for (var i = 1; i < arguments.length; i++) { var reg = new RegExp("\\{" + (i - 1) + "\\}", "ig"); str = str.replace(reg, arguments[i]); } return str; };
Array.prototype.contains    = function(val) {for (var i = 0; i < this.length; i++) {if (val == this[i])  return true;} return false;}

var stringBuilder = function(str) {
    this.arr = new Array(); this.length = 0;
    if (str) { this.arr.push(str); this.length += str.length; }
    if (!stringBuilder.created) {
        stringBuilder.prototype.append = function(str) {
            this.arr.push(str);
            this.length += str.length;
            return this;
        };
        stringBuilder.prototype.clear = function() {
            this.arr.splice(0, this.arr.length);
            this.length = 0;
            return this;
        };
        stringBuilder.created = true;
    }
}

var max = {
    browser: {
        isIE: navigator.userAgent.toLowerCase().contains('msie'),
        isIE5: navigator.userAgent.toLowerCase().contains('msie 5'),
        isIE6: navigator.userAgent.toLowerCase().contains('msie 6'),
        isIE7: navigator.userAgent.toLowerCase().contains('msie 7'),
        isGecko: navigator.userAgent.toLowerCase().contains('gecko'),
        isSafari: navigator.userAgent.toLowerCase().contains('safari'),
        isOpera: navigator.userAgent.toLowerCase().contains('opera')
    },
    global: {
        getClientWidth: function() { return ((maxdoc.documentElement && maxdoc.documentElement.clientWidth) || maxdoc.body.clientWidth); },
        getClientHeight: function() { return ((maxdoc.documentElement && maxdoc.documentElement.clientHeight) || maxdoc.body.clientHeight); },
        getScrollTop: function() { return ((maxdoc.documentElement && maxdoc.documentElement.scrollTop) || maxdoc.body.scrollTop); },
        getScrollLeft: function() { return ((maxdoc.documentElement && maxdoc.documentElement.scrollLeft) || maxdoc.body.scrollLeft); },
        getFullHeight: function() { if (maxdoc.documentElement.clientHeight > maxdoc.documentElement.scrollHeight) return maxdoc.documentElement.clientHeight; else return maxdoc.documentElement.scrollHeight; },
        getFullWidth: function() { return maxdoc.documentElement.scrollWidth; },
        getBrowserRect: function() { var r = new Object(); r.left = this.getScrollLeft(); r.top = this.getScrollTop(); r.width = this.getClientWidth(); r.height = this.getClientHeight(); r.bottom = r.top + r.height; r.right = r.left + r.width; return r; }
    },
    coor: {
        left: function(e, left) { if (typeof (left) == "number") { e.style.position = "absolute"; e.style.left = left + "px"; } else { var offset = e.offsetLeft; if (e.offsetParent != null) offset += Left(e.offsetParent); return offset; } },
        top: function(e, top) { if (typeof (top) == "number") { e.style.position = "absolute"; e.style.top = top + "px"; } else { var offset = e.offsetTop; if (e.offsetParent != null) offset += Top(e.offsetParent); return offset; } },
        width: function(e, w) { if (typeof (w) == "number") { e.style.width = w + "px"; } else { return e.offsetWidth; } },
        height: function(e, h) { if (typeof (h) == "number") { e.style.height = h + "px"; } else { return e.offsetHeight; } },
        getRect: function(e) { var r = new Object(); r.left = getLeft(e); r.top = getTop(e); r.width = getWidth(e); r.height = getHeight(e); r.bottom = r.top + r.height; r.right = r.left + r.width; return r; }
    },
    consts: {
        loading16: root + '/max-assets/images/loading_16.gif',
        loading32: root + '/max-assets/images/loading.gif'
    },
    eval: function(s) {
        if (!s) return;
        if (max.browser.isIE) {
            return execScript(s);
        } else {
            return window.eval(s);
        }
    }
};

//根据id得到指定的对象
function $(id) { return (typeof id == 'string' ? maxdoc.getElementById(id) : id);}

//根据name得到一组对象
function $$(name) { return (typeof name == 'string' ? maxdoc.getElementsByName(name) : null);}

//根据tagname得到一组对象
function $T(name) { return (typeof name == 'string' ? maxdoc.getElementsByTagName(name) : null);}

//设置对象的样式
//例如 : setStyle($('id'),{ display:'none',position : 'absolute',zIndex : 999 });
function setStyle(e, s) { for (var k in s) { e.style[k] = s[k]; } }

//增加节点的CSS Class
function addCssClass(e, c) {if (!e.className) { e.className = c; return; } if (e.className.indexOf(c) > -1) return;e.className += " "+c;}

//删除CSS Class
function removeCssClass(e, c) {
    var cn = e.className;
    if (!cn) return;
    var indx = cn.indexOf(c);
    if (indx == -1) return;
    if (cn.length == c.length) {
        cn = "";
    }
    else if (indx == 0) {
        cn = cn.remove(0, c.length).trim();
    }
    else if (indx > 0) {
        cn = cn.remove(indx, c.length).trim().replace("  ", " ");
    }
    e.className = cn;
}


//新建一个HTML节点
//tagName : 
//parent  : 父节点
function addElement(t,p){ var e= maxdoc.createElement(t); if(typeof(p)!='undefined') p.appendChild(e);else maxdoc.body.appendChild(e); return e;}

function getFileSize(s) {
    var u = ["B", "KB", "MB", "GB", "TB"];
    var uc = 0;
    while (s > 1024) { uc++; s = s / 1024; }
    s = parseFloat(s).toFixed(2);
    
    return  s + u[uc];
}


///移除节点
//elm: 节点
function removeElement(elm){var p; if(typeof(elm)=='string') elm = $(elm);   p=elm.parentNode; p.removeChild(elm);};

//设置/获取节点的属性
//value 未传的时候是获取属性
//element : 节点
//attrName: 属性名称
//[value] : 值（如果为空就返回属性值）
function attr(element, attrName,value){ if(value)element.setAttribute(attrName,value); else return element.getAttribute(attrName);}


//停止事件冒泡
var endEvent = function(ev) {
    if (max.browser.isIE) {
        if (event) {

            event.cancelBubble = true;
            event.returnValue = false;
        }
    } else {
        ev.preventDefault(); ev.stopPropagation();
    }
    return false;
}

//对象事件注册
//elm   :节点
//n     :事件名称比如click,dbclick,change,可以在前面带on也可以不带
//h     :事件处理函数
function addHandler(elm,n,h){if(n.indexOf('on',0)==0){  n=n.substring(2,n.length); }if(!max.browser.isIE){ elm.addEventListener(n,h,false); } else {  elm.attachEvent("on"+n,h); }}


//html编码 
function HTMLEncode(html) { var temp = maxdoc.createElement("div");  (temp.textContent != null) ? (temp.textContent = html) : (temp.innerText = html);   var output = temp.innerHTML;
    temp = null;
    return output;
}


//刷新页面
function refresh(){var href = location.href;if(href.indexOf('#',0)>-1){    href=href.substring(0,href.indexOf('#',0))}location.replace( href );}

//写COOKIE
function writeCookie(name, value, hours)
{
    var expire = "";
    if(hours)
    {
        expire = new Date((new Date()).getTime() + hours * 3600000);
        expire = "; expires=" + expire.toGMTString();
    }
    maxdoc.cookie = name + "=" + escape(value) + expire;
}

//读取cookie
function readCookie(name)
{
    var cookieValue = "";
    var search = name + "=";
    if(maxdoc.cookie.length > 0)
    {
        offset = maxdoc.cookie.indexOf(search);
        if (offset != -1)
        {
          offset += search.length;
          end = maxdoc.cookie.indexOf(";", offset);
          if (end == -1) end = maxdoc.cookie.length;
          cookieValue = unescape(maxdoc.cookie.substring(offset, end))
        }
    }
    return cookieValue;
}


//设置对象的可视
function setVisible(e, v) { e.style.display = v ? "" : "none";}

//设置对象的透明度
function opacity(e, o) { if (max.browser.isIE) { e.style.filter = "alpha(opacity=" + o + ")"; return; } setStyle(e, { opacity: o / 100, MozOpacity: (o / 100), KhtmlOpacity: (o / 100), filter: "alpha(opacity=" + o + ")" }); };

//移除事件注册
//e :节点
//n :事件名称比如click,change
//h :事件处理函数
function removeHandler(e,n,h){ if(n.indexOf('on',0)>=0){n=n.substring(2,n.length);}if(!max.browser.isIE){  e.removeEventListener(n,h,false);}else  {e.detachEvent("on"+n,h);}}

///判断一个变量或者函数是否已经定义
//variable : 
function isUndefined(v) {return typeof(v) == 'undefined' ? true : false;}

//获取元素的纵坐标
//顶部距离文档顶部的高度 
function getTop(e){ var offset=e.offsetTop;  if(e.offsetParent!=null) offset+=getTop(e.offsetParent);  return offset; } 

//获取元素的横坐标 
//左边距
function getLeft(e){ var offset=e.offsetLeft;  if(e.offsetParent!=null) offset+=getLeft(e.offsetParent);  return offset; }

//获取一个对象的宽度
function getWidth(e){  return e.offsetWidth;}
//获取一个对象的高度
function getHeight(e) { return e.offsetHeight; }

//取得矩形
function getRect(e) { var r = new Object(); r.left = getLeft(e); r.top = getTop(e); r.width = getWidth(e); r.height = getHeight(e); r.bottom = r.top + r.height; r.right = r.left + r.width; return r; }


//回车自动提交表单
//tid:触发回车的对象
//bn:按钮的name
function onEnterSubmit(tid, bn) { addHandler($(tid), "keydown", function(ev) { var e = ev || window.event; if (e && (e.keyCode == 13 || e.which == 13)) { var p = e.target || e.srcElement; while (p && p.nodeName.toLowerCase() != "form") { p = p.parentNode; } if (bn) { clickButton(bn, p.id); } else { p.submit(); } return false; } }); }


//ctrl+enter处理
//tid:触发回车的对象
//callback:回调
function onCtrlEnter(tid, callback) {
    var t = typeof (tid) != "object" ? $(tid) : tid;

    var form = t;
    while (form && form.nodeName.toLowerCase() != "form") {
        form = form.parentNode;
    }

    if (form) {
        addHandler(form, "mouseover", function() { form.focus = 1; });
        addHandler(form, "mouseout", function() { form.focus=0; });
        addHandler(form, "keydown", function(ev) { var e = ev || window.event; if (!form.focus) return; if (e && (e.keyCode == 13 || e.which == 13) && e.ctrlKey) callback(e); });
    }    
}

var attachQuery = function(url, key, value) {
    var reg = new RegExp("(\\?|&)" + key + "=.*?(&|$)", "ig");
    if (reg.test(url)) {
        url = url.replace(reg, "$1" + key + "=" + escape(value) + "$2");
    }
    else {
        var f = url.indexOf("?") > -1 ? "&" : "?";
        url += f + key + "=" + escape(value);
    }
    return url;
}


var moveToCenter = function(e) {
    var rc = getRect(e);
    var rb = max.global.getBrowserRect();
    setVisible(e, true);
    var p = e.style.position;
    var t = (rb.top + (rb.height - rc.height) / 2);
    var l = (rb.left + (rb.width - rc.width) / 2);
    if (p == "fixed") {
        t -= rb.top;
        l -= rb.left;
    }
    setStyle(e, { left: l + "px", top: t + "px" });
}

///设置一个对象可拖拽
var maxDragObject = function(obj, mover) {
    var ht = mover || obj;

    function mouseup(e) { e = e || window.event; if (obj.drag) { obj.drag = 0; if (max.browser.isIE) { ht.releaseCapture(); } else { window.releaseEvents(Event.MOUSEMOVE | Event.MOUSEUP); e.preventDefault(); } document.body.onselectstart = null; } }
    function mousemove(e) {
        if (!obj.drag) return;
        e = e || window.event;
        var l, t;
        l = e.clientX - obj._x;
        t = e.clientY - obj._y;
        setStyle(obj, { left: l + "px", top: t + "px" });
    }

    function mousedown(e) {
        e = e || window.event; if (max.browser.isIE) { ht.setCapture(); } else { window.captureEvents(Event.MOUSEMOVE | Event.MOUSEUP); e.preventDefault(); } var l = getLeft(obj), t = getTop(obj); obj._x = e.clientX - l; obj._y = e.clientY - t; obj.drag = 1;
        document.body.onselectstart = function() { return false; };
    }
    
    addHandler(ht, "mousedown", mousedown);
    
    if(!max.browser.isIE){
        ht=document.body;
        ht=document.body;
    }
    addHandler(ht, "mousemove", mousemove);
    addHandler(ht, "mouseup", mouseup);
}

///在指定位置弹出层
//pop：弹出的对象
//trigger：在哪个对象旁边弹出
//position：弹出位置，left,right,top,bottom也可以组合。默认为自动
//offsetLeft：左边偏移量修正叠加值
//offsetTop：顶部偏移量修正叠加值  （修正在可滚动的容器内部出现定位错误的问题）
var showPopup = function(pop, target, position, offsetLeft, offsetTop) {
    if (!position) position = "auto";
    var s = pop.style;
    s.display = '';
    var t, l;
    rt = max.coor.getRect(target)
    rl = max.coor.getRect(pop);
    rw = max.global.getBrowserRect();


    if (rt.bottom + rl.height < rw.bottom)
        t = rt.bottom;
    else if (rw.bottom - rt.bottom > rt.top - rw.top)
        t = rt.bottom;
    else
        t = rt.top - rl.height;


    if (rt.left + rl.width > rw.right)
        l = rt.right - rl.width;
    else
        l = rt.left;

    if (position.indexOf("left") > -1) l = rt.left;
    else if (position.indexOf("right") > -1) l = rt.right - rl.width;
    else if (position.indexOf("center") > -1) l = (rt.left + rt.width / 2) - rl.width / 2;
    if (position.indexOf("top") > -1) { t = rt.top - rl.height; }
    else if (position.indexOf("bottom") > -1) t = rt.bottom;

    if(target.style.position=="fixed")
    {
        l+=rw.left;
        t+=rw.top;
    }
    if (offsetLeft) l += offsetLeft;
    if (offsetTop) t += offsetTop;
    s.left = l + "px";
    s.top = t + "px";
}

//时钟
var timer = function(i, ontick) {
    this.counter = 0;
    this.interval = i;
    this.ontick = ontick;
    this.handler = null;
}
timer.prototype.start = function() {
    //window._count=0;
    var _ts = this
    this.handler = window.setInterval(function() { _ts.ontick(++_ts.counter); }, this.interval);
    //var _this = this; if (this.interval > 0) { if (this.ontick != null) this.ontick(++this.counter); window.setTimeout(function() { _this.start() }, _this.interval); }

}
timer.prototype.stop = function() {
    window.clearInterval(this.handler);
    this.interval = 0;
}

///复选框列表对象全选，反选
///checkName:复选框name属性
///sallid:全选的那个复选框id属性
///   pid:复选框列表的容器id， 可以为空
var checkboxList = function(checkName, sallid, pid) {
    var chkName = checkName;
    var selectAllNode;
    var elements;
    var onItemChange;
    this.onSelectAllCallItemChange = true;
    if (chkName instanceof Array) {
        elements = new Array();
        var tempArray
        for (var i = 0; i < chkName.length; i++) {
            tempArray = $$(chkName[i]);
            for (var j = 0; j < tempArray.length; j++)
                elements.push(tempArray[j]);
        }
    }
    else {
        var elements = $$(checkName);
    }

    //if (elements==null||elements.length==0) return null;
    if (sallid) {
        selectAllNode = $(sallid);
        if (selectAllNode != null) addHandler(selectAllNode, "click", selectAll);
    }

    for (var i = 0; i < elements.length; i++) addHandler(elements[i], "click", function(e) {
        var src;
        selectAllNode && checkSelectAllBox();
        if (max.browser.isIE)
            src = window.event.srcElement;
        else
            src = e.target;
        callItemChange(src);
    });



    mutilSelect(pid);
    function checkSelectAllBox(e) {

        if (selectAllNode) {
            if (!elements.length)
                return;
            var isSelectAll = true;
            for (var i = 0; i < elements.length; i++) {
                if (elements[i].checked != true) {
                    isSelectAll = false;
                    break;
                }
            }
            selectAllNode.checked = isSelectAll;
        }
    };
    ///选择所有复选框
    function selectAll() {
        for (var i = 0; i < elements.length; i++) {
            elements[i].checked = selectAllNode.checked;
            if (this.onSelectAllCallItemChange) callItemChange(elements[i]);
        }
    };

    function callItemChange(sender) {
        if (onItemChange) onItemChange(sender);
    }

    ///按下Shift多选
    ///pid :复选框的容器ID
    function mutilSelect(pid) {
        var startNodeIndex;
        var lastNodeIndex;
        var pressShift = false;
        var parentNode;

        if (!pid)
            parentNode = maxdoc.documentElement;
        else
            parentNode = $(pid);

        addHandler(parentNode, "click", mSelect);
        addHandler(parentNode, "keydown", keydown);
        addHandler(parentNode, "keyup", keyup);
        function keydown(e) {
            var keynum;
            if (max.browser.isIE)
                keynum = window.event.keyCode;
            else
                keynum = e.which;
            pressShift = keynum == keyEnum.shift;
        }
        function keyup(e) { pressShift = false; }
        function mSelect() {
            if (pressShift == true) {
                startNodeIndex = -1;
                for (var i = 0; i < elements.length; i++) {
                    if (elements[i].checked == true) {
                        if (startNodeIndex == -1) startNodeIndex = i;
                        lastNodeIndex = i;
                    }
                }
                if (startNodeIndex > -1) {
                    for (var i = startNodeIndex; i <= lastNodeIndex; i++) {
                        elements[i].checked = true;
                        callItemChange(elements[i]);
                    }
                }
                checkSelectAllBox();
            }
        }
    };
    return {
        //返回已经选中的列表(array)
        selectedList: function() {
            var selectArray;
            var j = 0;
            selectArray = new Array();
            for (var i = 0; i < elements.length; i++) {
                if (elements[i].checked) {
                    selectArray[j] = elements[i].value;
                    j++;
                }
            }
            return selectArray;
        },
        //返回已经选中的个数(number)
        selectCount: function() {
            var j = 0;
            for (var i = 0; i < elements.length; i++) {
                if (elements[i].checked) j++;
            }
            return j;
        },
        reverse: function() {
            for (var i = 0; i < elements.length; i++) {
                elements[i].checked = !elements[i].checked;
                callItemChange(elements[i]);
            }
            checkSelectAllBox();
        },
        selectAll: function() {
            for (var i = 0; i < elements.length; i++) {
                elements[i].checked = true;
                callItemChange(elements[i]);
            }
            checkSelectAllBox();
        },
        SetItemChangeHandler: function(handler) {
            onItemChange = handler;
        }
    };
}

///查找指定类型的节点和id包含有文字 id的节点
function findElement(tagName,id)
{
    var elmArray,findArray;
    elmArray=$T(tagName);
    findArray=new Array();
    for( var i=0;i<elmArray.length;i++)
        if(elmArray[i].id && elmArray[i].id.indexOf(id,0)>=0)
            findArray.push(elmArray[i]);
    return findArray;
}

//等比缩小图片
//e：img节点
//w：宽度限制
//h：高度限制
function imageScale(e, w, h) {
    var width = e.width; var height = e.height; var scale = width / height; if (w && width > w) { e.width = w; e.height = w / scale; } if (h && height > h) { e.height = h; e.width = h * scale; }
    if (w && h) {
        if (e.width > w)
            imageScale(e, w);
        else if(e.height>h)
            imageScale(e,0,h);
    }
}

function hasQuery(url) {
    if (url.contains('/?')) {
        var index = url.toString().indexOf('/?');
        return (url.toString().indexOf('?', index + 2) != -1);
    }
    var lowUrl = url.toLowerCase();
    if (lowUrl.contains('/default.aspx?')) {
        var index = lowUrl.toString().indexOf('/default.aspx?');
        return (url.toString().indexOf('?', index + 14) != -1);
    }
    else if (lowUrl.contains('/index.aspx?')) {
        var index = lowUrl.toString().indexOf('/index.aspx?');
        return (url.toString().indexOf('?', index + 12) != -1);
    }
    else {
        return url.contains('?');
    }
}

function ctrlEnterEvent(callback, controlid1, controlid2) {

    for (var i = 1; i < arguments.length; i++) {
        var c = maxdoc.getElementById(arguments[i]);
        if (c) c.onkeydown = function(e) {
            e = e || window.event;
            if (e.ctrlKey && e.keyCode == 13) {
                callback && callback();
            }
        };
    }
}

//执行HTML内包含的JS
var execInnerJavascript = function(html) {
    var regJs = /<script[^>]*?>((?:.|\n|\r)*?)<\/script>/ig,
 regJs0 = /<!--\{js(?:\s|\n)((?:.|\n|\r)+?)\}-->/ig; //注释方式写的JS
    var m;
    while (m = regJs.exec(html)) max.eval(m[1]);
    while (m = regJs0.exec(html))
        max.eval(m[1]);
}

//十进制  转十六进制
function to16(num) {

    function _to16(num) { //十六进制单位转换
        switch (num) {
            case 0: return "0"
            case 1: return "1"
            case 2: return "2"
            case 3: return "3"
            case 4: return "4"
            case 5: return "5"
            case 6: return "6"
            case 7: return "7"
            case 8: return "8"
            case 9: return "9"
            case 10: return "A"
            case 11: return "B"
            case 12: return "C"
            case 13: return "D"
            case 14: return "E"
            case 15: return "F"
        }
    }

    num = num;
    if (!isNaN(num)) {
        var result = "";
        while (num >= 16) {  //循环求余后反向连接字符串
            result = _to16(num % 16) + result;
            num = parseInt(num / 16);
        }
        result = _to16(num % 16) + result;
        return result;
    }
    else
        return 0x0;
}

//----Begin MaxAjaxEngine----
var proxyUrl="/httpproxy.aspx"

ajaxWorker = function(url, method, content) {
    this.r = null;
    this.url = proxyUrl +"?url=" + encodeURI( url);
    this.method = method;
    this.content = content;
    this.header = {};
    this.header["Connection"] = "close";
    this.header["Content-type"] = "application/x-www-form-urlencoded";
    this.header["If-Modified-Since"] = "0";
    var self = this;
    if (window.XMLHttpRequest) {
        this.r = new XMLHttpRequest();
    } else if (window.ActiveXObject) {
        try {
            this.r = new ActiveXObject("Msxml2.XMLHTTP");
        } catch (e) {
            try {
                this.r = new ActiveXObject("Microsoft.XMLHTTP");
            } catch (e) {
            }
        }
    }

    this.addListener = function(http_status, func) {
        if (!this.L)
            this.L = [];
        this.L[http_status] = func;
        return this;
    };

    this.setHeader = function(name, value) {
        this.header[name] = value;
        this.r.setRequestHeader(name, value);
        return this;
    };

    this.send = function(newurl) {
        if (this.method != "post" && this.method != "get")
            this.method = "get";

        this.r.open(this.method, newurl ? newurl : this.url, true);

        for (var h in this.header) {
            this.r.setRequestHeader(h, this.header[h]);
        }

        this.r.send(this.content);
    };
    if (this.r) this.r.onreadystatechange = function() {
        if (self.r.readyState == 4 && self.L[self.r.status] != null) {
            self.L[self.r.status](self.r.responseText);
        }
    };
};

getFormData = function(f, buttonName) {
    var list = {};

    for (var i = 0, len = f.elements.length; i < len; i++) {
        var item = f.elements[i];
        var name = item.name;
        if (item.disabled) continue;
        if (!list[name]) list[name] = [];
        switch (item.tagName.toLowerCase()) {
            case 'input':
                switch (item.type) {
                    case 'text':
                    case 'hidden':
                    case 'password':
                        list[name].push(item.value);
                    case 'radio':
                    case 'checkbox':
                        if (item.checked) list[name].push(item.value);
                        break;
                }
                break;
            case 'textarea':
                list[name].push(item.value);
                break;
            case 'select':
                list[name].push(item.options[item.selectedIndex].value);
                break;
        }
    }
    
    if (buttonName) {
        if (!list[buttonName])
            list[buttonName] = [];

        list[buttonName].push('');
    }
    
    var query = '';
    for (var item in list) {
        for (var i = 0, len = list[item].length; i < len; i++) {
            //if (!item || !list[item][i]) continue;
            if (query != '') query += '&';
            query += item + '=' + encodeURIComponent(list[item][i]);
        }
    }
    return query;
};

///提交数据到指定的地址，datas是url编码过的数据，callback回调，会把HTML内容传给callback函数
ajaxPostData = function(url, datas, callback) {
    new ajaxWorker(url, 'post', datas).addListener(200, function(as) {
        if (callback)//如果不自动出来请求结果， 那么就把结果交由外面的函数处理
        {
            callback(as);
        }
        else {
            return;
        }
    }).send();
}

///根据指定的url和post数据发起ajax请求，并调用callback,把结果HTML处给callback
function ajaxRequest(url, callback) {
    var aw = new ajaxWorker(url, "GET");
    aw.addListener(200, function(r) { if (callback) callback(r); });
    aw.send();
}

ajaxPostForm = function(formID, url, buttonName, callback) {
    var _f = $(formID);

    url = url || _f.action || location.href;

    if (!_f) return true;

    var content = getFormData(_f, buttonName);

    ajaxPostData(url, content, callback);
}

ajaxSubmit = function(formID, buttonName, ids, onSucceed, onError, autoReplace) {
    var _f = $(formID);
    var url = _f.action || location.href;

    if (!ids || ids == '')
        ids = '*';

    if (ids != null) {
        if (hasQuery(url))
            url += '&_max_ajaxids_=' + ids ;//+ '&_random_query_id_=' + new Date().getMilliseconds(); //此处去除请求随机码。
        else
            url += '?_max_ajaxids_=' + ids; //+'&_random_query_id_=' + new Date().getMilliseconds();
    }

    if (!_f) return true;
    var content = getFormData(_f, buttonName);
    new ajaxWorker(url, 'post', content).addListener(200, function(as) {
        //=======所有页面上的弹出菜单全部隐藏初始化=======================
        var il = popupCollection.innerList;
        if (il) {
            for (var i = 0; i < il.length; i++)
                il[i].hide();
        }

        //==============================        
        if (!autoReplace) {
            if (onSucceed)//如果不自动出来请求结果， 那么就把结果交由外面的函数处理
            {
                onSucceed(as);
            }
            else {
                return;
            }
        }

        var r = {};
        var n = 0;
        var _c = 0;

        while (true) {
            var i = as.indexOf("|", _c); if (i <= _c) break;
            var id = as.substring(_c, i); _c = i + 1;
            var s = as.indexOf("|", _c); if (s <= _c) break;
            var size = parseInt(as.substring(_c, s));
            var html = as.substr(s + 1, size); _c = s + size + 2;
            r[id] = html; if (_c >= as.length) break;
        }

        var ar = null;
        var jsExcuted = false;
        for (var id in r) {
            if (id == '_max_ajaxresult_') {
                ar = eval("(" + r[id] + ")");
                continue;
            }
            var panel = maxdoc.getElementById(id);

            if (panel.onUpdate)
                panel.onUpdate(panel);

            var html = r[id];
            panel.innerHTML = html;
            execInnerJavascript(html);
            jsExcuted = true;
        }
        if (onSucceed != null) {
            onSucceed(ar);
        }
    }).send();
    return false;
};


//import MaxEngine.AjaxObj;
ajaxRender = function(url, ids, onSucceed, onError) {
    if (!url || url == '')
        url = location.href;

    if (!ids || ids == '')
        ids = '*';
    var temp = "";
    if (url.indexOf("#") > -1) {
        var f = url.indexOf("#");
        temp = url.substr(f, url.length - f);
        url = url.substr(0, f);
    }

    if (hasQuery(url))
        url += '&_max_ajaxids_=' + ids + '&_random_query_id_=' + new Date().getMilliseconds();
    else
        url += '?_max_ajaxids_=' + ids + '&_random_query_id_=' + new Date().getMilliseconds();
    url += temp;
    new ajaxWorker(url).addListener(200, function(as) {

        var r = {};
        var n = 0;
        var c = 0;

        while (true) {
            var i = as.indexOf("|", c); if (i <= c) break;
            var id = as.substring(c, i); c = i + 1;
            var s = as.indexOf("|", c); if (s <= c) break;
            var size = parseInt(as.substring(c, s));
            var html = as.substr(s + 1, size); c = s + size + 2;
            r[id] = html; if (c >= as.length) break;
        }
        var ar = null;

        for (var id in r) {
            if (id == '_max_ajaxresult_') {
                ar = eval("(" + r[id] + ")");
                continue;
            }
            var panel = $(id);

            var html = r[id];
            if (panel) {
                if (panel.onUpdate) panel.onUpdate(panel);
                panel.innerHTML = html;
            }
            execInnerJavascript(html);
        }

        if (onSucceed != null) {
            onSucceed(ar);
        }
    }).send();

    return false;
};


//----End MaxEngine----

/******************************* 弹出菜单容器 *********************************/
var popupCollection=function(){var c=popupCollection;if(!c.innerList)c.innerList=new Array();}
popupCollection.prototype.add=function(obj){obj.onShow=this.onshow;popupCollection.innerList.push(obj);} 
popupCollection.prototype.onshow=function(obj){var cc=popupCollection;for(var i=0;i<cc.innerList.length;i++){if(cc.innerList[i]!=obj){cc.innerList[i].hide();}}}
popupCollection.instance = function(){if(!window.dropCollection)window.dropCollection=new popupCollection();return window.dropCollection;}

//**弹出菜单基类
var popupBase = function(listid, trigger, autopopup) {

    this.triggers = new Array();

    this.list = typeof (listid) == "object" ? listid : $(listid); 
    this.auto = autopopup;

    if (trigger instanceof Array) {
        for (var i = 0; i < trigger.length; i++) {
            this.triggers.push($(trigger[i]));
        }
    }
    else if (trigger instanceof Object)
        this.triggers.push(trigger);
    else
        this.triggers.push($(trigger));

}
popupBase.prototype.show=null;
popupBase.prototype.hide=null;
popupBase.prototype.onShow=null;
popupBase.prototype.onHide=null;

/******************************* 下拉菜单 *********************************/
//下拉菜单
//listid:下拉菜单的那个容器ID
//triggerid:触发下拉菜单弹出的那个对象的ID
//autoPop:如果是true那么只要鼠标滑过就弹出， 否则需要点击才弹出
//position:位置目前只支持'auto'和'center'
var popup = function(listid, trigger, autoPop, onSelectClass, pos) {
    this.focus = false;
    popupBase.call(this, listid, trigger, autoPop);

    this.triggerStyle = onSelectClass;
    if (pos)
        this.position = pos;
    else
        this.position = 'auto';

    var _this = this;
    var s = this.list.style;
    s.position = 'absolute';
    s.zIndex = 50;

    for (var i = 0; i < this.triggers.length; i++) {
        var t = this.triggers[i];
        addHandler(t, this.auto ? 'mouseover' : 'click', function(e) { _this.show(e); });
        if (this.auto)
            addHandler(t, "mouseout", function() { _this.hideList(); });
        else {
            addHandler(t, "mouseout", function(e) { _this.focus = false; });
            addHandler(t, "mouseover", function(e) { _this.focus = true; });
        }
    }

    if (this.auto) {
        addHandler(this.list, 'mouseover', function(e) { _this.focus = true; });
        addHandler(this.list, "mouseout", function() { _this.hideList(); });
    }
    else {
        addHandler(this.list, "mouseout", function(e) { _this.focus = false; });
        addHandler(this.list, "mouseover", function(e) { _this.focus = true; });
        addHandler(maxdoc.documentElement, "click", function(e) { if (_this.focus == false) _this.hide(e); })
    }

    popupCollection.instance().add(this);
}
popup.prototype.hideList = function() {
    var ts = this;
    setTimeout(function() { if (ts.focus == false) ts.hide(); }, 1000);
    ts.focus = false;
}

popup.prototype.show = function(e) {

    var target;
    e = e || window.event;
    if (!e)
        target = this.triggers[0];
    else
        target = e.target ? e.target : e.srcElement;

    var flag = false;
    while (target && this.triggers.length) {
        for (var j = 0; j < this.triggers.length; j++) {
            var _tr = this.triggers[j];
            if (target.id)
                if (_tr == target || _tr.id == target.id) {
                target = _tr;
                flag = true;
                break;
            }
        }

        if (flag) break; // true; break;

        target = target.parentNode;
    }

    if (this.triggers.length && !flag) return;

    showPopup( this.list,target, this.position);

    this.focus = true;
    var cn = this.triggerStyle;

    if (cn) {
        addCssClass(target, cn);
        this.cssNode = target;
    }
    if (this.onShow) this.onShow.call(popupCollection.instance(), this);

}

popup.prototype.hide = function(e) {
    var s = this.list.style;
    s.display = 'none';
    var cn = this.triggerStyle;
    if (cn) {
        if (this.cssNode) {
            removeCssClass(this.cssNode, cn);
        }
    }
    if (this.onHide) this.onHide.call(popupCollection.instance(), this);
}


//====================================================================================================

//浏览器屏蔽层
var background = function () {
    var autoSize = function () {
        var g = max.global;
        if (e) { e.style.height = g.getFullHeight() + "px"; e.style.width = g.getFullWidth() + "px"; }
    }

    var e, selects = false;
    e = addElement("div");
    setStyle(e, { backgroundColor: 'black', position: 'absolute', top: '0px', left: '0px', zIndex: 50 });

    opacity(e, 30);
    addHandler(window, "resize", autoSize);
    if (max.browser.isIE6) {
        var temp;
        temp = $T("select");
        if (temp.length > 0) {
            selects = new Array(); var ti;
            for (var i = 0; i < temp.length; i++) {
                ti = temp[i];
                if (ti.style.display != "none" && ti.style.visibility != "hidden") {
                    selects.push(ti);
                    ti.style.visibility = "hidden";
                }
            }
        }
        temp = null;
    }

    autoSize();
    return {
        destroy: function () {
            removeHandler(document, "resize", autoSize);
            if (selects) {
                for (var i = 0; i < selects.length; i++) {
                    selects[i].style.visibility = '';
                }
                selects = null;
            }
            removeElement(e);
            e = null;
        }
    }
}

/*----------------------------------------------------------*/

function clickButton(name, formid) {
    var hidden;
    var form = formid ? $(formid) : document.forms[0];
    hidden = addElement("input", form);
    hidden.style.display = 'none';
    hidden.name = name;
    hidden.value = "1";
    form.submit();
}

///公告滚动
function announcement(id) {
    var ann_obj = $(id);
    var ann_index = 0;
    var ann_ognHTML = ann_obj.innerHTML;
    var ann_itemHeight = 0;  //ann_obj.parentNode.offsetHeight;
    var ann_li = ann_obj.getElementsByTagName("li");
    var ann_lcount = ann_li.length;
    var ann_run = 1;
    function ann_scroll() {
        var s = ann_obj.scrollTop;
        if (s % ann_itemHeight || ann_run) { ann_obj.scrollTop++; ann_obj.scrollTop %= ann_obj.scrollHeight >> 1; }
        var st = ann_obj.scrollTop % ann_itemHeight ? 10 : 2000;
        setTimeout(function () { ann_scroll(); }, st);
    }

    ann_obj.onmouseover = function () { ann_run = 0; };
    ann_obj.onmouseout = function () { ann_run = 1; };

    addHandler(window, "load", function () {
        ann_itemHeight += ann_li[0].offsetHeight;
        setStyle(ann_obj, { height: ann_itemHeight + "px", overflow: "hidden" });
        ann_obj.innerHTML += ann_ognHTML;
        ann_scroll();
    });
}

//页面元素 关联Radio显示与隐藏
function initDisplay(formName, args) {
    var v = '';
    _display = function (f, _args) {
        var cs = document.getElementsByName(f);
        for (var i = 0; i < cs.length; i++) {
            var c = cs[i];
            if (c.nodeName == 'SELECT') {
                v = c.value;
                c.onchange = function () { _display(f, _args) };
            }
            else if (c.nodeName == 'INPUT' && c.getAttribute('type') == 'radio') {
                if (c.checked) v = c.value;
                c.onclick = function () { _display(f, _args) };
            }
        }
        v = v.toLowerCase();
        for (var i = 0; i < _args.length; i++) {
            var a = _args[i];
            if (a.value.toLowerCase() == v) {
                if (typeof a.id == 'string') {
                    $(a.id).style.display = a.display ? '' : 'none';
                }
                else {
                    for (var j = 0; j < a.id.length; j++) {
                        $(a.id[j]).style.display = a.display ? '' : 'none';
                    }
                }
            }
        }
        //************以下几行代码是兼容IE6，等IE6死绝了就可以去掉这些代码
        //IE6下select 跑到浮动层上，用IFRAME挡在后面以解决这个问题， 但这个IFRAME没有随着对话框改变大小
        //因此需要这个恶心的做法来解决这个问， 我觉得相当无奈， 想骂人。
        //去你妈的去你妈的
        if (max.browser.isIE6) {
            if (pageLoadComplete) {
                for (var k in maxPanelManager) {
                    var p = maxPanelManager[k];
                    if (p && p.panel.frame) {
                        var frm = p.panel.frame;
                        var rect = getRect(p.panel);
                        setStyle(frm, { left: (rect.left + 5) + "px", top: (rect.top + 5) + "px", width: (rect.width - 10) + "px", height: (rect.height - 10) + "px" });
                    }
                }
            }
        }
        //*****************************************************************
    };
    if (!pageLoadComplete) {
        addPageEndEvent(function () { _display(formName, args) });
    }
    else {
        _display(formName, args);
    }
}

function showAlert(message) {
    alert(message);
}

function showSuccess(message) {
    alert(message);
}


ajaxCallback = function (result) {
    if (result != null) {
        if (result.iswarning)
            showAlert(result.message);
        else if (result.issuccess)
            showSuccess(result.message);
    }
}

ajaxCallbackLocationError = function (result) {
    if (result != null) {
        if (result.iswarning)
            showAlert(result.message);
        else if (result.issuccess)
            showSuccess(result.message);
    }
    else {
        location.href = '#errormsg';
    }
}

function maxConfirm(title, message, func, callback) {
    if (confirm(message)) {
        func();
        return true;
    }
    else {
        return false;
    }
}

function textCounter(target, display, maxcount) {
    var a = maxcount - target.value.length;
    if (a < 0) {
        target.value = target.value.substr(0, maxcount);
        a = 0;
    }
    document.getElementById(display).innerHTML = a;
}


//打开指定的对话框，如果打开成功，返回false，否则返回true。
//这样做的目的是：方便进行nojavascript兼容
//例如：<a href="对话框页.html" onclick="return openDialog()">打开对话框</a>
//这样写就可以让脚本正常的情况下打开对话框，脚本不正常的情况下以普通链接的形式打开
function openDialog() {

    var args = arguments;
    if (args.length < 1) return true;

    var settings;

    if (typeof args[0] == 'string') {

        settings = { src: args[0] };

        if (args.length == 2) {
            if (typeof args[1] == 'function')
                settings.return_handler = args[1];
            else if (typeof args[1] == 'object')
                settings.trigger = args[1];
        }
        else if (args.length == 3) {
            if (typeof args[1] == 'function') {
                settings.return_handler = args[1];
                settings.trigger = args[2];
            }
            else {
                settings.return_handler = args[2];
                settings.trigger = args[1];
            }
        }
    }
    else
        settings = args[0];

    var url = settings.src;

    if (url.indexOf("#") > -1) {
        var f = url.indexOf("#");

        temp = url.substr(f, url.length - f);
        url = url.substr(0, f);
    }

    var k = new ajaxPanel(url, settings.trigger, settings.return_handler);
    k.waiting();
    k.relocation();
    k.loadPage();
    window.setTimeout(function () { k.focus(); }, 20);
    //if (window.maxPopupManager) maxPopupManager.hideAll(); //页面上的所有弹出菜单隐藏
    return false;
}


//提交数据到指定对话框
//url      : 对话框地址
//formid   : 表单的ID
//callback : 关闭对话框后的回调函数
function postToDialog(param) {

    var formid, url, callback, form;
    formid = param.formId;
    url = param.url;
    callback = param.callback;

    if (formid)
        form = $(formid);
    else {
        form = document.forms[0];
    }

    if (!form.id)
        form.id = "bbsmax_default_form";
    if (url.startsWith('?'))
        url = 'default.aspx' + url;
    else if (url.toLowerCase().startsWith(root.toLowerCase() + '/?'))
        url = root + '/default.aspx' + url.substr(root.length + 1);

    if (typeof (url) == "undefined") url = form.action;

    var datas = getFormData($(formid), null);
    var k = new ajaxPanel(url, null, callback);
    k.waiting();
    k.relocation();
    k.postToPage(null, datas);
    k.focus();
    // if (window.maxPopupManager) maxPopupManager.hideAll(); //页面上的所有弹出菜单隐藏
    return false;
}

//动态载入指定的脚本，并在载入完成后调用callback
function loadScript(src, charset, callback) {
    var h = $T('head')[0];
    var ss = h.getElementsByTagName('script');
    if (ss && ss.length > 0) {
        for (var i = 0; i < ss.length; i++) {
            if (ss[i]._src && ss[i]._src == src) {
                callback && callback();
                return;
            }
        }
    }

    var s = document.createElement("script");
    s.type = 'text/javascript';
    s._src = src;
    s.src = src;
    if (charset) {
        s.charset = charset;
    }

    if (callback) {
        if (max.browser.isIE) {
            s.onreadystatechange = function () {
                if ('complete' == s.readyState || s.readyState == "loaded") {
                    callback && callback();
                }
            };
        }
        else s.onload = function () {
            callback && callback();
        };
    }

    h.appendChild(s);
}

//弹出层管理器
var maxPanelManager = {};

var maxPanelCore = function () { }
maxPanelCore.prototype.resize = function (w, h) {
    this.width = w;
    this.height = h;
    setStyle(this.panel, { width: w + "px", height: h + "px" });
}

maxPanelCore.prototype.relocation = function () {
    //重新调整坐标
    if (this.trigger) {
        showPopup(this.panel, this.trigger, this.position, this.offsetLeft, this.offsetTop);
    }
    else {
        moveToCenter(this.panel);
    }
    if (max.browser.isIE6) {
        if (this.panel.frame) {
            var f = this.panel.frame;
            var rect = getRect(this.panel);
            setStyle(f, { left: (rect.left + 5) + "px", top: (rect.top + 5) + "px", width: (rect.width - 10) + "px", height: (rect.height - 10) + "px" });
        }
    }
}

maxPanelCore.prototype.submit = function (btn) {
    var data = getFormData(this.forms[0], btn);
    this.postToPage(null, data);
}

maxPanelCore.prototype.setContent = function (s) {
    this.forms = [];
    window.currentPanel = this;
    window.dialog = this; //兼容以前的代码
    var panel = this.panel;
    var index = s.indexOf("<body>");
    if (index == -1) index = s.indexOf("<body ");
    if (index > -1) {
        s = s.substr(index, s.length - index);
        index = s.indexOf(">") + 1;
        s = s.substr(index, s.length - index);
    }
    index = s.indexOf("</body>");
    if (index > -1) {
        s = s.substr(0, index);
    }
    var regScriptSrc = /<script\s[^>]*?src="([^"]+?)"[^>]*?>\s*<\/script>/ig;
    var ss;
    while (ss = regScriptSrc.exec(s)) loadScript(ss[1]); //动态载入包含的JS

    panel.body.innerHTML = s; //加载HTML

    ///表单的AJAX提交
    var tags = ["input", "button"], findex = 0;
    var fid = new Date().getMilliseconds().toString();
    var btnIndex = 0;
    for (var tg = 0; tg < tags.length; tg++) {
        var submits = panel.getElementsByTagName(tags[tg]);
        if (submits && submits.length) {
            for (var i = 0; i < submits.length; i++) {
                var input = submits[i];

                if (input.type != "submit") continue;

                if (findex == 0) {
                    var f = input.parentNode;
                    var _s = 0;
                    do {
                        _s++;
                        if (s > 20) { f = null; break; }
                        if (f.tagName.toLowerCase() == "form") { break; }
                    } while (f = f.parentNode);

                    if (f == null) continue;
                    fid = panel.id + "_form_" + findex;
                    f.id = fid;
                    this.forms.push(f);
                    findex++;
                }

                if (!input.id) input.id = String.format("max_submit_{0}", i); //由于事件源每个浏览器居然不一样， 只好把SUBMIT的ID替换掉，才有办法找到这个按钮。 真是去他娘的

                var submitID = input.id;

                if (findex > 0) {
                    var scriptE = String.format('var e = arguments.length > 0 ? arguments[0] : event;var p=maxPanelManager["{0}"];  var t = e.srcElement || e.target;', this.panelID);
                    var scriptSubmit = String.format('if(p.onSubmit){ if(!p.onSubmit("{0}")){ return false; }  } document.forms["{0}"].onkeypress=null; ajaxPostForm("{0}",null, "{1}", function(r) { var p=maxPanelManager["{2}"];if(p)p.setContent(r); });  endEvent(e); return false;', fid, input.name, this.panelID);
                    var scriptMask = 'p.panel.style.cursor="wait";'; //var maskHTML =\'<div class="dropdownmenuloader" style="position:absolute;left:0;top:0; background-color:#ccc; color:white; text-align:center;"><span>请稍候...</span></div>\';p.panel.body.innerHTML +=maskHTML;';
                    var script = String.format(scriptE + 't=$("' + submitID + '"); if(t.disabled) return false; t.disabled="disabled";addCssClass(t,"button-disable"); ' + scriptMask + scriptSubmit);

                    ///IE和Safari下必须拦截表单的回车
                    if (btnIndex == 0) {
                        f.defaultButton = input;
                        if (max.browser.isIE || max.browser.isSafari) {
                            f.onsubmit = function (e) { endEvent(e); return false; };
                            f.onkeypress = new Function(scriptE + ' if (e.keyCode!=13)return true;  var tn = t.tagName.toLowerCase(); if(tn!="input" && tn!="select") return; ' + String.format(" var f = document.forms['{0}'];  if(!f.defaultButton || f.defaultButton.disabled) return; f.onkeypress=null;", fid) + scriptMask + scriptSubmit);
                        }
                    }
                    addHandler(input, "click", new Function(script));
                }

                btnIndex++;
            }
        }
    }

    ///链接的AJAX跳转
    if (this.useAjaxLink) {
        var links = panel.body.getElementsByTagName("a");
        for (var j = 0; j < links.length; j++) {
            var a = links[j];
            if (a.target) continue;
            var href = a.href;
            if (href.indexOf("/max-dialogs/") == -1) continue;
            if (href.indexOf("javascript:") > -1) continue;
            if (href.indexOf("#") > -1) continue;
            if (a.onclick) continue;
            if (href.indexOf("isdialog=1") == -1)
                href += (hasQuery(href) ? "&" : "?") + "isdialog=1";
            a.href = "javascript:void(panel.loadPage('" + href + "'))";
        }
    }
    if (!this.hasContent) this.relocation();
    this.hasContent = 1;
    execInnerJavascript(s); //执行内部JAVASCRIPT
    this.panel.style.cursor = "default";
    window.currentPanel = null;
}

maxPanelCore.prototype.waiting = function () {
    this.setContent(String.format('<div class="dialogloader"><span>正在载入...</span></div>', max.consts["loading16"]));
    this.hasContent = 0;
}

maxPanelCore.prototype.loadPage = function (url) {
    var th = this;
    ajaxRequest(url || this.url, function (r) { th.setContent(r); });
    return false;
}

maxPanelCore.prototype.postToPage = function (url, datas) {
    var th = this;
    ajaxPostData(url || this.url, datas, function (as) { th.setContent(as) });
}

maxPanelCore.prototype.close = function () {
    if (window.panel == this) window.panel = null;
    if (max.browser.isIE6) {
        if (this.panel.frame)
            try {
                removeElement(this.panel.frame);
            } catch (e) { }
    }
    this.panel.style.display = "none";
    //====== begin 销毁对话框 =================
    window.setTimeout(String.format("delete maxPanelManager['{0}']; try{ removeElement($('{0}'))}catch(e){};", this.panelID), 50);
    //====== end 销毁对话框====================
    if (this.closeCallback && this.result) this.closeCallback(this.result);
    if (this.onclick) { this.onclick(this.panelID); }
    if (this.closeHandlers)
        for (var i = 0; i < this.closeHandlers.length; i++) this.closeHandlers[i](this.panelID);

    return false;
}

maxPanelCore.prototype.show = function () {
    this.panel.style.display = "";
}

///关闭对话框时需要调用的事件处理函数
maxPanelCore.prototype.addCloseHandler = function (f) {
    if (!this.closeHandlers)
        this.closeHandlers = [];
    this.closeHandlers.push(f);
}

maxPanelCore.prototype.focus = function () {
    window.panel = this;
    window.dialog = this; //兼容以前的Dialog代码
    for (var k in maxPanelManager) {
        var t = maxPanelManager[k];
        if (t.panelID != this.panelID)
            t.panel.style.zIndex = 50;
    }
    this.panel.style.zIndex = 51;
}


var maxPanel = function (cfg) {
    var isCached = 0;
    if (maxPanelManager[cfg.id]) isCached = 1;
    var panel;
    var trigger = cfg.trigger, w = cfg.w, h = cfg.h, position = cfg.position, cssClass = cfg.cssClass, innerCssClass = cfg.innerCssClass, bodyCssClass = cfg.bodyCssClass;
    if (!isCached) {
        panel = addElement("div", document.body);
        panel.id = cfg.id;
        panel.onclick = function (e) { window.isClickDialog = 1; };
        panel.className = cssClass;
        if (cssClass) addCssClass(panel, cssClass);
        panel.inner = addElement("div", panel);
        panel.inner.className = innerCssClass;
        panel.body = addElement("div", panel.inner);
        if (bodyCssClass) panel.body.className = bodyCssClass;
        addHandler(panel, "mouseover", new Function(String.format('var p=maxPanelManager["{0}"]; window.panel=p; p.isFocus=1;\
        window.dialog = window.panel;'
        //兼容以前的Dialog代码\
        , cfg.id)));
        addHandler(panel, "mouseout", new Function(String.format('maxPanelManager["{0}"].isFocus=0;', cfg.id)));
        addHandler(panel, "mousedown", new Function(String.format('maxPanelManager["{0}"].focus();', cfg.id)));
        var pos = "absolute";
        if (!trigger && !max.browser.isIE6)
            pos = "fixed";
        setStyle(panel, { position: pos, zIndex: 50 });
        maxPanelManager[cfg.id] = this;

        if (max.browser.isIE6) {
            var f = document.createElement("iframe");
            setStyle(f, { position: "absolute", left: "-0px", top: "-0px", height: "1px", width: "1px", zIndex: 1 });
            f.frameBorder = "0";
            document.body.appendChild(f);
            panel.frame = f;
        }
    }
    else {
        panel = maxPanelManager[cfg.id].panel;
        panel.isOld = 1;
    }

    if (w) { panel.inner.style.width = w + "px"; this.width = w; }
    if (h) { panel.inner.style.height = h + "px"; this.height = h; }

    this.panelID = cfg.id;
    this.trigger = trigger;
    this.position = position;
    this.panel = panel;

}

maxPanel.prototype = new maxPanelCore();

/// ajaxPanel,可以当Ifream用的AJAXPanel
//URL:
//trigger: 出发这个Panel打开的对象，
//callback : 当这个Panel关闭时回调的函数
//用法： var a=new ajaxPanel("xxxx.aspx",null,null);
//       a.loadPage();这样就会把xxx.aspx载入到当前的Panel
function ajaxPanel(url, trigger, callback) {
    var dialogPath = root + "/max-dialogs/";
    var postDatas;
    var maxUnique = 0;
    var pageUrl = "";
    var fIndex = -1;
    //=============begin 对话框类型识别/如有二级目录， 以二级目录名为模块名标识对话框类型，否则以页面文件名
    pageUrl = url.toLowerCase();
    if (pageUrl.indexOf("http://") > -1) {
        pageUrl = pageUrl.substr(pageUrl.indexOf("http://") + "http://".length);
        pageUrl = pageUrl.substr(pageUrl.indexOf("/"));
    }
    pageUrl = pageUrl.substr(dialogPath.length);

    if (pageUrl.indexOf("/") > -1) {
        var model = pageUrl.substr(0, pageUrl.indexOf("/"));
        maxUnique = model.getHashCode();
    }
    else {
        if (pageUrl.indexOf("/?") > -1) {
            fIndex = pageUrl.indexOf("?", pageUrl.indexOf("/?") + 2);
        }
        else {
            fIndex = pageUrl.indexOf("?");
        }

        pageUrl = fIndex == -1 ? pageUrl : pageUrl.substr(0, fIndex);
        maxUnique = pageUrl.getHashCode();
    }
    //=============end 对话框类型识别

    if (maxUnique == 0) maxUnique = url.getHashCode();

    url += hasQuery(url) ? "&isdialog=1" : "?isdialog=1";
    this.url = url;
    var pid = "mx_dialog_" + maxUnique;

    maxPanel.call(this, { id: pid,
        cssClass: "dialog",
        innerCssClass: "dialog-inner",
        trigger: trigger
    });
    this.closeCallback = callback;
    this.useAjaxLink = 1;
    return this;
}

ajaxPanel.prototype = new maxPanelCore();

function openPanel(url, trigger, cssClass, w, h, position, callback) {

    var id = "max_panel_" + url.getHashCode();

    var panelObject = new maxPanel({ id: id, w: w, h: h, trigger: trigger,
        position: position,
        cssClass: cssClass,
        innerCssClass: "dropdownmenu",
        bodyCssClass: "clearfix dropdownmenu-inner"
    });
    panelObject.closeCallback = callback;
    var body = panelObject.panel.body;
    var fh = String.format('<iframe height="{1}" width="{0}" frameborder="0" scrolling="no"></iframe>', w - 4, h - 4);
    body.innerHTML = fh;
    var frame = body.childNodes[0];
    var doc;
    if (frame.contentDocument) {
        doc = frame.contentDocument;
    } else {
        var win = frame.contentWindow;
        doc = win.document;
    }

    doc.open();
    doc.write(String.format('<img src="{0}" alt=""  />正在载入...', max.consts["loading16"]));
    doc.close();
    var ie6 = max.browser.isIE6;
    var fixedContainer = null;
    if (trigger) {
        var ot = 0, ol = 0;
        ///====================滚动内框顶坐标的修正 
        var pn = trigger.parentNode;
        while (pn) {
            if (!ot && pn.className && pn.className.indexOf("scroller") > -1) {
                ot = 0 - pn.scrollTop;
                if (ie6) break;
            }

            if (pn.nodeName.indexOf("document") != -1)
                break;

            var pnid = pn.getAttribute("id");

            if (!ie6 && pnid && pnid.indexOf("mx_dialog_") > -1) {
                fixedContainer = pn;
                break;
            }
            pn = pn.parentNode
        }
        var _p = "absolute";
        if (fixedContainer) { _p = "fixed"; }

        panelObject.panel.style.position = _p;

        //====================================== end

        showPopup(panelObject.panel, trigger, position, 0, ot);
    }
    else {
        moveToCenter(panelObject.panel);
    }
    frame.src = url;

    window.setTimeout(function () { panelObject.focus(); }, 20);

    return panelObject;
}


var ajaxLayer = function (url, trigger, position, cssClass) {
    var pid = "mx_layer_" + url.getHashCode();
    if (ajaxLayer.instance) {
        var p = ajaxLayer.instance;
        if (p.panelID != pid) {
            p.close();
        }
    }

    ajaxLayer._clickTrigger = 1;

    maxPanel.call(this, {
        id: pid,
        trigger: trigger,
        position: position,
        cssClass: cssClass,
        innerCssClass: "dropdownmenu",
        bodyCssClass: "clearfix dropdownmenu-inner"
    }
    );

    if (!ajaxLayer.inited) {
        ajaxLayer.inited = 1;
        ajaxLayer._clickTrigger = 0;
        window.setTimeout(function () {
            addHandler(document.documentElement, "click", function () {
                if (window.isClickDialog) return;
                if (ajaxLayer._clickPanel) { ajaxLayer._clickPanel = 0; return; } if (ajaxLayer._clickTrigger) {
                    ajaxLayer._clickTrigger = 0; return;
                } if (ajaxLayer.instance) {
                    try { ajaxLayer.instance.close(); } catch (e) { } //这里加try是防止层已经被外面关闭
                    ajaxLayer.instance = null;
                }
            })
        }, 20);
    }

    if (max.browser.isIE6) {
        if (this.panel.frame) {
            removeElement(this.panel.frame)
            this.panel.frame = null;
        }
    }

    addHandler(this.panel, "click", function (e) { ajaxLayer._clickPanel = 1; });
    ajaxLayer.instance = this;
}

ajaxLayer.prototype = new maxPanelCore();


var topLayer = function (url, trigger, position, cssClass) {
    this.offsetLeft = 15;
    this.position = "right";
    ajaxLayer.call(this, url, trigger, position, cssClass);
}
topLayer.prototype = new maxPanelCore();

function openTopbarLayer(url, trigger, cssClass, w, h, position, event) {
    cssClass = cssClass ? "dropdownmenu-wrap " + cssClass : "dropdownmenu-wrap";
    var panel = new topLayer(url, trigger, position, cssClass);
    panel.setContent('<div class="dropdownmenuloader"><span>正在载入...</span></div>');
    panel.hasContent = 0;
    panel.loadPage(url);
    panel.focus();

    return false;
}

//ajax 层
function openAjaxLayer(url, trigger, cssClass, w, h, position, event) {

    cssClass = cssClass ? "dropdownmenu-wrap " + cssClass : "dropdownmenu-wrap";
    var panel = new ajaxLayer(url, trigger, position, cssClass);

    panel.setContent('<div class="dropdownmenuloader"><span>正在载入...</span></div>');
    panel.hasContent = 0;
    panel.loadPage(url);
    panel.focus();
    return false;
}


//ajax 层
function openFriendList(url, trigger, cssClass, w, h, position, event) {
    var pid = "mx_layer_" + url.getHashCode();
    cssClass = cssClass ? "dropdownmenu-wrap " + cssClass : "dropdownmenu-wrap";
    var panel = new maxPanel({
        id: pid,
        trigger: trigger,
        position: position,
        cssClass: cssClass,
        innerCssClass: "dropdownmenu",
        bodyCssClass: "clearfix dropdownmenu-inner"
    }
    );
    if (!max.browser.isIE6)
        setStyle(panel.panel, { position: "fixed" });
    panel.setContent('<div class="dropdownmenuloader"><span>正在载入...</span></div>');
    panel.hasContent = 0;
    panel.loadPage(url);
    panel.focus();
    return false;
}


//========================基于模板的表格对象=========================================================

//基于模版的动态表格
var DynamicTable = function (tableid, keyname) {
    this.maxKey = 1;
    var keyFiled = $$(keyname);
    if (keyFiled) {
        for (var i = 0; i < keyFiled.length; i++) {
            if (keyFiled[i].value != '{0}')
                if (parseInt(keyFiled[i].value) >= this.maxKey)
                    this.maxKey = parseInt(keyFiled[i].value) + 1;
        }
    }
    this.newRowId = "newrow-{0}"; //新行的ID  
    this.deleteTrigger = "deleteRow{0}"; //删除行的那个element的ID
    this.cellContentTemplates = new Array();
    this.table = $(tableid);
    this.body = null;
    var tbody = this.table.getElementsByTagName("tbody");
    if (tbody.length)
        this.body = tbody[0];
    else
        this.body = this.table;
    this.tamplate = "";
    for (var i = 0; i < this.body.rows.length; i++) {
        if (this.body.rows[i].id == "newrow") {
            for (var j = 0; j < this.body.rows[i].cells.length; j++) {
                this.cellContentTemplates.push(
                 { innerHTML: this.body.rows[i].cells[j].innerHTML
                 , style: this.body.rows[i].cells[j].style
                 }
                );
            }
            break;
        }
    }
    removeElement($('newrow'));
}

DynamicTable.prototype.insertRow = function (callback) {
    var cell, rowIndex = this.body.rows.length;
    var row = this.body.insertRow(rowIndex);
    for (var i = 0; i < this.cellContentTemplates.length; i++) {
        cell = row.insertCell(i);
        cell.innerHTML = String.format(this.cellContentTemplates[i].innerHTML, this.maxKey);
        if (this.cellContentTemplates[i].style) {
            for (var s in this.cellContentTemplates[i].style) {
                try {
                    cell.style[s] = this.cellContentTemplates[i].style[s];
                }
                catch (e) { }
            }
        }
    }
    row.id = String.format(this.newRowId, this.maxKey);
    var _this = this;
    var newrow = this.maxKey;
    addHandler($(String.format(this.deleteTrigger, this.maxKey)), "click", function () { _this.deleteRow(newrow) });
    if (callback) callback(this.maxKey);
    this.maxKey++;
}

DynamicTable.prototype.deleteRow = function (newrowindex) {
    var rowId = String.format(this.newRowId, newrowindex);
    removeElement($(rowId));
    //this.maxKey--;
}
//===========================================================

//剪贴板
function copyToClipboard(txt) {
    if (window.clipboardData) { window.clipboardData.clearData(); window.clipboardData.setData("Text", txt); }
    else if (navigator.userAgent.indexOf("Opera") != -1) { window.location = txt; }
    else if (window.netscape) {
        try { netscape.security.PrivilegeManager.enablePrivilege("UniversalXPConnect"); }
        catch (e) { alert("被浏览器拒绝！\n请在浏览器地址栏输入'about:config'并回车\n然后将'signed.applets.codebase_principal_support'设置为'true'"); return; }
        var clip = Components.classes['@mozilla.org/widget/clipboard;1'].createInstance(Components.interfaces.nsIClipboard); if (!clip)
            return; var trans = Components.classes['@mozilla.org/widget/transferable;1'].createInstance(Components.interfaces.nsITransferable); if (!trans)
            return; trans.addDataFlavor('text/unicode'); var str = new Object(); var len = new Object(); var str = Components.classes["@mozilla.org/supports-string;1"].createInstance(Components.interfaces.nsISupportsString); var copytext = txt; str.data = copytext; trans.setTransferData("text/unicode", str, copytext.length * 2); var clipid = Components.interfaces.nsIClipboard; if (!clip)
            return; clip.setData(trans, null, clipid.kGlobalClipboard);
    }
    alert("复制成功！");
}



//=======================================日期对话框=========================================

function initDatePicker(id, trigger) {
    var e, t;
    if (typeof (id) == "string")
        e = $(id);
    else {
        e = id;
    }
    if (trigger)
        t = $(trigger);

    if (e.readOnly || e.disabled) return;
    addHandler(e, "click", showDate);
    if (t) addHandler(t, "click", showDate);

    function showDate() {
        if (!initDatePicker.inited) {
            initDatePicker.inited = 1;
            addHandler(document.documentElement, "mousedown", closeDataPicker);
        }
        else {
            closeDataPicker();
        }

        var date = e.value;
        var datePickerUrl = root + "/max-assets/javascript/datepicker.html?date=" + date;

        window.setTimeout(function () {
            var dp = openPanel(datePickerUrl, e, '', 310, 190, "auto", function (r) {
                e.value = r;
            });
            dp.focus();
            window.datePanel = dp;
        }, 50);
    }

    function closeDataPicker() {
        if (window.datePanel) window.datePanel.close();
        window.datePanel = null;
    }
}

//=================================================
///初始化颜色选择器
//id  : 可以是节点的ID也可以是 Element
function initColorSelector(id, trigger) {
    var t;
    if (trigger)
        t = $(trigger);

    if (typeof (id) == "string") var e = $(id); else e = id;


    function setBg(c) {
        if (t) {

            var n;
            if (t && t.childNodes && t.childNodes.length > 0) {
                n = t.childNodes[0];
            }
            if (n) n.style.backgroundColor = c;
        }

    }

    var color = e.value;
    if (color == "") { color = "ffffff"; } else {
        setBg(color);
        color = color.replace("#", "")
    };
    var colorvalue = parseInt(color, 16);
    var colortext = to16(0xFFFFFF ^ colorvalue);
    if (e.readOnly) return;
    e.readOnly = true;
    addHandler(e, "click", showColor);
    if (t) addHandler(t, "click", showColor);

    function showColor() {
        if (!initColorSelector.inited) {
            initColorSelector.inited = 1;
            addHandler(document.documentElement, "mousedown", closeColorPicker);
        }
        else
            closeColorPicker();

        var color = e.value.replace("#", "");
        if (color == "") { color = "ffffff"; }
        var colorBoardUrl = root + "/max-assets/javascript/colorboard.html?color=" + color;

        window.setTimeout(function () {
            window.colorPanel = openPanel(colorBoardUrl, e, '', 240, 240, "auto", function (r) {
                e.value = r;
                setBg(r);
            });
            window.colorPanel.focus();
        }, 50);
    }

    function closeColorPicker() {
        if (window.colorPanel) window.colorPanel.close();
        window.colorPanel = null;
    }
}