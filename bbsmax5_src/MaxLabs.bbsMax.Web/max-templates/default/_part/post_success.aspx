<!--/* 发帖成功提示 */-->
<!--[ajaxpanel id="post_success" ajaxonly="true"]-->
<div class="dialog" id="messagePanel" style="display:none; position:absolute;">
    <div class="dialog-inner">
        <div class="dialogcontent publishtip">
            <div class="clearfix publishtip-info">
                <!--[if $IsPostSuccess]-->
                <h3><span class="info-success">.</span>$PostMessage</h3>
                
                <!--[else if $IsPostAlert]-->
                <h3><span class="info-alert">.</span>$PostMessage</h3>
                <!--[/if]-->
            </div>
            <div class="publishtip-link">
                <ul>
                    <!--[loop $link in $JumpLinks]-->
                    <li><a href="$link.Link">$link.Text</a></li>
                    <!--[/loop]-->
                </ul>
            </div>
        </div>
    </div>
</div>
<script type="text/javascript">
    window.setTimeout("location.replace('$PostReturnUrl')", 3000);
    var p=$('messagePanel');
    setVisible(p,1);
    moveToCenter(p);
    setStyle(p,{zIndex:999});
    var b = new background();

    if (window.enableAutosave) {
        window.enableAutosave = 0;
        deleteTempdata();
    }
var btns = $('postButton');
btns.disabled = false;
setVisible($('ajaxsending'),0);
</script>
<!--[/ajaxpanel]-->