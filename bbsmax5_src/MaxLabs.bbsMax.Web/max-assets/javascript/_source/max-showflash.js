
// 显示影视、音乐flash
function showFlash(root, host, flashvar, obj, shareid) {
    //TODO:百度视频显示有问题
    var baiduFalshAddr;
    //if(fGetxxIEVer()==0)//TODO
        //baiduFalshAddr = 'http://mv.baidu.com/export/flashplayer.swf?v=200';
    //else
        baiduFalshAddr = 'http://mv.baidu.com/export/flashplayer.swf?v=200&vid=FLASHVAR';

	var flashAddr = {
		'youku.com' : 'http://player.youku.com/player.php/sid/FLASHVAR=/v.swf',
		'ku6.com' : 'http://player.ku6.com/refer/FLASHVAR/v.swf',
		'youtube.com' : 'http://www.youtube.com/v/FLASHVAR',
		'5show.com' : 'http://www.5show.com/swf/5show_player.swf?flv_id=FLASHVAR',
		'sina.com.cn' : 'http://vhead.blog.sina.com.cn/player/outer_player.swf?vid=FLASHVAR',
		'sohu.com' : 'http://v.blog.sohu.com/fo/v4/FLASHVAR',
		'mofile.com' : 'http://tv.mofile.com/cn/xplayer.swf?v=FLASHVAR',
		'pomoho.com' : 'http://video.pomoho.com/swf/out_player.swf?flvid=FLASHVAR',
		'tieba.baidu.com' : baiduFalshAddr,
		'ouou.com' : 'http://www.ouou.com/mediaplayer.swf?file=FLASHVAR',
		'tudou.com':'http://www.tudou.com/v/FLASHVAR',
		'music' : 'FLASHVAR',
		'flash' : 'FLASHVAR'
	};
	var flash = '<object classid="clsid:D27CDB6E-AE6D-11cf-96B8-444553540000" codebase="http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=7,0,19,0" width="480" height="400">'
	    + '<param name="movie" value="FLASHADDR" />'
	    + '<param name="quality" value="high" />'
	    + '<param name="bgcolor" value="#FFFFFF" />'
        + '<param name="wmode" value="opaque" />'
	    + '<embed width="480" height="400" menu="false" wmode="opaque" quality="high" src="FLASHADDR" type="application/x-shockwave-flash" />'
	    + '</object>';
	var videoFlash = '<object classid="clsid:D27CDB6E-AE6D-11cf-96B8-444553540000" width="480" height="450">'
        + '<param value="opaque" name="wmode"/>'
		+ '<param value="FLASHADDR" name="movie" />'
		+ '<embed src="FLASHADDR" wmode="opaque" allowfullscreen="true" type="application/x-shockwave-flash"';
	if(host=='tieba.baidu.com')
		videoFlash = videoFlash + ' flashvars="vid='+flashvar+'" ';
		
		videoFlash = videoFlash + ' width="480" height="450"></embed>'
		+ '</object>';
	var musicFlash = '<object id="audioplayer_SHAREID" height="24" width="290" data="' + root + '/max-assets/flash/player.swf" type="application/x-shockwave-flash">'
		+ '<param value="' + root + '/max-assets/flash/player.swf" name="movie"/>'
        + '<param value="opaque" name="wmode"/>'
		+ '<param value="autostart=yes&bg=0xEBF3F8&leftbg=0x6B9FCE&lefticon=0xFFFFFF&rightbg=0x6B9FCE&rightbghover=0x357DCE&righticon=0xFFFFFF&righticonhover=0xFFFFFF&text=0x357DCE&slider=0x357DCE&track=0xFFFFFF&border=0xFFFFFF&loader=0xAF2910&soundFile=FLASHADDR" name="FlashVars"/>'
		+ '<param value="high" name="quality"/>'
		+ '<param value="false" name="menu"/>'
		+ '<param value="#FFFFFF" name="bgcolor"/>'
	    + '</object>';
	var musicMedia = '<object height="64" width="290" data="FLASHADDR" type="audio/x-ms-wma">'
	    + '<param value="FLASHADDR" name="src"/>'
	    + '<param value="1" name="autostart"/>'
	    + '<param value="true" name="controller"/>'
	    + '</object>';
	var flashHtml = videoFlash;
	var videoMp3 = true;
	if('' == flashvar) {
		alert('音乐地址错误，不能为空');
		return false;
	}
	if('music' == host) {
		var mp3Reg = new RegExp('.mp3$', 'ig');
		var flashReg = new RegExp('.swf$', 'ig');
		flashHtml = musicMedia;
		videoMp3 = false
		if(mp3Reg.test(flashvar)) {
			videoMp3 = true;
			flashHtml = musicFlash;
		} else if(flashReg.test(flashvar)) {
			videoMp3 = true;
			flashHtml = flash;
		}
	}
	flashvar = encodeURI(flashvar);
	if(flashAddr[host]) {
		var flash = flashAddr[host].replace('FLASHVAR', flashvar);
		flashHtml = flashHtml.replace(/FLASHADDR/g, flash);
		flashHtml = flashHtml.replace(/SHAREID/g, shareid);
	}
	
	
	if(!obj) {
		$('flash_div_' + shareid).innerHTML = flashHtml;
		return true;
	}
	if($('flash_div_' + shareid)) {
		$('flash_div_' + shareid).style.display = '';
		$('flash_hide_' + shareid).style.display = '';
		obj.style.display = 'none';
		return true;
	}
	if(flashAddr[host]) {
		var flashObj = document.createElement('div');
		flashObj.id = 'flash_div_' + shareid;
		obj.parentNode.insertBefore(flashObj, obj);
		flashObj.innerHTML = flashHtml;
		obj.style.display = 'none';
		var hideObj = document.createElement('div');
		hideObj.id = 'flash_hide_' + shareid;
		var nodetxt = document.createTextNode("收起");
		hideObj.appendChild(nodetxt);
		obj.parentNode.insertBefore(hideObj, obj);
		hideObj.style.cursor = 'pointer';
		hideObj.onclick = function() {
			if(true == videoMp3) {
				stopMusic('audioplayer', shareid);
				flashObj.parentNode.removeChild(flashObj);
				hideObj.parentNode.removeChild(hideObj);
			} else {
				flashObj.style.display = 'none';
				hideObj.style.display = 'none';
			}
			obj.style.display = '';
		}
	}
}
// 停止音乐flash
function stopMusic(preID, playerID) {
	var musicFlash = preID.toString() + '_' + playerID.toString();
	if($(musicFlash)) {
		$(musicFlash).SetVariable('closePlayer', 1);
	}
}