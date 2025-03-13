$(document).ready(function () {
    const url = window.location.href;  // Get the current URL
    function extractAndDecode(url) {
        const urlParts = url.split("?");
        if (urlParts.length === 2) {
            const decodedParams = decodeURIComponent(urlParts[1]);
            return urlParts[0] + "?" + decodedParams;
        }
        return url;
    }

    function extractParameters(url) {
        const params = {};
        const urlParts = url.split("?");
        if (urlParts.length === 2) {
            const queryParams = urlParts[1].split("&");
            for (const param of queryParams) {
                const [key, value] = param.split("=");
                params[key] = decodeURIComponent(value);
            }
        }
        return params;
    }
    $("#ShowOrhideoldPaswrd").show();
    //function parseCustomDateTimeString(datetime) {
    //    const parts = datetime.split(' ');
    //    const dateParts = parts[0].split('-');
    //    const timeParts = parts[1].split(':');

    //    const year = parseInt(dateParts[2], 10);
    //    const month = parseInt(dateParts[1], 10) - 1; // Months are 0-based in JavaScript
    //    const day = parseInt(dateParts[0], 10);
    //    const hours = parseInt(timeParts[0], 10);
    //    const minutes = parseInt(timeParts[1], 10);
    //    const seconds = parseInt(timeParts[2], 10);

    //    return new Date(year, month, day, hours, minutes, seconds);
    //}

    //function validateDateTime(datetime) {
    //    const targetDateTime = new parseCustomDateTimeString(datetime);
    //    const yesterday = new Date();
    //    yesterday.setDate(yesterday.getDate() - 1);
    //    const twentyFourHoursAgo = new Date();
    //    twentyFourHoursAgo.setHours(twentyFourHoursAgo.getHours() - 24);

    //    const dateMinusOneDay = new Date(targetDateTime);
    //    dateMinusOneDay.setDate(targetDateTime.getDate() - 1);

    //    //return targetDateTime > yesterday && targetDateTime <= twentyFourHoursAgo;
    //    return targetDateTime > yesterday && dateMinusOneDay <= twentyFourHoursAgo;
    //}

    //function GetTimeFromDB(EmailSetUpDtls_Key, datetime)
    //{

    //    var dataforsend = {
    //        EmailSetUpDtls_Key: EmailSetUpDtls_Key,
    //        timeslotMinute: datetime
    //    }
    //    $.ajax({
    //        url: "/ChangePasswordUI/GetTimeFromDB",
    //        type: "POST",
    //        contentType: "application/json; charset=utf-8",

    //        data: JSON.stringify(dataforsend),
    //        dataType: "json",
    //        success: function (data) {
    //            console.log(data);
    //            //var t = $.parseJSON(data);
    //            //console.log(t[0].timeslotMinute);
    //            //return t[0].timeslotMinute;

    //            return data;

    //        },
    //        error: function (error) {
    //            console.log(error);
    //        }

    //    });

    //}

    //const decodedUrl = extractAndDecode(url);
    ////const params = extractParameters(url);
    //const params = extractParameters(decodedUrl);
    //const id = params.UserID;
    //const datetime = params.DateTime;
    //const PageLable = params.PageType;

    //if (PageLable === "1") {
    //    $('[id*="h_PageHeader"]').html("Change Password");
    //}
    //else if (PageLable === "2") {
    //    $('[id*="h_PageHeader"]').html("Forgot Password");
    //}

    //const EmailSetUpDtls_Key = params.EmailSetUpDtls_Key;
    ////const getTimeFromDb = GetTimeFromDB(EmailSetUpDtls_Key, datetime);
    ////const JavaScriptObjectTime = new Date(getTimeFromDb);
    ////const currentTime = new Date();


    // var  JavaScriptObjectTime, currentTime=new Date();
    //// Call GetTimeFromDB and handle the returned promise
    //GetTimeFromDB(EmailSetUpDtls_Key, datetime)
    //    .then(function (data) {
    //        // Store the data in getTimeFromDb variable
    //        const getTimeFromDb = data;
    //        console.log('Data from GetTimeFromDB:', getTimeFromDb);

    //        // Proceed to the next operation here
    //         JavaScriptObjectTime = new Date(getTimeFromDb);
    //        console.log('JavaScriptObjectTime:', JavaScriptObjectTime);
    //    })
    //    .catch(function (error) {
    //        console.error('Error:', error);
    //    });



    //console.log("ID:", id);
    //console.log("DateTime:", datetime);

    //const isDateTimeValid = validateDateTime(datetime);
    //console.log("Is DateTime Valid:", isDateTimeValid);


    //$('[id*="Btn_PassChange"]').click(function (e) {

    //   // var regularExpression = /^(?=.*[0-9])(?=.*[!@#$%^&*])[a-zA-Z0-9!@#$%^&*]{8,15}$/;
    //   // var regularExpression = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,15}$/;

    //    //if (isDateTimeValid === false) {
    //    //    swal.fire("Alert !!", "Link expire !!", "error");
    //    //    return false;
    //    //}
    //    //if (JavaScriptObjectTime < currentTime) {
    //    //    swal.fire("Alert !!", "Link expire !!", "error");
    //    //    return false;
    //    //}
    //    //else if ($('[id*="New_Password"]').val().length < 8 || $('[id*="New_Password"]').val().length > 15) {
    //    //    swal.fire("Alert !!", "New password length should be 8 to 15 character's !!", "error");
    //    //    return false;
    //    //}
    //    //else if ($('[id*="Confrim_Password"]').val().length < 8 || $('[id*="Confrim_Password"]').val().length > 15) {
    //    //    swal.fire("Alert !!", "Confrim password length should be 8 to 15 character's !!", "error");
    //    //    return false;
    //    //}
    //    //else if (!regularExpression.test($('[id*="New_Password"]').val())) {
    //    //    swal.fire("Alert !!", "New password contain should has at least one uppercase letter, one lowercase letter, one number and one special character !!", "error");
    //    //    return false;
    //    //}
    //    //else if (!regularExpression.test($('[id*="Confrim_Password"]').val())) {
    //    //    swal.fire("Alert !!", "Confrim password contain should has at least one uppercase letter, one lowercase letter, one number and one special character !!", "error");
    //    //    return false;
    //    //}
    //    //else if ($('[id*="Confrim_Password"]').val() != $('[id*="New_Password"]').val()) {
    //    //    swal.fire("Alert !!", "Confrim Password is not matched with New password !!", "error");
    //    //    return false;
    //    //}
    //    // Create an object to send as JSON

    //    var dataToSend = {
    //        Employee_Master_Key: $('[id*="Employee_Master_Key"]').val() ,
    //        Confirm_Password: $('[id*="Confirm_Password"]').val(),
    //        New_Password: $('[id*="New_Password"]').val(),
    //        Old_Password: $('[id*="Old_Password"]').val(),
    //    };
    //    $.ajax({
    //        url: "/ChangePasswordUI/ChangePassword",
    //        type: "POST",
    //        contentType: "application/json; charset=utf-8",

    //        data: JSON.stringify(dataToSend),
    //        dataType:'json',
    //        success: function (response) {
    //            // Handle the success response here
    //            console.log(response);
    //            if (response === "1") {
    //                $("#ShowOrhidePaswrd").show();
    //            }
    //            //swal.fire("Done", "Password change successfully !!", "success").then(function () {
    //            //    window.location.href = "http://localhost:5197/";
    //            //});

    //        },
    //        error: function (error) {
    //            // Handle the error response here
    //            console.error(error);
    //        }
    //    });

    //e.preventDefault();
    //});
});

//function GetTimeFromDB(EmailSetUpDtls_Key, datetime) {
//    return new Promise(function (resolve, reject) {
//        var dataforsend = {
//            EmailSetUpDtls_Key: EmailSetUpDtls_Key,
//            timeslotMinute: datetime
//        };

//        $.ajax({
//            url: "/ChangePasswordUI/GetTimeFromDB",
//            type: "POST",
//            contentType: "application/json; charset=utf-8",
//            data: JSON.stringify(dataforsend),
//            dataType: "json",
//            success: function (data) {
//                console.log(data);
//                resolve(data);  // Resolve the promise with data
//            },
//            error: function (error) {
//                console.log(error);
//                reject(error);  // Reject the promise with an error
//            }
//        });
//    });
//}
$("#Btn_PassChange").click(function () {

    var Old_Password = $('#Old_Password').val();
    var New_Password = $('#New_Password').val();
    var Confirm_Password = $('#Confirm_Password').val();
    if (Old_Password === "") {
        swal.fire("Alert !!", "Please Enter Old Password  !!", "error");
        return false;
    }

    else if (New_Password !== Confirm_Password) {
        swal.fire("Alert !!", "Password  Not Matched  !!", "error");
        return false;
    }
    var dataToSend = {
        Old_Password: Old_Password,
        Confirm_Password: Confirm_Password
    };



    // Make the AJAX request
    $.ajax({
        url: "/ChangePasswordUI/ChangePassword",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(dataToSend),
        dataType: 'json',
        success: function (response) {
            console.log(response);
            if (response === "1") {
                $("#ShowOrhideoldPaswrd").hide();
                $("#ShowOrhidePaswrd").show();
            }
            else if (response === "0") {
                swal.fire("Alert !!", "Old Password didnt matched with records  !!", "error");
            }
            else if (response === "2") {
                swal.fire("Done", "Password change successfully !!", "success").then(function () {
                    window.location.href = "/Login/Login";
                });
            }
        },
        error: function (error) {
            console.error(error);  // Log the error
            alert("An error occurred.");
        }
    });



});

