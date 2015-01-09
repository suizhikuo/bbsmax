<!--[DialogMaster title="屏蔽用户组动态" width="400"]-->
<!--[place id="body"]-->
<form action="$_form.action" method="post">
    <!--[include src="_error_.ascx" /]-->
<div class="clearfix dialogbody">
    <div class="dialogconfirm">
        <h3>
        <!--[if $Group.IsShield]-->
        显示好友分组 $Group.Name 的动态.
        <!--[else]-->
        屏蔽好友分组 $Group.Name 的动态.
        <!--[/if]-->
        </h3>
    </div>
</div>

<div class="clearfix dialogfoot">
    <input type="hidden" name="isShield" value="$Group.IsShield" />
    <button class="button button-highlight" type="submit" name="shieldgroup" accesskey="y" title="确定"><span>确定(<u>Y</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>

</form>
<!--[/place]-->
<!--[/dialogmaster]-->