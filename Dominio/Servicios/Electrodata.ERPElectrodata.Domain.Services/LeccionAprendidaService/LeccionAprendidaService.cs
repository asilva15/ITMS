using Electrodata.ERPElectrodata.Domain.Entities;
using Electrodata.ERPElectrodata.Domain.Entities.EntityAlternative;
using Electrodata.ERPElectrodata.Domain.Entities.Enum;
using Electrodata.ERPElectrodata.Infra.Reposotories.LeccionAprendidaRepositorie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Electrodata.ERPElectrodata.Domain.Services.LeccionAprendidaService
{

    public class LeccionAprendidaService
    {
        LeccionAprendidaRepositorie _repLeccionAprendidaRepositorie;

        public LeccionAprendidaService(LeccionAprendidaRepositorie repLeccionAprendidaRepositorie)
        {
            this._repLeccionAprendidaRepositorie = repLeccionAprendidaRepositorie;
        }

        public int OperationLeccionAprendidaBusiness(ComLeccionAprendida objLeccionAprendida, int tipoOperacion, string KeyAtta, int IdAcco, string Usuario, int idPerfilAprobador, int idUsuario)
        {
            int result = (int)Tipo_Peracion.OPERATION_ERROR;
            if (tipoOperacion == (int)Tipo_Peracion.OPERATION_SAVE)
            {
                objLeccionAprendida.ID_ACCO = IdAcco;
                objLeccionAprendida.VigLeccionAprendida = true;
                objLeccionAprendida.NroRevisiones = idPerfilAprobador == 1 ? (objLeccionAprendida.NroRevisiones == null ? 0 : objLeccionAprendida.NroRevisiones + 1) : 0;
                objLeccionAprendida.EstadoAprobacion = idPerfilAprobador == 1 ? (string.IsNullOrEmpty(objLeccionAprendida.EstadoAprobacion) ? "P" : objLeccionAprendida.EstadoAprobacion) : "P";
                if (idPerfilAprobador == 1)
                    objLeccionAprendida.ColorEstado = objLeccionAprendida.EstadoAprobacion == "P" ? "#679700" : objLeccionAprendida.EstadoAprobacion == "A" ? "#FFBB00" : objLeccionAprendida.EstadoAprobacion == "D" ? "#FA5858" : "#679700";
                else
                    objLeccionAprendida.ColorEstado = "#679700";
                objLeccionAprendida.Usuario = Usuario;
                objLeccionAprendida.idUsuario = idUsuario;
                objLeccionAprendida.Aprobador = idPerfilAprobador == 1 ? idUsuario : 0;
                objLeccionAprendida.DateStart = DateTime.Now;
                objLeccionAprendida.IdTema = objLeccionAprendida.IdTema == null ? 0 : objLeccionAprendida.IdTema;
                objLeccionAprendida.ID_TICKET = objLeccionAprendida.ID_TICKET == null ? 0 : objLeccionAprendida.ID_TICKET;
                result = _repLeccionAprendidaRepositorie.SaveLeccionAprendida(objLeccionAprendida, KeyAtta, idPerfilAprobador);
            }
            else
            {
                var existe = _repLeccionAprendidaRepositorie.ObtenerLeccionAprendidaByCodigo(objLeccionAprendida.IdLeccioNAprendida);
                if (existe != null)
                {
                    existe.Titulo = objLeccionAprendida.Titulo;
                    existe.Nivel1 = objLeccionAprendida.Nivel1;
                    existe.Nvel2 = objLeccionAprendida.Nvel2;
                    existe.Nivel3 = objLeccionAprendida.Nivel3;
                    existe.Nivel4 = objLeccionAprendida.Nivel4;
                    existe.IdTema = objLeccionAprendida.IdTema;
                    existe.NroRevisiones = idPerfilAprobador == 1 ? (existe.NroRevisiones == null ? 0 : existe.NroRevisiones + 1) : 0;
                    existe.Puntuacion = idPerfilAprobador == 1 ? objLeccionAprendida.Puntuacion : "0";
                    existe.Impactonegocio = objLeccionAprendida.Impactonegocio;
                    existe.SolucionTemporal = objLeccionAprendida.SolucionTemporal;
                    existe.SolucionDefinitiva = objLeccionAprendida.SolucionDefinitiva;
                    existe.DescripcionProblema = objLeccionAprendida.DescripcionProblema;
                    existe.Porque2 = objLeccionAprendida.Porque2;
                    existe.Porque3 = objLeccionAprendida.Porque3;
                    existe.Porque4 = objLeccionAprendida.Porque4;
                    existe.Porque5 = objLeccionAprendida.Porque5;
                    existe.Aprobador = idPerfilAprobador == 1 ? idUsuario : 0;
                    existe.EstadoAprobacion = idPerfilAprobador == 1 ? (string.IsNullOrEmpty(objLeccionAprendida.EstadoAprobacion) ? "P" : objLeccionAprendida.EstadoAprobacion) : existe.EstadoAprobacion;
                    if (idPerfilAprobador == 1)
                        existe.ColorEstado = objLeccionAprendida.EstadoAprobacion == "P" ? "#679700" : objLeccionAprendida.EstadoAprobacion == "A" ? "#FFBB00" : objLeccionAprendida.EstadoAprobacion == "D" ? "#FA5858" : "#679700";
                    else
                        objLeccionAprendida.ColorEstado = "#679700";
                    existe.ID_ACCO = IdAcco;
                    existe.DateUpdate = DateTime.Now;
                    existe.UsuarioUpdate = Usuario;
                    existe.idUsuarioUpdate = idUsuario;
                    result = _repLeccionAprendidaRepositorie.UpdateLeccionAprendida(existe, KeyAtta, idPerfilAprobador, idUsuario, IdAcco, Usuario);
                }

            }
            return result;
        }


        /*gestión Problemas*/
        public int OperationLeccionAprendidaBusinessProblemas(ComLeccionAprendida objLeccionAprendida, int tipoOperacion, string KeyAtta, int IdAcco, string Usuario, int idPerfilAprobador, int idUsuario)
        {
            int result = (int)Tipo_Peracion.OPERATION_ERROR;
            if (tipoOperacion == (int)Tipo_Peracion.OPERATION_SAVE)
            {
                objLeccionAprendida.ID_ACCO = IdAcco;
                objLeccionAprendida.VigLeccionAprendida = true;
                objLeccionAprendida.NroRevisiones = idPerfilAprobador == 1 ? (objLeccionAprendida.NroRevisiones == null ? 0 : objLeccionAprendida.NroRevisiones + 1) : 0;
                objLeccionAprendida.Puntuacion = "5";
                objLeccionAprendida.EstadoAprobacion = "A";
                objLeccionAprendida.ColorEstado = "#FFBB00";
                objLeccionAprendida.Usuario = Usuario;
                objLeccionAprendida.idUsuario = idUsuario;
                objLeccionAprendida.Aprobador = idPerfilAprobador == 1 ? idUsuario : 0;
                objLeccionAprendida.DateStart = DateTime.Now;
                objLeccionAprendida.IdTema = objLeccionAprendida.IdTema == null ? 0 : objLeccionAprendida.IdTema;
                objLeccionAprendida.ID_TICKET = objLeccionAprendida.ID_TICKET == null ? 0 : objLeccionAprendida.ID_TICKET;
                result = _repLeccionAprendidaRepositorie.SaveLeccionAprendida(objLeccionAprendida, KeyAtta, idPerfilAprobador);
            }
            else
            {
                var existe = _repLeccionAprendidaRepositorie.ObtenerLeccionAprendidaByCodigo(objLeccionAprendida.IdLeccioNAprendida);
                if (existe != null)
                {
                    existe.Titulo = objLeccionAprendida.Titulo;
                    existe.Nivel1 = objLeccionAprendida.Nivel1;
                    existe.Nvel2 = objLeccionAprendida.Nvel2;
                    existe.Nivel3 = objLeccionAprendida.Nivel3;
                    existe.Nivel4 = objLeccionAprendida.Nivel4;
                    existe.IdTema = objLeccionAprendida.IdTema;
                    existe.NroRevisiones = idPerfilAprobador == 1 ? (existe.NroRevisiones == null ? 0 : existe.NroRevisiones + 1) : 0;
                    existe.Puntuacion = "5";
                    existe.DescripcionProblema = objLeccionAprendida.DescripcionProblema;
                    existe.SolucionAplicada = objLeccionAprendida.SolucionAplicada;
                    existe.Impactonegocio = objLeccionAprendida.Impactonegocio;
                    existe.SolucionTemporal = objLeccionAprendida.SolucionTemporal;
                    existe.SolucionDefinitiva = objLeccionAprendida.SolucionDefinitiva;
                    existe.Porque2 = objLeccionAprendida.Porque2;
                    existe.Porque3 = objLeccionAprendida.Porque3;
                    existe.Porque4 = objLeccionAprendida.Porque4;
                    existe.Porque5 = objLeccionAprendida.Porque5;
                    existe.Aprobador = idPerfilAprobador == 1 ? idUsuario : 0;
                    existe.EstadoAprobacion = "A";
                    existe.ColorEstado = "#FFBB00"; ;
                    existe.ID_ACCO = IdAcco;
                    existe.DateUpdate = DateTime.Now;
                    existe.UsuarioUpdate = Usuario;
                    existe.idUsuarioUpdate = idUsuario;
                    result = _repLeccionAprendidaRepositorie.UpdateLeccionAprendida(existe, KeyAtta, idPerfilAprobador, idUsuario, IdAcco, Usuario);
                }

            }
            return result;
        }

        /*Fin*/



        public List<ComTrayLessonsLearned_Result> SearchInitialLessonLearned(int Nivel1, int Nvel2, int Nivel3, int Nivel4, int IdTema, string FechaCreacionFin, string FechaCreacionInicio, int NroRevisiones, string PalabraClave, string estadoAprobacion, int Puntuacion, int TipoTicket)
        {
            return _repLeccionAprendidaRepositorie.SearchInitialLessonLearned(Nivel1, Nvel2, Nivel3, Nivel4, IdTema, FechaCreacionFin, FechaCreacionInicio, NroRevisiones, PalabraClave, estadoAprobacion, Puntuacion, TipoTicket);
        }

        public ComTrayLessonsLearned_Result FirstLessonLearned(int id)
        {
            return _repLeccionAprendidaRepositorie.FirstLessonLearned(id);
        }

        public List<AttachedEntity> listaAdjuntos(int id)
        {
            return _repLeccionAprendidaRepositorie.listaAdjuntos(id);
        }

        public List<ComListLessonsLearnedTickets_Result> ListLeccionesAprendidasTickets(int idTicket, int consulta)
        {
            return _repLeccionAprendidaRepositorie.ListLeccionesAprendidasTickets(idTicket, consulta);
        }

        public ComDetailLearnedLesson_Result DetalleModalLeccionAprendida(int id)
        {
            return _repLeccionAprendidaRepositorie.DetalleModalLeccionAprendida(id);
        }

        public List<ComLessonLearnedActivity_Result> listaActividades(int idLeccionAprendida, int idCuenta)
        {
            return _repLeccionAprendidaRepositorie.listaActividades(idLeccionAprendida, idCuenta);
        }

 
    }
}
