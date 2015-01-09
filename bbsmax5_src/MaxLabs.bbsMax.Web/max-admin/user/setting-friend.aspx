<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>好友设置</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
    <h3>好友设置</h3>
    <form action="$_form.action" method="post">
    <div class="FormTable">
        <table>
            <!--[error name="MaxFriendCount"]-->
            <!--[include src="../_error_.aspx" /]-->
            <!--[/error]-->
            <tr>
                <th>
                    <h4>用户好友数量上限</h4>
                    <input type="text" class="text number" name="MaxFriendCount" value="$_form.text('MaxFriendCount', $FriendSettings.MaxFriendCount)" />
                </th>
                <td>每个用户允许添加好友数</td>
            </tr>
            <!--[error name="MaxFriendGroupCount"]-->
            <!--[include src="../_error_.aspx" /]-->
            <!--[/error]-->
            <tr>
                <th>
                    <h4>用户好友分组个数</h4>
                    <input type="text" class="text number" name="MaxFriendGroupCount" value="$_form.text('MaxFriendGroupCount', $FriendSettings.MaxFriendGroupCount)" />
                </th>
                <td>用户默认好友分组个数</td>
            </tr>
            <tr class="nohover">
                <th>
                    <input type="submit" value="保存设置" name="savesetting" class="button" />
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
