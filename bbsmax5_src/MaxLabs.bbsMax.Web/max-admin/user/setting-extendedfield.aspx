<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>用户扩展信息项管理</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx" /]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
	<h3>管理用户扩展信息项</h3>
<form action="$_form.action" method="post">
    <div class="DataTable">
        <table>
            <thead>
                <tr>
                    <!--[if $IsEnablePassport]-->
                    <th class="CheckBoxHold">启用</th>
                    <!--[/if]-->
                    <th style="width:30px;">排序</th>
                    <th style="width:120px;">名称</th>
                    <th style="width:120px;">填写方式</th>
                    <th style="width:70px;">必须填写</th>
                    <th style="width:70px;">隐藏</th>
                    <th style="width:120px;">是否公开显示</th>
                    <th style="width:120px;">可用于搜索用户</th>
                    <th>描述</th>
                    <th style="width:100px;">操作</th>
                </tr>
            </thead>
            <tbody>
				<!--[ExtendedFieldList]-->
                <tr>
                    <!--[if $IsEnablePassport]-->
                    <td>
                    <!--[if $isPassport]-->
                    <input type="checkbox" name="extendfieldEnable" value="$key" $_if($isEnable,'checked="checked"','') />
                    <!--[else]-->
                    <input type="checkbox" name="extendfieldEnable" value="$key" checked="checked" disabled="disabled" />
                    <!--[/if]-->
                    </td>
                    <!--[/if]-->
                    <td><input type="text" class="text number" name="{=$key}_sortOrder" value="$SortOrder" /></td>
                    <td>$name</td>
                    <td>$fieldType.DisplayName</td>
                    <td>$_if($IsRequired, "是", "否")</td>
                    <td>$_if($IsHidden, "是", "否")</td>
                    <td>
                    <!--[if $DisplayType == ExtendedFieldDisplayType.AllVisible]-->
                    所有人可见
                    <!--[else if $DisplayType == ExtendedFieldDisplayType.FriendVisible]-->
                    仅用户好友可见
                    <!--[else if $DisplayType == ExtendedFieldDisplayType.SelfVisible]-->
                    仅用户自己可见
                    <!--[else if $DisplayType == ExtendedFieldDisplayType.UserCustom]-->
                    用户自定义
                    <!--[/if]-->
                    </td>
                    <td>$_if($Searchable, "是", "否")</td>
                    <td>$Description</td>
                    <td>
                    <!--[if $isPassport]-->
                    编辑 | 删除
                    <!--[else]-->
                    <a href="$admin/user/setting-extendedfield.aspx?key=$key">编辑</a> |
                    <a href="$dialog/extendedfield-delete.aspx?key=$key" onclick="openDialog(this.href, function(){ refresh(); }); return false;">删除</a>
                    <!--[/if]-->
                    </td>
                </tr> 
				<!--[/ExtendedFieldList]-->
            </tbody>
        </table>
        <div class="Actions">
            <!--[if $IsEnablePassport]-->
            <input name="checkAll" id="selectAll" type="checkbox" />
            <label for="selectAll">全选</label>
            <script type="text/javascript">
                new checkboxList('extendfieldEnable', 'selectAll');
            </script>
            <!--[/if]-->
            <input class="button" name="save" type="submit" value="保存更改" />
        </div>
        <!--
        <div class="Actions">
            <input type="checkbox" id="selectall" />
            <label for="selectall">全选</label>
            <input type="button" class="button" value="删除选中项" />
        </div>
        <div class="NoData">当前没有任何用户扩展信息项.</div>
        -->
    </div>
    </form>
    <!--[if $_get.type == null && $_get.key == null]-->
    <form action="$_form.action">
		<h3>新建用户扩展信息项 (第1步: 选择填写方式)</h3>
		<div class="FormTable">
		<table>
			<tr>
				<th><h4>信息输入方式</h4>
					<select style="width:300px" name="type">
						<!--[ExtendedFieldTypeList]-->
						<option value="$typeName">$displayName</option>
						<!--[/ExtendedFieldTypeList]-->
					</select>
				</th>
				<td>
				</td>
			</tr>
			<tr class="nohover">
				<th>
				<input type="submit" value="下一步" class="button" />
				</th>
				<td>&nbsp;</td>
			</tr>
		</table>
		</div>
    </form>
    <!--[else]-->
    <form action="$_form.action" method="post">
    <!--[if $_get.key == null]-->
	<h3>新建用户扩展信息项 (第2步: 信息项设置)</h3>
	<!--[else]-->
	<h3>编辑用户扩展信息项</h3>
	<!--[/if]-->
    <div class="FormTable">
    <table>
        <!--[error name="Name"]-->
        <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
        <tr>
            <th><h4>名称</h4>
            <input type="text" class="text" name="Name" value="$CurrentField.Name" />
            </th>
            <td>
            显示给网站用户的名称，如“家庭地址”，“您所使用的手机品牌”等
            </td>
        </tr>
        <!--[error name="SortOrder"]-->
        <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
        <tr>
            <th><h4>排序</h4>
            <input type="text" class="text number" name="SortOrder" value="$CurrentField.SortOrder" />
            </th>
            <td>
            显示顺序，数值越小越靠前
            </td>
        </tr>
        <!--[error name="Description"]-->
        <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
        <tr>
            <th><h4>描述</h4>
            <input type="text" class="text" name="Description" value="$CurrentField.Description" />
            </th>
            <td>
            帮助您或其他管理员记住和了解此扩展字段的用途
            </td>
        </tr>
        <!--[error name="IsRequired"]-->
        <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
        <tr>
            <th><h4>是否必须填写</h4>
                <div><input type="radio" name="IsRequired" id="IsRequired1" value="True" $_form.checked("IsRequired", "True", $CurrentField.IsRequired) /> <label for="IsRequired1">是</label></div>
                <div><input type="radio" name="IsRequired" id="IsRequired2" value="False" $_form.checked("IsRequired", "False", $CurrentField.IsRequired == false) /> <label for="IsRequired2">否</label></div>
            </th>
            <td>
            设置网站用户是否必须填写该字段，如果必须填写，已注册的用户在登陆后将提示必须完善个人资料<br />
            新注册用户在完成注册流程后将跳转到个人资料填写页面
            </td>
        </tr>
        <!--[error name="IsHidden"]-->
        <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
        <tr>
            <th><h4>是否隐藏</h4>
                <div><input type="radio" name="IsHidden" id="IsHidden1" value="True" $_form.checked("IsHidden", "True", $CurrentField.IsHidden) /> <label for="IsHidden1">是</label></div>
                <div><input type="radio" name="IsHidden" id="IsHidden2" value="False" $_form.checked("IsHidden", "False", $CurrentField.IsHidden == false) /> <label for="IsHidden2">否</label></div>
            </th>
            <td>
            如果该字段暂时不使用则可以设为隐藏，用户填写资料时不会看到该字段
            </td>
        </tr>
        <!--[error name="DisplayType"]-->
        <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
        <tr>
            <th><h4>是否公开显示</h4>
                <div><input type="radio" name="DisplayType" id="DisplayType1" value="AllVisible" $_form.checked("DisplayType", "AllVisible", $CurrentField.DisplayType) /> <label for="DisplayType1">所有人可见</label></div>
                <div><input type="radio" name="DisplayType" id="DisplayType2" value="FriendVisible" $_form.checked("DisplayType", "FriendVisible", $CurrentField.DisplayType) /> <label for="DisplayType2">仅用户的好友可见</label></div>
                <div><input type="radio" name="DisplayType" id="DisplayType3" value="SelfVisible" $_form.checked("DisplayType", "SelfVisible", $CurrentField.DisplayType) /> <label for="DisplayType3">仅用户自己可见</label></div>
                <div><input type="radio" name="DisplayType" id="DisplayType4" value="UserCustom" $_form.checked("DisplayType", "UserCustom", $CurrentField.DisplayType) /> <label for="DisplayType4">用户自定义</label></div>
            </th>
            <td>
            设置当前资料是否公开。用户自定义是指用户可以自己设置该资料对哪些人公开显示。
            </td>
        </tr>
        <!--[error name="Searchable"]-->
        <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
        <tr>
            <th><h4>是否用于搜索用户</h4>
                <div><input type="radio" name="Searchable" id="Searchable1" value="True" $_form.checked("Searchable", "True", $CurrentField.Searchable) /> <label for="Searchable1">是</label></div>
                <div><input type="radio" name="Searchable" id="Searchable2" value="False" $_form.checked("Searchable", "False", $CurrentField.Searchable == false) /> <label for="Searchable2">否</label></div>
            </th>
            <td>
            设置网站用户是否可以通过搜索此项信息以找到所有信息一致用户<br />
            只有"所有人可见"的资料才会被搜索到(用户自己设置的所有人可见的资料也会被搜索到)<br/>
            通常，如果您当前添加的用户信息可以提高网站的针对性和互动型，则可以设置为允许搜索，如“手机型号”、“擅长的乐器”等
            </td>
        </tr>
        <!--[ExtendedFieldType type="$CurrentField.FieldTypeName"]-->
        <!--[if $settingable && $BackendControlSrc!=null]-->
		<!--[load src="$BackendControlSrc" field="$CurrentField" /]-->
        <!--[/if]-->
        <!--[/ExtendedFieldType]-->
        <tr class="nohover">
            <th>
            <input type="button" value="返回" class="button" onclick="window.location.href = 'setting-extendedfield.aspx'" />
            <!--[if $_get.key == null]-->
            <input name="SaveExtendedField" type="submit" value="完成设置" class="button" />
            <!--[else]-->
            <input name="SaveExtendedField" type="submit" value="保存设置" class="button" />
            <!--[/if]-->
            </th>
            <td>&nbsp;</td>
        </tr>
    </table>
    </div>
    </form>
    <!--[/if]-->
</div>

<!--[include src="../_foot_.aspx" /]-->
</body>
</html>