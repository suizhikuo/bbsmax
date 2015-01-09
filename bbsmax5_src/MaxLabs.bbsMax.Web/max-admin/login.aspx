<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>管理员登录</title>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<meta name="robots" content="none" />
<link rel="Stylesheet" type="text/css" href="$Root/max-assets/style/max-adminlogin.css" />
</head>
<body>
<h1>bbsmax v5</h1>
<div class="login">
    <form action="$_form.action" method="post">
    <input type="hidden" name="rawurl" value="$RawUrl" />
        <!--[error name="password"]-->
        <p class="error">$message</p>
        <!--[/error]-->
        <!--[error name="vcode"]-->
        <p class="error">$message</p>
        <!--[/error]-->
        <ul>
            <li class="username">
                <label for="username">用户名</label>
                <input type="text" class="text" name="username" id="username" value="$_if($islogin,$my.username)" $_if($islogin,'readonly="readonly"') />
            </li>
            <li class="password">
                <label for="password">密码</label>
                <input type="password" class="text" name="Password" id="password" />
            
            </li>
            <!--[ValidateCode actionType="$validateActionName"]-->
            <li class="validate">
                <label for="password">验证码</label>
                <input type="text" class="text" $_if($disableIme,'style="ime-mode:disabled;"') name="$inputName" />
                <img src="$imageurl" alt=""  onclick="javascript:this.src=this.src+'&rnd=' + Math.random();" />
                
            </li>
            <!--[/ValidateCode]-->
            <li class="submit">
                <input type="submit" class="button" name="login" value="登录" />
            </li>
        </ul>
        <p class="back">
            <a href="$Url(default)">&laquo; 返回网站首页</a>
        </p>
    </form>
</div>
<div class="foot">
<p>&copy; 2002-2010 MaxLabs. Powered by $Version.</p>
</div>
</body>
</html>