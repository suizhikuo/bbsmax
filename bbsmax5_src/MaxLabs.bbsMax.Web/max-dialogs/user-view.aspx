<!--[DialogMaster title="查看用户资料" width="700"]-->
<!--[place id="body"]-->
<!--[UserView userid="$_get.id"]-->
<!--#include file="_error_.ascx" -->
<!--[success]-->
<div class="dialogmsg dialogmsg-success">修改用户信息成功</div>
<!--[/success]-->

<!--#include file="_tab_userinfo_.aspx" tab="view" -->

<form id="formInfo" action="$_form.action" method="post" >
<div class="clearfix dialogbody">
    <div class="scroller" style="height:300px;">
    <div class="dialogform dialogform-horizontal">
        <div class="formrow">
            <h3 class="label">用户名</h3>
            <div class="form-enter">$user.username</div>
        </div>
        <div class="formrow">
            <h3 class="label">总积分</h3>
            <div class="form-enter">$user.Points</div>
        </div>
        <div class="formrow">
            <h3 class="label">登录次数</h3>
            <div class="form-enter">$user.LoginCount</div>
        </div>
        <div class="formrow">
            <h3 class="label">总发帖数</h3>
            <div class="form-enter">$user.totalposts</div>
        </div>
        <div class="formrow">
            <h3 class="label">总主题数</h3>
            <div class="form-enter">$user.totaltopics</div>
        </div>
        <div class="formrow">
            <h3 class="label">总回复数</h3>
            <div class="form-enter">$user.totalreplies</div>
        </div>
        <!--[if $EnableInvite]-->
        <div class="formrow">
            <h3 class="label">邀请注册人数</h3>
            <div class="form-enter">$user.TotalInvite</div>
        </div>
        <!--[/if]-->
        <div class="formrow">
            <h3 class="label">好友数量</h3>
            <div class="form-enter">$user.TotalFriends</div>
        </div>
        <div class="formrow">
            <h3 class="label">在线时间</h3>
            <div class="form-enter">
                本月:
                <input class="text number" name="MonthOnlineTime" type="text" value="$user.MonthOnlineTime" /> 分钟
                总在线:
                <input class="text number" type="text" name="TotalOnlineTime" value="$user.TotalOnlineTime" />分钟
            </div>
        </div>
        <div class="formrow">
            <h3 class="label">注册时间</h3>
            <div class="form-enter">
                <span class="datepicker">
                    <input class="text" type="text" id="createdate" name="createdate" value="$outputDatetime($user.createdate)" />
                    <a href="javascript:void(0);" id="datepicker_1">日期</a>
                </span>
            </div>
        </div>
        <div class="formrow">
            <h3 class="label">注册IP</h3>
            <div class="form-enter">$outputip($user.CreateIP) ($OutputIPAddress($user.CreateIP))</div>
        </div>
        <div class="formrow">
            <h3 class="label">最后浏览IP</h3>
            <div class="form-enter">$outputip($user.lastvisitip) ($OutputIPAddress($user.lastvisitip))</div>
        </div>
    </div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" accesskey="s" name="save"><span>保存(<u>S</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<script type="text/javascript">
    initDatePicker('createdate','datepicker_1');
</script>
<!--[/userview]-->
<!--[/place]-->
<!--[/dialogmaster]-->