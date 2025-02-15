window.open("","_top","","true");
var valor = document.getElementById("id").value;
debugger;
//
if (valor == 'OK')
{
    document.getElementById("msjRespuesta").append("Tu opinión es muy valiosa, gracias por contestar la encuesta.");
    //setTimeout(function () {
    //    $("#msjRespuesta").fadeOut(3000);
    //}, 1000);
    //setTimeout("window.close()", 5000);
    //setTimeout(function () {
    //    window.close();
    //}, 3000);
    setTimeout(window.close, 5000);
}
else // Encuesta realizada
if (valor == 'ERROR')
{
    document.getElementById("msjRespuesta").append("Gracias por su respuesta.");
    //setTimeout(function () {
    //    document.getElementById("msjRespuesta").fadeOut(3000);
    //}, 1000);
    setTimeout(window.close, 5000);
}

//function uploadDone(msg) {
//    alert(msg);
//    if (msg == "OK") {
//        $('#msjRespuesta').empty();
//        $('#msjRespuesta').show("show");
//        $("#msjRespuesta").append("<strong>¡Bien!</strong> Tu opinión es muy valiosa, gracias por contestar la encuesta.");
//        setTimeout(function() {
//            $("#msjRespuesta").fadeOut(3000);
//        },1000);
//    }
//    else {
//        $('#msjRespuesta').empty();
//        $('#msjRespuesta').show("show");
//        $("#msjRespuesta").append("<strong>¡Cuidado!</strong>Esta encuesta ya se ha sido respondida");
//        setTimeout(function () {
//            $("#msjRespuesta").fadeOut(3000);
//        }, 1000);
//    }
//}

