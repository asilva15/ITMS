using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.IO;
//using System.Globalization;

namespace ERPElectrodata.Controllers
{
    public class ChangeController : Controller
    {
        private CoreEntities dbc = new CoreEntities();
        private EntityEntities dbe = new EntityEntities();

        //
        // GET: /Change/
        [Authorize]
        public ActionResult Index()
        {
            Session["MAIN"] = "mp3";
            //var assets = db.ASSETs.Include(a => a.MANUFACTURER_MODEL).Include(a => a.TICKET).Include(a => a.TYPE_ASSET);
            //return View(assets.ToList());
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            var query = dbc.TICKETs.Where(a => a.ID_ACCO == ID_ACCO)
                .Join(dbc.TYPE_TICKET, a => a.ID_TYPE_TICK, tt => tt.ID_TYPE_TICK, (a, tt) => new
                {
                    a.ID_STAT_END,
                    a.ID_PERS_ENTI_ASSI,
                    tt.NAM_TYPE_TICK
                }).Where(x => x.NAM_TYPE_TICK == "CHANGE");
                
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            ViewBag.Unaprovals = query.Where(x => x.ID_STAT_END == 7 && x.ID_PERS_ENTI_ASSI == ID_PERS_ENTI).Count();
            ViewBag.Aprovals = query.Where(x => x.ID_STAT_END == 8 && x.ID_PERS_ENTI_ASSI == ID_PERS_ENTI).Count();
            ViewBag.Rejected = query.Where(x => x.ID_STAT_END == 9 && x.ID_PERS_ENTI_ASSI == ID_PERS_ENTI).Count();
            ViewBag.Executed = query.Where(x => x.ID_STAT_END == 10 && x.ID_PERS_ENTI_ASSI == ID_PERS_ENTI).Count();

            ViewBag.TUnaprovals = query.Where(x => x.ID_STAT_END == 7).Count();
            ViewBag.TAprovals = query.Where(x => x.ID_STAT_END == 8).Count();
            ViewBag.TRejected = query.Where(x => x.ID_STAT_END == 9).Count();
            ViewBag.TExecuted = query.Where(x => x.ID_STAT_END == 10).Count();

            return View();
        }

        public ActionResult ListTicketByStatus(int id = 0)
        {
            int UserId = Convert.ToInt32(Session["UserId"]);
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int ID_STAT = Convert.ToInt32(Request.Params["filter[filters][0][value]"].ToString());
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());
            int Count = 0;

            var tick = dbc.TICKETs.Where(a => a.ID_ACCO == ID_ACCO)
                        .Join(dbc.TYPE_TICKET, a => a.ID_TYPE_TICK, tt => tt.ID_TYPE_TICK, (a, tt) => new
                        {
                            a.ID_TICK,
                            a.COD_TICK,
                            a.SUM_TICK,
                            a.ID_ACCO,
                            a.ID_PRIO,
                            a.ID_STAT_END,
                            a.ID_PERS_ENTI_ASSI,
                            a.ID_PERS_ENTI_END,
                            a.FEC_INI_TICK,
                            a.CREATE_TICK,
                            a.MODIFIED_TICK,
                            a.ID_SOUR,
                            a.ID_CATE,
                            a.UserId,
                            a.IS_PARENT,
                            tt.NAM_TYPE_TICK
                        }).Where(x => x.NAM_TYPE_TICK == "CHANGE");

            if (Convert.ToBoolean(Session["VIS_ALL_CHAN"].ToString()) == false)
            {
                tick = tick.Where(i => i.ID_PERS_ENTI_ASSI == ID_PERS_ENTI);
            }
            else {
                int cont = dbc.ACCOUNT_ALLOW.Where(x => x.ID_ACCO == ID_ACCO && x.ID_PERS_ENTI == ID_PERS_ENTI).Count();
                if (cont == 0) {
                    tick = tick.Where(i => i.ID_PERS_ENTI_ASSI == ID_PERS_ENTI);
                }
            }
            if (ID_STAT == 0)
            {
                tick = tick.Where(t => t.ID_STAT_END == 7);
                Count = tick.Count();
                tick = tick.OrderByDescending(t => t.CREATE_TICK).Skip(skip).Take(take);
            }
            else
            {
                tick = tick.Where(t => t.ID_STAT_END == ID_STAT);
                Count = tick.Count();
                tick = tick.OrderByDescending(t => t.CREATE_TICK).Skip(skip).Take(take);
            }

            var listuser = (from lu in dbe.ACCOUNT_ENTITY
                            join pe in dbe.PERSON_ENTITY on lu.ID_PERS_ENTI equals pe.ID_PERS_ENTI
                            join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                            where lu.ID_ACCO == ID_ACCO
                            select new
                            {
                                lu.ID_PERS_ENTI,
                                Client = ce.FIR_NAME + " " + ce.LAS_NAME,
                                ce.UserId,
                            }).ToList();

            var result = (from i in tick.ToList()
                          join so in dbc.SOURCEs on i.ID_SOUR equals so.ID_SOUR
                          join s in dbc.STATUS on i.ID_STAT_END equals s.ID_STAT
                          //join tt in db.TYPE_TICKET on i.ID_TYPE_TICK equals tt.ID_TYPE_TICK
                          join c4 in dbc.CATEGORies on i.ID_CATE equals c4.ID_CATE
                          join c3 in dbc.CATEGORies on c4.ID_CATE_PARE equals c3.ID_CATE
                          join pr in dbc.PRIORITies on i.ID_PRIO equals pr.ID_PRIO
                          join cr in listuser on i.UserId equals cr.UserId
                          join ac in dbc.ACCOUNTs on i.ID_ACCO equals ac.ID_ACCO
                          join asi in listuser on i.ID_PERS_ENTI_ASSI equals asi.ID_PERS_ENTI
                          join re in listuser on i.ID_PERS_ENTI_END equals re.ID_PERS_ENTI
                          select new
                          {
                              ID_CHAN = i.ID_TICK,
                              COD_CHAN = i.COD_TICK,
                              SUM_CHAN = i.SUM_TICK,
                              NAM_STAT = s.NAM_STAT.ToLower(),
                              CLA_STAT = s.CLA_STAT,
                              NAM_SOUR = so.NAM_SOUR.ToLower(),
                              NAM_SUBC = c4.NAM_CATE.ToLower(),
                              NAM_CLAS = c3.NAM_CATE.ToLower(),
                              NAM_TYPE_TICK = "Change",
                              REQU = re.Client.ToLower(),
                              CREATE_CHAN = String.Format("{0:G}", i.CREATE_TICK),
                              MODIFIED_CHAN = String.Format("{0:G}", i.MODIFIED_TICK),
                              SCHEDULED_CHAN = String.Format("{0:G}", i.FEC_INI_TICK),
                              NAM_PRIO = pr.NAM_PRIO.ToLower(),
                              ICO_PRIO = pr.ICO_PRIO,
                              CREATEBY = cr.Client.ToLower(),
                              ACCO = ac.NAM_ACCO,
                              ASSI = asi.Client.ToLower(),
                              PARENT = (i.IS_PARENT == true ? "expand" : "none")
                          });

            return Json(new { Data = result, Count = Count }, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Change/Create
        public ActionResult Create()
        {
            var tickCH = new TICKET();
            int ID_ASSI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int ID_USER = Convert.ToInt32(Session["UserId"]);
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int ID_QUEU = Convert.ToInt32(Session["ID_QUEU"]);

            tickCH.FEC_TICK = DateTime.Now;
            tickCH.ID_TYPE_TICK = 3;
            tickCH.ID_PRIO = 4;
            tickCH.ID_SOUR = 1;
            tickCH.ID_STAT = 7;
            tickCH.ID_STAT_END = 7;
            tickCH.ID_QUEU = ID_QUEU;
            tickCH.ID_ACCO = ID_ACCO;
            tickCH.ID_PERS_ENTI_ASSI = ID_ASSI;
            tickCH.UserId = ID_USER;

            ViewBag.FEC_TICK = String.Format("{0:g}", tickCH.FEC_TICK);
            return View(tickCH);
        }

        //
        // POST: /Change/Create
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(IEnumerable<HttpPostedFileBase> files, TICKET ticket)
        {             
            string FEC_INI_TICK = Convert.ToString(Request.Form["FEC_INI_TICK"]);
            string S_ID_PERS_ENTI = Convert.ToString(Request.Form["ID_PERS_ENTI"]);
            int ID_PERS_ENTI, ID_CATE;
            int sw = 0;

            string S_ID_CATE = Convert.ToString(ticket.ID_CATE);

            if (int.TryParse(S_ID_PERS_ENTI, out ID_PERS_ENTI) == false) { sw = 1; }
            else if (int.TryParse(S_ID_CATE, out ID_CATE) == false) { sw = 1; }

            if (sw == 1)
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
            }
            else if (String.IsNullOrEmpty(FEC_INI_TICK))
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
            }                     

            int ID_AREA = Convert.ToInt32(dbe.PERSON_ENTITY.Single(r => r.ID_PERS_ENTI == ID_PERS_ENTI).ID_AREA);

            ticket.ID_AREA = ID_AREA;
            ticket.REM_CTRL_TICK = false;
            ticket.IS_PARENT = false;
            ticket.CREATE_TICK = DateTime.Now;
            ticket.FEC_INI_TICK = DateTime.Now;
            ticket.MODIFIED_TICK = DateTime.Now;
            //ticket.ID_STAT = 1;            
            //ticket.ID_STAT_END = 5;
            //ticket.ID_PRIO = 5;

            if (ModelState.IsValid)
            {
                //Guardando el Ticket por Change
                dbc.TICKETs.Add(ticket);
                dbc.SaveChanges();

                int id = Convert.ToInt32(ticket.ID_TICK);
                dbc.Entry(ticket).State = EntityState.Detached;

                string code = Convert.ToString(dbc.TICKETs.Single(t => t.ID_TICK == id).COD_TICK);

                //Guardando y registrando los archivos
                if (files != null) {
                    foreach (var file in files)
                    {
                        try
                        {
                            var ATTA = new ATTACHED();
                            ATTA.NAM_ATTA = Path.GetFileNameWithoutExtension(file.FileName);
                            ATTA.EXT_ATTA = Path.GetExtension(file.FileName);
                            ATTA.ID_INCI = ticket.ID_TICK;
                            ATTA.CREATE_ATTA = DateTime.Now;

                            dbc.ATTACHEDs.Add(ATTA);
                            dbc.SaveChanges();

                            file.SaveAs(Server.MapPath("~/Adjuntos/Change/") + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA) + ATTA.EXT_ATTA);
                        }
                        catch
                        {

                        }
                    }                
                }
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','" + code + "');}window.onload=init;</script>");
            }
            else {
                //var errors = ModelState.Select(x => x.Value.Errors).ToList(); 
                return Content("<script type='text/javascript'> function init() {" +
                       "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");            
            }            
        }

        //
        // GET: /Change/Find
        public ActionResult Find()
        {
            return View();
        }

        //
        // GET: /Change/Find
        public ActionResult FindChange()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            int Count = 0, UserId, ID_QUEU, ID_PERS_ENTI_ASSI, ID_PRIO, ID_PERS_ENTI, ID_PERS_ENTI_END;
            int ID_CATE, ID_SUB_CATE, ID_CLAS, ID_SUB_CLAS;
            DateTime START_DATE, END_DATE, SCHE_DATE;

            string COD_TICK = Convert.ToString(Request.Params["COD_TICK"]),
                    SUM_TICK = Convert.ToString(Request.Params["SUM_TICK"]),
                    from = "", to = "";

            var listCate = dbc.CATEGORies
                .Join(dbc.CATEGORies, c1 => c1.ID_CATE, c2 => c2.ID_CATE_PARE, (c1, c2) => new { 
                    ID_CATE_1 = c1.ID_CATE, 
                    ID_CATE_2 = c2.ID_CATE 
                })
                .Join(dbc.CATEGORies, c2 => c2.ID_CATE_2, c3 => c3.ID_CATE_PARE, (c2, c3) => new { 
                    c2.ID_CATE_1, 
                    c2.ID_CATE_2, 
                    ID_CATE_3 = c3.ID_CATE 
                })
                .Join(dbc.CATEGORies, c3 => c3.ID_CATE_3, c4 => c4.ID_CATE_PARE, (c3, c4) => new { 
                    c3.ID_CATE_1, 
                    c3.ID_CATE_2, 
                    c3.ID_CATE_3, 
                    ID_CATE_4 = c4.ID_CATE 
                });

            int xcd = listCate.Count();

            if (Int32.TryParse(Convert.ToString(Request.Params["ID_CATE"]), out ID_CATE) == true)
            {
                listCate = listCate.Where(t => t.ID_CATE_1 == ID_CATE);
            }

            try
            {
                if (Int32.TryParse(Convert.ToString(Request.Params["ID_SUB_CATE"]), out ID_SUB_CATE) == true)
                {
                    listCate = listCate.Where(t => t.ID_CATE_2 == ID_SUB_CATE);
                }
            }
            catch
            {

            }

            try
            {
                if (Int32.TryParse(Convert.ToString(Request.Params["ID_CLAS"]), out ID_CLAS) == true)
                {
                    listCate = listCate.Where(t => t.ID_CATE_3 == ID_CLAS);
                }
            }
            catch
            {

            }

            try
            {
                if (Int32.TryParse(Convert.ToString(Request.Params["ID_SUB_CLAS"]), out ID_SUB_CLAS) == true)
                {
                    listCate = listCate.Where(t => t.ID_CATE_4 == ID_SUB_CLAS);
                }
            }
            catch
            {

            }

            var listClient = (from lu in dbe.ACCOUNT_ENTITY
                              join pe in dbe.PERSON_ENTITY on lu.ID_PERS_ENTI equals pe.ID_PERS_ENTI
                              join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                              where lu.ID_ACCO == ID_ACCO
                              select new
                              {
                                  lu.ID_PERS_ENTI,
                                  Client = ce.FIR_NAME + " " + ce.LAS_NAME,
                                  ce.UserId,
                              }).ToList();

            var tick = (from t in dbc.TICKETs
                        where t.ID_ACCO == ID_ACCO && t.ID_TYPE_TICK == 3
                        join lc in listCate on t.ID_CATE equals lc.ID_CATE_4
                        select new
                        {
                            t.ID_ACCO,
                            t.FEC_TICK,
                            t.FEC_INI_TICK,
                            t.COD_TICK,
                            t.UserId,
                            t.ID_QUEU,
                            t.ID_PERS_ENTI_ASSI,
                            t.ID_TYPE_TICK,
                            t.ID_PRIO,
                            t.ID_SOUR,
                            t.ID_STAT_END,
                            t.SUM_TICK,
                            t.ID_PERS_ENTI,
                            t.ID_PERS_ENTI_END,
                            t.ID_CATE,
                            t.CREATE_TICK,
                            t.MODIFIED_TICK,
                            t.ID_TICK,
                            t.IS_PARENT
                        });

            ////SEIS ÚLTIMOS MESES DEL TICKET DE LA CUENTA
            //var legendx = (from t in tick
            //               group t by new { t.FEC_TICK.Value.Year, t.FEC_TICK.Value.Month } into g
            //               select new
            //               {
            //                   g.Key.Year,
            //                   g.Key.Month,
            //                   Count = g.Count(),
            //               }).OrderByDescending(x => x.Year).ThenByDescending(x => x.Month).Take(5);

            //var legend = (from t in legendx
            //              join am in dbc.ACCOUNTING_MONTH on t.Year equals am.ID_ACCO_YEAR
            //              where t.Month == am.ACCO_MONT
            //              select new
            //              {
            //                  t.Year,
            //                  t.Month,
            //                  t.Count,
            //                  name = am.NAM_ACCO_MONT.Substring(0, 1) + am.NAM_ACCO_MONT.Substring(1, 2).ToLower()
            //              });

            if (!String.IsNullOrEmpty(COD_TICK))
            {
                tick = tick.Where(t => t.COD_TICK.ToUpper().Contains(COD_TICK.ToUpper()));
            }
            if (Int32.TryParse(Convert.ToString(Request.Params["UserId"]), out UserId) == true)
            {
                tick = tick.Where(t => t.UserId == UserId);
            }
            if (Int32.TryParse(Convert.ToString(Request.Params["ID_QUEU"]), out ID_QUEU) == true)
            {
                tick = tick.Where(t => t.ID_QUEU == ID_QUEU);
            }

            try
            {
                if (Int32.TryParse(Convert.ToString(Request.Params["ID_PERS_ENTI_ASSI"]), out ID_PERS_ENTI_ASSI) == true)
                {
                    tick = tick.Where(t => t.ID_PERS_ENTI_ASSI == ID_PERS_ENTI_ASSI);
                }
            }
            catch { }

            if (Int32.TryParse(Convert.ToString(Request.Params["ID_PRIO"]), out ID_PRIO) == true)
            {
                tick = tick.Where(t => t.ID_PRIO == ID_PRIO);
            }

            if (!String.IsNullOrEmpty(SUM_TICK))
            {
                tick = tick.Where(t => t.SUM_TICK.ToUpper().Contains(SUM_TICK.ToUpper()));
            }

            if (Int32.TryParse(Convert.ToString(Request.Params["ID_PERS_ENTI"]), out ID_PERS_ENTI) == true)
            {
                tick = tick.Where(t => t.ID_PERS_ENTI == ID_PERS_ENTI);
            }

            if (Int32.TryParse(Convert.ToString(Request.Params["ID_PERS_ENTI_END"]), out ID_PERS_ENTI_END) == true)
            {
                tick = tick.Where(t => t.ID_PERS_ENTI_END == ID_PERS_ENTI_END);
            }

            //var lineClose = (from x in legend
            //                 join t in tick on x.Year equals t.FEC_TICK.Value.Year
            //                 where x.Month == t.FEC_TICK.Value.Month && t.ID_STAT_END == 6
            //                 group t by new { t.ID_STAT_END, x.Year, x.Month, x.Count, x.name } into g
            //                 select new
            //                 {
            //                     g.Key.ID_STAT_END,
            //                     g.Key.Year,
            //                     g.Key.Month,
            //                     g.Key.Count,
            //                     g.Key.name,
            //                     RES_COUN = g.Count()

            //                 }).OrderByDescending(x => x.Year).ThenByDescending(x => x.Month);

            //var lineOpen = (from x in legend
            //                join t in tick on x.Year equals t.FEC_TICK.Value.Year
            //                where x.Month == t.FEC_TICK.Value.Month && (t.ID_STAT_END == 1 || t.ID_STAT_END == 3)
            //                group t by new { t.ID_STAT_END, x.Year, x.Month, x.Count, x.name } into g
            //                select new
            //                {
            //                    g.Key.ID_STAT_END,
            //                    g.Key.Year,
            //                    g.Key.Month,
            //                    g.Key.Count,
            //                    g.Key.name,
            //                    RES_COUN = g.Count()

            //                }).OrderByDescending(x => x.Year).ThenByDescending(x => x.Month);

            //var lineScheduled = (from x in legend
            //                     join t in tick on x.Year equals t.FEC_TICK.Value.Year
            //                     where x.Month == t.FEC_TICK.Value.Month && (t.ID_STAT_END == 5)
            //                     group t by new { t.ID_STAT_END, x.Year, x.Month, x.Count, x.name } into g
            //                     select new
            //                     {
            //                         g.Key.ID_STAT_END,
            //                         g.Key.Year,
            //                         g.Key.Month,
            //                         g.Key.Count,
            //                         g.Key.name,
            //                         RES_COUN = g.Count()

            //                     }).OrderByDescending(x => x.Year).ThenByDescending(x => x.Month);

            if (DateTime.TryParse(Convert.ToString(Request.Params["SCHE_DATE"]), out SCHE_DATE))
            {
                tick = tick.Where(t => t.FEC_INI_TICK == SCHE_DATE);
            }
            if (DateTime.TryParse(Convert.ToString(Request.Params["START_DATE"]), out START_DATE))
            {
                tick = tick.Where(t => t.FEC_TICK >= START_DATE);
            }

            if (DateTime.TryParse(Convert.ToString(Request.Params["END_DATE"]), out END_DATE))
            {
                END_DATE = ((END_DATE.AddHours(23)).AddMinutes(59)).AddSeconds(59);
                tick = tick.Where(t => t.FEC_TICK <= END_DATE);
            }

            Count = tick.Count();
            //CountClose = tick.Where(t => t.ID_STAT_END == 6).Count();
            try
            {
                from = String.Format("{0:d}", tick.OrderBy(t => t.FEC_TICK).First().FEC_TICK);
                to = String.Format("{0:d}", tick.OrderByDescending(t => t.FEC_TICK).First().FEC_TICK);
            }
            catch
            {

            }

            //var resultPie = (from t in tick
            //                 join st in dbc.STATUS on t.ID_STAT_END equals st.ID_STAT
            //                 where t.ID_STAT_END != 6 && t.ID_STAT_END != 2
            //                 group st by new { st.NAM_GRAP_STAT, st.COL_GRAP_STAT } into g
            //                 select new
            //                 {  name = g.Key.NAM_GRAP_STAT.Substring(0, 1) + g.Key.NAM_GRAP_STAT.Substring(1, g.Key.NAM_GRAP_STAT.Length).ToLower(),
            //                    color = g.Key.COL_GRAP_STAT,
            //                    y = g.Count()
            //                 }).ToList();

            tick = tick.OrderByDescending(t => t.FEC_TICK).ThenByDescending(t => t.ID_STAT_END).Skip(skip).Take(take);

            var result = (from i in tick.ToList()
                          join so in dbc.SOURCEs on i.ID_SOUR equals so.ID_SOUR
                          join s in dbc.STATUS on i.ID_STAT_END equals s.ID_STAT
                          join tt in dbc.TYPE_TICKET on i.ID_TYPE_TICK equals tt.ID_TYPE_TICK
                          join c4 in dbc.CATEGORies on i.ID_CATE equals c4.ID_CATE
                          join c3 in dbc.CATEGORies on c4.ID_CATE_PARE equals c3.ID_CATE
                          join pr in dbc.PRIORITies on i.ID_PRIO equals pr.ID_PRIO
                          join cr in listClient on i.UserId equals cr.UserId
                          join ac in dbc.ACCOUNTs on i.ID_ACCO equals ac.ID_ACCO
                          join asi in listClient on i.ID_PERS_ENTI_ASSI equals asi.ID_PERS_ENTI
                          join re in listClient on i.ID_PERS_ENTI_END equals re.ID_PERS_ENTI
                          select new
                          {
                              ID_INCI = i.ID_TICK,
                              COD_INCI = i.COD_TICK,
                              SUM_INCI = i.SUM_TICK,
                              NAM_STAT = s.NAM_STAT.ToLower(),
                              CLA_STAT = s.CLA_STAT,
                              NAM_SOUR = so.NAM_SOUR.ToLower(),
                              NAM_SUBC = c4.NAM_CATE.ToLower(),
                              NAM_CATE = c3.NAM_CATE.ToLower(),
                              NAM_TYPE_TICK = tt.NAM_TYPE_TICK.ToLower(),
                              REQU = re.Client.ToLower(),
                              CREATE_INCI = String.Format("{0:G}", i.CREATE_TICK),
                              MODIFIED_INCI = String.Format("{0:G}", i.MODIFIED_TICK),
                              ID_PRIO = pr.ID_PRIO,
                              NAM_PRIO = pr.NAM_PRIO.ToLower(),
                              ICO_PRIO = pr.ICO_PRIO,
                              CREATEBY = cr.Client.ToLower(),
                              ACCO = ac.NAM_ACCO,
                              ASSI = asi.Client.ToLower(),
                              EXP_TIME = ExpTime(i.ID_TICK),
                              PARENT = (i.IS_PARENT == true ? "expand" : "none")
                          });

            return Json(new { Data = result, Count = Count, From = from, To = to}, JsonRequestBehavior.AllowGet);
        }

        //funcion para sacar expiration time
        public string ExpTime(int id)
        {
            try
            {
                var tick = dbc.TICKETs.Single(t => t.ID_TICK == id);
                var prio = dbc.PRIORITies.Single(p => p.ID_PRIO == tick.ID_PRIO);

                if (tick.ID_STAT_END != 4 && tick.ID_STAT_END != 6 && tick.ID_STAT_END != 2 && tick.ID_STAT_END != 5)
                {
                    var time = DateTime.Now.Subtract(Convert.ToDateTime(tick.CREATE_TICK)).Days * 24 +
                                                DateTime.Now.Subtract(Convert.ToDateTime(tick.CREATE_TICK)).Hours;
                    return Convert.ToString(Convert.ToInt32(prio.HOU_PRIO) - Convert.ToInt32(time)) + " Hours";
                }
                else //if(tick.ID_STAT_END == 6)
                {
                    return "Stopped";
                }
            }

            catch
            {
                return "0 Hours";
            }
        }
    }
}
