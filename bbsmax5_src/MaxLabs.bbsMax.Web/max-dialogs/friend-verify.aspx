<!--[DialogMaster title="好友请求" width="400"]-->
<!--[place id="body"]-->
<form id="form1" action="$_form.action" method="post">
<!--[include src="_error_.ascx" /]-->

<!--[if !$IsMyFriend]-->
<div class="dialogmsg dialogmsg-alert">$TryAddUser.popupnamelink 请求你加为好友.
</div>
<!--[else]-->
<div class="dialogmsg dialogmsg-alert">您和 $TryAddUser.popupnamelink 已经是好友.</div>
<!--[/if]-->

<div class="clearfix dialogbody">
    <div class="dialogform dialogform-horizontal">
        <input type="hidden" value="false" name="createGroup" id="creategroup" />
        <!--[if $PostScript !="" ]-->
        <div class="formrow">
            <h3 class="label">附言</h3>
            <div class="form-enter">$PostScript</div>
        </div>
        <!--[/if]-->
        <div class="formrow" id="groupSelect">
            <h3 class="label"><label for="groupSelect">放入分组</label></h3>
            <div class="form-enter">
                <select id="friendGroupID" name="ToFriendGroupID">
                    <!--[loop $group in $FriendGroupList]-->
                    <option value="$Group.ID" $_form.selected("ToFriendGroupID","$Group.ID",$FriendGroupID==$Group.ID)>$group.Name</option>
                    <!--[/loop]-->
                </select>
                <a href="javascript:void(createGroup(true));">创建分组</a>
            </div>
        </div>
        <div class="formrow" id="createnewGroup" style="display:none;">
            <h3 class="label"><label for="newgroup">分组名称</label></h3>
            <div class="form-enter">
                <input name="newgroup" class="text" maxlength="10" id="newgroup" value="$_form.text('newgroup','')" style="width:80px;" />
                <a href="javascript:void(createGroup(false))">取消</a>
            </div>
        </div>
    </div>
</div>

<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" name="accept" accesskey="y" title="确认"><span>确认(<u>Y</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<script type="text/javascript">
    function createGroup(r) {
        if (r) {
            $('creategroup').value = 'true';
            $('createnewGroup').style.display = '';
            $('groupSelect').style.display = 'none';
        }
        else {
            $('createnewGroup').style.display = 'none';
            $('groupSelect').style.display = '';
            $('creategroup').value = 'false';
        }

    }

    $_if($CreateGroup, ' createGroup(true);')
</script>
<!--[/place]-->
<!--[/dialogmaster]-->