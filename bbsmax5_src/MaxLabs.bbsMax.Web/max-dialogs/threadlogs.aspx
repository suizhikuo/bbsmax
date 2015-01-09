<!--[DialogMaster title="主题操作记录" width="600"]-->
<!--[place id="body"]-->
<div class="clearfix dialogbody">
    <!--[if $threadlogList.Count != 0]-->
    <div class="datatablewrap" style="height:320px;">
        <table class="datatable">
            <thead>
            <tr>
                <td>操作者</td>
                <td>操作</td>
                <td>原因</td>
                <td>时间</td>
            </tr>
            </thead>
            <tbody>
            <!--[loop $log in $threadlogList]-->
            <!--[if $log.IsPublic]-->
            <tr>
                <td><a href="$url(space/$log.UserID)" target="_blank">$log.Username</td>
                <td>$log.ActionTypeName($thread)</td>
                <td>$log.Reason</td>
                <td>$outputFriendlyDateTime($log.CreateDate)</td>
            </tr>
            <!--[/if]-->
            <!--[/loop]-->
            </tbody>
        </table>
    </div>
    <!--[else]-->
    <div class="nodata">
        当前还没有改主题的操作记录.
    </div>
    <!--[/if]-->
</div>
<div class="clearfix dialogfoot">
    <button class="button" type="reset" accesskey="c" title="关闭" onclick="panel.close();"><span>关闭</span></button>
</div>
<!--[/place]-->
<!--[/DialogMaster]--> 