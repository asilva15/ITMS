using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;


namespace ERPElectrodata.Controllers
{
    public class CargoController : Controller
    {
        EntityEntities dbe = new EntityEntities();
        //
        // GET: /Cargo/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListCargos() {
            var query = (from q in dbe.CARGOes
                         select new
                         {
                             q.ID_CARG,
                             NAM_CARG = q.NAM_CARG.ToUpper()
                         }).OrderBy(x=>x.NAM_CARG);
            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);           
        }

        public ActionResult ListCargosByID_AREA()
        {
            int ID_AREA = 0;
            ID_AREA = Convert.ToInt32(Request.QueryString["filter[filters][0][value]"]);

            var QCargos = (from b in dbe.CARGO_AREA.Where(x=>x.ID_AREA == ID_AREA)
                           join c in dbe.CARGOes on b.ID_CARG equals c.ID_CARG
                           select new
                           {
                               b.ID_CARG_AREA,
                               b.ID_CARG,
                               c.NAM_CARG
                           }).OrderBy(x => x.NAM_CARG);

            return Json(new { Data = QCargos, Count = QCargos.Count() }, JsonRequestBehavior.AllowGet);
        }


        // cargo

        public ActionResult IndexMantCargos()
        {

            return View();
        }

        public ActionResult ListarCargo()

        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
            var result = dbe.ListarCargoCliente(ID_ACCO).ToList();

            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult ListarCargoTodo()
        {


            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
            int IdAreaPrincipal = 0;
            int IdAreaSecubdaria = 0;
            int Estado = 0;
            if (string.IsNullOrEmpty(Request.Params["IdAreaCargo"]))
            {
                IdAreaPrincipal = 0;
            }
            else
            {
                IdAreaPrincipal = int.Parse(Request.Params["IdAreaCargo"]);
            }

           

            if (string.IsNullOrEmpty(Request.Params["Estado"]))
            {
                Estado = 0;
            }
            else
            {
                Estado = int.Parse(Request.Params["Estado"]);
            }


            var result = dbe.ListarBuscarCargo(ID_ACCO, IdAreaPrincipal,  Estado).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CrearNuevoaCargo(FormCollection collection)
        {

            NAME_CHART objInforme = new NAME_CHART();
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
            string msjError = string.Empty;
            int areaprincial = 0;
            string flag = "";
            string  nombreCargo="";
            int nivelarea = 0;
            int estado = 0;

            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Request.Params["txtNombreCargo"])))
                {
                    msjError = "Coloque el  nombre del cargo";
                    return Content("<script type='text/javascript'> function init() {" +
                                "if(top.uploadDoneNR) top.uploadDoneNR('ERROR','" + msjError.ToString() + "');}window.onload=init;</script>");
                
            }
                else
                {
                    nombreCargo = Convert.ToString(Request.Params["txtNombreCargo"]);
                }


                var val = (from c in dbe.CHARTs
                           join nc in dbe.NAME_CHART on c.ID_NAM_CHAR equals nc.ID_NAM_CHAR
                           where c.ID_ACCO == 55 && 
                           nc.NAM_CHAR== nombreCargo
                           select new { Chart = c, NameChart = nc }).ToList();





                if (val.Count() > 0)
                {
                    msjError = "El nombre del CARGO ya se encuentra registrado.";
                    return Content("<script type='text/javascript'> function init() {" +
                                "if(top.uploadDoneNR) top.uploadDoneNR('ERROR','" + msjError.ToString() + "');}window.onload=init;</script>");
                }

                else {

                // Cargar Name Chart

                objInforme.NAM_CHAR = nombreCargo;
                objInforme.NAM_CHAR_SPAN = nombreCargo;
                objInforme.VIG_CHAR = true;
                objInforme.ID_TYPE_CHAR = 3;
                objInforme.MANAGEMENT = false;

                dbe.NAME_CHART.Add(objInforme);
                dbe.SaveChanges();

                // Obtener el ID insertado anteriormente
                int idNameChart = objInforme.ID_NAM_CHAR;

                // Crear Chart
                if (idNameChart != null && idNameChart != 0)
                {
                    // Crear Chart
                    CHART objChart = new CHART();
                    objChart.ID_CHAR_PARE = 0;
                    objChart.ID_NAM_CHAR = idNameChart;
                    objChart.ID_ACCO = ID_ACCO;
                    objChart.VIG_CHAR = true;

                    dbe.CHARTs.Add(objChart);
                    dbe.SaveChanges();
                }
                else
                {
                    // Manejar la situación cuando el ID es nulo o cero
                    string errorMessage = "No se LOGRO  generar correctamente.Comuniquese con el administrador";
                    return Content("<script type='text/javascript'> function init() {" +
                                 "if(top.uploadDoneNR) top.uploadDoneNR('ERROR','" + errorMessage.ToString() + "');}window.onload=init;</script>");
                }

                }



            }
            catch (Exception e)
            {
                string Error = e.Message;
                return Content("<script type='text/javascript'> function init() {" +
                                "if(top.uploadDoneNR) top.uploadDoneNR('ERROR','" + Error.ToString() + "');}window.onload=init;</script>");
            }
            return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.uploadDoneNR) top.uploadDoneNR('OK','El registro se guardó correctamente');}window.onload=init;</script>");

        }

        public ActionResult CrearCargos()
        {
            return View();
        }

        public ActionResult EditarCargos(int id = 0)
        {
            NAME_CHART objnameChart = dbe.NAME_CHART.SingleOrDefault(x => x.ID_NAM_CHAR == id);

            if (objnameChart == null)
            {
                // Manejar el caso cuando el objeto objPlantilla es nulo
                return RedirectToAction("Error");
            }

            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());

            var data = dbe.ListarBuscarCargo(ID_ACCO, id, 0).SingleOrDefault();

            if (data == null)
            {
                // Manejar el caso cuando los datos son nulos
                return RedirectToAction("Error");
            }

            ViewBag.ID_NAM_CHAR = id;
            ViewBag.NAM_CHAR = data.NAM_CHAR;
            ViewBag.NAM_ACCO = data.NAM_ACCO;
            ViewBag.VIG_CHAR = Convert.ToInt32(data.VIG_CHAR);

            return View(objnameChart);
        }



        [HttpPost, ValidateInput(false)]
        public ActionResult EdicionCargo(FormCollection collection)

        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
            string msjError = string.Empty;

            string flag = "";
            string nombrecargo = "";
            bool estado = false;


    

            int areacharprincipal = Convert.ToInt32(Request.Params["txtidCargo"]);
            string Estado = Convert.ToString(Request.Params["checkareaPrincipaled"]);


            try
            {
                if (Estado == "on")
                {
                    estado = true;
                }
                else
                {
                    estado = false;
                }



                NAME_CHART objCargo = dbe.NAME_CHART.SingleOrDefault(a => a.ID_NAM_CHAR == areacharprincipal);

                var areas = dbe.NAME_CHART
                    .Where(a => a.ID_NAM_CHAR == areacharprincipal)
                    .ToList();
                if (string.IsNullOrEmpty(Convert.ToString(Request.Params["txtNombreCargoed"])))
                {

                    msjError = "Coloque el  nombre del cargo";
                    return Content("<script type='text/javascript'> function init() {" +
                                "if(top.uploadDoneNR) top.uploadDoneNR('ERROR','" + msjError.ToString() + "');}window.onload=init;</script>");
                }
                else
                {
                    nombrecargo = Convert.ToString(Request.Params["txtNombreCargoed"]);
                }

            



                var val = (from c in dbe.CHARTs
                           join nc in dbe.NAME_CHART on c.ID_NAM_CHAR equals nc.ID_NAM_CHAR
                           where c.ID_ACCO == 55 &&
                           nc.NAM_CHAR == nombrecargo&&
                           nc.VIG_CHAR==estado
                           select new { Chart = c, NameChart = nc }).ToList();





                if (val.Count() > 0)
                {
                    msjError = "El nombre del CARGO ya se encuentra registrado.";
                    return Content("<script type='text/javascript'> function init() {" +
                                "if(top.uploadDoneNR) top.uploadDoneNR('ERROR','" + msjError.ToString() + "');}window.onload=init;</script>");
                }

                else
                {



                    objCargo.NAM_CHAR = nombrecargo;
                    if (Estado == "on")
                    {
                        objCargo.VIG_CHAR = true;
                    }
                    else
                    {
                        objCargo.VIG_CHAR = false;
                    }

                    dbe.Entry(objCargo).State = EntityState.Modified;


                    dbe.SaveChanges();
                }
            }
            catch (Exception e)
            {
                string Error = e.Message;
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDoneNR) top.uploadDoneNR('ERROR','" + Error.ToString() + "');}window.onload=init;</script>");
            }

            return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDoneNR) top.uploadDoneNR('OK','El registro se guardó correctamente');}window.onload=init;</script>");
        }




    }
}
