
let tablaPreguntas;
let idEditar = 0;
//let idCuest = 0;

$(function () {
    cargarCuestionarios();
});

function cargarCuestionarios() {

    // 1. Feedback inmediato al usuario
    $("#cboCuestionario").html('<option value="">Cargando...</option>');

    $.ajax({
        url: _urlLista, // Tu variable global Razor
        type: "GET",
        dataType: "json",
        // contentType: ... // En GET no es necesario enviar contentType, puedes quitarlo
        success: function (response) {

            // Limpiamos el "Cargando..."
            $("#cboCuestionario").empty();

            if (response.wasSuccess) {
                const lista = response.result;

                if (lista != null && lista.length > 0) {

                    // Opción por defecto
                    let opciones = '<option value="">Seleccione un Cuestionario</option>';

                    // Construimos el string gigante en memoria (mucho más rápido)
                    $.each(lista, function (i, row) {
                        opciones += `<option value="${row.id}">${row.titulo}</option>`;
                    });

                    // 2. Tocamos el DOM una sola vez al final
                    $("#cboCuestionario").html(opciones);

                } else {
                    $("#cboCuestionario").html('<option value="">No se encontraron registros</option>');
                }
            } else {
                // Mensaje de error controlado que viene del servidor
                $("#cboCuestionario").html(`<option value="">${response.message}</option>`);
            }
        },
        error: function () {
            $("#cboCuestionario").html('<option value="">Error al cargar datos</option>');
        }
    });
}

$("#cboCuestionario").on("change", function () {
    const id = $(this).val();

    if (id) {
        detalleLista(id);
    } else {
        // Si selecciona el "vacío", limpiamos la tabla visualmente
        if ($.fn.DataTable.isDataTable("#tbPreguntas")) {
            $("#tbPreguntas").DataTable().clear().draw();
            // Opcional: destruir totalmente si prefieres que desaparezca
            // $("#tbPreguntas").DataTable().destroy();
            // $('#tbPreguntas tbody').empty();
        }
    }
});

function detalleLista(cuestionarioId) {

    if ($.fn.DataTable.isDataTable("#tbPreguntas")) {
        $("#tbPreguntas").DataTable().destroy();
        $('#tbPreguntas tbody').empty();
    }

    tablaPreguntas = $('#tbPreguntas').DataTable({
        responsive: true,
        "ajax": {
            "url": _urlDetalle,
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
                render: function (data) {
                    return `<div style="white-space: normal;">${data}</div>`;
                }
            },
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
                        <button class="btn btn-danger btn-eliminar btn-sm" data-id="${id}">
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
}

$('#tbPreguntas tbody').on('click', '.btn-editar', function () {

    let fila = $(this).closest('tr');
    if (fila.hasClass('child')) {
        fila = fila.prev();
    }

    let data = tablaPreguntas.row(fila).data();
    idEditar = data.id;
    //idCuest = data.cuestionarioId;
    $("#txtPregunta").val(data.texto);
    $('#btnRegistrar').html('<i class="fas fa-pencil-alt mr-2"></i> Actualizar');

    // Opcional: Cambiamos el color (de verde 'success' a azul 'info' o naranja 'warning')
    //$('#btnRegistrar').removeClass('btn-success').addClass('btn-info');

});

$('#tbPreguntas tbody').on('click', '.btn-eliminar', function () {

    let fila = $(this).closest('tr');

    if (fila.hasClass('child')) {
        fila = fila.prev();
    }

    let data = tablaPreguntas.row(fila).data();
    // $("#mdAddCuesti").modal("show");
    swal("Mensaje", "Eliminar: " + data.texto, "success");

});

// Botón Limpiar / Nuevo  idCuest
$("#btnNuevore").on("click", function () {
    idEditar = 0;
    $("#txtPregunta").val("");
    $('#btnRegistrar').html('<i class="fas fa-check-square mr-2"></i>Registrar');
});

$('#btnRegistrar').on('click', function () {
    // Bloquear botón para evitar doble click
    let $btn = $(this);
    $btn.prop('disabled', true).html('<i class="fas fa-spinner fa-spin"></i> Guardando...');

    const cuestionarioId = $("#cboCuestionario").val();

    // Validaciones
    if (!cuestionarioId) {
        toastr.warning("", "Seleccione Un Cuestionario.");
        resetBtn($btn); return;
    }

    if ($("#txtPregunta").val().trim() === "") {
        toastr.warning("", "Debe ingresar una pregunta nueva.");
        $("#txtPregunta").trigger("focus");
        resetBtn($btn); return;
    }

    const objeto = {
        id: idEditar,
        texto: $("#txtPregunta").val().trim(),
        cuestionarioId: parseInt(cuestionarioId)
    }

    const urlAction = idEditar === 0 ? _urlGuardar : _urlEditar;
    const typeAction = idEditar === 0 ? "POST" : "PUT";

    $.ajax({
        url: urlAction,
        type: typeAction,
        data: JSON.stringify(objeto),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response.wasSuccess) {
                toastr.success(response.message, "Registrado");
                detalleLista(cuestionarioId);
                $("#txtPregunta").val("");
                idEditar = 0;

            } else {
                swal("Error", response.message, "error");
            }
        },
        error: function () {
            swal("Error", "Error de comunicación con el servidor.", "error");
        },
        complete: function () {
            resetBtn($btn);
        }
    });
});

function resetBtn($btn) {
    $btn.prop('disabled', false).html('<i class="fas fa-check-square mr-2"></i>Registrar');
}

// fin del archivo