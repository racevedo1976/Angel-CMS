// comment
function navMenuSelectionGrid_DataBound() {
    var menuId = $("#navMenuForm").find("input[name=NavMenuId]").val() || null;

    $("#navMenuSelectionGrid").find("[value=" + menuId + "]:checkbox").attr("checked", "checked");
    
    $("#navMenuSelectionGrid").find(":checkbox").on("click", function (event) {
        var $chk = $(this),
            isChecked = $chk.is(":checked"),
            id = $chk.val();

        if (isChecked) {
            $("#navMenuSelectionGrid").find(":checkbox").not($chk).removeAttr("checked");           
        }
      
        $("#navMenuForm").find("input[name=NavMenuId]").val(isChecked ? id : undefined);
    });


}
