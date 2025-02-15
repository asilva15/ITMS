using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.Data;
using System.Data.Entity;
using System.IO;

namespace ERPElectrodata.Controllers
{
    public class Person_ContractController : Controller
    {
        
        EntityEntities dbe = new EntityEntities();
        CoreEntities dbc = new CoreEntities();

        // GET: /Person_Contract/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TreeViewContract(int id = 0) {
            ViewBag.ID_PERS_ENTI = id;
            return View();
        }

        public ActionResult ListContract(int id = 0) {
            string ID_PARA = Convert.ToString(Request.Params["ID_PARA"]);
            if (ID_PARA == null)
            {
                var query = (from q in dbe.PERSON_CONTRACT.Where(x => x.ID_PERS_ENTI == id && x.ID_PERS_CONT_PARENT == null).ToList()
                             join t in dbe.PERSON_STATUS on q.ID_PERS_STAT equals t.ID_PERS_STAT
                             join d in dbe.PERSON_DOCUMENTS on q.ID_PERS_CONT equals d.ID_PERS_CONT into ld
                             from xd in ld.DefaultIfEmpty()
                             select new
                             {
                                 ID_PARA = q.ID_PERS_CONT,
                                 NAME_PARA = q.CODE,
                                 HAS_VALUE = CountAddendum(q.ID_PERS_CONT),
                                 START_DATE = String.Format("{0:d}",q.STAR_DATE),
                                 END_DATE = String.Format("{0:d}", q.END_DATE),
                                 STATUS = ": " + t.NAM_STAT,
                                 expanded = false,
                                 TYPE = (q.ID_PERS_CONT_PARENT == null ? ResourceLanguaje.Resource.Contract : ResourceLanguaje.Resource.Addendum),
                                 NAM_ATTA = (xd == null ? "" : xd.NAM_ATTA),
                                 NAM_FILE = (xd == null ? "" : xd.NAM_ATTA + "_" + Convert.ToString(xd.ID_PERS_DOCU) + xd.EXT_ATTA),
                                 DELETE = q.LAS_CONT,
                             });

                return Json(query.OrderByDescending(x => x.NAME_PARA), JsonRequestBehavior.AllowGet);
            }
            else {
                int IDPARA = Convert.ToInt32(ID_PARA);
                var query = (from q in dbe.PERSON_CONTRACT.Where(x => x.ID_PERS_CONT_PARENT == IDPARA).ToList()
                             join d in dbe.PERSON_DOCUMENTS.Where(x=>x.ID_TYPE_DOCU == 2) on q.ID_PERS_CONT equals d.ID_PERS_CONT into ld
                             from xd in ld.DefaultIfEmpty()
                             select new
                             {
                                 ID_PARA = q.ID_PERS_CONT,
                                 NAME_PARA = q.CODE,
                                 HAS_VALUE = false,
                                 DATE = q.STAR_DATE,
                                 START_DATE = String.Format("{0:d}", q.STAR_DATE),
                                 END_DATE = String.Format("{0:d}", q.END_DATE),
                                 STATUS = "",
                                 expanded = false,
                                 NAM_ATTA = (xd == null ? "" : xd.NAM_ATTA),
                                 NAM_FILE = (xd == null ? "" : xd.NAM_ATTA + "_" + Convert.ToString(xd.ID_PERS_DOCU) + xd.EXT_ATTA),
                                 TYPE = (q.ID_PERS_CONT_PARENT == null ? ResourceLanguaje.Resource.Contract : ResourceLanguaje.Resource.Addendum),
                                 DELETE = q.LAS_CONT,
                             });

                return Json(query.OrderByDescending(x => x.DATE), JsonRequestBehavior.AllowGet);            
            }
        }

        public bool CountAddendum(int id = 0) {
            int ctd = dbe.PERSON_CONTRACT.Where(x => x.ID_PERS_CONT_PARENT == id).Count();
            return (ctd == 0 ? false : true);
        }

        public ActionResult DataUserByIdPE(int id = 0) {
            var result = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == id)
                          join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                          join cec in dbe.CLASS_ENTITY on pe.ID_ENTI1 equals cec.ID_ENTI
                          select new { 
                            User = ce.FIR_NAME.ToLower() + " " + ce.LAS_NAME.ToLower(),
                            Company = cec.COM_NAME.ToLower(),
                            CEL_ENTI = (ce.CEL_ENTI == null ? "-" : ce.CEL_ENTI),
                            TEL_ENTI = (ce.TEL_ENTI == null ? "-" : ce.TEL_ENTI),
                            EMA_ENTI = (ce.EMA_ENTI == null ? "-" : ce.EMA_ENTI),
                            EMA_ELEC = (pe.EMA_ELEC == null ? "-" : pe.EMA_ELEC.ToLower()),
                          });

            return Json(new { data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewContract(int id = 0,int id1 = 0) {
            ViewBag.ID_PERS_CONT = id;
            ViewBag.ID_PERS_ENTI = id1;
            ViewBag.ID_CIA = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == id1).ID_ENTI1.Value;
            return View();
        }

        public ActionResult LoadViewContract(int id = 0, int id1 = 0)
        {

            //var qPC = dbe.PERSON_CONTRACT.Single(x => x.ID_PERS_CONT == id);
            //id: ID_PERS_ENTI
            string ID_ENTI = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == id1).ID_ENTI1.Value.ToString();

            //x.ID_PARA = 6 : CIA
            var ap = dbc.ACCOUNT_PARAMETER.Single(x => x.ID_PARA == 6 && x.VAL_ACCO_PARA == ID_ENTI);
            int ID_ACCO = (ap == null ? 0 : ap.ID_ACCO.Value);

            var qSL = dbc.SITEs.Where(x => x.ID_ACCO == ID_ACCO).ToList()
                      .Join(dbc.LOCATIONs, x => x.ID_SITE, l => l.ID_SITE, (x, l) => new { 
                        x.ID_SITE,
                        l.ID_LOCA,
                        SITE_LOCATON = x.NAM_SITE + " - " + l.NAM_LOCA,
                      });

            var result = (from pc in dbe.PERSON_CONTRACT.Where(x => x.ID_PERS_CONT == id).ToList()
                          join ch in dbe.CHARTs on (pc.ID_CHAR == null ? 0 : pc.ID_CHAR) equals ch.ID_CHAR
                          join nc in dbe.NAME_CHART on (ch == null ? 0 : ch.ID_NAM_CHAR) equals nc.ID_NAM_CHAR
                          join pl in dbe.PERSON_LOCATION on pc.ID_PERS_LOCA equals pl.ID_PERS_LOCA into lpl
                          from xpl in lpl.DefaultIfEmpty()
                          join q in qSL on (xpl == null ? 0 : xpl.ID_LOCA) equals q.ID_LOCA into lq
                          from xq in lq.DefaultIfEmpty()
                          join s in dbe.PERSON_STATUS on pc.ID_PERS_STAT equals s.ID_PERS_STAT
                          join c in dbe.CONTRACT_CONDITION on pc.ID_COND_CONT equals c.ID_COND_CONT
                          select new
                          {
                              pc.CODE,
                              NAM_STAT = s.NAM_STAT.ToLower(),
                              STAR_DATE = String.Format("{0:d}", pc.STAR_DATE),
                              END_DATE = String.Format("{0:d}", pc.END_DATE),
                              GROSS_SALARY = (pc.GROSS_SALARY == null? "-" : String.Format("{0:g}", pc.GROSS_SALARY)),
                              NOM_AREA = FindNodo(Convert.ToInt32((ch == null ? 0 : ch.ID_CHAR_PARE)), 2),
                              NAM_CARG = (nc == null ? "-" : nc.NAM_CHAR).ToLower(),
                              CONDITION = c.CONDITION.ToLower(),
                              TYPE = (pc.ID_PERS_CONT_PARENT== null ? ResourceLanguaje.Resource.Contract: ResourceLanguaje.Resource.Addendum),
                              SITE_LOCA = (xq == null ? "-" : xq.SITE_LOCATON.ToLower()),
                              SBU = FindNodo(Convert.ToInt32((ch == null ? 0 : ch.ID_CHAR_PARE)), 1),
                          });

            return Json(new { data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
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

        public string USB(int id)
        {
            string usb = "";
            try
            {
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
            }
            catch { }
            if (usb.Trim().Length == 0) { usb = "-"; }
            return usb.ToLower();
        }

        public ActionResult FormNewContract(int id = 0, int id1 = 0, string id2 = "NewCont", int id3 = 0)
        {
            ViewBag.ID_PERS_ENTI = id;
            ViewBag.ID_PERS_CONT = id1;
            ViewBag.ACCION = id2;
            ViewBag.ID_CIA = id3;
            if (id2 == "NewAdde")
            {

                var pc = (from a in dbe.PERSON_CONTRACT.Where(x => x.ID_PERS_CONT == id1).ToList()
                          join b in dbe.CHARTs on a.ID_CHAR equals b.ID_CHAR
                          join c in dbe.NAME_CHART on b.ID_NAM_CHAR equals c.ID_NAM_CHAR
                          join d in dbe.PERSON_LOCATION on a.ID_PERS_LOCA equals d.ID_PERS_LOCA into ld
                          from xd in ld.DefaultIfEmpty()
                          select new
                          {
                              a.STAR_DATE,
                              a.END_DATE,
                              a.GROSS_SALARY,
                              a.ID_COND_CONT,
                              a.ID_CHAR,
                              c.NAM_CHAR,
                              ID_LOCA = (xd == null ? 0 : xd.ID_LOCA),
                          });

                DateTime ed = Convert.ToDateTime(pc.First().END_DATE);
                ed = ed.AddDays(1);

                ViewBag.STAR_DATE = (pc.First().STAR_DATE == null ? "" : String.Format("{0:d}", pc.First().STAR_DATE) + " 12:00 AM");
                ViewBag.END_DATE = (pc.First().END_DATE == null ? "" : String.Format("{0:d}", ed) + " 12:00 AM");
                ViewBag.GROSS_SALARY = pc.First().GROSS_SALARY; // string.Format("{0:N2}", pc.First().GROSS_SALARY);
                ViewBag.ID_COND_CONT = pc.First().ID_COND_CONT;
                ViewBag.ID_CHAR = pc.First().ID_CHAR;
                ViewBag.NAM_CHAR = pc.First().NAM_CHAR;
                ViewBag.ID_LOCA = pc.First().ID_LOCA;
            }
            else {
                var pc = (from a in dbe.PERSON_CONTRACT.Where(x => x.ID_PERS_ENTI == id && x.LAS_CONT == true).ToList()
                          join b in dbe.CHARTs on a.ID_CHAR equals b.ID_CHAR
                          join c in dbe.NAME_CHART on b.ID_NAM_CHAR equals c.ID_NAM_CHAR
                          join d in dbe.PERSON_LOCATION on a.ID_PERS_LOCA equals d.ID_PERS_LOCA into ld
                          from xd in ld.DefaultIfEmpty()
                          select new
                          {
                              a.STAR_DATE,
                              a.END_DATE,
                              a.GROSS_SALARY,
                              a.ID_COND_CONT,
                              a.ID_CHAR,
                              c.NAM_CHAR,
                              ID_LOCA = (xd == null ? 0 : xd.ID_LOCA),
                          });

                DateTime ed = Convert.ToDateTime(pc.First().END_DATE);
                ed = ed.AddDays(1);

                ViewBag.STAR_DATE = (pc.First().STAR_DATE == null ? "" : String.Format("{0:d}", pc.First().STAR_DATE) + " 12:00 AM");
                ViewBag.END_DATE = (pc.First().END_DATE == null ? "" : String.Format("{0:d}", ed) + " 12:00 AM");
                ViewBag.GROSS_SALARY = pc.First().GROSS_SALARY; // string.Format("{0:N2}", pc.First().GROSS_SALARY);
                ViewBag.ID_COND_CONT = pc.First().ID_COND_CONT;
                ViewBag.ID_CHAR = pc.First().ID_CHAR;
                ViewBag.NAM_CHAR = pc.First().NAM_CHAR;
                ViewBag.ID_LOCA = pc.First().ID_LOCA;
            }
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FormNewContract(IEnumerable<HttpPostedFileBase> filesNewCont, PERSON_CONTRACT PersCont)
        {
            string acc = Convert.ToString(Request.Form["ACCION_HF"].ToString());            

            var pc = new PERSON_CONTRACT();
            DateTime STAR_DATE, END_DATE;
            int sw = 0, Error = 0, ID_COND_CONT, ID_LOCA = 0, ID_CHAR = 0;
            decimal salary = 0;
            ID_CHAR = Convert.ToInt32(Request.Form["ID_CHAR"]);
            
            if (DateTime.TryParse(Convert.ToString(Request.Form["STAR_DATE"]), out STAR_DATE) == false) { sw = 1; }
            else if (DateTime.TryParse(Convert.ToString(Request.Form["END_DATE"]), out END_DATE) == false) { sw = 1; }
            else if (STAR_DATE > END_DATE) { sw = 1; Error = 2; }            
            else if (Int32.TryParse(Convert.ToString(Request.Form["ID_COND_CONT"]), out ID_COND_CONT) == false) { sw = 1; }            
            else if (ID_CHAR == 0) { sw = 1; }
            else if (Int32.TryParse(Convert.ToString(Request.Form["ID_LOCA"]), out ID_LOCA) == false) { sw = 1; }
            //else if (filesNewCont == null) { sw = 1; Error = 3; }
            //else if (Request.Form["GROSS_SALARY"] != null || Request.Form["GROSS_SALARY"] != ""){salary = 0;}            

            else
            {
                pc.STAR_DATE = STAR_DATE;
                pc.END_DATE = END_DATE;
                pc.GROSS_SALARY = salary;
                pc.ID_COND_CONT = ID_COND_CONT;
                pc.ID_CHAR = ID_CHAR;
            }
            if (sw == 1)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','" + Error + "');}window.onload=init;</script>");
            }
            else
            {
                if (Request.Form["GROSS_SALARY"] == null || Request.Form["GROSS_SALARY"] == "")
                {
                    salary = 0;
                }
                else
                {
                    salary = Convert.ToDecimal(Request.Form["GROSS_SALARY"]);
                }

                //Guardando nuevo contrato              
                int ID_PERS_ENTI = Convert.ToInt32(Request.Form["ID_PERS_ENTI_FNC"].ToString());

                try
                {
                    //Actualizando el ultimo contrato del personal
                    var LastCont = new PERSON_CONTRACT();
                    var utlCtt = dbe.PERSON_CONTRACT.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI && x.LAS_CONT == true);
                    if (utlCtt.Count() > 0)
                    {
                        LastCont = utlCtt.First();
                        var bi = dbe.EMAIL_BOSS_INMSUP(ID_PERS_ENTI).ToList();
                        if (bi.Count() > 0) //Registrando el cargo y jefe inmediato superior
                        {
                            LastCont.ID_INME_BOSS_LAST = bi.First().ID_PERS_ENTI;
                            LastCont.ID_CHAR_LAST = bi.First().ID_CHAR;
                        }
                        pc.ID_PERS_STAT = LastCont.ID_PERS_STAT;
                        LastCont.LAS_CONT = false;
                        LastCont.VIG_CONT = false;                        
                        dbe.PERSON_CONTRACT.Attach(LastCont);
                        dbe.Entry(LastCont).State = EntityState.Modified;
                        dbe.SaveChanges();
                    }
                    else {
                        pc.ID_PERS_STAT = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == ID_PERS_ENTI).ID_PERS_STAT.Value;
                    }

                    //Actualizando Site-Location
                    var pl = dbe.PERSON_LOCATION.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI && x.END_DATE == null);
                    ID_LOCA = Convert.ToInt32(Request.Form["ID_LOCA"].ToString());
                    if (pl.Count() > 0) {
                        var plFirst = pl.First();
                        pc.ID_PERS_LOCA = plFirst.ID_PERS_LOCA;
                        if (plFirst.ID_LOCA != ID_LOCA)
                        {
                            plFirst.END_DATE = DateTime.Now;
                            plFirst.VIG_LOCA = false;
                            dbe.PERSON_LOCATION.Attach(plFirst);
                            dbe.Entry(plFirst).State = EntityState.Modified;
                            dbe.SaveChanges();

                            var NewPL = new PERSON_LOCATION();
                            NewPL.ID_PERS_ENTI = ID_PERS_ENTI;
                            NewPL.ID_LOCA = ID_LOCA;
                            NewPL.STAR_DATE = DateTime.Now;
                            NewPL.VIG_LOCA = true;

                            dbe.PERSON_LOCATION.Add(NewPL);
                            dbe.SaveChanges();

                            pc.ID_PERS_LOCA = NewPL.ID_PERS_LOCA;
                        }                    
                    }


                    //Registrando nuevo contrato
                    if (acc == "NewAdde") { pc.ID_PERS_CONT_PARENT =(LastCont.ID_PERS_CONT_PARENT == null ? LastCont.ID_PERS_CONT:LastCont.ID_PERS_CONT_PARENT); }
                    pc.ID_PERS_ENTI = ID_PERS_ENTI;
                    pc.ID_WORK_PERI = LastCont.ID_WORK_PERI;
                    pc.LAS_CONT = true;
                    pc.VIG_CONT = true;
                    //nuevos campos
                    pc.STAR_DATE = Convert.ToDateTime(Request.Form["STAR_DATE"]);
                    pc.END_DATE = Convert.ToDateTime(Request.Form["END_DATE"]);
                    pc.GROSS_SALARY = salary; 
                    pc.ID_COND_CONT = Convert.ToInt32(Request.Form["ID_COND_CONT"]);
                    pc.ID_CHAR = ID_CHAR;

                    dbe.PERSON_CONTRACT.Add(pc);
                    dbe.SaveChanges();
                    
                    int ID_PERS_CONT = Convert.ToInt32(pc.ID_PERS_CONT);

                    //Registrado PERSON_CONTRACT_CHART                    
                    var pcch = new PERSON_CONTRACT_CHART();
                    pcch.ID_PERS_CONT = ID_PERS_CONT;
                    pcch.ID_CHAR = ID_CHAR;
                    pcch.IS_CONTRACT = true;
                    pcch.VIG_CONT_CHAR = true;
                    pcch.DAT_STAR = pc.STAR_DATE;

                    dbe.PERSON_CONTRACT_CHART.Add(pcch);
                    dbe.SaveChanges();

                    //Terminando periodo en el cargo (PERSON_CONTRACT_CHART)
                    if (utlCtt.Count() > 0) {
                        var q = dbe.PERSON_CONTRACT_CHART.Single(x => x.IS_CONTRACT == true && x.ID_PERS_CONT == LastCont.ID_PERS_CONT);
                        q.DAT_END = pc.STAR_DATE;
                        q.VIG_CONT_CHAR = false;
                        dbe.Entry(q).State = EntityState.Modified;
                        dbe.SaveChanges();                    
                    }
                    
                    try
                    {
                        var psc = dbe.PERSON_STATUS_CHANGE.Single(x => x.ID_PERS_CONT == LastCont.ID_PERS_CONT);
                        psc.END_DATE = DateTime.Now;
                        dbe.PERSON_STATUS_CHANGE.Attach(psc);
                        dbe.Entry(psc).State = EntityState.Modified;
                        dbe.SaveChanges();
                    }
                    catch { }

                    var PerSC = new PERSON_STATUS_CHANGE();
                    PerSC.ID_PERS_ENTI = ID_PERS_ENTI;
                    PerSC.ID_PERS_CONT = ID_PERS_CONT;
                    PerSC.ID_PERS_STAT = pc.ID_PERS_STAT;
                    PerSC.STAR_DATE = DateTime.Now;
                    PerSC.UserId = Convert.ToInt32(Session["UserId"]);

                    dbe.PERSON_STATUS_CHANGE.Add(PerSC);
                    dbe.SaveChanges();


                    //PERSON_DOCUMENTS atta = dbe.PERSON_DOCUMENTS.Find(id);

                    //Guardando archivo adjunto
                    foreach (var file in filesNewCont)
                    {
                        try
                        {
                            var ATTA = new PERSON_DOCUMENTS();
                            ATTA.NAM_ATTA = Path.GetFileNameWithoutExtension(file.FileName);
                            ATTA.EXT_ATTA = Path.GetExtension(file.FileName);
                            ATTA.ID_PERS_ENTI = ID_PERS_ENTI;
                            ATTA.CREATE_ATTA = DateTime.Now;
                            ATTA.ID_TYPE_DOCU = 2;
                            ATTA.ID_PERS_CONT = ID_PERS_CONT;
                            ATTA.UserId = Convert.ToInt32(Session["UserId"]);

                            dbe.PERSON_DOCUMENTS.Add(ATTA);
                            dbe.SaveChanges();

                            file.SaveAs(Server.MapPath("~/Adjuntos/Talent/Documents/" + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_PERS_DOCU) + ATTA.EXT_ATTA));
                        }
                        catch
                        {
                            return Content("<script type='text/javascript'> function init() {" +
                                "if(top.uploadDone) top.uploadDone('ERROR','1');}window.onload=init;</script>");
                        }
                    }
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','NewCont|" + ID_PERS_CONT + "');}window.onload=init;</script>"); 
                }
                catch
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','1');}window.onload=init;</script>");
                }
            }
        }

        public int ID_USB(int id)
        {
            int usb = 0;
            try
            {
                var query = dbe.AREAs.Where(x => x.ID_AREA == id).First();
                if (query.NIV_AREA == 2)
                {
                    usb = query.ID_AREA;
                }
                else if (query.NIV_AREA == 3)
                {
                    var query1 = dbe.AREAs.Where(x => x.ID_AREA == query.ID_AREA_PARENT).First();
                    usb = query1.ID_AREA;
                }
                else if (query.NIV_AREA == 4)
                {
                    var query1 = dbe.AREAs.Where(x => x.ID_AREA == query.ID_AREA_PARENT)
                                .Join(dbe.AREAs, x => x.ID_AREA_PARENT, y => y.ID_AREA, (x, y) => new
                                {
                                    y.ID_AREA
                                }).First();

                    usb = query1.ID_AREA;
                }
            }
            catch { }

            return usb;
        }

        public string Delete(int id = 0)
        {
            string msn = "ERROR";
            try
            {
                PERSON_DOCUMENTS pdoc = dbe.PERSON_DOCUMENTS.Single(x=>x.ID_PERS_CONT == id);
                PERSON_STATUS_CHANGE psch = dbe.PERSON_STATUS_CHANGE.Single(x => x.ID_PERS_CONT == id);
                try
                {
                    string arc = pdoc.NAM_ATTA + "_" + Convert.ToString(pdoc.ID_PERS_DOCU) + pdoc.EXT_ATTA;
                    dbe.PERSON_DOCUMENTS.Remove(pdoc);
                    dbe.PERSON_STATUS_CHANGE.Remove(psch);
                    PERSON_CONTRACT pcLast = dbe.PERSON_CONTRACT.Find(id);

                    int ID_PERS_ENTI = pcLast.ID_PERS_ENTI.Value;
                    dbe.PERSON_CONTRACT.Remove(pcLast);
                    dbe.SaveChanges();

                    //Eliminando File
                    System.IO.File.Delete(Server.MapPath("~/Adjuntos/Talent/Documents/" + arc));

                    try
                    {
                        PERSON_CONTRACT pcNewLast = dbe.PERSON_CONTRACT.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI).
                            OrderByDescending(x => x.ID_PERS_CONT).First();
                        pcNewLast.VIG_CONT = true;
                        pcNewLast.LAS_CONT = true;
                        dbe.PERSON_CONTRACT.Attach(pcNewLast);
                        dbe.Entry(pcNewLast).State = EntityState.Modified;

                        PERSON_STATUS_CHANGE pdcNewLast = dbe.PERSON_STATUS_CHANGE.Single(X => X.ID_PERS_CONT == pcNewLast.ID_PERS_CONT);
                        pdcNewLast.END_DATE = null;
                        dbe.PERSON_STATUS_CHANGE.Attach(pdcNewLast);
                        dbe.Entry(pdcNewLast).State = EntityState.Modified;
                                                
                        dbe.SaveChanges();
                        msn = "OK|" + pdcNewLast.ID_PERS_CONT.ToString();
                    }
                    catch
                    {
                        return msn;
                    }
                }
                catch
                {
                    return msn;
                }
                return msn;
            }
            catch
            {
                return msn;
            }

        }

    }
}
