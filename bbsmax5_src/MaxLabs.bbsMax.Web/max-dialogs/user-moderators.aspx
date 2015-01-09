<!--[DialogMaster title="版主管理" subtitle="你选中版块： $forum.name" width="600"]-->
<!--[place id="body"]-->
<form id="form" method="post" action="$_form.action">
<!--[include src="_error_.ascx" /]-->
<!--[success]-->
<div class="dialogmsg dialogmsg-success">版主修改成功</div>
<!--[/success]-->

<div class="clearfix dialogbody">
    <div class="datatablewrap" style="height:400px;">
        <table class="datatable" id="table1">
            <thead>
            <tr>
                <th>序号</th>
                <th>类型</th>
                <th>版主(用户名)</th>
                <th>任期</th>
                <th>操作</th>
            </tr>
            </thead>
            <tbody>
            <!--[loop $m in $InheritedModeratorsList]-->
            <tr>
                <td>&nbsp;</td>
                <td>$m.name (父版块继承)</td>
                <td>$m.user.username</td>
                <td>
                    $outputdatetime($m.begindate) -
                    $outputdatetime($m.enddate) 
                </td>
                <td>&nbsp;</td>
            </tr>
            <!--[/loop]-->
            <!--[loop $m in $ModeratorsList]-->
            <tr>
                <td><input class="text" type="text" style="width:2em;" value="$m.sortorder" name="sortorder.$m.forumid.$m.userid" /> </td>
                <td>$m.name
                    <input type="hidden" value="$m.forumid.$m.userid" name="forum-user" />
                    <input type="hidden" value="$m.ModeratorType" name="ModeratorType.$m.forumid.$m.userid" />
                    <input type="hidden" value="$m.isnew" name="isnew.$m.forumid.$m.userid" />
                </td>
                <td>$m.user.username</td>
                <td>
                    <span class="datepicker">
                        <input type="text" class="text" id="begindate.$m.forumid.$m.userid" name="begindate.$m.forumid.$m.userid" value="$_form.text('',$outputdatetime($m.begindate))" />
                        <a href="#">日期</a>
                    </span>
                    -
                    <span class="datepicker">
                        <input type="text" class="text" id="enddate.$m.forumid.$m.userid" name="enddate.$m.forumid.$m.userid" value="$_form.text('',$outputdatetime($m.enddate))" />
                        <a href="#">日期</a>
                    </span>
                </td>
                <td><a href="$dialog/user-moderators-remove.aspx?forumid=$forumid&userid=$m.userid" onclick="return openDialog(this.href,this,refresh)">解除</a></td>
            </tr>
            <!--[/loop]-->
            <tr id="newrow">
                <td><input class="text" type="text" style="width:2em;" name="sortorder.new.{0}" value="0" /></td>
                <td>
                    <input type="hidden" id="newmoderatorsid" name="newmoderatorsid" value="{0}" />
                    <select name="ModeratorType.new.{0}">
                    <!--[if $forum.ParentID==0]-->
                    <option value="CategoryModerators">分类版主</option>
                    <!--[else]-->
                    <option value="Moderators">版主</option>
                    <option value="JackarooModerators">实习版主</option>
                    <!--[/if]-->
                    </select>
                </td>
                <td>
                    <input type="text" class="text" name="username.new.{0}" style="width:8em;" />
                </td>
                <td>
                    <span class="datepicker">
                        <input type="text" class="text" name="begindate.new.{0}" id="begindate.new.{0}" value="" />
                        <a href="#">日期</a>
                    </span>
                    -
                    <span class="datepicker">
                        <input type="text" class="text" name="enddate.new.{0}" id="enddate.new.{0}" />
                        <a href="#">日期</a>
                    </span>
                </td>
                <td>
                    <a id="deleteRow{0}" href="javascript:;">取消</a>
                </td>
            </tr>
            </tbody>
        </table>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" name="ok" accesskey="y" title="确定"><span>确定(<u>Y</u>)</span></button>
    <button class="button" type="button" onclick="dt.insertRow(createDatePicker)" accesskey="a" ><span>添加版主(<u>A</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="关闭" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
<script type="text/javascript">
var dt=new  DynamicTable("table1","newmoderatorsid");
</script>
<script type="text/javascript">
    var arrayD = findElement("input", "enddate");

    for (var i = 0; i < arrayD.length; i++) {
        initDatePicker(arrayD[i]);
    }
    arrayD = findElement("input", "begindate");

    for (var i = 0; i < arrayD.length; i++) {
        initDatePicker(arrayD[i]);
    }

    function createDatePicker(id) {
        initDatePicker("begindate.new." + id);
        initDatePicker("enddate.new." + id);
    }
</script>
</form>
<!--[/place]-->
<!--[/dialogmaster]-->