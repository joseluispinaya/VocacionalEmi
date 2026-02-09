
let tablaData;
let tablaDataPregunta;
let idEditar = 0;
const modal = "mdData";

$(function () {

    // ESTO ARREGLA EL DESBORDE VISUAL
    $('#mdData').on('shown.bs.modal', function () {
        if ($.fn.DataTable.isDataTable('#tbPreguntas')) {
            $('#tbPreguntas').DataTable().columns.adjust().responsive.recalc();
        }
    });

    tablaData = $('#tbData').DataTable({
        responsive: true,
        "ajax": {
            "url": "/Cuestionarios/ListaCuestionarios",
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
            { title: "Titulo", "data": "titulo" },
            { title: "Descripcion", "data": "descripcion" },
            {
                title: "Acciones",
                data: "id",
                width: "150px",
                className: "text-center",
                orderable: false,
                render: function (id) {
                    return `
                        <button class="btn btn-primary btn-editar btn-sm mr-2" data-id="${id}">
                            <i class="fas fa-pencil-alt"></i>
                        </button>
                        <button class="btn btn-info btn-detalle btn-sm mr-2" data-id="${id}">
                            <i class="fas fa-eye"></i>
                        </button>
                        <button class="btn btn-success btn-eliminar btn-sm" data-id="${id}">
                            <i class="fas fa-trash-alt"></i>
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

$("#btnNuevo").on("click", function () {
    idEditar = 0;
    $("#txtTitulo").val("");
    $("#txtDescripcion").val("");

    $("#myModalCuestio").text("Registrar Cuestionario");

    $("#mdAddCuesti").modal("show");
})

$('#tbData tbody').on('click', '.btn-editar', function () {

    let fila = $(this).closest('tr');

    if (fila.hasClass('child')) {
        fila = fila.prev();
    }

    let data = tablaData.row(fila).data();
    idEditar = data.id;
    $("#txtTitulo").val(data.titulo);
    $("#txtDescripcion").val(data.descripcion);
    $("#myModalCuestio").text("Editar Cuestionario");
    $("#mdAddCuesti").modal("show");

});

$('#tbData tbody').on('click', '.btn-detalle', function () {

    let fila = $(this).closest('tr');

    if (fila.hasClass('child')) {
        fila = fila.prev();
    }

    let data = tablaData.row(fila).data();
    detalleCuestionario(data.id);
    $("#myModalLabel").text("Detalle del Cuestionario");
    $("#mdData").modal("show");
    // $(`#${modal}`).modal('show');
    //swal("Mensaje", "Unidad para editar: " + data.nombre, "success");

});

$('#tbData tbody').on('click', '.btn-eliminar', function () {

    let fila = $(this).closest('tr');

    if (fila.hasClass('child')) {
        fila = fila.prev();
    }

    let data = tablaData.row(fila).data();
    // $("#mdAddCuesti").modal("show");
    swal("Mensaje", "Eliminar: " + data.titulo, "success");

});

function detalleCuestionario(cuestionarioId) {

    if ($.fn.DataTable.isDataTable("#tbPreguntas")) {
        $("#tbPreguntas").DataTable().destroy();
        $('#tbPreguntas tbody').empty();
    }

    tablaDataPregunta = $('#tbPreguntas').DataTable({
        responsive: true,
        autoWidth: false,
        searching: false,
        lengthChange: false,
        paging: true,
        info: false,
        "ajax": {
            "url": "/Cuestionarios/DetalleCuestionario",
            "type": "GET",
            "datatype": "json",
            "data": {
                "cuestionarioId": cuestionarioId
            },
            "dataSrc": function (json) {
                if (!json.wasSuccess) {
                    return [];
                }
                return json.result || [];
            }
        },
        "columns": [
            { "data": "id", "visible": false, "searchable": false },
            {
                title: "Pregunta",
                data: "texto",
                // Opcional: Si el texto es MUY largo, puedes forzar el css aquí
                render: function (data) {
                    return `<div style="white-space: normal;">${data}</div>`;
                }
            }
        ],
        order: [[0, "desc"]],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });
}

function habilitarBoton() {
    $('#btnGuardar').prop('disabled', false);
}

$("#btnGuardar").on("click", function () {
    // Bloqueo inmediato
    $('#btnGuardar').prop('disabled', true);

    if ($("#txtTitulo").val().trim() === "") {
        toastr.warning("", "Debe Completar el Titulo");
        $("#txtTitulo").trigger("focus");
        habilitarBoton();
        return;
    }

    if ($("#txtDescripcion").val().trim() === "") {
        toastr.warning("", "Debe Agregar una Descripcion");
        $("#txtDescripcion").trigger("focus");
        habilitarBoton();
        return;
    }

    const objeto = {
        id: idEditar,           // minúscula
        titulo: $("#txtTitulo").val().trim(),
        descripcion: $("#txtDescripcion").val().trim()
    }

    $("#mdAddCuesti").find("div.modal-content").LoadingOverlay("show");

    const urlAction = idEditar === 0 ? "/Cuestionarios/Guardar" : "/Cuestionarios/Editar";
    const typeAction = idEditar === 0 ? "POST" : "PUT";

    $.ajax({
        url: urlAction,
        type: typeAction,
        data: JSON.stringify(objeto),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            $("#mdAddCuesti").find("div.modal-content").LoadingOverlay("hide");

            if (response.wasSuccess) {
                swal("Mensaje", response.message, "success");
                $("#mdAddCuesti").modal('hide');
                tablaData.ajax.reload();

                idEditar = 0;
            } else {
                swal("Error", response.message, "error");
            }
        },
        error: function () {
            $("#mdAddCuesti").find("div.modal-content").LoadingOverlay("hide");
            swal("Error", "Error de comunicación con el servidor.", "error");
        },
        complete: function () {
            habilitarBoton();
        }
    });

})

function probarDetalle(idParaPrueba) {
    // ID quemado (hardcoded) para la prueba
    //const idParaPrueba = 1; 

    $.ajax({
        url: "/Cuestionarios/DetalleCuestionario", // La ruta a tu controlador
        type: "GET",
        data: { cuestionarioId: idParaPrueba }, // jQuery convierte esto en ?cuestionarioId=1 automáticamente
        dataType: "json",
        success: function (response) {
            console.log("Respuesta del servidor:", response);

            if (response.wasSuccess) {
                //console.log("¡Éxito! Preguntas encontradas:");
                swal("Mensaje", "¡Éxito! Preguntas encontradas", "success");
                // Recorremos la lista de preguntas (response.result)
                response.result.forEach(pregunta => {
                    console.log(`- ID: ${pregunta.id}, Texto: ${pregunta.texto}`);
                });

            } else {
                console.warn("Aviso del servidor:", response.message);
            }
        },
        error: function (xhr, status, error) {
            console.error("Error de comunicación:", error);
        }
    });
}

// fin