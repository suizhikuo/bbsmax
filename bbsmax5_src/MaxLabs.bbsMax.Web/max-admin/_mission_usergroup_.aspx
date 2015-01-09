<p>
    <input type="checkbox" id="group.$group.ID" value="$group.ID" name="prizeusergroups" $_form.checked("prizeusergroups",$group.ID,$out($prizeUserGroups))  />
    <label for="group.$group.ID">$group.Name</label> 
    <!--[UserGroupValidityTime mission="$mission" groupID="$group.ID" isEdit="$isEdit"]-->
    (有效时间:
    <input type="text" class="text" style="width:3em;" name="group.time.$group.ID" value="$time" />
    <select name="group.timetype.$group.ID">
    <option value="3" $_form.selected("group.timetype."+$group.ID,"3",$timeUnit)>天</option>
    <option value="2" $_form.selected("group.timetype."+$group.ID,"2",$timeUnit)>小时</option>
    <option value="1" $_form.selected("group.timetype."+$group.ID,"1",$timeUnit)>分钟</option>
    <option value="0" $_form.selected("group.timetype."+$group.ID,"0",$timeUnit)>秒</option>
    </select>)
    <!--[/UserGroupValidityTime]-->
</p>