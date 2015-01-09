<!--[DialogMaster title="添加 $UserToAdd.name 为好友" width="400"]-->
<!--[place id="body"]-->
<form id="form1" action="$_form.action" method="post">

<!--[include src="_error_.ascx" /]-->
<!--[if $IsMyFriend]-->
<div class="dialogmsg dialogmsg-alert">您和$UserToAdd.name已经是好友。</div>
<!--[/if]-->
    
<div class="clearfix dialogbody">
    <div class="dialogform dialogform-horizontal">
        <div class="formrow">
            <h3 class="label"><label for="groupSelect">好友</label></h3>
            <div class="form-enter">
                $UserToAdd.Avatar
                <input type="hidden" value="false" name="createGroup" id="creategroup" />
            </div>
        </div>
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
            <h3 class="label">分组名称</h3>
            <div class="form-enter">
                <input name="newgroup" class="text" maxlength="10" id="newgroup" value="$_form.text('newgroup','')" />
                <a href="javascript:void(createGroup(false))">取消</a>
            </div>
        </div>
        <!--[if $IsMyFriend==false]-->
        <div class="formrow">
            <h3 class="label">附言</h3>
            <div class="form-enter">
                <textarea cols="30" rows="3" name="Note"></textarea>
            </div>
        </div>

        <!--[validateCode actionType="$validateActionName"]-->
        <div class="formrow">
        <h3 class="label"><label for="validatecode">验证码</label></h3>
        <div class="form-enter">
        <input type="text" class="text validcode" name="$inputName" id="validatecode" tabindex="4" $_if($disableIme,'style="ime-mode:disabled;"') autocomplete="off" />
        <span class="captcha">
        <img alt="" src="$imageurl" title="看不清,点击刷新" onclick="this.src=this.src+'&rnd=' + Math.random();" />
        </span>
        </div>
        <!--[error name="$inputName"]-->
        <div class="form-tip tip-error">$message</div>
        <!--[/error]-->
        <div class="form-note">$tip</div>
        </div>
        <!--[/validateCode]-->
        <!--[/if]-->
    </div>
</div>

<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" name="addfriend" accesskey="y" title="确认"><span><!--[if $IsMyFriend==false]-->确认(<u>Y</u>)<!--[else]-->移动至指定分组<!--[/if]--></span></button>
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
