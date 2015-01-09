<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>隐私设置</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
	<form action="$_form.action" method="post">
	<div class="DataTable">
    <h4>用户默认隐私</h4>
    <table>
        <thead>
        <tr>
            <th>&nbsp;</th>
            <th class="center">全站用户可见</th>
            <th class="center">仅好友可见</th>
            <th class="center">仅自己可见</th>
        </tr>
        </thead>
        <tbody>
        <tr>
            <th>个人空间</th>
            <td class="center">
                <input type="radio" name="SpacePrivacy" value="All" $_if($PrivacySettings.SpacePrivacy==SpacePrivacyType.All,'checked="checked"') />
            </td>
            <td class="center">
                <input type="radio" name="SpacePrivacy" value="Friend" $_if($PrivacySettings.SpacePrivacy==SpacePrivacyType.Friend,'checked="checked"') />
            </td>
            <td class="center">
                <input type="radio" name="SpacePrivacy" value="Self" $_if($PrivacySettings.SpacePrivacy==SpacePrivacyType.Self,'checked="checked"') />
            </td>
        </tr>
        <tr>
            <th>个人基本资料</th>
            <td class="center">
                <input type="radio" name="InformationPrivacy" value="All" $_if($PrivacySettings.InformationPrivacy==SpacePrivacyType.All,'checked="checked"') />
            </td>
            <td class="center">
                <input type="radio" name="InformationPrivacy" value="Friend" $_if($PrivacySettings.InformationPrivacy==SpacePrivacyType.Friend,'checked="checked"') />
            </td>
            <td class="center">
                <input type="radio" name="InformationPrivacy" value="Self" $_if($PrivacySettings.InformationPrivacy==SpacePrivacyType.Self,'checked="checked"') />
            </td>
        </tr>
        <tr>
            <th>好友列表</th>
            <td class="center">
                <input type="radio" name="FriendListPrivacy" value="All" $_if($PrivacySettings.FriendListPrivacy==SpacePrivacyType.All,'checked="checked"') />
            </td>
            <td class="center">
                <input type="radio" name="FriendListPrivacy" value="Friend" $_if($PrivacySettings.FriendListPrivacy==SpacePrivacyType.Friend,'checked="checked"') />
            </td>
            <td class="center">
                <input type="radio" name="FriendListPrivacy" value="Self" $_if($PrivacySettings.FriendListPrivacy==SpacePrivacyType.Self,'checked="checked"') />
            </td>
        </tr>
        <tr>
            <th>留言板</th>
            <td class="center">
                <input type="radio" name="BoardPrivacy" value="All" $_if($PrivacySettings.BoardPrivacy==SpacePrivacyType.All,'checked="checked"') />
            </td>
            <td class="center">
                <input type="radio" name="BoardPrivacy" value="Friend" $_if($PrivacySettings.BoardPrivacy==SpacePrivacyType.Friend,'checked="checked"') />
            </td>
            <td class="center">
                <input type="radio" name="BoardPrivacy" value="Self" $_if($PrivacySettings.BoardPrivacy==SpacePrivacyType.Self,'checked="checked"') />
            </td>
        </tr>
        <tr>
            <th>个人动态</th>
            <td class="center">
                <input type="radio" name="FeedPrivacy" value="All" $_if($PrivacySettings.FeedPrivacy==SpacePrivacyType.All,'checked="checked"') />
            </td>
            <td class="center">
                <input type="radio" name="FeedPrivacy" value="Friend" $_if($PrivacySettings.FeedPrivacy==SpacePrivacyType.Friend,'checked="checked"') />
            </td>
            <td class="center">
                <input type="radio" name="FeedPrivacy" value="Self" $_if($PrivacySettings.FeedPrivacy==SpacePrivacyType.Self,'checked="checked"') />
            </td>
        </tr>
        <tr>
            <th>记录</th>
            <td class="center">
                <input type="radio" name="DoingPrivacy" value="All" $_if($PrivacySettings.DoingPrivacy==SpacePrivacyType.All,'checked="checked"') />
            </td>
            <td class="center">
                <input type="radio" name="DoingPrivacy" value="Friend" $_if($PrivacySettings.DoingPrivacy==SpacePrivacyType.Friend,'checked="checked"') />
            </td>
            <td class="center">
                <input type="radio" name="DoingPrivacy" value="Self" $_if($PrivacySettings.DoingPrivacy==SpacePrivacyType.Self,'checked="checked"') />
            </td>
        </tr>
        <tr>
            <th>日志</th>
            <td class="center">
                <input type="radio" name="BlogPrivacy" value="All" $_if($PrivacySettings.BlogPrivacy==SpacePrivacyType.All,'checked="checked"') />
            </td>
            <td class="center">
                <input type="radio" name="BlogPrivacy" value="Friend" $_if($PrivacySettings.BlogPrivacy==SpacePrivacyType.Friend,'checked="checked"') />
            </td>
            <td class="center">
                <input type="radio" name="BlogPrivacy" value="Self" $_if($PrivacySettings.BlogPrivacy==SpacePrivacyType.Self,'checked="checked"') />
            </td>
        </tr>
        <tr>
            <th>相册</th>
            <td class="center">
                <input type="radio" name="AlbumPrivacy" value="All" $_if($PrivacySettings.AlbumPrivacy==SpacePrivacyType.All,'checked="checked"') />
            </td>
            <td class="center">
                <input type="radio" name="AlbumPrivacy" value="Friend" $_if($PrivacySettings.AlbumPrivacy==SpacePrivacyType.Friend,'checked="checked"') />
            </td>
            <td class="center">
                <input type="radio" name="AlbumPrivacy" value="Self" $_if($PrivacySettings.AlbumPrivacy==SpacePrivacyType.Self,'checked="checked"') />
            </td>
        </tr>
        <tr>
            <th>分享</th>
            <td class="center">
                <input type="radio" name="SharePrivacy" value="All" $_if($PrivacySettings.SharePrivacy==SpacePrivacyType.All,'checked="checked"') />
            </td>
            <td class="center">
                <input type="radio" name="SharePrivacy" value="Friend" $_if($PrivacySettings.SharePrivacy==SpacePrivacyType.Friend,'checked="checked"') />
            </td>
            <td class="center">
                <input type="radio" name="SharePrivacy" value="Self" $_if($PrivacySettings.SharePrivacy==SpacePrivacyType.Self,'checked="checked"') />
            </td>
        </tr>
        <tr class="nohover">
	        <td>&nbsp;</td>
	        <td colspan="3" class="center"><input type="submit" value="保存设置" class="button" name="savesetting"/></td>
        </tr>
        </tbody>
    </table>
    </div>

	<div class="DataTable">
    <h4>动态发布隐私</h4>
    <div class="Help">
	    设置系统默认将哪些动作发布到个人动态里面。如果不是强制发送或者强制不发送则用户可以修改这些默认设置。
	</div>
	<table>
	    <thead>
	    <tr>
	        <th>&nbsp;</th>
	        <th class="center">默认发送</th>
	        <th class="center">默认不发送</th>
	        <th class="center">强制发送</th>
	        <th class="center">强制不发送</th>
	    </tr>
	    </thead>
	    <tbody>
	    <!--[loop $App in $AppList]-->
	    <!--[loop $AppAction in $App.AppActions with $i]-->
	    <!--[if $IsSiteFeed($App.AppID,$AppAction.ActionType) == false]-->
	    <tr>
            <th>$AppAction.ActionName</th>
            <td class="center">
                <input type="radio" name="app_{=$App.AppID}_$AppAction.ActionType" id="app_{=$App.AppID}_{=$i}_0" value="send" $_Form.checked('app_{=$App.AppID}_$AppAction.ActionType','send','$GetSendType($App.AppID,$AppAction.ActionType,"send")') />
            </td>
            <td class="center">
                <input type="radio" name="app_{=$App.AppID}_$AppAction.ActionType" id="app_{=$App.AppID}_{=$i}_1" value="notsend" $_Form.checked('app_{=$App.AppID}_$AppAction.ActionType','notsend','$GetSendType($App.AppID,$AppAction.ActionType,"send")') />
            </td>
            <td class="center">
                <input type="radio" name="app_{=$App.AppID}_$AppAction.ActionType" id="app_{=$App.AppID}_{=$i}_2" value="forcesend" $_Form.checked('app_{=$App.AppID}_$AppAction.ActionType','forcesend','$GetSendType($App.AppID,$AppAction.ActionType,"send")') />
            </td>
            <td class="center">
                <input type="radio" name="app_{=$App.AppID}_$AppAction.ActionType" id="app_{=$App.AppID}_{=$i}_3" value="forcenotsend" $_Form.checked('app_{=$App.AppID}_$AppAction.ActionType','forcenotsend','$GetSendType($App.AppID,$AppAction.ActionType,"send")') />
            </td>
        </tr>
        <!--[/if]-->
        <!--[/loop]-->
        <!--[/loop]-->
        <tr class="nohover">
	        <td>&nbsp;</td>
	        <td colspan="4" class="center"><input type="submit" value="保存设置" class="button" name="savesetting"/></td>
        </tr>
        </tbody>
    </table>
    </div>
	</form>
</div>
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>
