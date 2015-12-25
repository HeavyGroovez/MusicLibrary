function alertFiles() {
    $(function () {
        var filelist = $('#fileSelector').prop("files");
        var names = $.map(filelist, function (val) { return val.name; });
        $(names).each(function () {
            alert($(this).toString());
        }) // end each
    }); // End ready
}