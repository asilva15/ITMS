using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERPElectrodata.AppCode
{
    public class Sesiones : System.Web.UI.Page
    {
        public Sesiones() { }

        public int SesionIdCuenta
        {
            get { return (int)Session["ID_ACCO"]; }
            set { Session["ID_ACCO"] = value; }
        }

        public int SesionIdUsuario
        {
            get { return (int)Session["UserId"]; }
            set { Session["UserId"] = value; }
        }

        public string KeyAdjunto 
        {
            get { return "M1CT" + Convert.ToString(DateTime.Now.Ticks); } 
        }

        public string SesionNombre
        {
            get { return Session["NAM_PERS"].ToString(); }
            set { Session["NAM_PERS"] = value; }
        }

        public string SesionFoto
        {
            get { return Session["ID_FOTO"].ToString(); }
            set { Session["ID_FOTO"] = value; }
        }
    }
}