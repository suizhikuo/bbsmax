<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
<title>用户屏蔽日志</title>
<!--[include src="../_htmlhead_.aspx" /]-->
</head>
<body>
<!--[include src="../_head_.aspx" /]-->
<!--[include src="../_setting_msg_.aspx"/]-->

<div class="Content">
    <h3>用户屏蔽日志</h3>
    <div class="SearchTable">
    <form action ="$_form.action" method="post">
    <table>
    <tr>
         <td><label for="username">用户名</label></td>
         <td>            
            <input class="text" id="username" name="username" type="text" value="$Filter.Username" />
         </td>
         <td><label for="userid">用户ID</label></td>
         <td><input class="text" id="userid" name="userid" type="text" value="$Filter.UserID"</td>
    </tr>
    <tr>
       
        <td><label for="NewIP">论坛版块</label></td>
        <td>
        <select name="ForumID">
            <option value="">所有版块</option>
            <!--[loop $tempForum in $Forums with $i]-->
            <option value="$tempForum.ForumID" $_form.selected("ForumID","$tempForum.ForumID")>&nbsp;&nbsp;&nbsp;&nbsp;$ForumSeparators[$i]|--$tempForum.ForumName</option>
            <!--[/loop]-->
        </select>
        </td>  
        <td colspan="2">&nbsp;</td>
    </tr>
    <tbody></tbody>
    <tr>
        <td>搜索时间</td>
        <td colspan="3">
            <input name="BeginDate" class="text datetime" id="begindate" type="text" value="$Filter.BeginDate" />
            <a title="选择日期" id="A0" class="selector-date" href="javascript:void(0)"><img src="$Root/max-assets/images/calendar.gif" alt="选择日期" /></a>
            ~
            <input name="EndDate" class="text datetime" id="enddate" type="text" value="$Filter.EndDate" />
            <a title="选择日期" id="A1" class="selector-date" href="javascript:void(0)"><img src="$Root/max-assets/images/calendar.gif" alt="选择日期" /></a>
            <span class="desc">(时间格式:YYYY-MM-DD)</span>
        </td>
    </tr>
    <tr>
        <td><label for="isdesc">结果排序</label></td>
        <td>
            <select id="isdesc" name="isdesc">
                <option value="True" $_form.selected("isdesc","True",$Filter.isdesc==true)>按操作时间降序排列</option>
                <option value="False" $_form.selected("isdesc","False",$Filter.isdesc==false)>按操作时间升序排列</option>
            </select>
        </td>
        <td>每页显示数</td>
        <td>
            <select name="pagesize">
                <option value="10" $_form.selected("pagesize","10",$Filter.PageSize)>10</option>
                <option value="20" $_form.selected("pagesize","20",$Filter.PageSize)>20</option>
                <option value="50" $_form.selected("pagesize","50",$Filter.PageSize)>50</option>
                <option value="100" $_form.selected("pagesize","100",$Filter.PageSize)>100</option>
                <option value="200" $_form.selected("pagesize","200",$Filter.PageSize)>200</option>
                <option value="500" $_form.selected("pagesize","500",$Filter.PageSize)>500</option>
            </select>
        </td>
    </tr>
    <tr>
        <td>&nbsp;</td>
        <td colspan="3">
            <input type="submit" name="search" class="button" value="搜索" />
        </td>
    </tr>
    </table>
    </form>
    </div>
    
    <form action="$_form.action" method="post" id="loglistform" name="loglistform">
    <div class="DataTable">
        <h4>操作记录<span class="counts">总数: $TotalCount</span></h4>
        <!--[if $TotalCount>0]-->
        <table>
            <thead>
                <tr>
                    <th style="width:6em">用户ID</th>
                    <th style="width:10em">用户名</th>
                    <th style="width:10em">操作类型</th>
                    <th style="width:10em">屏蔽原因</th>
                    <th style="width:10em">操作者</th>
                    <th style="width:10em">IP</th>
                    <th style="width:10em">操作时间</th>
                    <th style="width:10em">操作</th>
                </tr>
            </thead>
            <tbody>
           <!--[loop $item in $BanUserOperationList]--> 
            <tr>
                <td>$item.UserID </td>
                <td>$item.UserName</td>
                <td><!--[if $item.OperationType==BanType.Ban]-->板块屏蔽<!--[else if $item.OperationType==BanType.UnBan]-->解除屏蔽  <!--[else]-->全站屏蔽<!--[/if]--></td>
                <td>$item.Cause</td>
                <td>$item.OperatorName</td>
                 <td>$item.UserIP</td>
                <td>$outputdatetime($item.OperationTime)</td>
                <td>
                <!--[if $item.OperationType!=BanType.UnBan]-->
                <a href="javascript:;" onclick="openDetails(this,$item.id, $_if($item.OperationType==BanType.Ban,'1','0'));">
                 详细
                </a>
                <!--[/if]-->
                </td>
            </tr>
            <tr style="display:none; background-color:#FFECEC" id="ditailRow_$item.id">            
            <td colspan="8">
            <!--[if $item.OperationType==BanType.BanAll]-->
            结束时间 $outputdate( $item.AllBanEndDate);
            <!--[else]--> 
            <div id="rowInner_$item.id">
            <img src="$root/max-assets/images/loading_16.gif" alt="" />请稍候...
            </div>
            <!--[/if]-->
            </td>
            </tr>
          <!--[/loop]-->
            </tbody>
            
        </table>
        <!--[AdminPager Count="$TotalCount" PageSize="$Filter.PageSize" /] -->
        <!--[else]-->
        <div class="NoData">未搜索到任何用户屏蔽记录</div>
        <!--[/if]-->
    </div>
    
    </form>
    
    
</div>  
<script type="text/javascript">
 initDatePicker('begindate','A0');
 initDatePicker('enddate', 'A1');

 function openDetails(a, id, ajax) {
     if (a.open) {
         setVisible($('ditailRow_'+id), 0);
         a.open = 0;
         a.innerHTML = "详细";
     }
     else {
         if (ajax && !a.ajaxopen) {
             a.ajaxopen = 1;
             ajaxRequest("manage-banuserlog.aspx?banid=" + id, function (r) {
                 var s = eval("(" + r + ")");
                 var sb = new stringBuilder();
                 for (var i = 0; i < s.length; i++) {
                     var si = s[i];
                     sb.append("板块：");
                     sb.append(si.ForumName);
                     sb.append(",");
                     sb.append(" 解除时间:");
                     sb.append(si.EndDate);
                     sb.append("<br />");
                 }
                 $('rowInner_' + id).innerHTML = sb.toString();
             });
         }
         setVisible($('ditailRow_' + id), 1);
         a.open = 1;
         a.innerHTML = "关闭";
     }
 }
</script>
<!--[include src="../_foot_.aspx"]-->
 
</body>
</html>
