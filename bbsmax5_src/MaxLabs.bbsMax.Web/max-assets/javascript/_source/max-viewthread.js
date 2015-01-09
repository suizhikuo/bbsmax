//=======================================用户扩展信息代码========================================================

var userExtraDatas = {};
var ajaxRequesting = false;
var showUserExtradata = function(th, id, pid) {
    var timerId = 0;
    var panelParent = $(String.format("extradata_{0}_{1}", id, pid));
    if (!panelParent.isProcessed) {
        var topDiv = $("authorinfoInner_" + pid);
        var h = th.offsetHeight;
        //var div = addElement("div", topDiv);
        //div.id = String.format("hide_{0}_{1}", id, pid);
        //var s = div.style;
        //        s.height = h + "px";
        topDiv.style.height = h + "px";
        addHandler(topDiv, "mouseover", function() { addCssClass(topDiv, "authorinfo-expand"); });
        addHandler(topDiv, "mouseout", function() { removeCssClass(topDiv, "authorinfo-expand"); /*if (timerId != 0) { window.clearTimeout(timerId); } */});
        addCssClass(topDiv, "authorinfo-expand");
        panelParent.isProcessed = true;
        timerId = window.setTimeout("getExtraDatas(" + id + "," + pid + ");", 200);
    }
    else {
        //var div = $(String.format("hide_{0}_{1}", id, pid));
        //setVisible(div, 1);
        return;
    }
}

function getExtraDatas(id, pid) {
 
    var panelParent = $(String.format("extradata_{0}_{1}", id, pid));
    var userdata = userExtraDatas[id.toString()];
    if (!userdata && ajaxRequesting == false) {
        ajaxRequesting = true;
        var tdiv = addElement("div", panelParent);
        tdiv.innerHTML = '<div class="ajaxloading"><span>加载中...</span></div>';
        var u = String.format(authorUrl, id, new Date().getMilliseconds());
        var ajax = new ajaxWorker(u, "get", '');
        ajax.addListener(200, function(r) {
            userdata =  eval("(" + r + ")");
            userExtraDatas[id.toString()] = userdata;
            panelParent.removeChild(panelParent.childNodes[panelParent.childNodes.length - 1]);
            showInfo();
            ajaxRequesting = false;
        });
        ajax.send();
    }
    else {
        if (ajaxRequesting == false)
            showInfo();
    }

    function showInfo() {
        var eid = String.format("panel_{0}_{1}", id, pid);
        if ($(eid)) return;
        var temp = $("authorExtraTempalte");
        var panel = addElement("div", panelParent);
        panel.id = eid;
        panel.className = temp.className;
        var html = temp.innerHTML;

        for (var k in userdata) {
            var obj = userdata[k];
            if (obj instanceof Array) {
                var f1 = "<!--{" + k + "}-->";
                var f2 = "<!--{/" + k + "}-->";
                var index1 = html.indexOf(f1);
                var index2 = html.indexOf(f2) + f2.length;

                if (index1 == -1 || index2 == -1 || index1 > index2)
                    continue;

                var listHtml = "";
                var html1 = html.substr(0, index1);
                var html2 = html.substr(index2, html.length - index2);
                var itemTemplate = html.substr(index1, index2 - index1);
                for (var i = 1; i <= obj.length; i++) {
                    var _item = obj[i-1];
                    var itemHTML = itemTemplate;
                    for (var ki in _item) {
                        itemHTML = itemHTML.replace("{" + ki + "}", _item[ki]);
                        itemHTML = itemHTML.replace("%7B" + ki + "%7D", _item[ki]);
                    }
                    itemHTML = itemHTML.replace("{_i}", i);
                    listHtml += itemHTML;
                }

                html = html1 + listHtml + html2;
            }
            else {
                html = html.replace("{" + k + "}", obj);
            }
        }
        panel.innerHTML = html;
    }
}

//==============================================================以下是图片浏览器代码=============================================================
var bhtml = '\
﻿<div class="imagebrowser-label" id="vLabel" style="display:none;">100%</div>\
<div class="imagebrowser-tools" id="vTopbar" style="display:none;">\
    <div class="imagebrowser-tools-inner">\
        <a class="prev" href="javascript:;"  title="上一张" id="vPrevious" onclick="_IV.previous();">|&lt;</a>\
        <a class="next" href="javascript:;" title="下一张" id="vNext" onclick="_IV.next();">&gt;|</a>\
        <a class="zoomout" href="javascript:;" title="缩小" id="Button2" onclick="_IV.zoom(-1);">-</a>\
        <a class="zoomin" href="javascript:;"  title="放大" id="Button3" onclick="_IV.zoom(1);">+</a>\
        <a class="original" href="javascript:;" title="原始尺寸" id="Button5" onclick="_IV.originalSize();">1:1</a>\
        <a class="blank" href="javascript:;"  title="新窗口打开" id="Button1" onclick="_IV.showInWindow();">□</a>\
        <a class="close" href="javascript:;"  title="关闭" id="Button4" onclick="_IV.close();">X</a>\
    </div>\
</div>\
<div class="imagebrowser-nav" id="vToolbar" style="display:none;">\
    <a class="prev" href="javascript:;" onmousedown="_IV.scroll(-1)" onmouseup="_IV.stopScroll()">&lt;</a>\
    <div class="imagebrowser-thumbs" id="vScroll">\
        <div class="imagebrowser-thumbs-inner" id="vList" style="left:0px; position:relative; top:0px;">\
            <div class="thumblist" id="vListTable"></div>\
        </div>\
    </div>\
    <a class="next" href="javascript:;" onmouseup="_IV.stopScroll()" onmousedown="_IV.scroll(1)">&gt;</a>\
</div>';

document.write(bhtml);


function createImageViewer() {
    var imageViewer = {};
    imageViewer.objectPool = {};
    imageViewer.toolbar = $("vToolbar");
    imageViewer.topbar = $("vTopbar");
    imageViewer.container = $("vContainer");
    imageViewer.list = $("vList");
    imageViewer.index = 0;
    imageViewer.listScroll = $("vScroll");
    imageViewer.listTable = $("vListTable");
    imageViewer.label = {};
    imageViewer.label.object = $("vLabel");
    imageViewer.label.show = function(text) {
        this.seconds = 25;
        setVisible(this.object, 1);
        this.object.innerHTML = text;
        opacity(this.object, 75);
        if (!this.visibled) {

            this.visibled = true;
            var t = new timer(20, function() {
                var l = _IV.label;
                l.seconds--;
                if (l.seconds <= 0) {
                    t.stop();
                    l.hide();
                }

            });
            t.start();
            _IV.setPos(this.object);
        }
    }

    imageViewer.label.hide = function() {
        this.seconds = 0;
        this.visibled = false;
        setVisible(this.object, 0);
    }



    imageViewer.appendImageItem = function (img) {
        var src = img.src;
        if (!this.imageList) this.imageList = {};
        if (this.imageList[src] != null) return;
        this.imageList[src] = 1;
        var aTag = document.createElement("a");
        aTag.index = img.index;
        aTag.href = "javascript:;_IV.open('" + img.src + "');";
        //aTag.onclick = function () { "_IV.open('" + img.src + "');alert('hello')" };

        var ihtml = '<img onload="setVisible(this,true);imageScale(this,80,80);if(this.height<80)this.style.paddingTop=(Math.abs(80-this.height)/2)+\'px\'; " style="display:none;" src="' + src + '" />';
        aTag.innerHTML = ihtml;

        var table = this.listTable;

        var flag = false;
        for (var i = 0; i < table.childNodes.length; i++) {
            var a = table.childNodes[i];
            if (a.index > aTag.index) {
                table.insertBefore(aTag, a);
                flag = true;
                break;
            }
        }
        if (flag == false) {
            table.appendChild(aTag);
        }
        this.list.style.width = (table.childNodes.length * 90) + "px";

        //        var t = window._temp;
        //        var t2 = window._temp2;
        //        if (!t) t = 0;
        //        if (!t2) t2 = "";
        //        window._temp = t;
        //        window._temp += 10;
        //        t2 += ihtml;
        //        window._temp2 = t2;
        //        window.setTimeout(function () {
        //            var _t = window._temp;
        //            _t -= 10;
        //            window._temp = _t;
        //            if (_t > 0) return;

        //            _IV.listTable.innerHTML = window._temp2;

        //            window.setTimeout(function () {
        //                var _w = 0;
        //                var cn = _IV.listTable.childNodes;
        //                for (var j = 0; j < cn.length; j++) {
        //                    var _o = cn[j];
        //                    _w += _o.offsetWidth;
        //                    _w += 7;
        //                }

        //                _IV.list.style.width = _w + "px";
        //            }, 10);
        //        }, 10);
    }

    imageViewer.showInWindow = function() {
        if (this.current) open(this.current.src);
    }

    imageViewer.originalSize = function() {
        if (!this.current) return;
        this.image.width = this.current.width;
        this.image.height = this.current.height;
        this.setPos(this.body);
        this.label.show("100%");
    }

    imageViewer.scroll = function(lr) {
        this.scrollFlag = lr;
        if (this.scrollTimer == null) {
            _scroll();
            this.scrollTimer = new timer(400, _scroll);
            this.scrollTimer.start();
        }
    }
    imageViewer.stopScroll = function() {
        if (this.scrollTimer) { this.scrollTimer.stop(); this.scrollTimer = null; }
    }

    function _scroll() {
        var ch = 92;
        var lr = _IV.scrollFlag;
        var l = parseInt(_IV.list.style.left);
        var w = _IV.list.offsetWidth;
        var sw = _IV.listScroll.offsetWidth;
        if (w < sw) return;
        if (lr == -1) {
            if (l < 0) l += ch;
            if (l > 0) l = 0;
        }
        else if (lr == 1) {
            if (0 - l < w - sw) l -= ch;
            if (0 - l > w - sw) l = sw - w;
        }

        _IV.list.style.left = l + "px";
    }

    imageViewer.setPosition = function (f) {

        var mr = max.global.getBrowserRect();
        var tBar = _IV.toolbar;
        var th = tBar.offsetHeight;
        mr.height -= th;
        mr.bottom -= th;
        _IV.mainArea = mr;
        var s = tBar.style;
        s.top = mr.bottom + "px";
        s.left = mr.left + "px";
        s.width = mr.width + "px";

        tBar = _IV.topbar;
        s = tBar.style;
        s.top = mr.top + "px";
        s.width = "250px"; // mr.width + "px";
        s.left = "auto"; // mr.left + "px";
        s.right = 0;
        _IV.listScroll.style.width = (mr.width * 0.90) + "px";
        if (!f && _IV.isOpen) {
            _IV.setPos(_IV.body);
        }

        if (_IV.isOpen) {
            var divMarker = _IV.masker;
            setStyle(divMarker, {
                left: mr.left + "px",
                top: mr.top + "px",
                width: mr.width + "px",
                height: mr.height + "px"
            });
        }
    };

    imageViewer.isOpen = false;
    var sb = new stringBuilder();
    for (var i = 0; i < threadImageCollection.length; i++) {
        var ti = threadImageCollection[i];
        var src = ti.src;
        imageViewer.appendImageItem(ti);
    }

    imageViewer.next = function() {
        if (this.index < threadImageCollection.length - 1) {
            this.open(threadImageCollection[this.index + 1].src);
        }
        else if (this.index == threadImageCollection.length - 1) {
            this.open(threadImageCollection[0].src);
        }
    }

    imageViewer.previous = function() {
        if (this.index > 0) {
            this.open(threadImageCollection[this.index - 1].src);
        }
        else if (this.index == 0) {
            this.open(threadImageCollection[threadImageCollection.length - 1].src);
        }
    }

    imageViewer.setPos = function(obj) {
        var s = obj;
        var bh = this.mainArea.height;
        var bw = this.mainArea.width;
        var ih = s.offsetHeight;
        var iw = s.offsetWidth;
        s.style.left = (this.mainArea.left + (bw - iw) / 2) + "px";
        s.style.top = (this.mainArea.top + (bh - ih) / 2) + "px";
    }
    
    
//=============================================图片拖动代码============================
    imageViewer.ondrag = function(e) { endEvent(e); return false; }//不能拖拽图片
    imageViewer.picmousedown = function (e) {
        var l = getLeft(_IV.body), t = getTop(_IV.body);
        var ev = window.event || e; _IV.mouseStart = { x: e.clientX - l, y: e.clientY - t };
        _IV.isMoveImage = 1;
        var ht = _IV.image;
        if (max.browser.isIE) { ht.setCapture(); } else { window.captureEvents(Event.MOUSEMOVE | Event.MOUSEUP); }
    }

    imageViewer.bodymouseup = function (e) {
        e = window.event || e;
        var sender = e.srcElement || e.target;


        if (_IV.isOpen) {
            if (sender == _IV.masker) {
                _IV.close();
            }
        }

        var ht = _IV.image;
        _IV.mouseStart = null;
        _IV.isMoveImage = 0;
        if (max.browser.isIE) { ht.releaseCapture(); } else { window.releaseEvents(Event.MOUSEMOVE | Event.MOUSEUP); }
    }
    imageViewer.picmousemove = function(e) {
    if(! _IV.isMoveImage) return ;
        var ev = window.event || e;
        if (_IV.mouseStart) {
            setStyle(_IV.body, { left: (ev.clientX - _IV.mouseStart.x) + "px", top: (ev.clientY - _IV.mouseStart.y) + "px" });
        }
    }
//=====================================================================================


    imageViewer.close = function () {
        setVisible(this.body, 0);
        setVisible(this.toolbar, 0);
        setVisible(this.topbar, 0);
        this.isOpen = false;
        removeElement(this.masker);
        this.masker = null;
        var bBody = max.browser.isIE ? document.documentElement : document.body;
        bBody.style.overflow = "auto";
        document.body.onmousewheel = function () { return true; };

        removeHandler(this.image, "mousedown", _IV.picmousedown);
        removeHandler(document, "mouseup", _IV.bodymouseup);
        removeHandler(document, "mousemove", _IV.picmousemove);
        removeHandler(document.body, "resize", _IV.setPosition);

        this.label.hide();
    }

    imageViewer.zoom = function (z) {
        setStyle(this.image, { visibility: "hidden" });

        var h, w, nw, nh, ch, cw;
        h = this.image.height;
        w = this.image.width;
        ch = this.current.height * 0.05;
        cw = this.current.width * 0.05;
        var l, t;
        if (z < 0) { ch = 0 - ch; cw = 0 - cw; }
        nh = h + ch; nw = w + cw;
        l = getLeft(this.body);
        t = getTop(this.body);
        imageViewer.close = function () {
            setVisible(this.body, 0);
            setVisible(this.toolbar, 0);
            setVisible(this.topbar, 0);
            this.isOpen = false;
            removeElement(this.masker);
            this.masker = null;
            var bBody = max.browser.isIE ? document.documentElement : document.body;
            bBody.style.overflow = "auto";
            document.body.onmousewheel = function () { return true; };
            removeHandler(document.body, "resize", _IV.setPosition);
            if (this.body) removeElement(this.body);
            this.label.hide();
        }
        t += (h - nh) / 2;
        l += (w - nw) / 2;

        if (nw < 30)
            return;
        var persend = Math.floor(100 * (nw / this.current.width)) + "%";
        this.image.width = nw;
        this.image.height = nh;
        this.body.style.left = l + "px";
        this.body.style.top = t + "px";
        var label = this.label;

        label.show(persend);
        setStyle(this.image, { visibility: "visible" });
        //this.setPos();
    }

    var showHide = function(obj, show, callback) {
        var ie = max.browser.isIE;
        if (ie) {
            if (show) {
                obj.style.visibility = "hidden";
                obj.style.filter = "BlendTrans(duration=.80);";

            }
            obj.filters[0].Apply();
            obj.style.visibility = show ? "visible" : "hidden";
            obj.filters[0].play();
            if (callback) callback();
            return;
        }
        var ts = 4;

        var t = new timer(1, function(i) {
            if (show > 0)
                opacity(obj, i * (100 / ts));
            else
                opacity(obj, (ts - i) * (100 / ts));

            if (i >= ts) {
                if (callback) callback.call(_IV); t.stop(); //break;
            }

        });
        t.start();
    }




    imageViewer.open = function (img) {
        if (this.current && this.current.src == img && this.isOpen) return;
        var ie = max.browser.isIE;
        //初始化
        if (!this.isOpen) {
            var divMarker = addElement("div");
            var brect = max.global.getBrowserRect();
            setStyle(divMarker, {
                position: "absolute",
                left: brect.left + "px",
                top: brect.top + "px",
                width: (brect.width +30)+ "px",
                height: brect.height + "px",
                backgroundColor:"black",
                zIndex:49
            });
            opacity(divMarker, 30);
            this.masker = divMarker; // new background();
            document.body.onmousewheel = function () { return false; };
            setVisible(this.toolbar, 1);
            setVisible(this.topbar, 1);
            var bBody = max.browser.isIE ? document.documentElement : document.body;
            bBody.style.overflow = "hidden";
            addHandler(window, "resize", _IV.setPosition);
            this.setPosition();
        }
        //选中
        window.setTimeout(function () {
            var cn = _IV.listTable.childNodes;
            for (var j = 0; j < cn.length; j++) {
                var _o = cn[j];
                if (_o.childNodes[0].src == img) addCssClass(_o, "current");
                else
                    removeCssClass(_o, "current");
            }
        }, 10);

        var ob = this.body;
        var oi = this.image;
        if (ob) {
            var z = ob.style.zIndex; z--; ob.style.zIndex = z; showHide(ob, 0, function () {
                if (oi) {
                    removeHandler(oi, "mousedown", _IV.picmousedown);
                    removeHandler(document, "mouseup", _IV.bodymouseup);
                    removeHandler(document, "mousemove", _IV.picmousemove);
                    removeHandler(oi, "dragstart", _IV.ondrag);
                    oi = null;
                }
                removeElement(ob);
                ob = null;
                _IV_C.call(_IV);
            });

        }
        else
            _IV_C.call(_IV);
        function _IV_C() {
            this.body = null;
            this.image = null;
            this.body = addElement("div");
            var isFromCache = 0;
            var ni;
            if (this.objectPool[img]) {
                ni = this.objectPool[img];
                this.body.appendChild(ni);
                isFromCache = 1;
            }
            else {
                ni = addElement("img", this.body);
                this.objectPool[img] = ni;
            }

            setStyle(this.body, { position: "absolute", padding: "5px", zIndex: 990, backgroundColor: "White" });

            if (!ie) opacity(this.body, 0);
            var w, h;
            for (var i = 0; i < threadImageCollection.length; i++) {
                var ti = threadImageCollection[i];
                var src = ti.src;
                if (img == src) {
                    h = ti.height;
                    w = ti.width;
                    this.index = i;
                }
            }
            this.current = threadImageCollection[this.index];

            var bh = this.mainArea.height;
            var bw = this.mainArea.width;
            var iw, ih, scale;
            var bestScale = 0.90;
            scale = w / h;
            if (w > bw * bestScale) {
                iw = bw * bestScale;
                ih = iw / scale;
            }
            else {
                iw = w;
                ih = h;
            }

            if (ih > bh * bestScale) { ih = bh * bestScale; iw = ih * scale; }


            this.body.onmousewheel = this.label.object.onmousewheel = function (e) {
                var delta = 0;
                if (!e) e = window.event;
                if (e.wheelDelta) {
                    delta = event.wheelDelta / 120;
                    if (window.opera) delta = -delta;
                } else if (e.detail) {
                    delta = -e.detail / 3;
                }
                if (delta) _IV.zoom(delta);
                return false;
            };

            ni.width = iw;
            ni.height = ih;
            if (!isFromCache) ni.src = img;
            this.image = ni;
            this.setPos(this.body);
            showHide(this.body, 1);
            addHandler(ni, "mousedown", _IV.picmousedown);
            addHandler(document, "mouseup", _IV.bodymouseup);
            addHandler(document, "mousemove", _IV.picmousemove);
            addHandler(ni, "dragstart", _IV.ondrag);
            if (!this.isOpen) {
                this.isOpen = true;
            }

            this.label.hide();
        }
    }

    return imageViewer;
}

//=============================== 运行CODE里面的代码
function ubbRun(th) { var txt = th.parentNode.parentNode.getElementsByTagName('code')[0].innerHTML; var win = window.open('about:blank'); txt = txt.replace(/&lt;/g, '<'); txt = txt.replace(/&gt;/g, '>'); txt = txt.replace(/&amp;/g, '&'); win.document.open(); win.document.write(txt); win.document.close(); }
function ubbCopy(th) { var txt = th.parentNode.parentNode.getElementsByTagName('code')[0].innerHTML; txt = txt.replace(/&lt;/g, '<'); txt = txt.replace(/&gt;/g, '>'); txt = txt.replace(/&nbsp;/g, ' '); txt = txt.replace(/&amp;/g, '&'); copyToClipboard(txt); }
//===============================================================================================