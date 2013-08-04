
$(document).ready(function () {
	$("#logout_menu_button").click(showLogoutModal);
});

function showLogoutModal(){
	$('#logout_modal').modal('show');
}