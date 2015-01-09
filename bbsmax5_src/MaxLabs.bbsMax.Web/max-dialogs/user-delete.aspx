<!--[DialogMaster title="删除用户 - $User.username" width="400"]-->
<!--[place id="body"]-->
<form action="$_form.action" method="post">
<!--[if CurrentTask != null]-->
<script type="text/javascript">
    var thDialog = currentPanel;
    var task_DoNotReload = true;
    function updateTask(i, p, t) {
        document.getElementById('task_progressing_' + i).style.width = p + '%';
        document.getElementById('task_title_' + i).innerHTML = t;
        task_DoNotReload = false;
    }
    function reloadTask() {
        if (task_DoNotReload != true) {
            task_DoNotReload = true;
            document.getElementById('task_frame_0').contentWindow.location.reload(true);
        }
        setTimeout(reloadTask, 200);
    }
    function finishTask(i, t) {
        task_DoNotReload = true;
        document.getElementById('task_progressing_' + i).style.width = '100%';
        document.getElementById('task_title_' + i).innerHTML = t;
        document.getElementById('clw').disabled = false;

        window.setTimeout(function() { thDialog.close(); }, 5000);
    }
</script>
<div class="clearfix dialogbody">
    <div class="dialogconfirm">
        <h3>正在删除用户 $User.username 请不要关闭窗口....</h3>
        <div class="progressbar" id="task_all_0">
            <div class="progressing" style="width:0%;" id="task_progressing_0"><p>&nbsp;</p></div>
            <div class="progressbar-text" id="task_title_0">$CurrentTask.Title</div>
        </div>
        <iframe id="task_frame_0" style="display:none;" width="0" height="0" frameborder="0" allowtransparency="true"></iframe>
    </div>
</div>
<script type="text/javascript">
$("task_frame_0").src = "$CurrentTask.HandlerUrl&i=0";
reloadTask();
thDialog.result = 1;
</script>
<!--[else]-->
<div class="clearfix dialogbody">
    <div class="dialogconfirm">
        <h3>将删除用户$User.username的所有相关数据, 您确定要这样做吗?</h3>
        <p><input type="checkbox" name="deletepost" id="deletepost" value="True" checked="checked" /> <label for="deletepost">同时删除主题、帖子、附件</label> </p>
    </div>
</div>
<!--[/if]-->
<div class="clearfix dialogfoot">
    <!--[if CurrentTask == null]-->
    <button class="button button-highlight" type="submit" name="delete" accesskey="y" title="确定删除"><span>确定删除(<u>Y</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
    <!--[else]-->
    <button class="button" id="clw" type="submit" name="delete" disabled="disabled" onclick="panel.close();" title="关闭窗口"><span>关闭窗口</span></button>
    <!--[/if]-->
</div>
</form>
<!--[/place]-->
<!--[/dialogmaster]-->