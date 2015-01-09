</div> <!-- end div.container -->

<div class="foot">
    <p>
    2002-2010 MaxLabs.
    Powered by $Version.
    Processed in $ProcessTime seconds, $QueryTimes Queries.
    GMT $_if($My.TimeZone>0,'+')$My.TimeZone $usernow
    </p>
</div>
<script type="text/javascript">
<!--[AdminMenu level="1"]--> 
$_if($page==$selected, 'current=$page.id;')
<!--[/AdminMenu]-->
<!--[AdminMenu level="2"]-->
<!--[AdminMenu level="3" parent="$page"]-->
    $_if($page==$selected, 'currentSub=$page.id;')
<!--[/AdminMenu]-->
<!--[/AdminMenu]-->
<!--[AdminMenuData]-->
menuTree=$menuJsonObject;
<!--[/AdminMenuData]-->
page_end();
var menuIsFocus = false;
var headMenu = $('headMenu');
if(headMenu!=null)
{
addHandler(headMenu,"mousemove",function(e){ menuIsFocus =true; endEvent(e)});
addHandler( headMenu, "mouseout",function(){menuIsFocus=false;} )
var menuTimeOutFlag = false;
function recoilMenu(){if(menuTimeOutFlag==false){ window.setTimeout(function(){  if(! menuIsFocus)recoverMenu();menuTimeOutFlag =false;},2000);};menuTimeOutFlag=true;}

addHandler(document.documentElement,"mousemove", recoilMenu);

}

var numberInput = document.getElementsByTagName("input");

function numberInputKeydown(e)
{
    
    e = window.event||e;
    var t = e.currentTarget||e.srcElement;
    if(e.keyCode>=48&&e.keyCode<=57)
        return true;
    if(e.keyCode==45||e.keyCode==46||e.keyCode==43)
        return true;
    e.returnValue = false;
    t.value=t.value.replace(/[^\d\.\+\-]/,'');
    return false;
}

for(var i=0;i<numberInput.length;i++)
{
    if(numberInput[i].className.toLowerCase().indexOf("number")>-1)
    {
        addHandler(numberInput[i],"keyup",numberInputKeydown);
    }
}

</script>