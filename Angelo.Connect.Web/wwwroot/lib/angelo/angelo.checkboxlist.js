
AngeloCheckboxList = function (id) {

    var control = document.getElementById(id);
    var containerName = id + "_container";

    function Selected(index, val) {
        var dataEl = document.getElementById(id + "__" + index + "_Selected");
        if (val) {
            var checkboxEl = document.getElementById(id + "__" + index + "_checkbox");
            checkboxEl.checked = val;
            dataEl.value = val;
            return val;
        }
        return dataEl.value;
    }

    function Value(index, val) {
        var dataEl = document.getElementById(id + "__" + index + "_Value");
        if (val) {
            var checkboxEl = document.getElementById(id + "__" + index + "_checkbox");
            checkboxEl.value = val;
            dataEl.value = val;
            return val;
        }
        return dataEl.value;
    }

    function Text(index, val) {
        var dataEl = document.getElementById(id + "__" + index + "_Text");
        if (val) {
            var labelEl = document.getElementById(id + "__" + index + "_label");
            labelEl.innerHTML = val;
            dataEl.value = val;
            return val;
        }
        return dataEl.value;
    }

    function Count() {
        return control.dataset.angeloCtrCount;
    }

    function IndexOfValue(val) {
        for (var index = Count() - 1; index > -1; index--) {
            if (Value(index) == val) {
                return index;
            }
        }
        return -1;
    }

    function ClearItems() {
        var container = document.getElementById(containerName);
        while (container.firstChild) {
            container.removeChild(container.firstChild);
        }
        control.dataset.angeloCtrCount = 0;
    }

    function AddItem(text, value, selected) {
        var index = control.dataset.angeloCtrCount;
        control.dataset.angeloCtrCount = parseInt(index, 10) + 1;

        var idPrefix = id + "__" + index + "_";

        var modelSelected = document.createElement("input");
        modelSelected.id = idPrefix + "Selected";
        modelSelected.value = selected;
        modelSelected.hidden = true;

        var modelText = document.createElement("input");
        modelText.id = idPrefix + "Text";
        modelText.value = text;
        modelText.hidden = true;

        var modelValue = document.createElement("input");
        modelValue.id = idPrefix + "Value";
        modelValue.value = value;
        modelValue.hidden = true;

        if (control.dataset.angeloCtrItemListName) {
            var namePrefix = control.dataset.angeloCtrItemListName + "[" + index + "].";
            modelSelected.name = namePrefix + "Selected";
            modelText.name = namePrefix + "Text";
            modelValue.name = namePrefix + "Value";
        }

        var checkbox = document.createElement("input");
        checkbox.type = "checkbox";
        checkbox.id = idPrefix + "checkbox";
        if (control.dataset.angeloCtrValueListName) {
            checkbox.name = control.dataset.angeloCtrValueListName;
        }
        checkbox.value = value;
        if (selected) {
            checkbox.checked = "checked";
        }
        checkbox.dataset.angeloCtrParentId = id;
        checkbox.dataset.angeloCtrIndex = index;
        checkbox.dataset.angeloCtrTargetId = modelSelected.id;
        checkbox.onchange = function () { AngeloCheckboxListOnChange(this); };

        var label = document.createElement("label");
        label.id = idPrefix + "label";
        label.htmlFor = checkbox.id;
        label.appendChild(document.createTextNode(text));

        var colDiv = document.createElement("div");
        colDiv.className = "col-sm-12";
        colDiv.appendChild(modelSelected);
        colDiv.appendChild(modelText);
        colDiv.appendChild(modelValue);
        colDiv.appendChild(checkbox);
        colDiv.appendChild(label);

        var rowDiv = document.createElement("div");
        rowDiv.className = "row";
        rowDiv.id = idPrefix;
        rowDiv.appendChild(colDiv);

        var el1 = document.getElementById(containerName);
        el1.appendChild(rowDiv);
    }

    function Fetch(data) {
        $.ajax({
            type: control.dataset.angeloCtrAction,
            url: control.dataset.angeloCtrUrl,
            data: data,
            success: function (result) {
                ClearItems();
                for (var i = 0; i < result.length; i++) {
                    AddItem(result[i].Text, result[i].Value, result[i].Selected);
                }
            }
        });
    }

    return {
        name: id,
        selected: Selected,
        text: Text,
        value: Value,
        count: Count,
        indexOfValue: IndexOfValue,
        clearItems: ClearItems,
        addItem: AddItem,
        fetch: Fetch
    }
}

function AngeloCheckboxListOnChange(checkbox) {
    var selected = document.getElementById(checkbox.dataset.angeloCtrTargetId);
    if (checkbox.checked)
        selected.value = "true"
    else
        selected.value = "false";
    var parentId = checkbox.dataset.angeloCtrParentId;
    var index = checkbox.dataset.angeloCtrIndex;
    $.trigger(parentId + ".change", index, checkbox.checked, checkbox.value);
}



