<!--[DialogMaster title="撤销评分" width="600"]-->
<!--[place id="body"]-->
<form action="$_form.action" method="post">
<!--[include src="_error_.ascx" /]-->
<div class="clearfix dialogbody">
    <div class="datatablewrap" style="height:300px;">
        <table class="datatable">
            <thead>
            <tr>
                <th>
                    <input type="checkbox" name="sellectAll" id="selectAll" />
                    <label for="selectAll">全选</label>
                </th>
                <th>用户名</th>
                <th>时间</th>
                <th>积分</th>
                <th>理由</th>
            </tr>
            </thead>
            <tbody>
            <!--[loop $PostMark in $PostMarkList]-->
            <tr>
                <td><input type="checkbox" name="postmarkIDs" value="$postMark.PostMarkID" /></td>
                <td><a href="$url(space/$PostMark.UserID)">$PostMark.Username</a></td>
                <td>$outputFriendlyDateTime($PostMark.CreateDate)</td>
                <td>$PostMark.PostMarkExtentedPoints</td>
                <td>$PostMark.Reason</td>
            </tr>
            <!--[/loop]-->
            </tbody>
        </table>
        <script type="text/javascript">
            new checkboxList('postmarkIDs', 'selectAll');
        </script>
    </div>
    <!--[pager name="list" skin="_pager.aspx"]-->
    <div class="dialogform dialogform-horizontal">
        <div class="formrow">
            <h3 class="label"><label for="reason">操作理由</label></h3>
            <div class="form-enter">
                <input type="text" class="text longtext" id="reason" name="reason" />
            </div>
            <div class="form-enter">
                <input type="checkbox" name="sendmessage" id="sendmessage" />
                <label for="sendmessage">发通知给作者</label>
            </div>
        </div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" name="cancelrate" accesskey="y" title="确认"><span>确认(<u>Y</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<!--[/place]-->
<!--[/dialogmaster]-->