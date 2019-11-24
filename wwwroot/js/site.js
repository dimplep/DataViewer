// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {

});

function GetParentData() {
    var model = {
        Name: "James",
        Location: "London"
    };

    var tableName = '#parentDataTable';
    var url = '/Home/GetParentTables';

    // clear table header
    ClearJQTableHeader(tableName);

    //fill data table
    FillJQTable(tableName, url, model);
}

function ClearJQTableHeader(tableName) {
    // clear datatable header row before fill again
    // assumed table html will always have "<thead><tr></tr></thead>" initially

    var table;
    if ($.fn.dataTable.isDataTable(tableName)) {

        table = $(tableName).DataTable();
        table.destroy();
        $(tableName).find("thead").find("tr").empty();
    }
}

// fill jQuery datatable using passed table name, url and model
function FillJQTable(tableName, url, model) {
    $.ajax({
        type: "GET",
        url: url,
        data: model,
        dataType: "json",
        success: function (result) {
            var str;

            $.each(result.columns, function (k, colObj) {
                str = '<th>' + colObj.name + '</th>';
                $(str).appendTo(tableName + '>thead>tr');
            });

            $(tableName).DataTable(
                {
                    "data": result.data,
                    "columns": result.columns
                }
            );
        },
        error: function (error) {
            alert('error');
            alert(error.toString());
        }
    });
}