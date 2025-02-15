using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;

namespace ERPElectrodata.Controllers
{
    public class DegreeInstructionController : Controller
    {
        EntityEntities dbe = new EntityEntities();
        //
        // GET: /DegreeInstruction/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListDegreeInstruction()
        {
            var result = (from a in dbe.DEGREE_INSTRUCTION
                          select new
                          {
                              a.ID_DEGR_INST,
                              a.NAM_DEGR_INST
                          }).OrderBy(x => x.NAM_DEGR_INST);
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

    }
}
