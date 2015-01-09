<table class="FormTable" width="100%">
<tr>
  <th>
    <h4>子任务（按顺序依次完成）</h4>
    <!--[if $mission != null && $mission.childmissions.count > 0]-->
    <table width="100%" class="DataTable">
    <tr>
      <th>任务名称</th><th>操作</th>
    </tr>
    <!--[loop $mission in $mission.childmissions]-->
    <tr>
      <td>$mission.name</td>
      <td><a href="#">编辑</a> <a href="#">删除</a></td>
    </tr>
    <!--[/loop]-->
    </table>
    <!--[/if]-->
    <a href="$dialog/mission-missionbaselist.aspx?pid=$out($mission.id)" onclick="return openDialog(this.href, function(result){})">添加子任务</a>
  </th>
  <td></td>
</tr>
</table>