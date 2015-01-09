<input type="hidden" id="$field.key" name="$field.key" value="$value" />
<!--[if CommonScriptOutputed == false]-->
<script type="text/javascript">
	var optionValueRegex = /'([^'\\\r\n]*(?:\\.[^'\\\r\n]*)*)'/g;

	function CreateSelectGroup(targetID, data, maxlevel) {

		var target = document.getElementById(targetID);
            
		var level0 = document.createElement('select'); level0.style.marginRight = '4px'; target.parentNode.insertBefore(level0, target);
		var level1 = document.createElement('select'); level1.style.marginRight = '4px'; if (maxlevel > 0) target.parentNode.insertBefore(level1, target);
		var level2 = document.createElement('select'); level2.style.marginRight = '4px'; if (maxlevel > 1) target.parentNode.insertBefore(level2, target);
		level0.id = targetID + "0";
		level1.id = targetID + "1";
		level2.id = targetID + "2";
		var emptyOption0 = document.createElement('option'); emptyOption0.innerHTML = '--请选择--'; level0.appendChild(emptyOption0);
		var emptyOption1 = document.createElement('option'); emptyOption1.innerHTML = '--请选择--'; level1.appendChild(emptyOption1);
		var emptyOption2 = document.createElement('option'); emptyOption2.innerHTML = '--请选择--'; level2.appendChild(emptyOption2);

		for (var key in data) {
			var option = document.createElement('option'); option.value = "'" + key.replace("'", "\\'") + "'"; option.innerHTML = key; option.data = data[key];

			level0.appendChild(option);
		}

		var matches = target.value.match(optionValueRegex);


		if (matches) {

			for (var j = 1; j < level0.options.length; j++) {
			    if (level0.options[j].value == HTMLEncode(matches[0])) {
			        level0.options[j].selected = true;
					
					var theData = level0.options[j].data;
					for (var key in theData) {
					    var option = document.createElement('option');
					    option.value = "'" + key.replace("'", "\\'") + "'";
					    option.innerHTML = key; 
						option.data = theData[key];

						level1.appendChild(option);
					}

					break;
				}
			}

			if (level1.options.length > 1 && matches.length > 1) {

				for (var j = 1; j < level1.options.length; j++) {
					if (level1.options[j].value == matches[1]) {
					    //level1.selectedIndex = j;
					    setTimeout("$('"+level1.id+"').options["+j+"].selected = true",10); //为解决IE6下的兼容性问题（IE6直接设置selected=true出异常）
						var theData = level1.options[j].data;

						for (var key = 0; key < theData.length; key++) {
						    var option = document.createElement('option');
						    option.value = "'" + theData[key].replace("'", "\\'") + "'";
						    option.innerHTML = theData[key];

						    level2.appendChild(option);
						}

						break;
					}
				}

				if (level2.options.length > 2 && matches.length > 2) {

					for (var j = 1; j < level2.options.length; j++) {
						if (level2.options[j].value == matches[2]) {
						    setTimeout("$('" + level2.id + "').options[" + j + "].selected = true", 10);
							break;
						}
					}
				}
			}
		}

		level0.onchange = function() {
			while (level1.options.length > 1) level1.remove(1);
			while (level2.options.length > 1) level2.remove(1);

			var theData = level0.options[level0.selectedIndex].data;

			for (var key in theData) {
				var option = document.createElement('option'); option.value = "'" + key.replace("'", "\\'") + "'"; option.innerHTML = key; option.data = theData[key];

				level1.appendChild(option);
			}

			target.value = "'" + level0.options[level0.selectedIndex].innerHTML + "'";
		}

		if (maxlevel > 0) {
		    level1.onchange = function() {
		        while (level2.options.length > 1) level2.remove(1);

		        var theData = level1.options[level1.selectedIndex].data;

		        for (var key = 0; key < theData.length; key++) {
		            var option = document.createElement('option'); option.value = "'" + theData[key].replace("'", "\\'") + "'"; option.innerHTML = theData[key];

		            level2.appendChild(option);
		        }

		        target.value = level0.options[level0.selectedIndex].value + "-" + level1.options[level1.selectedIndex].value;
		    }
		}

		if (maxlevel > 1) {
			level2.onchange = function() {
				target.value = level0.options[level0.selectedIndex].value + "-" + level1.options[level1.selectedIndex].value + "-" + level2.options[level2.selectedIndex].value;
			}
		}
	}
</script>
<!--[/if]-->
<script type="text/javascript">
	
	CreateSelectGroup('$field.Key', $SelectionDataToJson($field.settings['data']));
</script>