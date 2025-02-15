using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.Globalization;
using System.Data.Entity;

namespace ERPElectrodata.Controllers
{
    public class TalentChartController : Controller
    {
        EntityEntities dbe = new EntityEntities();
        CoreEntities dbc = new CoreEntities();

        //
        // GET: /TalentChart/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewTreeChart()
        {
            return View();
        }

        public ActionResult TreeChart()
        {
            int ID_PARA = Convert.ToInt32(Request.Params["ID_PARA"]);
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            if (ID_PARA == 0)
            {
                var query = (from a in dbe.CHARTs.Where(x => x.ID_CHAR_PARE == null && x.ID_ACCO == ID_ACCO)
                             join b in dbe.NAME_CHART on a.ID_NAM_CHAR equals b.ID_NAM_CHAR
                             join c in dbe.TYPE_CHART on b.ID_TYPE_CHAR equals c.ID_TYPE_CHAR
                             select new
                             {
                                 ID_PARA = a.ID_CHAR,
                                 NAME_PARA = b.NAM_CHAR.ToLower(),
                                 HAS_VALUE = true,
                                 expanded = true,
                                 b.ID_TYPE_CHAR,
                                 c.NAM_TYPE,
                                 ICON = (b.ICON == null ? c.ICON : b.ICON),
                                 USER = "-",
                             });

                return Json(query.OrderBy(x => x.NAME_PARA), JsonRequestBehavior.AllowGet);
            }
            else
            {
                var query = (from a in dbe.CHARTs
                                 .Where(x => x.ID_CHAR_PARE == ID_PARA && x.VIG_CHAR == true && x.ID_ACCO == ID_ACCO).ToList()
                             join b in dbe.NAME_CHART on a.ID_NAM_CHAR equals b.ID_NAM_CHAR
                             join c in dbe.TYPE_CHART on b.ID_TYPE_CHAR equals c.ID_TYPE_CHAR
                             join pcch in dbe.PERSON_CONTRACT_CHART.Where(x => x.VIG_CONT_CHAR == true)
                                        on (b.ID_TYPE_CHAR == 3 ? a.ID_CHAR : 0) equals pcch.ID_CHAR into lpcch
                             from xpcch in lpcch.DefaultIfEmpty()
                             join d in dbe.PERSON_CONTRACT.Where(x => x.VIG_CONT == true && x.LAS_CONT == true)
                                        on (xpcch == null ? 0 : xpcch.ID_PERS_CONT) equals d.ID_PERS_CONT into ld
                             from xd in ld.DefaultIfEmpty()
                             join e in dbe.PERSON_ENTITY on (xd == null ? 0 : xd.ID_PERS_ENTI) equals e.ID_PERS_ENTI into le
                             from xe in le.DefaultIfEmpty()
                             join f in dbe.CLASS_ENTITY on (xe == null ? 0 : xe.ID_ENTI2) equals f.ID_ENTI into lf
                             from xf in lf.DefaultIfEmpty()
                             select new
                             {
                                 ID_PARA = a.ID_CHAR,
                                 NAME_PARA = b.NAM_CHAR.ToLower(),
                                 HAS_VALUE = CountSon(Convert.ToInt32(a.ID_CHAR)),
                                 expanded = false,
                                 b.ID_TYPE_CHAR,
                                 c.NAM_TYPE,
                                 ICON = (b.ICON == null ? c.ICON : b.ICON),
                                 USER = (xf == null ? "-" : xf.FIR_NAME + " " + xf.SEC_NAME + " " + xf.LAS_NAME + " " + xf.MOT_NAME).ToLower(),
                             });

                var axxx = query.ToList();


                return Json(query.OrderBy(x => x.NAM_TYPE).ThenBy(x => x.NAME_PARA).ThenBy(x => x.USER), JsonRequestBehavior.AllowGet);
            }
        }

        public Boolean CountSon(int idch = 0)
        {
            int ctd = dbe.CHARTs.Where(x => x.ID_CHAR_PARE == idch).Count();
            if (ctd > 0) { return true; }
            return false;
        }

        public ActionResult ViewChartWithoutStaff(int id = 0, string proc = "", string idx = "")
        {
            ViewBag.ID_CIA = id;
            ViewBag.Procedencia = proc;
            ViewBag.CurIdx = idx;
            return View();
        }

        public ActionResult TreeChartWithoutStaff(int id = 0)
        {
            string ID_CIA = Convert.ToString(id);        
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());

            //if (qAP.Count() > 0)
            //{
            //    ID_ACCO = qAP.Single().ID_ACCO.Value;
            //}


            int ID_PARA = Convert.ToInt32(Request.Params["ID_PARA"]);            

            if (ID_PARA == 0)
            {
                var query = (from a in dbe.CHARTs.Where(x => x.ID_CHAR_PARE == null && x.ID_ACCO == ID_ACCO)
                             join b in dbe.NAME_CHART on a.ID_NAM_CHAR equals b.ID_NAM_CHAR
                             join c in dbe.TYPE_CHART on b.ID_TYPE_CHAR equals c.ID_TYPE_CHAR
                             select new
                             {
                                 ID_PARA = a.ID_CHAR,
                                 ID_CHAR_PARE = 0,
                                 NAME_PARA = b.NAM_CHAR.ToLower(),
                                 HAS_VALUE = true,
                                 expanded = true,
                                 b.ID_NAM_CHAR,
                                 b.ID_TYPE_CHAR,
                                 b.MANAGEMENT,
                                 c.NAM_TYPE,
                                 ICON = (b.ICON == null ? c.ICON : b.ICON),
                             });

                return Json(query.OrderBy(x => x.NAME_PARA), JsonRequestBehavior.AllowGet);
            }
            else
            {
                var query = (from a in dbe.CHARTs.Where(x => x.ID_CHAR_PARE == ID_PARA && x.ID_ACCO == ID_ACCO).ToList()
                             join b in dbe.NAME_CHART on a.ID_NAM_CHAR equals b.ID_NAM_CHAR
                             join c in dbe.TYPE_CHART on b.ID_TYPE_CHAR equals c.ID_TYPE_CHAR
                             select new
                             {
                                 ID_PARA = a.ID_CHAR,
                                 a.ID_CHAR_PARE,
                                 NAME_PARA = b.NAM_CHAR.ToLower(),
                                 HAS_VALUE = CountSon(Convert.ToInt32(a.ID_CHAR)),
                                 HAS_CONTRACT = (b.ID_TYPE_CHAR == 3 ? CountContractByIdChar(Convert.ToInt32(a.ID_CHAR)) : 0) > 0 ? true : false,
                                 expanded = false,
                                 b.ID_NAM_CHAR,
                                 b.ID_TYPE_CHAR,
                                 b.MANAGEMENT,
                                 c.NAM_TYPE,
                                 ICON = (b.ICON == null ? c.ICON : b.ICON),
                                 a.VIG_CHAR,
                             });

                return Json(query.OrderBy(x => x.NAM_TYPE).ThenBy(x => x.NAME_PARA), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult TreeChartWithoutStaffID_ACCO()
        {
            int ID_ACCO = 0;
            ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            int ID_PARA = Convert.ToInt32(Request.Params["ID_PARA"]);

            if (ID_PARA == 0)
            {
                var query = (from a in dbe.CHARTs.Where(x => x.ID_CHAR_PARE == null && x.ID_ACCO == ID_ACCO)
                             join b in dbe.NAME_CHART on a.ID_NAM_CHAR equals b.ID_NAM_CHAR
                             join c in dbe.TYPE_CHART on b.ID_TYPE_CHAR equals c.ID_TYPE_CHAR
                             select new
                             {
                                 ID_PARA = a.ID_CHAR,
                                 ID_CHAR_PARE = 0,
                                 NAME_PARA = b.NAM_CHAR.ToLower(),
                                 HAS_VALUE = true,
                                 expanded = true,
                                 b.ID_NAM_CHAR,
                                 b.ID_TYPE_CHAR,
                                 b.MANAGEMENT,
                                 c.NAM_TYPE,
                                 ICON = (b.ICON == null ? c.ICON : b.ICON),
                             });

                return Json(query.OrderBy(x => x.NAME_PARA), JsonRequestBehavior.AllowGet);
            }
            else
            {
                var query = (from a in dbe.CHARTs.Where(x => x.ID_CHAR_PARE == ID_PARA && x.ID_ACCO == ID_ACCO).ToList()
                             join b in dbe.NAME_CHART on a.ID_NAM_CHAR equals b.ID_NAM_CHAR
                             join c in dbe.TYPE_CHART on b.ID_TYPE_CHAR equals c.ID_TYPE_CHAR
                             select new
                             {
                                 ID_PARA = a.ID_CHAR,
                                 a.ID_CHAR_PARE,
                                 NAME_PARA = b.NAM_CHAR.ToLower(),
                                 HAS_VALUE = CountSon(Convert.ToInt32(a.ID_CHAR)),
                                 expanded = false,
                                 b.ID_NAM_CHAR,
                                 b.ID_TYPE_CHAR,
                                 b.MANAGEMENT,
                                 c.NAM_TYPE,
                                 ICON = (b.ICON == null ? c.ICON : b.ICON),
                             });

                return Json(query.OrderBy(x => x.NAM_TYPE).ThenBy(x => x.NAME_PARA), JsonRequestBehavior.AllowGet);
            }
        }

        public int CountContractByIdChar(int idchar = 0) {
            int tt = 0;
            tt = dbe.PERSON_CONTRACT_CHART.Where(x => x.ID_CHAR == idchar).Count();         
            return tt;
        }

        public ActionResult ListType_Chart() { 
            var result = (from a in dbe.TYPE_CHART
                          select new {
                            a.ID_TYPE_CHAR,
                            a.NAM_TYPE,
                          });
            return Json(new { Data = result, Count = result.Count()}, JsonRequestBehavior.AllowGet);        
        }

        public ActionResult ListNamCharByTypeChart()
        {
            int id = Convert.ToInt32(Request.QueryString["filter[filters][0][value]"]);
            var result = (from a in dbe.NAME_CHART.Where(x=>x.ID_TYPE_CHAR == id).ToList()
                          select new
                          {
                              a.ID_NAM_CHAR,
                              a.ID_TYPE_CHAR,
                              NAM_CHAR = CapitalizeCadena(a.NAM_CHAR),
                          }).OrderBy(x=>x.NAM_CHAR);
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public string CapitalizeCadena(string cadena)
        {
            cadena = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(cadena.ToLower());
            return cadena;
        }
        
        [HttpPost, ValidateInput(false)]
        public ActionResult EditNameChart()
        {
            string S_EDIT_NAM_CHAR = Convert.ToString(Request.Params["EDIT_NAM_CHAR"].ToString()).ToUpper();
            string S_EDIT_TYPE_CHAR = Convert.ToString(Request.Params["EDIT_TYPE_CHAR"].ToString()).ToUpper();

            string S_manaCB = "", S_activeCB = "";
            int ID_CHAR_PARE = 0, ID_NAM_CHAR = 0, sw = 0;

            try { S_manaCB = Convert.ToString(Request.Params["manaCB"].ToString()); }
            catch { }
            if (S_EDIT_NAM_CHAR.Trim().Length == 0) { sw = 1; }
            else if (S_EDIT_TYPE_CHAR.Trim().Length == 0) { sw = 1; }
            else if (Int32.TryParse(Convert.ToString(Request.Params["EDIT_ID_NAM_CHAR"]), out ID_NAM_CHAR) == false) { sw = 1; }
            else if (Int32.TryParse(Convert.ToString(Request.Params["EDIT_ID_CHAR_PARE"]), out ID_CHAR_PARE) == false) { sw = 1; }
            if (sw == 1)
            {
                //return Content("<script type='text/javascript'> function init() {" +
                //    "if(top.uploadDone) top.uploadDoneChart('ERROR','0');}window.onload=init;</script>");

                return Content("<script type='text/javascript'>function init() {" +
                    "if(top.mensaje){ top.mensaje('ERROR','Ingrese los datos correctos.');}}window.onload=init;</script>");
            }
            try { S_activeCB = Convert.ToString(Request.Params["activeCB"].ToString()); }
            catch { }

            var nch = dbe.NAME_CHART.Find(ID_NAM_CHAR);
            nch.NAM_CHAR = S_EDIT_NAM_CHAR;
            nch.MANAGEMENT = S_manaCB.ToLower() == "on" ? true : false;
            nch.VIG_CHAR = S_activeCB.ToLower() == "on" ? true : false;

            dbe.NAME_CHART.Attach(nch);
            dbe.Entry(nch).State = EntityState.Modified;
            dbe.SaveChanges();

            int ID_CHAR = Convert.ToInt32(Request.Params["ED_ID_CHAR"]);
            if (ID_CHAR != 0) {
                var ch = dbe.CHARTs.Find(ID_CHAR);
                ch.VIG_CHAR = nch.VIG_CHAR;
                dbe.CHARTs.Attach(ch);
                dbe.Entry(ch).State = EntityState.Modified;
                dbe.SaveChanges();
            }

            //return Content("<script type='text/javascript'> function init() {" +
            //    "if(top.uploadDone) top.uploadDoneChart('OK','" + ID_CHAR_PARE.ToString() + "');}window.onload=init;</script>");

            return Content("<script type='text/javascript'>function init() {" +
                "if(top.mensaje){ top.mensaje('OK','Se editaron los datos correctamente.');}}window.onload=init;</script>");
        }

        public ActionResult ListNamChartByIdTypeChar() {

            int ID_TYPE_CHAR = 0;
            ID_TYPE_CHAR = Convert.ToInt32(Request.QueryString["filter[filters][0][value]"]);

            var result = (from a in dbe.NAME_CHART.Where(x => x.ID_TYPE_CHAR == ID_TYPE_CHAR).ToList()
                          select new { 
                            a.ID_NAM_CHAR,
                            NAM_CHAR = CapitalizeCadena(a.NAM_CHAR.ToLower()),
                            ICON = (a.ID_TYPE_CHAR == 1 ? "star1.png" :
                                   (a.ID_TYPE_CHAR == 2 ? "star2.png" :
                                   (a.ICON == null ? "Personal.jpeg" : a.ICON))),
                          }).OrderBy(x=>x.NAM_CHAR);

            return Json(new{Data = result, Count = result.Count()}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult NewSubNodeChart() {

            string S_manaCB = "";
            int ID_CHAR = 0, ID_TYPE_CHAR = 0, sw = 0, ind = 0, ID_NAM_CHAR = 0;

            ind = Convert.ToInt32(Request.Params["swNewOrSelect"].ToString());

            if (Int32.TryParse(Convert.ToString(Request.Params["ID_TYPE_CHAR"]), out ID_TYPE_CHAR) == false) { sw = 1; }
            else if (Int32.TryParse(Convert.ToString(Request.Params["SN_ID_CHAR"]), out ID_CHAR) == false) { sw = 1; }
            else if(ind == 1){
                string S_EDIT_NAM_CHAR = Convert.ToString(Request.Params["NAM_CHAR"].ToString()).ToUpper();
                if(S_EDIT_NAM_CHAR.Trim().Length == 0){ sw = 1; }
            }

            if (Request.Params["ID_NAM_CHAR"] == "")
                sw = 1;

            if (sw == 1)
            {
                //return Content("<script type='text/javascript'> function init() {" +
                //    "if(top.uploadDone) top.uploadDoneChart('ERROR','0');}window.onload=init;</script>");
                return Content("<script type='text/javascript'>function init() {" +
                    "if(top.mensaje){ top.mensaje('ERROR','Ingrese los datos correctos.');}}window.onload=init;</script>");
            }

            if (ind == 1)
            {
                string S_EDIT_NAM_CHAR = Convert.ToString(Request.Params["NAM_CHAR"].ToString()).ToUpper();
                try { S_manaCB = Convert.ToString(Request.Params["SNmanaCB"].ToString()); }
                catch { }
                string ICON = Convert.ToString(Request.Params["ICON"].ToString());

                var nc = new NAME_CHART();
                nc.ID_TYPE_CHAR = ID_TYPE_CHAR;
                nc.NAM_CHAR = S_EDIT_NAM_CHAR;
                nc.VIG_CHAR = true;
                nc.MANAGEMENT = S_manaCB.ToLower() == "on" ? true : false;
                if (ID_TYPE_CHAR == 3) {
                    nc.ICON = ICON;
                }                

                dbe.NAME_CHART.Add(nc);
                dbe.SaveChanges();

                ID_NAM_CHAR = Convert.ToInt32(nc.ID_NAM_CHAR);
            }
            else {
                ID_NAM_CHAR = Convert.ToInt32(Request.Params["ID_NAM_CHAR"].ToString());
            }

            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            var v_char = new CHART();
            v_char.ID_CHAR_PARE = ID_CHAR;
            v_char.ID_NAM_CHAR = ID_NAM_CHAR;
            v_char.ID_ACCO = ID_ACCO;
            v_char.VIG_CHAR = true;

            dbe.CHARTs.Add(v_char);
            dbe.SaveChanges();

            string ID_CHAR_PARE = Request.Params["SN_ID_CHAR_PARE"].ToString();

            //return Content("<script type='text/javascript'> function init() {" +
            //    "if(top.uploadDone) top.uploadDoneChart('OK','" + ID_CHAR_PARE + "');}window.onload=init;</script>");
            return Content("<script type='text/javascript'>function init() {" +
                "if(top.mensaje){ top.mensaje('OK','Se creó el subnodo.');}}window.onload=init;</script>");
        }

        public string DeleteChart(int id = 0, int id1 = 0)
        {
            int idpare = 0;
            try
            {
                CHART orgcha = dbe.CHARTs.Find(id);
                
                try
                {
                    dbe.CHARTs.Remove(orgcha);
                    dbe.SaveChanges();

                    var org = dbe.CHARTs.Where(x => x.ID_CHAR == id1);
                    if (org.Count()>0) {
                        idpare = (org.First().ID_CHAR_PARE == null ? 0 : Convert.ToInt32(org.First().ID_CHAR_PARE));
                    }
                }
                catch (Exception)
                {
                    return "ERROR";
                }
                return "OK|" + idpare.ToString();
            }
            catch (Exception)
            {
                return "ERROR";
            }

        }

        public ActionResult ListStaffTempReplace(int id = 0) {
            var qStaff = (from a in dbe.PERSON_ENTITY.Where(x => x.ID_ENTI1 == 9 && x.ID_PERS_ENTI != id)
                          join b in dbe.CLASS_ENTITY on a.ID_ENTI2 equals b.ID_ENTI
                          join c in dbe.PERSON_CONTRACT.Where(x => x.LAS_CONT == true && x.VIG_CONT == true)
                                 on a.ID_PERS_ENTI equals c.ID_PERS_ENTI
                          join d in dbe.CHARTs on c.ID_CHAR equals d.ID_CHAR
                          join e in dbe.NAME_CHART on d.ID_NAM_CHAR equals e.ID_NAM_CHAR
                          select new
                          {
                              User = (b.FIR_NAME + " " + (b.SEC_NAME == null ? "" : b.SEC_NAME)  + " " + (b.LAS_NAME == null ? "" : b.LAS_NAME) + " " + (b.MOT_NAME == null ? "" : b.MOT_NAME)).ToLower(),
                              UserUpper = (b.FIR_NAME + " " + (b.SEC_NAME == null ? "" : b.SEC_NAME) + " " + (b.LAS_NAME == null ? "" : b.LAS_NAME) + " " + (b.MOT_NAME == null ? "" : b.MOT_NAME)).ToUpper(),
                              JobTitle = e.NAM_CHAR.ToLower(),
                              ID_PHOT = a.ID_FOTO,
                          }).OrderBy(x=>x.User);

            return Json(new { Data = qStaff, Count = qStaff.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListStaffED()
        {
            var qStaff = (from a in dbe.PERSON_ENTITY.Where(x => x.ID_ENTI1 == 9)
                          join b in dbe.CLASS_ENTITY on a.ID_ENTI2 equals b.ID_ENTI
                          join c in dbe.PERSON_CONTRACT.Where(x => x.LAS_CONT == true && x.VIG_CONT == true)
                                 on a.ID_PERS_ENTI equals c.ID_PERS_ENTI
                          join d in dbe.CHARTs on c.ID_CHAR equals d.ID_CHAR
                          join e in dbe.NAME_CHART on d.ID_NAM_CHAR equals e.ID_NAM_CHAR
                          select new
                          {
                              User = (b.FIR_NAME + " " + (b.SEC_NAME == null ? "" : b.SEC_NAME) + " " + (b.LAS_NAME == null ? "" : b.LAS_NAME) + " " + (b.MOT_NAME == null ? "" : b.MOT_NAME)).ToLower(),
                              UserUpper = (b.FIR_NAME + " " + (b.SEC_NAME == null ? "" : b.SEC_NAME) + " " + (b.LAS_NAME == null ? "" : b.LAS_NAME)+ " " + (b.MOT_NAME == null ? "" : b.MOT_NAME)).ToUpper(),
                              JobTitle = e.NAM_CHAR.ToLower(),
                              ID_PHOT = a.ID_FOTO,
                              a.ID_PERS_ENTI,
                          }).OrderBy(x => x.User);

            return Json(new { Data = qStaff, Count = qStaff.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListStaffEDNotPaymentBallots() {
            int ID_ACCO_MONT = 0;
            int ID_TYPE_PAYM = 0;
            try {
                if (Request.Params["filter[filters][0][field]"].ToString() == "ID_TYPE_PAYM")
                {
                    ID_TYPE_PAYM = Convert.ToInt32(Request.Params["filter[filters][0][value]"].ToString());
                    ID_ACCO_MONT = Convert.ToInt32(Request.Params["filter[filters][1][value]"].ToString());
                }
                else {                    
                    ID_ACCO_MONT = Convert.ToInt32(Request.Params["filter[filters][0][value]"].ToString());
                    ID_TYPE_PAYM = Convert.ToInt32(Request.Params["filter[filters][1][value]"].ToString());
                }

            }
            catch { }

            var qStaff = (from a in dbe.PERSON_ENTITY.Where(x => x.ID_ENTI1 == 9)
                          where !(from a1 in dbe.PERSON_INVOICE.Where(x=>x.ID_ACCO_MONT == ID_ACCO_MONT 
                                    && x.SIGNED == false && x.ID_TYPE_PAYM == ID_TYPE_PAYM)
                                  select a1.ID_PERS_ENTI).Contains(a.ID_PERS_ENTI)
                          join b in dbe.CLASS_ENTITY on a.ID_ENTI2 equals b.ID_ENTI
                          join c in dbe.PERSON_CONTRACT.Where(x => x.LAS_CONT == true && x.VIG_CONT == true)
                                 on a.ID_PERS_ENTI equals c.ID_PERS_ENTI
                          join d in dbe.CHARTs on c.ID_CHAR equals d.ID_CHAR
                          join e in dbe.NAME_CHART on d.ID_NAM_CHAR equals e.ID_NAM_CHAR                          
                          select new
                          {
                              User = (b.FIR_NAME + " " + (b.SEC_NAME == null ? "" : b.SEC_NAME) + " " + (b.LAS_NAME == null ? "" : b.LAS_NAME) + " " + (b.MOT_NAME == null ? "" : b.MOT_NAME)).ToLower(),
                              UserUpper = (b.FIR_NAME + " " + (b.SEC_NAME == null ? "" : b.SEC_NAME) + " " + (b.LAS_NAME == null ? "" : b.LAS_NAME) + " " + (b.MOT_NAME == null ? "" : b.MOT_NAME)).ToUpper(),
                              JobTitle = e.NAM_CHAR.ToLower(),
                              ID_PHOT = a.ID_FOTO,
                              a.ID_PERS_ENTI,
                          }).OrderBy(x => x.User);

            return Json(new { Data = qStaff, Count = qStaff.Count() }, JsonRequestBehavior.AllowGet);
        }
    }
}
