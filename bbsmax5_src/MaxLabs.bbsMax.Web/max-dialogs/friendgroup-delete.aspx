<!--[dialogmaster title="删除好友分组" width="400"]-->
<!--[place id="body"]-->
<!--[include src="_error_.ascx" /]-->
<form action="$_form.action" method="post">
<div class="clearfix dialogbody">
    <div class="dialogconfirm">
        <h3>您确定要删除这个好友分组"$Group.Name"?</h3>
        <p><input type="radio" id="deletefriends" name="deletefriends" $_form.checked("deletefriends", "1", "1") value="1" /><label for="deletefriends">同时删除该组所有好友</label></p>
        <p><input type="radio" id="movefriends" name="deletefriends" $_form.checked("deletefriends", "0", "1") value="0" /><label for="movefriends">将该组好友转移到</label>
            <select id="friendGroupID" name="ToFriendGroupID">
            <option value="-1">请选择分组</option>
        <!--[loop $temp in $FriendGroupList]-->
            <!--[if $Group.ID!= $temp.ID]-->
            <option value="$temp.ID" $_form.selected("ToFriendGroupID","$temp.ID")>$temp.Name</option>
            <!--[/if]-->
        <!--[/loop]-->
        </select>
        </p>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" name="delete" accesskey="y" title="确定"><span>确定(<u>Y</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<!--[/place]-->
<!--[/dialogmaster]-->