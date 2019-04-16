/* This code is based off of the Unobtrusive Ajax dropdown list
** see Responsible Coder article:
** http://responsiblecoder.com/2011/06/asp-net-mvc3-app-part-2-ajax-cascading-dropdown/
*/

jQuery(document).ready(function () {
    initializeAjaxSelect();
});

function initializeAjaxSelect() {
    try {
        $(document).on("change", "select", function () {
            try {
                var master = $(this).attr('id');
                var id = $(this).val();
                processAjaxSelectChange(master, id);
            }
            catch (e){
                Debug(e.message);
            }
        });
    }
    catch (e){
        alert(e.message);
    }
}

function processAjaxSelectChange(master, selectedId)
{
    if (master && selectedId) {
        $("select[angelo-dropdown-dependson='" + master + "'][angelo-dropdown-loadfrom]").each(function () {
            var target = $(this);
            var dependson = target.attr('angelo-dropdown-dependson');
            if (master == dependson) {
                var url = target.attr('angelo-dropdown-loadfrom');
                var formData = { id: selectedId };
                var optionLabel = target.attr('angelo-dropdown-option-label');
                target.html("");
                $.ajax({
                    type: 'POST',
                    url: url,
                    data: formData,
                    success: function (data, textStatus) {
                        if (data) {
                            if (optionLabel) {
                                target.append($("<option></option>").attr("value", "").text(optionLabel));
                            }
                            $(data.items).each(function () {
                                target.append($("<option></option>").attr("value", this.value).text(this.text));
                            });
                            target.change();
                        }
                    },
                    dataType: 'json'
                });
            }
            else {
                target.change();
            }
        });
    }
}








function selectFromAjax(url, formData, target) {
    $(target).html("");
    if (formData.id) {
        $.ajax({
            type: 'POST',
            url: url,
            data: formData,
            success: function (data, textStatus) {
                if (data) {
                    $(data.items).each(function () {
                        $(target).append($("<option></option>").attr("value", this.Value).text(this.Text));
                    });
                    $(target).change();
                }
            },
            dataType: 'json'
        });
    }
    else {
        $(target).change();
    }
}

