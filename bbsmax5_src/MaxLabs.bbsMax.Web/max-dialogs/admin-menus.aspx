<!--[DialogMaster title="管理员功能导航" width="700"]-->
<!--[place id="body"]-->
<div class="scroller" style="height:420px;margin-bottom:0;" id="menuContainer">

</div>
<script type="text/javascript">
var menuTree=parent.menuTree;
var builder=new stringBuilder();
var c=$('menuContainer');

function putSubMenus(parent,depth)
{
    if(parent.SubPages && parent.SubPages.length)
    {
  
        for(var i=0;i<parent.SubPages.length;i++)
        {   if(depth==1)
                builder.append( String.format('<a target="_parent" href="$admin/{0}">{1}</a>',parent.SubPages[i].Url,parent.SubPages[i].Title));
            else
                builder.append(String.format('<h4>{0}</h4>',parent.SubPages[i].Title));
                
            putSubMenus(parent.SubPages[i],depth+1);
       }
    }
}

builder.append('<table class="featurenav"><tr>')
for(var i=0;i<menuTree.length;i++)
{
       builder.append( String.format('<td><h3>{0}</h3>',menuTree[i].Title));
       putSubMenus(menuTree[i],0);
       builder.append("</td>");
}
builder.append("</tr></table>")
c.innerHTML = builder.toString();
</script>
<!--[/place]-->
<!--[/DialogMaster]-->
