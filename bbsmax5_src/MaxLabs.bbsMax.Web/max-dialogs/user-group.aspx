<!--[DialogMaster title="用户组" width="700"]-->
<!--[place id="body"]-->
<!--#include file="_error_.ascx"-->
<!--[success]-->
<div class="dialogmsg dialogmsg-success">你已成功修改该用户的用户组信息.</div>
<!--[/success]-->

<!--#include file="_tab_userinfo_.aspx" tab="role" -->

<form action="$_form.action" method="post">
<div class="clearfix dialogbody">
    <div class="datatablewrap" style="height:300px;">
    <table class="datatable">
        <thead>
            <tr>
                <th>所属用户组</th>
                <th>开始时间</th>
                <th>结束时间</th>
            </tr>
        </thead>
        <tbody>
            <!--[loop $r in $VirtualRoleList]-->
            <!--[if !$r.IsLevel && $ShowVirtualRole($r.roleID)]-->
            <tr>
                <td>
                    <input type="checkbox" name="groupids" disabled="disabled" value="$r.RoleId" $_form.checked("groupids","$r.RoleId",$InRole($r)) />
                    $r.Name
                </td>
                <td>-</td>
                <td>-</td>
            </tr>
            <!--[/if]-->
            <!--[/loop]-->
            <tr>
                <td>
                    <input type="checkbox" name="groupids" disabled="disabled" value="$LevelRole.RoleId" checked="checked"/>
                    $LevelRole.Name
                </td>
                <td>-</td>
                <td>-</td>
            </tr>
            <!--[loop $r in $RealRoleList with $i]-->
            <tr>
                <td>
                    <input type="checkbox" $_if(!$canchange($r),'disabled="disabled"') id="g_$i" name="groupids" value="$r.RoleId" $_form.checked("groupids","$r.RoleId",$InRole($r)) />
                    <label for="g_$i">$r.Name</label>
                </td>
                <!--[if $canchange($r)]-->
                <!--[if $InRole($r)]-->
                <td>
                    <span class="datepicker">
                        <input type="text" class="text" value="$_form.text('beginDate$r.roleID',$GetBeginDate($r.roleID).beginDate)" name="beginDate$r.roleID" id="begindate$r.RoleID" />
                        <a title="选择日期" id="c.begindate$r.roleID" href="javascript:void(0)">日期</a>
                    </span>
                </td>
                <td>
                    <span class="datepicker">
                        <input type="text" class="text" value="$_form.text('endDate$r.roleId',$GetEndDate($r.roleID).endDate)" name="endDate$r.roleId" id="enddate$r.RoleID" />
                        <a title="选择日期" id="c.enddate$r.roleID" href="javascript:void(0)">日期</a>
                    </span>
                </td>
                <!--[else]-->
                <td>
                    <span class="datepicker">
                        <input type="text" class="text" value="$_form.text('beginDate$r.RoleID')" name="beginDate$r.RoleID" id="begindate$r.RoleID" />
                        <a title="选择日期" id="c.begindate$r.roleID" href="javascript:void(0)">日期</a>
                    </span>
                </td>
                <td>
                    <span class="datepicker">
                        <input type="text" class="text" value="$_form.text('endDate$r.RoleID')" name="endDate$r.RoleID" id="enddate$r.RoleID" />
                        <a title="选择日期" id="c.enddate$r.roleID" href="javascript:void(0)">日期</a>
                    </span>
                </td>
                <!--[/if]-->
                <!--[else]-->
                <td>-</td>
                <td>-</td>
                <!--[/if]-->
            </tr>
            <!--[/loop]-->
        </tbody>
    </table>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" name="updateusergroup" title="更新用户组"><span>更新用户组</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<script type="text/javascript">
    var arrayD = findElement("input", "enddate");
    for (var i = 0; i < arrayD.length; i++) {
        initDatePicker(arrayD[i],'c.'+arrayD[i].id);
    }
    arrayD = findElement("input", "begindate");

    for (var i = 0; i < arrayD.length; i++) {
        initDatePicker(arrayD[i],'c.'+arrayD[i].id);
    }
</script>
<!--[/place]-->
<!--[/dialogmaster]-->