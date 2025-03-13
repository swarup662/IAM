/// <reference path="../js/site.js" />
/// <reference path="../assets/plugins/smart-wizard/js/jquery.smartwizard.min.js" />


$(document).ready(function () {
    //Data For First Wizard
  
    // Toolbar extra buttons
    var btnFinish = $('<button></button>').text('Finish').addClass('btn btn-info').on('click', function () {
        window.location.href = "/Authorization/UserCreation";
        //window.location.reload();
    });
    var btnCancel = $('<button></button>').text('Cancel').addClass('btn btn-danger').on('click', function () {
        $('#smartwizard').smartWizard("reset");
    });
    // Step show event
    $("#smartwizard").on("showStep", function (e, anchorObject, stepNumber, stepDirection, stepPosition) {
        e.preventDefault();
        $("#prev-btn").removeClass('disabled');
        $("#next-btn").removeClass('disabled');
        if (stepPosition === 'first') {
            $("#prev-btn").addClass('disabled');
        } else if (stepPosition === 'last') {

            $("#next-btn").addClass('disabled');
          
            
        } else {
            $("#prev-btn").removeClass('disabled');
            $("#next-btn").removeClass('disabled');
        }
        if (stepNumber == "0") {
       
          
           LoadCompany($("#USER_MASTER_KEY").val());
               

           
        }
        else if (stepNumber == "1") {
            
         
            LoadUserwiseCompany($("#USER_MASTER_KEY").val(), $("#empCompanyId").val());
           
            LoadUSERUsertypeCheckboxList($("#USER_MASTER_KEY").val(), $("#empCompanyId").val());
           
        }
        else if (stepNumber == "2") {
            
            $('#application_ID').val(0);
            LoadUserwiseCompanylbl3($("#USER_MASTER_KEY").val(), $("#empCompanyId_lbl3").val());
    
            LoadApplicationlist();
         
            if (!$("#ApprovalLevelthree").html()) {
                LoadUSER_WiseModuleAccess($("#USER_MASTER_KEY").val(), $("#empCompanyId_lbl3").val(), $("#application_ID").val(), $("#moduleIdCSV").val());

            }

            var USER_MASTER_KEY = $('#USER_MASTER_KEY').val();
            var companyId = $('#empCompanyId_lbl3').val();
            // Second AJAX call to UserTypeCheckbox
            if (companyId !== "0") {  // Check if CompanyId is not '0'
                $.post("/Authorization/UserTypeDropdown_W3",
                    {
                        USER_MASTER_KEY: USER_MASTER_KEY,
                        CompanyId: companyId
                    },
                    function (data) {
                        console.log(data);
                        $("#UserTypeId").html(data); // Update the UserTypeId dropdown with returned data
                    }
                );
            } else {
                $('#UserTypeId').empty(); // Clear dropdown if CompanyId is '0'
            }
        }
       
    });
    // Smart Wizard
    $('#smartwizard').smartWizard({
        selected: 0,
        theme: 'dots',
        transition: {
            animation: 'slide-horizontal', // Effect on navigation, none/fade/slide-horizontal/slide-vertical/slide-swing
        },
        toolbarSettings: {
            toolbarPosition: 'top', // both bottom
            toolbarExtraButtons: [btnFinish, btnCancel]
        }
    });
    // External Button Events
    $("#reset-btn").on("click", function () {
        // Reset wizard
        $('#smartwizard').smartWizard("reset");
        return true;
    });
    $("#prev-btn").on("click", function () {
        // Navigate previous
        $('#smartwizard').smartWizard("prev");
        return true;
    });
    $("#next-btn").on("click", function (e) {
        // Navigate next
        e.preventDefault();
        $('#smartwizard').smartWizard("next");
        return true;
    });
    // Demo Button Events
    $("#got_to_step").on("change", function () {
        // Go to step
        var step_index = $(this).val() - 1;
        $('#smartwizard').smartWizard("goToStep", step_index);
        return true;
    });
    $("#is_justified").on("click", function () {
        // Change Justify
        var options = {
            justified: $(this).prop("checked")
        };
        $('#smartwizard').smartWizard("setOptions", options);
        return true;
    });
    $("#animation").on("change", function () {
        // Change theme
        var options = {
            transition: {
                animation: $(this).val()
            },
        };
        $('#smartwizard').smartWizard("setOptions", options);
        return true;
    });
    $("#theme_selector").on("change", function () {
        // Change theme
        var options = {
            theme: $(this).val()
        };
        $('#smartwizard').smartWizard("setOptions", options);
        return true;
    });

    var idValue = null;
    if (window.location.href.indexOf('?') !== -1) {
        idValue = decodeIdFromUrl();
    }
    else {
       
        $("#UserProfileID").val("");
        $("#UserID").val("");
        $("#USER_MASTER_KEY").val("");

        $("#EmailID").val(""); 



    }
  
    function decodeIdFromUrl() {

        var url = decodeURIComponent(window.location.href); // Decode the entire URL
        var queryStringStart = url.indexOf('?'); // Find the start of the query string
        var hashIndex = url.indexOf('#'); // Find the index of the '#' character

        if (queryStringStart !== -1 && hashIndex !== -1) {
            var queryString = url.substring(queryStringStart + 1, hashIndex); // Extract the query string
            var params = new URLSearchParams(queryString); // Parse the query string
            var idValue = params.get("id"); // Get the 'id' parameter value

            return idValue;
        }

        return null; // Return null if the URL structure doesn't match
    }


    //console.log(idValue);

    if (idValue != null || typeof idValue != undefined) {
        //alert("not null");
        
        var actionIdCSV = [];
        var moduleIdCSV = [];
        var e = "";
       
        $.ajax({
            url: "/Authorization/GetUserDetailsById",
            type: "POST",
            dataType: "json",
            data: { USER_MASTER_KEY: idValue },
            success: function (d) {
                
              
                var data = JSON.parse(d.list)[0];
                console.log(data);
                populateUserFields(data) 
           
               
                LoadCompany(idValue);
                
                
                LoadUserwiseCompany(data.USER_MASTER_KEY, $("#empCompanyId").val());
                LoadUSERUsertypeCheckboxList(data.USER_MASTER_KEY, $("#empCompanyId").val());
                LoadUserwiseCompanylbl3(data.USER_MASTER_KEY, $("#empCompanyId").val());
             
                
               
              
                LoadUSER_WiseModuleAccess(data.USER_MASTER_KEY, data.empCompanyId_lbl3, data.application_ID, data.moduleId);
            
               
            },
            error: function (error) {

                console.log("GetUserDetailsById:" + error);
            }
        });
    }
    else {
        //alert("null");
    }

    function populateUserFields(data) {
        if (!data) return; // Prevent errors if data is null/undefined

        $("#UserID").val(data.UserID);
        $("#USER_MASTER_KEY").val(data.USER_MASTER_KEY);
        $("#UserProfileID").val(data.UserProfileID);
        $("#User_Unique_ID").val(data.User_Unique_ID);
        $("#UserCategoryId").val(data.UserCategoryId);
        $("#EmailTypeId").val(data.EmailTypeId);
        $("#UserCategoryName").val(data.UserCategoryName);

        $("#FirstName").val(data.FirstName);
        $("#MiddleName").val(data.MiddleName);
        $("#LastName").val(data.LastName);
        $("#FullName").val(data.FullName);
        // Check if both addresses are the same
        if (data.CurrentAddress === data.PermanentAddress) {
            if (data.CurrentAddress !== ""){
                $("#sameAsCurrent").prop("checked", true);
                $("#PermanentAddress").prop("readonly", true);
            }

            else {
                $("#sameAsCurrent").prop("checked", false);
                $("#PermanentAddress").prop("readonly", false);
            }

        } else {
            $("#sameAsCurrent").prop("checked", false);
            $("#PermanentAddress").prop("readonly", false);
        }


        $("#CurrentAddress").val(data.CurrentAddress);
        $("#PermanentAddress").val(data.PermanentAddress);

        $("#Gender").val(data.Gender);
        $("#GenderName").val(data.GenderName);

        $("#UserName").val(data.UserName);
        $("#Password").val(data.Password);
        $("#Pin").val(data.Pin);

        $("#DOB").val(data.DOB ? new Date(data.DOB).toISOString().split('T')[0] : ""); // Convert to YYYY-MM-DD format for input[type="date"]
        $("#Mobile_No").val(data.Mobile_No);
        $("#Email_ID").val(data.Email_ID);
        $("#Aadhar_no").val(data.Aadhar_no);

        $("#IsAcceptedTerms").prop("checked", data.IsAcceptedTerms === 1);
        $("#StatusKey").val(data.StatusKey);

        $("#UserTypeId").val(data.UserTypeId);
        $("#UserTypeName").val(data.UserTypeName);

      
    }




    $(".tab-content").css("height", "100%");
  
    
});




function LoadCompany(USER_MASTER_KEY) {
    $.getJSON("/Authorization/ComanyList",
        {
            USER_MASTER_KEY: USER_MASTER_KEY
        },
        function (data) {
            console.log(data);
            $("#CompanyList").html(data);
        });
}


function Validation_lbl1() {
    
    if (!$("#_lbl1").valid()) {
        return true;
    }


   

    // If there are validation errors, show them and return
 

    
    else if ($("#ItemList input[type='checkbox']:checked").length === 0) {
        $('#ErrorCompany').show();
        return false;
    }
    $('#ErrorCompany').hide();
    return true;
    //SaveRoomData();
}


/**
 * Function to validate user form fields
 */
function validateUserFields() {
    // Reference to the form and required fields
    const form = document.querySelector('.lable1');
    const fields = form.querySelectorAll('input[required], select[required], textarea[required]');
    const dateFields = form.querySelectorAll('input[type="date"]');
    let isValid = true;

    // Validate Mobile Number (10 digits)
    const mobileField = document.querySelector("#Mobile_No");
    const mobilePattern = /^\d{10}$/;
    if (mobileField.value && !mobilePattern.test(mobileField.value)) {
        mobileField.classList.add("is-invalid");
        mobileField.classList.remove("is-valid");
        isValid = false;
    } else {
        mobileField.classList.remove("is-invalid");
        mobileField.classList.add("is-valid");
    }

    // Function to handle validation
    function validateField(field) {
        if (!field.checkValidity()) {
            field.classList.add('is-invalid'); // Show red border
            field.classList.remove('is-valid');
            isValid = false;
        } else {
            field.classList.remove('is-invalid');
            field.classList.add('is-valid'); // Optional: Show green border
        }
    }

    // Validate all required fields
    fields.forEach(field => validateField(field));

    // Validate date fields (only if provided)
    dateFields.forEach(dateField => {
        if (dateField.value) {
            let dob = new Date(dateField.value);
            let today = new Date();
            if (dob > today) {
                dateField.classList.add('is-invalid');
                dateField.classList.remove('is-valid');
                isValid = false;
            } else {
                dateField.classList.remove('is-invalid');
                dateField.classList.add('is-valid');
            }
        } else {
            // If empty, do not mark it as invalid
            dateField.classList.remove('is-invalid');
            dateField.classList.remove('is-valid');
        }
    });

    // Real-time validation feedback
    fields.forEach(field => {
        field.addEventListener("input", () => validateField(field));
    });

    dateFields.forEach(dateField => {
        dateField.addEventListener("input", () => {
            if (dateField.value) {
                validateField(dateField);
            } else {
                dateField.classList.remove('is-invalid');
                dateField.classList.remove('is-valid');
            }
        });
    });

    // If valid, proceed with form submission
    if (isValid) {
        console.log("Form is valid! Ready to submit.");
        // form.submit(); // Uncomment this if you want to submit the form
    } else {
        console.log("Form has errors! Fix them before submitting.");
    }

    // ✅ Return the validation result
    return isValid;
}



$("#SaveAuth_Lavel1").click(function () {


   var isvalid= validateUserFields();

    if (isvalid) {
        if (Validation_lbl1() != false) {

            let userCreationData = {
                UserID: $("#UserID").val() ? parseInt($("#UserID").val()) : null,
                USER_MASTER_KEY: $("#USER_MASTER_KEY").val() ? parseInt($("#USER_MASTER_KEY").val()) : null,
                UserProfileID: $("#UserProfileID").val() ? parseInt($("#UserProfileID").val()) : null,
                User_Unique_ID: $("#User_Unique_ID").val(),
                UserCategoryId: $("#UserCategoryId").val() ? parseInt($("#UserCategoryId").val()) : 0,
                EmailTypeId: $("#EmailTypeId").val() ? parseInt($("#EmailTypeId").val()) : 0,
                UserCategoryName: $("#UserCategoryName").val(),
                FirstName: $("#FirstName").val(),
                MiddleName: $("#MiddleName").val(),
                LastName: $("#LastName").val(),
                FullName: $("#FullName").val(),
                CurrentAddress: $("#CurrentAddress").val(),
                PermanentAddress: $("#PermanentAddress").val(),
                Gender: $("#Gender").val() ? parseInt($("#Gender").val()) : null,
                GenderName: $("#GenderName").val(),
                UserName: $("#UserName").val(),
                Password: $("#Password").val(),
                Aadhar_no: $("#Aadhar_no").val(),
                Pin: $("#Pin").val(),
                DOB: $("#DOB").val() ? new Date($("#DOB").val()).toISOString() : null,
                Mobile_No: $("#Mobile_No").val(),
                Email_ID: $("#Email_ID").val(),
                Aadhar_no: $("#Aadhar_no").val(),
                IsAcceptedTerms: $("#IsAcceptedTerms").prop("checked") ? 1 : 0,
                StatusKey: $("#StatusKey").val() ? parseInt($("#StatusKey").val()) : 0,
            };










            console.log(userCreationData); // Debugging: Check collected data before sending

            $.ajax({
                url: "/Authorization/SaveLabelOne", // Update with your actual API endpoint
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(userCreationData),
                success: function (response) {
                    var data = parseInt(response);
                    if (data > 1) {



                        // Call function after success
                        SaveCompanyListByUser(data, data);
                    }
                    else if (data == -2) {
                        Swal.fire({
                            icon: "error",
                            title: "Error",
                            text: "Username already exists!"
                        }).then((result) => {
                            if (result.isConfirmed || result.isDismissed) {
                                window.location.reload();
                            }
                        });
                    }
                    else {
                        Swal.fire({
                            icon: "error",
                            title: "Error",
                            text: "some error occured, please contact admin!"
                        }).then((result) => {
                            if (result.isConfirmed || result.isDismissed) {
                                window.location.reload();
                            }
                        });
                    }

                },
                error: function (xhr, status, error) {
                    console.error(xhr.responseText);
                    alert("Error creating user!");
                }
            });
        }
        else {
            $('#div_lbl1').animate({ scrollTop: 0 }, 'fast'); // or 'fast'
        }
    }

});

function SaveCompanyListByUser(USER_MASTER_KEY, UserProfileID) {
    var arrItem = [];
    var commaseparatedIds = "";
   

    $("#ItemList li input[type=checkbox]").each(function (index, val) {
        var checkId = $(val).attr("id");
        var arr = checkId.split('_');
        var CurrentCheckboxId = arr[0];
        var IsChecked = $("#" + checkId).is(":checked");

        if (IsChecked) {
            arrItem.push(CurrentCheckboxId);
        }
    });

    if (arrItem.length !== 0) {
        commaseparatedIds = arrItem.toString();

        $.ajax({
            url: "/Authorization/SaveCompanyAccesss",
            type: "POST",
            data: {
                ItemList: commaseparatedIds,
                UserProfileID: UserProfileID,  // Fixed correct parameter assignment
                USER_MASTER_KEY: USER_MASTER_KEY,
                UserID: USER_MASTER_KEY
            },
            success: function (response) {
                Swal.fire({
                    icon: "success",
                    title: "Success",
                    text: "Data saved Successfully!"
                }).then((result) => {
                    if (result.isConfirmed || result.isDismissed) {
                        window.location.reload();

                    }
                });
            },
            error: function (xhr, status, error) {
                console.error(xhr.responseText);
                alert("Error saving company list!");
            }
        });
    }
}


$("#sameAsCurrent").change(function () {
    if ($(this).is(":checked")) {
        $("#PermanentAddress").val($("#CurrentAddress").val()).prop("readonly", true);
    } else {
        $("#PermanentAddress").val("").prop("readonly", false);
    }
});

// Update Permanent Address if Current Address changes & checkbox is checked
$("#CurrentAddress").on("input", function () {
    if ($("#sameAsCurrent").is(":checked")) {
        $("#PermanentAddress").val($(this).val());
    }
});





function showUserwiseCompany(USER_MASTER_KEY) {
    $.getJSON("/Authorization/empComanyList",
        {
            USER_MASTER_KEY: USER_MASTER_KEY
        },
        function (data) {
            console.log(data);
            $("#empCompanyId").html(data);
        });
}
function showUserWiseCompanylbl3(empId) {
    $.getJSON("/Authorization/empComanyList",
        {
            USER_MASTER_KEY: USER_MASTER_KEY
        },
        function (data) {
            console.log(data);
            $("#empCompanyId_lbl3").html(data);
        });
}
function AllUsertypeCheckboxList() {
    
    $.getJSON("/Authorization/UserTypeListCheckbox",
        {
            
        },
        function (data) {
            console.log(data);
            $("#UserTypeList").html(data);
        });
}
function AllUsertypeActionList() {
   

    $.getJSON("/Authorization/UserActionListCheckbox",
        {
           
        },
        function (data) {
           console.log(data);
          $("#ApprovalLevelthree").html(data);


    });
}

$(document).on('change', '#application_ID', function () {
    // Does some stuff and logs the event to the console
    
   
    var application = $('#application_ID').val();
    
   
    $.post("/Authorization/GetModuleAccess",
        {
            application_ID: application,
            Company_Key: $('#empCompanyId_lbl3').val(),
            UserID: $('#UserID').val(),
        },
        function (data) {
            console.log(data);
            $("#ApprovalLevelthree").html(data);
          

        }
    );
   

});

$(document).on('change', '#empCompanyId', function () {
    // Does some stuff and logs the event to the console
    console.log($('#empCompanyId').val());
    if ($('#empCompanyId').val() != 0) {
        $.post("/Authorization/UserTypeCheckbox",
            {
                USER_MASTER_KEY: $("#USER_MASTER_KEY").val(),
                CompanyId: $('#empCompanyId').val()
            },
            function (data) {
                console.log(data);
                $("#UserTypeList").html(data);
              
                
            }
        );
    }
    else {
        $('#UserTypeList').empty();
    }


});


function Validation_lbl2() {
    
    if (!$("#_lbl2").valid()) {
        return false;
    }

    else if ($("#ItemUserTypeList input[type='checkbox']:checked").length === 0) {
        $('#ErrorUserTypeList').show();
        return false;
    }
    $('#ErrorUserTypeList').hide();
    return true;


}









$('[id*="SaveApproval_Lavel2"]').click(function () {
  
   
    if (Validation_lbl2() != false) {

        var arrItem = [];
        var commaseparatedIds = "";
        var UserID = $('#UserID').val();
        var USER_MASTER_KEY = $('#USER_MASTER_KEY').val();
        var UserProfileID = $('#UserProfileID').val();
        var companyId = $('#empCompanyId').val();


        var arr = []; // Initialize an empty array to store checked checkbox IDs
        $('.checkbox-item').each(function () {
            // Check if the checkbox is checked and does not have the 'saved' attribute
            if ($(this).is(':checked')) {
                var checkboxValue = $(this).val();
                console.log("Checkbox Value:", checkboxValue);
                arr.push(checkboxValue);
            }
        });

        //if (arrItem.length != 0) {
        if (arr.length != 0) {

            //commaseparatedIds = arrItem.toString();
            commaseparatedIds = arr.toString();

            $.ajax({
                url: "/Authorization/SaveUserTypeMapDtls",
                type: "POST",

                data: {
                    ItemList: commaseparatedIds,
                    UserProfileID: UserProfileID,
                    USER_MASTER_KEY: USER_MASTER_KEY,
                    CompId: companyId

                },
                success: function (response) {
                    swal.fire("Done", "Record Save SuccessFully !!", "success");

                }
            })
        }
    }

    
});





function LoadUserwiseCompany(USER_MASTER_KEY, selectedCompanyId) {
        ;

        $.getJSON("/Authorization/empComanyList", {
            USER_MASTER_KEY: USER_MASTER_KEY
        }, function (data) {
          
            $("#empCompanyId").html(data);

           
            if (selectedCompanyId) {
                $("#empCompanyId").val(selectedCompanyId);
                LoadUSERUsertypeCheckboxList(USER_MASTER_KEY, selectedCompanyId);
            }
        });

    }


function LoadUSERUsertypeCheckboxList(USER_MASTER_KEY, selectedCompanyId) {
    ;
    $.post("/Authorization/UserTypeCheckbox",
        {
            USER_MASTER_KEY: USER_MASTER_KEY,
            CompanyId: selectedCompanyId
        },
        function (data) {
            console.log(data);
            $("#UserTypeList").html(data);


        }
    );
}
function LoadUserwiseCompanylbl3(USER_MASTER_KEY, empCompanyId_lbl3) {
    ;
    $.getJSON("/Authorization/empComanyList",
        {
            USER_MASTER_KEY: USER_MASTER_KEY
        },
        function (data) {
            console.log(data);
            $("#empCompanyId_lbl3").html(data);
            if (empCompanyId_lbl3) {
                $("#empCompanyId_lbl3").val(empCompanyId_lbl3);
             
                
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            console.error("Error occurred while loading company list:", errorThrown);
        });

   
}





function LoadApplicationlist() {
    $.ajax({
        url: "/Authorization/ApplicationNames", // Ensure this points to your actual endpoint
        type: "GET", // Use GET as the backend expects it
        success: function (data) {
            // Clear the dropdown and append new options
            let $dropdown = $("#application_ID");
            $dropdown.empty(); // Clear existing options

            // Append options
            data.forEach(function (item) {
                $dropdown.append(new Option(item.text, item.value));
            });
        },
        error: function (xhr, status, error) {
            console.error("Error loading application list:", error);
            alert("Failed to load application list. Please try again.");
        }
    });
}

function LoadRolelist() {
    $.ajax({
        url: "/Authorization/RoleNames", // Ensure this points to your actual endpoint
        type: "GET", // Use GET as the backend expects it
        success: function (data) {
            // Clear the dropdown and append new options
            let $dropdown = $("#UserTypeId");
            $dropdown.empty(); // Clear existing options

            // Append options
            data.forEach(function (item) {
                $dropdown.append(new Option(item.text, item.value));
            });
        },
        error: function (xhr, status, error) {
            console.error("Error loading application list:", error);
            alert("Failed to load application list. Please try again.");
        }
    });
}

function LoadUSER_WiseModuleAccess(empId, empCompanyId_lbl3, application, moduleIdCSV) {
   
    $.post("/Authorization/GetModuleAccess", {
        EmpID: empId,
        application_ID: application,
        CompanyId: empCompanyId_lbl3,
        ModuleId: moduleIdCSV
     
    }, function (data) {
        console.log(data);
        $("#ApprovalLevelthree").html(data);

        // Iterate through each module ID

        $.each(moduleIdCSV, function (index, value) {
            var parts = value.split(',');

            // Iterate over each part
            $.each(parts, function (i, part) {
               
                var moduleId = value.split('_')[0]; 
                var actionId = part.trim(); 

                
                var checkboxId = moduleId + "_" + actionId;

                var checkbox = $("#" + checkboxId);
                
                if (checkbox.length > 0) {
                    checkbox.prop('checked', true);
                } else {
                    console.log("Checkbox not found for ID: " + checkboxId);
                }
            });
        });
       

    });
}
function ValidateCheckboxesAccess(ModuleId,ActionId) {
    if ($('#' + ActionId + '_' + ModuleId).prop('checked')) {
        console.log('Checkbox is checked');
        
        $('#deny_' + ModuleId).prop('checked', false);
    } else {
        var totalCheckboxes = $('#' + ModuleId + 'input[type="checkbox"]:checked').length;
        console.log("Total number of checkboxes inside the <tr>: " + totalCheckboxes);
        if (totalCheckboxes === 0) {
            $('#deny_' + ModuleId).prop('checked', false);
        }
        else {
            $('#deny_' + ModuleId).prop('checked', true);
        }
    }
}
function ValidateCheckboxesDeny(id) {
    if ($('#deny_' + id).prop('checked')) {
        console.log('Checkbox is checked');
        $('#' + id + ' input[type="checkbox"]').prop('checked', false); // Replace '3' with the actual ID of the parent <tr> element
    } else {
        console.log('Checkbox is not checked');
    }
}




$(document).on('change', '#empCompanyId_lbl3', function () {
    var usertypeid = $('#UserTypeId').val() || "0";
    var application = $('#application_ID').val() || "0";
    var CompanyId = $('#empCompanyId_lbl3').val() || "0"; // Ensure consistency in the element you're using
    var UserProfileID = $('#UserProfileID').val() || "0";
    var USER_MASTER_KEY = $('#USER_MASTER_KEY').val() || "0";

    console.log(usertypeid);
    console.log(application);

    // First AJAX call to GetModuleAccess_UserCreation
    $.get("/Authorization/GetModuleAccess_UserCreation",
        {
            CompanyId: CompanyId,
            application_ID: application,
            UserTypeId: usertypeid,
            UserProfileID: UserProfileID,
            USER_MASTER_KEY: USER_MASTER_KEY
        },
        function (data) {
            var res = JSON.parse(data);
            var d = res[0];

            console.log(data);
            // Check for undefined or null and display Module
            if (d.Module) {
                $("#ModuleDiv").html(d.Module);
            } else {
                $("#ModuleDiv").html("No module available"); // Provide a placeholder or leave empty
            }
            handleCheckboxes(); // Ensure this function is defined elsewhere
        }
    );

    // Second AJAX call to UserTypeCheckbox
    if (CompanyId !== "0") {  // Check if CompanyId is not '0'
        $.post("/Authorization/UserTypeDropdown_W3",
            {
                USER_MASTER_KEY: USER_MASTER_KEY,
                CompanyId: CompanyId
            },
            function (data) {
                console.log(data);
                $("#UserTypeId").html(data); // Update the UserTypeId dropdown with returned data
            }
        );
    } else {
        $('#UserTypeId').empty(); // Clear dropdown if CompanyId is '0'
    }
});


$(document).on('change', '#UserTypeId', function () {
    var usertypeid = $('#UserTypeId').val() || "0";
    var application = $('#application_ID').val() || "0";
    var CompanyId = $('#empCompanyId_lbl3').val() || "0";
    var UserProfileID = $('#UserProfileID').val() || "0";
    var UserID = $('#UserID').val() || "0";
    var USER_MASTER_KEY = $('#USER_MASTER_KEY').val() || "0";

    console.log(usertypeid);
    console.log(application);

    // Send a GET request
    $.get("/Authorization/GetModuleAccess_UserCreation",
        {
            CompanyId: CompanyId,
            application_ID: application,
            UserTypeId: usertypeid,
            UserProfileID: UserProfileID,
            USER_MASTER_KEY: USER_MASTER_KEY
        },
        function (data) {
            var res = JSON.parse(data);
            var d = res[0];

            console.log(data);
            if (d.Module === undefined || d.Module === null || d.Module === "") {
                $("#ModuleDiv").html(d.Module);
            }
            else {
                $("#ModuleDiv").html(d.Module);
            }
            handleCheckboxes();
        }
    );
});


$(document).on('change', '#application_ID', function () {
    var usertypeid = $('#UserTypeId').val() || "0";
    var application = $('#application_ID').val() || "0";
    var CompanyId = $('#empCompanyId_lbl3').val() || "0";
    var UserProfileID = $('#UserProfileID').val() || "0";
    var UserID = $('#UserID').val() || "0";
    var USER_MASTER_KEY = $('#USER_MASTER_KEY').val() || "0";
    console.log(usertypeid);
    console.log(application);

    // Send a GET request
    $.get("/Authorization/GetModuleAccess_UserCreation",
        {
            CompanyId: CompanyId,
            application_ID: application,
            UserTypeId: usertypeid,
            UserProfileID: UserProfileID,
            USER_MASTER_KEY: USER_MASTER_KEY
        },
        function (data) {
            var res = JSON.parse(data);
            var d = res[0];

            console.log(data);
            if (d.Module === undefined || d.Module === null || d.Module === "") {
                $("#ModuleDiv").html(d.Module);
            }
            else {
                $("#ModuleDiv").html(d.Module);
            }
            handleCheckboxes();
        }
    );
});











function validateW3Fields() {
    // Reference to the form and required fields
    const form = document.querySelector('.lable3');
    const fields = form.querySelectorAll('input[required], select[required], textarea[required]');
    const dateFields = form.querySelectorAll('input[type="date"]');
    let isValid = true;


    function validateField(field) {
        if (!field.checkValidity()) {
            field.classList.add('is-invalid'); // Show red border
            field.classList.remove('is-valid');
            isValid = false;
        } else {
            field.classList.remove('is-invalid');
            field.classList.add('is-valid'); // Optional: Show green border
        }
    }

    // Validate all required fields
    fields.forEach(field => validateField(field));

    // Validate date fields (only if provided)
    dateFields.forEach(dateField => {
        if (dateField.value) {
            let dob = new Date(dateField.value);
            let today = new Date();
            if (dob > today) {
                dateField.classList.add('is-invalid');
                dateField.classList.remove('is-valid');
                isValid = false;
            } else {
                dateField.classList.remove('is-invalid');
                dateField.classList.add('is-valid');
            }
        } else {
            // If empty, do not mark it as invalid
            dateField.classList.remove('is-invalid');
            dateField.classList.remove('is-valid');
        }
    });

    // Real-time validation feedback
    fields.forEach(field => {
        field.addEventListener("input", () => validateField(field));
    });



    // If valid, proceed with form submission
    if (isValid) {
        console.log("Form is valid! Ready to submit.");
        // form.submit(); // Uncomment this if you want to submit the form
    } else {
        console.log("Form has errors! Fix them before submitting.");
    }

    // ✅ Return the validation result
    return isValid;
}




$('[id*="SaveApproval_Lavel3"]').click(function () {
    var isvalid = validateW3Fields();

    if (isvalid) {

    var arrItem = [];

    var UserTypeId = $('#UserTypeId').val();
    var applicationID = $('#application_ID').val();
    var CompanyId = $('#empCompanyId_lbl3').val();
    var UserProfileID = $('#UserProfileID').val() || "0";
    var USER_MASTER_KEY = $('#USER_MASTER_KEY').val() || "0";
    var UserID = $('#USER_MASTER_KEY').val() || "0";
    var arr = []; // Initialize an empty array to store checked checkbox IDs

    // Validate UserTypeId before proceeding
    //if (UserTypeId === "0" || UserTypeId === null || UserTypeId === undefined || UserTypeId === "") {
    //    Swal.fire("Warning", "Please choose a user type.", "warning");
    //    return; // Stop execution if UserTypeId is invalid
    //}
    //if (CompanyId === "0" || CompanyId === null || CompanyId === undefined || CompanyId === "") {
    //    Swal.fire("Warning", "Please choose a Company.", "warning");
    //    return; // Stop execution if CompanyId is invalid
    //}
    // Loop to get checked checkbox IDs
    $('#ModuleTable').find('tr[changed="changed"]').find('input[type="checkbox"]:checked').each(function () {
        var id = $(this).attr('id');

        // Skip checkboxes that have the saved="saved" attribute

        var split = id.split('_');
        var ids = split[0]; // Default to the original ID prefix

        if (split[0] != "1") {
            // Check the prefix and assign `ids` accordingly
            if (split[0] === "2") {
                ids = "2";
            } else if (split[0] === "3") {
                ids = "3";
            } else if (split[0] === "4") {
                ids = "4";
            } else if (split[0] === "5") {
                ids = "5";
            } else if (split[0] === "6") {
                ids = "6";
            } else if (split[0] === "deny") {
                ids = "7";
            }
            id = ids + "_" + split[1]; // Update the `id` with the transformed prefix
            arr.push(id);
        }

        // Add the transformed `id` to the array
    });


    console.log(arr);

    if (arr.length != 0) {
        commaseparatedIds = arrItem.toString();
        $.ajax({
            url: "/Authorization/SaveUserCrationView",
            type: "POST",
            data: {
                CompanyId: CompanyId,
                UserTypeId: UserTypeId,
                ApplicationID: applicationID,
                arrModule: arr,
                UserProfileID: UserProfileID,
                UserID: UserID

            },
            success: function (response) {
                Swal.fire("Done", "Record Save Successfully!", "success");
            }
        });
    }


    }
});



// Attach a change event listener to all checkboxes

$(document).on('change', 'input[type="checkbox"]', function () {
    // Find the closest <tr> for the changed checkbox
    var parentRow = $(this).closest('tr');

    // Add the attribute 'changed' to the corresponding <tr>
    parentRow.attr('changed', 'changed');

    // Optional log for debugging
    console.log('Row updated:', parentRow);
});






function handleCheckboxes() {

    // Attach event listener to checkboxes in the table
    // Access all rows in the ModuleTable
    $('#ModuleTable tr').each(function () {
        var $row = $(this);

        // Logic for deny checkbox
        if ($row.find('.edit').is(':checked') ||
            $row.find('.add').is(':checked') ||
            $row.find('.delete').is(':checked') ||
            $row.find('.print').is(':checked') ||
            $row.find('.view').is(':checked')) {

            $row.find('.deny').prop('checked', false); // Uncheck deny if any of the others are checked
        }

        // Logic for all checkbox
        if ($row.find('.edit').is(':checked') &&
            $row.find('.add').is(':checked') &&
            $row.find('.delete').is(':checked') &&
            $row.find('.print').is(':checked') &&
            $row.find('.view').is(':checked')) {

            $row.find('.all').prop('checked', true); // Check all if all specific checkboxes are checked
        } else {
            $row.find('.all').prop('checked', false); // Uncheck all if any are unchecked
        }
    });


}



function getModuleAccessById(ID) {
    // Log the ID to the console (optional)
    console.log("Application ID: " + ID);

    // Perform the AJAX post request
    $.post("/Authorization/GetModuleAccess",
        {
            application_ID: ID
        },
        function (data) {
            console.log(data); // Log the response data

            // Update the ModuleTable with the response HTML
            $("#ModuleDiv").html(data);
        }
    );
}






$(document).on("click", ".all", function () {
    var $row = $(this).closest('tr');
    $row.find('.add, .edit, .delete, .print, .view').prop('checked', $(this).is(':checked'));
    $row.find('.deny').prop('checked', !$(this).is(':checked'));
});

$(document).on("click", ".add", function () {
    var $row = $(this).closest('tr');
    if ($(this).is(':checked')) {
        $row.find('.deny').prop('checked', false);
        if ($row.find('.edit').is(':checked') &&
            $row.find('.delete').is(':checked') &&
            $row.find('.print').is(':checked') &&
            $row.find('.view').is(':checked')) {
            $row.find('.all').prop('checked', true); // Check the "all" checkbox
        }
    } else {
        $row.find('.all').prop('checked', false);
        if (!$row.find('.edit, .delete, .print, .view').is(':checked')) {
            $row.find('.deny').prop('checked', true);
        }
    }
});

$(document).on("click", ".edit", function () {
    var $row = $(this).closest('tr');
    if ($(this).is(':checked')) {
        $row.find('.deny').prop('checked', false);
        if ($row.find('.add').is(':checked') &&
            $row.find('.delete').is(':checked') &&
            $row.find('.print').is(':checked') &&
            $row.find('.view').is(':checked')) {
            $row.find('.all').prop('checked', true);
        }
    } else {
        $row.find('.all').prop('checked', false);
        if (!$row.find('.add, .delete, .print, .view').is(':checked')) {
            $row.find('.deny').prop('checked', true);
        }
    }
});

$(document).on("click", ".delete", function () {
    var $row = $(this).closest('tr');
    if ($(this).is(':checked')) {
        $row.find('.deny').prop('checked', false);
        if ($row.find('.edit').is(':checked') &&
            $row.find('.add').is(':checked') &&
            $row.find('.print').is(':checked') &&
            $row.find('.view').is(':checked')) {
            $row.find('.all').prop('checked', true);
        }
    } else {
        $row.find('.all').prop('checked', false);
        if (!$row.find('.edit, .add, .print, .view').is(':checked')) {
            $row.find('.deny').prop('checked', true);
        }
    }
});

$(document).on("click", ".view", function () {
    var $row = $(this).closest('tr');
    if ($(this).is(':checked')) {
        $row.find('.deny').prop('checked', false);
        if ($row.find('.edit').is(':checked') &&
            $row.find('.delete').is(':checked') &&
            $row.find('.print').is(':checked') &&
            $row.find('.add').is(':checked')) {
            $row.find('.all').prop('checked', true);
        }
    } else {
        $row.find('.all').prop('checked', false);
        if (!$row.find('.edit, .delete, .add, .print').is(':checked')) {
            $row.find('.deny').prop('checked', true);
        }
    }
});

$(document).on("click", ".print", function () {
    var $row = $(this).closest('tr');
    if ($(this).is(':checked')) {
        $row.find('.deny').prop('checked', false);
        if ($row.find('.edit').is(':checked') &&
            $row.find('.delete').is(':checked') &&
            $row.find('.add').is(':checked') &&
            $row.find('.view').is(':checked')) {
            $row.find('.all').prop('checked', true);
        }
    } else {
        $row.find('.all').prop('checked', false);
        if (!$row.find('.edit, .delete, .add, .view').is(':checked')) {
            $row.find('.deny').prop('checked', true);
        }
    }
});

$(document).on("click", ".deny", function () {
    var $row = $(this).closest('tr');
    $row.find('.add, .edit, .delete, .print, .view').prop('checked', !$(this).is(':checked'));
    $row.find('.all').prop('checked', !$(this).is(':checked'));
});


$(document).on('click', '.toggle-button', function () {
    var target = $(this).data('target'); // Get the target from data-target

    // Toggle the visibility of the rows with the class specified in data-target
    $('.' + target).toggle(); // Show or hide the submenu rows

    // Update the aria-expanded attribute and button text
    var isExpanded = $(this).attr('aria-expanded') === 'true';
    $(this).attr('aria-expanded', !isExpanded); // Toggle the expanded attribute
    $(this).text(isExpanded ? '+' : '-'); // Change button text accordingly

    // Find and toggle nested targets
    $('.' + target).each(function () {
        var nestedTarget = $(this).data('target'); // Get nested target from the current row

        if (nestedTarget) {
            // Toggle visibility of nested rows
            $('.' + nestedTarget).toggle();
            var nestedButton = $(this).find('.toggle-button'); // Find toggle button within this row
            if (nestedButton.length > 0) {
                var isNestedExpanded = nestedButton.attr('aria-expanded') === 'true';
                nestedButton.attr('aria-expanded', !isNestedExpanded); // Toggle the expanded attribute
                nestedButton.text(isNestedExpanded ? '+' : '-'); // Change button text accordingly
            }
        }
    });
});



