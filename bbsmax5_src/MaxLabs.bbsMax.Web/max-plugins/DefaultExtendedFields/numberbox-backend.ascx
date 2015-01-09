<!--[Page Inherits="MaxLabs.bbsMax.Web.ExtendedFieldSettingControlBase" /]-->
        <tr>
			<th><h4>数字范围</h4>
			<input type="text" class="text number" name="minvalue" value="$out($field.Settings['minvalue'])" /> -
			<input type="text" class="text number" name="maxvalue" value="$out($field.Settings['maxvalue'])" />
			</th>
			<td>设置当前项允许用户输入的最小值和最大值（为空则默认为0到最大值）(<font color="red">这里不是填写长度，例如100-203指用户只能输入100到203之间的数字，包括100和203</font>)</td>
        </tr>