using Electrodata.ERPElectrodata.Domain.Entities;
using Electrodata.ERPElectrodata.Domain.Entities.Enum;
using Electrodata.ERPElectrodata.Infra.Reposotories.TemaRepositorie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Electrodata.ERPElectrodata.Domain.Services.TemaService
{
    public class TemaService
    {
        TemaRepositorie _repTemaRepositorie;

        public TemaService(TemaRepositorie repTemaRepositorie)
        {
            this._repTemaRepositorie = repTemaRepositorie;
        }

        public List<ComFindTopics_Result> SearchInitialThemes(string Nomtema, int idCuenta, int idCategoria, string FechaCreacionInicio, string FechaCreacionFin, int Vigtema, string FechaBajaInicio, string FechaBajaFin)
        {
            return _repTemaRepositorie.SearchInitialThemes(Nomtema, idCuenta, idCategoria, FechaCreacionInicio, FechaCreacionFin, Vigtema, FechaBajaInicio, FechaBajaFin);
        }

        public int OperationThemeBusiness(int IdTema, int IdCuenta, int IdCuentaCatTema, int IdCategoria, string Nomtema, bool VigTema, string Usuario, string DateStart, string DateEnd, int AccoUser, string DateUpdate, string UsuarioUpdate, int TypeOperation)
        {
            int rpta = (int)Tipo_Peracion.OPERATION_ERROR;
            rpta = _repTemaRepositorie.OperationThemeBusiness(IdTema, IdCuenta, IdCuentaCatTema, IdCategoria, Nomtema, VigTema, Usuario, DateStart, DateEnd, AccoUser, DateUpdate, UsuarioUpdate, TypeOperation);
            return rpta;
        }

        public ComTema ObjectTheme(int iTheme)
        {
            return _repTemaRepositorie.ObjectTheme(iTheme);
        }

        public ComCuentaCategoryTema ThemeCategoryAccountList(int IdCuentaCatTema)
        {
            return _repTemaRepositorie.ThemeCategoryAccountList(IdCuentaCatTema);
        }

        public List<ComTema> ListTemaLessonLearned(int ID_ACCO, int ID_CATE)
        {
            return _repTemaRepositorie.ListTemaLessonLearned(ID_ACCO, ID_CATE);
        }
    }
}
