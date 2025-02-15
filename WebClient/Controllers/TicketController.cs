using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebClient.Models;
using System.Globalization;
using System.Threading;
using WebClient.Plugins.Ticket;
using WebClient.Plugins.Encription;

namespace WebClient.Controllers
{
    public class TicketController : Controller
    {
        CoreEntities dbc = new CoreEntities();
        EntityEntities dbe = new EntityEntities();
        public TicketIR tir = new TicketIR();

        private AESRijndael seg = new AESRijndael();
        private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        private TextInfo textInfo;
        
        // GET: Ticket
        public ActionResult Index(int id = 0)
        {
            string idt = Convert.ToString(Request.Params["idt"].ToString());
            ViewData["id"] = idt;

            return View();
        }

        public ActionResult Unique(int id = 0)
        {
            try
            {
                string idt = Convert.ToString(Request.Params["idt"].ToString());
                //id = Convert.ToInt32(seg.Decrypt(idt));
                id = Convert.ToInt32(idt);

                textInfo = cultureInfo.TextInfo;

                var query = dbc.TICKETs.Where(x => x.ID_TICK == id);
                var ticket = query.First();

                var listuser = (from lu in dbe.ACCOUNT_ENTITY
                                join pe in dbe.PERSON_ENTITY on lu.ID_PERS_ENTI equals pe.ID_PERS_ENTI
                                join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                where lu.ID_ACCO == ticket.ID_ACCO//&& ce.UserId != null
                                select new
                                {
                                    lu.ID_PERS_ENTI,
                                    Client = ce.FIR_NAME + " " + ce.LAS_NAME,
                                    ce.UserId,
                                    pe.ID_FOTO,
                                }).ToList();

                var listCIA = (from pe in dbe.PERSON_ENTITY
                               join ce in dbe.CLASS_ENTITY.Where(x => x.COM_NAME != null) on pe.ID_ENTI1 equals ce.ID_ENTI
                               select new
                               {
                                   pe.ID_PERS_ENTI,
                                   CIA = ce.COM_NAME,
                               }).ToList();

                var result = (from t in query.ToList()
                              join tt in dbc.TYPE_TICKET on t.ID_TYPE_TICK equals tt.ID_TYPE_TICK
                              join ts in dbc.STATUS on t.ID_STAT_END equals ts.ID_STAT
                              join tp in dbc.PRIORITies on t.ID_PRIO equals tp.ID_PRIO
                              join scl in dbc.CATEGORies on t.ID_CATE equals scl.ID_CATE
                              join cl in dbc.CATEGORies on scl.ID_CATE_PARE equals cl.ID_CATE
                              join scat in dbc.CATEGORies on cl.ID_CATE_PARE equals scat.ID_CATE
                              join cat in dbc.CATEGORies on scat.ID_CATE_PARE equals cat.ID_CATE
                              join tso   in dbc.SOURCEs on t.ID_SOUR equals tso.ID_SOUR
                              join cb in listuser on t.UserId equals cb.UserId
                              join at in listuser on t.ID_PERS_ENTI_ASSI equals at.ID_PERS_ENTI
                              join cia in listCIA on t.ID_PERS_ENTI equals cia.ID_PERS_ENTI
                              select new {
                                  AEU = textInfo.ToTitleCase(ReturnRequ(t.ID_PERS_ENTI_END.Value)),
                                  NAM_TYPE_TICK = textInfo.ToTitleCase(tt.NAM_TYPE_TICK.ToLower()),
                                  COD_TICK = t.COD_TICK,
                                  NAM_STAT = textInfo.ToTitleCase(ts.NAM_STAT.ToLower()),
                                  NAM_PRIO = textInfo.ToTitleCase(tp.NAM_PRIO.ToLower()),
                                  SLA = (tp.HOU_PRIO == 0 ? "Planning" : Convert.ToString(tp.HOU_PRIO)+" Hours"),
                                  EXP_TIME = tir.ExpirationTime(t.ID_TICK,tp.HOU_PRIO.Value),
                                  NAM_CATE = textInfo.ToTitleCase(cat.NAM_CATE.ToLower()),
                                  NAM_SUB_CATE = textInfo.ToTitleCase(scat.NAM_CATE.ToLower()),
                                  NAM_CLAS =textInfo.ToTitleCase(cl.NAM_CATE.ToLower()),
                                  NAM_SUB_CLAS = textInfo.ToTitleCase(scl.NAM_CATE.ToLower()),
                                  NAM_SOUR = textInfo.ToTitleCase(tso.NAM_SOUR.ToLower()),
                                  CREATED = String.Format("{0:d}", t.CREATE_TICK) + " " + String.Format("{0:HH:mm}", t.CREATE_TICK),
                                  MODIFIED = String.Format("{0:d}", t.MODIFIED_TICK) + " " + String.Format("{0:HH:mm}", t.MODIFIED_TICK),
                                  ATTA = TotAtta(Convert.ToInt32(t.ID_TICK)),
                                  RDP = (t.REM_CTRL_TICK == true ? "Yes" : "No"),
                                  SUM_TICK = t.SUM_TICK,
                                  PHOTO = cb.ID_FOTO,
                                  CREA_BY = textInfo.ToTitleCase(cb.Client.ToLower()),
                                  ASSI_TO = textInfo.ToTitleCase(at.Client.ToLower()),
                                  CIA = textInfo.ToTitleCase(cia.CIA.ToLower()),
                              });

               

                var querydt = dbc.DETAILS_TICKET.Where(dt => dt.ID_TICK == id).ToList();

                var resultdt = (from di in querydt.OrderByDescending(di => di.ID_DETA_TICK)
                              join tdt in dbc.TYPE_DETAILS_TICKET on di.ID_TYPE_DETA_TICK equals tdt.ID_TYPE_DETA_TICK
                              join p in listuser on di.UserId equals p.UserId

                              join s in dbc.STATUS on di.ID_STAT equals s.ID_STAT into ls
                              from x in ls.DefaultIfEmpty()

                              join q in dbc.QUEUEs on di.ID_QUEU equals q.ID_QUEU into lq
                              from y in lq.DefaultIfEmpty()

                              join a in listuser on di.ID_PERS_ENTI equals a.ID_PERS_ENTI into la
                              from z in la.DefaultIfEmpty()

                              //join sso in dbc.STATUS_SALES_OPPORTUNITY on di.ID_STAT_SALE_OPP equals sso.ID_STAT_SALE_OPPO into lsso
                              //from w in lsso.DefaultIfEmpty()

                              //join t in db.TICKETs on di.ID_TICK_PARENT equals t.ID_TICK into lt
                              //from k in lt.DefaultIfEmpty()

                              select new
                              {
                                  PERS = textInfo.ToTitleCase(p.Client.ToLower()),
                                  COM_DETA_INCI = di.COM_DETA_TICK,
                                  ID_TYPE_DETA_TICK = di.ID_TYPE_DETA_TICK,
                                  NAM_TYPE_DETA_TICK = textInfo.ToTitleCase(tdt.NAM_TYPE_DETA_TICK.ToLower()),
                                  NAM_STAT = (x == null ? String.Empty : " | "+textInfo.ToTitleCase(x.NAM_STAT.ToLower())),
                                  NAM_QUEU = (y == null ? String.Empty : y.NAM_QUEU),
                                  //COD_PARENT = (k == null ? String.Empty : k.COD_TICK),
                                  ASSI = (z == null ? String.Empty : " | " +  textInfo.ToTitleCase(z.Client.ToLower())),
                                  CREATE_DETA_INCI = String.Format("{0:d}", di.CREATE_DETA_INCI),
                                  ADJ = 1,//AttaDetaTick(di.ID_DETA_TICK),
                                  FEC_SCHE = String.Format("{0:d}", di.FEC_SCHE),
                                  ID_PHOT = "1",//p.ID_CLIE,
                                  PHOTO = p.ID_FOTO,
                                  //NAM_STAT_SALE_OPPO = (w == null ? String.Empty : w.NAM_STAT_SALE_OPPO),
                                  //AMM_SALE_OPP = string.Format("{0:0,0.00}", di.AMM_SALE_OPP == null ? 0 : di.AMM_SALE_OPP),
                                  //PRO_SALE_OPP = string.Format("{0:0,0.00}", di.PRO_SALE_OPP == null ? 0 : di.PRO_SALE_OPP),
                                  INEX = di.ID_DETA_TICK

                              }).ToList();

                return Json(new {Data=result,Details=resultdt }, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                return Content("ERROR");
            }
            
        }

        public int TotAtta(int id)
        {
            int atta_tick = dbc.ATTACHEDs.Where(a => a.ID_INCI == id).Count();

            int atta_all = (from x in dbc.DETAILS_TICKET.Where(dt => dt.ID_TICK == id)
                            join adt in dbc.ATTACHEDs on x.ID_DETA_TICK equals adt.ID_DETA_INCI
                            select new
                            {
                                adt.ID_ATTA
                            }).Count();

            return atta_tick + atta_all;
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
    }
}