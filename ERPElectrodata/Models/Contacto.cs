//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ERPElectrodata.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Contacto
    {
        public int IdContacto { get; set; }
        public Nullable<int> IdDocuSale { get; set; }
        public Nullable<int> IdAccoPara { get; set; }
        public Nullable<int> IdPersEnti { get; set; }
        public Nullable<System.DateTime> FechaCrea { get; set; }
        public Nullable<int> UsuarioCrea { get; set; }
        public Nullable<System.DateTime> FechaModifica { get; set; }
        public Nullable<int> UsuarioModifica { get; set; }
        public Nullable<bool> Estado { get; set; }
    
        public virtual ACCOUNT_PARAMETER ACCOUNT_PARAMETER { get; set; }
        public virtual DOCUMENT_SALE DOCUMENT_SALE { get; set; }
    }
}
