using Electrodata.ERPElectrodata.Domain.Entities;
using Electrodata.ERPElectrodata.Domain.Entities.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Electrodata.ERPElectrodata.Infra.Reposotories.GestionDocumentosRepositorie
{
   public class GestionDocumentosRepositorie
    {

        public int GuardarGestionDocumenatria(GestionDocumento objGestionDocumento, string keyAta, int idCuenta, int idUsuario)
        {
            int result = (int)Tipo_Peracion.OPERATION_ERROR;
            using (var db = new ECoreEntities())
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.GestionDocumentoes.Add(objGestionDocumento);
                        db.SaveChanges();
                        int id = Convert.ToInt32(objGestionDocumento.Id);

                        if (keyAta != null)
                        {
                            var Attachs = db.Adjuntoes.Where(x => x.KeyAdjunto == keyAta).Where(x => x.IdGestionDocumento == null).ToList();
                            if (Attachs.Count() > 0)
                            {
                                foreach (Adjunto attach in Attachs)
                                {
                                    var EObj = db.Adjuntoes.Single(x => x.Id == attach.Id);
                                    EObj.IdGestionDocumento = id;
                                    db.Entry(EObj).State = System.Data.Entity.EntityState.Modified;
                                    db.SaveChanges();
                                    //db.Entry(EObj).State = System.Data.Entity.EntityState.Detached;
                                }
                            }
                        }
                        dbContextTransaction.Commit();
                        result = id;
                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                    }
                }
            }
            return result;
        }

        public List<ProcListarGestionDocumentos_Result> ListarArchivos(int idPersona, int tipoArchivo, int IdTipoDocumento, string nombreDocumento, int sesionIdCuenta)
        {
            var lista = (dynamic)null;
            using (var db = new ECoreEntities())
            {
                lista = db.ProcListarGestionDocumentos(idPersona, tipoArchivo, IdTipoDocumento, nombreDocumento, sesionIdCuenta).ToList();
            }
            return lista;

        }

        public List<ProcObtenerTipos_Result> ObtenerTipos(int idCuenta)
        {
            var lista = (dynamic)null;
            using (var db = new ECoreEntities())
            {
                lista = db.ProcObtenerTipos(idCuenta).ToList();
            }
            return lista;
        }
    }
}
