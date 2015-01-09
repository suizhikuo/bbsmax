function createQuicklyEditor(id) {
    smallEditorID = id;
    var ehtml = '<div class="quickpost-editor">\
    <div class="clearfix quickpost-editortool" id="editortool">\
        <div class="quickpost-editorbutton">\
            <a class="fontname" id="' + smallEditorID + '_font" href="javascript:void(0);" title="字体">字体</a>\
            <a class="fontsize" id="' + smallEditorID + '_size" href="javascript:void(0);" title="字号">字号</a>\
            <a class="textcolor" id="' + smallEditorID + '_color" href="javascript:void(0);" onclick="mx_editor2_' + smallEditorID + '.createRange();" title="颜色">颜色</a>\
            <a class="bold" id="' + smallEditorID + '_bold" href="javascript:void(0);" onclick="mx_editor2_' + smallEditorID + '.createUbb(\'[b]\',\'[/b]\')" title="加粗">加粗</a>\
            <a class="italic" id="' + smallEditorID + '_italic" href="javascript:void(0);" onclick="mx_editor2_' + smallEditorID + '.createUbb(\'[i]\',\'[/i]\')" title="斜体">斜体</a>\
            <a class="link" id="' + smallEditorID + '_link" href="javascript:void(0);" onclick="mx_editor2_' + smallEditorID + '.insertLink()" title="链接">链接</a>\
            <a class="image" id="' + smallEditorID + '_image" href="javascript:void(0);" onclick="mx_editor2_' + smallEditorID + '.insertImage()" title="图片">图片</a>\
            <a class="quote" id="' + smallEditorID + '_quote" href="javascript:void(0);" onclick="mx_editor2_' + smallEditorID + '.createUbb(\'[quote]\',\'[/quote]\')" title="引用">引用</a>\
            <a class="emoticon" id="' + smallEditorID + '_face" href="javascript:void(mx_editor2_' + smallEditorID + '.insertFace());" title="表情">表情</a>\
        </div>\
    </div>\
    <div class="quickpost-editorenter">\
        <textarea name="' + smallEditorID + '" id="' + smallEditorID + '_area" cols="65" rows="5"></textarea>\
    </div>\
</div>\
<div id="font_' + smallEditorID + '" class="quickpost-menu editor-font" style="display:none;" onclick="this.style.display=\'none\';">\
<script type="text/javascript">\
    var _fTable = {"SimSun": "宋体","SimHei": "黑体","FangSong_GB2312": "仿宋体","KaiTi_GB2312": "楷体","NSimSun": "新宋体","Arial": "Arial","Arial Black": "Arial Black","Times New Roman": "Times New Roman","Courier New": "Courier New","Tahoma": "Tahoma","Verdana": "Verdana"};\
    document.write("<ul>");\
    for (var k in _fTable) {document.write(\'<li onclick="mx_editor2_' + smallEditorID + '.createUbb(\\\'[font=\' + k + \']\\\',\\\'[/font]\\\')"><a href="javascript:;" style="font-family:\'+_fTable[k]+\'">\' + _fTable[k] + "</a></li>")}\
    document.write("</ul>");\
</scr' + 'ipt>\
</div>\
<div id="size_' + smallEditorID + '" class="quickpost-menu editor-size" style="display:none;" onclick="this.style.display=\'none\';">\
<script type="text/javascript">\
    document.write("<ul>");\
    for (var i = 1; i <= 7; i++) {document.write(\'<li onclick="mx_editor2_' + smallEditorID + '.createUbb(\\\'[size=\' + i + \']\\\',\\\'[/size]\\\')"><font size="\' + i + \'"><a href="javascript:;">\' + i + "号</a></font></li>")}\
    document.write("</ul>");\
</scr' + 'ipt>\
</div>\
<div id="color_' + smallEditorID + '" class="quickpost-menu editor-palette" style="display:none;" onclick="this.style.display=\'none\';">\
<script type="text/javascript">\
    var _cTable = [\
    ["#FFFFFF", "#E5E4E4", "#D9D8D8", "#C0BDBD", "#A7A4A4", "#8E8A8B", "#827E7F", "#767173", "#5C585A", "#000000"],\
    ["#FEFCDF", "#FEF4C4", "#FEED9B", "#FEE573", "#FFED43", "#F6CC0B", "#E0B800", "#C9A601", "#AD8E00", "#8C7301"],\
    ["#FFDED3", "#FFC4B0", "#FF9D7D", "#FF7A4E", "#FF6600", "#E95D00", "#D15502", "#BA4B01", "#A44201", "#8D3901"],\
    ["#FFD2D0", "#FFBAB7", "#FE9A95", "#FF7A73", "#FF483F", "#FE2419", "#F10B00", "#D40A00", "#940000", "#6D201B"],\
    ["#FFDAED", "#FFB7DC", "#FFA1D1", "#FF84C3", "#FF57AC", "#FD1289", "#EC0078", "#D6006D", "#BB005F", "#9B014F"],\
    ["#FCD6FE", "#FBBCFF", "#F9A1FE", "#F784FE", "#F564FE", "#F546FF", "#F328FF", "#D801E5", "#C001CB", "#8F0197"],\
    ["#E2F0FE", "#C7E2FE", "#ADD5FE", "#92C7FE", "#6EB5FF", "#48A2FF", "#2690FE", "#0162F4", "#013ADD", "#0021B0"],\
    ["#D3FDFF", "#ACFAFD", "#7CFAFF", "#4AF7FE", "#1DE6FE", "#01DEFF", "#00CDEC", "#01B6DE", "#00A0C2", "#0084A0"],\
    ["#EDFFCF", "#DFFEAA", "#D1FD88", "#BEFA5A", "#A8F32A", "#8FD80A", "#79C101", "#3FA701", "#307F00", "#156200"],\
    ["#D4C89F", "#DAAD88", "#C49578", "#C2877E", "#AC8295", "#C0A5C4", "#969AC2", "#92B7D7", "#80ADAF", "#9CA53B"]\
];\
    document.write(\'<table cellpadding="0" cellspacing="0">\');\
    for (var i = 0; i < _cTable.length; i++) {\
        document.write("<tr>");\
        for (var j = 0; j < _cTable[i].length; j++) {\
            document.write(\'<td style="font-size:0;height:12px;width:12px;background:\' + _cTable[i][j] + \'" onclick="mx_editor2_' + smallEditorID + '.createUbb(\\\'[color=\' + _cTable[i][j] + \']\\\',\\\'[/color]\\\')">&nbsp;</td>\');\
        }\
        document.write("</tr>");\
    }\
    document.write("</table>");\
</sc' + 'ript>\
</div>\
<div id="link_' + smallEditorID + '" class="quickpost-menu editor-link" style="display:none;">\
    <div class="editor-link-inner">\
        <h3 class="label"><label for="' + smallEditorID + '_url">链接地址</label></h3>\
        <div class="clearfix editor-link-enter">\
            <input class="text" id="' + smallEditorID + '_url" value="http://" type="text" />\
        </div>\
        <div class="clearfix editor-link-submit">\
            <span class="minbtn-wrap"><span class="btn"><input class="button" type="button" id="' + smallEditorID + '_urlok" value="确定" /></span></span>\
        </div>\
    </div>\
</div>';
    document.write(ehtml);
    function mxeditor2(id) {
        new popup("font_" + id, id + "_font", 0, '', "top");
        new popup("size_" + id, id + "_size", 0, '', "top");
        new popup("color_" + id, id + "_color", 0, '', "top");
        new popup("link_" + id, [id + "_image", id + "_link"], 0, '', "top");
        //onCtrlEnter("editor", clickPostButton);
        this.linkType = 0;
        this.id = id;
        this.createRange = function() {
            if (document.all) {
                var t = $(this.id + "_area");
                t.focus();
                //this.range = t.createTextRange();
                this.range = document.selection.createRange();
            }
        }
        this.createUbb = function(ubb1, ubb2, empty) {
            var text = $(this.id + "_area");
            var s = "";
            text.focus();
            if (document.all) {
                var range;
                if (this.range)
                    range = this.range;
                else {
                    range = document.selection.createRange();
                    this.range = range;
                }

                range.select();
                s = range.text;
            }
            else {
                var i1 = text.selectionStart,
                i2 = text.selectionEnd, i3;
                var len = text.value.length;
                var txt3 = text.value.substr(i1, i2 - text.selectionStart);
                s = txt3;
            }
            if (!s && empty) s = empty;
            this.insertContent(ubb1 + s + ubb2,!s? ubb2.length:0);
        };

        var insertUrl = function(id) {
            var uo = $(id + '_url')
            var url = uo.value;
            var t = window[smallEditorID + "_" + id];
            if (t.linkType == 1)
                t.insertContent('[img]' + url + '[/img]');
            else
                t.createUbb('[url=' + url + ']', '[/url]', url);
            uo.value = "http://";
        };

        this.insertLink = function() {
            this.createRange();
            this.linkType = 0;
            var varthis = smallEditorID + "_" + this.id;
            window[varthis] = this;

            $(id + "_urlok")["onclick"] = function() { $("link_" + id).style.display = "none"; insertUrl(window[varthis].id); };
        };
        this.insertImage = function() {
            this.createRange();
            this.linkType = 1;
            var varthis = smallEditorID + "_" + this.id;
            window[varthis] = this;
            $(id + "_urlok")["onclick"] = function() { $("link_" + id).style.display = "none"; insertUrl(window[varthis].id); };
        };


        this.insertFace = function() {
            this.createRange();
            var th = this;
            var t = $(smallEditorID + "_face");
            window.facePanel = openPanel(root + '/max-dialogs/editor/emoticons.aspx?id='+ smallEditorID +'&ispanel=true&from=quickreply', t, "", 391, 310, "top"); //算好了 391刚刚好，实在没办法
            addHandler(document.documentElement, "click", hidePanel);

            function hidePanel() {
                if (window.facePanel) window.facePanel.close();
                removeHandler(document.documentElement, "click", hidePanel);
            }
            //openDialog(url, $(this.id + "_face"), function(r) { th.insertContent(r.ubb); });
        };

        var closeDialog = function() {
            var dialog = openDialog.obj;
            dialog.close();
            removeHandler(document.documentElement, "click", closeDialog);
        };

        // 插入内容到textarea当前位置
        //content :要插入的内容
        //cursor: 如果有指定这个参数，代表光标后退多少字符
        this.insertContent = function (content, cursor) {
            var text = $(this.id + "_area");
            text.focus();
            if (document.all) {
                var range = this.range;
                range.text = content;
                range.collapse(false);
                this.range = null;
                var newRange = document.selection.createRange();

                if (cursor) {
                    newRange.move("character", 0 - cursor);
                }
                else {
                    newRange.move("character", content.length);
                }
                newRange.collapse(true);       //折叠选区
                newRange.select();             //显示光标
            }
            else {
                var i1 = text.selectionStart,  //i1起始位置
                i2 = text.selectionEnd, i3;    //i2选中结束位置  
                //i3光标最终要停留的位置         
                var len = text.value.length;
                var txt1 = text.value.substr(0, i1);
                var txt2 = text.value.substr(i2, len - i2);
                text.value = txt1 + content + txt2
                i3 = i1 + content.length;
                i3 = cursor ? i3 - cursor : i3;
                text.selectionStart = i3;
                text.selectionEnd = i3;
            }
        };
    }

    window["mx_editor2_" + smallEditorID] = new mxeditor2(smallEditorID);
}