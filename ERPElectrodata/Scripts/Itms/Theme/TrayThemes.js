$(document).ready(function () {

    $("#cboBuscaCategoria").kendoComboBox({
        autoBind: false,
        filter: "contains",
        //index: 2,
        //placeholder: "Select Category...",
        dataTextField: "NAM_CATE",
        dataValueField: "ID_CATE",
        template: '<div style="font-weight:bold;">${data.NAM_CATE_1}</div>' +
                    '<div style="margin-left:5px;" >  ${data.NAM_CATE_2} </div>' +
                    '<div style="margin-left:10px;">${data.NAM_CATE_3}</div>' +
                    '<div style="margin-left:10px;">${data.NAM_CATE_4}</div>',
        dataSource: {
            serverFiltering: false,
            schema: {
                data: "Data",
                total: "Count"
            },
            transport: {
                read: "/CategoryTicket/List"
            }
        }
    }).data("kendoComboBox");

    id = $('#hdCuenta').val();
    /*carga de Combos*/
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
    var NivelAccion2 = $("#Nvel2").kendoComboBox({}).data("kendoComboBox");
    var NivelAccion3 = $("#Nivel3").kendoComboBox({}).data("kendoComboBox");
    var NivelAccion4 = $("#Nivel4").kendoComboBox({}).data("kendoComboBox");
    var IdTema = $("#cboBusquedatema").kendoComboBox({}).data("kendoComboBox");

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

    /*################################################################## GCR161203 #######################################################################################*/

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

    function cargarTemas(id, idcatNivel1) {
        var IdTema = $("#cboBusquedatema").kendoComboBox({
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

    function limpiarCategorias() {
        $("#Nvel2").data("kendoComboBox").value("");
        $("#Nivel3").data("kendoComboBox").value("");
        $("#Nivel4").data("kendoComboBox").value("");
    }
   
    $("#cboBuscaEstadoTema").kendoComboBox({
        autoBind: false,
        index: -1,
        dataTextField: "Nam_Vig",
        filter: "contains",
        dataValueField: "VigTema",
        dataSource: {
                data: [{ "VigTema": "1", "Nam_Vig": "Activo" }, { "VigTema": "2", "Nam_Vig": "Inactivo" }]
        }
    }).data("kendoComboBox");

    var FechaCreacionInicio = $("#dtFechaInicioCreacion").kendoDateTimePicker().data("kendoDateTimePicker");
    var FechaCreacionFin = $("#dtFechaFinCreacion").kendoDateTimePicker().data("kendoDateTimePicker");
    var FechaBajaInicio = $("#dtFechaBajaInicio").kendoDateTimePicker().data("kendoDateTimePicker");
    var FechaBajaFin = $("#dtFechaBajaFin").kendoDateTimePicker().data("kendoDateTimePicker");

});

$('#NuevoTema').click(function () {

    $("#lblTitulo").text("Módulo de Gestión del Conocimiento - Nuevo Tema");

    $("#NuevoTema").attr("data-style", "expand-right");
    $("#NuevoTema").attr("data-toggle", "modal");
    $("#NuevoTema").attr("data-target", "#miModal");

    $(".modal-dialog").removeClass("modal-sm");
    $(".modal-dialog").removeClass("modal-lg");
    $(".modal-dialog").addClass("modal-lg");

    $('#modal-content').empty();
    $('#modal-content').load('/KnowledgeManagement/_NewTheme');

});


function onclickDetalle1() {

    
}

$("#btnBuscartemas").click(function () {
    listarTemas();
});


function listarTemas() {
    
    var theme = {
        Nomtema: $("#txtBuscaNomTema").val().toUpperCase(),
        idCategoria: $("#cboBuscaCategoria").val(),
        FechaCreacionInicio: $("#dtFechaInicioCreacion").val(),
        FechaCreacionFin: $("#dtFechaFinCreacion").val(),
        Vigtema: $("#cboBuscaEstadoTema").val(),
        FechaBajaInicio: $("#dtFechaBajaInicio").val(),
        FechaBajaFin: $("#dtFechaBajaFin").val(),
    };
    var param = {
        objThemeModel: theme,
    };
    $.ajax({
        contentType: 'application/json; charset=utf-8',
        url: "/KnowledgeManagement/SearchThemesByParametres",
        data: JSON.stringify(param),
        datatype: "json",
        type: 'POST',
        cache: false,
        beforeSend: function () {
            swal({ title: "Obteniendo resultados de su búsqueda, por favor espere.", text: "Bandeja de Temas", showConfirmButton: false, imageUrl: "../../images/search_abs.gif" })
        },
        success: function (data) {
            
            if (data != undefined && data.length > 0) {
                var gridBandejaTemas = $("#GridTema").data("kendoGrid");
                gridBandejaTemas.dataSource.data(data);
                gridBandejaTemas.dataSource.page(1);
                gridBandejaTemas.dataSource.sort({ field: "IdTema", dir: "desc" });
                swal.close();
            } else {
                BusquedaFallida();
            }
        },
        error: function () {
            BusquedaFallida();
        }
    });
}



function BusquedaFallida() {
    $("#GridTema").data("kendoGrid").dataSource.data([]);
    swal("¡No se encontraron resultados para su búsqueda!", "Bandeja de Temas", "info");
}
















