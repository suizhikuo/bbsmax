<!--[DialogMaster title="移动好友到新分组" width="400"]-->
<!--[place id="body"]-->
<form action="$_form.action" method="post">
<!--[include src="_error_.ascx" /]-->
<div class="clearfix dialogbody">
    <div class="dialogform dialogform-horizontal">
        <div class="formrow">
            <h3 class="label">好友</h3>
            <div class="form-enter">
                <!--[loop $friend in $FriendListToMove]-->
                $friend.user.name 
                <!--[/loop]-->
                <input type="hidden" value="false" name="createGroup" id="creategroup" />
            </div>
        </div>
        <div class="formrow" id="groupSelect">
            <h3 class="label"><label for="GroupID">目标分组</label></h3>
            <div class="form-enter">
                <select id="GroupID" name="GroupID">
                    <!--[loop $group in $FriendGroupList]-->
                        <!--[if $CurrentGroupID==$group.GroupID]-->
                        <option value="$group.ID" selected="selected" >$group.Name</option>
                        <!--[else]-->
                        <option value="$group.ID" >$group.Name</option>
                        <!--[/if]-->
                    <!--[/loop]-->
                </select>
                <a href="javascript:void(createGroup(true));">创建分组</a>
            </div>
        </div>
        <div class="formrow" id="createnewGroup" style="display:none;">
            <h3 class="label">分组名称</h3>
            <div class="form-enter">
                <input name="newgroup" class="text" maxlength="10" id="newgroup" value="$_form.text('newgroup','')" />
                <a href="javascript:void(createGroup(false))">取消</a>
            </div>
        </div>
    </div>
</div>

<div class="clearfix dialogfoot">
    <input type="hidden" name="uid" value="$FriendUserIdsText" />
    <button class="button button-highlight" type="submit" name="movefriend" accesskey="y" title="确认"><span>确认(<u>Y</u>)</span></button>
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

    $_if($CreateGroup, 'createGroup(true);')
  
</script>
<!--[/place]-->
<!--[/dialogmaster]-->