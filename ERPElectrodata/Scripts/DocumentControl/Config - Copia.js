var phonecatApp = angular.module('phonecatApp', [
  'ngRoute'
  , 'phonecatControllers', 'ui.bootstrap.demo'
]);

phonecatApp.config(['$routeProvider',
  function ($routeProvider) {
      $routeProvider
          .when('/', { templateUrl: function (params) { return '/Home/Init?' + Math.random() } })
          .when('/Home', { templateUrl: function (params) { return '/Home/Init?' + Math.random() } })
          .when('/Inicio', { templateUrl: function (params) { return '/Home/PaginaInicio?' + Math.random() } })

          //Supervisor
          .when('/SupervisorInicio', { templateUrl: function (params) { return '/Supervision/Inicio?' + Math.random() } })
          .when('/SupervisorReportes', { templateUrl: function (params) { return '/Supervision/Reportes?' + Math.random() } })
          .when('/Supervision/Solicitante/:id/:tipo', { templateUrl: function (params) { return '/Supervision/ReporteSolicitante/' + params.id + '/' + params.tipo + '?var=' + Math.random() } })
          .when('/Supervision/Asignado/:id/:tipo', { templateUrl: function (params) { return '/Supervision/ReporteAsignado/' + params.id + '/' + params.tipo + '?var=' + Math.random() } })
          .when('/Supervision/TicketActivoDetalle/:id/:tipo', { templateUrl: function (params) { return '/Supervision/ReporteTicketActivoDetalle/' + params.id + '/' + params.tipo + '?var=' + Math.random() } })

        //Incident & Request
          .when('/NewTicket', { templateUrl: function (params) { return '/Ticket/Create?var=' + Math.random() }, controller: 'PhoneDetailCtrl' })
          .when('/TicketPortal', { templateUrl: function (params) { return '/Ticket/ListTicketPortal?var=' + Math.random() } })
          .when('/FindTicket', { templateUrl: '/Ticket/FindTicket?var=' + Math.random() })
          .when('/WorkTicket', { templateUrl: '/Ticket/Work?var=' + Math.random() })
          .when('/WorkLoadTicket', { templateUrl: '/ReportTicket/WorkLoad?var=' + Math.random() })
          .when('/DetailsTicket/Index/:id', { templateUrl: function (params) { return '/DetailsTicket/Index/' + params.id + '?var=' + Math.random() } })
          .when('/Ticket/AsignarTicketPortal/:id', { templateUrl: function (params) { return '/Ticket/AsignarTicketPortal/' + params.id + '?var=' + Math.random() } })
          .when('/DeliveryReception/Details/:id', { templateUrl: function (params) { return '/DeliveryReception/Details/' + params.id + '?var=' + Math.random() } })
          .when('/ManagingRoles', { templateUrl: '/Administrator/ManagingRoles?var=' + Math.random() })
          .when('/TicketActividad', { templateUrl: '/Ticket/ReporteActividadTicket?var=' + Math.random() })
          .when('/TicketAutomatico', { templateUrl: '/Ticket/TicketAutomatico?var=' + Math.random() })
          .when('/ReportesPlanos', { templateUrl: '/Ticket/ReportesPlanos?var=' + Math.random() })
      
          /*Gestión de Problemas*/
          .when('/BandejaTicketProblema', { templateUrl: '/Ticket/FindTicketProblema?var=' + Math.random() })
          .when('/TicketProblema', { templateUrl: '/Ticket/CrearTicketProblema' })
          //Proyectos
          .when('/OrderForm', { templateUrl: '/OrderForm/Index?var=' + Math.random() })
          .when('/TicketOP', { templateUrl: '/OrderForm/TicketOP?var=' + Math.random() })
          .when('/TicketMantenimiento', { templateUrl: '/OrderForm/ReporteMantenimientos?var=' + Math.random() }) 
          .when('/FindOrderForm', { templateUrl: '/OrderForm/BuscarOP?var=' + Math.random() })
          .when('/FindOPSidige', { templateUrl: '/OrderForm/CrearOPSidige?var=' + Math.random() })
          .when('/Renovation', { templateUrl: '/OrderForm/Renovation?var=' + Math.random() })
          .when('/ProyectosGerencia', { templateUrl: '/OrderForm/ReporteGerencia?var=' + Math.random() })
          .when('/ProyectosGerenciaITO', { templateUrl: '/OrderForm/ReporteGerenciaITO?var=' + Math.random() })
          .when('/Proyectos', { templateUrl: '/OrderForm/ReporteProyectos?var=' + Math.random() })
          .when('/OPsIniciales', { templateUrl: '/OrderForm/OPsIniciales?var=' + Math.random() })
          .when('/OPsRenovaciones', { templateUrl: '/OrderForm/OPsRenovaciones?var=' + Math.random() })
          .when('/ProductosRenovados', { templateUrl: '/OrderForm/ProductosRenovados?var=' + Math.random() })
          .when('/Graficos', { templateUrl: '/OrderForm/ReporteGraficos?var=' + Math.random() })
          .when('/DocumentSaleActivity/Index/:id', { templateUrl: function (params) { return '/DocumentSaleActivity/Index/' + params.id } })
          .when('/OrderForm/Details/:id', { templateUrl: function (params) { return '/OrderForm/Details/' + params.id } })
          .when('/OrderForm/DatosProductosServicios/:id', { templateUrl: function (params) { return '/OrderForm/DatosProductosServicios/' + params.id } })
          //.when('/OrderForm/Gantt/:id', { templateUrl: '/OrderForm/Gantt/' + params.id })
          .when('/OrderForm/Gantt/:id', { templateUrl: function (params) { return '/OrderForm/Gantt/' + params.id } })
          .when('/OrderForm/CronogramaHistorico/:id', { templateUrl: function (params) { return '/OrderForm/CronogramaHistorico/' + params.id } })
        //.when('/OrderForm/verComentarios/:id', { templateUrl: function (params) { return '/OrderForm/verComentarios/' + params.id } })
        //.when('/OrderForm/verAcciones/:id', { templateUrl: function (params) { return '/OrderForm/verAcciones/' + params.id } })
          .when('/OrderForm/AdjuntarArchivos/:id', { templateUrl: function (params) { return '/OrderForm/AdjuntarArchivos/' + params.id } })
        //Project
          .when('/Project', { templateUrl: '/Project/ListProjects?var=' + Math.random() })
        //Asset
          .when('/Asset', { templateUrl: function (params) { return '/Asset/Index?' + Math.random() } })
          .when('/AssetTicket', { templateUrl: '/Asset/Ticket' })
          .when('/NewAsset', { templateUrl: '/Asset/Create' })
          .when('/FindAsset', { templateUrl: '/Asset/Find' })
          .when('/Delivery/:id', { templateUrl: function (params) { return '/DeliveryReception/Create/' + params.id; } })
          .when('/Reception', { templateUrl: '/DeliveryReception/CreateRecepcion' })
          .when('/Reasignacion', { templateUrl: '/DeliveryReception/CreateReasignacion' })
          .when('/FindFormat', { templateUrl: '/Ticket/FindFormat' })
          .when('/ReportAsset', { templateUrl: '/Asset/viewAssetReport' })
          .when('/Asset/Details/:id', { templateUrl: function (params) { return '/Asset/Details/' + params.id; } })
          .when('/Asset/Detalle/:id', { templateUrl: function (params) { return '/Asset/Detalle/' + params.id; } })
          .when('/DeliveryReception/DetailsDelivery/:id', { templateUrl: function (params) { return '/DeliveryReception/DetailsDelivery/' + params.id; } })
          .when('/ReporteActivos', { templateUrl: '/Asset/Reportes' })
          .when('/CargaMasiva', { templateUrl: '/Asset/CargaMasiva' })

          .when('/GestionPersonas', { templateUrl: '/Persona/Index' })

        //ActivoMantenimiento
          .when('/Alertas', { templateUrl: '/ActivoMantenimiento/Alertas' })

        //Componente
          .when('/VistaAgrupada', { templateUrl: '/ComponenteStockCabecera/VistaAgrupada' })
          .when('/BandejaComponente/:id', { templateUrl: function (params) { return '/ComponenteStockCabecera/Index/' + params.id; } })
          .when('/NuevoComponente', { templateUrl: '/ComponenteStockCabecera/Crear' })
          .when('/ComponenteDetalle/:id', { templateUrl: function (params) { return '/Componente/Detalle/' + params.id; } })
          .when('/DetalleComponente/:id', { templateUrl: function (params) { return '/Componente/DetalleComponente/' + params.id; } })
        //ComponenteStockCabecera
          .when('/ComponenteStockDetalle/:id', { templateUrl: function (params) { return '/ComponenteStockCabecera/Detalle/' + params.id; } })
        //Tipo Activo
          .when('/TipoActivo/:id', { templateUrl: function (params) { return '/TipoActivo/Editar/' + params.id; } })
        //Programa
          .when('/BandejaPrograma', { templateUrl: '/Programa/Index' })
          .when('/BuscarPrograma', { templateUrl: '/Programa/Busqueda' })
          .when('/NuevoPrograma', { templateUrl: '/Programa/Crear' })
          .when('/ProgramaDetalle/:id', { templateUrl: function (params) { return '/Programa/Detalle/' + params.id; } })
          .when('/ReportePrograma', { templateUrl: '/Programa/Reportes' })
        //ProgramaLicencia
          .when('/BandejaLicencia', { templateUrl: '/ProgramaLicencia/Index' })
          .when('/NuevaLicencia', { templateUrl: '/ProgramaLicencia/Crear' })
          .when('/LicenciasPorVencer', { templateUrl: '/ProgramaLicencia/LicenciasPorVencer' })
          .when('/LicenciaDetalle/:id', { templateUrl: function (params) { return '/ProgramaLicencia/Detalle/' + params.id; } })
        //Inventario
          .when('/Inventario', { templateUrl: '/Inventario/Index' })
          .when('/Inventario/Detalle/:id', { templateUrl: function (params) { return '/Inventario/Detalle/' + params.id; } })
          .when('/Inventario/TicketDetalle/:id', { templateUrl: function (params) { return '/Inventario/TicketDetalle/' + params.id; } })
          .when('/TicketInventario', { templateUrl: '/Inventario/Ticket' })
          .when('/BuscarInventario', { templateUrl: '/Inventario/Buscar' })
          .when('/NuevoInventario', { templateUrl: '/Inventario/Crear' })
          .when('/EntregaInventario', { templateUrl: '/Inventario/Entrega' })
          .when('/RecepcionInventario', { templateUrl: '/Inventario/Recepcion' })
          .when('/ReporteInventario', { templateUrl: '/Inventario/Reporte' })
          .when('/GraficoInventario', { templateUrl: '/Inventario/Graficos' })
        //Change Management
          .when('/Change', { templateUrl: '/Change/Index' })
          .when('/NewChange', { templateUrl: '/Change/Create' })
          .when('/FindChange', { templateUrl: '/Change/Find' })
          //KnowlEdge
          .when('/Knowledge', { templateUrl: '/Knowledge/Index/97' })
          .when('/Knowledge/:id', { templateUrl: function (params) { return '/Knowledge/Index/' + params.id; } })
        //Document Control
          .when('/DocumentControl', { templateUrl: '/DocumentControl/Index' })
          .when('/DocumentControl/Reception', { templateUrl: '/DocumentControl/CreateReception' })
          .when('/RegistroVenta', { templateUrl: '/DocumentoVenta/Index' })
          .when('/RegistroVenta/Nuevo', { templateUrl: '/DocumentoVenta/Crear' })
          .when('/RegistroVenta/Detalle/:id', { templateUrl: function (params) { return '/DocumentoVenta/Detalle/' + params.id; } })
          .when('/RegistroVenta/Reporte', { templateUrl: '/DocumentoVenta/ReporteSoluciones' })
        //Talent
          .when('/Talent', { templateUrl: '/Talent/Index' })
          .when('/Talent/Profile/:id', { templateUrl: function (params) { return '/Talent/Profile/' + params.id; } })
          .when('/Talent/Edit/:id/:id1', { templateUrl: function (params) { return '/Talent/Edit/' + params.id + '/' + params.id1; } })
          .when('/Talent/Contract/:id', { templateUrl: function (params) { return '/Talent/Contract/' + params.id } })
          .when('/NewProfile', { templateUrl: '/Talent/Create' })
          .when('/FindProfile', { templateUrl: '/Talent/Find?id=0' })
          .when('/Achievements', { templateUrl: '/Talent/ViewFindAchievements' })
          .when('/Organization', { templateUrl: '/Talent/ViewChart' })
          //.when('/Organigrama', { templateUrl: '/Organigrama/Inicio'})
          .when('/Attendance', { templateUrl: '/Assistance/Report_Attendance' })
          .when('/Reports', { templateUrl: '/Talent/Reports' })
          .when('/Scheduler', { templateUrl: '/Room/Index' })
          .when('/Vacation', { templateUrl: '/Vacation/Index' })
          .when('/RequestVacation', { templateUrl: '/Vacation/IndexRequestVacation' })
          .when('/PaymentBallots', { templateUrl: '/Talent/LoadPaymentBallots' })
          .when('/EvaluationStaff', { templateUrl: '/EvaluationStaff/Index' })
          .when('/Organization', { templateUrl: '/Talent/ViewChart' })
          .when('/Events', { templateUrl: '/Event/Index' })
          .when('/ReporteGradoInstruccion', { templateUrl: '/Talent/ReporteGradoInstruccion' })
          .when('/Evaluacion', { templateUrl: '/EvaluacionPersonal/Index' })
          .when('/SolicitudVacaciones', { templateUrl: '/Vacaciones/Index' })
          .when('/AprobacionVacaciones', { templateUrl: '/Vacaciones/Aprobaciones' })
          //Account
          .when('/ChangePassword', { templateUrl: '/Account/Manage' })
          .when('/ChangePassword/:id', { templateUrl: function (params) { return '/Account/Manage/' + params.id } })
        //Financial
          .when('/Financial', { templateUrl: '/Financial/Index' })
          .when('/NewTransaction', { templateUrl: '/Transaction/Create' })
          .when('/FindTransaction', { templateUrl: '/Transaction/Find' })
          .when('/DeliverySustain', { templateUrl: '/DeliverySustain/Index' })
          .when('/Closure', { templateUrl: '/DeliverySustain/viewClosure' })
        //Sales
          .when('/SalesOpportunities', { templateUrl: '/SalesOpportunities/Index' })
          .when('/NewSalesOpportunity', { templateUrl: '/SalesOpportunities/Create' })
          .when('/PreSaleReport', { templateUrl: '/SalesOpportunities/PreSaleReport' })
          .when('/Scheduled', { templateUrl: '/Sales/viewChartScheduled' })
          .when('/SalesDashboard', { templateUrl: '/Sales/viewSalesDashboard' })
          .when('/PerformanceTotal', { templateUrl: '/Sales/viewPerformanceTotal' })
          //Inform
          .when('/Inform', { templateUrl: '/Inform/Index' })
          .when('/IndicatorTicket', { templateUrl: '/Inform/ViewReportTicket' })
          .when('/IndexPlantilla', { templateUrl: '/Inform/IndexPlantilla' })
          .when('/Detalle', { templateUrl: '/Inform/Detalle' })
          .when('/CrearPlantilla', { templateUrl: '/Inform/CrearPlantilla' })
          .when('/EditarPlantilla/:id', { templateUrl: function (params) { return '/Inform/EditarPlantilla/' + params.id; } })
          .when('/InformeAutomatico/:id', { templateUrl: function (params) { return '/Inform/InformeAutomatico/' + params.id; } })
      //.otherwise({redirectTo: '/Home'});
      //Administration
          .when('/Eventos', { templateUrl: '/Seguridad/IndexEvento' })
          .when('/Seguridad', { templateUrl: '/Seguridad/Index' })
          .when('/ManagingCategory', { templateUrl: '/Administrator/ManagingCategory' })
          .when('/ProgramacionMasiva', { templateUrl: '/Administrator/ProgramacionMasiva' })
          .when('/FindRoles', { templateUrl: '/Administrator/FindRoles' })
          .when('/ReporteTicket', { templateUrl: '/TicketReporte/Index' })
          .when('/Mantenimientos', { templateUrl: '/Maintenance/IndexMantenimiento' })
          .when('/Encuestas', { templateUrl: '/EncuestaConfiguracion/IndexEncuestaConfiguracion' })
          //.when('/Encuesta/Graficos', { templateUrl: '/EncuestaConfiguracion/Graficos' })
      //Activity
          .when('/NewActivity', { templateUrl: '/Activity/NewActivity' })
          .when('/DetailsActivity', { templateUrl: '/Activity/DetailsActivity' })
          .when('/ReporteActivity', { templateUrl: '/Activity/Reportes' })
      //Reporte Activity
          .when('/ReportActivity', { templateUrl: '/Activity/ReportActivity' })
          .when('/ReportClientEnginer', { templateUrl: '/Activity/ReportClientEnginer' })
          .when('/ReportTypeActivity', { templateUrl: '/Activity/ReportTypeActivity' })
          //.when('/ReportTimeActivity', { templateUrl: '/Activity/ReportTimeActivity' })
          .when('/ReportOPImplementation', { templateUrl: '/Activity/ReportOPImplementation' })
          .when('/ReportExternalSupport', { templateUrl: '/Activity/ReportExternalSupport' })
          .when('/ReporteActividad', { templateUrl: '/Activity/ReporteActividad' })
          .when('/Soporte', { templateUrl: '/Soporte/Index' })
      // Soporte Electrodata
          .when('/SoporteED/ListaSoportes/:id', { templateUrl: function (params) { return '/SoporteED/ListaSoportes/' + params.id + '?var=' + Math.random() } })
      //Gestion de Cambios
          .when('/ChangeRequest', { templateUrl: '/ChangeRequest/Index' })
          .when('/ResumenGestionCambio/:id', { templateUrl: function (params) { return '/ChangeRequest/ResumenGestionCambio/' + params.id; } })
          .when('/Notificaciones', { templateUrl: '/ChangeRequest/VerTodosNotificaciones' })
          .when('/NewChangeRequest', { templateUrl: '/ChangeRequest/Create' })
          .when('/ReportChange', { templateUrl: '/ChangeRequest/Reportes' })
          .when('/Consolidado', { templateUrl: '/ChangeRequest/Consolidado' })

          .when('/Soporte', { templateUrl: '/Soporte/Index' })
      //Gestión del Conocimiento
        .when('/ApprovinKnowledge', { templateUrl: '/KnowledgeManagement/TrayApprover' })
        .when('/TrayLessonsLearned', { templateUrl: '/KnowledgeManagement/TrayLessonsLearned' })
        .when('/KnowledgeManagement/LeccionAprendida/:id', { templateUrl: function (params) { return '/KnowledgeManagement/LeccionAprendida/' + params.id + '?var=' + Math.random() } })
        .when('/KnowledgeManagement/EditLessonLearned/:id', { templateUrl: function (params) { return '/KnowledgeManagement/EditLessonLearned/' + params.id + '?var=' + Math.random() } })
        .when('/TrayThemes', { templateUrl: '/KnowledgeManagement/TrayThemes' })
        .when('/ReportLesson', { templateUrl: '/KnowledgeManagement/ReportLessonLearned' })
        .when('/NewTopic', { templateUrl: '/KnowledgeManagement/NewTopic' })
        .when('/NewLessonLearned', { templateUrl: '/KnowledgeManagement/NewLessonLearned' })
        .when('/MassTicketLoading', { templateUrl: '/KnowledgeManagement/MassTicketLoading' })
      //Acordeón Lección aprendida
       .when('/collapseOne', { templateUrl: '/collapseOne' })
       .when('/collapseTwo', { templateUrl: '/collapseTwo' })
       .when('/collapseThree', { templateUrl: '/collapseThree' })
          //cmdb
          .when('/Cmdb', { templateUrl: '/Cmdb/Index' })
          //.when('/CmdbReporte', { templateUrl: '/Cmdb/Reporte' })
          .when('/CmdbReporte/:id', { templateUrl: function (params) { return '/Cmdb/Reporte/' + params.id + '?var=' + Math.random() } })
          .when('/cmdbTicket', { templateUrl: '/Cmdb/ReporteTicket' })
          .when('/cmdbActivo', { templateUrl: '/Cmdb/ReporteActivos' })
          .when('/cmdbPersona', { templateUrl: '/Cmdb/ReportePersonas' })
          .when('/cmdbservicioImplementacion', { templateUrl: '/Cmdb/ServicioImplementacion' })
          .when('/cmdbImpOP', { templateUrl: '/Cmdb/RptImplementacionOP' })
          .when('/cmdbImpActivo', { templateUrl: '/Cmdb/RptImplementacionActivo' })
          .when('/cmdbImpPersona', { templateUrl: '/Cmdb/RptImplementacionPersona' })
          .when('/cmdbImpCambio', { templateUrl: '/Cmdb/RptImplementacionCambio' })
          .when('/cmdbImpConocimiento', { templateUrl: '/Cmdb/RptImplementacionConocimiento' })
          .when('/cmdbservicioServiceDesk', { templateUrl: '/Cmdb/ServicioServiceDesk' })
          .when('/cmdbservicioNOC', { templateUrl: '/Cmdb/ServicioNOC' })
          .when('/cmdbservicioOutsourcing/:id', { templateUrl: function (params) { return '/Cmdb/ServicioOutsourcing/' + params.id } })
      /*Gestión Documentaria*/
          .when('/GestionDocumentaria/GestionDocumentos', { templateUrl: '/GestionDocumentaria/GestionDocumentos' })
      //Error
          .when('/NecesitaPermisos', { templateUrl: '/Error/Index' })
  }]);

var phonecatControllers = angular.module('phonecatControllers', []);

phonecatControllers.controller('PhoneListCtrl', ['$scope', '$http',
  function ($scope, $http) {
      //$http.get('phones/phones.json').success(function (data) {
      //    $scope.phones = data;
      //});

      //$scope.orderProp = 'age';
      //console.log("Funcionaaaaaaaaaaa");
  }]);

phonecatControllers.controller('PhoneDetailCtrl', ['$scope', '$routeParams', '$templateCache',
  function ($scope, $routeParams, $templateCache) {
      //$scope.phoneId = Math.random();//$routeParams.phoneId;
      //console.log($scope);
      //console.log($routeParams);
      //console.log($templateCache);
      //$templateCache.remove();
      //console.log($templateCache);
  }]);


angular.module('ui.bootstrap.demo', ['ui.bootstrap', 'ngSanitize', 'ngResource', 'ui.select']);
angular.module('ui.bootstrap.demo').controller('AccordionDemoCtrl', function ($scope) {
    $scope.oneAtATime = true;

    $scope.groups = [
      {
          title: 'Dynamic Group Header - 1',
          content: 'Dynamic Group Body - 1'
      },
      {
          title: 'Dynamic Group Header - 2',
          content: 'Dynamic Group Body - 2'
      }
    ];

    $scope.items = ['Item 1', 'Item 2', 'Item 3'];

    $scope.addItem = function () {
        var newItemNo = $scope.items.length + 1;
        $scope.items.push('Item ' + newItemNo);
    };

    $scope.status = {
        isFirstOpen: true,
        isFirstDisabled: false
    };
});

angular.module('ui.bootstrap.demo').controller('DropdownCtrl', function ($scope, $log) {
    $scope.items = [
      'Shared!',
      'Send To Validation',
      'Validation',
      'Delete'
    ];

    $scope.status = {
        isopen: false
    };

    $scope.toggled = function (open) {
        $log.log('Dropdown is now: ', open);
    };

    $scope.toggleDropdown = function ($event) {
        $event.preventDefault();
        $event.stopPropagation();
        $scope.status.isopen = !$scope.status.isopen;
    };
});

angular.module('ui.bootstrap.demo').controller('PaginationDemoCtrl', function ($scope, $log, $http) {
    $scope.itemsPerPage = 6;//Items por pagina
    //$scope.bigTotalItems = 5;//Total de items
    $scope.maxSize = 10;//Numero de botones en pager
    //$scope.totalItems = 1000;
    $scope.currentPage = 1;

    $scope.url = 'Inbox';


    $scope.bigCurrentPage = 1;

    $scope.setPage = function (pageNo) {
        $scope.currentPage = pageNo;
        console.log(pageNo);
    };

    $scope.init = function (id) {
        //$http.get('../../Ticket/Unique/' + id)
        $http.get('/DocumentControl/Inbox?take=' + $scope.currentPage * $scope.itemsPerPage + '&skip=0')
        .success(function (data) {
            $scope.datos = data.Data;
            //$scope.detalle = data.Details;
            $scope.bigTotalItems = data.Count;
            //$scope.totalItems = data.Count;
        });
    };

    $scope.pageChanged = function () {
        //$log.log('Page changed to: ' + $scope.currentPage);

        $http.get('/DocumentControl/' + $scope.url + '?take=' + $scope.currentPage * $scope.itemsPerPage + '&skip=' + ($scope.currentPage - 1) * $scope.itemsPerPage)
        .success(function (data) {
            $scope.datos = data.Data;
            //$scope.detalle = data.Details;
            $scope.bigTotalItems = data.Count;
        });

    };

    $scope.menu = function (url) {
        //console.log("ok");
        $scope.url = url;
        $scope.currentPage = 1;
        $scope.pageChanged();
    }

    $scope.work = function (id) {
        $http.get('/DocumentControlDetail/Work/' + id + '?take=' + $scope.currentPage * $scope.itemsPerPage + '&skip=' + ($scope.currentPage - 1) * $scope.itemsPerPage)
        .success(function (data) {
            $scope.detalles = data.Data;
        });

        $http.get('/DocumentControlDetail/Acces/' + id + '?take=' + $scope.currentPage * $scope.itemsPerPage + '&skip=' + ($scope.currentPage - 1) * $scope.itemsPerPage)
        .success(function (data) {
            $scope.compartidos = data.Data;
        });

        $http.get('/DocumentControlDetail/ByDC/' + id + '?take=' + $scope.currentPage * $scope.itemsPerPage + '&skip=' + ($scope.currentPage - 1) * $scope.itemsPerPage)
        .success(function (data) {
            $scope.files = data.Data;
        });
    }
    $scope.downloadFile = function (id) {
        window.open("/DocumentControl/Download/" + id);
    }
    $scope.confirmReception = function (id) {
        var r = confirm("Confirma que ha recibido el documento fisico");
        if (r == true) {
            $.ajax({
                url: "/DocumentControl/Confirm/" + id,
                cache: false,
            }).done(function (msg) {
                if (msg == "OK") {
                    alert("Se Confirmo la Recepcion en Fisico");
                    //gridInbox();
                }
                else {
                    alert("Contacte su Administrador");
                }
            })
            .fail(function () {
                alert("Error");
                //gridInbox();
            });
        }
    }

});

angular.module('ui.bootstrap.demo').controller('ModalDemoCtrl', function ($scope, $http, $modal, $log) {

    $scope.items = ['item1', 'item2', 'item3'];

    $scope.title = 'Uno';

    $scope.open = function (size, tag) {
        var modalInstance = $modal.open({
            templateUrl: 'myModalContent.html',
            controller: 'ModalInstanceCtrl',
            size: size,
            resolve: {
                items: function () {
                    return [size, tag];
                }
            }
        });

        modalInstance.result.then(function (selectedItem) { $scope.selected = selectedItem; },
            function () {
                $log.info('Modal dismissed at: ' + new Date());
            });
    };

    $scope.openDC = function (size, tag) {
        var modalInstance = $modal.open({
            templateUrl: 'myModalAlertDC.html',
            controller: 'ModalInstanceCtrl',
            size: size,
            resolve: {
                items: function () {
                    return [size, tag];
                }
            }
        });

        modalInstance.result.then(function (selectedItem) { $scope.selected = selectedItem; },
            function () {
                $log.info('Modal dismissed at: ' + new Date());
            });
    };
});

// Please note that $modalInstance represents a modal window (instance) dependency.
// It is not the same as the $modal service used above.
angular.module('ui.bootstrap.demo').controller('ModalInstanceCtrl', function ($scope, $http, $modalInstance, items) {
    console.log(items[0]);
    $scope.titles = ['Validate'];
    $scope.items = items;
    $scope.counter = 0;
    $scope.selected = {
        item: $scope.items[0]
    };
    var url = "";
    $scope.title = 'Dos';

    $scope.someFunction = function (item, model) {
        $scope.counter++;
        $scope.eventResult = { item: item };
        console.log($scope.eventResult);
    };

    switch (items[1]) {
        case -1:
            $scope.titles = ['Delete Document'];
            $scope.textos = ['Desea Eliminar el Documento'];
            $scope.Buttons = ['Delete'];
            url = '/DocumentControl/Delete';
            break;
        case 0:
            $scope.titles = ['Validate Document'];
            $scope.textos = ['Seguro que el Documento es Conforme'];
            $scope.Buttons = ['Validate'];
            url = '/DocumentControl/Validate';
            break;
        case 1:
            $scope.titles = ['Shared Document'];
            $scope.Buttons = ['Shared'];
            url = '/DocumentControl/Shared';
            $http.get('/DocumentControl/Reciever').then(function (response) {
                $scope.addresses = response.data.Data;
            });
            break;
        case 2:
            $scope.titles = ['Send To Validate Document'];
            $scope.Buttons = ['Send'];
            url = '/DocumentControl/SendValidate';
            $http.get('/DocumentControl/Reciever').then(function (response) {
                $scope.addresses = response.data.Data;
            });
            break;

    }


    $scope.ok = function (id) {
        if ($scope.counter > 0) {
            //console.log($scope.eventResult.item.ID_PERS_ENTI);

            $http.post(url + '?id=' + $scope.eventResult.item.ID_PERS_ENTI + '&id_d=' + items[0]).
              success(function (data, status, headers, config) {
                  // this callback will be called asynchronously
                  // when the response is available
                  //console.log(data);
                  //console.log(status);
                  //console.log(headers);
                  //console.log(config);
                  if (data = "OK") {

                  }
                  else {
                      alert();
                  }

                  $modalInstance.close($scope.selected.item);
              }).
              error(function (data, status, headers, config) {
                  // called asynchronously if an error occurs
                  // or server returns response with an error status.
              });


        }

    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
});

//Request Vacation
angular.module('ui.bootstrap.demo').controller('PagerRequestVacation', function ($scope, $log, $http) {
    $scope.itemsPerPage = 6;//Items por pagina
    //$scope.bigTotalItems = 5;//Total de items
    $scope.maxSize = 10;//Numero de botones en pager
    //$scope.totalItems = 1000;
    $scope.currentPage = 1;

    $scope.url = 'Inbox';


    $scope.bigCurrentPage = 1;

    $scope.setPage = function (pageNo) {
        $scope.currentPage = pageNo;
        console.log(pageNo);
    };

    $scope.init = function (id) {
        //$http.get('../../Ticket/Unique/' + id)
        $http.get('/Vacation/ListRequest?take=' + $scope.currentPage * $scope.itemsPerPage + '&skip=0')
        .success(function (data) {
            $scope.datos = data.Data;
            //$scope.detalle = data.Details;
            $scope.bigTotalItems = data.Count;
            //$scope.totalItems = data.Count;
        });
        //alert("yeah")
    };

    $scope.pageChanged = function () {
        //$log.log('Page changed to: ' + $scope.currentPage);

        $http.get('/DocumentControl/' + $scope.url + '?take=' + $scope.currentPage * $scope.itemsPerPage + '&skip=' + ($scope.currentPage - 1) * $scope.itemsPerPage)
        .success(function (data) {
            $scope.datos = data.Data;
            //$scope.detalle = data.Details;
            $scope.bigTotalItems = data.Count;
        });

    };

    $scope.menu = function (url) {
        //console.log("ok");
        $scope.url = url;
        $scope.currentPage = 1;
        $scope.pageChanged();
    }

    $scope.work = function (id) {
        $http.get('/DocumentControlDetail/Work/' + id + '?take=' + $scope.currentPage * $scope.itemsPerPage + '&skip=' + ($scope.currentPage - 1) * $scope.itemsPerPage)
        .success(function (data) {
            $scope.detalles = data.Data;
        });

        $http.get('/DocumentControlDetail/Acces/' + id + '?take=' + $scope.currentPage * $scope.itemsPerPage + '&skip=' + ($scope.currentPage - 1) * $scope.itemsPerPage)
        .success(function (data) {
            $scope.compartidos = data.Data;
        });

        $http.get('/DocumentControlDetail/ByDC/' + id + '?take=' + $scope.currentPage * $scope.itemsPerPage + '&skip=' + ($scope.currentPage - 1) * $scope.itemsPerPage)
        .success(function (data) {
            $scope.files = data.Data;
        });
    }
    $scope.downloadFile = function (id) {
        window.open("/DocumentControl/Download/" + id);
    }
    $scope.confirmReception = function (id) {
        var r = confirm("Confirma que ha recibido el documento fisico");
        if (r == true) {
            $.ajax({
                url: "/DocumentControl/Confirm/" + id,
                cache: false,
            }).done(function (msg) {
                if (msg == "OK") {
                    alert("Se Confirmo la Recepcion en Fisico");
                    //gridInbox();
                }
                else {
                    alert("Contacte su Administrador");
                }
            })
            .fail(function () {
                alert("Error");
                //gridInbox();
            });
        }
    }

});

//angular.module('ui.bootstrap.demo').controller('ModalInstanceCtrl', function ($scope, $modalInstance, items) {
//angular.module('ui.bootstrap.demo').controller('DemoCtrl', function($scope, $http, $timeout, $interval) {
//    $scope.address = {};
//    $scope.refreshAddresses = function (address) {
//        var params = { address: address, sensor: false };
//        return $http.get(
//          '/DocumentControl/RecieverFilter',
//          { params: params }
//        ).then(function (response) {
//            //console.log(response.data.Data);
//            $scope.addresses = response.data.Data;
//        });
//    };
//});

////Define an angular module for our app
//var sampleApp = angular.module('sampleApp', []);

////Define Routing for app
////Uri /AddNewOrder -> template add_order.html and Controller AddOrderController
////Uri /ShowOrders -> template show_orders.html and Controller AddOrderController
//sampleApp.config(['$routeProvider',
//  function ($routeProvider) {
//      $routeProvider.
//        when('/AddNewOrder', {
//            url:'/DocumentControl',
//            templateUrl: '/DocumentControl/Index?var='+ Math.random()//,
//            //controller: 'AddOrderController'
//        }).
//        when('/ShowOrders', {
//            templateUrl: '/DocumentControl/Index?var=' + Math.random(),
//            controller: 'ShowOrdersController'
//        }).
//        otherwise({
//            redirectTo: '/AddNewOrder'
//        });
//  }]);

//sampleApp.controller("AnotherCtrlX", function ($scope) {
//    //console.log("ok");
//    $scope.name = "/Navigator/IncidentRequest?_=0";
//})

//sampleApp.controller('AddOrderController', function ($scope) {

//    $scope.message = 'This is Add new order screen';

//});


//sampleApp.controller('ShowOrdersController', function ($scope) {

//    $scope.message = 'This is Show orders screen';

//});


//var module = angular.module('App', []);

////module.config(['$routeProvider',
////  function ($routeProvider) {
////      $routeProvider.
////        when('/AddNewOrder', {
////            url:'/DocumentControl',
////            templateUrl: '/DocumentControl/Index?var='+ Math.random()//,
////            //controller: 'AddOrderController'
////        }).
////        when('/ShowOrders', {
////            templateUrl: '/DocumentControl/Index?var=' + Math.random(),
////            controller: 'ShowOrdersController'
////        }).
////        otherwise({
////            redirectTo: '/AddNewOrder'
////        });
////  }]);

//module

//    //.config(['$routeProvider', function ($routeProvider) {
//    //    $routeProvider.
//    //  when('/AddNewOrder', {
//    //      url: '/DocumentControl',
//    //      templateUrl: '/DocumentControl/Index?var=' + Math.random()//,
//    //      //controller: 'AddOrderController'
//    //  }).
//    //  when('/ShowOrders', {
//    //      templateUrl: '/DocumentControl/Index?var=' + Math.random(),
//    //      controller: 'ShowOrdersController'
//    //  }).
//    //  otherwise({
//    //      redirectTo: '/AddNewOrder'
//    //  });
//    //}])
//    .controller("ProjectCtrl", function ($scope) {
//    $scope.types = [
//            { url: "/DocumentControl/Index?var=" + Math.random(), text: "url1" },
//            { text: "someurl2", url: "someurl2.html" }
//    ];

//    $scope.selectedType = $scope.types[0];

//    //function exis() {
//    //    return true;
//    //}

//})
//.controller("AnotherCtrl", function ($scope) {
//    //console.log("ok");
//    $scope.name = "/Navigator/IncidentRequest?_=0";
//})
//.controller("AnotherCtrl2", function ($scope) {
//    console.log("ok2");
//    $scope.name = "/DocumentControlX/Index?_=0"; 
//    $scope.click = function () {
//        console.log("Click Akiii");
//        $scope.namex = "/DocumentControl/Index?_=0";
//    }
//});



////(function () {
////    "use strict";

////    angular.module('Bookmarks', [
////        //dependencies here
////    ])

////    .controller('MainController', function ($scope) { //Step 1
////        $scope.name = 'Carl'                       //Step 2

////    });

////})();

////var module = angular.module('App', []);

////module.controller("ProjectCtrl", function ($scope) {
////    $scope.types = [
////            { url: "/DocumentControl/CreateReception?var=" + Math.random(), text: "url1" },
////            { text: "someurl2", url: "/DocumentControl/CreateReception?var=" + Math.random() }
////    ];

////    $scope.selectedType = $scope.types[0];

////})
////.controller("AnotherCtrl", function ($scope) {
////    $scope.name = "World";
////});

////(function (angular) {
////    'use strict';
////    angular.module('includeExample', ['ngAnimate'])
////      .controller('ExampleController', ['$scope', function ($scope) {
////          $scope.templates =
////            [{ name: 'template1.html', url: 'template1.html' },
////              { name: 'template2.html', url: 'template2.html' }];
////          $scope.template = $scope.templates[0];
////      }]);
////})(window.angular);

////var app = angular.module("app", ["ngResource", "ngSanitize"]);

//////con dataResource inyectamos la factoría
////app.controller("appController", function ($scope, $http) {



////    $scope.init = function (id) {

////        //$http.get('../../Ticket/Unique/' + id)
////        $http.get('../Ticket/Unique?idt=' + encodeURIComponent(id))
////        .success(function (data) {
////            console.log("OK");
////            $scope.datos = data.Data;
////            $scope.detalle = data.Details;
////        });
////    };
////});


////angular.module('ui.bootstrap.demo', ['ui.bootstrap']);
////angular.module('ui.bootstrap.demo').controller('DropdownCtrl', function ($scope, $log) {

////    $scope.items = [
////      'The first choice!',
////      'And another choice for you.',
////      'but wait! A third!'
////    ];

////    $scope.status = {
////        isopen: false
////    };

////    $scope.toggled = function (open) {
////        console.log("OK");
////        $log.log('Dropdown is now: ', open);
////    };

////    $scope.toggleDropdown = function ($event) {
////        $event.preventDefault();
////        $event.stopPropagation();
////        $scope.status.isopen = !$scope.status.isopen;
////    };
////});

