<!--[DialogMaster title="附件购买记录" subtitle="$attachment.FileName" width="400"]-->
<!--[place id="body"]-->
<div class="clearfix dialogbody">
    <div class="dialogform dialogform-horizontal">
        <div class="formrow">
            <h3 class="label">盈利</h3>
            <div class="form-enter">
                $TradePoint.Name <strong>$totalMoney</strong>$TradePoint.UnitName.
            </div>
            <div class="form-enter">
                截至目前为止, 总共售出 <strong>$totalCount</strong> 份附件.
            </div>
            <div class="form-note">
                这里指未扣除交易税的赢利, 实际赢利按购买附件时的税率扣除交易税
            </div>
        </div>
    </div>
    <!--[if $TotalCount != 0]-->
    <div class="datatablewrap" style="height:200px;">
        <table class="datatable">
            <thead>
                <tr>
                    <td>用户</td>
                    <td>购买所用$TradePoint.Name</td>
                    <td>时间</td>
                </tr>
            </thead>
            <tbody>
            <!--[loop $Exchange in $ExchangeList]-->
                <tr>
                    <td><a href="$url(space/$Exchange.UserID)" target="_blank">$Exchange.User.Name</td>
                    <td>$Exchange.Price$TradePoint.UnitName</td>
                    <td>$outputdatetime($Exchange.CreateDate)</td>
                </tr>
            </tbody>
            <!--[/loop]-->
        </table>
    </div>
    <!--[pager name="list" skin="_pager.aspx"]-->
    <!--[else]-->
    <div class="nodata">
        当前还没有用户购买过该附件.
    </div>
    <!--[/if]-->
</div>
<div class="clearfix dialogfoot">
    <button class="button" type="reset" accesskey="c" title="关闭" onclick="panel.close();"><span>关闭(<u>C</u>)</span></button>
</div>
<!--[/place]-->
<!--[/DialogMaster]--> 