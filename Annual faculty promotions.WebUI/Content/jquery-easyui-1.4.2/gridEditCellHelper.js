var editNumCell = undefined;
var zdyBeforeEdit = undefined;
var comboboxValue = {};
//开启编辑单元格状态
function beginEditCell(target, options) {

    var opts = $.data(target, "datagrid").options;
    var tr = opts.finder.getTr(target, options.index);
    var row = opts.finder.getRow(target, options.index);

    //        //暂时还不知道该代码的含义,忽略使用
    //        if (tr.hasClass("datagrid-row-editing")) {
    //            return;
    //        }
    //        tr.addClass("datagrid-row-editing");

    _initCellEditor(target, options.index, options.field);
    _outerWidthOfEditable(target);
    //$.validateRow(target, options.index);暂时先不使用,不知道该方法作用
}

function _initCellEditor(target, _index, _field) {
    var opts = $.data(target, "datagrid").options;
    var tr = opts.finder.getTr(target, _index);
    var row = opts.finder.getRow(target, _index);

    tr.children("td").each(function () {
        var cell = $(this).find("div.datagrid-cell");
        var field = $(this).attr("field");

        if (field == _field) {//找到与传递参数相同field的单元格
            var col = $(target).datagrid("getColumnOption", field);
            if (col && col.editor) {
                var editorType, editorOp;
                if (typeof col.editor == "string") {
                    editorType = col.editor;
                } else {
                    editorType = col.editor.type;
                    editorOp = col.editor.options;
                }
                var editor = opts.editors[editorType];
                if (editor) {
                    var html = cell.html();
                    var outerWidth = cell._outerWidth();
                    cell.addClass("datagrid-editable");
                    cell._outerWidth(outerWidth);
                    cell.html("<table border=\"0\" cellspacing=\"0\" cellpadding=\"1\"><tr><td></td></tr></table>");
                    cell.children("table").bind(
                        "click dblclick contextmenu",
                        function (e) {
                            e.stopPropagation();
                        });
                    $.data(cell[0], "datagrid.editor", {
                        actions: editor,
                        target: editor.init(cell.find("td"),
                            editorOp),
                        field: field,
                        type: editorType,
                        oldHtml: html
                    });
                }
            }

            tr.find("div.datagrid-editable").each(function () {
                var field = $(this).parent().attr("field");
                var ed = $.data(this, "datagrid.editor");
                ed.actions.setValue(ed.target, row[field]);
            });
        }
    });
}

//为可编辑的单元格设置外边框
//来自jquery.easyui.1.8.0.js的 function _4d8()方法
function _outerWidthOfEditable(target) {
    var dc = $.data(target, "datagrid").dc;
    dc.view.find("div.datagrid-editable").each(function () {
        var _this = $(this);
        var field = _this.parent().attr("field");
        var col = $(target).datagrid("getColumnOption", field);
        _this._outerWidth(col.width);
        var ed = $.data(this, "datagrid.editor");
        if (ed.actions.resize) {
            ed.actions.resize(ed.target, _this.width());
        }
    });
}

//关闭编辑单元格状态
function endEditCell(target, options) {
    var opts = $.data(target, "datagrid").options;

    var updatedRows = $.data(target, "datagrid").updatedRows;
    var insertedRows = $.data(target, "datagrid").insertedRows;

    var tr = opts.finder.getTr(target, options.index);
    var row = opts.finder.getRow(target, options.index);

    //        //与beginEditCell相呼应,暂时取消
    //        if (!tr.hasClass("datagrid-row-editing")) {//行不能编辑时,返回
    //            return;
    //        }
    //        tr.removeClass("datagrid-row-editing");

    var _535 = false;
    var _536 = {};
    tr.find("div.datagrid-editable").each(function () {
        var _537 = $(this).parent().attr("field");
        var ed = $.data(this, "datagrid.editor");
        var _538 = ed.actions.getValue(ed.target);
        if (row[_537] != _538) {
            row[_537] = _538;
            _535 = true;
            _536[_537] = _538;
        }
    });
    if (_535) {
        if (_45f(insertedRows, row) == -1) {
            if (_45f(insertedRows, row) == -1) {
                updatedRows.push(row);
            }
        }
    }

    _destroyCellEditor(target, options);
    $(target).datagrid("refreshRow", options.index);
    opts.onAfterEdit.call(target, options.index, row, _536);
}

function _45f(a, o) {
    for (var i = 0, len = a.length; i < len; i++) {
        if (a[i] == o) {
            return i;
        }
    }
    return -1;
}

//销毁单元格编辑器
function _destroyCellEditor(target, options) {

    var opts = $.data(target, "datagrid").options;
    var tr = opts.finder.getTr(target, options.index);

    tr.children("td").each(function () {
        var field = $(this).attr("field");

        if (field == options.field) {//找到与传递参数相同field的单元格

            var cell = $(this).find("div.datagrid-editable");
            if (cell.length) {
                var ed = $.data(cell[0], "datagrid.editor");
                if (ed.actions.destroy) {
                    ed.actions.destroy(ed.target);
                }
                cell.html(ed.oldHtml);
                $.removeData(cell[0], "datagrid.editor");
                cell.removeClass("datagrid-editable");
                cell.css("width", "");
            }
        }
    });
}

$.extend($.fn.datagrid.methods, {
    beginEditCell: function (target, options) {
        return target.each(function () {
            beginEditCell(this, options);
        });
    },
    endEditCell: function (target, options) {
        return target.each(function () {
            endEditCell(this, options);
        });
    }
});

/**
 * 开启单击单元格
 * 
 */
var _rowIndex = undefined;
var _field = undefined;
var _rowId = undefined;
function onClickRow(rowIndex, field, value, id) {
    if (zdyBeforeEdit != undefined && $.isFunction(zdyBeforeEdit)) {
        zdyBeforeEdit(rowIndex, id);
    }
    var columnOption = $(id).datagrid("getColumnOption", field);
    if (columnOption && columnOption.readonly == true)
        return false;
    if (_rowIndex != rowIndex || _field != field) {
        if (endEditing(id)) {
            $(id).datagrid('selectRow', rowIndex);
            $(id).datagrid('beginEditCell', { index: rowIndex, field: field });
            var ed = $(id).datagrid('getEditor', { index: rowIndex, field: field });
            if (ed == null)
                return false;
            _rowIndex = rowIndex;
            _field = field;
            _rowId = id;

            $(".datagrid-editable-input").focus(function () {
                this.select();
            });
            $(ed.target).focus();

            var row = $(id).datagrid("getRowData", rowIndex);
            if (zdyCheckField != undefined && $.isFunction(zdyCheckField)) {
                zdyCheckField(row, field);
                $(".datagrid-editable-input")[0].select();
            }

            var changes = {};
            $(".datagrid-editable-input").blur(function () {
                if (ed.type != "combobox") {
                    _destroyCellEditor($(id)[0], { index: rowIndex, field: field });
                    var oldVal = row[field];
                    if (showNumValue != undefined) {
                        oldVal = showNumValue;
                    }
                    var newVal = this.value;
                    if (oldVal != newVal) {//值有改变
                        changes[field] = newVal;
                        row[field] = newVal;
                        changes["status"] = "P";
                        onAfterEditing(rowIndex, row, changes, id);
                    }
                    _rowIndex = undefined;
                    _field = undefined;
                } else {
                    comboboxValue.id = id;
                    comboboxValue.rowIndex = rowIndex;
                    comboboxValue.field = field;
                    changes['status'] = "p";
                    comboboxValue.row = row;
                    comboboxValue.changes = changes;
                    comboboxValue.value = $(".datagrid-editable-input").val();
                }
            });
            var rows = $(id).datagrid("getRows");
            $(ed.target).bind('keydown', function () {
                switch (window.event.keyCode) {
                    case 13:
                        $(this).blur();
                        endEditing(id);
                        event.returnValue = false;
                        if (rowIndex < rows.length - 1) {
                            if (editNumCell != undefined && $.isFunction(editNumCell) && field != "num") {
                                editNumCell(rowIndex, value, id);
                            } else {
                                onClickRow(rowIndex + 1, field, value, id);
                            }
                        }
                        break;
                    case 37:
                        $(this).blur();
                        //停止上一个框的编辑
                        endEditing(id);
                        //禁止默认键盘事件
                        event.returnValue = false;
                        var prevfiled = $(id).datagrid("prevColumn", field);
                        if (prevfiled != null && prevfiled.editor) {
                            onClickRow(rowIndex, prevfiled.field, value, id);
                        }
                        break;
                    case 38:
                        $(this).blur();
                        endEditing(id);
                        event.returnValue = false;
                        if (rowIndex > 0) {
                            onClickRow(rowIndex - 1, field, value, id);
                        }
                        break;
                    case 39:
                        $(this).blur();
                        endEditing(id);
                        event.returnValue = false;
                        var nextfiled = $(id).datagrid("nextColumn", field);
                        if (nextfiled != null && nextfiled.editor) {
                            onClickRow(rowIndex, nextfiled.field, value, id);
                        }
                        break;
                    case 40:
                        $(this).blur();
                        endEditing(id);
                        event.returnValue = false;
                        if (rowIndex < rows.length - 1) {
                            onClickRow(rowIndex + 1, field, value, id);
                        }
                        break;
                    case 27:
                        $(this).blur();
                        endEditing(id);
                        break;
                }
            });
        }
    }
    return true;
}
function endEditing(id) {
    if (_rowIndex == undefined || _field == undefined || id == undefined) {
        return true;
    }

    $(id).datagrid('endEditCell', { index: _rowIndex, field: _field });
    _rowIndex = undefined;
    _field = undefined;
    _rowId = undefined;
    return true;
}

function afterEdit(rowIndex, rowData, changes, id) {
    if (changes.status == "P") {
        $(id).datagrid('updateRow', {
            index: rowIndex,
            row: rowData
        });
        $(id).datagrid('acceptChanges');
    } else if (changes.status == "") {
        $(id).datagrid('updateRow', {
            index: rowIndex,
            row: rowData
        });
        $(id).datagrid('acceptChanges');
    }
}
function onAfterEditing(rowIndex, row, changes, id) {
    var saveData = save(rowIndex, row, changes, id);
    row = saveData[0];
    changes = saveData[1];

    afterEdit(rowIndex, row, changes, id);
}
/**
 * 得到combobox的编辑器
 */
function getEditorCombobox(url) {
    var editorCombobox = {
        type: 'combobox', options: {
            valueField: 'unitUid', textField: 'unitName', url: url, editable: true,
            onHidePanel: function () {
                $(".combo-text").focus();
            },
            onLoadSuccess: function (data) {
                $(".combo-text").unbind("keydown");
                $(".combo-text").bind("keydown", function () {
                    var key = window.event.keyCode;
                    _keyCode = key;
                    if ((key == 13 || key == 37 || key == 38 || key == 39 || key == 40 || key == 27) && _field != "" && _id != 0) {
                        var val = $(".datagrid-editable-input").val();
                        $(".combo-value").val(val);
                    }
                });

            }
        }
    };
    return editorCombobox;
}