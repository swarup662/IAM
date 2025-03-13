function Validation() {
    //debugger;
    if (!$("#_APR_S1").valid()) {
        $("#_APR_S1").addClass("was-validated");
        return false;

    }
  
}

function toggleCompanySelection() {
    var selectAllCheckbox = document.querySelector('input[value="100"]');
    var checkboxes = document.querySelectorAll('.checkCompany:not([value="100"])');

    if (selectAllCheckbox.checked) {
        checkboxes.forEach(function (checkbox) {
            checkbox.checked = true;
        });
    } else {
        checkboxes.forEach(function (checkbox) {
            checkbox.checked = false;
        });
    }
}

function updateSelectAll() {
    var selectAllCheckbox = document.querySelector('input[value="100"]');
    var checkboxes = document.querySelectorAll('.checkCompany:not([value="100"])');

    if ([...checkboxes].some(checkbox => !checkbox.checked)) {
        selectAllCheckbox.checked = false;
    }
    else {
        selectAllCheckbox.checked = true;
    }
}



$('#SaveApproval_Lavel1').click(function () {
    debugger;
    if (Validation() != false) {
        

        var rdb_aprpath;
        if ($('#Rdb_Manual').prop('checked')) {
            rdb_aprpath = 1;

        }
        if ($('#Rdb_Auto').prop('checked')) {
            rdb_aprpath = 2;
        }


        var rdb_apvrchoose;
        if ($('#Rdb_Yes').prop('checked')) {
            rdb_apvrchoose = 1;

        }
        if ($('#Rdb_No').prop('checked')) {
            rdb_apvrchoose = 2;
        }

        var SelectedComp = [];
        $('.checkbox').each(function () {
            var id = $(this).find('input[type="checkbox"]').attr('id');

            if ($(this).find('input[type="checkbox"]').prop('checked')) {
                SelectedComp.push($(this).find('input[type="checkbox"]').attr('id'));
            }
        });
        var selectedCompString = SelectedComp.join(', ');


        var data = {
            Application_Name_Id: $('#Application_Name_Id').val(),
            Application_Module_Id: $('#Application_Module_Id').val(),
            Approval_Type: $('#Approval_Type').val(),
            COMPANY_KEY: selectedCompString,
            Approval_choose_: rdb_apvrchoose,
            Approval_Path_setup_type: rdb_aprpath,
            Approval_level_One_key: $('#Approval_level_One_key').val(),
            OprMode_Id: $('#OprMode_Id').val()

        };

        $.ajax({
            type: "POST",
            url: "/Approval/SaveApprovalSetupLabelOne",
            data: data,  // Sending plain object without JSON.stringify
            success: function (data) {
                if (data.msg === "Success") {
                    Swal.fire({
                        title: "Data Saved Successfully",
                        icon: "success",
                        showConfirmButton: false,
                        timer: 2000

                    });
                    $("#Approval_level_One_key").val(data.info);
                } else {
                    swal.fire("Oops!", "Please Contact Admin", "error");
                }
            },
            error: function () {
                swal.fire("Error", "An error occurred", "error");
            }
        });
    }
});





function GetUserType(rowCount) {
    $.getJSON("/Approval/UserType", {}, function (data) {
        var msg = "<option value=''>-Select Option-</option>";
        $.each(data, function (i, dt) {
            msg += "<option value='" + dt["value"] + "'>" + dt["text"] + "</option>";
        });

        $("select[name=form" + rowCount + "]").html(msg);
    });
}



$('#SaveApproval_Lavel2').click(function () {
    //debugger;
    var SelectedOpt = [];
    var Step = [];

    $('tr.data-contact-person').each(function (index) {
        var USER_TYPE = $(this).find('.form01').val();

        if (USER_TYPE && USER_TYPE.trim() !== '') { 
            SelectedOpt.push(USER_TYPE);
            Step.push(index + 1);
        }
    });

    var SelectedoptStr = SelectedOpt.join(', ');
    var stepstr = Step.join(', ');

    var postdata = {
        arr_opt: SelectedoptStr,
        Approval_level_One_key: $('#Approval_level_One_key').val(),
        Approval_level_two_key: $('#Approval_level_two_key').val(),
        StepNo: stepstr
    }

    $.post("/Approval/SaveApprovalSetupTwo", postdata, function (data) {

        console.log(data);
        //success: function (data)
        if (data.id > 0) {
            //debugger;

            Swal.fire({
                title: "Data Saved Successfully",
                icon: "success",
                showConfirmButton: false,
                timer: 2000

            });
            //   $("#Approval_level_two_key").val(data.id);

       //     LoadLevelThree($('#Approval_level_One_key').val());
            LoadLeveltwodetails($('#Approval_level_One_key').val());
        } else {
            swal.fire("Oops!", "Please Contact Admin", "error");
        }

    }).fail(function (error) {

        swal.fire("Alert", "Something went wrong!!", "error");
    });

});

$('#Selectedcompany').change(function () {
    $.getJSON("/Approval/FetchdataApproval_level_three",
        {
            Approval_level_One_key: $('#Approval_level_One_key').val(),
            CompanyID: $('#Selectedcompany').val()
        },
        function (data) {
            console.log(data);
            $("#ApprovalLevelthree").val();

            $("#ApprovalLevelthree").html(data);
            //$('#Approval_level_One_key').val();
            //$('#Approval_level_two_key').val();

        });


})


function LoadSelectedCompany(LabelOneId) {

    $.get("/Approval/GetSelectedCompany",
        {
            LevelOneId: LabelOneId
        },

        function (data) {
            //debugger;
            //console.log(data);
            $('#Selectedcompany').empty();
            $('#Selectedcompany')
                .append($("<option></option>")
                    .attr("value", "0")
                    .text("Select"));

            $.each(data, function (key, value) {
                $('#Selectedcompany')
                    .append($("<option></option>")
                        .attr("value", value.value)
                        .text(value.text));

            });


        }
    );

}

function AddEmployee(APPROVAL_LVL1_KEY, APPROVAL_LVL2_KEY, APPROVAL_User_Type_Key) {
    var employeeSaveModal = new bootstrap.Modal(document.getElementById('EmployeeSaveModal'));
    employeeSaveModal.show();
    $('#UserType_key').val(APPROVAL_User_Type_Key);
    // alert($('#UserType_key').val());

    $("#Usernames").val('');

}


function closeEmployeeSaveModal() {
    $("#Usernames").val('');

    var modal = document.getElementById('EmployeeSaveModal');
    modal.classList.remove('show');
    modal.style.display = 'none';
    var modalBackdrop = document.querySelector('.modal-backdrop');
    if (modalBackdrop) {
        modalBackdrop.remove();
    }
}




function ApprovalDetails(ApprovalLblOneId) {
    var info = "";
    var ModelId = "";
    var ModelName = "";
    if (ApprovalLblOneId == "0") {
        alert("New");
   
    } else {
        $("#Approval_level_One_key").val(ApprovalLblOneId);
        //alert(ApprovalLblOneId);
    }
}

function LoadLeveltwodetails(LabeltwoId) {
    ////debugger;
    console.log(LabeltwoId);

    $.getJSON("/Approval/FetchdataApproval_level_two",
        {
            Approval_level_One_key: LabeltwoId

        },
        function (data) {

            $("#approvalleveltwo").html(data);
            //$('#Approval_level_One_key').val();
            //$('#Approval_level_two_key').val();

        });
}

function LoadLevelTWOPART2(LabelOneId) {
    ////debugger;
    console.log(LabelOneId); // Check the value in the browser console

    $.getJSON("/Approval/FetchdataApproval_level_two",
        {
            Approval_level_One_key: LabelOneId
            //,StepNo: LabelOneId,
            //UserType_key: LabelOneId
        },
        function (data) {
            $("#approvalleveltwo").html(data);
            //$("#ApprovalLevelthree").html(data);
            //$('#Approval_level_One_key').val();
            //$('#Approval_level_two_key').val();

        });
}





$(document).on('change', '#Application_Name_Id', function () {
    ////debugger;
    $.post("/Approval/GetModulenames",
        {
            RecType: 'MAIN_MENU',
            Application_Name_Id: $('#Application_Name_Id').val(),
            Menu_Id: $('#Application_Main_Menu_Id').val()
        },
        function (data) {
            ////debugger;
            console.log(data);

            var dropdown = $('#Application_Main_Menu_Id');
            dropdown.empty();    

            $('#Application_Main_Menu_Id')
                .append($("<option></option>")
                    .attr("value", "0")
                    .text("Select"));

            // Iterate through the data array and add options to the dropdown
            for (var i = 0; i < data.length; i++) {
                var item = data[i];
                dropdown.append($('<option></option>').attr('value', item.value).text(item.text));
            }

            // Optionally, trigger a change event on the dropdown if needed
            dropdown.trigger('change');
        }
    );
});

$(document).on('change', '#Application_Main_Menu_Id', function () {

    $.post("/Approval/GetModulenames",
        {
            RecType: 'MODULE',
            Application_Name_Id: $('#Application_Name_Id').val(),
            Menu_Id: $('#Application_Main_Menu_Id').val()
        },
        function (data) {
            console.log(data);

            var dropdown = $('#Application_Module_Id');
            dropdown.empty(); 

            $('#Application_Module_Id')
                .append($("<option></option>")
                    .attr("value", "0")
                    .text("Select"));

            // Iterate through the data array and add options to the dropdown
            for (var i = 0; i < data.length; i++) {
                var item = data[i];
                dropdown.append($('<option></option>').attr('value', item.value).text(item.text));
            }

            // Optionally, trigger a change event on the dropdown if needed
            dropdown.trigger('change');
        }
    );
});
function LoadLevelThree(Approval_L1_ID) {
    
    $.getJSON("/Approval/Fetch_L3_Dtls",
        {
            Approval_level_One_key: Approval_L1_ID
            
        },
        function (d) {
            

            //$('#L3DetailsGrid').html(data.tbody);
            console.log(d); // Ensure this is the HTML string you expect
            $('#L3DetailsGrid').html(d); 

        }
    )
}



/// <reference path="../js/site.js" />
/// <reference path="../assets/plugins/smart-wizard/js/jquery.smartwizard.min.js" />

$(document).ready(function () {
    //Data For First Wizard

    // Toolbar extra buttons
    var btnFinish = $('<button></button>').text('Finish').addClass('btn btn-info').on('click', function () {
        window.location.href = "/Approval/Approval";
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
            ApprovalDetails($('#Approval_level_One_key').val())

            //   LoadCompany($("#EmployeeID").val());



        }
        else if (stepNumber == "1") {
            debugger;
            if ($('#Approval_Type').val() === "5") {

                document.getElementById('st2').style.display = 'none';
            }
            else {
              
                LoadLevelTWOPART2(($('#Approval_level_One_key').val()));
            }


        }
        else if (stepNumber == "2") {
            debugger;
            if ($('#Rdb_Yes').prop('checked')) {
                document.getElementById('st3').style.display = 'block';

                // added to fire st3
                LoadLevelThree($('#Approval_level_One_key').val());
                LoadSelectedCompany($('#Approval_level_One_key').val());

            }
            else {
                document.getElementById('st3').style.display = 'none';
              

            }
            if ($('#Approval_Type').val() === "5") {

                document.getElementById('st3').style.display = 'none';
            }
            else {
                if ($('#Rdb_No').prop('checked')) {

                    document.getElementById('st3').style.display = 'none';
                }
                else {
                    document.getElementById('st3').style.display = 'block';

                    // added to fire st3
                    LoadLevelThree($('#Approval_level_One_key').val());
                    LoadSelectedCompany($('#Approval_level_One_key').val());


                }
               
                
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

    var idValue;
    if (window.location.href.indexOf('?') !== -1) {
        idValue = decodeIdFromUrl();
    }
    else {
        $("#Approval_level_One_key").val("");
        $("#Application_Name_Id").val("");
        $("#Application_Main_Menu_Id").val("");
        $("#Application_Module_Id").val("");
        $("#Approval_Type").val("");
        $("#Approval_Path_setup_type").val("");
        $("#Approval_choose_").val("");
        $("#COMPANY_KEY").val("");


        //   LoadCompany();
    }
    /* var idValue = decodeIdFromUrl();*/
    function decodeIdFromUrl() {

        ////debugger;
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

    if (idValue !== null) {
        //alert("not null");

        var actionIdCSV = [];
        var moduleIdCSV = [];
        var e = "";

        $.get("/Approval/_ApprovalView",
            {
                ApprovalLevelKey: idValue
            },
            function (d) {
                debugger;
                console.log(d);
                $("#Approval_level_One_key").val(d.approval_level_One_key);   //Application_Main_Menu_Id
                $("#Application_Name_Id").val(d.application_Name_Id);
                $("#Application_Main_Menu_Id").val(d.application_Main_Menu_Id);
                $("#Application_Module_Id").val(d.application_Module_Id);
                $("#Approval_Type").val(d.approval_Type);
                $("#Approval_Path_setup_type").val(d.approval_Path_setup_type);    // RDB Approval PathSetUp
                $("#Approval_choose_").val(d.approval_choose_);        //RDB  Approver Choose
                $("#COMPANY_KEY").val(d.companY_KEY);
                $("#OprMode_Id").val(d.oprMode_Id);
                
                //debugger;

                if (d.companY_KEY != undefined) {
                    var selectedcompany = d.companY_KEY.split(',').map(function (item) {
                        return item.trim(); // Remove any leading or trailing spaces
                    });
                    console.log(selectedcompany);


                }


                if (d.approval_Path_setup_type == 1) {

                    $('#Rdb_Manual').prop('checked', true);

                }
                if (d.approval_Path_setup_type == 2) {

                    $('#Rdb_Auto').prop('checked', true);

                }
                if (d.approval_choose_ == 1) {

                    $('#Rdb_Yes').prop('checked', true);

                }
                if (d.approval_choose_ == 2) {

                    $('#Rdb_No').prop('checked', true);

                }
                Get_company();
                Get_MainMenu(d.application_Name_Id, d.application_Main_Menu_Id);
                Get_Module(d.application_Name_Id, d.application_Main_Menu_Id, d.application_Module_Id);
                function Get_company() {
                    //debugger;
                    $.post("/Approval/FetchCompanyName",
                        {
                            COMPANY_KEY: "0"
                        },
                        function (data) {
                            //debugger;

                            $('#CompanyContainer').html(data);

                            $('#CompanyContainer input[type="checkbox"][name="checkbox"]').each(function () {
                                var id = $(this).attr('id');
                                console.log("Checkbox ID:", id, " | Should be checked:", selectedcompany.includes(id));
                                if (selectedcompany.includes(id)) {
                                    $(this).prop('checked', true);
                                    console.log("Checkbox ID:", id, " | Checked:", selectedcompany.includes(id));
                                }
                            });


                        }

                    )
                };

                function Get_MainMenu(Application_Id, MainMenu_Id) {
                    //debugger;
                    $.post("/Approval/GetModulenames",
                        {
                            RecType: 'MAIN_MENU',
                            Application_Name_Id: Application_Id,
                            Menu_Id: "0"
                        },
                        function (data) {
                            //////debugger;
                            console.log("Data received:", data);

                            var dropdown = $('#Application_Main_Menu_Id');
                            dropdown.empty();

                            dropdown.append($("<option></option>")
                                .attr("value", "0")
                                .text("Select"));

                            for (var i = 0; i < data.length; i++) {
                                var item = data[i];
                                dropdown.append($('<option></option>')
                                    .attr('value', item.value.toString())
                                    .text(item.text));
                            }
                            //////debugger;
                            // Set the selected value and trigger change
                            const selectedValue = MainMenu_Id.toString(); // Ensure it's a string
                            $('#Application_Main_Menu_Id').val(selectedValue).trigger('change');
                        }
                    );
                }
                function Get_Module(Application_Id, MainMenu_Id, Module_Id) {
                    //debugger;
                    $.post("/Approval/GetModulenames",
                        {
                            RecType: 'MODULE',
                            Application_Name_Id: Application_Id,
                            Menu_Id: MainMenu_Id
                        },
                        function (data) {
                            ////debugger;
                            console.log("Data received:", data);

                            var dropdown = $('#Application_Module_Id');
                            dropdown.empty();

                            dropdown.append($("<option></option>")
                                .attr("value", "0")
                                .text("Select"));

                            for (var i = 0; i < data.length; i++) {
                                var item = data[i];
                                dropdown.append($('<option></option>')
                                    .attr('value', item.value.toString())
                                    .text(item.text));
                            }
                            ////debugger;
                            // Set the selected value and trigger change
                            const selectedValue = Module_Id.toString(); // Ensure it's a string
                            $('#Application_Module_Id').val(selectedValue).trigger('change');
                        }
                    );
                }


            }
        )
    }

    else {
        //alert("null");
    }

});

function srch(Srch_Key,UserType_Key,CompanyID) {
    const inputId = `searchemployee${Srch_Key}`;
    const Savebtn = `save-btn-${Srch_Key}`;
    // added 2day
    const hiddenInputId = `employee_master_key_${Srch_Key}`;

    $(`#${inputId}`).autocomplete({

        source: function (request, response) {

            debugger;
            $.ajax({
                url: "/Approval/GetUserDetails",
                type: "POST",
                dataType: "json",
                data: {
                    prefix: request.term,
                    UserType_Key: UserType_Key,
                    Company_Key: CompanyID
                },
                success: function (data) {
                    console.log(data);
                    response($.map(data, function (item) {
                        
                        console.log(item);

                        return {
                            value: item.employee_master_key,
                            label: item.employee_Name

                        };
                    }));
                },
                error: function (xhr, status, error) {
                    console.log(error);
                }
            });
        },
        select: function (event, ui) {
            console.log(ui.item.label);
            console.log(ui.item.value);

            $(`#${inputId}`).val(ui.item.label);
            //  $("#employee_master_key").val(ui.item.value);
          // added 2day
            $(`#${hiddenInputId}`).val(ui.item.value);
            document.getElementById(Savebtn).style.display = 'inline-block';

            return false;
        }
    });
}

function SaveApproval_Lavel3(ApvL1_Key, ApvL2_Key, APPROVAL_User_Type_Key) {
    const Savebtn = `save-btn-${APPROVAL_User_Type_Key}`;
    //added 
    const hiddenInputId = `employee_master_key_${APPROVAL_User_Type_Key}`;
    const employeeMasterKey = $(`#${hiddenInputId}`).val();

    //debugger;
    var data = {
   
        Approval_level_One_key: ApvL1_Key,
        Approval_level_two_key: ApvL2_Key,
        employee_master_key: employeeMasterKey, //$('#employee_master_key').val(),
        COMPANY_KEY: $('#Selectedcompany').val()
    };
    console.log(data);

    $.ajax({
        type: "POST",
        url: "/Approval/SaveApprovalSetupThree",
        data: data,  
        success: function (data) {
            //debugger;
            if (data.id > 0) {
                Swal.fire({
                    title: "Data Saved Successfully",
                    icon: "success",
                    showConfirmButton: false,
                    timer: 1000

                });
                $(`#${Savebtn}`).prop("disabled", true);
                LoadLevelThree(ApvL1_Key);
            } else {
                swal.fire("Oops!", "Please Contact Admin", "error");
            }
        },
        error: function () {
            swal.fire("Error", "An error occurred", "error");
        }

          
    });


}



function DeleteApprovalStep(APVL_L1_KEY, APVL_L2_KEY) {

    Swal.fire({
        title: 'Do you want to Delete?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.getJSON("/Approval/DeleteApprovalStep",
                {
                    Approval_level_One_key: APVL_L1_KEY,
                    Approval_level_Two_key: APVL_L2_KEY
                },
                function (data) {

                    if (data.msg = "success") {
                        swal.fire("Done", "Record Deleted SuccessFully !!", "success");
                    }
                    else {
                        swal.fire("Oppss!!!", "Please Contact Admin", "error");
                    }
                    // window.location.reload();
                    setTimeout(function () {
                        window.location.reload();
                    }, 3000);
                });
        } else if (result.isDenied) {
            Swal.fire('Welcome ', '', 'info')
        }
    })

}