<!--[DialogMaster title="主题评级用户" width="400"]-->
<!--[place id="body"]-->
<div class="clearfix dialogbody">
    <div class="dialogform dialogform-horizontal">
        <!--[if $TotalCount != 0]-->
        <div class="scroller" style="height:180px;">
            <table class="datatable">
            <tr>
                <td>评级用户</td>
                <td>评级</td>
                <td>时间</td>
            </tr>
            <!--[loop $threadRank in $threadRankList]-->
            <tr>
                <td><a href="$url(space/$threadRank.UserID)" target="_blank">$threadRank.RankUser.Name</td>
                <td>$threadRank.Rank</td>
                <td>$outputdatetime($threadRank.CreateDate)</td>
            </tr>
            <!--[/loop]-->
        </table>
        </div>
        <div class="pagination">
        <!--[pager name="list" skin="_pager.aspx" totalCount="$totalCount"]-->
        </div>
        <!--[else]-->
        <div class="nodata">
            当前还没有用户对该主题进行评级.
        </div>
        <!--[/if]-->
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
<!--[/place]-->
<!--[/DialogMaster]--> 