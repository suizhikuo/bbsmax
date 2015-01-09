<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>积分设置</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<div class="Content">
	<h3>积分策略</h3>
	<form action="$_form.action" method="post">
<div class="Columns">
<div class="ColumnLeft">
	<div class="MenuDock">
	<h3>选择积分策略分类</h3>
	<ul>
	<!--[PointActionTypeList]-->
	<!--[item]-->
	    <li>
	        <a class="$IsSeclected($_get.type,'current')" href="?type=$pointActionType.type">$pointActionType.Name</a>
	        <%--<!--[if $pointActionType.HasNodeList == true]-->
	        <div class="MenuTree MenuWrapper">
	        $pointActionType.NodeItemList.GetTreeHtml("<ul>{0}</ul>","<li><a href=\"?type=$pointActionType.type&nodeID={0}\" class=\"{3}\">{1}</a>{2}</li>","","current",$NodeID,"")
	        </div>
	        <!--[/if]-->--%>
	    </li>
	<!--[/item]-->
	<!--[/PointActionTypeList]-->
	</ul>
	</div>
</div>
<div class="ColumnRight">
    <!--[load src="../_setting-pointaction_.aspx" nodeID="$NodeID" type="$Type" /]-->
</div>
</div>
	</form>
</div>
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>
