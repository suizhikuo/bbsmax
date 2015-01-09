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
    <!--[if $ActionList.Count > 0]-->
        <!--[if $PointActionType.Type=="ForumPointAction"]-->
        <!--[if $nodeID==0]-->
        <div class="minitip minitip-alert">
            当前设置的是版块的全局积分，如果需要具体设置某个版块的积分请编辑具体版块.
             <!--[if $IsForumPage == false]-->
            <a href="$admin/bbs/manage-forum.aspx">点此处进入版块列表</a>
            <!--[/if]-->
        </div>
        <!--[else]-->
        <table style="margin-bottom:1px;">
        <tr class="nohover">
            <th>
                <h4>当前版块积分是</h4>
                <p>
                <input type="radio" id="custom" name="inheritType" value="False" onclick="inheritChange()" $_form.Checked("inheritType","False","{=$NodeItem.NodeID==$NodeID}") />
                <label for="custom">自定义</label>
                </p>
                <p>
                <input type="radio" id="inherit" name="inheritType" value="True" onclick="inheritChange()"  $_form.Checked("inheritType","True","{=$NodeItem.NodeID!=$NodeID}") />
                <label for="inherit">继承上级</label>
                <!--[if $NodeItem.NodeID!=$NodeID]-->
                    <!--[if $NodeItem.NodeID == 0]-->
                        (继承至版块全局积分 <a href="$admin/bbs/manage-forum-detail.aspx?action=editpoint&forumid=0">编辑</a>)
                    <!--[else]-->
                        (继承至版块：$NodeItem.Name <a href="$admin/bbs/manage-forum-detail.aspx?action=editpoint&forumid=$NodeItem.NodeID">编辑</a>)
                    <!--[/if]-->
                <!--[/if]-->
                </p>
            </th>
            <td>
                如果是继承至上级，你将不能进行编辑当前版块的积分和删除例外，如果要编辑删除请选择自定义并保存(保存之后才能进行删除操作)或者编辑所继承版块的积分.
            </td>
        </tr>
        </table>
        <!--[/if]-->
        <!--[/if]-->
    
        <div class="Help">
            <p>以下动作中, 如需要操作积分, 请在相应积分中填上值, 如不需要操作请留空或为0, 扣除积分请填写负数, 如: "-2".</P>
            <p>例外是指当用户隶属于例外用户组的时候，对该用户的积分操作将按例外设置来操作，如果用户同时隶属于多个例外用户组，将按优先级（即排序值越小优先级越高）最高的例外设置来操作积分</p>
        </div>
        
    <table id="p1" class="multiColumns" style="margin-bottom:1px;">
    <thead>
        <tr class="nohover">
            <td style="width:15em;">&nbsp;</td>
            <td>
                <table style="width:auto;">
                <tr>
                <!--[EnabledUserPointList]-->
                    <td style="width:4em;">$Name</td>
                <!--[/EnabledUserPointList]-->
                    <td style="width:1em;">&nbsp;</td>
                    <td style="width:4em;font-weight:normal;">排序</td>
                    <td style="width:3em;">&nbsp;</td>
                </tr>
                </table>
            </td>
        </tr>
    </thead>
    <tbody>
    <!--[loop $action in $ActionList]-->
        <!--[loop $pointActionItem in $GetPointActionItems($action) with $index]-->
        <!--[error line="$index" name = "$action"]-->
            <!--[include src="_error_.aspx"/]-->
        <!--[/error]-->
        <!--[if $index==0]-->
        <tr>
            <td>
                <div>
                <strong>$GetActionName($action)</strong>
                <a class="addexceptrule" href="#" id="except_tr_$action" onclick="display('tr_$action',true); return false;">添加例外</a>
                <input type="hidden" value="$index" name="id.$action" />
                </div>
            </td>
            <td>
                <table style="width:auto;">
                <tr>
                <!--[EnabledUserPointList]-->
                    <td style="width:4em;"><input type="text" class="text" style="width:90%;" name="pointaction.$action.$pointID.$index" value="$_form.text('pointaction.$action.$pointID.$index',$pointActionItem.Points[$pointID])" /></td>
                <!--[/EnabledUserPointList]-->
                    <td style="width:1em;">&nbsp;</td>
                    <td style="width:4em;">&nbsp;</td>
                    <td style="width:3em;">&nbsp;</td>
                </tr>
                </table>
            </td>
        </tr>
        <!--[error name = "new.$action"]-->
        <tr class="nohover" id="error_tr_$action">
            <td colspan="2" class="Message">
                <div class="Tip Tip-error">$Message</div>
                <div class="TipArrow">&nbsp;</div>
            </td>
        </tr>
        <!--[/error]-->
        <tr id="tr_$action">
            <td>
                <div style="padding-left:2em;">
                <select name="new.role.$action" style="width:8em;">
                <option value="">请选择用户组</option>
                <!--[loop $role in $RoleList]-->
                <option value="$role.RoleId" $_form.selected('new.role.$action','$role.RoleId')>$role.name</option>
                <!--[/loop]-->
                </select>
                <input type="hidden" id="display_tr_$action" name="display.tr.$action" value="$_form.text('display.tr.$action','0')" />
                </div>
            </td>
            <td>
                <table style="width:auto;">
                <tr>
                <!--[EnabledUserPointList]-->
                    <td style="width:4em;"><input type="text" class="text" style="width:90%;" name="new.pointaction.$action.$pointID" value="$_form.text('new.pointaction.$action.$pointID')" /></td>
                <!--[/EnabledUserPointList]-->
                    <td style="width:1em;">&nbsp;</td>
                    <td style="width:4em;"><input type="text" class="text" style="width:90%;" name="new.sortorder.$action" value="$_form.text('new.sortorder.$action')" /></td>
                    <td style="width:3em;text-align:center;">
                        <a href="#" onclick="cancle('tr_$action','new.role.$action');return false;">取消</a>
                    </td>
                </tr>
                </table>
            </td>
        </tr>
        <tr style="display:none;">
            <td colspan="2">
            <script type="text/javascript">
                load('tr_$action');
            </script>
            </td>
        </tr>
        <!--[else]-->
        <tr>
            <td>
                <div style="padding-left:2em;color:#666;">
                <p>$GetRoleName($pointActionItem.RoleId)</p>
                <input type="hidden" name="role.$action.$index" value="$pointActionItem.RoleId" />
                <input type="hidden" value="$index" name="id.$action" />
                </div>
            </td>
            <td>
                <table style="width:auto;">
                <tr>
                <!--[EnabledUserPointList]-->
                    <td style="width:4em;"><input type="text" class="text" style="width:90%;" name="pointaction.$action.$pointID.$index" value="$_form.text('pointaction.$action.$pointID.$index',$pointActionItem.Points[$pointID])" /></td>
                <!--[/EnabledUserPointList]-->
                    <td style="width:1em;">&nbsp;</td>
                    <td style="width:4em;"><input type="text" class="text" style="width:90%;" name="sortorder.$action.$index" value="$_form.text('sortorder.$action.$index',$pointActionItem.RoleSortOrder)" /></td>
                    <td style="width:3em;text-align:center;">
                        <a href="?$params&type=$PointActionType.type&paction=delete&actiontype=$action&sortorder=$pointActionItem.RoleSortOrder&nodeID=$NodeID">删除</a>
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
    <!--[/if]-->

    <!--[if $NeedValueActionList.Count > 0]-->
    <div class="Help">
    <p>以下动作中，请选择要操作的积分类型</p>
    <p>最低值是指该交易允许的最小积分值，低于该值交易失败，最低值必须大等于0，不填默认为1</p>
    <p>最高值是指该交易允许的最大积分值，高于该值交易失败，最高值不能小于最低值，不填则没有限制</p>
    <p>最低余额是指交易后该积分必须剩余的值，如果剩余值小于最低余额则交易失败，不填则为该积分的下限</p>
    <p>例外是指当用户隶属于例外用户组的时候，对该用户的积分操作将按例外设置来操作，如果用户同时隶属于多个例外用户组，将按优先级（即排序值越小优先级越高）最高的例外设置来操作积分</p>
    </div>
    <table id="p2" class="multiColumns" style="margin-bottom:1px;">
    <!--[loop $action in $NeedValueActionList]-->
    <!--[loop $pointActionItem in $GetPointActionItems($action) with $index]-->
    <!--[error line="$index" name = "pointtype.$action"]-->
    <tr class="nohover">
        <td class="Message">
            <div class="Tip Tip-error">$Message</div>
            <div class="TipArrow">&nbsp;</div>
        </td>
    </tr>
    <!--[/error]-->
    <!--[if $index==0]-->
    <tr>
        <td>
            <div><strong>$GetNeedValueActionName($action)</strong>
            <a href="#" id="except_tr_pointtype_$action" onclick="display('tr_pointtype_$action',true);return false;">添加例外</a>
            </div>
            <table style="width:auto;">
            <thead>
            <tr>
                <td style="width:14em;">&nbsp;</td>
                <td style="width:6em;">积分类型</td>
                <td style="width:4em;">最低值</td>
                <td style="width:4em;">最高值</td>
                <td style="width:5em;">最低余额</td>
                <td style="width:2em;">&nbsp;</td>
                <td style="width:4em;font-weight:normal;">排序</td>
                <td style="width:3em;">&nbsp;</td>
            </tr>
            </thead>
            <tbody>
            <tr>
                <td style="width:14em;">所有用户组</td>
                <td style="width:6em;">
                    <select name="pointaction.$action.$index" style="width:90%;">
                    <!--[EnabledUserPointList]-->
                    <option value="$type" $_form.selected('pointaction.$action.$index','$type',$pointActionItem.PointType)>$Name</option>
                    <!--[/EnabledUserPointList]-->
                    </select>
                </td>
                <td style="width:4em;"><input name="minvalue.$action.$index" type="text" class="text" style="width:90%;" value="$_form.text('minvalue.$action.$index',$pointActionItem.minValue)" /></td>
                <td style="width:4em;"><input name="maxvalue.$action.$index" type="text" class="text" style="width:90%" value="$_form.text('maxvalue.$action.$index',$out($pointActionItem.displayMaxValue))" /></td>
                <td style="width:5em;"><input name="minremaining.$action.$index" type="text" class="text" style="width:90%;" value="$_form.text('minremaining.$action.$index',$out($pointActionItem.displayMinRemaining))" /></td>
                <td style="width:2em;">&nbsp;</td>
                <td style="width:4em;font-weight:normal;">&nbsp;</td>
                <td style="width:3em;">&nbsp;</td>
            </tr>
            </tbody>
            </table>
        </td>
    </tr>
    <!--[error name="pointtype.new.$action"]-->
    <tr class="nohover" id="error_tr_pointtype_$action">
        <td class="Message">
            <div class="Tip Tip-error">$Message</div>
            <div class="TipArrow">&nbsp;</div>
        </td>
    </tr>
    <!--[/error]-->
    <tr id="tr_pointtype_$action">
        <td>
            <table style="width:auto;">
            <tbody>
            <tr>
                <td style="width:14em;">
                    <div style="padding-left:2em;color:#666;">
                    例外:
                    <select  name="pointtype.new.role.$action" style="width:8em;">
                    <option value="">请选择用户组</option>
                    <!--[loop $role in $RoleList]-->
                    <option value="$role.RoleId" $_form.selected('pointtype.new.role.$action','$role.RoleId')>$role.name</option>
                    <!--[/loop]-->
                    </select>
                    <input type="hidden" value="$index" name="pointtype.id.$action" />
                    <input type="hidden" id="display_tr_pointtype_$action" name="display.tr.pointtype.$action" value="$_form.text('display.tr.pointtype.$action','0')"/>
                    </div>
                </td>
                <td style="width:6em;">
                    <select name="pointaction.new.$action" style="width:90%;">
                    <!--[EnabledUserPointList]-->
                    <option id="pointtype.new.$action.$pointID"value="$type" $_form.selected('pointaction.new.$action',$type,$pointActionItem.PointType)>$Name</option>
                    <!--[/EnabledUserPointList]-->
                    </select>
                </td>
                <td style="width:4em;"><input name="minvalue.new.$action" type="text" class="text" style="width:90%;" value="$_form.text('minvalue.new.$action')" /></td>
                <td style="width:4em;"><input name="maxvalue.new.$action" type="text" class="text" style="width:90%;" value="$_form.text('maxvalue.new.$action')" /></td>
                <td style="width:5em;"><input name="minremaining.new.$action" type="text" class="text" style="width:90%;" value="$_form.text('minremaining.new.$action')" /></td>
                <td style="width:2em;">&nbsp;</td>
                <td style="width:4em;"><input type="text" class="text" style="width:90%;" name="pointtype.new.sortorder.$action" value="$_form.text('pointtype.new.sortorder.$action')" /></td>
                <td style="width:3em;text-align:center;">
                    <a href="#" onclick="cancle('tr_pointtype_$action','pointtype.new.role.$action');return false;">取消</a>
                </td>
            </tr>
            </tbody>
            </table>
        </td>
    </tr>
    <tr style="display:none;">
        <td>
        <script type="text/javascript">
            load('tr_pointtype_$action');
        </script>
        </td>
    </tr>
    <!--[else]-->
    <tr>
        <td>
            <table style="width:auto;">
            <tbody>
            <tr>
            <td style="width:14em;">
                <div style="padding-left:2em;color:#666;">
                例外: $GetRoleName($pointActionItem.RoleId)
                <input type="hidden" name="pointtype.role.$action.$index" value="$pointActionItem.RoleId" />
                <input type="hidden" value="$index" name="pointtype.id.$action" />
                </div>
            </td>
            <td style="width:6em;">
                <select name="pointaction.$action.$index"  style="width:90%;">
                <!--[EnabledUserPointList]-->
                <option value="$type" $_form.selected('pointaction.$action.$index',$type,$pointActionItem.PointType)>$Name</option>
                <!--[/EnabledUserPointList]-->
                </select>
            </td>
            <td style="width:4em;"><input name="minvalue.$action.$index" type="text" class="text" style="width:90%;" value="$_form.text('minvalue.$action.$index',$pointActionItem.minValue)" /></td>
            <td style="width:4em;"><input name="maxvalue.$action.$index" type="text" class="text" style="width:90%;" value="$_form.text('maxvalue.$action.$index',$out($pointActionItem.displayMaxValue))" /></td>
            <td style="width:4em;"><input name="minremaining.$action.$index" type="text" class="text" style="width:90%;" value="$_form.text('minremaining.$action.$index',$out($pointActionItem.displayMinRemaining))" /></td>
            <td style="width:2em;">&nbsp;</td>
            <td style="width:4em;"><input type="text" class="text" style="width:90%;" name="pointtype.sortorder.$action.$index" value="$_form.text('pointtype.sortorder.$action.$index',$pointActionItem.RoleSortOrder)" /></td>
            <td style="width:3em;text-align:center;vertical-align:middle;">
                <a href="?$params&type=$PointActionType.type&paction=delete&actiontype=$action&sortorder=$pointActionItem.RoleSortOrder&nodeID=$NodeID">删除</a>
            </td>
            </tr>
            </tbody>
            </table>
        </td>
    </tr>
    <!--[/if]-->
    <!--[/loop]-->
    <!--[/loop]-->
    </table>
    <!--[/if]-->
    <table class="multiColumns">
        <tr class="nohover">
            <td style="width:12em;">&nbsp;</td>
            <td><input type="submit" value="保存设置" class="button" name="savepointaction" /></td>
        </tr>
    </table>

    </div>
    
<!--[if $PointActionType.Type=="ForumPointAction"]-->
<!--[if $nodeID!=0]-->
<script type="text/javascript">
inheritChange();
function inheritChange()
{
    if($("custom").checked)
    {
        enableControl("p1",true);
        enableControl("p2",true);
    }
    else
    {
        enableControl("p1",false);
        enableControl("p2",false);
    }
}
</script>
<!--[/if]-->
<!--[/if]-->

