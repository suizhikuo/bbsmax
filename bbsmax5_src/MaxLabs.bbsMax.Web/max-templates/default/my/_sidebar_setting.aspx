<div class="content-sub">
    <div class="content-sub-inner">
        <div class="sidebar">
            
            <div class="panel settinglist">
                <div class="panel-head">
                    <h3 class="panel-title"><span>设置中心</span></h3>
                </div>
                <div class="panel-body">
                    <!--[if ! $EnablePassportClient]-->
                    <ul class="setting-list">
                        <li><a $_if($PageName=='setting','class="current"','') href="$url(my/setting)">修改个人资料</a></li>
                        <li><a $_if($PageName=='avatar','class="current"','') href="$url(my/avatar)">设置头像</a></li>
                        <li><a $_if($PageName=='changeemail','class="current"','') href="$url(my/changeemail)">更改邮箱</a></li>
                        <li><a $_if($PageName=='changepassword','class="current"','') href="$url(my/changepassword)">修改密码</a></li>
                        <!--[if $EnableMobileBind]-->
                        <li><a $_if($PageName=='bindmobile','class="current"','') href="$url(my/bindmobile)">手机绑定</a></li>
                        <!--[/if]-->
                        <!--[if $EnableRealnameCheck]-->
                        <li><a $_if($PageName=='realname','class="current"','') href="$url(my/realname)">实名认证</a></li>
                        <!--[/if]-->
                    </ul>
                    <!--[/if]-->
                    <ul class="setting-list">
                        <li><a $_if($PageName=='privacy','class="current"','') href="$url(my/privacy)">隐私设置</a></li>
                        <li><a $_if($PageName=='feedfilter','class="current"','') href="$url(my/feedfilter)">好友动态过滤</a></li>
                        <li><a $_if($PageName=='spacestyle','class="current"','') href="$url(my/spacestyle)">个人空间风格设置</a></li>
                        <li><a $_if($PageName=='point','class="current"','') href="$url(my/point)">我的积分</a></li>
                        <!--[if $EnablePointExchange]-->
                        <li><a $_if($PageName=='point-exchange','class="current"','') href="$url(my/point-exchange)">积分兑换</a></li>
                        <!--[/if]-->
                        <!--[if $EnablePointTransfer]-->
                        <li><a $_if($PageName=='point-transfer','class="current"','') href="$url(my/point-transfer)">积分转账</a></li>
                        <!--[/if]-->
                        <!--[if $EnablePointRecharge]-->
                        <li><a $_if($PageName=='pay','class="current"','') $_if($PageName=='paylog','class="current"','') href="$url(my/pay)">积分充值</a></li>
                        <!--[/if]-->
                    </ul>
                </div>
            </div>
        </div>
    </div>
</div>
