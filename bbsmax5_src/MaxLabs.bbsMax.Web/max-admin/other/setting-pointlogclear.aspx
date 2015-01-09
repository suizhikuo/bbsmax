<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>清理积分记录设置</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<!--[if $_get.success == "1"]-->
<div class="Tip Tip-success">操作成功</div>
<!--[/if]-->
<div class="Content">
     <div class="PageHeading">
    <h3>清理积分记录设置</h3>
    <div class="ActionsBar">
        <a href="manage-pointlog.aspx" class="back"><span>返回积分记录管理</span></a>
    </div>
    </div>
    <form action="$_form.action" method="post">
    <div class="FormTable">
    <table>
        <!--[error name="days"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
        <tr>
            <th>
                <h4>过期数据清除方式</h4>
                <!--[load src="../_dataclearoption_.ascx" SaveRows="$Settings.SaveLogRows" DataClearMode="$Settings.DataClearMode" SaveDays="$Settings.SaveLogDays" /]-->
            </th>
            <td>
            </td>
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
