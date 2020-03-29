var indexEditing = null;
$(document.ready(function() {
        $('#dgEducationReasearch').edatagrid({
        toolbar:
       [
           {
               text: 'جدید',
               iconCls: 'icon-add',
               handler: function () {
                   $('#dgEducationReasearch').edatagrid('addRow');
               }
           }, '-',
           {
               text: 'حذف',
               iconCls: 'icon-remove',
               handler: function () {
                   $('#dgEducationReasearch').edatagrid('destroyRow');
               }
           }, '-',
           {
               text: 'ذخیره',
               iconCls: 'icon-save',
               handler: function () {
                   $('#dgEducationReasearch').edatagrid('saveRow');
               }
           }, '-',
           {
               text: 'انصراف',
               iconCls: 'icon-undo',
               handler: function () {
                   $('#dgEducationReasearch').edatagrid('cancelRow');
               }
           }
       ],
        idField: 'Id',
        loadMsg: 'شکیبا باشید...',
        rownumbers: true,
        height: '280',
        singleSelect: 'true',

        url: '@Url.Action("GetEducationReasearch", "Request", new {requestId =Model.Request== null ?0 :Model.Request.Id})',
        saveUrl: '@Url.Action("CreateEducationReasearch", "Request", new { requestId = Model.Request == null ? 0 : Model.Request.Id })',
        updateUrl: '@Url.Action("EditEducationReasearch", "Request")',
        destroyUrl: '@Url.Action("DeleteEducationReasearch", "Request")',
        onError: function (index, row) {
            alert('error');
        },
        columns: [
            [
                {
                    field: 'EducationalResearchStatus',
                    title: 'نوع',
                    value: 1,
                    width: 80,
                    editor: { type: 'textbox', width: 200, autoRowHeight: true, options: { required: true } }
                },
                {
                    field: 'Term',
                    title: 'نیمسال',
                    width: 80,
                    editor: {
                        type: 'combobox',
                        options: {
                            valueField: 'TermId',
                            textField: 'Term',
                            method: 'get',
                            url: '@Url.Action("GetTerm", "Request")'
                        }
                    }
                },
                {
                    field: 'Subject',
                    title: 'عنوان درس',
                    width: 300,
                    editor: { type: 'validatebox', width: 200, autoRowHeight: true, options: { required: true } }
                },
                {
                    field: 'CourseNo',
                    title: 'شماره درس',
                    width: 100,
                    editor: {
                        type: 'validatebox',
                        width: 100,
                        options: {
                            required: true
                        }
                    }
                },
                {
                    field: 'UnitCount',
                    autoRowHeight: true,
                    title: 'تعداد واحد',
                    width: 100,
                    editor: { type: 'validatebox', width: 100, options: { required: true } }
                },
                {
                    field: 'GradeEducation',
                    autoRowHeight: true,
                    title: 'مقطع',
                    width: 200,
                    hidden: "hidden",
                    editor: {
                        type: 'combobox',
                        options: {
                            valueField: 'GradeEducationId',
                            textField: 'GradeEducation',
                            method: 'get',
                            url: '@Url.Action("GetGradeEducation", "Request")'
                        }
                    }
                },
                {
                    field: 'StudentCount',
                    autoRowHeight: true,
                    title: 'تعداد دانشجو',
                    width: 200,
                    editor: { type: 'validatebox', width: 100, options: { required: true } }
                }
            ]
        ],
        onBeginEdit: function (index, row) {
            var ed = $('#dgEducationReasearch').edatagrid('getEditor',
                                    { index: index, field: 'EducationalResearchStatus' });
            $(ed.target).textbox('setValue', 1);
        }
    });


    $('a').btooltip({ placement: 'top' });

    function saverequest() {
        if ($('#addRequestForm').valid()) {
            $('#addRequestForm').ajaxSubmit({
                cashe: false,
                url: '@Url.Action("Create", "Request", new {area = "UserArea"}, "http")',
                type: 'Post',
                dataType: 'json',
                data: $('#addRequestForm').serialize(),
                success: function (result) {
                    $('#Name').val('');
                    $('#uniTree').tree('reload');
                    $.messager.show({
                        title: 'پیام سیستم',
                        msg: result.Msg,
                        showType: 'show'
                    });
                },
                error: function (xhr, status) {
                    xhr.responseText();
                }
            });
        } else {
            return $(this).form('validate');
        }
    }


}))

