<!--[DialogMaster width="350" ]-->
<!--[place id="body"]-->
<div class="dialoginfowrap">
    <div class="dialogclose"><a href="javascript:void(panel.close());" accesskey="Q" title="关闭">关闭(<u>Q</u>)</a></div>
    <div class="dialoginfo dialoginfo-$mode">
        <h3>$message</h3>
        <!--[loop $jump in $JumpLinkList]-->
        <!--[if $jump.link == ""]-->
        <p>$jump.Text</p>
        <!--[else]-->
        <p><a href="$jump.Link">$jump.Text</a></p>
        <!--[/if]-->
        <!--[/loop]-->
    </div>
    <div class="clearfix dialogfoot">
        <button class="button button-highlight" type="button" accesskey="c" onclick="panel.close();" title="关闭"><span>关闭(<u>C</u>)</span></button>
    </div>
</div>
<script type="text/javascript">
    currentPanel.result = $ReturnValue;
    if(!window.max_infoPanel)window.max_infoPanel={};
    //<!--[if $IsSuccess]-->
    var rnd = new Date().getTime().toString();
    max_infoPanel[rnd] = currentPanel;
    max_infoPanel[rnd].tHandler = window.setTimeout(new Function("var tmp =max_infoPanel["+rnd+"]; try { tmp.close(); delete max_infoPanel["+rnd+"]; } catch (e) { }"), 3000);
    currentPanel.addCloseHandler(new Function("var tmp = max_infoPanel["+rnd+"]; if (tmp) { window.clearTimeout(tmp.tHandler); delete max_infoPanel["+rnd+"]; } "));
    //<!--[/if]-->
    currentPanel.relocation();
</script>
<!--[/place]-->
<!--[/DialogMaster]-->