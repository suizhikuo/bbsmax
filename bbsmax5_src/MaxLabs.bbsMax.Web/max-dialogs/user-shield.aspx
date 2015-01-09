<!--[DialogMaster title="用户屏蔽状态" subtitle="$User.username" width="500"]-->
<!--[place id="body"]-->
<form id="form1" method="post" action="$_form.action">
<!--[include src="_error_.ascx" /]-->
<!--[success]-->
<div class="dialogmsg dialogmsg-success">成功修改屏蔽状态</div>
<!--[/success]-->
<input type="hidden" name="username" value="$user.username" />

<div class="clearfix dialogtabwrap">
    <div class="dialogtab" id="list">
        <ul>
            <li><a href="$dialog/user-shield.aspx?tabid=1&userid=$user.userid&isdialog=1" $_IF($_get.tabid!="2",'class="current"','')  ><span>屏蔽操作</span></a></li>
            <li><a href="$dialog/user-shield.aspx?tabid=2&userid=$user.userid&isdialog=1" $_IF($_get.tabid=="2",'class="current"','')  ><span>屏蔽记录</span></a></li>
        </ul>
    </div>
</div>

<!--[if $BanUserList==null]-->
<div class="clearfix dialogbody" id="dialog-form">
    <div class="dialogform">
        <div class="formrow">
            <div class="form-enter">
                <input id="unshield" value="0" type="radio" name="shield" $_form.checked("shield",'0',!$IsBanned) />
                <label for="unshield">未在任何版块被屏蔽</label>
            </div>
        </div>
        <div class="formrow">
            <div class="form-enter">
                <input id="shieldallforum" value="1" type="radio" name="shield" $_form.checked("shield",'1',$FullSiteBanned) />
                <label for="shieldallforum">整站屏蔽</label>
                解除时间
                <span class="datepicker">
                    <input type="text" class="text" name="enddate.0" id="enddate.0" value="$_form.text('enddate.0',$GetBannedEndDate(0))" />
                    <a id="d_0" href="javascript:void(0)">日期</a>
                </span>
            </div>
        </div>
        <div class="formrow">
            <div class="form-enter">
                <input id="shieldspecifyforum" value="2" type="radio" name="shield" $_form.checked("shield",'2',$IsForumBanned) />
                <label for="shieldspecifyforum">在指定的版块屏蔽</label>
            </div>
        </div>
        <div class="formrow" id="forumContainer" style="display:$_if($IsForumBanned,"block","none");">
        <div class="datatablewrap" style="height:200px;">
            <table class="datatable" id="forumTable">
            <thead>
            <tr>
                <th>被屏蔽的版块</th>
                <th>自动解除屏蔽时间</th>
            </tr>
            </thead>
            <tbody>
            <!--[loop $forum in $forums with $i]-->
            <tr>
                <td>
                    $ForumSeparators[$i]<input onclick="checkForum(this)" type="checkbox"  $_if($forum.islink||!$CanBan($forum.forumId), 'disabled="disabled"') id="forum-$GetForumPath($forum.ForumID)" $_form.Checked('forums',$forum.forumid,$IsBannedByForum($forum.forumid)) name="forums" value="$forum.ForumID" />
                    <label for="forum-$GetForumPath($forum.ForumID)">$forum.ForumName</label>
                </td>
                <td>
                    <span class="datepicker">
                        <input $_if($forum.islink||!$CanBan($forum.forumId), 'disabled="disabled"') type="text" class="text" value="$_form.text('enddate.$forum.forumId',$GetBannedEndDate($forum.forumid))" name="enddate.$forum.forumId" id="enddate.$forum.forumId" />
                        <a id="d_$forum.forumid" href="javascript:void(0)">日期</a>
                    </span>
                </td>
            </tr>
            <!--[/loop]-->
            </tbody>
            </table>
        </div>
        </div>
        <div class="formrow">
            <h3 class="label"><label for="cause">屏蔽原因</label></h3>
            <div class="form-enter">
                <input type="text" name="cause" id="cause" class="text longtext" id="cause"/>
            </div>
        </div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" name="ok" accesskey="y" title="确认"><span>确认(<u>Y</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
<script type="text/javascript">
    initDisplay('shield', [
 { value: '0', display: false, id: 'forumContainer' }
, { value: '1', display: false, id: 'forumContainer' }
, { value: '2', display: true, id: 'forumContainer' }
]);

 function checkForum(forum) {
     var prefix = forum.id;
     var childCheckbox = findElement("input", prefix);
     for (var i = 0; i < childCheckbox.length; i++)
         childCheckbox[i].checked = forum.checked;
 }

    var arrayD = findElement("input", "enddate");
    var regid = /\d+/;
    for (var i = 0; i < arrayD.length; i++) {
        var d = arrayD[i].id.match(regid);
        initDatePicker(arrayD[i], 'd_' + d);
    }
</script>
<!--[else]-->
<div class="clearfix dialogbody" id="shieldlist">
    <!--[if $BanUserList.count>0]-->
    <div class="datatablewrap" style="height:300px;">
    <table class="datatable">
        <thead>
            <tr>
                <td>ID</td>
                <td>用户名</td>
                <td>操作类型</td>
                <td>论坛版块</td>
                <td>屏蔽结束时间</td>
                <td>操作原因</td>
                <td>操作者</td>
                <td>操作时间</td>
            </tr>
        </thead>
        <tbody>
        <!--[loop $item in $BanUserList]-->  
            <!--[if $item.OperationType==BanType.UnBan]-->
            <tr>
                <td>$item.UserID </td>
                <td>$item.UserName</td>
                <td>解除屏蔽</td>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
                <td>$item.Cause</td>
                <td>$item.OperatorName</td>
                <td>$item.OperationTime</td>
            </tr>
            <!--[else]-->
            <!--[loop $info in $item.foruminfolist]-->  
            <tr>
                <td>$item.UserID </td>
                <td>$item.UserName</td>
                <td><!--[if $item.OperationType==BanType.Ban]-->屏蔽<!--[else if $item.OperationType==BanType.UnBan]-->解除屏蔽  <!--[else]-->全站屏蔽<!--[/if]--></td>
                <td><!--[if $info.ForumID!=0]-->$info.ForumName<!--[/if]--></td>
                <td><!--[if $info.EndDate.ToString("d")=="0001-1-1"]--> <!--[else if $info.EndDate.ToString("d")=="9999-12-31"]-->无限期<!--[else]-->$info.EndDate.Tostring("d")<!--[/if]--></td>
                <td>$item.Cause</td>
                <td>$item.OperatorName</td>
                <td>$item.OperationTime</td>
            </tr>
            <!--[/loop]-->
            <!--[/if]-->
        <!--[/loop]-->
        </tbody>
     </table>
    </div>
    <!--[else]-->
    <div class="nodata">
        该用户暂无屏蔽记录
    </div>
    <!--[/if]-->
</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="reset" accesskey="y" title="确认" onclick="panel.close();"><span>确认(<u>Y</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
<!--[/if]-->


</form>

<!--[/place]-->
<!--[/dialogmaster]-->