<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>用户邀请码排行</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<div class="Content">
    <h3>用户邀请码排行</h3>
    <form action="$_form.action" method="post">
    <div class="SearchTable">
        <table>
        <tr>
            <td>排序顺序</td>
            <td>
                <select name="sortby">
                <option $_form.selected("sortby","total") value="total">总数量</option>
                <option $_form.selected("sortby","unused") value="unused">未使用数量</option>
                <option $_form.selected("sortby","used") value="used">已使用数量</option>
                <option $_form.selected("sortby","noreg") value="noreg">未注册数量</option>
                <option $_form.selected("sortby","expiress") value="expiress">已过期数量</option> 
                </select>
            </td>
            <td>每页显示数</td>
            <td>
                <select  name="pagesize">
                <option value="10" $_form.selected("size","10")>10</option>
                <option value="20" $_form.selected("size","20",true)>20</option>
                <option value="50" $_form.selected("size","50")>50</option>
                <option value="100" $_form.selected("size","100")>100</option>
                <option value="200" $_form.selected("size","100")>200</option>
                <option value="500" $_form.selected("size","500")>500</option>
                </select>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td colspan="3"><input type="submit" class="button" value="显示" /></td>
        </tr>
        </table>
    </div>
    <div class="DataTable">
    <h4>用户邀请码排行</h4>
<!--[InviteSerialStatList sortBy="$_post.sortby" pageSize="$_form.text('pagesize',$_get.size)" pageNumber="$_get.page"]-->
<!--[head]-->
<!--[if $hasItems]-->
    <table>
        <thead>
        <tr>
            <td>用户名</td>
            <td>已使用</td>
            <td>已过期</td>
            <td>未注册</td>
            <td>未使用</td>
            <td>总数量</td>
        </tr>
        </thead>
        <tbody>
<!--[/if]-->
<!--[/head]-->
<!--[item]-->
        <tr>
            <td>$stat.user.popupnamelink</td>
            <td>$stat.used</td>
            <td>$stat.expiress</td>
            <td>$stat.noregister</td>
            <td>$stat.unused</td>
            <td>$stat.TotalSerial</td>
        </tr>
<!--[/item]-->
<!--[foot]-->
<!--[if $HasItems]-->
        </tbody>
    </table>
    <!--[AdminPager Count="$rowCount" PageSize="$pagesize"/]-->
<!--[/if]-->
<!--[/foot]-->
<!--[/InviteSerialStatList]-->
      </div>
    </form>
</div>
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>
