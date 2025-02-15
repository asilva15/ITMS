using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Electrodata.ERPElectrodata.Domain.Entities.EntityAlternative
{
   public class GestionDocumentoEntity
    {
       public GestionDocumentoEntity() {
          
           this.Adjuntoes = new HashSet<Adjunto>();
       }

       public int Id { get; set; }
       public string Codigo { get; set; }
       public Nullable<int> IdTipoDocumento { get; set; }
       public Nullable<int> IdCuenta { get; set; }
       public Nullable<int> IdCompania { get; set; }
       public string NumeroDocumento { get; set; }
       public Nullable<int> IdPersona { get; set; }
       public string Descripcion { get; set; }
       public Nullable<bool> Activo { get; set; }
       public Nullable<int> UsuarioCreacion { get; set; }
       public Nullable<System.DateTime> Creado { get; set; }
       public Nullable<int> UsuarioModificacion { get; set; }
       public Nullable<System.DateTime> Modificado { get; set; }

       public string TipoArchivo { get; set; }
       public string NombreDocumento { get; set; }

       public virtual ICollection<Adjunto> Adjuntoes { get; set; }

    }
}
