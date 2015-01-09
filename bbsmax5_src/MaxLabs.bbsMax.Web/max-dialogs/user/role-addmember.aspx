<!--[DialogMaster title="添加用户组成员" width="400"]-->
<!--[place id="body"]-->
<form id="form1" action="$_form.action" method="post">
<!--[include src="_error_.ascx" /]-->
<!--[success]-->
<div class="dialogmsg dialogmsg-success">添加成员成功, 共$useridCount个有效用户名, 成功加入$addcount个.</div>
<!--[/success]-->

<div class="clearfix dialogbody">
    <div class="dialogform dialogform-horizontal">
        <div class="formrow">
            <h3 class="label"><label for="begindate">起始时间</label></h3>
            <div class="form-enter">
                <span class="datepicker">
                    <input type="text" class="text" name="begindate" id="begindate" />
                    <a title="选择日期" id="A0" href="javascript:void(0)">选择日期</a>
                </span>
            </div>
        </div>
        <div class="formrow">
            <h3 class="label"><label for="enddate">过期时间</label></h3>
            <div class="form-enter">
                <span class="datepicker">
                    <input type="text" class="text" name="enddate" id="enddate" />
                    <a title="选择日期" id="A1" href="javascript:void(0)">选择日期</a>
                </span>
            </div>
        </div>
        <div class="formrow">
            <h3 class="label"><label for="usernames">用户名</label></h3>
            <div class="form-enter">
                <textarea cols="50" rows="10" name="usernames" id="usernames"></textarea>
            </div>
            <div class="form-note">用户名每行填写一个.</div>
        </div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" name="addmember" accesskey="y" title="确认"><span>确认(<u>Y</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<script type="text/javascript" src="$root/max-assets/javascript/max-admin.js"></script>
<script type="text/javascript">
    initDatePicker('begindate', 'A0');
    initDatePicker('enddate', 'A1');
</script>
<!--[if $AllSuccess]-->
<script type="text/javascript">
    parent.location.replace(parent.location.href);
    dialog.close();

</script>
<!--[/if]-->
<!--[/place]-->
<!--[/dialogmaster]-->