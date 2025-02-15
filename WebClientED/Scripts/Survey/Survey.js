
$.ajax({
    url: "../Survey/Grupo?idst=" + $("#id").val() + "&_="+Math.random(),
    context: document.body
}).done(function (json) {
    readGroup(json);
});

function readGroup(result) {
    var i = 0;
    $.each(result["Data"], function (i, val) {
        if (val.work == 1) {

            $("#bodySurvey").append("<p style='margin-top:10px;text-align:justify;padding:10px 0px 10px 0px;font-size:medium;'>" + val.nombre + "</p>");
            if (i == 0) {
                if (val.cuenta == 1) {
                    $("#bodySurvey").append("<p style='margin-top:10px;text-align:justify;padding:10px 0px 10px 0px;font-size:medium;'>Por favor indica como te sientes con respecto al Servicio y Soporte brindado durante la atención de tu solicitud " +
                    "<a href='http://itms.electrodata.com.pe/webclient/Ticket/Index?idt=" + val.id_xt + "' target='_blank'>" + val.ticket + "</a>" + " , haciendo un click en uno de los íconos que aparece líneas abajo:</p>");
                }
                else {
                    $("#bodySurvey").append("<p style='margin-top:10px;text-align:justify;padding:10px 0px 10px 0px;font-size:medium;'>Ticket de Referencia para encuesta " +
                        "<a href='http://itms.electrodata.com.pe/webclient/Ticket/Index?idt=" + val.id_xt + "' target='_blank'>" + val.ticket + "</a>" + " Dar click para visualizar su atención</p>");
                }
            }
            InsertQuestion(val.id, $("#id").val());
        }
        else {
            $("#bodySurvey").html("<div style='margin:30px 0px 30px 0px;' class='alert alert-success' role='alert'>" + val.msg + "</div>");
            return false;
        }
        i = i + 1;
    });
}

function InsertQuestion(id, idst) {
    $.ajax({
        url: "../Survey/Question/" + id + "?idst=" + idst + "&_=" + Math.random(),
    }).done(function (result) {
        $("#bodySurvey").append('<form id="frmSurvey" method="post" role="form" action="../Survey/Create/0" ></form>');

        $("#frmSurvey").append('<input type="hidden" value="' + idst + '" name="idst" id="idst" />' +
            '<input type="hidden" name="puntos" id="puntos" />');

        $.each(result["Data"], function (i, val) {
            constructor(val);
        });

        $("#frmSurvey").append('<div class="row" style="margin-top:20px;">' +
             '<div class="col-xs-12 col-sm-11 col-md-11 col-lg-11">' +
                '<input class="btn btn-default" style="float:right;" id="btnSubmit" type="submit" value="Enviar">' +
             '</div>' +
            '</div>');

        $('#btnSubmit').click(function(e) {
            var isValid = true;
            $('textarea').each(function () {
                if ($.trim($(this).val()) == '') {
                    isValid = false;
                        $("#g-" + this.id).addClass('has-error');
                    }
                else {
                        $("#g-" + this.id).removeClass('has-error');
                }
            });

            $('input[type="number"]').each(function () {
                if ($("#" + this.id).val() == 0) {
                    isValid = false;
                    $("#g-" + this.id).addClass('has-error');
                }
                else {
                    $("#g-" + this.id).removeClass('has-error');
                }
            });

        if (isValid == false)
            e.preventDefault();

        }) 
    });
}

function constructor(val) {
    switch(val.Type) {
        case 1:
            $("#frmSurvey").append(
                '<div class="row"  id="g-' + val.idqt + '"' + ' style="margin-top:10px;border-bottom:0px solid #e5e5e5;">' +
                    '<div style="border-bottom:0px solid #e5e5e5;padding-top:12px;" class="col-xs-12 col-sm-7 col-sm-offset-1 col-md-7 col-md-offset-1"><label class="control-label" for="">' + val.nombre + '</label></div>' +
                    '<div  class="col-xs-6 col-sm-4 col-md-3">' + '<input required="required" type="number" pattern="^([1-5]){1,1}$" style="float:right;"  id="' + val.idqt + '" name="' + val.idqt + '"/>' + '</div>' +
                 '</div>');

            $("#" + val.idqt).rating({
                showClear: false,
                showCaption: false,
                glyphicon: false,
                size: "xs",
                min: 0,
                step: 1,
                max: 5
            });
            break;
        case 2:
            break;
        case 3:
            $("#frmSurvey").append(              
                '<div id="g-'+val.idqt+'"'+' class="row control-group" style="margin-top:20px;">' +
                    '<div style="border:0px solid red;" class="col-xs-12 col-sm-10 col-sm-offset-1 col-md-11 col-md-offset-1"><label class="control-label" for="">' + val.nombre + '</label></div>' +
                    '<div class="col-xs-12 col-sm-10 col-sm-offset-1 col-md-10 col-md-offset-1">' + '<textarea required="required" data-match-error="Favor Ingrese su comentario" data-error="Bby" style="resize: none;margin-top:5px;" rows=2 class="form-control" id="' + val.idqt + '" name="' + val.idqt + '"/>' + '</div>' +
                 '</div>'
                 );
            break;
        case 4:
            alert(4);
            $("#frmSurvey").append(
                '<div style="width:80%;margin:0 auto;height:70px;">' +
                    '<div class="col-xs-5 col-sm-4" onclick="enviarPuntaje(1);">' +
                        '<div class="rostro"' +
                            '<span style="color:#c0392b;">DESCONTENTO</span></br>' +
                            '<img src="../Images/Encuesta/icons8-triste-50.png"/><br>' +
                            '<span  id="check1" class="glyphicon glyphicon-ok" aria-hidden="true" style="color: #4CAF50; cursor: pointer; font-size:20px;"></span>' +
                        '</div>' +
                    '</div>' +
                    '<div class="col-xs-5 col-sm-4" onclick="enviarPuntaje(3);">' +
                        '<div class="rostro">' +
                            '<span style="color:#f1c40f;">NEUTRO</span></br>' +
                            '<img src="../Images/Encuesta/icons8-neutral-50.png"/><br>' +
                            '<span  id="check3" class="glyphicon glyphicon-ok" aria-hidden="true" style="color: #4CAF50; cursor: pointer; font-size:20px;"></span>' +
                        '</div>' +
                    '</div>' +
                    '<div class="col-xs-5 col-sm-4" onclick="enviarPuntaje(5);">' +
                        '<div class="rostro">' +
                            '<span style="color:#3498db;">CONTENTO</span></br>' +
                            '<img src="../Images/Encuesta/icons8-feliz-50.png"/><br>' +
                            '<span  id="check5" class="glyphicon glyphicon-ok" aria-hidden="true" style="color: #4CAF50; cursor: pointer; font-size:20px;"></span>' +
                        '</div>' +
                    '</div>' +
                '</div>'
                );
            break;
        default:
    }
}

function enviarPuntaje(puntaje) {
    $("#puntos").val(puntaje);
    for (var i = 1; i <= 5; i = i + 2) {
        document.getElementById("check" + i).style.visibility = "hidden";
    }
    document.getElementById("check" + puntaje).style.visibility = "visible";
}