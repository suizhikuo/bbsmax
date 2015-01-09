<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<!--[Mission missionID="$_get.missionID"]-->
<head>
<title><!--[if $isEdit]-->编辑<!--[else]-->添加<!--[/if]--><!--[if $ParentMission != null]-->“$ParentMission.Name”子<!--[/if]-->任务<!--[if $_get.type == "group"]-->组<!--[/if]--></title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->

<form action="$_form.action" method="post">
<div class="Content">
    <div class="PageHeading">
	<h3><!--[if $isEdit]-->编辑<!--[else]-->添加<!--[/if]--><!--[if $ParentMission != null]-->“$ParentMission.Name”子<!--[/if]-->任务<!--[if $_get.type == "group"]-->组<!--[/if]--></h3>
	<div class="ActionsBar">
	    <a class="back" href="$admin/interactive/manage-mission-list.aspx"><span>任务列表</span></a>
	</div>
	</div>
	<div class="FormTable">
	<input type="hidden" name="pid" value="$_get.pid" />
	<table>
        <!--[error name="name"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
		<tr>
			<th>
			<h4>任务名称</h4>
			<input type="text" class="text" name="name" value="$_form.text("name",$out($mission.Name))" />
			</th>
			<td>任务的名称，会显示在前台的任务列表中，请尽量使用简洁的文字</td>
		</tr>
		<!--[if ($isedit && $mission.parentid == null) || ($isedit == false && $_get.pid == null)]-->
		<tr>
		  <th>
		  <h4>任务归类</h4>
		  <select name="category">
		  <option $_if($mission == null || $mission.categoryid == null, 'selected="selected"')>未分类</option>
		  <!--[loop $category in $CategoryList]-->
		  <option value="$category.id" $_if($mission != null && $mission.categoryid == $category.id, 'selected="selected"')>$category.name</option>
		  <!--[/loop]-->
		  </select>
		  </th>
		  <td>当任务多的时候，适当规律可以有效的引导用户</td>
		</tr>
		<tr>
			<th>
			<h4>启用</h4>
			<input type="radio" name="isenable" id="enable" value="True" $_if($_post.isenable == null || $_post.isenable == "True" || ($mission != null && $mission.IsEnable), 'checked="checked"') /><label for="enable">是</label>
			<input type="radio" name="isenable" id="disable" value="False" $_if($_post.isenable == "False" || ($mission != null && !$mission.IsEnable), 'checked="checked"') /><label for="disable">否</label>
			</th>
			<td>只有启用的任务才会显示在前台的任务列表中</td>
		</tr>
		<!--[/if]-->
        <!--[error name="sortorder"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
		<tr>
			<th>
			<h4>排序</h4>
			<input type="text" class="text number" name="sortorder" value="$_form.text("sortorder",$out($mission.SortOrder))" />
			</th>
			<td>填写整数,数字越小越靠前</td>
		</tr>
        <!--[error name="description"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
		<tr>
			<th>
			<h4>任务描述</h4>
			<textarea name="description" cols="30" rows="6">$_form.text("description",$out($mission.Description))</textarea>
			</th>
			<td>任务的细致说明，这里可以尽量详细说明此任务的要求和奖励，以便吸引会员参与. (支持HTML)</td>
		</tr>
		<!--[if ($isedit && $mission.parentid == null) || ($isedit == false && $_get.pid == null)]-->
        <!--[error name="iconurl"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
		<tr>
			<th>
			<h4>任务图标</h4>
			<input type="text" class="text" name="iconurl" id="iconurl" value="$_form.text("iconurl",$out($mission.IconUrl))" />
			<a title="选择图片" class="selector-image" href="javascript:void(browseImage('Assets_MissionIcon','iconurl'))">
			    <img src="$Root/max-assets/images/image.gif" alt="" />
			</a>
			</th>
			<td>如果留空，则使用默认图标。</td>
		</tr>
		<!--[/if]-->
		<!--[if ($isedit && $mission.parentid == null) || ($isedit == false && $_get.pid == null)]-->
        <!--[error name="begindate"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
		<tr>
			<th>
			<h4>开始时间</h4>
			<!--[if $mission==null]-->
			<input type="text" class="text datetime" name="begindate" id="begindate" value="$_form.text("begindate")" />
			<!--[else]-->
		    <input type="text" class="text datetime" name="begindate" id="begindate" value="$_form.text("begindate",$outputDateTime($mission.BeginDate,"",""))" />
		    <!--[/if]-->
		    <a class="selector-date" title="选择日期" id="dts_begindate" href="javascript:void(0);">
                <img src="$Root/max-assets/images/calendar.gif" alt="选择日期" />
            </a>
            <script type="text/javascript">
	            initDatePicker('begindate','dts_begindate');
	        </script>
			</th>
			<td>预定一个日期来开始这个任务，如果留空，则此任务立即开始</td>
		</tr>
        <!--[error name="enddate"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
		<tr>
			<th>
			<h4>结束时间</h4>
			<!--[if $mission==null]-->
			<input type="text" class="text datetime" name="enddate" id="enddate" value="$_form.text("enddate")" />
			<!--[else]-->
			<input type="text" class="text datetime" name="enddate" id="enddate" value="$_form.text("enddate",$outputDateTime($mission.EndDate,"",""))" />
			<!--[/if]-->
			<a class="selector-date" title="选择日期" id="dts_enddate" href="javascript:void(0);">
                <img src="$Root/max-assets/images/calendar.gif" alt="选择日期" />
            </a>
            <script type="text/javascript">
	            initDatePicker('enddate','dts_enddate');
	        </script>
			</th>
			<td>设置这个任务在某个时间结束，用于设计一个阶段性的活动。如果留空，则此任务可以长期执行。
			</td>
		</tr>
        <!--[error name="cycletime"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
		<tr>
			<th>
			<h4>周期</h4>
		    <!--[TimeFormater seconds="$out($mission.CycleTime,'0')"]-->
			<input type="text" class="text number" name="cycletime" value="$_form.text("cycletime",$time)" />
			<select name="cycletime.timetype">
                <option value="3" $_form.selected("cycletime.timetype","3",$timeUnit)">天</option>
                <option value="2" $_form.selected("cycletime.timetype","2",$timeUnit)>小时</option>
                <option value="1" $_form.selected("cycletime.timetype","1",$timeUnit)>分钟</option>
                <option value="0" $_form.selected("cycletime.timetype","0",$timeUnit)>秒</option>
            </select>
            <!--[/TimeFormater]-->
			</th>
			<td>设置允许用户多长时间后可以重新执行该任务。为0或者空，则只允许用户执行一次</td>
		</tr>
		<!--[if $_get.type != "group"]-->
        <!--[error name="deductpoint"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->	
		<tr>
			<th>
		    <h4>扣除积分</h4>
		    <table>
		    <!--[EnabledUserPointList]-->
			<tr><td style="padding:2px;">$Name:</td><td style="padding:2px;"><input type="text" class="text" style="width:10em;" name="deductpoint.$pointID" value="$_form.text("deductpoint.$pointID",$out($mission.DeductPoint[$pointID]))" /></td></tr>
			<!--[/EnabledUserPointList]-->
			</table>
			</th>
			<td>表示用户申请该任务时扣除用户的相应积分,如不需要扣除请设置为0或留空</td>
		</tr>
		<!--[/if]-->
		<!--[/if]-->
	</table>
	</div>

		<!--[if ($isedit && $mission.parentid == null) || ($isedit == false && $_get.pid == null)]-->
    <h3>申请任务条件</h3>
    <div class="FormTable">
    <table>
    <tr>
        <th>
        <h4>用户组限制</h4>
        <div style="margin-left:20px;">
        <!--[loop $role in $AllRoleList]-->
        <p style="float:left;width:200px;margin-right:5px;white-space:nowrap;">
            <!--[if $isEdit == true]-->
            <input type="checkbox" $_form.checked("applycondition.groups","$role.RoleID","$applyConditionUserGroups") value="$role.RoleID" id="applycondition.group.$role.RoleID" name="applycondition.groups" />
            <!--[else]-->
            <input type="checkbox" value="$role.RoleID" id="applycondition.group.$role.RoleID" name="applycondition.groups" />
            <!--[/if]-->
            <label for="applycondition.group.$role.RoleID">$role.Name</label>
        </p>
        <!--[/loop]-->
        </div>
        </th>
        <td>在选中用户组里的用户才能申请该任务，如果都不选，则所有用户组的用户都能申请.</td>
    </tr>
    
    <!--[error name="applyCondition.point"]-->
        <!--[include src="../_error_.aspx" /]-->
    <!--[/error]-->
    <tr>
        <th>
        <h4>积分达到</h4>
            <table>
            <tr><td style="padding:2px;">总积分</td><td style="padding:2px;"><input type="text" class="text number" name="applyCondition.point.total" value="$_form.text("applyCondition.point.total",$GetPointString($mission,-1))" /></td></tr>
            <!--[EnabledUserPointList]-->
            <tr><td style="padding:2px;">$Name</td><td style="padding:2px;"><input type="text" class="text number" name="applyCondition.point.$pointID" value="$_form.text("applyCondition.point.$pointID",$GetPointString($mission,$pointID))" /></td></tr>
            <!--[/EnabledUserPointList]-->
            </table>
        </th>
        <td>用户的每个积分类型的积分都大于左边的设置时才能申请该任务，如果不限制请留空.</td>
    </tr>
    <!--[error name="totalposts"]-->
        <!--[include src="../_error_.aspx" /]-->
    <!--[/error]-->
    <tr>
        <th>
            <h4>发帖数</h4>
            <input type="text" class="text number" name="totalposts" value="$_form.text("totalposts",$out($mission.ApplyCondition.TotalPosts))" />
        </th>
        <td>用户的发帖数大于左边的设置时才能申请。如果不限制发帖数，请设置为0或留空.</td>
    </tr>
    <!--[error name="onlinetime"]-->
        <!--[include src="../_error_.aspx" /]-->
    <!--[/error]-->
    <tr>
        <th>
            <h4>在线时间</h4>
            <input type="text" class="text datetime" name="onlinetime" value="$_form.text("onlinetime",$out($mission.ApplyCondition.OnlineTime))" /> 小时
        </th>
        <td>用户的在线时间大于左边的设置时才能申请。如果不限制在线时间，请设置为0或留空.</td>
    </tr>
    <tr>
        <th>
            <h4>必须完成指定任务</h4>
            <input type="text" class="text" name="othermissionids" value="$_form.text("othermissionids",$out($mission.ApplyCondition.OtherMissionIDString))" />
        </th>
        <td>
        <p>填写任务ID(可在<a href="$admin/interactive/manage-mission-list.aspx" target="_blank">任务列表</a>查看任务ID),多个用逗号","隔开.放空为不需要完成其他任务。</p>
        <p>在用户参与本任务的时候，是否需要先完成其他任务。利用此项设置，您可以设计一个系列任务，让会员逐一完成。</p>
        </td>
    </tr>
    <!--[error name="maxapplycount"]-->
        <!--[include src="../_error_.aspx" /]-->
    <!--[/error]-->	
    <tr>
        <th>
        <h4>申请人数上限</h4>
        <input type="text" class="text number" name="maxapplycount" value="$_form.text("maxapplycount",$out($mission.ApplyCondition.MaxApplyCount))" />
        </th>
        <td>当申请本任务的人次达到这个数值的时候，系统会自动拒绝新用户参与任务。如果不限制参与的人次，请设置为0或留空.</td>
    </tr>
		<!--[if $_get.type == "group"]-->
    <tr>
		    <th>
		    <input type="submit" value="保存设置" name="savemission" class="button" />
		    </th>
		    <td>&nbsp;</td>
	  </tr>
	  <!--[/if]-->
    </table>
    </div>
  <!--[/if]-->
  
		<!--[if $_get.type != "group"]-->
    <h3>完成任务条件</h3>
    <!--[if $isEdit]-->
        <!--[load src="~/max-plugins/$Mission.MissionBase.HtmlPath" mission="$mission" /]-->
    <!--[else]-->
        <!--[MissionBase type="$_get.type"]-->
            <!--[load src="~/max-plugins/$currentMissionBase.HtmlPath" mission="$mission" /]-->
        <!--[/MissionBase]-->
    <!--[/if]-->

    <h3>任务奖励</h3>
    <div class="FormTable">
    <table>
	    <tr>
		    <th>
		    <h4>奖励类型</h4>
            <input type="checkbox" id="Prize1" value="0" name="prizetypes" $_form.checked("prizetypes","0",$out($prizeTypes))  onclick="displayPrize();" /> <label for="Prize1">积分</label>
            <input type="checkbox" id="Prize2" value="1" name="prizetypes" $_form.checked("prizetypes","1",$out($prizeTypes))  onclick="displayPrize();" /> <label for="Prize2">用户组</label>
            <input type="checkbox" id="Prize3" value="2" name="prizetypes" $_form.checked("prizetypes","2",$out($prizeTypes))  onclick="displayPrize();" /> <label for="Prize3">点亮图标</label>
            <input type="checkbox" id="Prize4" value="3" name="prizetypes" $_form.checked("prizetypes","3",$out($prizeTypes))  onclick="displayPrize();" /> <label for="Prize4">邀请码</label>
            <input type="checkbox" id="Prize5" value="4" name="prizetypes" $_form.checked("prizetypes","4",$out($prizeTypes))  onclick="displayPrize();" /> <label for="Prize5">道具</label>
		    </th>
		    <td>
		    当会员完成这个任务后，系统将根据您的设置，自动为该会员颁发奖励。奖励的类型可以是积分、勋章、注册邀请码或者是将用户加入某个特殊用户组。您也可以设置这个任务没有任何奖励，不过这个任务对会员的吸引力就降低了
		    </td>
	    </tr>
	    <!--[error name="prize.point"]-->
            <tr id="jl1-1e">
                <td colspan="2" class="Message">
                    <div class="Tip Tip-error">$Message</div>
                    <div class="TipArrow">&nbsp;</div>
                </td>
            </tr>
        <!--[/error]-->	
	    <tr id="jl1-1">
		    <th>
		    <h4>积分奖励值</h4>
            <table>
            <!--[EnabledUserPointList]-->
		    <tr><td style="padding:2px;">$Name</td><td style="padding:2px;"><input type="text" class="text number" style="width:10em;" name="prize.point.$pointID" value="$_form.text("prize.point.$pointID",$out($mission.Prize.Points[$pointID]))" /></td></tr>
		    <!--[/EnabledUserPointList]-->
		    </table>
		    </th>
		    <td>
		    当会员完成这个任务后，系统将根据您的设置，自动为该会员颁发奖励。奖励的类型可以是积分、勋章、注册邀请码或者是将用户加入特殊用户组。您也可以设置这个任务没有任何奖励，不过这个任务对会员的吸引力就降低了
		    </td>
	    </tr>
        <!--[error name="prize.usergroup"]-->
            <tr id="jl2-1e">
                <td colspan="2" class="Message">
                    <div class="Tip Tip-error">$Message</div>
                    <div class="TipArrow">&nbsp;</div>
                </td>
            </tr>
        <!--[/error]-->	
        <tr id="jl2-1">
            <th>
            <h4>用户组</h4>
            <!--[if $RoleList.Count==0]-->
            <p>当前没有可以做为奖励的用户组，管理员组和等级组不能做为奖励，请到 <a href="$admin/user/setting-roles-other.aspx">用户组管理</a> 添加普通用户组，只有您新添加的普通用户组才能做为奖励</p>
            <!--[/if]-->
            <!--[loop $Role in $RoleList]-->
                <p>
                    <input type="checkbox" id="group.$Role.RoleID" value="$Role.RoleID" name="prizeusergroups" $_form.checked("prizeusergroups",$Role.RoleID,$out($prizeUserGroups))  />
                    <label for="group.$Role.RoleID">$Role.Name</label> 
                    <!--[UserGroupValidityTime mission="$mission" groupID="$Role.RoleID" isEdit="$isEdit"]-->
                    (有效时间:
                    <input type="text" class="text" style="width:3em;" name="group.time.$Role.RoleID" value="$time" />
                    <select name="group.timetype.$Role.RoleID">
                    <option value="3" $_form.selected("group.timetype."+$Role.RoleID,"3",$timeUnit)>天</option>
                    <option value="2" $_form.selected("group.timetype."+$Role.RoleID,"2",$timeUnit)>小时</option>
                    <option value="1" $_form.selected("group.timetype."+$Role.RoleID,"1",$timeUnit)>分钟</option>
                    <option value="0" $_form.selected("group.timetype."+$Role.RoleID,"0",$timeUnit)>秒</option>
                    </select>)
                    <!--[/UserGroupValidityTime]-->
                </p>
            <!--[/loop]-->
            </th>
	        <td>
	        <p>用户完成任务后会被加入选定的用户组</p>
	        <p>如果用户组不过期, 有效时间请留空或为0</p>
	        </td>
        </tr>
        <!--[error name="prize.medal"]-->
            <tr id="jl3-1e">
                <td colspan="2" class="Message">
                    <div class="Tip Tip-error">$Message</div>
                    <div class="TipArrow">&nbsp;</div>
                </td>
            </tr>
        <!--[/error]-->
        <tr id="jl3-1">
            <th>
            <h4>点亮图标</h4>
            <div style="margin-left:20px;">
            <!--[loop $medal in $Medals]-->
            <p>
            <input type="checkbox" name="checkMedal" value="$medal.ID" $_form.checked("checkMedal","$medal.ID","$prizeMedalIDs") />
            $medal.Name
            <select name="medal.$Medal.ID">
            <!--[loop $medalLevel in $medal.Levels]-->
            <option value="{=$Medal.ID}_$medalLevel.ID" $_form.selected("medal.$Medal.ID","{=$Medal.ID}_$medalLevel.ID","$prizeMedals")>
            <!--[if $medalLevel.Name == ""]-->
            $medal.Name
            <!--[else]-->
            $medalLevel.Name
            <!--[/if]-->
            </option>
            <!--[/loop]-->
            </select>
            </p>
            <p>
            <!--[MedalValidityTime mission="$mission" medalID="$Medal.ID" isEdit="$isEdit"]-->
            (有效时间:
            <input type="text" class="text" style="width:3em;" name="medal.time.$Medal.ID" value="$time" /> 
            <select name="medal.timetype.$Medal.ID">
            <option value="3" $_form.selected("medal.timetype."+$Medal.ID,"3",$timeUnit)>天</option>
            <option value="2" $_form.selected("medal.timetype."+$Medal.ID,"2",$timeUnit)>小时</option>
            <option value="1" $_form.selected("medal.timetype."+$Medal.ID,"1",$timeUnit)>分钟</option>
            <option value="0" $_form.selected("medal.timetype."+$Medal.ID,"0",$timeUnit)>秒</option>
            </select>)
            <!--[/MedalValidityTime]-->
            </p>
            <!--[/loop]-->
            </div>
            </th>
            <td>
            <p>用户完成任务后会被点亮选定的图标</p>
            <p>如果图标不过期，有效时间请留空或为0</p>
            </td>
        </tr>
        <!--[error name="inviteSerialCount"]-->
            <tr id="jl4-1e">
                <td colspan="2" class="Message">
                    <div class="Tip Tip-error">$Message</div>
                    <div class="TipArrow">&nbsp;</div>
                </td>
            </tr>
        <!--[/error]-->
        <tr id="jl4-1">
            <th>
            <h4>邀请码个数</h4>
            <input type="text" class="text" style="width:5em;" name="inviteSerialCount" value="$_form.text("InviteSerialCount",$out($mission.Prize.InviteSerialCount))" /> 个
            </th>
            <td>如果邀请功能已经关闭，邀请码仍然会正常发放，在下次开启邀请功能时可以使用</td>
        </tr>
        <tr id="jl5-1">
          <th>
          <h4>道具奖励个数</h4>
          <table>
          <!--[loop $prop in $PropList]-->
          <tr>
            <td>$prop.name</td>
            <td><input type="text" class="text number" name="prop_count_$prop.propid" value="$_form.text("prop_count_$prop.propid",$out($mission.Prize.Props[$prop.propid]))" /></td>
          </tr>
          <!--[/loop]-->
          </table>
          </th>
          <td></td>
        </tr>
        <tr>
		    <th>
		    <input type="submit" value="保存设置" name="savemission" class="button" />
		    </th>
		    <td>&nbsp;</td>
	    </tr>
    </table>
    </div>
    <!--[/if]-->
</div>
</form>
<!--[if $_get.type != "group"]-->
<script type="text/javascript">
displayPrize();
function displayPrize()
{
    var obj1 = document.getElementById("Prize1");
    var obj2 = document.getElementById("Prize2");
    var obj3 = document.getElementById("Prize3");
    var obj4 = document.getElementById("Prize4");
    var obj5 = document.getElementById("Prize5");
    var p1_1 = document.getElementById("jl1-1");
    var p2_1 = document.getElementById("jl2-1");
    var p3_1 = document.getElementById("jl3-1");
    var p4_1 = document.getElementById("jl4-1");
    var p5_1 = document.getElementById("jl5-1");
    
    var p1_1e = document.getElementById("jl1-1e");
    var p2_1e = document.getElementById("jl2-1e");
    var p3_1e = document.getElementById("jl3-1e");
    var p4_1e = document.getElementById("jl4-1e");
    
    if(obj1.checked)
    {
        p1_1.style.display="";
        if(p1_1e!=null)
            p1_1e.style.display="";
    }
    else
    {
        p1_1.style.display="none";
        if(p1_1e!=null)
            p1_1e.style.display="none";
    }
    if(obj2.checked)
    {
        p2_1.style.display="";
        if(p2_1e!=null)
            p2_1e.style.display="";
    }
    else
    {
        p2_1.style.display="none";
        if(p2_1e!=null)
            p2_1e.style.display="none";
    }
    if(obj3.checked)
    {
        p3_1.style.display="";
        if(p3_1e!=null)
            p3_1e.style.display="";
    }
    else
    {
        p3_1.style.display="none";
        if(p3_1e!=null)
            p3_1e.style.display="none";
    }
    if(obj4.checked)
    {
        p4_1.style.display="";
        if(p4_1e!=null)
            p4_1e.style.display="";
    }
    else
    {
        p4_1.style.display="none";
        if(p4_1e!=null)
            p4_1e.style.display="none";
    }
    if(obj5.checked)
    {
        p5_1.style.display="";
    }
    else
    {
        p5_1.style.display="none";
    }
}
</script>
<!--[/if]-->
<!--[include src="../_foot_.aspx"/]-->
</body>
<!--[/Mission]-->
</html>