function myalert(message, datatype) {
    $('html,body').animate({ scrollTop: 0 }, 500);
    var options = {
        type: datatype,
        namespace: 'pa_page_alerts_default',
        auto_close: 4
    };
    LanderApp.plugins.alerts.add(message, options);
}

$.validator.setDefaults({
    showErrors: function (errorMap, errorList) {
        this.defaultShowErrors();
        //اگر المانی معتبر است نیاز به نمایش تول‌تیپ ندارد
        $("." + this.settings.validClass).tooltip("destroy");
        //افزودن تول‌تیپ‌ها
        for (var i = 0; i < errorList.length; i++) {
            var error = errorList[i];
            //$(error.element).tooltip({
            //    position: 'left',
            //    content: error.message,
            //    onShow: function () {
            //        $(this).tooltip('tip').css({
            //            color: "#000",
            //            borderColor: "#CC9933",
            //            backgroundColor: "#FFFFCC"
            //        });
            //    }
            //});
            $(error.element).tooltip({ trigger: "focus", placement: "left", cssClass: 'error' }) // فقط در حالت فوکوس نمایش داده شود
                                    .attr("data-original-title", error.message);
            $(error.element).tooltip('show');
        }
    },
    // همانند قبل برای رنگی کردن کل ردیف در صورت عدم اعتبار سنجی و برعکس
    highlight: function (element, errorClass, validClass) {
        if (element.type === 'radio') {
            this.findByName(element.name).addClass(errorClass).removeClass(validClass);
        } else {
            $(element).addClass(errorClass).removeClass(validClass);
            $(element).closest('.form-group').removeClass('success').addClass('has-error');
        }
        $(element).trigger('highlited');
    },
    unhighlight: function (element, errorClass, validClass) {
        if (element.type === 'radio') {
            this.findByName(element.name).removeClass(errorClass).addClass(validClass);
        } else {
            $(element).removeClass(errorClass).addClass(validClass);
            $(element).closest('.form-group').removeClass('has-error').addClass('success');
        }
        $(element).trigger('unhighlited');
    }

})