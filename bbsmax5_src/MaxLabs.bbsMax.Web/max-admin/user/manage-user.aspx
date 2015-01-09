<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>用户管理</title>
<!--[include src="../_htmlhead_.aspx" /]-->
<script type="text/javascript">
function switchForm(style)
{
$("complexForm").style.display=style==0?"":"none";
$("simpleForm").style.display=style==0?"none":"";
}

function submitForm(operationCode)
{
    if(l.selectCount()==0)
    {
        alert("请选择用户");
        return false;
    }
    
    else if(operationCode=='sendEmail')
    {
        var selectedString=''; 
        var selectedArray=l.selectedList();
        selectedString=selectedArray.join(",");
        
        if(selectedString=='')
        {
            alert("请选择邮件接收人");
            return false;
        }
        open('$dialog/user-massemailing.aspx?userid='+selectedString);
        return false;
    }
    else if(operationCode=='delete')
    {
        return confirm("您确定要删除这些用户账号吗？\n这将导致这些用户的所有数据也一并被删除。\n是否继续？")
    }
}
var l;
addPageEndEvent(function()
{
     l=new checkboxList("userids","select");
});
</script>
</head>
<body>
<!--[include src="../_head_.aspx"/]-->

<!--[include src="../_setting_msg_.aspx"/]-->

<!--[if $HasNoPermissionRole]-->
<div class="Tip Tip-permission">由于权限限制, 你无法管理以下用户组的用户: $NoPermissionRoleList. 此处不会列出这些用户.</div>
<!--[/if]-->
<div class="Content">
    <div class="PageHeading">
        <h3>用户管理</h3>
        <div class="ActionsBar">
        <a href="$admin/user/manage-user-add.aspx"><span>添加用户</span></a>
        </div>
    </div>
<!--[AdminUserSearchForm filtername="filter"]-->
    <div class="SearchTable" id="complexForm" style="$_if($_get.mode=='complex','','display:none');">
        <form action="$_form.action" method="post" id="complex">
        <table>
        <tr>
            <td>用户ID</td>
            <td><input name="id" type="text" class="text" value="$filter.userid" /></td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>用户名</td>
            <td><input name="username" type="text" class="text" value="$_form.text('username',$filter.username)" /></td>
            <td>真实姓名</td>
            <td><input name="realname"  type="text" class="text" value="$_form.text('realname',$filter.realname)" /></td>
        </tr>
        <tr>
                <td>用户组</td>
                <td>
                <select name="roleid"> 
                <option value="">不限</option>
                <!--[loop $r in $allRoleList]-->
                <option value="$r.roleid" $_form.selected('role','$r.roleid',$_this.filter.role==$r.roleid)>$r.name</option>
                <!--[/loop]-->
                 </select>
                 </td>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>注册IP</td>
                <td><input name="registerip" type="text" class="text" value="$_form.text('registerip',$filter.registerip)" /></td>
                <td>上次访问IP</td>
                <td><input name="lastip" type="text" class="text" value="$_form.text('lastip',$filter.LastVisitIP)" /></td>
            </tr>
            <tr>
                <td>Email</td>
                <td colspan="3"><input name="email" type="text" class="text" value="$_form.text('email',$filter.email)" /></td>
            </tr>
            <tr>
                <td>注册时间介于</td>
                <td><input name="regdate_1" id="regdate_1" type="text" class="text" value="$_form.text('regdate_1',$filter.RegisterDate_1)" />
                <a title="选择日期" id="A0" class="selector-date" href="javascript:void(0)"><img src="$Root/max-assets/images/calendar.gif" alt="选择日期" /></a>

                </td>
                <td>到</td>
                <td><input name="regdate_2" id="regdate_2" type="text" class="text" value="$_form.text('regdate_2',$filter.RegisterDate_2)" />
                <a title="选择日期" id="A1" class="selector-date" href="javascript:void(0)"><img src="$Root/max-assets/images/calendar.gif" alt="选择日期" /></a>
                </td>
            </tr>
            <tr>
                <td>最后访问时间介于</td>
                <td><input name="visitdate_1" id="visitdate_1" type="text" class="text" value="$_form.text('visitdate_1',$filter.LastVisitDate_1)" />
                <a title="选择日期" id="A2" class="selector-date" href="javascript:void(0)"><img src="$Root/max-assets/images/calendar.gif" alt="选择日期" /></a>
                </td>
                <td>到</td>
                <td><input name="visitdate_2" id="visitdate_2" type="text" class="text" value="$_form.text('visitdate_2',$filter.LastVisitDate_2)" />
                <a title="选择日期" id="A3" class="selector-date" href="javascript:void(0)"><img src="$Root/max-assets/images/calendar.gif" alt="选择日期" /></a>
                </td>
            </tr>
            <tr>
                <td>出生时间</td>
                <td>
                <select id="birthyear" name="birthyear">
                    <option value="0" $_form.selected("birthyear","0",$filter.birthyear==0)>年</option>
                    <!--[loop $year in $Years]--> 
                    <option value="$year" $_form.selected("birthyear",$year.tostring(),$year==$_this.filter.birthyear) >$year</option>
                    <!--[/loop]-->
                </select>
                <select id="birthmonth" name="birthmonth">
                    <option value="0" $_form.selected("birthmonth","0",$filter.birthmonth==0)>月</option>
                    <!--[loop 1 to 12 with $var1]-->
                    <option value="$var1" $_form.selected("birthmonth","$var1",$_this.filter.birthmonth==$var1)>$var1</option>
                    <!--[/loop]-->
                </select>
                <select id="birthday" name="birthday">
                    <option value="0" $_form.selected("birthday","0",$filter.birthday==0)>日</option>
                    <!--[loop 1 to 31 with $var]-->
                    <option value="$var" $_form.selected("birthday","$var",$_this.filter.birthday==$var)>$var</option>
                    <!--[/loop]-->
                </select>
                </td>
                <td>年龄段</td>
                <td>
                    <input type="text" class="text" style="width:3em;" value="$_if($filter.beginage>0,$filter.beginage.tostring())" name="beginage" /> ~
                    <input type="text" class="text" style="width:3em;" value="$_if($filter.endage>0,$filter.endage.tostring())" name="endage" />
                </td>
            </tr>
            <tr>
                <td>性别</td>
                <td colspan="3">
                    <input id="gender0" type="radio" name="gender" value="" $_form.checked('gender','',$filter.gender==null) /><label for="gender0">不限</label>
                    <input id="gender1" type="radio" name="gender" value="0" $_form.checked('gender','0',$filter.gender==MaxLabs.bbsMax.Enums.Gender.NotSet) /><label for="gender1">未设置</label>
                    <input id="gender2" type="radio" name="gender" value="1" $_form.checked('gender','1',$filter.gender==MaxLabs.bbsMax.Enums.Gender.Male) /><label for="gender2">男</label>
                    <input id="gender3" type="radio" name="gender" value="2" $_form.checked('gender','2',$filter.gender==MaxLabs.bbsMax.Enums.Gender.Female) /><label for="gender3">女</label>
                </td>
            </tr>
            <tr>
                <td>Email验证</td>
                <td colspan="3">
                <input type="radio" name="emailvalid" $_form.checked("emailvalid",null,$filter.emailvalidated==null) value="" id="emailvalidAll" /><label for="emailvalidAll">不限</label>
                <input type="radio" name="emailvalid" $_form.checked("emailvalid","true",$filter.emailvalidated==true) value="true" id="emailvalidYes" /><label for="emailvalidYes">通过验证</label>
                <input type="radio" name="emailvalid" $_form.checked("emailvalid","false",$filter.emailvalidated==false) value="false" id="emailvalidNo" /><label for="emailvalidNo">未通过验证</label>
                </td>
            </tr>
            <!--[ExtendedFieldList]-->
            <tr>
                <td>$Name</td>
                <td colspan="3">
                    <!--[load src="$fieldType.FrontendControlSrc" value="$parent($Filter.ExtendedFields[$this($key)])" field="$_this" /]-->
                </td>
            </tr>
            <!--[/ExtendedFieldList]-->
            <tr>
                <td>模糊搜索</td>
                <td colspan="3">
                    <input name="fuzzy" id="fuzzysearch_on" type="radio" $_form.checked("fuzz","true",$filter.FuzzySearch==null||$filter.FuzzySearch==true) value="true" /><label for="fuzzysearch_on">使用模糊搜索</label>
                    <input name="fuzzy" id="fuzzysearch_off" type="radio" $_form.checked("fuzz","false",$filter.FuzzySearch==false) value="false" /><label for="fuzzysearch_off">不使用模糊搜索</label>
                </td>
            </tr>
            <tr>
                <td>排列顺序</td>
                <td>
                    <select id="Select1" name="order">
                        <option value="ID" $_form.selected('orderField', "", $filter.Order==UserOrderBy.UserID)>ID</option>
                        <option value="CreateDate" $_form.selected("orderField","",$filter.Order==UserOrderBy.CreateDate)>注册时间</option>
                        <option value="UpdateDate" $_form.selected("orderField","",$filter.Order==UserOrderBy.UpdateDate)>最后更新时间</option>
                        <option value="TotalOnlineTime" $_form.selected("orderField","",$filter.Order==UserOrderBy.TotalOnlineTime)>累积在线时长</option>
                        <option value="MonthOnlineTime" $_form.selected("orderField","",$filter.Order==UserOrderBy.MonthOnlineTime)>本月在线时长</option>
                        <option value="Points" $_form.selected("orderField","",$filter.Order==UserOrderBy.Points)>总积分</option>
                        <option value="TotalFriends" $_form.selected("orderField","",$filter.Order==UserOrderBy.TotalFriends)>好友数量</option>
                        <option value="LoginCount" $_form.selected("orderField","",$filter.Order==UserOrderBy.LoginCount)>登陆次数</option>
                        <option value="TotalInvite" $_form.selected("orderField","",$filter.Order==UserOrderBy.TotalInvite)>邀请注册人数</option>
                        <option value="TotalViews" $_form.selected("orderField","",$filter.Order==UserOrderBy.TotalViews)>空间浏览量</option>
                    </select>
                    <select id="Select2" name="desc">
                        <option value="true" $_form.selected("ordetype","true", $filter.isdesc)>按降序排列</option>
                        <option value="false"  $_form.selected("ordetype","false", !$filter.isdesc)>按升序排列</option>
                    </select>
                </td>
                <td>每页显示数</td>
                <td>
                    <select id="Select3" name="pagesize">
                        <option value="10"  $_form.selected("pagesize","",$filter.pagesize==10)>10</option>
                        <option value="20"  $_form.selected("pagesize","",$filter.pagesize==20)>20</option>
                        <option value="50"  $_form.selected("pagesize","",$filter.pagesize==50)>50</option>
                        <option value="100" $_form.selected("pagesize","",$filter.pagesize==100)>100</option>
                        <option value="200" $_form.selected("pagesize","",$filter.pagesize==200)>200</option>
                        <option value="500" $_form.selected("pagesize","",$filter.pagesize==500)>500</option>
                    </select>
                </td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td><input name="searchusers" value="搜索" class="button" type="submit" /></td>
                <td>&nbsp;</td>
                <td style="text-align:right;">
                    <input name="mode" type="hidden" value="complex" />
                    <a href="#" onclick="switchForm(1);return false;">切换到简单搜索模式</a>
                </td>
            </tr>
        </table>
        </form>
    </div>
    <div class="SearchTable" id="simpleForm" style="$_if($_get.mode!='complex','','display:none');">
        <form action="$_form.action" method="post" id="simple">
        <table>
            <tr>
                <td>用户ID</td>
                <td><input name="id" type="text" class="text" value="$filter.userid" /></td>
                <td>用户名</td>
                <td><input name="username" type="text" class="text" value="$filter.username" /></td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td colspan="3">
                    <input name="fuzzy" id="chkFuzzy" type="checkbox" value="false" $_form.checked("fuzzy","false",$filter.FuzzySearch==false) />
                    <label for="chkFuzzy">精确搜索</label>
                </td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td colspan="2">
                    <input type="hidden" name="IsDesc" value="true" />
                    <input type="submit" class="button" name="searchusers" value="搜索" />
                </td>
                <td style="text-align:right;">
                    <input name="mode" type="hidden" value="simple" />
                    <a href="#" onclick="switchForm(0);return false;">切换到高级搜索模式</a>
                </td>
            </tr>
        </table>
        </form>
    </div>
<!--[/AdminUserSearchForm]-->

<!--[UserSearchList filter="filter" mode="admin" pageNumber="$_get.page" ]-->
<!--[head]-->
    <form id="listForm" action="$_form.action" method="post">
    <div class="DataTable">
        <h4>用户 <span class="counts">总数: $rowcount</span></h4>
        <!--[if $hasItems]-->
        <table>
        <thead>
            <tr>
                <td class="CheckBoxHold">&nbsp;</td>
                <td>&nbsp;</td>
                <td>ID</td>
                <td>用户名</td>
                <td>真实姓名</td>
                <td>性别</td>
                <td>总积分</td>
                <td>注册时间</td>
                <td>最后浏览时间</td>
                <td>最后浏览IP</td>
                <td>本月/总在线时间</td>
                <td style="width:80px;">操作</td>
            </tr>
        </thead>
        <tbody>
        <!--[/if]-->
    <!--[/head]-->
    <!--[item]-->
        <tr>
            <td><input type="checkbox" name="userids" value="$userItem.ID" /></td>
            <td>$userItem.Avatar</td>
            <td>$useritem.UserID</td>
            <td><a onclick="openDialog('$dialog/user-view.aspx?id=$userItem.ID'); return false;" href="$dialog/user-view.aspx?id=$userItem.ID">$userItem.Username</a></td>
            <td>$userItem.realname</td>
            <td>$userItem.gendername</td>
            <td><a href="$dialog/user-pointdetails.aspx?id=$userItem.id" onclick="return openDialog(this.href,this)">$userItem.Points</a></td>
            <td>$outputDatetime($useritem.CreateDate)</td>
            <td>$outputDatetime($useritem.LastVisitDate)</td>
            <td>
            $outputip($useritem.LastVisitip)<br />
            $OutputIPAddress($useritem.LastVisitip)
            </td>
            <td>$OutputTotalTime($userItem.MonthOnlineTime) / $OutputTotalTime($userItem.TotalOnlineTime, MaxLabs.bbsMax.Enums.TimeUnit.Day)</td>
            <td>
            <a class="menu-dropdown" href="javascript:;" id="m_t_$useritem.UserID" onclick="return openUserMenu(this,$useritem.UserID,'user')">操作</a>
            </td>
        </tr>
    <!--[/item]-->
    <!--[foot]-->
        <!--[if $hasItems]-->
        </tbody>
        </table>
        <div class="Actions">
            <input type="checkbox" id="select" />
            <label for="select">全选</label>
            <input class="button" id="Button1" type="button" name="delete" onclick="postToDialog({url:'$dialog/user/user-batchdelete.aspx',formId:'listForm',callback:refresh})" value="删除用户" />
            <!--input class="button" id="Button2" type="submit" value="激活账号" onclick="return submitForm('')" name="active" /-->                
        </div>
        <!--[AdminPager Count="$rowcount" PageSize="$pagesize" /]-->
        <!--[else]-->
        <div class="NoData">没有符合条件的用户数据.</div>
        <!--[/if]-->
    </div>
    </form>
    <!--[/foot]-->
<!--[/UserSearchList]-->
</div>
<script type="text/javascript">

    initDatePicker('visitdate_1','A2');
    initDatePicker('visitdate_2', 'A3');
    initDatePicker('regdate_1', 'A0');
    initDatePicker('regdate_2', 'A1');
    
</script>
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>