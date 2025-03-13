///Refreshing-Page////
var RefreshButton = document.getElementById("RefreshModel");
RefreshButton.addEventListener("click", function () {
    location.reload();
});

$(document).ready(function () {
    $('#dynamicTable_GroupHead').DataTable({
        "pageLength": 50
    });

    $('#MobileNo').on('keypress', function (e) {
        // Ensure that it is a number and stop the keypress
        if (e.shiftKey || (e.keyCode < 48 || e.keyCode > 57) || ($('#MobileNo').val().length > 9)) {
            e.preventDefault();
        }
    });

    $('#Aadhar').on('keypress', function (e) {
        // Ensure that it is a number and stop the keypress
        if (e.shiftKey || (e.keyCode < 48 || e.keyCode > 57) || ($('#Aadhar').val().length > 11)) {
            e.preventDefault();
        }
    });

    $('#CloseGroupHead').click(function () {
        window.location.reload();
    });

    $('#VerifyName').click(function () {
        var User_Name = document.getElementById('UserName').value;

        $.getJSON("/Authorization/VerifyUserName",
            {
                id: User_Name
            },
            function (data) {
                $('#User_Name').val(data.userName);
            });
    });

    $('.checkbox-item').change(function () {
        if ($(this).is(':checked')) {
            var checkboxValue = $(this).val();
            // Optional: Add logic for the checked value
        }
    });
});

// Add function to handle card name and username generation
function Add() {
    var First_Name = document.getElementById('FirstName').value.trim();
    var Middle_Name = document.getElementById('MiddleName').value.trim();
    var Last_Name = document.getElementById('LastName').value.trim();
    var CardName = "";
    var Username = "";

    if (Middle_Name === "" && First_Name === "") {
        CardName = Last_Name;
        Username = Last_Name;
    } else if (Middle_Name === "" && Last_Name !== "" && First_Name !== "") {
        CardName = First_Name + ' ' + Last_Name;
        Username = (First_Name + Last_Name).replace(/\s+/g, '');
    } else if (Middle_Name !== "" && First_Name !== "" && Last_Name !== "") {
        CardName = First_Name + ' ' + Middle_Name + ' ' + Last_Name;
        Username = (First_Name + Middle_Name + Last_Name).replace(/\s+/g, '');
    }

    document.getElementById('Name').value = CardName;
    document.getElementById('UserName').value = Username + '-GH';
}

// Generate password based on username and mobile number
function generatePassword() {
    const nameInput = document.getElementById('User_Name');
    const mobileInput = document.getElementById('MobileNo');

    let password = "";

    const name = nameInput.value;
    const mobile = mobileInput.value;

    // Get the last four digits of the mobile number
    const mobileDigits = mobile.match(/\d/g).join('').substr(-4);

    // Combine the first four characters of the name with the last four digits of the mobile number
    password = name.substring(0, Math.min(name.length, 4)) + mobileDigits;

    document.getElementById('PWD').value = password;
}






function EditItem(id) {
    $.getJSON("/Authorization/GetGroupHead", { id: id }, function (datasetA) {
        console.log("Full Dataset:", datasetA);
        const parsedData = JSON.parse(datasetA);
        // Show the modal and set its title
        $('#GroupHeadPersonnelModal').modal('show');
        document.getElementById("staticBackdropLabel").innerHTML = "Guest User Registration Form:: [Edit]";

        // Extract Table and Table1 data
        const tableData = parsedData.Table;
        const table1Data = parsedData.Table2;

        // Validate and log Table data
        if (tableData && tableData.length > 0) {
            const record = tableData[0]; // Extract the first object from Table
            console.log("Table Data:", record);

            // Populate form fields with values from Table
            $("#GroupHeadId").val(record.GroupHeadId || "");
            $("#GroupHeadPersonnelId").val(record.GroupHeadPersonnelId || "");
            $("#TenantId").val(record.TenantId || "");
            $("#Name").val(record.Name || "");
            $("#FirstName").val(record.FirstName || "");
            $("#LastName").val(record.LastName || "");
            $("#FathersName").val(record.FathersName || "");
            $("#Address").val(record.Address || "");
            $("#PAN").val(record.PAN || "");
            $("#Aadhar").val(record.Aadhar || "");
            $("#EmailId").val(record.EmailId || "");
            $("#MobileNo").val(record.MobileNo || "");
            $("#ResidentialStatusId").val(record.ResidentialStatusId || "");
            $("#EffectiveDate").val(record.EffectiveDate ? record.EffectiveDate.split("T")[0] : ""); // Format date
            $("#User_Name").val(record.User_Name || "");
            $("#PWD").val(record.PWD || "");

            var selectedIds = record.DepartmentIds.split(',');

            // Iterate through the checkboxes and check them based on the selected IDs
            var checkboxes = document.querySelectorAll('.checkbox-item');

            checkboxes.forEach(function (checkbox) {
                if (selectedIds.includes(checkbox.value)) {
                    checkbox.checked = true;
                }
            });
        } else {
            console.error("Table data is missing or empty.");
        }

        // Validate and log Table1 data
        if (table1Data && table1Data.length > 0) {
            console.log("Table1 Data:", table1Data);

            // Iterate through Table1 and update checkboxes dynamically
            table1Data.forEach(function (item) {
                const companyIdCheckbox = $(`#CompanyId-${item.CompanyId}`);
                const userTypeIdCheckbox = $(`#UserTypeId-${item.UserTypeId}`);

                // Check corresponding checkboxes if they exist
                if (companyIdCheckbox.length > 0) {
                    companyIdCheckbox.prop("checked", true);
                }
                if (userTypeIdCheckbox.length > 0) {
                    userTypeIdCheckbox.prop("checked", true);
                }
            });
            const allCheckedCom = $('.CompanyId-checkbox').length === $('.CompanyId-checkbox:checked').length;
            $('#selectAllCompany').prop('checked', allCheckedCom);

            const allCheckedUser = $('.UserTypeId-checkbox').length === $('.UserTypeId-checkbox:checked').length;
            $('#selectAllUserTypeId').prop('checked', allCheckedUser);

        } else {
            console.warn("Table1 data is missing or empty.");
        }
    });





}

function EmailSend(id) {

    //alert(id);
    $("#email-loader").fadeIn();
    $.getJSON("/Authorization/GetGroupHead",
        {
            id: id
        },

        function (response) {
            const parsedData = JSON.parse(response);

            const tableData = parsedData.Table;
            const table1Data = parsedData.Table1;

            // Validate and log Table data
            if (tableData && tableData.length > 0) {
                const record = tableData[0];
                console.log("Table Data:", record);
                var datatoSend = {
                    EmailID: record.EmailId,
                    GroupHeadName: record.Name,
                    UserName: record.User_Name,
                    Password: record.PWD,
                    Pin: record.pin,
                    EmailSetUpDtls_Key: 3
                }
            }
            else {
                var datatoSend = {
                    EmailID: record.emailId,
                    GroupHeadName: record.name,
                    UserName: record.user_Name,
                    Password: record.pwd,
                    Pin: record.pin,
                    EmailSetUpDtls_Key: 3
                }
            }

            $.ajax({
                url: "/Authorization/CredentialsSend",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(datatoSend),
                dataType: 'json',
                success: function (response) {
                    $("#email-loader").fadeOut();
                    //console.log(response);
                    var d = $.parseJSON(response);
                    if (d[0].mailBody != null) {
                        swal.fire("Thank You!!!", "Email send to Group Head!!", "success");
                    }
                    else {
                        $("#email-loader").fadeOut();
                        swal.fire("Alert!!!", "Please enter proper Email ID!!", "error");
                    }
                },
                error: function (error) {
                    $("#email-loader").fadeOut();
                    console.log(error);
                    swal.fire("Alert!!!", "Please contact to admin!!", "error");
                    console.error('Error:', error.responseText);
                }
            });

        });

}

function TagDelete(GroupHead_ID) {
    Swal.fire({
        title: 'Do you want to Delete?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        /* Read more about isConfirmed, isDenied below */
        if (result.isConfirmed) {
            $.getJSON("/Authorization/DeleteGroupHeadPersonnel",
                {
                    id: GroupHead_ID
                },
                function (data) {
                    ////console.log(data)
                    if (data.msg = "success") {
                        swal.fire("Done", "Record Delete SuccessFully !!", "success");
                    }
                    else {
                        swal.fire("Oppss!!!", "Please Contact Admin", "error");
                    }
                    window.location.reload();
                });
        } else if (result.isDenied) {
            Swal.fire('Welcome ', '', 'info')
        }
    })
}



// Handle Select All for Company checkboxes
$(document).on('change', '#selectAllCompany', function () {
    $('.CompanyId-checkbox').prop('checked', $(this).is(':checked'));
    updateSelectedText();
    fetchrole(getSelectedCompanies());
});

// Handle individual Company checkbox changes
$(document).on('change', '.CompanyId-checkbox', function () {
    const allChecked = $('.CompanyId-checkbox').length === $('.CompanyId-checkbox:checked').length;
    $('#selectAllCompany').prop('checked', allChecked);
    
    /* fetchrole(getSelectedCompanies());*/
});

$(document).on('change', '.UserTypeId-checkbox', function () {
    const allChecked = $('.UserTypeId-checkbox').length === $('.UserTypeId-checkbox:checked').length;
    $('#selectAllUserTypeId').prop('checked', allChecked);

    /* fetchrole(getSelectedCompanies());*/
});


// Handle Select All for UserType checkboxes
$(document).on('change', '#selectAllUserTypeId', function () {
    $('.UserTypeId-checkbox').prop('checked', $(this).is(':checked'));
});

// Update selected text for Companies
//function updateSelectedText() {
//    const selected = $('.CompanyId-checkbox:checked').map(function () {
//        return $(this).siblings('label').text().trim();
//    }).get();
//    const displayText = selected.length > 0 ? selected.join(', ') : '-- Select Company --';
//    $('#CompanyIdDropdownButton').text(displayText).attr('title', displayText);
//}

// Get selected Company IDs
function getSelectedCompanies() {
    return $('.CompanyId-checkbox:checked').map(function () {
        return $(this).val();
    }).get().join(',');
}



function resetCheckboxes() {
    $('#CompanyId input[type="checkbox"]').prop('checked', false);
    $('#UserTypeId input[type="checkbox"]').prop('checked', false);
}

$(function () {
    $('#btnSaveGroupHeadPersonnel').on('click', function (e) {
        e.preventDefault();

        // Initialize a flag to track form validation status
        let isValid = true;

        // Validate Role
        if ($('#GroupHeadId').val() === "") {
            Swal.fire({
                icon: 'error',
                title: 'Validation Error',
                text: 'Role is required.'
            });
            isValid = false;
        }
        else if ($('#FirstName').val().trim() === "") {
            // Validate First Name
            Swal.fire({
                icon: 'error',
                title: 'Validation Error',
                text: 'First Name is required.'
            });
            isValid = false;
        }
        else if ($('#FirstName').val().length > 50) {
            Swal.fire({
                icon: 'error',
                title: 'Validation Error',
                text: 'First Name cannot exceed 50 characters.'
            });
            isValid = false;
        }
        else if ($('#LastName').val().trim() === "") {
            // Validate Last Name
            Swal.fire({
                icon: 'error',
                title: 'Validation Error',
                text: 'Last Name is required.'
            });
            isValid = false;
        }
        else if (!/^\d{10}$/.test($('#MobileNo').val())) {
            // Validate Mobile Number
            Swal.fire({
                icon: 'error',
                title: 'Validation Error',
                text: 'Mobile Number must be 10 digits.'
            });
            isValid = false;
        }
        else if (!/^\S+@\S+\.\S+$/.test($('#EmailId').val())) {
            // Validate Email
            Swal.fire({
                icon: 'error',
                title: 'Validation Error',
                text: 'Invalid Email format.'
            });
            isValid = false;
        }
        else if (!/^[A-Z]{5}[0-9]{4}[A-Z]$/.test($('#PAN').val())) {
            // Validate PAN
            Swal.fire({
                icon: 'error',
                title: 'Validation Error',
                text: 'Invalid PAN format.'
            });
            isValid = false;
        }
        else if (!/^\d{12}$/.test($('#Aadhar').val())) {
            // Validate Aadhar
            Swal.fire({
                icon: 'error',
                title: 'Validation Error',
                text: 'Aadhar must be 12 digits.'
            });
            isValid = false;
        }

        // If validation fails, stop the form submission
        if (!isValid) {
            return;
        }

        // Prepare combinations of selected CompanyId and UserTypeId
        var combinations = [];

        // Loop through each CompanyId checkbox
        $('.CompanyId-checkbox:checked').each(function () {
            var companyId = $(this).val();  // Get CompanyId value
            var companyName = $(this).attr('name');  // Get CompanyId checkbox name

            // Loop through each checked UserTypeId checkbox
            $('.UserTypeId-checkbox:checked').each(function () {
                var userTypeId = $(this).val();  // Get UserTypeId value
                var userTypeName = $(this).attr('name');  // Get UserTypeId checkbox name

                // Add the combination to the array
                combinations.push({
                    companyId: companyId,
                    companyName: companyName,
                    userTypeId: userTypeId,
                    userTypeName: userTypeName
                });
            });
        });
        if (combinations.length === 0) {
            Swal.fire({
                icon: 'error',
                title: 'Validation Error',
                text: 'Please select at least one combination of Company and Role.'
            });
            return; // Stop the form submission
        }

        var selectedGroupHeadTypes = $("input[name='selectedGroupHeadTypes']:checked").map(function () { return this.value; }).get();
        if (selectedGroupHeadTypes.length === 0) {
            Swal.fire({
                icon: 'error',
                title: 'Validation Error',
                text: 'Please select at least one department.'
            });
            return; // Stop further execution
        }

        // Convert the array to a comma-separated string
        var SelectedDepartments = selectedGroupHeadTypes.join(',');


        // Create an object that contains the form data and combinations
        var formData = {
            GroupHeadId: $('#GroupHeadId').val(),
            GroupHeadPersonnelId: $('#GroupHeadPersonnelId').val(),
            FirstName: $('#FirstName').val(),
            MiddleName: $('#MiddleName').val(),
            LastName: $('#LastName').val(),
            MobileNo: $('#MobileNo').val(),
            EmailId: $('#EmailId').val(),
            PAN: $('#PAN').val(),
            User_Name: $('#User_Name').val(),
            PWD: $('#PWD').val(),
            Aadhar: $('#Aadhar').val(),
            Address: $('#Address').val(),
            Name: $('#Name').val(),
            FathersName: $('#FathersName').val(),
            EffectiveDate: $('#EffectiveDate').val(),
            DIN: $('#DIN').val(),
            SelectedDepartments: SelectedDepartments,
            Combinations: combinations
        };

        // Send the form data and combinations to the server using POST
        $.post('/Authorization/SaveGroupHeadPersonnel', formData, function (response) {
            Swal.fire({
                icon: 'success',
                title: 'Data Sent Successfully',
                text: 'The data has been successfully sent to the server.'
            });
        }).fail(function (error) {
            Swal.fire({
                icon: 'error',
                title: 'Error Sending Data',
                text: 'There was an error while sending the data.'
            });
        });
    });
});




