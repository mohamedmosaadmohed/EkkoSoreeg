var dtble;
$(document).ready(function () {
    loaddata();
});

function loaddata() {
    dtble = $("#mytable").DataTable({
        "filter": true,
        "processing": true,
        "serverSide": true,
        "ajax": {
            "url": "/Admin/Product/GetData",
            "type": "GET",
            "datatype": "json",
            "data": function (d) {
                // Additional parameters to send to the server
                d.searchTerm = d.search.value; // DataTables provides this search term
                d.pageNumber = (d.start / d.length) + 1; // Calculating the page number
                d.pageSize = d.length; // Page size
            },
            "dataSrc": function (json) {
                // Parsing the data from the server response
                return json.data;
            }
        },
        "columns": [
            { "data": "name" },
            { "data": "stock" },
            { "data": "price" },
            { "data": "createDate" },
            { "data": "tbCatagory.name" },
            {
                "data": "productColors",
                "render": function (data) {
                    return data.join(", ");
                }
            },
            {
                "data": "id",
                "render": function (data) {
                    return `<a href="/Admin/Product/Update/${data}" class="btn btn-primary"><i class="bi bi-pencil-square"></i></a>`;
                },
                "orderable": false
            },
            {
                "data": "id",
                "render": function (data) {
                    return `<a onClick="DeleteItem('/Admin/Product/DeleteProduct/${data}')" class="btn btn-danger"><i class="bi bi-trash"></i></a>`;
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
                text: "Your product has been deleted.",
                icon: "success"
            });
        }
    });
}
