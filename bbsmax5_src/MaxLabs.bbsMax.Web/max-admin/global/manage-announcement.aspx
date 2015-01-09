<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>公告管理</title>
<!--[include src="../_htmlhead_.aspx"/]-->
<script type="text/javascript">

var inputString=['<input type="checkbox" id="" name="Announcementids" value="{0}" />'];
function createAnnouncement(ann)
{
    var listBody = $('listBody');
    var row = listBody.insertRow( 0 ); 
    row.id="ann-"+ann.AnnouncementID;
    var cell;
    cell=row.insertCell(row.cells.length);
    cell.innerHTML=String.format(inputString[0],ann.AnnouncementID);
    cell=row.insertCell(row.cells.length);
    cell.innerHTML=ann.SortOrder;
    cell=row.insertCell(row.cells.length);
    cell.innerHTML= ann.Subject ;
    cell=row.insertCell(row.cells.length);
    cell.innerHTML= "$my.username" ;
    cell=row.insertCell(row.cells.length);
    cell.innerHTML=ann.TypeName;
    cell=row.insertCell(row.cells.length);
    cell.innerHTML=ann.BeginDate;
    cell=row.insertCell(row.cells.length);
    cell.innerHTML=ann.EndDate;
    cell=row.insertCell(row.cells.length);
    cell.innerHTML=String.format('<a href="$dialog/announcement-edit.aspx?Announcementid={0}" onclick="return openDialog(this.href,refresh)">编辑</a> | <a href="$dialog/announcement-delete.aspx?announcementid={0}" onclick="return openDialog(this.href,this,function(r){removeElement($(\'ann-{0}\'))})">删除</a>',ann.AnnouncementID);
    row.style.backgroundColor = "#FFF0F0";
    var re=$("rowEmpty");
    if (re) {
        re.style.display = "none";
    }
}

var l;
addPageEndEvent(
function(){
l=new checkboxList( "Announcementids","deleteAll" );
}
);

function deleteAnnouncement(r)
{
    if(r)
        for( var i=0;i<r.length;i++)
        {
            removeElement( $("ann-"+r[i]));
        }
}

function batchDelete()
{
    if (l.selectCount() > 0)
        postToDialog({ formId: 'announcementForm', url: '$dialog/announcement-delete.aspx', callback: deleteAnnouncement });
    else
        alert("请选择要删除的公告");
}
</script>
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
    <div class="Content">
    <div class="Help">对于在多条同时在有效期内的公告， 序号越小的越排列在前面</div>
	 <div class="PageHeading">
	 <h3>公告管理 </h3>
	<div class="ActionsBar">
        <a href="$dialog/announcement-edit.aspx" onclick="return openDialog(this.href,createAnnouncement)"><span>添加公告</span></a>
    </div>
    </div>
	<form action="$_form.action" method="post" id="announcementForm">
	<div class="DataTable">
        <table>
        <thead>
            <tr>
            <td class="CheckBoxHold">&nbsp;</td>
            <td style="width:30px;">序号</td>
            <td style="width:150px">公告标题</td>
            <td style="width:80px;">作者</td>
            <td style="width:80px;">公告类型</td>
            <td style="width:150;">开始时间</td>
            <td style="width:150;">结束时间</td>
            <td>可用操作</td>
            </tr>
        </thead>
        <tbody id="listBody">
        <!--[if $AnnouncementList.Count != 0]-->
        <!--[loop $ann in $AnnouncementList with $i]-->
            <tr id="ann-$ann.Announcementid">
            <td><input type="checkbox" id="Announcementids" name="Announcementids" value="$ann.Announcementid" /></td>
            <td>$ann.sortorder</td>
            <td>$ann.subject</td>
            <td>$ann.user.username</td>
            <td>$ann.TypeName </td>
            <td>$outputdatetime($ann.begindate)</td>
            <td>$outputdatetime($ann.enddate)</td>
            <td><a href="$dialog/announcement-edit.aspx?Announcementid=$ann.Announcementid" onclick="return openDialog(this.href,refresh)">编辑</a> | <a href="$dialog/announcement-delete.aspx?announcementid=$ann.announcementid" onclick="return openDialog(this.href,this,function(r){removeElement($('ann-$ann.AnnouncementID'))})">删除</a></td>
            </tr>
        <!--[/loop]-->
        <!--[else]-->
        <tr id="rowEmpty">
        <td colspan="8">
	        <div class="NoData">当前没有公告.</div>
	    </td>
	    </tr>
	    <!--[/if]-->
        </tbody>
        </table>
        <div class="Actions">
            <input type="checkbox" id="deleteAll" />
            <label for="">全选</label>
            <input type="button" onclick="return batchDelete()" name="deleteAll" class="button" value="删除选中" />
        </div>
	</div>
	</form>
</div>
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>