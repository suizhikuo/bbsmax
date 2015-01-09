<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
<title>任务管理</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<form action="$_form.action" method="post" name="missionLists">
<div class="Content">
    <div class="PageHeading">
        <h3>任务列表</h3>
        <div class="ActionsBar">
            <a href="?disable=$EnableMission"><span><!--[if $EnableMission]-->关闭任务功能<!--[else]-->开启任务功能<!--[/if]--></span></a>
            <a href="$dialog/mission-missionbaselist.aspx" onclick="return openDialog(this.href, function(result){})"><span>添加任务</span></a>
            <a href="$admin/interactive/manage-mission-detail.aspx?type=group"><span>添加任务组</span></a>
        </div>
    </div>
    <!--[if $EnableMission == false]-->
    <div class="Help">
    <span style="color:Red">当前的任务功能是关闭的，如果要使以下任务有效请先开启任务功能</span>
    </div>
    <!--[/if]-->
<!--[MissionList page="$_get.page" pageSize="20"]-->
    <!--[head]-->
	<div class="DataTable">
        <h4>任务 <span class="counts">总数: $totalMissions</span></h4>
        <!--[if $hasItems]-->
        <table>
		<thead>
			<tr>
				<td class="CheckBoxHold">&nbsp;</td>
				<td style="width:30px;">ID</td>
				<td style="width:30px;">启用</td>
				<td style="width:30px;">排序</td>
				<td>任务名称</td>
				<td>分类</td>
				<td style="width:140px;">图标</td>
				<td style="width:140px;">开始时间</td>
				<td style="width:140px;">结束时间</td>
				<td style="width:100px;">操作</td>
			</tr>
		</thead>
		<tbody>
		<!--[/if]-->
    <!--[/head]-->
    <!--[item]-->
            <!--[error line="$i"]-->
                <tr class="ErrorMessage">
                <td colspan="9" class="Message"><div class="Tip Tip-error">$Messages</div></td>
                </tr>
                <tr class="ErrorMessageArrow">
                <td>&nbsp;</td>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
                <td><!--[if $HasError("sortorder")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
                <td><!--[if $HasError("name")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
                <td></td>
                <td><!--[if $HasError("iconurl")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
                <td><!--[if $HasError("begindate")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
                <td><!--[if $HasError("enddate")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
                <td>&nbsp;</td>
                </tr>
            <!--[/error]-->
			<tr id="mission_$mission.ID">
			    <td><input value="$Mission.ID" name="missionIDs" type="checkbox" /></td>
			    <td>$Mission.ID <input type="hidden" name="ids" value="$Mission.ID" /><input type="hidden" name="missionID.$Mission.ID" value="$Mission.ID" /></td>
			    <td><input type="checkbox" name="isenable.$Mission.ID" value="true" $_form.checked('isenable.$Mission.ID','true',$Mission.IsEnable) /></td>
			    <td><input type="text" class="text" style="width:5em;" name="sortorder.$Mission.ID" value="$_form.text('sortorder.$Mission.ID',$Mission.SortOrder)" /></td>
			    <td><input type="text" class="text" style="width:98%;" name="name.$Mission.ID" value="$_form.text('name.$Mission.ID',$Mission.Name)" /></td>
			    <td>
		        <select name="category_$mission.id">
		        <option $_if($mission.categoryid == null, 'selected="selected"')>未分类</option>
		        <!--[loop $category in $CategoryList]-->
		        <option value="$category.id" $_if($mission.categoryid == $category.id, 'selected="selected"')>$category.name</option>
		        <!--[/loop]-->
		        </select>
			    </td>
			    <td>
			        <input type="text" class="text" style="width:8em;" name="iconurl.$Mission.ID" id="iconurl.$Mission.ID" value="$_form.text('iconurl.$Mission.ID',$Mission.IconUrl)" />
			        <a title="选择图片" class="selector-image" href="javascript:void(browseImage('Assets_MissionIcon','iconurl.$Mission.ID'))">
			        <img src="$root/max-assets/images/image.gif" alt="" />
			        </a>
			    </td>
			    <td>
			        <input type="text" class="text" style="width:8em;" name="begindate.$Mission.ID" id="begindate.$Mission.ID" value="$_form.text('begindate.$Mission.ID',$outputDateTime($Mission.BeginDate,"",""))" />
			        <a class="selector-date" title="选择日期" id="dts_$Mission.ID" href="javascript:void(0);">
                        <img src="$Root/max-assets/images/calendar.gif" alt="选择日期" />
                    </a>
			        <script type="text/javascript">
			            initDatePicker('begindate.$Mission.ID','dts_$Mission.ID');
			        </script>
			    </td>
			    <td>
			        <input type="text" class="text" style="width:8em;" name="enddate.$Mission.ID" id="enddate.$Mission.ID" value="$_form.text('enddate.$Mission.ID',$outputDateTime($Mission.EndDate,"",""))" />
			        <a class="selector-date" title="选择日期" href="javascript:void(0);" id="dte_$Mission.ID">
                        <img src="$Root/max-assets/images/calendar.gif" alt="选择日期" />
                    </a>
			        <script type="text/javascript">
			            initDatePicker('enddate.$Mission.ID','dte_$Mission.ID');
			        </script>
			    </td>
			    <td>
			    <!--[if $mission.type == "group"]-->
          <a href="$dialog/mission-missionbaselist.aspx?pid=$mission.id" onclick="return openDialog(this.href, function(result){})">添加子任务</a>
			    <!--[else]-->
			    <a href="$admin/interactive/manage-mission-detail.aspx?missionid=$Mission.ID">编辑</a>
			    <!--[/if]-->
			    <a href="$dialog/mission-delete.aspx?missionid=$Mission.ID" onclick="return openDialog(this.href, function(r){removeElement('mission_$mission.id')})">删除</a>
			    </td>
			</tr>
			<!--[if $mission.childmissions.count > 0]-->
			<!--[loop $child in $mission.childmissions with $j]-->
			<tr id="mission_$child.ID">
			    <td style="padding-left:20px"><input value="$child.ID" name="missionIDs" type="checkbox" /></td>
			    <td>$child.ID <input type="hidden" name="ids" value="$child.ID" /><input type="hidden" name="missionID.$child.ID" value="$child.ID" /></td>
			    <td><input type="checkbox" checked="checked" disabled="disabled" /></td>
			    <td><input type="text" class="text" style="width:5em;" name="sortorder.$child.ID" value="$_form.text('sortorder.$child.ID',$child.SortOrder)" /></td>
			    <td><input type="text" class="text" style="width:98%;" name="name.$child.ID" value="$_form.text('name.$child.ID',$child.Name)" /></td>
			    <td>
		        <select disabled="disabled">
		        <!--[loop $category in $CategoryList]-->
		        <option $_if($mission.categoryid == null, 'selected="selected"')>未分类</option>
		        <!--[if $category.id == $mission.categoryid]-->
		        <option value="$category.id" $_if($child.categoryid == $category.id, 'selected="selected"')>$category.name</option>
		        <!--[/if]-->
		        <!--[/loop]-->
		        </select>
			    </td>
			    <td>
			        <input type="text" class="text" style="width:8em;" name="iconurl.$child.ID" id="iconurl.$child.ID" value="$_form.text('iconurl.$child.ID',$child.IconUrl)" />
			        <a title="选择图片" class="selector-image" href="javascript:void(browseImage('Assets_MissionIcon','iconurl.$child.ID'))">
			        <img src="$root/max-assets/images/image.gif" alt="" />
			        </a>
			    </td>
			    <td colspan="3">
			    <a href="$admin/interactive/manage-mission-detail.aspx?missionid=$child.ID">编辑</a>
			    <a href="$dialog/mission-delete.aspx?missionid=$child.ID" onclick="return openDialog(this.href, function(r){removeElement('mission_$child.id')})">删除</a>
			    </td>
			</tr>
			<!--[/loop]-->
			<!--[/if]-->
    <!--[/item]-->
    <!--[foot]-->
		<!--[if $hasItems]-->
		</tbody>
	    </table>
        <div class="Actions">
            <input name="checkAll" id="selectAll" type="checkbox" />
            <label for="selectAll">全选</label>
            <input class="button" name="deletemissions" onclick="return confirm('确认要删除所选吗?');" type="submit" value="删除所选" /> 
            <input class="button" name="savemissions" type="submit" value="保存更改" />
        </div>
        <script type="text/javascript">
            new checkboxList('missionIDs', 'selectAll');
        </script>
        <!--[AdminPager ListID="" Count="$totalMissions" PageSize="20"/]-->
        <!--[else]-->
        <div class="NoData">当前没有任何任务.</div>
        <!--[/if]-->
	</div>
    <!--[/foot]-->
<!--[/MissionList]-->

</div>
</form>
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>