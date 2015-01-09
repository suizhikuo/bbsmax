<!--[DialogMaster title="图片浏览" subtitle="$DirName" width="650"]-->
<!--[place id="body"]-->
<form id="form1" method="post" action="$_form.action" enctype="multipart/form-data">
<!--[include src="_error_.ascx" /]-->
<div class="clearfix dialogbody">
    <div class="clearfix dialogfluidform">
        <div class="formrow">
            <h3 class="label">上传文件</h3>
            <div class="form-enter">
                <iframe frameborder="0" scrolling="no" style="height:25px;" src="$dialog/image-browser-upload.aspx?dir=$_get.dir" allowtransparency="true"></iframe>
            </div>
        </div>
    </div>

    <div class="filethumbview" style="height:320px;" id="listContainer">
        <ul class="clearfix filethumb-list" id="fileList">
        <!--[loop $f in $Files with $i]-->
            <li class="thumbitem">
                <a class="thumb-entry" title="$f.name" href="javascript:;" ondblclick="chooseImage();return false;" onclick="select(this,'$GetRelativeUrl($f.name)','$GetUrl($f.name)');return false;">
                    <span class="thumb">
                        <img alt="" src="$GetUrl($f.name)" onload="imageScale(this,100,100)" />
                    </span>
                    <span class="title">
                        $f.name
                    </span>
                </a>
            </li>
        <!--[/loop]-->
        </ul>
    </div>

</div>
<div class="clearfix dialogfoot">
    <input type="hidden" name="selected" id="selected" />
    <button class="button button-highlight" type="button" accesskey="y" id="btnOk" disabled="disabled" onclick="chooseImage();"><span>确定(<u>Y</u>)</span></button>
    <button class="button" type="submit" id="delete" name="delete" accesskey="d" disabled="disabled" onclick="return confirm('您确定要删除这些文件吗？');"><span>删除选中(<u>D</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
<script type="text/javascript">

    var onFileUpload = function (f) {
        var html = String.format('<a class="thumb-entry" title="{0}" href="javascript:void(0);">\
        <span class="thumb"><img alt="" id="img_{3}"  src="{2}"/></span>\
                    <span class="title">{0}</span>\
                </a>', f.filename, f.value, f.url, f.url.getHashCode());
        var l = $("fileList");
        var item = addElement("li", l);
        addCssClass(item, "thumbitem");
        item.innerHTML = html;
        var atag = item.childNodes[0];
        atag.ondblclick = chooseImage;
        atag.onclick = new Function('select(this, "'+f.value+'", "' + f.url + '");');
        $("img_" + f.url.getHashCode()).onload = function () { imageScale($("img_" + f.url.getHashCode()), 100, 100); $("listContainer").scrollTop = 65535; };
    }
var imageStruct=function() {
    this.selectedImages=new Array();
    this.url="";
    this.filename="";
}
imageStruct.prototype.remove=function(relativeUrl) {
    var j=-1;
    for(var i=0;i<this.selectedImages.length;i++)
    {
        if(this.selectedImages[i]==relativeUrl)
        {    
            j=i;
            break;
        }
    }
    if(j!=-1)
    {
        this.selectedImages.splice(j,1);
    }
}

imageStruct.prototype.hasItem=function(){ return this.selectedImages.length>0;}
imageStruct.prototype.add=function(relativeurl,url){ this.url=url;this.selectedImages.push(relativeurl); }
imageStruct.prototype.toString=function(){ return this.selectedImages.toString();}

var images=new imageStruct();
var imageurl;
function select(sender, path, imageurl) {
        if(sender.selected) {
            sender.className = "thumb-entry";
           sender.selected = 0;
           images.remove(path);
       }
        else {
            sender.selected = 1;
            sender.className = "thumb-entry selected";
            images.add( path,imageurl);
        }
    $("btnOk").disabled= !images.hasItem();
    $("selected").value= images.toString();
    $("delete").disabled= !images.hasItem();
}

function chooseImage()
{
    panel.result=images;
    dialog.close();
}
</script>
</form>
<!--[/place]-->
<!--[/dialogmaster]-->
