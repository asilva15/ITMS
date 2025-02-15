using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.Globalization;
using System.Threading;
using System.IO;
using System.Data.Entity;

namespace ERPElectrodata.Controllers
{
    public class ClientITOController : Controller
    {
        public CoreEntities db = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();

        private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        private TextInfo textInfo;

        public int ctd = 1;
        //
        // GET: /ClientITO/

        public ActionResult Index()
        {
            return View();
        }

        //
        //  GET: /PERSONAL DE CIA
        public ActionResult LystForName()
        {
            int ID_COMP = Convert.ToInt32(Session["ID_COMP"]);

            var query = (from ce in dbe.CLASS_ENTITY
                         join pe in dbe.PERSON_ENTITY on ce.ID_ENTI equals pe.ID_ENTI2
                         where pe.ID_ENTI1 == ID_COMP
                         where pe.VIG_PERS_ENTI == true
                         select new
                         {
                             ID_ENTI2 = pe.ID_ENTI2,
                             ID_PERS_ENTI = pe.ID_PERS_ENTI,
                             NOMBRE = ce.FIR_NAME,
                         }).ToList();

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }
        //
        // GET: /ClientITO/UpdateBarStatus
        public ActionResult UpdateBarStatus()
        {
            var stados = db.STATUS;

            var result = (from s in stados.ToList()
                          where s.ID_STAT != 2
                          select new
                          {
                              s.ID_STAT,
                              s.NAM_STAT,
                              Tickets = TicketsByStatus(s.ID_STAT),
                              Total = TicketsByStatus(s.ID_STAT),

                          }).ToList();

            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);

        }

        public int TicketsByStatus(int id)
        {
            int[] numbers = new int[0];
            int iq = 0;
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
            int ID_ENTI = Convert.ToInt32(Session["ID_COMP"]);

            //bool VIS_ALL_QUEU = Convert.ToBoolean(Session["VIS_ALL_QUEU"].ToString());
            //int ID_ASSI = Convert.ToInt32(Session["ID_PERS_ENTI"].ToString());

            var cant = db.TICKETs.Where(i => i.ID_ACCO == ID_ACCO && i.ID_STAT_END == id && i.ID_TICK_PARENT == null
                && i.ID_COMP == ID_ENTI && i.ID_DOCU_SALE == null);

            //if (VIS_ALL_QUEU == false)
            //{
                //int supQueu = 0;
                //try
                //{
                //    var Queus = db.ACCOUNT_QUEUE.Where(x => x.ID_ACCO == ID_ACCO)
                //        .Where(x => x.ID_PERS_ENTI_CORD == ID_ASSI);

                //    supQueu = Queus.Count();
                //    numbers = new int[supQueu];
                //    foreach (var x in Queus)
                //    {
                //        //var orderKeys = new int[] { 1, 12, 306, 284, 50047 };
                //        numbers[iq] = (int)x.ID_QUEU.Value;
                //        iq++;
                //        //Customers.Rows.Add(row);
                //    }
                //}
                //catch
                //{

                //}

                //if (supQueu == 0)
                //{
                //    cant = cant.Where(i => i.ID_PERS_ENTI_ASSI == ID_ASSI);
                //    //tick = tick.Where(i => i.ID_PERS_ENTI_ASSI == ID_PERS_ENTI);
                //}
                //else
                //{
                //    //Mostrar el ticket cuya cola es la indicada
                //    cant = cant.Where(i => numbers.Contains((int)i.ID_QUEU.Value));
                //}

            //}

            return cant.Count();
        }

        public ActionResult ListByStatus(int id = 0)
        {
            textInfo = cultureInfo.TextInfo;
            ctd = 1;
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int ID_COMP = Convert.ToInt32(Session["ID_COMP"]);

            //var listuser = (//from lu in dbe.ACCOUNT_ENTITY
            //                from pe in dbe.PERSON_ENTITY// on lu.ID_PERS_ENTI equals pe.ID_PERS_ENTI
            //                join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
            //                where pe.ID_ENTI1 == ID_COMP
            //                select new
            //                {
            //                    pe.ID_PERS_ENTI,
            //                    Client = ce.FIR_NAME + " " + ce.LAS_NAME,
            //                    ce.UserId,
            //                }).ToList();

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

            var ss = listuser.ToList();

            var listCIA = (from pe in dbe.PERSON_ENTITY
                           join ce in dbe.CLASS_ENTITY.Where(x => x.COM_NAME != null) on pe.ID_ENTI1 equals ce.ID_ENTI
                           select new
                           {
                               pe.ID_PERS_ENTI,
                               COMPANY = ce.COM_NAME,
                           }).ToList();


            //int UserId = Convert.ToInt32(Session["UserId"]);
            int ID_STAT = Convert.ToInt32(Request.Params["filter[filters][0][value]"].ToString());
            //int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());
            var tick = db.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.ID_COMP == ID_COMP && t.ID_DOCU_SALE == null);
            int Count = 0, iq = 0;
            int[] numbers = new int[0];


            if (ID_STAT == 0)
            {
                tick = tick.Where(t => t.ID_STAT_END == 1 || t.ID_STAT_END == 3);
                Count = tick.Count();
                tick = tick.OrderByDescending(t => t.IS_ROLE_USER).ThenBy(t => t.ID_PRIO).ThenByDescending(t => t.CREATE_TICK).Skip(skip).Take(take);
            }
            else if (ID_STAT == 5)
            {
                tick = tick.Where(i => i.ID_STAT_END == 5);
                Count = tick.Count();
                tick = tick.OrderByDescending(t => t.ID_TICK).Skip(skip).Take(take);
            }
            else if (ID_STAT == 4)
            {
                tick = tick.Where(i => i.ID_STAT_END == 4);
                Count = tick.Count();
                tick = tick.OrderByDescending(t => t.ID_TICK).Skip(skip).Take(take);
            }
            else if (ID_STAT == 6)
            {
                tick = tick.Where(i => i.ID_STAT_END == 6);
                Count = tick.Count();
                tick = tick.OrderByDescending(t => t.ID_TICK).Skip(skip).Take(take);
            }

            var result = (from i in tick.ToList()
                          join so in db.SOURCEs on i.ID_SOUR equals so.ID_SOUR
                          join s in db.STATUS on i.ID_STAT_END equals s.ID_STAT
                          join tt in db.TYPE_TICKET on i.ID_TYPE_TICK equals tt.ID_TYPE_TICK
                          join c4 in db.CATEGORies on i.ID_CATE equals c4.ID_CATE
                          join c3 in db.CATEGORies on c4.ID_CATE_PARE equals c3.ID_CATE
                          join c2 in db.CATEGORies on c3.ID_CATE_PARE equals c2.ID_CATE
                          join c1 in db.CATEGORies on c2.ID_CATE_PARE equals c1.ID_CATE
                          join pr in db.PRIORITies on i.ID_PRIO equals pr.ID_PRIO
                          join cr in listuser on i.UserId equals cr.UserId
                          join ac in db.ACCOUNTs on i.ID_ACCO equals ac.ID_ACCO
                          //join asi in listuser on i.ID_PERS_ENTI_ASSI equals asi.ID_PERS_ENTI
                          join cia in listCIA on i.ID_PERS_ENTI equals cia.ID_PERS_ENTI
                          //join ds in db.DOCUMENT_SALE on (i.ID_DOCU_SALE == null ? 0 : i.ID_DOCU_SALE) equals ds.ID_DOCU_SALE into lds
                          //from xds in lds.DefaultIfEmpty()
                          //join tds in db.TYPE_DOCUMENT_SALE on (xds == null ? 0 : xds.ID_TYPE_DOCU_SALE) equals tds.ID_TYPE_DOCU_SALE into ltds
                          //from xtds in ltds.DefaultIfEmpty()
                          select new
                          {
                              ID_INCI = i.ID_TICK,
                              COD_INCI = i.COD_TICK,
                              SUM_INCI = i.SUM_TICK,
                              NAM_STAT = s.NAM_STAT.ToLower(),
                              CLA_STAT = s.CLA_STAT,
                              NAM_SOUR = so.NAM_SOUR.ToLower(),
                              NAM_SUBCLAS = c4.NAM_CATE.ToLower(),
                              NAM_CLAS = c3.NAM_CATE.ToLower(),
                              NAM_SUBC = c2.NAM_CATE.ToLower(),
                              NAM_CATE = c1.ABR_CATE,
                              NAM_TYPE_TICK = tt.NAM_TYPE_TICK.ToLower(),
                              URL = (i.ID_TYPE_FORM == null ? "/DetailsTicket/Index/" : "/DeliveryReception/Details/"),
                              REQU = ReturnRequ(Convert.ToInt32(i.ID_PERS_ENTI_END)),
                              CREATE_INCI = String.Format("{0:d}", i.CREATE_TICK) + " " + String.Format("{0:HH:mm}", i.CREATE_TICK),
                              MODIFIED_INCI = String.Format("{0:d}", i.MODIFIED_TICK) + " " + String.Format("{0:HH:mm}", i.MODIFIED_TICK),
                              ID_PRIO = pr.ID_PRIO,
                              NAM_PRIO = pr.NAM_PRIO.ToLower(),
                              HOU_PRIO = pr.HOU_PRIO != 0 ? Convert.ToString(pr.HOU_PRIO) + "h" : "",
                              ICO_PRIO = pr.ICO_PRIO,
                              CREATEBY = cr.Client.ToLower(),
                              ACCO = ac.NAM_ACCO,
                              ASSI = "",//asi.Client.ToLower(),
                              EXP_TIME = ExpTime(i.ID_TICK),
                              PARENT = (i.IS_PARENT == true ? "expand" : "none"),
                              COUNTSON = CountTicketSon(i.ID_TICK),
                              CIA = "",//textInfo.ToTitleCase(cia.COMPANY.ToLower()).Substring(0, (cia.COMPANY.Length > 48 ? 48 : cia.COMPANY.Length)) +
                                    //(cia.COMPANY.Length > 48 ? "..." : ""),
                              CIA_TOOL = "",//textInfo.ToTitleCase(cia.COMPANY.ToLower()),
                              ID_ACCO,
                              ac.VIS_COMP,
                              i.ID_STAT_END,
                              NUM_OP = "",//(xds == null ? "" : xds.NUM_DOCU_SALE),
                              COD_TYPE_DOCU_SALE = "",//(xtds == null ? "" : xtds.COD_TYPE_DOCU_SALE),
                              ID_DOCU_SALE = 0,//(xds == null ? 0 : xds.ID_DOCU_SALE),
                              //DATE_INI = String.Format("{0:MMM d yyyy HH:mm:ss}", Convert.ToDateTime(i.CREATE_TICK).AddHours(Convert.ToInt32(pr.HOU_PRIO))),
                              Seque = contador(),
                              DATE_MAX = DateMaxima(Convert.ToInt32(i.ID_TICK), Convert.ToDateTime(i.FEC_INI_TICK), Convert.ToInt32(pr.HOU_PRIO)),
                              DATE_SCHE = i.ID_STAT_END == 5 ? ScheduleDate(Convert.ToInt32(i.ID_TICK), i.FEC_INI_TICK.Value) : "",
                              HOUR_SCHE = i.ID_STAT_END == 5 ? ScheduleTime(Convert.ToInt32(i.ID_TICK), i.FEC_INI_TICK.Value) : ""
                          });

            return Json(new { Data = result, Count = Count }, JsonRequestBehavior.AllowGet);
        }
        public string ReturnRequ(int idend = 0)
        {

            var listClient = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == idend)
                              join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                              select new
                              {
                                  Client = ce.FIR_NAME + " " + (ce.LAS_NAME == null ? "" : ce.LAS_NAME),
                              });

            return listClient.First().Client.ToLower();
        }
        public string ExpTime(int id)
        {
            try
            {

                var tick = db.TICKETs.Single(t => t.ID_TICK == id);
                var prio = db.PRIORITies.Single(p => p.ID_PRIO == tick.ID_PRIO);

                if (tick.ID_STAT_END != 4 && tick.ID_STAT_END != 6 && tick.ID_STAT_END != 2 && tick.ID_STAT_END != 5)
                {
                    var time = DateTime.Now.Subtract(Convert.ToDateTime(tick.CREATE_TICK)).Days * 24 +
                                                DateTime.Now.Subtract(Convert.ToDateTime(tick.CREATE_TICK)).Hours;

                    return Convert.ToString(Convert.ToInt32(prio.HOU_PRIO) - Convert.ToInt32(time));
                }
                else //if(tick.ID_STAT_END == 6)
                {
                    return "Stopped";
                }


            }

            catch
            {
                return "0";
            }
        }
        public int CountTicketSon(int id = 0)
        {
            return db.TICKETs.Where(x => x.ID_TICK_PARENT == id).Count();
        }
        public int contador()
        {
            return ctd++;
        }
        public string DateMaxima(int IdTick, DateTime DatIni, int HH)
        {
            string FecMax = "";
            Double MinTransc = 0;
            var qDet = db.DETAILS_TICKET.Where(x => x.ID_TICK == IdTick && x.ID_STAT == 5);
            if (qDet.Count() > 0)
            {
                MinTransc = qDet.AsEnumerable().Sum(x => x.MINUTES).Value;
                qDet = qDet.OrderByDescending(x => x.ID_DETA_TICK);

                DateTime fecSche = qDet.First().FEC_SCHE.Value;
                Double hh = Convert.ToDouble(HH) - (MinTransc / 60);
                FecMax = String.Format("{0:MMM d yyyy HH:mm:ss}", fecSche.AddHours(hh));
            }
            else
            {
                FecMax = String.Format("{0:MMM d yyyy HH:mm:ss}", DatIni.AddHours(HH));
            }
            return FecMax;
        }
        public string ScheduleDate(int ID_TICK, DateTime FEC_INI_TICK)
        {
            try
            {
                var query = db.DETAILS_TICKET.Where(x => x.ID_TICK == ID_TICK)
                    .Where(x => x.ID_STAT == 5);

                if (query.Count() != 0)
                {
                    var xyz = query.OrderByDescending(x => x.ID_DETA_TICK).First().FEC_SCHE;
                    return String.Format("{0:d}", Convert.ToDateTime(xyz.Value));
                }
                else
                {
                    return String.Format("{0:d}", Convert.ToDateTime(FEC_INI_TICK));

                }
                    //.OrderByDescending(x => x.ID_DETA_TICK)
                      //          .Where(x => x.ID_STAT == 5).First().FEC_SCHE;

            }
            catch
            {
                return "Stopped";
            }
            
        }
        public string ScheduleTime(int ID_TICK, DateTime FEC_INI_TICK)
        {
            try
            {
                var query = db.DETAILS_TICKET.Where(x => x.ID_TICK == ID_TICK)
                    .Where(x => x.ID_STAT == 5);

                if (query.Count() != 0)
                {
                    var xyz = query.OrderByDescending(x => x.ID_DETA_TICK).First().FEC_SCHE;
                    return String.Format("{0:HH:mm}", Convert.ToDateTime(xyz.Value));
                }
                else
                {
                    return String.Format("{0:HH:mm}", Convert.ToDateTime(FEC_INI_TICK));

                }
            }
            catch
            {
                return "Stopped";
            }

        }

        public ActionResult Create()
        {

            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Create(IEnumerable<HttpPostedFileBase> files, TICKET ticket)
        {
            //int id1;
            //Int32.TryParse(ticket.ID_PERS_ENTI, out id1);
            try{
                
                int ID_ACCO = 0;
                try { ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]); }
                catch { ID_ACCO = 0; }

                if (ID_ACCO == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
                }
                else if(ticket.ID_PERS_ENTI>0){

            }
            else{
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
            }

            if(String.IsNullOrEmpty(ticket.SUM_TICK)){
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
            }

            int ID_COMP = Convert.ToInt32(Session["ID_COMP"]);
            int UserId = Convert.ToInt32(Session["UserId"]);

            ticket.ID_ACCO = ID_ACCO;
            ticket.ID_TYPE_TICK = 1;
            ticket.ID_QUEU = 5;
            ticket.ID_PRIO = 4;
            ticket.ID_STAT = 3;
            ticket.ID_STAT_END = 3;
            ticket.ID_SOUR = 4;
            ticket.FEC_TICK = DateTime.Now;
            ticket.CREATE_TICK = DateTime.Now;
            ticket.ID_CATE = 2440;
            ticket.MODIFIED_TICK = DateTime.Now;
            ticket.ID_PERS_ENTI_END = ticket.ID_PERS_ENTI;
            ticket.IS_PARENT = false;
            ticket.ID_PERS_ENTI_ASSI = 1007;
            ticket.ID_COMP = ID_COMP;
            ticket.UserId = UserId;
            ticket.FEC_INI_TICK = DateTime.Now;
            ticket.DAT_EXPI_TICK = DateTime.Now.AddHours(48);

            db.TICKETs.Add(ticket);
            db.SaveChanges();

            if (files != null)
            {
                foreach (var file in files)
                {
                    try
                    {
                        var ATTA = new ATTACHED();
                        ATTA.NAM_ATTA = Path.GetFileNameWithoutExtension(file.FileName);
                        ATTA.EXT_ATTA = Path.GetExtension(file.FileName);
                        ATTA.ID_INCI = ticket.ID_TICK;
                        ATTA.CREATE_ATTA = DateTime.Now;

                        db.ATTACHEDs.Add(ATTA);
                        db.SaveChanges();

                        file.SaveAs(Server.MapPath("~/Adjuntos/") + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA) + ATTA.EXT_ATTA);
                    }
                    catch
                    {

                    }
                }
            }

            int id = Convert.ToInt32(ticket.ID_TICK);
            db.Entry(ticket).State = EntityState.Detached;

            string code = Convert.ToString(db.TICKETs.Single(t => t.ID_TICK == id).COD_TICK);

            return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','" + code + "');}window.onload=init;</script>");
            }
            catch{
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','0');}window.onload=init;</script>");
            }
            //return View();
        }
    }
}
