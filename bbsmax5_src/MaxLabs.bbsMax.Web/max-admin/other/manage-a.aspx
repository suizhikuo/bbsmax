<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>广告管理</title>
<!--[include src="../_htmlhead_.aspx"/]-->
<script type="text/javascript">
var categoryName="$category.name";
var adList;
var listCount=$AdvertList.count;
addPageEndEvent(function()
{
    adList=new checkboxList('AdID','checkAll');
});

function addAD( ad ) {
    if(ad==null)return ;
    var listBody=$('listBody');
    var row = listBody.insertRow(0);
    var td=row.insertCell(row.cells.length);
    td.innerHTML='<input type="checkbox" name="AdID" value="'+ad.ADID+'" />';
    td=row.insertCell(row.cells.length);
    td.innerHTML=ad.Index+'.';
    td=row.insertCell(row.cells.length);
    td.innerHTML=ad.Title;
    td=row.insertCell(row.cells.length);
    td.innerHTML= ad.Available ?"是":"否";
    td=row.insertCell(row.cells.length);
    td.innerHTML=categoryName;
    td=row.insertCell(row.cells.length);
    td.innerHTML=ad.TypeName;
    td=row.insertCell(row.cells.length);
    td.innerHTML=ad.BeginDate;
    td=row.insertCell(row.cells.length);
    td.innerHTML=ad.EndDate;
    td=row.insertCell(row.cells.length);
    td.innerHTML='<a href="'+root+'/max-dialogs/a-edit.aspx?adid='+ad.ADID+'&Categoryid='+ad.CategoryID+'" onclick="return openDialog(this.href)">编辑</a>';
    row.style.backgroundColor="#FFF0F0";
    
    if(listCount==0){
    $('divHidden').style.display='';
       $('nodate').style.display='none';
    }

    $('count').innerText= ++listCount;
}
</script>
</head>
<body>
<!--[include src="../_head_.aspx" /]-->
<div class="Content">
    <div class="Help">
    <!--[if $IsEnable == false]-->
    <p style="color:Red">系统当前设置已经关闭广告系统，要使广告生效请到 <a href="setting-a.aspx">“广告设置”</a> 启用广告系统</p>
    <!--[/if]-->
    $category.Description
    </div>
    <div class="PageHeading">
    <h3>广告管理——$Category.name
    <!--[if $ADPosition == ADPosition.Right]-->
    (帖子右侧)
    <!--[else if  $ADPosition == ADPosition.Top ]-->
    (帖子上方)
    <!--[else if  $ADPosition == ADPosition.Bottom ]-->
    (帖子下方)
    <!--[/if]-->
    <!--[/if]-->
    </h3>
    <div class="ActionsBar">
        <a href="setting-a.aspx" class="back"><span>返回</span></a>
    <!--[if $Category.id!=0]-->
        <a href="$dialog/a-edit.aspx?adid=0&categoryid=$Category.id&pos=$ADPosition" onclick="return openDialog(this.href,addAD)"><span>添加 $Category.name</span></a>
    <!--[/if]-->
    </div>
    </div>
    <form id="form2" action="$_form.action" method="post">
    <div class="DataTable">
        <!--[if $TotalCount==0]--><div style="display:none" id="divHidden"><!--[/if]-->
        <h4>$category.name <span class="counts">总数: <span id="count">$TotalCount</span></span></h4>
        <table>
        <thead>
            <tr>
                <td class="CheckBoxHold">&nbsp;</td>
                <td>显示顺序</td>
                <td style="width:200px;">标题</td>
                <td>启用</td>
                <td>类型</td>
                <td>形式</td>
                <td>起始时间</td>
                <td>终止时间</td>
                <!-- td style="width:100px;">调用广告</td -->
                <td style="width:80px;">编辑</td>
            </tr>
        </thead>
        <tbody id="listBody">
        <!--[loop $ad in $AdvertList]-->
            <tr>
                <td><input type="checkbox" name="AdID" value="$ad.adID" /></td>
                <td>$ad.index</td>
                <td>$ad.Title</td>
                <td> $_if($ad.Available,'是','否')</td>
                <td> $ad.category.name</td>
                <td> $ad.typename </td>
                <td> $outputdate($ad.begindate )</td>
                <td> $outputdate($ad.enddate)</td>
                <%--<td><a href="$root/max-templates/default/admin/manage-a-detail.aspx?id=$ad.adID&op=tpl">模板内嵌代码</a> |
                    <a href="$root/max-templates/default/admin/manage-a-detail.aspx?id=$ad.adID&op=js">Javascript代码</a>
                </td>--%>
                <td><a href="$dialog/a-edit.aspx?adid=$ad.adID&Categoryid=$Category.id" onclick="return openDialog(this.href,refresh)">编辑</a></td>
            </tr>
        <!--[/loop]-->
        </tbody>
        </table>
        <div class="Actions">
            <input type="checkbox" name="checkAll" value="checkbox" id="checkAll" />
            <label for="checkAll">全选</label>
            <input name="deletesubmit" class="button" type="submit" value="批量删除" onclick="return confirm('确实要删除这些广告吗？')" />
            <input name="batchEnable" class="button" type="submit" value="批量启用" />
            <input name="batchDisable" class="button" type="submit" value="批量禁用" />
        </div>
        <!--[AdminPager Count="$TotalCount" pageSize="20" /]-->
        <!--[if $TotalCount==0]-->
        </div>
        <div class="NoData" id="nodate">$Category.name下暂时没有广告项目</div>
        <!--[/if]-->
    </div>
    </form>
</div>
<!--[include src="../_foot_.aspx" /]-->
</body>
</html>
