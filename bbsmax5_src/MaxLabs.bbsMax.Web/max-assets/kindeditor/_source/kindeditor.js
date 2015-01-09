/******************************************************************************
* KindEditor - WYSIWYG HTML Editor for Internet
*
* @author Roddy <luolonghao@gmail.com>
* @site http://www.kindsoft.net/
* @licence LGPL(http://www.opensource.org/licenses/lgpl-license.php)
* @version 3.1.2
* @updated by wenquan@bbsmax.com ,at 2010-08-13
******************************************************************************/

var editorMode = { html: 'html', ubb: 'ubb' };

var KE = {};

KE.lang = {
    source: '切换模式',
    preview: '预览',
    zoom: '放大',
    undo: '后退',
    redo: '前进',
    cut: '剪切',
    copy: '复制',
    paste: '粘贴',
    plainpaste: '粘贴为无格式文本',
    wordpaste: '从Word粘贴',
    selectall: '全选',
    justifyleft: '左对齐',
    justifycenter: '居中',
    justifyright: '右对齐',
    justifyfull: '两端对齐',
    insertorderedlist: '编号',
    insertunorderedlist: '项目符号',
    indent: '增加缩进',
    outdent: '减少缩进',
    subscript: '下标',
    superscript: '上标',
    date: '插入当前日期',
    time: '插入当前时间',
    fontname: '字体',
    fontsize: '文字大小',
    textcolor: '文字颜色',
    bgcolor: '文字背景',
    bold: '粗体',
    italic: '斜体',
    underline: '下划线',
    strikethrough: '删除线',
    removeformat: '删除格式',
    image: '插入图片',
    flash: '插入Flash',
    media: '插入多媒体',
    audio: '音频',
    video: '视频',
    layer: '插入层',
    table: '插入表格',
    emoticons: '表情',
    photo: '图片',
    attachment: "附件",
    link: '超级连接',
    unlink: '取消超级连接',
    code: '插入代码',
    hide: '插入隐藏内容',
    fullscreen: '全屏显示',
    yes: '确定',
    no: '取消',
    close: '关闭',
    quote: '插入引用',
    free: '插入免费内容',
    fontTable: {
        'SimSun': '宋体',
        'SimHei': '黑体',
        'FangSong_GB2312': '仿宋体',
        'KaiTi_GB2312': '楷体',
        'NSimSun': '新宋体',
        'Arial': 'Arial',
        'Arial Black': 'Arial Black',
        'Times New Roman': 'Times New Roman',
        'Courier New': 'Courier New',
        'Tahoma': 'Tahoma',
        'Verdana': 'Verdana'
    },
    colorTable: [
        ["#FFFFFF", "#E5E4E4", "#D9D8D8", "#C0BDBD", "#A7A4A4", "#8E8A8B", "#827E7F", "#767173", "#5C585A", "#000000"],
        ["#FEFCDF", "#FEF4C4", "#FEED9B", "#FEE573", "#FFED43", "#F6CC0B", "#E0B800", "#C9A601", "#AD8E00", "#8C7301"],
        ["#FFDED3", "#FFC4B0", "#FF9D7D", "#FF7A4E", "#FF6600", "#E95D00", "#D15502", "#BA4B01", "#A44201", "#8D3901"],
        ["#FFD2D0", "#FFBAB7", "#FE9A95", "#FF7A73", "#FF483F", "#FE2419", "#F10B00", "#D40A00", "#940000", "#6D201B"],
        ["#FFDAED", "#FFB7DC", "#FFA1D1", "#FF84C3", "#FF57AC", "#FD1289", "#EC0078", "#D6006D", "#BB005F", "#9B014F"],
        ["#FCD6FE", "#FBBCFF", "#F9A1FE", "#F784FE", "#F564FE", "#F546FF", "#F328FF", "#D801E5", "#C001CB", "#8F0197"],
        ["#E2F0FE", "#C7E2FE", "#ADD5FE", "#92C7FE", "#6EB5FF", "#48A2FF", "#2690FE", "#0162F4", "#013ADD", "#0021B0"],
        ["#D3FDFF", "#ACFAFD", "#7CFAFF", "#4AF7FE", "#1DE6FE", "#01DEFF", "#00CDEC", "#01B6DE", "#00A0C2", "#0084A0"],
        ["#EDFFCF", "#DFFEAA", "#D1FD88", "#BEFA5A", "#A8F32A", "#8FD80A", "#79C101", "#3FA701", "#307F00", "#156200"],
        ["#D4C89F", "#DAAD88", "#C49578", "#C2877E", "#AC8295", "#C0A5C4", "#969AC2", "#92B7D7", "#80ADAF", "#9CA53B"]
    ],
    invalidSwf: "请输入有效的URL地址。\n只允许swf,flv格式。",
    invalidImg: "请输入有效的URL地址。\n只允许jpg,gif,bmp,png格式。",
    invalidMedia: "请输入有效的URL地址。\n只允许mp3,wav,wma,wmv,mid,avi,mpg,asf,rm,rmvb,swf,flv格式。",
    invalidWidth: "宽度必须为数字。",
    invalidHeight: "高度必须为数字。",
    invalidBorder: "边框必须为数字。",
    invalidUrl: "URL不正确。",
    pleaseInput: "请输入内容",
    shrink: "减少高度",
    elongate: "增加高度"
};

KE.$ = function(id, doc) {
    var doc = doc || document;
    return doc.getElementById(id);

};
KE.$$ = function(name, doc) {
    var doc = doc || document;
    return doc.createElement(name);
};
KE.event = {
    add: function(el, event, listener) {
        if (el.addEventListener) {
            el.addEventListener(event, listener, false);
        } else if (el.attachEvent) {
            el.attachEvent('on' + event, listener);
        }
    },
    remove: function(el, event, listener) {
        if (el.removeEventListener) {
            el.removeEventListener(event, listener, false);
        } else if (el.detachEvent) {
            el.detachEvent('on' + event, listener);
        }
    }
};
KE.each = function(obj, func) {
    for (var key in obj) {
        if (obj.hasOwnProperty(key)) func(key, obj[key]);
    }
};


KE.util = {
    getDocumentElement: function () {
        return (document.compatMode != "CSS1Compat") ? document.body : document.documentElement;
    },
    getDocumentHeight: function () {
        var el = this.getDocumentElement();
        return Math.max(el.scrollHeight, el.clientHeight);
    },
    getDocumentWidth: function () {
        var el = this.getDocumentElement();
        return Math.max(el.scrollWidth, el.clientWidth);
    },
    getScriptPath: function () {
        var elements = document.getElementsByTagName('script');
        for (var i = 0; i < elements.length; i++) {
            if (elements[i].src && elements[i].src.match(/kindeditor[\w\-\.]*\.js/) != null) {
                return elements[i].src.substring(0, elements[i].src.lastIndexOf('/') + 1);
            }
        }
    },
    getHtmlPath: function () {
        return location.href.substring(0, location.href.lastIndexOf('/') + 1);
    },
    getBrowser: function () {
        var ua = navigator.userAgent.toLowerCase();
        var obj = {};
        obj.IsIE = ua.indexOf("msie") > -1;
        obj.IsIE6 = ua.indexOf("msie 6") > -1;
        obj.IsIE7 = ua.indexOf("msie 7") > -1;
        obj.IsFF = ua.indexOf("firefox") > -1;
        obj.IsOpera = ua.indexOf("opera") > -1;
        obj.IsGecko = ua.indexOf("gecko") > -1;
        obj.IsSafari = ua.indexOf("safari") > -1;
        obj.IsChrome = ua.indexOf("chrome") > -1;
        return obj;
    },
    loadStyle: function (path) {
        var link = KE.$$('link');
        link.setAttribute('type', 'text/css');
        link.setAttribute('rel', 'stylesheet');
        link.setAttribute('href', path);
        document.getElementsByTagName("head")[0].appendChild(link);
    },
    inArray: function (str, arr) {
        for (var i = 0; i < arr.length; i++) { if (str == arr[i]) return true; }
        return false;
    },
    escape: function (html) {
        html = html.replace(/&/g, "&amp;");
        html = html.replace(/</g, "&lt;");
        html = html.replace(/>/g, "&gt;");
        html = html.replace(/\xA0/g, "&nbsp;");
        html = html.replace(/\x20/g, " ");
        return html;
    },
    createLink: function (id, url) {
        KE.util.select(id);
        var iframeDoc = KE.g[id].iframeDoc;
        var range = KE.g[id].range;
        var url = url;
        var target = "_blank";
        if (url.match(/\w+:\/\/.{3,}/) == null) {
            alert(KE.lang['invalidUrl']);
            window.focus();
            return false;
        }
        var node;
        if (KE.browser.IsIE) {
            if (range.item) {
                node = range.item(0).parentNode
            }
            else {
                iframeDoc.body;
            }
        } else {
            if (range.startContainer == range.endContainer) {
                node = range.startContainer.parentNode
            }
            else {
                iframeDoc.body;
            }
        }

        var html = '<a href="' + url + '" target="_blank">' + url + '</a>';
        var ubb = '[url=' + url + ']' + url + '[/url]';
        var insertMode = false;                                                //add by wenquan 选中文本的标记变量
        insertMode = range.collapsed || (KE.browser.IsIE && !range.htmlText);  //add by wenquan 处理未选中文本的情况
        if (insertMode) { KE.util.insertContent(id, html, ubb); return; }      //add by wenquan 处理未选中文本的情况

        if (node && node.tagName == 'A') node = node.parentNode;
        if (!node) node = iframeDoc.body;

        iframeDoc.execCommand("createlink", false, "__ke_temp_url__");
        var arr = node.getElementsByTagName('a');
        for (var i = 0, l = arr.length; i < l; i++) {
            if (arr[i].href.match(/\/?__ke_temp_url__$/) != null) {
                arr[i].href = url;
                if (target) arr[i].target = target;
            }
        }
        KE.history.add(id);
        KE.layout.hide(id);
        KE.util.focus(id);
    },

    getElementPos: function (el) {
        var x = 0;
        var y = 0;
        if (el.getBoundingClientRect) {
            var box = el.getBoundingClientRect();
            var el = this.getDocumentElement();
            x = box.left + el.scrollLeft - el.clientLeft;
            y = box.top + el.scrollTop - el.clientTop;
        } else {
            x = el.offsetLeft;
            y = el.offsetTop;
            var parent = el.offsetParent;
            while (parent) {
                x += parent.offsetLeft;
                y += parent.offsetTop;
                parent = parent.offsetParent;
            }
        }
        return { 'x': x, 'y': y };
    },
    getCoords: function (ev) {
        ev = ev || window.event;
        var el = this.getDocumentElement();
        if (ev.pageX) return { x: ev.pageX, y: ev.pageY };
        return {
            x: ev.clientX + el.scrollLeft - el.clientLeft,
            y: ev.clientY + el.scrollTop - el.clientTop
        };
    },
    setOpacity: function (el, opacity) {
        if (KE.browser.IsIE) {
            el.style.filter = (opacity == 100) ? "" : "gray() alpha(opacity=" + opacity + ")";
        } else {
            el.style.opacity = (opacity == 100) ? "" : "0." + opacity.toString();
        }
    },
    showBottom: function (id) {
        // KE.g[id].bottom.style.display = 'block';
    },
    hideBottom: function (id) {
        // KE.g[id].bottom.style.display = 'none';
    },
    drag: function (id, mousedownObj, moveObj, func) {
        var obj = KE.g[id];
        mousedownObj.onmousedown = function (event) {

            if (!KE.browser.IsIE) event.preventDefault();
            var ev = event || window.event;
            var pos = KE.util.getCoords(ev);
            var objTop = parseInt(moveObj.style.top);
            var objLeft = parseInt(moveObj.style.left);
            var objWidth = parseInt(moveObj.style.width);
            var objHeight = parseInt(moveObj.style.height);
            var mouseTop = pos.y;
            var mouseLeft = pos.x;
            var dragFlag = true;
            var moveListener = function (event) {
                if (dragFlag) {
                    var ev = event || window.event;
                    var pos = KE.util.getCoords(ev);
                    var top = pos.y - mouseTop;
                    var left = pos.x - mouseLeft;
                    func(objTop, objLeft, objWidth, objHeight, top, left);
                }
                return false;
            };
            var upListener = function (event) {
                if (obj.wyswygMode) {
                    obj.iframe.style.display = '';
                }
                dragFlag = false;
                KE.event.remove(document, 'mousemove', moveListener);
                KE.event.remove(document, 'mouseup', upListener);
            };
            KE.event.add(document, 'mousemove', moveListener);
            KE.event.add(document, 'mouseup', upListener);
        };
    },
    setDefaultPlugin: function (id) {
        var items = [
            'cut', 'copy', 'paste', 'selectall',
            'justifyfull'
            , 'outdent',
            'removeformat', 'unlink'
        ];
        for (var i = 0; i < items.length; i++) {
            KE.plugin[items[i]] = {
                click: new Function('id', 'KE.util.execCommand(id, "' + items[i] + '", null);')
            };
        }
        //bbsmax begin
        var ubbItems = [
            ['justifyleft', 'align=left'], ['justifycenter', 'align=center'], ['justifyright', 'align=right'],
            ['indent', 'indent'], ['subscript', 'sub'], ['superscript', 'sup'],
            ['bold', 'b'], ['italic', 'i'], ['underline', 'u'], ['strikethrough', 's'], ['free', 'free'], ['quote', 'quote']
        ];
        for (var i = 0; i < ubbItems.length; i++) {
            KE.plugin[ubbItems[i][0]] = {
                click: new Function('id', 'KE.util.execCommand(id, "' + ubbItems[i][0] + '", null);'),
                ubbClick: new Function('id', 'KE.util.execUbb(id, "' + ubbItems[i][1] + '", null);')
            };
        }

        var panelItems = [
            ['flash', -1, -1],
            ['photo', -1, -1],
            ['audio', -1, -1],
            ['video', -1, -1],
            ['attachment', -1, -1],
            ["code", 350, 290],
            ['wordpaste', 350, 310],
            ['hide', 350, 310],
            ['plainpaste', 350, 310],
            ["link", 300, 130]
            ];

        for (var i = 0; i < panelItems.length; i++) {

            KE.plugin[panelItems[i][0]] = {
                click: new Function('id', 'KE.panel.show(id, "' + panelItems[i][0] + '", ' + panelItems[i][1] + ', ' + panelItems[i][2] + ');'),
                ubbClick: function (id) { this.click(id); }
            };
        }
        var activables = [
            'justifyleft', 'justifycenter', 'justifyright', 'justifyfull',
            'insertorderedlist', 'insertunorderedlist', 'indent', 'outdent', 'subscript', 'superscript',
            'bold', 'italic', 'underline', 'strikethrough', 'removeformat', 'unlink'
        ];
        KE.activable = activables;
        //bbsmax end
    },
    getIframeDoc: function (iframe) {
        var doc = null;
        if (iframe.contentDocument) {
            doc = iframe.contentDocument;
        } else {
            var win = iframe.contentWindow;
            doc = win.document;
        }
        return doc;
    },
    getFullHtml: function (id) {
        var html = '<html>';
        html += '<head>';
        html += '<base href="' + KE.htmlPath + '" />';
        html += '<title>editor</title>';
        if (KE.g[id].cssPath) {
            html += '<link href="' + KE.g[id].cssPath + '" rel="stylesheet" type="text/css" />';
        }
        html += '</head>';
        html += '<body>';
        html += '</body>';
        html += '</html>';
        return html;
    },
    resize: function (id, width, height) {
        var obj = KE.g[id];

        if (height <= obj.minHeight) return;

        var formDiv = obj.formDiv;
        var tv = obj.toolbarDiv;
        var bv = obj.bottomDiv;
        var s = obj.container.style;
        var hBody = height;

        hBody -= bv.offsetHeight;
        hBody -= tv.offsetHeight;
        hBody -= 10; // border width


        s.width = width.toString().indexOf("%") > -1 ? width : width + 'px';

        //s.height = "auto";// height + 'px';
        s = obj.formDiv.style;
        s.height = hBody + 'px';
        if (KE.browser.IsIE6 || KE.browser.IsIE7) obj.newTextarea.style.height = hBody + "px";

        var formBorder = obj.formDiv.offsetHeight - obj.formDiv.clientHeight;
    },

    //收缩
    shrink: function (id) {
        var obj = KE.g[id];
        if (obj.fullscreenMode) return;
        obj.minHeight = obj.minHeight || 120;
        var h = obj.container.offsetHeight; //parseInt(obj.height);
        if (h <= 100 || h <= obj.minHeight) return;
        var s = -80;
        h += s;
        if (h < obj.minHeight) {
            s += obj.minHeight - h;
            h = obj.minHeight;
        }

        var hBody = obj.wyswygMode ? obj.iframe.offsetHeight + s : obj.newTextarea.offsetHeight + s;
        var form = obj.formDiv;
        form.style.height = hBody + "px";
        if (KE.browser.IsIE6 || KE.browser.IsIE7) { var newTextarea = KE.$('ke_textarea_' + id); newTextarea.style.height = form.style.height; }

        //obj.formDiv.style.height = hBody + 'px';
        //        obj.iframe.style.height = hBody + 'px';
        //        obj.newTextarea.style.height = hBody + 'px';
        //        obj.container.style.height = h + 'px';
        //        obj.height = h + 'px';
    },
    //拉大
    elongate: function (id) {
        var s = 80;
        var obj = KE.g[id];
        if (obj.fullscreenMode) return;
        var h = parseInt(obj.height);
        h += s;
        //obj.container.style.height = h + 'px';
        var hBody = obj.wyswygMode ? obj.iframe.offsetHeight + s : obj.newTextarea.offsetHeight + s;
        var form = obj.formDiv;
        form.style.height = hBody + "px";
        if (KE.browser.IsIE6 || KE.browser.IsIE7) { var newTextarea = KE.$('ke_textarea_' + id); newTextarea.style.height = form.style.height; }
        //obj.formDiv.style.height = hBody + 'px';
        //obj.iframe.style.height = hBody + 'px';
        //obj.newTextarea.style.height = hBody + 'px';
        //obj.height = h + 'px';
    },

    setValue: function (id, value, format) {
        var isHtml = false;
        var isUbb = false;
        var m;
        m = KE.util.getCurrentMode(id);
        isHtml = m == 'html'; //是否HTML模式
        isUbb = m == 'ubb';   //是否UBB模式

        if (KE.g[id].wyswygMode) {
            if (format == "ubb") {
                value = ubb2html(value);
            }
            var obj = KE.g[id];
            var doc = KE.util.getIframeDoc(obj.iframe);
            doc.body.innerHTML = value;
        } else {
            KE.g[id].newTextarea.value = value;
        }
    },

    getData: function (id, filterMode) {
        var isHtml = false;
        var isUbb = false;
        var m;
        m = KE.util.getCurrentMode(id);
        isHtml = m == 'html'; //是否HTML模式
        isUbb = m == 'ubb';   //是否UBB模式
        var data;
        filterMode = (typeof filterMode == "undefined") ? true : filterMode;
        if (KE.g[id].wyswygMode) {
            if (filterMode) {
                data = KE.util.outputHtml(id, KE.g[id].iframeDoc.body);
            } else {
                data = KE.g[id].iframeDoc.body.innerHTML;
            }
            if (isUbb) {
                data = html2ubb(data); //切换到UBB模式
            }
            else {

                if (window.beforeProcessImg) data = window.beforeProcessImg(data); //处理localmedia等标记
                data = KE.util.flag2Html(data); //替换占位图片
            }

        } else {
            data = KE.g[id].newTextarea.value;
        }
        return data;
    },


    setData: function (id) {
        var data = this.getData(id, KE.g[id].filterMode);
        KE.g[id].srcTextarea.value = data;
    },

    focus: function (id) {
        if (KE.g[id].wyswygMode) {
            KE.g[id].iframeWin.focus();
        } else {
            KE.g[id].newTextarea.focus();
        }
    },
    click: function (id, cmd) {
        KE.layout.hide(id);
        KE.util.focus(id);
        if (KE.g[id].wyswygMode) {
            KE.plugin[cmd].click(id);
            KE.util.setActived(id);
        }
        else if (KE.plugin[cmd].ubbClick) {

            KE.plugin[cmd].ubbClick(id);
        }
    },
    selection: function (id) {
        var win = KE.g[id].iframeWin;
        var doc = KE.g[id].iframeDoc;
        win.focus();
        var sel = win.getSelection ? win.getSelection() : doc.selection;
        var range;
        try {
            if (sel.rangeCount > 0) {
                range = sel.getRangeAt(0);
            } else {
                range = sel.createRange ? sel.createRange() : doc.createRange();
            }
        } catch (e) { }
        if (!range) {
            range = (KE.browser.IsIE) ? doc.body.createTextRange() : doc.createRange();
        }
        KE.g[id].selection = sel;
        KE.g[id].range = range;
    },
    select: function (id) {
        var range = KE.g[id].range;
        if (KE.browser.IsIE) {
            if (range)
                range.select();
        }
        else {
            KE.util.selection(id);
        }
    },
    pToBr: function (id) {

        if (KE.browser.IsIE) {
            KE.event.add(KE.g[id].iframeDoc, 'keypress', function (e) {
                if (e.keyCode == 13 && e.ctrlKey == false) {
                    KE.util.selection(id);
                    if (KE.g[id].range.parentElement().tagName != 'LI') {
                        KE.util.insertHtml(id, '<br />');
                        KE.util.select(id);
                        e.returnValue = false;
                        return false;
                    }
                }
            });
        }
    },
    createBlock: function (id, startText, endText) {
        var doc = KE.g[id].iframeDoc;
        KE.util.selection(id);
        if (KE.browser.IsIE) {
            var r = doc.selection.createRange();
            var c = startText + r.htmlText + endText;
            r.pasteHTML(c);
            return;
        }
        var range = KE.g[id].range;
        var f = range.extractContents();
        range = KE.g[id].range;
        range.collapse(true);
        var sNode = doc.createTextNode(startText);
        var eNode = doc.createTextNode(endText);
        var fs = f.childNodes[0];
        f.insertBefore(sNode, fs);
        f.appendChild(eNode);
        range.insertNode(f);
        range.setEndBefore(eNode);
        range.collapse(false);
    },
    execCommand: function (id, cmd, value) {
        if (cmd == 'free' || cmd == 'quote') {
            this.createBlock(id, "[" + cmd + "]", "[/" + cmd + "]");
            return;
        }

        try {
            KE.g[id].iframeDoc.execCommand(cmd, false, value);
        } catch (e) { }
        KE.history.add(id, false);
    },

    callCtrlEnter: function (id, e) {
        e = window.event || e;
        if (e.ctrlKey && e.keyCode == 13) {
            var h = KE.g[id].onCtrlEnter;
            if (h) {
                KE.util.setData(id);
                h();
                e.returnValue = false;
                return false;
            }
        }
    },

    execUbb: function (id, cmd, value) {
        var tag = cmd;
        var eb = KE.g[id].newTextarea;
        if (tag.indexOf('=') != -1)
            tag = tag.substring(0, tag.indexOf('='));
        eb.focus();
        if (document.selection) {
            var r = document.selection.createRange();
            r.text = '[' + cmd + ']' + r.text + '[/' + tag + ']';
        }
        else {
            var sv = eb.value;
            var ss = eb.selectionStart;
            sl = eb.selectionEnd - eb.selectionStart;
            ts = '[' + cmd + ']';
            te = '[/' + tag + ']';
            sv = sv.substring(0, ss) + ts + sv.substring(ss);
            ss += ts.length + sl;
            sv = sv.substring(0, ss) + te + sv.substring(ss);
            eb.value = sv;
        }
    },
    setActived: function (id) {

        for (var i = 0; i < KE.activable.length; i++) {

            var cmd = KE.activable[i];
            var b = KE.g[id].toolbarIcon[cmd];

            if (!b) {
                continue;
            }


            if (KE.g[id].wyswygMode) {
                if (KE.util.queryCommandState(id, cmd)) {
                    //b.onmouseover = function() { KE.util.addClassName(this, 'selected'); };
                    //b.onmouseout = function() { this.className = 'ke-icon-actived'; };
                    //b.className = 'ke-icon-actived';3

                    KE.util.addClassName(b, 'selected');
                } else {
                    //b.onmouseover = function() { this.className = 'ke-icon-selected'; };
                    //b.onmouseout = function() { this.className = 'ke-icon'; };
                    //b.className = 'ke-icon';
                    KE.util.removeClassName(b, 'selected');
                }
            }
        }
    },
    queryCommandState: function (id, cmd) {
        try {
            if (KE.g[id].iframeDoc.queryCommandState(cmd)) return true;
            else return false;
        } catch (e) {
            return false;
        }
    },
    setWyswygMode: function (id) {
        var m = KE.util.getCurrentMode(id);
        var obj = KE.g[id];
        if (obj.wyswygMode) return;
        var data = obj.newTextarea.value;
        if (m == 'ubb')
            data = ubb2html(data);
        else {
            data = KE.util.html2Flag(data);
            if (window.beforeProcessUbbImg) data = window.beforeProcessUbbImg(data);

        }
        obj.iframeDoc.body.innerHTML = data;
        obj.iframe.style.display = 'block';
        obj.newTextarea.style.display = 'none';

        obj.tabDiv.tabItem_1.className = 'current';
        obj.tabDiv.tabItem_2.className = '';
        //KE.toolbar.able(id, ['source', 'preview', 'fullscreen']);
        obj.wyswygMode = true;

        KE.util.focus(id);
    },
    setSourceMode: function (id) {
        var obj = KE.g[id];
        if (obj.wyswygMode == false) return;
        KE.layout.hide(id);

        var m = KE.util.getCurrentMode(id);

        var data;

        if (KE.g[id].filterMode) {

            data = KE.util.outputHtml(id, obj.iframeDoc.body);
        } else {
            data = obj.iframeDoc.body.innerHTML;
        }

        if (m == 'ubb') {
            data = html2ubb(data);
        }
        else {
            data = KE.util.flag2Html(data);
            if (window.beforeProcessImg) data = window.beforeProcessImg(data);
        }

        obj.newTextarea.value = data;
        obj.tabDiv.tabItem_1.className = '';
        obj.tabDiv.tabItem_2.className = 'current';

        obj.iframe.style.display = 'none';
        obj.newTextarea.style.display = 'block';
        //KE.toolbar.disable(id, ['source', 'preview', 'fullscreen']);
        obj.wyswygMode = false;

        KE.util.focus(id);
    },
    //往编辑器插入内容，ubb内容如果不指定，将自动根据html转换
    insertContent: function (id, html, ubb) {
        KE.util.focus(id);
        var m = KE.util.getCurrentMode(id);
        var data = KE.g[id].wyswygMode || m == 'html' ? html : ubb;
        if (KE.g[id].wyswygMode) {
            KE.util.insertHtml(id, html);
        } else {
            if (m == 'ubb' && !ubb)
                data = html2ubb(html);
            else if (m == "html") {
                data = KE.util.flag2Html(html);
            }
            KE.util.insertUbb(id, data);
        }
    },
    insertUbb: function (id, ubb) {
        if (!ubb) return;
        var txt = KE.g[id].newTextarea;
        KE.util.select(id);
        txt.focus();
        if (document.selection) {
            // txt.value += ubb;
            document.selection.createRange().text = ubb;
        }
        else {

            var value = txt.value; var l = txt.value.length;
            var s = txt.selectionStart;
            txt.value = value.substr(0, s) + ubb + value.substring(s, l);
            txt.selectionStart = s + ubb.length;
            txt.selectionEnd = s + ubb.length;
        }
    },

    createUbbTag: function (id, tag) {

    },
    setCurrentMode: function (id, m)//设置当前的编辑器模式UBB或者HTML
    {
        var cm = KE.util.getCurrentMode(id);

        if (m == cm)
            return;

        var obj = KE.g[id];

        window.ke_EditorMode[id] = m;

        if (obj && KE.g[id].isCreated) {
            if (obj.wyswygMode) return;
            var data = obj.newTextarea.value;
            if (m == 'html') {
                data = ubb2html(data);
                data = KE.util.flag2Html(data);
                if (window.beforeProcessImg) data = window.beforeProcessImg(data);
            }
            else if (m == 'ubb') {
                data = html2ubb(data);
            }
            obj.newTextarea.value = data;
        }
    },

    getCurrentMode: function (id)//获取当前模式， 是UBB还是HTML模式
    {
        if (!window.ke_EditorMode) {
            window.ke_EditorMode = new Object();
        }

        if (!window.ke_EditorMode[id]) {
            window.ke_EditorMode[id] = 'html';
        }

        return window.ke_EditorMode[id];
    },

    //bbsmax end
    insertHtml: function (id, html) {
        if (html == '') return;
        KE.util.select(id);
        if (KE.browser.IsIE) {
            if (!document.implementation.hasFeature("Range", "2.0")) {
                if (KE.g[id].selection.type.toLowerCase() == 'control') {
                    KE.g[id].range.item(0).outerHTML = html;
                } else {
                    KE.g[id].range.pasteHTML(html);
                }
                KE.history.add(id, false);
                return;
            }
        }
        this.execCommand(id, 'inserthtml', html);
    },
    removeDomain: function (id, url) {
        var domains = KE.g[id].siteDomains;
        for (var i = 0, len = domains.length; i < len; i++) {
            var domain = "http://" + domains[i];
            if (url.indexOf(domain) == 0) return url.substr(domain.length);
        }
        return url;
    },
    outputHtml: function (id, element) {
        var htmlTags = KE.g[id].htmlTags;
        var htmlList = [];
        var startTags = [];
        var setStartTag = function (tagName, attrStr, styleStr, endFlag) {
            var html = '';
            html += '<' + tagName;
            if (attrStr) html += attrStr;
            if (styleStr) html += ' style="' + styleStr + '"';
            html += endFlag ? ' />' : '>';
            if (KE.browser.IsIE && endFlag && KE.util.inArray(tagName, ['br', 'hr'])) html += "\n";
            htmlList.push(html);
            if (!endFlag) startTags.push(tagName);
        };

        var setEndTag = function () {
            if (startTags.length > 0) {
                var endTag = startTags.pop();
                var html = '</' + endTag + '>';
                if (KE.browser.IsIE && KE.util.inArray(endTag, ['p', 'div', 'table', 'ol', 'ul'])) html += "\n";
                htmlList.push(html);
            }
        };

        var scanNodes = function (el) {
            var nodes = el.childNodes;
            for (var i = 0, len = nodes.length; i < len; i++) {
                var node = nodes[i];
                var endFlag = false;
                switch (node.nodeType) {
                    case 1:
                        var tagName = node.tagName.toLowerCase();
                        if (typeof htmlTags[tagName] != 'undefined') {
                            var attrStr = '';
                            var styleStr = '';
                            var attrList = htmlTags[tagName];

                            for (var j = 0, l = attrList.length; j < l; j++) {
                                var attr = attrList[j];
                                if (attr == '/') endFlag = true;
                                else if (attr.charAt(0) == '.') {
                                    var key = attr.substr(1);
                                    var arr = key.split('-');
                                    var jsKey = '';
                                    for (var k = 0, length = arr.length; k < length; k++) {
                                        jsKey += (k > 0) ? arr[k].charAt(0).toUpperCase() + arr[k].substr(1) : arr[k];
                                    }
                                    if (key == 'border') {
                                        if (node.style.border) {
                                            styleStr += 'border:' + node.style.border + ';';
                                        } else if (node.style.borderWidth && node.style.borderStyle && node.style.borderColor) {
                                            styleStr += 'border:' + node.style.borderWidth + ' ' + node.style.borderStyle + ' ' + node.style.borderColor + ';';
                                        }
                                    } else if (key == 'margin') {
                                        if (node.style.margin) {
                                            styleStr += 'margin:' + node.style.margin + ';';
                                        } else {
                                            if (node.style.marginLeft) styleStr += 'margin-left:' + node.style.marginLeft + ';';
                                            if (node.style.marginRight) styleStr += 'margin-right:' + node.style.marginRight + ';';
                                            if (node.style.marginTop) styleStr += 'margin-top:' + node.style.marginTop + ';';
                                            if (node.style.marginBottom) styleStr += 'margin-bottom:' + node.style.marginBottom + ';';
                                        }
                                    } else if (key == 'padding') {
                                        if (node.style.padding) {
                                            styleStr += 'padding:' + node.style.padding + ';';
                                        } else {
                                            if (node.style.paddingLeft) styleStr += 'padding-left:' + node.style.paddingLeft + ';';
                                            if (node.style.paddingRight) styleStr += 'padding-right:' + node.style.paddingRight + ';';
                                            if (node.style.paddingTop) styleStr += 'padding-top:' + node.style.paddingTop + ';';
                                            if (node.style.paddingBottom) styleStr += 'padding-bottom:' + node.style.paddingBottom + ';';
                                        }
                                    } else {
                                        if (node.style[jsKey]) styleStr += key + ':' + node.style[jsKey] + ';';
                                    }
                                }
                                else {
                                    var val = node.getAttribute(attr);
                                    if (attr == "colspan" || attr == "rowspan") {//修正IE 下表格的单元格 rowspan 和 colspan 默认为1 的问题
                                        if (val == "1") {
                                            val = null;
                                        }
                                    }
                                    if (val != null && val !== '') {
                                        if (typeof val == 'string' && val.match(/^javascript:/)) val = '';
                                        if ((tagName == 'a' && attr == 'href') || (tagName == 'img' && attr == 'src') ||
                                        (tagName == 'embed' && attr == 'src')) {
                                            val = KE.util.removeDomain(id, val);
                                            val = val.replace(/&(?!amp;)/ig, "&amp;");
                                        }
                                        attrStr += ' ' + attr + '="' + val + '"';
                                    }
                                }
                            }
                            setStartTag(tagName, attrStr, styleStr, endFlag);
                        }
                        if (node.hasChildNodes()) {
                            scanNodes(node);
                        } else {
                            if (startTags.length > 0) {
                                var prevHtml = htmlList[htmlList.length - 1];
                                if (prevHtml.match(/^<p|^<div/) != null) {
                                    htmlList.push("&nbsp;");

                                }
                                if (!endFlag) setEndTag();
                            }
                        }
                        break;
                    case 3:
                        htmlList.push(KE.util.escape(node.nodeValue));
                        break;
                    default:
                        break;
                }
            }
            setEndTag();
        };

        var delTags = ["script", "style"]; //这些节点将被从内容删除
        var doc = element.ownerDocument;
        for (var i = 0; i < delTags.length; i++) {
            var nodes = doc.getElementsByTagName(delTags[i]);
            for (var j = 0; j < nodes.length; j++) {
                nodes[j].parentNode.removeChild(nodes[j]);
            }
        }

        scanNodes(element);
        return htmlList.join('');
    },

    addClassName: function (node, classname) {
        if (node.className == classname || node.className.indexOf(classname + " ") == 0)
            return;
        if (node.className.indexOf(" " + classname) + classname.length + 1 == node.className.length)
            return;

        node.className += " " + classname;
    },

    removeClassName: function (node, classname) {
        var cn = node.className;

        if (cn == classname) {
            node.className = "";
            return;
        }
        if (cn.indexOf(classname + " ") == 0) {
            node.className = cn.substring(cn.indexOf(classname + " "));
            return;
        }

        if (cn.indexOf(" " + classname) + classname.length + 1 == cn.length) {
            node.className = cn.substring(0, cn.indexOf(" " + classname));
        }
    },


    /*media parser /add by wenquan 09-7-29*/

    flag2Ubb: function (html) {
        var reg = new RegExp('<img\\s[^>]*?max_editor_([^\\s>"]+)[^>]*?/?>', "ig");
        html = html.replace(reg, function (all, tag) {
            var h = 400, w = 550, src, auto;
            if (all.match(/width\s*:\s*(\d+)/i))
                w = all.match(/width\s*:\s*(\d+)/i)[1];
            else if (all.match(/width="?(\d+)/i))
                w = all.match(/width="?(\d+)/i)[1];

            if (all.match(/height\s*:\s*(\d+)/i))
                h = all.match(/height\s*:\s*(\d+)/i)[1];
            else if (all.match(/height="?(\d+)/i))
                h = all.match(/height="?(\d+)/i)[1];

            src = all.match(/alt="?(.+?)(?=["\s>])/i);
            src = src ? src[1] : '';
            auto = all.match(/\?max_autoplay=([truefalse]+)/i);
            auto = auto ? auto[1] : 'false';
            auto = auto.toLowerCase();
            return '[' + tag + '=' + w + ',' + h + ',' + (auto == 'true' ? '1' : '0') + ']' + src + '[/' + tag + ']';
        });
        return html;
    },

    flag2Html: function (html)//处理替代图片到HTML的切换
    {
        var reg = new RegExp('<img\\s[^>]*?max_editor_([^\\s>"]+)[^>]*?/?>', "ig");
        html = html.replace(reg, function (all, tag) {
            var h, w, src, auto;
            w = all.match(/width[=:][^\d]*(\d+)/i);
            w = w ? w[1] : 540;
            h = all.match(/height[=:][^\d]*(\d+)/i);
            h = h ? h[1] : 400;
            src = all.match(/alt="?(.+?)(?=["\s>])/i);
            src = src ? src[1] : '';
            auto = all.match(/\?max_autoplay=([truefalse]+)/i);
            auto = auto ? auto[1] : 'false';


            if (tag == "flash") {
                return String.format('<embed src="{0}" type="application/x-shockwave-flash" wmode="opaque" width="{1}" height="{2}" autostart="{3}" />', src, w, h, auto);
            }
            if (tag == "mp3" || tag == "wma" || tag == "wav" || tag == "mid" || tag == 'wmv') {
                return String.format('<embed src="{0}" type="application/x-mplayer2" width="{1}" height="{2}" autostart="{3}" />', src, w, h, auto);
            }
            if (tag == "rm" || tag == "rmvb" || tag == "ra") {
                return String.format('<embed src="{0}" type="audio/x-pn-realaudio-plugin" controls="ImageWindow,ControlPanel" width="{1}" height="{2}" autostart="{3}" />', src, w, h, auto);
            }

            if (tag == "flv") {
                return String.format('<embed type="application/x-shockwave-flash" src="{0}/max-assets/flash/flvplayer.swf" content="flv"  flashvars="file={1}" wmode="opaque" width="{2}" height="{3}" autostart="{4}" />', root, src, w, h, auto);
            }

            return '[' + tag + '' + (w ? '=' + w + ',' + h + ',1' : '') + ']' + src + '[/' + tag + ']';
        });

        var reg_object = new RegExp('<(?:object|param).*?/?>', 'ig');
        html = html.replace(reg_object, ''); //删除IE专用的非标准object标签

        return html;
    },

    ubb2flag: function (ubb) {
        var tags = ['mp3', 'wma', 'wmv', 'rm', 'ra', 'flash', 'flv'];
        var isAudio = false;
        function convert(tag, source) {
            var imgFile = "editor_media.gif";

            if (tag == "mp3" || tag == "wma" || tag == "wma" || tag == "wmv" || tag == "wav" || tag == "mid") {
                imgFile = "editor_audio.gif";
            }
            else if (tag == "ra" || tag == "rm" || tag == "rmvb") {
                imgFile = "editor_real.gif";
            }
            else if (tag == "flash") {
                imgFile = "editor_flash.gif";
            }
            switch (tag) {
                case "mp3":
                case "wma":
                case "ra":
                case "mid":
                case "wav":
                    isAudio = true;
                    break;
                default:
                    isAudio = false;
                    break;
            }

            var reg = new RegExp('\\[' + tag + '(.*?)]([\\s\\S]+?)\\[\\/' + tag + '\\]', "ig");
            source = source.replace(reg, function (all, attr, url) {
                var attrs = attr ? attr.replace('=', '').split(',') : '';
                var w, h, m;
                if (attrs) {
                    if (attrs.length > 0)
                        w = attrs[0];
                    if (attrs.length > 1)
                        h = attrs[1];
                    if (attrs.length > 2)
                        m = attrs[2];
                }
                if (!w) w = isAudio ? 400 : 550; if (!h) h = isAudio ? 64 : 400; if (!m) m = 0;
                return '<img src="' + root + '/max-assets/images/blank.gif?max_autoplay=' + (m == '1' ? 'true' : 'false') + '" class="max_editor_' + tag + '" title="max_editor_' + tag + '" alt="' + url + '" width="' + w + '" height="' + h + '"  style="border:solid 1px #aaaaaa; background-position:center center; background-repeat:no-repeat;background-image:url(' + root + '/max-assets/images/' + imgFile + ')" />';
            });
            return source;
        }
        for (var i = 0; i < tags.length; i++) {
            ubb = convert(tags[i], ubb);
        }
        return ubb;
    },

    html2Flag: function (html) {
        //切换HTML到替换图片的过程
        var reg_flash = new RegExp('<embed\\s[^>]*?(?:src="[^>]*?\\.(?:swf|flv)"|application\\/x\\-shockwave\\-flash)[^>]*?/?>(?:</embed>)?', 'ig');
        var reg_wm = new RegExp('<embed\\s[^>]*?(?:src="[^>]*?\\.(?:mp3|mid|wav|wma|avi)"|application\\/x\-mplayer2)[^>]*?/?>(?:</embed>)?', 'ig');
        var reg_real = new RegExp('<embed\\s[^>]*?(?:src="[^>]*?\\.(?:rmvb|rm|ra)"|audio\/x\-pn\-realaudio\-plugin)[^>]*?/?>(?:</embed>)?', 'ig');
        var reg_object = new RegExp('<(?:object|param).*?/?>', 'ig');

        var embedImage = '<img src="' + root + '/max-assets/images/blank.gif?max_autoplay={5}" class="max_editor_{0}" title="max_editor_{0}" alt="{1}" width="{2}" height="{3}" style="border:solid 1px #aaaaaa; background-position:center center; background-repeat:no-repeat;background-image:url(' + root + '/max-assets/images/{4})" />';

        html = html.replace(reg_flash, function (all) {
            var w = all.match(/width="(.*?)"/i);
            var h = all.match(/height="(.*?)"/i);
            var s = all.match(/src="(.*?)"/i);
            var a = all.match(/autostart="(.+?)"/i);
            var t = all.match(/content="(.+?)"/i);
            w = w ? w[1] : 400;
            h = h ? h[1] : 300;
            a = a ? a[1] : "false";
            a = a.toLowerCase();
            if (a != "false" && a != "true") a = 'false';
            s = s[1];
            var tag = "flash";
            if (t)tag = t[1];
            return String.format(embedImage, tag, s, w, h, 'editor_flash.gif', a);

        });

        html = html.replace(reg_wm, function (all) {
            var w = all.match(/width="(.*?)"/i);
            var h = all.match(/height="(.*?)"/i);
            var s = all.match(/src="(.*?)"/i);
            var a = all.match(/autostart="(.+?)"/i);
            w = w ? w[1] : 400;
            h = h ? h[1] : 300;
            a = a ? a[1] : "false";
            a = a.toLowerCase();
            if (a != "false" && a != "true") a = 'false';
            s = s[1];
            return String.format(embedImage, 'mp3', s, w, h, 'editor_audio.gif', a);
        });

        html = html.replace(reg_real, function (all) {
            var w = all.match(/width="(.*?)"/i);
            var h = all.match(/height="(.*?)"/i);
            var s = all.match(/src="(.*?)"/i);
            var a = all.match(/autostart="(.+?)"/i);
            w = w ? w[1] : 400;
            h = h ? h[1] : 300;
            s = s[1];
            a = a ? a[1] : "false";
            a = a.toLowerCase();
            if (a != "false" && a != "true") a = 'false';
            return String.format(embedImage, 'rm', s, w, h, 'editor_real.gif', a);

        });

        html = html.replace(reg_object, ''); //删除IE专用的非标准object标签

        return html;
    },
    html2Ubb: function (id, tag) {

    }
};

KE.layout = {
    show: function(id, div) {
        KE.layout.hide(id);
        KE.g[id].hideDiv.appendChild(div);
        KE.g[id].hideDiv.style.display = 'block';
        KE.g[id].layoutDiv = div;
    },
    hide: function(id, keepDiv) {
        var div = KE.g[id].layoutDiv;
        if (keepDiv) {
            div.style.display = "none";
        }
        else {
            try {
                KE.g[id].hideDiv.removeChild(div);
            } catch (e) { }
        }

        KE.g[id].hideDiv.style.display = 'none';
        KE.g[id].maskDiv.style.display = 'none';
        KE.util.focus(id);
    },
    make: function(id) {
        var div = KE.$$('div');
        div.style.position = 'absolute';
        div.style.zIndex = 900;
        return div;
    }
};
KE.menu = function(arg) {
    this.arg = arg;
    var div = KE.layout.make(arg.id);
    div.className = 'ke-menu';
    var obj = KE.g[arg.id].toolbarIcon[arg.cmd];
    this.div = div;
    setPos(obj, div);
    this.add = function(html, event) {
        var cDiv = KE.$$('div');
        cDiv.className = 'ke-menu-noselected';
        cDiv.style.width = this.arg.width;
        cDiv.onmouseover = function() { this.className = 'ke-menu-selected'; }
        cDiv.onmouseout = function() { this.className = 'ke-menu-noselected'; }
        cDiv.onclick = event;
        cDiv.innerHTML = html;
        this.append(cDiv);
    };

    function setPos(source, div) {
        var pos = getRect(source); // KE.util.getElementPos(obj);
        div.style.top = pos.bottom + "px"; // +obj.offsetHeight + 'px';
        div.style.left = pos.left + "px";
    }

    this.append = function(el) {
        this.div.appendChild(el);
    };
    this.insert = function(html) {
        this.div.innerHTML = html;
    };
    this.show = function(e) {
        if (e)
            setPos(e, this.div);
        KE.layout.show(this.arg.id, this.div);
    };
    this.picker = function() {
        var colorTable = KE.lang['colorTable'];
        var table = KE.$$('table');
        table.cellPadding = 0;
        table.cellSpacing = 0;
        table.border = 0;
        table.style.margin = 0;
        table.style.padding = 0;
        table.style.borderCollapse = 'separate';
        for (var i = 0; i < colorTable.length; i++) {
            var row = table.insertRow(i);
            for (var j = 0; j < colorTable[i].length; j++) {
                var cell = row.insertCell(j);
                cell.className = 'ke-picker-cell';
                cell.style.backgroundColor = colorTable[i][j];
                cell.title = colorTable[i][j];
                cell.onmouseover = function() { this.style.borderColor = '#000000'; }
                cell.onmouseout = function() { this.style.borderColor = '#F0F0EE'; }
                cell.onclick = new Function('KE.plugin["' + this.arg.cmd + '"].exec("' +
                                            this.arg.id + '", "' + colorTable[i][j] + '")');
                cell.innerHTML = '&nbsp;';
            }
        }
        this.append(table);
        this.show();
    };
};

KE.toolbar = {
    able: function(id, arr) {
        KE.each(KE.g[id].toolbarIcon, function(cmd, obj) {
            if (!KE.util.inArray(cmd, arr)) {
                //obj.className = 'ke-icon';
                KE.util.setOpacity(obj, 100);
                //obj.onmouseover = function() { this.className = "ke-icon-selected"; };
                //obj.onmouseout = function() { this.className = "ke-icon"; };
                obj.onclick = new Function('KE.util.click("' + id + '", "' + cmd + '")');
            }
        });
    },
    disable: function(id, arr) {
        KE.each(KE.g[id].toolbarIcon, function(cmd, obj) {
            if (!KE.util.inArray(cmd, arr)) {
                //obj.className = 'ke-icon-disabled';
                KE.util.setOpacity(obj, 50);
                //                obj.onmouseover = null;
                //                obj.onmouseout = null;
                obj.onclick = null;
            }
        });
    },
    create: function(id) {
        var toolbarContainer;
        KE.g[id].toolbarIcon = [];
        var toolbar = KE.$$('table');
        toolbar.oncontextmenu = function() { return false; };
        toolbar.onmousedown = function() { return false; };
        toolbar.onmousemove = function() { return false; };
        toolbar.className = 'ke-toolbar';
        toolbar.cellPadding = 0;
        toolbar.cellSpacing = 0;
        toolbar.border = 0;
        var row = toolbar.insertRow(0);
        var toolbarCell = row.insertCell(0);
        toolbarCell.style.padding = 0;
        toolbarCell.style.margin = 0;
        toolbarCell.style.border = 0;
        var length = KE.g[id].items.length;
        var cellNum = 0;
        var row;
        for (var i = 0; i < length; i++) {
            var cmd = KE.g[id].items[i];
            if (i == 0 || cmd == '-') {
                var table = KE.$$('table');
                table.cellPadding = 0;
                table.cellSpacing = 0;
                table.border = 0;
                table.className = 'ke-toolbar-table';
                row = table.insertRow(0);
                cellNum = 0;
                toolbarCell.appendChild(table);
                if (cmd == '-') continue;
            }
            var cell = row.insertCell(cellNum);
            cellNum++;
            var obj = KE.$$('img');
            obj.src = KE.g[id].skinsPath + 'spacer.gif';
            if (KE.util.inArray(cmd, KE.g[id].defaultItems)) {
                //var url = KE.g[id].skinsPath + KE.g[id].skinType + '.gif';
                //var url = KE.g[id].skinsPath + KE.g[id].skinType + '.png';
                //obj.style.backgroundImage = "url(" + url + ")";
            }
            obj.className = "ke-common-icon ke-icon-" + cmd;
            obj.alt = KE.lang[cmd];
            cell.className = 'ke-icon';
            cell.title = KE.lang[cmd];
            cell.onmouseover = function() { this.className = "ke-icon-selected"; };
            cell.onmouseout = function() { this.className = "ke-icon"; };
            cell.onclick = new Function('KE.util.click("' + id + '", "' + cmd + '")');
            cell.appendChild(obj);
            KE.g[id].toolbarIcon[cmd] = cell;
        }
        return toolbar;
    }
};
KE.history = {
    add: function(id, minChangeFlag) {
        //        var obj = KE.g[id];
        //        var html = KE.util.getData(id, false);
        //        if (obj.undoStack.length > 0) {
        //            var prevHtml = obj.undoStack[obj.undoStack.length - 1];
        //            if (html == prevHtml) return;
        //            if (minChangeFlag && Math.abs(html.length - prevHtml.length) < obj.minChangeSize) return;
        //        }
        //        obj.undoStack.push(html);
        //        obj.redoStack = [];
    },
    undo: function(id) {
        KE.util.execCommand(id, "undo", "");
        //        var obj = KE.g[id];
        //        if (obj.undoStack.length == 0) return;
        //        var html = KE.util.getData(id, false);
        //        obj.redoStack.push(html);
        //        var prevHtml = obj.undoStack.pop();
        //        if (html == prevHtml && obj.undoStack.length > 0) {
        //            prevHtml = obj.undoStack.pop();
        //        }
        //        obj.iframeDoc.body.innerHTML = prevHtml;
        //        obj.newTextarea.value = prevHtml;
    },
    redo: function(id) {
        KE.util.execCommand(id, "redo", "");
        //        var obj = KE.g[id];
        //        if (obj.redoStack.length == 0) return;
        //        var html = KE.util.getData(id, false);
        //        obj.undoStack.push(html);
        //        var nextHtml = obj.redoStack.pop();
        //        obj.iframeDoc.body.innerHTML = nextHtml;
        //        obj.newTextarea.value = nextHtml;
    }
};
KE.remove = function(id, mode) {
    mode = (typeof mode == "undefined") ? 0 : mode;
    var container = KE.g[id].container;
    if (mode == 1) {
        document.body.removeChild(container);
    } else {
        var srcTextarea = KE.$(id);
        srcTextarea.parentNode.removeChild(container);
    }

    KE.g[id].containner = null;
};


//新建编辑器
KE.create = function (id, mode) {
    if (KE.browser.IsIE) try { document.execCommand('BackgroundImageCache', false, true); } catch (e) { }
    var srcTextarea = KE.$(id);
    mode = (typeof mode == "undefined") ? 0 : mode;
    if (mode == 0 && KE.g[id].container != null) return;

    var width = srcTextarea.style.width;
    var height = srcTextarea.style.height;
    var isload = false;
    var container;
    container = KE.$("ke_container_" + id);
    if (container)
        isload = true;
    else
        container = KE.$$('div');
    var toolbarDiv = KE.$('ke_toolbar_' + id) || KE.toolbar.create(id);
    var iframe = KE.$('ke_iframe_' + id) || KE.$$('iframe');

    var newTextarea = KE.$('ke_textarea_' + id) || KE.$$('textarea');
    var formDiv = KE.$('ke_form_' + id) || KE.$$('div');
    var tabDiv = KE.$('ke_tab_' + id) || KE.$$('div');
    var bottom = KE.$('ke_bottom_' + id) || KE.$$('table'); //div
    var hideDiv = KE.$('ke_hide_' + id) || KE.$$('div');
    var maskDiv = KE.$('ke_mask_' + id) || KE.$$('div');

    var sizebox = KE.$('ke_sizebox_' + id) || KE.$$('div');
    var sizebox_left = KE.$('ke_sizebox_left_' + id) || KE.$$('a');
    var sizebox_right = KE.$('ke_sizebix_right_' + id) || KE.$$('a');

    if (KE.browser.IsIE6 || KE.browser.IsIE7) { newTextarea.style.height = formDiv.style.height; }

    KE.util.setOpacity(maskDiv, 50);
    maskDiv.className = 'ke-mask';
    hideDiv.style.display = 'none';
    if (!isload) {

        iframe.className = 'ke-iframe';
        //iframe.setAttribute('frameBorder', '0');
        iframe.style.border = 'none';

        container.id = "ke_container_" + id;
        if (mode == 1)
            document.body.appendChild(container);
        else
            srcTextarea.parentNode.insertBefore(container, srcTextarea);
        container.appendChild(toolbarDiv);
        formDiv.appendChild(iframe);
        formDiv.appendChild(newTextarea);
        container.appendChild(formDiv);
        container.appendChild(tabDiv);
        var tabItems = ['可视化', '源代码'];
        for (var i = 0; i < tabItems.length; i++) {
            var tabItem_a = document.createElement('a');
            if (i == 0) {
                tabItem_a.setAttribute('href', 'javascript:KE.util.setWyswygMode(\'' + id + '\')');
                //if (KE.g[id].wyswygMode) 
                tabItem_a.className = 'ke-tabitem-actived';
                tabDiv.tabItem_1 = tabItem_a;
            }
            else {
                tabItem_a.setAttribute('href', 'javascript:KE.util.setSourceMode(\'' + id + '\')');
                //if (!KE.g[id].wyswygMode) tabItem_a.className = 'ke-tabitem-actived';
                tabDiv.tabItem_2 = tabItem_a;
            }
            tabItem_a.className = 'ke-tabitem-' + (i + 1) + ' ' + tabItem_a.className;
            var tabItem_text = document.createTextNode(tabItems[i]);
            tabItem_a.appendChild(tabItem_text);
            tabDiv.appendChild(tabItem_a);
        }

        sizebox.className = 'ke-adjust';
        sizebox.id = 'ke_sizebox_' + id;
        sizebox_left.className = 'ke-adjust-del';
        sizebox_left.id = 'ke_sizebox_left_' + id;
        sizebox_right.className = 'ke-adjust-add';
        sizebox_right.id = 'ke_sizebix_right_' + id;
        sizebox_left.href = 'javascript:void(KE.util.shrink("' + id + '"));';
        sizebox_right.href = 'javascript:void(KE.util.elongate("' + id + '"));';
        sizebox_left.setAttribute('title', KE.lang['shrink']);
        sizebox_right.setAttribute('title', KE.lang['elongate']);

        sizebox.appendChild(sizebox_left);
        sizebox.appendChild(sizebox_right);

        tabDiv.appendChild(sizebox);

        var row = bottom.insertRow(0);
        bottomLeft = row.insertCell(0);
        bottomLeft.className = 'ke-bottom-left';

        var img = KE.$$('img');
        img.className = 'ke-bottom-left-img';
        img.src = KE.g[id].skinsPath + 'spacer.gif';
        bottomLeft.appendChild(img);

        bottomRight = row.insertCell(1);
        bottomRight.className = 'ke-bottom-right';

        document.body.appendChild(hideDiv);
        document.body.appendChild(maskDiv);

        container.className = 'ke-container';
        container.style.width = width;
        container.style.height = height;

        newTextarea.className = 'ke-textarea';
        newTextarea.style.display = 'none';

        formDiv.className = 'ke-form';
        tabDiv.className = 'ke-tab';
    }
    else {
        tabDiv.tabItem_1 = KE.$('ke_tab_item1_' + id);
        tabDiv.tabItem_2 = KE.$('ke_tab_item2_' + id);

    }

    if (KE.g[id].resizeMode != 2 || KE.g[id].fullscreenMode) {
        sizebox.style.display = 'none';
    }
    srcTextarea.style.display = "none";
    KE.util.setDefaultPlugin(id);
    var iframeWin = iframe.contentWindow;
    var iframeDoc = KE.util.getIframeDoc(iframe);

    var html = KE.util.getFullHtml(id);

    //*****************这两个如果不分开判断会出现奇奇怪怪的鸡巴问题

    if (!KE.browser.IsIE)
        iframeDoc.designMode = "On";

    //****************************

    iframeDoc.open();
    iframeDoc.write(html);
    iframeDoc.close();
    if (KE.browser.IsIE) {
        iframeDoc.body.contentEditable = "true";
    }
    if (KE.g[id].autoOnsubmitMode) {
        var form = srcTextarea.parentNode;
        while (form != null && form.tagName != 'FORM') { form = form.parentNode; }
        if (form != null && form.tagName == 'FORM') {

            KE.event.add(form, 'submit', new Function('KE.util.setData("' + id + '")'));
        }
    }
    KE.event.add(iframeDoc, 'click', new Function('KE.panel.hide("' + id + '");  KE.layout.hide("' + id + '")'));
    KE.event.add(newTextarea, 'click', new Function('KE.panel.hide("' + id + '") ;KE.layout.hide("' + id + '")'));

    KE.event.add(iframeDoc, 'keyup', new Function('KE.util.setActived("' + id + '");KE.history.add("' + id + '", true);'));
    KE.event.add(iframeDoc, 'mouseup', new Function('KE.util.setActived("' + id + '")'));

    if (KE.g[id].onCtrlEnter) {
        KE.event.add(iframeDoc, 'keydown', function (e) { KE.util.callCtrlEnter(id, e); });
        KE.event.add(newTextarea, 'keydown', function (e) { KE.util.callCtrlEnter(id, e); });
        KE.event.add(newTextarea, 'keydown', new Function('KE.util.callCtrlEnter("' + id + '");'));
    }

    KE.event.add(newTextarea, 'keyup', new Function('KE.history.add("' + id + '", true)'));
    KE.g[id].container = container;
    KE.g[id].toolbarDiv = toolbarDiv;
    KE.g[id].formDiv = formDiv;
    KE.g[id].tabDiv = tabDiv;
    KE.g[id].iframe = iframe;
    KE.g[id].newTextarea = newTextarea;
    KE.g[id].sizebox = sizebox;
    KE.g[id].srcTextarea = srcTextarea;
    KE.g[id].hideDiv = hideDiv;
    KE.g[id].bottomDiv = bottom;
    KE.g[id].maskDiv = maskDiv;
    KE.g[id].iframeWin = iframeWin;
    KE.g[id].iframeDoc = iframeDoc;
    if (!isload) {
        width = container.offsetWidth;
        height = container.offsetHeight;
        KE.g[id].width = width + 'px';
    }
    else {
        width = KE.g[id].width;
        height = KE.g[id].height;
    }

    KE.g[id].height = height + 'px';
    if (!isload)
        KE.util.resize(id, width, height);


    setTimeout(
        function () {

            var data = srcTextarea.value;
            //  if (data == null || data == '') data = '&nbsp;';

            if (data) {
                if (KE.util.getCurrentMode(id) == 'ubb') {

                    data = ubb2html(data);
                    data = KE.util.html2Flag(data);
                    if (window.beforeProcessUbbImg) data = window.beforeProcessUbbImg(data);
                }
                else {
                    data = KE.util.html2Flag(data);
                    if (window.beforeProcessImg) data = window.beforeProcessImg(data);
                }
            }

            iframeDoc.body.innerHTML = data;
            KE.util.pToBr(id);
            KE.history.add(id, false);
            if (!KE.g[id].wyswygMode) { KE.g[id].wyswygMode = true; KE.util.setSourceMode(id) };

        }, 1);
    KE.g[id].isCreated = true;
};
KE.version = '3.1.2';
KE.scriptPath = KE.util.getScriptPath();
KE.htmlPath = KE.util.getHtmlPath();
KE.browser = KE.util.getBrowser();
KE.plugin = {};
KE.g = {};
KE.init = function(config) {
    config.wyswygMode = (typeof config.wyswygMode == "undefined") ? true : config.wyswygMode;
    config.autoOnsubmitMode = (typeof config.autoOnsubmitMode == "undefined") ? true : config.autoOnsubmitMode;
    config.resizeMode = (typeof config.resizeMode == "undefined") ? 2 : config.resizeMode;
    config.filterMode = (typeof config.filterMode == "undefined") ? true : config.filterMode;
    config.skinType = config.skinType || 'default';
    config.cssPath = config.cssPath || '';
    config.skinsPath = config.skinsPath || KE.scriptPath + 'skins/';
    config.pluginsPath = config.pluginsPath || root + '/max-dialogs/editor/plugins/';
    config.minWidth = config.minWidth || 200;
    config.minHeight = config.minHeight || 120;
    config.minChangeSize = config.minChangeSize || 5;
    config.siteDomains = config.siteDomains || [];
    //bbsmax begin
    config.onCtrlEnter = config.onCtrlEnter || null;
    config.panelSize = config.panelSize || { width: 600, height: 360 };
    //bbsmax end
    var defaultItems = [
        'source', 'preview', 'fullscreen', 'undo', 'redo', 'cut', 'copy', 'paste',
        'plainpaste', 'wordpaste', 'justifyleft', 'justifycenter', 'justifyright',
        'justifyfull', 'insertorderedlist', 'insertunorderedlist', 'indent', 'outdent', 'subscript',
        'superscript', '-',
        'fontname', 'fontsize', 'textcolor', 'bgcolor', 'bold',
        'italic', 'underline', 'strikethrough', 'removeformat', 'selectall', 'image',
        'flash', 'media', 'layer', 'table',
        'emoticons', 'link', 'unlink'
    ];
    config.defaultItems = defaultItems;
    config.items = config.items || defaultItems;
    var defaultHtmlTags = {
        'font': ['color', 'size', 'face', '.background-color'],
        'span': ['.color', '.background-color', '.font-size', '.font-family', '.font-weight', '.font-style', '.text-decoration', '.vertical-align'],
        'div': ['class', 'align', '.border', '.margin', '.padding', '.text-align', '.color', '.background-color', '.font-size', '.font-family', '.font-weight', '.font-style', '.text-decoration', '.vertical-align'],
        'a': ['href', 'target'],
        'embed': ['src', 'type', 'loop', 'wmode', 'autostart', 'quality', '.width', '.height', '/'],
        'img': ['src', 'width', 'height', 'border', 'alt', 'title', '.width', '.height', '/'],
        'hr': ['/'],
        'br': ['/'],
        'p': ['align', '.text-align', '.color', '.background-color', '.font-size', '.font-family', '.font-weight', '.font-style', '.text-decoration', '.vertical-align'],
        'table': ['border', 'cellspacing', 'cellpadding', 'width', 'height', '.padding', '.margin', '.border', '.width', '.height'],
        'tbody': [],
        'tr': [],
        'td': ['align', 'rowspan', 'colspan', 'width', 'height', '.text-align', '.color', '.background-color', '.font-size', '.font-family', '.font-weight', '.font-style', '.text-decoration', '.vertical-align'],
        'th': ['align', 'rowspan', 'colspan', 'width', 'height', '.text-align', '.color', '.background-color', '.font-size', '.font-family', '.font-weight', '.font-style', '.text-decoration', '.vertical-align'],
        'strong': [],
        'b': [],
        'ol': ['align', '.text-align', '.color', '.background-color', '.font-size', '.font-family', '.font-weight', '.font-style', '.text-decoration', '.vertical-align'],
        'ul': ['align', '.text-align', '.color', '.background-color', '.font-size', '.font-family', '.font-weight', '.font-style', '.text-decoration', '.vertical-align'],
        'li': ['align', '.text-align', '.color', '.background-color', '.font-size', '.font-family', '.font-weight', '.font-style', '.text-decoration', '.vertical-align'],
        'sub': [],
        'sup': [],
        'blockquote': ['align', '.text-align', '.color', '.background-color', '.font-size', '.font-family', '.font-weight', '.font-style', '.text-decoration', '.vertical-align'],
        'h1': ['align', '.text-align', '.color', '.background-color', '.font-size', '.font-family', '.font-weight', '.font-style', '.text-decoration', '.vertical-align'],
        'h2': ['align', '.text-align', '.color', '.background-color', '.font-size', '.font-family', '.font-weight', '.font-style', '.text-decoration', '.vertical-align'],
        'h3': ['align', '.text-align', '.color', '.background-color', '.font-size', '.font-family', '.font-weight', '.font-style', '.text-decoration', '.vertical-align'],
        'h4': ['align', '.text-align', '.color', '.background-color', '.font-size', '.font-family', '.font-weight', '.font-style', '.text-decoration', '.vertical-align'],
        'h5': ['align', '.text-align', '.color', '.background-color', '.font-size', '.font-family', '.font-weight', '.font-style', '.text-decoration', '.vertical-align'],
        'h6': ['align', '.text-align', '.color', '.background-color', '.font-size', '.font-family', '.font-weight', '.font-style', '.text-decoration', '.vertical-align'],
        'em': [],
        'u': [],
        'strike': []
    };
    config.htmlTags = config.htmlTags || defaultHtmlTags;
    KE.g[config.id] = config;

    KE.g[config.id].undoStack = [];
    KE.g[config.id].redoStack = [];
    KE.util.loadStyle(config.skinsPath + config.skinType + '.css');
}
KE.show = function(config) {
    KE.init(config);
    KE.event.add(window, 'load', new Function('KE.create("' + config.id + '")'));


};

KE.plugin['fullscreen'] = {
    resetFull: function(id) {
        var rect = max.global.getBrowserRect();
        var left = rect.left,top = rect.top;
        var div = KE.g[id].container;
        div.style.left = "0px";// left + 'px';
        div.style.top ="0px";// top + 'px';
        div.style.zIndex = 49;
        KE.util.resize(id,rect.width, rect.height);
    },
    ubbClick: function(id) { this.click(id); },
    click: function(id) {
        var obj = KE.g[id];
        var div = obj.container;
        var s = div.style;
        var resizeListener = function(event) {
            if (obj.fullscreenMode) {
                KE.plugin["fullscreen"].resetFull(id);
            }
        }
        if (obj.fullscreenMode) {
            obj.fullscreenMode = false;
            s.position = "static";
            document.body.parentNode.style.overflow = 'auto';
            KE.util.resize(id,"100%", parseInt(this.height));
            KE.event.remove(window, 'resize', resizeListener);
            if (KE.browser.IsIE6) {
                var dl = this.dropDownList;
                for (var i = 0; i < dl.length; i++) {
                    var d = dl[i];
                    d.obj.style.display = d.disp;
                } dl = null;
                this.dropDownList = null;
            }
        } else {
            if (KE.browser.IsIE6) {
                this.dropDownList = [];
                var dl = document.getElementsByTagName("select");
                for (var i = 0; i < dl.length; i++) {
                    var d = {};
                    d.obj = dl[i];
                    d.disp = dl[i].style.display + "";
                    dl[i].style.display = "none";
                    this.dropDownList.push(d);
                }
            }
            document.body.parentNode.style.overflow = 'hidden';
            obj.fullscreenMode = true;
            this.width = div.offsetWidth;
            this.height = div.offsetHeight;
            KE.util.hideBottom(id);
            s.position = 'absolute';
            this.resetFull(id);
            KE.event.add(window, 'resize', resizeListener);
        }
    }
};
KE.plugin['bgcolor'] = {
    click: function(id) {
        KE.util.selection(id);
        var menu = new KE.menu({
            id: id,
            cmd: 'bgcolor'
        });
        menu.picker();
    },

    exec: function(id, value) {
        KE.util.select(id);
        if (KE.browser.IsIE) {
            KE.util.execCommand(id, 'backcolor', value);
        } else {
            KE.util.execCommand(id, 'hiliteColor', value);
        }
        KE.layout.hide(id);
        KE.util.focus(id);
    }
};
KE.plugin['fontname'] = {
    click: function(id) {
        var cmd = 'fontname';
        KE.util.selection(id);
        var fontName = KE.lang['fontTable'];
        var menu = new KE.menu({
            id: id,
            cmd: cmd,
            width: '160px'
        });
        KE.each(fontName, function(key, value) {
            var html = '<span style="font-family: ' + key + ';">' + value + '</span>';
            menu.add(html, new Function('KE.plugin["' + cmd + '"].exec("' + id + '", "' + key + '")'));
        });
        menu.show();
    },
    exec: function(id, value) {
        KE.util.select(id);
        KE.util.execCommand(id, 'fontname', value);
        KE.layout.hide(id);
        KE.util.focus(id);
    }
};
KE.plugin['fontsize'] = {
    click: function(id) {
        var fontSize = {
            '1': '1号',
            '2': '2号',
            '3': '3号',
            '4': '4号',
            '5': '5号',
            '6': '6号',
            '7': '7号'
        };
        var cmd = 'fontsize';
        KE.util.selection(id);
        var menu = new KE.menu({
            id: id,
            cmd: cmd,
            width: '100px'
        });
        KE.each(fontSize, function(key, value) {
            var html = '<font size="' + key + '">' + value + '</font>';
            menu.add(html, new Function('KE.plugin["' + cmd + '"].exec("' + id + '", "' + key + '")'));
        });
        menu.show();
    },
    exec: function(id, value) {
        KE.util.select(id);
        KE.util.execCommand(id, 'fontsize', value.substr(0, 1));
        KE.layout.hide(id);
        KE.util.focus(id);
    }
};

KE.plugin['source'] = {
    click: function(id) {
        if (obj.wyswygMode) {
            setSourceMode(id);
        } else {
            setWyswygMode(id);
        }
    }
};
KE.plugin['textcolor'] = {
    click: function(id) {
        KE.util.selection(id);
        var menu = new KE.menu({
            id: id,
            cmd: 'textcolor'
        });
        menu.picker();
    },
    exec: function(id, value) {
        KE.util.select(id);
        KE.util.execCommand(id, 'forecolor', value);
        KE.layout.hide(id);
        KE.util.focus(id);
    }
};

KE.plugin['layer'] = {
    click: function(id) {
        var cmd = 'layer';
        var styles = [
            'margin:5px;border:1px solid #000000;',
            'margin:5px;border:2px solid #000000;',
            'margin:5px;border:1px dashed #000000;',
            'margin:5px;border:2px dashed #000000;',
            'margin:5px;border:1px dotted #000000;',
            'margin:5px;border:2px dotted #000000;'
        ];
        KE.util.selection(id);
        var menu = new KE.menu({
            id: id,
            cmd: cmd,
            width: '150px'
        });
        for (var i = 0; i < styles.length; i++) {
            var html = '<div style="height:15px;' + styles[i] + '"></div>';
            menu.add(html, new Function('KE.plugin["' + cmd + '"].exec("' + id + '", "padding:5px;' + styles[i] + '")'));
        }
        menu.show();
    },
    exec: function(id, value) {
        KE.util.select(id);
        var html = '<div style="' + value + '">' + KE.lang['pleaseInput'] + '</div>';
        KE.util.insertContent(id, html);
        KE.layout.hide(id);
        KE.util.focus(id);
    }
};


KE.plugin['table'] = {
    selected: function(id, i, j) {
        var text = i.toString(10) + ' x ' + j.toString(10) + ' 表格';
        KE.$('tableLocation' + id).innerHTML = text;
        var num = 10;
        for (var m = 1; m <= num; m++) {
            for (var n = 1; n <= num; n++) {
                var td = KE.$('tableTd' + id + m.toString(10) + '_' + n.toString(10) + '');
                if (m <= i && n <= j) {
                    td.style.backgroundColor = '#eeeeee';
                } else {
                    td.style.backgroundColor = '#FFFFFF';
                }
            }
        }
    },
    click: function(id) {
        var cmd = 'table';
        KE.util.selection(id);
        var num = 10;
        var html = '<table cellpadding="0" cellspacing="0" border="0" style="width:120px;margin:1px 0 0 1px;border-collapse:collapse;">';
        for (var i = 1; i <= num; i++) {
            html += '<tr>';
            for (var j = 1; j <= num; j++) {
                var value = i.toString(10) + ',' + j.toString(10);
                html += '<td id="tableTd' + id + i.toString(10) + '_' + j.toString(10) +
                    '" style="font-size:0;width:12px;height:12px;' +
                    'border:1px solid #ccc;cursor:pointer;" ' +
                    'onclick="javascript:KE.plugin[\'table\'].exec(\'' + id + '\', \'' + value + '\');" ' +
                    'onmouseover="javascript:KE.plugin[\'table\'].selected(\'' + id + '\', \'' + i.toString(10) +
                    '\', \'' + j.toString(10) + '\');">&nbsp;</td>';
            }
            html += '</tr>';
        }
        html += '<tr><td colspan="10" id="tableLocation' + id +
            '" style="text-align:center;height:20px;margin:0;padding:0;border:0;color:#999;"></td></tr>';
        html += '</table>';
        var menu = new KE.menu({
            id: id,
            cmd: cmd
        });
        menu.insert(html);
        menu.show();
    },
    ubbClick: function(id) { this.click(id); },
    exec: function(id, value) {
        KE.util.select(id);
        var location = value.split(',');
        var html = '<table border="1">';
        for (var i = 0; i < location[0]; i++) {
            html += '<tr>';
            for (var j = 0; j < location[1]; j++) {
                html += '<td>&nbsp;</td>';
            }
            html += '</tr>';
        }
        html += '</table>';
        KE.util.insertContent(id, html);
        KE.layout.hide(id);
        KE.util.focus(id);
    }
};

KE.plugin['emoticons'] = {
    menu: null,
    click: function(id) {
        var cmd = 'emoticons';
        KE.util.selection(id);
       var width = 417;
       var height = 310;
        var op = "";
//        if (KE.g[id].ownerUserId)
//            op += '&userid=' + KE.g[id].ownerUserId;
        if (KE.g[id].action)
            op += "&action=" + KE.g[id].action;
        if (KE.g[id].targetID)
            op += "&targetid=" + KE.g[id].targetID;
        KE.panel.show(id, cmd, width, height, op);
    },
    ubbClick: function(id) { this.click(id); }
};

//=================
KE.panel = {};

KE.panel.show = function (id, cmd, width, height, otherParams) {
    if (height == -1) height = KE.g[id].panelSize.height;
    if (width == -1) width = KE.g[id].panelSize.width;
    KE.util.selection(id);
    if (KE.panel[id] == null) KE.panel[id] = {};
    if (KE.panel[id].popup == null) KE.panel[id].popup = {};
    var p = KE.panel[id].popup[cmd];
    if (!p) {
        var trigger = KE.$("ME_" + cmd);
        var div = addElement("div");
        div.className = "ke-menu";
        div.id = "ME_popup_" + cmd;

        p = new popup(div, trigger, false, null, "left bottom");
        p.maskDialog = 1;
        div.style.zIndex = 50;
        var ifr;
        var src = root + "/max-dialogs/editor/" + cmd + ".aspx?id=" + id;
        if (window.forumID) src += "&forumid=" + forumID;
        if (otherParams) src += otherParams;
        ifr = document.createElement("iframe"); //, div);
        ifr.setAttribute('scrolling', 'no');
        ifr.setAttribute('frameBorder', '0');
        ifr.style.border = 'none';
        div.appendChild(ifr);
        //        if (KE.g[id].ownerUserId)
        //            src += '&userid=' + KE.g[id].ownerUserId;

        try {
            var str = '<body style="width:100%;height:100%;background:url(' + root + '/max-templates/default/images/ajaxloading_big.gif) no-repeat 50% 45%;">&nbsp;</body>';
            var doc = KE.util.getIframeDoc(ifr);
            doc.open();
            doc.write(str);
            doc.close();
        }
        catch (e) {

        }
        if (width) ifr.width = width;
        if (height) ifr.height = height;
        ifr.src = src;
        KE.panel[id].popup[cmd] = p;
        p.show();
    }
};

KE.panel.hide = function(id, cmd) {

    var pc = KE.panel[id];

    if (!pc) return;
    pc = pc.popup;

    if (!pc) return;
    if (cmd) { var p = pc[cmd]; if (p) p.hide(); return; }
    for (var pk in pc) { pc[pk].hide(); }
}


KE.plugin['insertorderedlist'] = {
    click: function(id) {
        if (!KE.browser.IsIE)
            KE.util.execCommand(id, 'insertorderedlist', null);
        else {
            var doc = KE.g[id].iframeDoc;
            KE.util.selection(id);
            var r = doc.selection.createRange();
            var c;
            //var c = startText + r.htmlText + endText;
            var items = r.htmlText.split(/<br\s*\/?>/ig);
            c = "<ol>";
            for (var i = 0; i < items.length; i++)
                c += "<li>" + items[i] + "</li>";
            c += "</ol>";
            r.pasteHTML(c);
        }
    },
    ubbClick: function(id) {
        //  KE.util.execUbb(id, 'insertorderedlist', null);
    }
};

KE.plugin['insertunorderedlist'] = {
    click: function(id) {
        if (!KE.browser.IsIE)
            KE.util.execCommand(id, 'insertunorderedlist', null);
        else {
            var doc = KE.g[id].iframeDoc;
            KE.util.selection(id);
            var r = doc.selection.createRange();
            var c;
            //var c = startText + r.htmlText + endText;
            var items = r.htmlText.split(/<br\s*\/?>/ig);
            c = "<ul>";
            for (var i = 0; i < items.length; i++)
                c += "<li>" + items[i] + "</li>";
            c += "</ul>";
            r.pasteHTML(c);
        }
    },
    ubbClick: function(id) {

    }
};



function ubb2html(sUBB) {
    var i, sHtml = sUBB;
    sHtml = sHtml.replace(/&(?![a-z]{2,4};)/ig, "&amp;");
    sHtml = sHtml.replace(/</g, '&lt;');
    sHtml = sHtml.replace(/</g, '&lt;');
    sHtml = sHtml.replace(/>/g, '&gt;');
    sHtml = sHtml.replace(/\x20/g, '&nbsp;'); //空格处理， 此处没有判断空格是否是处于UBB属性内部
    sHtml = sHtml.replace(/\[quote\](?:\r?\n)+/ig, "[quote]\r\n");
    sHtml = sHtml.replace(/(?:\r?\n)+\[\/quote\]/ig, "\r\n[/quote]");
    sHtml = sHtml.replace(/\r?\n/g, "<br />");
    //sHtml = sHtml.replace(/ /g,'&nbsp;');

    sHtml = sHtml.replace(/\[code\](.*?)\[\/code\]/ig, function(all, code) {
        var c = code.replace(/\[/g, "{[}");
        c = c.replace(/\]/g, "{]}");
        c = "[code]" + c + "[/code]";
        return c;
    });

    sHtml = sHtml.replace(/\[(\/?)(b|u|s|sup|sub)\]/ig, '<$1$2>');
    sHtml = sHtml.replace(/\[(\/?)i\]/ig, '<$1em>');
    var regColor = /\[color\s*=\s*([^\]]+?)\](.*?)\[\/color\]/ig;
    var regFont = /\[font\s*=\s*([^\]]+?)\](.*?)\[\/font\]/ig;
    var regSize = /\[size\s*=\s*([^\]]+?)\](.*?)\[\/size\]/ig;
    var regBgColor = /\[bgcolor\s*=\s*([^\]]+?)\](.*?)\[\/bgcolor\]/ig;

    while (sHtml.match(regColor)) sHtml = sHtml.replace(regColor, '<font color="$1">$2</font>');
    while (sHtml.match(regFont)) sHtml = sHtml.replace(regFont, '<font face="$1">$2</font>');
    while (sHtml.match(regSize)) {
        sHtml = sHtml.replace(regSize, function(all, size, text) {
            if (/\d+$/.test(size)) {
                return '<font size="' + size + '">' + text + '</font>';
            }
            return '<span style="font-size:' + size + '">' + text + '</span>';
        });
    }

    while (sHtml.match(regBgColor)) sHtml = sHtml.replace(regBgColor, '<span style="background-color:$1;">$2</span>');

    for (i = 0; i < 3; i++) sHtml = sHtml.replace(/\[align\s*=\s*([^\]]+?)\](((?!\[align(?:\s+[^\]]+)?\])[\s\S])*?)\[\/align\](?:<br\s*\/?>)?/ig, '<div style="text-align:$1">$2</div>');

    if (window.beforeProcessUbbImg) sHtml = window.beforeProcessUbbImg(sHtml);

    sHtml = sHtml.replace(/\[img\]\s*([\s\S]+?)\s*\[\/img\]/ig, '<img src="$1" />');
    sHtml = sHtml.replace(/\[img\s*=\s*(\d+),(\d+)\s*\]\s*([\s\S]+?)\s*\[\/img\]/ig, '<img src="$3" width="$1" height="$2" />');
    sHtml = sHtml.replace(/\[url\]\s*([\s\S]+?)\s*\[\/url\]/ig, '<a href="$1">$1</a>');
    sHtml = sHtml.replace(/\[url\s*=\s*([^\]\s]+?)\s*\]\s*([\s\S]+?)\s*\[\/url\]/ig, '<a href="$1">$2</a>');
    sHtml = sHtml.replace(/\[email\]\s*([\s\S]+?)\s*\[\/email\]/ig, '<a href="mailto:$1">$1</a>');
    sHtml = sHtml.replace(/\[email\s*=\s*([^\]\s]+?)\s*\]\s*([\s\S]+?)\s*\[\/email\]/ig, '<a href="mailto:$1">$2</a>');
    sHtml = sHtml.replace(/\[indent\]([\s\S]*?)\[\/indent\]/ig, '<blockquote>$1</blockquote>');
    sHtml = KE.util.ubb2flag(sHtml);

    sHtml = sHtml.replace(/\[table(?:\s*=\s*(\d{1,4}%?)\s*(?:,\s*([^\]]+)\s*)?)?]/ig, function(all, w, b) {
        var str = '<table';
        if (w) str += ' width="' + w + '"';
        if (b) str += ' bgcolor="' + b + '"';
        return str + ' border="1">';
    });

    sHtml = sHtml.replace(/\[tr(?:\s*=(\s*[^\]]+))?\]/ig, function(all, bg) {
        return '<tr' + (bg ? ' bgcolor="' + bg + '"' : '') + '>';
    });
    sHtml = sHtml.replace(/\[td\s*=\s*([\d,]+)\]/ig, function(all, attr) {
        var temp = attr.split(',');
        var html = '<td';
        if (temp.length == 1)
            html += ' width="' + temp[0] + '"';
        else if (temp.length > 1) {
            html += ' colspan="' + temp[0] + '"';
            html += ' rowspan="' + temp[1] + '"';
            if (temp.length > 2)
                html += ' width="' + temp[2] + '"';
        }
        return html + '>';
    });

    sHtml = sHtml.replace(/\[(table|tr|td)\]/ig, "<$1>");
    sHtml = sHtml.replace(/\[\/?(table|tr|td)\]/ig, "</$1>");
    sHtml = sHtml.replace(/\s*\[\*\](.*?)<br\s*\/?>/ig, '<li>$1</li>');

    var regList = /\[list(.*?)\](.*?)\[\/list\]/ig;
    while (sHtml.match(regList)) {
        sHtml = sHtml.replace(regList, function(all, type, text) {
            if (type && type == '=1') {
                return "<ol>" + text + "</ol>";
            }
            return "<ul>" + text + "</ul>";
        });
    }

    sHtml = sHtml.replace(/\{\[\}/g, "[");
    sHtml = sHtml.replace(/\{\]\}/g, "]");
    if (window.emoticon2Html) sHtml = emoticon2Html(sHtml); //************表情切换
    return sHtml;
}

/*============================以下是 HTML转UBB转换器================================*/

/**公共变量*/
var mapSize = {}; //字体大小映射表
mapSize["xx-small"] = "1";
mapSize["small"] = "3";
mapSize["medium"] = "4";
mapSize["x-small"] = "2";
mapSize["large"] = "5";
mapSize["x-large"] = "6";
mapSize["xx-large"] = "7";

mapSize["6pt"] = "1";
mapSize["6.5pt"] = "1";
mapSize["7pt"] = "1";
mapSize["8pt"] = "2";
mapSize["9pt"] = "2";
mapSize["10pt"] = "2";
mapSize["11pt"] = "2";
mapSize["12pt"] = "3";
mapSize["13pt"] = "3";
mapSize["14pt"] = "4";
mapSize["15pt"] = "4";
mapSize["16pt"] = "4";
mapSize["17pt"] = "5";
mapSize["18pt"] = "5";
mapSize["19pt"] = "5";
mapSize["20pt"] = "6";
mapSize["21pt"] = "6";
mapSize["22pt"] = "6";
mapSize["24pt"] = "6";
mapSize["25pt"] = "7";
mapSize["36pt"] = "7";

mapSize["7px"] = "1";
mapSize["9px"] = "1";
mapSize["10px"] = "1";
mapSize["11px"] = "1";
mapSize["12px"] = "1";
mapSize["13px"] = "1";
mapSize["14px"] = "2";
mapSize["15px"] = "2";
mapSize["16px"] = "2";
mapSize["17px"] = "3";
mapSize["18px"] = "3";
mapSize["19px"] = "3";
mapSize["20px"] = "4";
mapSize["21px"] = "4";
mapSize["22px"] = "4";
mapSize["23px"] = "5";
mapSize["24px"] = "5";
mapSize["25px"] = "5";
mapSize["26px"] = "6";
mapSize["27px"] = "6";
mapSize["28px"] = "6";
mapSize["36px"] = "7";

/**/
/*********************Html Tag Class*****************************/
function HtmlTagBase(xmlNode) {
    this.attributes = {};
    this.Pair = null;
    this.StartUbb = '';
    this.EndUbb = '';
    this.Styles = {};
    this.isBlockTag = false;


    this.TagName = xmlNode.tagName;
    var attmap = xmlNode.attributes;
    var l = attmap.length;
    var v, k, a;
    for (var i = 0; i < l; i++) {
        a = attmap.item(i);
        k = a.nodeName;
        v = a.value;
        if (v) {
            if (k.indexOf('color') >= 0)
                v = Util.ConvertRGB(v);
            if (k == "src" || k == "href") {
                v = v.replace(/\[/g, "%5B");
                v = v.replace(/\]/g, "%5D");
            }
            this.attributes[k] = v;
        }
    }


    if (this.attributes["style"]) {
        var styleSet = (this.attributes["style"] + "").split(';');
        var v;
        for (var i = 0; i < styleSet.length; i++) {
            var styleItem = styleSet[i].split(':');
            if (styleItem.length == 2) {
                var key = styleItem[0].trim().toLowerCase();
                if (!this.Styles[key]) {
                    v = styleItem[1].trim().trim("'");
                    if (key.indexOf('color') >= 0)
                        v = Util.ConvertRGB(v);
                    this.Styles[key] = v;
                }
            }
        }
    }
    //  }
}

/// <summary>
/// 从此类继承的标签会解析样式表
/// </summary>
function HasStyleTag(xmlNode) {
    HtmlTagBase.call(this, xmlNode);
    process.call(this);

    var _startUbb;
    var _endUbb;

    function process() {
        var startUbb = '', endUbb = '';
        if (this.Styles["color"]) {
            startUbb += "[color=" + this.Styles["color"] + "]";
            endUbb = "[/color]" + endUbb;
        }

        if (this.Styles["font-size"]) {
            var size = this.Styles["font-size"].toString();
            if (mapSize[size])
                startUbb += "[size=" + mapSize[size] + "]";
            else {
                size = size.replace(/(\d+)\.\d+(\D+)/, "$1$2");
                startUbb += "[size=" + mapSize[size] + "]";
            }

            endUbb = "[/size]" + endUbb;
        }

        if (this.Styles["font-family"]) {
            startUbb += "[font=" + this.Styles["font-family"] + "]";
            endUbb = "[/font]" + endUbb;
        }

        if (this.Styles["font-style"]) {
            var fontStyle = this.Styles["font-style"] + "";
            if (fontStyle == "italic") {
                startUbb += "[i]";
                endUbb = "[/i]" + endUbb;
            }
        }

        if (this.Styles["font-weight"]) {
            var fontWeight = this.Styles["font-weight"] + "";
            if (fontWeight != "normal") {
                startUbb += "[b]";
                endUbb = "[/b]" + endUbb;
            }
        }

        if (this.Styles["text-decoration"]) {
            var dec = this.Styles["text-decoration"] + "";
            if (dec == "underline") {
                startUbb += "[u]";
                endUbb = "[/u]" + endUbb;
            }
            else if (dec == "line-through") {
                startUbb += "[s]";
                endUbb = "[/s]" + endUbb;
            }
        }

        if (this.Styles["text-align"]) {
            var align = this.Styles["text-align"].toString();
            if (align == "left" || align == "right" || align == "center") {
                startUbb += "[align=" + align + "]";
                endUbb = "[/align]" + endUbb;
            }
        }

        if (this.Styles["background-color"]) {
            startUbb += "[bgcolor=" + this.Styles["background-color"] + "]";
            endUbb = "[/bgcolor]" + endUbb;
        }

        this.StartUbb = startUbb;
        this.EndUbb = endUbb;
    }
}


/// <summary>
/// 成对出现的标签比如：tr,td,i,b等，无需解析属性和样式
/// </summary>
function PairTag(xmlNode) {
    HtmlTagBase.call(this, xmlNode);
    this.UbbTag = this.TagName;
    this.StartUbb = "[" + this.UbbTag + "]";
    this.EndUbb = "[/" + this.UbbTag + "]";
}

function TdTag(xmlNode) {
    HtmlTagBase.call(this, xmlNode);
    var w, c, r;
    w = this.attributes["width"];
    c = this.attributes["colspan"];
    r = this.attributes["rowspan"];
    var ubb = "[td";
    if ((c || r) && (r != "1" || r != "1")) {
        ubb += "=" + (c ? c : "1") + "," + (r ? r : "1");
        if (w) {
            ubb += "," + w;
        }
    }
    else if (w) {
        ubb += "=" + w;
    }

    this.StartUbb = ubb + "]";
    this.EndUbb = "[/td]";
}

function ImgTag(xmlNode) {
    HtmlTagBase.call(this, xmlNode);
    var width = 0, height = 0;
    var w = this.Styles["width"] || this.attributes["width"];
    var h = this.Styles["height"] || this.attributes["height"];

    if (w) width = parseInt(w);
    if (h) height = parseInt(h);

    var ubbTex = "[img";
    if (width > 0) {
        ubbTex += "=" + width;
        if (height > 0)
            ubbTex += "," + height;
    }
    ubbTex += "]" + this.attributes["src"] + "[/img]";
    this.StartUbb = ubbTex;
}

function ATag(xmlNode) {
    var emptyHref = new RegExp("^#+$");
    HtmlTagBase.call(this, xmlNode);

    this.Href = this.attributes["href"];
    var isEmptyHref = false;
    if (!this.Href || emptyHref.test(this.Href.trim())) {
        isEmptyHref = true;
    }

    if (isEmptyHref) {
        this.StartUbb = '';
        this.EndUbb = '';
    }
    else {
        if (this.Href.indexOf("mailto:") > -1) {
            var es = this.Href.indexOf("mailto:") + 7;
            this.Href = this.Href.substr(es, this.Href.length - es);
            this.StartUbb = "[email=" + this.Href + "]";
            this.EndUbb = "[/email]";
        }
        else {
            this.StartUbb = "[url=" + this.Href + "]";
            this.EndUbb = "[/url]";
        }
    }
}

function FontTag(xmlNode) {
    HasStyleTag.call(this, xmlNode);

    if (this.attributes["color"]) {
        this.StartUbb += "[color=" + this.attributes["color"] + "]";
        this.EndUbb = "[/color]" + this.EndUbb;
    }
    if (this.attributes["size"]) {
        var size = this.attributes["size"];
        this.StartUbb += "[size=" + (mapSize[size] ? mapSize[size].toString() : size) + "]";
        this.EndUbb = "[/size]" + this.EndUbb;
    }

    if (this.attributes["face"]) {
        this.StartUbb += "[font=" + this.attributes["face"] + "]";
        this.EndUbb = "[/font]" + this.EndUbb;
    }

}

function SpanTag(xmlNode) {
    HasStyleTag.call(this, xmlNode);
}

function BrTag(xmlNode) {
    HtmlTagBase.call(this, xmlNode);
    this.StartUbb = "\r\n";
}

function DivTag(xmlNode) {
    HasStyleTag.call(this, xmlNode);
    if (this.attributes['align']) {
        this.StartUbb = "[align=" + this.attributes['align'] + "]" + this.StartUbb;
        this.EndUbb += "[/align]";
    }
//    if (KE.browser.IsSafari) { //此处为解决Webkit 内核浏览器的第一个换行丢失的问题（webkit的换行是DIV）
//        this.StartUbb = "\r\n" + this.StartUbb;
//    }
//    else {
//        this.EndUbb += "\r\n";
//    }

    this.isBlockTag = true;
}

function PTag(xmlNode) {
    HasStyleTag.call(this, xmlNode);
    if (this.attributes['align']) {
        this.StartUbb = "[align=" + this.attributes['align'] + "]" + this.StartUbb;
        this.EndUbb += "[/align]";
    }
    this.isBlockTag = true;
//    this.EndUbb += "\r\n";
}

function Center(index, tagHtml, begin, xmlNode) {
    HtmlTagBase.call(this, xmlNode);
    this.StartUbb = "[align=center]";
    this.EndUbb = "[/align]";
}

function TableTag(xmlNode) {
    HtmlTagBase.call(this, xmlNode);
    var ubbText = "[table";
    ubbText += "]";
    this.StartUbb = ubbText;
    this.EndUbb = "[/table]";
}

function LiTag(xmlNode) {
    HtmlTagBase.call(this, xmlNode);
    this.StartUbb = "[*]";
    this.EndUbb = '\r\n';
}

function ListTag(xmlNode) {
    HtmlTagBase.call(this, xmlNode);
    this.EndUbb = "[/list]";
    var ubbText = "[list";
    if (this.TagName == 'ol')
        ubbText += "=1";
    else if (this.Styles["list-style-type"] == 'decimal') {
        ubbText += "=1";
    }
    ubbText += "]";
    this.StartUbb = ubbText;
}

function HTag(xmlNode) {
    HtmlTagBase.call(this, xmlNode);
    var size = parseInt(this.TagName.substring(1));
    size = 7 - size;
    this.StartUbb = "[size=" + size + "][b]";
    this.EndUbb = "[/b][/size]";
}

function EmbedTag(xmlNode) {
    HtmlTagBase.call(this, xmlNode);

    Src = this.attributes["src"] + "";
    var TypeOfFlash = "application/x-shockwave-flash";
    var TypeOfWMedia = "application/x-mplayer2";
    var TypeOfRealMedia = "audio/x-pn-realaudio-plugin";
    var fileTypeReg = /\.(\w+)\s*$/g;

    this.Type = this.attributes["type"] + "";
    width = 550;
    if (!isNaN(this.attributes["width"])) {
        width = parseInt(this.attributes["width"]);
    }
    var height = 400;
    if (!isNaN(this.attributes["height"])) {
        height = parseInt(this.attributes["height"]);
    }

    AutoStart = false;
    if (this.attributes["autostart"]) {
        var auto = this.attributes["autostart"].toString();
        if (auto == "1" || auto == "true")
            AutoStart = true;
    }

    var ubbText = '';
    var fileType = '';
    var tag = '';

    if (fileTypeReg.test(Src)) {
        fileType = fileTypeReg.exec(Src)[1];
    }

    switch (Type) {

        case TypeOfFlash:          //flash
            tag = "flash";
            break;

        case TypeOfWMedia:          //windows media
            tag = "wmv";
            switch (fileType) {
                case "mp3":
                case "wav":
                case "wma":
                case "midi":
                    tag = "mp3";
                    break;
            }
            break;
        case TypeOfRealMedia:       //realmedia
            tag = "rm";
            switch (fileType) {
                case "ra":
                    tag = "rm";
                    break;
            }
            break;
    }

    this.StartUbb = tag ? String.format("[{0}={3},{4},{2}]{1}[/{0}]", tag, Src, this.AutoStart ? "1" : "0", width, height) : "";
}

var Util = new function() {
    this.ConvertRGB = function(htmltext) {
        //替换 rgb(255,255,255)
        var regRGB = new RegExp("rgb\\s*\\(\\s*(\\d+),\\s*(\\d+),\\s*(\\d+)\\s*\\)", "ig");

        return htmltext.replace(regRGB, function(all, r, g, b) {
            var cr, cb, cg;
            cr = parseInt(r).toString(16);
            if (cr.length < 2) cr = '0' + cr;
            cg = parseInt(g).toString(16);
            if (cg.length < 2) cg = '0' + cg;
            cb = parseInt(b).toString(16);
            if (cb.length < 2) cb = '0' + cb;
            return '#' + cr + cg + cb;
        });
    }
}


/********** Converter **********/

var factoryUbbTag = function(tagIndex, tagName, html, charIndex, xmlNode) {
    var t = null;
    switch (tagName.toLowerCase()) {
        case "font":
            t = new FontTag(xmlNode);
            break;
        case "span":
            t = new SpanTag(xmlNode);
            break;
        case "ol":
        case "ul":
            t = new ListTag(xmlNode);
            break;
        case "li":
            t = new LiTag(xmlNode);
            break;
        case "blockquote":
            t = new PairTag(xmlNode);
            t.StartUbb = "[indent]"; t.EndUbb = "[/indent]"
            break;
        case "div":
            t = new DivTag(xmlNode);
            break;
        case "table":
            t = new TableTag(xmlNode);
            break;
        case "a":
            t = new ATag(xmlNode);
            break;
        case "img":
            t = new ImgTag(xmlNode);
            break;
        case "p":
            t = new PTag(xmlNode);
            break;
        case "td":
        case "th":
            t = new TdTag(xmlNode);
            break;
        case "tr":
            t = new PairTag(xmlNode);
            break;
        case "br":
            t = new BrTag(xmlNode);
            break;
        case "s":
        case "b":
        case "i":
        case "sub":
        case "sup":
        case "u":
            t = new PairTag(xmlNode);
            break;
        case "strong":
            t = new PairTag(xmlNode);
            t.StartUbb = "[b]"; t.EndUbb = "[/b]"
            break;
        case "em":
            t = new PairTag(xmlNode);
            t.StartUbb = "[i]"; t.EndUbb = "[/i]"
            break;
        case "strike":
            t = new PairTag(xmlNode);
            t.StartUbb = "[s]"; t.EndUbb = "[/s]"
            break;
        case "h1":
        case "h2":
        case "h3":
        case "h4":
        case "h5":
        case "h6":
            t = new HTag(xmlNode);
            break;
        case "embed":
            t = new EmbedTag(xmlNode);
            break;
    }
    return t;
}

var UbbConverter = new function () {

    this.ConvertToUbb = function (htmlContent) {

        var regStyle = new RegExp("<style.*?>.*?</style>", "ig");   //清除样式
        var regScript = new RegExp("<script[^>]*?>.*?</s" + "cript>", "ig"); //清除脚本
        htmlContent = htmlContent.replace(/\n/g, '');
        htmlContent = htmlContent.replace(/\r/g, '');
        htmlContent = htmlContent.replace(regScript, '');
        htmlContent = htmlContent.replace(regStyle, '');
        htmlContent = htmlContent.replace(/&nbsp;/ig, ' ');
        htmlContent = htmlContent.replace(/&(?!amp;)/ig, "&amp;");
        htmlContent = htmlContent.trim();

        htmlContent = '<?xml version="1.0"?><root>' + htmlContent + "</root>";
        var doc;
        if (KE.browser.IsIE) {
            try {
                doc = new ActiveXObject("Msxml2.DOMDocument");
            }
            catch (e) {
                doc = new ActiveXObject("Msxml.DOMDocument");
            }
            doc.loadXML(htmlContent);
        }
        else {
            doc = (new DOMParser()).parseFromString(htmlContent, "text/xml");
        }
        var tagList = [];
        var results = [];
        var tagIndex = 0;
        var lastIsBlock = false;
        var tagIndex = 0;
        var processNode = function (node) {

            if (node.nodeType == 1) {
                var ubbTag = factoryUbbTag(tagIndex++, node.tagName, "", 0, node);
                if (ubbTag != null) {
                    if (ubbTag.isBlockTag && tagIndex > 0) {
                        if (lastIsBlock == false) {
                            results.push("\r\n");
                        }
                    }
                    results.push(ubbTag.StartUbb);
                }

                if (node.childNodes.length > 0) {
                    var l = node.childNodes.length;
                    for (var i = 0; i < l; i++) {
                        processNode(node.childNodes[i]);
                    }
                }

                if (ubbTag != null) {
                    results.push(ubbTag.EndUbb);
                    if (ubbTag.isBlockTag)
                        results.push("\r\n");

                    lastIsBlock = ubbTag.isBlockTag;
                }
                tagIndex++;
            }
            else if (node.nodeType == 3) {
                results.push(node.nodeValue);
            }
        }

        var r = doc.getElementsByTagName("root")[0];
        processNode(r);
        var resultUBB = results.join("");
        resultUBB = resultUBB.replace(/&amp;/ig, '&');
        resultUBB = resultUBB.replace(/&gt;/ig, '>');
        resultUBB = resultUBB.replace(/&lt;/ig, '<');
        return resultUBB;
    }
}

function html2ubb(content) {
    content = KE.util.html2Flag(content);  //多媒体切换
    content = KE.util.flag2Ubb(content);   //多媒体切换
    if (window.emoticon2Ubb) content = emoticon2Ubb(content); //-----------表情切换
    if (window.beforeProcessImg) content = window.beforeProcessImg(content); //附件图片切换
    content = UbbConverter.ConvertToUbb(content);  //HTML--->UBB切换
    content = content.replace(/\[quote\](?:\r?\n)+/ig, "[quote]\r\n");
    content = content.replace(/(?:\r?\n)+\[\/quote\]/ig, "\r\n[/quote]");
    return content;
}