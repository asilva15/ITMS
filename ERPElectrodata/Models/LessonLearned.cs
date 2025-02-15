using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ERPElectrodata.Models
{
    public class LessonLearned
        {
        public LessonLearned() {
            this.Ruta = @"/KnowledgeManagement/EditLessonLearned/";
           /* this.Categoria = "Redes";
            this.TipoTicket = "Incidencia";
            this.NroRevisiones= 6;
            this.Status = "Activo";*/
        }    


            public int idLeccionAprendida { get; set; }
            public int idCuenta { get; set; }
            public int idTicket { get; set; }
            public string Titulo { get; set; }
            public int Nivel1 { get; set; }
            public int Nvel2 { get; set; }
            public int Nivel3 { get; set; }
            public int Nivel4 { get; set; }
            public int IdTema { get; set; }
            public int IdTipoTicket { get; set; }
            public DateTime FechaCreacionInicio { get; set; }
            public DateTime FechaCreacionFin { get; set; }
            public int NroRevisiones { get; set; }
            public string PalabraClave { get; set; }
            public int VigLeccionAprendida { get; set; }
            public string Puntuacion { get; set; }
            public string DescripcionProblema { get; set; }
            public string SolucionAplicada { get; set; }
            public string Impactonegocio { get; set; }
            public string SolucionTemporal { get; set; }
            public string SolucionDefinitiva { get; set; }
            public string Usuario { get; set; }
            public string DataStart { get; set; }
            public string DataEnd { get; set; }
            public string AccoUser { get; set; }
            public string UsuarioUpdate { get; set; }
            public string DateUpdate { get; set; }
            public string    EstadoAprobacion { get; set; }
            public string ColorEstado { get; set; }
            public string Status { get; set; }
            public int CodAprobador {get;set;}
            public string Aprobador { get; set; }
            public string Ruta { get; set; }
            public string NombreUsuario { get; set; }
            public string Aprobacion { get; set; }
            public string Categoria { get; set; }
            public int TipoTicket { get; set; }
            public string idPerfilAprobador { get; set; }
            public Nullable<System.DateTime> fechaInicio { get; set; }
            public Nullable<System.DateTime> fechaFin { get; set; }
            public string NombreNivel1 { get; set; }
            public string NombreNivel2 { get; set; }
            public string NombreNivel3 { get; set; }
            public string NombreNivel4 { get; set; }
            public string NombreTema { get; set; }
            public string fechaI_ { get; set; }
            public string fechaF_ { get; set; }
            public string Porque2 { get; set; }
            public string Porque3 { get; set; }
            public string Porque4 { get; set; }
            public string Porque5 { get; set; }


            
        }
}