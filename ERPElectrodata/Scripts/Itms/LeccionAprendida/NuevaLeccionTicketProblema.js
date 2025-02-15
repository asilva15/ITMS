$(document).ready(function () {
 /*##########################################################################################################################################################################################*/
    $("#DescripcionProblema").kendoEditor({
        encoded: false
    });

    $("#SolucionAplicada").kendoEditor({
        encoded: false
    });

    $("#Impactonegocio").kendoEditor({
        encoded: false
    });

    $("#SolucionTemporal").kendoEditor({
        encoded: false
    });

    $("#SolucionDefinitiva").kendoEditor({
        encoded: false
    });

/*##########################################################################################################################################################################################*/
    id = $('#hdCuenta').val();
    $("#NroRevisiones").attr("readonly", "true")
    /*carga de Combos*/
    debugger;
    var Nivel1 = $("#Nivel1").kendoComboBox({
        autoBind: false,
        index: -1,
        dataTextField: "NAM_CATE",
        filter: "contains",
        dataValueField: "ID_CATE",
        dataBound: function (e) {

        },
        dataSource: {
            serverFiltering: false,
            schema: {
                data: "Data",
                total: "Count"
            },
            transport: {
                read: "/Administrator/ListCategory?ID_ACCO=" + id + "&var=" + Math.random()
            }
        }
    }).data("kendoComboBox");
    var NivelAccion1 = $("#Nvel2").kendoComboBox({}).data("kendoComboBox");
    var NivelAccion2 = $("#Nivel3").kendoComboBox({}).data("kendoComboBox");
    var NivelAccion3 = $("#Nivel4").kendoComboBox({}).data("kendoComboBox");
    var IdTema = $("#IdTema").kendoComboBox({}).data("kendoComboBox");
/*##########################################################################################################################################################################################*/
    Nivel1.bind("change", function (e) {
        if (Nivel1.dataItem()) {
            $('#Nivel1').attr('value', Nivel1.dataItem().ID_CATE);
            cargarCombosAnidados(Nivel1.dataItem().ID_CATE, id);
            limpiarCategorias();
        }
        else {
            $('#Nivel1').attr('value', 0);
        }
    });
    /*##########################################################################################################################################################################################*/
    function cargarTemas(id, idcatNivel1) {
    var IdTema = $("#IdTema").kendoComboBox({
        autoBind: false,
        index: -1,
        dataTextField: "Nomtema",
        filter: "contains",
        dataValueField: "IdTema",
        dataSource: {
            serverFiltering: false,
            schema: {
                data: "Data",
                total: "Count"
            },
            transport: {
                read: "/KnowledgeManagement/ListTemaLessonLearned?ID_ACCO=" + id + "&ID_CATE=" + idcatNivel1
            }
        }
    }).data("kendoComboBox");
}
/*##########################################################################################################################################################################################*/
function cargarCombosAnidados(idcatNivel1, id) {
    var Nvel2 = $("#Nvel2").kendoComboBox({
        autoBind: false,
        index: -1,
        dataTextField: "NAM_CATE",
        filter: "contains",
        dataValueField: "ID_CATE",
        dataSource: {
            serverFiltering: false,
            schema: {
                data: "Data",
                total: "Count"
            },
            transport: {
                read: "/Administrator/ListCategoryLessonLearned?ID_ACCO=" + id + "&ID_CATE=" + idcatNivel1 + "&var=" + Math.random()
            }
        }
    }).data("kendoComboBox");
    Nvel2.bind("change", function (e) {
        if (Nvel2.dataItem()) {
            $('#Nvel2').attr('value', Nvel2.dataItem().ID_CATE);
            cargarTemas(id, Nvel2.dataItem().ID_CATE);
        }
        else {
            $('#Nvel2').attr('value', 0);
        }
    });

    var Nivel3 = $("#Nivel3").kendoComboBox({
        autoBind: false,
        index: -1,
        dataTextField: "NAM_CATE",
        filter: "contains",
        cascadeFrom: "Nvel2",
        dataValueField: "ID_CATE",
        dataSource: {
            serverFiltering: true,
            schema: {
                data: "Data",
                total: "Count"
            },
            transport: {
                read: "/Administrator/ListCategory?ID_ACCO=" + id + "&var=" + Math.random()
            }
        }
    }).data("kendoComboBox");
    Nivel3.bind("change", function (e) {
        if (Nivel3.dataItem()) {
            $('#Nivel3').attr('value', Nivel3.dataItem().ID_CATE);
            cargarTemas(id, Nivel3.dataItem().ID_CATE);
        }
        else {
            $('#Nivel3').attr('value', 0);
        }
    });

    var Nivel4 = $("#Nivel4").kendoComboBox({
        autoBind: false,
        index: -1,
        dataTextField: "NAM_CATE",
        filter: "contains",
        cascadeFrom: "Nivel3",
        dataValueField: "ID_CATE",
        dataSource: {
            serverFiltering: true,
            schema: {
                data: "Data",
                total: "Count"
            },
            transport: {
                read: "/Administrator/ListCategory?ID_ACCO=" + id + "&var=" + Math.random()
            }
        }
    }).data("kendoComboBox");
    Nivel4.bind("change", function (e) {

        if (Nivel4.dataItem()) {
            $('#Nivel4').attr('value', Nivel4.dataItem().ID_CATE);
            cargarTemas(id, Nivel4.dataItem().ID_CATE);
        }
        else {
            $('#Nivel4').attr('value', 0);
        }
    });
    /*Fin*/
}
/*##########################################################################################################################################################################################*/
function Crear_Success() {

    swal("¡Lección aprendida creada correctamente!", "Mantenimiento de Lecciones Aprendidas", "success");
    return 
   // var newurl = location.host + "/#/TrayLessonsLearned";
    //location = "http://" + location.host + "/#/TrayLessonsLearned";
}

function Crear_Failed() {
    swal("¡Error al intentar crear la lección aprendida!", "Mantenimiento de lecciones aprendidas", "error");
}
/*##########################################################################################################################################################################################*/
function limpiarCategorias() {
    $("#Nvel2").data("kendoComboBox").value("");
    $("#Nivel3").data("kendoComboBox").value("");
    $("#Nivel4").data("kendoComboBox").value("");
    $("#IdTema").data("kendoComboBox").value("");
}
/*##########################################################################################################################################################################################*/
$("#idGuardarLeccionAprendida").click(function () {


    var objLeccionAprendida = {
        Titulo: $("#Titulo").val(),
        Nivel1: $("#Nivel1").val(),
        Nvel2: $("#Nvel2").val(),
        Nivel3: $("#Nivel3").val(),
        Nivel4: $("#Nivel4").val(),
        IdTema: $("#IdTema").val(),
        EstadoAprobacion: $("#EstadoAprobacion").val(),
        NroRevisiones: $("#NroRevisiones").val(),
        Puntuacion: $("#idCalificacion").val() == "" ? 0 : $("#idCalificacion").val(),
        DescripcionProblema: $("#DescripcionProblema").val(),
        SolucionAplicada: $("#SolucionAplicada").val(),
        Impactonegocio: $("#Impactonegocio").val(),
        SolucionTemporal: $("#SolucionTemporal").val(),
        SolucionDefinitiva: $("#SolucionDefinitiva").val(),
        ID_TICKET: $("#idTicket").val(),
        IdLeccioNAprendida: $("#idLeccionAprendidaAprobador").val(),
    }

    /*Validaciones*/
    if (objLeccionAprendida.Titulo == "" || objLeccionAprendida.Titulo == null) {
        swal("¡Por favor Ingrese el Título de la Lección aprendida!", "Nueva Lección Aprendida", "info");
        return false;
    }

    if (objLeccionAprendida.Nivel1 == "" || objLeccionAprendida.Nivel1 == null) {
        swal("¡Por favor seleccione una Unidad de Negocio!", "Nueva Lección Aprendida", "info");
        return false;
    }

    if (objLeccionAprendida.Nvel2 == "" || objLeccionAprendida.Nvel2 == null) {
        swal("¡Por favor seleccione un Macroservicio!", "Nueva Lección Aprendida", "info");
        return false;
    }

    /*
    if (objLeccionAprendida.IdTema == "" || objLeccionAprendida.IdTema == null) {
        swal("¡Por favor Ingrese el Tema!", "Nueva Lección Aprendida", "info");
        return false;
    }*/

    if (parseInt($("#idPerfilAprobador").val()) == 1) {
        if (objLeccionAprendida.EstadoAprobacion == "" || objLeccionAprendida.EstadoAprobacion == null) {
            swal("¡Por favor seleccione un estado de  aprobación!", "Nueva Lección Aprendida", "info");
            return false;
        }
        debugger;
        if (objLeccionAprendida.Puntuacion == "0" || objLeccionAprendida.Puntuacion == null) {
            swal("¡Por favor elija una calificación correcta!", "Nueva Lección Aprendida", "info");
            return false;
        }
    }

    if (objLeccionAprendida.DescripcionProblema == "" || objLeccionAprendida.DescripcionProblema == null) {
        swal("¡Por favor ingrese la descripción del problema!", "Nueva Lección Aprendida", "info");
        return false;
    }

    if (objLeccionAprendida.SolucionAplicada == "" || objLeccionAprendida.SolucionAplicada == null) {
        swal("¡Por favor ingrese la solución del problema!", "Nueva Lección Aprendida", "info");
        return false;
    }

    if (objLeccionAprendida.Impactonegocio == "" || objLeccionAprendida.Impactonegocio == null) {
        swal("¡Por favor ingrese el impacto generado!", "Nueva Lección Aprendida", "info");
        return false;
    }

    if (objLeccionAprendida.SolucionTemporal == "" || objLeccionAprendida.SolucionTemporal == null) {
        swal("¡Por favor elija la solución temporal usada!", "Nueva Lección Aprendida", "info");
        return false;
    }
    /*
    if (objLeccionAprendida.SolucionDefinitiva == "" || objLeccionAprendida.SolucionDefinitiva == null) {
        swal("¡Por favor elija una solución definitiva aplicada!", "Nueva Lección Aprendida", "info");
        return false;
    }*/
    /*Fin*/

    swal({
        title: "¿Está seguro de registrar la Lección Aprendida?",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        cancelButtonText: "Cancelar",
        confirmButtonText: "Aceptar",
        closeOnConfirm: false
    }, function () {

        var param = {
            objLeccionAprendida: objLeccionAprendida,
            TypeOperation: $("#idTypeOperacion").val(),
            KEY_ATTA: $("#KEY_ATTA").val()
        };
        var url = $("#form_create").attr('action');
        var type = $("#form_create").attr('method');
        $.ajax({
            contentType: 'application/json; charset=utf-8',
            url: url,
            type: type,
            data: JSON.stringify(param),
            datatype: "json",
            cache: false,
            beforeSend: function () {
                swal({ title: "Procesando información, por favor espere.", text: "Mantenimiento del Lecciones aprendidas", showConfirmButton: false, imageUrl: "../../images/search_abs.gif" })
            },
            success: function (listaTemas) {

                if (listaTemas != 0) {
                    Crear_Success();
                    ClosewinFormPopup();
                } else
                    Crear_Failed()
            },
            error: function () {
                Crear_Failed()
            }
        });


    }

    );


});
/*##########################################################################################################################################################################################*/
});