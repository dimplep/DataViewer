/* html component ids */
var mainTableSelect = '#mainTableSelect';
var columnSelect = '#columnSelect';
var operatorSelect = '#operatorSelect';


/* MVC urls */
var initialScreenDataUrl = '/Home/InitialScreenData';

$(document).ready(function () {

    GetJsonAsync(initialScreenDataUrl, null, SetupInitialScreen);

});

function SetupInitialScreen(data, textStatus, xhr) {
    FillSelect($(mainTableSelect), data.allTables);
    FillSelect($(columnSelect), data.columns);
    FillSelect($(operatorSelect), data.operators);
}

//function FillMainSelect(data, textStatus, xhr) {
//    FillSelect($('#mainTableSelect'), data);
//}

//function GetParentData() {
//    var tableName = '#parentDataTable';
//    var url = '/Home/GetParentTables';

//    // clear table header
//    ClearJQTableHeader(tableName);

//    //fill data table
//    FillJQTable(tableName, url, model);
//}

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
            alert("ERROR: " + error);
        }
    });
}

function GetJsonAsync(url, data, callback) {
    $.ajax({
        type: "GET",
        url: url,
        data: data,
        dataType: "json",
        success: callback,
        error: function (error) {
            return "ERROR";
        }
    });
}

function GetJsonSync(url, data) {
    var result;

    $.ajax({
        type: "GET",
        url: url,
        data: data,
        async: false,
        dataType: "json",
        success: function (jsonResult) {
            result = jsonResult;
        },
        error: function (error) {
            return "ERROR";
        }
    });

    return result;
}

function FillSelect(selectObj, json) {
    // fills select using json
    $(selectObj).empty();
    $.each(json, function (i, value) {
        selectObj.append($('<option></option>').attr('value', value).text(value));
    });
}