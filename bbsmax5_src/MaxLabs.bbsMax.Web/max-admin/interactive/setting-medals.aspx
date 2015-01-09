<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>点亮图标</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->

<div class="Content">
    <div class="PageHeading">

    <h3>点亮图标管理</h3>
    <div class="ActionsBar">
        <a href="$admin/interactive/setting-medal-detail.aspx"><span>添加图标</span></a>
    </div>
    </div>
    <div class="DataTable">
        <!--[if $MedalList.Count != 0]-->
        <form action="$_form.action" method="post">
        <table>
            <thead>
            <tr>
                <td class="CheckBoxHold"><input type="checkbox" id="selectAll" title="全选" /></td>
                <td>排序</td>
                <td>名称</td>
                <td>启用</td>
                <td>隐藏</td>
                <td>等级名称</td>
                <td>图标</td>
                <td>点亮规则</td>
                <td style="width:220px;">操作</td>
            </tr>
            </thead>
            <tbody>
            <!--[loop $Medal in $MedalList with $i]-->
            <!--[error line="$i"]-->
            <tr class="ErrorMessage">
                <td colspan="9" class="Message"><div class="Tip Tip-error">$Messages</div></td>
            </tr>
            <tr class="ErrorMessageArrow">
                <td>&nbsp;</td>
                <td>&nbsp;</td>
                <td><!--[if $HasError("name")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
                <td><!--[if $HasError("levelname")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
                <td><!--[if $HasError("iconsrc")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
                <td><!--[if $HasError("condition")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
                <td>&nbsp;</td>
            </tr>
            <!--[/error]-->
            <tr>
                <td><input type="checkbox" name="medalids" value="$medal.ID" /></td>
                <td><input class="text order" name="sortorder_$Medal.ID" type="text" value="$_form.text('sortorder_$Medal.ID','$Medal.SortOrder')" /></td>
                <td><input class="text" name="name_$Medal.ID" type="text" value="$_form.text('name_$Medal.ID','$Medal.Name')" /></td>
                <td><input type="checkbox" name="enable_$Medal.ID" value="true" $_form.checked('enable_$Medal.ID','true',$Medal.Enable) /></td>
                <td><input type="checkbox" name="isHidden_$Medal.ID" value="true" $_form.checked('isHidden_$Medal.ID','true',$Medal.IsHidden) /></td>
                <td>
                    <!--[loop $MedalLevel in $Medal.Levels]-->
                    <p>$MedalLevel.Name</p>
                    <!--[/loop]-->
                </td>
                <td>
                    <!--[loop $MedalLevel in $Medal.Levels]-->
                    <p><img alt="" src="$MedalLevel.LogoUrl" /></p>
                    <!--[/loop]-->
                </td>
                <td>
                    <!--[loop $MedalLevel in $Medal.Levels]-->
                    <p>
                    <!--[if $Medal.IsCustom]-->
                    说明: $MedalLevel.Condition
                    <!--[else]-->
                    $GetConditionName($Medal.Condition) 达到 $MedalLevel.Value
                    <!--[/if]-->
                    </p>
                    <!--[/loop]-->
                </td>
                <td>
                    <a href="$admin/interactive/setting-medal-detail.aspx?id=$medal.ID">编辑</a> |
                    <a href="$admin/interactive/setting-medals.aspx?action=delete&medalid=$medal.ID">删除</a> |
                    <a href="$dialog/medal-users.aspx?id={=$medal.ID}_all" onclick="return openDialog(this.href,refresh)">查看该勋章用户</a>
                </td>
            </tr>
            <!--[/loop]-->
            </tbody>
        </table>
         <script type="text/javascript">
            new checkboxList('medalids', 'selectAll');
        </script>
        <div class="Actions">
            <input type="submit" name="deletemedals" class="button" value="删除选中项目" onclick="return confirm('你确定要删除吗?')" />
            <input type="submit" name="savesetting" class="button" value="保存" />
        </div>
        </form>
        <!--[else]-->
        <div class="NoData">当前没有任何图标点亮规则.</div>
        <!--[/if]-->
    </div>
</div>
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>
