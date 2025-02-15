function recentrarForm() {
    var top = 0;
    altpan = $(window).height(); // devuelve la altura del viewport del navegador
    altbar = $(window).scrollTop(); // devuelve la posición vertical actual de la barra de scroll
    altobkg = $(document).height(); // devuelve la altura del documento HTML
    $("#bkgF").height(altobkg);
    top = altbar + (altpan / 2);
    alto = $("#dlgF").height();
    $("#dlgF").css("top", a_entero(top) + "px");
    $("#dlgF").css("margin-top", "-" + (a_entero(alto) / 2).toString() + "px");
}

function recentrarForm2() {
    var top = 0;
    altpan = $(window).height(); // devuelve la altura del viewport del navegador
    altbar = $(window).scrollTop(); // devuelve la posición vertical actual de la barra de scroll
    altobkg = $(document).height(); // devuelve la altura del documento HTML
    $("#bkgF2").height(altobkg);
    top = altbar + (altpan / 2);
    alto = $("#dlgF2").height();
    $("#dlgF2").css("top", a_entero(top) + "px");
    $("#dlgF2").css("margin-top", "-" + (a_entero(alto) / 2).toString() + "px");
}

function a_entero(valor) {
    //intento convertir a entero.  
    //si era un entero no le afecta, si no lo era lo intenta convertir  
    valor = parseInt(valor);
    //comprobamos si es un valor entero  
    if (isNaN(valor)) {
        //no es entero 0  
        return 0;
    } else {
        //es un valor entero  
        return valor;
    }
}

function winPopUpModal(title, contHtml, ancho, alto) {
    
    var altobkg = 0;
    var top = 0;
    altpan = $(window).height(); // devuelve la altura del viewport del navegador
    altobkg = $(document).height(); // devuelve la altura del documento HTML
    altbar = $(window).scrollTop(); // devuelve la posición vertical actual de la barra de scroll
    top = altbar + (altpan / 2);
    $("#divpopupmodal").prepend("<div id='bkg' class='backgroundcad' style='z-index: 999; display: visibility; height: " + altobkg + "px;'>" +
        "<div id='dlg' class='modalcad' style='z-index: 999; display: none; top: " + a_entero(top) + "px; width: " + a_entero(ancho) + "px; margin-left: -" + a_entero(ancho) / 2 + "px; height: " + a_entero(alto) + "px; margin-top: -" + a_entero(alto) / 2 + "px;'>" +
        "<div id='titulomodal' class='titlemodalcad'>" +
        "<div id='closebtnmodal' class='closebtnmodalcad' title='Cerrar'>X</div>" +
        "</div><div id='contenidomodal' class='contenidomodal'></div></div></div>");

    if (document.getElementById('bkg').style.visibility == 'hidden') {
        document.getElementById('bkg').style.visibility = '';
        $("#bkg").hide();
    }
    if (document.getElementById('dlg').style.visibility == 'hidden') {
        document.getElementById('dlg').style.visibility = '';
        $("#dlg").hide();
    }
    $("#bkg").fadeIn(300, "linear", function () {
        $("#dlg").show(200, "swing");
        $("#titulomodal").prepend(title)
        $("#contenidomodal").prepend('<div style="text-align:justify">' + contHtml + '</div>')
    });
    $("#closebtnmodal").click(function () {
        $("#dlg").hide('200', "swing", function () {
            $("#bkg").fadeOut("300");
        });
    });
}

function winPopUpModal2(title, contHtml, ancho, alto) {

    var altobkg = 0;
    var top = 0;
    altpan = $(window).height(); // devuelve la altura del viewport del navegador
    altobkg = $(document).height(); // devuelve la altura del documento HTML
    altbar = $(window).scrollTop(); // devuelve la posición vertical actual de la barra de scroll
    top = altbar + (altpan / 2);
    $("#divpopupmodal2").prepend("<div id='bkg2' class='backgroundcad' style='z-index: 999; display: visibility; height: " + altobkg + "px;'>" +
        "<div id='dlg2' class='modalcad' style='z-index: 999; display: block; top: " + a_entero(top) + "px; width: " + a_entero(ancho) + "px; margin-left: -" + a_entero(ancho) / 2 + "px; height: " + a_entero(alto) + "px; margin-top: -" + a_entero(alto) / 2 + "px;'>" +
        "<div id='titulomodal2' class='titlemodalcad'>" +
        "<div id='closebtnmodal2' class='closebtnmodalcad' title='Cerrar'>X</div>" +
        "</div><div id='contenidomodal2' class='contenidomodal'></div></div></div>");

    if (document.getElementById('bkg2').style.visibility == 'hidden') {
        document.getElementById('bkg2').style.visibility = '';
        $("#bkg2").hide();
    }

    if (document.getElementById('dlg2').style.visibility == 'hidden') {
        document.getElementById('dlg2').style.visibility = '';
        $("#dlg2").hide();
    }

    $("#bkg2").fadeIn(300, "linear", function () {
        $("#dlg2").show(200, "swing");
        $("#titulomodal2").prepend(title)
        $("#contenidomodal2").prepend('<div style="text-align:justify">' + contHtml + '</div>')
    });
    $("#closebtnmodal2").click(function () {
        $("#dlg2").hide('200', "swing", function () {
            $("#bkg2").fadeOut("300");
        });
    });
}

function winPopUpModalLoad(title, contHtml, ancho, alto) {
    
    var altobkg = 0;
    var top = 0;
    altpan = $(window).height(); // devuelve la altura del viewport del navegador
    altobkg = $(document).height(); // devuelve la altura del documento HTML
    altbar = $(window).scrollTop(); // devuelve la posición vertical actual de la barra de scroll
    top = altbar + (altpan / 2);
    $("#winPopUpModalLoad").prepend("<div id='bkgl' class='backgroundcad' style='z-index: 998; display: visibility; height: " + altobkg + "px;'>" +
        "<div id='dlgl' class='modalcad' style='z-index: 998; display: none; top: " + a_entero(top) + "px; width: " + a_entero(ancho) + "px; margin-left: -" + a_entero(ancho) / 2 + "px; height: " + a_entero(alto) + "px; margin-top: -" + a_entero(alto) / 2 + "px;'>" +
        "<div id='titulomodall' class='titlemodalcad'>" +
        "</div><div id='contenidomodall' class='contenidomodalcad'></div>" +
        "<div class='divLoading'></div>"+
        "</div></div>");

    if (document.getElementById('bkgl').style.visibility == 'hidden') {
        document.getElementById('bkgl').style.visibility = '';
        $("#bkgl").hide();
    }
    if (document.getElementById('dlgl').style.visibility == 'hidden') {
        document.getElementById('dlgl').style.visibility = '';
        $("#dlgl").hide();
    }
    $("#bkgl").fadeIn(300, "linear", function () {
        $("#dlgl").show(200, "swing");
        $("#titulomodall").prepend(title)
        $("#contenidomodall").prepend('<div style="text-align:justify">' + contHtml + '</div>')
    });
    $("#closebtnmodall").click(function () {
        $("#dlgl").hide('200', "swing", function () {
            $("#bkgl").fadeOut("300");
        });
    });
}

function winFormPopUpModal(title, contHtml, ancho, alto) {
    
    var altobkg = 0;
    var top = 0;
    altpan = $(window).height(); // devuelve la altura del viewport del navegador
    altobkg = $(document).height(); // devuelve la altura del documento HTML    
    altbar = $(window).scrollTop(); // devuelve la posición vertical actual de la barra de scroll
    top = altbar + (altpan / 2);
    $("#winFormPopUpModal").prepend("<div id='bkgF' class='backgroundcad' style='display: visibility; z-index: 996; height: " + altobkg + "px;'>" +
        "<div id='dlgF' class='modalcad' style='z-index: 997; display: none; top: " + a_entero(top) + "px; width: " + a_entero(ancho) + "px; margin-left: -" + a_entero(ancho) / 2 + "px; height: " + a_entero(alto) + "px; margin-top: -" + a_entero(alto) / 2 + "px;'>" +
        "<div id='titulomodalF' class='titlemodalcad'>" +
        "<div id='closebtnmodalF' class='closebtnmodalcad' title='Cerrar'>X</div>" +
        "</div><div id='contenidomodalF' class='contenidomodalFormcad'></div></div></div>");

    if (document.getElementById('bkgF').style.visibility == 'hidden') {
        document.getElementById('bkgF').style.visibility = '';
        $("#bkgF").hide();
    }
    if (document.getElementById('dlgF').style.visibility == 'hidden') {
        document.getElementById('dlgF').style.visibility = '';
        $("#dlgF").hide();
    }
    $("#bkgF").fadeIn(300, "linear", function () {
        $("#dlgF").show(200, "swing");
        $("#titulomodalF").prepend(title)
        $("#contenidomodalF").prepend('<div style="text-align:justify">' + contHtml + '</div>')
    });
    $("#closebtnmodalF").click(function () {
        $("#dlgF").hide('200', "swing", function () {
            $("#bkgF").fadeOut("300");
        });
    });
}


function winFormPopup(titulo, htmlForm) {
     
    //winFormPopup
    $("#miModal").prepend("<div style='display: visibility; z-index: 996; with height: 1750px;overflow-y: scroll;' id='TicketLecciones' class='modal-contenido'><div class='titlemodalcad'>" + titulo +
                "<div id='closebtnmodalF' class='closebtnmodalcad' title='Cerrar'>X</div></div>" +
                "<div class='leccion' style='text-align:justify'>" + htmlForm +  "</div></div></div>");

    $("#miModal").fadeIn(300, "linear", function () {
        $("#miModal").show();
        //$("#contenidomodalF").prepend("<div class='leccion' style='text-align:justify'>" + htmlForm + "</div>")
    });

    $("#closebtnmodalF").hover(function () {
        $(this).css("cursor", "default");
    })
    $("#closebtnmodalF").click(function () {
         
        $("#miModal").hide('200', "swing", function () {
            $("#miModal").replaceWith("<div id='miModal' class='modal'></div>");
            $("#miModal").fadeOut("300");
        });
    });
}

function ClosewinFormPopup() {
    $("#miModal").hide('200', "swing", function () {
        $("#miModal").replaceWith("<div id='miModal' class='modal'></div>");
        $("#miModal").fadeOut("300");
    });
}

function winFormPopUpModal2(title, contHtml, ancho, alto) {
    var altobkg = 0;
    var top = 0;
    altpan = $(window).height(); // devuelve la altura del viewport del navegador
    altobkg = $(document).height(); // devuelve la altura del documento HTML
    altbar = $(window).scrollTop(); // devuelve la posición vertical actual de la barra de scroll
    top = altbar + (altpan / 2);
    $("#winFormPopUpModal2").prepend("<div id='bkgF2' class='backgroundcad' style='display: visibility; z-index: 994; height: " + altobkg + "px;'>" +
        "<div id='dlgF2' class='modalcad' style='z-index: 995; display: none; top: " + a_entero(top) + "px; width: " + a_entero(ancho) + "px; margin-left: -" + a_entero(ancho) / 2 + "px; height: " + a_entero(alto) + "px; margin-top: -" + a_entero(alto) / 2 + "px;'>" +
        "<div id='titulomodalF2' class='titlemodalcad'>" +
        "<div id='closebtnmodalF2' class='closebtnmodalcad' title='Cerrar'>X</div>" +
        "</div><div id='contenidomodalF2' class='contenidomodalFormcad'></div></div></div>");

    if (document.getElementById('bkgF2').style.visibility == 'hidden') {
        document.getElementById('bkgF2').style.visibility = '';
        $("#bkgF2").hide();
    }
    if (document.getElementById('dlgF2').style.visibility == 'hidden') {
        document.getElementById('dlgF2').style.visibility = '';
        $("#dlgF2").hide();
    }
    $("#bkgF2").fadeIn(300, "linear", function () {
        $("#dlgF2").show(200, "swing");
        $("#titulomodalF2").prepend(title)
        $("#contenidomodalF2").prepend('<div style="text-align:justify">' + contHtml + '</div>')
    });
    $("#closebtnmodalF2").click(function () {
        $("#dlgF2").hide('200', "swing", function () {
            $("#bkgF2").fadeOut("300");
        });
    });
}

function closeWinModalPopUp2(sw) {
    $("#dlg2").hide('200', "swing", function () {
        $("#bkg2").fadeOut("300");
    });
}

function closeWinModalPopUp(sw) {
    $("#dlg").hide('200', "swing", function () {
        $("#bkg").fadeOut("300");
    });
}

function closeWinModalPopUpLoad(sw) {
    
    $("#dlgl").hide('200', "swing", function () {
        $("#bkgl").fadeOut("300");
    });
}

function closeWinFormModalPopUp() {
    $("#dlgF").hide('200', "swing", function () {
        $("#bkgF").fadeOut("300");
    });
}

function closeWinFormModalPopUp2() {
    $("#dlgF2").hide('200', "swing", function () {
        $("#bkgF2").fadeOut("300");
    });
}

function comprobarnavegador() {
    /* Variables para cada navegador, la funcion indexof() si no encuentra la cadena devuelve -1, 
    las variables se quedaran sin valor si la funcion indexof() no ha encontrado la cadena. */
    var is_safari = navigator.userAgent.toLowerCase().indexOf('safari/') > -1;
    var is_chrome = navigator.userAgent.toLowerCase().indexOf('chrome/') > -1;
    var is_firefox = navigator.userAgent.toLowerCase().indexOf('firefox/') > -1;
    var is_ie = navigator.userAgent.toLowerCase().indexOf('msie ') > -1;
    var nav = '';
    /* Detectando  si es Safari, vereis que en esta condicion preguntaremos por chrome ademas, esto es porque el 
    la cadena de texto userAgent de Safari es un poco especial y muy parecida a chrome debido a que los dos navegadores
    usan webkit. */

    if (is_safari && !is_chrome) {

        /* Buscamos la cadena 'Version' para obtener su posicion en la cadena de texto, para ello
        utilizaremos la funcion, tolowercase() e indexof() que explicamos anteriormente */
        var posicion = navigator.userAgent.toLowerCase().indexOf('Version/');

        /* Una vez que tenemos la posición de la cadena de texto que indica la version capturamos la
        subcadena con substring(), como son 4 caracteres los obtendremos de 9 al 12 que es donde 
        acaba la palabra 'version'. Tambien podraimos obtener la version con navigator.appVersion, pero
        la gran mayoria de las veces no es la version correcta. */
        var ver_safari = navigator.userAgent.toLowerCase().substring(posicion + 9, posicion + 12);

        // Convertimos la cadena de texto a float y mostramos la version y el navegador
        ver_safari = parseFloat(ver_safari);

        nav = 'Safari';
    }

    //Detectando si es Chrome
    if (is_chrome) {
        var posicion = navigator.userAgent.toLowerCase().indexOf('chrome/');
        var ver_chrome = navigator.userAgent.toLowerCase().substring(posicion + 7, posicion + 11);
        //Comprobar version
        ver_chrome = parseFloat(ver_chrome);
        nav = 'Chrome';
    }

    //Detectando si es Firefox
    if (is_firefox) {
        var posicion = navigator.userAgent.toLowerCase().lastIndexOf('firefox/');
        var ver_firefox = navigator.userAgent.toLowerCase().substring(posicion + 8, posicion + 12);
        //Comprobar version
        ver_firefox = parseFloat(ver_firefox);
        nav = 'Firefox';
    }

    //Detectando Cualquier version de IE
    if (is_ie) {
        var posicion = navigator.userAgent.toLowerCase().lastIndexOf('msie ');
        var ver_ie = navigator.userAgent.toLowerCase().substring(posicion + 5, posicion + 8);
        //Comprobar version
        ver_chrome = parseFloat(ver_ie);
        nav = 'IE';
    }

    return nav;
}

function centerModals() {
    console.log("Aqui Modal");
    //$('.modal').each(function (i) {
    //    var $clone = $(this).clone().css('display', 'block').appendTo('body');
    //    var top = Math.round(($clone.height() - $clone.find('.modal-content').height()) / 2);
    //    console.log(top);
    //    top = top > 0 ? top : 0;
    //    if (top > 290) {
    //        top = top - top * 30 / 100
    //    }
    //    $clone.remove();
    //    $(this).find('.modal-content').css("margin-top", top);
    //});
    var modal = $(this),
           dialog = modal.find('.modal-dialog');
    modal.css('display', 'block');

    console.log($(window).height());
    console.log(dialog.height());
    // Dividing by two centers the modal exactly, but dividing by three 
    // or four works better for larger screens.
    dialog.css("margin-top", Math.max(0, ($(window).height() - (dialog.height() <= 10 ? $(window).height() / 2 : dialog.height())) / 2));
}
//$('.modal').on('show.bs.modal', centerModals);
//$("#ModalContent").on('show.bs.modal', centerModals);
//$('#myModal').on('shown.bs.modal', function (e) {
//    // do something...
//    console.log("Aqui Modal test");
//})
$(window).on('resize', centerModals);

$(document).ready(function () {
    //centerModals();
    //centerModals();
    $('.modal').on('show.bs.modal', centerModals);
});

function UpdatePoints() {
    $.ajax({
        url: '/Point/ListPoints?var=' + Math.random(),
        dataType: 'text',
        cache: false,
        async: true,
        success: function (cantpoints) {
            $("#idvalorpoint").empty();
            $("#idvalorpoint").text(cantpoints);
        }
    });
}

/*Módulo de Gestión del conocimiento*/
function soloLetras(e) {
    key = e.keyCode || e.which;
    tecla = String.fromCharCode(key).toLowerCase();
    letras = " áéíóúäëïöüabcdefghijklmnñopqrstuvwxyz";
    especiales = [8, 32];

    tecla_especial = false
    for (var i in especiales) {
        if (key == especiales[i]) {
            tecla_especial = true;
            break;
        }
    }

    if (letras.indexOf(tecla) == -1 && !tecla_especial)
        return false;
}


function soloNumeros(e) {

    key = e.keyCode || e.which;
    tecla = String.fromCharCode(key).toLowerCase();
    letras = "1234567890";
    especiales = [8];

    tecla_especial = false
    for (var i in especiales) {
        if (key == especiales[i]) {
            tecla_especial = true;
            break;
        }
    }

    if (letras.indexOf(tecla) == -1 && !tecla_especial)
        return false;
}

function soloLetrasyNumeros(e) {
    key = e.keyCode || e.which;
    tecla = String.fromCharCode(key).toLowerCase();
    letras = " áéíóúäëïöüabcdefghijklmnñopqrstuvwxyz1234567890";
    especiales = [8, 32];

    tecla_especial = false
    for (var i in especiales) {
        if (key == especiales[i]) {
            tecla_especial = true;
            break;
        }
    }

    if (letras.indexOf(tecla) == -1 && !tecla_especial)
        return false;
}

function soloDecimal(e, field) {
    key = e.keyCode ? e.keyCode : e.which
    // backspace
    if (key == 8) return true
    // 0-9
    if (key > 47 && key < 58) {
        if (field.value== "") return true;
        var existePto = (/[.]/).test(field.value);
        if (existePto === false) {
            regexp = /.[0-9]{10}$/; //PARTE ENTERA 10
        }
        else {
            regexp = /.[0-9]{2}$/; //PARTE DECIMAL2
        }
        return !(regexp.test(field.value));
    }
    // .
    if (key == 46) {
        if (field.value == "") return false
        regexp = /^[0-9]+$/
        return regexp.test(field.value)
    }
    // other key
    return false
}



/*Fin*/