using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.Globalization;
using System.Threading;

using System.Data.Entity;

namespace ERPElectrodata.Controllers
{
    public class AreaController : Controller
    {
        public EntityEntities dbe = new EntityEntities();
        public CoreEntities dbc = new CoreEntities();
        private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        private TextInfo textInfo;

        public ActionResult UENByPerson()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            //int ID_ACCO = 4;

            textInfo = cultureInfo.TextInfo;

            var query = (from pe in dbe.PERSON_ENTITY
                             .Where(x=>x.ID_PERS_STAT == 1 /*|| x.ID_PERS_STAT == 2*/).ToList()
                         join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                         join ae in dbe.ACCOUNT_ENTITY on pe.ID_PERS_ENTI equals ae.ID_PERS_ENTI
                         where ae.ID_ACCO == ID_ACCO && ae.VIS_TALE == true
                         join jbt in dbe.CARGO_AREA on pe.ID_CARG_AREA equals jbt.ID_CARG_AREA
                         join ar in dbe.AREAs on jbt.ID_AREA equals ar.ID_AREA
                         join ar2 in dbe.AREAs on ar.ID_AREA_PARENT equals ar2.ID_AREA
                         group ar2 by new { ar2.ID_AREA,ar2.NOM_AREA} into g
                         select new {
                             name = textInfo.ToTitleCase(g.Key.NOM_AREA.ToLower()),
                            y = g.Count()
                         }).OrderByDescending(x=>x.y);

            return Json(new { pie = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListByClient()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());

            var query = dbe.AREAs.Where(a => a.ID_ACCO == ID_ACCO);

            var result = (from a in query.ToList()
                          select new
                          {
                              a.ID_AREA,
                              NAM_AREA = a.NOM_AREA
                          }).ToList();

            return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }
        //
        // GET: /Area/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListDetailsByClient()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());

            var query = dbe.AREAs.Where(a => a.ID_ACCO == ID_ACCO && a.ID_AREA_PARENT != null);

            var result = (from a in query
                          join ar in dbe.AREAs on a.ID_AREA_PARENT equals ar.ID_AREA into lj
                          from x1 in lj.DefaultIfEmpty()
                          orderby a.NOM_AREA
                          select new
                          {
                              a.ID_AREA,
                              NAM_AREA = a.NOM_AREA,
                              NOM_AREA_PARE = (x1 == null ? String.Empty : x1.NOM_AREA)
                          }).ToList();

            return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListAreaCIA()
        {
            int ID_ACCO = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 18)
                        .Join(dbc.ACCOUNT_PARAMETER, x => x.VAL_ACCO_PARA, ap => ap.VAL_ACCO_PARA, (x, ap) => new
                        {
                            ap.ID_ACCO,
                            ap.ID_PARA
                        }).Where(x => x.ID_PARA == 6).First().ID_ACCO.Value;

            var result = (from a in dbe.AREAs.Where(x => x.ID_ACCO == ID_ACCO && x.VIG_AREA == true).ToList()
                          where a.NIV_AREA != 1
                          join b in dbe.AREAs on a.ID_AREA_PARENT equals b.ID_AREA into lb
                          from xb in lb.DefaultIfEmpty()
                          join c in dbe.AREAs on xb.ID_AREA_PARENT equals c.ID_AREA into lc
                          from xc in lc.DefaultIfEmpty()
                          select new
                          {
                              a.ID_AREA,
                              a.ID_AREA_PARENT,
                              a.NOM_AREA,
                              a.NIV_AREA,
                              Order = (a.NIV_AREA == 2 ? a.NOM_AREA : (a.NIV_AREA == 3 ? xb.NOM_AREA + a.NOM_AREA : xc.NOM_AREA + xb.NOM_AREA + a.NOM_AREA)),
                              SwImagen = NumerosHijos(Convert.ToInt32(a.ID_AREA)),
                              NAM_AREA = (a.NIV_AREA == 2 ? (a.ACRONYM == null ? "" : a.ACRONYM) : (a.NIV_AREA == 3 ? (xb.ACRONYM == null ? "" : xb.ACRONYM) : (xc.ACRONYM == null ? "" : xc.ACRONYM))) + " - " + a.NOM_AREA,
                          }).OrderBy(x => x.Order);

            return Json(new { Data = result, Count = 0 }, JsonRequestBehavior.AllowGet);
        }

        public int NumerosHijos(int id = 0)
        {
            int ctd = dbe.AREAs.Where(x => x.ID_AREA_PARENT == id && x.VIG_AREA == true).Count();
            return ctd;
        }

        public ActionResult ListAreaByID_AREA(int id = 0)
        {

            var result = (from a in dbe.AREAs.Where(x => x.ID_AREA_PARENT == id && x.VIG_AREA == true).ToList()
                          join b in dbe.AREAs on a.ID_AREA_PARENT equals b.ID_AREA into lb
                          from xb in lb.DefaultIfEmpty()
                          join c in dbe.AREAs on xb.ID_AREA_PARENT equals c.ID_AREA into lc
                          from xc in lc.DefaultIfEmpty()
                          select new
                          {
                              a.ID_AREA,
                              a.ID_AREA_PARENT,
                              a.NOM_AREA,
                              a.NIV_AREA,
                              Order = (a.NIV_AREA == 3 ? xb.NOM_AREA + a.NOM_AREA : xc.NOM_AREA + xb.NOM_AREA + a.NOM_AREA),
                              SwImagen = NumerosHijos(Convert.ToInt32(a.ID_AREA)),
                              NAM_AREA = (a.NIV_AREA == 3 ? (xb.ACRONYM == null ? "" : xb.ACRONYM) : (xc.ACRONYM == null ? "" : xc.ACRONYM)) + " - " + a.NOM_AREA,
                          }).OrderBy(x => x.Order);

            return Json(new { Data = result, Count = 0 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult USB()
        {
            var result = (from a in dbe.AREAs.Where(a => a.ID_ACCO == 4)
                          where a.NIV_AREA == 2
                          select new { 
                            NAM_AREA = a.NOM_AREA,
                            a.ID_AREA
                          }).OrderBy(x=>x.NAM_AREA);

            return Json(new { Data = result,Count = result.Count()},JsonRequestBehavior.AllowGet);
        }

        public ActionResult AreaByUSB()
        {
            string NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][0][field]"]), i1;

            if (NAM_PAR == "ID_AREA")
            {
                i1 = "0";
            }
            else
            {
                i1 = "1";
            }

            int ID_ARE_PARE = Convert.ToInt32(Request.QueryString[("filter[filters][" + i1 + "][value]")]);

            var result = (from a in dbe.AREAs.Where(a => a.ID_AREA_PARENT == ID_ARE_PARE)
                          //where a.NIV_AREA == 2
                          select new
                          {
                              NAM_AREA = a.NOM_AREA,
                              a.ID_AREA
                          });

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarAreas(string q, string page)
        {
            string termino = "";

            if (Request.QueryString["q"] == null)
            {
                termino = "";
            }
            else
            {
                termino = Request.QueryString["q"].ToString();
            }

            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);

            List<ListarAreas_Result> resultado = dbe.ListarAreas(IdAcco, termino).ToList();

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarAreasCuenta(int id , string q, string page)
        {
            string termino = "";

            if (Request.QueryString["q"] == null)
            {
                termino = "";
            }
            else
            {
                termino = Request.QueryString["q"].ToString();
            }
            
            List<ListarAreas_Result> resultado = dbe.ListarAreas(id, termino).ToList();

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarAreaxCompania(int IdEnti = 0, int Tipo = 0)
        {
            //int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);

            int ID_CIA = 0;
            string CTE = "";
            if (Tipo == 1)
            {

                ID_CIA = IdEnti;
            }
            else
            {
                string NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][0][field]"]);
                if (NAM_PAR == "ID_CIA")
                {
                    ID_CIA = Convert.ToInt32(Request.QueryString["filter[filters][0][value]"]);
                }
                else if (NAM_PAR == "text")
                {
                    CTE = Convert.ToString(Request.QueryString["filter[filters][0][value]"]);
                }

                NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][1][field]"]);
                if (NAM_PAR == "ID_CIA")
                {
                    ID_CIA = Convert.ToInt32(Request.QueryString["filter[filters][1][value]"]);
                }
                else if (NAM_PAR == "text")
                {
                    CTE = Convert.ToString(Request.QueryString["filter[filters][1][value]"]);
                }
            }
            var result = dbe.ListarAreaxCompania(ID_CIA);
            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }


        // area mantenimiento
        public ActionResult IndexMantAreas()
        {

            return View();
        }

        public ActionResult ListarAreaprincipal()

        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
            var result = dbe.ListarAreaPrincipal(ID_ACCO).ToList();

            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult CrearAreas()
        {
            return View();
        }

        public ActionResult EditarAreas(int id = 0, int id1 = 0)
        {
          

            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());

            if (id == 0)
            {
                AREA objPlantilla = dbe.AREAs.SingleOrDefault(x => x.ID_AREA == id1);

                if (objPlantilla == null)
                {
                    // Manejar el caso cuando el objeto objPlantilla es nulo
                    return RedirectToAction("Error");
                }
                var data = dbe.ListarEditarArea(ID_ACCO, id1, id, 0).SingleOrDefault();
                if (data == null)
                {
                    // Manejar el caso cuando los datos son nulos
                    return RedirectToAction("Error");
                }

                ViewBag.ID_AREA_PARENT = id1;
                ViewBag.NOM_AREA_PARENT = data.NOM_AREA;

                ViewBag.ID_AREA = id;
                ViewBag.NOM_AREA = data.NOM_AREA_PARENT;
                ViewBag.NIV_AREA = data.NIV_AREA;
                ViewBag.VIG_AREA = Convert.ToInt32(data.VIG_AREA);

                return View(objPlantilla);
            }
            else
            {
                AREA objPlantilla = dbe.AREAs.SingleOrDefault(x => x.ID_AREA == id);

                if (objPlantilla == null)
                {
                    // Manejar el caso cuando el objeto objPlantilla es nulo
                    return RedirectToAction("Error");
                }

                var data = dbe.ListarBuscarArea(ID_ACCO, id, id1, 0,0).SingleOrDefault();
                if (data == null)
                {
                    // Manejar el caso cuando los datos son nulos
                    return RedirectToAction("Error");
                }

                ViewBag.ID_AREA_PARENT = id;
                ViewBag.NOM_AREA_PARENT = data.NOM_AREA_PARENT;

                ViewBag.ID_AREA = id1;
                ViewBag.NOM_AREA = data.NOM_AREA;
                ViewBag.NIV_AREA = data.NIV_AREA;
                ViewBag.VIG_AREA = Convert.ToInt32(data.VIG_AREA);

                return View(objPlantilla);

            }



           




          
        }

        [HttpPost, ValidateInput(false)]  
        public ActionResult CrearNuevaArea(FormCollection collection)
        {

            AREA objInforme = new AREA();
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
            string msjError = string.Empty;
            int? areaprincial = null;
            string flag = "";
            string nombreArea = "";
            int nivelarea =0 ;
            int estado = 0;

            try
            {
            

                if (string.IsNullOrEmpty(Convert.ToString(Request.Params["txtNombreAreas"])))
                {
                    msjError = "El nombre del área no puede estar vacio";
                    return Content("<script type='text/javascript'> function init() {" +
                                "if(top.uploadDoneNR) top.uploadDoneNR('ERROR','" + msjError.ToString() + "');}window.onload=init;</script>");
                }
                else
                {
                    nombreArea = Convert.ToString(Request.Params["txtNombreAreas"]);
                }



                if (Request.Params["checkareaPrincipal"] == null || Request.Params["checkareaPrincipal"] == "false")
                {

                    if (Convert.ToString(Request.Params["areaprincial"]) == null || Convert.ToString(Request.Params["areaprincial"]) == "")
                    {
                        msjError = "No se encontro ninguna área";
                        return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.uploadDoneNR) top.uploadDoneNR('ERROR','" + msjError.ToString() + "');}window.onload=init;</script>");
                    }
                    else
                    {
                        areaprincial = Convert.ToInt32(Request.Params["areaprincial"]);
                    }


                    var val = (from a in dbe.AREAs
                               where a.ID_AREA_PARENT != null &&
                                     a.ID_AREA_PARENT == areaprincial &&
                                     a.ID_ACCO == ID_ACCO &&
                                     a.NOM_AREA == nombreArea
                               select a).ToList();

                 


                    if (val.Count() > 0)
                    {
                        msjError = "El nombre del ÁREA PRINCIPAL ya se encuentra registrada.";
                        return Content("<script type='text/javascript'>   function init() { " +
                                    "if(top.uploadDoneNR) top.uploadDoneNR('ERROR','" + msjError.ToString() + "');}window.onload=init;</script>");
                    }

             

                
                    nivelarea = 2;

                }
                else
                {
                    var val = (from a in dbe.AREAs
                               where a.ID_AREA_PARENT == areaprincial &&
                                     a.NIV_AREA == 1 &&
                                     a.ID_ACCO == ID_ACCO &&
                                     a.NOM_AREA == nombreArea
                               select a).ToList();



                    if (val.Count() > 0)
                    {
                        msjError = "El nombre del ÁREA PRINCIPAL ya se encuentra registrada.";
                        return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.uploadDoneNR) top.uploadDoneNR('ERROR','" + msjError.ToString() + "');}window.onload=init;</script>");
                    }



                    areaprincial = null;
                    nivelarea = 1;
                }


             

                //INFORME AUTOMATICO



                objInforme.ID_AREA_PARENT = areaprincial;
                objInforme.ID_ACCO = ID_ACCO;
                objInforme.NOM_AREA = nombreArea;
                objInforme.NIV_AREA = nivelarea;
                objInforme.VIG_AREA = true;
                objInforme.ID_ACCO_PERT = ID_ACCO;






                //FIN


                dbe.AREAs.Add(objInforme);
                dbe.SaveChanges();

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





        [HttpPost, ValidateInput(false)]
        public ActionResult EdicioAreas(FormCollection collection)

        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
            string msjError = string.Empty;
         
            string flag = "";
            string nombreArea = "";
            bool estado = false;

            int valor = 0;
            int  idGrupo = Convert.ToInt32(Request.Params["txtareaprincipal"]);
            int idareasecundaria = Convert.ToInt32(Request.Params["NombreArea"]);
            int txtniv_area = Convert.ToInt32(Request.Params["txtniv_area"]);

            int areaprincial = Convert.ToInt32(Request.Params["txtareaprincipal"]);

            AREA objInforme = null;
            string Estado = "";

            bool niv_area = false;




            try
            {

                Estado = Convert.ToString(Request.Params["checkareaPrincipaled"]);

                if (Estado == "on")
                {
                    niv_area = true;
                }
                else
                {
                    niv_area = false;
                }

                if (string.IsNullOrEmpty(Convert.ToString(Request.Params["txtNombreArea"])))
                {
                    msjError = "El nombre no puede estar vacio";
                    return Content("<script type='text/javascript'> function init() {" +
                                "if(top.uploadDoneNR) top.uploadDoneNR('warning','" + msjError.ToString() + "');}window.onload=init;</script>");
                }
                else
                {
                    nombreArea = Convert.ToString(Request.Params["txtNombreArea"]);
                }


                if (txtniv_area == 1) {

                     objInforme = dbe.AREAs.SingleOrDefault(a =>a.ID_AREA == areaprincial);

                    var areas = dbe.AREAs
                        .Where(a => a.ID_AREA == areaprincial && a.ID_ACCO == ID_ACCO)
                        .ToList(); 
                    var val = (from a in dbe.AREAs
                                              where 
                                              
                                                  a.NIV_AREA == txtniv_area &&
                                                    a.ID_ACCO == ID_ACCO &&
                                                      a.VIG_AREA == niv_area&&
                                                    a.NOM_AREA == nombreArea
                                              select a).ToList();

                    valor = val.Count();


                }
                else
                {
                     objInforme = dbe.AREAs.SingleOrDefault(a => a.ID_AREA_PARENT == areaprincial && a.ID_AREA == idareasecundaria);

                    var areas = dbe.AREAs
                        .Where(a => a.ID_AREA_PARENT == areaprincial && a.ID_AREA == idareasecundaria && a.ID_ACCO == ID_ACCO)
                        .ToList();




                  





                    var val = (from a in dbe.AREAs
                            where a.ID_AREA_PARENT != null &&
                             a.ID_AREA_PARENT == areaprincial &&
                                  a.ID_ACCO == ID_ACCO &&
                                  a.NOM_AREA == nombreArea &&
                                
                                  a.VIG_AREA == niv_area
                               select a).ToList();
                    valor = val.Count();

                }

                Estado = Convert.ToString(Request.Params["checkareaPrincipaled"]);

                if (valor > 0)
                {
                    msjError = "No se puede agregar esta AREA  porque ya se encuentra registrada.";
                    return Content("<script type='text/javascript'> function init() {" +
                                "if(top.uploadDoneNR) top.uploadDoneNR('ERROR','" + msjError.ToString() + "');}window.onload=init;</script>");
                }
                else
                {


                    objInforme.NOM_AREA = nombreArea;
                    if (Estado == "on")
                    {
                        objInforme.VIG_AREA = true;
                    }
                    else
                    {
                        objInforme.VIG_AREA = false;
                    }

                    dbe.Entry(objInforme).State = EntityState.Modified;


                    dbe.SaveChanges();
                }





            }
            catch (Exception e)
            {
                string Error = e.Message;
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDoneNR) top.uploadDoneNR('ERROR','" + Error.ToString() + "');}window.onload=init;</script>");
            }




            //prueba parwa actualizar






            return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDoneNR) top.uploadDoneNR('OK','El registro se guardó correctamente');}window.onload=init;</script>");
        }
        public ActionResult ListarAreaTodo()
        {


            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
            int IdAreaPrincipal = 0;
            int IdAreaSecubdaria = 0;
            int Estado = 0;
            int nivelarea = 0;
            if (string.IsNullOrEmpty(Request.Params["IdAreaPrincipal"]))
            {
                IdAreaPrincipal = 0;
            }
            else
            {
                IdAreaPrincipal = int.Parse(Request.Params["IdAreaPrincipal"]);
            }

            if (string.IsNullOrEmpty(Request.Params["IdAreaSecubdaria"]))
            {
                IdAreaSecubdaria = 0;
            }
            else
            {
                IdAreaSecubdaria = int.Parse(Request.Params["IdAreaSecubdaria"]);
            }

            if (string.IsNullOrEmpty(Request.Params["Estado"]))
            {
                Estado = 0;
            }
            else
            {
                Estado = int.Parse(Request.Params["Estado"]);
            }

            if (string.IsNullOrEmpty(Request.Params["nivelarea"]))
            {
                nivelarea = 0;
            }
            else
            {
                nivelarea = int.Parse(Request.Params["nivelarea"]);
            }


            var result = dbe.ListarBuscarArea(ID_ACCO, IdAreaPrincipal, IdAreaSecubdaria, Estado, nivelarea).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ListarGruposPorArea() //plantilla y ticket
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
            string txt = "";
            int id = Convert.ToInt32(Request.QueryString["filter[filters][0][value]"]);
            txt = Convert.ToString(Request.Params["filter[filters][1][value]"]);
            if (txt == null) { txt = ""; }

            var query = dbe.ListarGrupoArea(ID_ACCO, id).ToList();

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);

        }


        //fin areas mantenimiento


        public ActionResult ListarAreasTotales()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
            var result = dbe.ListarAreasTotales(ID_ACCO).ToList();
                
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }



    }
}
