<script type="text/javascript">
function load(id)
{
    if(document.getElementById('display_'+id).value=='1')
    {
        display(id,true);
    }
    else
    {
        display(id,false);
    }
}
function display(id,isDisplay)
{
    if(isDisplay)
    { 
        if($("custom")!=null && $("custom").checked == false)
            return;
        document.getElementById(id).style.display='';
        document.getElementById('display_'+id).value='1';
        document.getElementById('except_'+id).style.display='none';
    }
    else
    {
        document.getElementById(id).style.display='none';
        document.getElementById('display_'+id).value='0';
        document.getElementById('except_'+id).style.display='';
    }
}
function cancle(trId,selectName)
{
    display(trId,false);
    document.getElementsByName(selectName)[0].value = '';
    var error = document.getElementById('error_'+trId);
    if(error!=null)
        error.style.display='none';
}
</script>
<script type="text/javascript">
function enableControl( container, enable)
{   var childs;
    if( typeof( container)=="string")
        childs=$(container).childNodes;
    else
        childs=container.childNodes;
        
    if(childs.length)
    {   
        for(var i=0;i<childs.length;i++)
        {

            var tagName=childs[i].nodeName.toLowerCase();
            if(tagName=="input"||tagName=="select" || tagName=="textarea" )
            {
            
                if(childs[i].name.toLowerCase().indexOf('maxvalueintime_')==-1)
                    childs[i].disabled=!enable;
               
            }
            else
            {
                enableControl( childs[i], enable);
            }
        }
    }
}
</script>
    <!--[include src="_setting_msg_.aspx"/]-->
    <!--[if $success]-->
    <div class="Tip Tip-success">操作成功</div>
    <!--[/if]-->
    <div class="FormTable">
        <!--[if $ForumID==0]-->
        <div class="minitip minitip-alert">
            当前设置的是全局帖子评分，如果需要具体设置某个版块帖子评分请编辑具体版块.
            <!--[if $IsForumPage == false]-->
            <a href="$admin/bbs/manage-forum.aspx">点此处进入版块列表</a>
            <!--[/if]-->
        </div>
        <!--[else]-->
        <table style="margin-bottom:1px;">
            <tr class="nohover">
                <th>
                    <h4>当前版块帖子评分是</h4>
                    <p>
                    <input type="radio" id="custom" name="inheritType" value="False" onclick="inheritChange()" $_form.Checked("inheritType","False","{=$CurrentRateSet.NodeID==$ForumID}") />
                    <label for="custom">自定义</label>
                    </p>
                    <p>
                    <input type="radio" id="inherit" name="inheritType" value="True" onclick="inheritChange()"  $_form.Checked("inheritType","True","{=$CurrentRateSet.NodeID!=$ForumID}") />
                    <label for="inherit">继承上级</label>
                    <!--[if $CurrentRateSet.NodeID!=$ForumID]-->
                        <!--[if $CurrentRateSet.NodeID == 0]-->
                            (继承至全局 <a href="$admin/bbs/manage-forum-detail.aspx?action=editrate&forumid=0">编辑</a>)
                        <!--[else]-->
                            (继承至版块：$ParentForum.ForumName <a href="$admin/bbs/manage-forum-detail.aspx?action=editrate&forumid=$ForumID">编辑</a>)
                        <!--[/if]-->
                    <!--[/if]-->
                    </p>
                </th>
                <td>
                    如果是继承至上级，你将不能进行编辑当前的版块帖子评分设置，如果要编辑请选择自定义并保存或者编辑所继承版块的设置.
                </td>
            </tr>
        </table>
        <!--[/if]-->
        <div class="Help">
            <p>请填写整数或者负数，最大值必须大等于最小值，每天最大评分数必须为不小于最大值的绝对值和最小值的绝对值(每天最大评分数只能在<a href="$admin/bbs/manage-forum-detail.aspx?action=editrate&forumid=0">全局设置</a>中修改) </p>
            <p>例外是指当用户隶属于例外用户组的时候，该用户的评分操作将按例外设置，如果用户同时隶属于多个例外用户组，将按优先级（即排序值越大优先级越高）最高的例外设置</p>
        </div>
        <table id="p1" class="multiColumns" style="margin-bottom:1px;">
        <thead>
            <tr>
                <td style="width:12em;">积分类型</td>
                <td>
                    <table style="width:auto;">
                    <tr>
                        <td style="width:6em;">评分最小值</td>
                        <td style="width:6em;">评分最大值</td>
                        <td style="width:9em;">1天内最大评分数</td>
                        <td style="width:1em;">&nbsp;</td>
                        <td style="width:4em;font-weight:normal;">排序</td>
                        <td style="width:3em;">&nbsp;</td>
                    </tr>
                    </table>
                </td>
            </tr>
        </thead>
        <tbody>
        <!--[loop $point in $EnabledUserPointList]-->
            <!--[loop $RateItem in $GetRateSetItems($point.Type) with $index]-->
            <!--[error line="$index" name = "$point.Type.ToString()"]-->
                <!--[include src="_error_.aspx"/]-->
            <!--[/error]-->
            <!--[if $index==0]-->
            <tr>
                <td>
                    <strong>$point.Name</strong>
                    <a class="addexceptrule" href="#" id="except_tr_$point.Type" onclick="display('tr_$point.Type',true); return false;">添加例外</a>
                    <input type="hidden" value="$index" name="id_$point.Type" />
                </td>
                <td>
                    <table style="width:auto;">
                    <tr>
                        <td style="width:6em;"><input type="text" class="text" style="width:90%;" name="MinValue_{=$point.Type}_$index" value="$_form.text('MinValue_{=$point.Type}_$index',$RateItem.MinValue)" /></td>
                        <td style="width:6em;"><input type="text" class="text" style="width:90%;" name="MaxValue_{=$point.Type}_$index" value="$_form.text('MaxValue_{=$point.Type}_$index',$RateItem.MaxValue)" /></td>
                        <td style="width:9em;">
                        <input type="text" class="text" style="width:90%;" name="MaxValueInTime_{=$point.Type}_$index" value="$_form.text('MaxValueInTime_{=$point.Type}_$index',$RateItem.MaxValueInTime)" <!--[if $forumID!=0]-->disabled="disabled"<!--[/if]--> />
                        </td>
                        <td style="width:1em;">&nbsp;</td>
                        <td style="width:4em;">&nbsp;</td>
                        <td style="width:3em;">&nbsp;</td>
                    </tr>
                    </table>
                </td>
            </tr>
            <!--[error name = "new_$point.Type"]-->
            <tr class="nohover" id="error_tr_$point.Type">
                <td colspan="2" class="Message">
                    <div class="Tip Tip-error">$Message</div>
                    <div class="TipArrow">&nbsp;</div>
                </td>
            </tr>
            <!--[/error]-->
            <tr id="tr_$point.Type">
                <td>
                    <div style="padding-left:2em;">
                    <select name="new_role_$point.Type" style="width:8em;">
                    <option value="">请选择用户组</option>
                    <!--[loop $role in $RoleList]-->
                    <option value="$role.RoleId" $_form.selected('new_role_$point.Type','$role.RoleId')>$role.name</option>
                    <!--[/loop]-->
                    </select>
                    <input type="hidden" id="display_tr_$point.Type" name="display_tr_$point.Type" value="$_form.text('display_tr_$point.Type','0')" />
                    </div>
                </td>
                <td>
                    <table style="width:auto;">
                    <tr>
                        <td style="width:6em;"><input type="text" class="text" style="width:90%;" name="new_minvalue_$point.Type" value="$_form.text('new_minvalue_$point.Type')" /></td>
                        <td style="width:6em;"><input type="text" class="text" style="width:90%;" name="new_maxvalue_$point.Type" value="$_form.text('new_maxvalue_$point.Type')" /></td>
                        <td style="width:9em;"><input type="text" class="text" style="width:90%;" name="new_maxvalueintime_$point.Type" value="$_form.text('new_maxvalueintime_$point.Type')"  <!--[if $forumID!=0]--> disabled="disabled"<!--[/if]-->/></td>
                        <td style="width:1em;">&nbsp;</td>
                        <td style="width:4em;"><input type="text" class="text" style="width:90%;" name="new_sortorder_$point.Type" value="$_form.text('new_sortorder_$point.Type')" /></td>
                        <td style="width:3em;">
                            <a href="#" onclick="cancle('tr_$point.Type','new_role_$point.Type');return false;">取消</a>
                        </td>
                    </tr>
                    </table>
                </td>
            </tr>
            <tr style="display:none;">
                <td colspan="2">
                <script type="text/javascript">
                    load('tr_$point.Type');
                </script>
                </td>
            </tr>
            <!--[else]-->
            <tr>
                <td>
                    <div style="padding-left:2em;color:#666;">
                    $GetRoleName($RateItem.RoleId)
                    <input type="hidden" name="role_{=$point.Type}_$index" value="$RateItem.RoleId" />
                    <input type="hidden" value="$index" name="id_$point.Type" />
                    </div>
                </td>
                <td>
                    <table style="width:auto;">
                    <tr>
                        <td style="width:6em;"><input type="text" class="text" style="width:90%;" name="minvalue_{=$point.Type}_$index" value="$_form.text('minvalue_{=$point.Type}_$index',$RateItem.MinValue)" /></td>
                        <td style="width:6em;"><input type="text" class="text" style="width:90%;" name="maxvalue_{=$point.Type}_$index" value="$_form.text('maxvalue_{=$point.Type}_$index',$RateItem.MaxValue)" /></td>
                        <td style="width:9em;"><input type="text" class="text" style="width:90%;" name="maxvalueintime_{=$point.Type}_$index" value="$_form.text('maxvalueintime_{=$point.Type}_$index',$RateItem.MaxValueInTime)"  <!--[if $forumID!=0]--> disabled="disabled"<!--[/if]--> /></td>
                        <td style="width:1em;">&nbsp;</td>
                        <td style="width:4em;"><input type="text" class="text" style="width:90%;" name="sortorder_{=$point.Type}_$index" value="$_form.text('sortorder_{=$point.Type}_$index',$RateItem.RoleSortOrder)" /></td>
                        <td style="width:3em;">
                            <a href="?$params&paction=delete&nodeID=$ForumID&pointtype=$point.Type&sortorder=$RateItem.RoleSortOrder">删除</a>
                        </td>
                    </tr>
                    </table>
                </td>
            </tr>
            <!--[/if]-->
            <!--[/loop]-->
        <!--[/loop]-->
        </tbody>
        </table>
        <table class="multiColumns">
            <tr class="nohover">
                <td style="width:12em;">&nbsp;</td>
                <td><input type="submit" value="保存设置" class="button" name="savesetting" /></td>
            </tr>
        </table>
    </div>
</div>
<!--[if $ForumID!=0]-->
<script type="text/javascript">
inheritChange();
function inheritChange()
{
    if($("custom").checked)
    {
        enableControl("p1",true);
    }
    else
    {
        enableControl("p1",false);
    }
}
</script>
<!--[/if]-->
