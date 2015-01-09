<!--[DialogMaster title="$poll.SubjectText (投票详细情况)" width="400"]-->
<!--[place id="body"]-->
<div class="clearfix dialogbody">
    <div class="dialogform">
        <div class="scroller">
        <!--[loop $pollItem in $poll.PollItems with $loopIndex]-->
        <div class="formrow">
            <h3 class="label">{=$LoopIndex+1},$pollItem.ItemName <span class="form-note">票数: $pollItem.PollItemCount ({=$GetPercent($pollItem.PollItemCount,$VoteTotalCount)}%)</span></h3>
            <div class="form-enter">
                <!--[loop $detail in $GetPollItemDetails($pollItem.ItemID)]-->
                <a href="$url(space/$detail.userID)">$detail.Nickname</a>,
                <!--[/loop]-->
            </div>
        </div>
        <!--[/loop]-->
        </div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button" type="reset" accesskey="c" title="关闭" onclick="panel.close();"><span>关闭(<u>C</u>)</span></button>
</div>
<!--[/place]-->
<!--[/DialogMaster]-->