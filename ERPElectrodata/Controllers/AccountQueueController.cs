using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.Data.Entity;

namespace ERPElectrodata.Controllers
{
    public class AccountQueueController : Controller
    {
        public CoreEntities dbc = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();
        //
        // GET: /AccountQueue/
        public ActionResult ListByAcco()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            var query = dbc.ACCOUNT_QUEUE.Where(aq => aq.ID_ACCO == ID_ACCO && aq.VIG_ACCO_QUEU == true);
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            var result = (from aq in query.ToList()
                          join q in dbc.QUEUEs on aq.ID_QUEU equals q.ID_QUEU
                          join a in dbc.ACCOUNTs on aq.ID_ACCO equals a.ID_ACCO
                          select new
                          {
                              QUEU = a.ACR_ACCO + '.' + q.NAM_QUEU,
                              ID_QUEU = q.ID_QUEU,
                              ID_PERS_ENTI = ID_PERS_ENTI,// assiSelect(q.ID_QUEU),
                              ID_STAT = q.ID_STAT,
                              DES_QUEU = (q.DES_QUEU == null ? "" : q.DES_QUEU.ToLower()),
                              ORD_ACCO_QUEU = aq.ORD_ACCO_QUEU,
                              ID_PERS_ENTI_ASSI_DEFA = aq.ID_PERS_ENTI_ASSI_DEFA,
                          }).OrderBy(x => x.ORD_ACCO_QUEU);

            return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListByAccoBVN()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            var query = dbc.ACCOUNT_QUEUE.Where(aq => aq.ID_ACCO == ID_ACCO && aq.VIG_ACCO_QUEU == true);
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            var JEFATURA = dbc.Jefatura.Where(x => x.ID_PERS_ENTI_RESP == ID_PERS_ENTI).FirstOrDefault();


            if (ID_PERS_ENTI == 1007)
            {
                var result = (from aq in query.ToList()
                              join q in dbc.QUEUEs on aq.ID_QUEU equals q.ID_QUEU
                              join a in dbc.ACCOUNTs on aq.ID_ACCO equals a.ID_ACCO
                              select new
                              {
                                  QUEU = a.ACR_ACCO + '.' + q.NAM_QUEU,
                                  ID_QUEU = q.ID_QUEU,
                                  ID_PERS_ENTI = ID_PERS_ENTI,// assiSelect(q.ID_QUEU),
                                  ID_STAT = q.ID_STAT,
                                  DES_QUEU = (q.DES_QUEU == null ? "" : q.DES_QUEU.ToLower()),
                                  ORD_ACCO_QUEU = aq.ORD_ACCO_QUEU,
                                  ID_PERS_ENTI_ASSI_DEFA = aq.ID_PERS_ENTI_ASSI_DEFA
                              }).OrderBy(x => x.ORD_ACCO_QUEU);
                return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);
            }
            else if (JEFATURA != null) 
            {
                
                int IdJefaturaBVN = Convert.ToInt32(JEFATURA.IdJefatura);
                var result = (from aq in query.ToList()
                              join q in dbc.QUEUEs on aq.ID_QUEU equals q.ID_QUEU
                              join a in dbc.ACCOUNTs on aq.ID_ACCO equals a.ID_ACCO
                              join jq in dbc.JefaturaQueue on q.ID_QUEU equals jq.IdQueu
                              where jq.IdJefatura == IdJefaturaBVN
                              select new
                              {
                                  QUEU = a.ACR_ACCO + '.' + q.NAM_QUEU,
                                  ID_QUEU = q.ID_QUEU,
                                  ID_PERS_ENTI = ID_PERS_ENTI,// assiSelect(q.ID_QUEU),
                                  ID_STAT = q.ID_STAT,
                                  DES_QUEU = (q.DES_QUEU == null ? "" : q.DES_QUEU.ToLower()),
                                  ORD_ACCO_QUEU = aq.ORD_ACCO_QUEU,
                                  ID_PERS_ENTI_ASSI_DEFA = aq.ID_PERS_ENTI_ASSI_DEFA
                              }).OrderBy(x => x.ORD_ACCO_QUEU);
                return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var result = (from aq in query.ToList()
                              join q in dbc.QUEUEs on aq.ID_QUEU equals q.ID_QUEU
                              join a in dbc.ACCOUNTs on aq.ID_ACCO equals a.ID_ACCO
                              select new
                              {
                                  QUEU = a.ACR_ACCO + '.' + q.NAM_QUEU,
                                  ID_QUEU = q.ID_QUEU,
                                  ID_PERS_ENTI = ID_PERS_ENTI,// assiSelect(q.ID_QUEU),
                                  ID_STAT = q.ID_STAT,
                                  DES_QUEU = (q.DES_QUEU == null ? "" : q.DES_QUEU.ToLower()),
                                  ORD_ACCO_QUEU = aq.ORD_ACCO_QUEU,
                                  ID_PERS_ENTI_ASSI_DEFA = aq.ID_PERS_ENTI_ASSI_DEFA
                              }).OrderBy(x => x.ORD_ACCO_QUEU);
                return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);
            }

        }

        //
        // GET: /AccountQueue/
        public ActionResult ListarAreaResponsablexCuenta(int ID_ACCO)
        {
            var query = dbc.ACCOUNT_QUEUE.Where(aq => aq.ID_ACCO == ID_ACCO && aq.VIG_ACCO_QUEU == true);
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            var result = (from aq in query.ToList()
                          join q in dbc.QUEUEs on aq.ID_QUEU equals q.ID_QUEU
                          join a in dbc.ACCOUNTs on aq.ID_ACCO equals a.ID_ACCO
                          select new
                          {
                              QUEU = a.ACR_ACCO + '.' + q.NAM_QUEU,
                              ID_QUEU = q.ID_QUEU,
                              DES_QUEU = (q.DES_QUEU == null ? "" : q.DES_QUEU.ToLower()),
                              ORD_ACCO_QUEU = aq.ORD_ACCO_QUEU
                          }).OrderBy(x => x.ORD_ACCO_QUEU);

            return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarOperaciones(string q, string page)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            string termino = "";

            if (Request.QueryString["q"] == null)
            {
                termino = "";
            }
            else
            {
                termino = Request.QueryString["q"].ToString();
            }
            var query = dbc.ACCOUNT_QUEUE.Where(aq => aq.ID_ACCO == ID_ACCO && aq.VIG_ACCO_QUEU == true && (aq.ID_QUEU == 3 || aq.ID_QUEU == 4 || aq.ID_QUEU == 14 || aq.ID_QUEU == 25 || aq.ID_QUEU == 9 || aq.ID_QUEU == 22 || aq.ID_QUEU == 15));
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            var result = (from aq in query.ToList()
                          join qu in dbc.QUEUEs on aq.ID_QUEU equals qu.ID_QUEU
                          join a in dbc.ACCOUNTs on aq.ID_ACCO equals a.ID_ACCO
                          select new
                          {
                              QUEU = a.ACR_ACCO + '.' + qu.NAM_QUEU,
                              ID_QUEU = qu.ID_QUEU,
                              ID_PERS_ENTI = ID_PERS_ENTI,// assiSelect(q.ID_QUEU),
                              ID_STAT = qu.ID_STAT,
                              DES_QUEU = (qu.DES_QUEU == null ? "" : qu.DES_QUEU.ToLower()),
                              ORD_ACCO_QUEU = aq.ORD_ACCO_QUEU,
                              ID_PERS_ENTI_ASSI_DEFA = aq.ID_PERS_ENTI_ASSI_DEFA
                          }).OrderBy(x => x.ORD_ACCO_QUEU);

            return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListaOperaciones(string q, string page)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            string termino = "";

            if (Request.QueryString["q"] == null)
            {
                termino = "";
            }
            else
            {
                termino = Request.QueryString["q"].ToString();
            }
            var query = dbc.ACCOUNT_QUEUE.Where(aq => aq.ID_ACCO == ID_ACCO && aq.VIG_ACCO_QUEU == true && (aq.ID_QUEU == 3 || aq.ID_QUEU == 4 || aq.ID_QUEU == 14 || aq.ID_QUEU == 25 || aq.ID_QUEU == 9 || aq.ID_QUEU == 22 || aq.ID_QUEU == 27 || aq.ID_QUEU == 15));
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            var result = (from aq in query.ToList()
                          join qu in dbc.QUEUEs on aq.ID_QUEU equals qu.ID_QUEU
                          join a in dbc.ACCOUNTs on aq.ID_ACCO equals a.ID_ACCO
                          select new
                          {
                              QUEU = a.ACR_ACCO + '.' + qu.NAM_QUEU,
                              ID_QUEU = qu.ID_QUEU,
                              ID_PERS_ENTI = ID_PERS_ENTI,// assiSelect(q.ID_QUEU),
                              ID_STAT = qu.ID_STAT,
                              DES_QUEU = (qu.DES_QUEU == null ? "" : qu.DES_QUEU.ToLower()),
                              ORD_ACCO_QUEU = aq.ORD_ACCO_QUEU,
                              ID_PERS_ENTI_ASSI_DEFA = aq.ID_PERS_ENTI_ASSI_DEFA,
                              id = qu.ID_QUEU,
                              text = a.ACR_ACCO + '.' + qu.NAM_QUEU + " - " + (qu.DES_QUEU == null ? "" : qu.DES_QUEU.ToLower())
                          }).Where(x => x.text.Contains(termino)).OrderBy(x => x.text);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public int assiSelect(int ID_QUEU)
        {
            try
            {
                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

                var query = dbe.PERSON_ENTITY.Where(pe => pe.ID_QUEU == ID_QUEU);

                var result = (from x in query.ToList()
                              join a in dbe.ACCOUNT_ENTITY on x.ID_PERS_ENTI equals a.ID_PERS_ENTI
                              join ce in dbe.CLASS_ENTITY on x.ID_ENTI2 equals ce.ID_ENTI
                              where a.ID_ACCO == ID_ACCO && a.VIS_ASSI == true
                              select new
                              {
                                  ASSI = ce.FIR_NAME + ' ' + ce.LAS_NAME,
                                  x.ID_PERS_ENTI
                              }).First();

                return result.ID_PERS_ENTI;
            }
            catch
            {
                return 0;
            }
        }

        //
        // GET: /AccountQueue/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListarAreaResponsable(string q, string page)
        {
            string termino = "";
            if (Request.QueryString["q"] == null)
            {
                termino = "";
            }
            else
            {
                termino = Request.QueryString["q"].ToString();
            }

            var ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());
            var result = dbc.ListarAreaResponsable(ID_ACCO, 0, termino).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarColaCuenta(int id, string q, string page)
        {
            string termino = "";
            if (Request.QueryString["q"] == null)
            {
                termino = "";
            }
            else
            {
                termino = Request.QueryString["q"].ToString();
            }

            var result = dbc.ListarColaCuenta(id, termino).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //Mantenimiento

        public static int ID_ACCO = 0;
        public static int ID_QUEU = 0;
        public static int ID_PERS_ENTI_CORD = 0;

        public ActionResult IndexCuentaCola()
        {
            return View();
        }

        public ActionResult ListaCuentaCombo()
        {
            var query = (from at in dbc.ACCOUNTs select new { at.ID_ACCO, at.NAM_ACCO }).ToList();
            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListaColaCombo()
        {
            var query = (from at in dbc.QUEUEs select new { at.ID_QUEU, at.NAM_QUEU_REPO }).ToList();
            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreaCuentaCola()
        {
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CreaCuentaCola(ACCOUNT_QUEUE objACCOQUE)
        {
            string cnt = Request.Params["ID_CUENTA"];
            int i;
            if (!int.TryParse(cnt, out i))
            {
                return Content("<script type='text/javascript'> function init() {" +
                         "if(top.uploadDone) top.uploadDone('ERROR','1','0');}window.onload=init;</script>");
            }

            string cl = Request.Params["ID_COLA"];
            int o;
            if (!int.TryParse(cl, out o))
            {
                return Content("<script type='text/javascript'> function init() {" +
                         "if(top.uploadDone) top.uploadDone('ERROR','1','0');}window.onload=init;</script>");
            }

            string prsn = Request.Params["ID_PERSONA"];
            int u;
            if (!int.TryParse(prsn, out u))
            {
                return Content("<script type='text/javascript'> function init() {" +
                         "if(top.uploadDone) top.uploadDone('ERROR','1','0');}window.onload=init;</script>");
            }

            int Cuenta = Convert.ToInt32(Request.Params["ID_CUENTA"]);
            int Cola = Convert.ToInt32(Request.Params["ID_COLA"]);
            int Usuario = Convert.ToInt32(Request.Params["ID_PERSONA"]);

            int flag = 0;

            if (Convert.ToString(Cuenta) == "") { flag = 1; }
            if (Convert.ToString(Cola) == "") { flag = 2; }
            if (Convert.ToString(Usuario) == "") { flag = 3; }
            if ((objACCOQUE.EMA_ACCO_QUEU) == "") { flag = 4; }

            var DuplicadoCuentaCola = dbc.ACCOUNT_QUEUE.Where(a => a.ID_ACCO == Cuenta && a.ID_QUEU == Cola);
            int cuencol = DuplicadoCuentaCola.Count();
            if (DuplicadoCuentaCola.Count() > 1)
            {
                return Content("<script type='text/javascript'> function init() {" +
                           "if(top.uploadDone) top.uploadDone('ERROR','1','0');}window.onload=init;</script>");
            }

            if (flag != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','" + flag.ToString() + "','0');}window.onload=init;</script>");
            }

            objACCOQUE.VIG_ACCO_QUEU = true;

            if (objACCOQUE.VIS_ALL_QUEU == null)
            {
                objACCOQUE.VIS_ALL_QUEU = false;
            }
            else
            {
                objACCOQUE.VIS_ALL_QUEU = Convert.ToBoolean(objACCOQUE.VIS_ALL_QUEU);
            }

            objACCOQUE.ID_ACCO = Convert.ToInt32(Cuenta.ToString());
            objACCOQUE.ID_QUEU = Convert.ToInt32(Cola.ToString());
            objACCOQUE.ID_PERS_ENTI_CORD = Convert.ToInt32(Usuario.ToString());

            if (ModelState.IsValid)
            {
                dbc.ACCOUNT_QUEUE.Add(objACCOQUE);
                dbc.SaveChanges();

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','0','" + objACCOQUE.ID_ACCO_QUEUE.ToString() + "');}window.onload=init;</script>");
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','1','0');}window.onload=init;</script>");
            }
        }

        public ActionResult ListarBuscarCuentaColaTodo(int i = 0)
        {
            int Cuenta = 0;
            if (Request.Params["ID_ACCO"] != null)
            { Cuenta = Convert.ToInt32(Request.Params["ID_ACCO"].ToString()); }

            int Cola = 0;
            if (Request.Params["ID_QUEU"] != null)
            { Cola = Convert.ToInt32(Request.Params["ID_QUEU"].ToString()); }

            int Usuario = 0;
            if (Request.Params["ID_PERS_ENTI_CORD"] != null)
            { Usuario = Convert.ToInt32(Request.Params["ID_PERS_ENTI_CORD"].ToString()); }

            String Estado = Convert.ToString(Request.Params["VIG_ACCO_QUEU"]);
            String Visibilidad = Convert.ToString(Request.Params["VIS_ALL_QUEU"]);

            var query = dbc.ListarBuscarCuentaCola(Cuenta, Cola, Usuario, Estado, Visibilidad).ToList();

            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            var query2 = query.Skip(skip).Take(take);
            return Json(new { Data = query2, Cantidad = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditaCuentaCola(int Id = 0)
        {
            var p = dbc.ACCOUNT_QUEUE.Find(Id);
            var c = dbc.ACCOUNTs.Find(p.ID_ACCO);
            var q = dbc.QUEUEs.Find(p.ID_QUEU);
            var u = dbe.PERSON_ENTITY.Find(p.ID_PERS_ENTI_CORD);

            ViewBag.IdCuenta = c.ID_ACCO;
            ViewBag.IdCola = q.ID_QUEU;
            ViewBag.IdUsuario = u.ID_PERS_ENTI;
            ViewBag.Estado = p.VIG_ACCO_QUEU;
            ViewBag.Correo = p.EMA_ACCO_QUEU;
            ViewBag.Visibilidad = p.VIS_ALL_QUEU;

            return View(p);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult EditaCuentaCola(ACCOUNT_QUEUE objACQUE)
        {
            //Validar campos en blanco
            int flag = 0;

            if (Request.Params["ID_CUENTA"].ToString() == null) { flag = 1; }
            else { objACQUE.ID_ACCO = Convert.ToInt32(Request.Params["ID_CUENTA"]); }

            if (Request.Params["ID_COLA"].ToString() == null) { flag = 2; }
            else { objACQUE.ID_QUEU = Convert.ToInt32(Request.Params["ID_COLA"]); }

            if (Request.Params["ID_PERSONA"].ToString() == null) { flag = 3; }
            else { objACQUE.ID_PERS_ENTI_CORD = Convert.ToInt32(Request.Params["ID_PERSONA"]); }

            int Cuenta = Convert.ToInt32(Request.Params["ID_CUENTA"]);
            int Cola = Convert.ToInt32(Request.Params["ID_COLA"]);

            if (Convert.ToString(objACQUE.EMA_ACCO_QUEU) == null) { flag = 4; }
            if (Convert.ToString(Cuenta) == null) { flag = 5; }
            if (Convert.ToString(Cola) == null) { flag = 6; }
            if (Convert.ToString(objACQUE.ID_PERS_ENTI_CORD) == null) { flag = 7; }

            if (flag != 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','" + flag.ToString() + "','0');}window.onload=init;</script>");
            }

            if (ModelState.IsValid)
            {
                var DuplicadoCuentaCola = dbc.ACCOUNT_QUEUE.Where(a => a.ID_ACCO == Cuenta && a.ID_QUEU == Cola);
                int cuencol = DuplicadoCuentaCola.Count();
                if (DuplicadoCuentaCola.Count() > 1)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                               "if(top.uploadDone) top.uploadDone('ERROR','1','0');}window.onload=init;</script>");
                }

                Convert.ToBoolean(objACQUE.VIS_ALL_QUEU);
                Convert.ToBoolean(objACQUE.VIG_ACCO_QUEU);

                ACCOUNT_QUEUE objCuentaCola = dbc.ACCOUNT_QUEUE.Find(objACQUE.ID_ACCO_QUEUE);
                objCuentaCola.ID_ACCO = objACQUE.ID_ACCO;
                objCuentaCola.ID_QUEU = objACQUE.ID_QUEU;
                objCuentaCola.ID_PERS_ENTI_CORD = objACQUE.ID_PERS_ENTI_CORD;
                objCuentaCola.VIG_ACCO_QUEU = objACQUE.VIG_ACCO_QUEU;
                objCuentaCola.EMA_ACCO_QUEU = objACQUE.EMA_ACCO_QUEU;
                objCuentaCola.VIS_ALL_QUEU = objACQUE.VIS_ALL_QUEU;

                dbc.Entry(objCuentaCola).State = EntityState.Modified;
                dbc.SaveChanges();

                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','0','" + objACQUE.ID_ACCO_QUEUE.ToString() + "');}window.onload=init;</script>");
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','1','0');}window.onload=init;</script>");
            }
        }
    }
}
