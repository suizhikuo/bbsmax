<!--[if $ForumID>0]-->
<!--[if $_get.tab == "3"]-->
<div class="filelistview">
    <div class="filelistview-head">
        <table class="filelist-table">
            <tr>
                <td class="filename">文件</td>
                <td class="filesize">大小</td>
                <td class="fileaddtime">时间</td>
                <td class="fileaction">操作</td>
            </tr>
        </table>
    </div>
    <!--[if $AllFiles.count != 0]-->
    <div class="filelistview-wrap">
        <table class="filelist-table">
        <!--[loop $DiskFile in $PagedFiles]-->
            <!--[if !$DiskFile.isfile]-->
            <tr>
                <td class="filename">
                    <div class="filename-wrap">
                        <span style="padding-left:20px;background:url($Root/max-assets/icon/folder.gif) no-repeat 0 50%;"><a href="$GetUrl($DiskFile.ID)">$DiskFile.Name</a></span>
                    </div>
                </td>
                <td class="filesize">&nbsp;</td>
                <td class="fileaddtime">$outputDate($diskfile.CreateDate)</td>
                <td class="fileaction">&nbsp;</td>
            </tr>
            <!--[else]-->
            <tr>
                <td class="filename">
                    <div class="filename-wrap">
                        <span style="padding-left:20px;background:url($DiskFile.smallIcon) no-repeat 0 50%;">$DiskFile.Name</span>
                    </div>
                </td>
                <td class="filesize">$outputFilesize($DiskFile.Size)</td>
                <td class="fileaddtime">$outputDate($diskfile.CreateDate)</td>
                <td class="fileaction">
                    <!--[if $MaxFileSize>=$diskfile.Size]-->
                    <!--[if $isImage( $diskFile.TypeName)]-->
                    <a href="javascript:void(selectFile($DiskFile.id,'$tojs('$DiskFile.Name')',$DiskFile.size,'$tojs($DiskFile.smallIcon)'),false);">插入图片</a>
                    <!--[else if $isFlash( $diskFile.TypeName)]-->
                    <a href="javascript:void(selectFile($DiskFile.id,'$tojs('$DiskFile.Name')',$DiskFile.size,'$tojs($DiskFile.smallIcon)'),false);">插入Flash</a>
                    <!--[else if $isvideo( $diskFile.TypeName)]-->
                    <a href="javascript:void(selectFile($DiskFile.id,'$tojs('$DiskFile.Name')',$DiskFile.size,'$tojs($DiskFile.smallIcon)'),false);">插入视频</a>
                    <!--[else if $isaudio( $diskFile.TypeName)]-->
                    <a href="javascript:void(selectFile($DiskFile.id,'$tojs('$DiskFile.Name')',$DiskFile.size,'$tojs($DiskFile.smallIcon)'),false);">插入音频</a>
                    <!--[/if]-->
                    <a href="javascript:void(selectFile($DiskFile.id,'$tojs('$DiskFile.Name')',$DiskFile.size,'$tojs($DiskFile.smallIcon)',true));">插入链接</a>
                    <!--[else]-->
                    <span class="tip">大小超出限制.</span>
                    <!--[/if]-->
                </td>
            </tr>
            <!--[/if]-->
        <!--[/loop]-->
        </table>
    </div>
    <!--[else]-->
    <div class="filelist-nodata">
        <p>当前目录没有$TypeName文件.</p>
    </div>
    <!--[/if]-->
</div>
<div class="pagination">
  <!--[pager name="list" skin="../_pager.aspx"]-->
</div>
<script type="text/javascript">
    function selectFile(id, filename, filesize, fileicon,islink) {
        var f = { "id": id, "filename": filename, "filesize": filesize, "icon": fileicon, "type": 1 };
        if (parent.window.addDiskFile(f))  parent.window.insertAttachment(f, islink);
    }

    function changeDirectory(id) {
        var url = location.href;
        if (url.indexOf('directoryid') > 0) {
            url = url.replace(/&directoryid=\d+/i, '');
        }
        url = url + "&directoryid=" + id;
        location.replace(url);
    }

    function ok() {
        closePanel();
    }
</script>
<!--[/if]-->

<!--[if $_get.tab == "2"]-->
<div class="filelistview">
    <div class="filelistview-head">
        <table class="filelist-table">
            <tr>
                <td class="filename">文件名</td>
                <td class="filesize">大小</td>
                <td class="fileaddtime">时间</td>
                <td class="fileaction">操作</td>
            </tr>
        </table>
    </div>
    <!--[if $AttachmentList.TotalRecords != 0]-->
    <div class="filelistview-wrap">
        <table class="filelist-table">
            <!--[loop $Attachment in $AttachmentList]-->
            <tr>
                <td class="filename">
                    <div class="filename-wrap">
                        <span style="padding-left:20px;background:url($Attachment.FileIcon) no-repeat 0 50%;">$Attachment.FileName</span>
                    </div>
                </td>
                <td class="filesize">$outputFilesize($Attachment.FileSize)</td>
                <td class="fileaddtime">$outputdate($Attachment.CreateDate)</td>
                <td class="fileaction">
                    <!--[if $MaxFileSize>=$Attachment.fileSize]-->
                    <!--[if $isImage( $Attachment.filetype)]-->
                    <a href="javascript:void(selectFile($Attachment.AttachmentID,'$tojs('$Attachment.FileName')',$Attachment.FileSize,'$tojs($Attachment.FileIcon)',false));"> 插入图片</a>
                    <!--[else if $isFlash($Attachment.filetype)]-->
                    <a href="javascript:void(selectFile($Attachment.AttachmentID,'$tojs('$Attachment.FileName')',$Attachment.FileSize,'$tojs($Attachment.FileIcon)',false));"> 插入Flash</a>
                    <!--[else if $isvideo( $Attachment.filetype)]-->
                    <a href="javascript:void(selectFile($Attachment.AttachmentID,'$tojs('$Attachment.FileName')',$Attachment.FileSize,'$tojs($Attachment.FileIcon)',false));"> 插入视频</a>
                    <!--[else if $isaudio($Attachment.filetype)]-->
                    <a href="javascript:void(selectFile($Attachment.AttachmentID,'$tojs('$Attachment.FileName')',$Attachment.FileSize,'$tojs($Attachment.FileIcon)',false));"> 插入音频</a>
                    <!--[/if]-->
                    <a href="javascript:void(selectFile($Attachment.AttachmentID,'$tojs('$Attachment.FileName')',$Attachment.FileSize,'$tojs($Attachment.FileIcon)',true));">插入链接</a>
                    <!--[else]-->
                    <span class="tip">大小超出限制.</span>
                    <!--[/if]-->
                </td>
            </tr>
        <!--[/loop]-->
        </table>
    </div>
    <!--[else]-->
    <div class="filelist-nodata">
        <p>当前没有$TypeName历史附件.</p>
    </div>
    <!--[/if]-->
</div>
<!--[pager name="list" skin="../_pager.aspx"]-->
<script type="text/javascript">
    var fileCollection = new Array();

    function clickFile(e, id, name, size, icon) {
        if (e.checked) {
            selectFile(id, name, size, icon);
        }
        else {
            unselectFile(id);
        }
    }
    function selectFile(id, filename, filesize, fileicon,islink) {
        var f = { "id": id, "filename": filename, "filesize": filesize, "icon": fileicon, "type": 0 };
        parent.window.insertAttachment(f, islink);
    }

    function unselectFile(id) {
        var index = -1;
        for (var i = 0; i < fileCollection.length; i++) {
            if (fileCollection[i].id == id) {
                index = i;
                break;
            }
        }
        if (index != -1) {
            fileCollection.splice(index, 1);
        }
    }

    function ok() {
        parent.window.addHistoryAttach(fileCollection);
        closePanel();
    }
</script>
<!--[/if]-->

<!--[if $_get.tab == null]-->
<script type="text/javascript" src="$root/max-assets/swfupload/swfupload.js"></script>

<div class="fileuploadlist" id="file_upload">
    <div class="clearfix fileuploadlist-head">
        <span class="icon"><span>&nbsp;</span></span>
        <span class="title"><span>文件</span></span>
        <span class="filesize"><span>大小</span></span>
        <span class="action"><span>操作</span></span>
    </div>
    <div class="fileuploadlist-items" id="item_container">
        <ul class="clearfix fileuploaditem" id="file_item_{0}">
            <li class="status"><span style="width:0%;" id="percent_{0}">100%</span></li>
            <li class="icon"><span id="fileicon_{0}">-</span></li>
            <li class="title" id="filename_{0}">
                <span id="lblFilename{0}">{1}<em class="tip" id="filestatus_{0}">{3}</em>
                    <input type="hidden" id="txtFilename{0}" name="filenames{0}" value="{1}" />
                </span>
            </li>
            <li class="filesize"><span id="filesize_{0}">{2}</span></li>
            <li class="action" id="action_{0}" style="display:none;"><span><a href="javascript:void(0);" title="删除" onclick="deleteFile('{0}');">删除</a></span></li>
        </ul>
    </div>
</div>

<script type="text/javascript">
    var maxFileSize = $MaxFileSize;
    var UserAuthCookie = "$UserAuthCookie";
    var skinPath = "$skin";
    var MaxFileSizeForSwfUpload = "$MaxFileSizeForSwfUpload";
    var AllowedFileType = "$FileTypeForSwfUpload";
    var MaxFileCount = $MaxFileCount;

    function deleteFile() {
        var fid =  arguments[0];
        var f = window.uploadFiles[fid];
        parent.removeAttachment(f.id);
        var itemId = "file_item_" + fid;
        removeElement($(itemId)); 
    }
</script>
<script src="$root/max-assets/kindeditor/attachupload.js" type="text/javascript"></script>
<!--[/if]-->
<!--[/if]-->