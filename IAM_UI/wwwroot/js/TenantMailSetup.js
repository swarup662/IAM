

//Back Button
var BackButton = document.getElementById("BackButton");
BackButton.addEventListener("click", function () {
    location.reload();
});



//Referesh Button
var refereshButton = document.getElementById("RefreshModel");
refereshButton.addEventListener("click", function () {
    location.reload();
});




// Datatable
$(document).ready(function () {
    $('#PurposeTable').DataTable();

});


//Populating Raised Section Module Names on Change Application
$(document).on('change', '#PurposeApplicationId', function () {
    let purposeId = $('#PurposeApplicationId').val();
    if (purposeId != 0) {
        $.post("/MailSetup/GetPurposeById", { ApplicationId: purposeId },
            function (data) {
                let purposeList = JSON.parse(data);

                $("#TenantMailSetupPurposeKey").empty();

                $("#TenantMailSetupPurposeKey").append('<option value="">-- Select --</option>');

                purposeList.forEach(function (module) {
                    $("#TenantMailSetupPurposeKey").append(

                        `<option value="${module.TenantMailSetupPurposeKey}">${module.PurposeName}</option>`
                    );

                });
            });
    }
    else {
        $('#TenantMailSetupPurposeKey').empty().append('<option value="">-- Select --</option>');
    }
});








$(document).on("click", "#AddRow", function () {

    const form = document.querySelector('.needs-validation');
    const fields = form.querySelectorAll('input[required], select[required], textarea[required]');
    let isValid = true;

    // Loop through all required fields and check validity
    fields.forEach(field => {
        if (!field.checkValidity()) {
            field.classList.add('is-invalid'); // Add invalid styling
            isValid = false;
        } else {
            field.classList.remove('is-invalid');
            field.classList.add('is-valid'); // Optional: Add valid styling
        }

        // Real-time validation feedback
        field.addEventListener('input', function () {
            if (field.checkValidity()) {
                field.classList.remove('is-invalid');
                field.classList.add('is-valid');
            } else {
                field.classList.add('is-invalid');
            }
        });
    });


    $("#PurposeApplicationId, #TenantMailSetupPurposeKey").on("change", function () {
        clearInputFields();
    });

    function clearInputFields() {
        $("#ReceiverMail").val("");
    }

    if (isValid) {



        let receiverMail = $("#ReceiverMail").val();

        let rowCount = $("#EditAllUser tr").length + 1; // Serial number

        let newRow = `
        <tr>
            <td>${rowCount}</td>
            <td>${receiverMail}</td>
            <td>
                <a href="javascript:;" class="action-icon delete-row">
                    <i class="mdi mdi-delete" data-toggle="tooltip" title="Delete" style="color:red"></i>
                </a>
            </td>
        </tr>
    `;

        $("#EditAllUser").append(newRow);
        $("#ReceiverMail").val(""); // Clear input after adding


    }
});

// Delete Row Functionality
$(document).on("click", ".delete-row", function () {
    console.log("Deleting Row:", $(this).closest("tr").html());
    $(this).closest("tr").remove();
    updateSerialNumbers();
});

// Function to update serial numbers after deleting a row
function updateSerialNumbers() {
    $("#EditAllUser tr").each(function (index) {
        $(this).find("td:first").text(index + 1);
    });
    console.log("Updated Serial Numbers After Deletion");
}











$("#SaveMailSetup").click(function () {
    // Check if there's at least one row in the grid before saving
    if ($("#EditAllUser tr").length === 0) {
        const IsReceiverMailRequired = $('#IsReceiverMailRequired').prop('checked') ? 1 : 0; 
        if (IsReceiverMailRequired ==0) {
            Swal.fire({
                title: "No Data",
                text: "Please add at least one record to the grid before saving.",
                icon: "warning",
                showConfirmButton: true,
                confirmButtonText: 'OK'
            });
            return; // Prevent save if grid is empty
        }
      
    }

    // Get form values
    const purposeId = $("#TenantMailSetupPurposeKey").val();
    const applicationId = $("#PurposeApplicationId").val();
    const senderMail = $("#SenderMail").val();
    const senderPassword = $("#SenderPassword").val();
    const isCC = $('#IsCC').prop('checked') ? 1 : 0;
    const isAttachment = $('#IsAttachment').prop('checked') ? 1 : 0;

    const MailBody = $("#MailBody").val();
    const PageLink = $("#PageLink").val();
    const MailSubject = $("#MailSubject").val();
    const TimeSlotMinute = $("#TimeSlotMinute").val();
    const Parameter1 = $("#Parameter1").val();
    const Parameter2 = $("#Parameter2").val();
    const Parameter3 = $("#Parameter3").val();


    // Get all table data, including existing and newly added ReceiverMails
    const tableData = getTableData();

    // Handle Service Desk ID (set to 0 if not provided)
    var id = $('#TenantMailSetupKey').val();
    if (!id) {
        id = 0;
    }

    // Prepare the model to send to the server
    const model = {
        TenantMailSetupKey: id,
        TenantMailSetupPurposeKey: purposeId,
        PurposeApplicationId: applicationId,
        SenderMail: senderMail,
        SenderPassword: senderPassword,
        MailBody: MailBody,
        MailSubject: MailSubject,
        IsCC: isCC,
        IsAttachment: isAttachment,
        PageLink: PageLink,
        TimeSlotMinute: TimeSlotMinute,
        Parameter1: Parameter1,
        Parameter2: Parameter2,
        parameter3: Parameter3,

        Maildata: tableData.map(row => ({
            ReceiverMail: row.receiverMail,
        }))
    };

    console.log(model); // Log the data being sent for verification

    // Send data to the server using AJAX
    $.ajax({
        url: '/MailSetup/SaveMailSetup',
        type: 'POST',
        data: JSON.stringify(model),
        contentType: 'application/json',
        success: function (response) {
            var resultStatus = response.status;
            if (resultStatus === "success") {
                Swal.fire({
                    title: "Done",
                    text: "Record Saved Successfully !!",
                    icon: "success",
                    timer: 3000,
                    showConfirmButton: false,
                    timerProgressBar: true
                });
            } else if (resultStatus === "exist") {
                Swal.fire({
                    title: "Oppss!!!",
                    text: "Record already exists",
                    icon: "error",
                    timer: 3000,
                    showConfirmButton: false,
                    timerProgressBar: true
                });
            } else if (resultStatus === "fail") {
                Swal.fire({
                    title: "Oppss!!!",
                    text: "Please Contact Admin",
                    icon: "error",
                    timer: 3000,
                    showConfirmButton: false,
                    timerProgressBar: true
                });
            }

            setTimeout(function () {
                window.location.reload();
            }, 2000);
        },
        error: function (xhr, status, error) {
            console.log("Error:", error);
            Swal.fire({
                title: "Oppss!!!",
                text: "An error occurred while saving data.",
                icon: "error",
                timer: 3000,
                showConfirmButton: false,
                timerProgressBar: true
            });
        }
    });
});





function getTableData() {
    const tableData = [];
    const table = document.getElementById("AddPurposeTable");
    const rows = table.querySelectorAll("tbody tr");

    rows.forEach(row => {
        const cells = row.querySelectorAll("td");
        const rowData = {
            receiverMail: cells[1]?.textContent.trim() || "",

        };
        tableData.push(rowData);
    });

    return tableData;
}





function populateModules(purposeId, purposeApplicationId) {
    if (purposeId != 0) {
        $.post("/MailSetup/GetPurposeById", { ApplicationId: purposeId }, function (data) {
            let purposeList = JSON.parse(data);

            $("#TenantMailSetupPurposeKey").empty();

            $("#TenantMailSetupPurposeKey").append('<option value="">-- Select --</option>');

            purposeList.forEach(function (module) {
                $("#TenantMailSetupPurposeKey").append(
                    `<option value="${module.TenantMailSetupPurposeKey}">${module.PurposeName}</option>`
                );
            });
            $('#TenantMailSetupPurposeKey').val(purposeApplicationId);
        });
    } else {
        $('#TenantMailSetupPurposeKey').empty().append('<option value="">-- Select --</option>');
    }

}



//For edit item
function EditItem(TenantMailSetupKey) {
    $('#TenantMailSetupKey').val(TenantMailSetupKey);

    $.get("/MailSetup/EditMailSetup", { id: TenantMailSetupKey })
        .done(function (data) {
            console.log(data);

            var response = data;

            if (response) {
                $('#TenantMailSetupKey').val(response.tenantMailSetupKey);
                $('#PurposeApplicationId').val(response.application_ID);
                populateModules(response.application_ID, response.tenantMailSetupPurposeKey);
                $('#SenderMail').val(response.senderMail);
                $('#SenderPassword').val(response.senderPassword);
                $('#MailBody').val(response.mailBody);
                $('#MailSubject').val(response.mailSubject);
                $('#PageLink').val(response.pageLink);
                $('#Parameter1').val(response.parameter1);
                $('#Parameter2').val(response.parameter2);
                $('#Parameter3').val(response.parameter3);
                $('#TimeSlotMinute').val(response.timeSlotMinute);
                $('#IsCC').prop('checked', response.isCC == 1);
                $('#IsAttachment').prop('checked', response.isAttachment == 1);


                $('#EditAllUser').html(response.editAllUser);
                $('#AddPurpose').modal('show');
                if (!response.editAllUser || response.editAllUser.trim() === "") {
                    $('#IsReceiverMailRequired').prop('checked', true).trigger('change'); // Uncheck and trigger change event
                    $('#AddRow').prop('disabled', true); // Disable AddRow button
                    $('#AddPurposeTable').css('opacity', '0.5'); // Blur AddPurposeTable
                    $('#ReceiverMail').prop('disabled', true); // Disable ReceiverMail textbox
                } else {
                    $('#IsReceiverMailRequired').prop('checked', false).trigger('change'); // Check and trigger change event
                }
                document.getElementById("staticBackdropLabel").innerHTML = "Saas Bill Mail Setup Form :: [Edit]";
            } else {
                Swal.fire("Error", "No data found for the selected item.", "error");
            }
        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            console.error('Error occurred:', textStatus, errorThrown);
            console.error('Response Text:', jqXHR.responseText);

            Swal.fire("Error", "An error occurred while processing the request. Please check the server logs.", "error");
        });
}





$('#IsReceiverMailRequired').change(function () {
    if ($(this).prop('checked')) {
        $('#AddRow').prop('disabled', true); // Disable AddRow button
        $('#AddPurposeTable').css('opacity', '0.5'); // Blur AddPurposeTable
        $('#ReceiverMail').prop('disabled', true); // Disable ReceiverMail textbox
    } else {
        $('#AddRow').prop('disabled', false); // Enable AddRow button
        $('#AddPurposeTable').css('opacity', '1'); // Remove blur from AddPurposeTable
        $('#ReceiverMail').prop('disabled', false); // Enable ReceiverMail textbox
    }
});



//For delete Item
function Delete(TenantMailSetupKey) {
    Swal.fire({
        title: 'Do you want to Delete?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {

        if (result.isConfirmed) {
            $.getJSON("/MailSetup/DeleteMailSetup",
                {
                    id: TenantMailSetupKey
                },
                function (data) {
                    //console.log(data)
                    if (data.msg = "success") {
                        swal.fire("Done", "Record Delete SuccessFully !!", "success");
                        setTimeout(() => {
                            window.location.reload();
                        }, 1000);
                    }

                    else {
                        swal.fire("Oppss!!!", "Please Contact Admin", "error");
                    }
                    //window.location.reload();
                });
        } else if (result.isDenied) {
            Swal.fire('Welcome ', '', 'info')
        }
    })
}

