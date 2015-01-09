<!--[DialogMaster title="未使用的用户ID" width="400"]-->
<!--[place id="body"]-->
<form action="$_form.action" method="post">
<div class="clearfix dialogbody">
    <div class="dialogform dialogfluidform">
        <div class="formrow">
            <h3 class="label">查询未使用ID</h3>
            <div class="form-enter">
                从
                <input type="text" class="text" id="beginid" name="beginid" style="width:4em;" value="$_form.text('beginid','$BeginUserID')" />
                到
                <input type="text" class="text" id="endid" name="endid" style="width:4em;" value="$_form.text('endid','$EndUserID')" />
                间未使用的ID
            </div>
        </div>
        <div class="formrow formrow-action">
            <button name="search" class="button" type="submit" accesskey="y" onclick=""><span>搜索</span></button>
        </div>
    </div>
    
    <!--[if $HasGet == true]-->
        <!--[if $UserIDs == null]-->
    <div class="successmsg">
    从 $BeginUserID 到 $EndUserID 之间的用户ID全部未被使用
    </div>
        <!--[else if $UserIDs.Count == 0]-->
    <div class="alertmsg">
    从 $BeginUserID 到 $EndUserID 之间的用户ID全部已被使用
    </div>
        <!--[else]-->
    <h3>以下ID未被使用</h3>
    <ul class="clearfix unusedidlist">
        <!--[loop $id in $UserIDs with $i]-->
        <li><a href="#" onclick="document.getElementById('userid').value='$id';">$id</a></li>
        <!--[/loop]-->
    </ul>
    <!--[pager name="list" skin="_pager.aspx"]-->
        <!--[/if]-->
    <!--[/if]-->
</div>

<div class="clearfix dialogfoot">
    <button class="button" type="reset" accesskey="c" title="关闭" onclick="panel.close();"><span>关闭(<u>C</u>)</span></button>
</div>
</form>

<script type="text/javascript">
function showView()
{
    var beginid= $("beginid").value;
    var endid =$("endid").value;
    location.replace( String.format("user-notuseids.aspx?beginid={0}&endid={1}&isdialog=1",beginid,endid))
}
</script>
<!--[/place]-->
<!--[/DialogMaster]-->