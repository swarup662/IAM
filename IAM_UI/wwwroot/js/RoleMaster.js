$('#RefreshModel').click(function () {
    window.location.reload();
});


function EditItem(UserTypeId) {
    $.getJSON("/Authorization/FetchRoleMasterID",
        {
            id: UserTypeId
        },
        function (d) {
            //console.log(d)
            $('#UserTypeId').val(d.userTypeId);
            $('#UserTypeCode').val(d.userTypeCode);
            $('#UserTypeName').val(d.userTypeName);
            $('#Hierarchy').val(d.hierarchy);
            $('#UserTypeDescription').val(d.userTypeDescription);
            $('#LevelUpto').val(d.levelUpto);

            $('#AutoInApproval').prop('checked', d.autoInApproval === true);
            
            // Show the modal
            $("#RoleMasterModal").modal('show');
            document.getElementById("staticBackdropLabel").innerHTML = "Role Master:: [Edit]";
        });

}

function TagDelete(UserTypeId) {
    Swal.fire({
        title: 'Do you want to Delete?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        
        if (result.isConfirmed) {
            $.getJSON("/Authorization/DeleteRoleByID",
                {
                    id: UserTypeId
                },
                function (data) {
                    //console.log(data)
                    if (data.msg = "success") {
                        swal.fire("Done", "Record Delete SuccessFully !!", "success");
                        setTimeout(() => {
                            window.location.reload();
                        }, 4000); 
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
