<!--[DialogMaster title="设置主题分类" subTitle="您共选择了 $ThreadList.count 个主题" width="400"]-->
<!--[place id="body"]-->
<!--[include src="_error_.ascx" /]-->
<form method="post" action="$_form.action">
<div class="clearfix dialogbody">
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
    <div class="dialogform dialogform-horizontal">
        <div class="formrow">
            <h3 class="label"><label for="threadCatalogID">选择分类</label></h3>
            <div class="form-enter">
                <select name="threadCatalogID" id="threadCatalogID" size="1">
                    <!--[if $IsMustSellectThreadCatalog == false]-->
                    <option value="0">不选</option>
                    <!--[/if]-->
                    <!--[loop $c in $Catalogs]-->
                    <option value="$c.ThreadCatalogID">$c.ThreadCatalogName</option>
                    <!--[/loop]-->
                </select>
            </div>
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
            <div class="form-enter">
                <input name="sendNotify" type="checkbox" id="Checkbox1" checked="checked" value="true" />
                <label for="sendNotify">通知作者</label>
            </div>
        </div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <input type="hidden" value="$_form.text('threadids','$threadIDString')" name="threadids" />
    <button class="button button-highlight" type="submit" name="ok" accesskey="y" title="确认"><span>确认(<u>Y</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<!--[/place]-->
<!--[/DialogMaster]-->