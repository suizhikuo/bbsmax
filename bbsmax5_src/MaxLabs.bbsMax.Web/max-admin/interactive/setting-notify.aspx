<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>通知系统设置</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx" /]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
    <form id="form1" method="post" action="$_form.action">
    <div class="FormTable">
    <table>
    <!--[loop $item in $NotifyTypeList]-->
    <tr>
    <th>
     <h4>$item.typename</h4>    
     <input type="radio" name="{=$item.typeid}_OpenState" value="AlwaysOpen" $_form.checked($item.typeid+"_OpenState","AlwaysOpen",$GetState($item.typeid)==NotifyState.AlwaysOpen) id="m_{=$item.typeid}_1" /> <label for="m_{=$item.typeid}_1">始终打开</label>
     <input type="radio" name="{=$item.typeid}_OpenState" value="AlwaysClose" $_form.checked($item.typeid+"_OpenState","AlwaysClose",$GetState($item.typeid)==NotifyState.AlwaysClose) id="m_{=$item.typeid}_2" /> <label for="m_{=$item.typeid}_2">始终关闭</label>
     <input type="radio" name="{=$item.typeid}_OpenState" value="DefaultOpen" $_form.checked($item.typeid+"_OpenState","DefaultOpen",$GetState($item.typeid)==NotifyState.DefaultOpen) id="m_{=$item.typeid}_3" /> <label for="m_{=$item.typeid}_3">默认打开</label>
     <input type="radio" name="{=$item.typeid}_OpenState" value="DefaultClose" $_form.checked($item.typeid+"_OpenState","DefaultClose",$GetState($item.typeid)==NotifyState.DefaultClose) id="m_{=$item.typeid}_4" /> <label for="m_{=$item.typeid}_4">默认关闭</label>
    </th>
    <td>
    <!--$-item.descript-->
    </td>
    </tr>
    <!--[/loop]--> 
    <!--[error name="NotifySaveDays"]-->
            <!--[include src="../_error_.aspx" /]-->
    <!--[/error]-->
    <tr>
    <th>
    <h4>历史通知数据清理模式</h4>    
     <!--[load src="../_dataclearoption_.ascx" SaveRows="$NotifySaveRows" DataClearMode="$DataClearMode" SaveDays="$NotifySaveDays" /]-->
    </th>
    <td>
    </td>
    </tr>
    <!--[error name="ClearJobExecuteTime"]-->
    <!--[include src="../_error_.aspx" /]-->
    <!--[/error]-->
    <tr>
    <th>
    <h4>任务执行时间</h4>  每天的<input type="text" class="text" style="width:50px;" value="$_form.text('ClearJobExecuteTime',$ClearJobExecuteTime)" name="ClearJobExecuteTime" /> 
    点
    </th>
    <td>
    每天的几点执行清除通知的任务， 请输入0~23之间的数字（一般凌晨1点到5点间网站访问量较少，适合执行这样的任务）
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
<script type="text/javascript">

</script>
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>
