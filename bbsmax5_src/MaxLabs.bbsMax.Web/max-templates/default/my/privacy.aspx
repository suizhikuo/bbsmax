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
                                <h3 class="pagecaption-title"><span>隐私设置</span></h3>
                            </div>

                            <div class="formcaption">
                                <h3>主页内容隐私</h3>
                                <p>你可以完全控制哪些人可以看到你的主页上面的内容.</p>
                            </div>
                            <!--[success]-->
                            <div class="successmsg">操作成功.</div>
                            <!--[/success]-->
                            <!--[unnamederror]-->
                            <div class="errormsg">$message</div>
                            <!--[/unnamederror]-->
                            <form method="post" action="$_form.action">
                            <div class="formgroup">
                                <div class="formrow">
                                    <label class="label">个人主页</label>
                                    <div class="form-enter">
                                        <select name="SpacePrivacy">
                                        <option value="All" $_Form.selected("SpacePrivacy","All",$My.SpacePrivacy==SpacePrivacyType.All)>全站用户可见</option>
                                        <option value="Friend" $_Form.selected("SpacePrivacy","Friend",$My.SpacePrivacy==SpacePrivacyType.Friend)>仅好友可见</option>
                                        <option value="Self" $_Form.selected("SpacePrivacy","Self",$My.SpacePrivacy==SpacePrivacyType.Self)>仅自己可见</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="formrow">
                                    <label class="label">个人基本资料</label>
                                    <div class="form-enter">
                                        <select name="InformationPrivacy">
                                        <option value="All" $_Form.selected("InformationPrivacy","All",$My.InformationPrivacy==SpacePrivacyType.All)>全站用户可见</option>
                                        <option value="Friend" $_Form.selected("InformationPrivacy","Friend",$My.InformationPrivacy==SpacePrivacyType.Friend)>仅好友可见</option>
                                        <option value="Self" $_Form.selected("InformationPrivacy","Self",$My.InformationPrivacy==SpacePrivacyType.Self)>仅自己可见</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="formrow">
                                    <label class="label">好友列表</label>
                                    <div class="form-enter">
                                        <select name="FriendListPrivacy">
                                        <option value="All" $_Form.selected("FriendListPrivacy","All",$My.FriendListPrivacy==SpacePrivacyType.All)>全站用户可见</option>
                                        <option value="Friend"  $_Form.selected("FriendListPrivacy","Friend",$My.FriendListPrivacy==SpacePrivacyType.Friend)>仅好友可见</option>
                                        <option value="Self" $_Form.selected("FriendListPrivacy","Self",$My.FriendListPrivacy==SpacePrivacyType.Self)>仅自己可见</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="formrow">
                                    <label class="label">留言板</label>
                                    <div class="form-enter">
                                        <select name="BoardPrivacy">
                                        <option value="All" $_Form.selected("BoardPrivacy","All",$My.BoardPrivacy==SpacePrivacyType.All)>全站用户可见</option>
                                        <option value="Friend" $_Form.selected("BoardPrivacy","Friend",$My.BoardPrivacy==SpacePrivacyType.Friend)>仅好友可见</option>
                                        <option value="Self"  $_Form.selected("BoardPrivacy","Self",$My.BoardPrivacy==SpacePrivacyType.Self)>仅自己可见</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="formrow">
                                    <label class="label">个人动态</label>
                                    <div class="form-enter">
                                        <select name="FeedPrivacy">
                                        <option value="All" $_Form.selected("FeedPrivacy","All",$My.FeedPrivacy==SpacePrivacyType.All)>全站用户可见</option>
                                        <option value="Friend"  $_Form.selected("FeedPrivacy","Friend",$My.FeedPrivacy==SpacePrivacyType.Friend)>仅好友可见</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="formrow">
                                    <label class="label">记录</label>
                                    <div class="form-enter">
                                        <select name="DoingPrivacy">
                                        <option value="All"  $_Form.selected("DoingPrivacy","All",$My.DoingPrivacy==SpacePrivacyType.All)>全站用户可见</option>
                                        <option value="Friend" $_Form.selected("DoingPrivacy","Friend",$My.DoingPrivacy==SpacePrivacyType.Friend)>仅好友可见</option>
                                        </select>
                                    </div>
                                    <p class="form-note">
                                        本隐私设置仅在其他用户查看您主页时有效; 在全站的记录列表中可能会出现您的记录.
                                    </p>
                                </div>
                                <div class="formrow">
                                    <label class="label">日志</label>
                                    <div class="form-enter">
                                        <select name="BlogPrivacy">
                                        <option value="All" $_Form.selected("BlogPrivacy","All",$My.BlogPrivacy==SpacePrivacyType.All)>全站用户可见</option>
                                        <option value="Friend"  $_Form.selected("BlogPrivacy","Friend",$My.BlogPrivacy==SpacePrivacyType.Friend)>仅好友可见</option>
                                        </select>
                                    </div>
                                    <p class="form-note">
                                        本隐私设置仅在其他用户查看您主页时有效; 相关浏览权限需要在每篇日志中单独设置方可完全生效.
                                    </p>
                                </div>
                                <div class="formrow">
                                    <label class="label">相册</label>
                                    <div class="form-enter">
                                        <select name="AlbumPrivacy">
                                        <option value="All"  $_Form.selected("AlbumPrivacy","All",$My.AlbumPrivacy==SpacePrivacyType.All)>全站用户可见</option>
                                        <option value="Friend" $_Form.selected("AlbumPrivacy","Friend",$My.AlbumPrivacy==SpacePrivacyType.Friend)>仅好友可见</option>
                                        </select>
                                    </div>
                                    <p class="form-note">
                                        本隐私设置仅在其他用户查看您主页时有效; 相关浏览权限需要在每个相册中单独设置方可完全生效.
                                    </p>
                                </div>
                                <div class="formrow">
                                    <label class="label">分享</label>
                                    <div class="form-enter">
                                        <select name="SharePrivacy">
                                        <option value="All"  $_Form.selected("SharePrivacy","All",$My.SharePrivacy==SpacePrivacyType.All)>全站用户可见</option>
                                        <option value="Friend" $_Form.selected("SharePrivacy","Friend",$My.SharePrivacy==SpacePrivacyType.Friend)>仅好友可见</option>
                                        </select>
                                    </div>
                                    <p class="form-note">
                                        本隐私设置仅在其他用户查看您主页时有效; 在全站的分享列表中可能会出现您的分享.
                                    </p>
                                </div>
                                <div class="formrow formrow-action">
                                    <span class="minbtn-wrap"><span class="btn"><input type="submit" name="savespaceprivacy" value="保存设置" class="button" /></span></span>
                                </div>
                            </div>
                            <div class="formcaption">
                                <h3>个人动态隐私</h3>
                                <p>系统会将您的各项动作反映到个人动态里，方便朋友了解你的动态。你可以控制是否在下列动作发生时，在个人动态里发布相关信息.</p>
                            </div>
                            <div class="formgroup">
                            <!--[loop $app in $AppList with $i]-->
                                <div class="formrow">
                                    <!--[if $AppCount == 1]-->
                                    <h3 class="label"><label>$App.AppName</label></h3>
                                    <!--[else]-->
                                    <h3 class="label"><label for="app.$i">$App.AppName</label></h3>
                                    <ul class="clearfix form-optionlist">
                                        <li>
                                            <input type="checkbox" $_form.checked("apps","$App.AppID") value="$App.AppID" name="apps" id="app.$i" onclick="checkChild(this)" />
                                            <label for="app.$i">$App.AppName</label>
                                        </li>
                                    </ul>
                                    <!--[/if]-->
                                    <!--[if $GetAppActionList($app).count > 0]-->
                                    <ul class="clearfix form-optionlist">
                                        <!--[loop $appAction in $GetAppActionList($app) with $j]-->
                                        <li>
                                            <!--[if $AppCount == 1]-->
                                            <input type="checkbox" $_form.checked("app_{=$App.AppID}_Actions", "$AppAction.ActionType",$IsAddFeedAction($app.AppID,$AppAction.ActionType)) value="$AppAction.ActionType" name="app_{=$App.AppID}_Actions" id="app.$i.$j" />
                                            <!--[else]-->
                                            <input type="checkbox" $_form.checked("app_{=$App.AppID}_Actions", "$AppAction.ActionType",$IsAddFeedAction($app.AppID,$AppAction.ActionType)) value="$AppAction.ActionType" name="app_{=$App.AppID}_Actions" id="app.$i.$j" onclick="checkParent('app.$i',this)" />
                                            <!--[/if]-->
                                            <label for="app.$i.$j">$AppAction.ActionName</label>
                                        </li>
                                        <!--[/loop]-->
                                    </ul>
                                    <!--[/if]-->
                                </div>
                            <!--[/loop]-->
                                <div class="formrow formrow-action">
                                    <span class="minbtn-wrap"><span class="btn"><input name="button_savefeedprivacy" type="submit" value="保存设置" class="button" /></span></span>
                                </div>
                            </div>
                            </form>
                            
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
<!--[if $AppCount > 1]-->
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
<!--[/if]-->
</body>
</html>
