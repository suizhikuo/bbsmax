var mediaFileTypes = new Object();
mediaFileTypes.flash = ["swf"];
mediaFileTypes.audio = ["mp3", "wav", "wma", "mid", "ra", "midi", "asf", "ape"];
mediaFileTypes.video = ["wmv", "rm", "rmvb", "avi", "mov", "3gp", "mp4", "flv", "mkv"];
mediaFileTypes.image = ["jpg", "gif", "bmp", "png", "jpeg"];


function isImage(ft) {
    return mediaFileTypes.image.contains(ft.toLowerCase());
}

function isAudio(ft) {
    return mediaFileTypes.audio.contains(ft.toLowerCase());
}

function isVideo(ft) {
    return mediaFileTypes.video.contains(ft.toLowerCase());
}

function isFlash(ft) {
    return mediaFileTypes.flash.contains(ft.toLowerCase());
}

function delAttach(id, resultCallback) {

    if (id < 0) {
        var ajax = new ajaxWorker(deleteTempFileUrl, "get", null);
        ajax.addListener(200, function(r) {
            if (r == "ok")
                resultCallback(true, null);
            else
                resultCallback(false, f);
        });
        ajax.send(deleteTempFileUrl + '?TempUploadFileID=' + (0 - id));
    }
}

function cancelUploadFile(file_id)
{
    swfu.cancelUpload(file_id, false);
    $('attach_uploaditem_' + file_id).parentNode.removeChild($('attach_uploaditem_' + file_id));
}

function insertAttach(id,fileName,fileExtName,type){

    if (fileExtName != null && fileExtName != "") if (fileExtName.indexOf(".") != 0) fileExtName = "." + fileExtName;

    if (fileExtName && fileExtName.indexOf(".") == 0) fileExtName = fileExtName.remove(0,1);
    
   
    
    var isimage = isImage(fileExtName.toLowerCase());
    var isMedia = false;
    var mediaHeight = 0;

    if (isAudio(fileExtName.toLowerCase())) {
        isMedia = true;
        mediaHeight = 64;
    }
    else if (isVideo(fileExtName.toLowerCase())) {
        isMedia = true;
        mediaHeight = 300;
    }
    else if (isFlash(fileExtName.toLowerCase())) {
        isMedia = true;
        mediaHeight = 300;
    }
    var ubb,html;
    if(isimage || isMedia)
    {
        if(type == 1)
        {
            if(isMedia)
            {
                ubb = "[diskmedia=400,"+mediaHeight+",0]"+id+"[/diskmedia]";
                html = "<img src=\""+root+"/max-assets/images/blank.gif\" width=\"400\" height=\""+ mediaHeight +"\" alt=\" _auto=0 &mode=media&diskfileid="+id+"\" style=\"border:solid 1px #aaaaaa; background-position:center center; background-repeat:no-repeat;background-image:url("+attachUrl+"&mode=media&diskfileid="+id+")\" />";
            }
            else {
                ubb = "[diskimg]"+id+"[/diskimg]";
                html = "<img src=\""+attachUrl+"&mode=image&diskfileid="+id+"\" alt=\"\" />";
            }
        }
        else
        {
            if(isMedia)
            {
                if(id<0)
                    ubb = "[localmedia=400,"+mediaHeight+",0]"+(0-id)+"[/localmedia]";
                else
                    ubb = "[attachmedia=400,"+mediaHeight+",0]"+id+"[/attachmedia]";
                    
                html = "<img src=\""+root+"/max-assets/images/blank.gif\" width=\"400\" height=\""+ mediaHeight +"\"  alt=\" _auto=0 &mode=media&id="+id+"\" style=\"border:solid 1px #aaaaaa; background-position:center center; background-repeat:no-repeat;background-image:url("+attachUrl+"&mode=media&id="+id+")\" />";
            
            }
            else
            {
                if(id<0)
                    ubb = "[localimg]"+(0-id)+"[/localimg]";
                else
                    ubb = "[attachimg]"+id+"[/attachimg]";
                    
                html = "<img src=\""+attachUrl+"&mode=image&id="+id+"\" alt=\"\" />";
            }
        }
    }
    else
    {
        if(type == 1)
        {
            ubb = "[diskfile]"+id+"[/diskfile]";
            html = ubb;
        }
        else
        {
            if(id<0)
                ubb = "[local]"+(0-id)+"[/local]";
            else
                ubb = "[attach]"+id+"[/attach]";
            html = ubb;
        }
    }
    if(hasEditor)
    {
        if (KE.g['editor_content'].wyswygMode == false)
            html = ubb;
        else
            KE.util.selection('editor_content');
            
        KE.util.insertContent('editor_content', html, ubb);
    }
    else
        $('editor_content').value += ubb;
}

function postCheck () {
    if (checkSubject==true && $('subject').value == '') {
        alert( '标题不能为空');
        return false;
    }
    if ($('editor_content').value == '') {
        alert( '内容不能为空');
        return false;
    }
    return true;
}

function setRealReward(rewardTax) {
    var reward=document.getElementsByName("reward")[0];
    if (reward.value=="") {
        $("realReward").innerHTML="0";
        return ;
    }
    if(reward.value.indexOf("0")==0)
        reward.value=reward.value.substring(1,reward.value.length-1);
    var realReward=parseInt(reward.value)+reward.value*rewardTax;
    var dotIndex=realReward.toString().indexOf(".");
    if(dotIndex>-1) {
        realReward=parseInt(realReward.toString().substring(0,dotIndex));
        realReward=realReward+1;
    }
    $("realReward").innerHTML=realReward;
}


function iconSelected (th) {
    var src = value = '';
    if (th.tagName.toLowerCase() == 'a') {
        src = root + '/max-assets/icon-post/icon0.gif';
        value = '0';
    }
    else {
        src = th.src;
        value = th.alt;
    }
    $('post_icon').src = src;
    document.getElementsByName('postIcon')[0].value = value;
    $('editor_moodlist').hide();
    return false;
}

// 编辑状态，选择帖子图标
function initIconSelected () {
    var icons = $('editor_moodlist').getElementsByTagName('img');
    var value = document.getElementsByName('postIcon')[0].value;
    for (var i = 0, len = icons.length; i < len; i++) {
        if (icons[i].alt == value) {
            $('post_icon').src = icons[i].src;
            return;
        }
    }
}

function savePost(e) {
    if (!postCheck()) {
        if (e) Events.CancelAll(e);
        return;
    }
    // 设置当前进行提交
    //editor.Submitting(true);
//    if (AjaxSettingPost) {
		Widget.ShowLoading('viewloading');
		//setTimeout(function () {
		if(hasEditor)
			KE.util.setData('editor_content');
			AjaxRequest.Submit('postButton','ap_validatecode','ap_error');
		//}, 100);
//	}
//	else {
//		Request.Submit('postButton');
//	}
}


var beforeProcessUbbImg = function(ubb) {
    ubb = ubb.replace( new RegExp('\\[localmedia\\s*=\\s*(\\d+),(\\d+),(\\d+)\s*\\]([\\d]+?)\\[\\/localmedia\\]','ig'), '<img src="' + root + '/max-assets/images/blank.gif"  width="$1" height="$2" alt=" _auto=$3 &mode=media&id=-$4" style="border:solid 1px #aaaaaa; background-position:center center; background-repeat:no-repeat;background-image:url('+attachUrl+'&mode=media&id=-$4)\" />');
    ubb = ubb.replace(new RegExp('\\[localmedia\\]([\\d]+?)\\[\\/localmedia\\]','ig'), '<img src="' + root + '/max-assets/images/blank.gif"  width="400" height="300" alt=" _auto=0 &mode=media&id=-$1" style="border:solid 1px #aaaaaa; background-position:center center; background-repeat:no-repeat;background-image:url('+attachUrl+'&mode=media&id=-$1)\" />');
    
    ubb = ubb.replace(/\[attachmedia\s*=\s*(\d+),(\d+),(\d+)\s*\]([\d]+?)\[\/attachmedia\]/ig, '<img src="' + root + '/max-assets/images/blank.gif"  width="$1" height="$2" alt=" _auto=$3 &mode=media&id=$4" style="border:solid 1px #aaaaaa; background-position:center center; background-repeat:no-repeat;background-image:url('+attachUrl+'&mode=media&id=$4)\" />');
    ubb = ubb.replace(/\[attachmedia\]([\d]+?)\[\/attachmedia\]/ig, '<img src="' + root + '/max-assets/images/blank.gif"  width="400" height="300" alt=" _auto=0 &mode=media&id=$1" style="border:solid 1px #aaaaaa; background-position:center center; background-repeat:no-repeat;background-image:url('+attachUrl+'&mode=media&id=$1)\" />');
    
    ubb = ubb.replace(/\[localimg\s*=\s*(\d+),(\d+)\s*\]([\d]+?)\[\/localimg\]/ig, '<img src="'+attachUrl+'&mode=image&id=-$3" width="$1" height="$2" alt="" />');
    ubb = ubb.replace(/\[localimg\]([\d]+?)\[\/localimg\]/ig, '<img src="'+attachUrl+'&mode=image&id=-$1" alt="" />');
    ubb = ubb.replace(/\[attachimg\s*=\s*(\d+),(\d+)\s*\]([\d]+?)\[\/attachimg\]/ig, '<img src="'+attachUrl+'&mode=image&id=$3" width="$1" height="$2" alt="" />');
    ubb = ubb.replace(/\[attachimg\]([\d]+?)\[\/attachimg\]/ig, '<img src="'+attachUrl+'&mode=image&id=$1" alt="" />');
    
    
    ubb = ubb.replace(/\[diskmedia\s*=\s*(\d+),(\d+),(\d+)\s*\]([\d]+?)\[\/diskmedia\]/ig, '<img src="' + root + '/max-assets/images/blank.gif"  width="$1" height="$2" alt=" _auto=$3 &mode=media&diskfileid=$4" style="border:solid 1px #aaaaaa; background-position:center center; background-repeat:no-repeat;background-image:url('+attachUrl+'&mode=media&diskfileid=$4)\" />');
    ubb = ubb.replace(/\[diskmedia\]([\d]+?)\[\/diskmedia\]/ig, '<img src="' + root + '/max-assets/images/blank.gif"  width="400" height="300" alt=" _auto=0 &mode=media&diskfileid=$1" style="border:solid 1px #aaaaaa; background-position:center center; background-repeat:no-repeat;background-image:url('+attachUrl+'&mode=media&diskfileid=$1)\" />');
    ubb = ubb.replace(/\[diskimg\s*=\s*(\d+),(\d+)\s*\]([\d]+?)\[\/diskimg\]/ig, '<img src="'+attachUrl+'&mode=image&diskfileid=$3" width="$1" height="$2" alt="" />');
    ubb = ubb.replace(/\[diskimg\]([\d]+?)\[\/diskimg\]/ig, '<img src="'+attachUrl+'&mode=image&diskfileid=$1" alt="" />');
    
    return ubb;
}

var beforeProcessImg = function(html) {

    //'<img src="'+attachUrl+'&mode=image&id=3" width="12" height="13" alt="" />'
    //'[localimg=12,13]3[/localimg]'


    html = html.replace(/<img(\s+[^>]+?)\/?>/ig, function(all, attr) {

        var url = attr.match(/\s+src="([^"]+?)"/i), str = '[localimg';

        var w, h;


        w = attr.match(/width\s*:\s*(\d+)/i);
        if (w == null) {
            w = all.match(/width[=:][^\d]*(\d+)/i);
        }
        if (w) w = w[1];
        h = attr.match(/height\s*:\s*(\d+)/i);
        if (h == null)
            h = all.match(/height[=:][^\d]*(\d+)/i);

        if (h) h = h[1];


        var id = 0;
        var reg = new RegExp('&(?:amp;)?mode=(.+?)&(?:amp;)?id=(-?[\\d]+)[^"]*?"', 'ig');
        var isMatch = false;
        var isDiskFile = false;
        var isMedia = false;
        var m = reg.exec(attr);
        if (m) {
            if (m[1] == 'media')
                isMedia = true;
            isMatch = true;
            id = m[2];
        }
        reg = new RegExp('&(?:amp;)?mode=(.+?)&(?:amp;)?diskfileid=([\\d]+)[^"]*?"', 'ig');

        if (isMatch == false) {
            if (m = reg.exec(attr)) {
                if (m[1] == 'media')
                    isMedia = true;
                isDiskFile = true;
                isMatch = true;
                id = m[2];
            }
        }
        var reg=new RegExp

        if (isMatch == false)
            return all;

        if (w && h) str += '=' + w + ',' + h;

        if (isMedia) {
            reg = new RegExp('\\s_auto=(\\d+)\\s', 'ig');
            if(m = reg.exec(attr)) {
                if (m[1] == '0')
                    str += ',0';
                else
                    str += ',1';
            }
        }

        str += ']';

        if (isDiskFile) {
            if (isMedia) {
                str = str.replace('localimg', 'diskmedia');
                return str + id + '[/diskmedia]';
            }
            else {
                str = str.replace('localimg', 'diskimg');
                return str + id + '[/diskimg]';
            }
        }
        else {
            if (id > 0) {
                if (isMedia) {
                    str = str.replace('localimg', 'attachmedia');
                    return str + id + '[/attachmedia]';
                }
                else {
                    str = str.replace('localimg', 'attachimg');
                    return str + id + '[/attachimg]';
                }
            }
            else {
                if (isMedia) {
                    str = str.replace('localimg', 'localmedia');
                    return str + (0 - id) + '[/localmedia]';
                }
                else
                    return str + (0 - id) + '[/localimg]';
            }
        }
    });

    return html;
}