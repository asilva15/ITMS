/*##########################################################################################################################################################################################*/
$(document).ready(function () {
    //var obtenerNivel2 = $("#Nvel2").val();
    var obtenerNivel3 = 0;
    var obtenerNivel4 = 0;
    /*Se reciben los datos al después de cerrar un ticket*/
    var param_url = !!location.href.match(/&/);
    if (param_url) {
        $.urlParam = function (name) {
            var results = new RegExp('[\?&]' + name + '=([^&#]*)').exec(window.location.href);
            if (results == null) {
                return null;
            }
            else {
                return results[1] || 0;
            }
        }
        var nivel1 = $.urlParam('nivel1');
        var nivel2 = $.urlParam('nivel2');
        var nivel3 = $.urlParam('nivel3');
        var nivel4 = $.urlParam('nivel4');
        var idTema = $.urlParam('idTema');
        var idtick = $.urlParam('idtick');
        var nameNivel1 = unescape($.urlParam('nameNivel1'));
        var nameNivel2 = unescape($.urlParam('nameNivel2'));
        var nameNivel3 = unescape($.urlParam('nameNivel3'));
        var nameNivel4 = unescape($.urlParam('nameNivel4'));
        var nameTema = $.urlParam('nameTema') == "0" ? "" : unescape($.urlParam('nameTema'));// $.urlParam('nameTema').split("%20")[0];// + " " + $.urlParam('nameTema').split("%20")[1] == "undefined" ? "" : $.urlParam('nameTema').split("%20")[1];
        $("#idTicket").val(idtick);
    }
    /*Fin*/


    id = $('#hdCuenta').val();
    $("#NroRevisiones").attr("readonly", "true")
    /*carga de Combos*/
    //var Nivel1 = $("#Nivel1").select2({
    //        placeholder: 'Seleccione...',
    //        minimumInputLength: 0,
    //        multiple: false,
    //        allowClear: true,
    //        ajax: {
    //            url: "/Administrator/ListarCategoriasxNivel/" + id + '/' + 1 + '/' + 0,
    //            dataType: 'json',
    //            quietMillis: 100,
    //            data: function (params) {
    //                return {
    //                    q: params.term,
    //                    page: params.page
    //                };
    //            },
    //            processResults: function (data, page) {
    //                return { results: data };
    //            },
    //        },
    //    });
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

    /*
        estado.bind("change", function (e) {
            if (estado.dataItem()) {
                
                $('#EstadoAprobacion').attr('value', estado.dataItem().IdEstadoAprobacion);
                limpiarPuntuacion();
            }
            else {
                $('#EstadoAprobacion').attr('value', 0);
            }
        });
        */

    /*Si los datos se reciben de la bandeja de Lecciones avanzadas y lección Aprobadores*/
    if ($("#combos").val() == 3) {
        $.get('/KnowledgeManagement/CombosEdicion', { id: $("#idLeccionAprendidaAprobador").val() }, function (data) {

            var id = data["obj"]["IdLeccioNAprendida"];
            var nivel1 = data["obj"]["Nivel1"];
            var nivel2 = data["obj"]["Nvel2"];
            var nivel3 = data["obj"]["Nivel3"];
            var nivel4 = data["obj"]["Nivel4"];
            var codTema = data["obj"]["IdTema"];
            var estadoAprobacion = data["obj"]["EstadoAprobacion"];
            var revisiones = data["obj"]["NroRevisiones"];
            var puntuacion = data["obj"]["Puntuacion"];
            var nameNivel1 = data["obj"]["nombreNivel1"];
            var nameNivel2 = data["obj"]["nombreNivel2"];
            var nameNivel3 = data["obj"]["nombreNivel3"];
            var nameNivel4 = data["obj"]["nombreNivel4"];
            var nameTema = (data["obj"]["nombreTema"]) == null ? "" : data["obj"]["nombreTema"];
            $("#idTicket").val(data["obj"]["idTicket"]);

            if (nivel1 != 0)
                Nivel1.value(nivel1);

            var n2 = $("#Nvel2").kendoComboBox({
                autoBind: false,
                index: -1,
                dataTextField: "NAM_CATE",
                filter: "contains",
                dataValueField: "ID_CATE",
                dataBound: function (e) {
                },
                dataSource: {
                    serverFiltering: false,
                    data: [{ "ID_CATE": nivel2, "NAM_CATE": nameNivel2 }]
                }
            }).data("kendoComboBox");

            if (nivel2 != 0)
                n2.value(nivel2); else $("#Nvel2").data("kendoComboBox").value("");

            var n3 = $("#Nivel3").kendoComboBox({
                autoBind: false,
                index: -1,
                dataTextField: "NAM_CATE",
                filter: "contains",
                dataValueField: "ID_CATE",
                dataSource: {
                    serverFiltering: false,
                    data: [{ "ID_CATE": nivel3, "NAM_CATE": nameNivel3 }]
                }
            }).data("kendoComboBox");

            if (nivel3 != 0)
                n3.value(nivel3); else $("#Nivel3").data("kendoComboBox").value("");

            var n4 = $("#Nivel4").kendoComboBox({
                autoBind: false,
                index: -1,
                dataTextField: "NAM_CATE",
                filter: "contains",
                dataValueField: "ID_CATE",
                dataSource: {
                    serverFiltering: false,
                    data: [{ "ID_CATE": nivel4, "NAM_CATE": nameNivel4 }]
                }
            }).data("kendoComboBox");

            if (nivel4 != 0)
                n4.value(nivel4); else $("#Nivel4").data("kendoComboBox").value("");

            var tema = $("#IdTema").kendoComboBox({
                autoBind: false,
                index: -1,
                dataTextField: "Nomtema",
                filter: "contains",
                dataValueField: "IdTema",
                dataSource: {
                    serverFiltering: false,
                    data: [{ "IdTema": codTema, "Nomtema": (nameTema) == null ? "" : nameTema }]
                }
            }).data("kendoComboBox");

            if (codTema != 0)
                tema.value(codTema); else $("#IdTema").data("kendoComboBox").value("");

            var estadoAprobacion_ = $("#EstadoAprobacion").kendoComboBox({
                autoBind: false,
                index: -1,
                dataTextField: "EstadoAprobacion",
                filter: "contains",
                dataValueField: "IdEstadoAprobacion",
                dataSource: {
                    data: [{ "IdEstadoAprobacion": "P", "EstadoAprobacion": "Pendiente" }, { "IdEstadoAprobacion": "A", "EstadoAprobacion": "Aprobado" }, { "IdEstadoAprobacion": "D", "EstadoAprobacion": "Desaprobado" }]
                }
            }).data("kendoComboBox");

            if (parseInt($("#idPerfilAprobador").val()) == 1) {
                estadoAprobacion_.value(estadoAprobacion);
                $("#NroRevisiones").val(revisiones);
                $('.stars-existing').starrr({
                    rating: puntuacion
                });
                $(".stars-count-existing").html(puntuacion);
                $("#idCalificacion").val(puntuacion);
            }

            /*Carga de los archivos adjuntos*/

            $("#divAttachFiles").kendoGrid({
                dataSource: {
                    transport: {
                        read: {
                            url: "KnowledgeManagement/LessonLearnedArchivos/" + parseInt(id),
                            type: "GET",
                            dataType: "json",
                            data: {
                                ran: Math.random()
                            }
                        }
                    },
                    schema: {
                        data: "Data",
                        total: "Count",
                        model: {
                            fields: {
                                NAM_ATTA: { type: "string" },
                                ID_ATTA: { type: "number" },
                                EXT_ATTA: { type: "string" },
                                NAM_TYPE_DOCU_ATTA: { type: "string" }
                            }
                        }
                    },
                    group: {
                        field: "NAM_TYPE_DOCU_ATTA", aggregates: [
                            { field: "NAM_ATTA", aggregate: "count" }
                            , { field: "NAM_TYPE_DOCU_ATTA", aggregate: "count" }
                        ]
                    }
                },
                sortable: true,
                filterable: true,
                pageable: false,
                height: 185,
                columns: [
                    { field: "NAM_ATTA", title: "File Name", template: "<a href='../../Adjuntos/#: NAM_ATTA #_#: ID_ATTA ##: EXT_ATTA #' target='_blank'>#: NAM_ATTA # </a>" }
                    , { field: "NAM_TYPE_DOCU_ATTA", title: "Type Attach", groupHeaderTemplate: "#= value # (#= count#)" }
                ]
            });
            /*Fin*/

            /*Log de Actividades*/
            $.getJSON("/KnowledgeManagement/LessonLearnedActivity/", { id: id }, function (data) {
                debugger
                if (parseInt(data.length) > 0) {
                    $.each(data, function (index, value) {

                        $(".actividades").append('<div class="form-row"><div class="col-md-4"><div class="position-relative form-group"><article class="media event" style="display: inline-block;"><a class="pull-left date log" style="color: #5A738E;text-decoration:none;">' +
                            '<p class="month"><font><font>' + data[index]["Mes"] + '</font></font></p>' +
                            '<p class="day"><font><font>' + data[index]["Dia"] + '</font></font></p></a></article></div></div>' +
                            '<div class="col-md-6"><div class="position-relative form-group"><a style="color: #5A738E;text-decoration:none;" class="title" ><font><font class="log">' +
                            data[index]["nombreUsuario"] + '</font></font></a>' + '<span style="display: block;"><font><font>Estado: ' + data[index]["EstadoAprobacion"] +
                            '</font></font></span>' + ' <span style="display: block;"><font><font>Puntuación: ' + data[index]["Puntuacion"] + ' estrellas </font></font></span></div></div></div>');
                    });
                } else {
                    $(".actividades").append('<article>Aún no se cuenta con log de actividades</article>');
                }
            });
            /*Fin*/
            if (parseInt($("#idPerfilAprobador").val()) == 1) {
                /*Reloj del Aprobador*/
                $.getJSON("/KnowledgeManagement/RelojAprobador/", { id: id }, function (data) {

                    var opts = {
                        lines: 12,
                        angle: 0,
                        lineWidth: 0.4,
                        pointer: {
                            length: 0.75,
                            strokeWidth: 0.042,
                            color: '#1D212A'
                        },
                        limitMax: 'false',
                        colorStart: '#1ABC9C',
                        colorStop: '#1ABC9C',
                        strokeColor: '#F0F3F3',
                        generateGradient: true
                    };
                    var target = document.getElementById('foo'),
                        gauge = new Gauge(target).setOptions(opts);

                    gauge.maxValue = 100;
                    gauge.animationSpeed = 32;
                    gauge.set(data["PorcentajeAprobacion"]);
                    gauge.setTextField(document.getElementById("gauge-text"));
                });
                /*Fin*/
            }
            /**/
            /*
            $.each(data['archivos'], function (index, value) {
                
                $(".dadjuntos").append((data['archivos'][index]))
            });
            */
        })
    } else {

        $('.stars-existing').starrr({
            rating: 0
        });
    }
    /*Fin*/

    /*Seteo de categorías*/
    Nivel1.value(nivel1);

    var n2 = $("#Nvel2").kendoComboBox({
        autoBind: false,
        index: -1,
        dataTextField: "NAM_CATE",
        filter: "contains",
        dataValueField: "ID_CATE",
        dataBound: function (e) {
        },
        dataSource: {
            serverFiltering: false,
            data: [{ "ID_CATE": nivel2, "NAM_CATE": nameNivel2 }]
        }
    }).data("kendoComboBox");
    n2.value(nivel2?nivel2:"");


    var n3 = $("#Nivel3").kendoComboBox({
        autoBind: false,
        index: -1,
        dataTextField: "NAM_CATE",
        filter: "contains",
        dataValueField: "ID_CATE",
        dataSource: {
            serverFiltering: false,
            data: [{ "ID_CATE": nivel3, "NAM_CATE": nameNivel3 }]
        }
    }).data("kendoComboBox");
    n3.value(nivel3?nivel3:"");


    var n4 = $("#Nivel4").kendoComboBox({
        autoBind: false,
        index: -1,
        dataTextField: "NAM_CATE",
        filter: "contains",
        dataValueField: "ID_CATE",
        dataSource: {
            serverFiltering: false,
            data: [{ "ID_CATE": nivel4, "NAM_CATE": nameNivel4 }]
        }
    }).data("kendoComboBox");
    n4.value(nivel4?nivel4:"");
    /*Fin de seteo de categorías*/

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

    var tema = $("#IdTema").kendoComboBox({
        autoBind: false,
        index: -1,
        dataTextField: "Nomtema",
        filter: "contains",
        dataValueField: "IdTema",
        dataSource: {
            serverFiltering: false,
            data: [{ "IdTema": idTema, "Nomtema": nameTema }]
        }
    }).data("kendoComboBox");
    tema.value(idTema);

    var estado = $("#EstadoAprobacion").kendoComboBox({
        autoBind: false,
        index: -1,
        dataTextField: "EstadoAprobacion",
        filter: "contains",
        dataValueField: "IdEstadoAprobacion",
        dataSource: {
            data: [{ "IdEstadoAprobacion": "P", "EstadoAprobacion": "Pendiente" }, { "IdEstadoAprobacion": "A", "EstadoAprobacion": "Aprobado" }, { "IdEstadoAprobacion": "D", "EstadoAprobacion": "Desaprobado" }]
        }
    }).data("kendoComboBox");





    /*Fin de la carga de combos*/

    /*carga de Text Area*/
    //$("#DescripcionProblema").kendoEditor({
    //    encoded: false
    //});

    //$("#SolucionAplicada").kendoEditor({
    //    encoded: false
    //});

    //$("#Impactonegocio").kendoEditor({
    //    encoded: false
    //});

    //$("#SolucionTemporal").kendoEditor({
    //    encoded: false
    //});

    //$("#SolucionDefinitiva").kendoEditor({
    //    encoded: false
    //});

    /*Fin*/
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

function limpiarPuntuacion() {
    $('.stars-existing').starrr({
        rating: 0
    });
    $(".stars-count-existing").html(0);
}



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
            obtenerNivel2 = $("#Nvel2").val();

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
            obtenerNivel3 = $("#Nivel3").val();
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
            obtenerNivel4 = $("#Nivel4").val();
            cargarTemas(id, Nivel4.dataItem().ID_CATE);
        }
        else {
            $('#Nivel4').attr('value', 0);
        }
    });
    /*Fin*/
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
    debugger
    if ($("#idTypeOperacion").val() == 1) {
        let n2 = $("#Nvel2").data("kendoComboBox");
        let n3 = $("#Nivel3").data("kendoComboBox");
        let n4 = $("#Nivel4").data("kendoComboBox");
        var objLeccionAprendida = {
            Titulo: $("#Titulo").val(),
            Nivel1: $("#Nivel1").val(),
            Nvel2: n2.value(),
            Nivel3: n3.value(),
            Nivel4: n4.value(),
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
            Porque2: $("#Porque2").val(),
            Porque3: $("#Porque3").val(),
            Porque4: $("#Porque4").val(),
            Porque5: $("#Porque5").val(),
        }
    } else {
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
            Porque2: $("#Porque2").val(),
            Porque3: $("#Porque3").val(),
            Porque4: $("#Porque4").val(),
            Porque5: $("#Porque5").val(),
        }
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

        if (objLeccionAprendida.Puntuacion == "0" || objLeccionAprendida.Puntuacion == null) {
            swal("¡Por favor elija una calificación correcta!", "Nueva Lección Aprendida", "info");
            return false;
        }
    }

    //if (objLeccionAprendida.DescripcionProblema == "" || objLeccionAprendida.DescripcionProblema == null) {
    //    swal("¡Por favor ingrese la descripción del problema!", "Nueva Lección Aprendida", "info");
    //    return false;
    //}

    if ((objLeccionAprendida.DescripcionProblema == "" || objLeccionAprendida.DescripcionProblema == null) ||
        (objLeccionAprendida.Porque2 == "" || objLeccionAprendida.Porque2 == null) ||
        (objLeccionAprendida.Porque3 == "" || objLeccionAprendida.Porque3 == null)) {
        swal("¡Por favor ingrese al menos 3 'porqués' en el Análisis Causa del Problema!", "Nueva Lección Aprendida", "info");
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
                swal({ title: "Procesando información, por favor espere.", text: "Mantenimiento del Lecciones aprendidas", showConfirmButton: false, imageUrl: "../../images/spinner.gif" })
            },
            success: function (listaTemas) {

                if (listaTemas != 0) {
                    Crear_Success()

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


function Crear_Success() {

    swal("¡Lección aprendida creada correctamente!", "Mantenimiento de Lecciones Aprendidas", "success");
    var newurl = location.host + "/KnowledgeManagement/TrayLessonsLearned";
    location = "http://" + location.host + "/KnowledgeManagement/TrayLessonsLearned";
}

function Crear_Failed() {
    swal("¡Error al intentar crear la lección aprendida!", "Mantenimiento de lecciones aprendidas", "error");
}

