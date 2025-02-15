using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using ERPElectrodata.Models;
using System.Web.Security;
using WebMatrix.WebData;
using System.Text;

namespace ERPElectrodata.Controllers
{
    [Authorize]
    public class KnowledgeController : Controller
    {
        //
        // GET: /Knowledge/
        EntityEntities dbe = new EntityEntities();
        CoreEntities dbc = new CoreEntities();

        [Authorize]
        public ActionResult Index(int id=0)
        {
            try {
                if (Convert.ToInt32(Session["USUARIO_EXTERNO_TICKET"]) == 0)
                {
                    Session["MAIN"] = "mp7";
                    ViewBag.id = id;

                    string IdPersEnti = Convert.ToString(Session["ID_PERS_ENTI"]);
                    int flagArchivos = 0;
                    int ctd = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 20 && x.VAL_ACCO_PARA == IdPersEnti && x.VIG_ACCO_PARA == true).Count();
                    if (ctd >0 || Convert.ToInt32(Session["SUPERVISOR_SERVICEDESK"]) == 1)
                    {
                        flagArchivos = 1;
                    }
                    ViewBag.AdjuntarArchivos = flagArchivos;
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Error");
                }
            }
            catch
            {
                if (Request.Cookies["ASP.NET_SessionId"] != null)
                {
                    HttpCookie myCookie = new HttpCookie("ASP.NET_SessionId");
                    HttpCookie myCookie2 = new HttpCookie("__RequestVerificationToken");
                    HttpCookie myCookie3 = new HttpCookie(".ASPXAUTH");


                    myCookie.Expires = DateTime.Now.AddDays(-15);
                    myCookie2.Expires = DateTime.Now.AddDays(-15);
                    myCookie3.Expires = DateTime.Now.AddDays(-15);

                    Response.Cookies.Add(myCookie);
                    Response.Cookies.Add(myCookie2);
                    Response.Cookies.Add(myCookie3);
                }

                return RedirectToAction("Index");
            }
        }

        [Authorize]
        public ActionResult IndexX()
        {
            try
            {
                Session["MAIN"] = "mp7";
                string IdPersEnti = Convert.ToString(Session["ID_PERS_ENTI"]);
                int flagArchivos = 0;
                int ctd = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 20 && x.VAL_ACCO_PARA == IdPersEnti && x.VIG_ACCO_PARA == true).Count();
                if (ctd > 0 || Convert.ToInt32(Session["SUPERVISOR_SERVICEDESK"]) == 1)
                {
                    flagArchivos = 1;
                }
                ViewBag.AdjuntarArchivos = flagArchivos;
                return View();
            }
            catch
            {
                if (Request.Cookies["ASP.NET_SessionId"] != null)
                {
                    HttpCookie myCookie = new HttpCookie("ASP.NET_SessionId");
                    HttpCookie myCookie2 = new HttpCookie("__RequestVerificationToken");
                    HttpCookie myCookie3 = new HttpCookie(".ASPXAUTH");


                    myCookie.Expires = DateTime.Now.AddDays(-15);
                    myCookie2.Expires = DateTime.Now.AddDays(-15);
                    myCookie3.Expires = DateTime.Now.AddDays(-15);

                    Response.Cookies.Add(myCookie);
                    Response.Cookies.Add(myCookie2);
                    Response.Cookies.Add(myCookie3);
                }

                return RedirectToAction("IndexX");
            }
        }

        public ActionResult MenuAreas() {
            var result = (from a in dbe.AREAs.Where(x => x.ID_ACCO == 4 && x.NIV_AREA == 2)
                          select new { 
                              NOM_AREA = a.NOM_AREA.ToLower(),
                              a.ID_AREA
                          });
            return Json(new { Data = result, Count = result.Count()},JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewTreeKnowledge(int id = 0) {
            ViewBag.ID_AREA = id;
            return View();
        }
        public ActionResult VerDocumento(string id = "", string id1 = "")
        {
            ViewBag.Flag = id;
            ViewBag.Documento = id1;
            return View();
        }
        public FileResult DescargarArchivo(string id = "", string id1 = "")
        {
            try
            {
                string ruta = "";
                if (id == "1")
                {
                    ruta = "~/Adjuntos/Knowledge/";
                }
                else if (id == "2")
                {
                    ruta = "~/Adjuntos/Knowledge/Template/";
                }
                FileStream fileStream = System.IO.File.OpenRead(Server.MapPath(ruta + id1));
                MemoryStream storeStream = new MemoryStream();

                storeStream.SetLength(fileStream.Length);
                fileStream.Read(storeStream.GetBuffer(), 0, (int)fileStream.Length);
                byte[] byteArray = storeStream.ToArray();

                storeStream.Flush();
                fileStream.Close();
                storeStream.Close();

                Random r = new Random();
                int aleatorio = r.Next(10000, 99999);

                Response.Clear();

                //Response.ContentType = "application/octet-stream";
                if ((id1.ToLower()).Contains(".pdf"))
                {
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", "filename=" + Convert.ToString(aleatorio) + ".pdf");
                    Response.BinaryWrite(byteArray);
                    Response.End();
                    return File(storeStream.GetBuffer(), "xml");
                }
                else if ((id1.ToLower()).Contains(".txt"))
                {
                    Response.ContentType = "text/text";
                    Response.AddHeader("content-disposition", "filename=" + Convert.ToString(aleatorio) + ".txt");
                    Response.BinaryWrite(byteArray);
                    Response.End();
                    return File(storeStream.GetBuffer(), "xml");
                }
                else if ((id1.ToLower()).Contains(".jpg"))
                {
                    Response.ContentType = "image/jpeg";
                    Response.AddHeader("content-disposition", "filename=" + Convert.ToString(aleatorio) + ".jpg");
                    Response.BinaryWrite(byteArray);
                    Response.End();
                    return File(storeStream.GetBuffer(), "xml");
                }
                else if ((id1.ToLower()).Contains(".png"))
                {
                    Response.ContentType = "image/png";
                    Response.AddHeader("content-disposition", "filename=" + Convert.ToString(aleatorio) + ".png");
                    Response.BinaryWrite(byteArray);
                    Response.End();
                    return File(storeStream.GetBuffer(), "xml");
                }
                else
                {
                    string filename = ruta + id1;
                    return File(filename, "application/pdf", id1);
                }

            }
            catch
            {
                string text = "Error.";
                var stream = new MemoryStream(Encoding.ASCII.GetBytes(text));

                return File(stream, "text/plain", "Error.txt");
            }

        }
        public ActionResult TreeKnowledge(int id = 0)
        {
            string ID_PARA = Convert.ToString(Request.Params["ID_PARA"]);
            if (ID_PARA == null)
            {
                int i = 0;
                string[] rolesArray = Roles.GetRolesForUser();
                string[] roles = new String[rolesArray.Count() + 1];                
                foreach (string xc in rolesArray)
                {
                    roles[i] = xc;
                    i++;
                }
                roles[i] = "ALL";

                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

                var query = (from q in dbc.KNOWLEDGE_CATEGORY.Where(x=>x.VIG_CATE== true && x.ID_KNOW_CATE_PARE == null && x.ID_ACCO == ID_ACCO).ToList()
                             where roles.Contains((q.ROL_ACCE == null ? "ALL" : q.ROL_ACCE).ToString())
                             select new
                             {
                                 ID_PARA = q.REG_FOLD == true ? "ISO*" + Convert.ToString(q.ID_KNOW_CATE) : Convert.ToString(q.ID_KNOW_CATE),
                                 NAME_PARA = q.REG_FOLD == true ? q.NAM_CATE : q.NAM_CATE.ToLower(),
                                 HAS_VALUE = (CountKnowledgeByCategory(Convert.ToBoolean(q.REG_FOLD), q.ID_KNOW_CATE, id) > 0 ? true : false),
                                 DESC_PARA = q.REG_FOLD == true ? "ISO 0" : "",
                                 expanded = false,
                                 ATTACH = false,
                             });

                return Json(query.OrderBy(x => x.NAME_PARA), JsonRequestBehavior.AllowGet);
            }
            else if (ID_PARA.Contains("|")) {
                string idpe = Convert.ToString(Session["ID_PERS_ENTI"]);
                int UserId = Convert.ToInt32(Session["UserId"]);
                string[] para = ID_PARA.Split('|');
                int ANIO = Convert.ToInt32(para[0]);
                int ID_CATE = Convert.ToInt32(para[1]);

                int ctd = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 20 && x.VAL_ACCO_PARA == idpe && x.VIG_ACCO_PARA == true).Count();
                var queryUser = dbe.CLASS_ENTITY;

                var qKnow = (from a in dbc.KNOWLEDGEs
                             join b in dbc.KNOWLEDGE_CATEGORY.Where(x => x.ALL_AREA == true && x.ID_KNOW_CATE == ID_CATE) on a.ID_KNOW_CATE equals b.ID_KNOW_CATE
                             select a);

                var query = (from q in dbc.KNOWLEDGEs.Where(x => x.ID_KNOW_CATE == ID_CATE && x.VIG_KNOW == true && x.ID_AREA == id
                                 || (from a in qKnow select a.ID_KNOW).Contains(x.ID_KNOW)).ToList()
                             join a in dbc.ACCOUNTING_MONTH on q.ID_ACCO_MONT equals a.ID_ACCO_MONT
                             where a.ID_ACCO_YEAR == ANIO
                             join u in queryUser on q.UserId equals u.UserId
                             join b in dbc.KNOWLEDGE_CATEGORY on q.ID_KNOW_CATE equals b.ID_KNOW_CATE                             
                             select new
                             {
                                 ID_PARA = q.ID_KNOW,
                                 NAME_PARA = q.NAM_KNOW.ToLower(),
                                 DESC_PARA = (q.DESC_KNOW == null ? "" : q.DESC_KNOW.ToLower()),
                                 HAS_VALUE = false,
                                 expanded = false,
                                 EXT_ATTA = (q.EXT_ATTA == null ? "pdf.png" : q.EXT_ATTA.Substring(1, q.EXT_ATTA.Length - 1) + ".png"),
                                 q.ID_ACCO_MONT,
                                 NAM_ATTA = q.NAM_ATTA,
                                 NAM_FILE = (q.EXT_ATTA).ToLower()==".pdf" ||(q.EXT_ATTA).ToLower()==".png" ||
                                            (q.EXT_ATTA).ToLower()==".jpg" ||(q.EXT_ATTA).ToLower()==".txt"
                                            ? "/Knowledge/VerDocumento/1/" + q.NAM_ATTA + "_" + q.ID_KNOW.ToString() + q.EXT_ATTA : "/Knowledge/DescargarArchivo/1/" + (q.NAM_ATTA + "_" + q.ID_KNOW.ToString() + q.EXT_ATTA),
                                 ATTACH = true,
                                 DOWNLOAD = (b.ANY_DOWN == true ? true : q.UserId == UserId ? true : RolAdminitratorOrPMO()),
                                 DELETE = (ctd == 0 ? false : true),
                                 USER_NAME = u.FIR_NAME.Substring(0, 1).ToUpper() + u.FIR_NAME.Substring(1, u.FIR_NAME.Length - 1).ToLower() + " " +
                                            u.LAS_NAME.Substring(0, 1).ToUpper() + u.LAS_NAME.Substring(1, u.LAS_NAME.Length - 1).ToLower(),
                             });

                return Json(query.OrderBy(x => x.ID_ACCO_MONT), JsonRequestBehavior.AllowGet);
            }
            else if (ID_PARA.Contains("*"))
            {
                string idpe = Convert.ToString(Session["ID_PERS_ENTI"]);
                int UserId = Convert.ToInt32(Session["UserId"]);
                string[] para = ID_PARA.Split('*');
                int ID_CATE = Convert.ToInt32(para[1]);

                var query = (from a in dbc.KNOWLEDGE_CATEGORY.Where(x => x.ID_KNOW_CATE_PARE == ID_CATE && 
                                                                    x.VIG_CATE == true).ToList()
                             select new {
                                 ID_PARA = "ISO*" + Convert.ToString(a.ID_KNOW_CATE),
                                 NAME_PARA = a.NAM_CATE,
                                 DESC_PARA = "ISO",
                                 EXT_ATTA = "",
                                 HAS_VALUE = (CountKnowledgeISO(a.ID_KNOW_CATE) > 0 ? true : false),
                                 expanded = false,
                                 ID_ACCO_MONT = 0,
                                 NAM_ATTA = "",
                                 NAM_FILE = "",
                                 ATTACH = false,
                                 DOWNLOAD = false,
                                 DELETE = false,
                                 USER_NAME = "",
                             });

                int ctd = dbc.ACCOUNT_PARAMETER
                    .Where(x => x.ID_PARA == 20 && x.VAL_ACCO_PARA == idpe && x.VIG_ACCO_PARA == true)
                    .Count();

                var queryUser = dbe.CLASS_ENTITY;

                var query2 = (from a in dbc.KNOWLEDGEs.Where(x => x.ID_KNOW_CATE == ID_CATE &&
                                                                    x.VIG_KNOW == true).ToList()
                              join kc in dbc.KNOWLEDGE_CATEGORY on a.ID_KNOW_CATE equals kc.ID_KNOW_CATE
                              join u in queryUser on a.UserId equals u.UserId
                             select new
                             {
                                 ID_PARA = "ISO*" + Convert.ToString(a.ID_KNOW),
                                 NAME_PARA = a.NAM_KNOW,
                                 DESC_PARA = a.DESC_KNOW,
                                 EXT_ATTA = (a.EXT_ATTA == null ? "pdf.png" : a.EXT_ATTA.Substring(1, a.EXT_ATTA.Length - 1) + ".png"),
                                 HAS_VALUE = false,
                                 expanded = false,
                                 ID_ACCO_MONT = 1,
                                 NAM_ATTA = a.NAM_ATTA,
                                 NAM_FILE = (a.EXT_ATTA).ToLower() == ".pdf" || (a.EXT_ATTA).ToLower() == ".png" ||
                                            (a.EXT_ATTA).ToLower() == ".jpg" || (a.EXT_ATTA).ToLower() == ".txt"
                                            ? "/Knowledge/VerDocumento/1/" + a.NAM_ATTA + "_" + a.ID_KNOW.ToString() + a.EXT_ATTA : "/Knowledge/DescargarArchivo/1/" + (a.NAM_ATTA + "_" + a.ID_KNOW.ToString() + a.EXT_ATTA),
                                 ATTACH = true,
                                 DOWNLOAD = (kc.ANY_DOWN == true ? true: (a.UserId == UserId ? true : RolAdminitratorOrPMO())),
                                 DELETE = (ctd == 0 ? false : true),
                                 USER_NAME = u.FIR_NAME.Substring(0, 1).ToUpper() + u.FIR_NAME.Substring(1, u.FIR_NAME.Length - 1).ToLower() + " " +
                                            u.LAS_NAME.Substring(0, 1).ToUpper() + u.LAS_NAME.Substring(1, u.LAS_NAME.Length - 1).ToLower(),
                             });

                var query3 = query.Union(query2);

                return Json(query3.OrderBy(x => x.ID_ACCO_MONT), JsonRequestBehavior.AllowGet);
            }
            else //Enlazando los años
            {
                int ID_CATE = Convert.ToInt32(ID_PARA);
                var query = (from a in dbc.KNOWLEDGEs.Where(x => x.ID_KNOW_CATE == ID_CATE).ToList()
                             join b in dbc.ACCOUNTING_MONTH on a.ID_ACCO_MONT equals b.ID_ACCO_MONT
                             select new
                             {
                                 ID_PARA = Convert.ToString(b.ID_ACCO_YEAR) + "|" + ID_PARA,
                                 NAME_PARA = b.ID_ACCO_YEAR.ToString(),
                                 ID_ACCO_YEAR = b.ID_ACCO_YEAR,
                                 DESC_PARA = "",
                                 HAS_VALUE = true,
                                 ATTACH = false,
                                 expanded = false,
                                 USER_NAME = "",
                                 DELETE = false,
                                 DOWNLOAD = false,
                                 NAM_ATTA = "",
                                 NAM_FILE = "",
                             }).OrderByDescending(x => x.ID_ACCO_YEAR).Distinct();

                return Json(query, JsonRequestBehavior.AllowGet);
            }
        }

        public bool RolAdminitratorOrPMO() {
            bool down = false;
            //string[] rolesArray = Roles.GetRolesForUser();
            //foreach (string xc in rolesArray)
            //{
            //    if (xc == "ADMINISTRADOR" || xc == "PMO" || xc == "SALES" ) { down = true; }
            //}
            if ((int)Session["ADMINISTRADOR"] == 1 || (int)Session["PROJECTMANAGER"] == 1)
            {
                down = true;
            }
            return down;
        }

        public int CountKnowledgeByCategory(Boolean reg, int id = 0, int area = 0)
        {
            int con = 0;            
            if (reg)
            {
                con = dbc.KNOWLEDGE_CATEGORY.Where(x => x.ID_KNOW_CATE_PARE == id).Count();
            }
            else {
                bool sts = dbc.KNOWLEDGE_CATEGORY.Single(x => x.ID_KNOW_CATE == id).ALL_AREA.Value;
                if (sts) { area = 0; }
                con = dbc.KNOWLEDGEs.Where(x => x.ID_KNOW_CATE == id && x.ID_AREA == area).Count();
            }            
            return con;
        }

        public int CountKnowledgeISO(int id = 0)
        {
            int con = 0;
            con = dbc.KNOWLEDGE_CATEGORY.Where(x => x.ID_KNOW_CATE_PARE == id).Count();

            if (con == 0) {
                con = dbc.KNOWLEDGEs.Where(x => x.ID_KNOW_CATE == id).Count();
            }

            return con;
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Create(IEnumerable<HttpPostedFileBase> files, KNOWLEDGE know)
        {
            //string S_NAM_KNOW = Convert.ToString(Request.Params["NAM_KNOW"].ToString()).ToUpper();
            string S_DESC_KNOW = Convert.ToString(Request.Params["DESC_KNOW"].ToString()).ToUpper();
            int sw = 0, error = 0, ID_KNOW_CATE = 0, ID_KNOW_CATE_ATTA = 0, mon = 0, ani = 0;
            DateTime DATE_ATTA;

            try { ID_KNOW_CATE_ATTA = Convert.ToInt32(Request.Form["ID_KNOW_CATE_ATTA"]); }
            catch{ }
            
            if (ID_KNOW_CATE_ATTA != 0) {
                if (files == null) { sw = 1; error = 2; }
            }
            else if (S_DESC_KNOW.Trim().Length == 0) { sw = 1; }
            else if (Int32.TryParse(Convert.ToString(Request.Form["ID_KNOW_CATE"]), out ID_KNOW_CATE) == false) { sw = 1; }
            else if (files == null) { sw = 1; error = 2; }
            else if (Convert.ToInt32(Session["UserId"]) == 0) { sw = 1; error = 3; }

            int flag = Convert.ToInt32(Request.Form["SW_DATE"]);
            if (flag == 1)
            {
                if (DateTime.TryParse(Convert.ToString(Request.Form["DATE_ATTA"]), out DATE_ATTA) == false) { sw = 1; }
                else{
                    mon = DATE_ATTA.Month;
                    ani = DATE_ATTA.Year;
                }
            }

            if (sw == 1)
            {
                return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDone) top.uploadDone('ERROR','" + error + "');}window.onload=init;</script>");
            }
            //Registrando Knowledge
            string code = "";

            foreach (var file in files)
            {
                if (file != null) {
                    try
                    {
                        if (ID_KNOW_CATE_ATTA != 0) {
                            int tt = dbc.KNOWLEDGEs.Where(x => x.ID_AREA == know.ID_AREA && x.ID_KNOW_CATE == ID_KNOW_CATE_ATTA).Count();
                            tt = tt + 1;
                            know.ID_KNOW_CATE = ID_KNOW_CATE_ATTA;
                            know.NAM_ATTA = tt.ToString() + ". " + Path.GetFileNameWithoutExtension(file.FileName);
                            know.EXT_ATTA = Path.GetExtension(file.FileName);
                            know.UserId = Convert.ToInt32(Session["UserId"]);
                            know.CREATED_KNOW = DateTime.Now;
                            know.VIG_KNOW = true;
                            know.NAM_KNOW = know.NAM_ATTA.ToLower();
                            know.DESC_KNOW = "";
                            dbc.KNOWLEDGEs.Add(know);
                            dbc.SaveChanges();

                            code = "ISO*" + Convert.ToString(ID_KNOW_CATE_ATTA);
                        }
                        else
                        {
                            if (flag == 1)
                            {                                
                                know.ID_ACCO_MONT = dbc.ACCOUNTING_MONTH.Single(x => x.ACCO_MONT == mon && x.ID_ACCO_YEAR == ani).ID_ACCO_MONT;
                            }
                            bool ALL_AREA = dbc.KNOWLEDGE_CATEGORY.Single(x => x.ID_KNOW_CATE == know.ID_KNOW_CATE).ALL_AREA.Value;
                            if (ALL_AREA)
                            {
                                know.ID_AREA = 0;
                            }

                            int tt = dbc.KNOWLEDGEs.Where(x => x.ID_ACCO_MONT == know.ID_ACCO_MONT && x.ID_AREA == know.ID_AREA && x.ID_KNOW_CATE == know.ID_KNOW_CATE).Count();

                            var cat = dbc.KNOWLEDGE_CATEGORY.Single(x => x.ID_KNOW_CATE == ID_KNOW_CATE);
                            var keep = cat.KEEP_NAME;
                            var desc = cat.DESC_CATE;

                            tt = tt + 1;
                            var tiempo = dbc.ACCOUNTING_MONTH.Single(x => x.ID_ACCO_MONT == know.ID_ACCO_MONT);
                            int mes = tiempo.ACCO_MONT.Value;
                            int anio = tiempo.ID_ACCO_YEAR.Value;
                            string mesName = tiempo.NAM_ACCO_MONT;

                            know.NAM_ATTA = mes.ToString() + "." + tt.ToString() + ". " + (keep == true ? desc : Path.GetFileNameWithoutExtension(file.FileName));
                            know.EXT_ATTA = Path.GetExtension(file.FileName);
                            know.UserId = Convert.ToInt32(Session["UserId"]);
                            know.CREATED_KNOW = DateTime.Now;
                            know.VIG_KNOW = true;
                            know.NAM_KNOW = know.NAM_ATTA.ToLower();
                            know.DESC_KNOW = S_DESC_KNOW;
                            dbc.KNOWLEDGEs.Add(know);
                            dbc.SaveChanges();

                            code = Convert.ToString(ID_KNOW_CATE);
                        }

                        file.SaveAs(Server.MapPath("~/Adjuntos/Knowledge/" + know.NAM_ATTA + "_" + Convert.ToString(know.ID_KNOW) + know.EXT_ATTA));
                    }
                    catch
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('ERROR','1');}window.onload=init;</script>");
                    }                
                }
            }
            return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('OK','" + code + "');}window.onload=init;</script>");            
        }

        public string Delete(int id = 0) {
            try
            {
                KNOWLEDGE know = dbc.KNOWLEDGEs.Find(id);
                try
                {
                    string arc = know.NAM_ATTA + "_" + Convert.ToString(know.ID_KNOW) + know.EXT_ATTA;
                    dbc.KNOWLEDGEs.Remove(know);
                    dbc.SaveChanges();

                    //Eliminando
                    string filePath = Server.MapPath("~/Adjuntos/Knowledge/" + arc);
                                        
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                    else
                    {
                        Console.WriteLine("no existe archivo");
                    }
                    
                }
                catch (Exception)
                {
                    return "ERROR";
                }
                return "OK";
            }
            catch (Exception)
            {
                return "ERROR";
            }
        }

        public ActionResult ListCategory() {

            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            var result = (from r in dbc.KNOWLEDGE_CATEGORY.Where(x=>x.ID_KNOW_CATE_PARE==null && x.ID_KNOW_CATE != 20 && x.VIG_CATE == true && x.ID_ACCO == ID_ACCO)
                          select new { 
                            r.ID_KNOW_CATE,
                            r.NAM_CATE,
                            r.REG_DATE_ATTA,
                          }).OrderBy(x=>x.NAM_CATE);

            return Json(new { Data = result, Count = result.Count()},JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListKnowledgeTemplate()
        {
            var result = (from r in dbc.KNOWLEDGE_TEMPLATE
                          select new
                          {
                              r.NAM_TEMP,
                              r.EXT_FILE,
                              NAM_FILE = (r.EXT_FILE).ToLower()==".pdf" ||(r.EXT_FILE).ToLower()==".png" ||
                                            (r.EXT_FILE).ToLower()==".jpg" ||(r.EXT_FILE).ToLower()==".txt"
                                            ? "/Knowledge/VerDocumento/2/" + r.NAM_TEMP + r.EXT_FILE : "/Knowledge/DescargarArchivo/2/" + (r.NAM_TEMP + r.EXT_FILE),
                              ICONO = r.EXT_FILE.Substring(1,r.EXT_FILE.Length - 1) + ".png",
                              r.ID_KNOW_TEMP,
                          }).OrderBy(x=>x.NAM_TEMP);
            return Json(new{Data = result, Count = result.Count()},JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListClientByAccount() {

            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            var result = (from ae in dbe.ACCOUNT_ENTITY.Where(x => x.ID_ACCO == ID_ACCO)
                          join pe in dbe.PERSON_ENTITY on ae.ID_PERS_ENTI equals pe.ID_PERS_ENTI
                          join ce in dbe.CLASS_ENTITY on pe.ID_ENTI1 equals ce.ID_ENTI
                          select new { 
                            ce.ID_ENTI,
                            ce.COM_NAME,
                          }).Distinct();

            return Json(new { Data = result, Count = result.Count()},JsonRequestBehavior.AllowGet);
        }

    }
}
