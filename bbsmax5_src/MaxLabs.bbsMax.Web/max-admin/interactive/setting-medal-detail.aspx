<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title><!--[if $isEdit]-->编辑图标<!--[else]-->添加图标<!--[/if]--></title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->

<form action="$_form.action" method="post">
<div class="Content">
    <div class="PageHeading">
	<h3><!--[if $isEdit]-->编辑图标<!--[else]-->添加图标<!--[/if]--></h3>
	<div class="ActionsBar">
	    <a class="back" href="$admin/interactive/setting-medals.aspx"><span>返回图标管理列表</span></a>
	</div>
	</div>
	<div class="FormTable">
	<table>
        <!--[error name="medalname"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
		<tr>
			<th>
			    <h4>图标名称</h4>
			    <input name="medalname" id="medalname" type="text" class="text" value="$_form.text('medalname',$out($Medal.Name))" />
			</th>
			<td>图标名称</td>
		</tr>
        <!--[error name="sortorder"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
		<tr>
			<th>
			    <h4>排序</h4>
			    <input name="sortorder" id="sortorder" type="text" class="text number" value="$_form.text('sortorder',$out($Medal.SortOrder))" />
			</th>
			<td>填写数字，数字越小勋章排在越前面</td>
		</tr>
		<tr>
			<th>
			    <h4>启用</h4>
                <!--[if $isEdit]-->
                <input name="enable" id="enable" type="checkbox" value="true" $_form.checked('enable','true',$out($Medal.Enable)) />
                <!--[else]-->
                <input name="enable" id="enable" type="checkbox" value="true" $_form.checked('enable','true',true) />
                <!--[/if]-->
                <label for="enable">是否启用该图标</label>
			</th>
			<td></td>
		</tr>
		<tr>
			<th>
			    <h4>未点亮时隐藏</h4>
                <input name="isHidden" id="isHidden" type="checkbox" value="true" $_form.checked('isHidden','true',$out($Medal.IsHidden)) />
                <label for="isHidden">未点亮时隐藏</label>
			</th>
			<td>如果用户的图标未点亮，隐藏了，用户不会看到此图标，不对已点亮的图标有效</td>
		</tr>
        <!--[if $isEdit]-->
        <tr>
            <th>
                <h4>自动点亮图标</h4>
                <input type="checkbox" name="isauto" id="isauto" value="true" onclick="setConditionType(this.checked)" $_form.checked('isauto','true',{=$Medal.IsCustom == false}) />
                <label for="isauto">是否自动点亮图标</label>
            </th>
            <td>&nbsp;</td>
        </tr>
        <!--[else]-->
        <tr>
            <th>
                <h4>自动点亮图标</h4>
                <input type="checkbox" name="isauto" id="isauto" value="true" onclick="setConditionType(this.checked)" $_form.checked('isauto','true',true) />
                <label for="isauto">是否自动点亮图标</label>
            </th>
            <td>自动点亮图标是指当用户的条件达到点亮规则的值时系统自动给用户点亮图标，如果不自动点亮则需要管理员手动点亮图标;
            </td>
        </tr>
        <!--[/if]-->
        <!--[error name="medallevel"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
		<tr>
			<th>
			    <div class="itemtitle">
			        <strong>图标等级</strong>
			        <a class="addexceptrule" href="#" name="addlevel" onclick="add();return false;">添加图标等级</a>
                    <input type="hidden" name="ids" id="ids" value="" />
                    <input type="hidden" name="maxid" id="maxid" value="0" />
                    <input type="hidden" name="condition" id="condition" value="" />
                </div>
                <div class="exceptrule">
                <table id="" style="display:none">
                        <tr id="level_list_item">
                            <td id="level_content">
                                <p>
                                等级名称:
                                <input name="levelName_{id}" type="text" class="text" style="width:10em;" value="{levelName}" />
                                </p>
                                <p id="condition_auto_{id}">
                                    点亮图标规则:
                                    <select name="condition_{id}" id="condition_{id}" onchange="selectAllCondition(this.value)">
                                    <option value="">请选择规则</option>
                                    <option value="Point_0">总积分</option>
                                    <!--[loop $Colum in $Colums]-->
                                    <option value="$Colum.Colum">$Colum.Description</option>
                                    <!--[/loop]-->
                                    </select>
                                    达到:
                                    <input type="text" name="levelValue_{id}" class="text"  style="width:5em" value="{levelValue}" />
                                </p>
                                <p id="condition_custom_{id}">
                                    点亮图标条件:
                                    <input type="text" name="conditionDescription_{id}" class="text" style="width:15em;" value="{conditionDescription}" />
                                </p>
                                <p>
                                    图标:
                                    <input name="iconSrc_{id}" type="text" class="text" id="iconSrc_{id}" style="width:9em;" value="{iconSrc}" />
	                                <a title="选择图片" class="selector-image" href="javascript:void(browseImage('Assets_MedalIcon','iconSrc_{id}'))"><img src="$Root/max-assets/images/image.gif" alt="" /></a>
                                </p>
                            </td>
                            <td id="level_action">
                                <a href="#" name="delete" onclick="deleteLevel('{id}');return false;">删除</a>
                            </td>
                        </tr>
                </table>
                <table id="level_list">
                </table>
                </div>
			</th>
			<td>&nbsp;</td>
		</tr>
		<tr class="nohover">
		    <th>
		        <input type="submit" name="savesetting" value="保存设置" class="button" />
		    </th>
		    <td>&nbsp;</td>
	    </tr>
	</table>
    </div>
</div>
</form>
<script type="text/javascript">

var level_list = $('level_list');
var level_content = fix($('level_content').innerHTML);
var level_action = fix($('level_action').innerHTML);
//var level_list_item = fix($('level_list_item').innerHTML);
//level_list.outerHTML = '';

var isAuto = true;
//<!--[if $IsCustom]-->
    isAuto = false;
//<!--[/if]-->
//<!--[loop $MedalLevel in $MedalLevels]-->
    addLevel($MedalLevel.ID,'$MedalLevel.Name','$MedalCondition','$MedalLevel.Value','$MedalLevel.Condition','$MedalLevel.IconSrc',isAuto);
//<!--[/loop]-->
init();
function init()
{
    if($('ids').value!='')
    {
    }
    else
        add();
}
function add()
{
    var maxid = parseInt($('maxid').value);
    addLevel(maxid+1,'',$('condition').value,'','','',true);
}



function addLevel(id,levelName,condition,levelValue,conditionDescription,iconSrc,isauto)
{
    var maxid = parseInt($('maxid').value);
    if(id>maxid)
        $('maxid').value = id;
        
    $('ids').value = ($('ids').value+ ',' + id).trimStart(',');
    var contentString = level_content.replace(/\{id\}/g, id).replace(/\{levelName\}/g, levelName).replace(/\{condition\}/g, condition).replace(/\{levelValue\}/g, levelValue).replace(/\{conditionDescription\}/g, conditionDescription).replace(/\{iconSrc\}/g, iconSrc);
    var actionString = level_action.replace(/\{id\}/g, id).replace(/\{levelName\}/g, levelName).replace(/\{condition\}/g, condition).replace(/\{levelValue\}/g, levelValue).replace(/\{conditionDescription\}/g, conditionDescription).replace(/\{iconSrc\}/g, iconSrc);
    
    var row = level_list.insertRow(level_list.rows.length);
    row.id = 'level_list_item_' + id;
    var cell1 = row.insertCell(0);
    var cell2 = row.insertCell(1);
    cell1.innerHTML = contentString;
    cell2.innerHTML = actionString;
    
    $('condition').value = condition;
    selectCondition(id,condition);
    setItemConditionType($('isauto').checked,id);
}
function deleteLevel(id)
{
    $('ids').value = (',' + $('ids').value + ',').replace(',' + id + ',', ',').trimStart(',').trimEnd(',');
    removeElement($('level_list_item_' + id));
}

var isSetCondition = false;
function selectAllCondition(value)
{
    if(isSetCondition)
        return;
       
    isSetCondition = true;
    $('condition').value = value;
    var ids =  $('ids').value.trimStart(',').trimEnd(',').split(',');
    for(var i = 0;i < ids.length; i++)
    {
        selectCondition(ids[i],value);
    }
    
    isSetCondition = false;
}
function selectCondition(id,value)
{
    var obj = $('condition_'+id);
    for(var i=0; i<obj.options.length; i++)
    {
         if(obj.options[i].value==value)
         {
            obj.options[i].selected = true;
            break;
         }
    }
}
function setConditionType(isauto)
{
    var ids =  $('ids').value.trimStart(',').trimEnd(',').split(',');
    for(var i = 0;i < ids.length; i++)
    {
        setItemConditionType(isauto,ids[i]);
    }
}
function setItemConditionType(isauto,id)
{
    if(isauto)
    {
        $('condition_auto_'+id).style.display = '';
        $('condition_custom_'+id).style.display = 'none';
    }
    else
    {
        $('condition_auto_'+id).style.display = 'none';
        $('condition_custom_'+id).style.display = '';
    }
}
function fix(str) {
        return str.replace(/value=\{([\w]+)\}/g, 'value="{$1}"');
    };
</script>
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>