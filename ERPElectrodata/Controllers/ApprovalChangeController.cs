using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using ERPElectrodata.Models;
using System.Data.Entity;

namespace ERPElectrodata.Controllers
{
    [Authorize]
    //[InitializeSimpleMembership]
    public class ApprovalChangeController : Controller
    {
        public EntityEntities dbe = new EntityEntities();
        public CoreEntities dbc = new CoreEntities();
        //
        // GET: /ApprovalChange/

        public ActionResult Index(int id)
        {
            Session["MAIN"] = "mp3";
            DETAILS_TICKET DETA_TICK = new DETAILS_TICKET();
            DETA_TICK.ID_TICK = id;

            var ticket = (from t in dbc.TICKETs.Where(t => t.ID_TICK == id).ToList()
                          join tt in dbc.TYPE_TICKET on t.ID_TYPE_TICK equals tt.ID_TYPE_TICK
                          select new
                          {   t.ID_ACCO,
                              t.COD_TICK,
                              NAM_TYPE_TICK = tt.NAM_TYPE_TICK.ToLower(),
                          }).First();

            Session["ID_ACCO"] = ticket.ID_ACCO;
            Session["NAM_ACCO"] = dbc.ACCOUNTs.Single(a => a.ID_ACCO == ticket.ID_ACCO).NAM_ACCO;

            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int ID_USER = Convert.ToInt32(Session["UserId"]);

            int cont = dbc.ACCOUNT_ALLOW.Where(x => x.ID_ACCO == ticket.ID_ACCO && 
                                                x.VIG_ALLO == true && 
                                                x.ID_PERS_ENTI == ID_PERS_ENTI).Count();

            //Si cont = 0, el usuario no puede aprobar
            if(cont != 0){
                var query = dbc.DETAILS_TICKET.Where(x => x.ID_TICK == id && x.UserId == ID_USER && 
                                                        (x.ID_TYPE_DETA_TICK == 9 || x.ID_TYPE_DETA_TICK == 10));

                if (query.Count() == 0) { cont = 1; } //Solicitud aun no ni aprobada ni rechaza
                else { cont = query.OrderByDescending(x => x.CREATE_DETA_INCI).First().ID_TYPE_DETA_TICK.Value; }
            }

            ViewBag.Contador = cont;
            ViewBag.NAM_TYPE_TICK = ticket.NAM_TYPE_TICK;
            ViewBag.COD_TICK = ticket.COD_TICK;
            ViewBag.ID_TICK = id;

            return View(DETA_TICK);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Approval(DETAILS_TICKET detaTick) {

            string COM_DETA_TICK = Convert.ToString(Request.Params["COM_DETA_TICK"]);
            COM_DETA_TICK = COM_DETA_TICK.Replace("&nbsp;", "");
            COM_DETA_TICK = COM_DETA_TICK.Replace("<br />", "");
            if (String.IsNullOrEmpty(COM_DETA_TICK))
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('ERROR','" + ResourceLanguaje.Resource.Message1 +"','');}window.onload=init;</script>");
            }

            try
            {
                detaTick.UserId = Convert.ToInt32(Session["UserId"].ToString());
                detaTick.CREATE_DETA_INCI = DateTime.Now;
                detaTick.ID_TYPE_DETA_TICK = 9;

                dbc.DETAILS_TICKET.Add(detaTick);
                dbc.SaveChanges();

                //Verificando si debe de cambiar a estado aprobado
                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

                var query = dbc.ACCOUNT_ALLOW.Where(x => x.ID_ACCO == ID_ACCO);
                int TTCount = query.Count();
                int TTAppr = 0;

                foreach (var a in query)
                {
                    int UserId = dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == a.ID_PERS_ENTI)
                        .Join(dbe.CLASS_ENTITY, x => x.ID_ENTI2, ce => ce.ID_ENTI, (x, ce) => new
                        {   ce.UserId
                        }).First().UserId.Value;

                    var query1 = dbc.DETAILS_TICKET.Where(x => x.ID_TICK == detaTick.ID_TICK && x.UserId == UserId &&
                        x.ID_TYPE_DETA_TICK == 9 || x.ID_TYPE_DETA_TICK == 10);

                    if (query1.Count() > 0)
                    {
                        int ID_TYPE_DETA_TICK = query1.OrderByDescending(x => x.CREATE_DETA_INCI).First().ID_TYPE_DETA_TICK.Value;
                        if (ID_TYPE_DETA_TICK == 9) { TTAppr = TTAppr + 1; }
                    }
                }
                if (TTCount == TTAppr) { //Aprobando Ticket Change
                    var ticket = new TICKET();
                    ticket = dbc.TICKETs.Single(x => x.ID_TICK == detaTick.ID_TICK);
                    ticket.ID_STAT_END = 8;
                    ticket.MODIFIED_TICK = DateTime.Now;

                    dbc.TICKETs.Attach(ticket);
                    dbc.Entry(ticket).State = EntityState.Modified;
                    dbc.SaveChanges();              
                }
            }
            catch
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('ERROR','" + ResourceLanguaje.Resource.BasedDrawback + "','');}window.onload=init;</script>");
            }
            return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('OK','" + ResourceLanguaje.Resource.RequestApproved + "','" + detaTick.ID_TICK.ToString() + "');}window.onload=init;</script>");
        }

        [ValidateInput(false)]
        public string Unapproval()
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
                DETAILS_TICKET detaTick = new DETAILS_TICKET();
                detaTick.ID_TICK = Convert.ToInt32(Request.Params["ID_TICK"]);
                detaTick.UserId = Convert.ToInt32(Session["UserId"].ToString());
                detaTick.CREATE_DETA_INCI = DateTime.Now;
                detaTick.ID_TYPE_DETA_TICK = 10;
                detaTick.COM_DETA_TICK = COM_DETA_TICK;

                dbc.DETAILS_TICKET.Add(detaTick);
                dbc.SaveChanges();

                //Rechazando el ticket
                var ticket = new TICKET();
                ticket = dbc.TICKETs.Single(x => x.ID_TICK == detaTick.ID_TICK);
                ticket.ID_STAT_END = 9;
                ticket.MODIFIED_TICK = DateTime.Now;

                dbc.TICKETs.Attach(ticket);
                dbc.Entry(ticket).State = EntityState.Modified;
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
