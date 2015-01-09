<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head><title>竞价排名榜管理</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
    <div class="PageHeading">
        <h3>
            竞价排名榜管理 
        </h3>
    </div>
	<!--[if $PointShowList.totalrecords>0]-->
	<form action="$_form.action" method="post">
    <div class="DataTable">
        <h4>上榜用户 <span class="counts">总数: $PointShowList.totalrecords</span></h4>
        <table>
            <thead>
                <tr>
                    <td style="width:20px">
                    </td>
                    <td>
                        用户
                    </td>
                    <td>
                       上榜时间 
                    </td>
                    <td>
                    上榜单价($pointType.Name)
                    </td>
                    <td>
                       剩余$pointType.Name
                    </td>
                    <td>
                    上榜宣言
                    </td>
                </tr>
            </thead>
        <!--[loop $ps in $PointShowList with $i]-->
            <tr>
                <td>
                    <input name="userids" type="checkbox" value="$ps.userid" /></td>
                <td>
                    $ps.user.avatar <br />
                    $ps.user.popupnamelink
                </td>
                <td>
                    $outputdatetime( $ps.createDate)</td>
                <td>$ps.price</td>
                <td>
                    $ps.showpoints
                </td>
                <td>
                $ps.content
                </td>
            </tr>
        <!--[/loop]-->
        </table>
    </div>
    <div class="Actions">
        <input id="selectAll" type="checkbox" />
        <label for="selectAll">
        全选</label>
        <input id="Button1" class="button" name="delete" 
            onclick="return  deleteFiles()" type="submit" value="删除" />             
    <!--[AdminPager count="$PointShowList.totalrecords" PageSize="20" /]-->
    </div>
    </form>
   <!--[else]-->
    <div class="NoData">
        没有符合条件的数据.</div>
    <!--[/if]-->
</div>
<!--[include src="../_foot_.aspx"/]-->
<script type="text/javascript">
var l=new checkboxList("userids","selectAll");

function   deleteFiles()
{
    if(!l.selectCount())
    {
        alert("没有选择任何项！");
        return false;
    }
    else
    {
        return confirm("确定删除选中的竞价用户吗？");
    }
}

</script>
</body>
</html>