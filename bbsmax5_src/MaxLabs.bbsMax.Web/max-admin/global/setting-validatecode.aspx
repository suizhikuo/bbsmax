<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>验证码设置</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
    <h3>验证码设置</h3>
    <div class="Help">
    <p><strong>"智能设置"</strong>是指在指定的固定时间内发生指定次数的相同动作操作的情况下, 则出现验证码. 如果未设置则表示每次动作操作都出现验证码.</p>
    </div>
    <form action="$_form.action" method="post">
    <div class="DataTable">
        <table>
        <thead>
        <tr>
            <td>动作</td>
            <td class="CheckBoxHold">启用</td>
            <td>验证码样式</td>
            <td>编辑</td>
            <td>达到此操作频率才出现</td>
            <td>例外 (以下用户组不需要验证码)</td>
        </tr>
        </thead>
        <tbody>
        <!--[loop $action in $ValidateCodeActionList with $i]-->
            <!--[error line="$i"]-->
            <tr class="ErrorMessage">
            <td colspan="6" class="Message"><div class="Tip Tip-error">$Messages</div></td>
            </tr>
            <tr class="ErrorMessageArrow">
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td><!--[if $HasError("limited")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
            <td>&nbsp;</td>
            </tr>
            <!--[/error]-->
        <tr>
            <td>$action.Name</td>
            <td><input type="checkbox" value="true" name="enable.$action.type" $_form.checked('enable.$action.type','true',$IsEnable($action.Type)) /></td>
            <td><img src="$GetImageUrl($action.Type)" alt="" /></td>
            <td><a href="$dialog/validatecode-style.aspx?actiontype=$action.Type" onclick="return openDialog(this.href, refresh)">更换验证码样式</a></td>
            <td>
                <input type="text" class="text" style="width:3em;" name="limitedTime.$action.type" value="$_form.text("limitedTime.$action.type",$GetLimitedTime($action.type))" />
			    <select name="limitedTimeType.$action.type">
                    <option value="0" $_form.selected("limitedTimeType.$action.type","0",$GetLimitedTimeUnit($action.type))>秒</option>
                    <option value="1" $_form.selected("limitedTimeType.$action.type","1",$GetLimitedTimeUnit($action.type))>分钟</option>
                    <option value="2" $_form.selected("limitedTimeType.$action.type","2",$GetLimitedTimeUnit($action.type))>小时</option>
			        <option value="3" $_form.selected("limitedTimeType.$action.type","3",$GetLimitedTimeUnit($action.type))>天</option>
                </select>
                <input type="text" name="limitedCount.$action.type" class="text" style="width:3em;" value="$_form.text('limitedCount.$action.type',$GetLimitedCount($action.type))" />
                次
            </td>
            <td>
                <!--[if $action.CanSetExceptRoleId]-->
                $GetRoleNames($action.Type)
                <a href="$dialog/validatecode-role.aspx?actiontype=$action.Type" onclick="return openDialog(this.href, refresh)">修改例外</a>
                <!--[else]-->
                该动作不能设置例外
                <!--[/if]-->
            </td>
        </tr>
        <!--[/loop]-->
        </tbody>
        </table>
        <div class="Actions">
            <input class="button" name="savevalidatecode" type="submit" value="保存设置" />
        </div>
    </div>
    </form>
</div>
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>
