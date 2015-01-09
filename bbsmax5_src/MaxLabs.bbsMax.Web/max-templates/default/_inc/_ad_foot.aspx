<!--[if $hasfooterad]-->
    <div class="banner-footer-wrapper">
        <div class="banner-footer">
            $footerad
        </div>
    </div>
<!--[/if]-->

<!--[if $HasFloatAD || $HasDoubleAD ]-->
<script type="text/javascript" src="$root/max-assets/javascript/max-floatLR.js"></script>
<script type="text/javascript">
var doubleAd="$DoubleAD",floatAD="$floatad";
if(doubleAd){theFloaters.addItem('coupleBannerR','document.body.clientWidth-6',0, String.format('<div style="position: absolute; right: 6px; top: 6px;">{0}<br /><img alt="close" src="'+root+'/max-assets/images/advclose.gif" onmouseover="this.style.cursor=\'pointer\'" onClick="closeBanner();"></div>', doubleAd));theFloaters.addItem('coupleBannerL',6,0,String.format('<div style="position: absolute; left: 6px; top: 6px;">{0}<br /><img alt="close" src="'+root+'/max-assets/images/advclose.gif" onMouseOver="this.style.cursor=\'pointer\'" onclick="closeBanner();"></div>', doubleAd));}
if(floatAD)theFloaters.addItem('floatAdv1',6,'document.documentElement.clientHeight-200',String.format('<div style="position: absolute; right: 6px; top: 6px;">{0}<br /><img alt="close" src="'+root+'/max-assets/images/advclose.gif" onmouseover="this.style.cursor=\'pointer\'" onClick="closeBanner();"></div>',floatAD));
theFloaters.play();
</script>
<!--[/if]-->
<!--[if $AdSettings.EnableDefer]-->
<!-- bugbear -->
<div id="hide_bugbears" style="display:none;"><!--[loop $Bugbear in $AdDeferList]--><div id="hide_bugbear_$Bugbear.id">$Bugbear.code</div><!--[/loop]--></div>
<!-- end bugbear-->
<script type="text/javascript">
var ac=$('hide_bugbears');var bugbears = ac.getElementsByTagName("div");for(var i =0;i<bugbears.length;i++ ){
    var id=bugbears[i].id.substr(13);    var c=bugbears[i].innerHTML;    var truebugbear = $(id);    if(truebugbear&&c)truebugbear.parentNode.innerHTML=c;
}
ac.parentNode.removeChild(ac);
</script>
<!--[/if]-->