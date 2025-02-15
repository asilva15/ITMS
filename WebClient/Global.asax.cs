using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebClient.Plugins;
//using WebClient.Models;

namespace WebClient
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //Agente que verifica Creación de encuestas
            Application["Temporal"] = new System.Threading.Timer(new System.Threading.TimerCallback(GeneraEncuesta), null, new TimeSpan(0, 0, 0, 0, 0), new TimeSpan(0, 2, 0, 0, 0));
        }

        public void GeneraEncuesta(object state)
        {

            try
            {
                //var query = 
                Tickets wt = new Tickets();
                wt.GeneraEncuesta();
                //wt.EnviarEncuesta();
            }
            catch (Exception e)
            {
                var x = e.Message;
                //Console.Write(e.Message.ToString); 
            }

            
        }
    }
}
