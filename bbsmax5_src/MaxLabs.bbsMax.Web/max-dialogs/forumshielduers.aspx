<!--[DialogMaster title="本版屏蔽用户列表" subtitle="$Forum.ForumNameText" width="500"]-->
<!--[place id="body"]-->
<!--[include src="_error_.ascx" /]-->
<!--[success]-->
<div class="dialogmsg dialogmsg-success">成功修改屏蔽状态</div>
<!--[/success]-->

<form action="$_form.action" method="post" id="form2">
<div class="clearfix dialogbody">
    <div class="clearfix dialogfluidform">
        <div class="formrow">
            <h3 class="label">用户名</h3>
            <div class="form-enter">
                <input type="text" class="text username" name="username" value="$_form.text('username')" />
            </div>
        </div>
        <div class="formrow">
            <h3 class="label">解除屏蔽时间</h3>
            <div class="form-enter">
                <span class="datepicker">
                    <input type="text" value="$_form.text('banneddate','无限期')" name="banneddate" id="banneddate" class="text" />
                    <a href="javascript:void(0);" id="A0">日期</a>
                </span>
                <script type="text/javascript">
                    initDatePicker('banneddate', 'A0');
                </script>
            </div>
        </div>
        <div class="formrow formrow-action">
            <button class="button button-highlight" type="submit" name="shielduser" title="在当前位置屏蔽"><span>在当前位置屏蔽</span></button>
        </div>
    </div>

    <!--[if $TotalCount>0]-->
    <div class="scroller">
        <table class="datatable">
            <thead>
                <tr>
                    <td>
                        <input type="checkbox" id="selectAll" />
                        <label for="selectAll">全选</label>
                        <script type="text/javascript">
                            new checkboxList('userids', 'selectAll');
                        </script>
                    </td>
                    <td>用户</td>
                    <td>解除时间</td>
                </tr>
            </thead>
            <tbody>
                <!--[loop $b in $BannedUserList]-->
                <tr>
                    <td>
                        <input type="checkbox" name="userids" value="$b.userid" id="chk$b.userid" />
                    </td>
                    <td><label for="chk$b.userid">$b.user.name</label></td>
                    <td>$outputdate($b.EndDate)</td>
                </tr>
                <!--[/loop]-->
            </tbody>
        </table>
    </div>
    <!--[AdminPager count="$totalcount" PageSize="$PageSize" /]-->
    <!--[else]-->
    <div class="nodata">
        当前版块没有屏蔽任何用户.
    </div>
    <!--[/if]-->
</div>

<div class="clearfix dialogfoot">
    <!--[if $TotalCount>0]-->
    <button class="button button-highlight" id="Button1" type="submit" name="unban" title="将选中的用户解除屏蔽"><span>解除屏蔽选中用户</span></button>
    <!--[/if]-->
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>

</form>
<!--[/place]-->
<!--[/dialogmaster]-->