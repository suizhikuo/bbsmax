<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>邀请功能设置</title>
<!--[include src="../_htmlhead_.aspx" /]-->
<script type="text/javascript" src="$root/max-assets/nicedit/nicEdit.js"></script>
<script type="text/javascript">
addPageEndEvent( function(){initMiniEditor(editorToolBar.setting);} );
//bkLib.onDomLoaded(function() { nicEditors.allTextAreas() });

initDisplay('Interval',[
 {value : 'Disable', display : false , id : 'spInviteSerialBuyCount'}
,{value : 'ByYear',  display : true ,  id : 'spInviteSerialBuyCount'}
,{value : 'ByMonth', display : true ,  id : 'spInviteSerialBuyCount'}
,{value : 'ByDay',   display : true ,  id : 'spInviteSerialBuyCount'}
,{value : 'ByHour',  display : true ,  id : 'spInviteSerialBuyCount'}
]);
</script>
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
	<h3>设置邀请或推广</h3>
	<form action="$_form.action" method="post">
	<div class="FormTable">
	<table style="margin-bottom:1px;">
        <!--[error name="InviteMode"]-->
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
		<tr>
			<th>
		    <h4>模式</h4>
			<p><input type="radio" name="InviteMode" id="InviteMode1" value="Close" $_form.checked('InviteMode','Close',$InvitationSettings.InviteMode) /> <label for="InviteMode1">关闭邀请注册</label></p>
			<p><input type="radio" name="InviteMode" id="InviteMode2" value="InviteSerialRequire" $_form.checked('InviteMode','InviteSerialRequire',$InvitationSettings.InviteMode) /> <label for="InviteMode2">必须通过邀请码注册</label></p>
			<p><input type="radio" name="InviteMode" id="InviteMode3" value="InviteSerialOptional" $_form.checked('InviteMode','InviteSerialOptional',$InvitationSettings.InviteMode) /> <label for="InviteMode3">可以通过邀请码注册，但不是必需的</label></p>
			<p id="showInviteInput">&nbsp;&nbsp;&nbsp;&nbsp;<input type="checkbox" name="ShowRegisterInviteInput" id="ShowRegisterInviteInput" value="false"   $_form.checked('ShowRegisterInviteInput','false',$InvitationSettings.ShowRegisterInviteInput==false) /><lable for="ShowRegisterInviteInput">不显示注册页面邀请码输入框</lable></p>
			<p><input type="radio" name="InviteMode" id="InviteMode4" value="InviteLinkRequire" $_form.checked('InviteMode','InviteLinkRequire',$InvitationSettings.InviteMode) /> <label for="InviteMode4">必须通过推广链接注册</label></p>
			<p><input type="radio" name="InviteMode" id="InviteMode5" value="InviteLinkOptional" $_form.checked('InviteMode','InviteLinkOptional',$InvitationSettings.InviteMode) /> <label for="InviteMode5">可以通过推广链接注册，但不是必需的</label></p>
			</th>
			<td>&nbsp;</td>
		</tr>
    </table>

	<div id="AddToRole">
	<table style="margin-bottom:1px;">
        <!--[error name="AddToUserRoleWhenHasInvite"]-->
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
	    <tr>
	        <th>
            <h4>通过邀请码或推广链接注册自动加入用户组</h4>
            <select name="AddToUserRoleWhenHasInvite">
            <option value="{=Guid.Empty}" $_form.selected('AddToUserRoleWhenHasInvite', {=Guid.Empty}, $InvitationSettings.AddToUserRoleWhenHasInvite)>不加入任何用户组</option>
            <!--[loop $role in $rolelistForAdd]-->
            <option value="$role.id" $_form.selected('AddToUserRoleWhenHasInvite', $role.id, $InvitationSettings.AddToUserRoleWhenHasInvite)>$role.name</option>
            <!--[/loop]-->
            </select>
	        </th>
	        <td>&nbsp;</td>
	    </tr>
	    <tbody id="AddToRole2">
        <!--[error name="AddToUserRoleWhenNoInvite"]-->
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
	    <tr>
	        <th>
            <h4>未通过邀请码或推广链接注册自动加入用户组</h4>
            <select name="AddToUserRoleWhenNoInvite">
            <option value="{=Guid.Empty}" $_form.selected('AddToUserRoleWhenNoInvite', {=Guid.Empty}, $InvitationSettings.AddToUserRoleWhenNoInvite)>不加入任何用户组</option>
            <!--[loop $role in $rolelistForAdd]-->
            <option value="$role.id" $_form.selected('AddToUserRoleWhenNoInvite', $role.id, $InvitationSettings.AddToUserRoleWhenNoInvite)>$role.name</option>
            <!--[/loop]-->
            </select>
	        </th>
	        <td>&nbsp;</td>
	    </tr>
	    </tbody>
	</table>
	</div>

	<div id="InviteLinkOptions">
	<table style="margin-bottom:1px;">
        <!--[error name="InviteEmailTitle"]-->
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
	    <tr>
	        <th>
            <h4>发送推广链接邮件标题</h4>
            <input type="text" class="text" name="InviteEmailTitle" value="$_form.text('InviteEmailTitle',$InvitationSettings.InviteEmailTitle)" />
	        </th>
	        <td>&nbsp;</td>
	    </tr>
        <!--[error name="InviteEmailContent"]-->
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
	    <tr>
	        <th>
            <h4>发送推广链接邮件内容</h4>
            <textarea cols="50" rows="8" name="InviteEmailContent" style="width: 550px; height: 200px;">$_form.text('InviteEmailContent',$InvitationSettings.InviteEmailContent)</textarea>
	        </th>
	        <td>&nbsp;</td>
	    </tr>
	</table>
	</div>
	
	<div id="InviteSerialOptions">
	<table style="margin-bottom:1px;">
        <!--[error name="InviteEffectiveHours"]-->
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
	    <tr>
            <th>
            <h4>邀请码有效时间(小时)</h4>
            <input type="text" class="text number" name="InviteEffectiveHours" value="$_form.text('InviteEffectiveHours',$InvitationSettings.InviteEffectiveHours)" onkeyup="value=value.replace(/[^\d]/g,'')" />
            </th>
            <td>如果为0则不限制邀请码有效时间。</td>
        </tr>
        <!--[error name="Interval"]-->
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
           <tr>
            <th>
            <h4>购买邀请码数量限制</h4>
            <select id="Interval" name="Interval">
                <option value="ByHour" $_form.selected('Interval','ByHour',$InvitationSettings.Interval==InviteBuyInterval.ByHour)>1小时</option>
                <option value="ByDay" $_form.selected('Interval','ByDay',$InvitationSettings.Interval==InviteBuyInterval.ByDay)>1天</option>
                <option value="ByMonth" $_form.selected('Interval','ByMonth',$InvitationSettings.Interval==InviteBuyInterval.ByMonth)>1个月</option>
                <option value="ByYear" $_form.selected('Interval','ByYear',$InvitationSettings.Interval==InviteBuyInterval.ByYear)>1年</option>
                <option value="Disable" $_form.selected('Interval','Disable',$InvitationSettings.Interval==InviteBuyInterval.Disable)>无限制</option>
            </select><span id="spInviteSerialBuyCount"><input type="text" class="text number" id="InviteSerialBuyCount" name="InviteSerialBuyCount" value="$_form.text('InviteSerialBuyCount',$InvitationSettings.InviteSerialBuyCount)" onkeyup="value=value.replace(/[^\d]/g,'')" />个</span>
            </th>
            <td>&nbsp;</td>
        </tr>
   
        <!--[error name="IntiveSerialPoint"]-->
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
        <tr>
            <th>
            <h4>购买邀请码所需积分</h4>
            <input type="text" class="text number" name="IntiveSerialPoint" value="$_form.text('IntiveSerialPoint',$InvitationSettings.IntiveSerialPoint)" onkeyup="value=value.replace(/[^\d]/g,'')" />
            <select name="PointFieldIndex">
            <!--[loop $p in $PointList]-->
            <option $_form.selected("PointFieldIndex","$p.type", $p.type==$InvitationSettings.PointFieldIndex) value="$p.type">$p.name</option>
            <!--[/loop]-->
            </select>
            </th>
            <td>请输入0以上的数字</td>
        </tr>
        <!--[error name="InviteSerialEmailTitle"]-->
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
	    <tr>
	        <th>
            <h4>发送邀请码邮件标题</h4>
            <input type="text" class="text" name="InviteSerialEmailTitle" value="$_form.text('InviteSerialEmailTitle',$InvitationSettings.InviteSerialEmailTitle)" />
	        </th>
	        <td>&nbsp;</td>
	    </tr>
        <!--[error name="InviteSerialEmailContent"]-->
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
	    <tr>
	        <th>
            <h4>发送邀请码邮件内容</h4>
            <textarea cols="50" rows="8" name="InviteSerialEmailContent" style="width: 550px; height: 200px;">$_form.text('InviteSerialEmailContent',$InvitationSettings.InviteSerialEmailContent)</textarea>
	        </th>
	        <td>&nbsp;</td>
	    </tr>
	</table>
	</div>
	
	<table>
		<tr>
			<th>
			<input type="submit" value="保存设置" class="button" name="savesetting" />
			</th>
			<td>&nbsp;</td>
		</tr>
	</table>
	</div>
	</form>
</div>
<script type="text/javascript">
initDisplay('InviteMode',[
     { value: "Close", display: false, id: ['AddToRole', 'InviteSerialOptions', 'InviteLinkOptions', 'showInviteInput'] }
    
    ,{value : "InviteSerialRequire", display : true, id : ['InviteSerialOptions', 'AddToRole']}
    , { value: "InviteSerialRequire", display: false, id: ['InviteLinkOptions', 'AddToRole2', 'showInviteInput'] }

    , { value: "InviteSerialOptional", display: true, id: ['InviteSerialOptions', 'AddToRole', 'AddToRole2', 'showInviteInput'] }
    ,{value : "InviteSerialOptional", display : false,id : 'InviteLinkOptions'}
    
    ,{value : "InviteLinkRequire", display : true, id : ['InviteLinkOptions', 'AddToRole']}
    , { value: "InviteLinkRequire", display: false, id: ['InviteSerialOptions', 'AddToRole2', 'showInviteInput'] }

    , { value: "InviteLinkOptional", display: true, id: ['InviteLinkOptions', 'AddToRole', 'AddToRole2'] }
    , { value: "InviteLinkOptional", display: false, id: ['InviteSerialOptions', 'showInviteInput'] }

]);
</script>
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>
