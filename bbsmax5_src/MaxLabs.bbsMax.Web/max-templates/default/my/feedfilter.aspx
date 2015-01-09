<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>$PageTitle</title>
<!--#include file="../_inc/_htmlhead.aspx"-->
<link rel="stylesheet" type="text/css" href="$skin/styles/setting.css" />
</head>
<body>
<div class="container section-setting">
<!--#include file="../_inc/_head.aspx"-->
    <div id="main" class="main">
        <div class="clearfix main-inner">
            <div class="content">
                <!--#include file="../_inc/_round_top.aspx"-->
                <div class="clearfix content-inner">
                    <div class="content-main">
                        <div class="content-main-inner">
                            
                            <div class="clearfix pagecaption">
                                <h3 class="pagecaption-title"><span>好友动态过滤</span></h3>
                            </div>
                            <div class="formcaption">
                                <h3>好友分组动态过滤</h3>
                                <p>系统将会屏蔽以下未选中好友分组的成员动态. </p>
                                <p>需要修改用户组名称或管理好友分组请前往<a href="$url(my/friends)">好友列表</a>页面.</p>
                            </div>
                            
                            <!--[success]-->
                            <div class="successmsg">你已成功修改好友分组动态过滤。</div>
                            <!--[/success]-->
                            <!--[unnamederror]-->
                            <div class="errormsg">$message</div>
                            <!--[/unnamederror]-->
                            
                            <!--[if $FriendGroupList.count < 2]-->
                            <div class="setting-message">
                                <h3>暂无好友分组</h3>
                                <p>当前没有任何好友分组, 请前往<a href="$url(my/friends)">好友列表</a>页面添加分组.</p>
                            </div>
                            <!--[else]-->
                            <form action="$_form.action" method="post">
                            <div class="formgroup">
                                <div class="formrow">
                                    <h3 class="label">好友分组</h3>
                                    <ul class="clearfix form-optionlist">
                                    <!--[loop $friendGroup in $FriendGroupList]-->
                                        <!--[if $friendGroup.GroupID!=0]-->
                                        <li>
                                            <input type="checkbox" value="$FriendGroup.GroupID" $_form.checked("friendGroups","$FriendGroup.GroupID",$FriendGroup.IsShield==false) name="friendGroups" id="FriendGroup.$FriendGroup.GroupID" />
                                            <label for="FriendGroup.$FriendGroup.GroupID">$FriendGroup.Name</label>
                                        </li>
                                        <!--[/if]-->
                                    <!--[/loop]-->
                                </div>
                                <div class="formrow formrow-action">
                                    <span class="minbtn-wrap"><span class="btn"><input type="submit" name="savefriendgroups" value="保存设置" class="button" /></span></span>
                                </div>
                            </div>
                            </form>
                            <!--[/if]-->
                            
                            
                            <div class="formcaption">
                                <h3>当前屏蔽的个别好友动态</h3>
                                <p>以下选中的好友和好友相关的动态都将被屏蔽. 你可以在这里取消选中状态以否取消屏蔽该项.</p>
                            </div>
                            <!--[if $HasFriendFeedFilter]-->
                            <form action="$_form.action" method="post">
                            <div class="formgroup">
                                <!--[loop $filterUser $FeedFilterUserList with $i]-->
                                <div class="formrow">
                                    <label class="label" for="f_$i">$filterUser.FriendUser.NameLink(所有动态)</label>
                                    <div class="form-enter">
                                        <input type="checkbox" value="$filterUser.ID" name="friendFeedFilterIDs" id="f_$i" />
                                        <label for="f_$i">屏蔽动态</label>
                                    </div>
                                </div>
                                <!--[/loop]-->
                                <!--[loop $feedFilter in $FiltratedAppFriendList with $i]-->
                                <div class="formrow">
                                    <label class="label" for="fa_$i">$FeedFilter.FriendUser.NameLink</label>
                                    <div class="form-enter">
                                        <input type="checkbox" value="$feedFilter.UserID" name="friendAppFilters" id="fa_$i" onclick="checkChild(this);" />
                                        <label for="fa_$i">选中全部</label>
                                        <!--[loop $actionFilter in $GetActionFeedFilterList($FeedFilter.FriendUserID.Value) with $j]-->
                                        <p>
                                            <input type="checkbox" value="$actionFilter.ID" name="friendFeedFilterIDs" id="fa_$i.$j" onclick="checkParent('fa_$i',this);" />
                                            <label for="fa_$i.$j">$GetActionName($actionFilter.AppID,$actionFilter.ActionType.Value)</label>
                                        </p>
                                        <!--[/loop]-->
                                    </div>
                                </div>
                                <!--[/loop]-->
                            </div>
                            <!--[/loop]-->
                            <div class="formrow formrow-action">
                                <span class="btn-wrap"><span class="btn"><input type="submit" name="savefriendfeedfilter" value="保存设置" class="button" /></span></span>
                            </div>
                            </form>
                            <!--[else]-->
                            <div class="setting-message">
                                <h3>没有屏蔽好友动态</h3>
                                <p>当前没有屏蔽任何好友动态.</p>
                            </div>
                            <!--[/if]-->
                            
                            
                            <div class="formcaption">
                                <h3>当前屏蔽的个别应用程序动态</h3>
                                <p>以下选中的应用和应用相关的动态都将被屏蔽. 你可以在这里取消选中状态以否取消屏蔽该项.</p>
                            </div>
                            <!--[if $HasAppFeedFilter]-->
                            <form action="$_form.action" method="post">
                            <div class="formgroup">
                            <!--[loop $app in $FiltratedAppList with $i]-->
                                <!--[if $AppCount == 1]-->
                                <!--[loop $appAction in $GetFiltratedAppActionList($App.AppID) with $j]-->
                                <div class="formrow">
                                    <label class="label" for="appFilters_$i.$j">$GetActionName($App.AppID,$AppAction.ActionType.Value)</label>
                                    <div class="form-enter">
                                        <input type="checkbox" value="$GetFeedFilterID($App.AppID,$AppAction.ActionType.Value)" name="appFeedFilterIDs" id="appFilters_$i.$j" />
                                        <label for="appFilters_$i.$j">屏蔽动态</label>
                                    </div>
                                </div>
                                <!--[/loop]-->
                                <!--[else]-->
                                <div class="formgroup">
                                    <label class="label" for="appFilters_$i">$App.AppName</label>
                                    <div class="form-enter">
                                        <input type="checkbox" value="$App.AppID" name="apps" id="appFilters_$i" onclick="checkChild(this);" />
                                        <label for="appFilters_$i">选中全部</label>
                                        <!--[loop $appAction in $GetFiltratedAppActionList($App.AppID) with $j]-->
                                        <p>
                                            <input type="checkbox" value="$GetFeedFilterID($App.AppID,$AppAction.ActionType.Value)" name="appFeedFilterIDs" id="Checkbox1" onclick="checkParent('appFilters_$i',this);" />
                                            <label for="appFilters_$i.$j">$GetActionName($App.AppID,$AppAction.ActionType.Value)</label>
                                        </p>
                                        <!--[/loop]-->
                                    </div>
                                </div>
                                <!--[/if]-->
                                <!--[/loop]-->
                                <div class="formrow">
                                    <span class="btn-wrap"><span class="btn"><input type="submit" name="saveappfeedfilters" value="保存设置" class="button" /></span></span>
                                </div>
                            </div>
                            </form>
                            <!--[else]-->
                            <div class="setting-message">
                                <h3>没有屏蔽应用程序动态</h3>
                                <p>当前没有屏蔽任何应用程序动态.</p>
                            </div>
                            <!--[/if]-->
                            
                        </div>
                    </div>
                    <!--#include file="_sidebar_setting.aspx"-->
                </div>
                <!--#include file="../_inc/_round_bottom.aspx"-->
            </div>
        </div>
    </div>
<!--#include file="../_inc/_foot.aspx"-->
</div>
</body>
<script type="text/javascript">
    function checkChild(obj)
    {
        var chkitem= document.getElementsByTagName("input");
        for (var i=0;i<chkitem.length;i++)
        {
            if( chkitem[i].type=="checkbox")
            {
                if(chkitem[i].id.indexOf(obj.id+".")>-1)
                {
                    chkitem[i].checked = obj.checked;
                }
            }
        }
    }
    function checkParent(id,obj)
    {
        if(obj.checked == false)
        {
            document.getElementById(id).checked = false;
        }
    }
</script>

</html>
