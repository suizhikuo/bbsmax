<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<!--[include src="../_htmlhead_.aspx"/]-->
    <title>对话设置</title>
</head>
<body>
<!--[include src="../_head_.aspx" /]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
    <form id="form1" method="post" action="$_form.action">
    <div class="FormTable">
    
       <table style="margin-bottom:1px;">
<tr>
    <th>
        <div class="itemtitle">
            <strong>开启对话（短消息）功能</strong>
            </div>

            <input type="radio" name="EnableChatFunction" value="true" $_form.checked('EnableChatFunction','true',$ChatSettings.EnableChatFunction) id="EnableChatFunction1" /><label for="EnableChatFunction1">开启</label>
            <input type="radio" name="EnableChatFunction" value="false" $_form.checked('EnableChatFunction','false',!$ChatSettings.EnableChatFunction) id="EnableChatFunction2" /><label for="EnableChatFunction2">关闭</label>
</th>
</tr>
</table>
    <table>
    <tr>
    <th>
     <h4>启用默认表情</h4>    <input type=checkbox value="true" id="Checkbox0" name="EnableDefaultEmoticon" $_form.checked("EnableDefaultEmoticon", "true", $EnableDefaultEmoticon) /> <label for="Checkbox0">开启</label>
    </th>
    <td>
 
    </td>
    </tr>
   <tr>
    <th>
     <h4>启用自定义表情</h4>    <input type=checkbox value="true"  id="Checkbox1" name="EnableUserEmoticon" $_form.checked("EnableUserEmoticon", "true", $EnableUserEmoticon) /> <label for="Checkbox1">开启</label>
    </th>
    <td>
 
    </td>
    </tr>
    <!--[error name="DefaultMessageSound"]-->
    <!--[pre-include src="../_error_.aspx"/]-->
    <!--[/error]-->
        <tr>
            <th>
                <h4>默认短消息声音</h4>
                <select id="ssss" name="MessageSoundSrc" onchange="playSound()">
                <option value="">无</option>
                <!--[loop $s in $SoundList]-->
                <option value="$s.url" $_form.selected("MessageSoundSrc",$s.url,$s.url==$CurrentSound)>$s.filename</option>
                <!--[/loop]-->
                </select> &nbsp;&nbsp;&nbsp;<a href="javascript:void(playSound())">播放</a>
                <br />
                <iframe style="height:30px; width:350px;" scrolling="no" frameborder="0" src="$dialog/chat-uploadsound.aspx" allowtransparency="true"></iframe>
                <div style="display:none;" >
                <object id="soundplayer" classid="clsid:6BF52A52-394A-11D3-B153-00C04F79FAA6" type="application/x-oleobject" codebase="http://activex.microsoft.com/activex/controls/mplayer/en/nsmp2inf.cab#Version=6,4,7,1112" width="0" height="0"><param name="url" value="#" /><param name="autoStart" value="0" /></object>
                </div>
                <script type="text/javascript">
                    function playSound() {
                        var s = $("ssss").value;
                        var p = $("soundplayer");
                        var m;
                        if (s) {
                            p.controls.stop();
                            p.url = s.replace('~','');
                            p.controls.play();
                        }
                    }

                var onFileUpload = function(r) {
                        var s = $("ssss");
                        var o = addElement("option", s);
                        o.innerHTML = r.filename;
                        o.value = r.url;
                        o.selected = true;
                    }
                </script>
            </th>
            <td>支持的声音文件类型:mp3、wma、wave、midi。</td>
        </tr>
    
    <!--[error name="NotifySaveDays"]-->
            <!--[include src="../_error_.aspx" /]-->
    <!--[/error]-->
    <tr>
    <th>
    <h4>对话历史消息清理设置</h4>    
    <!--[load src="../_dataclearoption_.ascx" SaveRows="$SaveMessageRows" DataClearMode="$DataClearMode" SaveDays="$SaveMessageDays" /]-->
    </th>
    
    <td>
    超过这个天数的通知将被自动清除（如果为0或者小于0 ， 将不会执行清除通知任务）
    </td>
    
    </tr>
    <!--[error name="ClearJobExecuteTime"]-->
    <!--[include src="../_error_.aspx" /]-->
    <!--[/error]-->
    <tr>
    <th>
    
 
    
    <h4>任务执行时间</h4>  每天的<input type="text" class="text" style="width:50px;" value="$_form.text('ClearMassgeExecuteTime',$ClearMassgeExecuteTime)" name="ClearMassgeExecuteTime" /> 
    点
    </th>
    <td>
    每天的几点执行对话内容的任务， 请输入0~23之间的数字（一般凌晨1点到5点间网站访问量较少，适合执行这样的任务）
    </td>
    </tr>
    
    <tr>
    <th>
        <input type="submit" value="保存设置" class="button" name="savesetting" />
    </th>
    <td colspan="2">&nbsp;</td>
        </tr>
    </table>
    </div>
    </form>
</div>
    <!--[include src="../_foot_.aspx"/]-->
</body>
</html>
