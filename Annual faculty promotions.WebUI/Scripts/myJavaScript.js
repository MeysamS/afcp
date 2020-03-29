function ValidateFileUpload(e, size, fileTypeArray) {
    if (e.files[0].size > size) {
        $.messager.alert('پیام سیستم', 'حجم فایل برای آپلود زیاد می باشد!');
        $(e).val('');
        e.preventDefault();
        return false;
    }
    // Array with information about the uploaded files
    var files = e.files;
    var ext = $(e).val().split('.').pop().toLowerCase();
    if ($.inArray(ext, fileTypeArray) == -1) {
        $.messager.alert('پیام سیستم', 'نوع فایل انتخابی معتبر نمی باشد!\nانواع معتبر [gif,jpeg,jpg,png,tif,pdf]');
        $(e).val('');
        e.preventDefault();
        return false;
    }
    return true;
}