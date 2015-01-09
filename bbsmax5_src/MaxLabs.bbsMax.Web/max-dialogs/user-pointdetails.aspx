<!--[DialogMaster title="用户积分详细信息" width="400"]-->
<!--[place id="body"]-->
<!--[UserView userid="$_get.id"]-->
<form id="form1" method="post" action="">
<div class="clearfix dialogbody">
    <div class="datatablewrap" style="height:200px;">
        <table class="datatable">
            <thead>
            <tr>
                <th>类型</th>
                <th>分值</th>
            </tr>
            </thead>
            <tbody>
            <!--[loop $point in $PointList]-->
            <tr>
                <th>$point.name</th>
                <td><span class="numeric">$point.value</span></td>
            </tr>
            <!--[/loop]-->
            </tbody>
        </table>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button" type="reset" accesskey="c" title="关闭" onclick="panel.close();"><span>关闭</span></button>
</div>
</form>
<!--[/UserView]-->
<!--[/place]-->
<!--[/dialogmaster]-->