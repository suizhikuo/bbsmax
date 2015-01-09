
var templete = "";

var swfu;

function initUploader(swfuConf) {
    templete = swfuConf.list_container.innerHTML;
    swfuConf.list_container.innerHTML = '';

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
    				$('fileicon_' + file.id).style.backgroundPosition = '0 -48px';
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
    		var url = swfuConf.upload_url + String.format('&filename={0}&filesize={1}', encodeURIComponent(file.name), file.size);

    		swfu.setUploadURL(url);

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
            $('fileicon_' + file.id).style.backgroundPosition = '0 -16px';
            if (swfu.getStats().files_queued > 0) {
                swfu.startUpload();
            }
        },

    	upload_success_handler: function(file, data, response) {
    		var fileItem = $('filestatus_' + file.id);
    		var datas = data.split('|');
    		if (datas[0] == "error") {
    			$('percent_' + file.id).style.width = '0';
    			fileItem.innerHTML = datas[1];
    			$('fileicon_' + file.id).style.backgroundPosition = '0 -48px';
    		}
    		else {
    			$('fileicon_' + file.id).style.backgroundPosition = '0 -32px'
    			fileItem.innerHTML = '上传成功';
    			var f = {};
    			if (!window.uploadFiles) window.uploadFiles = {};
    			f.id = 0 - Math.abs(parseInt(datas[0]));
    			f.filename = datas[1];
    			f.filesize = datas[2];
    			f.icon = datas[4];
    			f.type = 0;
    			parent.addAttachment(f);
    			window.uploadFiles[file.id] = f;
    			setVisible($("action_" + file.id), true);
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
    		//            $('fileicon_'+ file.id).style.backgroundPosition='0 -32px';
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

initUploader({ authCookie: UserAuthCookie,
    list_container: $("item_container"),
    upload_url: String.format('{3}/default.aspx?uploadtempfile.aspx?action=attach&forumID={0}&key={1}&UserAuthCookie={2}', parent.forumID, parent.searchInfo, UserAuthCookie, root),
    button_image_url: skinPath + '/images/swfupload_button.png',
    button_placeholder_id: 'uploadContainet',
    button_width: 86,
    button_height: 30,
    //button_text: '<span class="theFont">添加文件</span>',
    //button_text_style: '.theFont{text-indent:22px;}',
    //button_text_left_padding: 0,
    //button_text_top_padding: 5,
    file_size_limit: MaxFileSizeForSwfUpload, // '$,MaxFileSizeForSwfUpload',
    file_types: AllowedFileType, //'$,AllowedFileType',
    file_types_description: '',
    file_upload_limit: MaxFileCount, //'$,MaxFileCount',
    processingCalback: processCallback
});

function processCallback(file, complete, total) {
    $('percent_' + file.id).style.width = Math.ceil(complete * 100 / total) + '%';
    var p = Math.floor(complete / total * 100.0);
    if (p == 100) p -= 1;
    $('filestatus_' + file.id).innerHTML = '正在上传:' + p + "%";
}