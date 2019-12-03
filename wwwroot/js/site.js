/* html component ids */
const COLUMN_CATEGORY_TEXT = "text";      // text, str, char etc.
const COLUMN_CATEGORY_NUMERIC = "numeric";  // int, decimal, float, bit etc.
const COLUMN_CATEGORY_DATE = "date";      // date, datetime, time etc.

// column info (returned from MVC) properties
const COLUMN_INFO_NAME = "name";
const COLUMN_INFO_CATEGORY = "category";

// Operators
const OPERATOR_LIKE = "Like";
const OPERATOR_IN = "In";
const OPERATOR_NOT_IN = "Not In";
const OPERATOR_IS_NULL = "Is Null";
const OPERATOR_IS_NOT_NULL = "Is Not Null";

// UI elements
var mainTableSelect = '#mainTableSelect';
var columnSelect = '#columnSelect';
var operatorSelect = '#operatorSelect';
var columnFilterText = '#columnFilterText';
var filterCriteriaTextArea = '#filterCriteriaTextArea';
var topNText = "topNText";

// jquery datatables
var mainDataTableId = "#mainDataTable";
var childDataTableId = "#childDataTable";
var parentDataTableId = "#parentDataTable";

var category;

// page global variables
var columnInfoArr;     // create placeholder
var allOperators;       // all possible operators

/* MVC urls */
var initialScreenDataUrl = '/Home/InitialScreenData';
var getColumnsUrl = "/Home/GetColumns";
var mainTableDataGetUrl = "/Home/MainTableDataFetch";

$(document).ready(function () {

    GetJsonAsync(initialScreenDataUrl, null, SetupInitialScreen);

    //$(document).on("click", "tr[role='row']", function () {
    //    //alert($(this).children('td:first-child').text());
    //    $(this).toggleClass('selected');
    //});

    //$(tableName + ' tbody').on('click', 'tr', function () {
    //    $(this).toggleClass('selected');
    //});

});

function SetupInitialScreen(data, textStatus, xhr) {
    FillSelect($(mainTableSelect), data.allTables);
    columnInfoArr = data.columns;
    allOperators = data.operators;
    FillSelectByProperty($(columnSelect), data.columns, COLUMN_INFO_NAME);
    FillSelect($(operatorSelect), data.operators);

    // set gui for first column
    //columnSelectionChanged(columnInfoArr[0][COLUMN_INFO_NAME]);
    columnSelectionChanged($(columnSelect).val());
}

function OperatorChanged() {
    SetColumnFilterTextDisabled($(operatorSelect).val());
}

function ToSQLInCompatible(value, isNumeric) {
    // converts comma delimited to IN comatible
    // e.g. for numeric 4,7,9 it should return (4,7,9). For text value jack,larry,ken it should return ('jack','larry','ken')
    var arr = value.split(',');
    var result = "";
    var currentVal = "";
    $.each(arr, function (i, element) {
        currentVal = (isNumeric ? element.trim() : "'" + element.trim() + "'");
        result = result + (result === "" ? "" : ",") + currentVal;
    });
    return "(" + result + ")";
}
function AddFilter() {
    var newFilter = "";
    var filterCriteria = $(filterCriteriaTextArea).val();
    var selectedOperator = $(operatorSelect).val();
    var filterText = $(columnFilterText).val();

    if (selectedOperator === OPERATOR_IS_NULL || selectedOperator === OPERATOR_IS_NOT_NULL) {
        newFilter = $(columnSelect).val() + " " + selectedOperator;
    } else if (selectedOperator === OPERATOR_IN || selectedOperator === OPERATOR_NOT_IN) {
        filterText = ToSQLInCompatible(filterText, category === COLUMN_CATEGORY_NUMERIC);
        newFilter = $(columnSelect).val() + " " + selectedOperator + " " + filterText;
    } else if (selectedOperator === OPERATOR_LIKE) {
        if (filterText.indexOf("%") < 0) {
            filterText = filterText + "%";   // add % if user did not entered, otherwise leave it alone
        }
        filterText = "'" + filterText + "'";
        newFilter = $(columnSelect).val() + " " + selectedOperator + " " + filterText;
    }
    else {
        if (category !== COLUMN_CATEGORY_NUMERIC) {
            filterText = "'" + filterText + "'";
        }
        newFilter = $(columnSelect).val() + " " + selectedOperator + " " + filterText;
    }

    filterCriteria = filterCriteria + (filterCriteria === "" ? "" : " AND ") + newFilter;
    $(filterCriteriaTextArea).val(filterCriteria);
}

//function AddFilter() {
//    var newFilter = $(columnFilterText).val();
//    if ($(columnSelect).val() && $(operatorSelect).val() && newFilter) {

//        var data = { table: $(mainTableSelect).val(), column: $(columnSelect).val(), filterOperator: $(operatorSelect).val(), newFilter: newFilter, currentFilters: $(filterCriteriaText).val() };

//        var result = GetJsonSync(addFilterUrl, data);
//        $(filterCriteriaText).val(result);
//    }
//    else {
//        alert("Column/Operator/Filter not selected");
//    }
//}


function tableChanged(newTable) {
    var data = { table: $(mainTableSelect).val() };
    var result = GetJsonSync(getColumnsUrl, data);
    columnInfoArr = result.columns;
    FillSelectByProperty($(columnSelect), columnInfoArr, COLUMN_INFO_NAME);
    columnSelectionChanged($(columnSelect).val());
    //$(filterCriteriaTextArea).val("");

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

// get main table data using main table and filter criteria
function GetMainTableData() {
    var model = { table: $(mainTableSelect).val(), criteria: $(filterCriteriaTextArea).val(), topN: $(topNText).val() };
    ClearJQTableHeader(mainDataTableId);
    FillJQTable(mainDataTableId, mainTableDataGetUrl, model);
}

function ClearJQTableHeader(tableName) {
    // clear datatable header row before fill again
    // assumed table html will always have "<thead><tr></tr></thead>" initially

    var table;
    if ($.fn.dataTable.isDataTable(tableName)) {
        table = $(tableName).DataTable();
        table.destroy();
        $(tableName).find("thead").find("tr").empty();
        $(tableName).find("tbody").empty();
    }
    else {
        $(tableName).on("click", "tr[role='row']", function () {
            //alert($(this).children('td:first-child').text());
            $(this).toggleClass('selected');
        });
    }
}

// NOT WORKING
function SetRowSelectEvent(tableName, table) {
    $(tableName + ' tbody').on('click', 'tr', function () {
        if ($(this).hasClass('selected')) {
            $(this).removeClass('selected');
        }
        else {
            table.$('tr.selected').removeClass('selected');
            $(this).addClass('selected');
        }
    });
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
                    "columns": result.columns,
                    select: true
                    //,
                    //"initComplete": function () {
                    //    $(document).on("click", "tr[role='row']", function () {
                    //        alert($(this).children('td:first-child').text());
                    //    });
                    //}
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

function FillSelectByProperty(selectObj, json, property) {
    // fills select using json
    $(selectObj).empty();
    $.each(json, function (i, element) {
        selectObj.append($('<option></option>').attr('value', element[property]).text(element[property]));
    });
}
function FillSelect(selectObj, json) {
    // fills select using json
    $(selectObj).empty();
    $.each(json, function (i, value) {
        selectObj.append($('<option></option>').attr('value', value).text(value));
    });
}

// searches array resturns required property
function SearchArray(arr, valueToSearch, searchProperty, returnProperty) {
    // fills select using json
    var foundElement = arr.find(obj => obj[searchProperty] === valueToSearch)[returnProperty];

    return foundElement;
}

function columnSelectionChanged(val) {
    SetupWhenColumnChanged(val);
    SetColumnFilterTextDisabled($(operatorSelect).val());
}

function SetupWhenColumnChanged(colName) {
    // find data type
    category = SearchArray(columnInfoArr, colName, COLUMN_INFO_NAME, COLUMN_INFO_CATEGORY);
    var newCopy = JSON.parse(JSON.stringify(allOperators));
    if (category !== COLUMN_CATEGORY_TEXT) {
        // remove 'like'
        RemoveArrayElement(newCopy, OPERATOR_LIKE);
    }
    FillSelect($(operatorSelect), newCopy);
}

function RemoveArrayElement(array, element) {
    var index = array.indexOf(element);
    if (index > -1) {
        array.splice(index, 1);
    }
}

function SetColumnFilterTextDisabled(selectedOperator) {
    var shouldDisabled = selectedOperator === OPERATOR_IS_NULL || selectedOperator === OPERATOR_IS_NOT_NULL;
    $(columnFilterText).val("");
    $(columnFilterText).prop("disabled", shouldDisabled);
}
