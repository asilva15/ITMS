using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.IO;
using System.Data.Entity;
using System.Globalization;
using System.Threading;

namespace ERPElectrodata.Controllers
{
    public class ProjectController : Controller
    {
        public CoreEntities dbc = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();

        private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        private TextInfo textInfo;

        // GET: /Project/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            ViewBag.DATE = DateTime.Now;
            ViewBag.KeyUploadFile = "PROJ" + Convert.ToString(DateTime.Now.Ticks);

            return View();
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult Create(PROJECT proj)
        {
            int UserId = Convert.ToInt32(Session["UserId"]);

            string KEY_ATTA = Convert.ToString(Request.Params["KEY_ATTA_PROJ"]);

            string DATE_STAR = Convert.ToString(Request.Params["DATE_STAR"].ToString());
            string DATE_END = Convert.ToString(Request.Params["DATE_END"].ToString());

            if (String.IsNullOrEmpty(DATE_STAR))
            {
                return Content("<script type='text/javascript'> function init() {" +
                  "if(top.PopUpProject) top.PopUpProject('ERROR','Enter Start Date for the Project');}window.onload=init;</script>");
            }
            else if (String.IsNullOrEmpty(DATE_END))
            {
                return Content("<script type='text/javascript'> function init() {" +
                  "if(top.PopUpProject) top.PopUpProject('ERROR','Enter Closure Date for the Project');}window.onload=init;</script>");
            }
            else if (String.IsNullOrEmpty(proj.NAM_PROJ))
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.PopUpProject) top.PopUpProject('ERROR','Enter Project Name');}window.onload=init;</script>");
            }
            else if (String.IsNullOrEmpty(proj.DES_PROJ))
            {
                return Content("<script type='text/javascript'> function init() {" +
                   "if(top.PopUpProject) top.PopUpProject('ERROR','Enter a Description for the Project');}window.onload=init;</script>");
            }

            else
            {
                try
                {
                    proj.UserId = UserId;
                    proj.CREATED = DateTime.Now;
                    proj.COD_PROJ = "PED0001";//Se tiene que modificar 
                    dbc.PROJECTs.Add(proj);
                    dbc.SaveChanges();

                    if (KEY_ATTA != null)
                    {
                        var query = dbc.ATTACHED_PROJECT.Where(x => x.KEY_ATTA_PROJ == KEY_ATTA && x.DELETE_ATTA_PROJ == false && x.ID_PROJ==null).ToList();
                        foreach (var x in query)
                        {
                            x.ID_PROJ = proj.ID_PROJ;
                            dbc.ATTACHED_PROJECT.Attach(x);
                            dbc.Entry(x).State = EntityState.Modified;
                            dbc.SaveChanges();
                        }

                    }

                }
                catch (Exception e)
                {
                    string Error = (e.InnerException == null ? e.Message : e.InnerException.Message);

                    return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.PopUpProject) top.PopUpProject('ERROR','" + Error.ToString() + "');}window.onload=init;</script>");
                }

            }

            return Content("<script type='text/javascript'> function init() {" +
                                       "if(top.PopUpProject) top.PopUpProject('OK','Project Created Successfully');}window.onload=init;</script>");

        }

        public ActionResult ListTypeAttaProj()
        {
            var query = (from x in dbc.TYPE_ATTACH_PROJECT
                         select new
                         {
                             x.ID_TYPE_ATTA_PROJ,
                             x.NAM_ATTA_PROJ,
                         });

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListProjects()
        {
            return View();
        }
        public ActionResult DataListProjects()
        {   
            textInfo = cultureInfo.TextInfo;

            //int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            //int take = Convert.ToInt32(Request.Params["take"].ToString());
            //int UserId=Convert.ToInt32(Session["UserId"]);

            var proj = (from p in dbc.PROJECTs.ToList()
                        select new
                        {
                            p.UserId,
                            p.ID_PROJ,
                            p.NAM_PROJ,
                            p.DES_PROJ,
                            p.COD_PROJ,
                            p.CREATED,
                            p.DATE_STAR,
                            p.DATE_END,

                        });

            var data = (from p in proj
                         join ce in dbe.CLASS_ENTITY on p.UserId equals ce.UserId
                         join pe in dbe.PERSON_ENTITY on ce.ID_ENTI equals pe.ID_ENTI2
                         select new
                         {  
                             ce.FIR_NAME,                            
                             ce.LAS_NAME,                           
                             ce.MOT_NAME,
                             pe.ID_PERS_ENTI,
                             p.ID_PROJ,
                             p.NAM_PROJ, 
                             p.DES_PROJ,
                             p.COD_PROJ,
                             p.CREATED,
                             p.DATE_STAR,
                             p.DATE_END,
                             
                         });

            var query = (from d in data.ToList()
                         select new
                         {
                             NAME = textInfo.ToTitleCase((d.FIR_NAME.ToLower() + " " + d.LAS_NAME.ToLower() + " " + (d.MOT_NAME == null ? "" : d.MOT_NAME).ToLower())),
                             d.ID_PERS_ENTI,
                             d.ID_PROJ,
                             d.NAM_PROJ,
                             d.DES_PROJ,
                             CREATED = String.Format("{0:f}", d.CREATED),
                             DATE_STAR = String.Format("{0:d}", d.DATE_STAR),
                             DATE_END = String.Format("{0:d}", d.DATE_END),
                         });



            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);  
        }

        public ActionResult ProjectPlanning(int id=0, int id1=0)
        {
            //id = 1145;
            //id1 = 1;
            ViewBag.ID_PROJ = id;
            ViewBag.ID_PERS_ENTI = id1;
           
            return View();
        }

        public ActionResult TabTeam(int id = 0, int id1 = 0)
        {
            ViewBag.ID_PROJ_TEAM = id;
            ViewBag.ID_PERS_ENTI_TEAM = id1;
            ViewBag.banderateam = 1;

            return View();
        }

        public ActionResult ListTeam(int id = 0, int id1 = 0)
        {
            textInfo = cultureInfo.TextInfo;

            int ID_PROJ = id;
            int ID_PERS_ENTI_PROJ = id1;//Creador del Projecto
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);//El que se ha logueado.

            //Comprueba si el que ingreso pertenece a PMO
            var qisproj = (from pc in dbe.PERSON_CONTRACT.Where(pc => pc.ID_PERS_ENTI == ID_PERS_ENTI && pc.VIG_CONT == true && pc.LAS_CONT == true)
                           join c in dbe.CHARTs on pc.ID_CHAR equals c.ID_CHAR
                           join nc in dbe.NAME_CHART on c.ID_NAM_CHAR equals nc.ID_NAM_CHAR
                           select new
                           {  c.ID_CHAR, c.ID_CHAR_PARE, nc.ID_TYPE_CHAR,
                           }).FirstOrDefault();

            int bandarea = 0;
            int idtypechar=0;
            int idchararea = 0;
            try{ idtypechar=Convert.ToInt32(qisproj.ID_TYPE_CHAR);}catch{idtypechar=2;}
            while (idtypechar != 2)
            {
                var area = (from c in dbe.CHARTs.Where(x => x.ID_CHAR == qisproj.ID_CHAR_PARE)
                            join nc in dbe.NAME_CHART on c.ID_NAM_CHAR equals nc.ID_NAM_CHAR
                            select new
                            {c.ID_CHAR, c.ID_CHAR_PARE, nc.ID_TYPE_CHAR,
                            }).FirstOrDefault();

                try { idchararea = Convert.ToInt32(area.ID_CHAR); } catch { }

                //Si ha creado el proyecto y es PMO puede agregar un lider Técnico
                if (idchararea == 14 && ID_PERS_ENTI_PROJ==ID_PERS_ENTI) { bandarea = 1; }

                qisproj = area;

                try { idtypechar = Convert.ToInt32(qisproj.ID_TYPE_CHAR); } catch { idtypechar = 2; }
            }

            //var q = dbe.Obtiene_Nombre_UEN_AREA_CARGO(ID_PERS_ENTI_PROJ).ToList();
            //var ispmo = q.Where(x => x.ID_CHAR_AREA == 14);
            //int bandera = 0;
            //if (ispmo.Count() == 1) { bandera = 1; }

            //Nombre de quien ha creado el proyecto
            var projmang = (from ce in dbe.CLASS_ENTITY.ToList()
                            join pe in dbe.PERSON_ENTITY on ce.ID_ENTI equals pe.ID_ENTI2
                            where pe.ID_PERS_ENTI == ID_PERS_ENTI_PROJ
                            select new
                            {
                                pe.ID_PERS_ENTI,
                                NAME = textInfo.ToTitleCase((ce.FIR_NAME.ToLower() + " " + ce.LAS_NAME.ToLower() + " " + (ce.MOT_NAME == null ? "" : ce.MOT_NAME).ToLower())),
                            });

            //Equipo Tecnico
            var Tecteam = (from t in dbc.TEAMs.Where(x => x.ID_PROJ == ID_PROJ)
                                 join rt in dbc.ROL_TEAM on t.ID_ROL_TEAM equals rt.ID_ROL_TEAM
                                 select new
                                 {
                                     t.ID_PERS_ENTI,
                                     rt.ID_ROL_TEAM,
                                     rt.NAM_ROL_TEAM,
                                 });

            //Nombre del Equipo Tecnico
              var result = (from ce in dbe.CLASS_ENTITY.ToList()
                          join pe in dbe.PERSON_ENTITY on ce.ID_ENTI equals pe.ID_ENTI2
                          join lt in Tecteam on pe.ID_PERS_ENTI equals lt.ID_PERS_ENTI
                          select new
                          {
                              NAM_TEAM = textInfo.ToTitleCase((ce.FIR_NAME.ToLower() + " " + ce.LAS_NAME.ToLower() + " " + (ce.MOT_NAME == null ? "" : ce.MOT_NAME).ToLower())),
                              lt.NAM_ROL_TEAM,
                              lt.ID_ROL_TEAM,
                          });

            //Si el que se ha logueado es el lider tecnico
              var leaderteam = Tecteam.Where(x => x.ID_ROL_TEAM == 1 && x.ID_PERS_ENTI == ID_PERS_ENTI);
              int bandleaderteam = 0;
              if (leaderteam.Count() == 1)
              {
                  bandleaderteam = 1;
              }

              return Json(new { Data = projmang, Count = projmang.Count(), Equipo = result, Cant = result.Count(), banderapmo = bandarea, banderaleaderteam = bandleaderteam }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Equipo(int id = 0)
        {
            try
            {
                int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
              
                var query=(from ce in dbe.CLASS_ENTITY
                           join pe in dbe.PERSON_ENTITY on ce.ID_ENTI equals pe.ID_ENTI2
                           join pc in dbe.PERSON_CONTRACT on pe.ID_PERS_ENTI equals pc.ID_PERS_ENTI
                           join ae in dbe.ACCOUNT_ENTITY on pe.ID_PERS_ENTI equals ae.ID_PERS_ENTI
                           where pc.LAS_CONT ==true
                           where pc.VIG_CONT==true
                           where ae.ID_ACCO==4
                           where pe.ID_ENTI1==9
                           select new
                           {
                               pe.ID_PERS_ENTI,
                               FIR_NAME = (ce.FIR_NAME.ToUpper() + " " + ce.LAS_NAME.ToUpper()),
                               pe.ID_FOTO,
                       
                           }).OrderBy(x=>x.FIR_NAME); 

                return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { Count = 0, ID_ENTI = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SaveTeam(int id=0, int id1=0, int id2=0)
        {
           
            int ID_PROJ=id;
            int ID_PERS_ENTI = id1;

            int TeamTec = id2;
            
            //Para verificar si existe un lider de proyecto asignado
            var leader = dbc.TEAMs.Where(x => x.ID_PROJ == ID_PROJ && x.ID_ROL_TEAM == 1);

            var repetido = dbc.TEAMs.Where(x=>x.ID_PERS_ENTI==ID_PERS_ENTI && x.ID_PROJ==ID_PROJ);

            int ID_PERS_ENTI_PROJ = Convert.ToInt32(Request.Params["ID_PERS_PROJ"]);

            if (leader.Count()== 0)
            {

                var team = new TEAM();

                team.ID_PERS_ENTI = ID_PERS_ENTI;
                team.ID_PROJ = ID_PROJ;
                team.ID_ROL_TEAM = 1;
                team.DATE_STAR = DateTime.Now;

                dbc.TEAMs.Add(team);
                dbc.SaveChanges();
                return Content("OK");              
            }
            else if (TeamTec==2 && repetido.Count()==0 && ID_PERS_ENTI!=ID_PERS_ENTI_PROJ)
            {
                var team = new TEAM();

                team.ID_PERS_ENTI = ID_PERS_ENTI;
                team.ID_PROJ = ID_PROJ;
                team.ID_ROL_TEAM = 2;
                team.DATE_STAR = DateTime.Now;

                dbc.TEAMs.Add(team);
                dbc.SaveChanges();
                return Content("OK");       
            }
            else
            {
                return Content("ERROR");
            }           
        }
        public ActionResult ListOps(int id=0)     
        {
            ViewBag.ID_PROJ_OP = id;
            ViewBag.Bandera = 1;

            return View();
        }
        public ActionResult DataListOps()
        {
           var query=(from t in dbc.TICKETs.Where(x=>x.ID_DOCU_SALE!=null).Where(x=>x.ID_STAT_END!=6).Where(x=>x.ID_STAT_END!=4)
                      join ds in dbc.DOCUMENT_SALE on t.ID_DOCU_SALE equals ds.ID_DOCU_SALE
                      join tds in dbc.TYPE_DOCUMENT_SALE on ds.ID_TYPE_DOCU_SALE equals tds.ID_TYPE_DOCU_SALE
                      join re in dbc.REQUEST_EXPENSE on t.ID_TICK equals re.ID_TICK
                      select new
                      {
                          t.ID_TICK,
                          t.COD_TICK,
                          t.ID_DOCU_SALE,
                          ds.NUM_DOCU_SALE,
                          tds.COD_TYPE_DOCU_SALE,
                          NAMEOP=(tds.COD_TYPE_DOCU_SALE +" "+ ds.NUM_DOCU_SALE).Trim(),
                         
                      });

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveOPProject(int id=0, int id1=0)
        {
            var opproject = new OP_PROJECT();
            int UserId = Convert.ToInt32(Session["UserId"]);
            int ID_PROJ = id;
            int ID_TICK = id1;
            int repetido=0;

            var query = dbc.OP_PROJECT.Where(x => x.ID_TICK == ID_TICK);

            try {repetido=Convert.ToInt32(query.Count());}catch{repetido=-1;}
            
            if (repetido== 0)
            {

                try
                {
                    opproject.ID_PROJ = ID_PROJ;
                    opproject.ID_TICK = ID_TICK;
                    opproject.UserId = UserId;
                    opproject.CREATED = DateTime.Now;

                    dbc.OP_PROJECT.Add(opproject);
                    dbc.SaveChanges();                  
                }
                catch
                {
                    return Content("ERROR, Could not Add");
                }
            }
            else
            {
                return Content("ERROR, This OP is Already Assigned to a Project");
            }

            return Content("OK, Added Successfully");
        }

        public ActionResult DataListOpsAssiProj( int id=0)
        {
            //int ID_PROJ = id;
            int ID_PROJ = id;

            var data = (from opp in dbc.OP_PROJECT.Where(x => x.ID_PROJ == ID_PROJ)
                         join t in dbc.TICKETs on opp.ID_TICK equals t.ID_TICK
                         join ds in dbc.DOCUMENT_SALE on t.ID_DOCU_SALE equals ds.ID_DOCU_SALE
                         join tds in dbc.TYPE_DOCUMENT_SALE on ds.ID_TYPE_DOCU_SALE equals tds.ID_TYPE_DOCU_SALE
                         join re in dbc.REQUEST_EXPENSE on t.ID_TICK equals re.ID_TICK
                       
                         select new
                         {
                             t.COD_TICK,   
                             re.ID_TICK,
                             NAMEOP = (tds.COD_TYPE_DOCU_SALE + " " + ds.NUM_DOCU_SALE).Trim(),
                             ds.NUM_DOCU_SALE,                            
                             re.AMOUNT,                        
                            // CREATED=String.Format("{0:d}",opp.CREATED), 
                             opp.CREATED, 
                             COIN = (re.CURRENCY == "MN" ? "S/." : "US$"),

                         });

            var query = (from d in data
                         group d by new { d.ID_TICK } into g
                         select new
                         {
                             CANT = g.Count(),
                             ID_TICK = g.Key.ID_TICK,
                             COD_TICK = g.FirstOrDefault().COD_TICK,
                             NAMEOP = g.FirstOrDefault().NAMEOP,
                             NUM_DOCU_SALE = g.FirstOrDefault().NUM_DOCU_SALE,
                             CREATED = g.FirstOrDefault().CREATED,
                             TOTAL = g.Sum(_ => _.AMOUNT),
                             COIN = g.FirstOrDefault().COIN,

                         }).OrderByDescending(x => x.CREATED);

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DataListDeatailsOps()
        {
            textInfo = cultureInfo.TextInfo;

            int ID_TICK = Convert.ToInt32(Request.Params["filter[filters][0][value]"].ToString());

            var data = (from re in dbc.REQUEST_EXPENSE.Where(x => x.ID_TICK == ID_TICK).ToList()
                      //join ds in dbc.DELIVERY_SUSTAIN on re.ID_DELI_SUST equals ds.ID_DELI_SUST 
                      join tds in dbc.TYPE_DELI_SUST on re.ID_TYPE_DELI_SUST equals tds.ID_TYPE_DELI_SUST
                      select new
                      {
                           tds.NAM_TYPE_DELI_SUST,
                           re.JUSTIFICATION,
                           re.ID_PERS_ENTI_REQU,
                           DAT_REGI=String.Format("{0:d}",re.DAT_REGI),
                           re.COD_REQU_EXPE,
                           re.AMOUNT,
                           COIN = (re.CURRENCY == "MN" ? "S/." : "US$"),

                      });

            var query = (from ce in dbe.CLASS_ENTITY.ToList()
                         join pe in dbe.PERSON_ENTITY on ce.ID_ENTI equals pe.ID_ENTI2
                         join d in data on pe.ID_PERS_ENTI equals d.ID_PERS_ENTI_REQU
                         select new
                         {   
                             d.NAM_TYPE_DELI_SUST,
                             d.JUSTIFICATION,
                             d.DAT_REGI,
                             d.COD_REQU_EXPE,
                             d.AMOUNT,
                             d.COIN,
                             NAME = textInfo.ToTitleCase((ce.FIR_NAME.ToLower() + " " + ce.LAS_NAME.ToLower() + " " + (ce.MOT_NAME == null ? "" : ce.MOT_NAME).ToLower())),
                         });


            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        
        }

        public ActionResult CreateHito(int id=0)
       {
           ViewBag.ID_PROJE = id;       
           ViewBag.DATE_HITO = DateTime.Now;  
            return View();
        }

        public ActionResult ListTypeHito()
        {
            var query = (from x in dbc.TYPE_HITO
                         select new
                         {
                             x.ID_TYPE_HITO,
                             x.NAM_TYPE_HITO,
                         });

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SaveHito(HITO hito)
        {
           

                int UserId = Convert.ToInt32(Session["UserId"]);
                int ID_TYPE_HITO = Convert.ToInt32(Request.Params["ID_TYPE_HITO"]);
                int ID_PROJ = Convert.ToInt32(Request.Params["ID_PROJ"]);
                string STAR_DATE = Convert.ToString(Request.Params["STAR_DATE"].ToString());
                //string END_DATE = Convert.ToString(Request.Params["END_DATE"].ToString());
                int repetido = 0;
           
            //if (String.IsNullOrEmpty(hito.NAM_HITO))
            //{
            //    return Content("<script type='text/javascript'> function init() {" +
            //        "if(top.PopUpHito) top.PopUpHito('ERROR','Enter Name of Milestone');}window.onload=init;</script>");
            //} else 

            if (hito.ID_TYPE_HITO==null)
            {
                return Content("<script type='text/javascript'> function init() {" +
                   "if(top.PopUpHito) top.PopUpHito('ERROR','Enter a Type for the Milestone');}window.onload=init;</script>");

            }
            else if (String.IsNullOrEmpty(hito.DES_HITO))
            {
                return Content("<script type='text/javascript'> function init() {" +
                   "if(top.PopUpHito) top.PopUpHito('ERROR','Enter a Description for the Milestone');}window.onload=init;</script>");

            }
            else if (String.IsNullOrEmpty(STAR_DATE))
            {
                return Content("<script type='text/javascript'> function init() {" +
                   "if(top.PopUpHito) top.PopUpHito('ERROR','Enter Star Date for the Milestone');}window.onload=init;</script>");
            }
            //else if (String.IsNullOrEmpty(END_DATE))
            //{
            //    return Content("<script type='text/javascript'> function init() {" +
            //       "if(top.PopUpHito) top.PopUpHito('ERROR','Enter End Date for the Milestone');}window.onload=init;</script>");
            //}

            var query = dbc.HITOes.Where(x => x.ID_PROJ == ID_PROJ && x.ID_TYPE_HITO == ID_TYPE_HITO);
            try { repetido = Convert.ToInt32(query.Count()); } catch { repetido = -1; }

            if (repetido == 0)
            {
                try
                {
                    hito.ID_PROJ = ID_PROJ;
                    hito.UserId = UserId;
                    hito.ID_TYPE_HITO = ID_TYPE_HITO;                 
                    hito.CREATED = DateTime.Now;

                    dbc.HITOes.Add(hito);
                    dbc.SaveChanges();

                }
                catch
                {
                    return Content("<script type='text/javascript'> function init() {" +
                     "if(top.PopUpHito) top.PopUpHito('ERROR','Could not Add');}window.onload=init;</script>");                  
                }
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.PopUpHito) top.PopUpHito('ERROR','Already Added this Type of Milestone');}window.onload=init;</script>");  
                
            }

            return Content("<script type='text/javascript'> function init() {" +
                   "if(top.PopUpHito) top.PopUpHito('OK','Milestone Added Successfully...');}window.onload=init;</script>");       

        }
         public ActionResult ListHito(int id=0)
        {
            ViewBag.ID_PROJ_HITO = id;
            ViewBag.banderaHito = 1;
            return View();
        }
        public ActionResult DataListHito(int id=0)
        {
            //id = 5;
            string fecha = String.Format("{0:d}", DateTime.Now);
            int ID_PROJ = id;
            string stardate="";

            int numdays = 0;

            var q = (from h in dbc.HITOes.Where(x => x.ID_PROJ == id)
                     group h by h.ID_PROJ into g
                    select new 
                    {
                        IdProj = g.Key,
                        Minino = g.Min(t => t.STAR_DATE),
                        Maximo = g.Max(t => t.STAR_DATE),
                    });

            var dateshito = (from x in q.Where(x => x.IdProj == ID_PROJ).ToList()
                             select new
                             {   
                                 STAR_DATE=String.Format("{0:d}",x.Minino),
                                 END_DATE=x.Maximo,
                                 NumDays = (new DateTime(Convert.ToDateTime(x.Maximo).Year, Convert.ToDateTime(x.Maximo).Month, Convert.ToDateTime(x.Maximo).Day) - new DateTime(Convert.ToDateTime(x.Minino).Year, Convert.ToDateTime(x.Minino).Month, Convert.ToDateTime(x.Minino).Day)).TotalDays,
                             }).FirstOrDefault();
            try
            {
                numdays = Convert.ToInt32(dateshito.NumDays);
                stardate = dateshito.STAR_DATE;
            }
            catch { }

            var query = (from h in dbc.HITOes.Where(x => x.ID_PROJ == id).ToList()
                         join th in dbc.TYPE_HITO on h.ID_TYPE_HITO equals th.ID_TYPE_HITO
                         select new
                         {
                             h.NAM_HITO,
                             h.DES_HITO,
                             STAR_DATE = String.Format("{0:d}", h.STAR_DATE),                            
                             th.NAM_TYPE_HITO,
                             h.ID_TYPE_HITO,
                             th.SCA_TYPE_HITO,
                             DAT = h.STAR_DATE,
                         }).OrderBy(x=>x.DAT);

         
           var data = (from opp in dbc.OP_PROJECT.Where(x => x.ID_PROJ == ID_PROJ)
                       join t in dbc.TICKETs on opp.ID_TICK equals t.ID_TICK                      
                       join re in dbc.REQUEST_EXPENSE on t.ID_TICK equals re.ID_TICK
                       select new
                       {
                           re.AMOUNT,
                           re.DAT_REGI,
                           COIN = (re.CURRENCY == "MN" ? "S/." : "US$"),
                       });

           var result = (from d in data.ToList()
                         select new
                         {
                             d.AMOUNT,
                             //DAT_REGI = ((new DateTime(Convert.ToDateTime(d.DAT_REGI).Year, Convert.ToDateTime(d.DAT_REGI).Month, Convert.ToDateTime(d.DAT_REGI).Day) - new DateTime(1970, 1, 1)).TotalSeconds) * 1000,
                             DAT_REGI=String.Format("{0:d}",d.DAT_REGI),
                             d.COIN,
                         }).OrderBy(x => x.DAT_REGI);

           return Json(new { Data = query, Result=result, Count = query.Count(), fecha=fecha,NUM_DAYS=numdays, STAR_DATE=stardate, }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateTask(int id = 0)
        {
            id = 39;
            ViewBag.ID_PROJ_TASK = id;         
            return View();
        }

         [HttpPost, ValidateInput(false)]
        public ActionResult SaveTask(TASK task)
        {
            int UserId = Convert.ToInt32(Session["UserId"]);         
            int ID_PROJ = Convert.ToInt32(Request.Params["ID_PROJ"]);
            int ID_TICK = Convert.ToInt32(Request.Params["ID_TICK"]);
            string ID_TICK_STRING = Convert.ToString(Request.Params["ID_TICK"].ToString());

            string DATE_STAR = Convert.ToString(Request.Params["DATE_STAR"].ToString());
            string DATE_END = Convert.ToString(Request.Params["DATE_END"].ToString());
            int repetido = 0;

            if (String.IsNullOrEmpty(task.NAM_TASK))
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.PopUpTask) top.PopUpTask('ERROR','Enter the Task Name');}window.onload=init;</script>");
            }
            else if (String.IsNullOrEmpty(ID_TICK_STRING))
            {
                return Content("<script type='text/javascript'> function init() {" +
                   "if(top.PopUpTask) top.PopUpTask('ERROR','Select an OP');}window.onload=init;</script>");
            }
            else if (String.IsNullOrEmpty(DATE_STAR))
            {
                return Content("<script type='text/javascript'> function init() {" +
                   "if(top.PopUpTask) top.PopUpTask('ERROR','Enter Star Date for the Task');}window.onload=init;</script>");
            }
            else if (String.IsNullOrEmpty(DATE_END))
            {
                return Content("<script type='text/javascript'> function init() {" +
                   "if(top.PopUpTask) top.PopUpTask('ERROR','Enter End Date for the Task');}window.onload=init;</script>");
            }
          
            else if (String.IsNullOrEmpty(task.DESC_TASK))
            {
                return Content("<script type='text/javascript'> function init() {" +
                   "if(top.PopUpTask) top.PopUpTask('ERROR','Enter a Description for the Task');}window.onload=init;</script>");

            }
           

            //var query = dbc.TASKs.Where(x => x.ID_PROJ == ID_PROJ);
            //try { repetido = Convert.ToInt32(query.Count()); }
            //catch { repetido = -1; }

            //if (repetido == 0)
            //{
                try
                {
                    task.ID_PROJ = ID_PROJ;
                    task.UserId = UserId;                
                    task.CREATED = DateTime.Now;
                    task.ID_TICK = ID_TICK;
                    dbc.TASKs.Add(task);
                    dbc.SaveChanges();

                }
                catch
                {
                    return Content("<script type='text/javascript'> function init() {" +
                     "if(top.PopUpTask) top.PopUpTask('ERROR','Could not Add');}window.onload=init;</script>");
                }
            //}
            //else
            //{
            //    return Content("<script type='text/javascript'> function init() {" +
                //        "if(top.PopUpTask) top.PopUpTask('ERROR','Already Added this ');}window.onload=init;</script>");

            //}

            return Content("<script type='text/javascript'> function init() {" +
                   "if(top.PopUpTask) top.PopUpTask('OK','Task Added Successfully...');}window.onload=init;</script>");

        }

        public ActionResult ListTask(int id=0)
         {
             //id = 39;
             ViewBag.ID_PROJ_TASK = id;
             ViewBag.banderaTask = 1;

             return View();
         }
        public ActionResult DataListTask(int id=0)
        {
            textInfo = cultureInfo.TextInfo;
            int ID_PROJ = id;
            var data = (from t in dbc.TASKs.Where(x => x.ID_PROJ == ID_PROJ)
                         select new
                         {
                             t.ID_TASK,
                             t.NAM_TASK,
                             t.DESC_TASK,
                             t.ID_PROJ,
                             t.UserId,
                             t.DATE_STAR,
                             t.DATE_END,
                             t.ID_TICK,
                             t.CREATED,
                         }).ToList();

            var qry = (from d in data
                         join ce in dbe.CLASS_ENTITY on d.UserId equals ce.UserId
                         join pe in dbe.PERSON_ENTITY on ce.ID_ENTI equals pe.ID_ENTI2
                         select new
                         {
                             d.ID_TASK,
                             d.NAM_TASK,
                             d.DESC_TASK,
                             d.ID_PROJ,
                             d.DATE_STAR,
                             d.DATE_END,
                             d.ID_TICK,
                             d.CREATED,
                             ce.FIR_NAME,
                             ce.LAS_NAME,
                             ce.MOT_NAME,
                             pe.ID_PERS_ENTI,
                             
                         });

            var query = (from q in qry.ToList()
                         select new
                         {
                             q.ID_TASK,
                             NAME = textInfo.ToTitleCase((q.FIR_NAME.ToLower() + " " + q.LAS_NAME.ToLower() + " " + (q.MOT_NAME == null ? "" : q.MOT_NAME).ToLower())),
                             q.ID_PERS_ENTI,
                             q.ID_PROJ,
                             q.NAM_TASK,
                             q.DESC_TASK,
                             CREATED = String.Format("{0:f}", q.CREATED),
                             DATE_STAR = String.Format("{0:d}", q.DATE_STAR),
                             DATE_END = String.Format("{0:d}", q.DATE_END),
                         });

            return Json(new { Data = query}, JsonRequestBehavior.AllowGet);
        }

    }
}
