<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>远程调用管理</title>
<!--[include src="../_htmlhead_.aspx"/]-->
<script type="text/javascript">
</script>
</head>
<body>
<!--[include src="../_head_.aspx" /]-->
<div class="Content">
    <div class="Help">
    <p>远程调用是指在其它网站调用本站的数据。</p>
    <p>在需要调用的页面上写上以下红色代码即可,其中"{key}"替换成每个调用的key</p>
    <p style="color:Red">
        &lt;script type=&quot;text/javascript&quot; charset=&quot;utf-8&quot; 
        src=&quot;{=$FullAppRoot}/max-js/{key}.aspx&quot;&gt;</p>
    </div>
    <div class="PageHeading">
    <h3>远程调用管理</h3>
    <div class="ActionsBar">
        <a href="manage-invoker-detail.aspx"><span>添加远程调用</span></a>
    </div>
    </div>
    <form id="form2" action="$_form.action" method="post">
    <div class="DataTable">
        <table>
        <thead>
            <tr>
                <td class="CheckBoxHold">&nbsp;</td>
                <td>key</td>
                <td style="width:200px;">名称</td>
                <td>描述</td>
                <td style="width:150px;">操作</td>
            </tr>
        </thead>
        <tbody id="listBody">
        <!--[loop $invoker in $JsInvokerList]-->
            <tr>
                <td><input type="checkbox" name="keys" value="$invoker.key" /></td>
                <td>$invoker.key</td>
                <td>$invoker.name</td>
                <td>$invoker.description</td>
                <td><a href="manage-invoker.aspx?action=delete&key=$invoker.key" onclick="if(confirm('确实要删除该调用吗？')){location.href=this.href;}else{return false;}">删除</a>
                 <a href="manage-invoker-detail.aspx?action=edit&key=$invoker.key">编辑</a>
                 <a href="$dialog/invoker-code.aspx?key=$invoker.key" onclick="return openDialog(this.href,null)">获取调用代码</a>
                 </td>
            </tr>
        <!--[/loop]-->
        </tbody>
        </table>
        <div class="Actions">
            <input type="checkbox" name="checkAll" value="checkbox" id="checkAll" />
            <label for="checkAll">全选</label>
            <input name="deletesubmit" class="button" type="submit" value="删除选中" onclick="return confirm('确实要删除这些调用吗？')" />
        </div>
        <script type="text/javascript">
            new checkboxList('keys', 'checkAll');
        </script>
        <!--[if $JsInvokerList.Count==0]-->
        <div class="NoData" id="nodate">暂时没有远程调用</div>
        <!--[/if]-->
    </div>
    </form>
</div>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<!--[include src="../_foot_.aspx" /]-->
</body>
</html>
