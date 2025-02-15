using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.Net;
using System.Configuration;
using System.Globalization;
using ERPElectrodata.Objects;
using System.Data.Entity;

namespace ERPElectrodata.Controllers
{
    public class MailController : Controller
    {
        public CoreEntities dbc = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();

        //
        // GET: /Mail/
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Mail/SendMail/16300
        public ActionResult SendMail(int id)
        {
            var ticket = (from t in dbc.TICKETs.Where(t => t.ID_TICK == id).ToList()
                          join p in dbc.PRIORITies on t.ID_PRIO equals p.ID_PRIO
                          join e in dbc.STATUS on t.ID_STAT_END equals e.ID_STAT
                          join scl in dbc.CATEGORies on t.ID_CATE equals scl.ID_CATE
                          join cl in dbc.CATEGORies on scl.ID_CATE_PARE equals cl.ID_CATE
                          join sc in dbc.CATEGORies on cl.ID_CATE_PARE equals sc.ID_CATE
                          join c in dbc.CATEGORies on sc.ID_CATE_PARE equals c.ID_CATE
                          select new
                          {
                              t.ID_PRIO,
                              t.ID_STAT_END,
                              t.COD_TICK,
                              NAM_PRIO = p.NAM_PRIO.Substring(0,1)+p.NAM_PRIO.Substring(1,p.NAM_PRIO.Length-1).ToLower(),
                              p.HOU_PRIO,
                              NAM_STAT = e.NAM_STAT.Substring(0,1)+e.NAM_STAT.Substring(1,e.NAM_STAT.Length-1).ToLower(),
                              NAM_SCLAS = scl.NAM_CATE,
                              NAM_CLAS = cl.NAM_CATE,
                              NAM_SCAT = sc.NAM_CATE,
                              NAM_CATE = c.NAM_CATE,
                              t.ID_PERS_ENTI,
                              t.ID_PERS_ENTI_END,
                              t.ID_PERS_ENTI_ASSI,
                              t.UserId,
                              DAT_EXPI_TICK = String.Format("{0:g}",t.DAT_EXPI_TICK),
                              CREA =  String.Format("{0:g}",t.FEC_TICK),
                              //VENC = String.Format("{0:g}", t.FEC_TICK.Value.AddHours(Convert.ToDouble(p.HOU_PRIO))),
                              SCHE = String.Format("{0:g}", t.FEC_INI_TICK),
                              //VENC_SCHE = String.Format("{0:g}", t.FEC_INI_TICK.Value.AddHours(Convert.ToDouble(p.HOU_PRIO))),
                          }).First();

            ViewBag.AffectedUser = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_END).ToList()
                                    join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                    select new
                                    {
                                        User = ce.FIR_NAME.Substring(0, 1) + ce.FIR_NAME.Substring(1, ce.FIR_NAME.Length-1).ToLower()
                                        +' '+ce.LAS_NAME.Substring(0,1)+ce.LAS_NAME.Substring(1,ce.LAS_NAME.Length-1).ToLower(),
                                    }).First().User;
            ViewBag.StaffAssigned = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_ASSI).ToList()
                                    join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                    select new
                                    {
                                        User = ce.FIR_NAME.Substring(0, 1) + ce.FIR_NAME.Substring(1, ce.FIR_NAME.Length - 1).ToLower()
                                        + ' ' + ce.LAS_NAME.Substring(0, 1) + ce.LAS_NAME.Substring(1, ce.LAS_NAME.Length - 1).ToLower(),
                                    }).First().User;

            ViewBag.CreateBy = (from ce in dbe.CLASS_ENTITY.Where(ce => ce.UserId == ticket.UserId).ToList()
                                     select new
                                     {
                                         User = ce.FIR_NAME.Substring(0, 1) + ce.FIR_NAME.Substring(1, ce.FIR_NAME.Length - 1).ToLower()
                                        + ' ' + ce.LAS_NAME.Substring(0, 1) + ce.LAS_NAME.Substring(1, ce.LAS_NAME.Length - 1).ToLower(),
                                     }).First().User;

            ViewBag.ExpirationDate = ticket.DAT_EXPI_TICK;

            if (ticket.ID_STAT_END == 5)
            {
                ViewBag.ScheduledDate = ticket.SCHE;
            }

            ViewBag.SLA = ticket.HOU_PRIO;

            if (ticket.ID_PRIO == 6)
            {
                ViewBag.SLA = "Planning";
                ViewBag.ExpirationDate = "Planning";
            }

            ViewBag.CreationDate = ticket.CREA;
            ViewBag.Code = ticket.COD_TICK;
            ViewBag.Priority = ticket.NAM_PRIO;
            
            ViewBag.Status = ticket.NAM_STAT;
            ViewBag.Category = ticket.NAM_CATE;
            ViewBag.SubCategory = ticket.NAM_SCAT;
            ViewBag.Class = ticket.NAM_CLAS;
            ViewBag.SubClass = ticket.NAM_SCLAS;
            ViewBag.UrlEndUser = "http://190.8.136.204/Ticket/View/"+Convert.ToString(id);
            ViewBag.UrlStaff = "http://190.8.136.204/DetailsTicket/Index/"+Convert.ToString(id);

            return View(Convert.ToString(ticket.ID_STAT_END));
        }

        //
        // GET: /Mail/SendMail/16300
        public ActionResult SendMailUpdateStatus(int id)
        {
            id = 17508;
            int horas = 0, ID_TICK = id, TimeTrans = 0, time=0;
            DateTime Fecha;
            string text = "In SLA";
            var ticket = (from t in dbc.TICKETs.Where(t => t.ID_TICK == id).ToList()
                          join p in dbc.PRIORITies on t.ID_PRIO equals p.ID_PRIO
                          join e in dbc.STATUS on t.ID_STAT_END equals e.ID_STAT
                          join scl in dbc.CATEGORies on t.ID_CATE equals scl.ID_CATE
                          join cl in dbc.CATEGORies on scl.ID_CATE_PARE equals cl.ID_CATE
                          join sc in dbc.CATEGORies on cl.ID_CATE_PARE equals sc.ID_CATE
                          join c in dbc.CATEGORies on sc.ID_CATE_PARE equals c.ID_CATE
                          select new
                          {
                              t.ID_PRIO,
                              t.ID_STAT_END,
                              t.MINUTES,
                              t.COD_TICK,
                              NAM_PRIO = p.NAM_PRIO.Substring(0, 1) + p.NAM_PRIO.Substring(1, p.NAM_PRIO.Length - 1).ToLower(),
                              p.HOU_PRIO,
                              NAM_STAT = e.NAM_STAT.Substring(0, 1) + e.NAM_STAT.Substring(1, e.NAM_STAT.Length - 1).ToLower(),
                              NAM_SCLAS = scl.NAM_CATE,
                              NAM_CLAS = cl.NAM_CATE,
                              NAM_SCAT = sc.NAM_CATE,
                              NAM_CATE = c.NAM_CATE,
                              t.ID_PERS_ENTI,
                              t.ID_PERS_ENTI_END,
                              t.ID_PERS_ENTI_ASSI,
                              t.UserId,
                              CREA = String.Format("{0:g}", t.FEC_TICK),
                              VENC = String.Format("{0:g}", t.FEC_TICK.Value.AddHours(Convert.ToDouble(p.HOU_PRIO-Convert.ToInt32(t.MINUTES/60)))),
                              SCHE = String.Format("{0:g}", t.FEC_INI_TICK),
                              VENC_SCHE = String.Format("{0:g}", t.FEC_INI_TICK.Value.AddHours(Convert.ToDouble(p.HOU_PRIO))),
                          }).First();
            ViewBag.AffectedUser = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_END).ToList()
                                    join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                    select new
                                    {
                                        User = ce.FIR_NAME.Substring(0, 1) + ce.FIR_NAME.Substring(1, ce.FIR_NAME.Length - 1).ToLower()
                                        + ' ' + ce.LAS_NAME.Substring(0, 1) + ce.LAS_NAME.Substring(1, ce.LAS_NAME.Length - 1).ToLower(),
                                    }).First().User;
            ViewBag.StaffAssigned = (from pe in dbe.PERSON_ENTITY.Where(pe => pe.ID_PERS_ENTI == ticket.ID_PERS_ENTI_ASSI).ToList()
                                     join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                     select new
                                     {
                                         User = ce.FIR_NAME.Substring(0, 1) + ce.FIR_NAME.Substring(1, ce.FIR_NAME.Length - 1).ToLower()
                                         + ' ' + ce.LAS_NAME.Substring(0, 1) + ce.LAS_NAME.Substring(1, ce.LAS_NAME.Length - 1).ToLower(),
                                     }).First().User;
            ViewBag.CreateBy = (from ce in dbe.CLASS_ENTITY.Where(ce => ce.UserId == ticket.UserId).ToList()
                                select new
                                {
                                    User = ce.FIR_NAME.Substring(0, 1) + ce.FIR_NAME.Substring(1, ce.FIR_NAME.Length - 1).ToLower()
                                   + ' ' + ce.LAS_NAME.Substring(0, 1) + ce.LAS_NAME.Substring(1, ce.LAS_NAME.Length - 1).ToLower(),
                                }).First().User;
            ViewBag.ExpirationDate = ticket.VENC;
            if (ticket.ID_STAT_END == 5)
            {
                try
                {
                    Fecha = Convert.ToDateTime(dbc.DETAILS_TICKET.Where(x => x.ID_TICK == ID_TICK)
                                .Where(x => x.ID_STAT == 5)
                                .OrderByDescending(x => x.CREATE_DETA_INCI)
                                .Take(1).Single().FEC_SCHE);
                }
                catch
                {
                    Fecha = Convert.ToDateTime(dbc.TICKETs.Single(x => x.ID_TICK == ID_TICK).FEC_INI_TICK);
                }
            }
            else
            {
                try
                {
                    Fecha = Convert.ToDateTime(dbc.DETAILS_TICKET.Where(x => x.ID_TICK == ID_TICK)
                                .Where(x => x.ID_STAT != null)
                                .OrderByDescending(x => x.CREATE_DETA_INCI)
                                .Take(1).Single().CREATE_DETA_INCI);
                }
                catch
                {
                    Fecha = Convert.ToDateTime(dbc.TICKETs.Single(x => x.ID_TICK == ID_TICK).FEC_TICK);
                }
            }

            TimeTrans = Convert.ToInt32(dbc.TICKETs.Single(x => x.ID_TICK == ID_TICK).MINUTES);

            if (Fecha > DateTime.Now)
            {
                time = TimeTrans;
            }
            else
            {
                time = Convert.ToInt32(TimeTrans / 60) + DateTime.Now.Subtract(Fecha).Days * 24 + DateTime.Now.Subtract(Fecha).Hours;
            }

            horas = Convert.ToInt32(ticket.HOU_PRIO) - Convert.ToInt32(time);
            if(horas<0)
            {
                text = "Out SLA";
            }
            if (ticket.ID_PRIO == 6)
            {
            }
            ViewBag.CreationDate = ticket.CREA;
            ViewBag.Code = ticket.COD_TICK;
            ViewBag.Priority = ticket.NAM_PRIO;
            ViewBag.SLA = ticket.HOU_PRIO;
            ViewBag.Status = ticket.NAM_STAT;
            ViewBag.Category = ticket.NAM_CATE;
            ViewBag.SubCategory = ticket.NAM_SCAT;
            ViewBag.Class = ticket.NAM_CLAS;
            ViewBag.SubClass = ticket.NAM_SCLAS;
            ViewBag.UrlEndUser = "http://190.8.136.204/Ticket/View/" + Convert.ToString(id);
            ViewBag.UrlStaff = "http://190.8.136.204/DetailsTicket/Index/" + Convert.ToString(id);
            ViewBag.StatusResolution = text + " " + Convert.ToString(horas);
            return View("US"+Convert.ToString(ticket.ID_STAT_END));
        }

        public ActionResult SendMailExpirationContract(int id = 0)
        {

            DateTime END_DATE = DateTime.Today;
            END_DATE = END_DATE.AddDays(id);

            var qLoca = dbc.LOCATIONs.ToList()
                .Join(dbc.SITEs, x => x.ID_SITE, s => s.ID_SITE, (x, s) => new
                {
                    x.ID_LOCA,
                    x.NAM_LOCA,
                    x.VIG_LOCA,
                    s.ID_SITE,
                    s.NAM_SITE,
                });

            var query = (from a in dbe.PERSON_CONTRACT.Where(x => x.LAS_CONT == true && x.END_DATE == END_DATE).ToList()
                         join b in dbe.PERSON_ENTITY on a.ID_PERS_ENTI equals b.ID_PERS_ENTI
                         join c in dbe.CLASS_ENTITY on b.ID_ENTI2 equals c.ID_ENTI
                         join d in dbe.PERSON_LOCATION on a.ID_PERS_LOCA equals d.ID_PERS_LOCA
                         where d.VIG_LOCA == true
                         join e in qLoca on d.ID_LOCA equals e.ID_LOCA
                         select new
                         {
                             a.ID_PERS_CONT,
                             a.ID_PERS_ENTI,
                             a.STAR_DATE,
                             a.END_DATE,
                             a.CODE,
                             c.LAS_NAME,
                             c.FIR_NAME,
                             c.MOT_NAME,
                             e.NAM_LOCA,
                             e.NAM_SITE,
                         });

            var list = new List<V_Employees>();

            foreach (var a in query.ToList())
            {
                V_Employees ve = new V_Employees();
                ve.ID_PERS_CONT = a.ID_PERS_CONT;
                ve.CODE = a.CODE;
                ve.ID_PERS_ENTI = a.ID_PERS_ENTI;
                ve.STAR_DATE = a.STAR_DATE;
                ve.END_DATE = a.END_DATE;
                ve.LAS_NAME = a.LAS_NAME.ToUpper().Substring(0, 1) + a.LAS_NAME.ToLower().Substring(1, a.LAS_NAME.Length - 1);
                ve.FIR_NAME = a.FIR_NAME.ToUpper().Substring(0, 1) + a.FIR_NAME.ToLower().Substring(1, a.FIR_NAME.Length - 1);
                ve.MOT_NAME = (a.MOT_NAME == null ? "" : a.MOT_NAME.ToUpper().Substring(0, 1) + a.MOT_NAME.ToLower().Substring(1, a.MOT_NAME.Length - 1));
                ve.NAM_LOCA = a.NAM_LOCA.ToUpper().Substring(0, 1) + a.NAM_LOCA.ToLower().Substring(1, a.NAM_LOCA.Length - 1);
                ve.NAM_SITE = a.NAM_SITE.ToUpper().Substring(0, 1) + a.NAM_SITE.ToLower().Substring(1, a.NAM_SITE.Length - 1);
                list.Add(ve);
            }

            if (id == 5) { ViewBag.Color = "#B21E1E"; }
            else if (id == 15) { ViewBag.Color = "#FFBB00"; }
            else { ViewBag.Color = "#7CBB00"; }

            ViewBag.Dias = id;
            ViewBag.AdminTalent = "http://190.8.136.204/Talent/Index";
            //ViewBag.AdminTalent = "http://localhost:50143/Talent/Index";
            return View("ExpirationContract", list);
        }

        public ActionResult SendMailContract(int id = 0, int id1 = 1)
        {
            var qLoca = dbc.LOCATIONs.ToList()
                .Join(dbc.SITEs, x => x.ID_SITE, s => s.ID_SITE, (x, s) => new
                {
                    x.ID_LOCA,
                    x.NAM_LOCA,
                    x.VIG_LOCA,
                    s.ID_SITE,
                    s.NAM_SITE,
                });

            var query = (from a in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == id).ToList()
                         join b in dbe.PERSON_CONTRACT on a.ID_PERS_ENTI equals b.ID_PERS_ENTI
                         where b.LAS_CONT == true
                         join c in dbe.PERSON_LOCATION on a.ID_PERS_ENTI equals c.ID_PERS_ENTI
                         join d in qLoca on c.ID_LOCA equals d.ID_LOCA
                         where d.VIG_LOCA == true
                         join f in dbe.CHARTs on b.ID_CHAR equals f.ID_CHAR
                         join g in dbe.NAME_CHART on f.ID_NAM_CHAR equals g.ID_NAM_CHAR
                         join i in dbe.CLASS_ENTITY on a.ID_ENTI2 equals i.ID_ENTI
                         join j in dbe.PERSON_STATUS on a.ID_PERS_STAT equals j.ID_PERS_STAT
                         join k in dbe.CONTRACT_CONDITION on b.ID_COND_CONT equals k.ID_COND_CONT
                         join l in dbe.WORK_PERIOD on b.ID_WORK_PERI equals l.ID_WORK_PERI into ll
                         from xl in ll.DefaultIfEmpty()
                         select new
                         {
                             Code = b.CODE,
                             Employee = CapitalizeCadena(i.FIR_NAME.ToLower() + " " + i.LAS_NAME.ToLower()),
                             Area = CapitalizeCadena(FindNodo(Convert.ToInt32(f.ID_CHAR_PARE), 2)),
                             JobTitle = CapitalizeCadena(g.NAM_CHAR),
                             SiteLocation = CapitalizeCadena(d.NAM_SITE.ToLower() + " - " + d.NAM_LOCA.ToLower()),
                             STAR_DATE = (b.STAR_DATE == null ? "-" : String.Format("{0:M/d/yyyy}", b.STAR_DATE)),
                             END_DATE = (b.END_DATE == null ? "-" : String.Format("{0:M/d/yyyy}", b.END_DATE)),
                             TER_DATE = (xl == null ? "-" : (xl.CESS_DATE == null ? "-" : String.Format("{0:M/d/yyyy}", xl.CESS_DATE))),
                             FEC_INI = (b.STAR_DATE == null ? "-" : String.Format("{0:dd/MM/yyyy}", b.STAR_DATE)),
                             FEC_FIN = (b.END_DATE == null ? "-" : String.Format("{0:dd/MM/yyyy}", b.END_DATE)),
                             FEC_TER = (xl == null ? "-" : (xl.CESS_DATE == null ? "-" : String.Format("{0:dd/MM/yyyy}", xl.CESS_DATE))),
                             Condition = CapitalizeCadena(j.NAM_STAT.ToLower() + " - " + k.CONDITION.ToLower()),
                             SBU = CapitalizeCadena(FindNodo(Convert.ToInt32(f.ID_CHAR_PARE), 1)),
                         });

            int ctd = query.Count();

            ViewBag.Code = query.First().Code;
            ViewBag.Employee = query.First().Employee;
            ViewBag.Area = query.First().Area;
            ViewBag.JobTitle = query.First().JobTitle;
            ViewBag.SiteLocation = query.First().SiteLocation;
            ViewBag.STAR_DATE = query.First().STAR_DATE;
            ViewBag.END_DATE = query.First().END_DATE;
            ViewBag.TER_DATE = query.First().TER_DATE;
            ViewBag.FEC_INI = query.First().FEC_INI;
            ViewBag.FEC_FIN = query.First().FEC_FIN;
            ViewBag.FEC_TER = query.First().FEC_TER;
            ViewBag.Condition = query.First().Condition;
            ViewBag.SBU = query.First().SBU;
            if (id1 == 1)
            {
                return View("NewContract");
            }
            else
            {
                return View("CeasedEmployee");
            }

        }

        public ActionResult SendMailPettyCashPayment(int id = 0) { 
            //id: ID_REQU_EXPE
            var qUser = (from a in dbe.CLASS_ENTITY
                         join b in dbe.PERSON_ENTITY.Where(x=>x.ID_ENTI1 == 9) on a.ID_ENTI equals b.ID_ENTI2
                         select new {
                            a.ID_ENTI,
                            a.UserId,
                            USER = a.FIR_NAME + " " + a.LAS_NAME,
                            b.ID_PERS_ENTI
                         });

            DateTime FecApp = FechaActivity(id, 2);
            string DatAppEsp = String.Format("{0:dd/MM/yyyy}",FecApp);
            string DatAppIng = String.Format("{0:M/d/yyyy}", FecApp);

            var query = (from a in dbc.REQUEST_EXPENSE.Where(x => x.ID_REQU_EXPE == id).ToList()
                         join b in qUser on a.ID_PERS_ENTI_REQU equals b.ID_PERS_ENTI
                         join c in qUser on a.ID_PERS_ENTI_APPR equals c.ID_PERS_ENTI
                         join d in qUser on a.ID_PERS_ENTI_ASSI equals d.ID_PERS_ENTI
                         join e in dbc.TYPE_DELI_SUST on a.ID_TYPE_DELI_SUST equals e.ID_TYPE_DELI_SUST
                         select new { 
                             a.ID_REQU_EXPE,
                             Code = e.NAM_TYPE_DELI_SUST + " " + a.COD_REQU_EXPE,
                             Requester = b.USER,
                             Approves = c.USER,
                             Payer = d.USER,
                             DateRegisterIng = String.Format("{0:M/d/yyyy}",a.DAT_REGI),
                             DateRegisterEsp = String.Format("{0:dd/MM/yyyy}", a.DAT_REGI),
                             Amount = (a.CURRENCY == "MN" ?
                             "S/. " + String.Format("{0:N2}", a.AMOUNT) + " Peruvian Nuevo Sol": 
                             "US$ " + String.Format("{0:N2}",a.AMOUNT) + " Dollar American"),
                             AmountEsp = (a.CURRENCY == "MN" ?
                             "S/. " + String.Format("{0:N2}", a.AMOUNT) + " Nuevos Soles" :
                             "US$ " + String.Format("{0:N2}", a.AMOUNT) + " Dolares Americanos")
                         });

            ViewBag.Code = CapitalizeCadena(query.First().Code.ToLower());
            ViewBag.Requester = CapitalizeCadena(query.First().Requester.ToLower());
            ViewBag.DateRegisterIng = query.First().DateRegisterIng;
            ViewBag.DateRegisterEsp = query.First().DateRegisterEsp;
            ViewBag.Approves = CapitalizeCadena(query.First().Approves.ToLower());
            ViewBag.DateApprovesIng = DatAppIng;
            ViewBag.DateApprovesEsp = DatAppEsp;
            ViewBag.Payer = CapitalizeCadena(query.First().Payer.ToLower());
            ViewBag.Amount = query.First().Amount;
            ViewBag.AmountEsp = query.First().AmountEsp;

            return View();
        }

        public ActionResult SendMailPettyCashAccountability(int id = 0)
        {
            //id: ID_REQU_EXPE
            var qUser = (from a in dbe.CLASS_ENTITY
                         join b in dbe.PERSON_ENTITY.Where(x => x.ID_ENTI1 == 9) on a.ID_ENTI equals b.ID_ENTI2
                         select new
                         {
                             a.ID_ENTI,
                             a.UserId,
                             USER = a.FIR_NAME + " " + a.LAS_NAME,
                             b.ID_PERS_ENTI
                         });

            DateTime FecApp = FechaActivity(id, 2);
            string DatAppEsp = String.Format("{0:dd/MM/yyyy hh:mm tt}", FecApp);
            string DatAppIng = String.Format("{0:M/d/yyyy HH:mm}", FecApp);

            DateTime FecxRendir = FechaActivity(id, 5);
            string DatxRenEsp = String.Format("{0:dd/MM/yyyy hh:mm tt}", FecxRendir);
            string DatxRenIng = String.Format("{0:M/d/yyyy HH:mm}", FecxRendir);

            var query = (from a in dbc.REQUEST_EXPENSE.Where(x => x.ID_REQU_EXPE == id).ToList()
                         join b in qUser on a.ID_PERS_ENTI_REQU equals b.ID_PERS_ENTI
                         join c in qUser on a.ID_PERS_ENTI_APPR equals c.ID_PERS_ENTI
                         join d in qUser on a.ID_PERS_ENTI_ASSI equals d.ID_PERS_ENTI
                         join e in dbc.TYPE_DELI_SUST on a.ID_TYPE_DELI_SUST equals e.ID_TYPE_DELI_SUST
                         select new
                         {
                             a.ID_REQU_EXPE,
                             Code = e.NAM_TYPE_DELI_SUST + " " + a.COD_REQU_EXPE,
                             Requester = b.USER,
                             Approves = c.USER,
                             Payer = d.USER,
                             DateRegisterIng = String.Format("{0:M/d/yyyy HH:mm}", a.DAT_REGI),
                             DateRegisterEsp = String.Format("{0:dd/MM/yyyy hh:mm tt}", a.DAT_REGI),
                             a.CURRENCY,
                             Amount = (a.CURRENCY == "MN" ?
                             "S/. " + String.Format("{0:N2}", a.AMOUNT) + " Peruvian Nuevo Sol" :
                             "US$ " + String.Format("{0:N2}", a.AMOUNT) + " Dollar American"),
                             AmountEsp = (a.CURRENCY == "MN" ?
                             "S/. " + String.Format("{0:N2}", a.AMOUNT) + " Nuevos Soles" :
                             "US$ " + String.Format("{0:N2}", a.AMOUNT) + " Dolares Americanos")
                         });

            var qDocs = (from a in dbc.DOCUMENT_EXPENSE.Where(x => x.ID_REQU_EXPE == id && x.ID_STAT_DOCU_EXPE != 2)
                         select new {
                             ID_TYPE_DOCU_EXPE  = (a.ID_TYPE_DOCU_EXPE != 5 && a.ID_TYPE_DOCU_EXPE != 6 ? 0 : a.ID_TYPE_DOCU_EXPE),
                            a.AMOUNT,
                         });

            var qAccount = (from a in qDocs
                            group a by new { a.ID_TYPE_DOCU_EXPE } into g
                            select new
                            {
                                g.Key.ID_TYPE_DOCU_EXPE,
                                SUMA = g.Sum(x => x.AMOUNT)
                            });

            Decimal TTAcco = 0;
            Decimal TTDevo = 0;
            Decimal TTRein = 0;
            if (qAccount.Where(x => x.ID_TYPE_DOCU_EXPE == 0).Count() > 0) {
                TTAcco = qAccount.Single(x => x.ID_TYPE_DOCU_EXPE == 0).SUMA.Value;
            }
            if (qAccount.Where(x => x.ID_TYPE_DOCU_EXPE == 5).Count() > 0) {
                TTDevo = qAccount.Single(x => x.ID_TYPE_DOCU_EXPE == 5).SUMA.Value;
            }
            if (qAccount.Where(x => x.ID_TYPE_DOCU_EXPE == 6).Count() > 0) {
                TTRein = qAccount.Single(x => x.ID_TYPE_DOCU_EXPE == 6).SUMA.Value * -1;
            }

            ViewBag.Code = CapitalizeCadena(query.First().Code.ToLower());
            ViewBag.Requester = CapitalizeCadena(query.First().Requester.ToLower());
            ViewBag.DateRegisterIng = query.First().DateRegisterIng;
            ViewBag.DateRegisterEsp = query.First().DateRegisterEsp.ToLower();
            ViewBag.Approves = CapitalizeCadena(query.First().Approves.ToLower());
            ViewBag.DateApprovesIng = DatAppIng;
            ViewBag.DateApprovesEsp = DatAppEsp.ToLower();
            ViewBag.DatexRendirIng = DatxRenIng;
            ViewBag.DatexRendirEsp = DatxRenEsp.ToLower();
            ViewBag.Payer = CapitalizeCadena(query.First().Payer.ToLower());
            ViewBag.AmountRequ = query.First().Amount;
            ViewBag.AmountRequEsp = query.First().AmountEsp;

            ViewBag.AmountAcco = (query.First().CURRENCY == "MN" ? "S/. " + String.Format("{0:N2}", TTAcco) + " Peruvian Nuevo Sol" :
                                                                  "US$ " + String.Format("{0:N2}", TTAcco) + " Dollar American");
            ViewBag.AmountAccoEsp = (query.First().CURRENCY == "MN" ? "S/. " + String.Format("{0:N2}", TTAcco) + " Nuevos Soles" :
                                                                  "US$ " + String.Format("{0:N2}", TTAcco) + " Dolares Americanos");

            ViewBag.AmountDevo = (query.First().CURRENCY == "MN" ? "S/. " + String.Format("{0:N2}", TTDevo) + " Peruvian Nuevo Sol" :
                                                                  "US$ " + String.Format("{0:N2}", TTDevo) + " Dollar American");
            ViewBag.AmountDevoEsp = (query.First().CURRENCY == "MN" ? "S/. " + String.Format("{0:N2}", TTDevo) + " Nuevos Soles" :
                                                                  "US$ " + String.Format("{0:N2}", TTDevo) + " Dolares Americanos");

            ViewBag.AmountRein = (query.First().CURRENCY == "MN" ? "S/. " + String.Format("{0:N2}", TTRein) + " Peruvian Nuevo Sol" :
                                                                  "US$ " + String.Format("{0:N2}", TTRein) + " Dollar American");
            ViewBag.AmountReinEsp = (query.First().CURRENCY == "MN" ? "S/. " + String.Format("{0:N2}", TTRein) + " Nuevos Soles" :
                                                                  "US$ " + String.Format("{0:N2}", TTRein) + " Dolares Americanos");

            return View();
        }

        public DateTime FechaActivity(int id = 0, int id1 = 0)
        {
            REQUEST_EXPENSE_ACTIVITY rea = dbc.REQUEST_EXPENSE_ACTIVITY.Single(x=>x.ID_REQU_EXPE == id && x.ID_STAT_REQU_EXPE == id1);
            return rea.DAT_STAR.Value;
        }

        public ActionResult SendEmilio() {

            SendMail mail = new SendMail();
            string HB = mail.NotificationPettyCashAccountability(21);

            return Content("OK");
        }

        public ActionResult SendBeneficiaryAgeLimit(int id = 0, int id1 = 0)
        {
            var query = (from a in dbe.BENEFICIARies.Where(x => x.ID_BENE == id)
                         join b in dbe.PERSON_ENTITY on a.ID_PERS_ENTI equals b.ID_PERS_ENTI
                         join c in dbe.CLASS_ENTITY on b.ID_ENTI2 equals c.ID_ENTI
                         select new { 
                            Beneficiary = a.FIR_NAME + " " + a.LAS_NAME,
                            Holder = c.FIR_NAME + " " + c.LAS_NAME,
                            a.DATE_BRITH,
                            a.NUMB_DNI,
                         });

            ViewBag.Beneficiary = CapitalizeCadena(query.First().Beneficiary.ToLower());
            ViewBag.Holder = CapitalizeCadena(query.First().Holder.ToLower());
            ViewBag.DATE_BRITH = String.Format("{0:d}",query.First().DATE_BRITH);
            ViewBag.NUMB_DNI = query.First().NUMB_DNI;
            ViewBag.Title = (id1 == 18 ? "Eighteen Years Old" : "Twenty Four Years Old");
            ViewBag.Age = id1.ToString();
            return View("SendBeneficiaryEighteenAge");
        }

        public ActionResult SendMailIssues(int id = 0)
        { 
            var qSiteLoca = dbc.SITEs.Where(x => x.ID_ACCO == 4)
                            .Join(dbc.LOCATIONs, x => x.ID_SITE, l => l.ID_SITE, (x, l) => new {
                                SiteLocation = x.NAM_SITE + " - " + l.NAM_LOCA,
                                x.ID_SITE,
                                l.ID_LOCA,
                            });

            var query = (from pd in dbe.PERSON_DOCUMENTS.Where(x => x.ID_PERS_DOCU == id).ToList()
                         join pe in dbe.PERSON_ENTITY on pd.ID_PERS_ENTI equals pe.ID_PERS_ENTI
                         join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                         join co in dbe.CLASS_ENTITY on pe.ID_ENTI1 equals co.ID_ENTI
                         join pc in dbe.PERSON_CONTRACT.Where(x=>x.LAS_CONT == true) on pe.ID_PERS_ENTI equals pc.ID_PERS_ENTI into lpc                         
                         from xpc in lpc.DefaultIfEmpty()
                         join ch in dbe.CHARTs on (xpc == null ? 0 : xpc.ID_CHAR) equals ch.ID_CHAR into lch
                         from xch in lch.DefaultIfEmpty()
                         join nch in dbe.NAME_CHART on (xch == null ? 0 : xch.ID_NAM_CHAR) equals nch.ID_NAM_CHAR into lnch
                         from xnch in lnch.DefaultIfEmpty()
                         join pl in dbe.PERSON_LOCATION.Where(x=>x.END_DATE == null) on pe.ID_PERS_ENTI equals pl.ID_PERS_ENTI into lpl
                         from xpl in lpl.DefaultIfEmpty()
                         join qsl in qSiteLoca on (xpl==null ? 0 : xpl.ID_LOCA) equals qsl.ID_LOCA into lqsl
                         from xqsl in lqsl.DefaultIfEmpty()
                         select new { 
                            User = ce.FIR_NAME.ToLower() + " " + ce.LAS_NAME.ToLower(),
                            CIA = co.COM_NAME.ToLower(),
                            SBU = FindNodo(Convert.ToInt32((xch.ID_CHAR_PARE == null ? 0 : xch.ID_CHAR_PARE)), 1).ToLower(),
                            AREA = FindNodo(Convert.ToInt32((xch.ID_CHAR_PARE == null ? 0 : xch.ID_CHAR_PARE)), 2).ToLower(),
                            JOBTITLE = (xnch == null ? "-" : xnch.NAM_CHAR.ToLower()),
                            SITELOCATION = (xqsl == null ? "-" : xqsl.SiteLocation.ToLower()),
                         });

            ViewBag.USER = CapitalizeCadena(query.First().User);
            ViewBag.CIA = CapitalizeCadena(query.First().CIA);
            ViewBag.SBU = CapitalizeCadena(query.First().SBU);
            ViewBag.AREA = CapitalizeCadena(query.First().AREA);
            ViewBag.JOBTITLE = CapitalizeCadena(query.First().JOBTITLE);
            ViewBag.SITELOCATION = CapitalizeCadena(query.First().SITELOCATION);

            return View("MailIssues");
        }

        public string SBU(int id)
        {
            string usb = "";
            var query = dbe.AREAs.Where(x => x.ID_AREA == id).First();
            if (query.NIV_AREA == 2)
            {
                usb = query.NOM_AREA;
            }
            else if (query.NIV_AREA == 3)
            {
                var query1 = dbe.AREAs.Where(x => x.ID_AREA == query.ID_AREA_PARENT).First();
                usb = query1.NOM_AREA;
            }
            else if (query.NIV_AREA == 4)
            {
                var query1 = dbe.AREAs.Where(x => x.ID_AREA == query.ID_AREA_PARENT)
                            .Join(dbe.AREAs, x => x.ID_AREA_PARENT, y => y.ID_AREA, (x, y) => new
                            {
                                y.NOM_AREA
                            }).First();

                usb = query1.NOM_AREA;
            }
            usb = CapitalizeCadena(usb);
            return usb;
        }

        public string FindNodo(int idchpare = 0, int type = 0)
        {

            var query = (from a in dbe.CHARTs.Where(x => x.ID_CHAR == idchpare)
                         join b in dbe.NAME_CHART on a.ID_NAM_CHAR equals b.ID_NAM_CHAR

                         join c in dbe.CHARTs on a.ID_CHAR_PARE equals c.ID_CHAR into lc
                         from xc in lc.DefaultIfEmpty()
                         join d in dbe.NAME_CHART on (xc == null ? 0 : xc.ID_NAM_CHAR) equals d.ID_NAM_CHAR into ld
                         from xd in ld.DefaultIfEmpty()

                         join e in dbe.CHARTs on (xc == null ? 0 : xc.ID_CHAR_PARE) equals e.ID_CHAR into le
                         from xe in le.DefaultIfEmpty()
                         join f in dbe.NAME_CHART on (xe == null ? 0 : xe.ID_NAM_CHAR) equals f.ID_NAM_CHAR into lf
                         from xf in lf.DefaultIfEmpty()

                         join g in dbe.CHARTs on (xe == null ? 0 : xe.ID_CHAR_PARE) equals g.ID_CHAR into lg
                         from xg in lg.DefaultIfEmpty()
                         join h in dbe.NAME_CHART on (xg == null ? 0 : xg.ID_NAM_CHAR) equals h.ID_NAM_CHAR into lh
                         from xh in lh.DefaultIfEmpty()

                         select new
                         {
                             NAM_CHAR = (b.ID_TYPE_CHAR == type ? b.NAM_CHAR :
                                        (xd == null ? "-" : xd.ID_TYPE_CHAR == type ? xd.NAM_CHAR :
                                        (xf == null ? "-" : xf.ID_TYPE_CHAR == type ? xf.NAM_CHAR :
                                        (xh == null ? "-" : xh.ID_TYPE_CHAR == type ? xh.NAM_CHAR : "-"
                                        )))),
                         });
            if (query.Count() > 0)
            {
                return query.First().NAM_CHAR.ToLower();
            }
            return "-";
        }

        public string CapitalizeCadena(string cadena)
        {
            cadena = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(cadena.ToLower());
            return cadena;
        }
    }
}
