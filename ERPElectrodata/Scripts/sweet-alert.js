!(function (a, b) {
    function c(b) {
        var c = p(),
            d = c.querySelector("h2"),
            e = c.querySelector("p"),
            f = c.querySelector("button.cancel"),
            g = c.querySelector("button.confirm");
        if (
            ((d.innerHTML = u(b.title).split("\n").join("<br>")),
                (e.innerHTML = u(b.text || "")
                    .split("\n")
                    .join("<br>")),
                b.text && w(e),
                y(c.querySelectorAll(".icon")),
                b.type)
        ) {
            for (var h = !1, i = 0; i < n.length; i++)
                if (b.type === n[i]) {
                    h = !0;
                    break;
                }
            if (!h) return a.console.error("Unknown alert type: " + b.type), !1;
            var j = c.querySelector(".icon." + b.type);
            switch ((w(j), b.type)) {
                case "success":
                    s(j, "animate"), s(j.querySelector(".tip"), "animateSuccessTip"), s(j.querySelector(".long"), "animateSuccessLong");
                    break;
                case "error":
                    s(j, "animateErrorIcon"), s(j.querySelector(".x-mark"), "animateXMark");
                    break;
                case "warning":
                    s(j, "pulseWarning"), s(j.querySelector(".body"), "pulseWarningIns"), s(j.querySelector(".dot"), "pulseWarningIns");
            }
        }
        if (b.imageUrl) {
            var k = c.querySelector(".icon.custom");
            (k.style.backgroundImage = "url(" + b.imageUrl + ")"), w(k);
            var l = 80,
                m = 80;
            if (b.imageSize) {
                var o = b.imageSize.split("x")[0],
                    q = b.imageSize.split("x")[1];
                o && q ? ((l = o), (m = q), k.css({ width: o + "px", height: q + "px" })) : a.console.error("Parameter imageSize expects value with format WIDTHxHEIGHT, got " + b.imageSize);
            }
            k.setAttribute("style", k.getAttribute("style") + "width:" + l + "px; height:" + m + "px");
        }
        c.setAttribute("data-has-cancel-button", b.showCancelButton),
            b.showCancelButton ? (f.style.display = "inline-block") : y(f),
            c.setAttribute("data-has-confirm-button", b.showConfirmButton),
            b.showConfirmButton ? (g.style.display = "inline-block") : y(g),
            b.cancelButtonText && (f.innerHTML = u(b.cancelButtonText)),
            b.confirmButtonText && (g.innerHTML = u(b.confirmButtonText)),
            (g.className = "confirm btn btn-lg"),
            s(c, b.containerClass),
            s(g, b.confirmButtonClass),
            s(f, b.cancelButtonClass),
            s(d, b.titleClass),
            s(e, b.textClass),
            c.setAttribute("data-allow-ouside-click", b.allowOutsideClick);
        var r = b.doneFunction ? !0 : !1;
        c.setAttribute("data-has-done-function", r), c.setAttribute("data-timer", b.timer);
    }
    function d(a, b) {
        for (var c in b) b.hasOwnProperty(c) && (a[c] = b[c]);
        return a;
    }
    function e() {
        var a = p();
        B(q(), 10), w(a), s(a, "showSweetAlert"), t(a, "hideSweetAlert"), (h = b.activeElement);
        var c = a.querySelector("button.confirm");
        c.focus(),
            setTimeout(function () {
                s(a, "visible");
            }, 500);
        var d = a.getAttribute("data-timer");
        "null" !== d &&
            "" !== d &&
            setTimeout(function () {
                f();
            }, d);
    }
    function f() {
        var c = p();
        C(q(), 5), C(c, 5), t(c, "showSweetAlert"), s(c, "hideSweetAlert"), t(c, "visible");
        var d = c.querySelector(".icon.success");
        t(d, "animate"), t(d.querySelector(".tip"), "animateSuccessTip"), t(d.querySelector(".long"), "animateSuccessLong");
        var e = c.querySelector(".icon.error");
        t(e, "animateErrorIcon"), t(e.querySelector(".x-mark"), "animateXMark");
        var f = c.querySelector(".icon.warning");
        t(f, "pulseWarning"), t(f.querySelector(".body"), "pulseWarningIns"), t(f.querySelector(".dot"), "pulseWarningIns"), (a.onkeydown = j), (b.onclick = i), h && h.focus(), (k = void 0);
    }
    function g() {
        var a = p();
        a.style.marginTop = A(p());
    }
    var h,
        i,
        j,
        k,
        l = ".sweet-alert",
        m = ".sweet-overlay",
        n = ["error", "warning", "info", "success"],
        o = {
            title: "",
            text: "",
            type: null,
            allowOutsideClick: !1,
            showCancelButton: !1,
            showConfirmButton: !0,
            closeOnConfirm: !0,
            closeOnCancel: !0,
            confirmButtonText: "Aceptar",
            confirmButtonClass: "btn-primary",
            cancelButtonText: "Cancel",
            cancelButtonClass: "btn-default",
            containerClass: "",
            titleClass: "",
            textClass: "",
            imageUrl: null,
            imageSize: null,
            timer: null,
        },
        p = function () {
            return b.querySelector(l);
        },
        q = function () {
            return b.querySelector(m);
        },
        r = function (a, b) {
            return new RegExp(" " + b + " ").test(" " + a.className + " ");
        },
        s = function (a, b) {
            b && !r(a, b) && (a.className += " " + b);
        },
        t = function (a, b) {
            var c = " " + a.className.replace(/[\t\r\n]/g, " ") + " ";
            if (r(a, b)) {
                for (; c.indexOf(" " + b + " ") >= 0;) c = c.replace(" " + b + " ", " ");
                a.className = c.replace(/^\s+|\s+$/g, "");
            }
        },
        u = function (a) {
            var c = b.createElement("div");
            return c.appendChild(b.createTextNode(a)), c.innerHTML;
        },
        v = function (a) {
            (a.style.opacity = ""), (a.style.display = "block");
        },
        w = function (a) {
            if (a && !a.length) return v(a);
            for (var b = 0; b < a.length; ++b) v(a[b]);
        },
        x = function (a) {
            (a.style.opacity = ""), (a.style.display = "none");
        },
        y = function (a) {
            if (a && !a.length) return x(a);
            for (var b = 0; b < a.length; ++b) x(a[b]);
        },
        z = function (a, b) {
            for (var c = b.parentNode; null !== c;) {
                if (c === a) return !0;
                c = c.parentNode;
            }
            return !1;
        },
        A = function (a) {
            (a.style.left = "-9999px"), (a.style.display = "block");
            var b = a.clientHeight,
                c = parseInt(getComputedStyle(a).getPropertyValue("padding"), 10);
            return (a.style.left = ""), (a.style.display = "none"), "-" + parseInt(b / 2 + c) + "px";
        },
        B = function (a, b) {
            if (+a.style.opacity < 1) {
                (b = b || 16), (a.style.opacity = 0), (a.style.display = "block");
                var c = +new Date(),
                    d = function () {
                        (a.style.opacity = +a.style.opacity + (new Date() - c) / 100), (c = +new Date()), +a.style.opacity < 1 && setTimeout(d, b);
                    };
                d();
            }
        },
        C = function (a, b) {
            (b = b || 16), (a.style.opacity = 1);
            var c = +new Date(),
                d = function () {
                    (a.style.opacity = +a.style.opacity - (new Date() - c) / 100), (c = +new Date()), +a.style.opacity > 0 ? setTimeout(d, b) : (a.style.display = "none");
                };
            d();
        },
        D = function (c) {
            if (MouseEvent) {
                var d = new MouseEvent("click", { view: a, bubbles: !1, cancelable: !0 });
                c.dispatchEvent(d);
            } else if (b.createEvent) {
                var e = b.createEvent("MouseEvents");
                e.initEvent("click", !1, !1), c.dispatchEvent(e);
            } else b.createEventObject ? c.fireEvent("onclick") : "function" == typeof c.onclick && c.onclick();
        },
        E = function (b) {
            "function" == typeof b.stopPropagation ? (b.stopPropagation(), b.preventDefault()) : a.event && a.event.hasOwnProperty("cancelBubble") && (a.event.cancelBubble = !0);
        };
    (a.sweetAlertInitialize = function () {
        var a =
            '<div class="sweet-overlay"></div><div class="sweet-alert"><div class="icon error"><span class="x-mark"><span class="line left"></span><span class="line right"></span></span></div><div class="icon warning"> <span class="body"></span> <span class="dot"></span> </div> <div class="icon info"></div> <div class="icon success"> <span class="line tip"></span> <span class="line long"></span> <div class="placeholder"></div> <div class="fix"></div> </div> <div class="icon custom"></div> <h2>Title</h2><p class="lead text-muted">Text</p><p><button class="confirm btn btn-lg" tabIndex="1">Aceptar</button><button class="cancel btn btn-lg" tabIndex="2">Cancel</button></p></div>',
            c = b.createElement("div");
        (c.innerHTML = a), b.body.appendChild(c);
    }),
        (a.sweetAlert = a.swal = function () {
            function h(a) {
                var b = a.keyCode || a.which;
                if (-1 !== [9, 13, 32, 27].indexOf(b)) {
                    for (var c = a.target || a.srcElement, d = -1, e = 0; e < w.length; e++)
                        if (c === w[e]) {
                            d = e;
                            break;
                        }
                    9 === b
                        ? ((c = -1 === d ? u : d === w.length - 1 ? w[0] : w[d + 1]), E(a), c.focus())
                        : ((c = 13 === b || 32 === b ? (-1 === d ? u : void 0) : 27 !== b || v.hidden || "none" === v.style.display ? void 0 : v), void 0 !== c && D(c, a));
                }
            }
            function l(a) {
                var b = a.target || a.srcElement,
                    c = a.relatedTarget,
                    d = r(n, "visible");
                if (d) {
                    var e = -1;
                    if (null !== c) {
                        for (var f = 0; f < w.length; f++)
                            if (c === w[f]) {
                                e = f;
                                break;
                            }
                        -1 === e && b.focus();
                    } else k = b;
                }
            }
            if (void 0 === arguments[0]) return a.console.error("sweetAlert expects at least 1 attribute!"), !1;
            var m = d({}, o);
            switch (typeof arguments[0]) {
                case "string":
                    (m.title = arguments[0]), (m.text = arguments[1] || ""), (m.type = arguments[2] || "");
                    break;
                case "object":
                    if (void 0 === arguments[0].title) return a.console.error('Missing "title" argument!'), !1;
                    (m.title = arguments[0].title),
                        (m.text = arguments[0].text || o.text),
                        (m.type = arguments[0].type || o.type),
                        (m.allowOutsideClick = arguments[0].allowOutsideClick || o.allowOutsideClick),
                        (m.showCancelButton = void 0 !== arguments[0].showCancelButton ? arguments[0].showCancelButton : o.showCancelButton),
                        (m.showConfirmButton = void 0 !== arguments[0].showConfirmButton ? arguments[0].showConfirmButton : o.showConfirmButton),
                        (m.closeOnConfirm = void 0 !== arguments[0].closeOnConfirm ? arguments[0].closeOnConfirm : o.closeOnConfirm),
                        (m.closeOnCancel = void 0 !== arguments[0].closeOnCancel ? arguments[0].closeOnCancel : o.closeOnCancel),
                        (m.timer = arguments[0].timer || o.timer),
                        (m.confirmButtonText = o.showCancelButton ? "Confirm" : o.confirmButtonText),
                        (m.confirmButtonText = arguments[0].confirmButtonText || o.confirmButtonText),
                        (m.confirmButtonClass = arguments[0].confirmButtonClass || (arguments[0].type ? "btn-" + arguments[0].type : null) || o.confirmButtonClass),
                        (m.cancelButtonText = arguments[0].cancelButtonText || o.cancelButtonText),
                        (m.cancelButtonClass = arguments[0].cancelButtonClass || o.cancelButtonClass),
                        (m.containerClass = arguments[0].containerClass || o.containerClass),
                        (m.titleClass = arguments[0].titleClass || o.titleClass),
                        (m.textClass = arguments[0].textClass || o.textClass),
                        (m.imageUrl = arguments[0].imageUrl || o.imageUrl),
                        (m.imageSize = arguments[0].imageSize || o.imageSize),
                        (m.doneFunction = arguments[1] || null);
                    break;
                default:
                    return a.console.error('Unexpected type of argument! Expected "string" or "object", got ' + typeof arguments[0]), !1;
            }
            c(m), g(), e();
            for (
                var n = p(),
                q = function (a) {
                    var b = a.target || a.srcElement,
                        c = b.className.indexOf("confirm") > -1,
                        d = r(n, "visible"),
                        e = m.doneFunction && "true" === n.getAttribute("data-has-done-function");
                    switch (a.type) {
                        case "click":
                            if (c && e && d) m.doneFunction(!0), m.closeOnConfirm && f();
                            else if (e && d) {
                                var g = String(m.doneFunction).replace(/\s/g, ""),
                                    h = "function(" === g.substring(0, 9) && ")" !== g.substring(9, 10);
                                h && m.doneFunction(!1), m.closeOnCancel && f();
                            } else f();
                    }
                },
                s = n.querySelectorAll("button"),
                t = 0;
                t < s.length;
                t++
            )
                s[t].onclick = q;
            (i = b.onclick),
                (b.onclick = function (a) {
                    var b = a.target || a.srcElement,
                        c = n === b,
                        d = z(n, a.target),
                        e = r(n, "visible"),
                        g = "true" === n.getAttribute("data-allow-ouside-click");
                    !c && !d && e && g && f();
                });
            var u = n.querySelector("button.confirm"),
                v = n.querySelector("button.cancel"),
                w = n.querySelectorAll("button:not([type=hidden])");
            (j = a.onkeydown),
                (a.onkeydown = h),
                (u.onblur = l),
                (v.onblur = l),
                (a.onfocus = function () {
                    a.setTimeout(function () {
                        void 0 !== k && (k.focus(), (k = void 0));
                    }, 0);
                });
        }),
        (a.swal.setDefaults = function (a) {
            if (!a) throw new Error("userParams is required");
            if ("object" != typeof a) throw new Error("userParams has to be a object");
            d(o, a);
        }),
        (a.swal.close = function () {
            f();
        }),
        (function () {
            "complete" === b.readyState || ("interactive" === b.readyState && b.body)
                ? sweetAlertInitialize()
                : b.addEventListener
                    ? b.addEventListener(
                        "DOMContentLoaded",
                        function a() {
                            b.removeEventListener("DOMContentLoaded", a, !1), sweetAlertInitialize();
                        },
                        !1
                    )
                    : b.attachEvent &&
                    b.attachEvent("onreadystatechange", function c() {
                        "complete" === b.readyState && (b.detachEvent("onreadystatechange", c), sweetAlertInitialize());
                    });
        })();
})(window, document);
