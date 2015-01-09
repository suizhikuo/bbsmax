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

    <div class="crumbnav">
        <a href="$url(default)">&laquo; 返回首页</a>
    </div>

    <section class="main forumsignin">
        <form id="signinforum" method="post" enctype="multipart/form-data" action="$_form.action">
            <div class="form">
                <!--[unnamederror]-->
                <div class="errormsg">$Message</div>
                <!--[/unnamederror]-->
                <div class="row">
                    <label class="label" for="password">访问"$forum.ForumName"版块需要密码</label>
                    <div class="enter">
                        <input type="password" name="password" id="password" class="text" value="$_form.text("password")" placeholder="请输入密码">
                    </div>
                </div>
                <!--[ValidateCode actionType="SignInForum"]-->
                <div class="row">
                    <label class="label" for="$inputName">验证码</label>
                    <div class="enter">
                        <input name="$inputName" id="$inputName" type="text" class="text validcode" $_if($disableIme,'style="ime-mode:disabled;"') placeholder="$tip">
                    </div>
                    <div class="captchaimg">
                        <img src="$imageurl" alt="" onclick="javascript:this.src=this.src+'&rnd=' + Math.random();">
                        点击图片更换验证码
                    </div>
                </div>
                <!--[/ValidateCode]-->
                <div class="row row-button">
                    <input class="button" type="submit" name="SignInButton" value="进入">
                </div>
            </div>
        </form>
    </section>
    
    <!--#include file="../_inc/_footer.aspx"-->
</div>
</body>
</html>
