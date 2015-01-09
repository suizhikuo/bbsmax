<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>动态模板管理</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx"/]-->

<!--[include src="../_setting_msg_.aspx"/]-->
<!--[FeedTemplateList appID="$_get.appid"]-->


<!--[head]-->
<div class="Content">
    <h3>$CurrentApp.AppName动态模板</h3>
    <form action="$_form.action" method="post">
    <div class="FormTable">
    <table class="multiColumns">
        <thead>
            <tr>
                <td style="width:100px;">动作名称</td>
                <td style="width:200px;">图标</td>
                <td>标题</td>
                <td>摘要</td>
            </tr>
        </thead>
        <tbody id="TableBody">
<!--[/head]-->
<!--[item]-->
    <!--[error line="$i"]-->
        <tr class="ErrorMessage">
            <td colspan="4" class="Message"><div class="Tip Tip-error">$Messages</div></td>
        </tr>
        <tr class="ErrorMessageArrow">
            <td>&nbsp;</td>
            <td><!--[if $HasError("FeedTemplate.IconSrc")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
            <td><!--[if $HasError("FeedTemplate.Title")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
            <td><!--[if $HasError("FeedTemplate.Description")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
        </tr>
    <!--[/error]-->
        <tr>
            <td>
            <input type="hidden" name="FeedTemplateIDs.$i" value="$_form.text('FeedTemplateIDs.$i',$i)" />
            <input type="hidden" name="FeedTemplateIDs" value="$_form.text('FeedTemplateIDs.$i',$i)" />
            <input type="hidden" name="FeedTemplate.ActionType.$i" value="$_form.text('FeedTemplate.ActionType.$i',$FeedTemplate.ActionType)" />
            <input type="hidden" name="FeedTemplate.Changed.$i" id="FeedTemplate.Changed.$i" value="1" />
            <script type="text/javascript">$('FeedTemplate.Changed.$i').value = '0';</script>
            <input type="hidden" name="FeedTemplate.ActionName.$i" value="$_form.text('FeedTemplate.ActionName.$i',$GetActionNameByType($currentApp,$FeedTemplate.ActionType))" />
            <!--[if $hasAnyError]-->
            $_post['FeedTemplate.ActionName.$i']
            <!--[else]-->
            $GetActionNameByType($currentApp,$FeedTemplate.ActionType)
            <!--[/if]-->
            </td>
            <td><input type="text" class="text" style="width:150px;" id="FeedTemplate.IconSrc.$i" name="FeedTemplate.IconSrc.$i" value="$_form.text('FeedTemplate.IconSrc.$i',$FeedTemplate.IconSrc)" onchange="$('FeedTemplate.Changed.$i').value = '1';" />
                <a title="选择图片" class="selector-image" href="javascript:void(browseImage('Assets_Icon','FeedTemplate.IconSrc.$i'))"><img src="$Root/max-assets/images/image.gif" alt="" /></a>
            </td>
            <td><textarea name="FeedTemplate.Title.$i" style="width:100%;" cols="20" rows="2" onchange="$('FeedTemplate.Changed.$i').value = '1';">$_form.text('FeedTemplate.Title.$i',$FeedTemplate.Title)</textarea></td>
            <td><textarea name="FeedTemplate.Description.$i" style="width:100%;" cols="20" rows="2" onchange="$('FeedTemplate.Changed.$i').value = '1';">$_form.text('FeedTemplate.Description.$i',$FeedTemplate.Description)</textarea></td>
        </tr>
<!--[/item]-->
<!--[foot]-->
        <tr>
            <td>&nbsp;</td>
            <td colspan="3">
            <input type="submit" name="savefeedtemplates" value="保存" class="button" />
            要恢复默认值的模板请设置为空.
            </td>
        </tr>
        </tbody>
    </table>
    </div>
    </form>
</div>
<!--[/foot]-->
<!--[/FeedTemplateList]-->
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>
