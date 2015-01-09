<!--[DialogMaster title="帖子评分记录" width="500"]-->
<!--[place id="body"]-->
<div class="clearfix dialogbody">
    <!--[if $TotalCount != 0]-->
    <div class="datatablewrap" style="height:320px;">
    <table class="datatable">
        <thead>
            <tr>
                <td>评分用户</td>
                <td>分数</td>
                <td>时间</td>
                <td>理由</td>
            </tr>
        </thead>
        <tbody>
        <!--[loop $postMark in $postMarkList]-->
            <tr>
                <td><a href="$url(space/$PostMark.UserID)" target="_blank">$PostMark.Username</td>
                <td>$PostMark.PostMarkExtentedPoints</td>
                <td>$outputFriendlyDateTime($PostMark.CreateDate)</td>
                <td>$PostMark.Reason</td>
            </tr>
        <!--[/loop]-->
        </tbody>
    </table>
    </div>
    <!--[pager name="list" count="$totalcount" pagesize="$pagesize" skin="_pager.aspx"]-->
    <!--[else]-->
    <div class="nodata">
        当前还没有用户对该帖子进行评分.
    </div>
    <!--[/if]-->
</div>
<div class="clearfix dialogfoot">
    <button class="button" type="button" accesskey="c" title="关闭" onclick="panel.close();"><span>关闭(<u>C</u>)</span></button>
</div>
<!--[/place]-->
<!--[/DialogMaster]--> 