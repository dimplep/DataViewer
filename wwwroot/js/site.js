/* backend data categories */
const COLUMN_CATEGORY_TEXT = "text";      // text, str, char etc.
const COLUMN_CATEGORY_NUMERIC = "numeric";  // int, decimal, float, bit etc.
const COLUMN_CATEGORY_DATE = "date";      // date, datetime, time etc.
const COLUMN_CATEGORY_BOOLEAN = "bool";      // bit
const PARENT_TABLE_FK_SEPARATOR = " -> ";    // parent table selection will contain fk name if multiple fk in child (e.g. shipping and billing address ids in shipping header)

// column info (returned from MVC) properties
const COLUMN_INFO_NAME = "name";
const COLUMN_INFO_CATEGORY = "category";

// Operators
const OPERATOR_LIKE = "Like";
const OPERATOR_IN = "In";
const OPERATOR_NOT_IN = "Not In";
const OPERATOR_IS_NULL = "Is Null";
const OPERATOR_IS_NOT_NULL = "Is Not Null";

// RELATION Types
const RELATION_PARENT = "Parent";
const RELATION_CHILD = "Child";

// UI elements
var mainEntitySelect = '#mainEntitySelect';
var columnSelect = '#columnSelect';
var operatorSelect = '#operatorSelect';
var columnFilterText = '#columnFilterText';
var filterCriteriaTextArea = '#filterCriteriaTextArea';
var topNText = "#topNText";
var parentEntitySelect = "#parentEntitySelect";
var childEntitySelect = "#childEntitySelect";
var hideIfNoDataCheck = '#hideIfNoDataCheck';
var childNavigateFromBtn = '#childNavigateFromBtn';
var parentNavigateFromBtn = '#parentNavigateFromBtn';


// jquery datatables
var mainDataTableId = "#mainDataTable";
var childDataTableId = "#childDataTable";
var parentDataTableId = "#parentDataTable";

// page global variables
//var columnInfoArr;     // stores main entity available columns for filter
var allOperators;       // all possible operators
var jqDtColArr = [ null, null, null];   // stores datatable column info into array (so primary key columns can be queried)
var jqDtNameArr = [mainDataTableId, childDataTableId, parentDataTableId];

/* MVC urls */
var initialScreenDataUrl = '/Home/InitialScreenData';
var getColumnsUrl = "/Home/GetColumns";
var mainEntityDataGetUrl = "/Home/MainEntityDataFetch";
var mainEntityRowSelectUrl = "/Home/MainEntityRowSelect";
var parentOrChildGetData = "/Home/ParentOrChildGetData";

$(document).ready(function () {

    getJsonAsync(initialScreenDataUrl, null, setupInitialScreen);

    $(document).on("click", "tr[role='row']", function (e) {
        if (typeof e.currentTarget._DT_RowIndex === 'undefined' || e.currentTarget._DT_RowIndex === null) {
            return;
        }
        //if (e.currentTarget._DT_RowIndex != null) {
        var tableId = '#' + $(this).closest('table').attr('id');

        if (tableId === mainDataTableId) {
            var selectedRowCount = $(tableId).DataTable().rows({ selected: true }).count();

            if (selectedRowCount > 0) {
                // if selected

                var colNameVals = selectedRowPkColAndValues(tableId);

                var data =
                {
                    table: $(mainEntitySelect).val(),
                    colNameVals: colNameVals,
                    hideChilEntitiesWhenNoData: $(hideIfNoDataCheck).prop("checked")
                };

                //FillJQTable(childDataTableId, mainTableRowSelectUrl, JSON.stringify(model));
                postJsonAsync(mainEntityRowSelectUrl, data, setupParentChildSections);
            }
            else {
                // unselected row
                $(parentEntitySelect).empty();
                $(childEntitySelect).empty();
            }

            // clear parent, child tables
            clearTable(childDataTableId);
            clearTable(parentDataTableId);
        }
        else {
            RefreshNavigationBtnForTable(tableId);
        }

    });

    //$(tableName + ' tbody').on('click', 'tr', function () {
    //    $(this).toggleClass('selected');
    //});

});

function navigateFromChild() {
    navigateFromParentOrChild(childDataTableId, childEntitySelect);
}

function navigateFromParent() {
    navigateFromParentOrChild(parentDataTableId, parentEntitySelect);
}

function navigateFromParentOrChild(tableId, selectId) {
    var newMainEntity = $(selectId).val();
    var index = newMainEntity.indexOf(PARENT_TABLE_FK_SEPARATOR);
    if (index > 0) {
        newMainEntity = newMainEntity.substring(index + PARENT_TABLE_FK_SEPARATOR.length, newMainEntity.length);
    }

    var pkCriteria = primaryKeyCriteriaForSelectedRow(tableId);
    $(filterCriteriaTextArea).val(pkCriteria);

    $(mainEntitySelect).val(newMainEntity);
    mainEntityChanged(newMainEntity, true);
    getMainEntityData();
}


// according to table row selected will refresh (enable/disable) navigation button
function RefreshNavigationBtnForTable(tableId) {
    var disable = true;
    if ($.fn.dataTable.isDataTable(tableId)) {
        disable = $(tableId).DataTable().rows({ selected: true }).count() <= 0;
    }

    var btnId = "";
    if (tableId === childDataTableId) {
        btnId = childNavigateFromBtn;
    }
    else {
        btnId = parentNavigateFromBtn;
    }
    DisableNavigationFromBtn(btnId, disable);
}

function DisableNavigationFromBtn(btnId, disable) {
    $(btnId).prop('disabled', disable);
}

// assumed a row is selected
function selectedRowPkColAndValues(tableId) {
    // loop through columns, find primary key values and create json
    var indexOfArr = jqDtNameArr.indexOf(tableId);
    var colArr = jqDtColArr[indexOfArr];
    var colNameVals = [];
    var selectedRow = $(tableId).DataTable().rows({ selected: true }).data()[0];
    for (var ii = 0; ii < colArr.length; ii++) {
        if (colArr[ii].isPrimaryKey) {
            colNameVals.push({
                colName: colArr[ii].name,
                //colValue: $(this)[0].cells[ii].textContent
                colValue: selectedRow[colArr[ii].name]
            });
        }
    }

    return colNameVals;
}

function setupParentChildSections(data, textStatus, xhr) {
    // fill parent / child table selects
    fillSelect($(parentEntitySelect), data.parentEntities, true);
    fillSelect($(childEntitySelect), data.childEntities, true);
}

function parentEntityChange(entity) {
    if (entity !== '') {

        var data =
        {
            fromEntity: $(mainEntitySelect).val(),
            toEntity: entity,
            toEntityType: RELATION_PARENT,
            keyVals: selectedRowPkColAndValues(mainDataTableId),
            topN: $(topNText).val()
        };

        postJsonAsync(parentOrChildGetData, data, setupParentDataTable);
    }
    else {
        clearTable(parentDataTableId);
    }
    RefreshNavigationBtnForTable(parentDataTableId);
}

function setupParentDataTable(data, textStatus, xhr) {
    fillJQTable(data, parentDataTableId);
}


function childEntityChange(entity) {
    //var selectedRowCount = $(mainDataTableId).DataTable().rows({ selected: true }).count();
    //var selectedRowData = $(mainDataTableId).DataTable().rows({ selected: true }).data()[0];
    //alert(selectedRowData['DepartmentID']);
    

    if (entity !== '') {
        var data =
        {
            fromEntity: $(mainEntitySelect).val(),
            toEntity: entity,
            toEntityType: RELATION_CHILD,
            keyVals: selectedRowPkColAndValues(mainDataTableId),
            topN: $(topNText).val()
        };

        postJsonAsync(parentOrChildGetData, data, setupChildDataTable);
    }
    else {
        clearTable(childDataTableId);
    }

    RefreshNavigationBtnForTable(childDataTableId);
}

function setupChildDataTable(data, textStatus, xhr) {
    fillJQTable(data, childDataTableId);
}

function setupInitialScreen(data, textStatus, xhr) {
    fillSelect($(mainEntitySelect), data.allTables);
    //columnInfoArr = data.columns;
    var indexOfArr = jqDtNameArr.indexOf(mainDataTableId);
    jqDtColArr[indexOfArr] = data.columns;

    allOperators = data.operators;
    fillSelectByProperty($(columnSelect), data.columns, COLUMN_INFO_NAME);
    fillSelect($(operatorSelect), data.operators);

    // set gui for first column
    //columnSelectionChanged(columnInfoArr[0][COLUMN_INFO_NAME]);
    columnSelectionChanged($(columnSelect).val());

    RefreshNavigationBtnForTable(childDataTableId);
    RefreshNavigationBtnForTable(parentDataTableId);
}

function operatorChanged() {
    setColumnFilterTextDisabled($(operatorSelect).val());
}

function toSQLInCompatible(value, isNumeric) {
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
function appendFilter() {
    var newFilter = "";
    var filterCriteria = $(filterCriteriaTextArea).val();
    var selectedColumn = $(columnSelect).val();
    var selectedOperator = $(operatorSelect).val();
    var filterText = $(columnFilterText).val();

    var indexOfArr = jqDtNameArr.indexOf(mainDataTableId);
    var category = searchArray(jqDtColArr[indexOfArr], selectedColumn, COLUMN_INFO_NAME, COLUMN_INFO_CATEGORY);

    filterCriteria = addCriteria(filterCriteria, selectedColumn, category, selectedOperator, filterText);
    $(filterCriteriaTextArea).val(filterCriteria);
}

function addCriteria(currentCriteria, selectedColumn, category, selectedOperator, filterText) {
    if (selectedOperator === OPERATOR_IS_NULL || selectedOperator === OPERATOR_IS_NOT_NULL) {
        newFilter = selectedColumn + " " + selectedOperator;
    } else if (selectedOperator === OPERATOR_IN || selectedOperator === OPERATOR_NOT_IN) {
        filterText = toSQLInCompatible(filterText, category === COLUMN_CATEGORY_NUMERIC);
        newFilter = selectedColumn + " " + selectedOperator + " " + filterText;
    } else if (selectedOperator === OPERATOR_LIKE) {
        if (filterText.indexOf("%") < 0) {
            filterText = filterText + "%";   // add % if user did not entered, otherwise leave it alone
        }
        filterText = "'" + filterText + "'";
        newFilter = selectedColumn + " " + selectedOperator + " " + filterText;
    }
    else {
        if (category !== COLUMN_CATEGORY_NUMERIC && category !== COLUMN_CATEGORY_BOOLEAN) {
            filterText = "'" + filterText + "'";
        }
        newFilter = selectedColumn + " " + selectedOperator + " " + filterText;
    }

    filterCriteria = currentCriteria + (currentCriteria === "" ? "" : " AND ") + newFilter;

    return filterCriteria;
}

// returns criteria for selected ientity row, assumed a row is selected
function primaryKeyCriteriaForSelectedRow(tableId) {
    var indexOfArr = jqDtNameArr.indexOf(tableId);
    var colArr = jqDtColArr[indexOfArr];
    var filterCriteria = "";

    var selectedRow = $(tableId).DataTable().rows({ selected: true }).data()[0];
    for (var ii = 0; ii < colArr.length; ii++) {
        if (colArr[ii].isPrimaryKey) {
            filterCriteria = addCriteria(filterCriteria, colArr[ii].name, colArr[ii].category, '=', selectedRow[colArr[ii].name]);
        }
    }

    return filterCriteria;
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


function mainEntityChanged(newTable, leaveCriteriaTextAlone) {

    var data = { table: $(mainEntitySelect).val() };
    var result = getJsonSync(getColumnsUrl, data);
    //columnInfoArr = result.columns;
    var indexOfArr = jqDtNameArr.indexOf(mainDataTableId);
    jqDtColArr[indexOfArr] = result.columns;

    fillSelectByProperty($(columnSelect), result.columns, COLUMN_INFO_NAME);
    columnSelectionChanged($(columnSelect).val());
    setSectionsAfterEntityChange(leaveCriteriaTextAlone);
}


function setSectionsAfterEntityChange(leaveCriteriaTextAlone) {
    if (!leaveCriteriaTextAlone) {
        $(filterCriteriaTextArea).val("");
    }
    clearParentChildSections();
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
function getMainEntityData() {
    var model = { table: $(mainEntitySelect).val(), criteria: $(filterCriteriaTextArea).val(), topN: $(topNText).val() };
    //clearJQTableHeader(mainDataTableId);
    var result = getJsonSync(mainEntityDataGetUrl, model);
    fillJQTable(result, mainDataTableId);
    clearParentChildSections();
}

function clearParentChildSections() {
    // clear parent/child selects, tables
    $(parentEntitySelect).empty();
    $(childEntitySelect).empty();
    clearTable(childDataTableId);
    clearTable(parentDataTableId);
}

function fillJQTable(result, tableName) {
    var str;

    var indexOfArr = jqDtNameArr.indexOf(tableName);
    jqDtColArr[indexOfArr] = result.columns;

    // clear prior table
    clearTable(tableName);

    $.each(result.columns, function (k, colObj) {
        str = '<th>' + colObj.name + '</th>';
        $(str).appendTo(tableName + '>thead>tr');
    });

    $(tableName).DataTable(
        {
            "data": result.data,
            "columns": result.columns,
            select: true,
            lengthMenu: [[5, 10, 25, -1], [5, 10, 25, "All"]]
            //destroy: true
        }
    );
}

function clearTable(tableName) {
    var table;
    if ($.fn.dataTable.isDataTable(tableName)) {
        table = $(tableName).DataTable();
        table.destroy();
        $(tableName).find("thead").find("tr").empty();
        $(tableName).find("tbody").empty();
    }
}

//function clearJQTableHeader(tableName) {
//    // clear datatable header row before fill again
//    // assumed table html will always have "<thead><tr></tr></thead>" initially

//    var table;
//    if ($.fn.dataTable.isDataTable(tableName)) {
//        table = $(tableName).DataTable();
//        table.destroy();
//        $(tableName).find("thead").find("tr").empty();
//        $(tableName).find("tbody").empty();
//    }
//    else {
//        $(tableName).on("click", "tr[role='row']", function () {
//            //alert($(this).children('td:first-child').text());
//            $(this).toggleClass('selected');
//        });
//    }
//}

function getJsonAsync(url, data, callback) {
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

function postJsonAsync(url, model, callback) {
    var result;

    $.ajax({
        type: "POST",
        url: url,
        data: JSON.stringify(model),
        dataType: 'json',
        contentType: "application/json;charset=utf-8",
        success: callback,
        error: function (error) {
            return "ERROR";
        }
    });

    return result;
}

function getJsonSync(url, data) {
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

function fillSelectByProperty(selectObj, json, property) {
    // fills select using json
    $(selectObj).empty();
    $.each(json, function (i, element) {
        selectObj.append($('<option></option>').attr('value', element[property]).text(element[property]));
    });
}
function fillSelect(selectObj, json, addBlank) {
    // fills select using json
    $(selectObj).empty();

    if (addBlank === true) {
        selectObj.append($('<option></option>').attr('value', '').text(''));
    }

    $.each(json, function (i, value) {
        selectObj.append($('<option></option>').attr('value', value).text(value));
    });
}

// searches array resturns required property
function searchArray(arr, valueToSearch, searchProperty, returnProperty) {
    // fills select using json
    var foundElement = arr.find(obj => obj[searchProperty] === valueToSearch)[returnProperty];

    return foundElement;
}

function columnSelectionChanged(val) {
    setupWhenColumnChanged(val);
    setColumnFilterTextDisabled($(operatorSelect).val());
}

function setupWhenColumnChanged(colName) {
    // find data type
    var indexOfArr = jqDtNameArr.indexOf(mainDataTableId);

    var category = searchArray(jqDtColArr[indexOfArr], colName, COLUMN_INFO_NAME, COLUMN_INFO_CATEGORY);
    var newCopy = JSON.parse(JSON.stringify(allOperators));
    if (category !== COLUMN_CATEGORY_TEXT) {
        // remove 'like'
        removeArrayElement(newCopy, OPERATOR_LIKE);
    }
    fillSelect($(operatorSelect), newCopy);
}

function removeArrayElement(array, element) {
    var index = array.indexOf(element);
    if (index > -1) {
        array.splice(index, 1);
    }
}

function setColumnFilterTextDisabled(selectedOperator) {
    var shouldDisabled = selectedOperator === OPERATOR_IS_NULL || selectedOperator === OPERATOR_IS_NOT_NULL;
    $(columnFilterText).val("");
    $(columnFilterText).prop("disabled", shouldDisabled);
}
