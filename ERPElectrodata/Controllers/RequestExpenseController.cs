using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.Entity;
using ERPElectrodata.Models;
using ERPElectrodata.Objects;
using System.IO;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPElectrodata.Controllers
{
    public class RequestExpenseController : Controller
    {

        CoreEntities dbc = new CoreEntities();
        EntityEntities dbe = new EntityEntities();

        // GET: /RequestExpense/

        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        //CREATE DEL REQUEST_EXPENSE
        public ActionResult Create(REQUEST_EXPENSE re)
        {
            int sw = 0;
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            string Error = "";
            string JUSTI = Convert.ToString(Request.Form["JUSTIFICATION"]);
            // Validaciones...
            if (re.ID_TYPE_DELI_SUST == null) { sw = 1; Error = ResourceLanguaje.Resource.PettyCashMsn16; }
            else if (re.ID_TICK == null) { sw = 1; Error = ResourceLanguaje.Resource.PettyCashMsn17; }
            else if (re.CURRENCY == null) { sw = 1; Error = ResourceLanguaje.Resource.PettyCashMsn20; }
            else if (re.ID_TYPE_DELI_SUST == 1) // Tipo Caja Chica()
            {
                if (re.AMOUNT == null) { sw = 1; Error = ResourceLanguaje.Resource.PettyCashMsn18; }
                else if (re.AMOUNT == 0) { sw = 1; Error = ResourceLanguaje.Resource.PettyCashMsn19; }
                else if (re.AMOUNT > 5000) { sw = 1; Error = "El monto máximo es 2000."; }
                else if ((JUSTI == null ? "" : JUSTI.Trim()).Length == 0) { sw = 1; Error = ResourceLanguaje.Resource.PettyCashMsn21; }
            }else if (re.ID_TYPE_DELI_SUST == 3)
            {
                if (re.AMOUNT == null) { sw = 1; Error = ResourceLanguaje.Resource.PettyCashMsn18; }
                else if (re.AMOUNT == 0) { sw = 1; Error = ResourceLanguaje.Resource.PettyCashMsn19; }
                else if ((JUSTI == null ? "" : JUSTI.Trim()).Length == 0) { sw = 1; Error = ResourceLanguaje.Resource.PettyCashMsn21; }
            }
            else
            { //Viáticos
                decimal tt = 0;
                if (re.NUM_DAYS == null) { sw = 1; Error = ResourceLanguaje.Resource.PettyCashMsn23; }
                else if (re.DAT_DEPA == null) { sw = 1; Error = ResourceLanguaje.Resource.PettyCashMsn22; }
                else if (re.DESTINATION == null) { sw = 1; Error = ResourceLanguaje.Resource.PettyCashMsn11; }
                else
                {
                    string conc = Convert.ToString(Request.Form["HF_CONC"]);
                    if (conc.Length > 0)
                    {
                        string[] arrConc = conc.Split('*');
                        for (int i = 0; i <= arrConc.Count() - 1; i++)
                        {
                            string[] amount = arrConc[i].Split('|');
                            tt = tt + Convert.ToDecimal(amount[1]);
                        }
                    }

                    if (tt == 0)
                    {
                        sw = 1; Error = ResourceLanguaje.Resource.PettyCashMsn19;
                    }
                    else if ((JUSTI == null ? "" : JUSTI.Trim()).Length == 0) { sw = 1; Error = ResourceLanguaje.Resource.PettyCashMsn21; }
                    re.AMOUNT = tt;
                }
            }
            

            if (sw == 1)
            {
                return Content("<script type='text/javascript'> function init() {" +
                                "if(top.uploadDoneNR) top.uploadDoneNR('ERROR','" + Error.ToString() + "');}window.onload=init;</script>");
            }

            try
            {

                re.JUSTIFICATION = JUSTI.Replace("'", "");
                re.ID_PERS_ENTI_REQU = ID_PERS_ENTI;
                re.ID_STAT_REQU_EXPE = 1;
                if (re.ID_TYPE_DELI_SUST == 3 || re.ID_TYPE_DELI_SUST ==1)
                {
                    re.ID_STAT_REQU_EXPE = 5;
                }
                re.DAT_REGI = DateTime.Now;
                re.ID_ENTI = 9;

                //Determinando si puede aprobarse a si mismo
                int swApp = 0;

                if ((int)Session["ADMINISTRADOR"] == 1 || (int)Session["FINANZAS"] == 1)
                {
                    swApp = 1;
                }
                //int ID_PERS_ENTI_JF = Convert.ToInt32(dbe.PERSON_CONTRACT.Where(x => (x.ID_CHAR == 1241 || x.ID_CHAR == 1269) && x.VIG_CONT == true).FirstOrDefault().ID_PERS_ENTI);
                var usuario = dbe.ValidaUsuario(ID_PERS_ENTI).Single();
                int ID_PERS_ENTI_JF = 0;
                try
                {   //jefe inmediato superior --> Procedure que obtiene el jefe superior
                    ID_PERS_ENTI_JF = Convert.ToInt32(dbe.EMAIL_BOSS_INMSUP(ID_PERS_ENTI).First().ID_PERS_ENTI);
                }
                catch
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDoneNR) top.uploadDoneNR('ERROR','Error. No se encontró el cargo del jefe inmediato superior.');}window.onload=init;</script>");
                }

                if (swApp == 1) // Si es jefe 
                {
                    var pca = dbc.PETTY_CASH_ASSIGNED.Where(x => x.ID_PERS_ENTI_APPR == re.ID_PERS_ENTI_REQU
                              && x.VIG_PETT_CASH_ASSI == true);  //Verifica si es que aprueba a el mismo 

                    if (pca.Count() == 0)
                    {
                        swApp = 0;
                    }
                    else 
                    {
                        re.ID_PERS_ENTI_APPR = ID_PERS_ENTI_JF;
                        re.ID_PERS_ENTI_ASSI = pca.First().ID_PERS_ENTI_ASSI;
                        re.ID_PERS_ENTI_APPR_VIAT = re.ID_PERS_ENTI_REQU;
                        re.ID_STAT_REQU_EXPE = 2; // Si es que es jefe ya se crea el requerimirnto aprobado.
                        if (re.ID_TYPE_DELI_SUST == 3 || re.ID_TYPE_DELI_SUST == 1)
                        {
                            re.ID_STAT_REQU_EXPE = 5;
                        }
                        if (re.ID_TYPE_DELI_SUST == 2)
                        {   //Si es viatico                     
                            re.ID_PERS_ENTI_APPR_VIAT = pca.First().ID_PERS_ENTI_APPR_VIAT;
                            if (re.ID_PERS_ENTI_APPR != re.ID_PERS_ENTI_APPR_VIAT)
                            {
                                re.ID_STAT_REQU_EXPE = 8;
                                if (re.ID_TYPE_DELI_SUST == 3 || re.ID_TYPE_DELI_SUST == 1)
                                {
                                    re.ID_STAT_REQU_EXPE = 5;
                                }
                            }
                        }
                    }
                   
                }

                if (swApp == 1) // Si es jefe 
                {
                    //var pca = dbc.PETTY_CASH_ASSIGNED.Where(x => x.ID_PERS_ENTI_APPR == re.ID_PERS_ENTI_REQU
                    //          && x.VIG_PETT_CASH_ASSI == true);  //Verifica si es que aprueba a el mismo 

                    //if (pca.Count() == 0)
                    //{
                    //    return Content("<script type='text/javascript'> function init() {" +
                    //                    "if(top.uploadDoneNR) top.uploadDoneNR('ERROR','Error. Su usuario no se encuentra en PETTY_CASH_ASSIGNED.');}window.onload=init;</script>");
                    //}
                    //re.ID_PERS_ENTI_APPR = re.ID_PERS_ENTI_REQU;
                    //re.ID_PERS_ENTI_ASSI = pca.First().ID_PERS_ENTI_ASSI;
                    //re.ID_STAT_REQU_EXPE = 2; // Si es que es jefe ya se crea el requerimirnto aprobado.
                    //if (re.ID_TYPE_DELI_SUST == 2)
                    //{   //Si es viatico                     
                    //    re.ID_PERS_ENTI_APPR_VIAT = pca.First().ID_PERS_ENTI_APPR_VIAT;
                    //    if (re.ID_PERS_ENTI_APPR != re.ID_PERS_ENTI_APPR_VIAT)
                    //    {
                    //        re.ID_STAT_REQU_EXPE = 8;
                    //    }
                    //}
                }
                else if (usuario.ID_CHAR_PERS == 1269)
                {
                    re.ID_STAT_REQU_EXPE = 2;
                    if (re.ID_TYPE_DELI_SUST == 3 || re.ID_TYPE_DELI_SUST == 1)
                    {
                        re.ID_STAT_REQU_EXPE = 5;
                    }
                    re.ID_PERS_ENTI_APPR = 29311;
                    if (re.ID_TYPE_DELI_SUST == 2) // Si es viático
                    {
                        re.ID_PERS_ENTI_APPR_VIAT = ID_PERS_ENTI_JF;//Asignamos quien crea el Viático
                    }
                }
                else
                {

                    var busUsuario = dbe.BuscarUsuario(ID_PERS_ENTI_JF, ID_PERS_ENTI).ToList();
                    if (busUsuario.Count() == 1)
                    {
                        if (busUsuario.Single().N1 == 1248 || busUsuario.Single().N2 == 1248 || busUsuario.Single().N3 == 1248
                            || busUsuario.Single().N4 == 1248 || busUsuario.Single().N5 == 1248) // Gestor de operaciones
                        {
                            var pContract = dbe.PERSON_CONTRACT.Where(x => x.ID_CHAR == 1248 && x.VIG_CONT == true && x.LAS_CONT == true).ToList();
                            ID_PERS_ENTI_JF = Convert.ToInt32(pContract.Count() == 0 ? 63133 : pContract.First().ID_PERS_ENTI);
                        }
                        else if (busUsuario.Single().N1 == 7840 || busUsuario.Single().N2 == 7840 || busUsuario.Single().N3 == 7840
                            || busUsuario.Single().N4 == 7840 || busUsuario.Single().N5 == 7840) // Gestor de servicios
                        {
                            var pContract = dbe.PERSON_CONTRACT.Where(x => x.ID_CHAR == 7840 && x.VIG_CONT == true && x.LAS_CONT == true).ToList();
                            ID_PERS_ENTI_JF = Convert.ToInt32(pContract.Count() == 0 ? 63133 : pContract.First().ID_PERS_ENTI);
                        }
                        else if (busUsuario.Single().N1 == 7356 || busUsuario.Single().N2 == 7356 || busUsuario.Single().N3 == 7356
                            || busUsuario.Single().N4 == 7356 || busUsuario.Single().N5 == 7356) //Líder de minería
                        {
                            ID_PERS_ENTI_JF = 63133;
                        }
                    }

                    //sacar jefe 


                    //re.ID_PERS_ENTI_APPR = ID_PERS_ENTI_JF; //Asignamos quien crea el Viático
                    //    var pcaIS = dbc.PETTY_CASH_ASSIGNED.Where(x => x.ID_PERS_ENTI_APPR == re.ID_PERS_ENTI_APPR
                    //              && x.VIG_PETT_CASH_ASSI == true);
                    //    if (pcaIS.Count() == 0)
                    //    {
                    //        return Content("<script type='text/javascript'> function init() {" +
                    //                        "if(top.uploadDoneNR) top.uploadDoneNR('ERROR','Error. Su jefe directo no se encuentra en PETTY_CASH_ASSIGNED.');}window.onload=init;</script>");
                    //    }
                    //    re.ID_PERS_ENTI_APPR_VIAT = pcaIS.First().ID_PERS_ENTI_APPR_VIAT; //Asignamos al aprobador en base a la tabla PETTY_CASH_ASSIGNED

                    re.ID_PERS_ENTI_APPR = ID_PERS_ENTI_JF;


                    while (true)
                    {

                        var pcaIS = dbc.PETTY_CASH_ASSIGNED.Where(x => x.ID_PERS_ENTI_APPR == re.ID_PERS_ENTI_APPR && x.VIG_PETT_CASH_ASSI == true).ToList();


                        if (pcaIS.Any())
                        {
                            re.ID_PERS_ENTI_APPR_VIAT = pcaIS.First().ID_PERS_ENTI_APPR_VIAT;
                            break;
                        }


                        var bossEmailRecord = dbe.EMAIL_BOSS_INMSUP(ID_PERS_ENTI_JF).FirstOrDefault();
                        ID_PERS_ENTI_JF = (bossEmailRecord == null ? 0 : Convert.ToInt32(bossEmailRecord.ID_PERS_ENTI));


                        re.ID_PERS_ENTI_APPR = ID_PERS_ENTI_JF;


                        if (ID_PERS_ENTI_JF == 0)
                        {
                            return Content("<script type='text/javascript'> function init() {" +
                                           "if(top.uploadDoneNR) top.uploadDoneNR('ERROR','Error. Su jefe directo no se encuentra en PETTY_CASH_ASSIGNED.');}window.onload=init;</script>");
                        }
                    }
                }

                re.ID_STAT_REQU_EXPE = 1;
                if (re.ID_TYPE_DELI_SUST == 3 || re.ID_TYPE_DELI_SUST == 1)
                {
                    re.ID_STAT_REQU_EXPE = 5;
                }

                re.SEND_MAIL = false;
                re.ID_STAT_REQU_EXPE_STAR = re.ID_STAT_REQU_EXPE.Value;
                dbc.REQUEST_EXPENSE.Add(re);
                dbc.SaveChanges();

                try
                {
                    //SendMail smail = new SendMail();
                    //smail.RequestAccountability(re);
                }
                catch
                {

                }

                REQUEST_EXPENSE_ACTIVITY rea = new REQUEST_EXPENSE_ACTIVITY();
                rea.UserId = Convert.ToInt32(Session["UserId"]);
                rea.ID_STAT_REQU_EXPE = re.ID_STAT_REQU_EXPE;
                rea.DAT_STAR = re.DAT_REGI;
                rea.ID_REQU_EXPE = re.ID_REQU_EXPE;

                dbc.REQUEST_EXPENSE_ACTIVITY.Add(rea);
                dbc.SaveChanges();

                if (re.ID_TYPE_DELI_SUST == 2)
                { //Si es viático
                    decimal tt = 0;
                    int idtr = 0;
                    string conc = Convert.ToString(Request.Form["HF_CONC"]);
                    if (conc.Length > 0) // si hay una cadena de los viáticos
                    {
                        string[] arrConc = conc.Split('*');// Lee el vector de los tipos de viaticos
                        for (int i = 0; i <= arrConc.Count() - 1; i++)
                        {
                            string[] amount = arrConc[i].Split('|');
                            idtr = Convert.ToInt32(amount[0]);
                            tt = Convert.ToDecimal(amount[1]);

                            REQUEST_VIATICAL rv = new REQUEST_VIATICAL();
                            rv.ID_REQU_EXPE = re.ID_REQU_EXPE;
                            rv.ID_TYPE_VIAT = idtr;
                            rv.AMOUNT = tt;

                            dbc.REQUEST_VIATICAL.Add(rv);
                            dbc.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Error = (e.InnerException == null ? e.Message : e.InnerException.Message);
                return Content("<script type='text/javascript'> function init() {" +
                                "if(top.uploadDoneNR) top.uploadDoneNR('ERROR','" + Error.ToString() + "');}window.onload=init;</script>");
            }
            return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDoneNR) top.uploadDoneNR('OK','Saved Succesfully...');}window.onload=init;</script>");
        }

        //LISTA LOS REQUERIMIENTOS PARA MOSTRARLOS EN EL ACCOUNTABILITY
        public ActionResult List()
        {
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());

            int idt = Convert.ToInt32(Request.Params["idt"]);
            int ids = Convert.ToInt32(Request.Params["ids"]);
            int idr = Convert.ToInt32(Request.Params["idr"]);
            int ID_TYPE_DOCU_SALE = Convert.ToInt32(Request.Params["ID_TYPE_DOCU_SALE"]);
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            string codigo = Convert.ToString(Request.Params["Codigo"]);
            var flag = true;
            string nroOP = null, nroOPs = Convert.ToString(Request.Params["nroOP"]);
            CultureInfo ci = new CultureInfo("Es-Es");
            if (!String.IsNullOrEmpty(nroOPs)) //Para filtros por OP
            {
                if (nroOPs.Trim() != "")
                {
                    nroOP = nroOPs;
                }
            }

            bool resp = false;
            int ID_PERS_ENTI_RESP = 0;

            var qPer = (from a in dbe.PERSON_ENTITY.Where(x => x.ID_ENTI1 == 9) //Listado de Personas
                        join b in dbe.CLASS_ENTITY on a.ID_ENTI2 equals b.ID_ENTI
                        select new
                        {
                            a.ID_PERS_ENTI,
                            User = b.FIR_NAME + " " + b.LAS_NAME,
                            Usua = b.FIR_NAME.Substring(0, 1) + ". " + b.LAS_NAME
                        });

            int qPerc = qPer.Count();
            var q = qPer.ToList();
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            string ID_PERS_ENTIs = Convert.ToString(ID_PERS_ENTI);
            //var qPer = dbe.ListadoViaticos(37361, ID_ACCO).ToList();

            try
            {
                // Muestra a los responsables de Caja Chica: Melisa, Juan, Ronald, Luis
                ID_PERS_ENTI_RESP = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 37) //ID_PARA=37=>RESPONSABLE PETTY CASH
                    .Where(x => x.ID_ACCO == ID_ACCO)
                    .Where(x => x.VAL_ACCO_PARA == ID_PERS_ENTIs).Count();

                if (ID_PERS_ENTI_RESP > 0) //Si es que se ha logueado un reposnsable de Caja chica. 
                {
                    ID_PERS_ENTI_RESP = ID_PERS_ENTI;
                    resp = true;
                }
            }
            catch
            {

            }
            //Hasta aca se ha obtenido una el responsable de caja chica
            //Verifica si tiene permisos para aprobar requerimientos
            Boolean UserApp = false;
            var qApp = dbc.PETTY_CASH_ASSIGNED.Where(x => x.ID_PERS_ENTI_APPR == ID_PERS_ENTI && x.VIG_PETT_CASH_ASSI == true && x.ID_PERS_ENTI_APPR_VIAT == ID_PERS_ENTI);
            if (qApp.Count() > 0)
            {
                UserApp = qApp.First().APP_ANY_REQU.Value; // Aprueba cualquier  Requerimiento
            }

            //Consigue los roles del usuario            
            Boolean UserAccount = false;

            if ((int)Session["ADMINISTRADOR"] == 1 || (int)Session["FINANZAS"] == 1)
            {
                UserAccount = true;
            }

            
            var qRequest = dbe.ListadoPersonalViaticosOperaciones(Convert.ToInt32(UserAccount), Convert.ToInt32(UserApp), ID_PERS_ENTI, ID_ACCO).ToList();
            var qres = qRequest.ToList();
            var qRequestT = qRequest.Where(x => flag == true);
            //-----------------------------------------FILTROS--------------------------------------------------
            if (idt != 0)
            { qRequestT = qRequestT.Where(x => x.ID_TYPE_DELI_SUST == idt); }
            if (ids != 0)
            { qRequestT = qRequestT.Where(x => x.ID_STAT_REQU_EXPE == ids); }
            if (idr != 0)
            { qRequestT = qRequestT.Where(x => x.ID_PERS_ENTI_REQU == idr); }
            if (!String.IsNullOrEmpty(nroOP))
            { qRequestT = qRequestT.Where(x => x.NUM_DOCU_SALE.ToLower().Contains(nroOP.ToLower())); }
            if (ID_TYPE_DOCU_SALE != 0)
            { qRequestT = qRequestT.Where(x => x.ID_TYPE_DOCU_SALE == ID_TYPE_DOCU_SALE); }
            if (!String.IsNullOrEmpty(codigo))
            { qRequestT = qRequestT.Where(x => x.COD_REQU_EXPE.ToLower().Contains(codigo.ToLower())); }
            //------------------------------------------------------------------------------------------------------

            int tt = qRequestT.Count();
            var querydetalle = qRequest.ToList();
            int fil = tt - skip;
            if (ID_PERS_ENTI != 1007)
            {
                var usuario = dbe.ValidaUsuario(ID_PERS_ENTI).Single();
                if (usuario.ID_CHAR_JEF.Value == 1269 || usuario.ID_CHAR_PARE == 1269 || usuario.ID_CHAR_PERS == 1269)
                {
                    var query = dbe.ListadoPersonalViaticos(ID_PERS_ENTI, 4, 0, UserApp, UserAccount, resp, idt, ids, idr, ID_TYPE_DOCU_SALE, nroOP).ToList().Skip(skip).Take(take);
                    if (query.Count() == 0)
                        return Content("<script>alert('No se encontraron registros');</script>");
                    else
                        return Json(new { Data = query, Count = query.First().tt }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //Lista todos los requerimientos que se mostraran con toda su información

                    var query = (from a in qRequestT.OrderBy(x => x.ORD_STAT)
                                           .ThenByDescending(x => x.DAT_REGI).Skip(skip).Take(take)
                                 join b in dbc.TYPE_DELI_SUST on a.ID_TYPE_DELI_SUST equals b.ID_TYPE_DELI_SUST
                                 join d in qPer on a.ID_PERS_ENTI_REQU equals d.ID_PERS_ENTI
                                 join e in qPer on a.ID_PERS_ENTI_APPR equals e.ID_PERS_ENTI into le
                                 from xe in le.DefaultIfEmpty()
                                 join f in qPer on a.ID_PERS_ENTI_APPR_VIAT equals f.ID_PERS_ENTI into lf
                                 from xf in lf.DefaultIfEmpty()
                                 join o in dbc.DOCUMENT_SALE on a.ID_DOCU_SALE equals o.ID_DOCU_SALE into lo
                                 from xo in lo.DefaultIfEmpty()
                                 join to in dbc.TYPE_DOCUMENT_SALE on (xo == null ? 0 : xo.ID_TYPE_DOCU_SALE) equals to.ID_TYPE_DOCU_SALE into lto
                                 from xto in lto.DefaultIfEmpty()
                                 select new
                                 {
                                     a.ID_DELI_SUST,
                                     a.ID_REQU_EXPE,
                                     a.COD_REQU_EXPE,
                                     a.JUSTIFICATION,
                                     a.ID_PERS_ENTI_REQU,
                                     a.ID_PERS_ENTI_APPR,
                                     NAM_TYPE_DELI_SUST = b.NAM_TYPE_DELI_SUST_SPAN,
                                     b.ID_TYPE_DELI_SUST,
                                     ID_PERS_ENTI_APPR_VIAT = a.ID_PERS_ENTI_APPR_VIAT == null ? 0 : a.ID_PERS_ENTI_APPR_VIAT,
                                     NAM_STAT_REQU_EXPE = ProperCase(a.NAM_STAT_REQU_EXPE_SPAN),
                                     a.ID_STAT_REQU_EXPE,
                                     a.COLOR,
                                     a.NAM_ABBR,
                                     a.COD_TICK,
                                     NUM_DAYS = a.NUM_DAYS == null ? 0 : a.NUM_DAYS,
                                     DAT_DEPA = a.DAT_DEPA == null ? "" : String.Format("{0:d}", a.DAT_DEPA),
                                     DESTINATION = a.DESTINATION == null ? "" : a.DESTINATION,
                                     NUM_OP = (xto == null ? "" : xto.NAM_TYPE_DOCU_SALE.Trim() + " ") + (xo == null ? "" : xo.NUM_DOCU_SALE.Trim()),
                                     ID_OP = (xo == null ? 0 : xo.ID_DOCU_SALE),
                                     a.ID_TICK,
                                     MONEY = a.CURRENCY,
                                     CURRENCY = (a.CURRENCY == "MN" ? "S/." : "US$"),
                                     NAM_CURR = (a.CURRENCY == "MN" ? ResourceLanguaje.Resource.PeruvianNuevoSol : ResourceLanguaje.Resource.DollarAmerican),
                                     AMOUNT = (a.AMOUNT == null ? "-" : String.Format("{0:N2}", a.AMOUNT)),
                                     TOTAL = a.AMOUNT,
                                     DAY_NAME = (a.DAT_REGI == null ? "" : ci.DateTimeFormat.GetDayName(Convert.ToDateTime(a.DAT_REGI).DayOfWeek)),
                                     MONTH_YEAR = (a.DAT_REGI == null ? "" : String.Format("{0:MMM dd yyyy}", a.DAT_REGI).ToLower()),
                                     DAT_LONG = (a.DAT_REGI == null ? "" : ci.DateTimeFormat.GetDayName(Convert.ToDateTime(a.DAT_REGI).DayOfWeek) + " " + String.Format("{0:MMM dd yyyy}", a.DAT_REGI).ToLower()),
                                     TIME = (a.DAT_REGI == null ? "" : String.Format("{0:hh:mm tt}", a.DAT_REGI).ToUpper()),
                                     UserRequest = d.User.ToLower(),
                                     UserApprov = (b.ID_TYPE_DELI_SUST == 1 ? (xe == null ? "" : xe.User.ToLower()) :
                                                 (xe != null && xf != null ? (a.ID_PERS_ENTI_APPR == a.ID_PERS_ENTI_APPR_VIAT ? xe.User.ToLower() :
                                                                              xe.Usua.ToLower() + " / " + xf.Usua.ToLower()) :
                                                 (xe == null ? "" : xe.User.ToLower()))),
                                     //(xe == null ? "" : xe.Usua.ToLower() + (xf == null ? "" : " / " + xf.Usua.ToLower()))),
                                     OtherAppro = xf == null ? "" : xf.Usua.ToLower(),
                                     AbrevAppro = xe == null ? "" : xe.Usua.ToLower(),
                                     Alto = (fil > 9 ? 865 : (79 * fil) + 75),
                                     SwApproval = ((ID_PERS_ENTI == a.ID_PERS_ENTI_APPR || UserApp == true) && a.ID_STAT_REQU_EXPE == 1 ? true :
                                                    ID_PERS_ENTI == a.ID_PERS_ENTI_APPR_VIAT && (a.ID_STAT_REQU_EXPE == 1 || a.ID_STAT_REQU_EXPE == 8) ? true : false // Muestra los botones que pueda aprobars
                                                  ),
                                     SwDelete = ((ID_PERS_ENTI == a.ID_PERS_ENTI_REQU || ID_PERS_ENTI == a.ID_PERS_ENTI_APPR || ID_PERS_ENTI == 1007) && ((a.ID_TYPE_DELI_SUST == 2 && (a.ID_STAT_REQU_EXPE == 1 || a.ID_STAT_REQU_EXPE == 2 || a.ID_STAT_REQU_EXPE == 3)) || ((a.ID_TYPE_DELI_SUST == 1 || a.ID_TYPE_DELI_SUST == 3) && (a.ID_STAT_REQU_EXPE == 5))) ? true : false),// Muestra los botones que pueda eliminar
                                     SwPayment = ((ID_PERS_ENTI == a.ID_PERS_ENTI_ASSI || UserAccount == true) && ((a.ID_STAT_REQU_EXPE == 9 ? true : false) || (a.ID_STAT_REQU_EXPE == 2 ? true : false))),
                                     //SwPayment = ((ID_PERS_ENTI == a.ID_PERS_ENTI_ASSI || UserAccount == true) && ((a.ID_STAT_REQU_EXPE == 9 ? true : false) || (a.ID_STAT_REQU_EXPE == 2 ? true : false))),
                                     SwxRendir = ((ID_PERS_ENTI == a.ID_PERS_ENTI_REQU || resp) && a.ID_STAT_REQU_EXPE == 5 ? true :
                                                  (ID_PERS_ENTI == a.ID_PERS_ENTI_REQU || resp) && a.ID_STAT_REQU_EXPE == 3 ? CountApproval(Convert.ToInt32(a.ID_REQU_EXPE)) : CountApproval(Convert.ToInt32(a.ID_REQU_EXPE))),
                                     SwRendir = ((ID_PERS_ENTI == a.ID_PERS_ENTI_ASSI || resp) && a.ID_STAT_REQU_EXPE == 6 ? true : false),
                                     SwPrint = (b.ID_TYPE_DELI_SUST == 2 && (a.ID_STAT_REQU_EXPE == 6 || a.ID_STAT_REQU_EXPE == 7) && (ID_PERS_ENTI == a.ID_PERS_ENTI_REQU || resp)),
                                 });

                    var p = query.ToList();
                    if (query.Count() == 0)
                        return Content("<script>alert('No se encontraron registros');</script>");
                    else
                        
                        return Json(new { Data = query.ToList(), Count = tt }, JsonRequestBehavior.AllowGet);
                }
                //
            }
            else
            {
                var query = (from a in qRequestT.ToList().OrderBy(x => x.ORD_STAT)
                                           .ThenByDescending(x => x.DAT_REGI).Skip(skip).Take(take)
                             join b in dbc.TYPE_DELI_SUST on a.ID_TYPE_DELI_SUST equals b.ID_TYPE_DELI_SUST
                             join d in qPer on a.ID_PERS_ENTI_REQU equals d.ID_PERS_ENTI
                             join e in qPer on a.ID_PERS_ENTI_APPR equals e.ID_PERS_ENTI into le
                             from xe in le.DefaultIfEmpty()
                             join f in qPer on a.ID_PERS_ENTI_APPR_VIAT equals f.ID_PERS_ENTI into lf
                             from xf in lf.DefaultIfEmpty()
                             join o in dbc.DOCUMENT_SALE on a.ID_DOCU_SALE equals o.ID_DOCU_SALE into lo
                             from xo in lo.DefaultIfEmpty()
                             join to in dbc.TYPE_DOCUMENT_SALE on (xo == null ? 0 : xo.ID_TYPE_DOCU_SALE) equals to.ID_TYPE_DOCU_SALE into lto
                             from xto in lto.DefaultIfEmpty()
                             select new
                             {
                                 a.ID_DELI_SUST,
                                 a.ID_REQU_EXPE,
                                 a.COD_REQU_EXPE,
                                 a.JUSTIFICATION,
                                 a.ID_PERS_ENTI_REQU,
                                 a.ID_PERS_ENTI_APPR,
                                 NAM_TYPE_DELI_SUST = b.NAM_TYPE_DELI_SUST_SPAN,
                                 b.ID_TYPE_DELI_SUST,
                                 ID_PERS_ENTI_APPR_VIAT = a.ID_PERS_ENTI_APPR_VIAT == null ? 0 : a.ID_PERS_ENTI_APPR_VIAT,
                                 NAM_STAT_REQU_EXPE = ProperCase(a.NAM_STAT_REQU_EXPE_SPAN),
                                 a.ID_STAT_REQU_EXPE,
                                 a.COLOR,
                                 a.NAM_ABBR,
                                 a.COD_TICK,
                                 NUM_DAYS = a.NUM_DAYS == null ? 0 : a.NUM_DAYS,
                                 DAT_DEPA = a.DAT_DEPA == null ? "" : String.Format("{0:d}", a.DAT_DEPA),
                                 DESTINATION = a.DESTINATION == null ? "" : a.DESTINATION,
                                 NUM_OP = (xto == null ? "" : xto.NAM_TYPE_DOCU_SALE.Trim() + " ") + (xo == null ? "" : xo.NUM_DOCU_SALE.Trim()),
                                 ID_OP = (xo == null ? 0 : xo.ID_DOCU_SALE),
                                 a.ID_TICK,
                                 MONEY = a.CURRENCY,
                                 CURRENCY = (a.CURRENCY == "MN" ? "S/." : "US$"),
                                 NAM_CURR = (a.CURRENCY == "MN" ? ResourceLanguaje.Resource.PeruvianNuevoSol : ResourceLanguaje.Resource.DollarAmerican),
                                 AMOUNT = (a.AMOUNT == null ? "-" : String.Format("{0:N2}", a.AMOUNT)),
                                 TOTAL = a.AMOUNT,
                                 DAY_NAME = (a.DAT_REGI == null ? "" : ci.DateTimeFormat.GetDayName(Convert.ToDateTime(a.DAT_REGI).DayOfWeek)),
                                 MONTH_YEAR = (a.DAT_REGI == null ? "" : String.Format("{0:MMM dd yyyy}", a.DAT_REGI).ToLower()),
                                 DAT_LONG = (a.DAT_REGI == null ? "" : ci.DateTimeFormat.GetDayName(Convert.ToDateTime(a.DAT_REGI).DayOfWeek) + " " + String.Format("{0:MMM dd yyyy}", a.DAT_REGI).ToLower()),
                                 TIME = (a.DAT_REGI == null ? "" : String.Format("{0:hh:mm tt}", a.DAT_REGI).ToUpper()),
                                 UserRequest = d.User.ToLower(),
                                 UserApprov = (b.ID_TYPE_DELI_SUST == 1 ? (xe == null ? "" : xe.User.ToLower()) :
                                             (xe != null && xf != null ? (a.ID_PERS_ENTI_APPR == a.ID_PERS_ENTI_APPR_VIAT ? xe.User.ToLower() :
                                                                          xe.Usua.ToLower() + " / " + xf.Usua.ToLower()) :
                                             (xe == null ? "" : xe.User.ToLower()))),
                                 //(xe == null ? "" : xe.Usua.ToLower() + (xf == null ? "" : " / " + xf.Usua.ToLower()))),
                                 OtherAppro = xf == null ? "" : xf.Usua.ToLower(),
                                 AbrevAppro = xe == null ? "" : xe.Usua.ToLower(),
                                 Alto = (fil > 9 ? 865 : (79 * fil) + 75),
                                 SwApproval = ((ID_PERS_ENTI == a.ID_PERS_ENTI_APPR || UserApp == true || ID_PERS_ENTI == 1007) && a.ID_STAT_REQU_EXPE == 1 ? true :
                                                ID_PERS_ENTI == a.ID_PERS_ENTI_APPR_VIAT && (a.ID_STAT_REQU_EXPE == 1 || a.ID_STAT_REQU_EXPE == 8) ? true : false // Muestra los botones que pueda aprobar
                                              ),
                                 SwDelete = ((ID_PERS_ENTI == a.ID_PERS_ENTI_REQU || ID_PERS_ENTI == a.ID_PERS_ENTI_APPR || ID_PERS_ENTI == 1007) && ((a.ID_TYPE_DELI_SUST == 2 && (a.ID_STAT_REQU_EXPE == 1 || a.ID_STAT_REQU_EXPE == 2 || a.ID_STAT_REQU_EXPE == 3)) || ((a.ID_TYPE_DELI_SUST == 1 || a.ID_TYPE_DELI_SUST == 3) && (a.ID_STAT_REQU_EXPE == 5))) ? true : false),// Muestra los botones que pueda eliminar
                                 SwPayment = ((ID_PERS_ENTI == a.ID_PERS_ENTI_ASSI || UserAccount == true) && ((a.ID_STAT_REQU_EXPE == 9 ? true : false)||(a.ID_STAT_REQU_EXPE == 2 ? true : false))),
                                 SwxRendir = ((ID_PERS_ENTI == a.ID_PERS_ENTI_REQU || resp) && a.ID_STAT_REQU_EXPE == 5 ? true :
                                              (ID_PERS_ENTI == a.ID_PERS_ENTI_REQU || resp) && a.ID_STAT_REQU_EXPE == 3 ? CountApproval(Convert.ToInt32(a.ID_REQU_EXPE)) : CountApproval(Convert.ToInt32(a.ID_REQU_EXPE))),
                                 SwRendir = ((ID_PERS_ENTI == a.ID_PERS_ENTI_ASSI || resp) && a.ID_STAT_REQU_EXPE == 6 ? true : false),
                                 SwPrint = (b.ID_TYPE_DELI_SUST == 2 && (a.ID_STAT_REQU_EXPE == 6 || a.ID_STAT_REQU_EXPE == 7) && (ID_PERS_ENTI == a.ID_PERS_ENTI_REQU || resp)),
                             });
                var p = query.ToList();
                if (query.Count() == 0)
                    return Content("<script>alert('No se encontraron registros');</script>");
                else
                    return Json(new { Data = query, Count = tt }, JsonRequestBehavior.AllowGet);
            }
        }

        public bool CountApproval(int id = 0)
        {
            try
            {
                bool ctd = false;
                var query = dbc.REQUEST_EXPENSE_ACTIVITY.Where(x => x.ID_REQU_EXPE == id && x.ID_STAT_REQU_EXPE == 2);
                if (query.Count() > 0) { ctd = true; }

                return ctd;
            }
            catch
            {
                return false;
            }
        }

        [Authorize]
        public string UpdateStatus(int id = 0, int id1 = 0, int id2 = 0)
        {
            //id: ID_REQU_EXPE, id1:Status (1:Aprobar, 2:Rechazar, 3:Cancelar) id2:ID_STAT_REQU_EXPE(1:New Request, 2: Approved, 3:Reject, 4:Cancelled, 5:Pending, 6:Validate, 7:Liquidated, 8:Pre-Aproved)
            REQUEST_EXPENSE re = dbc.REQUEST_EXPENSE.Find(id);
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            if (re != null && ID_PERS_ENTI != 0)
            {
                if (re.ID_STAT_REQU_EXPE == id2)//Verificando si su estado no ha cambiado
                {
                    if (id1 == 1) // Estado por aprobar 
                    {   
                           
                            re.ID_STAT_REQU_EXPE = 2; // Cambia el estado ha aprobado. 
                            //re.ID_PERS_ENTI_ASSI = pca.First().ID_PERS_ENTI_ASSI; // Asigna a quien debe pagara la caja chica(Ejemplo Melisa)

                            if (re.ID_TYPE_DELI_SUST == 2) //aprobación de viatico
                            {
                                //Obteniendo la ID_PERS_ENTI del pagador por defecto
                                var qPARA = (from ce in dbe.CLASS_ENTITY
                                             join pe in dbe.PERSON_ENTITY on ce.ID_ENTI equals pe.ID_ENTI2
                                             join pc in dbe.PERSON_CONTRACT.Where(x => x.VIG_CONT == true && x.LAS_CONT == true) on pe.ID_PERS_ENTI equals pc.ID_PERS_ENTI into pcGroup
                                             from pc in pcGroup.DefaultIfEmpty()
                                             join c in dbe.CHARTs on pc.ID_CHAR equals c.ID_CHAR into cGroup
                                             from c in cGroup.DefaultIfEmpty()
                                             join nc in dbe.NAME_CHART on c.ID_NAM_CHAR equals nc.ID_NAM_CHAR into ncGroup
                                             from nc in ncGroup.DefaultIfEmpty()
                                             where nc.NAM_CHAR.Contains("tesorer")
                                                     && pe.VIG_PERS_ENTI == true
                                                     && ce.VIG_ENTI == true
                                                     && c.ID_ACCO == 4
                                             select new { ID_PERS_ENTI = pe.ID_PERS_ENTI }); // ID_PARA=34 Pago de Viáticos
                            if (qPARA.Count() > 0)
                                {
                                    re.ID_PERS_ENTI_ASSI = Convert.ToInt32(qPARA.Single().ID_PERS_ENTI); // Assigna a Milagros Romero. 
                                }


                                var usuario = dbe.ValidaUsuario(ID_PERS_ENTI).Single();
                                if (usuario.ID_CHAR_JEF.Value == 1269 || usuario.ID_CHAR_PARE == 1269) //JEFES OUTSOURCING
                                {
                                    re.ID_STAT_REQU_EXPE = 2;
                                }
                                else
                                if (ID_PERS_ENTI != re.ID_PERS_ENTI_APPR_VIAT)
                                {
                                    re.ID_STAT_REQU_EXPE = 8; //Pre-Aprobado Viatico
                                }
                            }
                            else if (re.ID_TYPE_DELI_SUST == 3)
                            {
                                if (re.ID_PERS_ENTI_APPR != ID_PERS_ENTI)
                                {
                                    re.ID_PERS_ENTI_APPR_VIAT = ID_PERS_ENTI;
                                }
                                re.ID_STAT_REQU_EXPE = 6;
                            }
                            else
                            { //Caja chica aprobado por otro usuario
                                if (re.ID_PERS_ENTI_APPR != ID_PERS_ENTI)
                                {
                                    re.ID_PERS_ENTI_APPR_VIAT = ID_PERS_ENTI;
                                }
                                re.ID_STAT_REQU_EXPE = 6;
                            }
                        //}
                    }
                    else if (id1 == 2) //Rechazar
                    { //Se actualiza el ID del personal que rechaza la solicitud
                        re.ID_STAT_REQU_EXPE = 3;
                        re.ID_PERS_ENTI_APPR = ID_PERS_ENTI;
                    }
                    else if (id1 == 3)
                    { //Cancelar
                      //Validar si el Request Expense ha sido pagado

                        re.ID_STAT_REQU_EXPE = 4;
                        try
                        {
                            if (re.ID_DELI_SUST != null)
                            {
                                int id_deli_sust = Convert.ToInt32(re.ID_DELI_SUST);
                                //Actualizando saldo disponible
                                DELIVERY_SUSTAIN dsu = dbc.DELIVERY_SUSTAIN.Find(id_deli_sust);

                                //dsu.AMO_AVAI = dsu.AMO_AVAI + re.AMOUNT;
                                dsu.AMO_AVAI = GetTotalAvailable(dsu.ID_DELI_SUST) + re.AMOUNT; // Se devuelve el monto, calculando el monto disponible dinamicamente. 
                                dbc.DELIVERY_SUSTAIN.Attach(dsu);
                                dbc.Entry(dsu).State = EntityState.Modified;
                                dbc.SaveChanges();
                            }
                        }
                        catch
                        {

                        }


                    } //Cancelar

                    dbc.REQUEST_EXPENSE.Attach(re);
                    dbc.Entry(re).State = EntityState.Modified;
                    dbc.SaveChanges();

                    //SendMail smail = new SendMail();
                    //smail.RequestAccountability(re);


                    REQUEST_EXPENSE_ACTIVITY rea = new REQUEST_EXPENSE_ACTIVITY();
                    rea.UserId = Convert.ToInt32(Session["UserId"]);
                    rea.ID_STAT_REQU_EXPE = re.ID_STAT_REQU_EXPE;
                    rea.DAT_STAR = DateTime.Now;
                    rea.ID_REQU_EXPE = re.ID_REQU_EXPE;
                    rea.SEND_MAIL = false;

                    dbc.REQUEST_EXPENSE_ACTIVITY.Add(rea);
                    dbc.SaveChanges();
                }
                else
                {
                    return "Change Status";
                }
            }
            return "OK";
        }

        [Authorize]
        //RECEPCION DE DOCUMENTOS
        public string ReceptionRequest(int id = 0)
        {
            REQUEST_EXPENSE re = dbc.REQUEST_EXPENSE.Find(id);

            var amount = (from de in dbc.MontoRecepcion(id) select new {de.AMOUNT});

            decimal TotalRequest = Convert.ToDecimal(re.AMOUNT);
            decimal TotalRecepcion = amount.Sum(x => Convert.ToDecimal(x.AMOUNT));
            int sw = 0;

            int ID_TYPE_DELI_SUST = Convert.ToInt32(re.ID_TYPE_DELI_SUST);

            if (re != null)
            {
                if (re.ID_STAT_REQU_EXPE == 5)//Verificando si su estado actual aun es Por rendir documento
                {
                    if(ID_TYPE_DELI_SUST == 2)
                    {
                        re.ID_STAT_REQU_EXPE = 6;
                        re.SEND_MAIL = false;

                        if (TotalRequest > TotalRecepcion)
                        {
                            re.SEND_MAIL_USUARIO_DEVUELVE = false;
                        }
                        
                    }
                    else
                    {
                        if(TotalRequest == TotalRecepcion)
                        { re.ID_STAT_REQU_EXPE = 1; }
                        else { sw = 1; }
                    }
                    if (sw == 1)
                    {
                        return "Error Editar";
                    }

                    dbc.REQUEST_EXPENSE.Attach(re);
                    dbc.Entry(re).State = EntityState.Modified;
                    dbc.SaveChanges();

                    REQUEST_EXPENSE_ACTIVITY rea = new REQUEST_EXPENSE_ACTIVITY();
                    rea.UserId = Convert.ToInt32(Session["UserId"]);
                    rea.ID_STAT_REQU_EXPE = re.ID_STAT_REQU_EXPE;
                    rea.DAT_STAR = DateTime.Now;
                    rea.ID_REQU_EXPE = re.ID_REQU_EXPE;

                    dbc.REQUEST_EXPENSE_ACTIVITY.Add(rea);
                    dbc.SaveChanges();

                    
                }
                else
                {
                    bool stt = CountApproval(id);//contando si ya ha sido aprobado
                    if (re.ID_STAT_REQU_EXPE == 3 && stt == true) //si proviene de un rechazo de recepcion
                    {
                        re.ID_STAT_REQU_EXPE = 6;
                        dbc.REQUEST_EXPENSE.Attach(re);
                        dbc.Entry(re).State = EntityState.Modified;
                        dbc.SaveChanges();

                        REQUEST_EXPENSE_ACTIVITY rea = new REQUEST_EXPENSE_ACTIVITY();
                        rea.UserId = Convert.ToInt32(Session["UserId"]);
                        rea.ID_STAT_REQU_EXPE = re.ID_STAT_REQU_EXPE;
                        rea.DAT_STAR = DateTime.Now;
                        rea.ID_REQU_EXPE = re.ID_REQU_EXPE;

                        dbc.REQUEST_EXPENSE_ACTIVITY.Add(rea);
                        dbc.SaveChanges();
                    }
                    else
                    {
                        return "Change Status";
                    }
                }
            }
            else { return "No register"; }

            return "OK";
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RejectReception(int id = 0)
        {

            REQUEST_EXPENSE re = dbc.REQUEST_EXPENSE.Find(id);
            string REA_REQ = Convert.ToString(Request.Form["REASON_REJECT"]);
            string msn = "";

            if (REA_REQ.Trim().Length == 0)
            {
                msn = ResourceLanguaje.Resource.ReasonReject;
                return Content("ERROR1");
            }
            try
            {
                if (re != null)
                {
                    if (re.ID_STAT_REQU_EXPE.Value == 6)//Verificando si su estado actual aun es Solicitud de Recepcion
                    {
                        re.ID_STAT_REQU_EXPE = 3;
                        dbc.REQUEST_EXPENSE.Attach(re);
                        dbc.Entry(re).State = EntityState.Modified;
                        dbc.SaveChanges();

                        DELIVERY_SUSTAIN ds = dbc.DELIVERY_SUSTAIN.Find(re.ID_DELI_SUST);
                        ds.AMO_AVAI = GetTotalAvailable(ds.ID_DELI_SUST);
                        dbc.DELIVERY_SUSTAIN.Attach(ds);
                        dbc.Entry(ds).State = EntityState.Modified;
                        dbc.SaveChanges();

                        //var qde= (from de in dbc.DOCUMENT_EXPENSE.Where(x=>x.ID_REQU_EXPE==re.ID_REQU_EXPE)
                        //          select new
                        //          {
                        //              de.ID_DOCU_EXPE,
                        //          });


                        //foreach(var d in qde)
                        //{
                        //    DOCUMENT_EXPENSE docexp=dbc.DOCUMENT_EXPENSE.Find(d.ID_DOCU_EXPE);

                        //    docexp.ID_STAT_DOCU_EXPE = 2;
                        //    dbc.DOCUMENT_EXPENSE.Attach(docexp);
                        //    dbc.Entry(docexp).State = EntityState.Modified;
                        //    dbc.SaveChanges();

                        //    if(docexp.ID_TYPE_DOCU_EXPE==4)
                        //    {
                        //        var qdm= (from dm in dbc.DETAIL_MOBILITY.Where(x=>x.ID_DOCU_EXPE==d.ID_DOCU_EXPE)
                        //                      select new
                        //                      {
                        //                          dm.ID_DETA_MOVI,
                        //                      });

                        //        foreach(var m in qdm)
                        //        {
                        //            DETAIL_MOBILITY detmov= dbc.DETAIL_MOBILITY.Find(m.ID_DETA_MOVI);
                        //            detmov.ID_STAT_DETA_MOBI=2;
                        //            dbc.DETAIL_MOBILITY.Attach(detmov);
                        //            dbc.Entry(detmov).State = EntityState.Modified;
                        //            dbc.SaveChanges();
                        //    }
                        //  }
                        //}
                        //Descomentar
                        SendMail smail = new SendMail();
                        smail.RequestAccountability(re);

                        REQUEST_EXPENSE_ACTIVITY rea = new REQUEST_EXPENSE_ACTIVITY();
                        rea.UserId = Convert.ToInt32(Session["UserId"]);
                        rea.ID_STAT_REQU_EXPE = re.ID_STAT_REQU_EXPE;
                        rea.REASON_REJECT = REA_REQ;
                        rea.DAT_STAR = DateTime.Now;
                        rea.ID_REQU_EXPE = re.ID_REQU_EXPE;

                        dbc.REQUEST_EXPENSE_ACTIVITY.Add(rea);
                        dbc.SaveChanges();
                    }
                    else
                    {
                        return Content("ERROR2");
                    }
                }

                else
                {
                    return Content("ERROR3");
                }
            }
            catch
            {
                return Content("ERROR");
            }
            return Content("OK");
        }


        [Authorize]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult RejectRequest(int id = 0)
        {

            REQUEST_EXPENSE re = dbc.REQUEST_EXPENSE.Find(id);
            string REA_REQ = Convert.ToString(Request.Form["REASON_REJECT"]);
            //string msn = "";

            if (REA_REQ.Trim().Length == 0)
            {
                //msn = ResourceLanguaje.Resource.ReasonReject;
                return Content("ERROR1");
            }

            if (re != null)
            {
                if (re.ID_STAT_REQU_EXPE.Value == 1 || re.ID_STAT_REQU_EXPE.Value == 8)//Verificando si su estado actual aun es Por Aprobar
                {
                    re.ID_STAT_REQU_EXPE = 3;
                    dbc.REQUEST_EXPENSE.Attach(re);
                    dbc.Entry(re).State = EntityState.Modified;
                    dbc.SaveChanges();
                    //Descomentar
                    SendMail smail = new SendMail();
                    smail.RequestAccountability(re);

                    REQUEST_EXPENSE_ACTIVITY rea = new REQUEST_EXPENSE_ACTIVITY();
                    rea.UserId = Convert.ToInt32(Session["UserId"]);
                    rea.ID_STAT_REQU_EXPE = re.ID_STAT_REQU_EXPE;
                    rea.REASON_REJECT = REA_REQ;
                    rea.DAT_STAR = DateTime.Now;
                    rea.ID_REQU_EXPE = re.ID_REQU_EXPE;

                    dbc.REQUEST_EXPENSE_ACTIVITY.Add(rea);
                    dbc.SaveChanges();
                }
                else
                {
                    return Content("ERROR2");
                }
            }
            else
            {
                return Content("ERROR3");
            }

            return Content("OK");
        }

        public static string ProperCase(string s)
        {
            s = s.ToLower();
            string sProper = "";

            char[] seps = new char[] { ' ' };
            foreach (string ss in s.Split(seps))
            {
                sProper += char.ToUpper(ss[0]);
                sProper += (ss.Substring(1, ss.Length - 1) + ' ');
            }

            return sProper;
        }

        [Authorize]
        public string ApprovalAll(string id = "")
        {

            int ID_PERS_ENTI_APPR = Convert.ToInt32(Session["ID_PERS_ENTI"]);

            var pca = dbc.PETTY_CASH_ASSIGNED.Where(x => x.ID_PERS_ENTI_APPR == ID_PERS_ENTI_APPR
                      && x.VIG_PETT_CASH_ASSI == true);

            if (pca.Count() == 0)
            {
                return "Not Petty Cash";
            }
            else
            {
                string[] iden = id.Split(new char[] { '|' });
                DateTime fec = DateTime.Now;
                foreach (string s in iden)
                {
                    int ID_REQU_EXPE = Convert.ToInt32(s);
                    REQUEST_EXPENSE re = dbc.REQUEST_EXPENSE.Find(ID_REQU_EXPE);
                    re.ID_STAT_REQU_EXPE = 2;
                    re.ID_PERS_ENTI_ASSI = pca.First().ID_PERS_ENTI_ASSI;

                    dbc.REQUEST_EXPENSE.Attach(re);
                    dbc.Entry(re).State = EntityState.Modified;
                    dbc.SaveChanges();

                    REQUEST_EXPENSE_ACTIVITY rea = new REQUEST_EXPENSE_ACTIVITY();
                    rea.UserId = Convert.ToInt32(Session["UserId"]);
                    rea.ID_STAT_REQU_EXPE = 2;
                    rea.DAT_STAR = DateTime.Now;
                    rea.ID_REQU_EXPE = re.ID_REQU_EXPE;

                    dbc.REQUEST_EXPENSE_ACTIVITY.Add(rea);
                    dbc.SaveChanges();
                }
            }

            return "OK";
        }

        [Authorize]
        //-------------Pago de Requerimientos--------------
        public string PaymentRequest(int id = 0, int id1 = 0)
        {
            //id: ID_REQU_EXPE, id1: ID_DELI_SUST
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]); // ID_PERS_ENTI del que paga Ejem: Melisa
            REQUEST_EXPENSE re = dbc.REQUEST_EXPENSE.Find(id);
            if (re.ID_STAT_REQU_EXPE.Value == 2) //Verificando si su estado actual es Aprobado
            {
                //Validando que no tengo ninguna requerimiento o viatico en estado por rendir o solicitud de recepción
                var qCount = dbc.REQUEST_EXPENSE.Where(x => x.ID_TYPE_DELI_SUST == re.ID_TYPE_DELI_SUST &&
                                x.ID_PERS_ENTI_REQU == re.ID_PERS_ENTI_REQU && (x.ID_STAT_REQU_EXPE == 5 || x.ID_STAT_REQU_EXPE == 6));//(5=Pending, 6=Validate)

                if (qCount.Count() > 0)
                {
                    return "No register";
                }

                var ds = dbc.DELIVERY_SUSTAIN.Where(x => x.ID_DELI_SUST == id1 && x.ID_PERS_ENTI_ASSI == ID_PERS_ENTI && x.COIN == re.CURRENCY);// Valida si hay monto en la caja chica y el tipo de moneda es correcta

                //--------------Este AMO_AVAI se tiene que cambiar----------------------------

                if (ds.Count() > 0)
                {
                    re.ID_STAT_REQU_EXPE = 5;//Pendiente
                    re.ID_DELI_SUST = id1;
                    dbc.REQUEST_EXPENSE.Attach(re);
                    dbc.Entry(re).State = EntityState.Modified;
                    dbc.SaveChanges();

                    //Registrando la actividad
                    REQUEST_EXPENSE_ACTIVITY rea = new REQUEST_EXPENSE_ACTIVITY();
                    rea.UserId = Convert.ToInt32(Session["UserId"]);
                    rea.ID_STAT_REQU_EXPE = re.ID_STAT_REQU_EXPE;
                    rea.DAT_STAR = DateTime.Now;
                    rea.ID_REQU_EXPE = re.ID_REQU_EXPE;

                    dbc.REQUEST_EXPENSE_ACTIVITY.Add(rea);
                    dbc.SaveChanges();

                    //Actualizando saldo disponible
                    DELIVERY_SUSTAIN dsu = dbc.DELIVERY_SUSTAIN.Find(id1);
                    dsu.AMO_AVAI = GetTotalAvailable(dsu.ID_DELI_SUST);                        // Se tiene que modificar
                    dbc.DELIVERY_SUSTAIN.Attach(dsu);
                    dbc.Entry(dsu).State = EntityState.Modified;
                    dbc.SaveChanges();

                    try   // Envio de Correos
                    {
                        //Descomentar
                        SendMail mail = new SendMail();
                        //string HB = mail.NotificationPettyCashPayment(id);
                        mail.RequestAccountability(re);

                        PERSON_ENTITY_NOTIFICATION penReq = new PERSON_ENTITY_NOTIFICATION();
                        penReq.CREATED = DateTime.Now;
                        penReq.ID_PERS_NOTI = 3;
                        penReq.ID_PERS_ENTI = re.ID_PERS_ENTI_REQU;
                        penReq.UserId = rea.UserId;
                        dbe.PERSON_ENTITY_NOTIFICATION.Add(penReq);
                        dbe.SaveChanges();

                        PERSON_ENTITY_NOTIFICATION penApp = new PERSON_ENTITY_NOTIFICATION();
                        penApp.CREATED = DateTime.Now;
                        penApp.ID_PERS_NOTI = 3;
                        penApp.ID_PERS_ENTI = re.ID_PERS_ENTI_APPR;
                        penApp.UserId = rea.UserId;
                        dbe.PERSON_ENTITY_NOTIFICATION.Add(penApp);
                        dbe.SaveChanges();

                        PERSON_ENTITY_NOTIFICATION penAss = new PERSON_ENTITY_NOTIFICATION();
                        penAss.CREATED = DateTime.Now;
                        penAss.ID_PERS_NOTI = 3;
                        penAss.ID_PERS_ENTI = re.ID_PERS_ENTI_ASSI;
                        penAss.UserId = rea.UserId;
                        dbe.PERSON_ENTITY_NOTIFICATION.Add(penAss);
                        dbe.SaveChanges();
                    }
                    catch (Exception)
                    {
                    }
                }
                else //Saldo disponible insuficiente
                {
                    return "Saldo Insuficiente";
                }
            }
            else
            {
                return "Change Status";
            }
            return "OK";
        }

        [Authorize]
        public string ValidateRequest(int id = 0)
        {
            REQUEST_EXPENSE re = dbc.REQUEST_EXPENSE.Find(id);

            var amount = (from de in dbc.MontoRecepcion(id) select new { de.AMOUNT });

            decimal TotalRequest = Convert.ToDecimal(re.AMOUNT);
            decimal TotalRecepcion = amount.Sum(x => Convert.ToDecimal(x.AMOUNT));

            int countdocumentosobservados = dbc.DOCUMENT_EXPENSE.Where(x => x.ID_REQU_EXPE == re.ID_REQU_EXPE && x.DocumentoObservado == true).Count();
            if (countdocumentosobservados > 0)
            {
                return "Observacion Pendiente";
            }

            if (re != null)
            {
                
                if (re.ID_STAT_REQU_EXPE.Value == 6)//Verificando si su estado actual aun es Solicita recepcion
                {
                    if (re.ID_TYPE_DELI_SUST == 2)
                    {
                        re.ID_STAT_REQU_EXPE = 7;
                        re.SEND_MAIL = false;

                        if (TotalRequest > TotalRecepcion)
                        {
                            re.SEND_MAIL_USUARIO_DEVUELVE = false;
                            re.ID_STAT_REQU_EXPE = 7;
                        }
                        if (TotalRequest < TotalRecepcion)
                        {
                            re.ID_STAT_REQU_EXPE = 9;
                        }
                    }

                    if (re.ID_TYPE_DELI_SUST == 3)
                    {
                        re.ID_STAT_REQU_EXPE = 9;
                    }
                    if (re.ID_TYPE_DELI_SUST == 1)
                    {
                        re.ID_STAT_REQU_EXPE = 2;
                    }
                    
                    
                    dbc.REQUEST_EXPENSE.Attach(re);
                    dbc.Entry(re).State = EntityState.Modified;
                    dbc.SaveChanges();

                    REQUEST_EXPENSE_ACTIVITY rea = new REQUEST_EXPENSE_ACTIVITY();
                    rea.UserId = Convert.ToInt32(Session["UserId"]);
                    rea.ID_STAT_REQU_EXPE = re.ID_STAT_REQU_EXPE;
                    rea.DAT_STAR = DateTime.Now;
                    rea.ID_REQU_EXPE = re.ID_REQU_EXPE;

                    dbc.REQUEST_EXPENSE_ACTIVITY.Add(rea);
                    dbc.SaveChanges();

                    try
                    {

                        var docexp = (from de in dbc.DOCUMENT_EXPENSE.Where(x => x.ID_REQU_EXPE == re.ID_REQU_EXPE)
                                      select new
                                      {
                                          de.ID_DOCU_EXPE,
                                          de.ID_STAT_DOCU_EXPE,
                                          de.ID_TYPE_DOCU_EXPE,
                                      });

                        foreach (var doc in docexp) // Aprueba las movilidades
                        {
                            if (doc.ID_TYPE_DOCU_EXPE == 4)
                            {
                                var dm = (from d in dbc.DETAIL_MOBILITY.Where(x => x.ID_DOCU_EXPE == doc.ID_DOCU_EXPE)
                                          select new
                                          {
                                              d.ID_DETA_MOVI,
                                          });

                                foreach (var detmob in dm)
                                {
                                    var vdm = dbc.DETAIL_MOBILITY.Where(x => x.ID_DETA_MOVI == detmob.ID_DETA_MOVI).First();
                                    vdm.ID_STAT_DETA_MOBI = 3;
                                    vdm.DAT_APPR = DateTime.Now;

                                    dbc.DETAIL_MOBILITY.Attach(vdm);
                                    dbc.Entry(vdm).State = EntityState.Modified;
                                    dbc.SaveChanges();

                                }
                            }
                        }

                        var qde = (from dox in docexp.Where(x => x.ID_STAT_DOCU_EXPE == 1) //Aprueba los documentes expense
                                   select new
                                   {
                                       dox.ID_DOCU_EXPE,
                                   });

                        foreach (var dc in qde)
                        {
                            var dexpens = dbc.DOCUMENT_EXPENSE.Single(x => x.ID_DOCU_EXPE == dc.ID_DOCU_EXPE);

                            dexpens.ID_STAT_DOCU_EXPE = 3;
                            dexpens.DAT_APPR = DateTime.Now;

                            dbc.DOCUMENT_EXPENSE.Attach(dexpens);
                            dbc.Entry(dexpens).State = EntityState.Modified;
                            dbc.SaveChanges();

                        }

                        var ds = dbc.DELIVERY_SUSTAIN.Find(re.ID_DELI_SUST);

                        var dex = (from de in dbc.DOCUMENT_EXPENSE.Where(x => x.ID_STAT_DOCU_EXPE == 3 && x.ID_REQU_EXPE == re.ID_REQU_EXPE)
                                   join tde in dbc.TYPE_DOCUMENT_EXPENSE.Where(x => x.AFF_AVAI != false) on de.ID_TYPE_DOCU_EXPE equals tde.ID_TYPE_DOCU_EXPE

                                   select new
                                   {
                                       de.ID_REQU_EXPE,
                                       de.AMOUNT,
                                   });

                        decimal total = 0;

                        if (dex.Count() > 0)
                        {
                            var tg = (from d in dex.ToList()
                                      group d by new { d.ID_REQU_EXPE } into g
                                      select new
                                      {
                                          g.Key.ID_REQU_EXPE,
                                          TT = g.Sum(x => x.AMOUNT),
                                      }).FirstOrDefault();

                            total = Convert.ToDecimal(tg.TT);
                        }
                        else
                        {
                            total = 0;
                        }


                    }
                    catch(Exception e)
                    {
                        return "Error Transaction";
                    }
                    try
                    {
                        if (re.ID_TYPE_DELI_SUST == 1)
                        {
                            re.SEND_MAIL = false;
                            dbc.SaveChanges();
                        }
                        else if (re.ID_TYPE_DELI_SUST == 3)
                        {
                            re.SEND_MAIL = false;
                            dbc.SaveChanges();
                        }
                        else {
                            

                        SendMail smail = new SendMail();
                        smail.RequestAccountability(re);

                        PERSON_ENTITY_NOTIFICATION penReq = new PERSON_ENTITY_NOTIFICATION();
                        penReq.CREATED = DateTime.Now;
                        penReq.ID_PERS_NOTI = 4;
                        penReq.ID_PERS_ENTI = re.ID_PERS_ENTI_REQU;
                        penReq.UserId = rea.UserId;
                        dbe.PERSON_ENTITY_NOTIFICATION.Add(penReq);
                        dbe.SaveChanges();

                        PERSON_ENTITY_NOTIFICATION penApp = new PERSON_ENTITY_NOTIFICATION();
                        penApp.CREATED = DateTime.Now;
                        penApp.ID_PERS_NOTI = 4;
                        penApp.ID_PERS_ENTI = re.ID_PERS_ENTI_APPR;
                        penApp.UserId = rea.UserId;
                        dbe.PERSON_ENTITY_NOTIFICATION.Add(penApp);
                        dbe.SaveChanges();

                        PERSON_ENTITY_NOTIFICATION penAss = new PERSON_ENTITY_NOTIFICATION();
                        penAss.CREATED = DateTime.Now;
                        penAss.ID_PERS_NOTI = 4;
                        penAss.ID_PERS_ENTI = re.ID_PERS_ENTI_ASSI;
                        penAss.UserId = rea.UserId;
                        dbe.PERSON_ENTITY_NOTIFICATION.Add(penAss);
                        dbe.SaveChanges();
                            
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                {
                    return "Change Status";
                }
            }
            else { return "Change Status"; }

            return "OK";
        }

        public ActionResult ListUnsubstantiatedByID_DELI_SUST()
        {
            int ID_DELI_SUST = Convert.ToInt32(Request.Params["filter[filters][0][value]"].ToString());
            var qUser = (from a in dbe.CLASS_ENTITY.Where(x => x.COM_NAME == null)
                         join b in dbe.PERSON_ENTITY.Where(x => x.ID_ENTI1 == 9) on a.ID_ENTI equals b.ID_ENTI2
                         select new
                         {
                             USER = a.FIR_NAME + " " + a.LAS_NAME,
                             b.ID_PERS_ENTI
                         });

            var query = (from a in dbc.REQUEST_EXPENSE.Where(x => x.ID_DELI_SUST == ID_DELI_SUST &&
                                (x.ID_STAT_REQU_EXPE == 5 || x.ID_STAT_REQU_EXPE == 6)).ToList()
                         join b in dbc.REQUEST_EXPENSE_ACTIVITY.Where(x => x.ID_STAT_REQU_EXPE == 5) on
                                   a.ID_REQU_EXPE equals b.ID_REQU_EXPE
                         join c in qUser on a.ID_PERS_ENTI_REQU equals c.ID_PERS_ENTI
                         select new
                         {
                             a.COD_REQU_EXPE,
                             a.DAT_REGI,
                             b.DAT_STAR,
                             a.AMOUNT,
                             COIN = (a.CURRENCY == "MN" ? "S/." : "US$"),
                             a.ID_REQU_EXPE,
                             a.JUSTIFICATION,
                             USER = TextCapitalize(c.USER)
                         });

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public string TextCapitalize(string txt = "")
        {
            return System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(txt.ToLower());
        }

        //-----------------------------LISTA LOS COMBOS-------------------------------------------------------

        public ActionResult ListByType()
        {
            var query = (from tds in dbc.TYPE_DELI_SUST
                         select new
                         {
                             tds.ID_TYPE_DELI_SUST,
                             tds.NAM_TYPE_DELI_SUST,
                         });

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListByStatus()
        {
            var query = (from sre in dbc.STATUS_REQUEST_EXPENSE
                         select new
                         {
                             sre.ID_STAT_REQU_EXPE,
                             sre.NAM_STAT_REQU_EXPE,
                         });

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListByRequest()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            var qcuenta = (from cuenta in dbc.ACCOUNT_PARAMETER.Where(x => x.ID_ACCO == ID_ACCO && x.ID_PARA == 6)
                           select new
                           {
                               idcuenta = cuenta.VAL_ACCO_PARA
                           }).Single();

            int account = Convert.ToInt32(qcuenta.idcuenta);

            string FIR_NAM = "";

            string PARAM = Convert.ToString(Request.QueryString["filter[filters][0][field]"]);

            if (PARAM == "NOMBRE")
            {
                FIR_NAM = Convert.ToString(Request.QueryString["filter[filters][0][value]"]);
            }


            var result = (from pe in dbe.PERSON_ENTITY.Where(x => x.VIG_PERS_ENTI == true).ToList()//here
                          join ce in dbe.CLASS_ENTITY.Where(x => x.FIR_NAME != null && x.LAS_NAME != null) on pe.ID_ENTI2 equals ce.ID_ENTI
                          where pe.ID_ENTI1 == account
                          select new
                          {
                              ID_PERS_ENTI = pe.ID_PERS_ENTI,
                              //NOMBRE = ce.FIR_NAME + " " + ce.LAS_NAME,
                              NOMBRE = (ce.FIR_NAME.Trim() + (ce.SEC_NAME == null ? "" : " " + ce.SEC_NAME.Trim()) + (ce.LAS_NAME == null ? "" : " " + ce.LAS_NAME.Trim()) + (ce.MOT_NAME == null ? "" : " " + ce.MOT_NAME.Trim())).ToUpper(),
                          });

            FIR_NAM = FIR_NAM.ToUpper();
            string[] fir_nam_ = FIR_NAM.Split(' ');

            if (FIR_NAM.Length > 0)
            {
                //result = result.Where(x => x.NOMBRE.ToLower().Contains(FIR_NAM.ToLower()));
                result = result.Where(x => x.NOMBRE.Contains(FIR_NAM) || (x.NOMBRE.Contains(fir_nam_[0]) && x.NOMBRE.Contains(fir_nam_[1])));
            }

            result= result.OrderBy(x => x.NOMBRE).ToList();
            //var query = result.OrderBy(x => x.NOMBRE).ToList().Take(20);
            //return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }


        public decimal GetAmountAvailable(int id = 0)
        {
            try
            {
                var MPC = dbc.DELIVERY_SUSTAIN.Single(x => x.ID_DELI_SUST == id);

                var DocuExpe = (from de in dbc.DOCUMENT_EXPENSE.Where(x => x.ID_STAT_DOCU_EXPE == 3)
                                join tde in dbc.TYPE_DOCUMENT_EXPENSE.Where(x => x.AFF_AVAI != false) on de.ID_TYPE_DOCU_EXPE equals tde.ID_TYPE_DOCU_EXPE
                                select new
                                {
                                    de.ID_DOCU_EXPE,
                                    de.ID_REQU_EXPE,
                                    de.ID_TYPE_DOCU_EXPE,
                                    de.AMOUNT,
                                });

                var data = (from ds in dbc.DELIVERY_SUSTAIN.Where(x => x.ID_DELI_SUST == id)
                            join re in dbc.REQUEST_EXPENSE on ds.ID_DELI_SUST equals re.ID_DELI_SUST
                            join sre in dbc.STATUS_REQUEST_EXPENSE on re.ID_STAT_REQU_EXPE equals sre.ID_STAT_REQU_EXPE
                            join de in DocuExpe on re.ID_REQU_EXPE equals de.ID_REQU_EXPE into ld
                            from lde in ld.DefaultIfEmpty()
                            select new
                            {
                                re.ID_DELI_SUST,
                                MONTO = ds.DOC_AMOU,
                                MONTOAVAI = (lde.ID_DOCU_EXPE == null && sre.AFF_PETT_CASH_AVAI == true) ? re.AMOUNT :
                                          (lde.ID_DOCU_EXPE != null && sre.AFF_PETT_CASH_AVAI == true && lde.ID_TYPE_DOCU_EXPE != 5) ? lde.AMOUNT : 0
                            });

                if (data.Count() > 0) // Si Hay requerimientos
                {
                    var query = (from a in data
                                 group a by new { a.ID_DELI_SUST } into g
                                 select new
                                 {
                                     g.Key.ID_DELI_SUST,
                                     SUMA = g.Sum(x => x.MONTOAVAI)
                                 }).Single();

                    return (Convert.ToDecimal(query.SUMA));
                }
                else
                {
                    return 0; //return (Convert.ToDecimal(MPC.DOC_AMOU));
                }

            }
            catch (Exception)
            {
                return 0;
            }
        }

        public decimal GetAmountRemain(int id = 0)
        {
            try
            {
                var MPC = dbc.DELIVERY_SUSTAIN.Single(x => x.ID_DELI_SUST == id); //MPC:Monto Petty Cash

                var DocuExpe = (from de in dbc.DOCUMENT_EXPENSE.Where(x => x.ID_STAT_DOCU_EXPE == 3)
                                join tde in dbc.TYPE_DOCUMENT_EXPENSE.Where(x => x.AFF_AVAI != false && x.ID_TYPE_DOCU_EXPE != 5) on de.ID_TYPE_DOCU_EXPE equals tde.ID_TYPE_DOCU_EXPE
                                select new
                                {
                                    de.ID_DOCU_EXPE,
                                    de.ID_REQU_EXPE,
                                    de.ID_TYPE_DOCU_EXPE,
                                    de.AMOUNT,
                                });

                var data = (from ds in dbc.DELIVERY_SUSTAIN.Where(x => x.ID_DELI_SUST == id)
                            join re in dbc.REQUEST_EXPENSE on ds.ID_DELI_SUST equals re.ID_DELI_SUST
                            join sre in dbc.STATUS_REQUEST_EXPENSE on re.ID_STAT_REQU_EXPE equals sre.ID_STAT_REQU_EXPE
                            join de in DocuExpe on re.ID_REQU_EXPE equals de.ID_REQU_EXPE
                            select new
                            {
                                re.ID_DELI_SUST,
                                MONTO = ds.DOC_AMOU,
                                MONTOREMA = (sre.AFF_PETT_CASH_REMA == true) ? de.AMOUNT : 0

                            });
                if (data.Count() > 0)
                {
                    var query = (from a in data
                                 group a by new { a.ID_DELI_SUST } into g
                                 select new
                                 {
                                     g.Key.ID_DELI_SUST,
                                     SUMA = g.Sum(x => x.MONTOREMA)
                                 }).Single();
                    return (Convert.ToDecimal(query.SUMA));

                }
                else
                {
                    return 0; //return (Convert.ToDecimal(MPC.DOC_AMOU));
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public decimal GetTotalAvailable(int id = 0)
        {
            try
            {

                var query = (from ds in dbc.DELIVERY_SUSTAIN.Where(x => x.ID_DELI_SUST == id).ToList()
                             select new
                             {
                                 AVAI = ds.DOC_AMOU + ds.AMO_AVAI_PREV - GetAmountAvailable(id),
                             }).FirstOrDefault();

                return Convert.ToDecimal(query.AVAI);
            }
            catch
            {
                return 0;
            }
        }

        public decimal GetTotalRemaining(int id = 0)
        {
            try
            {
                var query = (from ds in dbc.DELIVERY_SUSTAIN.Where(x => x.ID_DELI_SUST == id).ToList()
                             select new
                             {
                                 REMA = ds.DOC_AMOU + ds.AMO_AVAI_PREV - GetAmountRemain(id),
                             }).FirstOrDefault();

                return Convert.ToDecimal(query.REMA);
            }
            catch
            {
                return 0;
            }
        }
        public ActionResult ExportRendiciones() //int filtertype
        {
            int idt = 0, ids = 0, idr = 0, ID_TYPE_DOCU_SALE = 0;
            String idtx = Request.Params["filtertype"].ToString();
            String idsx = Request.Params["filterstatus"].ToString();
            String idrx = Request.Params["filterrequet"].ToString();
            String ID_TYPE_DOCU_SALEx = Request.Params["ID_TYPE_DOCU_SALE"].ToString();


            if (idtx != "") { idt = Convert.ToInt32(idtx); }
            if (idsx != "") { ids = Convert.ToInt32(idsx); }
            if (idrx != "") { idr = Convert.ToInt32(idrx); }
            if (ID_TYPE_DOCU_SALEx != "") { ID_TYPE_DOCU_SALE = Convert.ToInt32(ID_TYPE_DOCU_SALEx); }

            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            var flag = true;
            string nroOP = null, nroOPs = Convert.ToString(Request.Params["nroOP"]);
            CultureInfo ci = new CultureInfo("Es-Es");
            if (!String.IsNullOrEmpty(nroOPs)) //Para filtros por OP
            {
                if (nroOPs.Trim() != "")
                {
                    nroOP = nroOPs;
                }
            }

            bool resp = false;
            int ID_PERS_ENTI_RESP = 0;

            var qPer = (from a in dbe.PERSON_ENTITY.Where(x => x.ID_ENTI1 == 9) //Listado de Personas
                        join b in dbe.CLASS_ENTITY on a.ID_ENTI2 equals b.ID_ENTI
                        select new
                        {
                            a.ID_PERS_ENTI,
                            User = b.FIR_NAME + " " + b.LAS_NAME,
                            Usua = b.FIR_NAME.Substring(0, 1) + ". " + b.LAS_NAME
                        });
            int qPerc = qPer.Count();
            var q = qPer.ToList();
            int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
            string ID_PERS_ENTIs = Convert.ToString(ID_PERS_ENTI);
            //var qPer = dbe.ListadoViaticos(37361, ID_ACCO).ToList();

            try
            {
                // Muestra a los responsables de Caja Chica: Melisa, Juan, Ronald, Luis
                ID_PERS_ENTI_RESP = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 37) //ID_PARA=37=>RESPONSABLE PETTY CASH
                    .Where(x => x.ID_ACCO == ID_ACCO)
                    .Where(x => x.VAL_ACCO_PARA == ID_PERS_ENTIs).Count();

                if (ID_PERS_ENTI_RESP > 0) //Si es que se ha logueado un reposnsable de Caja chica. 
                {
                    ID_PERS_ENTI_RESP = ID_PERS_ENTI;
                    resp = true;
                }
            }
            catch
            {

            }
            //Hasta aca se ha obtenido una el responsable de caja chica
            //Verifica si tiene permisos para aprobar requerimientos
            Boolean UserApp = false;
            var qApp = dbc.PETTY_CASH_ASSIGNED.Where(x => x.ID_PERS_ENTI_APPR == ID_PERS_ENTI && x.VIG_PETT_CASH_ASSI == true && x.ID_PERS_ENTI_APPR_VIAT == ID_PERS_ENTI);
            if (qApp.Count() > 0)
            {
                UserApp = qApp.First().APP_ANY_REQU.Value; // Aprueba cualquier  Requerimiento
            }

            //Consigue los roles del usuario            
            Boolean UserAccount = false;

            if ((int)Session["ADMINISTRADOR"] == 1 || (int)Session["FINANZAS"] == 1)
            {
                UserAccount = true;
            }

            var qRequest = dbe.ListadoPersonalViaticosOperaciones(Convert.ToInt32(UserAccount), Convert.ToInt32(UserApp), ID_PERS_ENTI, ID_ACCO).ToList();
            var qres = qRequest.ToList();
            var qRequestT = qRequest.Where(x => flag == true);
            //-----------------------------------------FILTROS--------------------------------------------------
            if (idt != 0)
            { qRequestT = qRequest.Where(x => x.ID_TYPE_DELI_SUST == idt); }
            if (ids != 0)
            { qRequestT = qRequest.Where(x => x.ID_STAT_REQU_EXPE == ids); }
            if (idr != 0)
            { qRequestT = qRequest.Where(x => x.ID_PERS_ENTI_REQU == idr); }
            if (!String.IsNullOrEmpty(nroOP))
            { qRequestT = qRequest.Where(x => x.NUM_DOCU_SALE.ToLower().Contains(nroOP.ToLower())); }
            if (ID_TYPE_DOCU_SALE != 0)
            { qRequestT = qRequest.Where(x => x.ID_TYPE_DOCU_SALE == ID_TYPE_DOCU_SALE); }
            //------------------------------------------------------------------------------------------------------

            int tt = qRequestT.Count();
            if (ID_PERS_ENTI != 1007)
            {
                var usuario = dbe.ValidaUsuario(ID_PERS_ENTI).Single();
                //if (usuario.ID_CHAR_JEF.Value == 1269 || usuario.ID_CHAR_PARE == 1269 || usuario.ID_CHAR_PERS == 1269)
                //{
                //    var query = dbe.ListadoPersonalViaticos(ID_PERS_ENTI, 4, 0, UserApp, UserAccount, resp, idt, ids, idr, ID_TYPE_DOCU_SALE, nroOP).ToList();
                //    if (query.Count() == 0)
                //        return Content("<script>alert('No se encontraron registros');</script>");
                //    else
                //        return Json(new { Data = query, Count = query.First().tt }, JsonRequestBehavior.AllowGet);
                //}
                //else
                //{

                    var query = (from a in qRequestT.OrderBy(x => x.ORD_STAT)
                                           .ThenByDescending(x => x.DAT_REGI)
                                 join b in dbc.TYPE_DELI_SUST on a.ID_TYPE_DELI_SUST equals b.ID_TYPE_DELI_SUST
                                 join d in qPer on a.ID_PERS_ENTI_REQU equals d.ID_PERS_ENTI
                                 join e in qPer on a.ID_PERS_ENTI_APPR equals e.ID_PERS_ENTI into le
                                 from xe in le.DefaultIfEmpty()
                                 join f in qPer on a.ID_PERS_ENTI_APPR_VIAT equals f.ID_PERS_ENTI into lf
                                 from xf in lf.DefaultIfEmpty()
                                 join o in dbc.DOCUMENT_SALE on a.ID_DOCU_SALE equals o.ID_DOCU_SALE into lo
                                 from xo in lo.DefaultIfEmpty()
                                 join to in dbc.TYPE_DOCUMENT_SALE on (xo == null ? 0 : xo.ID_TYPE_DOCU_SALE) equals to.ID_TYPE_DOCU_SALE into lto
                                 from xto in lto.DefaultIfEmpty()
                                 select new
                                 {
                                     //a.ID_REQU_EXPE,
                                     //a.ID_DELI_SUST,
                                     //b.ID_TYPE_DELI_SUST,
                                     Tipo = b.NAM_TYPE_DELI_SUST_SPAN,
                                     Codigo_Rendicion = a.COD_REQU_EXPE,
                                     // a.ID_PERS_ENTI_REQU,
                                     Solicitante = d.User,
                                     //a.ID_PERS_ENTI_APPR,
                                     Aprobador = (b.ID_TYPE_DELI_SUST == 1 ? (xe == null ? "" : xe.User) : //xe.User.ToLower()
                                                 (xe != null && xf != null ? (a.ID_PERS_ENTI_APPR == a.ID_PERS_ENTI_APPR_VIAT ? xe.User :
                                                                              xe.Usua + " / " + xf.Usua) :
                                                 (xe == null ? "" : xe.User))),
                                     //APROBADOR1 = xf == null ? "" : xf.Usua.ToLower(),
                                     //a.ID_STAT_REQU_EXPE,
                                     Estado = ProperCase(a.NAM_STAT_REQU_EXPE_SPAN),
                                     //Usuario = d.User.ToLower(),
                                     //Fecha_Inicio = a.DAT_REGI == null ? "" : String.Format("{0:G}", a.DAT_REGI),
                                     Fecha_Inicio = a.DAT_REGI == null ? "" : String.Format("{0:d}", a.DAT_REGI),

                                     //ID_PERS_ENTI_APPR_VIAT = a.ID_PERS_ENTI_APPR_VIAT == null ? 0 : a.ID_PERS_ENTI_APPR_VIAT,
                                     //a.ID_TICK,
                                     Codigo_Ticket = a.COD_TICK,
                                     //ID_OP = (xo == null ? 0 : xo.ID_DOCU_SALE),
                                     Nro_OP = (xto == null ? "" : xto.NAM_TYPE_DOCU_SALE.Trim() + " ") + (xo == null ? "" : xo.NUM_DOCU_SALE.Trim()),
                                     //CURRENCY = (a.CURRENCY == "MN" ? "S/." : "US$"),
                                     Monto = (a.CURRENCY == "MN" ? "S/." : "US$") + " " + (a.AMOUNT == null ? "-" : String.Format("{0:N2}", a.AMOUNT)),
                                     Moneda = (a.CURRENCY == "MN" ? ResourceLanguaje.Resource.PeruvianNuevoSol : ResourceLanguaje.Resource.DollarAmerican),
                                     //TOTAL = a.AMOUNT,
                                     Nro_Dias = a.NUM_DAYS == null ? 0 : a.NUM_DAYS,
                                     Fecha_Salida = a.DAT_DEPA == null ? "" : String.Format("{0:d}", a.DAT_DEPA),
                                     Destino = a.DESTINATION == null ? "" : a.DESTINATION,
                                     Justificacion = a.JUSTIFICATION,

                                 });

                    var grid = new GridView();
                    grid.DataSource = query.ToList();
                    grid.DataBind();

                    int countquery = query.ToList().Count();

                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename=RequestExpense" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
                    Response.ContentType = "application/ms-excel";

                    Response.Charset = "";
                    StringWriter sw = new StringWriter();
                    HtmlTextWriter htw = new HtmlTextWriter(sw);

                    grid.RenderControl(htw);

                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();

                    return View();
                //}

            }
            else
            {
                var query = (from a in qRequestT.ToList().OrderBy(x => x.ORD_STAT)
                                           .ThenByDescending(x => x.DAT_REGI)
                             join b in dbc.TYPE_DELI_SUST on a.ID_TYPE_DELI_SUST equals b.ID_TYPE_DELI_SUST
                             join d in qPer on a.ID_PERS_ENTI_REQU equals d.ID_PERS_ENTI
                             join e in qPer on a.ID_PERS_ENTI_APPR equals e.ID_PERS_ENTI into le
                             from xe in le.DefaultIfEmpty()
                             join f in qPer on a.ID_PERS_ENTI_APPR_VIAT equals f.ID_PERS_ENTI into lf
                             from xf in lf.DefaultIfEmpty()
                             join o in dbc.DOCUMENT_SALE on a.ID_DOCU_SALE equals o.ID_DOCU_SALE into lo
                             from xo in lo.DefaultIfEmpty()
                             join to in dbc.TYPE_DOCUMENT_SALE on (xo == null ? 0 : xo.ID_TYPE_DOCU_SALE) equals to.ID_TYPE_DOCU_SALE into lto
                             from xto in lto.DefaultIfEmpty()
                             select new
                             {
                                 //a.ID_REQU_EXPE,
                                 //a.ID_DELI_SUST,
                                 //b.ID_TYPE_DELI_SUST,
                                 Tipo = b.NAM_TYPE_DELI_SUST_SPAN,
                                 Codigo_Rendicion = a.COD_REQU_EXPE,
                                 // a.ID_PERS_ENTI_REQU,
                                 Solicitante = d.User,
                                 //a.ID_PERS_ENTI_APPR,
                                 Aprobador = (b.ID_TYPE_DELI_SUST == 1 ? (xe == null ? "" : xe.User) : //xe.User.ToLower()
                                                 (xe != null && xf != null ? (a.ID_PERS_ENTI_APPR == a.ID_PERS_ENTI_APPR_VIAT ? xe.User :
                                                                              xe.Usua + " / " + xf.Usua) :
                                                 (xe == null ? "" : xe.User))),
                                 //APROBADOR1 = xf == null ? "" : xf.Usua.ToLower(),
                                 //a.ID_STAT_REQU_EXPE,
                                 Estado = ProperCase(a.NAM_STAT_REQU_EXPE_SPAN),
                                 //Usuario = d.User.ToLower(),
                                 //Fecha_Inicio = a.DAT_REGI == null ? "" : String.Format("{0:G}", a.DAT_REGI),
                                 Fecha_Inicio = a.DAT_REGI == null ? "" : String.Format("{0:d}", a.DAT_REGI),

                                 //ID_PERS_ENTI_APPR_VIAT = a.ID_PERS_ENTI_APPR_VIAT == null ? 0 : a.ID_PERS_ENTI_APPR_VIAT,
                                 //a.ID_TICK,
                                 Codigo_Ticket = a.COD_TICK,
                                 //ID_OP = (xo == null ? 0 : xo.ID_DOCU_SALE),
                                 Nro_OP = (xto == null ? "" : xto.NAM_TYPE_DOCU_SALE.Trim() + " ") + (xo == null ? "" : xo.NUM_DOCU_SALE.Trim()),
                                 //CURRENCY = (a.CURRENCY == "MN" ? "S/." : "US$"),
                                 Monto = (a.CURRENCY == "MN" ? "S/." : "US$") + " " + (a.AMOUNT == null ? "-" : String.Format("{0:N2}", a.AMOUNT)),
                                 Moneda = (a.CURRENCY == "MN" ? ResourceLanguaje.Resource.PeruvianNuevoSol : ResourceLanguaje.Resource.DollarAmerican),
                                 //TOTAL = a.AMOUNT,
                                 Nro_Dias = a.NUM_DAYS == null ? 0 : a.NUM_DAYS,
                                 Fecha_Salida = a.DAT_DEPA == null ? "" : String.Format("{0:d}", a.DAT_DEPA),
                                 Destino = a.DESTINATION == null ? "" : a.DESTINATION,
                                 Justificacion = a.JUSTIFICATION,

                             });
                var p = query.ToList();

                var grid = new GridView();
                grid.DataSource = query.ToList();
                grid.DataBind();

                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=RequestExpense" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
                Response.ContentType = "application/ms-excel";

                Response.Charset = "";
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);

                grid.RenderControl(htw);

                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();

                return View();

            }

        }

        public string EditarMonto(int id,int IdEstado, decimal MontoActualizar)
        {
            REQUEST_EXPENSE re = dbc.REQUEST_EXPENSE.Where(x => x.ID_REQU_EXPE == id).FirstOrDefault();


            var amount = (from de in dbc.MontoRecepcion(id) select new { de.AMOUNT });

            decimal TotalRecepcion = amount.Sum(x => Convert.ToDecimal(x.AMOUNT));

            if(IdEstado == 1 || IdEstado == 6)
            {
                re.AMOUNT = MontoActualizar;
            }
            else
            {
                if (decimal.Round(TotalRecepcion, 2) == MontoActualizar)
                {
                    re.AMOUNT = MontoActualizar;
                }
                else
                {
                    return "ERROR";
                }
            }

            dbc.SaveChanges();

            return "OK";

        }
        public string EditarMontoModal(int id, decimal MontoActualizar,decimal MontoRendido)
        {


            REQUEST_EXPENSE re = dbc.REQUEST_EXPENSE.Where(x => x.ID_REQU_EXPE == id).FirstOrDefault();

            if(decimal.Round(MontoRendido,2) == MontoActualizar)
            {
                re.AMOUNT = MontoActualizar;
            }
            else
            {
                return "ERROR";
            }
            

            dbc.SaveChanges();

            return "OK";

        }
    }
}
