<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>道具管理</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx" /]-->
<!--[include src="../_setting_msg_.aspx"/]-->

<div class="Content">
    <form method="post" action="$_form.action">
    <input type="hidden" name="proptype" value="$_form.text('proptype', $_get.proptype)" />
    <h3>道具设置</h3>
    <div class="FormTable">
        <table>
            <!--[error name="Name"]-->
                <!--[include src="../_error_.aspx" /]-->
            <!--[/error]-->
            <tr>
                <th><h4>道具名称</h4>
                <input type="text" class="text" name="name" value="$_form.text('name',$out($prop.Name))" />
                </th>
                <td></td>
            </tr>
            <!--[error name="Description"]-->
                <!--[include src="../_error_.aspx" /]-->
            <!--[/error]-->
            <tr>
                <th><h4>道具描述</h4>
                <input type="text" class="text" name="description" value="$_form.text('description',$out($prop.Description))" />
                </th>
                <td></td>
            </tr>
            <!--[error name="Icon"]-->
                <!--[include src="../_error_.aspx" /]-->
            <!--[/error]-->
		    <tr>
			    <th>
			    <h4>道具图标</h4>
			    <img id="icon_preview" $_if($prop.Icon == null, 'style="display:none"') src="$prop.IconUrl"/><br />
			    <input type="text" class="text" id="Icon" name="Icon" value="$_form.text("Icon",$out($prop.Icon))" />
			    <a title="选择图片" class="selector-image" href="javascript:void(browseImage('Assets_PropIcons','Icon','icon_preview'))">
			        <img src="$Root/max-assets/images/image.gif" alt="" />
			    </a>
			    </th>
			    <td>如果留空，则使用默认猪头图标。</td>
		    </tr>
            <!--[error name="SortOrder"]-->
                <!--[include src="../_error_.aspx" /]-->
            <!--[/error]-->
            <tr>
                <th><h4>道具排序</h4>
                <input type="text" class="text number" name="SortOrder" value="$_form.text('SortOrder',$out($prop.SortOrder))" />
                </th>
                <td>值小的排前面</td>
            </tr>
            <!--[error name="Price"]-->
                <!--[include src="../_error_.aspx" /]-->
            <!--[/error]-->
            <tr>
                <th><h4>道具售价   </h4>
                <select name="PriceType">
		        <!--[EnabledUserPointList]-->
			    <option value="{=(int)$Type}" <!--[if ((int)$Type).ToString() == $out($prop.PriceType)]-->selected="selected"<!--[/if]-->>$Name</option>
			    <!--[/EnabledUserPointList]-->
			    </select>
                <input type="text" class="text number" name="Price" value="$_form.text('Price',$out($prop.Price))" />
                </th>
                <td></td>
            </tr>
            <!--[error name="PackageSize"]-->
                <!--[include src="../_error_.aspx" /]-->
            <!--[/error]-->
            <tr>
                <th><h4>道具重量</h4>
                <input type="text" class="text number" name="PackageSize" value="$_form.text('PackageSize',$out($prop.PackageSize))" />
                </th>
                <td></td>
            </tr>
            <tr>
                <th><h4>道具属性</h4>
                $PropParamFormHtml
                </th>
                <td>
                </td>
            </tr>
            <!--[error name="AllowExchange"]-->
                <!--[include src="../_error_.aspx" /]-->
            <!--[/error]-->
            <tr>
                <th><h4>允许转售或赠送</h4>
                <label><input type="radio" name="AllowExchange" value="True" $_form.checked('AllowExchange', 'True', $out($prop.AllowExchange)) /> 允许</label><br />
                <label><input type="radio" name="AllowExchange" value="False" $_form.checked('AllowExchange', 'False', $out($prop.AllowExchange)) /> 不允许</label>
                </th>
                <td></td>
            </tr>
            <!--[error name="TotalNumber"]-->
                <!--[include src="../_error_.aspx" /]-->
            <!--[/error]-->
            <tr>
                <th><h4>道具总数</h4>
                <input type="text" class="text number" name="TotalNumber" value="$_form.text('TotalNumber',$out($prop.TotalNumber))" />
                </th>
                <td></td>
            </tr>
            <!--[error name="AutoReplenish"]-->
                <!--[include src="../_error_.aspx" /]-->
            <!--[/error]-->
            <tr>
                <th><h4>自动补货</h4>
                <label><input type="radio" name="AutoReplenish" value="True" $_form.checked('AutoReplenish','True',$out($prop.AutoReplenish)) /> 是</label><br />
                <label><input type="radio" name="AutoReplenish" value="False" $_form.checked('AutoReplenish','False',$out($prop.AutoReplenish)) /> 否</label>
                </th>
                <td></td>
            </tr>
            <!--[error name="ReplenishLimit"]-->
                <!--[include src="../_error_.aspx" /]-->
            <!--[/error]-->
            <tr>
                <th><h4>补货阀值</h4>
                <input type="text" class="text number" name="ReplenishLimit" value="$_form.text('ReplenishLimit',$out($prop.ReplenishLimit))" />
                </th>
                <td>当库存小于或者等于这个值的时候才进行补货操作</td>
            </tr>
            <!--[error name="ReplenishTimeSpan"]-->
                <!--[include src="../_error_.aspx" /]-->
            <!--[/error]-->
            <tr>
                <th><h4>补货周期（小时）</h4>
                <input type="text" class="text number" name="ReplenishTimeSpan" value="$_form.text('ReplenishTimeSpan',$out($prop.ReplenishTimeSpan))" />
                </th>
                <td></td>
            </tr>
            <!--[error name="ReplenishNumber"]-->
                <!--[include src="../_error_.aspx" /]-->
            <!--[/error]-->
            <tr>
                <th><h4>每次补货数量</h4>
                <input type="text" class="text number" name="ReplenishNumber" value="$_form.text('ReplenishNumber',$out($prop.ReplenishNumber))" />
                </th>
                <td></td>
            </tr>
        </table>
    </div>
     
    <h3>购买条件</h3>
    <div class="FormTable">
    <table>
    <tr>
        <th>
        <h4>用户组限制</h4>
        <div style="margin-left:20px;">
        <!--[loop $role in $AllRoleList]-->
        <p style="float:left;width:200px;margin-right:5px;white-space:nowrap;">
            <input type="checkbox" $_form.checked("BuyCondition.groups","$role.RoleID","$Prop.BuyCondition.UserGroupIDString") value="$role.RoleID" id="BuyCondition.group.$role.RoleID" name="BuyCondition.groups" />
            <label for="BuyCondition.group.$role.RoleID">$role.Name</label>
        </p>
        <!--[/loop]-->
        </div>
        </th>
        <td>在选中用户组里的用户才能购买本道具，如果都不选，则所有用户组的用户都能购买。</td>
    </tr>
    
    <!--[error name="BuyCondition.point"]-->
        <!--[include src="../_error_.aspx" /]-->
    <!--[/error]-->
    <tr>
        <th>
        <h4>积分达到</h4>
            <table>
            <tr><td style="padding:2px;">总积分</td><td style="padding:2px;"><input type="text" class="text number" name="BuyCondition.totalPoint" value="$_form.text("BuyCondition.point.total",$GetPointString($prop,-1))" /></td></tr>
            <!--[EnabledUserPointList]-->
            <tr><td style="padding:2px;">$Name</td><td style="padding:2px;"><input type="text" class="text number" name="BuyCondition.$pointID" value="$_form.text("BuyCondition.$pointID",$GetPointString($prop,$pointID))" /></td></tr>
            <!--[/EnabledUserPointList]-->
            </table>
        </th>
        <td>用户的每个积分类型的积分都大于左边的设置时才能购买本道具，如果不限制请留空。</td>
    </tr>
    <!--[error name="BuyCondition.totalposts"]-->
        <!--[include src="../_error_.aspx" /]-->
    <!--[/error]-->
    <tr>
        <th>
            <h4>发帖数</h4>
            <input type="text" class="text number" name="BuyCondition.totalposts" value="$_form.text("BuyCondition.totalposts",$out($prop.BuyCondition.TotalPosts))" />
        </th>
        <td>用户的发帖数大于左边的设置时才能购买本道具。如果不限制发帖数，请设置为0或留空。</td>
    </tr>
    <!--[error name="BuyCondition.onlinetime"]-->
        <!--[include src="../_error_.aspx" /]-->
    <!--[/error]-->
    <tr>
        <th>
            <h4>在线时间</h4>
            <input type="text" class="text number" name="BuyCondition.onlinetime" value="$_form.text("BuyCondition.onlinetime",$out($prop.BuyCondition.OnlineTime))" /> 小时
        </th>
        <td>用户的在线时间大于左边的设置时才能购买本道具。如果不限制在线时间，请设置为0或留空。</td>
    </tr>
    <tr>
        <th>
            <h4>必须完成指定任务</h4>
            <input type="text" class="text" name="BuyCondition.releatedmissionids" value="$_form.text("BuyCondition.releatedmissionids",$out($prop.BuyCondition.ReleatedMissionIDString))" />
        </th>
        <td>
        <p>填写任务ID(可在<a href="$admin/interactive/manage-mission-list.aspx" target="_blank">任务列表</a>查看任务ID)，多个用逗号","隔开。放空为不需要完成任何任务。</p>
        <p>在用户购买本道具的时候，是否需要先完成特定任务。利用此项设置，您可以更有效的促进论坛用户互动。</p>
        </td>
    </tr>
    <tr>
	    <th>
	    <input type="submit" value="保存设置" name="save" class="button" />
	    </th>
	    <td>&nbsp;</td>
    </tr>
    </table>
    </div>
    </form>
</div>

<!--[include src="../_foot_.aspx" /]-->
</body>
</html>
