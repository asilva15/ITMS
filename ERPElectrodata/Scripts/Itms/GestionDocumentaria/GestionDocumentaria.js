

$(document).ready(function () {

    cargarArchivos();
    /*Sección de combos*/
    // cargarArchivos();
    /*Carga de las personas por cuenta*/
    id = $('#hdCuenta').val();
    var IdPersona = $("#ddlPropietarioBuscar").kendoComboBox({
        index: 1,
        dataTextField: "CLIE",
        dataValueField: "ID_PERS_ENTI",
        //template: '<div style="clear:both;"><div style="float:left"><img height="40" width="40" src=\"/Content/Fotos/${data.ID_FOTO}.jpg\" alt=\"#:data.ID_ENTI#\" /></div>' +
        //                  '<div style="" ><div style="height:40px ">&nbsp;${data.FIR_NAME}<br /></div></div></div>',
        filter: "contains",
        autoBind: false,
        delay: 500,
        minLength: 0,
        suggest: true,
        //dataSource: json['Data'],
        dataSource: {
            //type: "odata",
            serverFiltering: false,
            schema: {
                data: "Data",
                total: "Count"
            },
            transport: {
                read: "/AccountEntity/PersonaPorCompania?var=" + Math.random()
            }
        }
    }).data("kendoComboBox");


    /*Combo de tipo de documento*/
    //var ddlTipoDocumento = $("#ddlTipoDocumento").kendoComboBox({
    //    autoBind: false,
    //    index: -1,
    //    dataTextField: "Nombre",
    //    dataValueField: "Id",
    //    dataSource: {
    //        serverFiltering: false,
    //        schema: {
    //            data: "Data",
    //            total: "Count"
    //        },
    //        transport: {
    //            read: "/GestionDocumentaria/ObtenerTipos/?idCuenta=" + id + "&var=" + Math.random()
    //        }
    //    }
    //}).data("kendoComboBox");

    var ArrayFile = $("#ddlTipoDocumentoBuscar").kendoComboBox({
        autoBind: false,
        index: -1,
        dataTextField: "Nombre",
        dataValueField: "Id",
        /* dataBound: function (e) {
             // handle the event
             var dataItem = IdTipoDocumento.dataItem();
             //console.log(dataItem.HAVE_SUB_TYPE == true);
             if (dataItem.HAVE_SUB_TYPE == true) {
                 $("#divSubType").removeClass("hidden");
                 $("#divSpaceSubType").removeClass("hidden");
                 //console.log(dataItem.HAVE_SUB_TYPE);
                 createSubType(dataItem);
             }
         },*/
        dataSource: {
            serverFiltering: false,
            schema: {
                data: "Data",
                total: "Count"
            },
            transport: {
                read: "/GestionDocumentaria/ObtenerTipos/?idCuenta=" + id + "&var=" + Math.random()
            }
        }
    }).data("kendoComboBox");

    var estado = $("#ddlTipoArchivoBuscar").kendoComboBox({
        autoBind: false,
        index: -1,
        dataTextField: "Descripcion",
        filter: "contains",
        dataValueField: "Extension",
        dataSource: {
            data: [{ "Extension": ".doc", "Descripcion": "Documento doc" }, { "Extension": ".xls", "Descripcion": "Documento xls" }, { "Extension": ".pdf", "Descripcion": "Documento pdf" }]
        }
    }).data("kendoComboBox");

    $("#IdMarca").kendoComboBox({
        dataTextField: "NAM_MANU",
        dataValueField: "ID_MANU",
        filter: "contains",
        autoBind: false,
        delay: 500,
        minLength: 0,
        suggest: true,
        dataSource: {
            schema: {
                data: "Data",
                total: "Count"
            },
            transport: {
                read: "/Manufacturer/List?var=" + Math.random()
            }
        }
    });
    var IdMarca = $("#IdMarca").data("kendoComboBox");

    $("#ddlMarca").kendoComboBox({
        dataTextField: "NAM_MANU",
        dataValueField: "ID_MANU",
        filter: "contains",
        autoBind: false,
        delay: 500,
        minLength: 0,
        suggest: true,
        dataSource: {
            schema: {
                data: "Data",
                total: "Count"
            },
            transport: {
                read: "/Manufacturer/List?var=" + Math.random()
            }
        }
    });
    var ddlMarca = $("#ddlMarca").data("kendoComboBox");
    /*Fin Sección de combos*/
    var DateEndNuevo = $("#FechaInicioVigencia").kendoDateTimePicker({ enabled: false }).data("kendoDateTimePicker");
    var DateEndNuevo = $("#FechaFinVigencia").kendoDateTimePicker({ enabled: false }).data("kendoDateTimePicker");

});

$("#btnGuardar").click(function () {

    var objGestionDocumento = {
        idTipoDocumento: $("#ddlTipoDocumento").val(),
        IdCuenta: $('#hdCuenta').val(),
        NumeroDocumento: $("#txtNumeroDocumento").val(),
        IdPersona: $("#ddlPersona").val(),
        Descripcion: $("#txaDescripcion").val(),
        //Creado: $("#Creado").val(),
        IdMarca: $("#IdMarca").val(),
        Vigencia: $("#Vigencia").val(),
        FechaInicioVigencia: $("#FechaInicioVigencia").val(),
        FechaFinVigencia: $("#FechaFinVigencia").val(),
    }

    /*Validaciones*/
    if (objGestionDocumento.IdPersona == "" || objGestionDocumento.IdPersona == null) {
        swal("¡Por favor Ingrese la persona!", "Gestión Documentaria", "info");
        return false;
    }
    if (objGestionDocumento.NumeroDocumento == "" || objGestionDocumento.NumeroDocumento == null) {
        swal("¡Por favor Ingrese el Número  de documento!", "Gestión Documentaria", "info");
        return false;
    }

    if (objGestionDocumento.NumeroDocumento == "" || objGestionDocumento.NumeroDocumento == null) {
        swal("¡Por favor Ingrese el Número  de documento!", "Gestión Documentaria", "info");
        return false;
    }

    //if (objGestionDocumento.Creado == "" || objGestionDocumento.Creado == null) {
    //    swal("¡Por favor Ingrese una fecha!", "Gestión Documentaria", "info");
    //    return false;
    //}


    swal({
        title: "¿Está seguro de registrar la Gestión documentaria?",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        cancelButtonText: "Cancelar",
        confirmButtonText: "Aceptar",
        closeOnConfirm: false
    }, function () {

        var param = {
            objGestionDocumento:objGestionDocumento,
            keyAta: $("#KEY_ATTA").val()
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
                swal({ title: "Procesando información, por favor espere.", text: "Gestión Documentaria", showConfirmButton: false, imageUrl: "../../images/search_abs.gif" })
            },
            success: function (data) {
                
                if (data.length != 0) {
                    Crear_Success();
                    LimpiarCampos();
                    MostrarDocumentos();
                    //   var location = "http://" + location.host + "/#/GestionDocumentaria/GestionDocumentos";
                    // location.reload(true);
                } else
                    Crear_Failed();
            },
            error: function () {
                Crear_Failed()
            }
        });


    }

);


});

function MostrarDocumentos() {
        
        var objGestionDocumento = {
            TipoArchivo:null,
            IdTipoDocumento: 0,
            NombreDocumento: null,
            IdPersona: 0
        }

        var param = {
            objGestionDocumento: objGestionDocumento,
        };

        $.ajax({
            contentType: 'application/json; charset=utf-8',
            url: "/GestionDocumentaria/ListarArchivos", 
            type: "Post",
            data: JSON.stringify(param),
            datatype: "json",
            cache: false,
            beforeSend: function () {
                swal({ title: "Procesando información, por favor espere.", text: "Gestión Documentaria", showConfirmButton: false, imageUrl: "../../images/search_abs.gif" })
            },
            success: function (data) {
                if (Array.isArray(data)) {
                    generarArchivos(data);
                    swal.close();
                } else
                    Crear_Failed();
            },
            error: function () {
                Crear_Failed();
            }
        });
}


function LimpiarCampos() {

    $("#ddlPersona").data("kendoComboBox").value("");
    $("#IdMarca").data("kendoComboBox").value("");
   // $("#ddlTipoDocuemnto").data("kendoComboBox").value("");
   // var upload = $("#files").data("kendoUpload");
   // upload.clearAllFiles();
    var datetimepickerinicio = $("#FechaInicioVigencia").data("kendoDateTimePicker");
    datetimepickerinicio.destroy();
    var datetimepickerfin = $("#FechaFinVigencia").data("kendoDateTimePicker");
    datetimepickerfin.destroy();
    $("#txtNumeroDocumento").val('');
    $("#txaDescripcion").val('');
}

function Crear_Success() {
    swal("¡El documento fue generado correctamente!", "Gestión Documentaria", "success");
   // var newurl = location.host + "/#/TrayLessonsLearned";  
    //location = "http://" + location.host + "/#/GestionDocumentaria/GestionDocumentos";
}

function Crear_Failed() {
    swal("¡Error al realizar el proceso!", "Gestión Documentaria", "error");
}


$("#btnBuscar").click(function () {

    cargarArchivos();
});



function cargarArchivos() {
    
    var objGestionDocumento = {
        IdMarca: $("#ddlMarca").val(),
        IdTipoDocumento: $("#ddlTipoDocumentoBuscar").val(),
        NombreDocumento: $("#txtNombreDocumentoBuscar").val(), 
        IdPersona: $("#ddlPropietarioBuscar").val()
    }

    var param = {
        objGestionDocumento: objGestionDocumento,
    };

    $.ajax({
        contentType: 'application/json; charset=utf-8',
        url: "/GestionDocumentaria/ListarArchivos",
        type: "Post",
        data: JSON.stringify(param),
        datatype: "json",
        cache: false,
        beforeSend: function () {
            swal({ title: "Procesando información, por favor espere.", text: "Gestión Documentaria", showConfirmButton: false, imageUrl: "../../images/search_abs.gif" })
        },
        success: function (data) {
            
            if (Array.isArray(data)) {
                generarArchivos(data);
                swal.close();
            } else
                Crear_Failed();
        },
        error: function () {
            Crear_Failed();
        }
    });

}

function generarArchivos(data) {
     
    var filemanager = $('.filemanager'),
        fileList = filemanager.find('.data');

    var Files = [];

    if (Array.isArray(data)) {
        data.forEach(function (d) {
            Files.push(d);
        })
    }

    fileList.empty().hide();
    
    if (Files.length){
        //f.Extension == '.pdf' ? 'jpg' : f.Extension:f.Extension == '.xlsx'?'.xls',
        $(".documentos table.tblGestionDocuemntos").html("<thead><tr><th>Usuario</th><th>Tipo Documento</th><th>Marca</th><th>Documento</th><th>Extensión</th><th>Vigencia</th><th>Fecha Inicio</th><th>Fecha Fin</th><th>Fecha Creación</th><th>Acciones</th></tr></thead>");
        $(".documentos table.tblGestionDocuemntos").append('<tbody>');
        Files.forEach(function (f) {
            
            var TipoDocumento = f.TipoDocumento,
                Documento = f.Documento,
                Usuario = f.Usuario,
                ExtensionImagen = f.Extension.substring(1).toLowerCase() == 'jpg' ? 'html' : f.Extension.substring(1).toLowerCase();// : f.Extension == '.xlsx'?'xls':f.Extension == '.docx'?'doc':'html'
                Extension     = f.Extension
                Archivo = f.Archivo,
                FechaCreacion = f.Creado,
                Marca = f.Marca,
                Vigencia = f.Vigencia,
                FechaInicioVigencia = f.FechaInicioVigencia,
                FechaFinVigencia = f.FechaFinVigencia;

             //   icon = '<span class="icon file"></span>';

           // icon = '<span class="icon file f-' + ExtensionImagen + '">' + Extension + '</span>';

            /*
            $(".filemanager ul.data").append(
                '<li class="files"><span class="usuario">' + Usuario + '</span><a href="' + Archivo + '" target="_blank" title="' + Usuario + '" class="files">' + icon + '<span class="name">' + TipoDocumento + '</span> <span class="details">' + Documento + '</span></a></li>'
                );
                */
                $(".documentos table.tblGestionDocuemntos").append(
                    '<tr><td>' + Usuario + '</td><td>' + TipoDocumento + '</td><td>' + Marca + '</td><td>' + Documento + '</td><td class="icon file f-pdf">' + Extension + '</td><td>' + Vigencia + '</td><td>' + FechaInicioVigencia + '</td><td>' + FechaFinVigencia + '</td><td>' + FechaCreacion + '</td><td>' + '<a href="' + Archivo + '" target="_blank"><button  type="button" class="btn" style="background-color: rgb(240, 173, 78); border-radius: 4px; color: #fff; ">Visualizar</button></a></td></tr>'
                );
              
        })
        $(".documentos table.tblGestionDocuemntos").append('</tbody>');
       // $(".documentos table.tblGestionDocuemntos").append('<tfoot><tr class="final"><td colspan="12" class="footable-visible"><ul class="pagination pull-right"><li class="footable-page-arrow"><a data-page="first" href="#first">«</a></li><li class="footable-page-arrow"><a data-page="prev" href="#prev">‹</a></li><li class="footable-page"><a data-page="0" href="#">1</a></li><li class="footable-page active"><a data-page="1" href="#">2</a></li><li class="footable-page"><a data-page="2" href="#">3</a></li><li class="footable-page-arrow"><a data-page="next" href="#next">›</a></li><li class="footable-page-arrow"><a data-page="last" href="#last">»</a></li></ul></td></tr></tfoot>');
        fileList.show();
    }
}

