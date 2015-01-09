<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
<title>消息管理</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
    <div class="PageHeading">
        <h3>消息管理 </h3>
        <div class="ActionsBar">
            <a class="back" href="$admin/interactive/manage-chatsession.aspx"><span>返回对话管理</span></a>
        </div>
    </div>
    <div class="Help">
        以下是 $User.username 和 $TargetUser.username 的历史对话内容。<br />
        该对话内容 $User.username 和 $TargetUser.username 各保存一份，  点击这里<a href="$admin/interactive/manage-chatmessage.aspx?userid=$targetuser.userid&targetuserid=$user.userid">切换到 $TargetUser.username 和 $User.username  的对话列表 </a>
    </div>
    <form method="post" id="form1" action="">
    <div class="DataTable">
        <h4>消息管理 <span class="counts">总数:$messagelist.totalrecords </span></h4>
        <ul class="chatmessagelist">
            <!--[loop $m in $messagelist]-->
            <li>
                <div>
                    <input type="checkbox" name="ids" id="$m.messageid" value="$m.messageid" />
                    <!--[if $m.isreceive]-->
                    <label for="$m.messageid">$m.targetuser.username</label>
                    <!--[else]-->
                    <label for="$m.messageid">$m.user.username</label>
                    <!--[/if]-->
                    $outputfriendlydateTime($m.CreateDate)
                    <!--[if !$m.isread]-->
                    <span style="color:red;">(对方未读)</span>
                    <!--[/if]-->
                </div>
                <div>
                    $m.OriginalContent
                </div>
            </li>
            <!--[/loop]-->
        </ul>
    </div>
    <div class="Actions">
        <input type="checkbox" id="selectAll" />
        <label for="selectAll">全选</label>
        <input class="button" id="Button1" type="submit" name="delete" onclick="" value="删除" />
        <!--[AdminPager count="$messagelist.totalrecords" PageSize="20" /]-->
    </div>
    </form>
    </div>
    <script type="text/javascript">
        var l= new checkboxList('ids','selectAll');
    </script>
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>