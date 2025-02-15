using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.Globalization;
using System.Threading;
using WebMatrix.WebData;
using System.Web.Security;
using ERPElectrodata.Object.Plugins;

namespace ERPElectrodata.Controllers
{
    public class AccountEntityController : Controller
    {
        private EntityEntities dbe = new EntityEntities();
        public CoreEntities dbc = new CoreEntities();
        private CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        private TextInfo textInfo;

        public ActionResult ListEmployeeByComp()
        {
            int ID_COMP = 1;//

            var query = dbe.PERSON_ENTITY.Where(x => x.ID_ENTI1 == ID_COMP)
                .Join(dbe.CLASS_ENTITY, pe => pe.ID_ENTI2, ce => ce.ID_ENTI, (pe, ce) => new
                {
                    pe.ID_PERS_ENTI,
                    ce.FIR_NAME,
                    ce.SEC_NAME,
                    ce.LAS_NAME,
                    ce.MOT_NAME,
                    pe.EMA_PERS
                });

            var result = (from x in query.ToList()
                          select new
                          {
                              x.FIR_NAME,
                              x.SEC_NAME,
                              x.LAS_NAME,
                              x.MOT_NAME,
                              x.EMA_PERS
                          });

            return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListarPersonaResponsable()
        {
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            var result = dbe.ListarPersonaResponsable(IdAcco, 1007).ToList();
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListCompanyByAccount()
        {
            //Agregar filtrado de servidor
            string parm = Convert.ToString(Request.QueryString["filter[filters][0][field]"]);
            
            string valor = "";
            if (parm == "COM_NAME")
            {
                valor = Convert.ToString(Request.QueryString["filter[filters][0][value]"]);
            }
            var ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());

            var query = dbe.CLASS_ENTITY
                .Where(x => x.ID_TYPE_ENTI == 1 && x.VIG_ENTI == true);

            if (ID_ACCO == 60)//Buenaventura
            {
                //var filtroCompania = (from x in dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 1084)
                //                      select new
                //                      {
                //                          ID_ENTI = Convert.ToInt32(x.VAL_ACCO_PARA)
                //                      });

                query = query.Where(x => (x.ID_ENTI > 78250 && x.ID_ENTI < 78264) || x.ID_ENTI == 78440 || x.ID_ENTI == 78441);
            }

            var result = (from x in query.ToList()
                          join td in dbe.TYPE_DOCUMENTIDENT on x.ID_TYPE_DI equals td.ID_TYPE_DI
                          select new
                          {
                              ID_ENTI = x.ID_ENTI,
                              COM_NAME = (x.COM_NAME == null ? "" : x.COM_NAME.ToUpper()),
                              NAM_TYPE_DI = td.NAM_TYPE_DI,
                              NUM_TYPE_DI = (x.NUM_TYPE_DI == null ? "" : x.NUM_TYPE_DI),
                          });

            //Para Outsourcing
            if (ID_ACCO != 4 && ID_ACCO != 17 && ID_ACCO != 25 && ID_ACCO != 24 && ID_ACCO != 22 && ID_ACCO != 26 && ID_ACCO != 55 && ID_ACCO != 60)
            {
                var Compania = Convert.ToInt32(dbc.ACCOUNT_PARAMETER.Where(x => x.ID_ACCO == ID_ACCO && x.ID_PARA == 6).First().VAL_ACCO_PARA);

                result = result.Where(x => x.ID_ENTI == Compania);
                result = result.Where(x => x.ID_ENTI == Compania && x.COM_NAME.Contains(valor));
            }

            return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarCompaniaxCuentaTicket(int ID_ACCO)
        {
            var query = dbe.CLASS_ENTITY.Where(x => x.ID_TYPE_ENTI == 1);

            var result = (from x in query.ToList()
                          select new
                          {
                              ID_ENTI = x.ID_ENTI,
                              COM_NAME = (x.COM_NAME == null ? "" : x.COM_NAME.ToUpper()),
                          });

            //Para Outsourcing
            if (ID_ACCO != 4 && ID_ACCO != 17 && ID_ACCO != 25 && ID_ACCO != 24 && ID_ACCO != 22 && ID_ACCO != 26)
            {
                //var Compania = Convert.ToInt32(dbc.ACCOUNT_PARAMETER.Where(x => x.ID_ACCO == ID_ACCO && x.ID_PARA == 6).First().VAL_ACCO_PARA);
                //result = result.Where(x => x.ID_ENTI == Compania);
                if (ID_ACCO == 57)
                {
                    var Compania = Convert.ToInt32(dbc.ACCOUNT_PARAMETER.Where(x => x.ID_ACCO == ID_ACCO && x.ID_PARA == 6).First().VAL_ACCO_PARA);
                    var Compania2 = Convert.ToInt32(dbc.ACCOUNT_PARAMETER.Where(x => x.ID_ACCO == 56 && x.ID_PARA == 6).First().VAL_ACCO_PARA);


                    result = result.Where(x => x.ID_ENTI == Compania || x.ID_ENTI == Compania2).ToList();

                }
                else
                {
                    var Compania = Convert.ToInt32(dbc.ACCOUNT_PARAMETER.Where(x => x.ID_ACCO == ID_ACCO && x.ID_PARA == 6).First().VAL_ACCO_PARA);
                    result = result.Where(x => x.ID_ENTI == Compania);
                }
            }
            return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult ListarCompañiasxCuenta(string q, string page)
        //{
        //    string termino = "";
        //    if (Request.QueryString["q"] == null)
        //    {
        //        termino = "";
        //    }
        //    else
        //    {
        //        termino = Request.QueryString["q"].ToString();
        //    }

        //    var ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());

        //    //var query = dbe.CLASS_ENTITY.Where(x => x.ID_TYPE_ENTI == 1);
        //    //from x in query.ToList()
        //    var result = (from x in dbe.CLASS_ENTITY.Where(x => x.ID_TYPE_ENTI == 1).ToList()
        //                  join td in dbe.TYPE_DOCUMENTIDENT on x.ID_TYPE_DI equals td.ID_TYPE_DI
        //                  select new
        //                  {
        //                      id = x.ID_ENTI,
        //                      text = (x.COM_NAME == null ? "" : x.COM_NAME.ToUpper()),
        //                  }).Where(x => x.text.Contains(termino.ToUpper()));

        //    //Para Outsourcing
        //    if (ID_ACCO != 4 && ID_ACCO != 17 && ID_ACCO != 25 && ID_ACCO != 24 && ID_ACCO != 22)
        //    {
        //        var Compania = Convert.ToInt32(dbc.ACCOUNT_PARAMETER.Where(x => x.ID_ACCO == ID_ACCO && x.ID_PARA == 6).First().VAL_ACCO_PARA);

        //        result = result.Where(x => x.id == Compania);
        //    }

        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult CompaniaListarPorCuenta(string q, string page)
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

            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);

            List<CompaniaListarPorCuenta_Result> resultado = dbe.CompaniaListarPorCuenta(IdCuenta, termino).ToList();

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /AccountEntity/RequesterByAcco
        public ActionResult DetailPerson(int id = 0)
        {
            var IdCuenta = Convert.ToInt32(Session["ID_ACCO"].ToString());
            var result = dbe.DetailPerson(IdCuenta, id).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DetailPersonEdata(int id = 0)
        {
            var IdCuenta = Convert.ToInt32(Session["ID_ACCO"].ToString());
            var result = dbe.DatosDelUsuarioEdata(IdCuenta, id).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DetailPersonTicket(int id = 0)
        {
            var IdCuenta = Convert.ToInt32(Session["ID_ACCO"].ToString());
            var result = dbe.DetailPersonTicket(IdCuenta, id).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerCargoxUsuario(int id = 0)
        {
            var IdCuenta = Convert.ToInt32(Session["ID_ACCO"].ToString());
            var query = dbe.ObtenerCargoxUsuario(IdCuenta, id).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerLocacionxUsuario(int id = 0)
        {
            var IdCuenta = Convert.ToInt32(Session["ID_ACCO"].ToString());
            var query = dbe.ObtieneLocacionUsuario(id).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListUser()
        {
            var ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());

            var query1 = dbe.CLASS_ENTITY.Where(x => x.UserId != null);

            var result = (from t in query1.ToList()
                          join p in dbe.PERSON_ENTITY on t.ID_ENTI equals p.ID_ENTI2
                          join ae in dbe.ACCOUNT_ENTITY on p.ID_PERS_ENTI equals ae.ID_PERS_ENTI
                          where ae.ID_ACCO == ID_ACCO
                          select new
                          {
                              t.UserId,
                              User = t.FIR_NAME + " " + t.SEC_NAME + " " + t.LAS_NAME + " " + t.MOT_NAME,
                          }).OrderBy(x => x.User).ToList();

            return Json(new { Data = result, Count = query1.Count() }, JsonRequestBehavior.AllowGet);
        }
        //
        // GET: /AccountEntity/RequesterByAcco
        public ActionResult RequesterByAcco()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            var query = dbe.ACCOUNT_ENTITY.Where(ae => ae.ID_ACCO == ID_ACCO && ae.VIS_REQU == true && ae.VIG_ACCO_ENTI == true).ToList();

            var result = (from x in query
                          join pe in dbe.PERSON_ENTITY on x.ID_PERS_ENTI equals pe.ID_PERS_ENTI
                          join p in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals p.ID_ENTI
                          select new
                          {
                              //CLIE = p.FIR_NAME + (p.LAS_NAME == null ? "" : " " + p.LAS_NAME) + (p.MOT_NAME == null ? "" : " " + p.MOT_NAME),
                              CLIE = p.FIR_NAME + " " + p.SEC_NAME + " " + p.LAS_NAME + " " + p.MOT_NAME,
                              pe.ID_PERS_ENTI,
                              pe.ID_PRIO,
                              pe.ID_AREA,
                          }).Where(x => x.CLIE != " ").ToList().OrderBy(x => x.CLIE);

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PersonaPorCompania()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            int Compania = Convert.ToInt32(dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 6 && x.ID_ACCO == ID_ACCO).Single().VAL_ACCO_PARA);

            var queryPE = dbe.PERSON_ENTITY.Where(x => x.ID_ENTI1 == Compania).ToList();
            //var queryAE = dbe.ACCOUNT_ENTITY.Where(ae => ae.ID_ACCO == ID_ACCO && ae.VIS_REQU == true && ae.VIG_ACCO_ENTI == true).ToList();

            var result = (from pe in queryPE
                          join p in dbe.CLASS_ENTITY.Where(p => p.VIG_ENTI == true) on pe.ID_ENTI2 equals p.ID_ENTI
                          join ae in dbe.ACCOUNT_ENTITY.Where(ae => ae.ID_ACCO == ID_ACCO && ae.VIS_REQU == true && ae.VIG_ACCO_ENTI == true).ToList()
                            on pe.ID_PERS_ENTI equals ae.ID_PERS_ENTI
                          select new
                          {
                              CLIE = p.FIR_NAME + (p.SEC_NAME == null ? "" : " " + p.SEC_NAME) + (p.LAS_NAME == null ? "" : " " + p.LAS_NAME) + (p.MOT_NAME == null ? "" : " " + p.MOT_NAME),
                              pe.ID_PERS_ENTI,
                              pe.ID_PRIO,
                              pe.ID_AREA
                          }).ToList().OrderBy(x => x.CLIE);

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RequesterByCIA()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int ID_CIA = 0;
            string CTE = "";

            string NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][0][field]"]);
            if (NAM_PAR == "ID_ENTI")
            {
                ID_CIA = Convert.ToInt32(Request.QueryString["filter[filters][0][value]"]);
            }
            else if (NAM_PAR == "CLIE")
            {
                CTE = Convert.ToString(Request.QueryString["filter[filters][0][value]"]);
            }

            NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][1][field]"]);
            if (NAM_PAR == "ID_ENTI")
            {
                ID_CIA = Convert.ToInt32(Request.QueryString["filter[filters][1][value]"]);
            }
            else if (NAM_PAR == "CLIE")
            {
                CTE = Convert.ToString(Request.QueryString["filter[filters][1][value]"]);
            }

            var query = dbe.ACCOUNT_ENTITY.Where(ae => ae.VIS_REQU == true && ae.VIG_ACCO_ENTI == true && ae.ID_ACCO == ID_ACCO).ToList();

            var result = (from x in query
                          join pe in dbe.PERSON_ENTITY.Where(x => x.ID_ENTI1 == ID_CIA) on x.ID_PERS_ENTI equals pe.ID_PERS_ENTI
                          join p in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals p.ID_ENTI
                          select new
                          {
                              //CLIE = p.FIR_NAME + (p.LAS_NAME == null ? "" : " " + p.LAS_NAME) + (p.MOT_NAME == null ? "" : " " + p.MOT_NAME),
                              CLIE = (p.FIR_NAME == null ? "" : " " + p.FIR_NAME) + (p.SEC_NAME == null ? "" : " " + p.SEC_NAME) + (p.LAS_NAME == null ? "" : " " + p.LAS_NAME)  + (p.MOT_NAME == null ? "" : " " + p.MOT_NAME),
                              pe.ID_PERS_ENTI,
                              pe.ID_PRIO,
                              pe.ID_AREA,
                          }).Where(x => x.CLIE != " ");

            CTE = CTE.ToUpper();
            string[] cte_ = CTE.Split(' ');

            if (CTE.Length > 0)
            {
                //result = result.Where(x => x.CLIE.ToLower().Contains(CTE.ToLower()));
                result = result.Where(x => x.CLIE.Contains(CTE) || (x.CLIE.Contains(cte_[0]) && x.CLIE.Contains(cte_[1])));
            }
            result = result.Select(r =>
            {
                return new { CLIE = r.CLIE.ToUpper(), ID_PERS_ENTI = r.ID_PERS_ENTI, ID_PRIO = r.ID_PRIO, ID_AREA = r.ID_AREA };
            });
            result = result.OrderBy(x => x.CLIE).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RequesterByCIA2()
        {
            //int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int ID_CIA = 0;
            string CTE = "";

            string NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][0][field]"]);
            if (NAM_PAR == "ID_ENTI")
            {
                ID_CIA = Convert.ToInt32(Request.QueryString["filter[filters][0][value]"]);
            }
            else if (NAM_PAR == "CLIE")
            {
                CTE = Convert.ToString(Request.QueryString["filter[filters][0][value]"]);
            }

            NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][1][field]"]);
            if (NAM_PAR == "ID_ENTI")
            {
                ID_CIA = Convert.ToInt32(Request.QueryString["filter[filters][1][value]"]);
            }
            else if (NAM_PAR == "CLIE")
            {
                CTE = Convert.ToString(Request.QueryString["filter[filters][1][value]"]);
            }

            //var query = dbe.ACCOUNT_ENTITY.Where(ae => ae.VIS_REQU == true && ae.VIG_ACCO_ENTI == true).ToList();

            var result = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_ENTI1 == ID_CIA && x.VIG_PERS_ENTI == true).ToList()
                          join p in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals p.ID_ENTI
                          select new
                          {
                              CLIE = (p.FIR_NAME.Trim() + (p.SEC_NAME == null ? "" : " " + p.SEC_NAME.Trim()) + (p.LAS_NAME == null ? "" : " " + p.LAS_NAME.Trim()) + (p.MOT_NAME == null ? "" : " " + p.MOT_NAME.Trim())).ToUpper(),
                              pe.ID_PERS_ENTI,
                              pe.ID_PRIO,
                              ID_AREA = (pe.ID_AREA == null ? 0 : pe.ID_AREA),
                          });

            CTE= CTE.ToUpper();
            string[] cte_ = CTE.Split(' ');

            if (CTE.Length > 0)
            {
                //result = result.Where(x => x.CLIE.ToLower().Contains(CTE.ToLower()));
                result = result.Where(x => x.CLIE.Contains(CTE) || (x.CLIE.Contains(cte_[0]) && x.CLIE.Contains(cte_[1])));
            }

            result = result.OrderBy(x => x.CLIE).ToList().Take(20);

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RequesterByCIA2_Edit()
        {
            //int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int ID_CIA = 0;
            string CTE = "";

            string NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][0][field]"]);
            if (NAM_PAR == "ID_ENTI")
            {
                ID_CIA = Convert.ToInt32(Request.QueryString["filter[filters][0][value]"]);
            }
            else if (NAM_PAR == "CLIE")
            {
                CTE = Convert.ToString(Request.QueryString["filter[filters][0][value]"]);
            }

            NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][1][field]"]);
            if (NAM_PAR == "ID_ENTI")
            {
                ID_CIA = Convert.ToInt32(Request.QueryString["filter[filters][1][value]"]);
            }
            else if (NAM_PAR == "CLIE")
            {
                CTE = Convert.ToString(Request.QueryString["filter[filters][1][value]"]);
            }

            //var query = dbe.ACCOUNT_ENTITY.Where(ae => ae.VIS_REQU == true && ae.VIG_ACCO_ENTI == true).ToList();

            var result = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_ENTI1 == ID_CIA && x.VIG_PERS_ENTI == true).ToList()
                          join p in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals p.ID_ENTI
                          select new
                          {
                              CLIE = (p.FIR_NAME + (p.SEC_NAME == null ? "" : " " + p.SEC_NAME) + (p.LAS_NAME == null ? "" : " " + p.LAS_NAME) + (p.MOT_NAME == null ? "" : " " + p.MOT_NAME)).ToUpper(),
                              pe.ID_PERS_ENTI,
                              pe.ID_PRIO,
                              ID_AREA = (pe.ID_AREA == null ? 0 : pe.ID_AREA),
                          });

            if (CTE.Length > 0)
            {
                result = result.Where(x => x.CLIE.ToLower().Contains(CTE.ToLower()));
            }

            result = result.OrderBy(x => x.CLIE).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CargarListaAsignadosBnv()
        {           
            var listaAsignados = dbe.ListarSolicitanteCreacionTicketBuenaventura().Select(p=> {
                return new ListarSolicitanteCreacionTicketBuenaventura_Result { ID_PERS_ENTI = p.ID_PERS_ENTI, CLIE = p.CLIE.ToUpper(), ID_ENTI1=p.ID_ENTI1, ID_AREA=p.ID_AREA, ID_PRIO=p.ID_PRIO };
            }).OrderBy(x => x.CLIE).ToList();
            return Json(new { Data = listaAsignados, Count = listaAsignados.Count() }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CargarListaAsignadosBnvDePortales()  
        {
            var listaAsignados = dbe.ListarSolicitantePreticketBuenaventura().Select(p => {
                return new ListarSolicitantePreticketBuenaventura_Result { ID_PERS_ENTI = p.ID_PERS_ENTI, CLIE = p.CLIE.ToUpper(), ID_ENTI1 = p.ID_ENTI1, ID_AREA = p.ID_AREA, ID_PRIO = p.ID_PRIO };
            }).ToList();
            return Json(new { Data = listaAsignados, Count = listaAsignados.Count() }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CargarListaAfectadosBnv()
        {
            var listaAsignados = dbe.ListarUsuarioAfectadoBuenaventura("").ToList();
            return Json(new { Data = listaAsignados, Count = listaAsignados.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CargarListaUnidadesMineras(int id)
        {
            var listaUnidadesMineras = dbe.ObtenerUnidadMineraBuenaventura(id).ToList();
            return Json(new { Data = listaUnidadesMineras, Count = listaUnidadesMineras.Count() }, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /AccountEntity/
        public ActionResult AsignadoPorCola(string ID_QUEU, string VALOR)
        {
            int id_queue =Convert.ToInt32(ID_QUEU);
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            var text = VALOR;
              
            if (ID_ACCO == 56 || ID_ACCO == 57 || ID_ACCO == 58)
            {
                // Parametros termino, ID_ACCO, ID_QUEUE
                var query = dbe.AsignadosPorColaMinsur(text, ID_ACCO, id_queue).ToList();
                return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                // Parametros termino, ID_ACCO, ID_QUEUE
                var query = dbe.AsignadosPorCola(text, ID_ACCO, id_queue).ToList();
                return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
            }    

            //var query = dbe.AsignadosPorCola(text, ID_ACCO, id_queue).ToList();
            ////return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
            //return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AssigneByQueue(string q, string page)
        {
            string termino = "";
            int ID_QUEU = 0;
            
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            string NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][0][field]"]), i1;

            if (NAM_PAR == "ID_QUEU")
            {
                i1 = "0";
                ID_QUEU = Convert.ToInt32(Request.QueryString[("filter[filters][" + i1 + "][value]")]);

                    termino = "";

            }
            else if(NAM_PAR == "ASSI")
            {
                i1 = "1";
                ID_QUEU = Convert.ToInt32(Request.Params["filter[filters][1][value]"]);
                if (Request.QueryString[("filter[filters][0][value]")] == null)
                {
                    termino = "";
                }
                else
                {
                    termino = Request.QueryString[("filter[filters][0][value]")].ToString();
                }
            }
            else
            {
                ID_QUEU = Convert.ToInt32(Request.Params["AreaResp"]);
                //if (Request.QueryString[("filter[filters][0][AreaResp]")] == null)
                //{
                    termino = "";
                //}
                //else
                //{
                //    termino = Request.QueryString[("filter[filters][0][value]")].ToString();
                //}
            }


            if (ID_ACCO == 56 || ID_ACCO == 57 || ID_ACCO == 58)
            {
                // Parametros termino, ID_ACCO, ID_QUEUE
                var query = dbe.AsignadosPorColaMinsur(termino, ID_ACCO, ID_QUEU).ToList();
                return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                // Parametros termino, ID_ACCO, ID_QUEUE
                var query = dbe.AsignadosPorCola(termino, ID_ACCO, ID_QUEU).ToList();
                return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
            }





            #region
            //Crear lista contenedora
            //List<AsignadosPorCola_Result> listaContenedora = new List<AsignadosPorCola_Result>();
            //List<AsignadosPorCola_Result> queryFiltrado = new List<AsignadosPorCola_Result>();
            ////Agregar genericos y dar formato
            //var listaCoordinadores = dbe.ObtenerCoordinadoresPorGrupo(ID_QUEU, ID_ACCO).ToList();
            //listaCoordinadores.ForEach(coordinador =>
            //{
            //    var listaUsuarios = dbe.ListaUsuariosCoordinadores(coordinador.ID_PERS_ENTI,1, ID_ACCO).ToList();
            //    var crearNombre = coordinador.FIR_NAME + " " + coordinador.LAS_NAME;
            //    listaContenedora.Add(new AsignadosPorCola_Result {ID_PERS_ENTI=coordinador.ID_PERS_ENTI,ASSI= crearNombre, WorkLoad=coordinador.WorkLoad});
            //    listaUsuarios.ForEach(user =>
            //    {
            //        var crearNombreUser = user.FIR_NAME + " " + user.LAS_NAME;
            //        listaContenedora.Add(new AsignadosPorCola_Result { ID_PERS_ENTI = user.ID_PERS_ENTI, ASSI = crearNombreUser, WorkLoad = user.WorkLoad });
            //    });
            //});
            //query.ForEach(qe =>
            //{
            //    if (listaContenedora.Exists(t=>t.ID_PERS_ENTI == qe.ID_PERS_ENTI))
            //    {

            //    }
            //    else
            //    {
            //        queryFiltrado.Add(qe);
            //    }

            //});
            //queryFiltrado.ForEach(qf =>
            //{
            //    listaContenedora.Add(qf);
            //});
            //listaUsuariosGenericos.ForEach(x =>
            //{
            //    AsignadosPorCola_Result result = new AsignadosPorCola_Result();
            //    result.ID_PERS_ENTI = x.ID_PERS_ENTI;
            //    result.ASSI = x.CAR_PERS;
            //    result.WorkLoad = 0;
            //    query.Add(result);
            //});

            //int ID_QUEU = Convert.ToInt32(Request.QueryString[("filter[filters][" + i1 + "][value]")]);

            //string[] termino_ = termino.Split(' ');

            //var query = dbe.PERSON_ENTITY.Where(pe => pe.ID_QUEU == ID_QUEU && pe.VIG_PERS_ENTI == true)
            //    .Join(dbe.ACCOUNT_ENTITY, x => x.ID_PERS_ENTI, ae => ae.ID_PERS_ENTI, (x, ae) => new { x.ID_PERS_ENTI, x.ID_ENTI2, ae.ID_ACCO, ae.VIG_ACCO_ENTI, ae.VIS_ASSI })
            //    .Where(x => x.ID_ACCO == ID_ACCO && x.VIG_ACCO_ENTI == true && x.VIS_ASSI == true);

            //var result = (from x in query.ToList()
            //              join ce in dbe.CLASS_ENTITY.Where(x => x.VIG_ENTI == true) on x.ID_ENTI2 equals ce.ID_ENTI
            //              //join pc in dbe.PERSON_CONTRACT.Where(x => x.VIG_CONT == true) on x.ID_PERS_ENTI equals pc.ID_PERS_ENTI
            //              select new
            //              {
            //                  //ASSI = ce.FIR_NAME + ' ' + ce.LAS_NAME,
            //                  ASSI = (ce.FIR_NAME.Trim() + (ce.SEC_NAME == null ? "" : " " + ce.SEC_NAME.Trim()) + (ce.LAS_NAME == null ? "" : " " + ce.LAS_NAME.Trim()) + (ce.MOT_NAME == null ? "" : " " + ce.MOT_NAME.Trim())).ToUpper(),
            //                  x.ID_PERS_ENTI,
            //                  WorkLoad = TicketsByAssi(x.ID_PERS_ENTI)
            //              }).Where (x => x.ASSI.Contains(termino)).OrderBy(x => x.ASSI);//where no funciona ya que se envia en blanco/falta implementar filtro

            /*if (termino.Length > 0) { 
                result = result.Where(x => x.ASSI.Contains(termino) || (x.ASSI.Contains(termino_[0]) && x.ASSI.Contains(termino_[1]))).OrderBy(x => x.ASSI);
            }*/
            //var c = result.Count();
            //var ca = result.ToList();
            #endregion

            
        }

        //public ActionResult AssigneByQueueBnv(string q, string page, string idQUEUBnv = "", string idCompBnv = "")
        //{
        //    string termino = "";
        //    int ID_QUEU = 0;
        //    int ID_QUEUBNV = 0;
        //    int ID_COMPBNV = 0;
        //    if(idQUEUBnv != "")
        //    {
        //        ID_QUEUBNV = Convert.ToInt32(idQUEUBnv);
        //    }
        //    if (idCompBnv != "")
        //    {
        //        ID_COMPBNV = Convert.ToInt32(idCompBnv);
        //    }
        //    if (ID_QUEUBNV != 0 && ID_COMPBNV != 0)
        //    {
        //        if(ID_QUEUBNV == 25 && ID_QUEUBNV == 90 && ID_QUEUBNV == 89)
        //        {
        //            var query = dbe.ListaAsignadosPorUmYArea(ID_COMPBNV, ID_QUEUBNV).ToList();
        //            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //        {
        //            if (Request.QueryString["q"] == null)
        //            {
        //                termino = "";
        //            }
        //            else
        //            {
        //                termino = Request.QueryString["q"].ToString();
        //            }
        //            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
        //            string NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][0][field]"]), i1;

        //            if (NAM_PAR == "ID_QUEU")
        //            {
        //                i1 = "0";
        //                ID_QUEU = Convert.ToInt32(Request.QueryString[("filter[filters][" + i1 + "][value]")]);
        //            }
        //            else
        //            {
        //                i1 = "1";
        //                ID_QUEU = Convert.ToInt32(Request.Params["AreaResp"]);
        //            }

        //            var query = dbe.AsignadosPorCola(termino, ID_ACCO, ID_QUEU).ToList();

        //            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        //        }
                
        //    }
        //    else
        //    {
        //        if (Request.QueryString["q"] == null)
        //        {
        //            termino = "";
        //        }
        //        else
        //        {
        //            termino = Request.QueryString["q"].ToString();
        //        }
        //        int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
        //        string NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][0][field]"]), i1;

        //        if (NAM_PAR == "ID_QUEU")
        //        {
        //            i1 = "0";
        //            ID_QUEU = Convert.ToInt32(Request.QueryString[("filter[filters][" + i1 + "][value]")]);
        //        }
        //        else
        //        {
        //            i1 = "1";
        //            ID_QUEU = Convert.ToInt32(Request.Params["AreaResp"]);
        //        }

        //        var query = dbe.AsignadosPorCola(termino, ID_ACCO, ID_QUEU).ToList();

        //        return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        public ActionResult ListarPersonaxAreaResponsable(int ID_ACCO)
        {
            int ID_QUEU = 0;
            string NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][0][field]"]), i1;

            if (NAM_PAR == "ID_QUEU")
            {
                i1 = "0";
                ID_QUEU = Convert.ToInt32(Request.QueryString[("filter[filters][" + i1 + "][value]")]);
            }
            else
            {
                i1 = "1";
                ID_QUEU = Convert.ToInt32(Request.QueryString[("filter[filters][" + i1 + "][value]")]);
            }

            var result = dbe.PersonaPorAreaResponsable(ID_ACCO, ID_QUEU, "").ToList();
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarProjectManager(string q, string page)
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

            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            var result = (dynamic)null;

            if ((int)Session["ADMINISTRADOR"] == 1)
            {
                result = dbe.OPListarProjectManager(ID_ACCO, termino);
            }
            else if ((int)Session["PROJECTMANAGER"] == 1)
            {
                result = dbc.ListarPmoAsignados(1, ID_ACCO, termino);
            }
            else if ((int)Session["PROJECTMANAGER_OUTSOURCING"] == 1)
            {
                result = dbc.ListarPmoAsignados(2, ID_ACCO, termino);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult PersonaPorAreaResponsable(string q, string page)
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

            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            int IdAreaResponsable = Convert.ToInt32(Request.QueryString[("IdArea")]);

            List<PersonaPorAreaResponsable_Result> resultado = dbe.PersonaPorAreaResponsable(IdCuenta, IdAreaResponsable, termino).ToList();

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EspecialistaPorAreaResponsable(int id)
        {

            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);

            List<PersonaPorAreaResponsable_Result> resultado = dbe.PersonaPorAreaResponsable(IdCuenta, id, "").ToList();

            return Json(new { Data = resultado, Count = resultado.Count() }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CargarPreventaOutsourcing(string q, string page)
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

            int IdCuenta = Convert.ToInt32(Session["ID_ACCO"]);
            string IdAreaResponsable = Request.QueryString[("IdArea")];

            List<CargarPreventaOutsourcing_Result> resultado = dbe.CargarPreventaOutsourcing(IdCuenta, IdAreaResponsable, termino).ToList();

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AsignarOperaciones()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            var query = dbe.PERSON_ENTITY.Where(pe => pe.ID_QUEU == 3 || pe.ID_QUEU == 4 || pe.ID_QUEU == 14 || pe.ID_QUEU == 9 || pe.ID_QUEU == 25 || pe.ID_QUEU == 22)
                .Join(dbe.ACCOUNT_ENTITY, x => x.ID_PERS_ENTI, ae => ae.ID_PERS_ENTI, (x, ae) => new { x.ID_PERS_ENTI, x.ID_ENTI2, ae.ID_ACCO, ae.VIG_ACCO_ENTI, ae.VIS_ASSI })
                .Where(x => x.ID_ACCO == ID_ACCO && x.VIG_ACCO_ENTI == true && x.VIS_ASSI == true);

            var result = (from x in query.ToList()
                          join ce in dbe.CLASS_ENTITY on x.ID_ENTI2 equals ce.ID_ENTI
                          join pc in dbe.PERSON_CONTRACT.Where(x => x.VIG_CONT == true) on x.ID_PERS_ENTI equals pc.ID_PERS_ENTI
                          select new
                          {
                              ASSI = ce.FIR_NAME + ' ' + ce.SEC_NAME + ' ' + ce.LAS_NAME + ' ' + ce.MOT_NAME,
                              x.ID_PERS_ENTI,
                              WorkLoad = TicketsByAssi(x.ID_PERS_ENTI)
                          }).OrderBy(x => x.ASSI).ToList();

            result.Add(new { ASSI = "SIN ASIGNAR", ID_PERS_ENTI = -1, WorkLoad = 0 });

            return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarOperaciones(string q, string page)
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
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            var result = (dynamic)null;

            if ((int)Session["ADMINISTRADOR"] == 1)
            {
                result = dbe.OPListarOperaciones(ID_ACCO, termino);
            }
            else if ((int)Session["PROJECTMANAGER"] == 1)
            {
                result = dbe.OPListarOperaciones(ID_ACCO, termino);
            }
            else if ((int)Session["PROJECTMANAGER_OUTSOURCING"] == 1)
            {
                result = dbc.ListarPmoAsignados(29311, ID_ACCO, termino);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /AccountEntity/
        public ActionResult AssigneForFind()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            /* var query = (from t in dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO)
                          group t by new { t.ID_PERS_ENTI_ASSI } into g
                          select new
                          {
                              ID_PERS_ENTI_ASSI = g.Key.ID_PERS_ENTI_ASSI
                          }).ToList();


             //var query = db.PERSON_ENTITY.Where(pe => pe.ID_QUEU != null)
             //    .Join(db.ACCOUNT_ENTITY, x => x.ID_PERS_ENTI, ae => ae.ID_PERS_ENTI, (x, ae) => new { x.ID_PERS_ENTI, x.ID_ENTI2, ae.ID_ACCO, ae.VIG_ACCO_ENTI, ae.VIS_ASSI })
             //    .Where(x => x.ID_ACCO == ID_ACCO && x.VIG_ACCO_ENTI == true && x.VIS_ASSI == true);

             var result = (from pe in dbe.PERSON_ENTITY.ToList()
                           join f in query on pe.ID_PERS_ENTI equals f.ID_PERS_ENTI_ASSI
                           join ce in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals ce.ID_ENTI
                           select new
                           {
                               //ASSI = ce.FIR_NAME + ' ' + ce.LAS_NAME,
                               ASSI = ce.FIR_NAME + ' ' + (ce.SEC_NAME == null ? "" : " " + ce.SEC_NAME.Trim()) + ce.LAS_NAME + (ce.MOT_NAME == null ? "" : " " + ce.MOT_NAME),
                               pe.ID_PERS_ENTI,
                               WorkLoad = TicketsByAssi(Convert.ToInt32(pe.ID_PERS_ENTI))

                           }).OrderBy(x => x.ASSI);
            //var result = dbe.ASIGNADOS_LIST(ID_ACCO, "");*/

            /*var result = (from t in dbe.ListaAsignadosNew(ID_ACCO)
                          select new
                          {
                              ASSI = t.ASSI,
                              t.ID_PERS_ENTI,
                              WorkLoad = t.WorkLoad

                          }).ToList();*/
            var result = dbe.ListaAsignadosNew(ID_ACCO).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        private int TicketsByAssi(int id)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            var TICK = dbc.TICKETs.Where(t => t.ID_ACCO == ID_ACCO && t.ID_PERS_ENTI_ASSI == id && (t.ID_STAT_END == 1 || t.ID_STAT_END == 3));

            return TICK.Count();
        }
        //
        // GET: /AccountEntity/
        public ActionResult AEUByAcco()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            string NAM_PERS = "";
            string NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][0][field]"]), i1, i2;
            int ID_COMP = 0;
            if (NAM_PAR == "ID_PERS_ENTI")
            {
                int ID_PERS_ENTI = Convert.ToInt32(Request.QueryString[("filter[filters][0][value]")]);
                ID_COMP = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == ID_PERS_ENTI).ID_ENTI1.Value;
                NAM_PERS = Convert.ToString(Request.QueryString[("filter[filters][1][value]")]);
            }
            else
            {
                int ID_PERS_ENTI = Convert.ToInt32(Request.QueryString[("filter[filters][1][value]")]);
                ID_COMP = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == ID_PERS_ENTI).ID_ENTI1.Value;
                NAM_PERS = "";
            }

            var query = dbe.ACCOUNT_ENTITY.Where(ae => ae.VIS_REQU == true && ae.VIG_ACCO_ENTI == true && ae.ID_ACCO == ID_ACCO).ToList();

            if (String.IsNullOrEmpty(NAM_PERS))
            {
                NAM_PERS = "";
            }

            var result = (from x in query
                          join pe in dbe.PERSON_ENTITY.Where(x => x.ID_ENTI1 == ID_COMP) on x.ID_PERS_ENTI equals pe.ID_PERS_ENTI
                          join p in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals p.ID_ENTI
                          select new
                          {
                              CLIE = p.FIR_NAME + ' ' + (p.SEC_NAME == null ? "" : " " + p.SEC_NAME) + (p.LAS_NAME == null ? "" : " " + p.LAS_NAME) + (p.MOT_NAME == null ? "" : " " + p.MOT_NAME),
                              //CLIE = (p.FIR_NAME.Trim() + (p.SEC_NAME == null ? "" : " " + p.SEC_NAME.Trim()) + (p.LAS_NAME == null ? "" : " " + p.LAS_NAME.Trim()) + (p.MOT_NAME == null ? "" : " " + p.MOT_NAME.Trim())),
                              pe.ID_PERS_ENTI,
                              pe.ID_PRIO
                          }).ToList().OrderBy(x => x.CLIE).Where(x => x.CLIE.Contains(NAM_PERS.ToUpper()) && x.CLIE != " ");

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AEUByAcco2()
        {
            //int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            string NAM_PERS = "";
            string NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][0][field]"]), i1, i2;
            int ID_COMP = 0;

            if (NAM_PAR == "ID_PERS_ENTI")
            {
                int ID_PERS_ENTI = Convert.ToInt32(Request.QueryString[("filter[filters][0][value]")]);
                ID_COMP = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == ID_PERS_ENTI).ID_ENTI1.Value;
                NAM_PERS = Convert.ToString(Request.QueryString[("filter[filters][1][value]")]);
            }
            else
            {
                int ID_PERS_ENTI = Convert.ToInt32(Request.QueryString[("filter[filters][1][value]")]);
                ID_COMP = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == ID_PERS_ENTI).ID_ENTI1.Value;
                NAM_PERS = Convert.ToString(Request.QueryString["filter[filters][0][value]"]);
            }
            //NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][1][field]"]);
            //if (NAM_PAR == "ID_PERS_ENTI")
            //{
            //    ID_CIA = Convert.ToInt32(Request.QueryString["filter[filters][0][value]"]);
            //}
            //else if (NAM_PAR == "CLIE")
            //{
            //    va = Convert.ToString(Request.QueryString["filter[filters][0][value]"]);
            //}

            //NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][1][field]"]);
            //if (NAM_PAR == "ID_PERS_ENTI")
            //{
            //    ID_CIA = Convert.ToInt32(Request.QueryString["filter[filters][1][value]"]);
            //}
            //else if (NAM_PAR == "CLIE")
            //{
            //    CTE = Convert.ToString(Request.QueryString["filter[filters][1][value]"]);
            //}

            //var query = dbe.ACCOUNT_ENTITY.Where(ae => ae.VIS_REQU == true && ae.VIG_ACCO_ENTI == true).ToList();

            if (String.IsNullOrEmpty(NAM_PERS))
            {
                NAM_PERS = "";
            }

            var result = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_ENTI1 == ID_COMP).ToList()
                          join p in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals p.ID_ENTI
                          select new
                          {
                              //CLIE = (p.FIR_NAME + " " + (p.LAS_NAME == null ? "" : p.LAS_NAME) + (p.MOT_NAME == null ? "" : " " + p.MOT_NAME)).ToUpper(),
                              CLIE = (p.FIR_NAME + " " + (p.SEC_NAME == null ? "" : p.SEC_NAME) + (p.LAS_NAME == null ? "" : " " + p.LAS_NAME) + (p.MOT_NAME == null ? "" : " " + p.MOT_NAME)).ToUpper(),
                              pe.ID_PERS_ENTI,
                              pe.ID_PRIO,
                              ID_AREA = (pe.ID_AREA == null ? 0 : pe.ID_AREA)
                          }).ToList().OrderBy(x => x.CLIE).Where(x => x.CLIE.Contains(NAM_PERS.ToUpper()));
            var r = result.ToList();
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }
        //
        // GET: /AccountEntity/

        public ActionResult Index()
        {
            var account_entity = dbe.ACCOUNT_ENTITY.Include(a => a.PERSON_ENTITY);
            return View(account_entity.ToList());
        }

        //
        // GET: /AccountEntity/Details/5

        public ActionResult Details(int id = 0)
        {
            ACCOUNT_ENTITY account_entity = dbe.ACCOUNT_ENTITY.Find(id);
            if (account_entity == null)
            {
                return HttpNotFound();
            }
            return View(account_entity);
        }

        //
        // GET: /AccountEntity/Create

        public ActionResult Create()
        {
            ViewBag.ID_PERS_ENTI = new SelectList(dbe.PERSON_ENTITY, "ID_PERS_ENTI", "FOT_PERS");
            return View();
        }

        public ActionResult ViewNewClient(int id = 0)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            ViewBag.ID_ACCO = ID_ACCO;
            ViewBag.ID_COMP = id;
            ViewBag.IdAccoType = dbc.ACCOUNTs.Single(x => x.ID_ACCO == ID_ACCO).ID_ACCO_TYPE;
            return View();
        }

        public ActionResult ViewEditClient(int id = 0)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            ViewBag.ID_PERS_ENTI = id;
            var pe = dbe.PERSON_ENTITY.Find(id);

            var cp = dbe.CLASS_ENTITY.Single(x => x.ID_ENTI == pe.ID_ENTI2);
            ViewBag.FIR_NAME = cp.FIR_NAME;
            ViewBag.LAS_NAME = cp.LAS_NAME;
            ViewBag.SEX_ENTI = cp.SEX_ENTI;
            ViewBag.IdAccoType = dbc.ACCOUNTs.Single(x => x.ID_ACCO == ID_ACCO).ID_ACCO_TYPE;
            ViewBag.ID_LOCA = pe.ID_LOCA;
            ViewBag.DNI = pe.FOT_PERS;
            if (ID_ACCO == 60) {
                ViewBag.IDENTI = pe.ID_ENTI1;
            }

            return View(pe);
        }

        [HttpGet]
        public ActionResult GetSolicitante()
        {
            int idCia = 0;
            if (int.TryParse(Convert.ToString(Request.Params["ID_PERS_ENTI"]), out idCia) == false)
            {
                idCia = 0;
            }

            //x.ID_ENTI1 >= 78251 && x.ID_ENTI1 <= 78264
            var result = (from pe in dbe.PERSON_ENTITY.Where(x => x.VIG_PERS_ENTI == true && x.ID_PERS_ENTI == idCia).ToList()
                          join p in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals p.ID_ENTI
                          select new
                          {
                              CLIE = (p.FIR_NAME.Trim() + (p.SEC_NAME == null ? "" : " " + p.SEC_NAME.Trim())),
                              LAS_NAME = p.LAS_NAME == null ? "" : " " + p.LAS_NAME.Trim().Split(' ')[0],
                              MOT_NAME = (p.MOT_NAME == null ? ""+ string.Join(" ", p.LAS_NAME.Trim().Split(' ').Skip(1)) : " " + p.MOT_NAME.Trim()).ToUpper(),
                              pe.ID_PERS_ENTI,
                              pe.ID_PRIO,
                              ID_AREA = (pe.ID_AREA == null ? 0 : pe.ID_AREA),
                              ID_ENTI = pe.ID_ENTI1,
                              EMA_PERS = pe.EMA_PERS,
                              CEL_PERS = pe.CEL_PERS==null?"-":" "+pe.CEL_PERS,
                              CIP =  pe.FOT_PERS,
                              DNI = p.NUM_TYPE_DI,
                              TYPE_CLIENT = pe.IdTipoTrabajador,
                              ID_LOCA = pe.ID_LOCA,
                              UAD = pe.UAD_PERS
                          });
            result = result.OrderBy(x => x.CLIE).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
            
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult EditClient()
        {
            string fir_name = Convert.ToString(Request.Form["FIR_NAME"]);
            string las_name = Convert.ToString(Request.Form["LAS_NAME"]);
            string sex = Convert.ToString(Request.Form["SEX_ENTI"]);
            string cel = Convert.ToString(Request.Form["CEL_PERS"]);
            string rpm = Convert.ToString(Request.Form["RPM_PERS"]);
            string id_loca = Convert.ToString(Request.Form["ID_LOCA1"]);
            string ext = Convert.ToString(Request.Form["EXT_PERS"]);
            string ema = Convert.ToString(Request.Form["EMA_PERS"]);
            string car = Convert.ToString(Request.Form["CAR_PERS"]);
            string idtc = Convert.ToString(Request.Form["ID_TYPE_CLIE"]);
            string uad = Convert.ToString(Request.Form["UAD_PERS"]);
            string fot = Convert.ToString(Request.Form["FOT_PERS"]);
            String ar = Convert.ToString(Request.Form["ID_AREA"]);
            string idChar = Convert.ToString(Request.Form["ID_CHAR"]);
            int area = 0;
            string DNI = Convert.ToString(Request.Form["DNI"]);
            string NOMB = Convert.ToString(Request.Form["NOMB"]);
            string APEL = Convert.ToString(Request.Form["APEL"]);

            int sw = 0, existe = 0;
            int ID_TICL = 0;
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            if (ar.Trim().Length == 0)
            {
                sw = 1;
            }
            else
            {
                area = Convert.ToInt32(Request.Form["ID_AREA"]);
            }

            if (fir_name.Trim().Length == 0) { sw = 1; }
            else if (las_name.Trim().Length == 0) { sw = 1; }
            else if (sex != "M" && sex != "F") { sw = 1; }
            else if (ema.Trim().Length == 0) { sw = 1; }
            else if (idtc.Trim().Length == 0) { sw = 1; }
            if (int.TryParse(idtc, out ID_TICL))
            {
            }
            else { sw = 1; }

            if (ID_ACCO == 1 || ID_ACCO == 2 || ID_ACCO == 3)
            {
                if (fot.Trim().Length == 0) { sw = 1; }
                else if (uad.Trim().Length == 0) { sw = 1; }
            }

            if (dbc.ACCOUNTs.Single(x => x.ID_ACCO == ID_ACCO).ID_ACCO_TYPE == 1)
            {
                if (idChar.Trim().Length == 0) { sw = 1; }
            }
            else
            {
                if (car.Trim().Length == 0) { sw = 1; }
            }
            if (fot.Length != 8)
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDoneForm) top.uploadDoneForm('ERROR','DNI','0');}window.onload=init;</script>");
            }
            if (sw == 1)
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDoneForm) top.uploadDoneForm('ERROR','0','0');}window.onload=init;</script>");
            }

            else
            {
                //int ID_QUEU = Convert.ToInt32(Session["ID_QUEU"]);                
                int ID_PERS_ENTI = Convert.ToInt32(Request.Form["ID_PERS_ENTI"]);
                int ID_USER = Convert.ToInt32(Session["UserId"]);

                try
                {
                    var qPE = dbe.PERSON_ENTITY.Find(ID_PERS_ENTI);

                    var cla = dbe.CLASS_ENTITY.Find(qPE.ID_ENTI2);
                    if (las_name != APEL && fir_name != NOMB) {
                        var result = (from at in dbe.CLASS_ENTITY.Where(x => x.FIR_NAME == fir_name
                                        && x.LAS_NAME == las_name && x.ID_ENTI != qPE.ID_ENTI2).ToList()
                                      join pe in dbe.PERSON_ENTITY on at.ID_ENTI equals pe.ID_ENTI2
                                      where at.VIG_ENTI == true && pe.VIG_PERS_ENTI == true
                                      select new { id = at.ID_ENTI, name = at.FIR_NAME }).ToList();
                        if (result.Count() > 0)
                        {
                            existe = 1;
                        }

                        if (existe != 0)
                        {
                            return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.uploadDoneForm) top.uploadDoneForm('EXISTE','2','2');}window.onload=init;</script>");
                        }
                    }
                    if (fot != DNI) {
                        var existDNI = dbe.ValidarExistenciaDNI(fot, qPE.ID_ENTI1, cla.ID_ENTI);
                        var cantDNI = dbe.ValidarExistenciaDNI(fot, qPE.ID_ENTI1, cla.ID_ENTI).Count();
                        if (cantDNI > 0)
                        {
                            var DNISolicitante = existDNI.ToList()[0].Solicitante;
                            return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDoneForm) top.uploadDoneForm('ERROR','EXISTE','" + DNISolicitante + "');}window.onload=init;</script>");
                        }
                    }
                    cla.FIR_NAME = fir_name.Trim().ToUpper();
                    cla.LAS_NAME = las_name.Trim().ToUpper();
                    cla.SEX_ENTI = sex;
                    cla.VIG_ENTI = true;
                    cla.ID_TYPE_ENTI = 2;
                    cla.ID_TYPE_DI = 1;
                    cla.NUM_TYPE_DI = fot;

                    dbe.CLASS_ENTITY.Attach(cla);
                    dbe.Entry(cla).State = EntityState.Modified;
                    dbe.SaveChanges();

                    qPE.ID_TYPE_CLIE = ID_TICL;
                    qPE.FOT_PERS = fot;
                    if (dbc.ACCOUNTs.Single(x => x.ID_ACCO == ID_ACCO).ID_ACCO_TYPE == 1)
                        qPE.ID_CHAR = Convert.ToInt32(idChar);
                    else
                        qPE.CAR_PERS = car.Trim().ToUpper();
                    qPE.UAD_PERS = uad;
                    qPE.CEL_PERS = cel;
                    qPE.RPM_PERS = rpm;
                    qPE.EXT_PERS = ext;
                    qPE.EMA_PERS = ema;
                    qPE.ID_AREA = area;
                    if (id_loca.Trim().Length != 0)
                        qPE.ID_LOCA = Convert.ToInt32(id_loca);

                    dbe.PERSON_ENTITY.Attach(qPE);
                    dbe.Entry(qPE).State = EntityState.Modified;
                    dbe.SaveChanges();

                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDoneForm) top.uploadDoneForm('OK','" + ID_PERS_ENTI.ToString() + "','" + qPE.ID_PRIO.ToString() + "');}window.onload=init;</script>");
                }
                catch
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDoneForm) top.uploadDoneForm('ERROR','1','0');}window.onload=init;</script>");
                }
            }
        }

        //
        // POST: /AccountEntity/Create

        [HttpPost, ValidateInput(false)]
        public ActionResult EditClient2()
        {
            string fir_name = Convert.ToString(Request.Form["FIR_NAME"]);
            string las_name = Convert.ToString(Request.Form["LAS_NAME"]);
            //string sex = Convert.ToString(Request.Form["SEX_ENTI"]);
            string cel = Convert.ToString(Request.Form["CEL_PERS"]);
            //string rpm = Convert.ToString(Request.Form["RPM_PERS"]);
            string id_loca = Convert.ToString(Request.Form["ID_LOCA"]);
            //string ext = Convert.ToString(Request.Form["EXT_PERS"]);
            string ema = Convert.ToString(Request.Form["EMA_PERS"]);
            //string car = Convert.ToString(Request.Form["CAR_PERS"]);
            //string idtc = Convert.ToString(Request.Form["ID_TYPE_CLIE"]);
            //string uad = Convert.ToString(Request.Form["UAD_PERS"]);
            //string fot = Convert.ToString(Request.Form["FOT_PERS"]);
            //String ar = Convert.ToString(Request.Form["ID_AREA"]);
            //string idChar = Convert.ToString(Request.Form["ID_CHAR"]);
            int area = 0;

            int sw = 0, existe = 0;
            int ID_TICL = 0;
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            //if (ar.Trim().Length == 0)
            //{
            //    sw = 1;
            //}
            //else
            //{
            //    area = Convert.ToInt32(Request.Form["ID_AREA"]);
            //}

            //if (fir_name.Trim().Length == 0) { sw = 1; }
            //else if (las_name.Trim().Length == 0) { sw = 1; }
            //else if (sex != "M" && sex != "F") { sw = 1; }
            //else if (ema.Trim().Length == 0) { sw = 1; }
            //else if (idtc.Trim().Length == 0) { sw = 1; }
            //if (int.TryParse(idtc, out ID_TICL))
            //{
            //}
            //else { sw = 1; }

            //if (ID_ACCO == 1 || ID_ACCO == 2 || ID_ACCO == 3)
            //{
            //    if (fot.Trim().Length == 0) { sw = 1; }
            //    else if (uad.Trim().Length == 0) { sw = 1; }
            //}

            //if (dbc.ACCOUNTs.Single(x => x.ID_ACCO == ID_ACCO).ID_ACCO_TYPE == 1)
            //{
            //    if (idChar.Trim().Length == 0) { sw = 1; }
            //}
            //else
            //{
            //    if (car.Trim().Length == 0) { sw = 1; }
            //}
            //if (fot.Length != 8)
            //{
            //    return Content("<script type='text/javascript'> function init() {" +
            //    "if(top.uploadDoneForm) top.uploadDoneForm('ERROR','DNI','0');}window.onload=init;</script>");
            //}
            if (sw == 1)
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDoneForm) top.uploadDoneForm('ERROR','0','0');}window.onload=init;</script>");
            }

            else
            {
                //int ID_QUEU = Convert.ToInt32(Session["ID_QUEU"]);                
                int ID_PERS_ENTI = Convert.ToInt32(Request.Form["ID_PERS_ENTI"]);
                int ID_USER = Convert.ToInt32(Session["UserId"]);

                try
                {
                    var qPE = dbe.PERSON_ENTITY.Find(ID_PERS_ENTI);

                    var cla = dbe.CLASS_ENTITY.Find(qPE.ID_ENTI2);

                    var result = (from at in dbe.CLASS_ENTITY.Where(x => x.FIR_NAME == fir_name
                                    && x.LAS_NAME == las_name && x.ID_ENTI != qPE.ID_ENTI2).ToList()
                                  join pe in dbe.PERSON_ENTITY on at.ID_ENTI equals pe.ID_ENTI2
                                  select new { id = at.ID_ENTI, name = at.FIR_NAME }).ToList();
                    if (result.Count() > 0)
                    {
                        existe = 1;
                    }

                    if (existe != 0)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                                "if(top.uploadDoneForm) top.uploadDoneForm('EXISTE','2','2');}window.onload=init;</script>");
                    }

                    //var existDNI = dbe.ValidarExistenciaDNI(fot, qPE.ID_ENTI1, cla.ID_ENTI);
                    //var cantDNI = dbe.ValidarExistenciaDNI(fot, qPE.ID_ENTI1, cla.ID_ENTI).Count();
                    //if (cantDNI > 0)
                    //{
                    //    var DNISolicitante = existDNI.ToList()[0].Solicitante;
                    //    return Content("<script type='text/javascript'> function init() {" +
                    //    "if(top.uploadDoneForm) top.uploadDoneForm('ERROR','EXISTE','" + DNISolicitante + "');}window.onload=init;</script>");
                    //}

                    cla.FIR_NAME = fir_name.Trim().ToUpper();
                    cla.LAS_NAME = las_name.Trim().ToUpper();
                    //cla.SEX_ENTI = sex;
                    cla.VIG_ENTI = true;
                    cla.ID_TYPE_ENTI = 2;
                    cla.ID_TYPE_DI = 1;
                    //cla.NUM_TYPE_DI = fot;

                    dbe.CLASS_ENTITY.Attach(cla);
                    dbe.Entry(cla).State = EntityState.Modified;
                    dbe.SaveChanges();

                    qPE.ID_TYPE_CLIE = ID_TICL;
                    //qPE.FOT_PERS = fot;
                    //if (dbc.ACCOUNTs.Single(x => x.ID_ACCO == ID_ACCO).ID_ACCO_TYPE == 1)
                    //    qPE.ID_CHAR = Convert.ToInt32(idChar);
                    //else
                    //    qPE.CAR_PERS = car.Trim().ToUpper();
                    //qPE.UAD_PERS = uad;
                    qPE.CEL_PERS = cel;
                    //qPE.RPM_PERS = rpm;
                    //qPE.EXT_PERS = ext;
                    qPE.EMA_PERS = ema;
                    qPE.ID_AREA = area;
                    if (id_loca.Trim().Length != 0)
                        qPE.ID_LOCA = Convert.ToInt32(id_loca);

                    dbe.PERSON_ENTITY.Attach(qPE);
                    dbe.Entry(qPE).State = EntityState.Modified;
                    dbe.SaveChanges();

                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDoneForm) top.uploadDoneForm('OK','" + ID_PERS_ENTI.ToString() + "','" + qPE.ID_PRIO.ToString() + "');}window.onload=init;</script>");
                }
                catch
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDoneForm) top.uploadDoneForm('ERROR','1','0');}window.onload=init;</script>");
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ACCOUNT_ENTITY account_entity)
        {
            if (ModelState.IsValid)
            {
                dbe.ACCOUNT_ENTITY.Add(account_entity);
                dbe.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ID_PERS_ENTI = new SelectList(dbe.PERSON_ENTITY, "ID_PERS_ENTI", "FOT_PERS", account_entity.ID_PERS_ENTI);
            return View(account_entity);
        }

        //
        // GET: /AccountEntity/Edit/5

        public ActionResult Edit(int id = 0)
        {
            ACCOUNT_ENTITY account_entity = dbe.ACCOUNT_ENTITY.Find(id);
            if (account_entity == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_PERS_ENTI = new SelectList(dbe.PERSON_ENTITY, "ID_PERS_ENTI", "FOT_PERS", account_entity.ID_PERS_ENTI);
            return View(account_entity);
        }

        //
        // POST: /AccountEntity/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ACCOUNT_ENTITY account_entity)
        {
            if (ModelState.IsValid)
            {
                dbe.Entry(account_entity).State = EntityState.Modified;
                dbe.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID_PERS_ENTI = new SelectList(dbe.PERSON_ENTITY, "ID_PERS_ENTI", "FOT_PERS", account_entity.ID_PERS_ENTI);
            return View(account_entity);
        }

        //
        // GET: /AccountEntity/Delete/5

        public ActionResult Delete(int id = 0)
        {
            ACCOUNT_ENTITY account_entity = dbe.ACCOUNT_ENTITY.Find(id);
            if (account_entity == null)
            {
                return HttpNotFound();
            }
            return View(account_entity);
        }

        //
        // POST: /AccountEntity/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            ACCOUNT_ENTITY account_entity = dbe.ACCOUNT_ENTITY.Find(id);
            dbe.ACCOUNT_ENTITY.Remove(account_entity);
            dbe.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CreateClient()
        {
            string fir_name = Convert.ToString(Request.Form["FIR_NAME"]);
            string las_name = Convert.ToString(Request.Form["LAS_NAME"]);
            string sex = Convert.ToString(Request.Form["SEX_ENTI"]);
            string cel = Convert.ToString(Request.Form["CEL_PERS"]);
            string id_loca = Convert.ToString(Request.Form["ID_LOCA"]);
            string rpm = Convert.ToString(Request.Form["RPM_PERS"]);
            string ext = Convert.ToString(Request.Form["EXT_PERS"]);
            string ema = Convert.ToString(Request.Form["EMA_PERS"]);
            string car = Convert.ToString(Request.Form["CAR_PERS"]);
            //string idtc = Convert.ToString(Request.Form["ID_TYPE_CLIE"]);
            //string idcia = Convert.ToString(Request.Form["ID_COMP"]);
            string uad = Convert.ToString(Request.Form["UAD_PERS"]);
            string fot = Convert.ToString(Request.Form["FOT_PERS"]);
            string idChar = Convert.ToString(Request.Form["ID_CHAR"]);

            string ida = "0";

            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            if (ID_ACCO == 60)
            {
                idChar = Convert.ToString(Request.Form["ID_CHAR2"]);
                id_loca = Convert.ToString(Request.Form["ID_LOCA2"]);
            }
            int sw = 0;
            int ID_TICL = 0, ID_COMP;

            var PERS_ENTI = new PERSON_ENTITY();
            bool viscomp = dbc.ACCOUNTs.Single(x => x.ID_ACCO == ID_ACCO).VIS_COMP.Value;

            if (fir_name.Trim().Length == 0) { sw = 1; }
            else if (las_name.Trim().Length == 0) { sw = 1; }
            else if (sex != "M" && sex != "F") { sw = 1; }
            else if (ema.Trim().Length == 0) { sw = 1; }
            //else if (idtc.Trim().Length == 0) { sw = 1; }
            else if (Int32.TryParse(Convert.ToString(Request.Form["ID_TYPE_CLIE"]), out ID_TICL) == false) { sw = 1; }
            else
            {
                if (viscomp)
                {
                    if (Int32.TryParse(Convert.ToString(Request.Form["HD_ID_EMP"]), out ID_COMP) == false) { sw = 1; }
                }
                else
                {
                    ida = Convert.ToString(Request.Form["ID_AREA"]);
                    if (ID_ACCO == 60)
                    {
                        ida = Convert.ToString(Request.Form["ID_AREA2"]);
                    }
                    if (Int32.TryParse(Convert.ToString(Request.Form["ID_EMP"]), out ID_COMP) == false) { sw = 1; }
                    else if (ida.Trim().Length == 0) { sw = 1; }
                    else if (uad.Trim().Length == 0) { sw = 1; }
                }
                PERS_ENTI.ID_ENTI1 = ID_COMP;
            }

            if (dbc.ACCOUNTs.Single(x => x.ID_ACCO == ID_ACCO).ID_ACCO_TYPE == 1)
            {
                if (idChar.Trim().Length == 0) { sw = 1; }
            }
            else
            {
                if (car.Trim().Length == 0) { sw = 1; }
            }

            if (sw == 1)
            {
                return Content("<script type='text/javascript'> function init() {" +
                "if(top.uploadDoneForm) top.uploadDoneForm('ERROR','0','0');}window.onload=init;</script>");
            }
            else
            {
                //int ID_QUEU = Convert.ToInt32(Session["ID_QUEU"]);

                int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                //ID_COMP = Convert.ToInt32(Request.Form["ID_COMP"]);
                int ID_USER = Convert.ToInt32(Session["UserId"]);

                //if (ID_ACCO != 4) {
                //    ID_COMP = Convert.ToInt32(dbc.ACCOUNT_PARAMETER.Single(x => x.ID_PARA == 6 &&
                //              x.ID_ACCO == ID_ACCO && x.VIG_ACCO_PARA == true).VAL_ACCO_PARA);
                //}
                //Validar cantidad de digitos (8 DNI)

                if (fot.Length != 8)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDoneForm) top.uploadDoneForm('ERROR','DNI','0');}window.onload=init;</script>");
                }
                int cant = dbe.CLASS_ENTITY.Where(x => x.FIR_NAME == fir_name.Trim() && x.LAS_NAME == las_name.Trim())
                       .Join(dbe.PERSON_ENTITY.Where(x => x.ID_ENTI1 == PERS_ENTI.ID_ENTI1), x => x.ID_ENTI, pe => pe.ID_ENTI2, (x, pe) => new
                       {
                           x.ID_ENTI,
                           pe.ID_PERS_ENTI
                       })
                       .Join(dbe.ACCOUNT_ENTITY, x => x.ID_PERS_ENTI, ae => ae.ID_PERS_ENTI, (x, ae) => new
                       {
                           x.ID_ENTI,
                           x.ID_PERS_ENTI,
                           ae.ID_ACCO,
                           ae.VIG_ACCO_ENTI
                       }).Where(x => x.ID_ACCO == ID_ACCO && x.VIG_ACCO_ENTI == true).Count();
                var existDNI = dbe.ValidarExistenciaDNI(fot, PERS_ENTI.ID_ENTI1,0);
                var cantDNI = dbe.ValidarExistenciaDNI(fot, PERS_ENTI.ID_ENTI1,0).Count();
                if (cantDNI > 0)
                {
                    var DNISolicitante = existDNI.ToList()[0].Solicitante;
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDoneForm) top.uploadDoneForm('ERROR','EXISTE','" + DNISolicitante + "');}window.onload=init;</script>");
                }

                if (cant > 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDoneForm) top.uploadDoneForm('ERROR','2','0');}window.onload=init;</script>");
                }

                int idNew;
                try
                {
                    var cla = new CLASS_ENTITY();
                    cla.FIR_NAME = fir_name.Trim().ToUpper();
                    cla.LAS_NAME = las_name.Trim().ToUpper();
                    cla.SEX_ENTI = sex;
                    cla.VIG_ENTI = true;
                    cla.NUM_TYPE_DI = fot;
                    cla.ID_TYPE_ENTI = 2;
                    cla.ID_TYPE_DI = 1;
                    cla.CREATED = DateTime.Now;
                    cla.ID_USER = ID_USER;

                    dbe.CLASS_ENTITY.Add(cla);
                    dbe.SaveChanges();

                    int id = Convert.ToInt32(cla.ID_ENTI);
                    int ID_PRIO = Convert.ToInt32(dbe.TYPE_CLIENT.Single(x => x.ID_TYPE_CLIE == ID_TICL).ID_PRIO);

                    try
                    {
                        if (fot.Trim().Length == 0) { fot = "0"; }

                        if (dbc.ACCOUNTs.Single(x => x.ID_ACCO == ID_ACCO).ID_ACCO_TYPE != 1)
                            if (car.Trim().Length == 0) { car = "-"; }

                        if (uad.Trim().Length == 0) { uad = "-"; }

                        PERS_ENTI.ID_ENTI2 = id;
                        PERS_ENTI.ID_QUEU = null;
                        PERS_ENTI.ID_PRIO = ID_PRIO;
                        PERS_ENTI.ID_TYPE_CLIE = ID_TICL;
                        PERS_ENTI.FOT_PERS = fot;
                        if (dbc.ACCOUNTs.Single(x => x.ID_ACCO == ID_ACCO).ID_ACCO_TYPE == 1)
                            PERS_ENTI.ID_CHAR = Convert.ToInt32(idChar);
                        else
                            PERS_ENTI.CAR_PERS = car.Trim().ToUpper();
                        PERS_ENTI.UAD_PERS = uad;
                        PERS_ENTI.CEL_PERS = cel;
                        if (id_loca.Trim().Length != 0)
                            PERS_ENTI.ID_LOCA = Convert.ToInt32(id_loca);
                        PERS_ENTI.RPM_PERS = rpm;
                        PERS_ENTI.EXT_PERS = ext;
                        PERS_ENTI.EMA_PERS = ema;
                        PERS_ENTI.CREATED = DateTime.Now;
                        PERS_ENTI.ID_USER = ID_USER;
                        PERS_ENTI.VIG_PERS_ENTI = true;
                        if (ID_ACCO == 4)
                        {
                            PERS_ENTI.ID_AREA = 44;
                        }
                        else
                        {
                            PERS_ENTI.ID_AREA = Convert.ToInt32(ida);
                        }

                        dbe.PERSON_ENTITY.Add(PERS_ENTI);
                        dbe.SaveChanges();

                        idNew = Convert.ToInt32(PERS_ENTI.ID_PERS_ENTI);

                        var ACCO_ENTI = new ACCOUNT_ENTITY();
                        ACCO_ENTI.ID_PERS_ENTI = idNew;
                        ACCO_ENTI.ID_ACCO = ID_ACCO;
                        ACCO_ENTI.VIG_ACCO_ENTI = true;
                        ACCO_ENTI.DEF_ACCO = true;
                        ACCO_ENTI.VIS_ASSI = ID_ACCO == 4 ? false : true;
                        ACCO_ENTI.VIS_TALE = false;
                        ACCO_ENTI.VIS_REQU = true;

                        dbe.ACCOUNT_ENTITY.Add(ACCO_ENTI);
                        dbe.SaveChanges();
                    }
                    catch (Exception)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDoneForm) top.uploadDoneForm('ERROR','1','0');}window.onload=init;</script>");
                    }
                    //string NewId = Convert.ToString(idNew);   
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDoneForm) top.uploadDoneForm('OK','" + idNew.ToString() + "','" + ID_PRIO.ToString() + "');}window.onload=init;</script>");
                }
                catch
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDoneForm) top.uploadDoneForm('ERROR','1','0');}window.onload=init;</script>");
                }
            }
        }

        public ActionResult ListCIAByID_ACCO()
        {
            var ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());

            var query1 = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 6 && x.ID_ACCO == ID_ACCO && x.VIG_ACCO_PARA == true);
            var query2 = dbe.CLASS_ENTITY.Where(x => x.COM_NAME != null);

            if (ID_ACCO == 60)//Buenaventura
            {
                query1 = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 1084 && x.ID_ACCO == ID_ACCO && x.VIG_ACCO_PARA == true);
                query2 = query2.Where(x => (x.ID_ENTI > 78250 && x.ID_ENTI < 78264) || x.ID_ENTI == 78440 || x.ID_ENTI == 78441);
            }

            var result = (from t in query1.ToList()
                          join c in query2 on Convert.ToInt32(t.VAL_ACCO_PARA) equals c.ID_ENTI
                          select new
                          {
                              ID_CIA = c.ID_ENTI,
                              CIA = c.COM_NAME
                          }).ToList();

            return Json(new { Data = result, Count = query1.Count() }, JsonRequestBehavior.AllowGet);
        }
        //
        // GET: /AccountEntity/ResponsibleAssetByIdCond
        public string ResponsibleAssetByIdCond(int id = 0, int id1 = 0)
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            var query = dbc.CLIENT_ASSET.Where(x => x.ID_ASSE == id && x.DAT_END == null)
                .Join(dbc.OPTION_CONDITION, x => x.ID_COND, o => o.ID_COND, (x, o) => new
                {
                    o.ID_OPTION,
                    o.LOAD_USER
                }).Where(x => x.ID_OPTION == id1);

            bool load = Convert.ToBoolean(query.First().LOAD_USER);
            if (load)
            {
                var query1 = dbc.PARAMETERs.Where(x => x.NAM_PARA == "RESPONSIBLE ASSET")
                    .Join(dbc.ACCOUNT_PARAMETER, x => x.ID_PARA, ap => ap.ID_PARA, (x, ap) => new
                    {
                        ap.VAL_ACCO_PARA,
                        ap.ID_ACCO
                    }).Where(x => x.ID_ACCO == ID_ACCO);

                string idper = Convert.ToString(query1.First().VAL_ACCO_PARA);

                return idper;
            }
            else
            {
                return "0";
            }
        }
        protected override void Dispose(bool disposing)
        {
            dbe.Dispose();
            base.Dispose(disposing);
        }


        public ActionResult AllRequesterByAcco()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            //var result = (from x in dbe.ACCOUNT_ENTITY.Where(ae => ae.ID_ACCO == ID_ACCO).ToList()
            //              join pe in dbe.PERSON_ENTITY on x.ID_PERS_ENTI equals pe.ID_PERS_ENTI
            //              join p in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals p.ID_ENTI
            //              select new
            //              {
            //                  CLIE = p.FIR_NAME + ' ' + (p.LAS_NAME == null ? "" : p.LAS_NAME),
            //                  pe.ID_PERS_ENTI,
            //              }).ToList().OrderBy(x => x.CLIE);

            var result = dbc.ActListarUsuario(ID_ACCO).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ListarPorCuentas()
        {
            try
            {
                int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

                var result = dbc.ActListarUsuario(ID_ACCO)
                    .Select(r => new { CLIE = r.CLIE, ID_PERS_ENTI = r.ID_PERS_ENTI })
                    .ToList();

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { Error = "Error al obtener datos de usuarios." });
            }
        }








        public ActionResult PersonaxCuenta(int ID_ACCO)
        {
            var result = dbc.ActListarUsuario(ID_ACCO).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListSubCiaByAcco()
        {
            var ID_ACCO = Convert.ToInt32(Session["ID_ACCO"].ToString());

            var query1 = dbc.ACCOUNT_PARAMETER.Where(x => x.ID_PARA == 35 && x.ID_ACCO == ID_ACCO && x.VIG_ACCO_PARA == true);
            var query2 = dbe.CLASS_ENTITY.Where(x => x.COM_NAME != null);

            var result = (from t in query1.ToList()
                          join c in query2 on Convert.ToInt32(t.VAL_ACCO_PARA) equals c.ID_ENTI
                          select new
                          {
                              ID_CIA = c.ID_ENTI,
                              CIA = c.COM_NAME
                          }).ToList();

            return Json(new { Data = result, Count = query1.Count() }, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult Encriptar(string requestToken)
        //{
        //    Criptografia cripto = new Criptografia();
        //    string llavePrivada = "Z1AASHTGO0MGGM6LFEFCGGYPSEOZPKBTKTDSUPZATERM7S9OSTDUYBBUR5NKGL7N";
        //    string password = "SFDFUSOQIL";
        //    string hash = cripto.SHA256Encripta(llavePrivada + requestToken + password);
        //    return Json(new { Data = hash }, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult ListarPorCola(string q, string page)
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

            int idAcco = Convert.ToInt32(Session["ID_ACCO"]);
            int idAreaResp = Convert.ToInt32(Request.Params["AreaResp"]);

            List<ListarPorCola_Result> resultado = dbc.ListarPorCola(idAreaResp, idAcco, termino).ToList();

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarCompañiasxCuenta(string q, string page)
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

            //var query = dbe.CLASS_ENTITY.Where(x => x.ID_TYPE_ENTI == 1);
            //from x in query.ToList()
            var result = (from x in dbe.CLASS_ENTITY.Where(x => x.ID_TYPE_ENTI == 1).ToList()
                          join td in dbe.TYPE_DOCUMENTIDENT on x.ID_TYPE_DI equals td.ID_TYPE_DI
                          select new
                          {
                              id = x.ID_ENTI,
                              text = (x.COM_NAME == null ? "" : x.COM_NAME.ToUpper()),
                          }).Where(x => x.text.Contains(termino.ToUpper())).OrderBy(x => x.text);

            //Para Outsourcing
            if (ID_ACCO != 4 && ID_ACCO != 17 && ID_ACCO != 25 && ID_ACCO != 24 && ID_ACCO != 22)
            {
                var Compania = Convert.ToInt32(dbc.ACCOUNT_PARAMETER.Where(x => x.ID_ACCO == ID_ACCO && x.ID_PARA == 6).First().VAL_ACCO_PARA);

                result = result.Where(x => x.id == Compania).OrderBy(x => x.text);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListarPersonalEdata(string q, string page)
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

            var resultado = dbe.ListarPersonalEdata(termino).ToList();

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarPersonaPorCompania()
        {
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            int IdPersEnti = Convert.ToInt32(Request.Params["IdPersEnti"]);
            var result = dbe.ListarPersonaPorCompania(IdAcco, IdPersEnti).ToList();
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarPersonaxCompania(int id = 0)
        {
            var result = dbe.ListarPersonalxCompania(id).ToList();
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarCargos()
        {
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            var result = dbe.ListarCargos(IdAcco);
            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarCargo(int id, string q, string page)
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

            var result = dbe.ListarCargo(id, termino).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarAsignadoA(int id, string q, string page)
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
            var result = dbc.ListarAsignadoA(id, ID_ACCO, termino).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public ActionResult TicketSeleccionarAsignadoA(int id = 0)
        {
            if (id != 0)
            {
                String idPersEnti = "";
                int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
                var queryAsignadoA = dbc.TicketSeleccionarAsignadoA(IdAcco, id).SingleOrDefault();
                if (queryAsignadoA.ID_PERS_ENTI == 0)
                    idPersEnti = "";
                else
                    idPersEnti = Convert.ToString(queryAsignadoA.ID_PERS_ENTI);
                return Content(idPersEnti);
            }
            else
            {
                return Content("");
            }
        }
        public ActionResult ObtenerCompaniaxCuenta()
        {
            int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);
            if (IdAcco != 0)
            {
                String IdEnti = "";
                var query = dbc.ObtenerCompaniaxCuenta(IdAcco).ToList();
                if (query.Count() == 0)
                    IdEnti = "";
                else
                    IdEnti = Convert.ToString(query.SingleOrDefault().IdEnti);
                return Content(IdEnti);
            }
            else
            {
                return Content("");
            }
        }

        public ActionResult ValidarTipoUsuario(int id = 0)
        {
            var query = dbe.ValidarTipoUsuario(id).ToList();

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarPersonalxCuenta(int IdCuenta = 0)
        {
            var query = dbe.ListarPersonalxCuenta(IdCuenta).ToList(); 

            return Json(new { Data = query }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListarCargosxCompania(int IdEnti = 0, int Tipo = 0)
        {
            //int IdAcco = Convert.ToInt32(Session["ID_ACCO"]);

            int ID_CIA = 0;
            string CTE = "";
            if (Tipo == 1)
            {

                ID_CIA = IdEnti;
            }
            else
            {
                string NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][0][field]"]);
                if (NAM_PAR == "ID_CIA")
                {
                    ID_CIA = Convert.ToInt32(Request.QueryString["filter[filters][0][value]"]);
                }
                else if (NAM_PAR == "text")
                {
                    CTE = Convert.ToString(Request.QueryString["filter[filters][0][value]"]);
                }

                NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][1][field]"]);
                if (NAM_PAR == "ID_CIA")
                {
                    ID_CIA = Convert.ToInt32(Request.QueryString["filter[filters][1][value]"]);
                }
                else if (NAM_PAR == "text")
                {
                    CTE = Convert.ToString(Request.QueryString["filter[filters][1][value]"]);
                }
            }
            var result = dbe.ListarCargosxCompania(ID_CIA);
            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GestionUsuarioSolicitante()
        {
            if (Convert.ToInt32(Session["ID_ACCO"].ToString()) == 60)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Error");
            }
        }

        public ActionResult ListSolicitante() {
            int idCia = 0;

            if(int.TryParse(Convert.ToString(Request.Params["ID_ENTI"]), out idCia) == false)
            {
                idCia = 78251;
            }

            //x.ID_ENTI1 >= 78251 && x.ID_ENTI1 <= 78264
            var result = (from pe in dbe.PERSON_ENTITY.Where(x => x.VIG_PERS_ENTI == true && x.ID_ENTI1 == idCia).ToList()
                          join p in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals p.ID_ENTI
                          select new
                          {
                              CLIE = (p.FIR_NAME.Trim() + (p.SEC_NAME == null ? "" : " " + p.SEC_NAME.Trim()) + (p.LAS_NAME == null ? "" : " " + p.LAS_NAME.Trim()) + (p.MOT_NAME == null ? "" : " " + p.MOT_NAME.Trim())).ToUpper(),
                              pe.ID_PERS_ENTI,
                              pe.ID_PRIO,
                              ID_AREA = (pe.ID_AREA == null ? 0 : pe.ID_AREA),
                              ID_ENTI = pe.ID_ENTI1,
                              UAD = pe.UAD_PERS
                          });
            result = result.OrderBy(x => x.CLIE).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListSolicitante2()
        {
            int ID_CIA = 78251;
            string CTE = "";

            //string NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][0][field]"]);
            if (int.TryParse(Convert.ToString(Request.Params["comboBoxUnidadMinera"]), out ID_CIA) == false)
            {
                ID_CIA = 78251;
            }
            if (Convert.ToString(Request.QueryString["NOM_USUARIO"]) == "" || Convert.ToString(Request.QueryString["NOM_USUARIO"]) == null)
            {
                CTE = null;
            }


            var result = (from pe in dbe.PERSON_ENTITY.Where(x => x.VIG_PERS_ENTI == true && x.ID_ENTI1 == ID_CIA).ToList()
                          join p in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals p.ID_ENTI
                          select new
                          {
                              CLIE = (p.FIR_NAME.Trim() + (p.SEC_NAME == null ? "" : " " + p.SEC_NAME.Trim()) + (p.LAS_NAME == null ? "" : " " + p.LAS_NAME.Trim()) + (p.MOT_NAME == null ? "" : " " + p.MOT_NAME.Trim())).ToUpper(),
                              pe.ID_PERS_ENTI,
                              pe.ID_PRIO,
                              ID_AREA = (pe.ID_AREA == null ? 0 : pe.ID_AREA),
                              ID_ENTI = pe.ID_ENTI1
                          });

            result = result.OrderBy(x => x.CLIE).ToList().Take(20);

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CreateClient2()
        {
            string fir_name = Convert.ToString(Request.Form["txtNombreUsuario1"]);
            string ape_pat = Convert.ToString(Request.Form["txtApellidoPat"]);
            string ape_mat = Convert.ToString(Request.Form["txtApellidoMat"]);
            ////string sex = Convert.ToString(Request.Form["SEX_ENTI"]);
            string cel = Convert.ToString(Request.Form["CEL_PERS"]);
            string id_loca = Convert.ToString(Request.Form["ID_LOCA2"]);
            //string rpm = Convert.ToString(Request.Form["RPM_PERS"]);
            //string ext = Convert.ToString(Request.Form["EXT_PERS"]);
            string ema = Convert.ToString(Request.Form["txtCorreoPers"]);
            //string car = Convert.ToString(Request.Form["CAR_PERS"]);
            string idtc = Convert.ToString(Request.Form["ID_TYPE_CLIE"]);
            ////string idcia = Convert.ToString(Request.Form["ID_COMP"]);
            string uad = Convert.ToString(Request.Form["txtuad"]);
            string idPers = Convert.ToString(Request.Form["persEnti"]);
            string cip = Convert.ToString(Request.Form["FOT_PERS"]);
            string dni = Convert.ToString(Request.Form["txtDniPers"]);
            //string idChar = Convert.ToString(Request.Form["ID_CHAR"]);

            string ida = "0";

            int existe = 0;

            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            int sw = 0;
            int ID_TICL = 0, ID_COMP;

            var PERS_ENTI = new PERSON_ENTITY();
            bool viscomp = dbc.ACCOUNTs.Single(x => x.ID_ACCO == ID_ACCO).VIS_COMP.Value;
            
            if(uad == "-")
            {
                try
                {
                        var idPersFind = Convert.ToInt32(idPers);
                        var qPE = dbe.PERSON_ENTITY.Find(idPersFind);
                        
                        var cla = dbe.CLASS_ENTITY.Find(qPE.ID_ENTI2);
                        dni = cla.NUM_TYPE_DI;
                        fir_name = cla.FIR_NAME;
                        ape_pat = cla.LAS_NAME;
                        var result = (from at in dbe.CLASS_ENTITY.Where(x => x.FIR_NAME == fir_name
                                      && x.LAS_NAME ==ape_pat && x.NUM_TYPE_DI == dni).ToList()
                                      join pe in dbe.PERSON_ENTITY on at.ID_ENTI equals pe.ID_ENTI2
                                      select new { id = at.ID_ENTI, name = at.FIR_NAME }).ToList();
                        if (result.Count() > 0)
                        {
                            existe = 1;
                        }

                        if (existe == 0)
                        {
                            return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.uploadDoneForm) top.uploadDoneForm('ERROR','2');}window.onload=init;</script>");
                        }
                        var existDNI = dbe.ValidarExistenciaDNI(dni, qPE.ID_ENTI1, cla.ID_ENTI);
                        var cantDNI = dbe.ValidarExistenciaDNI(dni, qPE.ID_ENTI1, cla.ID_ENTI).Count();
                        if (cantDNI > 0)
                        {
                            var DNISolicitante = existDNI.ToList()[0].Solicitante;
                            return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDoneForm) top.uploadDoneForm('ERROR','3');}window.onload=init;</script>");
                        }
                        if (viscomp)
                        {
                            if (Int32.TryParse(Convert.ToString(Request.Form["HD_ID_EMP"]), out ID_COMP) == false) { sw = 1; }
                        }
                        else
                        {
                            //ida = Convert.ToString(Request.Form["ID_AREA"]);
                            //if (ID_ACCO == 60)
                            //{
                            //    ida = Convert.ToString(Request.Form["ID_AREA2"]);
                            //}
                            if (Int32.TryParse(Convert.ToString(Request.Form["comboBoxUnidadMineraEditar"]), out ID_COMP) == false) { sw = 1; }
                            //else if (ida.Trim().Length == 0) { sw = 1; }
                            //else if (uad.Trim().Length == 0) { sw = 1; }
                        }
                    //cla.FIR_NAME = fir_name.Trim().ToUpper();
                    //cla.LAS_NAME = ape_pat.Trim().ToUpper();
                    //cla.SEX_ENTI = sex;
                    cla.VIG_ENTI = true;
                        cla.ID_TYPE_ENTI = 2;
                        cla.ID_TYPE_DI = 1;
                        //cla.NUM_TYPE_DI = fot;

                        dbe.CLASS_ENTITY.Attach(cla);
                        dbe.Entry(cla).State = EntityState.Modified;
                        dbe.SaveChanges();

                        qPE.ID_TYPE_CLIE = 2;
                        //qPE.FOT_PERS = fot;
                        //if (dbc.ACCOUNTs.Single(x => x.ID_ACCO == ID_ACCO).ID_ACCO_TYPE == 1)
                        //    qPE.ID_CHAR = Convert.ToInt32(idChar);
                        //else
                        //    qPE.CAR_PERS = car.Trim().ToUpper();
                        //qPE.UAD_PERS = uad;
                        qPE.CEL_PERS = cel;
                        //qPE.RPM_PERS = rpm;
                        //qPE.EXT_PERS = ext;
                        qPE.EMA_PERS = ema;
                        qPE.ID_ENTI1 = ID_COMP;
                        qPE.FOT_PERS = cip;
                    qPE.IdTipoTrabajador = Convert.ToInt32(idtc);
                    if (id_loca.Trim().Length != 0)
                            qPE.ID_LOCA = Convert.ToInt32(id_loca);

                        dbe.PERSON_ENTITY.Attach(qPE);
                        dbe.Entry(qPE).State = EntityState.Modified;
                        dbe.SaveChanges();
                        //qPE.ID_AREA = area;

                        //RETURN OK MODIFICADO
                    }
                    catch (Exception ex)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                                    "if(top.uploadDoneForm) top.uploadDoneForm('EXISTE','4');}window.onload=init;</script>");
                    }
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDoneForm) top.uploadDoneForm('OK','2');}window.onload=init;</script>");
            }
            else
            {
                if (fir_name.Trim().Length == 0) { sw = 1; }
                else if (ape_pat.Trim().Length == 0) { sw = 1; }
                else if (ape_mat.Trim().Length == 0) { sw = 1; }
                //else if (sex != "M" && sex != "F") { sw = 1; }
                //else if (ema.Trim().Length == 0) { sw = 1; }
                else if (idtc.Trim().Length == 0) { sw = 1; }
                else if (Int32.TryParse(Convert.ToString(Request.Form["ID_TYPE_CLIE"]), out ID_TICL) == false) { sw = 1; }
                else
                {
                    if (viscomp)
                    {
                        if (Int32.TryParse(Convert.ToString(Request.Form["HD_ID_EMP"]), out ID_COMP) == false) { sw = 1; }
                    }
                    else
                    {
                        //ida = Convert.ToString(Request.Form["ID_AREA"]);
                        //if (ID_ACCO == 60)
                        //{
                        //    ida = Convert.ToString(Request.Form["ID_AREA2"]);
                        //}
                        if (Int32.TryParse(Convert.ToString(Request.Form["comboBoxUnidadMineraEditar"]), out ID_COMP) == false) { sw = 1; }
                        //else if (ida.Trim().Length == 0) { sw = 1; }
                        //else if (uad.Trim().Length == 0) { sw = 1; }
                    }
                    PERS_ENTI.ID_ENTI1 = ID_COMP;
                }

                if (sw == 1)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                    "if(top.uploadDoneForm) top.uploadDoneForm('ERROR','0');}window.onload=init;</script>");
                }
                else
                {
                    int ID_QUEU = Convert.ToInt32(Session["ID_QUEU"]);

                    int ID_PERS_ENTI = Convert.ToInt32(Session["ID_PERS_ENTI"]);
                    //ID_COMP = Convert.ToInt32(Request.Form["ID_COMP"]);
                    int ID_USER = Convert.ToInt32(Session["UserId"]);

                    //if (ID_ACCO != 4) {
                    //    ID_COMP = Convert.ToInt32(dbc.ACCOUNT_PARAMETER.Single(x => x.ID_PARA == 6 &&
                    //              x.ID_ACCO == ID_ACCO && x.VIG_ACCO_PARA == true).VAL_ACCO_PARA);
                    //}
                    //Validar cantidad de digitos (8 DNI)

                    if (dni.Length != 8 && dni.Length != 11)
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDoneForm) top.uploadDoneForm('ERROR','REVISAR DNI');}window.onload=init;</script>");
                    }
                    //int cant = dbe.CLASS_ENTITY.Where(x => x.FIR_NAME == fir_name.Trim() && x.LAS_NAME == las_name.Trim())
                    //       .Join(dbe.PERSON_ENTITY.Where(x => x.ID_ENTI1 == PERS_ENTI.ID_ENTI1), x => x.ID_ENTI, pe => pe.ID_ENTI2, (x, pe) => new
                    //       {
                    //           x.ID_ENTI,
                    //           pe.ID_PERS_ENTI
                    //       })
                    //       .Join(dbe.ACCOUNT_ENTITY, x => x.ID_PERS_ENTI, ae => ae.ID_PERS_ENTI, (x, ae) => new
                    //       {
                    //           x.ID_ENTI,
                    //           x.ID_PERS_ENTI,
                    //           ae.ID_ACCO,
                    //           ae.VIG_ACCO_ENTI
                    //       }).Where(x => x.ID_ACCO == ID_ACCO && x.VIG_ACCO_ENTI == true).Count();
                    var existDNI = dbe.ValidarExistenciaDNI(dni, PERS_ENTI.ID_ENTI1, 0);
                    var cantDNI = dbe.ValidarExistenciaDNI(dni, PERS_ENTI.ID_ENTI1, 0).Count();
                    if (cantDNI > 0)
                    {
                        //var DNISolicitante = existDNI.ToList()[0].Solicitante;

                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDoneForm) top.uploadDoneForm('ERROR','DNI EXISTE');}window.onload=init;</script>");
                    }

                    //if (cant > 0)
                    //{
                    //    return Content("<script type='text/javascript'> function init() {" +
                    //    "if(top.uploadDoneForm) top.uploadDoneForm('ERROR','2','0');}window.onload=init;</script>");
                    //}

                    int idNew;
                    try
                    {
                        var cla = new CLASS_ENTITY();
                        cla.FIR_NAME = fir_name.Trim().ToUpper();
                        cla.LAS_NAME = ape_pat.Trim().ToUpper();
                        cla.MOT_NAME = ape_mat.Trim().ToUpper();
                        //cla.SEX_ENTI = sex;
                        cla.VIG_ENTI = true;
                        cla.NUM_TYPE_DI = dni;
                        cla.ID_TYPE_ENTI = 2;
                        cla.ID_TYPE_DI = 1;
                        cla.CREATED = DateTime.Now;
                        cla.ID_USER = ID_USER;

                        dbe.CLASS_ENTITY.Add(cla);
                        dbe.SaveChanges();

                        int id = Convert.ToInt32(cla.ID_ENTI);
                        //int ID_PRIO = Convert.ToInt32(dbe.TIPO_TRABAJADOR.Single(x => x.Id == ID_TICL).ID_PRIO);

                        try
                        {
                            if (cip.Trim().Length == 0) { cip = "0"; }

                            //if (dbc.ACCOUNTs.Single(x => x.ID_ACCO == ID_ACCO).ID_ACCO_TYPE != 1)
                            //    if (car.Trim().Length == 0) { car = "-"; }

                            if (uad.Trim().Length == 0) { uad = "-"; }
                            if (ema.Trim().Length == 0) { uad = "-"; }

                            PERS_ENTI.ID_ENTI2 = id;
                            PERS_ENTI.ID_QUEU = null;
                            //PERS_ENTI.ID_PRIO = ID_PRIO;
                            PERS_ENTI.ID_TYPE_CLIE = 2;
                            PERS_ENTI.FOT_PERS = cip;
                            //if (dbc.ACCOUNTs.Single(x => x.ID_ACCO == ID_ACCO).ID_ACCO_TYPE == 1)
                            //    PERS_ENTI.ID_CHAR = Convert.ToInt32(idChar);
                            //else
                            //    PERS_ENTI.CAR_PERS = car.Trim().ToUpper();
                            PERS_ENTI.IdTipoTrabajador = Convert.ToInt32(idtc);
                            PERS_ENTI.UAD_PERS = uad;
                            PERS_ENTI.CEL_PERS = cel;
                            if (id_loca.Trim().Length != 0)
                                PERS_ENTI.ID_LOCA = Convert.ToInt32(id_loca);
                            //PERS_ENTI.RPM_PERS = rpm;
                            //PERS_ENTI.EXT_PERS = ext;
                            PERS_ENTI.EMA_PERS = ema;
                            PERS_ENTI.CREATED = DateTime.Now;
                            PERS_ENTI.ID_USER = ID_USER;
                            PERS_ENTI.VIG_PERS_ENTI = true;
                            PERS_ENTI.CAR_PERS = "Proveedor";
                            //if (ID_ACCO == 4)
                            //{
                            //    PERS_ENTI.ID_AREA = 44;
                            //}
                            //else
                            //{
                            //    PERS_ENTI.ID_AREA = Convert.ToInt32(ida);
                            //}

                            dbe.PERSON_ENTITY.Add(PERS_ENTI);
                            dbe.SaveChanges();

                            idNew = Convert.ToInt32(PERS_ENTI.ID_PERS_ENTI);


                            
                            var ACCO_ENTI = new ACCOUNT_ENTITY();
                            ACCO_ENTI.ID_PERS_ENTI = idNew;
                            ACCO_ENTI.ID_ACCO = ID_ACCO;
                            ACCO_ENTI.VIG_ACCO_ENTI = true;
                            ACCO_ENTI.DEF_ACCO = true;
                            ACCO_ENTI.VIS_REQU = true;
                            ACCO_ENTI.VIS_ASSI = ID_ACCO == 4 ? false : true;
                            ACCO_ENTI.VIS_TALE = false;

                            dbe.ACCOUNT_ENTITY.Add(ACCO_ENTI);
                            dbe.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            return Content("<script type='text/javascript'> function init() {" +
                            "if(top.uploadDoneForm) top.uploadDoneForm('ERROR','0');}window.onload=init;</script>");
                        }
                        //string NewId = Convert.ToString(idNew);   
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDoneForm) top.uploadDoneForm('OK','" + idNew.ToString() + "');}window.onload=init;</script>");
                    }
                    catch
                    {
                        return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDoneForm) top.uploadDoneForm('ERROR','0');}window.onload=init;</script>");
                    }
                }
            }
            
        }

        //function eliminarUsuario(idPersEnti)
        //{
        //    if (confirm("¿Estás seguro que deseas eliminar este usuario?"))
        //    {
        //    $.ajax({
        //        url: "/AccountEntity/DeleteSolicitante",
        //        data: { ID_PERS_ENTI: idPersEnti },
        //        type: 'POST',
        //        success: function(response) {
        //                alert("El usuario ha sido eliminado exitosamente");
        //                listarUsuarios();
        //            },
        //        error: function(error) {
        //                console.log(error);
        //            }
        //        });
        //    }
        //}

        [HttpPost, ValidateInput(false)]
        public ActionResult DeleteSolicitante()
        {
            int idPers = 0;
            if (int.TryParse(Convert.ToString(Request.Params["ID_PERS_ENTI"]), out idPers) == false)
            {
                idPers = 0;
            }

            try
            {
                var qPE = dbe.PERSON_ENTITY.Find(idPers);
                if(qPE.UAD_PERS == "-")
                {
                    qPE.VIG_PERS_ENTI = false;
                    dbe.PERSON_ENTITY.Attach(qPE);
                    dbe.Entry(qPE).State = EntityState.Modified;
                    dbe.SaveChanges();
                    
                }
                else
                {
                    return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDoneForm) top.uploadDoneForm('ERROR','0');}window.onload=init;</script>");
                }
                
            }
            catch(Exception e)
            {
                return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDoneForm) top.uploadDoneForm('ERROR','0');}window.onload=init;</script>");
            }
            return Content("<script type='text/javascript'> function init() {" +
                        "if(top.uploadDoneForm) top.uploadDoneForm('OK','0');}window.onload=init;</script>");
        }

        public ActionResult AEUByAcco3(int IdComp)
        {
            //int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int ID_COMP = IdComp;

            var result = (from pe in dbe.PERSON_ENTITY.Where(x => x.ID_ENTI1 == ID_COMP).ToList()
                          join p in dbe.CLASS_ENTITY on pe.ID_ENTI2 equals p.ID_ENTI
                          select new
                          {
                              //CLIE = (p.FIR_NAME + " " + (p.LAS_NAME == null ? "" : p.LAS_NAME) + (p.MOT_NAME == null ? "" : " " + p.MOT_NAME)).ToUpper(),
                              CLIE = (p.FIR_NAME + " " + (p.SEC_NAME == null ? "" : p.SEC_NAME) + (p.LAS_NAME == null ? "" : " " + p.LAS_NAME) + (p.MOT_NAME == null ? "" : " " + p.MOT_NAME)).ToUpper(),
                              pe.ID_PERS_ENTI,
                              pe.ID_PRIO,
                              ID_AREA = (pe.ID_AREA == null ? 0 : pe.ID_AREA)
                          }).ToList().OrderBy(x => x.CLIE);//.Where(x => x.CLIE.Contains(CLIE.ToUpper()));

            var r = result.ToList();
            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ActListarUsuarioCreador()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);

            var result = dbc.ActListarUsuarioCreador(ID_ACCO).ToList();

            return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        public JsonResult ObtenerEmailSolicitante(int id)
        {
            try
            {
                // Supongamos que quieres obtener un dato relacionado con un ticket o alguna otra entidad
                var dato = dbe.ObtenerEmailSolicitante(id).FirstOrDefault();

                if (dato == null)
                {
                    return Json(new { success = false, message = "Dato no encontrado" }, JsonRequestBehavior.AllowGet);
                }

                // Retorna el dato específico al cliente
                return Json(new { success = true, dato = dato.ToString() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Manejo de errores
                return Json(new { success = false, message = "Ocurrió un error: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


    }
}