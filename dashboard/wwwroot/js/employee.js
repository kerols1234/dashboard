var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#DT_load').DataTable({
        "ajax": {
            "url": "/Employee/GetAll/",
            "type": "GET",
            "datatype": "json"
        },
        "paging": false,
        "info": false,
        "lengthChange": false,
        searching: false,
        ordering: false,
        "columns": [
            { "data": "code"},
            { "data": "jobTitle"},
            { "data": "department.name", "width": "15%"},
            { "data": "arabicName"},
            { "data": "englishName"},
            { "data": "email" },
            { "data": "insurance"},
            {
                "data": "id",
                "render": function (data) {
                    return `<div class="text-center">
                        <a href="/Employee/Upsert?id=${data}" class='btn btn-info text-white' style='cursor:pointer;'>
                            <i class="fa fa-pencil-alt"></i>
                        </a>
                        &nbsp;
                        <a class='btn btn-danger text-white' style='cursor:pointer;'
                            onclick=Delete('/Employee/Delete?id=${data}')>
                            <i class="fa fa-times"></i>
                        </a>
                        </div>`;
                }, "width": "11%"
            }
        ],
        "language": {
            "emptyTable": "no data found"
        },
        "width": "100%"
    });
}

function Delete(url) {
    swal({
        title: "Are you sure?",
        text: "Once deleted, you will not be able to recover",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
}