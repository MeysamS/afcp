var chub = $.connection.cartableHub;
$.connection.hub.logging = true;

chub.client.sendNewCartable = function () {
    $('#dgCartable').datalist('reload');
};
chub.client.sendNotification = function (message) {
    alert(message);
    $('#lstMsgNotifications').append(message);
};
$.connection.hub.start().done(function () {
    $('#btnSendRequest').click(function () {
        var requestId = $("#Request_Id").val();
        var desc = $("#CartableDescription").val();
        $.ajax({
            cashe: false,
            url: '@Url.Action("SendRequest", "Cartable", "http")',
            type: 'Post',
            dataType: 'json',
            data: { requestId: requestId, desc: desc }, //$('#addRequestForm').serialize(),
            success: function (result) {
                if (result.isError) {
                    myalert(result.Msg, 'danger');
                    return string.empty;
                } else {
                    chub.server.addNewCartableNotification(result.uidRecive).done(function () {
                        myalert(result.Msg, 'success');
                        $('a.l-btn').linkbutton({ disabled: false });
                        $('#dlgConfirmed').dialog().close();
                    });


                }
            },
            error: function (xhr, status) {
                xhr.responseText();
            }
        });

    });

    chub.server.join();
    $('#btnSendCartable').click(function () {
        var rowSelected = $('#dgCartable').datalist('getSelected');
        var desc = $("#CartableDescription").val();
        if (rowSelected == null) {
            $.messager.alert('Warning', 'رکوردی انتخاب نشده است!');
            return;
        }
        $.messager.confirm('confirm', 'ارسال شود؟', function (r) {
            if (r) {
                $.post('@Url.Action("Send", "Cartable", new {area = "UserArea"}, "http")',
                { cartableId: rowSelected.Id, desc: desc }, function (result) {
                    if (!result.isError) {
                        chub.server.addNewCartableNotification(result.uidRecive).done(function () {
                            $('#dlgConfirmed').dialog().close();
                            myalert(result.Msg, "success");
                        });

                    } else {
                        myalert(result.Msg, "danger");
                    }
                }, 'json');
            }
        });

    });

    $('a.btnNotificationSend').click(function () {
        var text = $("#Messaging_Text").val();
        var reciever = $(this).attr('data-rl');
        $.post('@Url.Action("SendMEssage", "Profile", new {area = "UserArea"}, "http")',
               { text: text, recieverRoleName: reciever }, function (result) {
                   if (!result.isError) {
                       chub.server.addNewMessageNotification(result.messageid).done(function () {
                           $('#notificationCount').text(parseInt($('#notificationCount').text()) + 1);
                           $('#Messaging_Text').val("");
                           myalert(result.Msg, 'success');
                           $('a.l-btn').linkbutton({ disabled: false });
                       });

                   } else {
                       myalert(result.Msg, "danger");
                   }
               }, 'json');
    });
}).fail(function () { });