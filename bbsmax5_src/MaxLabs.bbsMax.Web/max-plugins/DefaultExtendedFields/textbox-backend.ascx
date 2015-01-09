<!--[Page Inherits="MaxLabs.bbsMax.Web.ExtendedFieldSettingControlBase" /]-->
        <tr>
			<th><h4>内容长度</h4>
			<input type="text" class="text number" name="minlength" value="$out($field.Settings['minlength'])" /> -
			<input type="text" class="text number" name="maxlength" value="$out($field.Settings['maxlength'])" />
			</th>
			<td>设置当前项允许用户输入的最小和最大字符长度（一个全角字符的长度是2，一个半角字符的长度是1,不填则不限制）</td>
        </tr>