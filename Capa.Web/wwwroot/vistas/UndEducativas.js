
let tablaData;
let idEditar = 0;
const modal = "mdData";

$(function () {

    tablaData = $('#tbData').DataTable({
        responsive: true,
        "ajax": {
            "url": "/UndsEducativas/ListaUnidadesEd",
            "type": "GET",
            "datatype": "json",
            // AQUÍ ESTÁ EL TRUCO: dataSrc como función
            "dataSrc": function (json) {

                // 1. Validar si la operación fue exitosa
                if (!json.wasSuccess) {
                    // Si falla, puedes mostrar una alerta con el mensaje del servidor
                    //console.error(json.message);
                    //alert("Error: " + json.message);

                    // Retornamos un array vacío para que DataTables no se rompa
                    return [];
                }

                // 2. Si fue exitosa, retornamos la lista que está en 'result'
                // Si json.result viene nulo, retornamos array vacío
                return json.result || [];
            }
        },
        "columns": [
            { "data": "id", "visible": false, "searchable": false },
            { title: "Unidad Educativa", "data": "nombre" },
            { title: "Responsable", "data": "responsable" },
            { title: "Nro. Contacto", "data": "nroContacto" },
            { title: "Cantidad. Ests.", "data": "cantStr" },
            {
                title: "Acciones",
                data: "id",
                width: "100px",
                className: "text-center",
                orderable: false,
                render: function (id) {
                    return `
                        <button class="btn btn-primary btn-editar btn-sm mr-2" data-id="${id}">
                            <i class="fas fa-pencil-alt"></i>
                        </button>
                        <button class="btn btn-info btn-detalle btn-sm" data-id="${id}">
                            <i class="fas fa-eye"></i>
                        </button>
                    `;
                }
            }
        ],
        order: [[0, "desc"]],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });

});

$('#tbData tbody').on('click', '.btn-editar', function () {

    let fila = $(this).closest('tr');

    if (fila.hasClass('child')) {
        fila = fila.prev();
    }

    let data = tablaData.row(fila).data();
    console.log("Detalless:", data);

    swal("Mensaje", "Unidad para editar: " + data.nombre, "success");

});

// fin