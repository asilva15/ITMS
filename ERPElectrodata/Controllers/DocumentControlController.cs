using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.Threading;
using System.Globalization;
using System.IO;
using System.Data.Entity;
using ERPElectrodata.Objects;

namespace ERPElectrodata.Controllers
{
    public class DocumentControlController : Controller
    {
        public CoreEntities dbc = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();
        public ELECTRODATAEntities dbed = new ELECTRODATAEntities();
        private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        private TextInfo textInfo;
        
        //
        // GET: /DocumentControl/
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult Create()
        {        
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Create(DOCUMENT_CONTROL document, IEnumerable<HttpPostedFileBase> files)
        {
            string Error = "";
            int UserId = Convert.ToInt32(Session["UserId"]);
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int Cont = Convert.ToInt32(Request.Params["Cont"]);

            int numfiles = 0;
            if (files != null)
            {
                foreach (var file in files)
                {
                    numfiles = numfiles + 1;
                }
            }
            if (String.IsNullOrEmpty(document.SUB_DOCU))
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('ERROR','You must introduce your subject');}window.onload=init;</script>");
            }

            else if (document.ID_DOCU_CONT_TYPE.Value == null)
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('ERROR','You must select a type of document ');}window.onload=init;</script>");
            }
            else if (document.ID_DOCU_CONT_CARR == null)
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('ERROR','You must select a type of source ');}window.onload=init;</script>");
            }
            else if (document.ID_CIA == null)
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('ERROR','You must select a company');}window.onload=init;</script>");
            }
            else if (Cont == 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('ERROR','You must select at least one receiver');}window.onload=init;</script>");
            }
            else if (files == null)
            {

                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('ERROR','You must select a file');}window.onload=init;</script>");



            }
            else if (numfiles == 1)
            {

                try
                {
                    document.UserId = UserId;
                    document.ID_ACCO = ID_ACCO;
                    document.ID_PERS_ENTI_REMI = ID_PERS_ENTI;
                    document.CREATED = DateTime.Now;
                    document.ID_DOCU_CONT_ACTI = 1;
                    dbc.DOCUMENT_CONTROL.Add(document);
                    dbc.SaveChanges();

                    DOCUMENT_CONTROL_RECEIVER docurece = new DOCUMENT_CONTROL_RECEIVER();

                    for (int i = 1; i <= Cont; i++)
                    {
                        int ID_PERS_ENTI_RECE = Convert.ToInt32(Request.Params["RECEIVER" + i + ""]);

                        docurece.ID_PERS_ENTI = ID_PERS_ENTI_RECE;
                        docurece.ID_DOCU_CONT = document.ID_DOCU_CONT;

                        if (docurece.ID_PERS_ENTI != 0)
                        {
                            dbc.DOCUMENT_CONTROL_RECEIVER.Add(docurece);
                            dbc.SaveChanges();
                        }
                    }


                    foreach (var file in files)
                    {
                        try
                        {
                            var ATTA = new DOCUMENT_CONTROL_ATTACHED();

                            ATTA.NAM_DOCU_CONT_ATTA = Path.GetFileNameWithoutExtension(file.FileName);
                            ATTA.EXT_DOCU_CONT_ATTA = Path.GetExtension(file.FileName);
                            ATTA.CREATED = DateTime.Now;
                            ATTA.ID_PERS_ENTI = ID_PERS_ENTI;
                            ATTA.UserId = UserId;
                            ATTA.ID_DOCU_CONT = document.ID_DOCU_CONT;

                            dbc.DOCUMENT_CONTROL_ATTACHED.Add(ATTA);
                            dbc.SaveChanges();

                            file.SaveAs(Server.MapPath("~/Adjuntos/DocumentControl/") + ATTA.NAM_DOCU_CONT_ATTA + "_" + Convert.ToString(ATTA.ID_DOCU_CONT_ATTA) + ATTA.EXT_DOCU_CONT_ATTA);
                        }
                        catch
                        {

                        }
                    }
                }
                catch (Exception e)
                {
                    Error = (e.InnerException == null ? e.Message : e.InnerException.Message);

                    return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.uploadDone) top.uploadDone('ERROR','" + Error.ToString() + "');}window.onload=init;</script>");
                }

            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('ERROR','You must select a single file');}window.onload=init;</script>");
            }

            return Content("<script type='text/javascript'> function init() {" +
                     "if(top.uploadDone) top.uploadDone('OK','Your Document has been successfully saved');}window.onload=init;</script>");


        }

        [Authorize]
        public ActionResult CreateReception()
        {
            DOCUMENT_CONTROL dc = new DOCUMENT_CONTROL();
            dc.ID_DOCU_CONT_CREA = 1;
            return View(dc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateReception(DOCUMENT_CONTROL document, IEnumerable<HttpPostedFileBase> files)
        {
            string Error = "";
            int UserId = Convert.ToInt32(Session["UserId"]);
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int Cont = Convert.ToInt32(Request.Params["Cont"]);

            int numfiles = 0;

            if (files != null)
            {
                foreach (var file in files)
                {
                    numfiles = numfiles + 1;
                }
            }

            if (UserId == 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('ERROR','Please Close Session');}window.onload=init;</script>");
            }

            if (document.ID_DOCU_CONT_TYPE == null)
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('ERROR','You must select a type');}window.onload=init;</script>");
            }
            else
            {
                var dct = dbc.DOCUMENT_CONTROL_TYPE.Single(x => x.ID_DOCU_CONT_TYPE == document.ID_DOCU_CONT_TYPE.Value);
                
                if (dct.HAVE_SUB_TYPE.Value)
                {
                    if (document.ID_DOCU_CONT_SUB_TYPE == null)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','You must select a Sub type');}window.onload=init;</script>");
                    }
                }
                else
                {
                    document.ID_DOCU_CONT_SUB_TYPE = null;
                }

                if (dct.HAVE_SIDIGE.Value)
                {
                    if (String.IsNullOrEmpty(document.DOC_SIDIGE))
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','You must select a Document in SIDIGE');}window.onload=init;</script>");
                    }
                    else
                    {

                    }
                }
                else
                {
                    document.DOC_SIDIGE = null;
                }

            }



            if (String.IsNullOrEmpty(document.SUB_DOCU))
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('ERROR','You must introduce a description');}window.onload=init;</script>");
            }

            //else 
            else if (document.ID_DOCU_CONT_CARR == null)
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('ERROR','You must select a carrier');}window.onload=init;</script>");
            }
            else if (document.ID_CIA == null)
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('ERROR','You must select a company');}window.onload=init;</script>");
            }
            else if (Cont == 0)
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('ERROR','You must select at least one receiver');}window.onload=init;</script>");
            }
            //else if (files == null)
            //{
            //    return Content("<script type='text/javascript'> function init() {" +
            //    "if(top.uploadDone) top.uploadDone('ERROR','You must select a file');}window.onload=init;</script>");

            //}

            else if (numfiles < 2)
            {
                try
                {
                    document.UserId = UserId;
                    document.ID_ACCO = ID_ACCO;
                    document.ID_PERS_ENTI_REMI = ID_PERS_ENTI;
                    document.ID_DOCU_CONT_CREA = 1;
                    //document.ID_DOCU_CONT_SUB_TYPE = ();
                    document.ID_DOCU_CONT_ACTI = 1;
                    document.CREATED = DateTime.Now;
                    document.MODIFIED = DateTime.Now;

                    dbc.DOCUMENT_CONTROL.Add(document);
                    dbc.SaveChanges();

                    //Document Control Work
                    DOCUMENT_CONTROL_WORK dcw = new DOCUMENT_CONTROL_WORK();
                    dcw.ID_DOCU_CONT = document.ID_DOCU_CONT;
                    dcw.ID_PERS_ENTI = null;
                    dcw.ID_DOCU_CONT_ACTI = 1;
                    dcw.UserId = UserId;
                    dcw.SEND_MAIL = false;
                    dcw.CREATED = DateTime.Now;
                    dbc.DOCUMENT_CONTROL_WORK.Add(dcw);
                    dbc.SaveChanges();
                    //End Document Control Work


                    string mail_to = "";

                    for (int i = 1; i <= Cont; i++)
                    {
                        DOCUMENT_CONTROL_RECEIVER docurece = new DOCUMENT_CONTROL_RECEIVER();

                        int ID_PERS_ENTI_RECE = Convert.ToInt32(Request.Params["RECEIVER" + i + ""]);
                        int ID_ROL = Convert.ToInt32(Request.Params["ROL" + i + ""]);

                        docurece.ID_DOCU_CONT = document.ID_DOCU_CONT;
                        docurece.ID_PERS_ENTI = ID_PERS_ENTI_RECE;
                        docurece.ID_DOCU_CONT_STOR = 1;
                        docurece.ID_DOCU_CONT_ROL = 1;
                        docurece.ID_DOCU_CONT_ACTI = 1;
                        docurece.ID_DOCU_CONT_ROL = ID_ROL;
                        docurece.UserId = UserId;
                        docurece.CREATED = DateTime.Now;

                        if (ID_ROL == 2)
                        {
                            docurece.ID_CHAR = dbe.Obtiene_Nombre_UEN_AREA_CARGO(ID_PERS_ENTI_RECE).First().ID_CHAR_AREA;
                        }
                        

                        if (docurece.ID_PERS_ENTI != 0)
                        {
                            dbc.DOCUMENT_CONTROL_RECEIVER.Add(docurece);
                            dbc.SaveChanges();
                        }

                        try{
                            var pe = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == ID_PERS_ENTI_RECE);

                            mail_to += (pe.EMA_ELEC != null ? pe.EMA_ELEC : "");
                        }
                        catch
                        {

                        }
                        
                    }

                    if (numfiles == 1)
                    {
                        foreach (var file in files)
                        {
                            try
                            {
                                var ATTA = new DOCUMENT_CONTROL_ATTACHED();

                                ATTA.NAM_DOCU_CONT_ATTA = Path.GetFileNameWithoutExtension(file.FileName);
                                ATTA.EXT_DOCU_CONT_ATTA = Path.GetExtension(file.FileName);
                                ATTA.CREATED = DateTime.Now;
                                ATTA.ID_PERS_ENTI = ID_PERS_ENTI;
                                ATTA.UserId = UserId;
                                ATTA.ID_DOCU_CONT = document.ID_DOCU_CONT;

                                dbc.DOCUMENT_CONTROL_ATTACHED.Add(ATTA);
                                dbc.SaveChanges();

                                file.SaveAs(Server.MapPath("~/Adjuntos/DocumentControl/") + ATTA.NAM_DOCU_CONT_ATTA + "_" + Convert.ToString(ATTA.ID_DOCU_CONT_ATTA) + ATTA.EXT_DOCU_CONT_ATTA);
                            }
                            catch
                            {

                            }
                        }
                    }

                    if(!String.IsNullOrEmpty(document.DOC_SIDIGE)){

                        var doc_deta = dbed.MOVDETE1FIL.Where(x => x.NUMDOC.Contains(document.DOC_SIDIGE)).ToList();
                        foreach (MOVDETE1FIL liq in doc_deta)
                        {
                            DOCUMENT_CONTROL_DETAIL dc_d = new DOCUMENT_CONTROL_DETAIL();

                            dc_d.ID_DOCU_CONT = document.ID_DOCU_CONT;
                            dc_d.CODART = liq.CODART.Trim();
                            dc_d.DESART = liq.DESART.Trim();
                            dc_d.TIPDOC_P = liq.TIPDOC_P.Trim();
                            dc_d.NUM_P = liq.NUM_P.Trim();
                            dc_d.PRECOS = liq.PRECOS;
                            dc_d.CANTOT = liq.CANTOT;

                            dbc.DOCUMENT_CONTROL_DETAIL.Add(dc_d);
                            dbc.SaveChanges();
                        }
                    }

                    SendMail mail = new SendMail();
                    var xx = mail.NewDocumentControl(mail_to, document);

                }
                catch (Exception e)
                {
                    Error = (e.InnerException == null ? e.Message : e.InnerException.Message);

                    return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.uploadDone) top.uploadDone('ERROR','" + Error.ToString() + "');}window.onload=init;</script>");
                }

            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('ERROR','You must select a single file');}window.onload=init;</script>");
            }

            return Content("<script type='text/javascript'> function init() {" +
                     "if(top.uploadDone) top.uploadDone('OK','Your Document has been successfully saved');}window.onload=init;</script>");


        }

        public ActionResult ListType(int id = 0)
        {
            var query = (from dcct in dbc.DOCUMENT_CONTROL_CREATION_TYPE.Where(x=>x.ID_DOCU_CONT_CREA == id)
                         join dct in dbc.DOCUMENT_CONTROL_TYPE on dcct.ID_DOCU_CONT_TYPE equals dct.ID_DOCU_CONT_TYPE
                         where dct.ACTIVO == true
                         select new
                         {
                             ID_DOCU_CONT_TYPE = dct.ID_DOCU_CONT_TYPE,
                             NAM_DOCU_CONT_TYPE = dct.NAM_DOCU_CONT_TYPE.ToUpper(),
                             HAVE_SUB_TYPE = dct.HAVE_SUB_TYPE,
                             HAVE_SIDIGE = dct.HAVE_SIDIGE,
                         }).OrderBy(x=>x.NAM_DOCU_CONT_TYPE);


            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ObtenerTipos(int idCuenta = 0)
        {
            var query = (from t in dbc.TipoDocumentoes
                         where t.Activo == true && t.IdCuenta == idCuenta
                         select new
                         {
                             Id = t.Id,
                             Nombre = t.Nombre.ToUpper(),
                         })
                         .Union(
                           from dct in dbe.TYPE_DOCUMENT
                           where dct.REGISTER == 1
                           select new
                           {
                               Id = dct.ID_TYPE_DOCU,
                               Nombre = dct.NAM_DOCU.ToUpper(),
                           });

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }



        public ActionResult ListSubType(int id = 0)
        {
            var query = (from dcst in dbc.DOCUMENT_CONTROL_SUB_TYPE.Where(x => x.ID_DOCU_CONT_TYPE == id && x.ACTIVO == true)
                         .OrderBy(x=>x.NAM_DOCU_CONT_SUB_TYPE)
                         //join dct in dbc.DOCUMENT_CONTROL_TYPE on dcct.ID_DOCU_CONT_TYPE equals dct.ID_DOCU_CONT_TYPE
                         select new
                         {
                             ID_DOCU_CONT_SUB_TYPE = dcst.ID_DOCU_CONT_SUB_TYPE,
                             NAM_DOCU_CONT_SUB_TYPE = dcst.NAM_DOCU_CONT_SUB_TYPE.ToUpper(),
                             //HAVE_SUB_TYPE = dct.HAVE_SUB_TYPE,
                         });


            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListCarrier()
        {
            var query = (from ds in dbc.DOCUMENT_CONTROL_CARRIER
                         select new
                         {
                             ID_DOCU_CONT_CARR = ds.ID_DOCU_CONT_CARR,
                             NAM_DOCU_CONT_CARR = ds.NAM_DOCU_CONT_CARR,
                         });


            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListCIA()
        {
            var query = (from ce in dbe.CLASS_ENTITY.Where(x => x.COM_NAME != null)
                         select new
                         {
                             ce.ID_ENTI,
                             COM_NAME = ce.COM_NAME.ToUpper(),
                         }).OrderBy(x => x.COM_NAME);

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Reciever()
        {
            textInfo = cultureInfo.TextInfo;

            var query = (from ce in dbe.CLASS_ENTITY.ToList()
                         join pe in dbe.PERSON_ENTITY on ce.ID_ENTI equals pe.ID_ENTI2
                         join pc in dbe.PERSON_CONTRACT on pe.ID_PERS_ENTI equals pc.ID_PERS_ENTI
                         where pe.ID_ENTI1 == 9 && pc.VIG_CONT == true
                         select new
                         {
                             FIR_NAME = textInfo.ToTitleCase((ce.FIR_NAME.ToUpper() + " " + ce.LAS_NAME.ToUpper())),
                             ce.LAS_NAME,
                             ce.ID_ENTI,
                             pe.ID_PERS_ENTI,
                             pe.ID_FOTO,

                         }).OrderBy(x => x.FIR_NAME);

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Recibidor(int idCuenta)
        {
            textInfo = cultureInfo.TextInfo;

            var query = (from ce in dbe.CLASS_ENTITY.ToList()
                         join pe in dbe.PERSON_ENTITY on ce.ID_ENTI equals pe.ID_ENTI2
                         join pc in dbe.PERSON_CONTRACT on pe.ID_PERS_ENTI equals pc.ID_PERS_ENTI
                         join cu in dbe.ACCOUNT_ENTITY on  pe.ID_PERS_ENTI equals cu.ID_PERS_ENTI
                         where cu.ID_ACCO == idCuenta && pc.VIG_CONT == true && cu.VIG_ACCO_ENTI==true
                         select new
                         {
                             FIR_NAME = textInfo.ToTitleCase((ce.FIR_NAME.ToUpper() + " " + ce.LAS_NAME.ToUpper())),
                             ce.LAS_NAME,
                             ce.ID_ENTI,
                             pe.ID_PERS_ENTI,
                             pe.ID_FOTO,

                         }).OrderBy(x => x.FIR_NAME);

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }
       




        public ActionResult RecieverFilter()
        {
            string query = Convert.ToString(Request.Params["address"]);
            textInfo = cultureInfo.TextInfo;

            var result = (from ce in dbe.CLASS_ENTITY.ToList()
                         join pe in dbe.PERSON_ENTITY on ce.ID_ENTI equals pe.ID_ENTI2
                         join pc in dbe.PERSON_CONTRACT on pe.ID_PERS_ENTI equals pc.ID_PERS_ENTI
                         where pe.ID_ENTI1 == 9 && pc.VIG_CONT == true
                         where (ce.FIR_NAME+" "+ce.LAS_NAME).ToUpper().Contains(query.ToUpper())
                         select new
                         {
                             FIR_NAME = textInfo.ToTitleCase((ce.FIR_NAME.ToUpper() + " " + ce.LAS_NAME.ToUpper())),
                             ce.LAS_NAME,
                             ce.ID_ENTI,
                             pe.ID_PERS_ENTI,
                             pe.ID_FOTO,

                         }).OrderBy(x => x.FIR_NAME);

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }
        //
        // GET: /DocumentControl/Incoming
        public ActionResult Inbox()
        {
            textInfo = cultureInfo.TextInfo;

            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int admin = Convert.ToInt16(Session["AdminDocument"]);

            var listuser = (from lu in dbe.ACCOUNT_ENTITY
                            join pe in dbe.PERSON_ENTITY on lu.ID_PERS_ENTI equals pe.ID_PERS_ENTI
                            join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                            where lu.ID_ACCO == ID_ACCO
                            select new
                            {
                                lu.ID_PERS_ENTI,
                                Client = ce.FIR_NAME + " " + ce.LAS_NAME,
                                ce.UserId,
                                ID_FOTO = pe.ID_FOTO,
                            }).ToList();

            var listCIA = (from ce in dbe.CLASS_ENTITY.Where(x=>x.ID_TYPE_ENTI == 1)
                           //join ce in dbe.CLASS_ENTITY.Where(x => x.COM_NAME != null) on pe.ID_ENTI1 equals ce.ID_ENTI
                           select new
                           {
                               ID_ENTI = ce.ID_ENTI,
                               COM_NAME = ce.COM_NAME,
                           }).ToList();

            var query = dbc.DOCUMENT_CONTROL//.Where(x => x.ID_DOCU_CONT_ACTI == 1)
                    .Join(dbc.DOCUMENT_CONTROL_RECEIVER, dc => dc.ID_DOCU_CONT, dcr => dcr.ID_DOCU_CONT, (dc, dcr) => new
                    {
                        ID_DOCU_CONT = dc.ID_DOCU_CONT,
                        ID_PERS_ENTI_REMI = dc.ID_PERS_ENTI_REMI,
                        ID_DOCU_CONT_TYPE = dc.ID_DOCU_CONT_TYPE,
                        SUB_DOCU = dc.SUB_DOCU,
                        CREATED = dc.CREATED,
                        ID_PERS_ENTI = dcr.ID_PERS_ENTI,
                        ID_CHAR = dcr.ID_CHAR,
                        ID_DOCU_CONT_ACTI = dcr.ID_DOCU_CONT_ACTI,
                        ID_CIA = dc.ID_CIA,
                        NUM_DOCU = dc.NUM_DOCU,
                    });

            if (admin == 1)
            {
                query = query.Where(x => x.ID_DOCU_CONT_ACTI != 7);
            }
            else
            {
                var chart = dbe.Obtiene_Nombre_UEN_AREA_CARGO(ID_PERS_ENTI).First();
                query = query.Where(x => x.ID_CHAR == chart.ID_CHAR_AREA || x.ID_PERS_ENTI == ID_PERS_ENTI)
                    .Where(x => x.ID_DOCU_CONT_ACTI != 7);  
            }

            var result = (from d in query.OrderByDescending(x=>x.CREATED).Skip(skip).Take(take).ToList()
                          join pe in listuser on d.ID_PERS_ENTI_REMI equals pe.ID_PERS_ENTI
                          join tdc in dbc.DOCUMENT_CONTROL_TYPE on d.ID_DOCU_CONT_TYPE equals tdc.ID_DOCU_CONT_TYPE
                          join cia in listCIA on d.ID_CIA equals cia.ID_ENTI
                          join dca in dbc.DOCUMENT_CONTROL_ACTION on d.ID_DOCU_CONT_ACTI equals dca.ID_DOCU_CONT_ACTIV
                          select new {
                              SUB_DOCU = d.SUB_DOCU,
                              REMI = textInfo.ToTitleCase(pe.Client.ToLower()),
                              NAM_TYPE_DOCU_CONT = tdc.NAM_DOCU_CONT_TYPE,
                              CREATED_DATE = String.Format("{0:d}", d.CREATED) + " " + String.Format("{0:HH:mm}", d.CREATED),
                              ID_DOCU_CONT = d.ID_DOCU_CONT,
                              ID_FOTO = Convert.ToString(pe.ID_FOTO)+".jpg",
                              NAM_DOCU_CONT_TYPE = tdc.NAM_DOCU_CONT_TYPE,
                              NUM_DOCU = d.NUM_DOCU,
                              COM_NAME = cia.COM_NAME,
                              NAM_DOCU_CONT_ACTI = dca.NAM_DOCU_CONT_ACTI,
                              COLOR = dca.COLOR,
                          });

            return Json(new {Data = result,Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Upload()
        {
            try
            {
                int UserId = Convert.ToInt32(Session["UserId"].ToString());

                textInfo = cultureInfo.TextInfo;

                int skip = Convert.ToInt32(Request.Params["skip"].ToString());
                int take = Convert.ToInt32(Request.Params["take"].ToString());

                //int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);

                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

                //var chart = dbe.Obtiene_Nombre_UEN_AREA_CARGO(ID_PERS_ENTI).First();

                var listuser = (from lu in dbe.ACCOUNT_ENTITY
                                join pe in dbe.PERSON_ENTITY on lu.ID_PERS_ENTI equals pe.ID_PERS_ENTI
                                join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                                where lu.ID_ACCO == ID_ACCO
                                select new
                                {
                                    lu.ID_PERS_ENTI,
                                    Client = ce.FIR_NAME + " " + ce.LAS_NAME,
                                    ce.UserId,
                                    ID_FOTO = pe.ID_FOTO,
                                }).ToList();

                var listCIA = (from ce in dbe.CLASS_ENTITY.Where(x => x.ID_TYPE_ENTI == 1)
                               //join ce in dbe.CLASS_ENTITY.Where(x => x.COM_NAME != null) on pe.ID_ENTI1 equals ce.ID_ENTI
                               select new
                               {
                                   ID_ENTI = ce.ID_ENTI,
                                   COM_NAME = ce.COM_NAME,
                               }).ToList();

                var query = dbc.DOCUMENT_CONTROL.Where(x => x.UserId == UserId);
                    //.Join(dbc.DOCUMENT_CONTROL_RECEIVER, dc => dc.ID_DOCU_CONT, dcr => dcr.ID_DOCU_CONT, (dc, dcr) => new
                    //{
                    //    ID_DOCU_CONT = dc.ID_DOCU_CONT,
                    //    ID_PERS_ENTI_REMI = dc.ID_PERS_ENTI_REMI,
                    //    ID_DOCU_CONT_TYPE = dc.ID_DOCU_CONT_TYPE,
                    //    SUB_DOCU = dc.SUB_DOCU,
                    //    CREATED = dc.CREATED,
                    //    ID_PERS_ENTI = dcr.ID_PERS_ENTI,
                    //    ID_CHAR = dcr.ID_CHAR,
                    //    ID_DOCU_CONT_ACTI = dcr.ID_DOCU_CONT_ACTI,
                    //    ID_CIA = dc.ID_CIA,
                    //    NUM_DOCU = dc.NUM_DOCU,
                    //}).Where(x => x.ID_CHAR == chart.ID_CHAR_AREA || x.ID_PERS_ENTI == ID_PERS_ENTI)
                    //.Where(x => x.ID_DOCU_CONT_ACTI == 1);

                var result = (from d in query.OrderBy(x=>x.ID_DOCU_CONT_ACTI).ThenByDescending(x => x.CREATED).Skip(skip).Take(take).ToList()
                              join pe in listuser on d.ID_PERS_ENTI_REMI equals pe.ID_PERS_ENTI
                              join tdc in dbc.DOCUMENT_CONTROL_TYPE on d.ID_DOCU_CONT_TYPE equals tdc.ID_DOCU_CONT_TYPE
                              join cia in listCIA on d.ID_CIA equals cia.ID_ENTI
                              join dca in dbc.DOCUMENT_CONTROL_ACTION on d.ID_DOCU_CONT_ACTI equals dca.ID_DOCU_CONT_ACTIV
                              select new
                              {
                                  SUB_DOCU = d.SUB_DOCU,
                                  REMI = textInfo.ToTitleCase(pe.Client.ToLower()),
                                  NAM_TYPE_DOCU_CONT = tdc.NAM_DOCU_CONT_TYPE,
                                  CREATED_DATE = String.Format("{0:d}", d.CREATED) + " " + String.Format("{0:HH:mm}", d.CREATED),
                                  ID_DOCU_CONT = d.ID_DOCU_CONT,
                                  ID_FOTO = Convert.ToString(pe.ID_FOTO) + ".jpg",
                                  NAM_DOCU_CONT_TYPE = tdc.NAM_DOCU_CONT_TYPE,
                                  NUM_DOCU = d.NUM_DOCU,
                                  COM_NAME = cia.COM_NAME,
                                  NAM_DOCU_CONT_ACTI = dca.NAM_DOCU_CONT_ACTI,
                                  COLOR = dca.COLOR,
                              });

                return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { Data = "", Count = 0 }, JsonRequestBehavior.AllowGet);
            }

            
        }

        public ActionResult Storage()
        {
            textInfo = cultureInfo.TextInfo;

            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            var listuser = (from lu in dbe.ACCOUNT_ENTITY
                            join pe in dbe.PERSON_ENTITY on lu.ID_PERS_ENTI equals pe.ID_PERS_ENTI
                            join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                            where lu.ID_ACCO == ID_ACCO
                            select new
                            {
                                lu.ID_PERS_ENTI,
                                Client = ce.FIR_NAME + " " + ce.LAS_NAME,
                                ce.UserId,
                                ID_FOTO = pe.ID_FOTO,

                            }).ToList();

            var query = dbc.DOCUMENT_CONTROL;//.Where(x => x.ID_STAT_DOCU_CONT == 1);

            var result = (from d in query.ToList()
                          join pe in listuser on d.ID_PERS_ENTI_REMI equals pe.ID_PERS_ENTI
                          join tdc in dbc.DOCUMENT_CONTROL_TYPE on d.ID_DOCU_CONT_TYPE equals tdc.ID_DOCU_CONT_TYPE
                          select new
                          {
                              SUB_DOCU = d.SUB_DOCU,
                              REMI = textInfo.ToTitleCase(pe.Client.ToLower()),
                              NAM_TYPE_DOCU_CONT = tdc.NAM_DOCU_CONT_TYPE,
                              CREATED_DATE = String.Format("{0:d}", d.CREATED) + " " + String.Format("{0:HH:mm}", d.CREATED),
                              ID_DOCU_CONT = d.ID_DOCU_CONT,
                              ID_FOTO = Convert.ToString(pe.ID_FOTO) + ".jpg"
                          });

            return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Rol()
        {
            var query = (from x in dbc.DOCUMENT_CONTROL_ROL.ToList()
                         select new {
                             ID_DOCU_CONT_ROL = x.ID_DOCU_CONT_ROL,
                             NAM_DOCU_CONT_ROL = x.NAM_DOCU_CONT_ROL,
                             ACR_DOC_CONT_ROL = x.ACR_DOC_CONT_ROL,
                             COL_DOC_CONT_ROL = x.COL_DOC_CONT_ROL,
                         });

            return Json(new {Data=query,Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Download(int id = 0)
        {
            try
            {
                int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                int UserId = Convert.ToInt32(Session["UserId"]);
                int admin = Convert.ToInt16(Session["SUPERVISOR_CONTROLDOCUMENTARIO"]);

                if (UserId <= 0)
                {
                    return Content("Please Close Session");
                }

                //if (admin == 1)
                var doc_rec = dbc.DOCUMENT_CONTROL_RECEIVER
                            .Where(x => x.ID_DOCU_CONT == id);

                if(admin == 0){
                    var ar = dbe.Obtiene_Nombre_UEN_AREA_CARGO(ID_PERS_ENTI).First();

                    doc_rec = dbc.DOCUMENT_CONTROL_RECEIVER
                            .Where(x => x.ID_DOCU_CONT == id)
                            .Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI || x.ID_CHAR == ar.ID_CHAR);
                }

                if (doc_rec.Count() > 0)
                {
                    DOCUMENT_CONTROL_RECEIVER o_doc_rec = doc_rec.First();

                    var query = dbc.DOCUMENT_CONTROL_WORK
                        .Where(x => x.ID_DOCU_CONT_RECE == o_doc_rec.ID_DOCU_CONT_RECE)
                        .Where(x => x.ID_DOCU_CONT_ACTI == 2);

                    var atta = dbc.DOCUMENT_CONTROL_ATTACHED.FirstOrDefault(x => x.ID_DOCU_CONT == id);

                    if (query.Count() == 0)
                    {
                        DOCUMENT_CONTROL_WORK wdc = new DOCUMENT_CONTROL_WORK();

                        wdc.ID_DOCU_CONT_RECE = o_doc_rec.ID_DOCU_CONT_RECE;
                        wdc.ID_PERS_ENTI = ID_PERS_ENTI;
                        wdc.ID_DOCU_CONT_ACTI = 2;
                        wdc.UserId = UserId;
                        wdc.SEND_MAIL = false;
                        wdc.CREATED = DateTime.Now;
                        wdc.ID_DOCU_CONT = id;
                        dbc.DOCUMENT_CONTROL_WORK.Add(wdc);
                        dbc.SaveChanges();

                        DOCUMENT_CONTROL dc = dbc.DOCUMENT_CONTROL.Single(x => x.ID_DOCU_CONT == id);
                        dc.ID_DOCU_CONT_ACTI = 2;
                        dbc.Entry(dc).State = EntityState.Modified;
                        dbc.SaveChanges();

                        o_doc_rec.ID_DOCU_CONT_ACTI = 2;
                        dbc.Entry(o_doc_rec).State = EntityState.Modified;
                        dbc.SaveChanges();
                    }
                    else
                    {

                    }

                    string id_atta = Convert.ToString(atta.ID_DOCU_CONT_ATTA);

                    byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath("~/Adjuntos/DocumentControl/" + atta.NAM_DOCU_CONT_ATTA + "_" + id_atta + atta.EXT_DOCU_CONT_ATTA));
                    string fileName = atta.NAM_DOCU_CONT_ATTA + atta.EXT_DOCU_CONT_ATTA;
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }
                else
                {
                    return Content("You Not Have Privilegies");
                }
            }
            catch
            {
                return Content("Please Close Session");
            }
        }

        public ActionResult Confirm(int id = 0)
        {
            try
            {
                int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                int UserId = Convert.ToInt32(Session["UserId"]);

                if (UserId <= 0)
                {
                    return Content("Please Close Session");
                }

                var ar = dbe.Obtiene_Nombre_UEN_AREA_CARGO(ID_PERS_ENTI).First();

                var doc_rec = dbc.DOCUMENT_CONTROL_RECEIVER
                    .Where(x => x.ID_DOCU_CONT == id)
                    .Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI || x.ID_CHAR == ar.ID_CHAR);

                if (doc_rec.Count() > 0)
                {
                    DOCUMENT_CONTROL_RECEIVER o_doc_rec = doc_rec.First();

                    var query = dbc.DOCUMENT_CONTROL_WORK
                        .Where(x => x.ID_DOCU_CONT_RECE == o_doc_rec.ID_DOCU_CONT_RECE)
                        .Where(x => x.ID_DOCU_CONT_ACTI == 3);

                    //var atta = dbc.DOCUMENT_CONTROL_ATTACHED.FirstOrDefault(x => x.ID_DOCU_CONT == id);
                    

                    if (query.Count() == 0)
                    {
                        DOCUMENT_CONTROL_WORK wdc = new DOCUMENT_CONTROL_WORK();

                        wdc.ID_DOCU_CONT_RECE = o_doc_rec.ID_DOCU_CONT_RECE;
                        wdc.ID_PERS_ENTI = ID_PERS_ENTI;
                        wdc.ID_DOCU_CONT_ACTI = 3;
                        wdc.UserId = UserId;
                        wdc.SEND_MAIL = false;
                        wdc.CREATED = DateTime.Now;
                        wdc.ID_DOCU_CONT = id;
                        dbc.DOCUMENT_CONTROL_WORK.Add(wdc);
                        dbc.SaveChanges();

                        DOCUMENT_CONTROL dc = dbc.DOCUMENT_CONTROL.Single(x=>x.ID_DOCU_CONT == id);
                        dc.ID_DOCU_CONT_ACTI = 3;
                        dbc.Entry(dc).State = EntityState.Modified;
                        dbc.SaveChanges();

                        o_doc_rec.ID_DOCU_CONT_ACTI = 3;
                        dbc.Entry(o_doc_rec).State = EntityState.Modified;
                        dbc.SaveChanges();

                    }
                    else
                    {
                        return Content("OK");
                    }

                    return Content("OK");
                }
                else
                {
                    return Content("Please Close Session");
                }

                //return Content("OK");
            }
            catch
            {
                return Content("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Shared()
        {
            try
            {
                int UserId = Convert.ToInt32(Session["UserId"]);
                int ID_PERS_ENTI = Convert.ToInt32(Request.Params["id"]);
                int ID_DOCU_CONT = Convert.ToInt32(Request.Params["id_d"]);

                int flag = dbc.DOCUMENT_CONTROL_RECEIVER.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI)
                    .Where(x=>x.ID_DOCU_CONT == ID_DOCU_CONT)
                    .Count();

                if(flag==0 && UserId > 0){
                    DOCUMENT_CONTROL_RECEIVER dcr = new DOCUMENT_CONTROL_RECEIVER();
                    dcr.ID_PERS_ENTI = ID_PERS_ENTI;
                    dcr.ID_DOCU_CONT = ID_DOCU_CONT;
                    dcr.ID_DOCU_CONT_ACTI = 4;
                    dcr.ID_DOCU_CONT_STOR = 1;
                    dcr.ID_DOCU_CONT_ROL = 1;
                    dcr.UserId = UserId;
                    dcr.CREATED = DateTime.Now;

                    dbc.DOCUMENT_CONTROL_RECEIVER.Add(dcr);
                    dbc.SaveChanges();

                    DOCUMENT_CONTROL_WORK dcw = new DOCUMENT_CONTROL_WORK();
                    dcw.ID_DOCU_CONT = ID_DOCU_CONT;
                    dcw.ID_PERS_ENTI = ID_PERS_ENTI;
                    dcw.ID_DOCU_CONT_ACTI = 4;
                    dcw.UserId = UserId;
                    dcw.SEND_MAIL = false;
                    dcw.CREATED = DateTime.Now;
                    dbc.DOCUMENT_CONTROL_WORK.Add(dcw);
                    dbc.SaveChanges();
                }
                else{
                    return Content("User Document Control");
                }
                
                return Content("OK");
            }
            catch
            {
                return Content("ERROR");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendValidate()
        {
            try
            {
                int UserId = Convert.ToInt32(Session["UserId"]);
                int ID_PERS_ENTI = Convert.ToInt32(Request.Params["id"]);
                int ID_DOCU_CONT = Convert.ToInt32(Request.Params["id_d"]);

                int flag = dbc.DOCUMENT_CONTROL_RECEIVER.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI)
                    .Where(x => x.ID_DOCU_CONT == ID_DOCU_CONT)
                    .Count();

                if (flag == 0 && UserId > 0)
                {
                    DOCUMENT_CONTROL_RECEIVER dcr = new DOCUMENT_CONTROL_RECEIVER();
                    dcr.ID_PERS_ENTI = ID_PERS_ENTI;
                    dcr.ID_DOCU_CONT = ID_DOCU_CONT;
                    dcr.ID_DOCU_CONT_ACTI = 4;
                    dcr.ID_DOCU_CONT_STOR = 1;
                    dcr.ID_DOCU_CONT_ROL = 1;
                    dcr.UserId = UserId;
                    dcr.CREATED = DateTime.Now;

                    dbc.DOCUMENT_CONTROL_RECEIVER.Add(dcr);
                    dbc.SaveChanges();

                    DOCUMENT_CONTROL_WORK dcw = new DOCUMENT_CONTROL_WORK();
                    dcw.ID_DOCU_CONT = ID_DOCU_CONT;
                    dcw.ID_PERS_ENTI = ID_PERS_ENTI;
                    dcw.ID_DOCU_CONT_ACTI = 5;
                    dcw.UserId = UserId;
                    dcw.SEND_MAIL = false;
                    dcw.CREATED = DateTime.Now;
                    dbc.DOCUMENT_CONTROL_WORK.Add(dcw);
                    dbc.SaveChanges();
                }
                else
                {
                    return Content("User Document Control");
                }

                return Content("OK");
            }
            catch
            {
                return Content("ERROR");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Validate()
        {
            try
            {
                int UserId = Convert.ToInt32(Session["UserId"]);
                int ID_PERS_ENTI = Convert.ToInt32(Request.Params["id"]);
                int ID_DOCU_CONT = Convert.ToInt32(Request.Params["id_d"]);

                //int flag = dbc.DOCUMENT_CONTROL_RECEIVER.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI)
                //    .Where(x => x.ID_DOCU_CONT == ID_DOCU_CONT)
                //    .Count();

                //if (flag == 0 && UserId > 0)
                //{

                    DOCUMENT_CONTROL_WORK dcw = new DOCUMENT_CONTROL_WORK();
                    dcw.ID_DOCU_CONT = ID_DOCU_CONT;
                    dcw.ID_PERS_ENTI = ID_PERS_ENTI;
                    dcw.ID_DOCU_CONT_ACTI = 6;
                    dcw.UserId = UserId;
                    dcw.SEND_MAIL = false;
                    dcw.CREATED = DateTime.Now;
                    dbc.DOCUMENT_CONTROL_WORK.Add(dcw);
                    dbc.SaveChanges();
                //}
                //else
                //{
                //    return Content("User Document Control");
                //}

                return Content("OK");
            }
            catch
            {
                return Content("ERROR");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete()
        {
            try
            {
                int UserId = Convert.ToInt32(Session["UserId"]);
                int ID_PERS_ENTI = Convert.ToInt32(Request.Params["id"]);
                int ID_DOCU_CONT = Convert.ToInt32(Request.Params["id_d"]);

                //int flag = dbc.DOCUMENT_CONTROL_RECEIVER.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI)
                //    .Where(x => x.ID_DOCU_CONT == ID_DOCU_CONT)
                //    .Count();

                //if (flag == 0 && UserId > 0)
                //{

                DOCUMENT_CONTROL_WORK dcw = new DOCUMENT_CONTROL_WORK();
                dcw.ID_DOCU_CONT = ID_DOCU_CONT;
                dcw.ID_PERS_ENTI = ID_PERS_ENTI;
                dcw.ID_DOCU_CONT_ACTI = 7;
                dcw.UserId = UserId;
                dcw.SEND_MAIL = false;
                dcw.CREATED = DateTime.Now;
                dbc.DOCUMENT_CONTROL_WORK.Add(dcw);
                dbc.SaveChanges();
                //}
                //else
                //{
                //    return Content("User Document Control");
                //}

                return Content("OK");
            }
            catch
            {
                return Content("ERROR");
            }
        }
    }
}
