

$(document).ready(function () {

    cargarArchivos();

});

function cargarArchivos() {

    var objGestionDocumento = {
        TipoArchivo: $("#ddlTipoArchivoBuscar").val(),
        IdTipoDocumento: $("#ddlTipoDocumentoBuscar").val(),
        NombreDocumento: $("#txtNombreDocumentoBuscar").val(),
        IdPersona: $("#ddlPropietarioBuscar").val()
    }

    var param = {
        objGestionDocumento: objGestionDocumento,
    };

    $.ajax({
        contentType: 'application/json; charset=utf-8',
        url: "/ProgramaLicencia/ListarProgramaLicencias",
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

    if (Files.length) {
        //f.Extension == '.pdf' ? 'jpg' : f.Extension:f.Extension == '.xlsx'?'.xls',
        $(".documentos table.tblGestionDocuemntos").html("<thead><tr><th>Usuario</th><th>Documento</th><th>Tipo Documento</th><th>Extensión</th><th>Fecha Creación</th><th>Acciones</th></tr></thead>");
        $(".documentos table.tblGestionDocuemntos").append('<tbody>');
        Files.forEach(function (f) {

            var TipoDocumento = f.TipoDocumento,
                Documento = f.Documento,
                Usuario = f.Usuario,
                ExtensionImagen = f.Extension.substring(1).toLowerCase() == 'jpg' ? 'html' : f.Extension.substring(1).toLowerCase();// : f.Extension == '.xlsx'?'xls':f.Extension == '.docx'?'doc':'html'
            Extension = f.Extension
            Archivo = f.Archivo,
            FechaCreacion = f.Creado;
            //   icon = '<span class="icon file"></span>';

            // icon = '<span class="icon file f-' + ExtensionImagen + '">' + Extension + '</span>';

            /*
            $(".filemanager ul.data").append(
                '<li class="files"><span class="usuario">' + Usuario + '</span><a href="' + Archivo + '" target="_blank" title="' + Usuario + '" class="files">' + icon + '<span class="name">' + TipoDocumento + '</span> <span class="details">' + Documento + '</span></a></li>'
                );
                */
            $(".documentos table.tblGestionDocuemntos").append(
                '<tr><td>' + Usuario + '</td><td>' + Documento + '</td><td>' + TipoDocumento + '</td><td class="icon file f-pdf">' + Extension + '</td><td>' + FechaCreacion + '</td><td>' + '<button  type="button" class="btn" style="background-color: rgb(240, 173, 78); border-radius: 4px; color: #fff; ">Visualizar</button></td></tr>'
            );

        })
        $(".documentos table.tblGestionDocuemntos").append('</tbody>');
        // $(".documentos table.tblGestionDocuemntos").append('<tfoot><tr class="final"><td colspan="12" class="footable-visible"><ul class="pagination pull-right"><li class="footable-page-arrow"><a data-page="first" href="#first">«</a></li><li class="footable-page-arrow"><a data-page="prev" href="#prev">‹</a></li><li class="footable-page"><a data-page="0" href="#">1</a></li><li class="footable-page active"><a data-page="1" href="#">2</a></li><li class="footable-page"><a data-page="2" href="#">3</a></li><li class="footable-page-arrow"><a data-page="next" href="#next">›</a></li><li class="footable-page-arrow"><a data-page="last" href="#last">»</a></li></ul></td></tr></tfoot>');
        fileList.show();
    }



}