using Electrodata.ERPElectrodata.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ERPElectrodata.Models
{
    public class ThemeModel
    {
        public ThemeModel()
        {
            this.ComCuentaCategoryTemas = new HashSet<ComCuentaCategoryTema>();
        }

        public int IdTema { get; set; }
        public int IdCuentaCatTema { get; set; }

        [Required(ErrorMessage = "Field Requierd")]
        [DisplayName("Nombre Tema")]
        public string Nomtema { get; set; }

        [Required(ErrorMessage = "Field Requierd")]
        [DisplayName("Fecha Fin")]
        public DateTime DateEnd { get; set; }


        public int idCategoria { get; set; }


        public int Vigtema { get; set; }
        public string Usuario { get; set; }
        public DateTime DateStart { get; set; }
        public int AccoUser { get; set; }
        public DateTime DateUpdate { get; set; }
        public string UsuarioUpdate { get; set; }

        public DateTime FechaCreacionInicio { get; set; }
        public DateTime FechaCreacionFin { get; set; }
        public string   PalabraClave { get; set; }
        public DateTime FechaBajaInicio { get; set; }
        public DateTime FechaBajaFin { get; set; }


        public virtual IEnumerable<ComCuentaCategoryTema> ComCuentaCategoryTemas { get; set; }

    }
}