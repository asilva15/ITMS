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
using System.Drawing;
using System.Web.Helpers;

namespace ERPElectrodata.Controllers
{
    public class EventController : Controller
    {
        public CoreEntities dbc = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();
        private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        private TextInfo textInfo;
        
        // GET: /Event/
        [Authorize]
        public ActionResult Index()
        {
            try
            {
                int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);

                var query = dbe.QUESTION_PERSON_ENTITY.Where(x => x.ID_PERS_ENTI == ID_PERS_ENTI).Where(x => x.VAL_QUES_PERS_ENTI == null);

                if (query.Count() == 0)
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "QuestionPersonEntity");
                }
                //return View();
            }
            catch
            {
                return Content("Please Close Session");
            }
            
        }

        public ActionResult Create()
        {
            ViewBag.DATE_EVENT = DateTime.Now;
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SaveEvent(EVENT eve, IEnumerable<HttpPostedFileBase> files)
        {
            int UserId = Convert.ToInt32(Session["UserId"]);
            string Error = "";            
            string extencion = "", ext = "", direct="";
           

            if (String.IsNullOrEmpty(eve.NAM_EVEN))
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDone) top.uploadDone('ERROR','You must introduce event name');}window.onload=init;</script>");
            }

            try
            {
                eve.UserId = UserId;            
                eve.CREATED = DateTime.Now;             
                dbe.EVENTs.Add(eve);
                dbe.SaveChanges();

                if (files != null)
                {
                    var ATTA = new ATTACH_EVENT();

                    foreach (var file in files)
                    {
                        try
                        {    
                            
                            extencion =Path.GetExtension(file.FileName);                      

                            ATTA.ID_EVEN = eve.ID_EVEN;
                            ATTA.NAM_ATTA_EVEN = Path.GetFileNameWithoutExtension(file.FileName);
                            ATTA.EXT_ATTA_EVEN = extencion;
                            ATTA.CREATED = DateTime.Now;
                            ATTA.UserId = UserId;
                            dbe.ATTACH_EVENT.Add(ATTA);
                            dbe.SaveChanges();

                            file.SaveAs(Server.MapPath("~/Adjuntos/Events/") + ATTA.NAM_ATTA_EVEN + "_" + Convert.ToString(ATTA.ID_ATTA_EVEN) + ATTA.EXT_ATTA_EVEN);
                            
                            ext = extencion.ToUpper();
                            
                            if (ext==".JPG")
                            {   //Redimensionando la imagen
                               
                                try
                                {
                                    direct = Server.MapPath("~/Adjuntos/Events/" + ATTA.NAM_ATTA_EVEN + "_" + Convert.ToString(ATTA.ID_ATTA_EVEN) + ATTA.EXT_ATTA_EVEN);
                                    string output = Server.MapPath("~/Adjuntos/Events/Thumbnail/" + ATTA.NAM_ATTA_EVEN + "_" + Convert.ToString(ATTA.ID_ATTA_EVEN) + ATTA.EXT_ATTA_EVEN);
                                   
                                    // Bitmap img = new Bitmap(direct);
                                    Image image = Image.FromFile(direct);

                                    // Compute thumbnail size.
                                    Size thumbnailSize = GetThumbnailSize(image);

                                    // Get thumbnail.
                                    Image thumbnail = image.GetThumbnailImage(thumbnailSize.Width,
                                        thumbnailSize.Height, null, IntPtr.Zero);

                                    // Save thumbnail.
                                    thumbnail.Save(output);
                                }
                                catch (Exception e)
                                {
                                       return Content("<script type='text/javascript'> function init() {" +
                                        "if(top.uploadDone) top.uploadDone('ERROR','Error Saving Thumbnails');}window.onload=init;</script>");
                                }

                            }

                        }
                        catch
                        {
                            return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDone) top.uploadDone('ERROR','Error Saving Attachments');}window.onload=init;</script>");
                        }
                    }
                }



            }
            catch (Exception e)
            {
                Error = (e.InnerException == null ? e.Message : e.InnerException.Message);

                return Content("<script type='text/javascript'> function init() {" +
                                "if(top.uploadDone) top.uploadDone('ERROR','" + Error.ToString() + "');}window.onload=init;</script>");
            }

            return Content("<script type='text/javascript'> function init() {" +
                   "if(top.uploadDone) top.uploadDone('OK','Your event has been successfully saved');}window.onload=init;</script>");
        }

        static Size GetThumbnailSize(Image original)
        {
            // Maximum size of any dimension.
            const int maxPixels = 100;

            // Width and height.
            int originalWidth = original.Width;
            int originalHeight = original.Height;

            // Compute best factor to scale entire image based on larger dimension.
            double factor;
            if (originalWidth > originalHeight)
            {
                factor = (double)maxPixels / originalWidth;
            }
            else
            {
                factor = (double)maxPixels / originalHeight;
            }

            // Return thumbnail size.
            return new Size((int)(originalWidth * factor), (int)(originalHeight * factor));
        }
        public ActionResult DataListEvent()
        {
            textInfo = cultureInfo.TextInfo;

            var data = (from e in dbe.EVENTs
                         join ce in dbe.CLASS_ENTITY on e.UserId equals ce.UserId
                         join pe in dbe.PERSON_ENTITY on ce.ID_ENTI equals pe.ID_ENTI2
                         select new
                         {
                             e.ID_EVEN,
                             e.NAM_EVEN,
                             e.DATE_STAR,
                             e.DATE_END,
                             e.CREATED,
                             
                             ce.FIR_NAME,
                             ce.LAS_NAME,
                             MOT_NAME=   ce.MOT_NAME == null ? "" : ce.MOT_NAME,                             
                             pe.ID_FOTO,

                         });

            var query = (from d in data.ToList()
                         select new
                         {
                             d.ID_EVEN,
                             d.NAM_EVEN,
                             DATE_STAR = (String.Format("{0:d}", d.DATE_STAR)),
                             DATE_END = (String.Format("{0:d}", d.DATE_END)),
                             CREATED = (String.Format("{0:d}", d.CREATED)),
                             d.ID_FOTO,
                             NAME = textInfo.ToTitleCase((d.FIR_NAME.ToLower() + " " + d.LAS_NAME.ToLower() + " " + (d.MOT_NAME == null ? "" : d.MOT_NAME).ToLower())),
                         });

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DataListAttaEvent (int id=0)
        {
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            var data = (from a in dbe.ATTACH_EVENT.Where(x => x.ID_EVEN == id)
                        select new
                        {
                            ID_ATTA_EVEN = a.ID_ATTA_EVEN,
                            NAM_ATTA_EVEN = a.NAM_ATTA_EVEN,
                            EXT_ATTA_EVEN = a.EXT_ATTA_EVEN,
                            ID_EVEN = a.ID_EVEN,
                            CREATED = a.CREATED,
                        });

            var query = (from d in data.OrderByDescending(x => x.CREATED).Skip(skip).Take(take)
                         select new
                         {
                             ID_ATTA_EVEN = d.ID_ATTA_EVEN,
                             NAM_ATTA_EVEN = d.NAM_ATTA_EVEN,
                             EXT_ATTA_EVEN = d.EXT_ATTA_EVEN,
                             Ext = d.EXT_ATTA_EVEN.ToLower(),
                             ID_EVEN = d.ID_EVEN,
                         });

            return Json(new { Data = query, Count = data.Count() }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult DataListNameEvent(int id = 0)
        {
            var query = (from e in dbe.EVENTs.Where(x => x.ID_EVEN == id)
                         select new
                         {
                            e.ID_EVEN,
                            e.NAM_EVEN,
                         });

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SaveAttachEvent(IEnumerable<HttpPostedFileBase> filesevent)
        {
            int UserId = Convert.ToInt32(Session["UserId"]);
            int ID_EVEN = Convert.ToInt32(Request.Params["ID_EVEN"]);
            string extencion = "", ext = "", direct = "", nameattachs = "", NAM_ATTA_EVEN = "";

            if (filesevent != null)
            {
                var ATTA = new ATTACH_EVENT();

                foreach (var file in filesevent)
                {
                    try
                    {

                        extencion = Path.GetExtension(file.FileName);
                        NAM_ATTA_EVEN = Path.GetFileNameWithoutExtension(file.FileName);

                        ATTA.ID_EVEN = ID_EVEN;
                        ATTA.NAM_ATTA_EVEN = NAM_ATTA_EVEN;
                        ATTA.EXT_ATTA_EVEN = extencion;
                        ATTA.CREATED = DateTime.Now;
                        ATTA.UserId = UserId;
                        dbe.ATTACH_EVENT.Add(ATTA);
                        dbe.SaveChanges();

                        file.SaveAs(Server.MapPath("~/Adjuntos/Events/") + ATTA.NAM_ATTA_EVEN + "_" + Convert.ToString(ATTA.ID_ATTA_EVEN) + ATTA.EXT_ATTA_EVEN);

                        nameattachs = nameattachs + NAM_ATTA_EVEN + extencion + "*";

                        ext = extencion.ToUpper();

                        if (ext == ".JPG")
                        {   //Redimensionando la imagen

                            try
                            {
                                direct = Server.MapPath("~/Adjuntos/Events/" + ATTA.NAM_ATTA_EVEN + "_" + Convert.ToString(ATTA.ID_ATTA_EVEN) + ATTA.EXT_ATTA_EVEN);
                                string output = Server.MapPath("~/Adjuntos/Events/Thumbnail/" + ATTA.NAM_ATTA_EVEN + "_" + Convert.ToString(ATTA.ID_ATTA_EVEN) + ATTA.EXT_ATTA_EVEN);

                                // Bitmap img = new Bitmap(direct);
                                Image image = Image.FromFile(direct);

                                // Compute thumbnail size.
                                Size thumbnailSize = GetThumbnailSize(image);

                                // Get thumbnail.
                                Image thumbnail = image.GetThumbnailImage(thumbnailSize.Width,
                                    thumbnailSize.Height, null, IntPtr.Zero);

                                // Save thumbnail.
                                thumbnail.Save(output);
                            }
                            catch (Exception e)
                            {
                                return Content("<script type='text/javascript'> function init() {" +
                                 "if(top.uploadDone) top.uploadDone('ERROR','Error Saving Thumbnails');}window.onload=init;</script>");
                            }

                        }

                    }
                    catch
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDone) top.uploadDone('ERROR','Error Saving Attachments; Note: Maximum Size 16 MB');}window.onload=init;</script>");
                    }
                }
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                                     "if(top.uploadDone) top.uploadDone('ERROR','You have not selected any file');}window.onload=init;</script>");
            }

            return Content("<script type='text/javascript'> function init() {" +
                   "if(top.uploadDone) top.uploadDone('OK','" + nameattachs + "');}window.onload=init;</script>");
        }

        public ActionResult ViewNewAttach(int id = 0)
        {
            var query = dbe.EVENTs.Single(x => x.ID_EVEN == id);

            ViewBag.NAM_EVEN = query.NAM_EVEN;
            ViewBag.ID_EVEN = query.ID_EVEN;

            return View();
        }

    }
}
