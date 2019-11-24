// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    

    $.ajax({
        type: "POST",
        url: "/Home/GetParentTables",
        dataType: "json",
        success: function (result) {
            
            //var staticData = { "data": [{ "name1": "Johns" }, { "name1": "Kevin" }] };
            //var stringfiedData = JSON.parse(staticData.data);
            $("#parentDataTable").DataTable(
                {
                    "data": result.data,
                    "columns": [
                        { "data": "Person Name" },
                        { "data": "Age" },
                        { "data": "DateHire" },
                        ]
                }
            );
        },
        error: function (error) {
            alert('error');
            alert(error.toString());
        }
    });
})