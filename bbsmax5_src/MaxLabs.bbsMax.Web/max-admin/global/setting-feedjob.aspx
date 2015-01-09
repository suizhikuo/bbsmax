<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>定时删除用户动态</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
    <h3>自动清理用户动态</h3>
    <form action="$_form.action" method="post">
    <div class="FormTable">
    <table>
        <!--[error name="enableJob"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
        <tr>
            <th>
                <h4>开启自动清理</h4>
                <input type="radio" id="enable" name="enableJob" value="true" $_form.checked('enableJob','true',$FeedJobSetting.Enable) />
                <label for="enable">是</label>
                <input type="radio" id="disable" name="enableJob" value="false" $_form.checked('enableJob','false',$FeedJobSetting.Enable==false) />
                <label for="disable">否</label>
            </th>
            <td>&nbsp;</td>
        </tr>
        <!--[error name="executetime"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
        <tr>
            <th>
                <h4>每天清理时间</h4>
                <input class="text number" type="text" name="executetime" value="$_form.text('executetime',$FeedJobSetting.ExecuteTime)" />点
            </th>
            <td>
                请填0-23中的一个数字.<br />
                例如填写的数值为1, 表示程序将在每天的01:00执行此操作. 请选择在网站用户较少的时间段执行此操作.
            </td>
        </tr>
        <!--[error name="clearMode"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
        <tr>
            <th>
                <h4>自动清理规则</h4>
                <input type="radio" name="clearMode" value="ClearByDay" $_form.checked('clearMode','ClearByDay','$ClearModeValue')>自动清理<input class="text number" type="text" name="days1" value="$_form.text('days1',$FeedJobSetting.Day)" />天以前的动态<br />
                <input type="radio" name="clearMode" value="ClearByRows" $_form.checked('clearMode','ClearByRows','$ClearModeValue')>只保留最新的<input class="text number" type="text" name="count" value="$_form.text('count',$FeedJobSetting.Count)" />条动态，其余的自动清理
                   <br />&nbsp;&nbsp;&nbsp;&nbsp;<input type="checkbox" name="CombinMode" value="true" $_form.checked('CombinMode','true',$IsCombinMode)>但<input class="text number" type="text" name="days2" value="$_form.text('days2',$FeedJobSetting.Day)" />天内的动态除外，不要自动清理
            </th>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <th>
                <input type="submit" name="savesetting" value="保存设置" class="button" />
            </th>
            <td>&nbsp;</td>
        </tr>
    </table>
    </div>
    </form>
</div>
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>
