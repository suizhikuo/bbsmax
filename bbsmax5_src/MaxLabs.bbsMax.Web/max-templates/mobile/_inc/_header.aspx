<header>
    <div class="left">
        <p>
            <!--[if $IsLogin]-->
            <a href="$url(logout/$My.ID)">退出</a>
            <!--[else]-->
            <a href="$url(login)">登录</a>
            <!--[/if]-->
        </p>
    </div>
    <hgroup>
        <h1><a href="$url(default)"><img src="$skin/images/logo.png" alt="$bbsName"></a></h1>
    </hgroup>
    <div class="right">
        <p>
            <!--[if $IsLogin]-->
            <!--/*<a class="app" href="#"><span>应用</span></a>*/-->
            <a class="myavatar" href="$url(space/$my.UserID)"><img src="$my.AvatarPath" alt="" width="24" height="24"></a>
            <!--[else]-->
            <a href="$url(register)">注册</a>
            <!--[/if]-->
        </p>
    </div>
</header>