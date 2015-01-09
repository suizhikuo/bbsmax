<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>$PageTitle</title>
<!--#include file="_forumhtmlhead_.aspx"-->
</head>
<body>
<div class="container section-forumsigin">
<!--#include file="../_inc/_head.aspx"-->
    <div id="main" class="main">
        <div class="clearfix main-inner">
            <div class="content">
                <div class="clearfix content-inner">
                    <div class="content-main">
                        <!--#include file="../_inc/_round_top.aspx"-->
                        <div class="content-main-inner">
                            
                            <div class="forumstatus">
                                 版主: $GetModeratorLinks($forum,@"<a class=""fn"" href=""{0}"">{1}</a>{2}",", ")
                                 <a class="rss" href="$root/rss.aspx?ticket=$ticket&forumid=$forumid">订阅</a>
                            </div>
                            
                            <form id="signinforum" method="post" enctype="multipart/form-data" action="$_form.action">
                            <div class="panel forumsigin">
                                <div class="panel-head"><div class="panel-head-inner"><div class="clearfix panel-head-inner">
                                    <h3 class="panel-title"><span>登录"$forum.ForumName"版块</span></h3>
                                </div></div></div>
                                <div class="panel-body">
                                    <!--[unnamederror]-->
                                    <div class="errormsg">$Message</div>
                                    <!--[/unnamederror]-->
                                    <div class="formgroup forumsiginform">
                                        <div class="formrow">
                                            <h3 class="label"><label for="password">请输入密码</label></h3>
                                            <div class="form-enter">
                                                <input type="password" name="password" id="password" class="text" value="$_form.text("password")" />
                                            </div>
                                        </div>
                                        <!--[ValidateCode actionType="SignInForum"]-->
                                        <div class="formrow">
                                            <h3 class="label"><label for="$inputName">验证码</label></h3>
                                            <div class="form-enter">
                                                <input name="$inputName" id="$inputName" type="text" class="text validcode" onfocus="showVCode(this,'$imageurl');" autocomplete="off" $_if($disableIme,'style="ime-mode:disabled;"') />
                                            </div>
                                            <div class="form-note">$tip</div>
                                        </div>
                                        <!--[/ValidateCode]-->
                                        <div class="formrow formrow-action">
                                            <span class="minbtn-wrap"><span class="btn"><input class="button" type="submit" name="SignInButton" value="进入" /></span></span>
                                            <a href="javascript:history.back()">[ 返回上一页 ]</a>
                                        </div>
                                    </div>
                                </div>
                                <div class="panel-foot"><div><div>-</div></div></div>
                            </div>
                            </form>
                            
                        </div>
                        <!--#include file="../_inc/_round_bottom.aspx"-->
                    </div>
                </div>
            </div>
        </div>
    </div>
<!--#include file="../_inc/_foot.aspx"-->
</div>
</body>
</html>
