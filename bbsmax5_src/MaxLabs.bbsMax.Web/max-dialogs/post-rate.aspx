<!--[DialogMaster title="帖子评分" width="500" ]-->
<!--[place id="body"]-->
<form action="$_form.action" method="post">
<!--[include src="_error_.ascx" /]-->
<div class="clearfix dialogbody">
    <div class="dialogform dialogform-horizontal">
        <div class="formrow">
            <h3 class="label">帖子</h3>
            <div class="form-enter">
                <!--[if $Post.subject != ""]-->
                $Post.subject
                <!--[else]-->
                楼层: $PostAlias
                <!--[/if]-->
                (作者: $Post.Username)
            </div>
        </div>
        <div class="scroller" style="height:150px;">
            <!--[loop $rateSetItem in $RateSetList]-->
            <div class="formrow">
                <h3 class="label">$rateSetItem.UserPoint.Name</h3>
                <div class="form-enter">
                    <input name="point_$rateSetItem.PointType" type="text" class="text number" value="$_form.Text('point_$rateSetItem.PointType')" />
                    <!--[error name="point"]-->
                    <span class="form-tip tip-error">$message</span>
                    <!--[/error]-->
                </div>
                <div class="form-note">
                    (范围: $rateSetItem.MinValue~$rateSetItem.MaxValue, 今天还可评分数:$GetTodayValue($rateSetItem), 1天内允许最大评分数:$rateSetItem.MaxValueInTime.)
                </div>
            </div>
            <!--[/loop]-->
        </div>

        <div class="formrow">
            <h3 class="label"><label for="actionReasonSelect">操作理由</label></h3>
            <div class="form-enter">
                <select name="actionReasonSelect" id="actionReasonSelect" onchange="document.getElementsByName('actionReasonText')[0].value=this.value;">
                <option value="">自定义</option>
                <option value="灌水">灌水</option>
                <option value="广告">广告</option>
                <option value="奖励">奖励</option>
                <option value="惩罚">惩罚</option>
                <option value="好文章">好文章</option>
                <option value="内容不符">内容不符</option>
                <option value="重复发帖">重复发帖</option>
                </select>
                <input type="text" class="text longtext" name="actionReasonText" />
            </div>
            <div class="form-enter">
                <input name="cbsendMessage" type="checkbox" id="cbsendMessage" checked="checked" value="1" />
                <label for="cbsendMessage">通知作者</label>
            </div>
        </div>
    </div>
</div>

<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" name="ratepost" accesskey="y" title="确认"><span>确认(<u>Y</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<!--[/place]-->
<!--[/dialogmaster]-->
