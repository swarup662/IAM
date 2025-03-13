$(document).ready(function () {
    $('#dynamicTable_CompanyWiseRoleMap').DataTable({
        "pageLength": 50
    });

});



$('#Close').click(function () {
    window.location.reload();
});














function DeleteItem_CompanyWiseRoleMap(CompanyId, UserTypeId, UserCategoryId, DepartmentId) {
    Swal.fire({
        title: 'Do you want to delete?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {

        
            $.ajax({
                url: "/Authorization/DeleteCompanyWiseRoleMap",
                type: "POST",
                dataType: "json",
                data: {
                    CompanyId: CompanyId,
                    UserTypeId: UserTypeId,
                    UserCategoryId: UserCategoryId,
                    DepartmentId: DepartmentId },
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





$('[id*="SaveCompanyWiseRoleMap"]').click(function () {
    var arrItem = [];  // Array to store each object
    var UserTypeId = $('#UserTypeId').val();
    var UserCategoryId = $('#UserCategoryId').val();

    var CompanyId = $('#CompanyId').val();
    var DepartmentId = $('#DepartmentId').val();

    // Validate UserTypeId before proceeding
    if (!UserTypeId || UserTypeId === "0") {
        Swal.fire("Warning", "Please choose a user type.", "warning");
        return; // Stop execution if UserTypeId is invalid
    }
    else if (!UserCategoryId || UserCategoryId === "0") {
        Swal.fire("Warning", "Please choose a user category.", "warning");
        return; // Stop execution if UserCategoryId is invalid
    }

    // Loop over each checked job role
    $('#JobRoleId input.jobrole-checkbox:checked').each(function () {
        var JobRoleId = $(this).val();
        var relatedDesignationId = $(this).data('hiddendesignation'); // Get the hidden designation ID for this job role

        // Check if a matching DesignationId checkbox is checked
        var designationChecked = $('#DesignationId input.designation-checkbox:checked').filter(function () {
            return $(this).val() == relatedDesignationId;
        }).length > 0;

        // Only create the object if there is a matching designation checked
        if (designationChecked) {
            var item = {
                UserTypeId: UserTypeId,
                UserCategoryId: UserCategoryId,
                CompanyId: CompanyId,
                DepartmentId: DepartmentId,
                DesignationId: relatedDesignationId,
                JobRoleId: JobRoleId
            };

            // Push the object into the array
            arrItem.push(item);
        }
    });

    // this is for Guest-User
    if (UserCategoryId == "4") {
        arrItem = [];
        var item = {
            UserTypeId: UserTypeId,
            UserCategoryId: UserCategoryId,
            CompanyId: CompanyId,
            DepartmentId: "0",
            DesignationId: "0",
            JobRoleId: "0"
        };

        // Push the object into the array
        arrItem.push(item);
    }

    if (UserCategoryId == "2") {
        arrItem = [];
        var item = {
            UserTypeId: UserTypeId,
            UserCategoryId: UserCategoryId,
            CompanyId: CompanyId,
            DepartmentId: DepartmentId,
            DesignationId: "0",
            JobRoleId: "0"
        };

        // Push the object into the array
        arrItem.push(item);
    }

    console.log("Data to be sent:", JSON.stringify(arrItem));
    if (arrItem.length > 0) {
        // Prepare data for POST request
        const data = JSON.stringify(arrItem); // Ensure arrItem is serialized to JSON

        // Send data to the server using fetch
        fetch("/Authorization/SaveCompanyWiseRoleMap", {
            method: "POST",
            headers: {
                "Content-Type": "application/json" // Specify the content type
            },
            body: data // Send the serialized data
        })
           
            .then(responseData => {
                // Handle success response

                Swal.fire("Done", "Record saved successfully!", "success").then(function () {
                    // Delay the reload until after the Swal closes
                    setTimeout(() => {
                        window.location.reload();
                    }, 100); // Adjust delay as needed
                });
            })
            .catch(error => {
                // Handle error response
                console.error("Error saving data:", error);
                Swal.fire("Error", "There was an error saving the data.", "error").then(function () {
                    // Delay the reload until after the Swal closes
                    setTimeout(() => {
                        window.location.reload();
                    }, 100); // Adjust delay as needed
                });
            });
    } else {
        Swal.fire("Warning", "Please select valid job roles and designations.", "warning");
    }

});










function EditItem_CompanyWiseRoleMap(CompanyId, UserTypeId, UserCategoryId, DepartmentId) {
    $.get("/Authorization/EditCompanyWiseRoleMap", {
        CompanyId: CompanyId,
        UserTypeId: UserTypeId,
        UserCategoryId: UserCategoryId,
        DepartmentId: DepartmentId
    }, function (response) {
          if (!response || response.length === 0) {
            console.error("No data found");
            return;
        }

        var data = JSON.parse(response);
        const designationIds = new Set(data.map(item => item.DesignationId));
        const jobRoleIds = new Set(data.map(item => item.JobRoleId));
        const designationIdsString = Array.from(designationIds).join(',');
        var d = data[0];

        // Set main fields
        loadRole(d.CompanyId, d.UserTypeId, () => {
            
        });
        $('#UserTypeId').val(d.UserTypeId);
        $('#UserCategoryId').val(d.UserCategoryId);
        $('#CompanyId').val(d.CompanyId);

        //This Check Is For guest User
        if (d.UserCategoryId === 4) {
            // Disable other dropdowns and checkboxes
            $('#DepartmentId').prop('disabled', true);
            $('#DesignationIdContainer input[type="checkbox"]').prop('disabled', true);
            $('#JobRoleContainer input[type="checkbox"]').prop('disabled', true);
        }
        if (d.UserCategoryId === 2) {
            // Disable other dropdowns and checkboxes
       
            $('#DesignationIdContainer input[type="checkbox"]').prop('disabled', true);
            $('#JobRoleContainer input[type="checkbox"]').prop('disabled', true);
            loadDepartments(d.CompanyId, d.DepartmentId, () => {
              
            });
        }
        else {
            loadDepartments(d.CompanyId, d.DepartmentId, () => {
                // After departments are loaded, load designations
                loadDesignations(d.CompanyId, d.DepartmentId, designationIds, () => {
                    // After designations are loaded, load job roles
                    loadJobRoles(d.CompanyId, d.DepartmentId, designationIdsString, jobRoleIds)
                });
            });
        }
        // Show modal and update title
        $("#CompanyWiseRoleMapModal").modal('show');
        var allChecked = $('.jobrole-checkbox').length === $('.jobrole-checkbox:checked').length;

        // Set the "Select All" checkbox based on the status of individual checkboxes
        $('#selectAllJobRoles .form-check-input').prop('checked', allChecked);
        $(".modal-title").text(function (_, text) {
            return text.replace(/\[New\]/g, "[Edit]");
        });
    });
}

// Helper function to load departments


function loadDepartments(companyId, selectedDepartmentId, callback) {
    $.ajax({
        url: '/Authorization/GetDepartment',
        type: 'GET',
        data: { CompanyId: companyId },
        success: function (data) {
            var departmentDropdown = $('#DepartmentId');
            departmentDropdown.empty().append('<option value="0">----Select Department-----</option>');

            if (data.length > 0) {
                $.each(data, function (i, department) {
                    departmentDropdown.append('<option value="' + department.DepartmentId + '">' + department.DepartmentName + '</option>');
                });
                departmentDropdown.val(selectedDepartmentId); // Set the selected department
            } else {
                departmentDropdown.append('<div class="text-muted">No departments found</div>');
            }
            callback();
        },
        error: function (xhr, status, error) {
            console.error("Error loading departments:", error);
        }
    });
}
function loadRole(companyId, selectedUserTypeId, callback) {
    $.ajax({
        url: '/Authorization/GetRole',
        type: 'GET',
        data: { CompanyId: companyId },
        success: function (data) {
            var RoleDropdown = $('#UserTypeId');
            RoleDropdown.empty().append('<option value="0">----Select Role-----</option>');

            if (data.length > 0) {
                $.each(data, function (i, UserType) {
                    RoleDropdown.append('<option value="' + UserType.UserTypeId + '">' + UserType.UserTypeName + '</option>');
                });
                RoleDropdown.val(selectedUserTypeId); // Set the selected department
            } else {
                RoleDropdown.append('<div class="text-muted">No Role found</div>');
            }
            callback();
        },
        error: function (xhr, status, error) {
            console.error("Error loading Roles:", error);
        }
    });
}

// Helper function to load designations
function loadDesignations(companyId, departmentId, selectedDesignations, callback) {
    $.ajax({
        url: '/Authorization/GetDesignation',
        type: 'GET',
        data: { CompanyId: companyId, DepartmentId: departmentId },
        success: function (data) {
            var designationDropdown = $('#DesignationId');
            designationDropdown.empty().append(`
                <div class="form-check">
                    <input type="checkbox" class="form-check-input" id="selectAllDesignations">
                    <label class="form-check-label" for="selectAllDesignations">Select All</label>
                </div>
            `);

            if (data.length > 0) {
                $.each(data, function (i, designation) {
                    designationDropdown.append(`
                        <div class="form-check">
                            <input type="checkbox" class="form-check-input designation-checkbox" id="designation-${designation.DesignationId}" 
                                   data-hiddendepartment="${designation.DepartmentId}" value="${designation.DesignationId}" 
                                   data-companyId="${designation.CompanyId}">
                            <label class="form-check-label" for="designation-${designation.DesignationId}">${designation.DesignationName}</label>
                        </div>
                    `);
                });
                // Check selected designations
                selectedDesignations.forEach(id => {
                    $('#DesignationId').find(`input.designation-checkbox[value="${id}"]`).prop('checked', true);
                });
            } else {
                designationDropdown.append('<div class="text-muted">No designations found</div>');
            }
            callback();
        },
        error: function (xhr, status, error) {
            console.error("Error loading designations:", error);
        }
    });
}

// Helper function to load job roles
function loadJobRoles(companyId, departmentId, designationIdsString, selectedJobRoles) {
    $.ajax({
        url: '/Authorization/GetJobRole',
        type: 'GET',
        data: {
            CompanyId: companyId,
            DepartmentId: departmentId,
            DesignationId: designationIdsString
        },
        success: function (data) {
            var jobRoleDropdown = $('#JobRoleId');
            jobRoleDropdown.empty().append(`
                <div class="form-check">
                    <input type="checkbox" class="form-check-input" id="selectAllJobRoles">
                    <label class="form-check-label" for="selectAllJobRoles">Select All</label>
                </div>
            `);

            if (data.length > 0) {
                $.each(data, function (i, jobRole) {
                    jobRoleDropdown.append(`
                        <div class="form-check">
                            <input type="checkbox" class="form-check-input jobrole-checkbox" id="jobrole-${jobRole.JobRoleId}" 
                                   data-hiddendesignation="${jobRole.DesignationId}" value="${jobRole.JobRoleId}">
                            <label class="form-check-label" for="jobrole-${jobRole.JobRoleId}">${jobRole.JobRoleName}</label>
                        </div>
                    `);
                });
                // Check selected job roles
                selectedJobRoles.forEach(id => {
                    $('#JobRoleId').find(`input.jobrole-checkbox[value="${id}"]`).prop('checked', true);
                });
            } else {
                jobRoleDropdown.append('<div class="text-muted">No job roles found</div>');
            }
        },
        error: function (xhr, status, error) {
            console.error("Error loading job roles:", error);
        }
    });
}



















// Function to handle common edit functionality
function commonedit(ID, callback) {
    $.get("/Authorization/EditCompanyWiseRoleMap", { ID: ID }, function (response) {
        if (response.length > 0) {
            var data = JSON.parse(response);
            var d = data[0];

            const designationIds = new Set();
            const jobRoleIds = new Set();

            // Populate sets for unique designation and job role IDs
            data.forEach(item => {
                designationIds.add(item.DesignationId);
                jobRoleIds.add(item.JobRoleId);
            });

            const designationIdsString = Array.from(designationIds).join(',');
            const jobRoleIdsString = Array.from(jobRoleIds).join(',');

            // Return data through callback
            callback(d, designationIdsString, jobRoleIdsString, designationIds, jobRoleIds);
        } else {
            console.error("No data found");
            callback(response, null, null, null, null);
        }
    });
}

// Company change event to load departments and setup checkboxes
$('#CompanyId').change(function () {
    var CompanyId = $(this).val();
    fetchDepartments(CompanyId);
    fetchRole(CompanyId);
    
        
});
$('#UserCategoryId').change(function () {
    $('#DepartmentId').prop('disabled', false);
    $('#DesignationIdContainer input[type="checkbox"]').prop('disabled', false);
    $('#JobRoleContainer input[type="checkbox"]').prop('disabled', false);

    // this is for Guest-User
    // Check if the selected value is 4
    if ($(this).val() === "4") {
        // Disable other dropdowns and checkboxes
        $('#DepartmentId').prop('disabled', true);
        $('#DesignationIdContainer input[type="checkbox"]').prop('disabled', true);
        $('#JobRoleContainer input[type="checkbox"]').prop('disabled', true);
    }

    if ($(this).val() === "2") {
        // Disable other dropdowns and checkboxes
       
        $('#DesignationIdContainer input[type="checkbox"]').prop('disabled', true);
        $('#JobRoleContainer input[type="checkbox"]').prop('disabled', true);
    }
});


// Department change event to load designations
$('#DepartmentId').change(function () {

    if ($('#UserCategoryId').val() !== "2") {
    var departmentId = $(this).val();
    fetchDesignations(departmentId);
    resetCheckboxes();

    commonEditCheck();

  
        // Disable other dropdowns and checkboxes

    }
});

// "Select All" functionality for designations
$(document).on('change', '#selectAllDesignations', function () {
    $('.designation-checkbox').prop('checked', $(this).is(':checked'));

    updateSelectedText();
    updateSelectedDesignations();
    fetchJobrole(getSelectedDesignations());

    commonEditCheckv2();
});

// Individual designation checkbox change event
$(document).on('change', '.designation-checkbox', function () {



    updateSelectedText();
    updateSelectedDesignations();
    fetchJobrole(getSelectedDesignations());
    commonEditCheckv2();


    // Check if all designation checkboxes are selected
    var allChecked = $('.designation-checkbox').length === $('.designation-checkbox:checked').length;

    // Set the "Select All" checkbox for designations based on the status of individual checkboxes
    $('#selectAllDesignations').prop('checked', allChecked);
});

// "Select All" functionality for job roles
$(document).on('change', '#selectAllJobRoles', function () {
    var isChecked = $(this).is(':checked');
    $('#JobRoleId .jobrole-checkbox').prop('checked', isChecked);

    // Populate selected job roles if "Select All" is checked, otherwise clear the array
    var selectedJobRoles = isChecked ? $('#JobRoleId .jobrole-checkbox').map(function () {
        return $(this).val();
    }).get() : [];


    // Call commonedit function with callback to handle checkbox checking logic
    commonEditCheckv2();
});

// Fetch departments based on CompanyId
function fetchDepartments(CompanyId, callback) {
    if (CompanyId) {
        $.ajax({
            url: '/Authorization/GetDepartment',
            type: 'GET',
            data: { CompanyId: CompanyId },
            success: function (data) {
                var departmentDropdown = $('#DepartmentId').empty().append('<option value="0">----Select Department-----</option>');

                data.forEach(department => {
                    departmentDropdown.append(`<option value="${department.DepartmentId}">${department.DepartmentName}</option>`);
                });

                resetCheckboxes();
                if (typeof callback === "function") callback();
            },
            error: function (xhr, status, error) {
                console.error("Error loading departments:", error);
            }
        });
    } else {
        $('#DepartmentId').empty().append('<option value="0">----Select Department-----</option>');
    }
}
function fetchRole(CompanyId, callback) {
    if (CompanyId) {
        $.ajax({
            url: '/Authorization/GetRole',
            type: 'GET',
            data: { CompanyId: CompanyId },
            success: function (data) {
                var UserTypeDropdown = $('#UserTypeId').empty().append('<option value="0">----Select Role-----</option>');

                data.forEach(UserType => {
                    UserTypeDropdown.append(`<option value="${UserType.UserTypeId}">${UserType.UserTypeName}</option>`);
                });

                resetCheckboxes();
                if (typeof callback === "function") callback();
            },
            error: function (xhr, status, error) {
                console.error("Error loading Role:", error);
            }
        });
    } else {
        $('#UserTypeId').empty().append('<option value="0">----Select Role-----</option>');
    }
}

// Fetch designations based on selected department
function fetchDesignations(departmentId, callback) {
    var CompanyId = $('#CompanyId').val();
    if (departmentId) {
        $.ajax({
            url: '/Authorization/GetDesignation',
            type: 'GET',
            data: { CompanyId: CompanyId, DepartmentId: departmentId },
            success: function (data) {
                var designationDropdown = $('#DesignationId').empty().append(`
                    <div class="form-check">
                        <input type="checkbox" class="form-check-input" id="selectAllDesignations">
                        <label class="form-check-label" for="selectAllDesignations">Select All</label>
                    </div>
                `);

                data.forEach(designation => {
                    designationDropdown.append(`
                        <div class="form-check">
                            <input type="checkbox" class="form-check-input designation-checkbox" id="designation-${designation.DesignationId}" 
                                   data-hiddendepartment="${designation.DepartmentId}" value="${designation.DesignationId}" 
                                   data-companyId="${designation.CompanyId}">
                            <label class="form-check-label" for="designation-${designation.DesignationId}">${designation.DesignationName}</label>
                        </div>
                    `);
                });

                if (typeof callback === "function") callback();
            },
            error: function (xhr, status, error) {
                console.error("Error loading designations:", error);
            }
        });
    } else {
        $('#DesignationId').empty();
    }
}

// Fetch job roles based on selected designations
function fetchJobrole(selectedDesignations, callback) {
    var DepartmentId = $('#DepartmentId').val();
    var CompanyId = $('#CompanyId').val();

    if (selectedDesignations) {
        $.ajax({
            url: '/Authorization/GetJobRole',
            type: 'GET',
            data: { CompanyId: CompanyId, DepartmentId: DepartmentId, DesignationId: selectedDesignations },
            success: function (data) {
                var jobRoleDropdown = $('#JobRoleId').empty().append(`
                    <div class="form-check">
                        <input type="checkbox" class="form-check-input" id="selectAllJobRoles">
                        <label class="form-check-label" for="selectAllJobRoles">Select All</label>
                    </div>
                `);

                data.forEach(jobRole => {
                    jobRoleDropdown.append(`
                        <div class="form-check">
                            <input type="checkbox" class="form-check-input jobrole-checkbox" id="jobrole-${jobRole.JobRoleId}" 
                                   data-hiddendesignation="${jobRole.DesignationId}" value="${jobRole.JobRoleId}">
                            <label class="form-check-label" for="jobrole-${jobRole.JobRoleId}">${jobRole.JobRoleName}</label>
                        </div>
                    `);
                });

                if (typeof callback === "function") callback();
            },
            error: function (xhr, status, error) {
                console.error("Error loading job roles:", error);
            }
        });
    } else {
        $('#JobRoleId').empty();
    }
}


// Update selected job roles array
$(document).on('change', '.jobrole-checkbox', function () {
    // Check if all job role checkboxes are selected
    var allChecked = $('.jobrole-checkbox').length === $('.jobrole-checkbox:checked').length;

    // Set the "Select All" checkbox based on the status of individual checkboxes
    $('#selectAllJobRoles').prop('checked', allChecked);
});

// Update selected designations array
function updateSelectedDesignations() {
    selectedDesignations = getSelectedDesignations().split(',');
}

// Fetch selected designations
function getSelectedDesignations() {
    return $('.designation-checkbox:checked').map(function () {
        return $(this).val();
    }).get().join(',');
}

// Update selected designation text
function updateSelectedText() {
    var selected = $('.designation-checkbox:checked').map(function () {
        return $(this).siblings('label').text().trim();
    }).get();

    var displayText = selected.length > 0 ? selected.join(', ') : '-- Select Designation --';
    $('#DesignationDropdownButton').text(displayText).attr('title', displayText);
    $('#selectAllDesignations').prop('checked', $('.designation-checkbox').length === $('.designation-checkbox:checked').length);
}

// Reset checkboxes
function resetCheckboxes() {
    $('#DesignationId input[type="checkbox"]').prop('checked', false);
    $('#JobRoleId input[type="checkbox"]').prop('checked', false);
}

// Function to perform common edit check
function commonEditCheck() {
    var UserTypeId = $('#UserTypeId').val();
    if (UserTypeId) {
        commonedit(UserTypeId, function (data, designationIdsString, jobRoleIdsString, designationIds, jobRoleIds) {
            designationIds.forEach(designationId => {
                $('#DesignationId')
                    .find(`input.designation-checkbox[value="${designationId}"]`)  // Corrected string interpolation
                    .each(function () {
                        // Check if the checkbox's data-hiddendepartment matches data.DepartmentId
                        if ($(this).data('hiddendepartment') === data.DepartmentId) {
                            $(this).prop('checked', true);  // Check if the department matches
                        } else {
                            $(this).prop('checked', false); // Uncheck if it doesn't match
                        }
                    });
            });


            jobRoleIds.forEach(jobRoleId => {
                $('#JobRoleId')
                    .find(`input.jobrole-checkbox[value="${jobRoleId}"]`)
                    .prop('checked', true);
            });
        });
    }
}

function commonEditCheckv2() {
    var UserTypeId = $('#UserTypeId').val();
    if (UserTypeId) {
        commonedit(UserTypeId, function (data, designationIdsString, jobRoleIdsString, designationIds, jobRoleIds) {
        

            jobRoleIds.forEach(jobRoleId => {
                $('#JobRoleId')
                    .find(`input.jobrole-checkbox[value="${jobRoleId}"]`)
                    .prop('checked', true);
            });
        });
    }
}
