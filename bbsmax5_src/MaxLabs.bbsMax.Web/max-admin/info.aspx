<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
<title>操作提示</title>
<!--[include src="_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="_head_.aspx"/]-->
<div class="$mode">
    <h3>$message</h3>
    <p>
        <!--[loop $jump in $JumpLinkList]-->
            <!--[if $jump.link == ""]-->
            $jump.Text
            <!--[else]-->
            <a href="$jump.Link">$jump.Text</a>
            <!--[/if]-->
        <!--[/loop]-->
    </p>
</div>
<!--[include src="_foot_.aspx"/]-->
</body>
</html>
