using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.IO;
using System.Data;
using System.Data.Entity;

namespace ERPElectrodata.Controllers
{
    public class DetailsChangeController : Controller
    {
        private CoreEntities dbc = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();

        //
        // GET: /DetailsChange/
        public ActionResult Index(int id = 0)
        {
            var DETA_TICK = new DETAILS_TICKET();
            DETA_TICK.ID_TICK = id;

            var ticket = (from t in dbc.TICKETs.Where(t => t.ID_TICK == id).ToList()
                          join tt in dbc.TYPE_TICKET on t.ID_TYPE_TICK equals tt.ID_TYPE_TICK
                          select new
                          { t.COD_TICK,
                            t.ID_STAT_END,
                            NAM_TYPE_TICK = tt.NAM_TYPE_TICK.ToLower(),
                          }).First();

            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            string ACR_ACCO = dbc.ACCOUNTs.Single(x => x.ID_ACCO == ID_ACCO).ACR_ACCO;

            ViewBag.NAM_TYPE_TICK = ticket.NAM_TYPE_TICK;
            ViewBag.COD_TICK = ticket.COD_TICK;
            ViewBag.ID_TICK = id;
            ViewBag.ID_STAT_END = ticket.ID_STAT_END;
            ViewBag.ACR_ACCO = ACR_ACCO + ".pdf";

            return View(DETA_TICK);
        }

        public ActionResult ListByIdTick(int id)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            var listuser = (from lu in dbe.ACCOUNT_ENTITY
                            join pe in dbe.PERSON_ENTITY on lu.ID_PERS_ENTI equals pe.ID_PERS_ENTI
                            join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                            where lu.ID_ACCO == ID_ACCO //&& ce.UserId != null
                            select new
                            {
                                lu.ID_PERS_ENTI,
                                Client = ce.FIR_NAME + " " + ce.LAS_NAME,
                                ce.UserId,
                            }).ToList();

            var query = dbc.DETAILS_TICKET.Where(dt => dt.ID_TICK == id).ToList();

            var result = (from di in query.OrderByDescending(di => di.ID_DETA_TICK)
                          join tdt in dbc.TYPE_DETAILS_TICKET on di.ID_TYPE_DETA_TICK equals tdt.ID_TYPE_DETA_TICK
                          join p in listuser on di.UserId equals p.UserId

                          join s in dbc.STATUS on di.ID_STAT equals s.ID_STAT into ls
                          from x in ls.DefaultIfEmpty()

                          join q in dbc.QUEUEs on di.ID_QUEU equals q.ID_QUEU into lq
                          from y in lq.DefaultIfEmpty()

                          join a in listuser on di.ID_PERS_ENTI equals a.ID_PERS_ENTI into la
                          from z in la.DefaultIfEmpty()

                          select new
                          {
                              PERS = p.Client.ToLower(),
                              COM_DETA_INCI = di.COM_DETA_TICK,
                              ID_TYPE_DETA_TICK = di.ID_TYPE_DETA_TICK,
                              NAM_TYPE_DETA_TICK = tdt.NAM_TYPE_DETA_TICK.ToLower(),
                              NAM_STAT = (x == null ? String.Empty : x.NAM_STAT),
                              NAM_QUEU = (y == null ? String.Empty : y.NAM_QUEU),
                              ASSI = (z == null ? String.Empty : z.Client),
                              CREATE_DETA_INCI = String.Format("{0:G}", di.CREATE_DETA_INCI),
                              ADJ = AttaDetaTick(di.ID_DETA_TICK),
                              FEC_SCHE = String.Format("{0:G}", di.FEC_SCHE),
                              ID_PHOT = "1",//p.ID_CLIE,
                          }).ToList();

            var ticket = (from t in dbc.TICKETs.Where(t => t.ID_TICK == id).ToList()
                          join p in dbc.PRIORITies on t.ID_PRIO equals p.ID_PRIO
                          join s in dbc.SOURCEs on t.ID_SOUR equals s.ID_SOUR
                          join st in dbc.STATUS on t.ID_STAT_END equals st.ID_STAT
                          join aeu in listuser on t.ID_PERS_ENTI_END equals aeu.ID_PERS_ENTI
                          join tt in dbc.TYPE_TICKET on t.ID_TYPE_TICK equals tt.ID_TYPE_TICK
                          join c4 in dbc.CATEGORies on t.ID_CATE equals c4.ID_CATE
                          join c3 in dbc.CATEGORies on c4.ID_CATE_PARE equals c3.ID_CATE
                          join c2 in dbc.CATEGORies on c3.ID_CATE_PARE equals c2.ID_CATE
                          join c1 in dbc.CATEGORies on c2.ID_CATE_PARE equals c1.ID_CATE
                          join cb in listuser on t.UserId equals cb.UserId
                          join at in listuser on t.ID_PERS_ENTI_ASSI equals at.ID_PERS_ENTI
                          select new
                          {
                              t.ID_TICK,
                              AEU = aeu.Client.ToLower(),
                              NAM_STAT = st.NAM_STAT.ToLower(),
                              NAM_PRIO = p.NAM_PRIO.ToLower(),
                              REM_CONT = (t.REM_CTRL_TICK == true ? "Yes" : "No"),
                              NAM_SOUR = s.NAM_SOUR.ToLower(),
                              t.COD_TICK,
                              t.SUM_TICK,
                              CREATE_TICK = String.Format("{0:G}", t.FEC_TICK),
                              MODIFIED_TICK = String.Format("{0:G}", t.MODIFIED_TICK),
                              DATE_SCHE = String.Format("{0:G}", t.FEC_INI_TICK),
                              ATTA_TOT = TotAtta(Convert.ToInt32(t.ID_TICK)),
                              NAM_TYPE_TICK = tt.NAM_TYPE_TICK.ToLower(),
                              ATTA = AttaTick(Convert.ToInt32(t.ID_TICK)),
                              APPROVED = TotApproved(Convert.ToInt32(t.ID_TICK)),
                              CREA_BY = cb.Client.ToLower(),
                              ASSI_TO = at.Client.ToLower(),
                              SUBCLASS = c4.NAM_CATE.ToLower(),
                              CLASS = c3.NAM_CATE.ToLower(),
                              SUBCATE = c2.NAM_CATE.ToLower(),
                              CATE = c1.NAM_CATE.ToLower(),
                              TTDetails456 = CountDetails456(Convert.ToInt32(t.ID_TICK))
                          }).ToList();


            return Json(new { data = result, datadeta = ticket }, JsonRequestBehavior.AllowGet);
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

        public string AttaTick(int id)
        {
            string Adjun = "";
            try
            {
                var query = dbc.ATTACHEDs.Where(a => a.ID_INCI == id);
                foreach (ATTACHED subqu in query)
                {
                    Adjun = Adjun + "<li><a href='../../Adjuntos/Change/" + subqu.NAM_ATTA + "_" + subqu.ID_ATTA + subqu.EXT_ATTA + " ' TARGET='_BLANK'>" + subqu.NAM_ATTA + subqu.EXT_ATTA + "</a></li>";
                }

                return Adjun;
            }
            catch
            {
                return String.Empty;
            }
        }

        public string AttaDetaTick(int id)
        {
            string Adjun = "";
            try
            {
                var query = dbc.ATTACHEDs.Where(a => a.ID_DETA_INCI == id);
                foreach (ATTACHED subqu in query)
                {
                    Adjun = Adjun + "<li><a href='../../Adjuntos/Change/" + subqu.NAM_ATTA + "_" + subqu.ID_ATTA + subqu.EXT_ATTA + " ' TARGET='_BLANK'>" + subqu.NAM_ATTA + subqu.EXT_ATTA + "</a></li>";
                }

                return Adjun;
            }
            catch
            {
                return String.Empty;
            }
        }

        public string TotApproved(int id)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            var query = dbc.ACCOUNT_ALLOW.Where(x => x.ID_ACCO == ID_ACCO);
            int TTCount = query.Count();
            int TTAppr = 0;

            foreach (var a in query)
            {
                int UserId = dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == a.ID_PERS_ENTI)
                    .Join(dbe.CLASS_ENTITY, x => x.ID_ENTI2, ce => ce.ID_ENTI, (x, ce) => new { 
                        ce.UserId
                    }).First().UserId.Value;

                var query1 = dbc.DETAILS_TICKET.Where(x => x.ID_TICK == id && x.UserId ==UserId &&
                    x.ID_TYPE_DETA_TICK == 9 || x.ID_TYPE_DETA_TICK == 10);

                if(query1.Count() > 0){
                    int ID_TYPE_DETA_TICK = query1.OrderByDescending(x => x.CREATE_DETA_INCI).First().ID_TYPE_DETA_TICK.Value;
                    if (ID_TYPE_DETA_TICK == 9) { TTAppr = TTAppr + 1; }                
                }

            }

            return TTAppr.ToString() + " de " + TTCount.ToString();
        }

        public int CountDetails456(int id)
        {

            var result = (from q in dbc.DETAILS_TICKET.Where(x => x.ID_TICK == id && (x.ID_TYPE_DETA_TICK == 4 || x.ID_TYPE_DETA_TICK == 5 || x.ID_TYPE_DETA_TICK == 6 || x.ID_TYPE_DETA_TICK == 8))
                          select new { 
                            q.ID_TYPE_DETA_TICK
                          }).GroupBy(x=>x.ID_TYPE_DETA_TICK);

            return result.Count();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Create(DETAILS_TICKET details_ticket, IEnumerable<HttpPostedFileBase> files)
        {
            if (String.IsNullOrEmpty(details_ticket.COM_DETA_TICK))
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('ERROR','You must introduce your comment');}window.onload=init;</script>");
            }

            if (files == null && details_ticket.ID_TYPE_DETA_TICK != 1 && details_ticket.ID_TYPE_DETA_TICK != 2 && details_ticket.ID_TYPE_DETA_TICK != 11)
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('ERROR','" + ResourceLanguaje.Resource.AttachMsn + "');}window.onload=init;</script>");            
            }

            if (details_ticket.ID_TYPE_DETA_TICK == 11) //Acccion Executed Ticket
            { //Verificando si la solicitud sigue aprobada
                int ID_STAT = dbc.TICKETs.Single(x => x.ID_TICK == details_ticket.ID_TICK).ID_STAT_END.Value;
                if (ID_STAT != 8) {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR1','" + ResourceLanguaje.Resource.Message2 + "');}window.onload=init;</script>");
                }
            }

            details_ticket.UserId = Convert.ToInt32(Session["UserId"].ToString());
            details_ticket.CREATE_DETA_INCI = DateTime.Now;

            dbc.DETAILS_TICKET.Add(details_ticket);
            dbc.SaveChanges();

            if (files != null)
            {
                foreach (var file in files)
                {
                    try
                    {
                        var ATTA = new ATTACHED();
                        ATTA.NAM_ATTA = Path.GetFileNameWithoutExtension(file.FileName);
                        ATTA.EXT_ATTA = Path.GetExtension(file.FileName);
                        ATTA.ID_DETA_INCI = details_ticket.ID_DETA_TICK;
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
            
            //Actualizando la cola del Ticket
            if (details_ticket.ID_TYPE_DETA_TICK == 2)
                {
                    var ticket = new TICKET();
                    ticket = dbc.TICKETs.Single(x => x.ID_TICK == details_ticket.ID_TICK);
                    ticket.ID_QUEU = details_ticket.ID_QUEU;
                    ticket.MODIFIED_TICK = DateTime.Now;

                    dbc.TICKETs.Attach(ticket);
                    dbc.Entry(ticket).State = EntityState.Modified;
                    dbc.SaveChanges();
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('OK','');}window.onload=init;</script>");
                }
            else if (details_ticket.ID_TYPE_DETA_TICK == 11) //Actualizando estado del ticket a EXECUTED
                {
                    var ticket = new TICKET();
                    ticket = dbc.TICKETs.Single(x => x.ID_TICK == details_ticket.ID_TICK);
                    ticket.ID_STAT_END = 10;
                    ticket.MODIFIED_TICK = DateTime.Now;

                    dbc.TICKETs.Attach(ticket);
                    dbc.Entry(ticket).State = EntityState.Modified;
                    dbc.SaveChanges();
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('OK','');}window.onload=init;</script>");            
                }

            return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('OK','');}window.onload=init;</script>");
        }

        [ValidateInput(false)]
        public string RequestApproval()
        {
            string COM_DETA_TICK = Convert.ToString(Request.Params["COM_DETA_TICK"]);
            COM_DETA_TICK = COM_DETA_TICK.Replace("&nbsp;", "");
            COM_DETA_TICK = COM_DETA_TICK.Replace("<br />", "");
            if (String.IsNullOrEmpty(COM_DETA_TICK))
            {
                return "ERROR 1";
            }

            try
            {
                var deta = new DETAILS_TICKET();
                int ID_TICK = Convert.ToInt32(Request.Params["ID_TICK"]);

                deta.ID_TICK = ID_TICK;
                deta.UserId = Convert.ToInt32(Session["UserId"].ToString());
                deta.CREATE_DETA_INCI = DateTime.Now;
                deta.ID_TYPE_DETA_TICK = 8;
                deta.COM_DETA_TICK = COM_DETA_TICK;

                dbc.DETAILS_TICKET.Add(deta);
                dbc.SaveChanges();
            }
            catch
            {
                return "ERROR 2";
            }           
            return "OK";
        }
    }
}
