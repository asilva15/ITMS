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
                $("#bodySurvey").append("<p style='margin-top:10px;text-align:justify;padding:10px 0px 10px 0px;font-size:medium;'>Ticket de Referencia para encuesta " +
                    "<a href='http://itms.electrodata.com.pe/webclient/Ticket/Index?idt=" + val.id_xt + "' target='_blank'>" + val.ticket + "</a>" + "</p>");
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

function InsertQuestion(id,idst) {
    $.ajax({
        url: "../Survey/Question/" + id + "?idst=" + idst + "&_=" + Math.random(),
    }).done(function (result) {
        $("#bodySurvey").append('<form id="frmSurvey" method="post" role="form" action="../../Survey/Create/0" ></form>');

        $("#frmSurvey").append('<input type="hidden" value="' + idst + '" name="idst" id="idst" />');

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
        case 33:

            $("#frmSurvey").append(              
                '<div id="g-'+val.idqt+'"'+' class="row control-group" style="margin-top:20px;">' +
                    '<div style="border:0px solid red;" class="col-xs-12 col-sm-10 col-sm-offset-1 col-md-11 col-md-offset-1"><label class="control-label" for="">' + val.nombre + '</label></div>' +
                    '<div class="col-xs-12 col-sm-10 col-sm-offset-1 col-md-10 col-md-offset-1">' + '<textarea required="required" data-match-error="Favor Ingrese su comentario" data-error="Bby" style="resize: none;margin-top:5px;" rows=2 class="form-control" id="' + val.idqt + '" name="' + val.idqt + '"/>' + '</div>' +
                 '</div>'
                 );
        default:
    }
}