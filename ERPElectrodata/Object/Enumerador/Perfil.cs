using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPElectrodata.Object.Enumerador
{
    public class Perfiles
    {
        public enum ROL
        {
            ADMINISTRADOR_SISTEMA = 1,
            ADMINISTRADOR = 2,
            SUPERVISOR_SERVICEDESK = 3,
            SUPERVISOR_OPERACIONES = 4,
            SUPERVISOR_PMO = 5,
            SUPERVISOR_SOPORTE = 6,
            SUPERVISOR_INVENTARIO = 7,
            SUPERVISOR_DOCUMENTOS = 8,
            SUPERVISOR_CONTROLDOCUMENTARIO = 9,
            SUPERVISOR_EJECUTIVOCOMERCIAL = 10,
            SUPERVISOR_RRHH = 11,
            SUPERVISOR_FINANZAS = 12,
            SERVICEDESK = 13,
            OPERACIONES = 14,
            PROJECTMANAGER = 15,
            SOPORTE = 16,
            INVENTARIO = 17,
            CONTROLDOCUMENTARIO = 18,
            EJECUTIVOCOMERCIAL = 19,
            RRHH = 20,
            FINANZAS = 21,
            USUARIO_PMO = 22,
            AUDITOR_TICKET = 23,
            AUDITOR_ACTIVIDADES = 24,
            AUDITOR_PMO = 25,
            AUDITOR_ACTIVO = 26,
            AUDITOR_INVENTARIO = 27,
            AUDITOR_DOCUMENTOS = 28,
            AUDITOR_CONTROLDOCUMENTOS = 29,
            AUDITOR_EJECUTIVOCOMERCIAL = 30,
            AUDITOR_TALENTO = 31,
            AUDITOR_FINANZAS = 32,
            /*Agregado para el módulo gestión del conocimiento*/
            APROBADOR_OPERACIONES = 34,
            /*Fin*/
            SISTEMA_INTEGRADO_GESTION = 35,
            CMDB = 36,
            PERMISO_LICENCIAS = 37,
            SUPERVISOR_OUTSOURCING = 38,
            RENOVACIONES = 39,
            SUPERVISOR_PMO_OUTSOURCING = 40,
            PROJECTMANAGER_OUTSOURCING = 41,
            VISUALIZA_LOGROS = 42,
            DESCARGAR_LOGROS = 43,
            USUARIO_EXTERNO = 44,
            PMO_GERENCIA = 45,
            PMO_GERENCIA_ITO = 46,
            ADMINPORTAL = 47,
            APROBAR_SELECCION_PERSONAL = 48,
            USUARIO_EXTERNO_TICKET = 49,
            PMO_SOPORTE = 50,
            RENOVACIONES_OP = 51,
            SUPERVISOR_INFORME = 52,
            INFORME = 53,
            EJECUTIVOCOMERCIAL_OUTSOURCING = 54,
            COORDINADOR_SERVICEDESK_BNV = 55,
            USUARIO_EXTERNO_TICKETS_ACTIVOS = 56,
            COORDINADOR_SERVICEDESK_MINSUR=57,
            EJECUTOR_TAREAS_MINSUR_MARCOBRE_RAURA = 58,
            GESTOR_TAREAS_MINSUR_MARCOBRE_RAURA = 59,
            SUPERVISOR_GESTOR_ACTIVOS = 60,
            GESTOR_ACTIVOS_MINSUR_MARCOBRE_RAURA=61,
            SUPERVISOR_PMO_LISTADO=62,
            GESTOR_DOCUMENTOS_ACTIVOS = 63,
            USUARIO_NOTIFICACIONES = 64,
            GESTOR_ACTIVOS_BNV_MICROINFORMATICO = 65,
            SUPERVISOR_ACTIVOS_BNV_MICROINFORMATICO = 66,
            GESTOR_ACTIVOS_BNV_INFRAESTRUCTURA = 67,
            SUPERVISOR_ACTIVOS_BNV_INFRAESTRUCTURA = 68,
            SUPERVISOR_PERMISO_LICENCIAS = 69,
            GESTOR_ACTIVOS_BNV_MOVIL = 70,
            SUPERVISOR_ACTIVOS_BNV_MOVIL = 71,
            //CUENTAS Y PERMISOS
            SUPERVISOR_SERVICEDESK_ELECTRODATA = 72,
            SUPERVISOR_SERVICEDESK_CUENTA = 73,
            GESTOR_ACTIVOS_HUDBAY_OT = 74,
            GESTOR_ACTIVOS_HUDBAY_IT = 75,
            ADMINISTRADOR_CATEGORIA = 76,
            CERTIFICADOS_VISUALIZACION_COMPLETA = 77,
            CERTIFICADOS_VISUALIZACION_PARCIAL = 78,
            CERTIFICADOS_VISUALIZACION_ORGANIGRAMA = 79
        }
    }
}
