<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>定时删除系统日志</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<!--[if $_get.success == "1"]-->
<div class="Tip Tip-success">操作成功</div>
<!--[/if]-->
<div class="Content">
    <h3>定时删除系统日志</h3>
    <form action="$_form.action" method="post">
    <div class="FormTable">
    <table>
        <!--[error name="executetime"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
        <tr>
            <th>
                <h4>每天执行时间</h4>
                <input class="text number" type="text" name="executetime" value="$_form.text('executetime',$DeleteOperationLogJobSettings.ExecuteTime)" />
            </th>
            <td>
                请填0-23中的一个数字.<br />
                例如填写的数值为1, 表示程序将在每天的01:00执行此操作. 请选择在网站用户较少的时间段执行此操作.
            </td>
        </tr>
        <!--[error name="days"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
        <tr>
            <th>
                <h4>过期数据清除方式</h4>
                <!--[load src="../_dataclearoption_.ascx" SaveRows="$DeleteOperationLogJobSettings.SaveLogRows" DataClearMode="$DeleteOperationLogJobSettings.DataClearMode" SaveDays="$DeleteOperationLogJobSettings.SaveLogDays" /]-->
            </th>
            <td></td>
        </tr>
        <tr class="nohover">
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
