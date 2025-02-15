using Electrodata.ERPElectrodata.Domain.Entities;
using Electrodata.ERPElectrodata.Domain.Entities.Enum;
using Electrodata.ERPElectrodata.Infra.Reposotories.GestionDocumentosRepositorie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Electrodata.ERPElectrodata.Domain.Services.GestionDocumentosService
{
   public class GestionDocumentosService
    {
           GestionDocumentosRepositorie _repGestionDocumentosRepositorie;

           public GestionDocumentosService(GestionDocumentosRepositorie repGestionDocumentosRepositorie)
              {
                  this._repGestionDocumentosRepositorie = repGestionDocumentosRepositorie;
       }

           public int GuardarGestionDocumenatria(Entities.GestionDocumento objGestionDocumento, string keyAta, int idCuenta, int idUsuario)
           {
               int result = (int)Tipo_Peracion.OPERATION_ERROR;
               return result= _repGestionDocumentosRepositorie.GuardarGestionDocumenatria(objGestionDocumento, keyAta,idCuenta,idUsuario);
           }

           public List<ProcListarGestionDocumentos_Result> ListarArchivos(int idPersona, int tipoArchivo, int IdTipoDocumento, string nombreDocumento, int sesionIdCuenta)
           {
               return _repGestionDocumentosRepositorie.ListarArchivos(idPersona, tipoArchivo, IdTipoDocumento, nombreDocumento, sesionIdCuenta);
           }

           public List<ProcObtenerTipos_Result> ObtenerTipos(int idCuenta)
           {
               return _repGestionDocumentosRepositorie.ObtenerTipos(idCuenta);
           }
    }
}
