$(document).ready(function () {
    $("#myInput1").on("keyup", function () {
        var value = $(this).val().toLowerCase();
        $("#SelectList1 option").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });
    $("#myInput2").on("keyup", function () {
        var value = $(this).val().toLowerCase();
        $("#SelectList2 option").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });
    $("#right").click(function () {
        var elem = $('#SelectList1').find(":selected");
        if (elem != null) {
            $('#SelectList1').find(":selected").remove();
            $('#SelectList2').append(elem);
            $('#Selected').append('<input id="prodId" name="prodId" type="hidden" value="' + elem.val() + '">');
            sortHidden();
        }
    });
    $("#left").click(function () {
        var elem = $('#SelectList2').find(":selected");
        if (elem != null) {
            $('#SelectList2').find(":selected").remove();
            $('#SelectList1').append(elem);
            console.log(elem.val());
            $("#Selected>input[value='" + elem.val() + "']").remove();
            sortHidden();
        }
    });
});
function sortHidden() {
    var list = $('#Selected').children();
    for (var i = 0; i < list.length; i++) {
        list[i].id = 'property[' + i + ']';
        console.log(list[i]);
    }
}