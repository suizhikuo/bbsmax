
function getFileSize(size) {
	if (size == 0)
		return '0KB';
	if (size < 1024) return '1KB';
	if (size < 1024 * 1024) return (size / 1024).toFixed(2) + 'KB';
	if (size < 1024 * 1024 * 1024) return (size / 1024 / 1024).toFixed(2) + 'MB';
	return (size / 1024 / 1024 / 1024).toFixed(2) + 'GB';

}

var swfu;
var photolist_item_template;

function initUploader(authCookie, url, fileSizeLimit) {

	var photolist = $('photolist');

	swfu = new SWFUpload({

		// File Upload Settings
	    file_size_limit: fileSizeLimit,
		file_types: "*.jpg;*.jpeg;*.png;*.gif",
		file_types_description: '图片文件',
		file_upload_limit: 0,
		
		post_params: {
			'UserAuthCookie': authCookie
		},

		file_queued_handler: function(file) {

			photolist.innerHTML += "<li>" + photolist_item_template.replace(/{id}/g, file.id).replace(/{name}/g, file.name) + "</li>";

			$('photo_stat_' + file.id).innerHTML = '等待上传，文件大小：' + getFileSize(file.size);
		},
		file_queue_error_handler: function(file) {
            alert(file.name + '超过了文件大小限制，您当前的文件大小权限为：每张不大于' + fileSizeLimit);
		},
		file_dialog_complete_handler: function(selected, queued, totalqueued) {

			$('uploadmultiphoto').disabled = true;
			
			swfu.startUpload();
		},
		upload_start_handler: function(file) {

			swfu.setUploadURL(url + '&filename=' + encodeURIComponent(file.name));
		},
		upload_progress_handler: function(file, complete, total) {

			$('photo_pgbar_' + file.id).style.width = Math.ceil(complete * 100 / total) + '%';
		    $('photo_stat_' + file.id).innerHTML = '已上传：' + Math.ceil(complete * 100 / total) + '%';
		},
		upload_error_handler: function(file, errorcode, message) {

			$('photo_stat_' + file.id).innerHTML = message;
		},
		upload_success_handler: function(file, data, response) {

			var datas = data.split('|');
			
			if(datas[0] == 'error') {
				
				$('photo_stat_' + file.id).innerHTML = datas[1];
				
			} else {
				var fileID = document.createElement("input");
				//fileID.type = "hidden";
				fileID.id = "photo_fileid_" + file.id;
				fileID.name = "photo_files";
				fileID.value = datas[0];
				fileID.style.display='none';
	            
				var stat = $('photo_stat_' + file.id);

				stat.parentNode.appendChild(fileID);

				stat.innerHTML = '上传完毕，已保存为临时文件';

				$('photo_pgbar_' + file.id).style.width = '100%';

				if (swfu.getStats().files_queued > 0) {
					swfu.startUpload();
				}
				else if (swfu.getStats().upload_errors > 0) {

				}
				else {

				}
			}
		},
		upload_complete_handler: function(file) {

			$('uploadmultiphoto').disabled = false;
			
		},

		// Button settings
		button_image_url: root + '/max-templates/default/images/upload_photo.png',
		button_placeholder_id: 'UploadFile',
		button_width: 100,
		button_height: 35,
		button_text_top_padding: 0,
		button_text_left_padding: 2,
		button_action: SWFUpload.BUTTON_ACTION.SELECT_FILES,
        button_disabled: false,
        button_cursor: SWFUpload.CURSOR.HAND,
        button_window_mode:SWFUpload.WINDOW_MODE.OPAQUE,

		// Flash Settings
		flash_url: root + '/max-assets/swfupload/Flash/swfupload.swf', // Relative to this file

		// Debug Settings
		debug: false
	});
}
