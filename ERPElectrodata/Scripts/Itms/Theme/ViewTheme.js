$(document).ready(function () {
    
    $("#txtIdTema").attr("readonly", "true")
    $("#txtNomTema").attr("readonly", "true")
    $("#DateEnd").attr("readonly", "true");
    //$("#idCategoria").kendoComboBox({
    //    autoBind: false,
    //    filter: "contains",
    //    enabled: false,
    //    //index: 2,
    //    //placeholder: "Select Category...",
    //    autoBind: true,
    //    dataTextField: "NAM_CATE",
    //    dataValueField: "ID_CATE",
    //    template: '<div style="font-weight:bold;">${data.NAM_CATE_1}</div>' +
    //                '<div style="margin-left:5px;" >  ${data.NAM_CATE_2} </div>' +
    //                '<div style="margin-left:10px;">${data.NAM_CATE_3}</div>' +
    //                '<div style="margin-left:10px;">${data.NAM_CATE_4}</div>',
    //    dataSource: {
    //        serverFiltering: true,
    //        schema: {
    //            data: "Data",
    //            total: "Count"
    //        },
    //        transport: {
    //            read: "/CategoryTicket/List"
    //        }
    //    }
    //}).data("kendoComboBox");
    
    var DateEndNuevo = $("#DateEnd").kendoDateTimePicker({ enabled: false}).data("kendoDateTimePicker");
});






