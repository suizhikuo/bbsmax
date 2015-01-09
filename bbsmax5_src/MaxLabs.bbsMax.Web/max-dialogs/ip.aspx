<!--[DialogMaster title="IP地址查询" width="500"]-->
<!--[place id="body"]-->
<form id="form1" action="$_form.action" method="post">
<div class="clearfix dialogbody">
    <div class="dialogform dialogform-horizontal">
        <div class="formrow">
            <h3 class="label">IP</h3>
            <div class="form-enter">
                 $IP (来自: $IPArea)
            </div>
        </div>
    </div>
    <!--[if $IPCollection.count > 0]-->
    <h3 style="margin-bottom:10px;">使用过该IP的用户</h3>
    <div class="scroller" style="height:240px;">
        <table class="datatable">
            <thead>
                <tr>
                    <th><input type="checkbox" id="selectall" /><label for="selectall">全选</label> </th>
                    <th>用户</th>
                    <th>登录时间</th>
                    <th>状态</th>
                    <th>操作</th>
                </tr>
            </thead>
            <tbody>
            <!--[loop $IP in $IPCollection with $i]-->
                <tr $_if($i%2==0,'class="odd"','class="even"')>
                    <td><input type="checkbox" $_If( $ip.user.IsDeleted, 'disabled="disabled"') name="userids" id="userid$IP.UserID" value="$IP.UserID" /></td>
                    <td>$IP.Username $_If($ip.user.IsDeleted, '<font color="red">(已删除)</font>')</td>
                    <td>$outputdatetime($IP.CreateDate)</td>
                    <!--[if $ip.user.IsDeleted]-->
                    <td>-</td>
                    <td>-</td>
                    <!--[else]-->
                    <td>
                    <!--[if $IP.BannedForumID==0]-->全站屏蔽<!--[else if $IP.BannedForumID==null]-->未屏蔽<!--[else]-->版块屏蔽<!--[/if]-->
                    <a href="$dialog/user-shield.aspx?userid=$IP.UserID" onclick="return openDialog(this.href,refresh)">[修改]</a>
                    </td>
                    <td><a href="$dialog/user-edit.aspx?id=$IP.UserID"  onclick="return openDialog(this.href);">[编辑资料]</a></td>
                    <!--[/if]-->
                </tr>
            <!--[/loop]-->
            </tbody>
        </table>
    </div>
    <!--[pager name="list" skin="_pager.aspx"]-->
    <!--[else]-->
    <div class="nodata">没用相同IP的用户.</div>
    <!--[/if]-->
</div>
<div class="clearfix dialogfoot">
    <!--[if $CanShiled]-->
    <button class="button button-highlight" type="submit" name="shieldall" title="全站屏蔽选中用户"><span>全站屏蔽选中用户</span></button>
    <!--[if $HadShiled == false]-->
    <button class="button button-highlight" type="submit" name="shieldip" title="屏蔽该IP"><span>屏蔽该IP</span></button>
    <!--[else]-->
    <button class="button button-highlight" type="submit" name="unshieldip" accesskey="y" title="解除屏蔽该IP"><span>解除屏蔽该IP</span></button>
    <!--[/if]-->
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
    <!--[else]-->
    <button class="button" type="reset" accesskey="c" title="关闭" onclick="panel.close();"><span>关闭(<u>C</u>)</span></button>
    <!--[/if]-->
</div>
</form>
<script type="text/javascript">
    var list = new checkboxList("userids", "selectall");
</script>
<!--[/place]-->
<!--[/DialogMaster]-->
