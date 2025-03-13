//$(document).ready(function () {
//    //
//    $('[id*="Btn_EmailSend"]').click(function (e) {
        
//        var regex = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,5}$/;
//        if (!regex.test($('[id*="Email"]').val()))
//        {
//            swal.fire("ALert", "Please enter proper Email ID !!", "error");
//            $('[id*="Email"]').focus();
//            return false;
//        }

//        $.ajax({
//            url: '/ChangePasswordUI/ValidateEmailForPassword',
//            type: 'POST',
//            data: { email: $('[id*="Email"]').val() },
//            dataType: 'json',
//            success: function (data) {

//                var d = $.parseJSON(data);
//                if (d[0].employee_Name != null) {
//                    swal.fire("Thank You!!!", "Please check your email!!", "success");
//                }
//                else {
//                    swal.fire("Alert!!!", "Please enter proper Email ID!!", "error");
//                }
                
//            },
//            error: function (error) {
//                //console.error('Error:', error);
//                swal.fire("Alert!!!", "Please contact to admin!!", "error");
//                console.error('Error:', error.responseText);
//            }
//        });
//        e.preventDefault();
//    });
//});



$('#Btn_Save').click(function () {

    var data = {
       
        UserID: $('#UserID').val(),
        Phone_No: $('#Phone_No').val(),
        Email: $('#Email').val()
    };

    $.ajax({
        type: "POST",
        url: "/ChangePasswordUI/AppliedToForgetPWD",
        data: data,  
        success: function (data) {
            if (data.msg === "Success") {
                Swal.fire({
                    title: "Data Saved Successfully",
                    icon: "success",
                    showConfirmButton: false,
                    timer: 4000

                });
           
            } else {
                swal.fire("Oops!", "Please Contact Admin", "error");
            }
        },
        error: function () {
            swal.fire("Error", "An error occurred", "error");
        }
    });
})
