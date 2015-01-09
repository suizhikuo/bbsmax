<script type="text/javascript" src="$root/max-assets/kindeditor/kindeditor.js"></script>
<!--[if $Emoticons]--><script type="text/javascript" src="$url(handler/emotelib)"></script><!--[/if]-->
<div class="webeditor" style="width:$width;" id="ke_container_$Id">
    <div class="webeditor-inner">
        <div class="webeditor-toolbar" id="ke_toolbar_$Id">
            <div class="clearfix webeditor-toolbar-inner">
            <!--[loop $bg in $SmallToolbarGroup with $i]-->
                <div class="smallbuttons">
                <!--[loop $item in $bg]-->
                    <!--[if $item!="-"]--><a href="javascript:void(0)" onclick="KE.util.click('$Id','$item')" id="ME_$item" class="$item"><b>.</b></a><!--[else]--><span class="linebreak">-</span><!--[/if]-->
                <!--[/loop]-->
                </div>
            <!--[/loop]-->
                <div class="largebuttons">
                    <!--[if $Emoticons]--><a href="javascript:void(0)" onclick="KE.util.click('$Id','emoticons')" id="ME_emoticons" class="emoticons"><em>.</em><b id="ME_Label_emoticons">表情</b></a><!--[/if]-->
                    <!--[if $Photo]--><a href="javascript:void(0)" onclick="KE.util.click('$Id','photo')" id="ME_photo" class="photo"><em>.</em><b id="ME_Label_photo">图片</b></a><!--[/if]-->
                    <!--[if $Attachment]--><a href="javascript:void(0)" onclick="KE.util.click('$Id','attachment')" id="ME_attachment" class="attachment"><em>.</em><b id="ME_Label_attachment">附件</b></a><!--[/if]-->
                    <!--[if $Video]--><a href="javascript:void(0)" onclick="KE.util.click('$Id','video')" id="ME_video" class="video"><em>.</em><b id="ME_Label_video">视频</b></a><!--[/if]-->
                    <!--[if $Audio]--><a href="javascript:void(0)" onclick="KE.util.click('$Id','audio')" id="ME_audio" class="audio"><em>.</em><b id="ME_Label_audio">音频</b></a><!--[/if]-->
                    <!--[if $Flash]--><a href="javascript:void(0)" onclick="KE.util.click('$Id','flash')" id="ME_flash" class="flash"><em>.</em><b id="ME_Label_flash">Flash</b></a><!--[/if]-->
                    <!--[if $Code]--><a href="javascript:void(0)" onclick="KE.util.click('$Id','code')" id="ME_code" class="code"><em>.</em><b id="ME_Label_code">代码</b></a><!--[/if]-->
                </div>
            <!--[if $fullscreen]-->
                <div class="clearfix largebuttons">
                    <a href="#" onclick="KE.util.click('$Id','fullscreen')" id="ME_fullscreen" class="fullscreen"><em>.</em><b id="ME_Label_fullscreen">全屏</b></a>
                </div>
            <!--[/if]-->
            </div>
        </div>
        <div class="clearfix webeditor-entrybox" id="ke_form_$Id" style="height:{=$BodyHeight}px;"><iframe frameborder="0" class="ke-iframe" id="ke_iframe_$Id" style="height:100%;width:100%;"></iframe><textarea class="ke-textarea" id="ke_textarea_$Id" style="display:none;height:100%;width:100%;border:none; padding:0px; margin:0px;"></textarea></div>
            <div class="webeditor-bottom" id="ke_bottom_$Id">
            <div class="clearfix webeditor-bottom-inner" id="ke_tab_$Id">
                <div class="webeditor-modechange">
                    <ul>
                        <li class="modechange-visual">
                            <a href="javascript:KE.util.setWyswygMode('$Id')" id="ke_tab_item1_$Id" class="current"><em>.</em>可视化模式</a>
                        </li>
                        <li class="modechange-code"><a href="javascript:KE.util.setSourceMode('$Id')" id="ke_tab_item2_$Id"><em>.</em>源代码模式</a></li>
                       <!--[if !$SupportWyswygMode]--><li><span style="color:Red">&nbsp;&nbsp;您的设备可能不支持可视化编辑</span></li><!--[/if]-->
                    </ul>
                </div>
                <div class="webeditor-sizechange">
                    <ul>
                        <li class="sizechange-less">
                            <a href="javascript:void(KE.util.shrink('$id'))">-</a>
                        </li>
                        <li class="sizechange-more">
                            <a href="javascript:void(KE.util.elongate('$id'))">+</a>
                        </li>
                    </ul>
                </div>
            </div>
        </div>
    </div>
</div>
<div id="ke_hide_$Id"></div>
<div id="ke_mask_$Id"></div>
<textarea id="$Id" name="$Id" style="width:$width;height:{=$Height}px;">$_form.text("$ID",$value)</textarea>

<script type="text/javascript">
        KE.g['$Id'] = new Object();
        $_if($UseMaxCode, "KE.util.setCurrentMode('$Id','ubb')")
        KE.g['$Id'].wyswygMode = $_if($WyswygMode, "true", "false");
        KE.g['$Id'].toolbarIcon = [];
        KE.g['$Id'].undoStack = [];
        KE.g['$Id'].redoStack = [];
        KE.g['$Id'].width = "$width";
        KE.g['$Id'].height = $Height;
        KE.g['$Id'].resizeMode = 2;
        KE.g['$Id'].autoOnsubmitMode = true;
        KE.g['$Id'].filterMode = true;
        KE.g['$Id'].skinType = 'default';
        KE.g['$Id'].cssPath = '';
        KE.g['$Id'].skinsPath = KE.scriptPath + 'skins/';
        KE.g['$Id'].pluginsPath = '/max-dialogs/editor/plugins/';
        KE.g['$Id'].minWidth = 200;
        KE.g['$Id'].minHeight = 160;
        KE.g['$Id'].minChangeSize = 5;
        KE.g['$Id'].panelSize = { width: 500, height: 360 };
        KE.g['$Id'].siteDomains = [];
        KE.g['$Id'].items = [];
        KE.g['$Id'].cssPath = '$root/max-assets/kindeditor/editor.css';
    KE.g['$Id'].defaultItems = [
        'source', 'preview', 'fullscreen', 'undo', 'redo', 'cut', 'copy', 'paste',
        'plainpaste', 'wordpaste', 'justifyleft', 'justifycenter', 'justifyright',
        'justifyfull', 'insertorderedlist', 'insertunorderedlist', 'indent', 'outdent', 'subscript',
        'superscript', '-',
        'fontname', 'fontsize', 'textcolor', 'bgcolor', 'bold',
        'italic', 'underline', 'strikethrough', 'removeformat', 'selectall', 'image',
        'flash', 'media', 'layer', 'table',
        'emoticons', 'link', 'unlink'
    ];

        KE.g['$Id'].htmlTags = {
            'font': ['color', 'size', 'face', '.background-color'],
            'span': ['.color', '.background-color', '.font-size', '.font-family', '.font-weight', '.font-style', '.text-decoration', '.vertical-align'],
            'div': ['class', 'align', '.border', '.margin', '.padding', '.text-align', '.color', '.background-color', '.font-size', '.font-family', '.font-weight', '.font-style', '.text-decoration', '.vertical-align'],
            'a': ['href', 'target'],
            'embed': ['src', 'type', 'loop', 'wmode', 'autostart', 'quality', '.width', '.height', '/'],
            'img': ['src', 'width', 'height', 'border', 'alt', 'title', '.width', '.height', '/'],
            'hr': ['/'],
            'br': ['/'],
            'p': ['align', '.text-align', '.color', '.background-color', '.font-size', '.font-family', '.font-weight', '.font-style', '.text-decoration', '.vertical-align'],
            'table': ['border', 'cellspacing', 'cellpadding', 'width', 'height', '.padding', '.margin', '.border'],
            'tbody': [],
            'tr': [],
            'td': ['align', 'rowspan', 'colspan', 'width', 'height', '.text-align', '.color', '.background-color', '.font-size', '.font-family', '.font-weight', '.font-style', '.text-decoration', '.vertical-align'],
            'th': ['align', 'rowspan', 'colspan', 'width', 'height', '.text-align', '.color', '.background-color', '.font-size', '.font-family', '.font-weight', '.font-style', '.text-decoration', '.vertical-align'],
            'strong': [],
            'b': [],
            's': [],
            'u': [],
            'i': [],
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
            'strike': []
        };
        var l = KE.lang;
        var b, i;
        for (var k in l) {
            b = KE.$("ME_" + k);
            if (b) {
                b.title = l[k];
                //b.onclick =new Function('KE.util.click("$Id", "' + k +'")');
                KE.g['$Id'].toolbarIcon[k] = b;
            }
        }
    
    KE.$('ke_container_$Id').style.display = '';
    if (window.editorCreateBeforeCallback)
        editorCreateBeforeCallback('$Id'); //编辑器创建之前的回调函数， 用于特殊页面初始化一些特定的参数
    KE.g["$id"].onCtrlEnter = new Function("if(window.clickPostButton)clickPostButton();");
    KE.create('$Id', 1);
</script>