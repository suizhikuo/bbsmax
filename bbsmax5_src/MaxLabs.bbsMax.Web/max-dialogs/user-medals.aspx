<!--[DialogMaster title="用户图标" width="700"]-->
<!--[place id="body"]-->
<script type="text/javascript">
function deleteE(id)
{
    removeElement($('item_'+id));
}
</script>
<!--#include file="_error_.ascx" -->
<!--[success]-->
<div class="dialogmsg dialogmsg-success">操作成功.</div>
<!--[/success]-->

<!--#include file="_tab_userinfo_.aspx" tab="medal" -->

<form action="$_form.action" method="post">
<div class="clearfix dialogbody">
    <div class="datatablewrap" style="height:260px;">
    <table class="datatable">
        <thead>
            <tr>
                <th>
                    <input type="checkbox" class="checkbox" id="selectall" />
                    <script type="text/javascript">
                        new checkboxList('medalIDs', 'selectall');
                    </script>
                </th>
                <th>图标名称</th>
                <th>管理员点亮</th>
                <th>实际达到点亮条件</th>
                <th>图标链接</th>
                <th>过期时间</th>
                <th>添加时间</th>
                <th>操作</th>
            </tr>
        </thead>
        <tbody>
        <!--[loop $userMedal in $UserMedalList with $i]-->
            <!--[error line="$i"]-->
            <tr>
                <td class="datatable-errorrow" colspan="8">
                    $Messages
                    <!--[if $HasError("enddates")]--><!--[/if]-->
                </td>
            </tr>
            <!--[/error]-->
            <tr id="item_$userMedal.MedalID">
                <td>
                    <!--[if $IsAutoGet($userMedal) == false]-->
                    <input type="checkbox" name="medalIDs" value="$userMedal.MedalID" />
                    <!--[else]-->
                    <input type="checkbox" name="medalIDs" value="" disabled="disabled" />
                    <!--[/if]-->
                    <input type="hidden" name="medalIDs2" value="$userMedal.MedalID" />
                </td>
                <td>$GetMedalName($userMedal)</td>
                <td>
                    <!--[if $IsAutoGet($userMedal)]-->否<!--[else]-->是<!--[/if]-->
                </td>
                <td>
                    <!--[if $getMedal($userMedal.MedalID).IsCustom]-->-<!--[else if $IsAutoGet($userMedal)]-->是<!--[else]-->否<!--[/if]-->
                </td>
                <td>
                    <input class="text" type="text" name="url_{=$userMedal.MedalID}" id="url_{=$userMedal.MedalID}" value="$_form.text('url_{=$userMedal.MedalID}','$userMedal.Url')" />
                </td>
                <td>
                    <!--[if $IsAutoGet($userMedal)]-->
                    -
                    <!--[else]-->
                    <span class="datepicker">
                        <input class="text" type="text" name="enddate_{=$userMedal.MedalID}" id="enddate_{=$userMedal.MedalID}" value="$_form.text('enddate_{=$userMedal.MedalID}',$outputDateTime($UserMedal.EndDate,"",""))" />
                        <a id="dts_enddate_{=$userMedal.MedalID}" href="javascript:void(0);">日期</a>
                    </span>
                    <script type="text/javascript">
                        initDatePicker('enddate_{=$userMedal.MedalID}','dts_enddate_{=$userMedal.MedalID}');
                    </script>
                    <!--[/if]-->
                    
                    <!--[error line="$i"]-->
                    <!--[if $HasError("enddates")]--><!--[/if]-->
                    <!--[/error]-->
                </td>
                <td>
                    $outputdatetime($UserMedal.CreateDate)
                </td>
                <td><a href="$dialog/usermedal-delete.aspx?medalID=$usermedal.MedalID&userid=$userID" onclick="return openDialog(this.href,this,deleteE);">删除</a></td>
            </tr>
        <!--[/loop]-->
        </tbody>
    </table>
    </div>
    <div class="clearfix dialogfluidform">
        <div class="formrow">
            <h3 class="label"><label for="userMedal">图标</label></h3>
            <div class="form-enter">
                <select id="userMedal" name="userMedal">
                    <option value="">请选择图标</option>
                    <!--[loop $medal in $MedalList]-->
                    <!--[loop $medalLevel in $Medal.Levels]-->
                    <option value="{=$medal.ID}_$medalLevel.ID" $_form.selected("medal","{=$medal.ID}_$medalLevel.ID") >{=$medal.Name} - $medalLevel.Name</option>
                    <!--[/loop]-->
                    <!--[/loop]-->
                </select>
                <!--[error name="userMedal"]-->
                <span class="form-tip tip-error">$message</span>
                <!--[/error]-->
            </div>
        </div>
        <div class="formrow">
            <h3 class="label"><label for="url">图标链接</label></h3>
            <div class="form-enter">
                <input class="text" type="text" name="url" id="url" value="$_form.text('url')" />
            </div>
            <div class="form-note">点击图标时跳转的地址，如不需要请留空</div>
        </div>
        <div class="formrow">
            <h3 class="label"><label for="enddate">过期时间</label></h3>
            <div class="form-enter">
                <span class="datepicker">
                    <input class="text" type="text" name="enddate" id="d_enddate" value="$_form.text('enddate')" />
                    <a id="d_dts_enddate" href="javascript:void(0);">日期</a>
                </span>
                <script type="text/javascript">
                    initDatePicker('d_enddate','d_dts_enddate');
                </script>
                <!--[error name="enddate"]-->
                <span class="form-tip tip-error">$message</span>
                <!--[/error]-->
            </div>
            <div class="form-note">留空为无限期</div>
        </div>
        <div class="formrow formrow-action">
            <button class="button" type="submit" name="addmedal"><span>点亮图标</span></button>
        </div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" accesskey="s" name="save" title="保存"><span>保存(<u>S</u>)</span></button>
    <button class="button button-highlight" type="submit" accesskey="d" name="deleteMedals" title="删除" onclick="if(confirm('确认删除选中的图标吗?')==false)return false;"><span>删除(<u>D</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<!--[/place]-->
<!--[/dialogmaster]-->