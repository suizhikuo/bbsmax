<!--[DialogMaster title="设置置顶" subTitle="您共选择了 $ThreadList.count 个主题" width="400"]-->
<!--[place id="body"]-->
<!--[include src="_error_.ascx" /]-->
<form method="post" action="$_form.action">
<div class="clearfix dialogbody">
    <div class="dialogform dialogform-horizontal">
        <div class="datatablewrap" style="height:100px;">
            <table class="datatable">
                <thead>
                    <tr>
                        <th>主题</th>
                        <th>作者</th>
                    </tr>
                </thead>
                <tbody>
                <!--[loop $thread in $ThreadList with $i]-->
                    <tr $_if($i%2==0,'class="odd"','class="even"')>
                        <td>$thread.subject</td>
                        <td>$thread.postusername</td>
                    </tr>
                <!--[/loop]-->
                </tbody>
            </table>
        </div>
        <div class="formrow">
            <h3 class="label">操作</h3>
            <div class="form-enter">
                <input type="radio" name="stick" id="stick2" value="2" onclick="setStatus()" $_form.checked("stick","2",$CheckStick) />
                <label for="stick2">版块置顶</label>
                <input type="radio" name="stick" id="stick3" value="3" onclick="setStatus()" $_form.checked("stick","3") />
                <label for="stick3">全局置顶</label>
                <input type="radio" name="stick" id="stick1" value="1" onclick="setStatus()" $_form.checked("stick","1",{=$CheckStick==false}) />
                <label for="stick1">取消置顶</label> 
            </div>
        </div>
        <div id="forums" class="formrow">
            <h3 class="label">在以下版块置顶</h3>
            <div class="form-enter">
                <div class="scroller" style="height:100px;" >
                <!--[loop $forum in $forums with $i]-->
                    <p>
                    $ForumSeparators[$i]
                    <!--[if $forum.ParentID == 0 || $HasStickPermission($forum) == false]-->
                    <input type="checkbox" disabled="disabled" />
                    <!--[else]-->
                    <input type="checkbox" name="forumids" value="$forum.forumid" <!--[if $CurrentForum.ForumID==$forum.forumid]-->checked="checked" disabled="disabled"<!--[else if $IsStickForum($forum.forumid)]-->checked="checked"<!--[/if]--> />
                    <!--[/if]-->
                    $forum.ForumName
                    </p>
                <!--[/loop]-->
                </div>
            </div>
        </div>
        <div id="times" class="formrow">
            <h3 class="label">置顶时间</h3>
            <div class="form-enter">
                <input class="text number" onkeyup="value=value.replace(/[^\d]/g,'');" type="text" name="time" id="locktime" value="" />
                <select name="locktimetype" id="locktimetype">
                    <option value="1">小时</option>
                    <option value="2">分钟</option>
                    <option value="0">天</option>
                </select>
            </div>
            <div class="form-note">该时间过后，将自动取消置顶，为空或者0则不自动取消，精确到3分钟.</div>
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
                <input type="text" class="text" name="actionReasonText" />
            </div>
        </div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <input type="hidden" value="$_form.text('threadids')" name="threadids" />
    <button class="button button-highlight" type="submit" name="ok" accesskey="y" title="确认"><span>确认(<u>Y</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<script type="text/javascript">
   
    function setStatus() {
        if ($('stick1').checked) {
            $('forums').style.display = 'none';
            $('times').style.display = 'none';
        } else{ 
            $('forums').style.display = '';
            $('times').style.display = ''; 
        }
    }
    setStatus();
</script>
<!--[/place]-->
<!--[/DialogMaster]-->