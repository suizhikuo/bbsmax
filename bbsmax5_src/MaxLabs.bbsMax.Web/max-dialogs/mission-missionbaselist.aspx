<!--[DialogMaster title="选择任务类型" width="400"]-->
<!--[place id="body"]-->
<div class="clearfix dialogbody">
    <div class="dialogform">
        <div class="formrow">
            <!--[MissionBaseList]-->
            <!--[item]-->
            <!--[if $missionbase.type != "group"]-->
            <a style="margin-right:1em;white-space:nowrap;" href="$admin/interactive/manage-mission-detail.aspx?type=$missionBase.Type$_if($_get.pid != null, '&pid=$_get.pid')" target="_parent">$missionBase.Name</a>
            <!--[/if]-->
            <!--[/item]-->
            <!--[/MissionBaseList]-->
        </div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
<!--[/place]-->
<!--[/dialogmaster]-->