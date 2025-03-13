
$(document).ready(function () {
    $('#dynamicTable_RoleWiseApplication').DataTable({
        "pageLength": 10
    });

});


$('#Close').click(function () {
    window.location.reload();
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





//    var abc = '@TempData["MSG"]';
//    if (abc == "success") {
//        swal.fire("Done", "Record Save SuccessFully !!", "success");

//    }
//    else if (abc == "Fail") {
//        swal.fire("Oppss!!!", "Please Contact Admin", "error");
//    }


//});






function DeleteItem_RoleWiseApplication(UserTypeId, CompanyId, ApplicationId) {
    Swal.fire({
        title: 'Do you want to delete?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {

            debugger;
            $.ajax({
                url: "/Authorization/DeleteRoleWiseApplication",
                type: "POST",
                dataType: "json",
                data: {
                    UserTypeId: UserTypeId,
                    CompanyId: CompanyId,
                    ApplicationId: ApplicationId },
                success: function (data) {
                    console.log("AJAX request successful.");
                    console.log(data);
                    if (data > 0) {
                        Swal.fire("Done", "Record Deleted Successfully!", "success").then(() => {
                            // Delay the reload to allow Swal to be dismissed
                            setTimeout(() => {
                                window.location.reload();
                            }, 100); // Delay of 1500ms (1.5 seconds)
                        });
                    } else {
                        Swal.fire("Oops!!!", "Please Contact Admin", "error").then(() => {
                            // Delay the reload to allow Swal to be dismissed
                            setTimeout(() => {
                                window.location.reload();
                            }, 100); // Delay of 1500ms (1.5 seconds)
                        });
                    }
                },
                error: function (xhr, status, error) {

                    Swal.fire("Error", "An error occurred. Please try again.", "error");
                }
            });
        } else if (result.dismiss === Swal.DismissReason.cancel) {
            Swal.fire('Cancelled', 'Your record is safe :)', 'info');
        }
    });
}




$(document).on('change', '#application_ID', function () {
    var usertypeid = $('#UserTypeId').val() || "0";
    var application = $('#application_ID').val() || "0";
    var CompanyId = $('#CompanyId').val() || "0";

    console.log(usertypeid);
    console.log(application);

    // Send a GET request
    $.get("/Authorization/GetModuleAccess",
        {
            CompanyId: CompanyId,
            application_ID: application,
            UserTypeId: usertypeid
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


$(document).on('change', '#UserTypeId', function () {
    var usertypeid = $('#UserTypeId').val() || "0";
    var application = $('#application_ID').val() || "0";
    var CompanyId = $('#CompanyId').val() || "0";

    console.log(usertypeid);
    console.log(application);

    // Send a GET request
    $.get("/Authorization/GetModuleAccess",
        {
            CompanyId: CompanyId,
            application_ID: application,
            UserTypeId: usertypeid
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


$(document).on('change', '#CompanyId', function () {
    var usertypeid = $('#UserTypeId').val() || "0";
    var application = $('#application_ID').val() || "0";
    var CompanyId = $('#CompanyId').val() || "0";

    console.log(usertypeid);
    console.log(application);

    // Send a GET request
    $.get("/Authorization/GetModuleAccess",
        {
            CompanyId: CompanyId,
            application_ID: application,
            UserTypeId: usertypeid
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


function EditItem_RoleWiseApplication(UserTypeId, CompanyId, ApplicationId) {
  

    $.get("/Authorization/EditRoleWiseApplication", {
        UserTypeId: UserTypeId,
        CompanyId: CompanyId,
        ApplicationId: ApplicationId
    }, function (response) {
        console.log(response);

        if (response.length > 0) {
            var data = JSON.parse(response);
            var d = data[0];
            console.log(d);

            // Set values for UserTypeId and ApplicationId
            $('#UserTypeId').val(d.UserTypeID);
            $('#CompanyId').val(d.CompanyId);
            $('#application_ID').val(d.ApplicationId);

            // Populate the table's HTML
            $("#ModuleDiv").html(d.Module);

            // Ensure handleCheckboxes is called after the table is populated
            handleCheckboxes();

            // Show the modal
            $("#RoleWiseApplicationModal").modal('show');

            // Update modal title, replacing "[New]" with "[Edit]"
            $(".modal-title").text(function (_, text) {
                return text.replace(/\[New\]/g, "[Edit]");
            });

        } else {
            console.error("No data found");
        }
    });


}

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





$('[id*="SaveRoleWiseApplication"]').click(function () {
    var arrItem = [];

    var UserTypeId = $('#UserTypeId').val();
    var applicationID = $('#application_ID').val();
    var CompanyId = $('#CompanyId').val();
    var arr = []; // Initialize an empty array to store checked checkbox IDs

    // Validate UserTypeId before proceeding
    if (UserTypeId === "0" || UserTypeId === null || UserTypeId === undefined || UserTypeId === "") {
        Swal.fire("Warning", "Please choose a user type.", "warning");
        return; // Stop execution if UserTypeId is invalid
    }
    if (CompanyId === "0" || CompanyId === null || CompanyId === undefined || CompanyId === "") {
        Swal.fire("Warning", "Please choose a Company.", "warning");
        return; // Stop execution if UserTypeId is invalid
    }
    // Loop to get checked checkbox IDs
    $('#ModuleTable').find('input[type="checkbox"]:checked').each(function () {
        var id = $(this).attr('id');
        var split = id.split('_');
        if (split[0] === "1") {
            var ids = '2,3,4,5,6';
            id = ids + "_" + split[1];
        }
        arr.push(id);
    });

    console.log(arr);

    if (arr.length != 0) {
        commaseparatedIds = arrItem.toString();
        $.ajax({
            url: "/Authorization/SaveRoleWiseApplication",
            type: "POST",
            data: {
                CompanyId: CompanyId,
                UserTypeId: UserTypeId,
                ApplicationID: applicationID,
                arrModule: arr
            },
            success: function (response) {
                Swal.fire("Done", "Record Save Successfully!", "success");
            }
        });
    }
});





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