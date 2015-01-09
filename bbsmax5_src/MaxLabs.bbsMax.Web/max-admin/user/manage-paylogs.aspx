<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>财务明细管理</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx" /]-->
<!--[include src="../_setting_msg_.aspx"/]-->
    <div class="Content">
	 <div class="PageHeading">
	 <h3>财务明细管理 </h3>
    </div>
    <div class="SearchTable" id="complexForm">
        <form action="$_form.action" method="post" id="complex">
        <table>
        <tr>
            <td>用户名</td>
            <td><input name="username" type="text" class="text" value="$_form.text('username',$filter.username)" /></td>
           <td>充值记录</td>
           <td>
           <select name="state">
            <option value="2">全部</option>
            <option value="1" $_form.selected("state","1",$filter.state==1)>成功</option>   
            <option value="0" $_form.selected("state","0",$filter.state==0)>失败</option>     
           </select>
           </td>
        </tr> 
        <tr>
            <td>流水号</td>
            <td><input name="orderno" type="text" class="text" value="$_form.text('orderno',$filter.orderno)" /></td>
            <td>交易号</td>
            <td><input name="transactionno" type="text" class="text" value="$_form.text('transactionno',$filter.transactionno)" /></td>
        </tr> 
        <tr>
           <td>充值类型</td>
           <td>
           <select name="paytype">
                <option value="">不限</option> 
                <option value="1" $_form.selected("paytype","1",$filter.paytype==1)>金币</option>   
                <option value="2" $_form.selected("paytype","2",$filter.paytype==2)>积分</option> 
                <option value="3" $_form.selected("paytype","3",$filter.paytype==3)>经验</option>     
           </select>
           </td>
           <td>支付方式</td>
           <td>
           <select name="payment">
                <option value="">不限</option> 
                <option value="1"  $_form.selected("payment","1",$filter.payment==1)>支付宝</option>   
                <option value="2"  $_form.selected("payment","2",$filter.payment==2)>财付通</option> 
                <option value="3"  $_form.selected("payment","3",$filter.payment==3)>快钱</option>     
           </select>
           </td>
        </tr>   
        <tr>
          <td>商品数量</td>
          <td> 
          <input type="text" class="text" style="width:3em;" value="$_if($filter.beginvalue>0,$filter.beginvalue.tostring())" name="beginvalue" /> ~
          <input type="text" class="text" style="width:3em;" value="$_if($filter.endvalue>0,$filter.endvalue.tostring())" name="endvalue" />个</td>
         <td>充值金额</td>
          <td><input type="text" class="text" style="width:3em;" value="$_if($filter.beginamount>0,$filter.beginamount.tostring())" name="beginamount" /> ~
          <input type="text" class="text" style="width:3em;" value="$_if($filter.endamount>0,$filter.endamount.tostring())" name="endamount" />元</td>
        </tr>       
        <tr>
        <td>交易时间</td>
        <td>
        <input name="begindate" id="begindate" type="text" class="text" value="$_form.text('begindate',$filter.begindate)" />
        <a title="选择日期" id="A0" class="selector-date" href="javascript:void(0)"><img src="$Root/max-assets/images/calendar.gif" alt="选择日期" /></a>
        </td>
        <td>到</td>
        <td>
        <input name="enddate" id="enddate" type="text" class="text" value="$_form.text('enddate',$filter.enddate)" />
        <a title="选择日期" id="A1" class="selector-date" href="javascript:void(0)"><img src="$Root/max-assets/images/calendar.gif" alt="选择日期" /></a>
        </td>
        </tr> 
        <tr>
                <td>&nbsp;</td>
                <td><input name="search" value="搜索" class="button" type="submit" /></td>
                <td>&nbsp;</td>
                <td style="text-align:right;"></td>
            </tr>
        </table>
        </form>
    </div>
	<form action="$_form.action" method="post">
	<div class="DataTable">
	    <!--[if $UserPayList.totalrecords>0]-->
        <table>
        <thead>
            <tr>
            <td class="CheckBoxHold" style="width:50px;"></td>
            <td style="width:80px;">用户名</td>
            <td style="width:120px;">流水号</td>
            <td style="width:120px;">交易号</td>
            <td style="width:120px;">交易时间</td>
            <td style="width:40px;">类型</td>
            <td style="width:40px;">数量</td>
            <td style="width:60px;">支付方式</td>
            <td style="width:60px;">充值金额</td>
            <td style="width:60px;">充值状态</td>
            </tr>
        </thead> 
        <tbody id="listBody">
        <!--[loop $usertype in $UserPayList]-->
        <tr>
            <td class="CheckBoxHold"><input type="checkbox" name="payids" value="$usertype.payid" /></td>
            <td>$usertype.user.username</td>
            <td>$usertype.orderno</td>
            <td>$usertype.transactionno</td>
            <td>$$_if($usertype.paydate.year<1900,"--",$outputdatetime($usertype.paydate))</td>
            <td>$usertype.paytypename</td>
            <td>$usertype.payvalue</td>
            <td>$usertype.paymentname</td>
            <td>$usertype.orderAmount</td>
            <td>$usertype.paystate</td>
        </tr>
       <!--[/loop]-->
        </tbody> 
       </table>
     <!--[/if]-->
       <!--[if $UserPayList.totalrecords>0]-->
        <div class="Actions">
            <input type="checkbox" id="selectAll" />
            <label for="">全选</label>
        </div> 
        <!--[AdminPager Count="$UserPayList.totalrecords" PageSize="$pagesize" /]-->
        <!--[else]-->
         <div class="NoData">没有获取到相应的充值记录.</div>
         <!--[/if]-->
	</div>
	</form>
</div>
<!--[include src="../_foot_.aspx"/]-->
<script type="text/javascript">
    var list = new checkboxList("payids", "selectAll");
    initDatePicker('begindate', 'A0');
    initDatePicker('enddate', 'A1'); 
</script>
</body>
</html>