<!--[DialogMaster title="上传文件" subtitle="上传至$CurrentDirectory.name" width="500"]-->
<!--[place id="body"]-->
<form id="form1" action="$_form.action" method="post">
<div class="clearfix dialogbody">
    <div class="clearfix fileupload-operate">
        <div class="fileupload-button"><div id="uploadContainet"></div></div>
        <div class="fileupload-tip">
            提示: 单个文件大小限制在 $outputfilesize($MaxFileSize), 一次可以选择多个文件上传.
        </div>
    </div>
    
    <div class="clearfix fileupload-prompt">
        当网络硬盘存在同名文件时:
        <input name="onfileexist" id="skip" checked="checked" type="radio" />
        <label for="skip">跳过</label>
        <input name="onfileexist" id="replace" type="radio" />
        <label for="replace">覆盖</label>
    </div>
    
    <div class="fileuploadlist" id="file_upload">
        <div class="clearfix fileuploadlist-head">
            <span class="icon"><span>&nbsp;</span></span>
            <span class="title"><span>文件</span></span>
            <span class="filesize"><span>大小</span></span>
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
            </ul>
        </div>
    </div>
</div>
</form>
<script type="text/javascript">
var maxFileSize = $MaxFileSize;
function getFileSize(size) {
    if (size == 0)
        return '0KB';
    if (size < 1024) return '1KB';
    if (size < 1024 * 1024) return (size / 1024).toFixed(2) + 'KB';
    if (size < 1024 * 1024 * 1024) return (size / 1024 / 1024).toFixed(2) + 'MB';
    return (size / 1024 / 1024 / 1024).toFixed(2) + 'GB';
}

function HTMLEncode(html) {
    var temp = document.createElement("div");
    (temp.textContent != null) ? (temp.textContent = html) : (temp.innerText = html);
    var output = temp.innerHTML;
    temp = null;
    return output;
}


var templete="";

var swfu;

function initUploader(swfuConf) {
   templete=swfuConf.list_container.innerHTML;
    swfuConf.list_container.innerHTML='';

    swfu = new SWFUpload({
        // Backend Settings
        upload_url: swfuConf.upload_url,

        // File Upload Settings
        file_size_limit: swfuConf.file_size_limit,
        file_types: swfuConf.file_types,
        file_types_description: swfuConf.file_types_description,
        file_upload_limit: swfuConf.file_upload_limit,    // Zero means unlimited

        file_queued_handler: function(file) {
            $('item_container').innerHTML += String.format(templete, file.id, file.name, getFileSize(file.size), "等待上传");

        },

        file_queue_error_handler: function(file, errorCode, message) {
            switch (errorCode) {
                case SWFUpload.QUEUE_ERROR.FILE_EXCEEDS_SIZE_LIMIT:
                    $('item_container').innerHTML += String.format(templete, file.id, file.name, getFileSize(file.size), "文件超出大小");
                    $('fileicon_' + file.id).style.backgroundPosition = '0px -32px';
                    break;
                case SWFUpload.QUEUE_ERROR.ZERO_BYTE_FILE:
                    break;
                default:
                    break;
            }
        },

        file_dialog_complete_handler: function(selected, queued, totalqueued) {
            swfuConf.list_container.scrollTop = swfuConf.list_container.scrollHeight;
            swfu.startUpload();

        },

        upload_start_handler: function(file) {
            swfu.setUploadURL(swfuConf.upload_url + String.format('&filename={0}&directory={1}&onexist={2}'
            , encodeURIComponent(file.name), $CurrentDirectory.id, $('skip').checked ? 'skip' : 'replace'));

            swfu.setPostParams({
                'UserAuthCookie': swfuConf.authCookie,
                'filesize': file.size
            });
        },

        upload_progress_handler: function(file, complete, total) {
            swfuConf.processingCalback(file, complete, total);
        },

        upload_error_handler: function(file, errorcode, message) {
            
            $('percent_' + file.id).style.width = '0';
            $('filestatus_' + file.id).innerHTML = message;

            // $('file_item_' + file.id).style.backgroundColor="#ff7869";
            $('fileicon_' + file.id).style.backgroundPosition = '0px -32px';
            if (swfu.getStats().files_queued > 0) {
                swfu.startUpload();
            }
        },

        upload_success_handler: function(file, data, response) {
            var fileItem = $('filestatus_' + file.id);
            var datas = data.split('|');

            if (datas[0] == "error") {
                //   alert(datas[1])
                $('percent_' + file.id).style.width = '0';
                fileItem.innerHTML = datas[1];
                $('fileicon_' + file.id).style.backgroundPosition = '0px -48';

            }
            else {
                $('fileicon_' + file.id).style.backgroundPosition = '0px -32px'
                fileItem.innerHTML = '上传成功';
                var hidden = addElement('input', $('form1'));
                hidden.style.display = 'none';
                hidden.value = datas[0];
                hidden.name = 'tempfileids';
                result = true;
            }

            if (swfu.getStats().files_queued > 0) {
                swfu.startUpload();
            }

            else if (swfu.getStats().upload_errors > 0) {

            }

            else {

            }
        },

        upload_complete_handler: function(file) {
            //            $('filestatus_' + file.id).innerHTML = '上传成功';
            //            $('fileicon_'+ file.id).style.backgroundPosition='-48px 0px';
        },

        // Button settings
        button_image_url: swfuConf.button_image_url, //"images/XPButtonNoText_160x22.png",
        button_placeholder_id: swfuConf.button_placeholder_id, //"spanButtonPlaceholder",
        button_width: swfuConf.button_width, //160,
        button_height: swfuConf.button_height, //22,
        button_text: swfuConf.button_text, //'<span class="button">Select Images <span class="buttonSmall">(2 MB Max)</span></span>',
        button_text_style: swfuConf.button_text_style, //'.button { font-family: Helvetica, Arial, sans-serif; font-size: 14pt; } .buttonSmall { font-size: 10pt; }',
        //        button_text_top_padding: swfuConf.button_text_top_padding, //1,
        //        button_text_left_padding: swfuConf.button_text_left_padding, //5,
        button_action: SWFUpload.BUTTON_ACTION.SELECT_FILES,
        button_disabled: false,
        button_cursor: SWFUpload.CURSOR.HAND,
        button_window_mode: SWFUpload.WINDOW_MODE.OPAQUE,

        // Flash Settings
        flash_url: root + '/max-assets/swfupload/Flash/swfupload.swf', // Relative to this file
        debug: false
    });
}


loadScript("$root/max-assets/swfupload/swfupload.js", null, function () {
   
    initUploader({
        authCookie: '$UserAuthCookie',
        upload_url: '$root/default.aspx?uploadtempfile.aspx?action=disk&UserAuthCookie=$UserAuthCookie',
        button_image_url: '$skin/images/swfupload_button.png',
        button_placeholder_id: 'uploadContainet',
        button_width: 86,
        button_height: 30,
        //button_text: '<span class="theFont">添加文件</span>',
        //button_text_style: '.theFont{text-indent:22px;}',
        //button_text_left_padding: 0,
        //button_text_top_padding: 5,
        file_size_limit: '$MaxFileSizeForSwfUpload',
        file_types: '$AllowedFileType',
        file_types_description: '',
        file_upload_limit: '$MaxFileCount',
        list_container: $("item_container"),
        processingCalback: processCallback //上传进度处理回调
    });
});


function processCallback(file, complete, total) {
    $('percent_' + file.id).style.width = Math.ceil(complete * 100 / total) + '%';
    $('filestatus_' + file.id).innerHTML = '正在上传：' + Math.floor(complete / total * 100) + "%";
}
currentPanel.result = 1;
</script>
<!--[/place]-->
<!--[/dialogmaster]-->
