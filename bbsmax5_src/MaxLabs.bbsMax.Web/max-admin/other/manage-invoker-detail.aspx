<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title><!--[if $isEdit]-->编辑<!--[else]-->添加<!--[/if]-->远程调用</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->

<form action="$_form.action" method="post">
<div class="Content">
    <div class="PageHeading">
	<h3><!--[if $isEdit]-->编辑<!--[else]-->添加<!--[/if]-->远程调用</h3>
	<div class="ActionsBar">
	    <a class="back" href="$admin/other/manage-invoker.aspx"><span>远程调用列表</span></a>
	</div>
	</div>
	<div class="FormTable">
	<table>
        <!--[error name="key"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
		<tr>
			<th>
			<h4>key</h4>
			<input type="text" class="text" name="key" value="$_form.text("key",$out($Invoker.key))" $_if($isedit,'disabled="disabled"') />
			</th>
			<td>调用地址中将包含该值,(禁止使用中/日/韩等双字节文字和特殊字符, 以及<code class="red">\ / ? - # &amp;</code>)不能与其它调用的key重复</td>
		</tr>
        <!--[error name="name"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
		<tr>
			<th>
			<h4>名称</h4>
			<input type="text" class="text" name="name" value="$_form.text("name",$out($Invoker.Name))" />
			</th>
			<td></td>
		</tr>
		<!--[error name="description"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
		<tr>
			<th>
			<h4>描述</h4>
			<textarea name="description" cols="30" rows="6">$_form.text("description",$out($Invoker.Description))</textarea>
			</th>
			<td>对该调用的简单描述</td>
		</tr>
		<!--[error name="invoketype"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
		<tr>
		  <th>
		  <h4>调用类型</h4>
		  <select name="invoketype" onchange="typechange(this.value)" $_if($isedit,'disabled="disabled"')>
		  <option>请选择一个类型</option>
		  <!--[loop $type in $InvokerTypeList]-->
		  <option value="$type.Type" $_form.selected("invoketype","$type.Type",$out($InvokerType.Type))>$type.name</option>
		  <!--[/loop]-->
		  </select>
		  </th>
		  <td></td>
		</tr>
		
        <!--[error name="count"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
		<tr>
			<th>
			<h4>调用条数</h4>
			<input type="text" class="text number" name="count" value="$_form.text("count",$out($Invoker.count))" />
			</th>
			<td>显示的条数，最多不能超过30</td>
		</tr>
		
		<!--[error name="forumID"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
		<tr id="forumList" style="display:none">
			<th>
			<h4>选择版块</h4>
			<select name="ForumID">
		    <option value="0">所有版块</option>
		    <!--[loop $tempForum in $Forums with $i]-->
		    <option value="$tempForum.ForumID" <!--[if $tempForum.parentid == 0]--> style="color:Red"<!--[/if]-->  $_form.selected("ForumID","$tempForum.ForumID","$out($Invoker.ForumID)")>&nbsp;&nbsp;&nbsp;&nbsp;$ForumSeparators[$i]|--$tempForum.ForumName</option>
		    <!--[/loop]-->
		    </select>
			</th>
			<td>红色的为分类版块，不允许选择分类版块</td>
		</tr>

		<tr>
			<th>
			<h4>输出形式</h4>
            <p>
			<input type="radio" id="outputtype1" name="outputtype" value="1" onclick="setOutputParamDisplay()" $_form.checked("outputtype","1",$IsDocumentWrite) /><label for="outputtype1">document.Write</label>
			</p>
            <p>
            <input type="radio" id="outputtype2" name="outputtype" value="2" onclick="setOutputParamDisplay()" $_form.checked("outputtype","2",{=$IsDocumentWrite==false}) /><label for="outputtype2">innerHTML</label>
			</p>
            </th>
			<td>document.Write形式将输出“<font color="red">document.Write('xxxx')</font>”的JS,innerHTML形式将输出“<font color="red">document.getElementById('容器ID').innerHTML='XXX'</font>”的JS.
            </td>
		</tr>

        <tbody id="tr_outputParam">
		<!--[error name="outputParam"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
        <tr>
			<th>
			<h4>容器ID</h4>
			<input type="text" class="text" name="outputParam" value="$_form.text("outputParam",$out($Invoker.OutputParam))" />
			</th>
			<td>如果是innerHTML输出形式，则必须填该值（调用该文件时将会输出形如“<font color="red">document.getElementById('容器ID').innerHTML='XXX'</font>”的JS），该容器ID必须是调用页面里的一个容器的ID，调用的数据将会显示在这个容器里</td>
		</tr>
        </tbody>
        <script type="text/javascript">
            function setOutputParamDisplay() {
                if (document.getElementsByName('outputtype')[0].checked)
                    $('tr_outputParam').style.display = 'none';
                else
                    $('tr_outputParam').style.display = '';
            }
            setOutputParamDisplay();
        </script>

		<!--[error name="template"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
		<tr>
			<th>
			<h4>模板</h4>
			<!--[ajaxpanel id="ap_template" idonly="true"]-->
			<textarea name="template" cols="30" rows="15">$_form.text("template",$Template)</textarea>
			<script type="text/javascript">
			    //<!--[if $InvokerType != null && $InvokerType.isForum]-->
			    $('forumList').style.display = '';
			    //<!--[else]-->
			    $('forumList').style.display = 'none';
			    //<!--[/if]-->
			</script>
			<!--[/ajaxpanel]-->
			</th>
			<td></td>
		</tr>
        <tr>
		    <th>
		    <input type="submit" value="保存" name="save" class="button" />
		    </th>
		    <td>&nbsp;</td>
	    </tr>
    </table>
    </div>
    <!--[/if]-->
</div>
</form>
<!--[include src="../_foot_.aspx" /]-->
<script type="text/javascript">

    function typechange(type) {
        ajaxRender("$AttachQueryString('InvokerType=')" + type, 'ap_template');
    }
</script>
<!--[/if]-->
</body>
</html>