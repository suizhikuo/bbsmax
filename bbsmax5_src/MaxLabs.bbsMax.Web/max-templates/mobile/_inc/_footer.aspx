<footer class="footer">
    <p>Powered by <a href="http://www.bbsmax.com/" rel="bookmark">BBSMAX 5</a> 2002-2010 MaxLabs.</p>
    <p>浏览: <a href="#">手机版</a> | <a href="#">桌面版</a></p>

    <select class="skinlist" name="theme" onchange="if(this.value!=''){location.replace('$url(handler/changeskin)?skin='+this.value+'&u='+encodeURIComponent(location.href));}">
        <option value="">界面风格</option>
        <!--[loop $TheSkin in $TheSkinList]-->
        <option <!--[if $theskin.SkinID == $CurrentSkinID]-->selected="selected" <!--[/if]--> value="$theskin.SkinID">$theskin.Name</option>
        <!--[/loop]-->
    </select>

    <script>
        document.write("<p>"+navigator.userAgent+"</p>");
    </script>

</footer>
