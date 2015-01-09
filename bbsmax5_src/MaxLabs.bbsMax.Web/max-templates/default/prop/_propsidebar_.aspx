<div class="panel round-tl propmoney">
    <div class="panel-body round-tr">
        <div class="round-bl"><div class="clearfix round-br">
            <h3 class="heading">可用金额</h3>
            <ul class="clearfix propmoney-list">
                <!--[EnabledUserPointList]-->
                <li>
                    <span class="label">$Name</span>
                    <span class="numeric">$GetUserPoint($Type)</span>
                </li>
                <!--[/EnabledUserPointList]-->
            </ul>
            
            <div class="propmoney-exchange">
                <!--[if $EnablePointExchange]-->
                <a href="$url(my/point-exchange)">积分兑换</a><br />
                <!--[/if]-->
                <!--[if $EnablePointRecharge]-->
                <a href="$url(my/pay)">充值</a>
                <!--[/if]-->
            </div>
        </div></div>
    </div>
</div>

<div class="panel round-tl propcontainer">
    <div class="panel-body round-tr">
        <div class="round-bl"><div class="clearfix round-br">
            <h3 class="heading">包袱状态</h3>
            <div class="clearfix contentusedchart propcontainerchart">
                <div class="chart-graph">
                    <div class="chart-used" style="width:{=$GetPercent($Status.UsedPackageSize,$MaxPackageSize)}%;"><div>&nbsp;</div></div>
                </div>
                <div class="chart-stat">
                    {=$GetPercent($Status.UsedPackageSize,$MaxPackageSize)}%
                </div>
            </div>
            <ul class="clearfix propcontainer-list">
                <li>
                    <span class="label">包袱容量</span>
                    <span class="numeric">$MaxPackageSize 斤</span>
                </li>
                <li>
                    <span class="label">已使用</span>
                    <span class="numeric">$Status.UsedPackageSize 斤</span>
                </li>
                <li>
                    <span class="label">未使用</span>
                    <span class="numeric">{= $MaxPackageSize - $Status.UsedPackageSize} 斤</span>
                </li>
                <li>
                    <span class="label">拥有道具</span>
                    <span class="numeric">{= $Status.Count + $Status.SellingCount } 件</span>
                </li>
                <li>
                    <span class="label">交易道具</span>
                    <span class="numeric">$Status.SellingCount 件</span>
                </li>
            </ul>
        </div></div>
    </div>
</div>