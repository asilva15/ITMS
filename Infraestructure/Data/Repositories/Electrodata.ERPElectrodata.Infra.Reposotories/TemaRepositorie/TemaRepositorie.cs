using Electrodata.ERPElectrodata.Domain.Entities;
using Electrodata.ERPElectrodata.Domain.Entities.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Electrodata.ERPElectrodata.Infra.Reposotories.TemaRepositorie
{
    public class TemaRepositorie
    {

        public List<ComFindTopics_Result> SearchInitialThemes(string Nomtema, int idCuenta, int idCategoria, string FechaCreacionInicio, string FechaCreacionFin, int Vigtema, string FechaBajaInicio, string FechaBajaFin)
        {
            var lista = (dynamic)null;
            using (var db = new ECoreEntities())
            {
                lista = db.ComFindTopics(Nomtema,idCuenta, idCategoria, FechaCreacionInicio, FechaCreacionFin, Vigtema, FechaBajaInicio, FechaBajaFin).ToList();
            }
            return lista;
        }

        public int OperationThemeBusiness(int IdTema, int IdCuenta,int IdCuentaCatTema, int IdCategoria, string Nomtema, bool VigTema, string Usuario, string DateStart, string DateEnd, int AccoUser, string DateUpdate, string UsuarioUpdate, int TypeOperation)
        {
            int rpta = (int)Tipo_Peracion.OPERATION_ERROR;
            using (var db = new ECoreEntities())
            {
                ComSaveTheme_Result retorno = db.ComSaveTheme(IdTema, IdCuenta,IdCuentaCatTema, IdCategoria, Nomtema, VigTema, Usuario, DateStart, DateEnd, AccoUser, DateUpdate, UsuarioUpdate, TypeOperation).FirstOrDefault();
                rpta = Convert.ToInt32(retorno.Column1);
            }
            return rpta;
        }

        public ComTema ObjectTheme(int iTheme)
        {
            var objeto = (dynamic)null;
            using (var db = new ECoreEntities())
            {
                objeto = db.ComTemas.ToList().Where(x => x.IdTema == iTheme).FirstOrDefault();
            }
            return objeto;
        }

        public ComCuentaCategoryTema ThemeCategoryAccountList(int IdCuentaCatTema)
        {
            var objeto = (dynamic)null;
            using (var db = new ECoreEntities())
            {
                objeto = db.ComCuentaCategoryTemas.Where(x => x.IdCuentaCategoryTema == IdCuentaCatTema).FirstOrDefault();
            }
            return objeto;
        }

        public List<ComTema> ListTemaLessonLearned(int ID_ACCO, int ID_CATE)
        {
            List<ComTema> lista = new List<ComTema>();
     
            using (var db = new ECoreEntities())
            {
              var  listaTema =db.ComTemas
                  .Join(db.ComCuentaCategoryTemas,c=>c.IdTema,ct =>ct.IdTema,(c,ct)=>new {c.IdTema, c.Nomtema,ct.ID_ACCO,ct.ID_CATE})
                  .Where(x=>x.ID_ACCO==ID_ACCO && x.ID_CATE==ID_CATE);
                
                foreach(var listaTema_ in listaTema){
                           ComTema objTema = new ComTema();
                           objTema.IdTema = listaTema_.IdTema;
                           objTema.Nomtema = listaTema_.Nomtema;
                           lista.Add(objTema);
                }

            }
            return lista;
        }
    }
}
