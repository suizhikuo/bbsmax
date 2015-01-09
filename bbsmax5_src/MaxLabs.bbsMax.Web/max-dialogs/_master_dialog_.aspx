<div class="dialogcontent" style="width:{=$width}px;">
    <!--[if $hasTitle]-->
    <div class="clearfix dialoghead" id="dialogTitleBar_$PanelID">
        <h1 class="dialogtitle">$DialogTitle</h1>
        <!--[if $hassubtitle]-->
        <div class="dialogstatus">$subtitle</div>
        <!--[/if]-->
        <div class="dialogclose"><a href="javascript:;" accesskey="Q" id="max_dialogclosebotton_$PanelID" title="关闭">关闭(<u>Q</u>)</a></div>
        <script type="text/javascript">
            maxDragObject(currentPanel.panel, $('dialogTitleBar_$PanelID'));
            $("max_dialogclosebotton_$PanelID").onclick = function (e) {
                panel.close();
                endEvent(e);
            }
        </script>
    </div>
    <!--[/if]-->
<!--[MasterPagePlace id="body" /]-->
</div>
<script type="text/javascript">    //之所以加在这里判断是因为有些对话框可能有返回值但是不关闭
    if ($DialogReturn) {
        currentPanel.result = $ResultJson;
        if (currentPanel.closeCallback) {
            currentPanel.closeCallback(currentPanel.result);
            delete currentPanel.result;
        }
    }
    page_end && page_end();
</script>