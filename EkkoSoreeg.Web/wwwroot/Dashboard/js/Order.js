var dtble;
$(document).ready(function () {
    loaddata();
});

function loaddata() {
    dtble = $("#mytable").DataTable({
        "filter": true,
        "ajax": {
            "url": "/Admin/Order/GetData"
        },
        "type": "GET",
        "datatype": "json",
        "columns": [
            { "data": "id" },
            {
                "data": null,
                "render": function (data, type, row) {
                    return row.firstName + ' ' + row.lastName;
                },
                "title": "name"
            },
            { "data": "phoneNumber" },
            { "data": "orderStatus" },
            { "data": "totalPrice" },
            { "data": "applicationUser.email" },
            {
                "data": "id",
                "render": function (data) {
                    return `<a href="/Admin/Order/Details?orderid=${data}" class="btn btn-warning">Details</a>`;
                },
                "orderable": false
            }
        ]
    });
}

function DeleteItem(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: "DELETE",
                success: function (data) {
                    if (data.success) {
                        dtble.ajax.reload();
                        toastr.success(data.message);
                    } else {
                        toastr.error(data.message);
                    }
                }
            });
            Swal.fire({
                title: "Deleted!",
                text: "Your Order has been deleted.",
                icon: "success"
            });
        }
    });
}
