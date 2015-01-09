<!--[if $index==0]-->
<table style="margin-bottom:1px;">
<!--[error line="$index" name = "$Name"]-->
<tr class="nohover" id="error_{=$name}_tr_$index">
    <td colspan="2" class="Message">
        <div class="Tip Tip-error">$Message</div>
        <div class="TipArrow">&nbsp;</div>
    </td>
</tr>
<!--[/error]-->
<tr>
	<th>
	    <div class="itemtitle">
	        <strong>$title</strong>
	        <a class="addexceptrule" href="#" id="except_tr_$name" onclick="exceptable_display('tr_$name',true,'{=$name}_exceptrule',$ItemCount); return false;">添加例外</a>
	        <input type="hidden" value="$index" name="id_$name" />
	    </div>
	    <p>
	        <input type="text" class="text number" name="{=$name}_value_$index" value="$_form.text('{=$name}_value_$index',$GetFileSize($item.Value))" />
	        <select name="{=$name}_filesize_$index">
	        <option value="M" $_form.selected("{=$name}_filesize_$index","M","$GetFileSizeUnit($item.Value)")>MB</option>
	        <option value="K" $_form.selected("{=$name}_filesize_$index","K","$GetFileSizeUnit($item.Value)")>KB</option>
	        <option value="B" $_form.selected("{=$name}_filesize_$index","B","$GetFileSizeUnit($item.Value)")>B</option>
	        <option value="G" $_form.selected("{=$name}_filesize_$index","G","$GetFileSizeUnit($item.Value)")>GB</option>
	        <option value="T" $_form.selected("{=$name}_filesize_$index","T","$GetFileSizeUnit($item.Value)")>TB</option>
	        </select>
	    </p>
<!--[/if]-->
        
        <!--[if $index==0]-->
        <div class="exceptrule" id="{=$name}_exceptrule">
	    <table>
	    <!--[else]-->
        <tbody id="{=$name}_tr_$index">
        <!--[error name = "new_$name"]-->
        <tr id="error_{=$name}_tr_$index">
            <td colspan="3" class="error">
                <div class="minitip minitip-error">$Message<span class="arrow">&nbsp;</span></div>
            </td>
        </tr>
        <!--[/error]-->
        <tr>
	        <td>
	            <h4>
	            “$GetRoleName($item.RoleId)”
	            <!--[if $item.LevelStatus == LevelStatus.Currently]--><!--[else if $item.LevelStatus == LevelStatus.Above]-->及以上级别<!--[else]-->及以下级别<!--[/if]-->
	            <input type="hidden" name="{=$name}_roleIDs" value="$item.RoleId" />
	            <input type="hidden" name="{=$name}_role_$index" value="$Item.RoleId" />
	            <input type="hidden" name="{=$name}_level_$index" value="$Item.LevelStatus" />
	            <input type="hidden" value="$index" name="id_$name" />
	            <input type="hidden" value="$_form.text('delete_{=$name}_$index',0)" name="delete_{=$name}_$index" />
	            <script type="text/javascript">
                    if(document.getElementsByName('delete_{=$name}_$index')[0].value=='1')
                        $('{=$name}_tr_$index').style.display='none';
                </script>
                </h4>
                <input type="text" class="text number" name="{=$name}_value_$index" value="$_form.text('{=$name}_value_$index',$GetFileSize($item.Value))" />
                <select name="{=$name}_filesize_$index">
                    <option value="M" $_form.selected("{=$name}_filesize_$index","M","$GetFileSizeUnit($item.Value)")>MB</option>
                    <option value="K" $_form.selected("{=$name}_filesize_$index","K","$GetFileSizeUnit($item.Value)")>KB</option>
                    <option value="B" $_form.selected("{=$name}_filesize_$index","B","$GetFileSizeUnit($item.Value)")>B</option>
                    <option value="G" $_form.selected("{=$name}_filesize_$index","G","$GetFileSizeUnit($item.Value)")>GB</option>
                    <option value="T" $_form.selected("{=$name}_filesize_$index","T","$GetFileSizeUnit($item.Value)")>TB</option>
                </select>
	        </td>
	        <td>
                <p>排序</p>
                <input type="text" class="text order" name="{=$name}_sortorder_$index" value="$_form.text('{=$name}_sortorder_$index',$item.sortorder)" />
		    </td>
            <td class="actions">
                <a href="#" onclick="exceptable_deleteItem('{=$name}_tr_$index','delete_{=$name}_$index');return false;">删除</a>
            </td>
		</tr>
		</tbody>
	    <!--[/if]-->
		
		<!--[if $index==($ItemCount-1)]-->
		<tbody id="tr_$name">
		<!--[error name = "new_$name"]-->
        <tr id="error_tr_$name">
            <td colspan="3" class="error">
                <div class="minitip minitip-error">$Message<span class="arrow">&nbsp;</span></div>
            </td>
        </tr>
        <!--[/error]-->
		<tr>
	        <td>
	            <h4>
	               <!--[include src="_exceptableitem_roles_.ascx"]-->
	            </h4>
	            <input type="text" class="text number" name="new_{=$name}_value" value="$_form.text('new_{=$name}_value')" />
                <select name="new_{=$name}_filesize">
                <option value="M" $_form.selected("new_{=$name}_filesize","M","$GetFileSizeUnit($item.Value)")>MB</option>
                <option value="K" $_form.selected("new_{=$name}_filesize","K","$GetFileSizeUnit($item.Value)")>KB</option>
                <option value="B" $_form.selected("new_{=$name}_filesize","B","$GetFileSizeUnit($item.Value)")>B</option>
                <option value="G" $_form.selected("new_{=$name}_filesize","G","$GetFileSizeUnit($item.Value)")>GB</option>
                <option value="T" $_form.selected("new_{=$name}_filesize","T","$GetFileSizeUnit($item.Value)")>TB</option>
                </select>
	        </td>
	        <td>
                <p>排序</p>
                <input type="text" class="text order" name="new_{=$name}_sortorder" value="$_form.text('new_{=$name}_sortorder')" />
            </td>
            <td class="actions">
                <a href="#" onclick="exceptable_cancle('tr_$name','new_{=$name}_role','{=$name}_exceptrule',$ItemCount);return false;">取消</a>
            </td>
        </tr>
		</tbody>
		</table>
        </div>
		<!--[/if]-->

<!--[if $index==($ItemCount-1)]-->
	</th>
	<td>
	    <p>$Description</p>
	    <!--[if $hasNode]-->
	    <p>
		    <input type="checkbox" name="{=$name}_aplyallnode" id="{=$name}_aplyallnode" value="true" $_form.checked('{=$name}_aplyallnode','true') />
		    <label for="{=$name}_aplyallnode">应用到所有$NodeName</label>
		</p>
		<!--[/if]-->
	</td>
</tr>
</table>
<script type="text/javascript">
    exceptable_load('tr_$name','{=$name}_exceptrule',$ItemCount);
</script>
<!--[/if]-->
