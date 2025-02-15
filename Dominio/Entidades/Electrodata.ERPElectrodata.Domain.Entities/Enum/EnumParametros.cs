using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Electrodata.ERPElectrodata.Domain.Entities.Enum
{
    public enum Tipo_Peracion
    {
        OPERATION_SAVE =1,
        OPERATION_MODIFY =2,
        OPERATION_SUCCESS = 1,
        OPERATION_ERROR = 0,
        OPERATION_REGISTER=1,
    }

    public enum Tipo_Aprobaciones
    {
        OPERATION_PENDIENTE=1,
        OPERATION_APROBADO =2,
        OPERATION_DESAPROBADO = 3,
    }

    public class EnumParametros
    {
        public sealed class Abreviaturas
        {

        }


    }
}
