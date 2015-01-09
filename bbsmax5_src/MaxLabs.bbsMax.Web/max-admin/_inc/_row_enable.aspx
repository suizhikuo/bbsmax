<tr>
    <th>
        <h4>$title</h4>
	    <p>
	        <input type="radio" name="$id" id="$id1" value="true" $_form.checked('$id','true',$checked) />
	        <label for="$id1">是</label>
        </p>
        <p>
            <input type="radio" name="$id" id="$id2" value="false" $_form.checked('$id','false',!$checked) />
            <label for="$id2">否</label>
        </p>
    </th>
    <td>$note</td>
</tr>