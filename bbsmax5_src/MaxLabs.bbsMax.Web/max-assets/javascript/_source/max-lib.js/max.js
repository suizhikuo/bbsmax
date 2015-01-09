/*
copyright:bbs.bbsmax.com
by wenquan wenquan@bbsmax.com
*/
if (!window.root) window.root = "";
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
        stringBuilder.prototype.insert = function(index, str) {
            var l = 0;
            var arrOffset = -1;
            var innerOffset = 0;
            if (index == 0) {
                this.arr.unshift(str);
            }
            else {
                for (var i = 0; i < this.arr.length; i++) {
                    l += this.arr[i].length;
                    if (l >= index) {
                        arrOffset = i;
                        innerOffset = l - index; // index this.arr[i].length - str.length;
                        var v = this.arr[i];
                        innerOffset = v.length - innerOffset;
                        v = v.substring(0, innerOffset) + str + v.substring(innerOffset, v.length);
                        this.arr[i] = v;
                        break;
                    }
                }
            }
            return this;
        };


        stringBuilder.prototype.remove = function(start, length) {

            var l = 0;
            var arrStartOffset = -1, arrEndOffset = -1;
            var startInnerOffset;
            var innerOffset = 0;
            var endIndex = start + length;
            if (length <= 0)
                return this;
            for (var i = 0; i < this.arr.length; i++) {
                l += this.arr[i].length;
                if (l >= start && arrStartOffset == -1) {
                    arrStartOffset = i + 1;
                    var v = this.arr[i];
                    innerOffset = v.length - (l - start);
                    if (innerOffset < v.length) {
                        var s = v.substring(innerOffset, v.length);
                        v = v.substring(0, innerOffset);
                        this.arr[i] = v;
                        if (this.arr.length == i + 1) {
                            this.arr.push(s);
                        }
                        else {
                            this.arr[i + 1] = s + this.arr[i + 1];
                        }
                        l -= s.length;
                    }
                    continue;
                }
                if (l >= endIndex) {
                    arrEndOffset = i;
                    innerOffset = endIndex - l; // index this.arr[i].length - str.length;
                    if (innerOffset > 0) {
                        var v = this.arr[i];
                        innerOffset = v.length - innerOffset;
                        var s = v.substring(innerOffset, v.length);
                        v = v.substring(0, v.length - innerOffset);
                        this.arr[i] = v;
                        this.arr.splice(i, 0, s);
                    }
                    this.arr.splice(arrStartOffset, arrEndOffset - arrStartOffset);
                    break;
                }
            }
            return this;
        };
        stringBuilder.prototype.toString = function() {
            return this.arr.join('');
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
        getClientWidth: function () { return ((document.documentElement && document.documentElement.clientWidth) || document.body.clientWidth); },
        getClientHeight: function () { return ((document.documentElement && document.documentElement.clientHeight) || document.body.clientHeight); },
        getScrollTop: function () { return ((document.documentElement && document.documentElement.scrollTop) || document.body.scrollTop); },
        getScrollLeft: function () { return ((document.documentElement && document.documentElement.scrollLeft) || document.body.scrollLeft); },
        getFullHeight: function () { if (document.documentElement.clientHeight > document.documentElement.scrollHeight) return document.documentElement.clientHeight; else return document.documentElement.scrollHeight; },
        getFullWidth: function () { return document.documentElement.scrollWidth; },
        getBrowserRect: function () { var r = new Object(); r.left = this.getScrollLeft(); r.top = this.getScrollTop(); r.width = this.getClientWidth(); r.height = this.getClientHeight(); r.bottom = r.top + r.height; r.right = r.left + r.width; return r; }
    },
    coor: {
        left: function (e, left) { if (typeof (left) == "number") { e.style.position = "absolute"; e.style.left = left + "px"; } else { var offset = e.offsetLeft; if (e.offsetParent != null) offset += Left(e.offsetParent); return offset; } },
        top: function (e, top) { if (typeof (top) == "number") { e.style.position = "absolute"; e.style.top = top + "px"; } else { var offset = e.offsetTop; if (e.offsetParent != null) offset += Top(e.offsetParent); return offset; } },
        width: function (e, w) { if (typeof (w) == "number") { e.style.width = w + "px"; } else { return e.offsetWidth; } },
        height: function (e, h) { if (typeof (h) == "number") { e.style.height = h + "px"; } else { return e.offsetHeight; } },
        getRect: function (e) { var r = new Object(); r.left = getLeft(e); r.top = getTop(e); r.width = getWidth(e); r.height = getHeight(e); r.bottom = r.top + r.height; r.right = r.left + r.width; return r; }
    },
    consts: {
        loading16: root + '/max-assets/images/loading_16.gif',
        loading32: root + '/max-assets/images/loading.gif'
    },
    eval: function (s) {
        if (!s) return;
        s = s.trim();
        if (s.indexOf("<!--") == 0) {
            s=s.substr(4);
        }
        if (!s) return;
        if (max.browser.isIE) {
            return execScript(s);
        } else {
            return window.eval(s);
        }
    }
};

//根据id得到指定的对象
function $(id) { return (typeof id == 'string' ? document.getElementById(id) : id);}

//根据name得到一组对象
function $$(name) { return (typeof name == 'string' ? document.getElementsByName(name) : null);}

//根据tagname得到一组对象
function $T(name) { return (typeof name == 'string' ? document.getElementsByTagName(name) : null);}

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
function addElement(t,p){ var e= document.createElement(t); if(typeof(p)!='undefined') p.appendChild(e);else document.body.appendChild(e); return e;}

function getFileSize(s) {
    var u = ["B", "KB", "MB", "GB", "TB"];
    var uc = 0;
    while (s > 1024) { uc++; s = s / 1024; }
    s = parseFloat(s).toFixed(2);
    
    return  s + u[uc];
}

//显示表情预览
function showPreview(th,src)
{
    var preview=$('face_preview');
    var container=$('preview_container');
    var img;
    if(!preview)
    {
      preview = addElement("div")
      preview.id="face_preview";
      container=addElement("div",preview);
      container.id="preview_container";
    }img=addElement("img",container);
    img.onload=function(){if(!this.parentNode)return;AvatarLoaded(this);preview.style.visibility='visible';};
    img.width=preview.offsetWidth;
    img.height=preview.offsetHeight;
    img.src=src;
    var l=getLeft(th);
    var s=preview.style;
    s.left=l>100?'10px':'auto';
    s.right=l>100?'auto':'10px';
    s.position="absolute";
    th.onmouseout||(th.onmouseout=hidePreview);
}

function hidePreview(){$('preview_container').innerHTML='';$('face_preview').style.visibility='hidden';}

function AvatarLoaded(th,nonMargin){th.onload=null;th.onerror=null;function reload(){if(th.width==0&&th.height==0){setTimeout(reload,10);return;}}}

///移除节点
//elm: 节点
function removeElement(elm){var p; if(typeof(elm)=='string') elm = $(elm);   p=elm.parentNode; p.removeChild(elm);elm = null;};

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

function hideElement(e,callback)
{
    var ivtO=5,ivt=50;
    var t=new timer(ivt, function(n){
    if(n<=ivtO)
    {
        opacity(e,100-n*(100/ivtO));
    }
    else
    {
        if(callback)callback();
        t.stop();
        t=null;
    }});
    t.start();
}

function HTMLEncode(html) { var temp = document.createElement("div");  (temp.textContent != null) ? (temp.textContent = html) : (temp.innerText = html);   var output = temp.innerHTML;
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
    document.cookie = name + "=" + escape(value) + expire;
}

//删除名称为name的Cookie
function deleteCookie (name) 
{   
    var exp = new Date();
    exp.setTime (exp.getTime() - 1);
    document.cookie = name + "=; expires=" + exp.toGMTString();
}

//读取cookie
function readCookie(name)
{
    var cookieValue = "";
    var search = name + "=";
    if(document.cookie.length > 0)
    {
        offset = document.cookie.indexOf(search);
        if (offset != -1)
        {
          offset += search.length;
          end = document.cookie.indexOf(";", offset);
          if (end == -1) end = document.cookie.length;
          cookieValue = unescape(document.cookie.substring(offset, end))
        }
    }
    return cookieValue;
}

function hide(e,callback){
    hideElement(e,function(){
    var h=max.coor.height(e);
    if(e.style.borderTopWidth!=""&&!isNaN(e.style.borderTopWidth))
        h-=parseInt(e.style.borderTopWidth);
    if(e.style.borderBottomWidth!=""&&!isNaN(e.style.borderBottomWidth))
        h-=parseInt( e.style.borderBottomWidth);
    if(e.style.paddingTop!=""&&!isNaN(e.style.paddingTop))
        h-=parseInt( e.style.paddingTop);
    if(e.style.paddingBottom!=""&&!isNaN(e.style.paddingBottom))
        h-=parseInt(e.style.paddingBottom);
    _buf.push({id:e.id,ov:e.style.overflow,height:h,ds:e.style.display});
    setStyle(e,{overflow:'hidden'});
    var nh;
    var t=new timer(10,function(n){
    if(n<=10){
    nh=((10-n)/10*h)+"px";
    setStyle(e,{height:nh})}
    else{t.stop();t=null;setStyle(e,{display:"none"}); if(callback)callback();}
    });
    t.start();
    })
}

var _buf=new Array();
function show(e)
{
    var b=null,i;
    for(var k in _buf)
    {
       if( _buf[k].id==e.id)
       {
            i=k;
            b=_buf[k];
            break;
       }
    }
    if(b==null){setStyle(e,{display:''}); return}
    setStyle(e,{display:b.ds});
      //opacity(e,100);
    var t=new timer(10,function(n){
    if(n<=10) {
        setStyle(e,{height:(b.height*(n/10))+"px"});
        }
    else if(n<=20)
        opacity(e,(n-10)*10);
    else{
            t.stop();t=null;
            setStyle(e,{overflow:b.ov,height:b.height+"px"});
            _buf.splice(i,1);
        }
    });
    t.start();
}

//带动画效果的删除节点
function delElement(e,callback) {if (typeof (e).toString().toLowerCase() == "string") { e = $(e); }
    hide(e, function() {
        for (var k in _buf) {
            if (_buf[k].id == e.id) {
                _buf.splice(k, 1);
                break;
            }
        }
        removeElement(e);
        if (callback)
            callback();
    });
};

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
//        addHandler(form, "click", function() { form.focus = 1; });
//        addHandler(form, "mouseout", function() { form.focus=0; });
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
var maxDragObject = function (obj, mover) {
    var ht = mover || obj;

    function mouseup(e) {
        if (obj.parentNode == null) {
            removeHandler(window, "mouseup", mouseup);
            removeHandler(window, "mousemove", mousemove);
            return;
        } e = e || window.event;
        if (obj.drag) { obj.drag = 0; if (max.browser.isIE) { ht.releaseCapture(); } else { window.releaseEvents(Event.MOUSEMOVE | Event.MOUSEUP); e.preventDefault(); } document.body.onselectstart = null; }
    }
    function mousemove(e) {
        if (!obj.drag) return;
        e = e || window.event;
        var l, t;
        l = e.clientX - obj._x;
        t = e.clientY - obj._y;
        setStyle(obj, { left: l + "px", top: t + "px" });
        if (max.browser.isIE6) {
            if (obj.frame) {
                var f = obj.frame;
                setStyle(f, { left: (l+5) + "px", top: (t+5) + "px" });
            }
        }
    }

    function mousedown(e) {
        e = e || window.event;
        if (max.browser.isIE) { ht.setCapture(); } else { window.captureEvents(Event.MOUSEMOVE | Event.MOUSEUP); e.preventDefault(); } var l = getLeft(obj), t = getTop(obj); obj._x = e.clientX - l; obj._y = e.clientY - t; obj.drag = 1;
        document.body.onselectstart = function () { return false; };
    }

    addHandler(ht, "mousedown", mousedown);

    if (!max.browser.isIE) {
        ht = window; // document.body;
        ht = window; // document.body;
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
var showPopup = function (pop, target, position, offsetLeft, offsetTop) {
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

    if (target.style.position == "fixed") {
        l += rw.left;
        t += rw.top;
    }
    if (offsetLeft) l += offsetLeft;
    if (offsetTop) t += offsetTop;

    s.left = l + "px";
    s.top = t + "px";
}


var createBackgroundFrame = function (obj) {


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
            parentNode = document.body;
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

//往Textarea插入内容，insertLast
function textareaInsert(id, text, insertLast){
    $(id).value += text;
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

function scrollToBottom(id){
    var l=$(id);
    l.scrollTop = 65535;
}



function ctrlEnterEvent(callback, controlid1, controlid2) {

    for (var i = 1; i < arguments.length; i++) {
        var c = document.getElementById(arguments[i]);
        if (c) c.onkeydown = function(e) {
            e = e || window.event;
            if (e.ctrlKey && e.keyCode == 13) {
                callback && callback();
            }
        };
    }
}

//执行HTML内包含的JS
var execInnerJavascript = function (html) {
    var regJs, regJs0;
    if (max.browser.isIE) {
        regJs = /<script[^>]+?>((?:.\n*)+?)<\/script>/ig;  //使用下面一样的写法就会使浏览器挂掉如果
        regJs0 = /<!--\{js(?:\s|\n)((?:.\n*)+?)\}-->/ig; //注释方式写的JS
    } else {
        regJs = /<script[^>]*?>((?:.|\n|\r)*?)<\/script>/ig,
     regJs0 = /<!--\{js(?:\s|\n)((?:.|\n|\r)+?)\}-->/ig; //注释方式写的JS
    }
    var m;
    while (m = regJs.exec(html)) {
        max.eval(m[1]);

    }
    while (m = regJs0.exec(html)) {
        max.eval(m[1]);
    }
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

//----Begin MaxAjaxEngine--

ajaxWorker = function (url, method, content) {
    this.r = null;
    this.url = url;
    this.method = method;
    this.content = content;
    this.header = {};
    //    try {
    //        this.header["Connection"] = "close";
    //    }
    //    catch (e) {

    //    }
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

    this.addListener = function (http_status, func) {
        if (!this.L)
            this.L = [];
        this.L[http_status] = func;
        return this;
    };

    this.setHeader = function (name, value) {
        this.header[name] = value;
        this.r.setRequestHeader(name, value);
        return this;
    };

    this.send = function (newurl) {
        if (this.method != "post" && this.method != "get")
            this.method = "get";

        this.r.open(this.method, newurl ? newurl : this.url, true);

        for (var h in this.header) {
            this.r.setRequestHeader(h, this.header[h]);
        }

        this.r.send(this.content);
    };
    if (this.r) this.r.onreadystatechange = function () {
        if (self.r.readyState == 4) {
            if (self.L[self.r.status] != null) {
                self.L[self.r.status](self.r.responseText);
            }
            else {

            }
        }
    };
};

//import MaxEngine.AjaxObj;
ajaxRender = function (url, ids, onSucceed, onError) {
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
    new ajaxWorker(url).addListener(200, function (as) {
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
getFormData = function (f, buttonName) {
    var list = {};

    for (var i = 0, len = f.elements.length; i < len; i++) {
        var item = f.elements[i];
        var name = item.name;
        if (item.disabled) continue;
        if (!name) continue;
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
                var _selected = 0;
                for (var j = 0; j < item.options.length; j++) {
                    if (item.options[j].selected) {
                        list[name].push(item.options[j].value);
                        _selected = 1;
                    }
                }
                if (!_selected) {
                    if (item.selectedIndex >= 0) {
                        list[name].push(item.options[item.selectedIndex].value);
                    }
                }
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
ajaxPostData = function (url, datas, callback) {
    var aw = new ajaxWorker(url, 'post', datas);
    aw.addListener(200, function (as) {
        if (callback)//如果不自动出来请求结果， 那么就把结果交由外面的函数处理
            callback(as);
        else
            return;
    });

    aw.addListener(500, function (r) { if (callback) callback(r); });
    aw.addListener(404, function (r) { if (callback) callback("404 Page Not Found"); });
    aw.addListener(403, function (r) { if (callback) callback("403 Forbidden"); });
    aw.addListener(400, function (r) { if (callback) callback("400 域名不能正确解析"+url); });

    aw.send();


}

///根据指定的url和post数据发起ajax请求，并调用callback,把结果HTML处给callback
function ajaxRequest(url, callback) {
    var aw = new ajaxWorker(url, "GET");
    aw.addListener(200, function (r) { if (callback) callback(r); });

    aw.send();
}

ajaxPostForm = function(formID, url, buttonName, callback) {
    var _f = $(formID);

    url = url || _f.action || location.href;

    if (!_f) return true;

    var content = getFormData(_f, buttonName);

    ajaxPostData(url, content, callback);
}

ajaxSubmit = function (formID, buttonName, ids, onSucceed, onError, autoReplace) {
    var _f = $(formID);
    var url = _f.action || location.href;

    if (!ids || ids == '')
        ids = '*';

    if (ids != null) {
        if (hasQuery(url))
            url += '&_max_ajaxids_=' + ids; //+ '&_random_query_id_=' + new Date().getMilliseconds(); //此处去除请求随机码。
        else
            url += '?_max_ajaxids_=' + ids; //+'&_random_query_id_=' + new Date().getMilliseconds();
    }

    if (!_f) return true;

    var content = getFormData(_f, buttonName);
    new ajaxWorker(url, 'post', content).addListener(200, function (as) {
        //=======所有页面上的弹出菜单全部隐藏初始化=======================
        var il = maxPopupCollection.innerList;
        if (il) {
            for (var i = 0; i < il.length; i++)
                il[i].hide();
        }
        
        //============================== 
        setButtonDisable(buttonName, false);
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
            var panel = document.getElementById(id);

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

function imageScale2(th,dw,dh,nonMargin){
	var ow=th.offsetWidth,oh=th.offsetHeight;
	th.ow=ow;
	th.oh=oh;
	
	if(ow/oh>dw/dh){
		if(ow>dw){
			th.style.width=dw+'px';
			th.style.height=dw/ow*oh+'px';
			nonMargin||(th.style.marginTop=(dh-dw/ow*oh)/2+'px');
		} else {
			nonMargin||(th.style.marginTop=(dh-oh)/2+'px');
		}
	}else{
		if(oh>dh){
			th.style.height=dh+'px';th.style.width=dh/oh*ow+'px';
		}else{
			nonMargin||(th.style.marginTop=(dh-oh)/2+'px');
		}
	}
}

function AvatarLoaded2(th,nonMargin){
	th.onload=null;
	th.onerror=null;
	
	function reload(){
	
		if(th.width == 0 && th.height == 0){
			setTimeout(reload,10);return;
		}
		
		th.style.visibility='inherit';
		
		var w=th.width,h=th.height;
		
		th.style.width='auto';
		th.style.height='auto';
		
		w && h && imageScale2(th,w,h,!!nonMargin);
		
		th.parentNode && (th.parentNode.style.backgroundImage='url('+BBSMAX.SpaceImage+')');
	}

	reload();
}

function AvatarError2(th,defaultImage){
	th.onload=null;
	th.onerror=null;
	th.src=BBSMAX.SpaceImage;
	th.parentNode&&(th.parentNode.style.background='url('+(defaultImage||BBSMAX.AvatarDefault)+') no-repeat center center');
}

//----End MaxEngine----

/******************************* 弹出菜单容器 *********************************/
var maxPopupCollection=function(){var c=maxPopupCollection;if(!c.innerList)c.innerList=new Array();}
maxPopupCollection.prototype.add=function(obj){obj.onShow=this.onshow;maxPopupCollection.innerList.push(obj);} 
maxPopupCollection.prototype.onshow=function(obj){var cc=maxPopupCollection;for(var i=0;i<cc.innerList.length;i++){if(cc.innerList[i]!=obj){cc.innerList[i].hide();}}}
maxPopupCollection.instance = function(){if(!window.maxPopupManager)window.maxPopupManager=new maxPopupCollection();return window.maxPopupManager;}
maxPopupCollection.prototype.hideAll = function () {
    for (var i = 0; i < maxPopupCollection.innerList.length; i++) {
        maxPopupCollection.innerList[i].hide();
    }
}
//**弹出菜单基类
var popupBase = function (listid, trigger, autopopup) {

    this.triggers = new Array();

    this.list = typeof (listid) == "object" ? listid : $(listid);
    this.auto = autopopup;

    if (trigger instanceof Array) {
        for (var i = 0; i < trigger.length; i++) {
            var t = $(trigger[i]);
            if (t != null) this.triggers.push(t);
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
var popup = function (listid, trigger, autoPop, onSelectClass, pos) {
    if (max.browser.isIE6) this.createBack = true;
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
    s.zIndex = 52;

    for (var i = 0; i < this.triggers.length; i++) {
        var t = this.triggers[i];
        addHandler(t, this.auto ? 'mouseover' : 'click', function (e) { _this.show(e); });
        if (this.auto)
            addHandler(t, "mouseout", function () { _this.hideList(); });
        else {
            addHandler(t, "mouseout", function (e) { _this.focus = false; });
            addHandler(t, "mouseover", function (e) { _this.focus = true; });
        }
    }

    if (this.auto) {
        addHandler(this.list, 'mouseover', function (e) { _this.focus = true; });
        addHandler(this.list, "mouseout", function () { _this.hideList(); });
    }
    else {
        addHandler(this.list, "mouseout", function (e) { _this.focus = false; });
        addHandler(this.list, "mouseover", function (e) { _this.focus = true; });
        addHandler(document.documentElement, "click", function (e) { if (_this.focus == false) _this.hide(e); })
    }

    maxPopupCollection.instance().add(this);
}
popup.prototype.hideList = function() {
    var ts = this;
    setTimeout(function() { if (ts.focus == false) ts.hide(); }, 1000);
    ts.focus = false;
}

popup.prototype.show = function (e, f, target) {

    if (!f && this.auto) { //延时弹出
        this.focus = true;
        e = e || window.event;
        var t = this;
        target = e.target ? e.target : e.srcElement;
        window.setTimeout(function () { if (t.focus) t.show(e, 1, target); }, 200);
        return;
    }

    e = e || window.event;
    this.focus = true;

    if (!target) {
        if (!e)
            target = this.triggers[0];
        else
            target = e.target ? e.target : e.srcElement;
    }

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

    showPopup(this.list, target, this.position, this.offsetLeft, this.offsetTop);

    /*************** 这段代码又是IE6下的Dropdown跑到上面来的问题 ***************/
    if (this.createBack) {
        if (!this.list.frame) {
            this.list.frame = addElement("iframe");
            setStyle(this.list.frame, { position: "absolute", zIndex: 48, border: "none" });
        }

        var lrect = getRect(this.list);
        setStyle(this.list.frame, {
            left: (lrect.left + 2) + "px"
            , top: (lrect.top + 2) + "px"
            , width: (lrect.width - 4) + "px"
            , height: (lrect.height - 4) + "px"
        });
        setVisible(this.list.frame, 1);
    }
    /*******************************IE6 死绝的时候就可以把这段代码去掉了********/
    var cn = this.triggerStyle;

    if (cn) {
        addCssClass(target, cn);
        this.cssNode = target;
    }
    if (this.onShow) this.onShow.call(maxPopupCollection.instance(), this);

}

popup.prototype.hide = function (e) {

    if (this.maskDialog && window.isClickDialog ) return; //这行代码的意思是说，点击对话框的时候这个对话框是不关闭的
    var s = this.list.style;
    s.display = 'none';
    var cn = this.triggerStyle;
    if (cn) {
        if (this.cssNode) {
            removeCssClass(this.cssNode, cn);
        }

    }

    /*************** 这段代码又是IE6下的Dropdown跑到上面来的问题 ***************/
    if (max.browser.isIE6) {
        if (this.list.frame) setVisible(this.list.frame, 0);
    }

    if (this.onHide) this.onHide.call(maxPopupCollection.instance(), this);
}

/**************************************用户菜单***********************************/

var userMenus={};
var userMenu = function(id, trigger, exclu, menuCreateInBody) {
    var list;
    var menu;
    var d = new Date();

    var controlId = 'user_menu_' + d.getMilliseconds();
    // list = $(controlId);
    // if(list==null)
    // {

    var template = $("usermenuTemplate");

    var sb = new stringBuilder();
    list = addElement("div", menuCreateInBody ? document.getElementsByTagName('body')[0] : trigger.parentNode);
    list.className = template.className;

    var txt = template.innerHTML;
    txt = txt.replace(/%7B/ig, "{");
    txt = txt.replace(/%7D/ig, "}");
    txt = String.format(txt, id);
    list.id = controlId;
    list.innerHTML = txt;
    list.style.display = 'none';

    return new popup(list.id, trigger, false);
}

function openUserMenu(e, userid, exclu ) {

if(!e.id) e.id = "UT_" + userid + "_" + new Date().getMilliseconds();
var menuCreateInBody=true;
    var ums=userMenus[userid.toString()+e.id];
    if(ums)
    {
        var f=false;
        for( var i=0;i<ums.length;i++)
        {
            if(ums[i]==e)
            {
                f=true;
                break;
            }
        }
        
       if(f==false) getUserMenu.call(e);
         return false;
    }
    else
    {
       ums=new Array();
       userMenus[userid.toString() + e.id] = ums;
       getUserMenu.call(e);
         return false;
    } 
    function getUserMenu()
    {
        ums.push(this);
        var menu = new userMenu(userid, this, exclu, menuCreateInBody);
        menu.show();     
    }
}


////===================== Debuger ========================================================
//var debug = new function() {
//    this.isInited = false;
//    this.panal = null;
//    this.frame = null;
//    this.print = function(str, encode) {
//        if (this.panal == null) {
//            this.openDebuger();
//        }
//        if (encode)
//            str = this.escape(str);
//        this.buffer.push("<br/>" + str);
//        var ts = this;
//        timeOut = 20;
//    }
//    var timeOut = 10;
//    var timerHandler = null;

//    this.openDebuger = function() {
//        if (this.panal == null) {
//            var s;
//            this.frame = document.createElement("table");
//            var frame = this.frame;
//            var head = frame.insertRow(0);
//            var toolbar = head.insertCell(0);
//            var body = frame.insertRow(1).insertCell(0);
//            s = frame.style;
//            s.width = '450px';

//            //body.align = "center";
//            frame.border = "1";

//            s = toolbar.style;
//            s.padding = '4px';

//            var tBtn = document.createElement("span");
//            toolbar.appendChild(tBtn);
//            tBtn.style.margin = "4px";
//            var l = document.createElement("a");
//            tBtn.appendChild(l);
//            l.href = "javascript:void(debug.clear())";
//            l.innerHTML = "清空";
//            tBtn.appendChild(l);

//            tBtn = document.createElement("span");
//            toolbar.appendChild(tBtn);
//            tBtn.style.margin = "4px";
//            l = document.createElement("a");
//            tBtn.appendChild(l);
//            l.href = "javascript:void(debug.expend())";
//            l.innerHTML = "缩/放";

//            tBtn = document.createElement("span");
//            toolbar.appendChild(tBtn);
//            tBtn.style.margin = "4px";
//            l = document.createElement("a");
//            tBtn.appendChild(l);
//            l.href = "javascript:void(debug.close())";
//            l.innerHTML = "关闭";


//            this.panal = document.createElement("div");
//            body.appendChild(this.panal);
//            s = this.panal.style;
//            s.textAlign = "left";
//            s.padding = "8px";
//            s.lineHeight = "160%";
//            s.color = "blue";
//            s.height = "250px";
//            s.width = "430px";
//            s.overflow = "scroll";

//            this.codeArea = document.createElement("textarea");
//            body.appendChild(this.codeArea);
//            s = this.codeArea.style;
//            s.lineHeight = "160%";
//            s.height = "80px";
//            s.width = "399px";
//            this.codeArea.id = "codeArea";

//            this.runButton = document.createElement("button");
//            body.appendChild(this.runButton);
//            var rbun = this.runButton;
//            s = rbun.style;
//            rbun.innerHTML = "执行";
//            try {
//                //rbun.type="button";
//            }
//            catch (e) {
//            }
//            s.height = "80px";
//            s.width = "50px";
//            this.codeArea.id = "runButton";
//            rbun.onclick = function() { debug.execute(debug.codeArea.value); }
//            this.buffer = [];

//            //s.left = '';
//            //s.top = '';

//            //            var codeDiv;
//            //            div = addElement("div", body);
//            //            this.codeArea = addElement("textarea", div);
//            //            s = this.codeArea.style;
//            //            s.width = "79%";
//            //            s.height = "80px";
//            //this.execButton=addElement(""
//            this.container = document.createElement("div");
//            s = this.container.style;
//            s.position = 'absolute';
//            s.left = '150px';
//            s.top = '0px';
//            s.width = "450px";
//            s.backgroundColor = 'white';
//            this.container.appendChild(frame);
//            document.body.appendChild(this.container);
//            this.panal.innerHTML = "springDebuger Ver:1.0.0";
//            this.buffer = [];

//            timerHandler = window.setInterval(function() {
//                timeOut -= 10;
//                if (window.debug && timeOut <= 0 && debug.buffer.length > 0) {
//                    var p = debug.panal;
//                    var txt = p.innerHTML;
//                    txt += debug.buffer.join("");
//                    p.innerHTML = txt;
//                    p.scrollTop = p.scrollHeight;
//                    debug.buffer = null;
//                    debug.buffer = [];
//                }
//            }, 20);

//            this.body = body;
//        }
//    }

//    this.escape = function(html) {
//        html = html.replace(/&/g, "&amp;");
//        html = html.replace(/</g, "&lt;");
//        html = html.replace(/>/g, "&gt;");
//        html = html.replace(/\xA0/g, "&nbsp;");
//        html = html.replace(/\x20/g, " ");
//        return html;
//    }

//    this.expend = function() {
//        var s = this.body.style;
//        if (s.display == "none")
//            s.display = "";
//        else
//            s.display = "none";
//    }

//    this.execute = function(code) {
//        var k;
//        if (code)
//            k = code;
//        else
//            k = prompt("请输入命令", '');
//        try {
//            eval(k);
//        }
//        catch (e) {
//            this.print('<font color="red">错误：' + e.message + '</font>');
//        }
//    }

//    this.clear = function() {
//        if (this.panal) {
//            this.buffer = [];
//            this.panal.innerHTML = '';
//        }
//    }

//    this.close = function() {
//        this.isInited = false;
//        document.body.removeChild(this.container);
//        this.panal = null;
//        window.clearInterval(timerHandler);
//    }

//    window.onerror = function(msg, url, line) {
//        var m = "错误：" + msg + "<br />行：" + line + "<br />URL：" + url;
//        debug.print(m);
//        return true;
//    }
//}
///*******************************/
