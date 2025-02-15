using System;
using System.Data;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.IO;
using ERPElectrodata.Object.Ticket;
using System.Text.RegularExpressions;
using System.Web.Security;

namespace ERPElectrodata.Controllers
{
    public class DeliveryReceptionController : Controller
    {
        public CoreEntities dbc = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();
        public TicketIR tir = new TicketIR();
        //
        // GET: /DeliveryReception/
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult Details(int id = 0, string id1 = "", int id2 = 0 )
        {
            var detTicket = new DETAILS_TICKET();
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            if (id1.Length > 0)
            {
                ViewBag.Id1 = id1;
                ViewBag.Id2 = id2;
            }

            if (ID_ACCO == 55)
            {
                var tk = dbc.TICKETs.FirstOrDefault(x => x.ID_TICK == id);
                string grupo = ObtenerNombreGrupo(Convert.ToInt32(tk?.IdGrupoActivo));

                if (grupo == "IT" && Convert.ToInt32(Session["GESTOR_ACTIVOS_HUDBAY_IT"]) == 0)
                    return RedirectToAction("Index", "Error");

                if (grupo == "OT" && Convert.ToInt32(Session["GESTOR_ACTIVOS_HUDBAY_OT"]) == 0)
                    return RedirectToAction("Index", "Error");
            }


            //string COD_TICK = Convert.ToString(dbc.TICKETs.Single(x => x.ID_TICK == id).COD_TICK);
            //string ID_TYPE_FORM = Convert.ToString(dbc.TICKETs.Single(x => x.ID_TICK == id).ID_TYPE_FORM);
            var query = (from x in dbc.TYPE_FORMAT
                             join t in dbc.TICKETs on x.ID_TYPE_FORM equals t.ID_TYPE_FORM
                             select new
                             {
                                 x.NAM_TYPE_FORM,
                                 t.ID_TICK,
                                 t.COD_TICK,
                                 t.ID_STAT_END,
                                 t.ID_CATE,
                             }).Where(x => x.ID_TICK == id).First();

                var query1 = dbc.DETAIL_TICKET_FORMAT.Where(dtf => dtf.ID_TICK == id).ToList();

                var result = (from t in query1.ToList()
                              join a in dbc.ASSETs on t.ID_ASSE equals a.ID_ASSE
                              join ta in dbc.TYPE_ASSET on a.ID_TYPE_ASSE equals ta.ID_TYPE_ASSE
                              select new
                              {
                                  ID_TICK = t.ID_TICK,
                                  ID_TYPE_ASSE = ta.ID_TYPE_ASSE,
                              });

                int reporteCel = 0, reporteRadio = 0, reporteEquipo = 0;

                foreach (var tipoActivo in result)
                {
                    if (tipoActivo.ID_TYPE_ASSE == 2)
                    {
                        reporteCel = 1;
                    }
                    else if (tipoActivo.ID_TYPE_ASSE == 14)
                    {
                        reporteRadio = 1;
                    }
                    else
                    {
                        reporteEquipo = 1;
                    }
                }

                string UserName = Convert.ToString(Session["UserName"]);
                string[] rolesArray = Roles.GetRolesForUser(UserName);

                foreach (string xc in rolesArray)
                {
                    int i = Array.IndexOf(rolesArray, xc);
                    if (xc == "SERVICE DESK")
                    {
                        ViewBag.ACCESO_EDIT = 1;
                    }
                    if (xc == "ADMINISTRADOR")
                    {
                        ViewBag.ACCESO_SEND_SURVEY = 1;
                    }

                }

                if (ID_ACCO == 60) ViewBag.ACCESO_EDIT = 1;

                CategoriasxIdCateTicket_Result listaCat = dbc.CategoriasxIdCateTicket(query.ID_CATE).First();

                ViewData["COD_TICK"] = query.COD_TICK;
                ViewData["ID_TICK"] = id;
                ViewData["NAM_TYPE_FORM"] = query.NAM_TYPE_FORM;
                detTicket.ID_TICK = id;
                ViewData["ID_STAT_END"] = query.ID_STAT_END;
                ViewData["reporteCel"] = reporteCel;
                ViewData["reporteRadio"] = reporteRadio;
                ViewData["reporteEquipo"] = reporteEquipo;
                ViewBag.IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
                ViewBag.Cate1 = listaCat.CATEGORIA_1;
                ViewBag.Cate2 = listaCat.CATEGORIA_2;
                ViewBag.Cate3 = listaCat.CATEGORIA_3;
                ViewBag.Cate4 = listaCat.CATEGORIA_4;
                ViewBag.Cate5 = listaCat.CATEGORIA_5;
                ViewBag.Cate6 = listaCat.CATEGORIA_6;
                ViewBag.ID_ACCO = ID_ACCO;
                return View(detTicket);

            

           
        }

        public ActionResult ListAssetByID_PERS_ENTI(int id = 0)
        {

            string txt = Convert.ToString(Request.Params["filter[filters][0][value]"]);
            if (txt == null) { txt = ""; }

            var query1 = dbc.CLIENT_ASSET.Where(ca => ca.ID_PERS_ENTI == id && ca.DAT_END == null)
                .Join(dbc.ASSETs, x => x.ID_ASSE, a => a.ID_ASSE, (x, a) => new
                {
                    x.ID_CLIE_ASSE,
                    x.ID_PERS_ENTI,
                    x.ID_COND,
                    a.ID_MANU,
                    a.ID_COMM_MODE,
                    a.COD_ASSE,
                    a.ID_TYPE_ASSE,
                    a.ID_ASSE,
                    a.SER_NUMB,
                    a.NAM_EQUI
                })
                .Join(dbc.TYPE_ASSET, x => x.ID_TYPE_ASSE, ta => ta.ID_TYPE_ASSE, (x, ta) => new
                {
                    x.ID_ASSE,
                    x.COD_ASSE,
                    x.SER_NUMB,
                    x.ID_TYPE_ASSE,
                    x.ID_MANU,
                    x.ID_COND,
                    x.ID_COMM_MODE,
                    x.NAM_EQUI,
                    ta.NAM_TYPE_ASSE
                })
                .Join(dbc.MANUFACTURERs, x => x.ID_MANU, m => m.ID_MANU, (x, m) => new
                {
                    x.ID_ASSE,
                    x.COD_ASSE,
                    x.SER_NUMB,
                    x.ID_TYPE_ASSE,
                    x.ID_MANU,
                    x.ID_COND,
                    x.NAM_TYPE_ASSE,
                    x.ID_COMM_MODE,
                    x.NAM_EQUI,
                    m.NAM_MANU
                })
                .Join(dbc.CONDITIONs, x => x.ID_COND, c => c.ID_COND, (x, c) => new
                {
                    x.ID_ASSE,
                    x.COD_ASSE,
                    x.SER_NUMB,
                    x.ID_TYPE_ASSE,
                    x.ID_MANU,
                    x.ID_COND,
                    x.NAM_TYPE_ASSE,
                    x.NAM_MANU,
                    x.ID_COMM_MODE,
                    x.NAM_EQUI,
                    c.ID_STAT_ASSE,
                    c.NAM_COND
                })
                .Join(dbc.STATUS_ASSET, x => x.ID_STAT_ASSE, sa => sa.ID_STAT_ASSE, (x, sa) => new
                {
                    x.ID_ASSE,
                    x.COD_ASSE,
                    x.SER_NUMB,
                    x.ID_TYPE_ASSE,
                    x.ID_MANU,
                    x.ID_COND,
                    x.NAM_TYPE_ASSE,
                    x.NAM_MANU,
                    x.ID_STAT_ASSE,
                    x.ID_COMM_MODE,
                    x.NAM_EQUI,
                    sa.NAM_STAT_ASSE,
                    x.NAM_COND,
                })
                .Join(dbc.COMMERCIAL_MODEL, x => x.ID_COMM_MODE, c => c.ID_COMM_MODE, (x, c) => new
                {
                    x.ID_ASSE,
                    x.COD_ASSE,
                    x.SER_NUMB,
                    x.ID_TYPE_ASSE,
                    x.ID_MANU,
                    x.ID_COND,
                    x.NAM_TYPE_ASSE,
                    x.NAM_MANU,
                    x.ID_STAT_ASSE,
                    x.ID_COMM_MODE,
                    x.NAM_EQUI,
                    x.NAM_STAT_ASSE,
                    c.NAM_COMM_MODE,
                    x.NAM_COND
                })
                .Where(x => x.NAM_TYPE_ASSE.Contains(txt) || x.COD_ASSE.Contains(txt) || x.SER_NUMB.Contains(txt) ||
                    x.NAM_MANU.Contains(txt) || x.NAM_COMM_MODE.Contains(txt) || (x.NAM_STAT_ASSE + " - " + x.NAM_COND).Contains(txt));

            var result = (from x in query1.OrderBy(x => x.NAM_TYPE_ASSE).ThenBy(x => x.COD_ASSE).ToList()
                          select new
                          {
                              x.ID_ASSE,
                              x.COD_ASSE,
                              x.SER_NUMB,
                              x.ID_TYPE_ASSE,
                              x.ID_MANU,
                              x.ID_COND,
                              NAM_TYPE_ASSE = x.NAM_TYPE_ASSE.ToUpper(),
                              x.NAM_MANU,
                              x.NAM_COMM_MODE,
                              x.ID_STAT_ASSE,
                              x.NAM_STAT_ASSE,
                              x.NAM_EQUI,
                              NAM_ASSE = x.NAM_TYPE_ASSE + " : " + x.COD_ASSE,
                              COND = x.NAM_STAT_ASSE + " - " + x.NAM_COND,
                          }).ToList();

            return Json(new { Data = result, Count = query1.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListByIdJson(int id = 0)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            if (ID_ACCO == 60)
            {
                var resultB = dbc.ObtenerDetalleTicketActivoBNV(id)
                              .Select(t => new
                              {
                                  t.ID_TICK,
                                  t.ID_STAT,
                                  t.PHOTO,
                                  t.CODE,
                                  t.CLIE,
                                  t.FEC_TICK,
                                  t.NAM_AREA,
                                  t.NAM_LOCA,
                                  t.NAM_SITE,
                                  t.NAM_SOUR,
                                  t.NAM_STAT,
                                  t.SUM_TICK,
                                  EXP_TIME = tir.ExpirationTime(t.ID_TICK, Convert.ToInt32(ObtenerHoraSLA(t.ID_TICK))),
                                  t.USER,
                                  DATE_SCHE = ScheduleDate(Convert.ToInt32(t.ID_TICK)),
                                  t.CREATE_TICK,
                                  t.MODIFIED_TICK,
                                  ADJUNTOS = AdjuntosD(id, Convert.ToInt32(t.ADJUNTOS)),
                                  t.SubClass,
                                  t.Class,
                                  t.SubCate,
                                  t.Cate,
                                  t.NAM_PRIO,
                                  ATTA_TOT = TotAtta(Convert.ToInt32(t.ID_TICK))
                              });
                return Json(new { data = resultB }, JsonRequestBehavior.AllowGet);
            }

            var query1 = dbc.TICKETs.Where(t => t.ID_TICK == id);

            var query2 = dbe.ACCOUNT_ENTITY.Where(ae => ae.ID_ACCO == ID_ACCO)
                         .Join(dbe.PERSON_ENTITY, x => x.ID_PERS_ENTI, pe => pe.ID_PERS_ENTI, (x, pe) => new
                         {
                             x.ID_PERS_ENTI,
                             pe.ID_ENTI2,
                             pe.ID_FOTO,
                             pe.ID_AREA
                         })
                         .Join(dbe.CLASS_ENTITY, x => x.ID_ENTI2, ce => ce.ID_ENTI, (x, ce) => new
                         {
                             x.ID_PERS_ENTI,
                             x.ID_FOTO,
                             x.ID_ENTI2,
                             x.ID_AREA,
                             ce.ID_ENTI,
                             ce.FIR_NAME,
                             ce.LAS_NAME,
                             ce.MOT_NAME,
                             ce.UserId
                         })
                         .Join(dbe.AREAs, x => x.ID_AREA, a => a.ID_AREA, (x, a) => new
                         {
                             x.ID_PERS_ENTI,
                             x.ID_FOTO,
                             x.ID_ENTI2,
                             x.ID_AREA,
                             x.ID_ENTI,
                             x.FIR_NAME,
                             x.LAS_NAME,
                             x.MOT_NAME,
                             x.UserId,
                             a.NOM_AREA
                         });
            var q2 = query2.ToList();

            var result = (from t in query1.ToList()
                          join c in query2 on t.ID_PERS_ENTI_END equals c.ID_PERS_ENTI
                          join l in dbc.LOCATIONs on t.ID_LOCA equals l.ID_LOCA into ls
                          from lx in ls.DefaultIfEmpty()
                          join so in dbc.SOURCEs on t.ID_SOUR equals so.ID_SOUR
                          join st in dbc.STATUS on t.ID_STAT_END equals st.ID_STAT
                          join p in dbc.PRIORITies on t.ID_PRIO equals p.ID_PRIO
                          //join s in dbc.SITEs on lx.ID_SITE equals s.ID_SITE into sd
                          //from sx in sd.DefaultIfEmpty()
                          join u in query2 on t.UserId equals u.UserId
                          join c4 in dbc.CATEGORies on t.ID_CATE equals c4.ID_CATE
                          join c3 in dbc.CATEGORies on c4.ID_CATE_PARE equals c3.ID_CATE
                          join c2 in dbc.CATEGORies on c3.ID_CATE_PARE equals c2.ID_CATE
                          join CATE1 in dbc.CATEGORies on (c2 == null ? 0 : c2.ID_CATE_PARE) equals CATE1.ID_CATE into CATE_1
                          from c1 in CATE_1.DefaultIfEmpty()
                          select new
                          {
                              ID_TICK = t.ID_TICK,
                              st.ID_STAT,
                              PHOTO = u.ID_FOTO,
                              CODE = t.COD_TICK,
                              CLIE = c.FIR_NAME + " " + c.LAS_NAME + " " + (c.MOT_NAME == null ? "" : c.MOT_NAME),
                              FEC_TICK = String.Format("{0:G}", t.FEC_TICK),
                              NAM_AREA = c.NOM_AREA.ToLower(),
                              NAM_LOCA = (lx == null ? String.Empty : lx.NAM_LOCA),
                              NAM_SITE = (lx == null ? String.Empty : dbc.SITEs.Where(x => x.ID_SITE == lx.ID_SITE).Single().NAM_SITE),
                              NAM_SOUR = so.NAM_SOUR,
                              NAM_STAT = st.NAM_STAT,
                              t.SUM_TICK,
                              //EXP_TIME = tir.ExpirationTime(t.ID_TICK, p.HOU_PRIO.Value),
                              EXP_TIME = tir.ExpirationTime(t.ID_TICK, Convert.ToInt32(ObtenerHoraSLA(t.ID_TICK))),
                              USER = u.FIR_NAME + " " + u.LAS_NAME,
                              DATE_SCHE = ScheduleDate(Convert.ToInt32(t.ID_TICK)),
                              CREATE_TICK = String.Format("{0:G}", t.FEC_TICK),
                              MODIFIED_TICK = String.Format("{0:G}", t.MODIFIED_TICK),
                              ADJUNTOS = AdjuntosD(id, Convert.ToInt32(t.ID_TYPE_FORM)),
                              SubClass = c4.NAM_CATE,
                              Class = c3.NAM_CATE,
                              SubCate = c2.NAM_CATE,
                              Cate = (c1 == null ? "" : c1.NAM_CATE.ToLower()),
                              p.NAM_PRIO,
                              ATTA_TOT = TotAtta(Convert.ToInt32(t.ID_TICK))
                          });

            var q = result.ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public string AdjuntosD(int id, int idTF)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            string ruta = "Delivery";
            if (idTF == 2) { ruta = "Reception"; }
            string Adjun = "";
            try
            {
                var query = dbc.ATTACHED_TICKET_FORMAT.Where(a => a.ID_TICK == id);
                foreach (ATTACHED_TICKET_FORMAT atta in query)
                {
                    Adjun = Adjun + "<li><a href='/Adjuntos/" + ruta + "/" + ID_ACCO.ToString() + "/" + atta.NAM_ATTA + "_" + atta.ID_ATTA_TICK_FORM + atta.EXT_ATTA + " ' TARGET='_BLANK'>" + atta.NAM_ATTA + atta.EXT_ATTA + "</a></li>";
                }

                return Adjun;
            }
            catch
            {
                return String.Empty;
            }
        }

        public int TotAtta(int id)
        {
            int atta_tick = dbc.ATTACHED_TICKET_FORMAT.Where(a => a.ID_TICK == id).Count();

            int atta_all = (from x in dbc.DETAILS_TICKET.Where(dt => dt.ID_TICK == id)
                            join adt in dbc.ATTACHED_TICKET_FORMAT on x.ID_DETA_TICK equals adt.ID_DETA_TICK
                            select new
                            {
                                adt.ID_ATTA_TICK_FORM
                            }).Count();

            return atta_tick + atta_all;
        }

        public String ExpTime(int id_stat_end, int id_prio, int hou_prio, DateTime date)
        {
            if (id_prio != 5)
            {
                if (id_stat_end == 1 || id_stat_end == 3)
                {
                    var time = DateTime.Now.Subtract(Convert.ToDateTime(date)).Days * 24 +
                                                DateTime.Now.Subtract(Convert.ToDateTime(date)).Hours;

                    return Convert.ToString(Convert.ToInt32(hou_prio) - Convert.ToInt32(time)) + " Hours";
                }
                else
                {
                    return "Stopped";
                }
            }
            else
            {
                return "Planing";
            }
        }
        public String ScheduleDate(int ID_TICK)
        {
            DateTime FechaScheduled;

            try
            {
                FechaScheduled = Convert.ToDateTime(dbc.DETAILS_TICKET.Where(x => x.ID_TICK == ID_TICK)
                                .Where(x => x.ID_STAT == 5)
                                .Where(x => x.ID_TYPE_DETA_TICK == 3)
                                .OrderByDescending(x => x.CREATE_DETA_INCI)
                                .Take(1).Single().FEC_SCHE);
            }
            catch
            {
                FechaScheduled = Convert.ToDateTime(dbc.TICKETs.Single(x => x.ID_TICK == ID_TICK).FEC_INI_TICK);
            }

            return String.Format("{0:G}", FechaScheduled);
        }

        public ActionResult ListAssetByID_TICK(int id = 0)
        {
            var query1 = dbc.DETAIL_TICKET_FORMAT.Where(dtf => dtf.ID_TICK == id).ToList();

            var result = (from t in query1.ToList()
                          join a in dbc.ASSETs on t.ID_ASSE equals a.ID_ASSE
                          join ta in dbc.TYPE_ASSET on a.ID_TYPE_ASSE equals ta.ID_TYPE_ASSE
                          join cm in dbc.COMMERCIAL_MODEL on a.ID_COMM_MODE equals cm.ID_COMM_MODE into comercial
                          from com in comercial.DefaultIfEmpty()
                          join f in dbc.MANUFACTURERs on a.ID_MANU equals f.ID_MANU
                          select new
                          {
                              ID_TICK = t.ID_TICK,
                              a.ID_ASSE,
                              NAM_TYPE_ASSE = (ta.NAM_TYPE_ASSE == null ? string.Empty : ta.NAM_TYPE_ASSE),
                              CODE = (a.COD_ASSE == null ? string.Empty : a.COD_ASSE),
                              NAM_EQUI = (a.NAM_EQUI == null ? string.Empty : a.NAM_EQUI),
                              SER_NUMB = (a.SER_NUMB == null ? string.Empty : a.SER_NUMB),
                              NAM_COMM_MODE = (com == null ? string.Empty : com.NAM_COMM_MODE),
                              NAM_MANU = (f.NAM_MANU == null ? string.Empty : f.NAM_MANU),
                          });

            var q = result.ToList();
            return Json(new { data = result}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerUsuarioResponsable(int id = 0)
        {
            var query = dbc.ObtenerUsuarioResponsable(id);
            
            return Json(new { data = query, }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListDetailsLicencia(string idLicencia = "", int idPersonEntity = 0)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            List<int> licenciaIds = idLicencia.Split(',').Select(int.Parse).ToList();

            List<object> resultList = new List<object>();

            foreach (int licenciaId in licenciaIds)
            {
                var result = dbc.ListarDetalleLicencia(idPersonEntity, licenciaId);

                resultList.AddRange(result);
            }

            return Json(new { data = resultList }, JsonRequestBehavior.AllowGet);
        }



        public ActionResult ListDetailsTicketByID_TICK(int id = 0)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            int id_TF = dbc.TICKETs.Single(x => x.ID_TICK == id).ID_TYPE_FORM.Value;

            var query1 = dbc.DETAILS_TICKET.Where(dt => dt.ID_TICK == id).OrderByDescending(x => x.CREATE_DETA_INCI);

            int tt = 0;
            tt = query1.Count();

            var query2 = dbe.ACCOUNT_ENTITY.Where(ae => ae.ID_ACCO == ID_ACCO)
                         .Join(dbe.PERSON_ENTITY, x => x.ID_PERS_ENTI, pe => pe.ID_PERS_ENTI, (x, pe) => new
                         {
                             x.ID_PERS_ENTI,
                             pe.ID_ENTI2,
                             pe.ID_FOTO,
                         })
                         .Join(dbe.CLASS_ENTITY, x => x.ID_ENTI2, ce => ce.ID_ENTI, (x, ce) => new
                         {
                             x.ID_PERS_ENTI,
                             x.ID_ENTI2,
                             x.ID_FOTO,
                             ce.ID_ENTI,
                             ce.FIR_NAME,
                             ce.LAS_NAME,
                             ce.UserId
                         });

            int tt2 = 0;
            tt2 = query2.Count();

            var result = (from q1 in query1.ToList()
                          join s in dbc.STATUS on q1.ID_STAT equals s.ID_STAT into ls
                          from x in ls.DefaultIfEmpty()

                          join q2 in query2 on q1.UserId equals q2.UserId
                          join t in dbc.TYPE_DETAILS_TICKET on q1.ID_TYPE_DETA_TICK equals t.ID_TYPE_DETA_TICK
                          select new
                          {
                              q1.COM_DETA_TICK,
                              q1.ID_TYPE_DETA_TICK,
                              q1.ID_DETA_TICK,
                              FEC_SCHE = String.Format("{0:G}", q1.FEC_SCHE),
                              NAM_STAT = (x == null ? String.Empty : x.NAM_STAT),
                              PHOTO = q2.ID_FOTO,
                              CREATE_DETA_INCI = String.Format("{0:G}", q1.CREATE_DETA_INCI),
                              PERS = (q2.FIR_NAME + " " + q2.LAS_NAME),
                              NAM_TYPE_DETA_TICK = t.NAM_TYPE_DETA_TICK,
                              ADJUNTOS = AdjuntosDetailsTicket(q1.ID_DETA_TICK, id_TF),
                          });

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarComentariosActivos(int id = 0)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            int id_TF = dbc.TICKETs.Single(x => x.ID_TICK == id).ID_TYPE_FORM.Value;

            var query1 = dbc.DETAILS_TICKET.Where(dt => dt.ID_TICK == id).OrderByDescending(x => x.CREATE_DETA_INCI);

            int tt = 0;
            tt = query1.Count();

            var query2 = dbe.ACCOUNT_ENTITY.Where(ae => ae.ID_ACCO == ID_ACCO)
                         .Join(dbe.PERSON_ENTITY, x => x.ID_PERS_ENTI, pe => pe.ID_PERS_ENTI, (x, pe) => new
                         {
                             x.ID_PERS_ENTI,
                             pe.ID_ENTI2,
                             pe.ID_FOTO,
                         })
                         .Join(dbe.CLASS_ENTITY, x => x.ID_ENTI2, ce => ce.ID_ENTI, (x, ce) => new
                         {
                             x.ID_PERS_ENTI,
                             x.ID_ENTI2,
                             x.ID_FOTO,
                             ce.ID_ENTI,
                             ce.FIR_NAME,
                             ce.LAS_NAME,
                             ce.UserId
                         });


            var result = (from q1 in query1.ToList()
                          join s in dbc.STATUS on q1.ID_STAT equals s.ID_STAT into ls
                          from x in ls.DefaultIfEmpty()

                          join q2 in query2 on q1.UserId equals q2.UserId
                          join t in dbc.TYPE_DETAILS_TICKET on q1.ID_TYPE_DETA_TICK equals t.ID_TYPE_DETA_TICK
                          join al in dbe.ACTIVITY_LOG.Where(x => x.COD_SUBTYPE_ACT == id) on q1.ID_DETA_TICK equals al.DETAILS_TICKETS into det
                          from act in det.DefaultIfEmpty()
                          select new
                          {
                              COM_DETA_TICK = ModificarDetalle(q1.COM_DETA_TICK),
                              q1.ID_TYPE_DETA_TICK,
                              q1.ID_DETA_TICK,
                              FEC_SCHE = String.Format("{0:G}", q1.FEC_SCHE),
                              NAM_STAT = (x == null ? String.Empty : x.NAM_STAT),
                              PHOTO = q2.ID_FOTO,
                              CREATE_DETA_INCI = String.Format("{0:G}", q1.CREATE_DETA_INCI),
                              PERS = (q2.FIR_NAME + " " + q2.LAS_NAME),
                              NAM_TYPE_DETA_TICK = t.NAM_TYPE_DETA_TICK,
                              ADJUNTOS = AdjuntosDetailsTicket(q1.ID_DETA_TICK, id_TF),
                              DATE_INIC = (act == null ? "-" : string.Format("{0:G}", act.DATE_INIC)),
                              DATE_END = (act == null ? "-" : string.Format("{0:G}", act.DATE_END)),
                          });
            int tt2 = 0;
            tt2 = result.Count();

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public string ModificarDetalle(string detalle)
        {
            var comentario = detalle;
            if (comentario.Contains("/#/DetailsTicket/Index/"))
            {
                var nuevoComentario = comentario.Replace("/#/DetailsTicket/Index/", "/DetailsTicket/Index/");
                comentario= nuevoComentario;
            }
            if (comentario.Contains("/#/DeliveryReception/Details/"))
            {
                var nuevoComentario = comentario.Replace("/#/DeliveryReception/Details/", "/DeliveryReception/Details/");
                comentario = nuevoComentario;
            }
            return comentario;
        }
        public string AdjuntosDetailsTicket(int id, int id_TF)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            string ruta = "Delivery";
            if (id_TF == 2) { ruta = "Reception"; }
            string Adjun = "";
            try
            {
                var query = dbc.ATTACHED_TICKET_FORMAT.Where(a => a.ID_DETA_TICK == id);
                foreach (ATTACHED_TICKET_FORMAT atta in query)
                {
                    Adjun = Adjun + "<li><a href='/Adjuntos/" + ruta + "/" + ID_ACCO.ToString() + "/" + atta.NAM_ATTA + "_" + atta.ID_ATTA_TICK_FORM + atta.EXT_ATTA + " ' TARGET='_BLANK'>" + atta.NAM_ATTA + atta.EXT_ATTA + "</a></li>";
                }

                return Adjun;
            }
            catch
            {
                return String.Empty;
            }
        }

        [Authorize]
        public ActionResult Create(int id = 0)
        {
            int tieneCambio = 0;
            int IdticketPadre = 0;
            int cambiotktPadre = 0;

            if (id != 0)
            {
                var queryCargo = dbc.TICKETs.Single(x => x.ID_TICK == id);
                var cargo = dbe.ObtieneCargouUsuario(Convert.ToInt32(queryCargo.ID_PERS_ENTI_END.Value)).Single();
                int cambio = Convert.ToInt32(dbc.CHANGE_REQUEST.Where(x => x.IdTicket == id && x.Activo == true).Count());

                try
                {
                    if (cambio == 0)
                    {
                        IdticketPadre = Convert.ToInt32(dbc.TICKETs.Single(x => x.ID_TICK == id).ID_TICK_PARENT);
                        cambiotktPadre = Convert.ToInt32(dbc.CHANGE_REQUEST.Where(x => x.IdTicket == IdticketPadre && x.Activo == true).Count());
                        if (cambiotktPadre == 1)
                        {
                            tieneCambio = 1;
                        }
                    }
                    else
                    {
                        tieneCambio = 1;
                    }
                }
                catch
                {
                }
                int IdChar = Convert.ToInt32(cargo.ID_CHAR);
                var perfilActivo = dbc.PerfilActivos.Where(x => x.IdChar == IdChar && x.Estado == true).ToList();
                //if (perfilActivo.Count() > 0)
                ViewBag.TienePerfil = 1;
                //else
                //    ViewBag.TienePerfil = 0;
                var ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                string locacion = "";
                string dni = "";
                if (ID_ACCO == 56 || ID_ACCO == 57 || ID_ACCO == 58)
                {
                    var queryD = dbe.ObtieneLocacionUsuario(Convert.ToInt32(queryCargo.ID_PERS_ENTI_END.Value)).Single();
                    locacion = queryD.Locacion;
                    dni = queryD.NUM_TYPE_DI;
                }

                var ID_PERS_ASSI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                var ID_QUEU = Convert.ToInt32(Session["ID_QUEU"]);

                if (ID_ACCO == 4)//ELECTRODATA
                {
                    if (Convert.ToInt32(Session["SOPORTE"] == null ? 0 : Session["SOPORTE"]) != 1)
                    {
                        return Content("Necesitas permisos para acceder a esta sección.");
                    }
                }
                else if (ID_ACCO == 55)
                {
                    if (Convert.ToInt32(Session["GESTOR_ACTIVOS_HUDBAY_IT"]) == 1)
                    {
                        int idGrupoIT = ObtenerIdGrupo("IT");
                        ViewBag.IdGrupoIT = idGrupoIT;
                    }
                    else if (Convert.ToInt32(Session["GESTOR_ACTIVOS_HUDBAY_OT"]) == 1)
                    {
                        return RedirectToAction("CreateOT", "DeliveryReception");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Error");
                    }
                }

                var ticket = new TICKET();
                ticket.FEC_TICK = DateTime.Now;
                ViewBag.FEC_TICK = String.Format("{0:g}", ticket.FEC_TICK);
                ViewBag.Cargo = cargo.CARGO;
                ViewBag.Locacion = locacion;
                ViewBag.DNI = dni;
                ViewBag.IdCargo = cargo.ID_CHAR;
                ViewBag.IdPersEnti = queryCargo.ID_PERS_ENTI_END;
                ViewBag.IdTicket = id;
                ViewBag.TieneCambio = tieneCambio;

                ViewBag.IdAcco = ID_ACCO;
                ticket.ID_TYPE_TICK = 2;
                ticket.ID_CATE = 119;
                //Razon para asignar
                ticket.ID_PERS_ENTI_END = ID_PERS_ASSI;
                ticket.ID_PERS_ENTI_ASSI = ID_PERS_ASSI;
                ticket.ID_QUEU = ID_QUEU;
                ticket.ID_PRIO = 1;
                ticket.ID_STAT = 1;
                ticket.SUM_TICK = "";

                return View(ticket);
            }
            else
            {
                var ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                var ID_PERS_ASSI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                var ID_QUEU = Convert.ToInt32(Session["ID_QUEU"]);

                if (ID_ACCO == 4)//ELECTRODATA
                {
                    if (Convert.ToInt32(Session["SOPORTE"] == null ? 0 : Session["SOPORTE"]) != 1)
                    {
                        return Content("Necesitas permisos para acceder a esta sección.");
                    }
                }

                var ticket = new TICKET();

                if (ID_ACCO == 60)
                {
                    int idGrupoActivo = ObtenerIdGrupoUsuarioBNV();

                    if (idGrupoActivo != 0)
                    {
                        string grupoActivo = ObtenerNomGrupoUsuarioBNV(idGrupoActivo);
                        ticket.IdGrupoActivo = idGrupoActivo;
                        ticket.ID_COMP = ObtenerUMineraUsuarioBNV(Convert.ToInt32(Session["ID_PERS_ENTI"]), idGrupoActivo);
                        ViewBag.Grupo = grupoActivo;
                    }
                    else
                    {
                        return RedirectToAction("Index", "Error");
                    }
                }
                else if (ID_ACCO == 55)
                {
                    if (Convert.ToInt32(Session["GESTOR_ACTIVOS_HUDBAY_IT"]) == 1)
                    {
                        int idGrupoIT = ObtenerIdGrupo("IT");
                        ViewBag.IdGrupoIT = idGrupoIT;
                    }
                    else if (Convert.ToInt32(Session["GESTOR_ACTIVOS_HUDBAY_OT"]) == 1)
                    {
                        return RedirectToAction("CreateOT", "DeliveryReception");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Error");
                    }
                }

                ticket.FEC_TICK = DateTime.Now;
                ViewBag.FEC_TICK = String.Format("{0:g}", ticket.FEC_TICK);
                ViewBag.IdAcco = ID_ACCO;
                ViewBag.TienePerfil = 1;
                ViewBag.Cargo = "";
                ViewBag.Locacion = "";
                ViewBag.DNI = "";
                ViewBag.IdCargo = 0;
                ViewBag.IdPersEnti = 0;
                ViewBag.IdTicket = id;
                ViewBag.TieneCambio = tieneCambio;
                ticket.ID_TYPE_TICK = 2;
                ticket.ID_CATE = 119;
                //Razon para asignar
                ticket.ID_PERS_ENTI_END = ID_PERS_ASSI;
                ticket.ID_PERS_ENTI_ASSI = ID_PERS_ASSI;
                ticket.ID_QUEU = ID_QUEU;
                ticket.ID_PRIO = 1;
                ticket.ID_STAT = 1;
                ticket.SUM_TICK = "";
                return View(ticket);
            }

        }


        //ACTIONRESULT PARA MANDAR LOS DATOS DE CARGO Y AREA SEGUN ID_PERS_ENTI
        public ActionResult ListarCargoYArea(int IdPersEnti = 0)
        {
            PERSON_ENTITY obj_Person = new PERSON_ENTITY(); ;
            AREA obj_Area = new AREA(); ;


            obj_Person = dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == IdPersEnti).FirstOrDefault();

            var obj_CargoEnviar = "";

            if (obj_Person != null)
            {
                
                obj_Area = dbe.AREAs.Where(x => x.ID_AREA == obj_Person.ID_AREA).FirstOrDefault();
                obj_CargoEnviar = obj_Person.CAR_PERS;
            }
            
            var obj_AreaEnviar = "";
            if (obj_Area != null)
            {
                obj_AreaEnviar = obj_Area.NOM_AREA;
            }

            //var obj_Respuesta = new object();
            //obj_Respuesta.Area = obj_Area.NOM_AREA;

            return Json(new { DataArea = obj_AreaEnviar, DataCargo = obj_CargoEnviar }, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost, ValidateInput(false)]
        //public ActionResult EntregaReasignacion(IEnumerable<HttpPostedFileBase> files, TICKET ticket)
        //{
        //    //Validando si los Asset siguen siendo UNASSIGNED
        //    string idsActivo = Convert.ToString(Request.Form["idsActivo"]);
        //    string idsCondicion = Convert.ToString(Request.Form["idsCondicion"]);

        //    if (idsActivo.Length > 0)
        //    {
        //        idsActivo = idsActivo.Substring(0, idsActivo.Length - 1);
        //        string[] ids_asset = idsActivo.Split(',');
        //        string msn = "";

        //        foreach (string ida in ids_asset)
        //        {
        //            var idsearch = Convert.ToInt32(ida);
        //            var query = dbc.ASSETs
        //                .Where(a => a.ID_ASSE == idsearch)
        //                .Join(dbc.CLIENT_ASSET, a => a.ID_ASSE, ca => ca.ID_ASSE, (x, ca) => new
        //                {
        //                    x.ID_ASSE,
        //                    ca.ID_COND,
        //                    ca.DAT_END
        //                }).Where(x => x.DAT_END == null && x.ID_COND != 9 && x.ID_COND != 3);

        //            int cant = query.Count();
        //            //if (cant > 0)
        //            //{
        //            //    msn = msn + idsearch.ToString() + ",";
        //            //}
        //        }
        //        if (msn.Length > 0)
        //        {
        //            return Content("<script type='text/javascript'> function init() {" +
        //                "if(top.uploadDone) top.uploadDone('" + msn + "','2');}window.onload=init;</script>");
        //        }
        //        else
        //        { //Guardando el registro
        //            var ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
        //            var ID_PERS_ASSI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
        //            var ID_QUEU = Convert.ToInt32(Session["ID_QUEU"]);
        //            var ID_USER = Convert.ToInt32(Session["UserId"]);
        //            int ID_PER_ENTI, ID_PRIO, ID_LOCA, ID_AREA, HOUR;

        //            var objPersonEntity = dbe.PERSON_ENTITY.Single(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI.Value);

        //            DateTime FEC_TICK;
        //            try
        //            {
        //                ID_PER_ENTI = ticket.ID_PERS_ENTI.Value;
        //                ID_LOCA = ticket.ID_LOCA.Value;
        //                FEC_TICK = ticket.FEC_TICK.Value;
        //                ID_AREA = Convert.ToInt32(objPersonEntity.ID_AREA.Value);
        //                ticket.ID_AREA = ID_AREA;
        //                ID_PRIO = Convert.ToInt32(objPersonEntity.ID_PRIO.Value);
        //                HOUR = Convert.ToInt32(dbc.PRIORITies.First(x => x.ID_PRIO == ID_PRIO).HOU_PRIO.Value);
        //                //try
        //                //{
        //                //    ID_LOCA = Convert.ToInt32(dbe.PERSON_LOCATION.Single(pl => pl.ID_PERS_ENTI == ID_PER_ENTI && pl.END_DATE == null && pl.VIG_LOCA == true).ID_LOCA);
        //                //    ticket.ID_LOCA = ID_LOCA;
        //                //}
        //                //catch { ID_LOCA = 0; ticket.ID_LOCA = ID_LOCA; }
        //                ticket.ID_SOUR = 3;
        //            }
        //            catch
        //            {   //Falta informacion
        //                return Content("<script type='text/javascript'> function init() {" +
        //                "if(top.uploadDone) top.uploadDone('" + msn + "','0');}window.onload=init;</script>");
        //            }

        //            //bool est = new bool();
        //            //est = ModelState.IsValidField("ID_ACCO");
        //            //est = ModelState.IsValidField("ID_TYPE_TICK");
        //            //est = ModelState.IsValidField("ID_CLIE");

        //            //if (ModelState.IsValid)
        //            if (ticket.ID_PERS_ENTI != null)
        //            {
        //                string Code = null;
        //                try
        //                {
        //                    ticket.ID_ACCO = ID_ACCO;
        //                    ticket.ID_TYPE_TICK = 2;
        //                    ticket.ID_PERS_ENTI = ID_PER_ENTI;
        //                    ticket.ID_PERS_ENTI_END = ID_PER_ENTI;
        //                    ticket.ID_PERS_ENTI_ASSI = ID_PERS_ASSI;
        //                    ticket.ID_AREA = ID_AREA;
        //                    ticket.ID_QUEU = ID_QUEU;
        //                    ticket.ID_PRIO = 5;
        //                    ticket.ID_STAT = 1;
        //                    ticket.ID_STAT_END = 1;
        //                    ticket.ID_SOUR = 3;
        //                    ticket.FEC_TICK = FEC_TICK;
        //                    //ticket.SUM_TICK = ticket.SUM_TICK;
        //                    ticket.REM_CTRL_TICK = false;
        //                    ticket.UserId = ID_USER;
        //                    ticket.CREATE_TICK = DateTime.Now;
        //                    ticket.FEC_INI_TICK = DateTime.Now;
        //                    ticket.DAT_EXPI_TICK = DateTime.Now.AddHours(HOUR);
        //                    ticket.IS_PARENT = false;
        //                    ticket.ID_TYPE_FORM = 1;
        //                    ticket.ID_CATE = 181;
        //                    ticket.SEND_MAIL = false;

        //                    dbc.TICKETs.Add(ticket);
        //                    dbc.SaveChanges();

        //                    int id = Convert.ToInt32(ticket.ID_TICK);

        //                    dbc.Entry(ticket).State = EntityState.Detached;

        //                    Code = dbc.TICKETs.Single(t => t.ID_TICK == id).COD_TICK;

        //                    if (files != null)
        //                    {
        //                        foreach (var file in files)
        //                        {
        //                            try
        //                            {
        //                                //var fileName = Path.GetFileNameWithoutExtension(file.FileName);
        //                                var ATTA = new ATTACHED_TICKET_FORMAT();
        //                                ATTA.NAM_ATTA = Path.GetFileNameWithoutExtension(file.FileName);
        //                                ATTA.EXT_ATTA = Path.GetExtension(file.FileName);
        //                                ATTA.ID_TICK = id;
        //                                ATTA.CREATE_DATE = DateTime.Now;

        //                                //db.AddToATTACHEDs(ATTA);
        //                                dbc.ATTACHED_TICKET_FORMAT.Add(ATTA);
        //                                dbc.SaveChanges();

        //                                file.SaveAs(Server.MapPath("~/Adjuntos/Delivery/" + ID_ACCO.ToString() + "/") + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA_TICK_FORM) + ATTA.EXT_ATTA);
        //                            }
        //                            catch
        //                            {

        //                            }
        //                        }
        //                    }

        //                    //insertando ahora el detalle
        //                    foreach (string dtide in ids_asset)
        //                    {
        //                        try
        //                        {
        //                            int ide = Convert.ToInt32(dtide);
        //                            //Actualizando el estado final de la fecha fin de la tabla DETAILS_TICKET_FORMAT
        //                            var query = dbc.DETAIL_TICKET_FORMAT.Where(x => x.ID_ASSE == ide && x.DATE_END == null);
        //                            if (query.Count() == 1)
        //                            {
        //                                var detaForm = dbc.DETAIL_TICKET_FORMAT.Single(x => x.ID_ASSE == ide && x.DATE_END == null);
        //                                detaForm.DATE_END = DateTime.Now;
        //                                dbc.DETAIL_TICKET_FORMAT.Attach(detaForm);
        //                                dbc.Entry(detaForm).State = EntityState.Modified;
        //                                dbc.SaveChanges();
        //                            }

        //                            //Registrando nuevo registro del activo
        //                            var DetaTickForm = new DETAIL_TICKET_FORMAT();
        //                            DetaTickForm.ID_ASSE = ide;
        //                            DetaTickForm.ID_TICK = id;
        //                            DetaTickForm.DATE_STAR = DateTime.Now;

        //                            dbc.DETAIL_TICKET_FORMAT.Add(DetaTickForm);
        //                            dbc.SaveChanges();

        //                            //Actualizando el estado final de la fecha final del Client_Asset
        //                            var clieAsse = dbc.CLIENT_ASSET.Single(x => x.ID_ASSE == ide && x.DAT_END == null);

        //                            clieAsse.DAT_END = DateTime.Now;
        //                            dbc.CLIENT_ASSET.Attach(clieAsse);
        //                            dbc.Entry(clieAsse).State = EntityState.Modified;
        //                            dbc.SaveChanges();

        //                            //buscando la condición del activo
        //                            string[] idCondicion = idsCondicion.Split(',');
        //                            int ID_COND = 1;
        //                            foreach (string cond in idCondicion)
        //                            {
        //                                string[] idCond = cond.Split('_');
        //                                if (idCond[0] == ide.ToString())
        //                                {
        //                                    ID_COND = Convert.ToInt32(idCond[1]);
        //                                }
        //                            }

        //                            //Registrando y asociado el activo al nuevo usuario
        //                            var NewClientAsset = new CLIENT_ASSET();
        //                            NewClientAsset.ID_AREA = ID_AREA;
        //                            NewClientAsset.ID_ASSE = ide;
        //                            NewClientAsset.ID_COND = ID_COND;
        //                            NewClientAsset.ID_PERS_ENTI = ID_PER_ENTI;
        //                            NewClientAsset.DAT_STAR = DateTime.Now;
        //                            NewClientAsset.CREATE_DATE = DateTime.Now;
        //                            NewClientAsset.UserId = ID_USER;
        //                            NewClientAsset.ID_LOCA = ticket.ID_LOCA;
        //                            NewClientAsset.ID_TYPE_CLIE_ASSE = 1;
        //                            NewClientAsset.IdTicket = id;
        //                            NewClientAsset.SUM_CLIE_ASSE = ticket.SUM_TICK;

        //                            dbc.CLIENT_ASSET.Add(NewClientAsset);
        //                            dbc.SaveChanges();
        //                        }
        //                        catch
        //                        {   //Falla al insertar en base de datos
        //                            return Content("<script type='text/javascript'> function init() {" +
        //                            "if(top.uploadDone) top.uploadDone('error','1','');}window.onload=init;</script>");
        //                        }
        //                    }
        //                    //CREAR NUEVA ENTREGA
        //                    //

        //                    if (idsActivo.Length > 0)
        //                    {
        //                        idsActivo = idsActivo.Substring(0, idsActivo.Length - 1);

        //                        foreach (string ida in ids_asset)
        //                        {
        //                            var idsearch = Convert.ToInt32(ida);
        //                            var query = dbc.ASSETs
        //                                .Where(a => a.ID_ASSE == idsearch)
        //                                .Join(dbc.CLIENT_ASSET, a => a.ID_ASSE, ca => ca.ID_ASSE, (x, ca) => new
        //                                {
        //                                    x.ID_ASSE,
        //                                    ca.ID_COND,
        //                                    ca.DAT_END
        //                                }).Where(x => x.DAT_END == null && x.ID_COND != 9 && x.ID_COND != 3);

        //                            int cant = query.Count();
        //                            //if (cant > 0)
        //                            //{
        //                            //    msn = msn + idsearch.ToString() + ",";
        //                            //}
        //                        }
        //                        if (msn.Length > 0)
        //                        {
        //                            return Content("<script type='text/javascript'> function init() {" +
        //                                "if(top.uploadDone) top.uploadDone('" + msn + "','2');}window.onload=init;</script>");
        //                        }
        //                        else
        //                        { //Guardando el registro
        //                            var ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
        //                            var ID_PERS_ASSI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
        //                            var ID_QUEU = Convert.ToInt32(Session["ID_QUEU"]);
        //                            var ID_USER = Convert.ToInt32(Session["UserId"]);
        //                            int ID_PER_ENTI, ID_PRIO, ID_LOCA, ID_AREA, HOUR;

        //                            var objPersonEntity = dbe.PERSON_ENTITY.Single(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI.Value);

        //                            DateTime FEC_TICK;
        //                            try
        //                            {
        //                                ID_PER_ENTI = ticket.ID_PERS_ENTI.Value;
        //                                ID_LOCA = ticket.ID_LOCA.Value;
        //                                FEC_TICK = ticket.FEC_TICK.Value;
        //                                ID_AREA = Convert.ToInt32(objPersonEntity.ID_AREA.Value);
        //                                ticket.ID_AREA = ID_AREA;
        //                                ID_PRIO = Convert.ToInt32(objPersonEntity.ID_PRIO.Value);
        //                                HOUR = Convert.ToInt32(dbc.PRIORITies.First(x => x.ID_PRIO == ID_PRIO).HOU_PRIO.Value);
        //                                //try
        //                                //{
        //                                //    ID_LOCA = Convert.ToInt32(dbe.PERSON_LOCATION.Single(pl => pl.ID_PERS_ENTI == ID_PER_ENTI && pl.END_DATE == null && pl.VIG_LOCA == true).ID_LOCA);
        //                                //    ticket.ID_LOCA = ID_LOCA;
        //                                //}
        //                                //catch { ID_LOCA = 0; ticket.ID_LOCA = ID_LOCA; }
        //                                ticket.ID_SOUR = 3;
        //                            }
        //                            catch
        //                            {   //Falta informacion
        //                                return Content("<script type='text/javascript'> function init() {" +
        //                                "if(top.uploadDone) top.uploadDone('" + msn + "','0');}window.onload=init;</script>");
        //                            }

        //                            //bool est = new bool();
        //                            //est = ModelState.IsValidField("ID_ACCO");
        //                            //est = ModelState.IsValidField("ID_TYPE_TICK");
        //                            //est = ModelState.IsValidField("ID_CLIE");

        //                            //if (ModelState.IsValid)
        //                            if (ticket.ID_PERS_ENTI != null)
        //                            {
        //                                string Code = null;
        //                                try
        //                                {
        //                                    ticket.ID_ACCO = ID_ACCO;
        //                                    ticket.ID_TYPE_TICK = 2;
        //                                    ticket.ID_PERS_ENTI = ID_PER_ENTI;
        //                                    ticket.ID_PERS_ENTI_END = ID_PER_ENTI;
        //                                    ticket.ID_PERS_ENTI_ASSI = ID_PERS_ASSI;
        //                                    ticket.ID_AREA = ID_AREA;
        //                                    ticket.ID_QUEU = ID_QUEU;
        //                                    ticket.ID_PRIO = 5;
        //                                    ticket.ID_STAT = 1;
        //                                    ticket.ID_STAT_END = 1;
        //                                    ticket.ID_SOUR = 3;
        //                                    ticket.FEC_TICK = FEC_TICK;
        //                                    //ticket.SUM_TICK = ticket.SUM_TICK;
        //                                    ticket.REM_CTRL_TICK = false;
        //                                    ticket.UserId = ID_USER;
        //                                    ticket.CREATE_TICK = DateTime.Now;
        //                                    ticket.FEC_INI_TICK = DateTime.Now;
        //                                    ticket.DAT_EXPI_TICK = DateTime.Now.AddHours(HOUR);
        //                                    ticket.IS_PARENT = false;
        //                                    ticket.ID_TYPE_FORM = 1;
        //                                    ticket.ID_CATE = 181;
        //                                    ticket.SEND_MAIL = false;

        //                                    dbc.TICKETs.Add(ticket);
        //                                    dbc.SaveChanges();

        //                                    int id = Convert.ToInt32(ticket.ID_TICK);

        //                                    dbc.Entry(ticket).State = EntityState.Detached;

        //                                    Code = dbc.TICKETs.Single(t => t.ID_TICK == id).COD_TICK;

        //                                    if (files != null)
        //                                    {
        //                                        foreach (var file in files)
        //                                        {
        //                                            try
        //                                            {
        //                                                //var fileName = Path.GetFileNameWithoutExtension(file.FileName);
        //                                                var ATTA = new ATTACHED_TICKET_FORMAT();
        //                                                ATTA.NAM_ATTA = Path.GetFileNameWithoutExtension(file.FileName);
        //                                                ATTA.EXT_ATTA = Path.GetExtension(file.FileName);
        //                                                ATTA.ID_TICK = id;
        //                                                ATTA.CREATE_DATE = DateTime.Now;

        //                                                //db.AddToATTACHEDs(ATTA);
        //                                                dbc.ATTACHED_TICKET_FORMAT.Add(ATTA);
        //                                                dbc.SaveChanges();

        //                                                file.SaveAs(Server.MapPath("~/Adjuntos/Delivery/" + ID_ACCO.ToString() + "/") + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA_TICK_FORM) + ATTA.EXT_ATTA);
        //                                            }
        //                                            catch
        //                                            {

        //                                            }
        //                                        }
        //                                    }

        //                                    //insertando ahora el detalle
        //                                    foreach (string dtide in ids_asset)
        //                                    {
        //                                        try
        //                                        {
        //                                            int ide = Convert.ToInt32(dtide);
        //                                            //Actualizando el estado final de la fecha fin de la tabla DETAILS_TICKET_FORMAT
        //                                            var query = dbc.DETAIL_TICKET_FORMAT.Where(x => x.ID_ASSE == ide && x.DATE_END == null);
        //                                            if (query.Count() == 1)
        //                                            {
        //                                                var detaForm = dbc.DETAIL_TICKET_FORMAT.Single(x => x.ID_ASSE == ide && x.DATE_END == null);
        //                                                detaForm.DATE_END = DateTime.Now;
        //                                                dbc.DETAIL_TICKET_FORMAT.Attach(detaForm);
        //                                                dbc.Entry(detaForm).State = EntityState.Modified;
        //                                                dbc.SaveChanges();
        //                                            }

        //                                            //Registrando nuevo registro del activo
        //                                            var DetaTickForm = new DETAIL_TICKET_FORMAT();
        //                                            DetaTickForm.ID_ASSE = ide;
        //                                            DetaTickForm.ID_TICK = id;
        //                                            DetaTickForm.DATE_STAR = DateTime.Now;

        //                                            dbc.DETAIL_TICKET_FORMAT.Add(DetaTickForm);
        //                                            dbc.SaveChanges();

        //                                            //Actualizando el estado final de la fecha final del Client_Asset
        //                                            var clieAsse = dbc.CLIENT_ASSET.Single(x => x.ID_ASSE == ide && x.DAT_END == null);

        //                                            clieAsse.DAT_END = DateTime.Now;
        //                                            dbc.CLIENT_ASSET.Attach(clieAsse);
        //                                            dbc.Entry(clieAsse).State = EntityState.Modified;
        //                                            dbc.SaveChanges();

        //                                            //buscando la condición del activo
        //                                            string[] idCondicion = idsCondicion.Split(',');
        //                                            int ID_COND = 1;
        //                                            foreach (string cond in idCondicion)
        //                                            {
        //                                                string[] idCond = cond.Split('_');
        //                                                if (idCond[0] == ide.ToString())
        //                                                {
        //                                                    ID_COND = Convert.ToInt32(idCond[1]);
        //                                                }
        //                                            }

        //                                            //Registrando y asociado el activo al nuevo usuario
        //                                            var NewClientAsset = new CLIENT_ASSET();
        //                                            NewClientAsset.ID_AREA = ID_AREA;
        //                                            NewClientAsset.ID_ASSE = ide;
        //                                            NewClientAsset.ID_COND = ID_COND;
        //                                            NewClientAsset.ID_PERS_ENTI = ID_PER_ENTI;
        //                                            NewClientAsset.DAT_STAR = DateTime.Now;
        //                                            NewClientAsset.CREATE_DATE = DateTime.Now;
        //                                            NewClientAsset.UserId = ID_USER;
        //                                            NewClientAsset.ID_LOCA = ticket.ID_LOCA;
        //                                            NewClientAsset.ID_TYPE_CLIE_ASSE = 1;
        //                                            NewClientAsset.IdTicket = id;
        //                                            NewClientAsset.SUM_CLIE_ASSE = ticket.SUM_TICK;

        //                                            dbc.CLIENT_ASSET.Add(NewClientAsset);
        //                                            dbc.SaveChanges();
        //                                        }
        //                                        catch
        //                                        {   //Falla al insertar en base de datos
        //                                            return Content("<script type='text/javascript'> function init() {" +
        //                                            "if(top.uploadDone) top.uploadDone('error','1','');}window.onload=init;</script>");
        //                                        }
        //                                    }
        //                                }
        //                                catch
        //                                {   //Falla al insertar en base de datos
        //                                    //var errors = ModelState.Select(x => x.Value.Errors).ToList();                        
        //                                    return Content("<script type='text/javascript'> function init() {" +
        //                                    "if(top.uploadDone) top.uploadDone('error','1','');}window.onload=init;</script>");
        //                                }
        //                                //Se registro correctamente
        //                                return Content("<script type='text/javascript'> function init() {" +
        //                                "if(top.uploadDone) top.uploadDone('OK','" + Code + "','" + ticket.ID_TICK.ToString() + "');}window.onload=init;</script>");
        //                            }
        //                            else
        //                            {
        //                                var errors = ModelState.Select(x => x.Value.Errors).ToList();
        //                                return Content("<script type='text/javascript'> function init() {" +
        //                                "if(top.uploadDone) top.uploadDone('error','0','');}window.onload=init;</script>");
        //                            }
        //                        } //fin de Guardando el registro
        //                    }
        //                    else
        //                    {
        //                        return Content("<script type='text/javascript'> function init() {" +
        //                            "if(top.uploadDone) top.uploadDone('ERROR','0','');}window.onload=init;</script>");
        //                    }

        //                    //
        //                }
        //                catch
        //                {   //Falla al insertar en base de datos
        //                    //var errors = ModelState.Select(x => x.Value.Errors).ToList();                        
        //                    return Content("<script type='text/javascript'> function init() {" +
        //                    "if(top.uploadDone) top.uploadDone('error','1','');}window.onload=init;</script>");
        //                }
        //                //Se registro correctamente
        //                return Content("<script type='text/javascript'> function init() {" +
        //                "if(top.uploadDone) top.uploadDone('OK','" + Code + "','" + ticket.ID_TICK.ToString() + "');}window.onload=init;</script>");
        //            }
        //            else
        //            {
        //                var errors = ModelState.Select(x => x.Value.Errors).ToList();
        //                return Content("<script type='text/javascript'> function init() {" +
        //                "if(top.uploadDone) top.uploadDone('error','0','');}window.onload=init;</script>");
        //            }
        //        } //fin de Guardando el registro
        //    }
        //    else
        //    {
        //        return Content("<script type='text/javascript'> function init() {" +
        //            "if(top.uploadDone) top.uploadDone('ERROR','0','');}window.onload=init;</script>");
        //    }
        //}

        // POST: /DeliveryReception/Create
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(IEnumerable<HttpPostedFileBase> files, TICKET ticket)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            //Validando si los Asset siguen siendo UNASSIGNED
            string idsActivo = Convert.ToString(Request.Form["idsActivo"]);
            string idsCondicion = Convert.ToString(Request.Form["idsCondicion"]);
            //int idSla = Convert.ToInt32(dbc.SLAs.Single(x => x.IdCuenta == ID_ACCO && x.Nombre == "SLA ESTANDAR" && x.Estado == true).Id);
            int idSla = 0;

            if (ID_ACCO >= 61 && ID_ACCO <= 74)
            {
                int idTypeTick = Convert.ToInt32(Request.Form["ID_TYPE_TICK"]);
                idSla = Convert.ToInt32(dbc.SLAs.Single(x => x.IdCuenta == ID_ACCO && x.Nombre == "SLA ESTANDAR" && x.Estado == true && x.ID_TYPE_TICK == idTypeTick).Id);

            }
            else if (ID_ACCO == 60)
            {
                idSla = Convert.ToInt32(dbc.SLAs.Single(x => x.IdCuenta == ID_ACCO && x.Nombre == "SLA REQUERIMIENTO" && x.Estado == true).Id);
            }
            else
            {
                idSla = Convert.ToInt32(dbc.SLAs.Single(x => x.IdCuenta == ID_ACCO && x.Nombre == "SLA ESTANDAR" && x.Estado == true).Id);

            }
            string idUsuarioResponsable = Request.Params["IdUsuarioResponsable"];

            if((ID_ACCO == 56  || ID_ACCO == 57 || ID_ACCO == 58)  && (idUsuarioResponsable == null || idUsuarioResponsable == ""))
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('INFO','Seleccione al usuario responsable.',0);}window.onload=init;</script>");

            if (ID_ACCO == 60)
            {
                string grupo = ObtenerNomGrupoUsuarioBNV(Convert.ToInt32(ticket.IdGrupoActivo));
                if (grupo == "MICROINFORMATICO" && (ticket.SUM_TICK == null || ticket.SUM_TICK == "")){
                    ticket.SUM_TICK = "Se realizó la entrega del activo.";
                }
                else if ((grupo == "INFRAESTRUCTURA" || grupo == "MOVIL") && (ticket.SUM_TICK == null || ticket.SUM_TICK == ""))
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('INFO','Ingrese un comentario.',0);}window.onload=init;</script>");
                }
            }
            else
            {
                if (ticket.SUM_TICK == null || ticket.SUM_TICK == "")
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('INFO','Ingrese un comentario.',0);}window.onload=init;</script>");
            }

            if (ticket.ID_PERS_ENTI == null)
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('INFO','Seleccione al usuario.',0);}window.onload=init;</script>");

            if (idsActivo.Length > 0)
            {
                idsActivo = idsActivo.Substring(0, idsActivo.Length - 1);
                string[] ids_asset = idsActivo.Split(',');
                string msn = "";

                foreach (string ida in ids_asset)
                {
                    var idsearch = Convert.ToInt32(ida);
                    var query = dbc.ASSETs
                        .Where(a => a.ID_ASSE == idsearch)
                        .Join(dbc.CLIENT_ASSET, a => a.ID_ASSE, ca => ca.ID_ASSE, (x, ca) => new
                        {
                            x.ID_ASSE,
                            ca.ID_COND,
                            ca.DAT_END
                        }).Where(x => x.DAT_END == null && x.ID_COND !=9 && x.ID_COND !=3 );

                    int cant = query.Count();
                    //if (cant > 0)
                    //{
                    //    msn = msn + idsearch.ToString() + ",";
                    //}
                }
                if (msn.Length > 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('" + msn + "','2');}window.onload=init;</script>");
                }
                else
                { //Guardando el registro
                    var ID_PERS_ASSI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                    var ID_QUEU = Convert.ToInt32(Session["ID_QUEU"]);
                    var ID_USER = Convert.ToInt32(Session["UserId"]);
                    int ID_PER_ENTI, ID_PRIO, ID_LOCA = 0, ID_AREA, HOUR = 0, HOUR_s = 0;

                    var objPersonEntity = dbe.PERSON_ENTITY.Single(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI.Value);

                    if ((ID_ACCO == 56 || ID_ACCO == 57 || ID_ACCO == 58) && objPersonEntity.ID_AREA == null)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('INFO','El usuario no tiene un área asociada.','0');}window.onload=init;</script>");
                    }

                    DateTime FEC_TICK;
                    try
                    {
                        ID_PER_ENTI = ticket.ID_PERS_ENTI.Value;
                        if (ID_ACCO != 56 && ID_ACCO != 57 && ID_ACCO != 58 && ID_ACCO != 60)
                        {
                            ID_LOCA = ticket.ID_LOCA.Value;
                        }

                        if (ID_ACCO == 60)
                        {
                            FEC_TICK = Convert.ToDateTime(Request.Form["Fecha"]);
                            ID_AREA = Convert.ToInt32(objPersonEntity.ID_AREA);
                            ticket.ID_AREA = ID_AREA;
                            ID_PRIO = Convert.ToInt32(objPersonEntity.ID_PRIO);
                        }
                        else
                        {
                            FEC_TICK = ticket.FEC_TICK.Value;
                            ID_AREA = Convert.ToInt32(objPersonEntity.ID_AREA.Value);
                            ticket.ID_AREA = ID_AREA;
                            ID_PRIO = Convert.ToInt32(objPersonEntity.ID_PRIO.Value);
                            HOUR_s = Convert.ToInt32(dbc.SLADetalles.Single(x => x.IdSLA == idSla && x.IdPrioridad == ID_PRIO).TiempoAtencion);
                            HOUR = Convert.ToInt32(dbc.SLADetalles.Single(x => x.IdSLA == idSla && x.IdPrioridad == ID_PRIO).TiempoAtencion);
                        }
                        //try
                        //{
                        //    ID_LOCA = Convert.ToInt32(dbe.PERSON_LOCATION.Single(pl => pl.ID_PERS_ENTI == ID_PER_ENTI && pl.END_DATE == null && pl.VIG_LOCA == true).ID_LOCA);
                        //    ticket.ID_LOCA = ID_LOCA;
                        //}
                        //catch { ID_LOCA = 0; ticket.ID_LOCA = ID_LOCA; }
                        ticket.ID_SOUR = 3;
                    }
                    catch
                    {   //Falta informacion
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('" + msn + "','0');}window.onload=init;</script>");
                    }

                    //bool est = new bool();
                    //est = ModelState.IsValidField("ID_ACCO");
                    //est = ModelState.IsValidField("ID_TYPE_TICK");
                    //est = ModelState.IsValidField("ID_CLIE");

                    if (ID_ACCO == 60)
                        ticket.ID_LOCA = (Request.Form["Locacion"] != "" && Request.Form["Locacion"] != null) ? Convert.ToInt32(Request.Form["Locacion"]) : ID_LOCA;

                    //if (ModelState.IsValid)
                    if (ticket.ID_PERS_ENTI != null)
                    {
                        string Code = null;
                        try
                        {
                            int IdTicketPadre = Convert.ToInt32(Request.Form["IdTicket"]);
                            ticket.ID_ACCO = ID_ACCO;
                            ticket.ID_TYPE_TICK = 2;
                            ticket.ID_PERS_ENTI = ID_PER_ENTI;
                            ticket.ID_PERS_ENTI_END = ID_PER_ENTI;
                            ticket.ID_PERS_ENTI_ASSI = ID_PERS_ASSI;
                            ticket.ID_AREA = ID_AREA;
                            ticket.ID_QUEU = ID_QUEU;
                            ticket.ID_PRIO = 5;
                            ticket.IdSLA = idSla; //Asignacion de SLA ESTANDAR
                            ticket.ID_STAT = 1;
                            ticket.ID_STAT_END = 1;
                            ticket.ID_SOUR = 3;
                            ticket.FEC_TICK = FEC_TICK;
                            //ticket.SUM_TICK = ticket.SUM_TICK;
                            ticket.REM_CTRL_TICK = false;
                            ticket.UserId = ID_USER;
                            ticket.CREATE_TICK = DateTime.Now;
                            ticket.FEC_INI_TICK = DateTime.Now;
                            ticket.DAT_EXPI_TICK = DateTime.Now.AddHours(HOUR);
                            ticket.IS_PARENT = false;
                            ticket.ID_TYPE_FORM = 1;
                            ticket.ID_CATE = 181;
                            ticket.ID_TICK_PARENT = IdTicketPadre;
                            ticket.SEND_MAIL = false;

                            dbc.TICKETs.Add(ticket);
                            dbc.SaveChanges();

                            //Modificando ticket a ticket Padre
                            dbc.tktModificarTicketPadre(IdTicketPadre);

                            int id = Convert.ToInt32(ticket.ID_TICK);

                            dbc.Entry(ticket).State = EntityState.Detached;

                            Code = dbc.TICKETs.Single(t => t.ID_TICK == id).COD_TICK;

                            if (files != null)
                            {
                                foreach (var file in files)
                                {
                                    try
                                    {
                                        String doc = Regex.Replace(Path.GetFileNameWithoutExtension(file.FileName), @"[^\w\.@-]", "",
                                                RegexOptions.None, TimeSpan.FromSeconds(1.5));
                                        var ATTA = new ATTACHED_TICKET_FORMAT();
                                        ATTA.NAM_ATTA = doc;
                                        ATTA.EXT_ATTA = Path.GetExtension(file.FileName);
                                        ATTA.ID_TICK = id;
                                        ATTA.CREATE_DATE = DateTime.Now;

                                        //db.AddToATTACHEDs(ATTA);
                                        dbc.ATTACHED_TICKET_FORMAT.Add(ATTA);
                                        dbc.SaveChanges();

                                        file.SaveAs(Server.MapPath("~/Adjuntos/Delivery/" + ID_ACCO.ToString() + "/") + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA_TICK_FORM) + ATTA.EXT_ATTA);
                                    }
                                    catch
                                    {

                                    }
                                }
                            }

                            //Insertando comentario en el ticket de solicitud
                            var detailsTicket = new DETAILS_TICKET();
                            detailsTicket.ID_TICK = ticket.ID_TICK;
                            detailsTicket.ID_TYPE_DETA_TICK = 1;
                            detailsTicket.COM_DETA_TICK = "Se realizó la entrega del activo. "+
                                "<a href='/DeliveryReception/Details/" + ticket.ID_TICK + "' target='_blank'>Ir al Ticket de activo</a>";
                            detailsTicket.UserId = 34;
                            detailsTicket.CREATE_DETA_INCI = DateTime.Now;
                            detailsTicket.MINUTES = 0;
                            dbc.DETAILS_TICKET.Add(detailsTicket);
                            dbc.SaveChanges();

                            //insertando ahora el detalle
                            foreach (string dtide in ids_asset)
                            {
                                try
                                {
                                    int ide = Convert.ToInt32(dtide);
                                    //Actualizando el estado final de la fecha fin de la tabla DETAILS_TICKET_FORMAT
                                    var query = dbc.DETAIL_TICKET_FORMAT.Where(x => x.ID_ASSE == ide && x.DATE_END == null);
                                    if (query.Count() == 1)
                                    {
                                        var detaForm = dbc.DETAIL_TICKET_FORMAT.Single(x => x.ID_ASSE == ide && x.DATE_END == null);
                                        detaForm.DATE_END = DateTime.Now;
                                        dbc.DETAIL_TICKET_FORMAT.Attach(detaForm);
                                        dbc.Entry(detaForm).State = EntityState.Modified;
                                        dbc.SaveChanges();
                                    }

                                    //Registrando nuevo registro del activo
                                    var DetaTickForm = new DETAIL_TICKET_FORMAT();
                                    DetaTickForm.ID_ASSE = ide;
                                    DetaTickForm.ID_TICK = id;
                                    DetaTickForm.DATE_STAR = DateTime.Now;

                                    dbc.DETAIL_TICKET_FORMAT.Add(DetaTickForm);
                                    dbc.SaveChanges();

                                    //Actualizando el estado final de la fecha final del Client_Asset
                                    var clieAsse = dbc.CLIENT_ASSET.Single(x => x.ID_ASSE == ide && x.DAT_END == null);
                                    //int id_loca_anti = Convert.ToInt32(clieAsse.ID_LOCA);
                                    clieAsse.DAT_END = DateTime.Now;
                                    dbc.CLIENT_ASSET.Attach(clieAsse);
                                    dbc.Entry(clieAsse).State = EntityState.Modified;
                                    dbc.SaveChanges();

                                    //buscando la condición del activo
                                    string[] idCondicion = idsCondicion.Split(',');
                                    int ID_COND = 1;
                                    foreach (string cond in idCondicion)
                                    {
                                        string[] idCond = cond.Split('_');
                                        if (idCond[0] == ide.ToString())
                                        {
                                            ID_COND = Convert.ToInt32(idCond[1]);
                                        }
                                    }

                                    //Registrando y asociado el activo al nuevo usuario
                                    var NewClientAsset = new CLIENT_ASSET();
                                    NewClientAsset.ID_AREA = ID_AREA;
                                    NewClientAsset.ID_ASSE = ide;
                                    NewClientAsset.ID_COND = ID_COND;
                                    NewClientAsset.ID_PERS_ENTI = ID_PER_ENTI;
                                    NewClientAsset.DAT_STAR = DateTime.Now;
                                    NewClientAsset.CREATE_DATE = DateTime.Now;
                                    NewClientAsset.UserId = ID_USER;
                                    // MINSUR / MARCOBRE / RAURA
                                    if (ID_ACCO == 56 || ID_ACCO == 57 || ID_ACCO == 58)
                                        NewClientAsset.ID_LOCA =  dbc.ObtieneLocacionInicialActivo(ide).Single().ID_LOCA;
                                    else if (ID_ACCO == 60)
                                        NewClientAsset.ID_LOCA = (Request.Form["Locacion"] != "" && Request.Form["Locacion"] != null) ? Convert.ToInt32(Request.Form["Locacion"]) : ID_LOCA;
                                    else
                                        NewClientAsset.ID_LOCA = ticket.ID_LOCA;
                                    NewClientAsset.ID_TYPE_CLIE_ASSE = 1;
                                    NewClientAsset.IdTicket = id;
                                    NewClientAsset.SUM_CLIE_ASSE = ticket.SUM_TICK;
                                    if (ID_ACCO == 56 || ID_ACCO == 57 || ID_ACCO == 58)
                                        NewClientAsset.ID_PERS_ENTI_RESP = Convert.ToInt32(idUsuarioResponsable);
                                    if (ID_ACCO == 60)
                                    {
                                        NewClientAsset.UMinera = clieAsse.UMinera;
                                        NewClientAsset.Ubicacion = (Request.Form["Ubicacion"] != "" && Request.Form["Ubicacion"] != null) ? Request.Form["Ubicacion"].ToString() : null;
                                    }

                                    if (ID_ACCO == 1)
                                    {
                                        bool porContratista = Request.Form["PorContratista"] == "on";
                                        NewClientAsset.PorContratista = porContratista;
                                        if (porContratista == true)
                                        {
                                            NewClientAsset.EmpresaContratista = Request.Form["EmpresaContratista"].ToString();
                                            NewClientAsset.UsuarioContratista = Request.Form["UsuarioContratista"].ToString();
                                            NewClientAsset.AreaContratista = Request.Form["AreaContratista"].ToString();
                                        }
                                    }

                                    dbc.CLIENT_ASSET.Add(NewClientAsset);
                                    dbc.SaveChanges();
                                }
                                catch
                                {   //Falla al insertar en base de datos
                                    return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.uploadDone) top.uploadDone('error','1','');}window.onload=init;</script>");
                                }
                            }
                        }
                        catch
                        {   //Falla al insertar en base de datos
                            //var errors = ModelState.Select(x => x.Value.Errors).ToList();                        
                            return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('error','1','');}window.onload=init;</script>");
                        }
                        //Se registro correctamente
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('OK','" + Code + "','" + ticket.ID_TICK.ToString() + "');}window.onload=init;</script>");
                    }
                    else
                    {
                        var errors = ModelState.Select(x => x.Value.Errors).ToList();
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('error','0','');}window.onload=init;</script>");
                    }
                } //fin de Guardando el registro
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','0','');}window.onload=init;</script>");
            }
        }

        [Authorize]
        public ActionResult CreateRecepcion()
        {
            var ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            var ID_PERS_ASSI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            var ID_QUEU = Convert.ToInt32(Session["ID_QUEU"]);

            if (ID_ACCO == 4)//ELECTRODATA
            {
                if ((int)Session["SOPORTE"] == 0)
                {
                    return RedirectToAction("Index", "Error");
                }
            }

            var ticket = new TICKET();

            if (ID_ACCO == 60)
            {
                int idGrupoActivo = ObtenerIdGrupoUsuarioBNV();

                if (idGrupoActivo != 0)
                {
                    string grupoActivo = ObtenerNomGrupoUsuarioBNV(idGrupoActivo);
                    ticket.IdGrupoActivo = idGrupoActivo;
                    ticket.ID_COMP = ObtenerUMineraUsuarioBNV(Convert.ToInt32(Session["ID_PERS_ENTI"]), idGrupoActivo);
                    ViewBag.Grupo = grupoActivo;
                }
                else
                {
                    return RedirectToAction("Index", "Error");
                }
            }
            else if (ID_ACCO == 55)
            {
                if (Convert.ToInt32(Session["GESTOR_ACTIVOS_HUDBAY_IT"]) == 1)
                {
                    int idGrupoIT = ObtenerIdGrupo("IT");
                    ViewBag.IdGrupoIT = idGrupoIT;
                }
                else if (Convert.ToInt32(Session["GESTOR_ACTIVOS_HUDBAY_OT"]) == 1)
                {
                    return RedirectToAction("CreateRecepcionOT", "DeliveryReception");
                }
                else
                {
                    return RedirectToAction("Index", "Error");
                }                
            }

            ticket.FEC_TICK = DateTime.Now;
            ViewBag.FEC_TICK = String.Format("{0:g}", ticket.FEC_TICK);
            ticket.ID_TYPE_TICK = 2;
            ticket.ID_CATE = 119;
            ticket.ID_PERS_ENTI_END = ID_PERS_ASSI;
            ticket.ID_PERS_ENTI_ASSI = ID_PERS_ASSI;
            ticket.ID_QUEU = ID_QUEU;
            ticket.ID_PRIO = 5;
            ticket.ID_STAT = 1;
            ticket.SUM_TICK = "";
            ViewBag.IdAcco = ID_ACCO;
            return View(ticket);
        }

        // Procedimiento nuevo para listar la persona por compania
        public ActionResult PersonaPorCompania()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            //int Compania = Convert.ToInt32(dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 6 && x.ID_ACCO == ID_ACCO).Single().VAL_ACCO_PARA);

            //var queryPE = dbe.PERSON_ENTITY.Where(x => x.ID_ENTI1 == Compania).ToList();
            //var queryAE = dbe.ACCOUNT_ENTITY.Where(ae => ae.ID_ACCO == ID_ACCO && ae.VIS_REQU == true && ae.VIG_ACCO_ENTI == true).ToList();

            //var queryVP = dbe.NumeroActivosPorPersona().ToList();
            //var queryAE = dbe.

            var result = (dbe.PersonaxCompaniayActivos(ID_ACCO)).ToList().OrderBy(x => x.CLIE);


            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateReasignacion()
        {
            var ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            var ID_PERS_ASSI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            var ID_QUEU = Convert.ToInt32(Session["ID_QUEU"]);

            if (ID_ACCO == 4)//ELECTRODATA
            {
                if ((int)Session["SOPORTE"] == 0)
                {
                    return Content("Necesitas permisos para acceder a esta sección.");
                }
            }

            var ticket = new TICKET();
            ticket.FEC_TICK = DateTime.Now;
            ViewBag.FEC_TICK = String.Format("{0:g}", ticket.FEC_TICK);
            ticket.ID_TYPE_TICK = 2;
            ticket.ID_CATE = 119;
            ticket.ID_PERS_ENTI_END = ID_PERS_ASSI;
            ticket.ID_PERS_ENTI_ASSI = ID_PERS_ASSI;
            ticket.ID_QUEU = ID_QUEU;
            ticket.ID_PRIO = 5;
            ticket.ID_STAT = 1;
            ticket.SUM_TICK = "";
            return View(ticket);
        }

        [Authorize]
        public ActionResult EntregaReasignacion()
        {
            var ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            var ID_PERS_ASSI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            var ID_QUEU = Convert.ToInt32(Session["ID_QUEU"]);

            if (ID_ACCO == 4)//ELECTRODATA
            {
                if ((int)Session["SOPORTE"] == 0)
                {
                    return Content("Necesitas permisos para acceder a esta sección.");
                }
            }

            var ticket = new TICKET();
            ticket.FEC_TICK = DateTime.Now;
            ViewBag.FEC_TICK = String.Format("{0:g}", ticket.FEC_TICK);
            ticket.ID_TYPE_TICK = 2;
            ticket.ID_CATE = 119;
            ticket.ID_PERS_ENTI_END = ID_PERS_ASSI;
            ticket.ID_PERS_ENTI_ASSI = ID_PERS_ASSI;
            ticket.ID_QUEU = ID_QUEU;
            ticket.ID_PRIO = 5;
            ticket.ID_STAT = 1;
            ticket.SUM_TICK = "";
            return View(ticket);
        }

        // POST: /DeliveryReception/CreateRecepcion
        [HttpPost, ValidateInput(false)]
        public ActionResult CreateRecepcion(IEnumerable<HttpPostedFileBase> files, TICKET ticket, string nombresLicencias, string idLicencias)
        {
            string idLicenciaString = "";
            var nombreLicencia = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(nombresLicencias);
            var idLicencia= Newtonsoft.Json.JsonConvert.DeserializeObject<int[]>(idLicencias);
            var usuarioModifica = Convert.ToInt32(Session["UserId"]);
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            List<int> licencias = new List<int>();
            List<int> activos = new List<int>();


            for (int i = 0; i < nombreLicencia.Length; i++)
            {
                if (nombreLicencia[i] == "Licencia")
                {
                    licencias.Add(idLicencia[i]);
                }
                else
                {
                    activos.Add(idLicencia[i]);
                }
            }

            if (licencias != null && licencias.Count > 0 && activos.Count==0)
            {

                if ((ID_ACCO == 60) && (ticket.ID_PERS_ENTI == null || ticket.SUM_TICK == null || ticket.FEC_TICK == null))
                    return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('error','0');}window.onload=init;</script>");

                foreach (int id in licencias)
                {
                    dbc.RecepcionLicenciaPrograma(id, usuarioModifica, ticket.ID_PERS_ENTI);
                }

                idLicenciaString = string.Join(",", licencias);

                return Content("<script type='text/javascript'> function init() {" +
    "if(top.uploadDone) top.uploadDone('OKLicencia', '0', '','" + idLicenciaString + "');}window.onload=init;</script>");
            }
            else if (licencias != null && licencias.Count > 0)
            {

                foreach (int id in licencias)
                {
                    dbc.RecepcionLicenciaPrograma(id, usuarioModifica, ticket.ID_PERS_ENTI);
                }

            }

            idLicenciaString = string.Join(",", licencias);

            string Programa = Convert.ToString(Request.Form["Programa"]);

            //Validando si los Asset siguen siendo UNASSIGNED
            string idsActivo = Convert.ToString(Request.Form["idsActivo"]);
            string idsCondicion = Convert.ToString(Request.Form["idsCondicion"]);
            string idsNombreActivo = Convert.ToString(Request.Form["idsNombreActivo"]);
            
            string msn = "";
            int ID_PER_ENTI, ID_PRIO, ID_LOCA = 0, ID_AREA, ID_SOUR, HOUR = 0, HOUR_s = 0;


            if (licencias.Count>0)
            {
                string[] idsActivoArray = idsActivo.Split(',');
                List<int> listaIdsActivo = new List<int>();

                foreach (string id in idsActivoArray)
                {
                    if (int.TryParse(id, out int numero))
                    {
                        listaIdsActivo.Add(numero);
                    }
                }

                idsActivo = string.Join(",", listaIdsActivo.Except(licencias).Select(x => x.ToString()).ToArray());

            }


            if (ID_ACCO == 60 && ticket.IdGrupoActivo == null)
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('INFO','Seleccione un grupo.',0);}window.onload=init;</script>");

            if (ticket.SUM_TICK == null || ticket.SUM_TICK == "")
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('INFO','Ingrese un comentario.',0);}window.onload=init;</script>");
            else if (ticket.ID_PERS_ENTI == null)
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('INFO','Seleccione un usuario.',0);}window.onload=init;</script>");


            var objPersonEntity = dbe.PERSON_ENTITY.Single(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI.Value);

            if ((ID_ACCO == 56 || ID_ACCO == 57 || ID_ACCO == 58) && objPersonEntity.ID_AREA == null)
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('INFO','El usuario no tiene un área asociada.','0');}window.onload=init;</script>");
            }

            DateTime FEC_TICK;
            int idSla = 0;
            string grupo = "";

            if (ID_ACCO == 60)
            {
                idSla = Convert.ToInt32(dbc.SLAs.Single(x => x.IdCuenta == ID_ACCO && x.Nombre == "SLA REQUERIMIENTO" && x.Estado == true).Id);
                grupo = ObtenerNomGrupoUsuarioBNV(Convert.ToInt32(ticket.IdGrupoActivo));
            }
            else
            {
                idSla = Convert.ToInt32(dbc.SLAs.Single(x => x.IdCuenta == ID_ACCO && x.Nombre == "SLA ESTANDAR" && x.Estado == true).Id);
            }

            try
            {
                ID_PER_ENTI = ticket.ID_PERS_ENTI.Value;
                if (ID_ACCO == 60)
                {
                    FEC_TICK = Convert.ToDateTime(Request.Form["Fecha"]);
                    ID_AREA = Convert.ToInt32(objPersonEntity.ID_AREA);
                    ticket.ID_AREA = ID_AREA;
                    ID_PRIO = Convert.ToInt32(objPersonEntity.ID_PRIO);
                }
                else
                {
                    FEC_TICK = ticket.FEC_TICK.Value;
                    ID_AREA = Convert.ToInt32(objPersonEntity.ID_AREA.Value);
                    ticket.ID_AREA = ID_AREA;
                    ID_PRIO = Convert.ToInt32(objPersonEntity.ID_PRIO.Value);
                    //int HOUR_s = Convert.ToInt32(dbc.PRIORITies.First(x => x.ID_PRIO == ID_PRIO).HOU_PRIO.Value);

                    HOUR_s = Convert.ToInt32(dbc.SLADetalles.Single(x => x.IdSLA == idSla && x.IdPrioridad == ID_PRIO).TiempoAtencion);
                    HOUR = Convert.ToInt32(dbc.SLADetalles.Single(x => x.IdSLA == idSla && x.IdPrioridad == ID_PRIO).TiempoAtencion);
                }
                
                if (ID_ACCO == 4)
                {
                    ID_LOCA = ticket.ID_LOCA.Value;
                }
                //try
                //{
                //    ID_LOCA = Convert.ToInt32(dbe.PERSON_LOCATION.Single(pl => pl.ID_PERS_ENTI == ID_PER_ENTI && pl.END_DATE == null && pl.VIG_LOCA == true).ID_LOCA);
                //    ticket.ID_LOCA = ID_LOCA;
                //}
                //catch { ID_LOCA = 0; ticket.ID_LOCA = ID_LOCA; }
                ticket.ID_SOUR = 3;
            }
            catch
            {   //Falta informacion
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('" + msn + "','0','0');}window.onload=init;</script>");
            }

            if (idsActivo.Length > 0)
            {
                
                if (licencias.Count > 0)
                {
                    idsActivo = idsActivo.Substring(0, idsActivo.Length);
                }
                else
                {
                    idsActivo = idsActivo.Substring(0, idsActivo.Length - 1);
                }

                string[] ids_asset = idsActivo.Split(',');

                int contador = 0;


                /**/
            
                foreach (string ida in ids_asset)
                {

                    var idsearch = Convert.ToInt32(ida);
                    //var idss = Convert.ToInt32(idUserSelect);
                    var query = dbc.CLIENT_ASSET.Where(x => x.ID_PERS_ENTI == ID_PER_ENTI && x.ID_ASSE == idsearch && x.DAT_END == null);

                    int cant = query.Count();
                    if (cant == 0)
                    {
                        msn = msn + idsearch.ToString() + ",";
                    }

                }
                if (msn.Length > 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('" + msn + "','2','0');}window.onload=init;</script>");
                }
                else
                { //Guardando el registro
                    //var ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                    var ID_PERS_ASSI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                    var ID_QUEU = Convert.ToInt32(Session["ID_QUEU"]);
                    var ID_USER = Convert.ToInt32(Session["UserId"]);

                    //if (ID_PERS_ASSI != null)
                    if (ModelState.IsValid)
                    {
                        string Code = null;
                        try
                        {
                            ticket.ID_ACCO = ID_ACCO;
                            ticket.ID_TYPE_TICK = 2;
                            ticket.ID_PERS_ENTI = ID_PER_ENTI;
                            ticket.ID_PERS_ENTI_END = ID_PER_ENTI;
                            ticket.ID_PERS_ENTI_ASSI = ID_PERS_ASSI;
                            ticket.ID_AREA = ID_AREA;
                            ticket.ID_QUEU = ID_QUEU;
                            ticket.ID_PRIO = 5;
                            ticket.IdSLA = idSla;
                            ticket.ID_STAT = 1;
                            ticket.ID_STAT_END = 1;
                            ticket.ID_SOUR = 3;
                            ticket.FEC_TICK = FEC_TICK;
                            //ticket.SUM_TICK = ticket.SUM_TICK;
                            ticket.REM_CTRL_TICK = false;
                            ticket.UserId = ID_USER;
                            ticket.CREATE_TICK = DateTime.Now;  
                            ticket.FEC_INI_TICK = DateTime.Now;
                            ticket.DAT_EXPI_TICK = DateTime.Now.AddHours(HOUR);
                            ticket.IS_PARENT = false;
                            ticket.ID_TYPE_FORM = 2;
                            ticket.ID_CATE = 181;
                            ticket.SEND_MAIL = false;
                            if (ID_ACCO == 56 || ID_ACCO == 57 || ID_ACCO == 58)
                                ticket.ID_LOCA = 0;
                            else if (ID_ACCO == 60)
                                ticket.ID_LOCA = (Request.Form["Locacion"] != "" && Request.Form["Locacion"] != null) ? Convert.ToInt32(Request.Form["Locacion"]) : ID_LOCA;
                            else
                                ticket.ID_LOCA = ticket.ID_LOCA;

                            dbc.TICKETs.Add(ticket);
                            dbc.SaveChanges();

                            int id = Convert.ToInt32(ticket.ID_TICK);

                            dbc.Entry(ticket).State = EntityState.Detached;

                            Code = dbc.TICKETs.Single(t => t.ID_TICK == id).COD_TICK;

                            if (files != null)
                            {
                                foreach (var file in files)
                                {
                                    try
                                    {
                                        //var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                                        String doc = Regex.Replace(Path.GetFileNameWithoutExtension(file.FileName), @"[^\w\.@-]", "",
                                                RegexOptions.None, TimeSpan.FromSeconds(1.5));
                                        var ATTA = new ATTACHED_TICKET_FORMAT();
                                        ATTA.NAM_ATTA = doc;
                                        ATTA.EXT_ATTA = Path.GetExtension(file.FileName);
                                        ATTA.ID_TICK = id;
                                        ATTA.CREATE_DATE = DateTime.Now;

                                        dbc.ATTACHED_TICKET_FORMAT.Add(ATTA);
                                        dbc.SaveChanges();

                                        file.SaveAs(Server.MapPath("~/Adjuntos/Reception/" + ID_ACCO.ToString() + "/") + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA_TICK_FORM) + ATTA.EXT_ATTA);
                                    }
                                    catch
                                    {

                                    }
                                }
                            }

                            if (ID_ACCO == 60) ID_AREA = 0;

                            //insertando ahora el detalle
                            foreach (string dtide in ids_asset)
                            {
                                try
                                {
                                   
                                    int ide = Convert.ToInt32(dtide);

                                    //Actualizando el estado final de la fecha fin de la tabla DETAILS_TICKET_FORMAT
                                    var query = dbc.DETAIL_TICKET_FORMAT.Where(x => x.ID_ASSE == ide && x.DATE_END == null);
                                    if (query.Count() == 1)
                                    {
                                        var detaForm = dbc.DETAIL_TICKET_FORMAT.Single(x => x.ID_ASSE == ide && x.DATE_END == null);
                                        detaForm.DATE_END = DateTime.Now;
                                        dbc.DETAIL_TICKET_FORMAT.Attach(detaForm);
                                        dbc.Entry(detaForm).State = EntityState.Modified;
                                        dbc.SaveChanges();
                                    }

                                    //Insertando nuevo registro
                                    var DetaTickForm = new DETAIL_TICKET_FORMAT();
                                    DetaTickForm.ID_ASSE = ide;
                                    DetaTickForm.ID_TICK = id;
                                    DetaTickForm.DATE_STAR = DateTime.Now;

                                    dbc.DETAIL_TICKET_FORMAT.Add(DetaTickForm);
                                    dbc.SaveChanges();

                                    //Actualizando el estado final de la fecha fin de la CLIENT_ASSET
                                    var clieAsse = dbc.CLIENT_ASSET.Single(x => x.ID_ASSE == ide && x.DAT_END == null);

                                    clieAsse.DAT_END = DateTime.Now;
                                    dbc.CLIENT_ASSET.Attach(clieAsse);
                                    dbc.Entry(clieAsse).State = EntityState.Modified;
                                    dbc.SaveChanges();

                                    //buscando la condición del activo

                                    string[] idCondicion = idsCondicion.Split(',');

                                    if (licencias.Count > 0)
                                    {
                                        idCondicion = idCondicion.Where(s => !licencias.Any(s2 => s.StartsWith(s2 + "_"))).ToArray();

                                    }


                                    int ID_COND = 9;
                                    foreach (string cond in idCondicion)
                                    {
                                        string[] idCond = cond.Split('_');
                                        if (idCond[0] == ide.ToString())
                                        {
                                            ID_COND = Convert.ToInt32(idCond[1]);
                                        }
                                    }

                                    //Registrando y asociado el activo al nuevo usuario
                                    var NewClientAsset = new CLIENT_ASSET();
                                    NewClientAsset.ID_AREA = ID_AREA;
                                    NewClientAsset.ID_ASSE = ide;
                                    NewClientAsset.ID_COND = ID_COND;
                                    //corregir 1007 al gerente de cuenta
                                    NewClientAsset.ID_PERS_ENTI = 1007;
                                    NewClientAsset.DAT_STAR = DateTime.Now;
                                    NewClientAsset.CREATE_DATE = DateTime.Now;
                                    NewClientAsset.UserId = ID_USER;
                                    if (ID_ACCO == 56 || ID_ACCO == 57 || ID_ACCO == 58)
                                    {
                                        NewClientAsset.ID_LOCA = dbc.ObtieneLocacionInicialActivo(ide).Single().ID_LOCA;
                                    }
                                    else if (ID_ACCO == 60)
                                    {
                                        NewClientAsset.ID_LOCA = (Request.Form["Locacion"] != "" && Request.Form["Locacion"] != null) ? Convert.ToInt32(Request.Form["Locacion"]) : ID_LOCA;
                                    }
                                    else
                                    {
                                        NewClientAsset.ID_LOCA = ID_LOCA;
                                    }
                                    NewClientAsset.ID_TYPE_CLIE_ASSE = 2;
                                    NewClientAsset.IdTicket = id;
                                    NewClientAsset.SUM_CLIE_ASSE = ticket.SUM_TICK;

                                    if (ID_ACCO == 60)
                                        NewClientAsset.UMinera = clieAsse.UMinera;

                                    dbc.CLIENT_ASSET.Add(NewClientAsset);
                                    dbc.SaveChanges();


                                    //Modificando Nombre Activo

                                 

                                    if (idsNombreActivo == "")
                                    {

                                    }
                                    else
                                    {
                                        string[] arrayIdsNombreActivo= idsNombreActivo.Split(',');

                                        var activo = dbc.ASSETs.SingleOrDefault(x => x.ID_ASSE == ide);
                                        if (arrayIdsNombreActivo.Length > 0)
                                        {
                                            if (activo != null)
                                            {
                                                activo.NAM_EQUI = arrayIdsNombreActivo[contador];
                                                dbc.Entry(activo).State = EntityState.Modified;
                                                dbc.SaveChanges();
                                                contador++;
                                            }
                                        }
                                    }

                                   

                                }
                                catch
                                {   //Falla al insertar en base de datos
                                    return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.uploadDone) top.uploadDone('error','1','0');}window.onload=init;</script>");
                                }
                            }
                        }
                        catch
                        {   //Falla al insertar en base de datos
                            return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('error','1','0');}window.onload=init;</script>");
                        }
                        //Se registro correctamente
                        return Content("<script type='text/javascript'> function init() {" +
          "if(top.uploadDone) top.uploadDone('OK','" + Code + "','" + ticket.ID_TICK.ToString() + "','" + idLicenciaString + "','" + ID_PER_ENTI + "');}window.onload=init;</script>");



                    }
                    else
                    {
                        var errors = ModelState.SelectMany(x => x.Value.Errors).ToList();
                        foreach (var error in errors)
                        {
                            System.Console.WriteLine(error.ErrorMessage);
                        }
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('error','0','0');}window.onload=init;</script>");
                    }
                } //fin de Guardando el registro
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','0','0');}window.onload=init;</script>");
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CreateDetailsDelivery(IEnumerable<HttpPostedFileBase> files, DETAILS_TICKET detTicket)
        {
            var ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            var ID_USER = Convert.ToInt32(Session["UserId"]);
            string dateschedule = Convert.ToString((detTicket.FEC_SCHE));
            int ID_STAT = 0;

            try { ID_STAT = Convert.ToInt32(detTicket.ID_STAT); }
            catch { ID_STAT = 0; }

            int id = 0;
            var DT = new DETAILS_TICKET();

            if (detTicket.ID_TYPE_DETA_TICK == 7)
            {
                if (files != null)
                {
                    foreach (var file in files)
                    {
                        var ext = Path.GetExtension(file.FileName);
                        if (ext.ToLower() != ".pdf")
                        {
                            return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('ERROR','4','');}window.onload=init;</script>");
                        }
                    }
                }
                else
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','3','');}window.onload=init;</script>");
                }
            }

            if (detTicket.ID_TYPE_DETA_TICK == 3)
            {
                if (ID_STAT == 5 && (String.IsNullOrEmpty(dateschedule)))
                {
                    return Content("<script type='text/javascript'> function init() {" +
                   "if(top.uploadDone) top.uploadDone('ERROR','5','');}window.onload=init;</script>");

                }
                else if (ID_STAT == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                   "if(top.uploadDone) top.uploadDone('ERROR','6','');}window.onload=init;</script>");
                }

            }

            if (detTicket.COM_DETA_TICK == null)
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('ERROR','0','');}window.onload=init;</script>");
            }
            else
            { //Validando que el comentario no sea llenado con solo espacios
                string com = detTicket.COM_DETA_TICK;
                com = com.Replace("&nbsp;", "");
                com = com.Replace("<br />", "");
                if (com.Trim().Length == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                                "if(top.uploadDone) top.uploadDone('ERROR','0','');}window.onload=init;</script>");
                }
            }

            try
            {
                DT.ID_TICK = detTicket.ID_TICK;
                DT.ID_TYPE_DETA_TICK = detTicket.ID_TYPE_DETA_TICK;
                DT.UserId = ID_USER;

                if (detTicket.ID_TYPE_DETA_TICK == 3)
                {

                    if (detTicket.ID_STAT == 5)
                    {
                        DT.FEC_SCHE = detTicket.FEC_SCHE;
                    }
                    DT.ID_STAT = ID_STAT;
                    DT.SEND_MAIL = false;
                }
                int IdComentarioTicket = Convert.ToInt32(detTicket.ID_TYPE_DETA_TICK);
                DT.CREATE_DETA_INCI = DateTime.Now;
                DT.COM_DETA_TICK = detTicket.COM_DETA_TICK;

                DT.ID_TYPE_DETA_TICK = IdComentarioTicket;
                dbc.DETAILS_TICKET.Add(DT);
                dbc.SaveChanges();

                id = Convert.ToInt32(DT.ID_DETA_TICK);
                int ID_TICK = Convert.ToInt32(DT.ID_TICK);

                //if (ID_STAT == 5)
                //{
                //    var tick = dbc.TICKETs.Single(x => x.ID_TICK == ID_TICK);
                //    tick.ID_STAT_END = 5;
                //    dbc.TICKETs.Attach(tick);
                //    dbc.Entry(tick).State = EntityState.Modified;
                //    dbc.SaveChanges();
                //}

                //Actividades
                //try
                //{
                int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);

                var RegistraActividad = (from ta in dbe.TYPE_ACT_LOG
                                         join sta in dbe.SUBTYPE_ACTITY on ta.ID_TYPE_ACT equals sta.ID_TYPE_ACT
                                         where ta.ID_ACCO == ID_ACCO && ta.VIG_ACT == true && sta.VIG_SUB_TYPE == true
                                         select new
                                         {
                                             ta.ID_TYPE_ACT,
                                             sta.ID_SUBTYPE_ACT,
                                             sta.DES_SUB_TYPE
                                         }).Where(x => x.DES_SUB_TYPE == "TICKETS").FirstOrDefault();


                var objTicket = dbc.TICKETs.Single(t => t.ID_TICK == ID_TICK);

                ACTIVITY_LOG objActividad = new ACTIVITY_LOG();
                objActividad.ID_CLIE =Convert.ToInt32(dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 6 && x.ID_ACCO == ID_ACCO && x.VIG_ACCO_PARA == true).FirstOrDefault().VAL_ACCO_PARA);
                objActividad.ID_TYPE_ACT = RegistraActividad.ID_TYPE_ACT;
                objActividad.ID_SUBTYPE_ACT = RegistraActividad.ID_SUBTYPE_ACT;
                objActividad.COD_SUBTYPE_ACT = ID_TICK;
                objActividad.NAM_SUBTYPE_ACT = objTicket.COD_TICK;
                objActividad.DATE_INIC = detTicket.FROM_TIME;
                objActividad.DATE_END = detTicket.TO_TIME;
                objActividad.COMENTARIO = "";//details_ticket.COM_DETA_TICK.Substring(0, 450);//html
                objActividad.USERID = ID_USER;
                objActividad.CREATE_ACT_LOG = DateTime.Now;
                objActividad.VIG_ACTI_LOG = true;
                objActividad.ID_PERS_ENTI = ID_PERS_ENTI;
                objActividad.CLOSE_TICKET = detTicket.ID_STAT == 4 ? true : false;
                objActividad.SCHEDULE_TICKET = detTicket.ID_STAT == 5 ? true : false;
                objActividad.TIEMPO_ACT = (int)(Convert.ToDateTime(detTicket.TO_TIME).Subtract(Convert.ToDateTime(detTicket.FROM_TIME))).TotalSeconds;
                objActividad.DETAILS_TICKETS = DT.ID_DETA_TICK;
                objActividad.ID_ACCO = ID_ACCO;

                dbe.ACTIVITY_LOG.Add(objActividad);
                dbe.SaveChanges();

                //}
                //catch
                //{

                //}
                if (files != null)
                {
                    int id_TF = dbc.TICKETs.Single(x => x.ID_TICK == ID_TICK).ID_TYPE_FORM.Value;
                    string ruta = "Delivery";
                    if (id_TF == 2) { ruta = "Reception"; }
                    foreach (var file in files)
                    {
                        try
                        {
                            var ATTA = new ATTACHED_TICKET_FORMAT();
                            ATTA.NAM_ATTA = Path.GetFileNameWithoutExtension(file.FileName);
                            ATTA.EXT_ATTA = Path.GetExtension(file.FileName);
                            ATTA.ID_DETA_TICK = id;
                            ATTA.CREATE_DATE = DateTime.Now;

                            dbc.ATTACHED_TICKET_FORMAT.Add(ATTA);
                            dbc.SaveChanges();

                            file.SaveAs(Server.MapPath("~/Adjuntos/" + ruta + "/" + ID_ACCO.ToString() + "/") + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA_TICK_FORM) + ATTA.EXT_ATTA);
                        }
                        catch
                        {

                        }
                    }
                }
                try
                {
                    //Cambiando a estado cerrado el Ticket 
                    if (detTicket.ID_TYPE_DETA_TICK == 7)
                    {
                        var ticket = new TICKET();
                        ticket = dbc.TICKETs.Single(x => x.ID_TICK == detTicket.ID_TICK);
                        if (ticket.ID_STAT_END != 6)
                        {
                            ticket.ID_STAT_END = 6;
                            ticket.MODIFIED_TICK = DateTime.Now;

                            dbc.TICKETs.Attach(ticket);
                            dbc.Entry(ticket).State = EntityState.Modified;
                            dbc.SaveChanges();
                        }
                        int IdTicketPadre = Convert.ToInt32(ticket.ID_TICK_PARENT);

                        if (IdTicketPadre != 0)
                        {
                            //Insertando comentario en el ticket de solicitud
                            var detailsTicket = new DETAILS_TICKET();
                            detailsTicket.ID_TICK = IdTicketPadre;
                            detailsTicket.ID_TYPE_DETA_TICK = 1;
                            detailsTicket.COM_DETA_TICK = "Se realizó la carga de la evidencia en el ticket de activo. ";
                            detailsTicket.UserId = 34;
                            detailsTicket.CREATE_DETA_INCI = DateTime.Now;
                            detailsTicket.MINUTES = 0;
                            dbc.DETAILS_TICKET.Add(detailsTicket);
                            dbc.SaveChanges();
                        }
                    }

                }
                catch
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','1','');}window.onload=init;</script>");
                }
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('OK','1','" + detTicket.ID_TICK.ToString() + "');}window.onload=init;</script>");
            }
            catch
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('ERROR','1','');}window.onload=init;</script>");
            }
        }

        public ActionResult FiltrarActivo(int id = 0)
        {
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            int IdChar = Convert.ToInt32(Request.Params["IdChar"]);
            string txt = Convert.ToString(Request.Params["txt"]);
            string ta = Convert.ToString(Request.Params["ta"]);
            if (txt == null) { txt = ""; }
            int idTipoActivo = 0;
            if (int.TryParse(ta, out idTipoActivo) == false) { idTipoActivo = 0; }
            if (txt == null) { txt = ""; }
            var cargo = dbc.CargoDeUsuario(IdAcco, IdChar, "ACTIVO").ToList().Single();
            //var cargo = dbc.CargoDeUsuario(IdAcco, 0, "ACTIVO").ToList().Single();
            string tipoActivo = Convert.ToString(cargo.TipoActivo);

            if (IdAcco == 60)
            {
                string uMin = Convert.ToString(Request.Params["IdUMin"]);
                string grupo = Convert.ToString(Request.Params["IdGrupo"]);
                int IdUMin = 0, IdGrupo = 0;
                if (int.TryParse(uMin, out IdUMin) == false) IdUMin = 0;
                if (int.TryParse(grupo, out IdGrupo) == false) IdGrupo = 0;

                var queryB = dbc.FiltrarActivoAsignacionBNV(IdAcco, txt, IdGrupo, IdUMin).ToList();
                return Json(new { Data = queryB }, JsonRequestBehavior.AllowGet);
            }
            else if (IdAcco == 55)
            {
                int idGrupo;
                string grupo = Convert.ToString(Request.Params["IdGrupo"]);
                if (int.TryParse(grupo, out idGrupo) == false) idGrupo = 0;

                var queryB = dbc.FiltrarActivoAsignacionPorGrupo(idGrupo, IdAcco, idTipoActivo, txt).ToList();
                return Json(new { Data = queryB }, JsonRequestBehavior.AllowGet);
            }

            var query = dbc.ActFiltrarActivo(IdAcco, txt, 0, tipoActivo, idTipoActivo).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FiltrarActivoRecepcion(int id = 0)
        {
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            string txt = Convert.ToString(Request.Params["txt"] == null ? "" : Request.Params["txt"]);
            int IdPersEnti = 0;
            try
            {
                IdPersEnti = Convert.ToInt32(Request.Params["IdPersEnti"]);
            }
            catch { }
            string ta = Convert.ToString(Request.Params["ta"]);

            int tipoActivo;
            if (int.TryParse(ta, out tipoActivo) == false) { tipoActivo = 0; }
            if (txt == "")
            {
                txt = Convert.ToString(Request.QueryString["filter[filters][0][value]"]);
                if (txt == null) { txt = ""; }
            }

            if (IdAcco == 60)
            {
                string grupo = Convert.ToString(Request.Params["IdGrupo"]);
                int IdGrupo = 0;
                if (int.TryParse(grupo, out IdGrupo) == false) IdGrupo = 0;

                var queryB = dbc.FiltrarActivoRecepcionBNV(IdAcco, txt, IdPersEnti, IdGrupo).ToList();
                return Json(new { Data = queryB }, JsonRequestBehavior.AllowGet);
            }
            else if (IdAcco == 55)
            {
                string grupo = Convert.ToString(Request.Params["IdGrupo"]);
                int IdGrupo = 0;
                if (int.TryParse(grupo, out IdGrupo) == false) IdGrupo = 0;

                var queryB = dbc.FiltrarActivoRecepcionPorGrupo(IdGrupo, IdAcco, IdPersEnti, tipoActivo, txt).ToList();
                return Json(new { Data = queryB }, JsonRequestBehavior.AllowGet);
            }

            if(IdAcco == 4) 
            {
                var activosQuery = dbc.ActFiltrarActivoRecepcion(IdAcco, txt, IdPersEnti, tipoActivo).ToList();
                var licenciasQuery = dbc.ListarLicenciaActivos(IdPersEnti).ToList();

                var combinedResult = new { Data = new List<object>() };
                combinedResult.Data.AddRange(activosQuery);
                combinedResult.Data.AddRange(licenciasQuery);

                return Json(combinedResult, JsonRequestBehavior.AllowGet);
            }

            var query = dbc.ActFiltrarActivoRecepcion(IdAcco, txt, IdPersEnti, tipoActivo).ToList();
            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CreateReasignacion(IEnumerable<HttpPostedFileBase> files, TICKET ticket, TICKET ticketAsignacion)
        {
            int recepcion = 0;
            //Validando si los Asset siguen siendo UNASSIGNED
            string idsActivo = Convert.ToString(Request.Form["idsActivo"]);
            string idsCondicion = Convert.ToString(Request.Form["idsCondicion"]);
            int IdPersAsignacion = 0;
            int IdLocacion = 0;
            string msn = "";
            int ID_PER_ENTI, ID_PRIO, ID_LOCA, ID_AREA, ID_SOUR, HOUR;
            string IDLocacion = Request.Form["IdLocacion"];
            if (Request.Form["IdPersAsignacion"] != "")
            {
                IdPersAsignacion = Convert.ToInt32(Request.Form["IdPersAsignacion"]);
            }
            else
            {
                //Falta informacion
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('0','0','0','0','0');}window.onload=init;</script>");
            }
            if (Request.Form["IdLocacion"] != "")
            {
                IdLocacion = Convert.ToInt32(Request.Form["IdLocacion"]);
            }
            else
            {
                //Falta informacion
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('0','0','0','0','0');}window.onload=init;</script>");
            }
            var objPersonEntity = dbe.PERSON_ENTITY.Single(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI.Value);

            DateTime FEC_TICK;
            try
            {
                ID_PER_ENTI = ticket.ID_PERS_ENTI.Value;
                FEC_TICK = ticket.FEC_TICK.Value;
                //ID_LOCA = ticket.ID_LOCA.Value;
                ID_AREA = Convert.ToInt32(objPersonEntity.ID_AREA.Value);
                ticket.ID_AREA = ID_AREA;
                ID_PRIO = Convert.ToInt32(objPersonEntity.ID_PRIO.Value);
                //HOUR = Convert.ToInt32(dbc.PRIORITies.First(x => x.ID_PRIO == ID_PRIO).HOU_PRIO.Value);
                HOUR = Convert.ToInt32(ObtenerHoraSLA(ticket.ID_TICK));
                ticket.ID_SOUR = 3;
            }
            catch
            {   //Falta informacion
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('" + msn + "','0','0','0','0');}window.onload=init;</script>");
            }

            if (idsActivo.Length > 0)
            {
                idsActivo = idsActivo.Substring(0, idsActivo.Length - 1);
                string[] ids_asset = idsActivo.Split(',');

                foreach (string ida in ids_asset)
                {
                    var idsearch = Convert.ToInt32(ida);
                    //var idss = Convert.ToInt32(idUserSelect);
                    var query = dbc.CLIENT_ASSET.Where(x => x.ID_PERS_ENTI == ID_PER_ENTI && x.ID_ASSE == idsearch && x.DAT_END == null);

                    int cant = query.Count();
                    if (cant == 0)
                    {
                        msn = msn + idsearch.ToString() + ",";
                    }
                }
                if (msn.Length > 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('" + msn + "','2','0','0','0');}window.onload=init;</script>");
                }
                else
                { //Guardando el registro
                    var ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                    var ID_PERS_ASSI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                    var ID_QUEU = Convert.ToInt32(Session["ID_QUEU"]);
                    var ID_USER = Convert.ToInt32(Session["UserId"]);

                    //if (ID_PERS_ASSI != null)
                    if (ModelState.IsValid)
                    {
                        string CodeRecepcion = null;
                        try
                        {
                            ticket.ID_ACCO = ID_ACCO;
                            ticket.ID_TYPE_TICK = 2;
                            ticket.ID_PERS_ENTI = ID_PER_ENTI;
                            ticket.ID_PERS_ENTI_END = ID_PER_ENTI;
                            ticket.ID_PERS_ENTI_ASSI = ID_PERS_ASSI;
                            ticket.ID_AREA = ID_AREA;
                            ticket.ID_QUEU = ID_QUEU;
                            ticket.ID_PRIO = 5;
                            ticket.ID_STAT = 1;
                            ticket.ID_STAT_END = 1;
                            ticket.ID_SOUR = 3;
                            ticket.FEC_TICK = FEC_TICK;
                            //ticket.SUM_TICK = ticket.SUM_TICK;
                            ticket.REM_CTRL_TICK = false;
                            ticket.UserId = ID_USER;
                            ticket.CREATE_TICK = DateTime.Now;
                            ticket.FEC_INI_TICK = DateTime.Now;
                            ticket.DAT_EXPI_TICK = DateTime.Now.AddHours(HOUR);
                            ticket.IS_PARENT = false;
                            ticket.ID_TYPE_FORM = 2;
                            ticket.ID_CATE = 181;
                            ticket.SEND_MAIL = false;
                            ticket.ID_LOCA = ticket.ID_LOCA;

                            dbc.TICKETs.Add(ticket);
                            dbc.SaveChanges();

                            int idRecepcion = Convert.ToInt32(ticket.ID_TICK);

                            dbc.Entry(ticket).State = EntityState.Detached;

                            CodeRecepcion = dbc.TICKETs.Single(t => t.ID_TICK == idRecepcion).COD_TICK;

                            if (files != null)
                            {
                                foreach (var file in files)
                                {
                                    try
                                    {
                                        //var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                                        var ATTA = new ATTACHED_TICKET_FORMAT();
                                        ATTA.NAM_ATTA = Path.GetFileNameWithoutExtension(file.FileName);
                                        ATTA.EXT_ATTA = Path.GetExtension(file.FileName);
                                        ATTA.ID_TICK = idRecepcion;
                                        ATTA.CREATE_DATE = DateTime.Now;

                                        dbc.ATTACHED_TICKET_FORMAT.Add(ATTA);
                                        dbc.SaveChanges();

                                        file.SaveAs(Server.MapPath("~/Adjuntos/Reception/" + ID_ACCO.ToString() + "/") + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA_TICK_FORM) + ATTA.EXT_ATTA);
                                    }
                                    catch
                                    {

                                    }
                                }
                            }

                            //insertando ahora el detalle
                            foreach (string dtide in ids_asset)
                            {
                                try
                                {
                                    int ide = Convert.ToInt32(dtide);

                                    //Actualizando el estado final de la fecha fin de la tabla DETAILS_TICKET_FORMAT
                                    var query = dbc.DETAIL_TICKET_FORMAT.Where(x => x.ID_ASSE == ide && x.DATE_END == null);
                                    if (query.Count() == 1)
                                    {
                                        var detaForm = dbc.DETAIL_TICKET_FORMAT.Single(x => x.ID_ASSE == ide && x.DATE_END == null);
                                        detaForm.DATE_END = DateTime.Now;
                                        dbc.DETAIL_TICKET_FORMAT.Attach(detaForm);
                                        dbc.Entry(detaForm).State = EntityState.Modified;
                                        dbc.SaveChanges();
                                    }

                                    //Insertando nuevo registro
                                    var DetaTickForm = new DETAIL_TICKET_FORMAT();
                                    DetaTickForm.ID_ASSE = ide;
                                    DetaTickForm.ID_TICK = idRecepcion;
                                    DetaTickForm.DATE_STAR = DateTime.Now;

                                    dbc.DETAIL_TICKET_FORMAT.Add(DetaTickForm);
                                    dbc.SaveChanges();

                                    //Actualizando el estado final de la fecha fin de la CLIENT_ASSET
                                    var clieAsse = dbc.CLIENT_ASSET.Single(x => x.ID_ASSE == ide && x.DAT_END == null);
                                    ID_LOCA = clieAsse.ID_LOCA.Value;
                                    clieAsse.DAT_END = DateTime.Now;
                                    dbc.CLIENT_ASSET.Attach(clieAsse);
                                    dbc.Entry(clieAsse).State = EntityState.Modified;
                                    dbc.SaveChanges();

                                    //buscando la condición del activo
                                    string[] idCondicion = idsCondicion.Split(',');
                                    int ID_COND = 9;
                                    foreach (string cond in idCondicion)
                                    {
                                        string[] idCond = cond.Split('_');
                                        if (idCond[0] == ide.ToString())
                                        {
                                            ID_COND = Convert.ToInt32(idCond[1]);
                                        }
                                    }

                                    //Registrando y asociado el activo al nuevo usuario
                                    var NewClientAsset = new CLIENT_ASSET();
                                    NewClientAsset.ID_AREA = ID_AREA;
                                    NewClientAsset.ID_ASSE = ide;
                                    NewClientAsset.ID_COND = ID_COND;
                                    //corregir 1007 al gerente de cuenta
                                    NewClientAsset.ID_PERS_ENTI = 1007;
                                    NewClientAsset.DAT_STAR = DateTime.Now;
                                    NewClientAsset.CREATE_DATE = DateTime.Now;
                                    NewClientAsset.UserId = ID_USER;
                                    NewClientAsset.ID_LOCA = ID_LOCA;
                                    NewClientAsset.ID_TYPE_CLIE_ASSE = 2;
                                    NewClientAsset.IdTicket = idRecepcion;
                                    NewClientAsset.SUM_CLIE_ASSE = ticket.SUM_TICK;

                                    dbc.CLIENT_ASSET.Add(NewClientAsset);
                                    dbc.SaveChanges();
                                    recepcion = 1;
                                }

                                catch
                                {   //Falla al insertar en base de datos
                                    return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.uploadDone) top.uploadDone('error','1','0','0','0');}window.onload=init;</script>");
                                }
                            }

                            //
                            //ASIGNACION DE ACTIVO
                            //

                            var objPersonEntityAsig = dbe.PERSON_ENTITY.Single(pe => pe.ID_PERS_ENTI == IdPersAsignacion);

                            if (recepcion == 1)
                            {
                                foreach (string ida in ids_asset)
                                {
                                    var idsearch = Convert.ToInt32(ida);
                                    var query = dbc.ASSETs
                                        .Where(a => a.ID_ASSE == idsearch)
                                        .Join(dbc.CLIENT_ASSET, a => a.ID_ASSE, ca => ca.ID_ASSE, (x, ca) => new
                                        {
                                            x.ID_ASSE,
                                            ca.ID_COND,
                                            ca.DAT_END
                                        }).Where(x => x.DAT_END == null && x.ID_COND != 9 && x.ID_COND != 3);

                                    int cant = query.Count();
                                    //if (cant > 0)
                                    //{
                                    //    msn = msn + idsearch.ToString() + ",";
                                    //}
                                }
                                if (msn.Length > 0)
                                {
                                    return Content("<script type='text/javascript'> function init() {" +
                                        "if(top.uploadDone) top.uploadDone('" + msn + "','2','0','0','0');}window.onload=init;</script>");
                                }
                                else
                                { //Guardando el registro
                                    try
                                    {
                                        ID_PER_ENTI = IdPersAsignacion;
                                        ID_LOCA = IdLocacion;
                                        FEC_TICK = ticketAsignacion.FEC_TICK.Value;
                                        ID_AREA = Convert.ToInt32(objPersonEntityAsig.ID_AREA.Value);
                                        ticketAsignacion.ID_AREA = ID_AREA;
                                        ID_PRIO = Convert.ToInt32(objPersonEntityAsig.ID_PRIO.Value);
                                        //HOUR = Convert.ToInt32(dbc.PRIORITies.First(x => x.ID_PRIO == ID_PRIO).HOU_PRIO.Value);
                                        HOUR = Convert.ToInt32(ObtenerHoraSLA(ticket.ID_TICK));
                                        //try
                                        //{
                                        //    ID_LOCA = Convert.ToInt32(dbe.PERSON_LOCATION.Single(pl => pl.ID_PERS_ENTI == ID_PER_ENTI && pl.END_DATE == null && pl.VIG_LOCA == true).ID_LOCA);
                                        //    ticket.ID_LOCA = ID_LOCA;
                                        //}
                                        //catch { ID_LOCA = 0; ticket.ID_LOCA = ID_LOCA; }
                                        ticketAsignacion.ID_SOUR = 3;
                                    }
                                    catch
                                    {   //Falta informacion
                                        return Content("<script type='text/javascript'> function init() {" +
                                        "if(top.uploadDone) top.uploadDone('" + msn + "','0','0','0','0');}window.onload=init;</script>");
                                    }

                                    //bool est = new bool();
                                    //est = ModelState.IsValidField("ID_ACCO");
                                    //est = ModelState.IsValidField("ID_TYPE_TICK");
                                    //est = ModelState.IsValidField("ID_CLIE");

                                    //if (ModelState.IsValid)
                                    if (ticketAsignacion.ID_PERS_ENTI != null)
                                    {
                                        string CodeAsignacion = null;
                                        try
                                        {
                                            ticketAsignacion.ID_ACCO = ID_ACCO;
                                            ticketAsignacion.ID_TYPE_TICK = 2;
                                            ticketAsignacion.ID_PERS_ENTI = ID_PER_ENTI;
                                            ticketAsignacion.ID_PERS_ENTI_END = ID_PER_ENTI;
                                            ticketAsignacion.ID_PERS_ENTI_ASSI = ID_PERS_ASSI;
                                            ticketAsignacion.ID_AREA = ID_AREA;
                                            ticketAsignacion.ID_QUEU = ID_QUEU;
                                            ticketAsignacion.ID_PRIO = 5;
                                            ticketAsignacion.ID_STAT = 1;
                                            ticketAsignacion.ID_STAT_END = 1;
                                            ticketAsignacion.ID_SOUR = 3;
                                            ticketAsignacion.FEC_TICK = FEC_TICK;
                                            //ticket.SUM_TICK = ticket.SUM_TICK;
                                            ticketAsignacion.REM_CTRL_TICK = false;
                                            ticketAsignacion.UserId = ID_USER;
                                            ticketAsignacion.CREATE_TICK = DateTime.Now;
                                            ticketAsignacion.FEC_INI_TICK = DateTime.Now;
                                            ticketAsignacion.DAT_EXPI_TICK = DateTime.Now.AddHours(HOUR);
                                            ticketAsignacion.IS_PARENT = false;
                                            ticketAsignacion.ID_TYPE_FORM = 1;
                                            ticketAsignacion.ID_CATE = 181;
                                            ticketAsignacion.SEND_MAIL = false;
                                            ticketAsignacion.ID_LOCA = ID_LOCA;

                                            dbc.TICKETs.Add(ticketAsignacion);
                                            dbc.SaveChanges();

                                            int idAsignacion = Convert.ToInt32(ticketAsignacion.ID_TICK);

                                            dbc.Entry(ticketAsignacion).State = EntityState.Detached;

                                            CodeAsignacion = dbc.TICKETs.Single(t => t.ID_TICK == idAsignacion).COD_TICK;

                                            if (files != null)
                                            {
                                                foreach (var file in files)
                                                {
                                                    try
                                                    {
                                                        //var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                                                        var ATTA = new ATTACHED_TICKET_FORMAT();
                                                        ATTA.NAM_ATTA = Path.GetFileNameWithoutExtension(file.FileName);
                                                        ATTA.EXT_ATTA = Path.GetExtension(file.FileName);
                                                        ATTA.ID_TICK = idAsignacion;
                                                        ATTA.CREATE_DATE = DateTime.Now;

                                                        //db.AddToATTACHEDs(ATTA);
                                                        dbc.ATTACHED_TICKET_FORMAT.Add(ATTA);
                                                        dbc.SaveChanges();

                                                        file.SaveAs(Server.MapPath("~/Adjuntos/Delivery/" + ID_ACCO.ToString() + "/") + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA_TICK_FORM) + ATTA.EXT_ATTA);
                                                    }
                                                    catch
                                                    {

                                                    }
                                                }
                                            }

                                            //insertando ahora el detalle
                                            foreach (string dtide in ids_asset)
                                            {
                                                try
                                                {
                                                    int ide = Convert.ToInt32(dtide);
                                                    //Actualizando el estado final de la fecha fin de la tabla DETAILS_TICKET_FORMAT
                                                    var query = dbc.DETAIL_TICKET_FORMAT.Where(x => x.ID_ASSE == ide && x.DATE_END == null);
                                                    if (query.Count() == 1)
                                                    {
                                                        var detaForm = dbc.DETAIL_TICKET_FORMAT.Single(x => x.ID_ASSE == ide && x.DATE_END == null);
                                                        detaForm.DATE_END = DateTime.Now;
                                                        dbc.DETAIL_TICKET_FORMAT.Attach(detaForm);
                                                        dbc.Entry(detaForm).State = EntityState.Modified;
                                                        dbc.SaveChanges();
                                                    }

                                                    //Registrando nuevo registro del activo
                                                    var DetaTickForm = new DETAIL_TICKET_FORMAT();
                                                    DetaTickForm.ID_ASSE = ide;
                                                    DetaTickForm.ID_TICK = idAsignacion;
                                                    DetaTickForm.DATE_STAR = DateTime.Now;

                                                    dbc.DETAIL_TICKET_FORMAT.Add(DetaTickForm);
                                                    dbc.SaveChanges();

                                                    //Actualizando el estado final de la fecha final del Client_Asset
                                                    var clieAsse = dbc.CLIENT_ASSET.Single(x => x.ID_ASSE == ide && x.DAT_END == null);

                                                    clieAsse.DAT_END = DateTime.Now;
                                                    dbc.CLIENT_ASSET.Attach(clieAsse);
                                                    dbc.Entry(clieAsse).State = EntityState.Modified;
                                                    dbc.SaveChanges();

                                                    //buscando la condición del activo
                                                    string[] idCondicion = idsCondicion.Split(',');
                                                    int ID_COND = 1;
                                                    foreach (string cond in idCondicion)
                                                    {
                                                        string[] idCond = cond.Split('_');
                                                        if (idCond[0] == ide.ToString())
                                                        {
                                                            ID_COND = Convert.ToInt32(idCond[1]);
                                                        }
                                                    }

                                                    //Registrando y asociado el activo al nuevo usuario
                                                    var NewClientAsset = new CLIENT_ASSET();
                                                    NewClientAsset.ID_AREA = ID_AREA;
                                                    NewClientAsset.ID_ASSE = ide;
                                                    NewClientAsset.ID_COND = ID_COND;
                                                    NewClientAsset.ID_PERS_ENTI = ID_PER_ENTI;
                                                    NewClientAsset.DAT_STAR = DateTime.Now;
                                                    NewClientAsset.CREATE_DATE = DateTime.Now;
                                                    NewClientAsset.UserId = ID_USER;
                                                    NewClientAsset.ID_LOCA = ticketAsignacion.ID_LOCA;
                                                    NewClientAsset.ID_TYPE_CLIE_ASSE = 1;
                                                    NewClientAsset.IdTicket = idAsignacion;
                                                    NewClientAsset.SUM_CLIE_ASSE = ticketAsignacion.SUM_TICK;

                                                    dbc.CLIENT_ASSET.Add(NewClientAsset);
                                                    dbc.SaveChanges();
                                                }
                                                catch
                                                {   //Falla al insertar en base de datos
                                                    return Content("<script type='text/javascript'> function init() {" +
                                                    "if(top.uploadDone) top.uploadDone('error','1','','0','0');}window.onload=init;</script>");
                                                }
                                            }
                                        }
                                        catch
                                        {   //Falla al insertar en base de datos
                                            //var errors = ModelState.Select(x => x.Value.Errors).ToList();                        
                                            return Content("<script type='text/javascript'> function init() {" +
                                            "if(top.uploadDone) top.uploadDone('error','1','','0','0');}window.onload=init;</script>");
                                        }
                                        //Se registro correctamente
                                        return Content("<script type='text/javascript'> function init() {" +
                                        "if(top.uploadDone) top.uploadDone('OK','" + CodeRecepcion + "','" + ticket.ID_TICK.ToString() + "','" + CodeAsignacion + "','" + ticketAsignacion.ID_TICK.ToString() + "');}window.onload=init;</script>");
                                        //recepcion = 1;
                                    }
                                    else
                                    {
                                        var errors = ModelState.Select(x => x.Value.Errors).ToList();
                                        return Content("<script type='text/javascript'> function init() {" +
                                        "if(top.uploadDone) top.uploadDone('error','0','','0','0');}window.onload=init;</script>");
                                    }
                                } //fin de Guardando el registro
                            }//Fin If Recepcion == 1
                        }
                        catch
                        {   //Falla al insertar en base de datos
                            return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('error','1','0','0','0');}window.onload=init;</script>");
                        }
                        //Se registro correctamente
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('OK','" + CodeRecepcion + "','" + ticket.ID_TICK.ToString() + "','0','0');}window.onload=init;</script>");
                    }
                    else
                    {
                        //var errors = ModelState.Select(x => x.Value.Errors).ToList();                        
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('error','0','0','0','0');}window.onload=init;</script>");
                    }
                } //fin de Guardando el registro
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','0','0','0','0');}window.onload=init;</script>");
            }
        }

        public int ObtenerHoraSLA(int idTicket = 0)
        {
            return dbc.ObtenerHoraSLA(idTicket).Single().TiempoAtencion;
        }

        public int ObtenerIdGrupoUsuarioBNV()
        {
            var grupos = new Dictionary<string, string>
                        {
                            { "GESTOR_ACTIVOS_BNV_MICROINFORMATICO", "MICROINFORMATICO" },
                            { "SUPERVISOR_ACTIVOS_BNV_MICROINFORMATICO", "MICROINFORMATICO" },
                            { "GESTOR_ACTIVOS_BNV_INFRAESTRUCTURA", "INFRAESTRUCTURA" },
                            { "SUPERVISOR_ACTIVOS_BNV_INFRAESTRUCTURA", "INFRAESTRUCTURA" },
                            { "GESTOR_ACTIVOS_BNV_MOVIL", "MOVIL" },
                            { "SUPERVISOR_ACTIVOS_BNV_MOVIL", "MOVIL" }
                        };

            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
            string nombreGrupo = grupos.FirstOrDefault(g => Convert.ToInt32(Session[g.Key]) == 1).Value;
            var grupoActivo = dbc.ACCOUNT_PARAMETER.FirstOrDefault(x => x.VAL_ACCO_PARA == nombreGrupo && x.VIG_ACCO_PARA == true && x.ID_ACCO == ID_ACCO && x.ID_PARA == 1070);

            if (grupoActivo != null)
                return grupoActivo.ID_ACCO_PARA;
            else
                return 0;
        }

        public int? ObtenerUMineraUsuarioBNV(int idPers, int idGrupo)
        {
            var usuario = dbc.UMineraxUsuario.FirstOrDefault(x => x.IdPers == idPers && x.IdGrupo == idGrupo && x.Estado == true);

            if (usuario != null)
                return usuario.UMinera;
            else
                return null;
        }

        public string ObtenerNomGrupoUsuarioBNV(int id)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
            var grupo = dbc.ACCOUNT_PARAMETER.FirstOrDefault(x => x.ID_ACCO_PARA == id);

            if (grupo != null)
                return grupo.VAL_ACCO_PARA;
            else
                return "";
        }

        public ActionResult ObtenerDatosContratista(int id = 0)
        {
            var query = dbc.ActObtenerDatosContratista(id);

            return Json(new { data = query, }, JsonRequestBehavior.AllowGet);
        }

        public int ObtenerIdGrupo(string nomGrupo)
        {
            int idAcco = Convert.ToInt32(Session["ID_ACCO"]);
            var grupo = dbc.ACCOUNT_PARAMETER.FirstOrDefault(x => x.ID_ACCO == idAcco && x.VAL_ACCO_PARA == nomGrupo && x.ID_PARA == 1070 && x.VIG_ACCO_PARA == true);

            if (grupo != null)
                return grupo.ID_ACCO_PARA;
            else
                return 0;
        }

        public string ObtenerNombreGrupo(int idGrupo)
        {
            var grupo = dbc.ACCOUNT_PARAMETER.FirstOrDefault(x => x.ID_ACCO_PARA == idGrupo);

            if (grupo != null)
                return grupo.VAL_ACCO_PARA;
            else
                return "";
        }

        [Authorize]
        public ActionResult CreateOT()
        {
            int idAcco = Convert.ToInt32(Session["ID_ACCO"]);

            if (idAcco == 55 && Convert.ToInt32(Session["GESTOR_ACTIVOS_HUDBAY_OT"]) == 1)
            {
                int idGrupoOT = ObtenerIdGrupo("OT");
                var ticket = new TICKET();
                ticket.FEC_TICK = DateTime.Now;

                ViewBag.FEC_TICK = String.Format("{0:g}", ticket.FEC_TICK);
                ViewBag.IdGrupoOT = idGrupoOT;

                return View("CreateHudbayOT", ticket);
            }
            else
            {
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CreateHudbayOT(IEnumerable<HttpPostedFileBase> files, TICKET ticket)
        {
            int idAcco = Convert.ToInt32(Session["ID_ACCO"]);
            //Validando si los Asset siguen siendo UNASSIGNED
            string idsActivo = Convert.ToString(Request.Form["idsActivo"]);
            string idsCondicion = Convert.ToString(Request.Form["idsCondicion"]);
            int idSla = Convert.ToInt32(dbc.SLAs.Single(x => x.IdCuenta == idAcco && x.Nombre == "SLA ESTANDAR" && x.Estado == true).Id);

            if (ticket.SUM_TICK == null || ticket.SUM_TICK == "")
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('INFO','Ingrese un comentario.',0);}window.onload=init;</script>");
            else if (ticket.ID_PERS_ENTI == null)
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('INFO','Seleccione un usuario.',0);}window.onload=init;</script>");
            else if (ticket.ID_LOCA == null)
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('INFO','Seleccione una locación.',0);}window.onload=init;</script>");

            if (idsActivo.Length > 0)
            {
                idsActivo = idsActivo.Substring(0, idsActivo.Length - 1);
                string[] ids_asset = idsActivo.Split(',');
                string msn = "";

                foreach (string ida in ids_asset)
                {
                    var idsearch = Convert.ToInt32(ida);
                    var query = dbc.ASSETs
                        .Where(a => a.ID_ASSE == idsearch)
                        .Join(dbc.CLIENT_ASSET, a => a.ID_ASSE, ca => ca.ID_ASSE, (x, ca) => new
                        {
                            x.ID_ASSE,
                            ca.ID_COND,
                            ca.DAT_END
                        }).Where(x => x.DAT_END == null && x.ID_COND != 9 && x.ID_COND != 3);

                    int cant = query.Count();
                }

                if (msn.Length > 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('" + msn + "','2');}window.onload=init;</script>");
                }
                else
                {   //Guardando el registro
                    var idPersAssi = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                    var idQueu = Convert.ToInt32(Session["ID_QUEU"]);
                    var userId = Convert.ToInt32(Session["UserId"]);
                    int idPersEnti, idPrio, idLoca = 0, idArea, hour = 0, hour_s = 0;

                    DateTime FEC_TICK;

                    try
                    {
                        idPersEnti = ticket.ID_PERS_ENTI.Value;
                        var objPersonEntity = dbe.PERSON_ENTITY.Single(pe => pe.ID_PERS_ENTI == idPersEnti);

                        idLoca = ticket.ID_LOCA.Value;
                        FEC_TICK = ticket.FEC_TICK.Value;
                        idArea = Convert.ToInt32(objPersonEntity.ID_AREA.Value);
                        ticket.ID_AREA = idArea;
                        idPrio = Convert.ToInt32(objPersonEntity.ID_PRIO.Value);
                        hour_s = Convert.ToInt32(dbc.SLADetalles.Single(x => x.IdSLA == idSla && x.IdPrioridad == idPrio).TiempoAtencion);
                        hour = Convert.ToInt32(dbc.SLADetalles.Single(x => x.IdSLA == idSla && x.IdPrioridad == idPrio).TiempoAtencion);
                        ticket.ID_SOUR = 3;
                    }
                    catch
                    {   //Falta informacion
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('" + msn + "','0');}window.onload=init;</script>");
                    }

                    //if (ModelState.IsValid)
                    if (ticket.ID_PERS_ENTI != null)
                    {
                        string Code = null;
                        try
                        {
                            int IdTicketPadre = Convert.ToInt32(Request.Form["IdTicket"]);
                            ticket.ID_ACCO = idAcco;
                            ticket.ID_TYPE_TICK = 2;
                            ticket.ID_PERS_ENTI = idPersEnti;
                            ticket.ID_PERS_ENTI_END = idPersEnti;
                            ticket.ID_PERS_ENTI_ASSI = idPersAssi;
                            ticket.ID_AREA = idArea;
                            ticket.ID_QUEU = idQueu;
                            ticket.ID_PRIO = 5;
                            ticket.IdSLA = idSla; //Asignacion de SLA ESTANDAR
                            ticket.ID_STAT = 1;
                            ticket.ID_STAT_END = 1;
                            ticket.ID_SOUR = 3;
                            ticket.FEC_TICK = FEC_TICK;
                            //ticket.SUM_TICK = ticket.SUM_TICK;
                            ticket.REM_CTRL_TICK = false;
                            ticket.UserId = userId;
                            ticket.CREATE_TICK = DateTime.Now;
                            ticket.FEC_INI_TICK = DateTime.Now;
                            ticket.DAT_EXPI_TICK = DateTime.Now.AddHours(hour);
                            ticket.IS_PARENT = false;
                            ticket.ID_TYPE_FORM = 1;
                            ticket.ID_CATE = 181;
                            ticket.ID_TICK_PARENT = IdTicketPadre;
                            ticket.SEND_MAIL = false;

                            dbc.TICKETs.Add(ticket);
                            dbc.SaveChanges();

                            int id = Convert.ToInt32(ticket.ID_TICK);

                            dbc.Entry(ticket).State = EntityState.Detached;

                            Code = dbc.TICKETs.Single(t => t.ID_TICK == id).COD_TICK;

                            if (files != null)
                            {
                                foreach (var file in files)
                                {
                                    try
                                    {
                                        String doc = Regex.Replace(Path.GetFileNameWithoutExtension(file.FileName), @"[^\w\.@-]", "",
                                                RegexOptions.None, TimeSpan.FromSeconds(1.5));
                                        var ATTA = new ATTACHED_TICKET_FORMAT();
                                        ATTA.NAM_ATTA = doc;
                                        ATTA.EXT_ATTA = Path.GetExtension(file.FileName);
                                        ATTA.ID_TICK = id;
                                        ATTA.CREATE_DATE = DateTime.Now;

                                        //db.AddToATTACHEDs(ATTA);
                                        dbc.ATTACHED_TICKET_FORMAT.Add(ATTA);
                                        dbc.SaveChanges();

                                        file.SaveAs(Server.MapPath("~/Adjuntos/Delivery/" + idAcco.ToString() + "/") + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA_TICK_FORM) + ATTA.EXT_ATTA);
                                    }
                                    catch
                                    {

                                    }
                                }
                            }

                            //Insertando comentario en el ticket de solicitud
                            var detailsTicket = new DETAILS_TICKET();
                            detailsTicket.ID_TICK = ticket.ID_TICK;
                            detailsTicket.ID_TYPE_DETA_TICK = 1;
                            detailsTicket.COM_DETA_TICK = "Se realizó la entrega del activo. " +
                                "<a href='/DeliveryReception/Details/" + ticket.ID_TICK + "' target='_blank'>Ir al Ticket de activo</a>";
                            detailsTicket.UserId = 34;
                            detailsTicket.CREATE_DETA_INCI = DateTime.Now;
                            detailsTicket.MINUTES = 0;
                            dbc.DETAILS_TICKET.Add(detailsTicket);
                            dbc.SaveChanges();

                            //insertando ahora el detalle
                            foreach (string dtide in ids_asset)
                            {
                                try
                                {
                                    int ide = Convert.ToInt32(dtide);
                                    //Actualizando el estado final de la fecha fin de la tabla DETAILS_TICKET_FORMAT
                                    var query = dbc.DETAIL_TICKET_FORMAT.Where(x => x.ID_ASSE == ide && x.DATE_END == null);
                                    if (query.Count() == 1)
                                    {
                                        var detaForm = dbc.DETAIL_TICKET_FORMAT.Single(x => x.ID_ASSE == ide && x.DATE_END == null);
                                        detaForm.DATE_END = DateTime.Now;
                                        dbc.DETAIL_TICKET_FORMAT.Attach(detaForm);
                                        dbc.Entry(detaForm).State = EntityState.Modified;
                                        dbc.SaveChanges();
                                    }

                                    //Registrando nuevo registro del activo
                                    var DetaTickForm = new DETAIL_TICKET_FORMAT();
                                    DetaTickForm.ID_ASSE = ide;
                                    DetaTickForm.ID_TICK = id;
                                    DetaTickForm.DATE_STAR = DateTime.Now;

                                    dbc.DETAIL_TICKET_FORMAT.Add(DetaTickForm);
                                    dbc.SaveChanges();

                                    //Actualizando el estado final de la fecha final del Client_Asset
                                    var clieAsse = dbc.CLIENT_ASSET.Single(x => x.ID_ASSE == ide && x.DAT_END == null);
                                    //int id_loca_anti = Convert.ToInt32(clieAsse.ID_LOCA);
                                    clieAsse.DAT_END = DateTime.Now;
                                    dbc.CLIENT_ASSET.Attach(clieAsse);
                                    dbc.Entry(clieAsse).State = EntityState.Modified;
                                    dbc.SaveChanges();

                                    //buscando la condición del activo
                                    string[] idCondicion = idsCondicion.Split(',');
                                    int ID_COND = 1;
                                    foreach (string cond in idCondicion)
                                    {
                                        string[] idCond = cond.Split('_');
                                        if (idCond[0] == ide.ToString())
                                        {
                                            ID_COND = Convert.ToInt32(idCond[1]);
                                        }
                                    }

                                    //Registrando y asociado el activo al nuevo usuario
                                    var NewClientAsset = new CLIENT_ASSET();
                                    NewClientAsset.ID_AREA = idArea;
                                    NewClientAsset.ID_ASSE = ide;
                                    NewClientAsset.ID_COND = ID_COND;
                                    NewClientAsset.ID_PERS_ENTI = idPersEnti;
                                    NewClientAsset.DAT_STAR = DateTime.Now;
                                    NewClientAsset.CREATE_DATE = DateTime.Now;
                                    NewClientAsset.UserId = userId;
                                    NewClientAsset.ID_LOCA = ticket.ID_LOCA;
                                    NewClientAsset.ID_TYPE_CLIE_ASSE = 1;
                                    NewClientAsset.IdTicket = id;
                                    NewClientAsset.SUM_CLIE_ASSE = ticket.SUM_TICK;

                                    dbc.CLIENT_ASSET.Add(NewClientAsset);
                                    dbc.SaveChanges();
                                }
                                catch
                                {   //Falla al insertar en base de datos
                                    return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.uploadDone) top.uploadDone('error','1','');}window.onload=init;</script>");
                                }
                            }
                        }
                        catch
                        {   //Falla al insertar en base de datos
                            //var errors = ModelState.Select(x => x.Value.Errors).ToList();                        
                            return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('error','1','');}window.onload=init;</script>");
                        }
                        //Se registro correctamente
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('OK','" + Code + "','" + ticket.ID_TICK.ToString() + "');}window.onload=init;</script>");
                    }
                    else
                    {
                        var errors = ModelState.Select(x => x.Value.Errors).ToList();
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('error','0','');}window.onload=init;</script>");
                    }
                }   //fin de Guardando el registro
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','0','');}window.onload=init;</script>");
            }
        }

        [Authorize]
        public ActionResult CreateRecepcionOT()
        {
            int idAcco = Convert.ToInt32(Session["ID_ACCO"]);

            if (idAcco == 55 && Convert.ToInt32(Session["GESTOR_ACTIVOS_HUDBAY_OT"]) == 1)
            {
                int idGrupoOT = ObtenerIdGrupo("OT");
                var ticket = new TICKET();
                ticket.FEC_TICK = DateTime.Now;

                ViewBag.FEC_TICK = String.Format("{0:g}", ticket.FEC_TICK);
                ViewBag.IdGrupoOT = idGrupoOT;

                return View("CreateRecepcionHudbayOT", ticket);
            }
            else
            {
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CreateRecepcionHudbayOT(IEnumerable<HttpPostedFileBase> files, TICKET ticket)
        {
            int idAcco = Convert.ToInt32(Session["ID_ACCO"]);

            //Validando si los Asset siguen siendo UNASSIGNED
            string idsActivo = Convert.ToString(Request.Form["idsActivo"]);
            string idsCondicion = Convert.ToString(Request.Form["idsCondicion"]);

            string msn = "";
            int idPersEnti, idPrio, idLoca = 0, idArea, idSour, hour = 0, hour_s = 0;
            
            DateTime FEC_TICK;

            int idSla = Convert.ToInt32(dbc.SLAs.Single(x => x.IdCuenta == idAcco && x.Nombre == "SLA ESTANDAR" && x.Estado == true).Id);

            if (ticket.SUM_TICK == null || ticket.SUM_TICK == "")
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('INFO','Ingrese un comentario.',0);}window.onload=init;</script>");
            else if (ticket.ID_PERS_ENTI == null)
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('INFO','Seleccione un usuario.',0);}window.onload=init;</script>");
            else if (ticket.ID_LOCA == null)
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('INFO','Seleccione una locación.',0);}window.onload=init;</script>");


            try
            {
                idPersEnti = ticket.ID_PERS_ENTI.Value;
                var objPersonEntity = dbe.PERSON_ENTITY.Single(pe => pe.ID_PERS_ENTI == idPersEnti);
                FEC_TICK = ticket.FEC_TICK.Value;
                idArea = Convert.ToInt32(objPersonEntity.ID_AREA.Value);
                ticket.ID_AREA = idArea;
                idPrio = Convert.ToInt32(objPersonEntity.ID_PRIO.Value);
                //int HOUR_s = Convert.ToInt32(dbc.PRIORITies.First(x => x.ID_PRIO == ID_PRIO).HOU_PRIO.Value);

                hour_s = Convert.ToInt32(dbc.SLADetalles.Single(x => x.IdSLA == idSla && x.IdPrioridad == idPrio).TiempoAtencion);
                hour = Convert.ToInt32(dbc.SLADetalles.Single(x => x.IdSLA == idSla && x.IdPrioridad == idPrio).TiempoAtencion);

                ticket.ID_SOUR = 3;
            }
            catch
            {   //Falta informacion
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('" + msn + "','0','0');}window.onload=init;</script>");
            }

            if (idsActivo.Length > 0)
            {
                idsActivo = idsActivo.Substring(0, idsActivo.Length - 1);
                string[] ids_asset = idsActivo.Split(',');

                int contador = 0;

                foreach (string ida in ids_asset)
                {

                    var idsearch = Convert.ToInt32(ida);
                    //var idss = Convert.ToInt32(idUserSelect);
                    var query = dbc.CLIENT_ASSET.Where(x => x.ID_PERS_ENTI == idPersEnti && x.ID_ASSE == idsearch && x.DAT_END == null);

                    int cant = query.Count();
                    if (cant == 0)
                    {
                        msn = msn + idsearch.ToString() + ",";
                    }

                }
                if (msn.Length > 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('" + msn + "','2','0');}window.onload=init;</script>");
                }
                else
                {   //Guardando el registro
                    var idPersAssi = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                    var idQueu = Convert.ToInt32(Session["ID_QUEU"]);
                    var userId = Convert.ToInt32(Session["UserId"]);

                    //if (ModelState.IsValid)
                    if (ticket.ID_PERS_ENTI != null)
                    {
                        string Code = null;
                        try
                        {
                            ticket.ID_ACCO = idAcco;
                            ticket.ID_TYPE_TICK = 2;
                            ticket.ID_PERS_ENTI = idPersEnti;
                            ticket.ID_PERS_ENTI_END = idPersEnti;
                            ticket.ID_PERS_ENTI_ASSI = idPersAssi;
                            ticket.ID_AREA = idArea;
                            ticket.ID_QUEU = idQueu;
                            ticket.ID_PRIO = 5;
                            ticket.IdSLA = idSla;
                            ticket.ID_STAT = 1;
                            ticket.ID_STAT_END = 1;
                            ticket.ID_SOUR = 3;
                            ticket.FEC_TICK = FEC_TICK;
                            //ticket.SUM_TICK = ticket.SUM_TICK;
                            ticket.REM_CTRL_TICK = false;
                            ticket.UserId = userId;
                            ticket.CREATE_TICK = DateTime.Now;
                            ticket.FEC_INI_TICK = DateTime.Now;
                            ticket.DAT_EXPI_TICK = DateTime.Now.AddHours(hour);
                            ticket.IS_PARENT = false;
                            ticket.ID_TYPE_FORM = 2;
                            ticket.ID_CATE = 181;
                            ticket.SEND_MAIL = false;
                            ticket.ID_LOCA = ticket.ID_LOCA;

                            dbc.TICKETs.Add(ticket);
                            dbc.SaveChanges();

                            int id = Convert.ToInt32(ticket.ID_TICK);

                            dbc.Entry(ticket).State = EntityState.Detached;

                            Code = dbc.TICKETs.Single(t => t.ID_TICK == id).COD_TICK;

                            if (files != null)
                            {
                                foreach (var file in files)
                                {
                                    try
                                    {
                                        //var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                                        String doc = Regex.Replace(Path.GetFileNameWithoutExtension(file.FileName), @"[^\w\.@-]", "",
                                                RegexOptions.None, TimeSpan.FromSeconds(1.5));
                                        var ATTA = new ATTACHED_TICKET_FORMAT();
                                        ATTA.NAM_ATTA = doc;
                                        ATTA.EXT_ATTA = Path.GetExtension(file.FileName);
                                        ATTA.ID_TICK = id;
                                        ATTA.CREATE_DATE = DateTime.Now;

                                        dbc.ATTACHED_TICKET_FORMAT.Add(ATTA);
                                        dbc.SaveChanges();

                                        file.SaveAs(Server.MapPath("~/Adjuntos/Reception/" + idAcco.ToString() + "/") + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA_TICK_FORM) + ATTA.EXT_ATTA);
                                    }
                                    catch
                                    {

                                    }
                                }
                            }

                            idArea = 0;

                            //insertando ahora el detalle
                            foreach (string dtide in ids_asset)
                            {
                                try
                                {

                                    int ide = Convert.ToInt32(dtide);

                                    //Actualizando el estado final de la fecha fin de la tabla DETAILS_TICKET_FORMAT
                                    var query = dbc.DETAIL_TICKET_FORMAT.Where(x => x.ID_ASSE == ide && x.DATE_END == null);
                                    if (query.Count() == 1)
                                    {
                                        var detaForm = dbc.DETAIL_TICKET_FORMAT.Single(x => x.ID_ASSE == ide && x.DATE_END == null);
                                        detaForm.DATE_END = DateTime.Now;
                                        dbc.DETAIL_TICKET_FORMAT.Attach(detaForm);
                                        dbc.Entry(detaForm).State = EntityState.Modified;
                                        dbc.SaveChanges();
                                    }

                                    //Insertando nuevo registro
                                    var DetaTickForm = new DETAIL_TICKET_FORMAT();
                                    DetaTickForm.ID_ASSE = ide;
                                    DetaTickForm.ID_TICK = id;
                                    DetaTickForm.DATE_STAR = DateTime.Now;

                                    dbc.DETAIL_TICKET_FORMAT.Add(DetaTickForm);
                                    dbc.SaveChanges();

                                    //Actualizando el estado final de la fecha fin de la CLIENT_ASSET
                                    var clieAsse = dbc.CLIENT_ASSET.Single(x => x.ID_ASSE == ide && x.DAT_END == null);

                                    clieAsse.DAT_END = DateTime.Now;
                                    dbc.CLIENT_ASSET.Attach(clieAsse);
                                    dbc.Entry(clieAsse).State = EntityState.Modified;
                                    dbc.SaveChanges();

                                    //buscando la condición del activo

                                    string[] idCondicion = idsCondicion.Split(',');

                                    int ID_COND = 9;
                                    foreach (string cond in idCondicion)
                                    {
                                        string[] idCond = cond.Split('_');
                                        if (idCond[0] == ide.ToString())
                                        {
                                            ID_COND = Convert.ToInt32(idCond[1]);
                                        }
                                    }

                                    //Registrando y asociado el activo al nuevo usuario
                                    var NewClientAsset = new CLIENT_ASSET();
                                    NewClientAsset.ID_AREA = idArea;
                                    NewClientAsset.ID_ASSE = ide;
                                    NewClientAsset.ID_COND = ID_COND;
                                    //corregir 1007 al gerente de cuenta
                                    NewClientAsset.ID_PERS_ENTI = 1007;
                                    NewClientAsset.DAT_STAR = DateTime.Now;
                                    NewClientAsset.CREATE_DATE = DateTime.Now;
                                    NewClientAsset.UserId = userId;
                                    NewClientAsset.ID_LOCA = idLoca;
                                    NewClientAsset.ID_TYPE_CLIE_ASSE = 2;
                                    NewClientAsset.IdTicket = id;
                                    NewClientAsset.SUM_CLIE_ASSE = ticket.SUM_TICK;

                                    dbc.CLIENT_ASSET.Add(NewClientAsset);
                                    dbc.SaveChanges();
                                }
                                catch
                                {   //Falla al insertar en base de datos
                                    return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.uploadDone) top.uploadDone('error','1','0');}window.onload=init;</script>");
                                }
                            }
                        }
                        catch
                        {   //Falla al insertar en base de datos
                            return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('error','1','0');}window.onload=init;</script>");
                        }
                        //Se registro correctamente
                        return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('OK','" + Code + "','" + ticket.ID_TICK.ToString() + "');}window.onload=init;</script>");

                    }
                    else
                    {
                        var errors = ModelState.SelectMany(x => x.Value.Errors).ToList();
                        foreach (var error in errors)
                        {
                            System.Console.WriteLine(error.ErrorMessage);
                        }
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('error','0','0');}window.onload=init;</script>");
                    }
                } //fin de Guardando el registro
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','0','0');}window.onload=init;</script>");
            }
        }

    }
}
