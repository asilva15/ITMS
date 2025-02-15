using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;
using ERPElectrodata.Models;
using System.Globalization;
using System.Threading;
using System.Data.Entity;
using System.IO;
using ERPElectrodata.Objects;
using System.Text;

namespace ERPElectrodata.Controllers
{
    public class EvaluationStaffController : Controller
    {
        public CoreEntities dbc = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();
        private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        private TextInfo textInfo;
        private int raw = 0;
        
        // GET: /EvaluationStaff/
        public ActionResult Index(int id = 0)
        {
            ViewBag.RETURN = 0;

            int ID_PERS_ENTI = 0;

            if (id == 0)
            {
                ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            }
            else
            {
                ID_PERS_ENTI = id;
                ViewBag.RETURN = 1;
            }
            
            try{
                var x = (from ee in dbe.EVALUATION_STAFF
                             join ev in dbe.EVALUATIONs on ee.ID_EVAL equals ev.ID_EVAL
                         where ee.ID_PERS_ENTI == ID_PERS_ENTI && ev.ID_STAT_EVAL == 2
                             select new{
                                 ee.ID_EVAL_STAF,
                                 NAM_EVAL = ev.NAM_EVAL,
                             }).First();

                ViewBag.ID_EVAL_STAF = x.ID_EVAL_STAF;
                ViewBag.NAME_EVAL = x.NAM_EVAL;

                return View();
            }
            catch{
                return Content("Not Evaluation Available");
            }
            
        }

        public ActionResult GPARS(int id)
        {
            textInfo = cultureInfo.TextInfo;

            //int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            // ID_PERS_ENTI = 1069;
            //string username = WebSecurity.CurrentUserName;
            

            var location = (from l in dbc.LOCATIONs.ToList()
                            select new
                            {
                                l.ID_LOCA,
                                NAM_LOCA = textInfo.ToTitleCase(l.NAM_LOCA.ToLower()),

                            }).ToList();

            //var location = dbc.LOCATIONs.ToList() ;

            var query = dbe.EVALUATION_STAFF
                .Where(x => x.ID_EVAL_STAF == id);
                //.Take(1)//.OrderBy(x => x.ID_EVAL_STAF).Select(p => new { p.ID_PERS_ENTI,p.ID_LOCA})
                //.ToList();

            //var query_x = dbe.EVALUATION_STAFF.Single(x => x.ID_EVAL_STAF == id);

            var result2 = (from ee in query
                         join pe in dbe.PERSON_ENTITY on ee.ID_PERS_ENTI equals pe.ID_PERS_ENTI
                          join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                          join ps in dbe.PERSON_STATUS on pe.ID_PERS_STAT equals ps.ID_PERS_STAT
                          join nc in dbe.NAME_CHART on ee.ID_NAME_CHAR equals nc.ID_NAM_CHAR
                          join pes in dbe.PERSON_ENTITY on ee.ID_PERS_ENTI_SUPE equals pes.ID_PERS_ENTI
                          join ces in dbe.CLASS_ENTITY on pes.ID_ENTI2 equals ces.ID_ENTI
                          join peuen in dbe.PERSON_ENTITY on ee.ID_PERS_ENTI_UEN equals peuen.ID_PERS_ENTI
                          join ceuen in dbe.CLASS_ENTITY on peuen.ID_ENTI2 equals ceuen.ID_ENTI
                          join peg in dbe.PERSON_ENTITY on ee.ID_PERS_ENTI_MAGE_GENE equals peg.ID_PERS_ENTI
                          join ceg in dbe.CLASS_ENTITY on peg.ID_ENTI2 equals ceg.ID_ENTI
                         //join loca in location on ee.ID_LOCA equals loca.ID_LOCA
                          join uen in dbe.NAME_CHART on ee.ID_NAME_CHAR_UEN equals uen.ID_NAM_CHAR
                          join area in dbe.NAME_CHART on ee.ID_NAME_CHAR_AREA equals area.ID_NAM_CHAR

                         select new
                             {
                                 EVA_TIME = ee.EVA_TIME,
                                 NUM_EMPL_EVAL = ee.NUM_EMPL_EVAL,
                                 NUM_EMPL_UEN = ee.NUM_EMPL_EVAL_UEN,
                                 NUM_EMPL_AREA = ee.NUM_EMPL_EVAL_AREA,
                                 NAM_EMPL = ce.FIR_NAME+" "+ce.LAS_NAME,
                                 COD_EMPL = ee.COD_EVAL_STAF,
                                 NAM_STAT = ps.NAM_STAT,
                                 JOB_TITL = nc.NAM_CHAR_ENGL,
                                 NAM_SUPER = ces.FIR_NAME + " " + ces.LAS_NAME,
                                 NAM_GER_UEN = ceuen.FIR_NAME + " " + ceuen.LAS_NAME,
                                 NAM_GERE_GENE = ceg.FIR_NAME + " " + ceg.LAS_NAME,
                                 NAM_UEN = uen.NAM_CHAR_ENGL,
                                 NAM_AREA = area.NAM_CHAR_ENGL,
                                 ID_FOTO = pe.ID_FOTO,
                                 FOT_SUPE = pes.ID_FOTO,
                                 FOT_UEN = peuen.ID_FOTO,
                                 FOT_GERE_GENE = peg.ID_FOTO,
                                 USER_ELEC = "",
                                 ID_LOCA = ee.ID_LOCA
                             }).ToList();

            var result = (from l in location
                          join p in result2 on l.ID_LOCA equals p.ID_LOCA
                          select new
                          {
                              EVA_TIME = p.EVA_TIME,
                              NUM_EMPL_EVAL = p.NUM_EMPL_EVAL,
                              NUM_EMPL_UEN = p.NUM_EMPL_UEN,
                              NUM_EMPL_AREA = p.NUM_EMPL_AREA,
                              NAM_EMPL = textInfo.ToTitleCase(p.NAM_EMPL.ToLower()),
                              COD_EMPL = p.COD_EMPL,
                              NAM_STAT = textInfo.ToTitleCase(p.NAM_STAT.ToLower()),
                              JOB_TITL = p.JOB_TITL,
                              NAM_SUPER = textInfo.ToTitleCase(p.NAM_SUPER.ToLower()),
                              NAM_GER_UEN = textInfo.ToTitleCase(p.NAM_GER_UEN.ToLower()),
                              NAM_GERE_GENE = textInfo.ToTitleCase(p.NAM_GERE_GENE.ToLower()),
                              NAM_LOCA = l.NAM_LOCA,
                              NAM_UEN = p.NAM_UEN,
                              NAM_AREA = p.NAM_AREA,
                              ID_FOTO = p.ID_FOTO,
                              FOT_SUPE = p.FOT_SUPE,
                              FOT_UEN = p.FOT_UEN,
                              FOT_GERE_GENE = p.FOT_GERE_GENE,
                              USER_ELEC = "username",//username,
                          });

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }


        //public ActionResult ObjectBySatff()
        //{
        //    textInfo = cultureInfo.TextInfo;

        //    //-------Estos Valores son lo que debe recibir.-----------

        //    int ID_PERS_ENTI = 10491;
        //    int ID_EVAL = 2;

        //    var query = (from oe in dbe.OBJECT_BY_STAFF.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI).ToList()
        //                 join o in dbe.OBJETIVES.Where(y => y.ID_EVAL == ID_EVAL) on oe.ID_OBJE equals o.ID_OBJE
        //                 join e in dbe.EVALUATIONs on o.ID_EVAL equals e.ID_EVAL
        //                 join pe in dbe.PERSON_ENTITY on oe.ID_PERS_ENTI equals pe.ID_PERS_ENTI
        //                 join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
        //                 select new
        //                 {
        //                     NAM_EVAL = e.NAM_EVAL,
        //                     DAT_STAR = String.Format("{0:d}", e.DAT_START),
        //                     DAT_END = String.Format("{0:d}",  e.DAT_END),
        //                     //NUM_DAYS = (e.DAT_END - e.DAT_START),
        //                     COD_OBJE = oe.COD_OBJE_STAF,
        //                     NAM_OBJE = o.NAM_OBJE,
        //                     NAM_EMPL = textInfo.ToTitleCase((ce.FIR_NAME.ToLower() + " " + ce.LAS_NAME.ToLower() + " " + (ce.MOT_NAME == null ? "" : ce.MOT_NAME).ToLower())),

        //                 });


        //    return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult ListObjectBySatff()
        //{
        //    return View();
        //}

        public ActionResult SatffBySupervisor()
        {
            textInfo = cultureInfo.TextInfo;

            //-------Estos Valores son lo que debe recibir.-----------

            int ID_PERS_ENTI_SUPE = Convert.ToInt32(Session["ID_PERS_ENTI"]);//507;
            // int ID_EVAL = 2;      
          
            var query1 = (from es in dbe.EVALUATION_STAFF.Where(ev=>ev.ID_PERS_ENTI_SUPE==ID_PERS_ENTI_SUPE).ToList()
                         join pe in dbe.PERSON_ENTITY on es.ID_PERS_ENTI equals pe.ID_PERS_ENTI
                         join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                         join pc in dbe.PERSON_CONTRACT on pe.ID_PERS_ENTI equals pc.ID_PERS_ENTI
                         join c in dbe.CHARTs on pc.ID_CHAR equals c.ID_CHAR
                         join nc in dbe.NAME_CHART on c.ID_NAM_CHAR equals nc.ID_NAM_CHAR
                         where pc.VIG_CONT==true
                         where c.VIG_CHAR==true
                         select new
                         {
                             ID_PERS_ENTI=pe.ID_PERS_ENTI,
                             NAM_EMPL = textInfo.ToTitleCase((ce.FIR_NAME.ToLower() + " " + ce.LAS_NAME.ToLower() + " " + (ce.MOT_NAME == null ? "" : ce.MOT_NAME).ToLower())),
                             ID_PHOTO=pe.ID_FOTO,
                             ID_EVAL_STAF=es.ID_EVAL_STAF,
                             NAM_CHAR = textInfo.ToTitleCase(nc.NAM_CHAR.ToLower()),
                             
                         }).ToList();


            var query2 = (from des in dbe.DETAIL_EVALUATION_STAFF.ToList()
                          join q in query1 on des.ID_EVAL_STAF equals q.ID_EVAL_STAF
                          where des.ID_PERS_ENTI==ID_PERS_ENTI_SUPE
                          group des by des.ID_EVAL_STAF into gdes
                             select new
                             {
                                 ID_EVAL_STAF = gdes.Key,
                                 Comments = gdes.Count(),
                             }).ToList();

            var query=(from q1 in query1
                       join q2 in query2 on q1.ID_EVAL_STAF equals q2.ID_EVAL_STAF into tabl
                       from a in tabl.DefaultIfEmpty() 
                       select new
                       {
                           ID_PERS_ENTI = q1.ID_PERS_ENTI,
                           ID_EVAL_STAF=q1.ID_EVAL_STAF,
                           NAM_EMPL = q1.NAM_EMPL,
                           ID_PHOTO = q1.ID_PHOTO,
                           NAM_CHAR=q1.NAM_CHAR,
                           Comments = (a==null ? 0 : a.Comments),  //a.Comments==null? 0 : a.Comments,                      
                       }).ToList(); 



            if(query.Count()>=0)
           {
                return Json(new { Data=query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Content("You still is not a supervisor");
            }
           
        }

        public ActionResult ListStaffBySupervisor()
        {
            return View();
        }

        //Luis Sempertegui
        public ActionResult ObjectivesByStaff(int id=0)
        {
            int skip = Convert.ToInt32(Request.Params["skip"]);
            int take = Convert.ToInt32(Request.Params["take"]);

            textInfo = cultureInfo.TextInfo;
            int ID_PERS_ENTI_LOG = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            //-------Estos Valores son lo que debe recibir.-----------
           //int ID_EVAL_STAF = 114;
           int ID_EVAL_STAF = id;

            var EVALUATION = dbe.EVALUATION_STAFF.Single(e => e.ID_EVAL_STAF == ID_EVAL_STAF);
            int ID_PERS_ENTI_SUPE = Convert.ToInt32(EVALUATION.ID_PERS_ENTI_SUPE);
            int ID_PERS_ENTI = Convert.ToInt32(EVALUATION.ID_PERS_ENTI);

            var query = dbe.OBJECT_BY_STAFF.Where(x => x.ID_EVAL_STAF == id)
                .Join(dbe.OBJETIVES, x => x.ID_OBJE, y => y.ID_OBJE, (x, y) => new {
                    NAM_OBJE = y.NAM_OBJE,
                    ID_TYPE_OBJE = y.ID_TYPE_OBJE,
                    ID_EVAL = y.ID_EVAL,
                    ID_PERS_ENTI = x.ID_PERS_ENTI,
                    COD_OBJE_STAF = x.COD_OBJE_STAF,
                    VAL_OBJE_STATF = x.VAL_OBJE_STATF,
                    ID_STAT_OBJE = x.ID_STAT_OBJE,
                    ID_OBJE_STAF = x.ID_OBJE_STAF,
                });
            raw = skip;

            var result = (from oe in query.OrderByDescending(x=>x.ID_TYPE_OBJE).Skip(skip).Take(take).ToList()
                         //join o in dbe.OBJETIVES on oe.ID_OBJE equals o.ID_OBJE
                         join tob in dbe.TYPE_OBJECTIVE on oe.ID_TYPE_OBJE equals tob.ID_TYPE_OBJE
                         join e in dbe.EVALUATIONs on oe.ID_EVAL equals e.ID_EVAL
                         join pe in dbe.PERSON_ENTITY on oe.ID_PERS_ENTI equals pe.ID_PERS_ENTI
                         join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                         join pc in dbe.PERSON_CONTRACT on pe.ID_PERS_ENTI equals pc.ID_PERS_ENTI
                         join c in dbe.CHARTs on pc.ID_CHAR equals c.ID_CHAR
                         join nc in dbe.NAME_CHART on c.ID_NAM_CHAR equals nc.ID_NAM_CHAR
                         where pc.VIG_CONT == true
                         where c.VIG_CHAR == true
                         
                         select new
                         {
                             NAM_EVAL = e.NAM_EVAL,
                             DAT_STAR = String.Format("{0:d}", e.DAT_START),
                             DAT_END = String.Format("{0:d}", e.DAT_END),
                             //NUM_DAYS = (e.DAT_END - e.DAT_START),
                             COD_OBJE = oe.COD_OBJE_STAF,
                             NAM_OBJE = oe.NAM_OBJE,
                             NAM_EMPL = textInfo.ToTitleCase((ce.FIR_NAME.ToLower() + " " + ce.LAS_NAME.ToLower() + " " + (ce.MOT_NAME == null ? "" : ce.MOT_NAME).ToLower())),
                             VAL_OBJET = String.Format("{0:P0}", oe.VAL_OBJE_STATF == null ? 0 : oe.VAL_OBJE_STATF),
                             ID_STAT_OBJE = oe.ID_STAT_OBJE,
                             ID_OBJE_STAF=oe.ID_OBJE_STAF,
                             ID_TYPE_OBJE=oe.ID_TYPE_OBJE,
                             NAM_CHAR = textInfo.ToTitleCase(nc.NAM_CHAR.ToLower()),
                             FOTO=pe.ID_FOTO,
                             NAM_TYPE_OBJE = tob.NAM_TYPE_OBJE,
                             RAW = FxRaw(raw),
                         });

            if (ID_PERS_ENTI_LOG == ID_PERS_ENTI)
            {
                return Json(new { Data = result, inspector = 1, Count = query.Count() }, JsonRequestBehavior.AllowGet);
            }

            if (ID_PERS_ENTI_LOG == ID_PERS_ENTI_SUPE)
            {
                return Json(new { Data = result, inspector = 2, Count = query.Count() }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Data = result, inspector = 0, Count = query.Count() }, JsonRequestBehavior.AllowGet);
            }


        }

        public Int32 FxRaw(int x)
        {
            raw = raw + 1;
            return raw;
        }

        public ActionResult ListObjectivesByStaff(int id=0)
        {
            ViewBag.ID_EVAL_STAF = id;
            return View();
        }

        public ActionResult CommentsCompliance()
        {


            int ID_OBJE_STAF = Convert.ToInt32(Request.Params["ID_OBJE_STAF"]);

            textInfo = cultureInfo.TextInfo;
            // var namobjective = dbe.OBJECT_BY_STAFF.Where(os => os.ID_OBJE_STAF == ID_OBJE_STAF).Join(dbe.OBJETIVES, os => os.ID_OBJE, o => o.ID_OBJE, (os, o) => new { o.NAM_OBJE }).First();

            var namstaffobj = (from os in dbe.OBJECT_BY_STAFF.Where(os => os.ID_OBJE_STAF == ID_OBJE_STAF).ToList()
                               join es in dbe.EVALUATION_STAFF on os.ID_EVAL_STAF equals es.ID_EVAL_STAF
                               join pe in dbe.PERSON_ENTITY on es.ID_PERS_ENTI equals pe.ID_PERS_ENTI
                               join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                               join o in dbe.OBJETIVES on os.ID_OBJE equals o.ID_OBJE
                               select new
                               {
                                   NAM_STAFF_OBJE = textInfo.ToTitleCase(((ce.FIR_NAME.ToLower()) + " " + (ce.LAS_NAME.ToLower()) + " " + ((ce.MOT_NAME == null ? "" : ce.MOT_NAME).ToLower()))),
                                   NAM_OBJE= o.NAM_OBJE,
                               }

                                 ).First();

            string input = Convert.ToString(namstaffobj.NAM_OBJE);

            StringBuilder output = new StringBuilder(input.Length);
            bool inATag = false;

            for (var i = 0; i < input.Length; i++)
            {
                if (!inATag && input[i] != '>' && input[i] != '<')
                {
                    output.Append(input[i]);
                }
                else if (input[i] == '<')
                {
                    inATag = true;
                }
                else if (input[i] == '>')
                {
                    inATag = false;
                }
            }

            string result = output.ToString();
            string NAM_OBJE = result;

            string NAM_STAFF_OBJE = namstaffobj.NAM_STAFF_OBJE;
           
            
            var query = (from co in dbe.COMPLIANCE_BY_OBJECTIVES.OrderByDescending(x => x.CREATE_COMP_OBJE).ToList()
                         join pe in dbe.PERSON_ENTITY on co.ID_PERS_ENTI equals pe.ID_PERS_ENTI
                         join ce in dbe.CLASS_ENTITY.ToList() on pe.ID_ENTI2 equals ce.ID_ENTI
                         where co.ID_OBJE_STAF == ID_OBJE_STAF
                         select new
                         {
                             CREATE = String.Format("{0:d}", co.CREATE_COMP_OBJE),
                             NAM_STAFF = textInfo.ToTitleCase(((ce.FIR_NAME.ToLower()) + " " + (ce.LAS_NAME.ToLower()) + " " + ((ce.MOT_NAME == null ? "" : ce.MOT_NAME).ToLower()))),
                             VALUATION = co.VAL_COMP_OBJE,
                             COMMENT = co.COM_COMP_OBJE,
                             PHOTO = pe.ID_FOTO,
                             ADJ = AttaCompliance(co.ID_COMP_OBJE),
                         });

            return Json(new { Data = query, NAM_STAFF_OBJE,NAM_OBJE, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ComplianceObjectivesByStaff(int id = 0)
        {
            int ID_OBJE_STAF = id;
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            // int ID_OBJE_STAF = Convert.ToInt32(Request.Params["ID_OBJE_STAF"]);
            var query = (from os in dbe.OBJECT_BY_STAFF.Where(x => x.ID_OBJE_STAF == ID_OBJE_STAF)
                         join es in dbe.EVALUATION_STAFF on os.ID_EVAL_STAF equals es.ID_EVAL_STAF
                         select new
                         {
                             os.VAL_OBJE_STATF,
                             os.ID_PERS_ENTI,
                             es.ID_PERS_ENTI_SUPE,

                         }).First();

            double val = Convert.ToDouble(query.VAL_OBJE_STATF);
            int ID_PERS_ENTI_OBJE = Convert.ToInt32(query.ID_PERS_ENTI);
            int ID_PERS_ENTI_SUPE = Convert.ToInt32(query.ID_PERS_ENTI_SUPE);


            if (ID_PERS_ENTI == ID_PERS_ENTI_OBJE || ID_PERS_ENTI == ID_PERS_ENTI_SUPE)
            {
                ViewBag.banderaval = 1;
                ViewBag.val = val;
            }
            else
            {
                ViewBag.banderaval = 0;
            }

            ViewBag.ID_OBJE_STAF = ID_OBJE_STAF;
            ViewBag.ID_PERS_ENTI_OBJE = ID_PERS_ENTI_OBJE;

            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SaveComplianceObjectivesByStaff(COMPLIANCE_BY_OBJECTIVES compliace_objetives, IEnumerable<HttpPostedFileBase> files)
        {
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int UserId = Convert.ToInt32(Session["UserId"]);
            int ID_OBJE_STAF = Convert.ToInt32(Request.Params["ID_OBJE_STAF"]);

            var queryx = dbe.OBJECT_BY_STAFF.Where(os => os.ID_OBJE_STAF == ID_OBJE_STAF)
                .Join(dbe.EVALUATION_STAFF, os => os.ID_EVAL_STAF, es => es.ID_EVAL_STAF, (os, es) => new { es.ID_PERS_ENTI, es.ID_PERS_ENTI_SUPE, es.ID_PERS_ENTI_UEN, es.ID_PERS_ENTI_MAGE_GENE, es.ID_PERS_ENTI_RRHH1, es.ID_PERS_ENTI_RRHH2 }).First();

            if (String.IsNullOrEmpty(compliace_objetives.COM_COMP_OBJE))
            {
                // return Content("<script>alert('Ingrese Su Comentario');</script>");
                return Content("<script type='text/javascript'> function init() {" +
            "if(top.uploadDone) top.uploadDone('ERROR','You have not made ​​any comment');}window.onload=init;</script>");
            }
            else
            {

                if (ID_PERS_ENTI == queryx.ID_PERS_ENTI)
                {
                    compliace_objetives.ID_OBJE_STAF = ID_OBJE_STAF;
                    compliace_objetives.CREATE_COMP_OBJE = DateTime.Now;
                    compliace_objetives.ID_PERS_ENTI = ID_PERS_ENTI;
                    compliace_objetives.ID_TYPE_COMP = 2;

                    dbe.COMPLIANCE_BY_OBJECTIVES.Add(compliace_objetives);
                    dbe.SaveChanges();

                    var qos = dbe.OBJECT_BY_STAFF.Find(compliace_objetives.ID_OBJE_STAF);
                    qos.VAL_OBJE_STATF = compliace_objetives.VAL_COMP_OBJE;                   

                    dbe.OBJECT_BY_STAFF.Attach(qos);
                    dbe.Entry(qos).State = EntityState.Modified;
                    dbe.SaveChanges();

                    //if (compliace_objetives.COM_COMP_OBJE != null)
                    //{
                    //    int ID_PERS_ENTI_SUPE = Convert.ToInt32(queryx.ID_PERS_ENTI_SUPE);
                    //    int ID_COMP_OBJE = Convert.ToInt32(compliace_objetives.ID_COMP_OBJE);

                    //    SendMail mail = new SendMail();
                    //    mail.UpdateCommetsObjectives(ID_PERS_ENTI_SUPE, ID_COMP_OBJE);

                    //}

                }

                if (ID_PERS_ENTI == queryx.ID_PERS_ENTI_SUPE)
                {
                    int ID_PERS_ENTI_OBJE = Convert.ToInt32(Request.Params["ID_PERS_ENTI_OBJE"]);

                    var query = dbe.COMPLIANCE_BY_OBJECTIVES.Where(c => c.ID_OBJE_STAF == ID_OBJE_STAF).OrderByDescending(x => x.CREATE_COMP_OBJE).Where(y => y.ID_PERS_ENTI == ID_PERS_ENTI_OBJE).FirstOrDefault();

                    int ID_COMP_OBJE_PARE = 0;

                    if (query != null)
                    {
                        ID_COMP_OBJE_PARE = Convert.ToInt32(query.ID_COMP_OBJE);
                    }

                    compliace_objetives.ID_OBJE_STAF = ID_OBJE_STAF;
                    compliace_objetives.ID_COMP_OBJE_PARE = ID_COMP_OBJE_PARE;
                    compliace_objetives.CREATE_COMP_OBJE = DateTime.Now;
                    compliace_objetives.ID_PERS_ENTI = ID_PERS_ENTI;
                    compliace_objetives.ID_TYPE_COMP = 3;

                    dbe.COMPLIANCE_BY_OBJECTIVES.Add(compliace_objetives);
                    dbe.SaveChanges();

                    var qos = dbe.OBJECT_BY_STAFF.Find(compliace_objetives.ID_OBJE_STAF);
                    qos.VAL_OBJE_STATF = compliace_objetives.VAL_COMP_OBJE;
                    qos.ID_STAT_OBJE = 4;

                    dbe.OBJECT_BY_STAFF.Attach(qos);
                    dbe.Entry(qos).State = EntityState.Modified;
                    dbe.SaveChanges();

                    //if (compliace_objetives.COM_COMP_OBJE != null)
                    //{
                    //    int ID_PERS_ENTI_EVAL = Convert.ToInt32(queryx.ID_PERS_ENTI);
                    //    int ID_COMP_OBJE = Convert.ToInt32(compliace_objetives.ID_COMP_OBJE);

                    //    SendMail mail = new SendMail();
                    //    mail.UpdateCommetsObjectives(ID_PERS_ENTI_EVAL, ID_COMP_OBJE);

                    //}
                }

                if (ID_PERS_ENTI != queryx.ID_PERS_ENTI)
                {
                    if (ID_PERS_ENTI != queryx.ID_PERS_ENTI_SUPE)
                    {
                        compliace_objetives.ID_OBJE_STAF = ID_OBJE_STAF;
                        compliace_objetives.CREATE_COMP_OBJE = DateTime.Now;
                        compliace_objetives.ID_PERS_ENTI = ID_PERS_ENTI;
                        compliace_objetives.ID_TYPE_COMP = 1;

                        dbe.COMPLIANCE_BY_OBJECTIVES.Add(compliace_objetives);
                        dbe.SaveChanges();
                    }
                }

                if (files != null)
                {
                    foreach (var file in files)
                    {
                        try
                        {
                            var ATTA = new ATTACH_EVALUATION_STAFF();

                            ATTA.NAM_ATTA = Path.GetFileNameWithoutExtension(file.FileName);
                            ATTA.EXT_ATTA = Path.GetExtension(file.FileName);
                            ATTA.CREATE_ATTA = DateTime.Now;
                            ATTA.ID_PERS_ENTI = ID_PERS_ENTI;
                            ATTA.UserId = UserId;
                            ATTA.ID_COMP_OBJE = compliace_objetives.ID_COMP_OBJE;
                            ATTA.ID_OBJE_STAF = ID_OBJE_STAF;


                            dbe.ATTACH_EVALUATION_STAFF.Add(ATTA);
                            dbe.SaveChanges();

                            file.SaveAs(Server.MapPath("~/Adjuntos/Talent/GPARS/") + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA) + ATTA.EXT_ATTA);
                        }
                        catch
                        {

                        }
                    }
                }


                //return Content("OK");
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','Saved Succesfully...');}window.onload=init;</script>");
            }
        }

        public string AttaCompliance(int id)
        {
            string Adjun = "";
            try
            {
                var query = dbe.ATTACH_EVALUATION_STAFF.Where(a => a.ID_COMP_OBJE == id);
                foreach (ATTACH_EVALUATION_STAFF subqu in query)
                {
                    Adjun = Adjun + "<li><a href='../../Adjuntos/Talent/GPARS/" + subqu.NAM_ATTA + "_" + subqu.ID_ATTA + subqu.EXT_ATTA + " ' TARGET='_BLANK'>" + subqu.NAM_ATTA + subqu.EXT_ATTA + "</a></li>";
                }

                return Adjun;
            }
            catch
            {
                return String.Empty;
            }
        }
        
    }
}
