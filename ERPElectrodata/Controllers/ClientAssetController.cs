using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class ClientAssetController : Controller
    {
        public CoreEntities db = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();
        //
        // GET: /ClientAsset/DetailsAsset
        public ActionResult DetailsAsset(int id = 0)
        {
            
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int ID_ACCOa = ID_ACCO;
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            if (ID_ACCOa == 2 || ID_ACCOa == 7)
                ID_ACCOa = 1;

            var listArea = (from a in dbe.AREAs
                                where a.ID_ACCO == ID_ACCOa //&& a.ID_AREA_PARENT != null
                                select new{
                                a.ID_AREA,
                                NAM_AREA = a.NOM_AREA
                                }).ToList();

            var listuser = (from lu in dbe.ACCOUNT_ENTITY
                            join pe in dbe.PERSON_ENTITY on lu.ID_PERS_ENTI equals pe.ID_PERS_ENTI
                            join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                            where lu.ID_ACCO == ID_ACCO
                            select new
                            {
                                lu.ID_PERS_ENTI,
                                Client = ce.FIR_NAME+" "+ce.LAS_NAME,
                                ce.UserId,
                                pe.ID_FOTO,
                            }).ToList();

            var query = db.CLIENT_ASSET.Where(x => x.ID_ASSE == id).ToList().OrderByDescending(x=>x.CREATE_DATE);

            var result = (from ca in query.Skip(skip).Take(take)
                          join a in listArea on ca.ID_AREA equals a.ID_AREA into la
                          from xa in la.DefaultIfEmpty()
                          join c in db.CONDITIONs on ca.ID_COND equals c.ID_COND
                          join s in db.STATUS_ASSET on c.ID_STAT_ASSE equals s.ID_STAT_ASSE
                          join l in db.LOCATIONs on ca.ID_LOCA equals l.ID_LOCA
                          join cl in listuser on ca.ID_PERS_ENTI equals cl.ID_PERS_ENTI
                          join u in listuser on ca.UserId equals u.UserId
                          join tca in db.TYPE_CLIENT_ASSET on ca.ID_TYPE_CLIE_ASSE equals tca.ID_TYPE_ASSE_CLIEN
                          join pt in db.PART_TYPE_ASSET on ca.ID_PART_TYPE_ASSE equals pt.ID_PART_TYPE_ASSE into lpt
                          from xpt in lpt.DefaultIfEmpty()
                          select new {
                              NAM_COND = c.NAM_COND.ToLower(),
                              NAM_STAT_ASSE = s.NAM_STAT_ASSE.ToLower(),
                              NAM_LOCA = l.NAM_LOCA,
                              CLIE = cl.Client.ToLower(),
                              DAT_STAR = String.Format("{0:d}" ,ca.DAT_STAR),
                              DAT_END = String.Format("{0:d}", ca.DAT_END),
                              CREATE_DATE = String.Format("{0:d}", ca.CREATE_DATE),
                              CREATE_BY = u.Client.ToLower(),
                              SUM_CLIE_ASSE = (ca.SUM_CLIE_ASSE == null ? "" : ca.SUM_CLIE_ASSE),
                              NAM_TYPE_ASSE_CLIE = tca.NAM_TYPE_ASSE_CLIE.ToLower(),
                              NAM_PART_TYPE_ASSE = (xpt == null ? String.Empty : xpt.NAM_PART_TYPE_ASSE.ToLower()),
                              ATTA = Adjuntos(ca.ID_CLIE_ASSE),
                              FOTO = Convert.ToString(u.ID_FOTO) + ".jpg",
                              NAME_AREA = (xa == null ? "-" : (xa.NAM_AREA == null ? "-" : (xa.NAM_AREA.Trim().Length > 19 ? xa.NAM_AREA.Trim().ToLower().Substring(0, 19) : xa.NAM_AREA.Trim().ToLower()))),
                              ADJ = DocAdjunto(ca.ID_CLIE_ASSE),
                              LINK = DocIdAtta(ca.ID_CLIE_ASSE),
                              IMG = DocAdjunto(ca.ID_CLIE_ASSE) == "" ? "" : "/Content/Images/pdf.png",
                          });
            var prueba = result.ToList();

            return Json(new {Data=result,Count = query.Count() },JsonRequestBehavior.AllowGet);
        }

        public ActionResult ActivoComentarios(int id = 0)
        {

            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int ID_ACCOa = ID_ACCO;

            if (ID_ACCO == 60)
            {
                var resultB = db.ObtenerActividadesBNV(id)
                                .Select(a => new
                                {
                                    a.NAM_COND,
                                    a.NAM_STAT_ASSE,
                                    a.CLIE,
                                    a.DAT_STAR,
                                    a.DAT_END,
                                    a.CREATE_DATE,
                                    a.CREATE_BY,
                                    a.SUM_CLIE_ASSE,
                                    a.NAM_TYPE_ASSE_CLIE,
                                    ATTA = Adjuntos(a.ATTA),
                                    a.FOTO,
                                    NAME_AREA = a.NAME_AREA.Trim().Length > 19 ? a.NAME_AREA.Trim().Substring(0, 19) : a.NAME_AREA.Trim(),
                                    ADJ = DocAdjunto(a.ADJ),
                                    LINK = DocIdAtta(a.LINK),
                                    IMG = DocAdjunto(a.IMG) == "" ? "" : "/Content/Images/pdf.png",
                                });
                return Json(new { Data = resultB }, JsonRequestBehavior.AllowGet);
            }
            else if (ID_ACCO == 55)
            {
                var resultB = db.ObtenerActividadesActivo(id)
                                .Select(a => new
                                {
                                    a.NAM_COND,
                                    a.NAM_STAT_ASSE,
                                    a.CLIE,
                                    a.DAT_STAR,
                                    a.DAT_END,
                                    a.CREATE_DATE,
                                    a.CREATE_BY,
                                    a.SUM_CLIE_ASSE,
                                    a.NAM_TYPE_ASSE_CLIE,
                                    ATTA = Adjuntos(a.ATTA),
                                    a.FOTO,
                                    NAME_AREA = a.NAME_AREA.Trim().Length > 19 ? a.NAME_AREA.Trim().Substring(0, 19) : a.NAME_AREA.Trim(),
                                    ADJ = DocAdjunto(a.ADJ),
                                    LINK = DocIdAtta(a.LINK),
                                    IMG = DocAdjunto(a.IMG) == "" ? "" : "/Content/Images/pdf.png",
                                });
                return Json(new { Data = resultB }, JsonRequestBehavior.AllowGet);
            }

            if (ID_ACCOa == 2 || ID_ACCOa == 7)
                ID_ACCOa = 1;

            var listArea = (from a in dbe.AREAs
                            where a.ID_ACCO == ID_ACCOa //&& a.ID_AREA_PARENT != null
                            select new
                            {
                                a.ID_AREA,
                                NAM_AREA = a.NOM_AREA
                            }).ToList();

            var listuser = (from lu in dbe.ACCOUNT_ENTITY
                            join pe in dbe.PERSON_ENTITY on lu.ID_PERS_ENTI equals pe.ID_PERS_ENTI
                            join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                            where lu.ID_ACCO == ID_ACCO
                            select new
                            {
                                lu.ID_PERS_ENTI,
                                Client = ce.FIR_NAME + " " + ce.LAS_NAME,
                                ce.UserId,
                                pe.ID_FOTO,
                            }).ToList();

            var query = db.CLIENT_ASSET.Where(x => x.ID_ASSE == id).ToList().OrderByDescending(x => x.CREATE_DATE);

            var result = (from ca in query.ToList()
                          join a in listArea on ca.ID_AREA equals a.ID_AREA into la
                          from xa in la.DefaultIfEmpty()
                          join c in db.CONDITIONs on ca.ID_COND equals c.ID_COND
                          join s in db.STATUS_ASSET on c.ID_STAT_ASSE equals s.ID_STAT_ASSE
                          join l in db.LOCATIONs on ca.ID_LOCA equals l.ID_LOCA
                          join cl in listuser on ca.ID_PERS_ENTI equals cl.ID_PERS_ENTI
                          join u in listuser on ca.UserId equals u.UserId
                          join tca in db.TYPE_CLIENT_ASSET on ca.ID_TYPE_CLIE_ASSE equals tca.ID_TYPE_ASSE_CLIEN
                          join pt in db.PART_TYPE_ASSET on ca.ID_PART_TYPE_ASSE equals pt.ID_PART_TYPE_ASSE into lpt
                          from xpt in lpt.DefaultIfEmpty()
                          select new
                          {
                              NAM_COND = c.NAM_COND,
                              NAM_STAT_ASSE = s.NAM_STAT_ASSE,
                              NAM_LOCA = l.NAM_LOCA,
                              CLIE = cl.Client,
                              DAT_STAR = String.Format("{0:d}", ca.DAT_STAR),
                              DAT_END = String.Format("{0:d}", ca.DAT_END),
                              CREATE_DATE = String.Format("{0:d}", ca.CREATE_DATE),
                              CREATE_BY = u.Client,
                              SUM_CLIE_ASSE = (ca.SUM_CLIE_ASSE == null ? "" : ca.SUM_CLIE_ASSE),
                              NAM_TYPE_ASSE_CLIE = tca.NAM_TYPE_ASSE_CLIE,
                              NAM_PART_TYPE_ASSE = (xpt == null ? String.Empty : xpt.NAM_PART_TYPE_ASSE),
                              ATTA = Adjuntos(ca.ID_CLIE_ASSE),
                              FOTO = Convert.ToString(u.ID_FOTO) + ".jpg",
                              NAME_AREA = (xa == null ? "-" : (xa.NAM_AREA == null ? "-" : (xa.NAM_AREA.Trim().Length > 19 ? xa.NAM_AREA.Trim().Substring(0, 19) : xa.NAM_AREA.Trim()))),
                              ADJ = DocAdjunto(ca.ID_CLIE_ASSE),
                              LINK = DocIdAtta(ca.ID_CLIE_ASSE),
                              IMG = DocAdjunto(ca.ID_CLIE_ASSE) == "" ? "" : "/Content/Images/pdf.png",
                          });
            var prueba = result.ToList();

            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }

        public string Adjuntos(int id)
        {
            string Adjun = "";
            try
            {
                var query = db.ATTACHEDs.Where(a => a.ID_CLIE_ASSE == id);
                foreach (ATTACHED subqu in query)
                {
                    Adjun = Adjun + "<li><a href='/Adjuntos/Asset/" + subqu.NAM_ATTA + "_" + subqu.ID_ATTA + subqu.EXT_ATTA + " ' TARGET='_BLANK'>" + subqu.NAM_ATTA + subqu.EXT_ATTA + "</a></li>";
                }

                return Adjun;
            }
            catch
            {
                return String.Empty;
            }
        }

        public string DocAdjunto(int id)
        {
            string Adjun = "";
            try
            {
                var query = db.ATTACHEDs.Where(a => a.ID_CLIE_ASSE == id);
                foreach (ATTACHED subqu in query)
                {
                    Adjun = subqu.NAM_ATTA + subqu.EXT_ATTA;
                }

                return Adjun;
            }
            catch
            {
                return String.Empty;
            }
        }
        public string DocIdAtta(int id)
        {
            string Adjun = "";
            try
            {
                var query = db.ATTACHEDs.Where(a => a.ID_CLIE_ASSE == id);
                foreach (ATTACHED subqu in query)
                {
                    Adjun = subqu.NAM_ATTA + "_" + subqu.ID_ATTA + subqu.EXT_ATTA;
                }

                return Adjun;
            }
            catch
            {
                return String.Empty;
            }
        }
        //
        // GET: /ClientAsset/

        public ActionResult Index()
        {
            //var client_asset = db.CLIENT_ASSET.Include(c => c.ASSET).Include(c => c.CLIENT).Include(c => c.CONDITION).Include(c => c.LOCATION).Include(c => c.TYPE_CLIENT_ASSET);
            return View();
        }

        //
        // GET: /ClientAsset/Details/5

        public ActionResult Details(int id = 0)
        {
            CLIENT_ASSET client_asset = db.CLIENT_ASSET.Find(id);
            if (client_asset == null)
            {
                return HttpNotFound();
            }
            return View(client_asset);
        }

        //
        // GET: /ClientAsset/Create

        public ActionResult Create()
        {
            ViewBag.ID_ASSE = new SelectList(db.ASSETs, "ID_ASSE", "COD_ASSE");
            ViewBag.ID_CLIE = new SelectList(db.CLIENTs, "ID_CLIE", "SEX_CLIE");
            ViewBag.ID_COND = new SelectList(db.CONDITIONs, "ID_COND", "NAM_COND");
            ViewBag.ID_LOCA = new SelectList(db.LOCATIONs, "ID_LOCA", "NAM_LOCA");
            ViewBag.ID_TYPE_CLIE_ASSE = new SelectList(db.TYPE_CLIENT_ASSET, "ID_TYPE_ASSE_CLIEN", "NAM_TYPE_ASSE_CLIE");
            return View();
        }

        //
        // POST: /ClientAsset/Create

        [HttpPost, ValidateInput(false)]
        public ActionResult Create(IEnumerable<HttpPostedFileBase> files, CLIENT_ASSET client_asset)
        {
            int UserId = Convert.ToInt32(Session["UserId"]);
            var asse = new ASSET();
            int ID_AREA = 18,ID_LOCA;
            
            var cal = db.CLIENT_ASSET.Single(x => x.ID_ASSE == client_asset.ID_ASSE && x.DAT_END == null);
            ID_AREA = Convert.ToInt32(cal.ID_AREA);
            ID_LOCA = Convert.ToInt32(cal.ID_LOCA);

            try
            {
                ID_LOCA = Convert.ToInt32(Request.Params["ID_LOCA"]);
            }
            catch
            {
            }

            try
            {
                int ID_PERS_ENTI = Convert.ToInt32(Request.Params["ID_PERS_ENTI"]);
                ID_AREA = Convert.ToInt32(dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == ID_PERS_ENTI).ID_AREA);
            }
            catch
            {

            }

            if (client_asset.ID_TYPE_CLIE_ASSE == 4) //CHANGE HARDWARE
            {
                if (client_asset.ID_PART_TYPE_ASSE == null)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                      "if(top.uploadDone) top.uploadDone('ERROR','Seleccione un Componente.');}window.onload=init;</script>");
                }
                if (client_asset.VAL_PART_ASSE == null)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                      "if(top.uploadDone) top.uploadDone('ERROR','Ingrese un valor de configuración.');}window.onload=init;</script>");
                }
                if (client_asset.VAL_PART_ASSE.Trim().Length == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                      "if(top.uploadDone) top.uploadDone('ERROR','Ingre un valor de configuración.');}window.onload=init;</script>");
                }
                client_asset.ID_COND = cal.ID_COND;
                client_asset.ID_PERS_ENTI = cal.ID_PERS_ENTI;
            }
            else if (client_asset.ID_TYPE_CLIE_ASSE == 3) //UPDATE STATUS
            {
                if (client_asset.ID_COND == null)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                      "if(top.uploadDone) top.uploadDone('ERROR','Seleccione una Condición.');}window.onload=init;</script>");
                }

                if (client_asset.ID_PERS_ENTI == null)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                      "if(top.uploadDone) top.uploadDone('ERROR','Seleccione un Usuario.');}window.onload=init;</script>");
                }
            }
            else if (client_asset.ID_TYPE_CLIE_ASSE == 5) //EDIT ASSET
            {
                client_asset.ID_PERS_ENTI = cal.ID_PERS_ENTI;
                client_asset.ID_COND = cal.ID_COND;

                int sw = 0;
                int ID_ASSE = Convert.ToInt32(client_asset.ID_ASSE);
                string msn = "";
                asse = db.ASSETs.Single(x => x.ID_ASSE == ID_ASSE);

                string COD_ASSE = Convert.ToString(Request.Form["COD_ASSE"]).Trim(); //OBLIGATORIO
                string TYPE_ASSE = Convert.ToString(Request.Form["ID_TYPE_ASSE"]).Trim(); //OBLIGATORIO
                string MANU = Convert.ToString(Request.Form["ID_MANU"]).Trim(); //OBLIGATORIO
                string COMM_MODE = Convert.ToString(Request.Form["ID_COMM_MODE"]).Trim();
                string MANU_MODE = Convert.ToString(Request.Form["ID_MANU_MODE"]).Trim();
                string SER_NUMB = Convert.ToString(Request.Form["SER_NUMB"]).Trim(); //OBLIGATORIO
                string BUY_MODE = Convert.ToString(Request.Form["ID_BUY_MODE"]).Trim(); //OBLIGATORIO
                string COST_CENT = Convert.ToString(Request.Form["ID_COST_CENT"]).Trim(); //OBLIGATORIO
                string COST = Convert.ToString(Request.Form["COST"]).Trim(); //OBLIGATORIO
                string LOTE = Convert.ToString(Request.Form["LOTE"]).Trim();
                string SOLPED = Convert.ToString(Request.Form["SOLPED"]).Trim();
                string NAM_EQUI = Convert.ToString(Request.Form["NAM_EQUI"]).Trim();
                string DATE_ACQ = Convert.ToString(Request.Form["ACQ_DATE"]).Trim();
                string MAC_ADRR = Convert.ToString(Request.Form["MAC_ADRR"]).Trim();
                string IpLocal = Convert.ToString(Request.Form["IpLocal"]).Trim();
                string IpPublica = Convert.ToString(Request.Form["IpPublica"]).Trim();

                string ID_SUBTYPE1 = Convert.ToString(String.IsNullOrEmpty(Request.Form["ID_SUBTYPE1"]) ? "" : Request.Form["ID_SUBTYPE1"]).Trim();
                string ID_SUBTYPE2 = Convert.ToString(String.IsNullOrEmpty(Request.Form["ID_SUBTYPE2"]) ? "" : Request.Form["ID_SUBTYPE2"]).Trim();
                string ID_SUBTYPE3 = Convert.ToString(String.IsNullOrEmpty(Request.Form["ID_SUBTYPE3"]) ? "" : Request.Form["ID_SUBTYPE3"]).Trim();
                string numeroTelefono = Convert.ToString(String.IsNullOrEmpty(Request.Form["numeroTelefono"]) ? "" : Request.Form["numeroTelefono"]).Trim();
                string idCompaniaTelefono = Convert.ToString(String.IsNullOrEmpty(Request.Form["idCompaniaTelefono"]) ? "" : Request.Form["idCompaniaTelefono"]).Trim();
                string DAT_STAR_LEAS = Convert.ToString(Request.Form["DAT_STAR_LEAS"]);
                string DAT_END_LEAS = Convert.ToString(Request.Form["DAT_END_LEAS"]);
                string fechaFinGarantia = Convert.ToString(Request.Form["fechaFinGarantia"]);
                string IdContrato = Convert.ToString(Request.Form["IdContrato"]);
                string Locacion = Convert.ToString(Request.Form["IdLocacion"]).Trim();

                if (Locacion != "0")
                    ID_LOCA = Convert.ToInt32(Locacion);

                int ID_TYPE_ASSE = 0, ID_MANU = 0, ID_COMM_MODE, ID_MANU_MODE, ID_BUY_MODE, ID_COST_CENT, ILOTE;
                decimal DCost;
                DateTime ACQ_DATE;

                if (COD_ASSE.Length == 0) { msn = "Ingrese el código del Activo."; sw = 1; }
                else if (TYPE_ASSE.Length == 0) { msn = "Seleccione el tipo de Activo."; sw = 1; }
                else if (MANU.Length == 0) { msn = "Seleccione una Marca."; sw = 1; }
                else if (DATE_ACQ.Length == 0) { msn = "Ingrese una fecha de Adquisición."; sw = 1; }
                else if (SER_NUMB.Length == 0) { msn = "Ingrese el número de Serie."; sw = 1; }
                else if (BUY_MODE.Length == 0) { msn = "Seleccione el modo de compra."; sw = 1; }
                else if (int.TryParse(TYPE_ASSE, out ID_TYPE_ASSE))
                {
                    if (int.TryParse(MANU, out ID_MANU))
                    {
                        if (int.TryParse(BUY_MODE, out ID_BUY_MODE))
                        {
                            if (DateTime.TryParse(DATE_ACQ, out ACQ_DATE))
                            {
                                asse.COD_ASSE = COD_ASSE;
                                asse.SER_NUMB = SER_NUMB;
                                asse.ID_TYPE_ASSE = ID_TYPE_ASSE;
                                asse.ID_MANU = ID_MANU;
                                asse.ID_BUY_MODE = ID_BUY_MODE;
                                asse.SOLPED = SOLPED;
                                asse.NAM_EQUI = NAM_EQUI;
                                asse.ACQ_DATE = ACQ_DATE;
                                asse.MAC_ADRR = MAC_ADRR;
                                //asse.ID_SUBTYPE1 = (String.IsNullOrEmpty(ID_SUBTYPE1) ? 0 : Convert.ToInt32(ID_SUBTYPE1));
                                //asse.ID_SUBTYPE2 = (String.IsNullOrEmpty(ID_SUBTYPE2) ? 0 : Convert.ToInt32(ID_SUBTYPE2));
                                //asse.ID_SUBTYPE3 = (String.IsNullOrEmpty(ID_SUBTYPE3) ? 0 : Convert.ToInt32(ID_SUBTYPE3));
                                //asse.numeroTelefono = numeroTelefono;
                                //asse.idCompaniaTelefono = (String.IsNullOrEmpty(idCompaniaTelefono) ? 0 : Convert.ToInt32(idCompaniaTelefono));
                                asse.IpLocal = IpLocal;
                                asse.IpPublica = IpPublica;

                                if (!String.IsNullOrEmpty(DAT_STAR_LEAS))
                                    asse.DAT_STAR_LEAS = Convert.ToDateTime(DAT_STAR_LEAS);
                                
                                if (!String.IsNullOrEmpty(DAT_END_LEAS))
                                    asse.DAT_END_LEAS = Convert.ToDateTime(DAT_END_LEAS);

                                if (!String.IsNullOrEmpty(fechaFinGarantia))
                                    asse.fechaFinGarantia = Convert.ToDateTime(fechaFinGarantia);

                                if (!String.IsNullOrEmpty(IdContrato))
                                    asse.IdContrato = Convert.ToInt32(IdContrato);

                                try
                                {
                                    asse.ID_SYST_OPER_TYPE_ASSE = Convert.ToInt32(Request.Form["ID_SYST_OPER_TYPE_ASSE"]);
                                }
                                catch{ }
                            }
                        }
                        else { sw = 1; }
                    }
                    else { sw = 1; }
                }
                else { sw = 1; }

                if (sw == 1)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                      "if(top.uploadDone) top.uploadDone('ERROR','" + msn + "');}window.onload=init;</script>");
                }
                if (int.TryParse(COMM_MODE, out ID_COMM_MODE)) { asse.ID_COMM_MODE = ID_COMM_MODE; }
                if (int.TryParse(MANU_MODE, out ID_MANU_MODE)) { asse.ID_MANU_MODE = ID_MANU_MODE; }
                if (int.TryParse(COST_CENT, out ID_COST_CENT)) { asse.ID_COST_CENT = ID_COST_CENT; }
                if (int.TryParse(LOTE, out ILOTE)) { asse.LOTE = ILOTE; }
                if (Decimal.TryParse(COST, out DCost)) { asse.COST = DCost; }
            }

            //Haciendo el comentario campo obligatorio
            if (String.IsNullOrEmpty(client_asset.SUM_CLIE_ASSE))
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','Ingrese un comentario');}window.onload=init;</script>");
            }

            if (client_asset.ID_TYPE_CLIE_ASSE == 5)//Actualizando el Asset
            {
                try
                {
                    db.Entry(asse).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch
                {
                    return Content("<script type='text/javascript'> function init() {" +
                      "if(top.uploadDone) top.uploadDone('ERROR','No se pudo editar el Activo. Contacte al Administrador.');}window.onload=init;</script>");
                }
            }

            cal.DAT_END = DateTime.Now;
            db.Entry(cal).State = EntityState.Modified;
            db.SaveChanges();
            db.Entry(cal).State = EntityState.Detached;

            client_asset.ID_AREA = ID_AREA;
            client_asset.ID_LOCA = ID_LOCA;
            client_asset.DAT_STAR = DateTime.Now;
            client_asset.CREATE_DATE = DateTime.Now;
            client_asset.UserId = UserId;
            db.CLIENT_ASSET.Add(client_asset);
            db.SaveChanges();

            if (files != null)
            {
                foreach (var file in files)
                {
                    try
                    {
                        //var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                        var ATTA = new ATTACHED();
                        ATTA.NAM_ATTA = Path.GetFileNameWithoutExtension(file.FileName);
                        ATTA.EXT_ATTA = Path.GetExtension(file.FileName);
                        ATTA.ID_CLIE_ASSE = client_asset.ID_CLIE_ASSE;
                        ATTA.CREATE_ATTA = DateTime.Now;

                        //db.AddToATTACHEDs(ATTA);
                        db.ATTACHEDs.Add(ATTA);
                        db.SaveChanges();

                        file.SaveAs(Server.MapPath("~/Adjuntos/Asset/") + ATTA.NAM_ATTA + "_" + Convert.ToString(ATTA.ID_ATTA) + ATTA.EXT_ATTA);
                    }
                    catch
                    {

                    }
                }
            }
            if (client_asset.ID_TYPE_CLIE_ASSE == 3)//Generando un ticket si fuese el caso
            {
                bool ct = Convert.ToBoolean(db.OPTION_CONDITION.Single(x => x.ID_COND == cal.ID_COND && x.ID_OPTION == client_asset.ID_COND).CREATE_TICKET);
                if (ct)
                {
                    int ID_QUEU = Convert.ToInt32(Session["ID_QUEU"]);
                    int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
                    int ID_PER_ASSI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                    int ID_PER_ENTI = Convert.ToInt32(client_asset.ID_PERS_ENTI);
                    string COND = Convert.ToString(Request.Form["ID_COND_input"]).Trim();

                    var query = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == ID_PER_ENTI);
                    int ID_PRIO = Convert.ToInt32(query.ID_PRIO);
                    //int ID_AREA = Convert.ToInt32(query.ID_AREA);


                    var ticket = new TICKET();
                    string Code = null;
                    try
                    {
                        ticket.ID_ACCO = ID_ACCO;
                        ticket.ID_TYPE_TICK = 2;
                        ticket.ID_PERS_ENTI = ID_PER_ENTI;
                        ticket.ID_PERS_ENTI_END = ID_PER_ENTI;
                        ticket.ID_PERS_ENTI_ASSI = ID_PER_ASSI;
                        ticket.ID_AREA = ID_AREA;
                        ticket.ID_QUEU = ID_QUEU;
                        ticket.ID_PRIO = ID_PRIO;
                        ticket.ID_STAT = 1;
                        ticket.ID_STAT_END = 4;
                        ticket.ID_ASSE = client_asset.ID_ASSE;
                        ticket.ID_SOUR = 4;
                        ticket.FEC_TICK = DateTime.Now;
                        ticket.SUM_TICK = "TICKET CREATED TO REGISTER CHANGE STATEMENT OF ASSETS TO " + COND;
                        ticket.REM_CTRL_TICK = false;
                        ticket.UserId = UserId;
                        ticket.CREATE_TICK = DateTime.Now;
                        ticket.MODIFIED_TICK = DateTime.Now;
                        ticket.IS_PARENT = false;
                        ticket.ID_CATE = 181;

                        db.TICKETs.Add(ticket);
                        db.SaveChanges();

                        int id = Convert.ToInt32(ticket.ID_TICK);
                        db.Entry(ticket).State = EntityState.Detached;
                        Code = db.TICKETs.Single(t => t.ID_TICK == id).COD_TICK;
                    }
                    catch
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','Contacte al Administrador');}window.onload=init;</script>");
                    }
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('OK','" + Code + "');}window.onload=init;</script>");
                }
                else
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('OK','0');}window.onload=init;</script>");
                }
            }
            else
            {
                if (client_asset.ID_TYPE_CLIE_ASSE == 4)//Actualizando la configuracion
                {
                    var conf = new CONFIGURATION();
                    int id_pc = db.PART_TYPE_ASSET.Single(x => x.ID_PART_TYPE_ASSE == client_asset.ID_PART_TYPE_ASSE).ID_PARA_CONF.Value;
                    
                    var query = db.CONFIGURATIONs.Where(x => x.ID_PARA_CONF == id_pc && x.ID_ASSE == client_asset.ID_ASSE);
                    if (query.Count() > 0)
                    {                        
                        conf = db.CONFIGURATIONs.Single(x => x.ID_PARA_CONF == id_pc && x.ID_ASSE == client_asset.ID_ASSE);

                        conf.VAL_CONF = client_asset.VAL_PART_ASSE;
                        db.CONFIGURATIONs.Attach(conf);
                        db.Entry(conf).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else {
                        int id_cong_pare = db.CONFIGURATIONs.Single(x => x.ID_ASSE == client_asset.ID_ASSE && x.ID_PARA_CONF == 1).ID_CONF;
                        conf.ID_CONF_PARE = id_cong_pare;
                        conf.ID_ASSE = client_asset.ID_ASSE;
                        conf.ID_PARA_CONF = id_pc;
                        conf.VAL_CONF = client_asset.VAL_PART_ASSE;
                        conf.VIG_CONF = true;
                        conf.NIV_CONF = 2;

                        db.CONFIGURATIONs.Add(conf);
                        db.SaveChanges();
                    }
                }
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('OK','0');}window.onload=init;</script>");
            }
        }

        //
        // GET: /ClientAsset/Edit/5

        public ActionResult Edit(int id = 0)
        {
            CLIENT_ASSET client_asset = db.CLIENT_ASSET.Find(id);
            if (client_asset == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_ASSE = new SelectList(db.ASSETs, "ID_ASSE", "COD_ASSE", client_asset.ID_ASSE);
            ViewBag.ID_CLIE = new SelectList(db.CLIENTs, "ID_CLIE", "SEX_CLIE", client_asset.ID_CLIE);
            ViewBag.ID_COND = new SelectList(db.CONDITIONs, "ID_COND", "NAM_COND", client_asset.ID_COND);
            ViewBag.ID_LOCA = new SelectList(db.LOCATIONs, "ID_LOCA", "NAM_LOCA", client_asset.ID_LOCA);
            ViewBag.ID_TYPE_CLIE_ASSE = new SelectList(db.TYPE_CLIENT_ASSET, "ID_TYPE_ASSE_CLIEN", "NAM_TYPE_ASSE_CLIE", client_asset.ID_TYPE_CLIE_ASSE);
            return View(client_asset);
        }

        //
        // POST: /ClientAsset/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CLIENT_ASSET client_asset)
        {
            if (ModelState.IsValid)
            {
                db.Entry(client_asset).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID_ASSE = new SelectList(db.ASSETs, "ID_ASSE", "COD_ASSE", client_asset.ID_ASSE);
            ViewBag.ID_CLIE = new SelectList(db.CLIENTs, "ID_CLIE", "SEX_CLIE", client_asset.ID_CLIE);
            ViewBag.ID_COND = new SelectList(db.CONDITIONs, "ID_COND", "NAM_COND", client_asset.ID_COND);
            ViewBag.ID_LOCA = new SelectList(db.LOCATIONs, "ID_LOCA", "NAM_LOCA", client_asset.ID_LOCA);
            ViewBag.ID_TYPE_CLIE_ASSE = new SelectList(db.TYPE_CLIENT_ASSET, "ID_TYPE_ASSE_CLIEN", "NAM_TYPE_ASSE_CLIE", client_asset.ID_TYPE_CLIE_ASSE);
            return View(client_asset);
        }

        //
        // GET: /ClientAsset/Delete/5

        public ActionResult Delete(int id = 0)
        {
            CLIENT_ASSET client_asset = db.CLIENT_ASSET.Find(id);
            if (client_asset == null)
            {
                return HttpNotFound();
            }
            return View(client_asset);
        }

        //
        // POST: /ClientAsset/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CLIENT_ASSET client_asset = db.CLIENT_ASSET.Find(id);
            db.CLIENT_ASSET.Remove(client_asset);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}