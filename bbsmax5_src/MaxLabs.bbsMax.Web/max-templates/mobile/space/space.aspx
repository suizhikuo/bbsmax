<!DOCTYPE html>
<html>
<head>
<meta charset="utf-8">
<title>$PageTitle</title>
<!--#include file="../_inc/_htmlmeta.aspx"-->
</head>
<body>
<div class="container">
    <!--#include file="../_inc/_header.aspx"-->

    <section class="main memberspace">
        <div class="membercaption">
            <p class="user">
                <span class="avatar">
                    <img class="photo" src="$SpaceOwner.AvatarPath" alt="" width="48" height="48">
                </span>
                <span class="fn nickname">{=$SpaceOwner.Username}</span>
                <!--[if $SpaceOwner.IsOnline]-->
                <span class="online">在线</span>
                <!--[/if]-->
            </p>
            <!--[if $SpaceOwner.Doing != ""]-->
            <p class="status">$SpaceOwner.Doing</p>
            <!--[/if]-->
        </div>

    <!--[if $SpaceCanAccess]-->
        <!--[if $SpaceDisplayAdminNote]-->
        <div class="spacewarning">
            <!--[if $IsSpaceOwnerFullSiteBanned]-->
            您当前访问的用户已被全站屏蔽，因为您是管理员所以可以查看此空间。
            <!--[else]-->
            您当前访问的用户空间只对空间主人所指定的用户群体开放，因为您是管理员所以可以查看此空间。
            <!--[/if]-->
        </div>
        <!--[/if]-->

        <!--[if $UserInfoCanDisplay]-->
        <h3 class="memberinfo-title">基本资料</h3>
        <ul class="memberinfo">
            <!--[if $IsShowRealName]-->
            <li><dfn>真实姓名</dfn> <var>$SpaceOwner.Realname</var></li>
            <!--[/if]-->
            <li><dfn>性别</dfn> <var>$SpaceOwner.GenderName</var></li>
            <!--[if $SpaceOwner.Birthday.Year > 1900]-->
            <li><dfn>生日</dfn> <var>$OutputDate($SpaceOwner.Birthday)</var></li>
            <li><dfn>星座</dfn> <var>$SpaceOwner.AtomName</var></li>
            <!--[/if]-->
            <li><dfn>邮箱</dfn> <var><a href="mailto:$SpaceOwner.email">$SpaceOwner.email</a></var></li>
            <!--[if $GetUserExtendProfile($SpaceOwner,"qq") != ""]-->
            <li><dfn>QQ</dfn> <var><a href="tencent://message/?uin={=$GetUserExtendProfile($SpaceOwner,"qq")}&amp;Site={=$BbsName}&amp;Menu=yes">$GetUserExtendProfile($SpaceOwner,"qq")</a></var>
            <!--[/if]-->
            <!--[if $GetUserExtendProfile($SpaceOwner,"msn") != ""]-->
            <li><dfn>MSN</dfn> <var><a href="msnim:chat?contact=$GetUserExtendProfile($SpaceOwner,"msn")">$GetUserExtendProfile($SpaceOwner,"msn")</a></var></li>
            <!--[/if]-->
            <!--[UserExtendedFieldList userID="$SpaceOwner.userID"]-->
            <!--[if $IsShow($privacyType,$SpaceOwner)]-->
            <li><dfn>$Name</dfn> <var>$fieldType.GetHtmlForDisplay($userValue)</var></li>
            <!--[/if]-->
            <!--[/UserExtendedFieldList]-->
            <!--[if $SpaceOwner.mobilephone > 0]-->
            <li><dfn>手机</dfn> <var><a href="tel:$SpaceOwner.mobilephone">$SpaceOwner.mobilephone</a></var></li>
            <!--[/if]-->
        </ul>
        <h3 class="memberinfo-title">论坛资料</h3>
        <ul class="memberinfo">
            <li><dfn>用户ID</dfn> <var>$SpaceOwner.UserID</var></li>
            <li><dfn>注册时间</dfn> <var>$OutputFriendlyDate($SpaceOwner.CreateDate)</var></li>
            <li><dfn>最后登录</dfn> <var>$OutputFriendlyDate($SpaceOwner.LastVisitDate)</var></li>
            <li><dfn>用户头衔</dfn> <var>$SpaceOwner.RoleTitle</var></li>
            <li><dfn>用户组</dfn> <var>$GetRoleNames($SpaceOwner.Roles,",")</var></li>
            <li><dfn>在线等级</dfn> <var>$SpaceOwner.OnlineLevel</var></li>
            <li><dfn>空间访问量</dfn> <var>$SpaceOwner.TotalViews</var></li>
            <li><dfn>论坛主题数</dfn> <var>$SpaceOwner.TotalTopics</var></li>
            <li><dfn>论坛发帖数</dfn> <var>$SpaceOwner.TotalPosts</var></li>
            <li><dfn>$GeneralPointName</dfn> <var>$SpaceOwner.Points</var></li>
            <!--[loop $point in $PointList]-->
            <li><dfn>$point.name</dfn> <var>$point.value</var></li>
            <!--[/loop]-->
        </ul>
        <!--[/if]-->
    <!--[else]-->
        <!--[if $IsSpaceOwnerFullSiteBanned]-->
        <div class="spacewarning">此空间的主人已被管理员全站屏蔽, 所以您暂时无法查看.</div>
        <!--[else]-->
        <div class="spacewarning">抱歉, 您无法查看此空间的内容. 此空间的主人只对特定的好友开放访问, 您暂时无法查看.</div>
        <!--[/if]-->
    <!--[/if]-->
    <!--[if !$IsLogin]-->
        <div class="spacewarning">您需要登录才能查看用户信息.</div>
    <!--[/if]-->
    </section>

    <!--#include file="../_inc/_footer.aspx"-->
</div>
</body>
</html>