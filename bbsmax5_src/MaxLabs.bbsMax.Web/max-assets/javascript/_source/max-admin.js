//打开小图标选择对话框
function browseImage(dir, id,imgid)
{    
        openDialog(root+'/max-dialogs/image-browser.aspx?dir='+dir,function(r){
        $(id).value=r;
        if(imgid)
        {
            var img=$(imgid);
            img.src=r.url;
            img.style.display='';
        }
    });

    return false;
}

//打开小图标选择对话框
function openImage(dir,callback)
{
    openDialog(root+'/max-dialogs/image-browser.aspx?dir='+dir,callback);
    return false;
}


//----例外 ----------------------------------------------------------
function exceptable_checkRole(name, roleID, roleName)
{
    var roleIDs = document.getElementsByName(name+'_roleIDs');
    for(var i=0;i<roleIDs.length;i++)
    {
        if(roleIDs[i].value == roleID)
        {
            showAlert("已经存在“" + roleName + "”的例外,不能重复添加");
            return false;
        }
    }
    return true;
}

function exceptable_load(id,divID,itemCount)
{
    if(document.getElementById('display_'+id).value=='1')
	{
	    exceptable_display(id,true,divID,itemCount);
	}
	else
	{
	    exceptable_display(id,false,divID,itemCount);
	}
}
function exceptable_display(id,isDisplay,divID,itemCount)
{
    if(isDisplay)
    {
        if($("except_canedit")!=null && $("except_canedit").value=='0')
            return;
        document.getElementById(id).style.display='';
        document.getElementById('display_'+id).value='1';
        document.getElementById('except_'+id).style.display='none';
        if(itemCount==1)
            document.getElementById(divID).style.display='';
    }
    else
    {
        document.getElementById(id).style.display='none';
        document.getElementById('display_'+id).value='0';
        document.getElementById('except_'+id).style.display='';
        if(itemCount==1)
            document.getElementById(divID).style.display='none';
    }
}
function exceptable_cancle(trId,selectName,divID,itemCount)
{
    if($("except_canedit")!=null && $("except_canedit").value=='0')
        return;
        
    exceptable_display(trId,false);
    document.getElementsByName(selectName)[0].value = '';
    if(itemCount==1)
        document.getElementById(divID).style.display='none';
        
    var error = document.getElementById('error_'+trId);
    if(error!=null)
        error.style.display='none';
}
function exceptable_deleteItem(trId,itemName)
{
    if($("except_canedit")!=null && $("except_canedit").value==0)
        return;
    
    document.getElementById(trId).style.display='none';
    document.getElementsByName(itemName)[0].value = '1';
    var error = document.getElementById('error_'+trId);
    if(error!=null)
        error.style.display='none';
}

//----例外 结束----------------------------------------------------------