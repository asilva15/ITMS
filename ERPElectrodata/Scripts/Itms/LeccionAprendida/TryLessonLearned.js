/*################################################################## GCR161203 #######################################################################################*/
$(document).ready(function () {
    
    $("#pagerLeccionesAprendidas").empty();
    $("#GridLeccionesAprendidas").empty();

    if ($("#GridLeccionesAprendidas").data("kendoListView")) {
        $("#pagerLeccionesAprendidas").data("kendoPager").destroy();
        $("#GridLeccionesAprendidas").data("kendoListView").destroy();
    }

    

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

    var FechaCreacionInicio = $("#dtFechaCreacionInicio").kendoDateTimePicker().data("kendoDateTimePicker");
    var FechaCreacionFin = $("#dtFechaCreacionFin").kendoDateTimePicker().data("kendoDateTimePicker");

    function limpiarCategorias() {
        $("#Nvel2").data("kendoComboBox").value("");
        $("#Nivel3").data("kendoComboBox").value("");
        $("#Nivel4").data("kendoComboBox").value("");
    }


});

$("#btnBuscarLeccionesAprendidas").click(function () {
    listarLeccionesAprendidas();
});

function listarLeccionesAprendidas() {

    
    var leccionAprendidas = {
        Nivel1: $("#Nivel1").val(),
        Nvel2: $("#Nvel2").val(),
        Nivel3: $("#Nivel3").val(),
        Nivel4: $("#Nivel4").val(),
        IdTema: $("#cboBusquedatema").val(),
        NroRevisiones: $("#txtusquedaNroRevisiones").val(),
        EstadoAprobacion: $("#cboEstadoLeccionAprendida").val(),
        Puntuacion: $("#idCalificacion").val(),
        FechaCreacionInicio: $("#dtFechaCreacionInicio").val(),
        FechaCreacionFin: $("#dtFechaCreacionFin").val(),
        TipoTicket: $("#cboBusquedaTipoTicket").val(),
    };
    var param = {
        objLessonLearned: leccionAprendidas,
        palabraClave: $("#txtBusquedapalabraClave").val(),
    };
    console.log(param);
    $("#pagerLeccionesAprendidas").empty();
    $("#GridLeccionesAprendidas").empty();

    if ($("#GridLeccionesAprendidas").data("kendoListView")) {
        $("#pagerLeccionesAprendidas").data("kendoPager").destroy();
        $("#GridLeccionesAprendidas").data("kendoListView").destroy();
    }

    $.ajax({
        contentType: 'application/json; charset=utf-8',
        url: "/KnowledgeManagement/BuscarLeccionesAprendidas",
        data: JSON.stringify(param),
        datatype: "json",
        type: 'POST',
        cache: false,
        beforeSend: function () {
            swal({ title: "Obteniendo resultados de su búsqueda, por favor espere.", text: "Bandeja de Lecciones Aprendidas", showConfirmButton: false, imageUrl: "../../images/search_abs.gif" })
        },
        success: function (dataSource) {
            
            if (dataSource != undefined && dataSource.length > 0) {
                $("#pagerLeccionesAprendidas").empty();
                $("#GridLeccionesAprendidas").empty();

                if ($("#GridLeccionesAprendidas").data("kendoListView")) {
                    $("#pagerLeccionesAprendidas").data("kendoPager").destroy();
                    $("#GridLeccionesAprendidas").data("kendoListView").destroy();
                }

                var dataSource = new kendo.data.DataSource({
                    data: dataSource,
                    pageSize: 15
                }); 

                $("#pagerLeccionesAprendidas").kendoPager({
                    dataSource: dataSource,
                    pageSizes: [15, 30, 65],
                    refresh: true
                });

                $("#GridLeccionesAprendidas").kendoListView({
                    dataSource: dataSource,
                    template: kendo.template($("#tmplLeccionAprendida").html())
                });
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
    $("#pagerLeccionesAprendidas").empty();
    $("#GridLeccionesAprendidas").empty();
    swal("¡No se encontraron resultados para     su búsqueda!", "Bandeja de Lecciones Aprendidas", "info");
}

function mouseOverBandeja(id) {
    $("#id" + id).css("background-color", "#ffffff");
    $("#idType" + id).css("color", "#000000");
    $("#idStat" + id).css("color", "#000000");
    $("#idUR" + id).css("color", "#000000");
    $("#idUA" + id).css("color", "#000000");
    $("#idJusti" + id).css("color", "#000000");
    $("#idMY" + id).css("color", "#000000");
    $("#idAmount" + id).css("color", "#000000");
}

function mouseOutBandeja(id) {
    $("#id" + id).css("background-color", "#f5f5f5");
    $("#idType" + id).css("color", "#808080");
    $("#idStat" + id).css("color", "#808080");
    $("#idUR" + id).css("color", "#808080");
    $("#idUA" + id).css("color", "#808080");
    $("#idJusti" + id).css("color", "#808080");
    $("#idMY" + id).css("color", "#808080");
    $("#idAmount" + id).css("color", "#808080");
}

function PaymentRequest(id) {
    alert("El número de id es " + id);
}

$("#cboBusquedaTipoTicket").kendoComboBox({
    autoBind: false,
    index: -1,
    dataTextField: "NAM_TYPE_TICK",
    filter: "contains",
    dataValueField: "ID_TYPE_TICK",
    dataSource: {
        data: [{ "ID_TYPE_TICK": "1", "NAM_TYPE_TICK": "INCIDENT" }, { "ID_TYPE_TICK": "2", "NAM_TYPE_TICK": "REQUEST" }, { "ID_TYPE_TICK": "3", "NAM_TYPE_TICK": "PROJECT" }, { "ID_TYPE_TICK": "4", "NAM_TYPE_TICK": "EVENT" }]
    }
}).data("kendoComboBox");


var estadoAprobacion_ = $("#cboEstadoLeccionAprendida").kendoComboBox({
    autoBind: false,
    index: -1,
    dataTextField: "EstadoAprobacion",
    filter: "contains",
    dataValueField: "IdEstadoAprobacion",
    dataSource: {
        data: [{ "IdEstadoAprobacion": "P", "EstadoAprobacion": "Pendiente" }, { "IdEstadoAprobacion": "A", "EstadoAprobacion": "Aprobado" }, { "IdEstadoAprobacion": "D", "EstadoAprobacion": "Desaprobado" }]
    }
}).data("kendoComboBox");

/*##########################################################################################################################################################################################*/

