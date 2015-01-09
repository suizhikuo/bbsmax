<form action="$_form.action" method="post">
<div class="clearfix rankingheader">
    <div class="rankingsearch">
        <a href="javascript:void(swithForm())">在注册会员中搜索</a>
    </div>
</div>
<div class="formgroup rankingsearchform" id="userSearchForm" $_if(!$ShowSearchForm,'style="display:none;"')>
    <div class="formrow">
        <h3 class="label"><label for="username">用户名</label></h3>
        <div class="form-enter">
            <input class="text" type="text" id="username" name="username" value="$_form.text('username',$filter.username)" />
        </div>
    </div>
    <!--[if $IsEnableRealname]-->
    <div class="formrow">
        <h3 class="label"><label for="realname">真实姓名</label></h3>
        <div class="form-enter">
            <input class="text" name="realname" id="realname" type="text" value="$_form.text('realname',$filter.realname)" />
        </div>
    </div>
    <!--[/if]-->
    <div class="formrow">
        <h3 class="label"><label>性别</label></h3>
        <div class="form-enter">
            <input id="gender0" type="radio" name="gender" value="" $_form.checked('gender','',$filter.gender==null) />
            <label for="gender0">不限</label>
            <input id="gender2" type="radio" name="gender" value="1" $_form.checked('gender','1',$filter.gender==MaxLabs.bbsMax.Enums.Gender.Male) />
            <label for="gender2">男</label>
            <input id="gender3" type="radio" name="gender" value="2" $_form.checked('gender','2',$filter.gender==MaxLabs.bbsMax.Enums.Gender.Female) />
            <label for="gender3">女</label>
        </div>
    </div>
    <div class="formrow">
        <h3 class="label"><label>年龄段</label></h3>
        <div class="form-enter">
            <input type="text" class="text number" value="$_if($filter.beginage>0,$filter.beginage.tostring())" name="beginage" /> -
            <input type="text" class="text number" value="$_if($filter.endage>0,$filter.endage.tostring())" name="endage" />
        </div>
    </div>
    <div class="formrow">
        <h3 class="label"><label>生日</label></h3>
        <div class="form-enter">
            <select id="birthyear" name="birthyear">
                <option value="" $_form.selected("birthyear","","$out($filter.birthyear)")>年</option>
                <!--[loop $year in $Years]--> 
                <option value="$year" $_form.selected("birthyear","$year","$out($filter.birthyear)") >$year</option>
                <!--[/loop]-->
            </select>
            <select id="birthmonth" name="birthmonth">
                <option value="" $_form.selected("birthmonth","","$out($filter.birthmonth)")>月</option>
                <!--[loop 1 to 12 with $var1]-->
                <option value="$var1" $_form.selected("birthmonth","$var1","$out($filter.birthmonth)")>$var1</option>
                <!--[/loop]-->
            </select>
            <select id="birthday" name="birthday">
                <option value="" $_form.selected("birthday","","$out($filter.birthday)")>日</option>
                <!--[loop 1 to 31 with $var]-->
                <option value="$var" $_form.selected("birthday","$var","$out($filter.birthday)")>$var</option>
                <!--[/loop]-->
            </select>
        </div>
    </div>
    <!--[ExtendedFieldList]-->
    <!--[if $Searchable]-->
    <div class="formrow">
        <h3 class="label"><label>$Name</label></h3>
        <div class="form-enter">
            <!--[load src="$fieldType.FrontendControlSrc" value="$parent($filter.ExtendedFields[$this($key)])" field="$_this" /]-->
        </div>
    </div> 
    <!--[/if]-->
    <!--[/ExtendedFieldList]-->
    <div class="formrow formrow-action">
        <span class="minbtn-wrap"><span class="btn"><input class="button" name="searchuser" type="submit" value="搜索" /></span></span>
        <input type="hidden" name="order" value="userid" />
    </div>
</div>
</form>
<div class="panel userstablepanel userstable-member">
    <div class="panel-head">
        <h3 class="panel-title"><span>会员</span></h3>
    </div>
    <div class="panel-body">
        <table class="userstable">
            <thead>
                <tr>
                    <td class="userid">ID</td>
                    <td class="avatar mediumavatar">&nbsp;</td>
                    <td class="user">用户</td>
                    <td class="value">性别</td>
                    <td class="bday">生日</td>
                    <td class="date">注册时间</td>
                </tr>
            </thead>
            <tbody>
            <!--[loop $user in $userlist]-->
                <tr $_if($user.UserID == $My.userID,'class="rankingtablerow-my"')>
                    <td class="userid">$user.id</td>
                    <td class="avatar mediumavatar">$user.smallavatarlink</td>
                    <td class="user">
                        <div class="rankinguser-name">
                            $user.popupnamelink
                            <!--[if $user.Realname!=""]-->
                            <span class="realname">($user.realname)</span>
                            <!--[/if]-->
                        </div>
                        <!--[if $islogin && ($user.UserID != $My.userID)]-->
                        <div class="rankinguser-action">
                            <!--[if !$My.ismyfriend($user.UserID)]-->
                            <a class="addnewfriend" href="$dialog/friend-tryadd.aspx?uid=$User.ID" onclick="return openDialog(this.href, function(result){});">加为好友</a>
                            <!--[/if]-->
                            <a class="<!--[if $user.isonline]-->chat-online<!--[else]-->chat-offline<!--[/if]-->" href="$dialog/chat.aspx?to=$User.ID" onclick="return openDialog(this.href, function(result){});"><!--[if $user.isonline]-->会话<!--[else]-->发消息<!--[/if]--></a>
                        </div>
                        <!--[/if]-->
                    </td>
                    <td class="value">$User.GenderName</td>
                    <td class="value"><!--[if $HasBirthday($user)]-->$outputdate($user.Birthday) <!--[if $InBirthday($user)]--><img src="$root/max-assets/icon/gift.gif" alt="" /><!--[/if]--><!--[else]-->-<!--[/if]--></td>
                    <td class="date">$outputdate($user.createDate)</td>
                </tr>
            <!--[/loop]-->
            </tbody>
        </table>
    </div>
</div>
<!--[pager name="list" skin="../_inc/_pager_app.aspx"]-->
<script type="text/javascript">
function swithForm() {
    var _temp = window.sForm==1 ? 0 :1;
    setVisible($('userSearchForm'),_temp);
    window.sForm = _temp==1;
}
</script>