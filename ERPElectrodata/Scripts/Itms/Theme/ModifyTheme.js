$(document).ready(function () {
    
    //$("#idCategoria").kendoComboBox({
    //    autoBind: false,
    //    filter: "contains",
    //    index: 2,
    //    placeholder: "Select Category...",
    //    autoBind: true,
    //    dataTextField: "NAM_CATE",
    //    dataValueField: "ID_CATE",
    //    template: '<div style="font-weight:bold;">${data.NAM_CATE_1}</div>' +
    //                '<div style="margin-left:5px;" >  ${data.NAM_CATE_2} </div>' +
    //                '<div style="margin-left:10px;">${data.NAM_CATE_3}</div>' +
    //                '<div style="margin-left:10px;">${data.NAM_CATE_4}</div>',
    //    dataSource: {
    //        serverFiltering: true,
    //        schema: {
    //            data: "Data",
    //            total: "Count"
    //        },
    //        transport: {
    //            read: "/CategoryTicket/List"
    //        }
    //    }
    //}).data("kendoComboBox");

    //$("#idCategoria").kendoComboBox({
    //    autoBind: false,
    //    dataTextField: "NAM_CATE",
    //    filter: "contains",
    //    cascadeFrom: "ID_CATE_N3",
    //    dataValueField: "ID_CATE",
    //    dataSource: {
    //        serverFiltering: true,
    //        schema: {
    //            data: "Data",
    //            total: "Count"
    //        },
    //        transport: {
    //            read: "/Administrator/ListCategory?ID_ACCO=" + @Session["ID_ACCO"] + "&var=" + Math.random()
    //        }
    //    }
    //}).data("kendoComboBox");



    /*var DateEndNuevo = $("#DateEnd").kendoDateTimePicker().data("kendoDateTimePicker");
    DateEndNuevo.value("@ViewBag.DATE");*/

    var DateEndNuevo = $("#DateEnd").kendoDateTimePicker().data("kendoDateTimePicker");
    // $("#DateEnd").value("@ViewBag.DATE");


});

$('#btnUpdateTheme').click(function (event) {
    
    var theme = {
        IdTema: $("#txtIdTema").val(),
        IdCuentaCatTema: $("#hdIdCuentaCatTema").val(),
        Nomtema: $("#txtNomTema").val().toUpperCase(),
        idCategoria: $("#idCategoria").val(),
        DateEnd: $("#DateEnd").val(),
    };

    /*Validaciones previas al registrpo de temas*/
    if (theme.Nomtema == "" || theme.Nomtema == null) {
        swal("¡Por favor Ingrese el nombre del tema!", "Nuevo Tema", "info");
        return false;
    }


    if (theme.idCategoria == "" || theme.idCategoria == null) {
        swal("¡Por favor Ingrese la categoría!", "Nuevo Tema", "info");
        return false;
    }

    if (theme.DateEnd == "" || theme.DateEnd == null) {
        swal("¡Por favor Ingrese una fecha de baja!", "Nuevo Tema", "info");
        return false;
    }

    var estado = ValidarFecha(theme.DateEnd);
    if (!estado) {
        swal("¡Por favor Ingrese una fecha mayor o igual a la actual!", "Nuevo Tema", "info");
        return false;
    }
    /*Fin*/

    swal({
        title: "¿Está seguro de registrar el Tema?",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        cancelButtonText: "Cancelar",
        confirmButtonText: "Aceptar",
        closeOnConfirm: false
    }, function () {
        var param = {
            objThemeModel: theme,
            TypeOperation: 2
        };
        var url = "/KnowledgeManagement/SaveTheme";

        $.ajax({
            contentType: 'application/json; charset=utf-8',
            url: url,
            type: "Post",
            data: JSON.stringify(param),
            datatype: "json",
            cache: false,
            beforeSend: function () {
                $("[data-dismiss=modal]").trigger({ type: "click" });
                $(".modal-backdrop").remove();
                swal({ title: "Procesando información, por favor espere.", text: "Mantenimiento del Temas", showConfirmButton: false, imageUrl: "../../images/search_abs.gif" })
            },
            success: function (listaTemas) {
                
                if (listaTemas) {
                    Crear_Success()
                    var Grilla = $("#GridTema").data("kendoGrid");
                    Grilla.dataSource.data(listaTemas["listaTemas"]);
                    Grilla.dataSource.page(1);
                } else {
                    Crear_Failed()
                }
            },
            error: function () {
                Crear_Failed()
            }
        });

    });
});


function ValidarFecha(Fecha) {
    
    var diaIngreso = 0;
    var mesingreso = 0;
    var valor = false;
    var hoy = new Date();
    var dd = hoy.getDate();
    var mm = hoy.getMonth() + 1;
    var yyyy = hoy.getFullYear();

    if (dd < 10) { dd = '0' + dd }

    if (mm < 10) { mm = '0' + mm }

    hoy = yyyy + mm + dd;

    var FechaString = Fecha.split("/");

    diaIngreso = parseInt(FechaString[1]); if (diaIngreso < 10) { diaIngreso = '0' + parseInt(FechaString[1]) }

    mesingreso = parseInt(FechaString[0]); if (parseInt(FechaString[0]) < 10) { mesingreso = '0' + parseInt(FechaString[0]) }

    var FechaFinal = FechaString[2].substring(0, 4) + mesingreso + diaIngreso;

    if (FechaFinal >= hoy) { var valor = true; }
    return valor;
}

function Crear_Success() {
    
    swal("¡Tema actualizado correctamente!", "Mantenimiento de Temas", "success");
}

function Crear_Failed() {
    swal("¡Error al intentar actualizar el tema!", "Mantenimiento de Temas", "error");
    $('#modal-NewTheme').modal('show');
}






