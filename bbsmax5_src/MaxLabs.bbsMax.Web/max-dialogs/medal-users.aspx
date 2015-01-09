<!--[DialogMaster title="已点亮图标（$MedalName）的用户" width="600"]-->
<!--[place id="body"]-->
<!--[include src="_error_.ascx" /]-->
<!--[success]-->
<div class="dialogmsg dialogmsg-success">操作成功</div>
<!--[/success]-->

<form id="form1" action="$_form.action" method="post">
<div class="clearfix dialogbody">
    <div class="dialogform dialogfluidform">
        <div class="formrow">
            <h3 class="label"><label for="medal">选择图标</label></h3>
            <div class="form-enter">
                <select id="medal" name="medal" onchange="search2()">
                <!--[loop $medal in $MedalList]-->
                <!--[if $Medal.Levels.Count>1]-->
                <option value="{=$medal.ID}_all" $_form.selected("medal","{=$medal.ID}_all","$ID") >{=$medal.Name} - 所有等级</option>
                <!--[/if]-->
                <!--[loop $medalLevel in $Medal.Levels]-->
                <option value="{=$medal.ID}_$medalLevel.ID" $_form.selected("medal","{=$medal.ID}_$medalLevel.ID","$ID") >{=$medal.Name}<!--[if $medalLevel.Name!=""]--> - $medalLevel.Name<!--[/if]--></option>
                <!--[/loop]-->
                <!--[/loop]-->
                </select>
            </div>
        </div>
        <div class="formrow">
            <h3 class="label"><label for="keyword">用户名:</label></h3>
            <div class="form-enter">
                <input type="text" class="text" id="keyword" name="keyword" onkeydown="if(event.keyCode == 13){search2();return false;}" value="$_form.text('keyword','$keyword')" />
            </div>
        </div>
        <div class="formrow formrow-action">
            <button type="button" class="button" id="s" name="search" onclick="search2()"><span>查找</span></button>
        </div>
    </div>
    
    <!--[if $TotalCount > 0]-->
    <div class="datatablewrap" style="height:150px;">
        <table class="datatable">
            <thead>
                <tr>
                    <th><input type="checkbox" class="checkbox" id="selectall" /></th>
                    <th>等级名称</th>
                    <th>用户名</th>
                    <th>真实姓名</th>
                    <th>管理员点亮</th>
                    <th>实际达到点亮条件</th>
                    <th>图标链接</th>
                    <th>过期时间</th>
                    <th>添加时间</th>
                </tr>
            </thead>
            <tbody>
                <!--[loop $User in $UserList with $i]-->
                <!--[error line="$i"]-->
            <tr class="datatable-rowerror">
                <td colspan="9"><div class="dialogmsg dialogmsg-error">$Messages</div></td>
            </tr>
            <tr class="datatable-rowerrorarrow">
                <td>&nbsp;</td>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
                <td><!--[if $HasError("enddates")]--><div class="errorarrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
                <td>&nbsp;</td>
            </tr>
                <!--[/error]-->
            <tr>
                <td>
                    <!--[if $IsAutoGet($User) == false]-->
                    <input type="hidden" name="userids2" value="$_form.text('userids2',$User.ID)" />
                    <input type="hidden" name="index_$User.ID" value="$_form.text('index_$User.ID',$i)" />
                    <!--[/if]-->
                    <input type="checkbox" name="userids" value="$User.ID" <!--[if $IsAutoGet($User) || $CanManage($User.ID)==false]--> disabled="disabled" <!--[else]--> $_form.checked('userids','$User.ID')<!--[/if]--> />
                </td>
                <td>$GetMedalLevelName($User)</td>
                <td><a href="$url(space/$user.UserID)" target="_blank">$User.UserName</a></td>
                <td>$User.Realname</td>
                <td> 
                    <!--[if $IsAutoGet($User)]-->
                    否
                    <!--[else]-->
                    是
                    <!--[/if]-->
                </td>
                <td>
                    <!--[if $medal.IsCustom]-->
                    --
                    <!--[else if $IsAutoGet($User)]-->
                    是
                    <!--[else]-->
                    否
                    <!--[/if]-->
                </td>
                <td>
                    <input class="text" type="text" name="url_{=$User.ID}" id="url_{=$user.ID}" value="$_form.text('url_{=$User.ID}','$GetUserMedal($user).Url')" <!--[if $CanManage($User.ID)==false]--> disabled="disabled" <!--[/if]--> />
                </td>
                <td>
                    <!--[if $IsAutoGet($User)]-->
                    --
                    <!--[else]-->
                    <!--[if $CanManage($User.ID)]--> 
                    <span class="datepicker">
                        <input class="text" type="text" name="enddate_{=$User.ID}" id="enddate_{=$User.ID}" value="$_form.text('enddate_{=$User.ID}',$outputDateTime($GetUserMedal($user).EndDate,"",""))" />
                        <a title="选择日期" id="dts_enddate_{=$User.ID}" href="javascript:void(0);">日期</a>
                    </span>
                    <script type="text/javascript">
                        initDatePicker('enddate_{=$User.ID}', 'dts_enddate_{=$User.ID}');
                    </script>
                    <!--[else]-->
                    <input type="text" name="enddate_{=$User.ID}" id="enddate_{=$User.ID}" value="$_form.text('enddate_{=$User.ID}',$outputDateTime($GetUserMedal($user).EndDate,"",""))" disabled="disabled" />
                    <!--[/if]-->
                    <!--[/if]-->
                    </td>
                    <td class="file-addtime">
                    <!--[if $IsAutoGet($User)]-->
                    --
                    <!--[else]-->
                        $outputdatetime($GetUserMedal($user).CreateDate)
                    <!--[/if]-->
                    </td>
                </tr>
                <!--[/loop]-->
            </tbody>
        </table>
        <script type="text/javascript">
            new checkboxList( 'userids', 'selectall');
        </script>
    </div>
    <!--[pager count="$TotalCount" pagesize="$PageSize" /]-->
    <!--[else]-->
    <div class="nodata">
        当前没有点亮该图标的用户.
    </div>
    <!--[/if]-->

    <div class="dialogform dialogform-horizontal">
        <div class="formrow">
            <h3 class="label"><label for="usernames">用户名</label></h3>
            <div class="form-enter">
                <input class="text" type="text" name="usernames" id="usernames" value="$_form.text('usernames')" />
                <!--[error name="usernames"]-->
                <span class="form-tip tip-error">$message</span>
                <!--[/error]-->
            </div>
            <div class="form-note">多个用逗号","分隔</div>
        </div>
        <div class="formrow">
            <h3 class="label"><label for="userMedal">图标</label></h3>
            <div class="form-enter">
                <select id="userMedal" name="userMedal">
                    <option value="">请选择图标</option>
                    <!--[loop $medal in $MedalList]-->
                        <!--[loop $medalLevel in $Medal.Levels]-->
                    <option value="{=$medal.ID}_$medalLevel.ID" $_form.selected("medal","{=$medal.ID}_$medalLevel.ID","$ID") >{=$medal.Name} - $medalLevel.Name</option>
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
                    <input class="text" type="text" name="enddate" id="enddate" value="$_form.text('enddate')" />
                    <a title="选择日期" id="dts_enddate" href="javascript:void(0);">日期</a>
                </span>
                <script type="text/javascript">
                    initDatePicker('enddate', 'dts_enddate');
                </script>
                <!--[error name="enddate"]-->
                <span class="form-tip tip-error">$message</span>
                <!--[/error]-->
            </div>
            <div class="form-note">留空为无限期</div>
        </div>
        <div class="formrow formrow-action">
            <button class="button button-highlight" type="submit" name="addmedal" accesskey="y" title="点亮图标"><span>点亮图标(<u>Y</u>)</span></button>
        </div>
    </div>
</div>

<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" name="saveMedals" accesskey="s" title="保存"><span>保存(<u>S</u>)</span></button>
    <button class="button button-highlight" type="submit" name="deleteMedals"><span>取消点亮所选</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>

</form>
<script type="text/javascript">
function search2() {
    var url = '$CurrentUrlBase?id={0}&keyword={1}&isdialog=$_if($isdialog,"1","0")';
    location.replace(String.format(url,$('medal').value,encodeURI($('keyword').value)));
}
</script>
<!--[/place]-->
<!--[/DialogMaster]--> 