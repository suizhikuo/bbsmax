<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>皮肤管理</title>
<!--[include src="../_htmlhead_.aspx"/]-->
<script src="$root/max-assets/codemirror/js/codemirror.js" type="text/javascript"></script>
<style type="text/css">
.CodeMirror-line-numbers {
    width:2.2em;
    color:#aaa;
    background: #f5f5f5;
    text-align:right;
    padding:0px .3em 0 0;
    font:12px/16px Consolas,"Courier New",monospace;
}
</style>
</head>
<body>
<!--[include src="../_head_.aspx" /]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
    <!--[if $_get.skin == null]-->
    <div class="PageHeading">
        <h3>皮肤列表</h3>
        <div class="ActionsBar">
            <a href="$dialog/skin-import.aspx" onclick="return openDialog(this.href, function(){ refresh(); })"><span>导入皮肤</span></a>
        </div>
    </div>
    <div class="DataTable">
        <table>
            <thead>
                <tr>
                    <th style="width:220px;">&nbsp;</th>
                    <th>皮肤信息</th>
                    <th style="width:220px;">编辑</th>
                </tr>
            </thead>
            <tbody>
                <!--[loop $Skin in $AllSkinList]-->
                <tr $_if($Skin.IsDefaultSkin, 'style="background-color:#edf3fa;"')>
                    <td class="template-thumb">
                        <a href="$admin/other/manage-template.aspx?skin=$Skin.SkinID"><img src="$root/max-templates/$Skin.SkinID/_skin.png" /></a>
                        $_if($Skin.IsDefaultSkin, '<p><strong>当前使用的皮肤</strong></p>')
                    </td>
                    <td class="template-info">
                        <p><strong>名称:</strong> $Skin.Name</p>
                        <p><strong>版本:</strong> $Skin.Version</p>
                        <p><strong>作者:</strong> $Skin.Author</p>
                        <p><strong>网站:</strong> $Skin.WebSite</p>
                        <p><strong>简介:</strong> $Skin.Description</p>
                    </td>
                    <td>
                        <p><a href="$admin/other/manage-template.aspx?skin=$Skin.SkinID">编辑</a></p>
                        <!--[if $Skin.IsDefaultSkin == false]-->
                        <!--[if $Skin.Enabled]-->
                        <p><a href="$dialog/skin-enable.aspx?skin=$Skin.SkinID&enable=0" onclick="return openDialog(this.href, function(){ refresh(); })">禁用</a></p>
                        <!--[else]-->
                        <p><a href="$dialog/skin-enable.aspx?skin=$Skin.SkinID&enable=1" onclick="return openDialog(this.href, function(){ refresh(); })">启用</a></p>
                        <!--[/if]-->
                        <!--[/if]-->
                        <!--[if $Skin.IsDefaultSkin == false &&  $Skin.Enabled]-->
                        <p><a href="$dialog/skin-setdefault.aspx?skin=$Skin.SkinID" onclick="return openDialog(this.href, function(){ refresh(); })">设为默认皮肤</a></p>
                        <!--[/if]-->
                        <p><a href="$dialog/skin-export.aspx?skin=$Skin.SkinID" onclick="return openDialog(this.href, function(){ })">导出</a></p>
                    </td>
                </tr>
                <!--[/loop]-->
            </tbody>
        </table>
    </div>
    <!--[else]-->
    <script type="text/javascript">
        function toggleFolder(id,iconid) {
            var folder = document.getElementById(id);
            var icon = document.getElementById(iconid);

            if (folder.style.display == 'none') {
                folder.style.display = '';
                icon.src = '$root/max-assets/images/arrow_extend.gif';
            } else {
                folder.style.display = 'none';
                icon.src = '$root/max-assets/images/arrow_collapse.gif';
            }
        }
    </script>
    <!--[if $TemplateFileList.Length > 0]-->
    <div class="PageHeading">
        <h3>文件编辑 (~\max-templates\$_get.skin$FilePath)</h3>
        <div class="ActionsBar">
            <a href="$admin/other/manage-template.aspx" class="back"><span>返回模板列表</span></a>
            <!--[if $_get.history != null]-->
            <a href="$admin/other/manage-template.aspx?skin=$_get.skin&file=$FilePath" class="back"><span>返回文件编辑</span></a>
            <!--[/if]-->
        </div>
    </div>
    <div class="clearfix">
    <div class="template-editer <!--[if $_get.history != null]--> template-historyedit<!--[/if]-->">
        <div class="template-editer-inner">
        <div class="clearfix">
            <div class="editer-col1">
            <form action="$_form.action" method="post">
                <div class="clearfix editer-toolbar">
                    <p class="left">
                    <!--[if $_get.history != null && $_get.history.IndexOf(',') > 0]-->
                    <input class="button" type="submit" value="还原到此版本" name="save" style="background-image:url($root/max-assets/icon/save.gif);" />
                    <!--[else]-->
                    <input class="button" type="submit" value="保存修改" name="save" style="background-image:url($root/max-assets/icon/save.gif);" />
                    <!--[/if]-->
                    </p>
                    <p class="right">
                    <input class="button" type="button" value="语法加亮" id="editorToggler" onclick="javascript:toggleEditor();" style="background-image:url($root/max-assets/icon/highlight.gif);" />
                    <input class="button" type="button" value="自动换行" onclick="editor.toggleWrapping(); if(editor2) editor2.toggleWrapping();" style="background-image:url($root/max-assets/icon/wordwrap.gif);" />
                    </p>
                </div>
                <div class="editer-area">
                <textarea cols="50" rows="15" name="FileContent" id="FileEditor" <!--[if $_get.history != null && $_get.history.IndexOf(',') > 0]-->readonly="readonly"<!--[/if]-->>$FileContent</textarea>
                </div>
            </form>
            </div>
            <!--[if $_get.history != null]-->
            <div class="editer-col2">
                <form action="$_form.action" method="post">
                <div class="clearfix editer-toolbar">
                    <div style="float:left">
                    <input type="submit" value="还原到此版本" name="save" class="button" style="background-image:url($root/max-assets/icon/save.gif);" />
                    </div>
                </div>
                <div class="editer-area">
                <textarea cols="50" rows="15" name="FileContent" id="FileEditor2" <!--[if $_get.history != null]-->readonly="readonly"<!--[/if]-->>$FileContent2</textarea>
                </div>
            </form>
             </div>
            <!--[/if]-->
            <script type="text/javascript">
                var FileEditor = document.getElementById('FileEditor');
                var FileEditor2 = document.getElementById('FileEditor2');
                FileEditor.style.height = (document.body.clientHeight - 250) + 'px';
                if(FileEditor2)
                    FileEditor2.style.height = FileEditor.style.height;
                //if (navigator.userAgent.toLowerCase().indexOf('webkit') != -1)
                    //document.getElementById('editorToggler').style.display = 'none';
                var editor = CodeMirror.fromTextArea(FileEditor, {
                    <!--[if $_get.history != null && $_get.history.IndexOf(',') > 0]-->readOnly: true,<!--[/if]-->
                    containerID: 'TheEditor',
                    lineNumbers: true,
                    textWrapping: false,
                    path: "$root/max-assets/codemirror/js/",
                    $FileType
                });

                var editor2 =null;
                
                if(FileEditor2) {
                    editor2 = CodeMirror.fromTextArea(FileEditor2, {
                        <!--[if $_get.history != null]-->readOnly: true,<!--[/if]-->
                        containerID: 'TheEditor2',
                        lineNumbers: true,
                        textWrapping: false,
                        path: "$root/max-assets/codemirror/js/",
                        $FileType
                    });
                }
                
                var editorEnable = true;

                function toggleEditor() {
                    var theEditor = document.getElementById('TheEditor');

                    if (theEditor.style.display == 'none') {
                        theEditor.style.display = '';
                        FileEditor.style.display = 'none';
                        editor.setCode(FileEditor.value);
                        FileEditor.parentNode.style.border = 'solid 1px #CCC';
                    } else {
                        theEditor.style.display = 'none';
                        FileEditor.style.display = '';
                        FileEditor.value = editor.getCode();
                        FileEditor.parentNode.style.border = '';
                    }
                    
                    var theEditor2 = document.getElementById('TheEditor2');
                    
                    if(theEditor2) {
                        if (theEditor2.style.display == 'none') {
                            theEditor2.style.display = '';
                            FileEditor2.style.display = 'none';
                            editor2.setCode(FileEditor2.value);
                            FileEditor2.parentNode.style.border = 'solid 1px #CCC';
                        } else {
                            theEditor2.style.display = 'none';
                            FileEditor2.style.display = '';
                            FileEditor2.value = editor2.getCode();
                            FileEditor2.parentNode.style.border = '';
                        }
                    }
                }
            </script>
        </div>
        <!--[if $BackupFileList.length > 0]-->
            <form action="$_form.action" method="get">
            <input type="hidden" name="file" value="$FilePath" />
            <input type="hidden" name="skin" value="$_get.skin" />
            <div class="DataTable">
                <table>
                    <thead>
                    <tr>
                        <th>&nbsp;</th>
                        <th>版本号</th>
                        <th>编辑时间</th>
                        <th style="width:6em;">操作</th>
                    </tr>
                    </thead>
                    <tbody>
                    <!--[loop $BackupFile in $BackupFileList]-->
                    <tr>
                        <td><input name="history" type="checkbox" value="$BackupFile.Version" /></td>
                        <td>$BackupFile.Version</td>
                        <td>$BackupFile.CreationTime</td>
                        <td><a href="$admin/other/manage-template.aspx?history=$BackupFile.Version&skin=$_get.skin&file=$FilePath">查看</a> - <a href="$dialog/skin-delete-backup-file.aspx?skin=$_get.skin&file=$FilePath&version=$BackupFile.Version" onclick="return openDialog(this.href, function(){ window.location.href = '$admin/other/manage-template.aspx?skin=$_get.skin&file=$FilePath.Replace("\\", "\\\\")' })">删除</a></td>
                    </tr>
                    <!--[/loop]-->
                    </tbody>
                </table>
                <div class="Actions">
                    <input type="submit" value="对比所选版本" class="button" />
                </div>
            </div>
            </form>
        <!--[/if]-->
        </div>
    </div>
    <!--[if $_get.history == null]-->
    <div class="template-sidebar">
        <div class="template-files">
            <ul>
            <!--[loop $File in $TemplateFileList with $i]-->
            <li>
            <!--[if $File.IsDir]-->
                <!--[if $FilePath.IndexOf($File.Path) == 0]-->
                <p class="folder" onclick="toggleFolder('folder_$i','ficon_$i')"><span class="arrow"><img alt="" id="ficon_$i" src="$root/max-assets/images/arrow_extend.gif" /></span><span class="name">$File.Name</span></p>
                <ul id="folder_$i">
                <!--[else]-->
                <p class="folder" onclick="toggleFolder('folder_$i','ficon_$i')"><span class="arrow"><img alt="" id="ficon_$i" src="$root/max-assets/images/arrow_collapse.gif" /></span><span class="name">$File.Name</span></p>
                <ul style="display:none" id="folder_$i">
                <!--[/if]-->
            <!--[else]-->
                <a href="$admin/other/manage-template.aspx?skin=$_get.skin&file=$File.Path" $_if($FilePath == $File.Path, 'class="current" ')>$File.Name</a>
                <!--[if $i == $TemplateFileList.Length - 1]-->
                $loop(0, $File.Level, "</li></ul>")
                <!--[else if $TemplateFileList[$i + 1].Level < $File.Level]-->
                $loop($TemplateFileList[$i + 1].Level, $File.Level, "</li></ul>")
                <!--[/if]-->
            </li>
            <!--[/if]-->
            <!--[/loop]-->
            </ul>
        </div>
    </div>
    </div>
    <!--[/if]-->
    <!--[/if]-->
    <!--[/if]-->
</div>
<!--[include src="../_foot_.aspx" /]-->
</body>
</html>
