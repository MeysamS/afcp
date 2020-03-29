function validateThisForm(formId) {
    var val = $(formId).validate();
    val.form();
    return val.valid();
}

function customSubmit(el, formId) {
    if (!validateThisForm(formId)) return;
    $(el).prop("onclick", null).attr("onclick", null);
    //$(formId).submit();
}
function lockButton(el) {
    $(el).prop("onclick", null).attr("onclick", null);
}