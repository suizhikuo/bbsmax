<!DOCTYPE html>
<html>
<head>
<meta charset="utf-8">
<title>$PageTitle</title>
<!--#include file="_htmlmeta.aspx"-->
</head>
<body>
<div class="container">
    <!--#include file="_header.aspx"-->

    <section class="main infopage">
        <div class="infomessage message-$mode">
            <h3 class="title">$message</h3>
            <!--[loop $jump in $JumpLinkList]-->
            <p class="intro">
                <!--[if $jump.link == ""]-->
                $jump.Text
                <!--[else]-->
                <a href="$jump.Link">$jump.Text</a>
                <!--[/if]-->
            </p>
            <!--[/loop]-->
            <p class="action">
                <a href="javascript:history.go(-1)">&laquo; 返回上页</a>
                <a href="$url(default)">前往首页 &raquo;</a>
            </p>
        </div>
    </section>
    
    <!--#include file="../_inc/_footer.aspx"-->
</div>
</body>
</html>
