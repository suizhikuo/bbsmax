/*
copyright:bbs.bbsmax.com
by wenquan wenquan@bbsmax.com
*/

//浏览器屏蔽层
var background = function() {
    var autoSize = function() {
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
        destroy: function() {
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

function setButtonDisable(btnID, disable,a) {
    var btn = $(btnID);
    if (!btn) return;
    if (!disable && arguments.length == 2) {
        window.setTimeout("setButtonDisable('" + btnID + "',false,0);", 40);
        return;
    }
    
    btn.disabled = disable;

    var bc = btn.parentNode.parentNode;
    var ac = "";
    if (bc.className.indexOf("minbtn-wrap") > -1) {
        ac = "minbtn-disable";
    }
    else if (bc.className.indexOf("btn-wrap") > -1) {
        ac = "btn-disable";
    }
    if (ac) {
        if (!disable)
            removeCssClass(bc, ac);
        else
            addCssClass(bc, ac);
    }
}

/*----------------------------------------------------------*/

//用户中心的那些下拉菜单
//listid:下拉菜单的那个容器ID
//triggerid:触发下拉菜单弹出的那个对象的ID
//autoPop:如果是true那么只要鼠标滑过就弹出， 否则需要点击才弹出
var dropdown = function(listid, triggerid, autoPop) {
    popupBase.call(this, listid, triggerid, autoPop);
    var _this = this;
    for (var i = 0; i < this.triggers.length; i++) {
        var s = this.triggers[i];
        addHandler(s, this.auto ? 'mouseover' : 'click', function() { _this.show(); });
        addHandler(document.documentElement, 'click', function() { _this.hide(); });
    }
    maxPopupCollection.instance().add(this);
}

dropdown.prototype.show=function(){var s=this.list.style;s.display='';if(this.onShow)this.onShow(this);}
dropdown.prototype.hide=function(){var s=this.list.style;s.display='none';}

function clickButton(name,formid)
{
    var hidden;
    var form = formid ? $(formid) : document.forms[0];
    hidden = addElement("input", form);
    hidden.style.display='none';
    hidden.name=name;
    hidden.value="1";
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
        setTimeout(function() { ann_scroll(); }, st);
    }

    ann_obj.onmouseover = function() { ann_run = 0; };
    ann_obj.onmouseout = function () { ann_run = 1; };

    addHandler(window, "load", function () {
        ann_itemHeight += ann_li[0].offsetHeight;
        setStyle(ann_obj, { height: ann_itemHeight + "px", overflow: "hidden" });
        ann_obj.innerHTML += ann_ognHTML;
        ann_scroll();
    });
}

var swfupload_loaded = false;
//将指定的对象初始化为上传按钮
function initUploader(obj) {
    if (!obj) return;

    loadScript(root + '/max-assets/swfupload/swfupload.js', null, function() {
    loadScript(root + '/max-assets/javascript/swfupload_handler.js', null, function() {
            swfupload_loaded = true;
        });
    });
}


//编辑器工具栏按钮列表
var editorToolBar={
        full: ['save', 'bold', 'italic', 'underline', 'left', 'center', 'right', 'justify', 'ol', 'ul', 'fontSize', 'fontFamily', 'indent', 'outdent', 'image', 'upload', 'link', 'unlink', 'forecolor', 'bgcolor', 'removeformat', 'xhtml'],
       simple : ['bold','italic','underline','forecolor','bgcolor'],
       normal: ['bold', 'italic', 'underline', 'left', 'center', 'right', 'justify', 'ol', 'ul', 'fontSize', 'fontFamily', 'indent', 'outdent', 'image', 'link', 'unlink', 'forecolor', 'bgcolor', 'removeformat', 'xhtml'],
       setting: ['bold', 'italic', 'underline', 'fontSize', 'fontFamily', 'image', 'link', 'unlink', 'forecolor', 'bgcolor', 'removeformat', 'xhtml']
}

//传入textarea的name属性，自动把这个textarea升级为可视化编辑器
//name    : textarea 的name 
//buttons : 是一个字符串数组， 表示当前要用到哪些按钮。可以用editorToolBar 对象下的四个枚举值
//autoSize: Boolean 表示编辑器是不是自动改变大小

//如果name为空的话， 会自动遍历页面上的textarea 并替换成编辑器
//如果buttons为空的话,默认就是全部按钮都启用

function initMiniEditor(name, buttons, autoSize) {
    var editor = null;
    var editortParams = new Object();
    if (typeof (name) == "string") {
        if (autoSize != true) editortParams.maxHeight = max.coor.height($$(name)[0]);
    }

    editortParams.buttonList = buttons || editorToolBar.full;
    editortParams.iconsPath = root + '/max-assets/nicedit/nicEditorIcons.gif';

    if (typeof (name) == "string") {
        var e = $$(name)[0];
        if (e) {
            //new nicEditor(editortParams).panelInstance(e);
            editor = new nicEditor(editortParams).panelInstance(e).nicInstances[0];

        }
    }
    else {
        nicEditors.allTextAreas(editortParams);
    }
    return editor;
}

function initEditor(config){
    KE.init(config);
    KE.create(config.id);
}

//------------------------------------------------------------

//页面元素 关联Radio显示与隐藏
function initDisplay(formName, args) {
var v='';
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

var pageEndEvents = [];
var pageLoadComplete = 0;
function addPageEndEvent(func) {
    pageEndEvents.push(func);
}

function page_end() {
    if (!pageLoadComplete) {

        for (var i in pageEndEvents) {
            pageEndEvents[i] && pageEndEvents[i]();
        }
        if (isExecuteJobTime) {
            ajaxRequest(jobUrl);
            isExecuteJobTime = false;
        }

        var buttons = document.getElementsByTagName("input");
        for (var i = 0; i < buttons.length; i++) {
            var b = buttons[i];
            if (b.type != "submit" || !b.name) continue;
            var id = b.id;
            if (!id) {
                id = "max_unnamedsubmit_" + i; b.id = id;
            }
            addHandler(b, "click", new Function('window.setTimeout(function(){ setButtonDisable("' + id + '",true)},20)'));
        }
    }
    pageLoadComplete = 1;
}

function showAlert(message){
    alert(message);
}

function showSuccess(message){
    alert(message);
}


ajaxCallback = function(result) {
    if (result != null) {
        if (result.iswarning)
            showAlert(result.message);
        else if (result.issuccess)
            showSuccess(result.message);
    }
}

ajaxCallbackLocationError = function(result) {
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

function maxConfirm(title, message, func, callback){
    if(confirm(message)){
        func();
        return true;
    }
    else{
        return false;
    }
}



function showVCode(inputObj, imgUrl) {
    var _vd = inputObj.pop;
    if (!inputObj.pop) {
        inputObj.value = "";
        inputObj.style.color = "black";
        var _vd = addElement("div");
        setStyle(_vd, { border: "solid 1px #ccc", backgroundColor: "white", height: "25px" });
        var _a = addElement("a", _vd);
        _a.href = "javascript:;";
        _a.onclick = function () { sp.nodeValue = " 载入... "; var vimg = this.childNodes[1]; var src = attachQuery(vimg.src, 'rnd', new Date().getTime()); removeElement(vimg); vimg = addElement("img", this); setVisible(vimg, 0); vimg.onload = imgLoad; vimg.src = src; };
        var sp = document.createTextNode(" 载入... ");  
        _a.appendChild(sp);
        var img = addElement("img", _a);
        setVisible(img, 0);
        img.border = 0;
        var p = new popup(_vd, inputObj, false, null, "top");
        p.createBack = 0;
        p.show();
        inputObj.pop = p;
        function imgLoad() { var n = this; sp.nodeValue = ""; setVisible(n, 1); var pn = n.parentNode.parentNode; pn.style.height = 'auto'; inputObj.pop.show({ target: inputObj }); };
        img.onload=imgLoad;
        img.src = attachQuery(imgUrl, "rnd", new Date().getTime());
    }
    else {
        inputObj.pop.show();
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

function ImageError(th) { th.error = true; th.onload = null; th.onerror = null; th.src = BBSMAX.ImageDefault; th.style.width = 'auto'; th.style.height = 'auto'; th.style.background = 'url()'; }

var BBSMAX = { AvatarDefault: root + '/max-assets/avatar/avatar_120.gif', ImageDefault: root + '/max-assets/images/notfound.gif', CodeUrl: 'validatecode.aspx?type=', CodeLoadingImage: root + '/max-assets/images/loading.gif', MessageSoundPath: root + '/max-assets/sound-msg/', SpaceImage: root + '/max-assets/images/blank.gif', NetExistsExt: /^(accdb|as|asm|aspx|avi|bmp|c|cfc|chm|cpp|cs|css|db|default|dll|doc|docx|eip|fla|flv|gif|h|html|jpg|js|mdb|mp3|mpeg|mpg|msi|pdf|php|png|ppt|pptx|psd|rar|sln|swf|tif|txt|vb|vs|wav|wma|wmv|xls|xlsx|xml|zip)$/i, NetExistsWidth: 48, NetExistsHeight: 48, NetDefaultWidth: 70, NetDefaultHeight: 70, ImagePreviewBg: '/max-assets/images/imagesPreview_percent_bg.gif' }


function showImage(e) {
    var IV = window._IV;

    if (!IV) {
        if (!window.createImageViewer) return;
        IV = createImageViewer();
        window._IV = IV;
    }

    e = e || window.event;
    var img = e.target || e.srcElement;
    IV.open(img.src);
}

function ImageLoaded(img) {
   
    addHandler(img, "click", showImage);

    img.processed = 1;
    var w = null;
    var n = img.parentNode;
    //var rt1 = max.global.getBrowserRect();
    var rt2 = getRect(n);
    //        while (rt2.width < rt1.width * 0.5) {
    //            n = n.parentNode;
    //            rt2 = getRect(n);
    //        }
    w = rt2.width * 0.95;

    if (window.threadImageCollection) {
        for (var i = 0; i < threadImageCollection.length; i++) {
            var t = threadImageCollection[i];
            if (img.src == t.src) {
                t.width = img.width;
                t.height = img.height;
                break;
            }
        }
    }

    if (img.width > w) {
        imageScale(img, w);
    }
}



//打开指定的对话框，如果打开成功，返回false，否则返回true。
//这样做的目的是：方便进行nojavascript兼容
//例如：<a href="对话框页.html" onclick="return openDialog()">打开对话框</a>
//这样写就可以让脚本正常的情况下打开对话框，脚本不正常的情况下以普通链接的形式打开
function openDialog() {

    var args = arguments;
    if (args.length < 1) return true;
    
    var settings;
    
    if (typeof args[0] == 'string'){

        settings = { src : args[0] };
        
        if( args.length == 2 )
        {
            if (typeof args[1] == 'function')
                settings.return_handler = args[1];
            else if(typeof args[1] == 'object')
                settings.trigger = args[1];
        }
        else if (args.length == 3){
            if (typeof args[1] == 'function')
            {
               settings.return_handler = args[1];
               settings.trigger = args[2];
            }
            else
            {
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

    var formid, url, callback,form;
    formid = param.formId;
    url = param.url;
    callback = param.callback;

    if (formid)
        form = $(formid);
    else {
        form = document.forms[0];
    }
    
    if(!form.id)
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
        for(var i = 0; i < ss.length; i ++) {
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
        s.charset = charset; }
    
    if (callback) {
       if (max.browser.isIE) {
            s.onreadystatechange = function () {
                if ('complete' == s.readyState||s.readyState=="loaded") {
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

var maxPanelCore = function() { }
maxPanelCore.prototype.resize = function(w, h) {
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
            setStyle(f, { left: (rect.left+5) + "px", top: (rect.top+5) + "px", width: (rect.width-10) + "px", height: (rect.height-10) + "px" });
        }
    }
}

maxPanelCore.prototype.submit = function(btn) {
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

                if (!input.id) input.id = String.format("max_submit_{0}", new Date().getTime()); //由于事件源每个浏览器居然不一样， 只好把SUBMIT的ID替换掉，才有办法找到这个按钮。 真是去他娘的

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

maxPanelCore.prototype.waiting = function() {
    this.setContent(String.format('<div class="dialogloader"><span>正在载入...</span></div>', max.consts["loading16"]));
    this.hasContent = 0;
}

maxPanelCore.prototype.loadPage = function(url) {
    var th = this;
    ajaxRequest(url || this.url, function(r) { th.setContent(r); });
    return false;
}

maxPanelCore.prototype.postToPage = function(url, datas) {
    var th = this;
    ajaxPostData(url||this.url, datas, function(as) { th.setContent(as) });
}

maxPanelCore.prototype.close = function () {
    if (window.panel == this) window.panel = null;
    if (max.browser.isIE6) {
        if (this.panel.frame)
        try{
            removeElement(this.panel.frame);
            }catch(e){}
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

maxPanelCore.prototype.show = function() {
    this.panel.style.display = "";
}

///关闭对话框时需要调用的事件处理函数
maxPanelCore.prototype.addCloseHandler = function (f) {
    if (!this.closeHandlers)
        this.closeHandlers = [];
    this.closeHandlers.push(f);
}

maxPanelCore.prototype.focus = function() {
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
        panel.onclick = function (e) {window.isClickDialog=1; };
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
    var pid = "mx_dialog_" +maxUnique;

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

function openPanel(url, trigger, cssClass, w, h, position,callback) {

    var id = "max_panel_" + url.getHashCode();

    var panelObject = new maxPanel({ id: id, w: w, h: h, trigger: trigger,
        position: position,
        cssClass: cssClass,
        innerCssClass: "dropdownmenu",
        bodyCssClass: "clearfix dropdownmenu-inner"
    });
    panelObject.closeCallback = callback;
    var body = panelObject.panel.body;
    var fh =String.format( '<iframe height="{1}" width="{0}" frameborder="0" scrolling="no"></iframe>',w-4,h-4);
    body.innerHTML =  fh;
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
    var fixedContainer=null;
    if (trigger) {
        var ot = 0,ol=0;
        ///====================滚动内框顶坐标的修正 
        var pn= trigger.parentNode;
        while (pn) {
            if (!ot && pn.className && pn.className.indexOf("scroller") > -1) {
                ot =0 - pn.scrollTop;
                if(ie6) break;
            }

            if (pn.nodeName.indexOf("document") != -1)
                break;

            var pnid = pn.getAttribute("id");

            if (!ie6 && pnid && pnid.indexOf("mx_dialog_") > -1) {
                  fixedContainer =pn;
                  break;
            }
            pn  = pn.parentNode
        }
        var _p = "absolute";
        if(fixedContainer){_p = "fixed";}

        panelObject.panel.style.position = _p;

        //====================================== end
        
        showPopup(panelObject.panel, trigger, position,0,ot);
    }
    else {
        moveToCenter(panelObject.panel);
    }
    frame.src = url;

    window.setTimeout(function() { panelObject.focus(); }, 20);
    
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
                if (window. isClickDialog) return;
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
function openAjaxLayer(url, trigger, cssClass, w, h, position,event) {

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
var DynamicTable = function(tableid, keyname) {
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

DynamicTable.prototype.insertRow = function(callback) {
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
    addHandler($(String.format(this.deleteTrigger, this.maxKey)), "click", function() { _this.deleteRow(newrow) });
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

        window.setTimeout(function() {
            var dp = openPanel(datePickerUrl, e, '', 310, 190, "auto", function(r) {
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

        window.setTimeout(function() {
            window.colorPanel = openPanel(colorBoardUrl, e, '', 240, 240, "auto", function(r) {
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