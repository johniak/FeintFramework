$(document).ready(function () {
    $("#show_new_message_modal_button").click(showNewMessageModal);
    $("#send_message_button").click(sendMessage);
    $("#reload_messages_button").click(getReciveBox);
});
function showNewMessageModal() {
    $("#new_message_modal").modal('show');
}
function sendMessage() {
    var formData = $('#new_message_form').serialize();
    $.ajax({
        url: '/message/send/',
        type: 'POST',
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response == "true") {
                $("#new_message_modal").modal('hide');
            } else {
                $("#new_message_modal_error").html("<div class=\"alert alert-error\">Problem with messge send.</div>");
            }

        }
    });
}
function getReciveBox() {
    $.ajax({
        url: "/api/messages/",
        tpye: "GET",
        success: function (response) {
            var messages = JSON.parse(response);
            var table = $("#recived_messages_table tbody");
            table.html("");
            $.each(messages, function (id, value) {

                table.append("<tr id=\""+value.Id+"\"><td>"+value.From+"</td><td>"+value.Title+"</th></td>");
                //console.log(value.Title);

            });
        }
    });
}