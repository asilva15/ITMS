using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class CategoryTicketController : Controller
    {
        private CoreEntities db = new CoreEntities();
        private EntityEntities dbe = new EntityEntities();

        public ActionResult ListCategory()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]), ID_CATE = 0;

            string NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][0][field]"]), i1;

            //string NAM_CATE = Convert.ToString(Request.Params["filter[filters][0][value]"]);

            string NAM_CATE1 = Convert.ToString(Request.Params["filter[filters][0][value]"]);

            //if (String.IsNullOrEmpty(NAM_CATE))
            //{
            //    NAM_CATE = "";
            //}
            if (!String.IsNullOrEmpty(NAM_PAR))
            {
                if (NAM_PAR == "ID_CATE")
                {
                    i1 = "0";
                }
                else
                {
                    i1 = "1";
                }

                ID_CATE = Convert.ToInt32(Request.QueryString[("filter[filters][" + i1 + "][value]")]);
            }

            var query = db.ACCOUNT_CATEGORY
                        .Join(db.CATEGORies, ac => ac.ID_CATE, c => c.ID_CATE, (ac, c) => new { c.NAM_CATE, c.ID_CATE, c.ID_CATE_PARE, c.ABR_CATE, c.NIV_CATE, ac.ID_ACCO })
                        .Where(c => c.ID_ACCO == ID_ACCO);

            //(from ac in db.ACCOUNT_CATEGORY.ToList()
            //         where ac.ID_ACCO == ID_ACCO && ac.VIG_ACCO_CATE == true
            //         join c in db.CATEGORies on ac.ID_CATE equals c.ID_CATE
            //         select new
            //         {
            //             c.NAM_CATE,
            //             c.ID_CATE,
            //             c.ID_CATE_PARE,
            //             c.ABR_CATE,
            //             c.NIV_CATE
            //         }).ToList();//.Where(c=>c.NAM_CATE.ToUpper().Contains(NAM_CATE.ToUpper()));

            if (ID_CATE == 0)
            {
                query = query.Where(c => c.ID_CATE_PARE == null);
            }
            else
            {
                query = query.Where(c => c.ID_CATE_PARE == ID_CATE);
            }

            if (!String.IsNullOrEmpty(NAM_PAR))
            {
                if (NAM_PAR == "NAM_CATE")
                {
                    query = query.Where(c => c.NAM_CATE.Contains(NAM_CATE1.ToUpper()));
                }
            }

            var result = (from c in query.ToList()
                              //where c.ID_CATE_PARE == null
                              //join c2 in query on c.ID_CATE equals c2.ID_CATE_PARE
                              //join c3 in query on c2.ID_CATE equals c3.ID_CATE_PARE
                              //join c4 in query on c3.ID_CATE equals c4.ID_CATE_PARE
                          select new
                          {
                              NAM_CATE_1 = c.NAM_CATE,
                              //NAM_CATE_2 = c2.NAM_CATE,
                              //NAM_CATE_3 = c3.NAM_CATE,
                              //NAM_CATE_4 = c4.NAM_CATE,
                              c.ID_CATE,
                              NAM_CATE = c.NAM_CATE,//c.ABR_CATE + '.' + c2.ABR_CATE + '.' + c3.ABR_CATE + '.' + c4.ABR_CATE
                          }).OrderBy(x => x.NAM_CATE);

            return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListCategoryGeneral()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            try
            {
                var query = db.CATEGORies.Where(x => x.VIG_CATE == true).ToList();

                var result = (from x in query
                              select new
                              {
                                  x.ID_CATE,
                                  x.NAM_CATE,
                              }).ToList();

                return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult ListCategoryGF()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int ID_CATE = 0;

            try
            {
                List<catListarCategoria_Result> query = db.catListarCategoria(ID_ACCO, ID_CATE).ToList();

                return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult ListCategory2(int ID_ACCO = 0)
        {
            ID_ACCO = ID_ACCO == 0 ? Convert.ToInt32(Session["ID_ACCO"]) : ID_ACCO;
            int ID_CATE = 0;
            string NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][0][field]"]), i1, text = "";

            if (!String.IsNullOrEmpty(NAM_PAR))
            {
                if (NAM_PAR == "ID_CATE")
                {
                    i1 = "0";
                }
                else
                {
                    i1 = "1";
                    text = Convert.ToString(Request.QueryString[("filter[filters][0][value]")]);
                }

                ID_CATE = Convert.ToInt32(Request.QueryString[("filter[filters][" + i1 + "][value]")]);
            }
            try
            {
                List<catListarCategoria_Result> query = db.catListarCategoria(ID_ACCO, ID_CATE).ToList();
                var result = (from c in query.ToList()
                              select new
                              {
                                  ID_CATE2 = c.ID_CATE,
                                  NAM_CATE2 = c.NAM_CATE
                              }).Where(x => x.NAM_CATE2.ToLower().Contains(text.ToLower())).ToList();

                return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult ListCategory3(int ID_ACCO = 0)
        {
            ID_ACCO = ID_ACCO == 0 ? Convert.ToInt32(Session["ID_ACCO"]) : ID_ACCO;
            int ID_CATE = 0;

            string NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][0][field]"]), i1, text = "";

            if (!String.IsNullOrEmpty(NAM_PAR))
            {
                if (NAM_PAR == "ID_CATE2")
                {
                    i1 = "0";
                }
                else
                {
                    i1 = "1";
                    text = Convert.ToString(Request.QueryString[("filter[filters][0][value]")]);
                }

                ID_CATE = Convert.ToInt32(Request.QueryString[("filter[filters][" + i1 + "][value]")]);
            }
            try
            {
                List<catListarCategoria_Result> query = db.catListarCategoria(ID_ACCO, ID_CATE).ToList();
                var result = (from c in query.ToList()
                              select new
                              {
                                  ID_CATE3 = c.ID_CATE,
                                  NAM_CATE3 = c.NAM_CATE
                              }).Where(x => x.NAM_CATE3.ToLower().Contains(text.ToLower())).ToList();

                return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult ListCategory4(int ID_ACCO = 0)
        {
            ID_ACCO = ID_ACCO == 0 ? Convert.ToInt32(Session["ID_ACCO"]) : ID_ACCO;
            int ID_CATE = 0;

            string NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][0][field]"]), i1, text = "";

            if (!String.IsNullOrEmpty(NAM_PAR))
            {
                if (NAM_PAR == "ID_CATE3")
                {
                    i1 = "0";
                }
                else
                {
                    i1 = "1";
                    text = Convert.ToString(Request.QueryString[("filter[filters][0][value]")]);
                }

                ID_CATE = Convert.ToInt32(Request.QueryString[("filter[filters][" + i1 + "][value]")]);
            }
            try
            {
                List<catListarCategoria_Result> query = db.catListarCategoria(ID_ACCO, ID_CATE).ToList();
                var result = (from c in query.ToList()
                              select new
                              {
                                  ID_CATE4 = c.ID_CATE,
                                  NAM_CATE4 = c.NAM_CATE,
                                  ID_TYPE_TICK = c.ID_TYPE_TICK
                              }).Where(x => x.NAM_CATE4.ToLower().Contains(text.ToLower())).ToList();

                return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult PersonalAsignadoGF()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            var query = dbe.ACCOUNT_ENTITY.Where(ae => ae.ID_ACCO == ID_ACCO && ae.VIS_ASSI == true && ae.VIG_ACCO_ENTI == true).ToList();

            var result = (from x in query
                          join pe in dbe.PERSON_ENTITY on x.ID_PERS_ENTI equals pe.ID_PERS_ENTI
                          join p in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals p.ID_ENTI
                          select new
                          {
                              CLIE = p.FIR_NAME + " " + p.SEC_NAME + " " + p.LAS_NAME + " " + p.MOT_NAME,
                              pe.ID_PERS_ENTI,
                              pe.ID_PRIO,
                              pe.ID_AREA,
                          }).Where(x => x.CLIE != " ").ToList().OrderBy(x => x.CLIE);

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AgregarConfiguracionCategoria(string DetalleCabecera, bool EsTicketHijo, bool EsTicketPadre, string ID_CATE_N10, string ID_CATE_N20, string ID_CATE_N30, string ID_CATE_N40, string ID_PERS_ENTI2, string TicketsPadres)
        {
            try
            {



                ConfiguracionCategorias configuracion = new ConfiguracionCategorias();

                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

                var cont = db.ConfiguracionCategorias.Where(t => t.Estado == true);

                var existe = (from t in cont.ToList()
                              where t.IdCate1 == Convert.ToInt32(ID_CATE_N10) &&
                              t.IdCate2 == Convert.ToInt32(ID_CATE_N20) &&
                              t.IdCate3 == Convert.ToInt32(ID_CATE_N30) &&
                              t.IdCate4 == Convert.ToInt32(ID_CATE_N40) &&
                              t.IdAcco == ID_ACCO
                              select new
                              {

                              }).Count();

                if (existe > 0)
                {
                    return Json(new { success = false, message = "Ya existe una configuración con las mismas categorías en esta cuenta." });
                }

                configuracion.IdAcco = ID_ACCO;
                configuracion.IdCate1 = ID_CATE_N10 == null ? 0 : Convert.ToInt32(ID_CATE_N10);
                configuracion.IdCate2 = ID_CATE_N20 == null ? 0 : Convert.ToInt32(ID_CATE_N20);
                configuracion.IdCate3 = ID_CATE_N30 == null ? 0 : Convert.ToInt32(ID_CATE_N30);
                configuracion.IdCate4 = ID_CATE_N40 == null ? 0 : Convert.ToInt32(ID_CATE_N40);
                configuracion.IdPersEnti = ID_PERS_ENTI2 == null ? 0 : Convert.ToInt32(ID_PERS_ENTI2);
                configuracion.FechaAgregacion = DateTime.Now;
                configuracion.IdUsuarioCreo = Convert.ToInt32(Session["UserId"]);
                configuracion.Estado = true;
                configuracion.FechaModificacion = DateTime.Now;
                configuracion.IdUsuarioModifica = Convert.ToInt32(Session["UserId"]);
                configuracion.EsHijo = EsTicketHijo;
                configuracion.EsPadre = EsTicketPadre;
                configuracion.DetalleCabecera = DetalleCabecera;

                if (TicketsPadres != "")
                {
                    configuracion.IdConfiguracionPadre = Convert.ToInt32(TicketsPadres);
                }

                db.ConfiguracionCategorias.Add(configuracion);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return Json(new { success = true, message = "OK" });
        }

        public ActionResult MantenimientoCategoriaGF()
        {
            if (Convert.ToInt32(Session["ID_ACCO"]) != 1 && Convert.ToInt32(Session["ID_ACCO"]) != 25)
            {
                return RedirectToAction("Index", "Error");
            }
            return View();
        }
        public ActionResult AgregarConfiguracionCategoriaGF()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GuardarCambiosConfiguracion(int idConfiguracion, string detalleCabecera, string persEnti)
        {
            try
            {
                var conf = new ConfiguracionCategorias();
                conf = db.ConfiguracionCategorias.Find(idConfiguracion);
                conf.DetalleCabecera = detalleCabecera;
                conf.IdPersEnti = Convert.ToInt32(persEnti);
                conf.FechaModificacion = DateTime.Now;
                conf.IdUsuarioModifica = Convert.ToInt32(Session["UserId"]);
                db.ConfiguracionCategorias.Attach(conf);
                db.Entry(conf).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }
            return Json(new { success = true, message = "OK" });
        }

        [HttpPost]
        public ActionResult EliminarConfiguracion(int idConfiguracion)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            try
            {
                var conf = new ConfiguracionCategorias();
                conf = db.ConfiguracionCategorias.Find(idConfiguracion);

                if (conf.EsPadre == true)
                {
                    var query = db.ConfiguracionCategorias.Where(x => x.IdAcco == ID_ACCO && x.Estado == true && x.EsHijo == true && x.IdConfiguracionPadre == idConfiguracion).ToList();

                    foreach (var hijo in query)
                    {
                        hijo.Estado = false;
                        hijo.FechaModificacion = DateTime.Now;
                        hijo.IdUsuarioModifica = Convert.ToInt32(Session["UserId"]);
                        db.ConfiguracionCategorias.Attach(hijo);
                        db.Entry(hijo).State = EntityState.Modified;
                    }
                }

                conf.Estado = false;
                conf.FechaModificacion = DateTime.Now;
                conf.IdUsuarioModifica = Convert.ToInt32(Session["UserId"]);
                db.ConfiguracionCategorias.Attach(conf);
                db.Entry(conf).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {

            }
            return null;
        }
        public ActionResult EditarConfiguracionCategoriaGF(int id)
        {
            var configuracion = db.ConfiguracionCategorias.Single(x => x.IdConfiguracionCategoria == id);

            ViewBag.IdConfiguracion = configuracion.IdConfiguracionCategoria;
            ViewBag.ID_CATE_N1 = configuracion.IdCate1;
            ViewBag.ID_CATE_N2 = configuracion.IdCate2;
            ViewBag.ID_CATE_N3 = configuracion.IdCate3;
            ViewBag.ID_CATE_N4 = configuracion.IdCate4;
            ViewBag.ID_PERS_ENTI = configuracion.IdPersEnti;
            ViewBag.DetalleCabecera = configuracion.DetalleCabecera;

            return View();
        }

        public ActionResult ObtenerTicketsPadres()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            var query = db.ConfiguracionCategorias.Where(x => x.IdAcco == ID_ACCO && x.Estado == true && x.EsPadre == true).ToList();

            var result = (from x in query
                          join cat1 in db.CATEGORies on x.IdCate1 equals cat1.ID_CATE
                          join cat2 in db.CATEGORies on x.IdCate2 equals cat2.ID_CATE
                          join cat3 in db.CATEGORies on x.IdCate3 equals cat3.ID_CATE
                          join cat4 in db.CATEGORies on x.IdCate4 equals cat4.ID_CATE
                          select new
                          {
                              x.IdConfiguracionCategoria,
                              NAM = cat1.NAM_CATE + "-" + cat2.NAM_CATE + "-" + cat3.NAM_CATE + "-" + cat4.NAM_CATE
                          }).Where(x => x.NAM != "---").ToList().OrderBy(x => x.NAM);

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult BuscarCategoria()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int count = 0;

            var query = db.ConfiguracionCategorias.Where(x => x.IdAcco == ID_ACCO && x.Estado == true).ToList();

            var resultado = (from x in query
                             join cat1 in db.CATEGORies on x.IdCate1 equals cat1.ID_CATE
                             join cat2 in db.CATEGORies on x.IdCate2 equals cat2.ID_CATE
                             join cat3 in db.CATEGORies on x.IdCate3 equals cat3.ID_CATE
                             join cat4 in db.CATEGORies on x.IdCate4 equals cat4.ID_CATE
                             join pe in dbe.PERSON_ENTITY on x.IdPersEnti equals pe.ID_PERS_ENTI
                             join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                             join que in db.QUEUEs on pe.ID_QUEU equals que.ID_QUEU
                             join aq in db.ACCOUNT_QUEUE on que.ID_QUEU equals aq.ID_QUEU
                             join acq in db.ACCOUNTs on aq.ID_ACCO equals acq.ID_ACCO
                             select new
                             {
                                 x.IdConfiguracionCategoria,
                                 categoria1 = cat1.NAM_CATE,
                                 categoria2 = cat2.NAM_CATE,
                                 categoria3 = cat3.NAM_CATE,
                                 categoria4 = cat4.NAM_CATE,
                                 personal = ce.FIR_NAME + " " + ce.LAS_NAME,
                                 area = acq.ACR_ACCO + "." + que.NAM_QUEU,
                                 aq.ID_ACCO,
                                 tipoTicket = x.EsPadre == true ? "Padre" : "Hijo",
                                 detalleCabecera = x.DetalleCabecera
                             }).ToList().Where(x => x.ID_ACCO == ID_ACCO).OrderBy(x => x.IdConfiguracionCategoria);

            count = resultado.Count();

            // var lista = resultado.Skip(skip).Take(count); // no es necesario

            var json = Json(new { Data = resultado }, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;

        }


        public ActionResult BuscarCategoriaxFiltro()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int ID_CATE_N1 = 0, ID_CATE_N2 = 0, ID_CATE_N3 = 0, ID_CATE_N4 = 0, ID_PERS_ENTI = 0, ID_QUEU = 0, count = 0;

            if (Int32.TryParse(Convert.ToString(Request.Params["ID_CATE_N1"]), out ID_CATE_N1) == false)
            {
                ID_CATE_N1 = 0;
            }
            if (Int32.TryParse(Convert.ToString(Request.Params["ID_CATE_N2"]), out ID_CATE_N2) == false)
            {
                ID_CATE_N2 = 0;
            }
            if (Int32.TryParse(Convert.ToString(Request.Params["ID_CATE_N3"]), out ID_CATE_N3) == false)
            {
                ID_CATE_N3 = 0;
            }
            if (Int32.TryParse(Convert.ToString(Request.Params["ID_CATE_N4"]), out ID_CATE_N4) == false)
            {
                ID_CATE_N4 = 0;
            }
            if (Int32.TryParse(Convert.ToString(Request.Params["ID_PERS_ENTI"]), out ID_PERS_ENTI) == false)
            {
                ID_PERS_ENTI = 0;
            }
            if (Int32.TryParse(Convert.ToString(Request.Params["ID_QUEU"]), out ID_QUEU) == false)
            {
                ID_QUEU = 0;
            }

            var query = db.ConfiguracionCategorias
                .Where(x => x.IdAcco == ID_ACCO && x.Estado == true);

            if (ID_CATE_N1 != 0)
            {
                query = query.Where(x => x.IdCate1 == ID_CATE_N1);
            }
            if (ID_CATE_N2 != 0)
            {
                query = query.Where(x => x.IdCate2 == ID_CATE_N2);
            }
            if (ID_CATE_N3 != 0)
            {
                query = query.Where(x => x.IdCate3 == ID_CATE_N3);
            }
            if (ID_CATE_N4 != 0)
            {
                query = query.Where(x => x.IdCate4 == ID_CATE_N4);
            }
            if (ID_PERS_ENTI != 0)
            {
                query = query.Where(x => x.IdPersEnti == ID_PERS_ENTI);
            }

            var resultado = db.Database.SqlQuery<ListaConfiguracionesxFiltro_Result>(
                    "exec [AutomatizacionGoldField].[ListaConfiguracionesxFiltro] @IdAcco, @IdCategoria1, @IdCategoria2, @IdCategoria3, @IdCategoria4, @IdPersEnti, @IdQueu",
                                    new SqlParameter("@IdAcco", ID_ACCO),
                                    new SqlParameter("@IdCategoria1", ID_CATE_N1),
                                    new SqlParameter("@IdCategoria2", ID_CATE_N2),
                                    new SqlParameter("@IdCategoria3", ID_CATE_N3),
                                    new SqlParameter("@IdCategoria4", ID_CATE_N4),
                                    new SqlParameter("@IdPersEnti", ID_PERS_ENTI),
                                    new SqlParameter("@IdQueu", ID_QUEU)
                ).ToList();


            count = resultado.Count();

            var json = Json(new { Data = resultado }, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;
        }


        //
        // GET: /CategoryTicket/
        public ActionResult List()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            string NAM_CATE = Convert.ToString(Request.Params["filter[filters][0][value]"]);

            if (String.IsNullOrEmpty(NAM_CATE))
            {
                NAM_CATE = "";
            }

            var query = (from ac in db.ACCOUNT_CATEGORY.ToList()
                         where ac.ID_ACCO == ID_ACCO && ac.VIG_ACCO_CATE == true
                         join c in db.CATEGORies on ac.ID_CATE equals c.ID_CATE
                         //where c.ID_CATE_PARE != null //adicionando validación JQR
                         select new
                         {
                             c.NAM_CATE,
                             c.ID_CATE,
                             c.ID_CATE_PARE,
                             c.ABR_CATE,
                             c.NIV_CATE
                         }).ToList();//.Where(c=>c.NAM_CATE.ToUpper().Contains(NAM_CATE.ToUpper()));

            var result = (from c in query
                          where c.ID_CATE_PARE == null
                          join c2 in query on c.ID_CATE equals c2.ID_CATE_PARE
                          join c3 in query on c2.ID_CATE equals c3.ID_CATE_PARE
                          join c4 in query on c3.ID_CATE equals c4.ID_CATE_PARE
                          select new
                          {

                              NAM_CATE_1 = c.NAM_CATE,
                              NAM_CATE_2 = c2.NAM_CATE,
                              NAM_CATE_3 = c3.NAM_CATE,
                              NAM_CATE_4 = c4.NAM_CATE,
                              c4.ID_CATE,
                              NAM_CATE = c.ABR_CATE + '.' + c2.ABR_CATE + '.' + c3.ABR_CATE + '.' + c4.ABR_CATE
                          }).ToList();

            return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListaCategoriasBnv(int ID_CATE = 0)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            string NAM_CATE = Convert.ToString(Request.Params["filter[filters][0][value]"]);

            if (String.IsNullOrEmpty(NAM_CATE))
            {
                NAM_CATE = "";
            }

            var query = (from ac in db.ACCOUNT_CATEGORY.ToList()
                         where ac.ID_ACCO == ID_ACCO && ac.VIG_ACCO_CATE == true
                         join c in db.CATEGORies on ac.ID_CATE equals c.ID_CATE
                         //where c.ID_CATE_PARE != null //adicionando validación JQR
                         select new
                         {
                             c.NAM_CATE,
                             c.ID_CATE,
                             c.ID_CATE_PARE,
                             c.ABR_CATE,
                             c.NIV_CATE
                         }).ToList();//.Where(c=>c.NAM_CATE.ToUpper().Contains(NAM_CATE.ToUpper()));

            var result = (from c in query
                          where c.ID_CATE_PARE == null
                          join c2 in query on c.ID_CATE equals c2.ID_CATE_PARE
                          join c3 in query on c2.ID_CATE equals c3.ID_CATE_PARE
                          join c4 in query on c3.ID_CATE equals c4.ID_CATE_PARE
                          select new
                          {

                              NAM_CATE_1 = c.ID_CATE,
                              NAM_CATE_2 = c2.ID_CATE,
                              NAM_CATE_3 = c3.ID_CATE,
                              NAM_CATE_4 = c4.ID_CATE,
                              c4.ID_CATE,
                              NAM_CATE = c.ABR_CATE + '.' + c2.ABR_CATE + '.' + c3.ABR_CATE + '.' + c4.ABR_CATE
                          });
            result = result.Where(x => x.NAM_CATE_4 == ID_CATE).ToList();
            return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }
        //
        // GET: /CategoryTicket/

        public ActionResult Index()
        {
            return View(db.CATEGORies.ToList());
        }

        //
        // GET: /CategoryTicket/Details/5

        public ActionResult Details(int id = 0)
        {
            CATEGORY category = db.CATEGORies.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        //
        // GET: /CategoryTicket/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /CategoryTicket/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CATEGORY category)
        {
            if (ModelState.IsValid)
            {
                db.CATEGORies.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(category);
        }

        //
        // GET: /CategoryTicket/Edit/5

        public ActionResult Edit(int id = 0)
        {
            CATEGORY category = db.CATEGORies.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        //
        // POST: /CategoryTicket/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CATEGORY category)
        {
            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        //
        // GET: /CategoryTicket/Delete/5

        public ActionResult Delete(int id = 0)
        {
            CATEGORY category = db.CATEGORies.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        //
        // POST: /CategoryTicket/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CATEGORY category = db.CATEGORies.Find(id);
            db.CATEGORies.Remove(category);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult ListarPermisoCategoriaBNV(int ID)
        {
            var result = "";
            var cate = db.ListarPermisoCategoriaBNV().SingleOrDefault(x => x.ID_CATE == ID); //db.ListarPermisoCategoriaBNV().ToList();

            if (cate != null)
            {
                result = "OK";
            }
            else
            {
                result = "false";
            }
            return Json(result, JsonRequestBehavior.AllowGet);

        }
    }
}