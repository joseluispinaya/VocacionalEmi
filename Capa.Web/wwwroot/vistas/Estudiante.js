
let tablaData;
let idEditar = 0;
const controlador = "Estudiantes";
const modal = "mdData";

$(function () {
    obtenerUnidadesEdu();
    tablaData = $('#tbData').DataTable({
        responsive: true,
        "ajax": {
            "url": "/Estudiantes/ListaEstudia",
            "type": "GET",
            "datatype": "json",
            "dataSrc": function (json) {

                if (!json.wasSuccess) {
                    // Retornamos un array vacío para que DataTables no se rompa
                    return [];
                }

                return json.result || [];
            }
        },
        "columns": [
            { "data": "id", "visible": false, "searchable": false },
            {
                title: "Imagen",
                "data": "photo",
                "orderable": false,
                "searchable": false,
                render: function (data) {
                    // Es buena práctica validar si data es null por si el estudiante no tiene foto
                    // Si data es null, mostramos una imagen genérica o vacío
                    if (!data) return '<span class="badge badge-secondary">Sin foto</span>';

                    return `<img src="${data}" class="rounded mx-auto d-block" style="max-width: 40px; max-height: 40px; object-fit: cover;" />`;
                }
            },
            { title: "Estudiante", "data": "fullName" },
            { title: "Correo", "data": "correo" },
            { title: "Nro CI", "data": "nroCi" },
            { title: "Unidad Educativa", "data": "unidadEducativa" },
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

function obtenerUnidadesEdu() {
    $.ajax({
        type: "GET",
        url: "/UndsEducativas/ListaUnidadesEd", // Asegúrate que la URL sea correcta
        contentType: 'application/json; charset=utf-8',
        dataType: "json",
        success: function (response) {

            // Limpiamos el combo antes de llenarlo para evitar duplicados
            $("#cboUnidadEd").empty();

            if (response.wasSuccess) {
                const lista = response.result;

                // Opción por defecto vacía para el placeholder
                $("#cboUnidadEd").append($("<option>").val("").text(""));

                if (lista != null && lista.length > 0) {
                    lista.forEach((item) => {
                        $("#cboUnidadEd").append($("<option>").val(item.id).text(item.nombre));
                    });
                }
            } else {
                // Si el servidor dice que falló, mostramos mensaje (opcional)
                console.warn("No se pudieron cargar las unidades educativas: " + response.message);
            }

            // Inicializamos Select2 (se debe hacer fuera del if(length > 0) por si la lista viene vacía, el select2 se dibuje igual)
            $('#cboUnidadEd').select2({
                dropdownParent: $('#mdData'),
                placeholder: "Seleccionar",
                allowClear: true // Permite borrar la selección con una 'x'
            });
        },
        error: function (xhr, status, error) {
            console.error("Error Ajax:", error);
            // Opcional: swal("Error", "Error al cargar listas", "error");
        }
    });
}

$('#tbData tbody').on('click', '.btn-editar', function () {

    let fila = $(this).closest('tr');

    if (fila.hasClass('child')) {
        fila = fila.prev();
    }

    let data = tablaData.row(fila).data();
    idEditar = data.id;
    $("#txtnombres").val(data.nombres);
    $("#cboUnidadEd").val(data.unidadEducativaId.toString()).trigger("change");
    $("#txtapellidos").val(data.apellidos);
    $("#txtNroci").val(data.nroCi);
    $("#txtCorreo").val(data.correo);
    // $("#imgEstu").attr("src", data.photo ? data.photo : "/images/sinimagen.png");
    $("#imgEstu").attr("src", data.photo || "/images/sinimagen.png");
    $("#txtImagen").val("");
    $(".custom-file-label").text('Ningún archivo seleccionado');

    $("#myModalLabel").text("Editar Estudiante");

    $(`#${modal}`).modal('show');

});

function esImagen(file) {
    return file && file.type.startsWith("image/");
}

function mostrarImagenSeleccionadaP(input) {
    let file = input.files[0];
    let reader = new FileReader();

    // Si NO se seleccionó archivo (ej: presionaron "Cancelar")
    if (!file) {
        $('#imgEstu').attr('src', "/images/sinimagen.png");
        $(input).next('.custom-file-label').text('Ningún archivo seleccionado');
        return;
    }

    // Validación: si no es imagen, mostramos error
    if (!esImagen(file)) {
        swal("Error", "El archivo seleccionado no es una imagen válida.", "error");
        $('#imgEstu').attr('src', "/images/sinimagen.png");
        $(input).next('.custom-file-label').text('Ningún archivo seleccionado');
        input.value = ""; // Limpia el input de archivo
        return;
    }

    // Si todo es válido → mostrar vista previa
    reader.onload = (e) => $('#imgEstu').attr('src', e.target.result);
    reader.readAsDataURL(file);

    // Mostrar nombre del archivo
    $(input).next('.custom-file-label').text(file.name);
}

$("#txtImagen").on("change", function () {
    mostrarImagenSeleccionadaP(this);
});

$("#btnNuevo").on("click", function () {
    idEditar = 0;
    $("#txtnombres").val("");
    $("#txtapellidos").val("");
    $("#cboUnidadEd").val("").trigger('change');
    $("#txtNroci").val("");
    $("#txtCorreo").val("");
    $("#imgEstu").attr("src", "/images/sinimagen.png");
    $("#txtImagen").val("");
    $(".custom-file-label").text('Ningún archivo seleccionado');

    $("#myModalLabel").text("Registrar Estudiante");

    $(`#${modal}`).modal('show');
})

function habilitarBoton() {
    $('#btnGuardar').prop('disabled', false);
}

$("#btnGuardar").on("click", function () {

    $('#btnGuardar').prop('disabled', true);

    // 2. Validación (Acotada al modal para evitar conflictos)
    const inputs = $(`#${modal} input.model`).serializeArray();
    const inputs_sin_valor = inputs.filter(item => item.value.trim() === "");

    if (inputs_sin_valor.length > 0) {
        const mensaje = `Debe completar el campo: "${inputs_sin_valor[0].name}"`;
        toastr.warning("", mensaje);
        $(`input[name="${inputs_sin_valor[0].name}"]`).trigger("focus");
        //$(`input[name="${inputs_sin_valor[0].name}"]`).focus();
        habilitarBoton();
        return;
    }

    if ($("#cboUnidadEd").val() === "") {
        toastr.warning("", "Debe Seleccionar una unidad educativa");
        habilitarBoton();
        return;
    }

    const formData = new FormData();
    formData.append("Id", idEditar); // Importante: Si es 0 es nuevo, si tiene valor es edición
    formData.append("NroCi", $("#txtNroci").val().trim());
    formData.append("Nombres", $("#txtnombres").val().trim());
    formData.append("Apellidos", $("#txtapellidos").val().trim());
    formData.append("Correo", $("#txtCorreo").val().trim());
    formData.append("UnidadEducativaId", $("#cboUnidadEd").val());

    // Solo agregamos la foto si el usuario seleccionó una nueva
    const fileInput = document.getElementById('txtImagen');
    if (fileInput.files.length > 0) {
        formData.append("PhotoFile", fileInput.files[0]);
    }

    $(`#${modal}`).find("div.modal-content").LoadingOverlay("show");

    // LÓGICA DE URL:
    // Si idEditar es 0 -> Vamos a Guardar
    // Si idEditar es > 0 -> Vamos a Editar
    const urlAction = idEditar === 0 ? "/Estudiantes/Guardar" : "/Estudiantes/Editar";

    // RECOMENDACIÓN: Usa POST siempre cuando hay archivos involucrados para evitar problemas de binding en .NET
    const typeAction = "POST"; 
    
    $.ajax({
        url: urlAction,
        type: typeAction,
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {

            $(`#${modal}`).find("div.modal-content").LoadingOverlay("hide");

            if (response.wasSuccess) {

                swal("Mensaje", response.message, "success");

                $(`#${modal}`).modal('hide');
                tablaData.ajax.reload();
                idEditar = 0;
                //tablaData.ajax.reload(null, true);

            } else {
                swal("Error", response.message, "error");
            }
        },
        error: function () {

            $(`#${modal}`).find("div.modal-content").LoadingOverlay("hide");
            swal("Error", "Error de comunicación con el servidor.", "error");
        },
        complete: function () {
            // ¡Excelente uso del complete! Esto reactiva el botón pase lo que pase.
            habilitarBoton();
        }
    });
});