using Electrodata.ERPElectrodata.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;
using Electrodata.ERPElectrodata.Domain.Entities.Enum;
using Electrodata.ERPElectrodata.Domain.Entities.EntityAlternative;

namespace Electrodata.ERPElectrodata.Infra.Reposotories.LeccionAprendidaRepositorie
{
   public class LeccionAprendidaRepositorie
    {
       public ComLeccionAprendida ObtenerLeccionAprendidaByCodigo(int IdLeccioNAprendida)
        {
            var objeto = (dynamic)null;
            using (var db = new ECoreEntities())
            {
                objeto = db.ComLeccionAprendidas.Where(x => x.IdLeccioNAprendida == IdLeccioNAprendida).FirstOrDefault();
            }
            return objeto;
        }

       public int SaveLeccionAprendida(ComLeccionAprendida objLeccionAprendida, string KeyAtta, int idPerfilAprobador)
       {
           int result = (int)Tipo_Peracion.OPERATION_ERROR;
           using (var db = new ECoreEntities())
           {
               using (var dbContextTransaction=db.Database.BeginTransaction())
               {
                   try
                   {
                         db.ComLeccionAprendidas.Add(objLeccionAprendida);
                        db.SaveChanges();
                         int id = Convert.ToInt32(objLeccionAprendida.IdLeccioNAprendida);

                       if (KeyAtta != null)
                       {
                           var Attachs = db.ATTACHEDs.Where(x => x.KEY_ATTA == KeyAtta).Where(x => x.ID_INCI == null).ToList();
                           if (Attachs.Count() > 0)
                           {
                               foreach (ATTACHED attach in Attachs)
                               {
                                       var EObj = db.ATTACHEDs.Single(x => x.ID_ATTA == attach.ID_ATTA);
                                       EObj.IdLeccionAprendida = id;
                                       db.Entry(EObj).State = System.Data.Entity.EntityState.Modified;
                                       db.SaveChanges();
                                       //db.Entry(EObj).State = System.Data.Entity.EntityState.Detached;
                               }
                           }
                       }
                       /*Registro el log de lecciones aprendidas*/
                       var logLeccionAprendida = new ComLogLeccionesAprendida()
                       {
                           idLeccioNAprendida = id,
                           Título = objLeccionAprendida.Titulo,
                           idTema = objLeccionAprendida.IdTema,
                           Revision=objLeccionAprendida.NroRevisiones,
                           Puntuacion=idPerfilAprobador==1? Convert.ToInt32(objLeccionAprendida.Puntuacion):0,
                           EstadoAprobacion = objLeccionAprendida.EstadoAprobacion,
                           Aprobador=idPerfilAprobador==1?1:0,
                           ColorEstado = objLeccionAprendida.ColorEstado,
                           accion = "insert",
                           declarado = idPerfilAprobador == 1 ? idPerfilAprobador : 0,
                           usuarioCreacion = objLeccionAprendida.Usuario,
                           idusuario=objLeccionAprendida.idUsuario,
                           FechaCreacion = objLeccionAprendida.DateStart
                       };

                       db.ComLogLeccionesAprendidas.Add(logLeccionAprendida);
                       db.SaveChanges();
                       /*Fin*/

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

       public int UpdateLeccionAprendida(ComLeccionAprendida objLeccionAprendidaCargada, string KeyAtta, int idPerfilAprobador, int idUsuario, int IdAcco, string Usuario)
       {
           int result = (int)Tipo_Peracion.OPERATION_ERROR;
           using (var db = new ECoreEntities())
           {
               using (var dbContextTransaction = db.Database.BeginTransaction())
               {
                   try
                   {
                       db.Entry(objLeccionAprendidaCargada).State = System.Data.Entity.EntityState.Modified;
                       db.SaveChanges();
                       db.Entry(objLeccionAprendidaCargada).State = System.Data.Entity.EntityState.Detached;

                       int id = Convert.ToInt32(objLeccionAprendidaCargada.IdLeccioNAprendida);

                       if (KeyAtta != null)
                       {
                           var Attachs = db.ATTACHEDs.Where(x => x.KEY_ATTA == KeyAtta).Where(x => x.ID_INCI == null).ToList();
                           if (Attachs.Count() > 0)
                           {
                               foreach (ATTACHED attach in Attachs)
                               {
                                   var EObj = db.ATTACHEDs.Single(x => x.ID_ATTA == attach.ID_ATTA);
                                   EObj.IdLeccionAprendida = id;
                                   db.Entry(EObj).State = System.Data.Entity.EntityState.Modified;
                                   db.SaveChanges();
                                   db.Entry(EObj).State = System.Data.Entity.EntityState.Detached;
                               }
                           }
                       }

                       /*Actualización en el log de lecciones aprendidas*/
                       var logLeccionAprendida = new ComLogLeccionesAprendida()
                       {
                           idLeccioNAprendida = id,
                           Título = objLeccionAprendidaCargada.Titulo,
                           idTema = objLeccionAprendidaCargada.IdTema,
                           Revision = objLeccionAprendidaCargada.NroRevisiones,
                           Puntuacion = idPerfilAprobador == 1 ? Convert.ToInt32(objLeccionAprendidaCargada.Puntuacion) : 0,
                           EstadoAprobacion = objLeccionAprendidaCargada.EstadoAprobacion,
                           Aprobador = idPerfilAprobador == 1 ? objLeccionAprendidaCargada.Aprobador : 0,
                           ColorEstado = objLeccionAprendidaCargada.ColorEstado,
                           accion = "update",
                           declarado = idPerfilAprobador == 1 ? idPerfilAprobador : 0,
                           usuarioCreacion =objLeccionAprendidaCargada.idUsuarioUpdate!=null?objLeccionAprendidaCargada.UsuarioUpdate:objLeccionAprendidaCargada.Usuario,
                           idusuario = objLeccionAprendidaCargada.idUsuarioUpdate!=null?objLeccionAprendidaCargada.idUsuarioUpdate:objLeccionAprendidaCargada.idUsuario ,
                           FechaCreacion = DateTime.Now
                       };

                       db.ComLogLeccionesAprendidas.Add(logLeccionAprendida);
                       db.SaveChanges();
                       /*Fin*/
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


       public List<ComTrayLessonsLearned_Result> SearchInitialLessonLearned(int Nivel1_, int Nvel2_, int Nivel3_, int Nivel4_, int IdTema_, string FechaCreacionFin_, string FechaCreacionInicio_, int NroRevisiones_, string PalabraClave_, string estadoAprobacion_, int puntuacion_, int TipoTicket_)
       {
           var lista = (dynamic)null;
          using(var dbcontex=new ECoreEntities()){

              lista = dbcontex.ComTrayLessonsLearned(Nivel1_, Nvel2_, Nivel3_, Nivel4_, IdTema_, FechaCreacionInicio_, FechaCreacionFin_, NroRevisiones_, PalabraClave_, estadoAprobacion_, puntuacion_, TipoTicket_).ToList();
          }
          return lista;
       }

       public ComTrayLessonsLearned_Result FirstLessonLearned(int id)
       {
           var objLeccionAprendida = (dynamic)null;
           using (var dbcontex = new ECoreEntities())
           {
               objLeccionAprendida = dbcontex.ComTrayLessonsLearned(0, 0, 0, 0, 0, null, null, 0, null, null, 0, 0).Where(x => x.IdLeccioNAprendida == id).FirstOrDefault();
               //objLeccionAprendida=(from x in dbcontex.ComLeccionAprendidas where x.IdLeccioNAprendida==id select x).FirstOrDefault();
           }
           return objLeccionAprendida;
       }



       public List<AttachedEntity> listaAdjuntos(int id)
       {
           List<AttachedEntity> listaAdjuntos = listaAdjuntos = new List<AttachedEntity>();
           
           try
           {
              // Adjun = Adjun + "<li><a href='../../Adjuntos/" + subqu.NAM_ATTA + "_" + subqu.ID_ATTA + subqu.EXT_ATTA + " ' TARGET='_BLANK'>" + subqu.NAM_TYPE_DOCU_ATTA + " - " + subqu.NAM_ATTA + subqu.EXT_ATTA + "</a></li>";
               using(var db=new ECoreEntities()){
               var query = (from a in db.ATTACHEDs.Where(a => a.IdLeccionAprendida == id)
                            .Where(a => a.DELETE_ATTA == null || a.DELETE_ATTA == false)
                            join tda in db.TYPE_DOCUMENT_ATTACH on a.ID_TYPE_DOCU_ATTA equals tda.ID_TYPE_DOCU_ATTA into ltda
                            from xtda in ltda.DefaultIfEmpty()
                            select new
                            {
                                NAM_ATTA = a.NAM_ATTA,
                                ID_ATTA = a.ID_ATTA,
                                EXT_ATTA = a.EXT_ATTA,
                                NAM_TYPE_DOCU_ATTA = (xtda == null ? "" : xtda.NAM_TYPE_DOCU_ATTA),
                            });
              
               foreach (var subqu in query)
               {
                   AttachedEntity objAttachedEntity = new AttachedEntity();
                    objAttachedEntity.ID_ATTA = subqu.ID_ATTA;
                    objAttachedEntity.EXT_ATTA = subqu.EXT_ATTA;
                    objAttachedEntity.NAM_ATTA = subqu.NAM_ATTA;
                    objAttachedEntity.NAM_TYPE_DOCU_ATTA = subqu.NAM_TYPE_DOCU_ATTA;
                    listaAdjuntos.Add(objAttachedEntity);
               }
               }
               return listaAdjuntos;
           }
           catch
           {
               return null;
           }
       }

       public List<ComListLessonsLearnedTickets_Result> ListLeccionesAprendidasTickets(int idTicket, int consulta)
       {
           var lista = (dynamic)null;

           using(var context =new ECoreEntities()){

               lista = context.ComListLessonsLearnedTickets(idTicket, consulta).ToList();
           }
           return lista;
       }

       public ComDetailLearnedLesson_Result DetalleModalLeccionAprendida(int id)
       {
           var detalle=(dynamic)null;
            using(var context=new ECoreEntities()){

                 detalle = context.ComDetailLearnedLesson(id).FirstOrDefault();
            }
            return detalle;
       }

       public List< ComLessonLearnedActivity_Result> listaActividades(int idLeccionAprendida,int idCuenta)
       {
           var detalle = (dynamic)null;
           using (var context = new ECoreEntities())
           {

               detalle = context.ComLessonLearnedActivity(idLeccionAprendida, idCuenta).ToList();
           }
           return detalle;
       }
    }
}
