using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPElectrodata.Models;
using System.Data.Entity;
using System.Web.Security;
using ERPElectrodata.Object.Talent;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.IO;
using System.Data.Entity.Validation;
using System.Text;
using Electrodata.ERPElectrodata.Domain.Services.TemaService;
using Electrodata.ERPElectrodata.Infra.Reposotories.TemaRepositorie;
using System.Data.SqlClient;

namespace ERPElectrodata.Controllers
{
    public class AdministratorController : Controller
    {
        #region "Variables Globales PSO"
        TemaService domainTema = new TemaService(new TemaRepositorie());
        #endregion


        CoreEntities dbc = new CoreEntities();
        EntityEntities dbe = new EntityEntities();

        // GET: /Administrator/
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult ManagingCategory()
        {
            bool Acceso = false;
            //string[] rolesArray = Roles.GetRolesForUser(Convert.ToString(Session["UserName"]));
            //foreach (string rol in rolesArray)
            //{
            if ((int)Session["ADMINISTRADOR"] == 1 || (int)Session["SUPERVISOR_SERVICEDESK"] == 1 || (int)Session["ADMINISTRADOR_CATEGORIA"] == 1)
            {
                Acceso = true; //permitiendo al acceso
            }
            //}
            if (Acceso == true)
            {
                return View();
            }
            else
            {
                return Content("<h3><b>Necesitas autorización para esta sección.</b></h3>");
            }
        }
        public ActionResult viewNewCategory()
        {
            return View();
        }
        public ActionResult viewFindCategory()
        {
            return View();
        }
        public ActionResult viewEditCategory(int id = 0, int ID_ACCO = 0)
        {
            //var cat = dbc.CATEGORies.Find(id);
            var cat = dbc.CATEGORies.Single(x => x.ID_CATE == id);
            var NIV_CATE = cat.NIV_CATE;
            switch (NIV_CATE)
            {
                case 1:
                    {
                        ViewBag.NAM_CATE6 = "";
                        ViewBag.NAM_CATE5 = "";
                        ViewBag.NAM_CATE4 = "";
                        ViewBag.ABR_CATE4 = "";
                        List<object> listcatempty = new List<object>();
                        ViewBag.COMBO_NAM_CATE4 = new SelectList(listcatempty, "", "");
                        ViewBag.EDIT_NIV_CAT_N4 = "";

                        ViewBag.NAM_CATE3 = "";
                        ViewBag.ABR_CATE3 = "";
                        ViewBag.COMBO_NAM_CATE3 = new SelectList(listcatempty, "", "");
                        ViewBag.EDIT_NIV_CAT_N3 = "";

                        ViewBag.NAM_CATE2 = "";
                        ViewBag.ABR_CATE2 = "";
                        ViewBag.COMBO_NAM_CATE2 = new SelectList(listcatempty, "", "");
                        ViewBag.EDIT_NIV_CAT_N2 = "";

                        ViewBag.NAM_CATE1 = cat.NAM_CATE;
                        ViewBag.ABR_CATE1 = cat.ABR_CATE;

                        IQueryable<object> objcat1 = (from x in dbc.CATEGORies.Where(x => x.ID_CATE == id)
                                                      select new
                                                      {
                                                          ID_CATE = x.ID_CATE,
                                                          NAM_CATE = x.NAM_CATE,
                                                      });
                        List<object> listcat1 = new List<object>(objcat1);
                        ViewBag.COMBO_NAM_CATE1 = new SelectList(listcat1, "ID_CATE", "NAM_CATE");
                        ViewBag.EDIT_NIV_CAT_N4 = NIV_CATE;

                        IQueryable<object> acco = (from x in dbc.ACCOUNTs.Where(x => x.ID_ACCO == ID_ACCO)
                                                   select new
                                                   {
                                                       ID_ACCO = x.ID_ACCO,
                                                       NAM_ACCO = x.NAM_ACCO
                                                   });
                        List<object> listacco = new List<object>(acco);
                        ViewBag.NAM_ACCO = new SelectList(acco, "ID_ACCO", "NAM_ACCO");
                        break;
                    }
                case 2:
                    {
                        ViewBag.NAM_CATE4 = "";
                        ViewBag.ABR_CATE4 = "";
                        ViewBag.COMBO_NAM_CATE4 = "";
                        List<object> listcatempty = new List<object>();
                        ViewBag.COMBO_NAM_CATE4 = new SelectList(listcatempty, "", "");
                        ViewBag.EDIT_NIV_CAT_N4 = "";
                        ViewBag.NivelSLA4 = "";

                        ViewBag.NAM_CATE3 = "";
                        ViewBag.ABR_CATE3 = "";
                        ViewBag.COMBO_NAM_CATE3 = new SelectList(listcatempty, "", "");
                        ViewBag.EDIT_NIV_CAT_N3 = "";
                        ViewBag.NivelSLA3 = "";

                        ViewBag.NAM_CATE2 = cat.NAM_CATE;
                        ViewBag.ABR_CATE2 = cat.ABR_CATE;
                        ViewBag.EDIT_NIV_CAT_N2 = NIV_CATE;
                        ViewBag.NivelSLA2 = cat.NivelSla;
                        IQueryable<object> objcat2 = (from x in dbc.CATEGORies.Where(x => x.ID_CATE == id)
                                                      select new
                                                      {
                                                          ID_CATE = x.ID_CATE,
                                                          NAM_CATE = x.NAM_CATE
                                                      });
                        List<object> listcat2 = new List<object>(objcat2);
                        ViewBag.COMBO_NAM_CATE2 = new SelectList(listcat2, "ID_CATE", "NAM_CATE");

                        var cat1 = dbc.CATEGORies.Single(x => x.ID_CATE == cat.ID_CATE_PARE);
                        ViewBag.NAM_CATE1 = cat1.NAM_CATE;
                        ViewBag.ABR_CATE1 = cat1.ABR_CATE;
                        ViewBag.EDIT_NIV_CAT_N1 = cat1.NIV_CATE;
                        ViewBag.NivelSLA1 = cat1.NivelSla;

                        IQueryable<object> objcat1 = (from x in dbc.CATEGORies.Where(x => x.ID_CATE == cat.ID_CATE_PARE)
                                                      select new
                                                      {
                                                          ID_CATE = x.ID_CATE,
                                                          NAM_CATE = x.NAM_CATE
                                                      });
                        List<object> listcat1 = new List<object>(objcat1);
                        ViewBag.COMBO_NAM_CATE1 = new SelectList(listcat1, "ID_CATE", "NAM_CATE");

                        IQueryable<object> acco = (from x in dbc.ACCOUNTs.Where(x => x.ID_ACCO == ID_ACCO)
                                                   select new
                                                   {
                                                       ID_ACCO = x.ID_ACCO,
                                                       NAM_ACCO = x.NAM_ACCO
                                                   });
                        List<object> listacco = new List<object>(acco);
                        ViewBag.NAM_ACCO = new SelectList(acco, "ID_ACCO", "NAM_ACCO");
                        break;
                    }
                case 3:
                    {
                        ViewBag.NAM_CATE6 = "";
                        ViewBag.ABR_CATE6 = "";
                        ViewBag.NAM_CATE5 = "";
                        ViewBag.ABR_CATE5 = "";
                        ViewBag.NAM_CATE4 = "";
                        ViewBag.ABR_CATE4 = "";

                        List<object> listcatempty = new List<object>();
                        ViewBag.COMBO_NAM_CATE4 = new SelectList(listcatempty, "", "");
                        ViewBag.EDIT_NIV_CAT_N4 = "";
                        ViewBag.NivelSLA4 = "";

                        List<object> listcatempty5 = new List<object>();
                        ViewBag.COMBO_NAM_CATE5 = new SelectList(listcatempty5, "", "");
                        ViewBag.EDIT_NIV_CAT_N5 = "";
                        ViewBag.NivelSLA5 = "";

                        List<object> listcatempty6 = new List<object>();
                        ViewBag.COMBO_NAM_CATE6 = new SelectList(listcatempty6, "", "");
                        ViewBag.EDIT_NIV_CAT_N6 = "";
                        ViewBag.NivelSLA6 = "";

                        ViewBag.NAM_CATE3 = cat.NAM_CATE;
                        ViewBag.ABR_CATE3 = cat.ABR_CATE;
                        ViewBag.EDIT_NIV_CAT_N3 = NIV_CATE;
                        ViewBag.NivelSLA3 = cat.NivelSla;

                        IQueryable<object> objcat3 = (from x in dbc.CATEGORies.Where(x => x.ID_CATE == id)
                                                      select new
                                                      {
                                                          ID_CATE = x.ID_CATE,
                                                          NAM_CATE = x.NAM_CATE
                                                      });
                        List<object> listcat3 = new List<object>(objcat3);
                        ViewBag.COMBO_NAM_CATE3 = new SelectList(listcat3, "ID_CATE", "NAM_CATE");

                        try
                        {
                            IQueryable<object> q = (from x in dbc.CATEGORies.Where(x => x.ID_CATE == id)
                                                    join tt in dbc.TYPE_TICKET on x.ID_TYPE_TICK equals tt.ID_TYPE_TICK
                                                    select new
                                                    {
                                                        ID_TYPE_TICK = x.ID_TYPE_TICK,
                                                        NAM_TYPE_TICK = tt.NAM_TYPE_TICK
                                                    }
                                                    );
                            List<object> TipoTicket = new List<object>(q);
                            ViewBag.TipoTicket = new SelectList(TipoTicket, "ID_TYPE_TICK", "NAM_TYPE_TICK");
                        }
                        catch
                        {

                        }

                        var cat2 = dbc.CATEGORies.Single(x => x.ID_CATE == cat.ID_CATE_PARE);
                        ViewBag.NAM_CATE2 = cat2.NAM_CATE;
                        ViewBag.ABR_CATE2 = cat2.ABR_CATE;
                        ViewBag.EDIT_NIV_CAT_N2 = cat2.NIV_CATE;
                        ViewBag.NivelSLA2 = cat2.NivelSla;
                        IQueryable<object> objcat2 = (from x in dbc.CATEGORies.Where(x => x.ID_CATE == cat.ID_CATE_PARE)
                                                      select new
                                                      {
                                                          ID_CATE = x.ID_CATE,
                                                          NAM_CATE = x.NAM_CATE
                                                      });
                        List<object> listcat2 = new List<object>(objcat2);
                        ViewBag.COMBO_NAM_CATE2 = new SelectList(listcat2, "ID_CATE", "NAM_CATE");

                        var cat1 = dbc.CATEGORies.Single(x => x.ID_CATE == cat2.ID_CATE_PARE);
                        ViewBag.NAM_CATE1 = cat1.NAM_CATE;
                        ViewBag.ABR_CATE1 = cat1.ABR_CATE;
                        ViewBag.EDIT_NIV_CAT_N1 = cat1.NIV_CATE;
                        ViewBag.NivelSLA1 = cat1.NivelSla;
                        IQueryable<object> objcat1 = (from x in dbc.CATEGORies.Where(x => x.ID_CATE == cat2.ID_CATE_PARE)
                                                      select new
                                                      {
                                                          ID_CATE = x.ID_CATE,
                                                          NAM_CATE = x.NAM_CATE
                                                      });
                        List<object> listcat1 = new List<object>(objcat1);
                        ViewBag.COMBO_NAM_CATE1 = new SelectList(listcat1, "ID_CATE", "NAM_CATE");

                        IQueryable<object> acco = (from x in dbc.ACCOUNTs.Where(x => x.ID_ACCO == ID_ACCO)
                                                   select new
                                                   {
                                                       ID_ACCO = x.ID_ACCO,
                                                       NAM_ACCO = x.NAM_ACCO
                                                   });
                        List<object> listacco = new List<object>(acco);
                        ViewBag.NAM_ACCO = new SelectList(acco, "ID_ACCO", "NAM_ACCO");
                        ViewBag.IdCuenta = ID_ACCO;

                        if (cat.IdTipoGestionCambio != null)
                        {
                            ViewBag.IdTipoCambioTicket = cat.IdTipoGestionCambio;
                            var cambio = dbc.CHANGE_TYPE.Where(x => x.id == cat.IdTipoGestionCambio).SingleOrDefault();
                            ViewBag.TipoCambioTicket = cambio.nombre;
                        }
                        var aCategory = dbc.ACCOUNT_CATEGORY.Where(x => x.ID_CATE == id && x.ID_ACCO == ID_ACCO).Single();
                        int tktProblema = 0;
                        int gActivo = 0;
                        tktProblema = Convert.ToInt32(cat.AplicaTicketProblema);
                        gActivo = Convert.ToInt32(cat.AplicaGestionActivos);
                        ViewBag.TicketProblema = tktProblema;
                        ViewBag.GestionActivos = gActivo;
                        ViewBag.EstadoCategoria = Convert.ToInt32(aCategory.VIG_CATE);

                        ViewBag.TieneTarea = Convert.ToInt32(cat.TieneTarea);
                        ViewBag.AplicaNotificacion = Convert.ToInt32(cat.AplicaNotificacion);
                        break;
                    }
                case 4:
                    {
                        ViewBag.NAM_CATE6 = "";
                        ViewBag.ABR_CATE6 = "";
                        ViewBag.NAM_CATE5 = "";
                        ViewBag.ABR_CATE5 = "";
                        ViewBag.NAM_CATE4 = cat.NAM_CATE;
                        ViewBag.ABR_CATE4 = cat.ABR_CATE;
                        ViewBag.EDIT_NIV_CAT_N4 = NIV_CATE;
                        ViewBag.NivelSLA4 = cat.NivelSla;

                        List<object> listcatempty5 = new List<object>();
                        ViewBag.COMBO_NAM_CATE5 = new SelectList(listcatempty5, "", "");
                        ViewBag.EDIT_NIV_CAT_N5 = "";
                        ViewBag.NivelSLA5 = "";

                        List<object> listcatempty6 = new List<object>();
                        ViewBag.COMBO_NAM_CATE6 = new SelectList(listcatempty6, "", "");
                        ViewBag.EDIT_NIV_CAT_N6 = "";
                        ViewBag.NivelSLA6 = "";

                        IQueryable<object> objcat4 = (from x in dbc.CATEGORies.Where(x => x.ID_CATE == id)
                                                      select new
                                                      {
                                                          ID_CATE = x.ID_CATE,
                                                          NAM_CATE = x.NAM_CATE
                                                      });
                        List<object> listcat4 = new List<object>(objcat4);
                        ViewBag.COMBO_NAM_CATE4 = new SelectList(listcat4, "ID_CATE", "NAM_CATE");
                        try
                        {
                            IQueryable<object> q = (from x in dbc.CATEGORies.Where(x => x.ID_CATE == id)
                                                    join tt in dbc.TYPE_TICKET on x.ID_TYPE_TICK equals tt.ID_TYPE_TICK
                                                    select new
                                                    {
                                                        ID_TYPE_TICK = x.ID_TYPE_TICK,
                                                        NAM_TYPE_TICK = tt.NAM_TYPE_TICK
                                                    });
                            List<object> TipoTicket = new List<object>(q);
                            ViewBag.TipoTicket = new SelectList(TipoTicket, "ID_TYPE_TICK", "NAM_TYPE_TICK");
                        }
                        catch
                        {
                            ViewBag.TipoTicket = "";
                        }

                        var cat3 = dbc.CATEGORies.Single(x => x.ID_CATE == cat.ID_CATE_PARE);
                        ViewBag.NAM_CATE3 = cat3.NAM_CATE;
                        ViewBag.ABR_CATE3 = cat3.ABR_CATE;
                        ViewBag.EDIT_NIV_CAT_N3 = cat3.NIV_CATE;
                        ViewBag.NivelSLA3 = cat3.NivelSla;

                        IQueryable<object> objcat3 = (from x in dbc.CATEGORies.Where(x => x.ID_CATE == cat.ID_CATE_PARE)
                                                      select new
                                                      {
                                                          ID_CATE = x.ID_CATE,
                                                          NAM_CATE = x.NAM_CATE
                                                      });
                        List<object> listcat3 = new List<object>(objcat3);
                        ViewBag.COMBO_NAM_CATE3 = new SelectList(listcat3, "ID_CATE", "NAM_CATE");

                        var cat2 = dbc.CATEGORies.Single(x => x.ID_CATE == cat3.ID_CATE_PARE);
                        ViewBag.NAM_CATE2 = cat2.NAM_CATE;
                        ViewBag.ABR_CATE2 = cat2.ABR_CATE;
                        ViewBag.EDIT_NIV_CAT_N2 = cat2.NIV_CATE;
                        ViewBag.NivelSLA2 = cat2.NivelSla;

                        IQueryable<object> objcat2 = (from x in dbc.CATEGORies.Where(x => x.ID_CATE == cat3.ID_CATE_PARE)
                                                      select new
                                                      {
                                                          ID_CATE = x.ID_CATE,
                                                          NAM_CATE = x.NAM_CATE
                                                      });
                        List<object> listcat2 = new List<object>(objcat2);
                        ViewBag.COMBO_NAM_CATE2 = new SelectList(listcat2, "ID_CATE", "NAM_CATE");

                        var cat1 = dbc.CATEGORies.Single(x => x.ID_CATE == cat2.ID_CATE_PARE);
                        ViewBag.NAM_CATE1 = cat1.NAM_CATE;
                        ViewBag.ABR_CATE1 = cat1.ABR_CATE;
                        ViewBag.EDIT_NIV_CAT_N1 = cat1.NIV_CATE;
                        ViewBag.NivelSLA1 = cat1.NivelSla;

                        IQueryable<object> objcat1 = (from x in dbc.CATEGORies.Where(x => x.ID_CATE == cat2.ID_CATE_PARE)
                                                      select new
                                                      {
                                                          ID_CATE = x.ID_CATE,
                                                          NAM_CATE = x.NAM_CATE
                                                      });
                        List<object> listcat1 = new List<object>(objcat1);
                        ViewBag.COMBO_NAM_CATE1 = new SelectList(listcat1, "ID_CATE", "NAM_CATE");

                        IQueryable<object> acco = (from x in dbc.ACCOUNTs.Where(x => x.ID_ACCO == ID_ACCO)
                                                   select new
                                                   {
                                                       ID_ACCO = x.ID_ACCO,
                                                       NAM_ACCO = x.NAM_ACCO
                                                   });
                        List<object> listacco = new List<object>(acco);
                        ViewBag.NAM_ACCO = new SelectList(acco, "ID_ACCO", "NAM_ACCO");
                        ViewBag.IdCuenta = ID_ACCO;

                        if (cat.IdTipoGestionCambio != null)
                        {
                            ViewBag.IdTipoCambioTicket = cat.IdTipoGestionCambio;
                            var cambio = dbc.CHANGE_TYPE.Where(x => x.id == cat.IdTipoGestionCambio).SingleOrDefault();
                            ViewBag.TipoCambioTicket = cambio.nombre;
                        }
                        var aCategory = dbc.ACCOUNT_CATEGORY.Where(x => x.ID_CATE == id && x.ID_ACCO == ID_ACCO).Single();
                        int tktProblema = 0;
                        int gActivo = 0;
                        tktProblema = Convert.ToInt32(cat.AplicaTicketProblema);
                        gActivo = Convert.ToInt32(cat.AplicaGestionActivos);
                        ViewBag.TicketProblema = tktProblema;
                        ViewBag.GestionActivos = gActivo;
                        ViewBag.EstadoCategoria = Convert.ToInt32(aCategory.VIG_CATE);

                        ViewBag.TieneTarea = Convert.ToInt32(cat.TieneTarea);
                        ViewBag.AplicaNotificacion = Convert.ToInt32(cat.AplicaNotificacion);
                        break;
                    }
                case 5:
                    {
                        ViewBag.NAM_CATE6 = "";
                        ViewBag.ABR_CATE6 = "";

                        List<object> listcatempty6 = new List<object>();
                        ViewBag.COMBO_NAM_CATE6 = new SelectList(listcatempty6, "", "");
                        ViewBag.EDIT_NIV_CAT_N6 = "";
                        ViewBag.NivelSLA6 = "";

                        ViewBag.NAM_CATE5 = cat.NAM_CATE;
                        ViewBag.ABR_CATE5 = cat.ABR_CATE;
                        ViewBag.EDIT_NIV_CAT_N5 = NIV_CATE;
                        ViewBag.NivelSLA5 = "";

                        IQueryable<object> objcat5 = (
                            from x in dbc.CATEGORies.Where(x => x.ID_CATE == id)
                            select new
                            {
                                ID_CATE = x.ID_CATE,
                                NAM_CATE = x.NAM_CATE
                            }
                            );
                        List<object> listcat5 = new List<object>(objcat5);
                        ViewBag.COMBO_NAM_CATE5 = new SelectList(listcat5, "ID_CATE", "NAM_CATE");

                        try
                        {
                            IQueryable<object> q =
                                (from x in dbc.CATEGORies.Where(x => x.ID_CATE == id)
                                 join tt in dbc.TYPE_TICKET on x.ID_TYPE_TICK equals tt.ID_TYPE_TICK
                                 select new
                                 {
                                     ID_TYPE_TICK = x.ID_TYPE_TICK,
                                     NAM_TYPE_TICK = tt.NAM_TYPE_TICK
                                 }
                                 );
                            List<object> TipoTicket = new List<object>(q);
                            ViewBag.TipoTicket = new SelectList(TipoTicket, "ID_TYPE_TICK", "NAM_TYPE_TICK");
                        }
                        catch
                        {
                            ViewBag.TipoTicket = "";
                        }

                        var cat4 = dbc.CATEGORies.Single(x => x.ID_CATE == cat.ID_CATE_PARE);
                        ViewBag.NAM_CATE4 = cat4.NAM_CATE;
                        ViewBag.ABR_CATE4 = cat4.ABR_CATE;
                        ViewBag.EDIT_NIV_CAT_N4 = cat4.NIV_CATE;
                        ViewBag.NivelSLA4 = cat4.NivelSla;

                        IQueryable<object> objcat4 = (
                            from x in dbc.CATEGORies.Where(x => x.ID_CATE == cat.ID_CATE_PARE)
                            select new
                            {
                                ID_CATE = x.ID_CATE,
                                NAM_CATE = x.NAM_CATE
                            }
                            );
                        List<object> listcat4 = new List<object>(objcat4);
                        ViewBag.COMBO_NAM_CATE4 = new SelectList(listcat4, "ID_CATE", "NAM_CATE");

                        var cat3 = dbc.CATEGORies.Single(x => x.ID_CATE == cat4.ID_CATE_PARE);
                        ViewBag.NAM_CATE3 = cat3.NAM_CATE;
                        ViewBag.ABR_CATE3 = cat3.ABR_CATE;
                        ViewBag.EDIT_NIV_CAT_N3 = cat3.NIV_CATE;
                        ViewBag.NivelSLA3 = cat3.NivelSla;

                        IQueryable<object> objcat3 = (
                            from x in dbc.CATEGORies.Where(x => x.ID_CATE == cat4.ID_CATE_PARE)
                            select new
                            {
                                ID_CATE = x.ID_CATE,
                                NAM_CATE = x.NAM_CATE
                            }
                            );
                        List<object> listcat3 = new List<object>(objcat3);
                        ViewBag.COMBO_NAM_CATE3 = new SelectList(listcat3, "ID_CATE", "NAM_CATE");

                        var cat2 = dbc.CATEGORies.Single(x => x.ID_CATE == cat3.ID_CATE_PARE);
                        ViewBag.NAM_CATE2 = cat2.NAM_CATE;
                        ViewBag.ABR_CATE2 = cat2.ABR_CATE;
                        ViewBag.EDIT_NIV_CAT_N2 = cat2.NIV_CATE;
                        ViewBag.NivelSLA2 = cat2.NivelSla;

                        IQueryable<object> objcat2 = (from x in dbc.CATEGORies.Where(x => x.ID_CATE == cat3.ID_CATE_PARE)
                                                      select new
                                                      {
                                                          ID_CATE = x.ID_CATE,
                                                          NAM_CATE = x.NAM_CATE
                                                      });
                        List<object> listcat2 = new List<object>(objcat2);
                        ViewBag.COMBO_NAM_CATE2 = new SelectList(listcat2, "ID_CATE", "NAM_CATE");

                        var cat1 = dbc.CATEGORies.Single(x => x.ID_CATE == cat2.ID_CATE_PARE);
                        ViewBag.NAM_CATE1 = cat1.NAM_CATE;
                        ViewBag.ABR_CATE1 = cat1.ABR_CATE;
                        ViewBag.EDIT_NIV_CAT_N1 = cat1.NIV_CATE;
                        ViewBag.NivelSLA1 = cat1.NivelSla;

                        IQueryable<object> objcat1 = (from x in dbc.CATEGORies.Where(x => x.ID_CATE == cat2.ID_CATE_PARE)
                                                      select new
                                                      {
                                                          ID_CATE = x.ID_CATE,
                                                          NAM_CATE = x.NAM_CATE
                                                      });
                        List<object> listcat1 = new List<object>(objcat1);
                        ViewBag.COMBO_NAM_CATE1 = new SelectList(listcat1, "ID_CATE", "NAM_CATE");

                        IQueryable<object> acco = (from x in dbc.ACCOUNTs.Where(x => x.ID_ACCO == ID_ACCO)
                                                   select new
                                                   {
                                                       ID_ACCO = x.ID_ACCO,
                                                       NAM_ACCO = x.NAM_ACCO
                                                   });
                        List<object> listacco = new List<object>(acco);
                        ViewBag.NAM_ACCO = new SelectList(acco, "ID_ACCO", "NAM_ACCO");
                        ViewBag.IdCuenta = ID_ACCO;

                        if (cat.IdTipoGestionCambio != null)
                        {
                            ViewBag.IdTipoCambioTicket = cat.IdTipoGestionCambio;
                            var cambio = dbc.CHANGE_TYPE.Where(x => x.id == cat.IdTipoGestionCambio).SingleOrDefault();
                            ViewBag.TipoCambioTicket = cambio.nombre;
                        }
                        var aCategory = dbc.ACCOUNT_CATEGORY.Where(x => x.ID_CATE == id && x.ID_ACCO == ID_ACCO).Single();
                        int tktProblema = 0;
                        int gActivo = 0;
                        tktProblema = Convert.ToInt32(cat.AplicaTicketProblema);
                        gActivo = Convert.ToInt32(cat.AplicaGestionActivos);
                        ViewBag.TicketProblema = tktProblema;
                        ViewBag.GestionActivos = gActivo;
                        ViewBag.EstadoCategoria = Convert.ToInt32(aCategory.VIG_CATE);

                        ViewBag.TieneTarea = Convert.ToInt32(cat.TieneTarea);
                        ViewBag.AplicaNotificacion = Convert.ToInt32(cat.AplicaNotificacion);
                        break;
                    }

                case 6:
                    {
                        ViewBag.NAM_CATE6 = cat.NAM_CATE;
                        ViewBag.ABR_CATE6 = cat.ABR_CATE;
                        ViewBag.EDIT_NIV_CAT_N6 = NIV_CATE;
                        ViewBag.NivelSLA6 = cat.NivelSla;
                        IQueryable<object> objcat6 = (
                            from x in dbc.CATEGORies.Where(x => x.ID_CATE == id)
                            select new
                            {
                                ID_CATE = x.ID_CATE,
                                NAM_CATE = x.NAM_CATE
                            }
                            );
                        List<object> listcat6 = new List<object>(objcat6);
                        ViewBag.COMBO_NAM_CATE6 = new SelectList(listcat6, "ID_CATE", "NAM_CATE");

                        try
                        {
                            IQueryable<object> q =
                                (from x in dbc.CATEGORies.Where(x => x.ID_CATE == id)
                                 join tt in dbc.TYPE_TICKET on x.ID_TYPE_TICK equals tt.ID_TYPE_TICK
                                 select new
                                 {
                                     ID_TYPE_TICK = x.ID_TYPE_TICK,
                                     NAM_TYPE_TICK = tt.NAM_TYPE_TICK
                                 }
                                 );
                            List<object> TipoTicket = new List<object>(q);
                            ViewBag.TipoTicket = new SelectList(TipoTicket, "ID_TYPE_TICK", "NAM_TYPE_TICK");
                        }
                        catch
                        {
                            ViewBag.TipoTicket = "";
                        }


                        var cat5 = dbc.CATEGORies.Single(x => x.ID_CATE == cat.ID_CATE_PARE);
                        ViewBag.NAM_CATE5 = cat5.NAM_CATE;
                        ViewBag.ABR_CATE5 = cat5.ABR_CATE;
                        ViewBag.NivelSLA5 = cat5.NivelSla;
                        ViewBag.EDIT_NIV_CAT_N5 = cat5.NIV_CATE;

                        IQueryable<object> objcat5 = (
                            from x in dbc.CATEGORies.Where(x => x.ID_CATE == cat.ID_CATE_PARE)
                            select new
                            {
                                ID_CATE = x.ID_CATE,
                                NAM_CATE = x.NAM_CATE
                            }
                            );
                        List<object> listcat5 = new List<object>(objcat5);
                        ViewBag.COMBO_NAM_CATE5 = new SelectList(listcat5, "ID_CATE", "NAM_CATE");

                        //4
                        var cat4 = dbc.CATEGORies.Single(x => x.ID_CATE == cat5.ID_CATE_PARE);
                        ViewBag.NAM_CATE4 = cat4.NAM_CATE;
                        ViewBag.ABR_CATE4 = cat4.ABR_CATE;
                        ViewBag.NivelSLA4 = cat4.NivelSla;
                        ViewBag.EDIT_NIV_CAT_N4 = cat4.NIV_CATE;

                        IQueryable<object> objcat4 = (
                            from x in dbc.CATEGORies.Where(x => x.ID_CATE == cat5.ID_CATE_PARE)
                            select new
                            {
                                ID_CATE = x.ID_CATE,
                                NAM_CATE = x.NAM_CATE
                            }
                            );
                        List<object> listcat4 = new List<object>(objcat4);
                        ViewBag.COMBO_NAM_CATE4 = new SelectList(listcat4, "ID_CATE", "NAM_CATE");

                        var cat3 = dbc.CATEGORies.Single(x => x.ID_CATE == cat4.ID_CATE_PARE);
                        ViewBag.NAM_CATE3 = cat3.NAM_CATE;
                        ViewBag.ABR_CATE3 = cat3.ABR_CATE;
                        ViewBag.NivelSLA3 = cat3.NivelSla;
                        ViewBag.EDIT_NIV_CAT_N3 = cat3.NIV_CATE;

                        IQueryable<object> objcat3 = (
                            from x in dbc.CATEGORies.Where(x => x.ID_CATE == cat4.ID_CATE_PARE)
                            select new
                            {
                                ID_CATE = x.ID_CATE,
                                NAM_CATE = x.NAM_CATE
                            }
                            );
                        List<object> listcat3 = new List<object>(objcat3);
                        ViewBag.COMBO_NAM_CATE3 = new SelectList(listcat3, "ID_CATE", "NAM_CATE");

                        var cat2 = dbc.CATEGORies.Single(x => x.ID_CATE == cat3.ID_CATE_PARE);
                        ViewBag.NAM_CATE2 = cat2.NAM_CATE;
                        ViewBag.ABR_CATE2 = cat2.ABR_CATE;
                        ViewBag.NivelSLA2 = cat2.NivelSla;
                        ViewBag.EDIT_NIV_CAT_N2 = cat2.NIV_CATE;

                        IQueryable<object> objcat2 = (from x in dbc.CATEGORies.Where(x => x.ID_CATE == cat3.ID_CATE_PARE)
                                                      select new
                                                      {
                                                          ID_CATE = x.ID_CATE,
                                                          NAM_CATE = x.NAM_CATE
                                                      });
                        List<object> listcat2 = new List<object>(objcat2);
                        ViewBag.COMBO_NAM_CATE2 = new SelectList(listcat2, "ID_CATE", "NAM_CATE");

                        var cat1 = dbc.CATEGORies.Single(x => x.ID_CATE == cat2.ID_CATE_PARE);
                        ViewBag.NAM_CATE1 = cat1.NAM_CATE;
                        ViewBag.ABR_CATE1 = cat1.ABR_CATE;
                        ViewBag.NivelSLA1 = cat1.NivelSla;
                        ViewBag.EDIT_NIV_CAT_N1 = cat1.NIV_CATE;

                        IQueryable<object> objcat1 = (from x in dbc.CATEGORies.Where(x => x.ID_CATE == cat2.ID_CATE_PARE)
                                                      select new
                                                      {
                                                          ID_CATE = x.ID_CATE,
                                                          NAM_CATE = x.NAM_CATE
                                                      });
                        List<object> listcat1 = new List<object>(objcat1);
                        ViewBag.COMBO_NAM_CATE1 = new SelectList(listcat1, "ID_CATE", "NAM_CATE");

                        IQueryable<object> acco = (from x in dbc.ACCOUNTs.Where(x => x.ID_ACCO == ID_ACCO)
                                                   select new
                                                   {
                                                       ID_ACCO = x.ID_ACCO,
                                                       NAM_ACCO = x.NAM_ACCO
                                                   });
                        List<object> listacco = new List<object>(acco);
                        ViewBag.NAM_ACCO = new SelectList(acco, "ID_ACCO", "NAM_ACCO");
                        ViewBag.IdCuenta = ID_ACCO;

                        if (cat.IdTipoGestionCambio != null)
                        {
                            ViewBag.IdTipoCambioTicket = cat.IdTipoGestionCambio;
                            var cambio = dbc.CHANGE_TYPE.Where(x => x.id == cat.IdTipoGestionCambio).SingleOrDefault();
                            ViewBag.TipoCambioTicket = cambio.nombre;
                        }
                        var aCategory = dbc.ACCOUNT_CATEGORY.Where(x => x.ID_CATE == id && x.ID_ACCO == ID_ACCO).Single();
                        int tktProblema = 0;
                        int gActivo = 0;
                        tktProblema = Convert.ToInt32(cat.AplicaTicketProblema);
                        gActivo = Convert.ToInt32(cat.AplicaGestionActivos);
                        ViewBag.TicketProblema = tktProblema;
                        ViewBag.GestionActivos = gActivo;
                        ViewBag.EstadoCategoria = Convert.ToInt32(aCategory.VIG_CATE);

                        ViewBag.TieneTarea = Convert.ToInt32(cat.TieneTarea);
                        ViewBag.AplicaNotificacion = Convert.ToInt32(cat.AplicaNotificacion);
                        break;
                    }

                default:
                    break;
            }
            return View();
        }
        public string DeleteCategory()
        {
            try
            {
                int id = Convert.ToInt32(Request.Params["id"]);
                int account = Convert.ToInt32(Request.Params["account"]);

                CATEGORY cat = dbc.CATEGORies.Find(id);
                var acco_cat = (from c in dbc.ACCOUNT_CATEGORY
                                where c.ID_CATE == id && c.ID_ACCO == account
                                select c).Single();
                //cat.VIG_CATE = false;
                //cat.ACCO_USR = Convert.ToInt32(Session["UserId"]);
                //cat.DATE_END = DateTime.Now;
                acco_cat.VIG_ACCO_CATE = false;
                acco_cat.VIG_CATE = 0;
                //dbc.CATEGORies.Attach(cat);
                dbc.ACCOUNT_CATEGORY.Attach(acco_cat);
                //dbc.Entry(cat).State = EntityState.Modified;
                dbc.Entry(acco_cat).State = EntityState.Modified;
                dbc.SaveChanges();
                return "Ok";
            }
            catch (Exception)
            {
                return "Error";
            }
        }
        public ActionResult CreateCategory()
        {
            int vacio = 0;
            int nivel1 = 0;
            int ID_ACCO = 0, ID_CAT_N1 = 0, ID_CAT_N2 = 0, ID_CAT_N3 = 0, ID_CAT_N4 = 0, IdTipoTicket = 0;
            string ABR_N1 = string.Empty, NOM_CATE_N1 = string.Empty, ABR_N2 = string.Empty, NOM_CATE_N2 = string.Empty,
                ABR_N3 = string.Empty, NOM_CATE_N3 = string.Empty, ABR_N4 = string.Empty, NOM_CATE_N4 = string.Empty, TipoTicket = string.Empty;
            int NIV_CAT = 0, ID_CAT = 0;
            string NEW_CATE = string.Empty;
            string NEW_ABRV = string.Empty;
            string msjError = string.Empty;
            DateTime STAR_DATE, END_DATE;
            int UserId = Convert.ToInt32(Session["UserId"]);
            if (Int32.TryParse(Request.Params["ID_ACCO"].ToString(), out ID_ACCO) == false)
            {
                vacio = 1;
            }
            else
            {
                vacio = 0;
            }
            if (Int32.TryParse((Request.Params["ID_CAT_N1"].ToString()), out ID_CAT_N1) == false)
            {
                nivel1 = 1;
            }
            else
            {
                vacio = 0;
                NIV_CAT = Convert.ToInt32(Request.Params["NIV_CAT_N1"].ToString());
                ID_CAT = ID_CAT_N1;
            }
            if (Int32.TryParse((Request.Params["ID_CAT_N2"].ToString()), out ID_CAT_N2) == false)
                vacio = 1;
            else
            {
                vacio = 0;
                NIV_CAT = Convert.ToInt32(Request.Params["NIV_CAT_N2"].ToString());
                ID_CAT = ID_CAT_N2;
            }
            if (Int32.TryParse((Request.Params["ID_CAT_N3"].ToString()), out ID_CAT_N3) == false)
                vacio = 1;
            else
            {
                vacio = 0;
                NIV_CAT = Convert.ToInt32(Request.Params["NIV_CAT_N3"].ToString());
                ID_CAT = ID_CAT_N3;
            }
            if (Int32.TryParse((Request.Params["ID_CAT_N4"].ToString()), out ID_CAT_N4) == false)
                vacio = 1;
            else
            {
                vacio = 0;
                NIV_CAT = Convert.ToInt32(Request.Params["NIV_CAT_N4"].ToString());
                ID_CAT = ID_CAT_N4;
            }
            if (Int32.TryParse((Request.Params["IdTipoTicket"].ToString()), out IdTipoTicket) == false)
                vacio = 1;
            else
            {
                vacio = 0;
            }
            if (NIV_CAT != 4 && ID_ACCO != 0)
            {
                NEW_CATE = Convert.ToString(Request.Params["NEW_CATE"]).ToString();
                NEW_ABRV = Convert.ToString(Request.Params["NEW_ABRV"]).ToString();

                if (string.IsNullOrEmpty(NEW_CATE) && string.IsNullOrEmpty(NEW_ABRV))
                {
                    string ABR = string.Empty, NOM_CATE = string.Empty;

                    ABR_N1 = Convert.ToString(Request.Params["ABR_N1"]).ToString();
                    NOM_CATE_N1 = Convert.ToString(Request.Params["NOM_CATE_N1"]).ToString();
                    if (!string.IsNullOrEmpty(ABR_N1))
                    {
                        ABR = ABR_N1;
                    }
                    if (!string.IsNullOrEmpty(NOM_CATE_N1))
                    {
                        NOM_CATE = NOM_CATE_N1;
                    }
                    ABR_N2 = Convert.ToString(Request.Params["ABR_N2"]).ToString();
                    NOM_CATE_N2 = Convert.ToString(Request.Params["NOM_CATE_N2"]).ToString();
                    if (!string.IsNullOrEmpty(ABR_N2))
                    {
                        ABR = ABR_N2;
                    }
                    if (!string.IsNullOrEmpty(NOM_CATE_N2))
                    {
                        NOM_CATE = NOM_CATE_N2;
                    }
                    ABR_N3 = Convert.ToString(Request.Params["ABR_N3"]).ToString();
                    NOM_CATE_N3 = Convert.ToString(Request.Params["NOM_CATE_N3"]).ToString();
                    if (!string.IsNullOrEmpty(ABR_N3))
                    {
                        ABR = ABR_N3;
                    }
                    if (!string.IsNullOrEmpty(NOM_CATE_N3))
                    {
                        NOM_CATE = NOM_CATE_N3;
                    }
                    ABR_N4 = Convert.ToString(Request.Params["ABR_N4"]).ToString();
                    NOM_CATE_N4 = Convert.ToString(Request.Params["NOM_CATE_N4"]).ToString();
                    if (!string.IsNullOrEmpty(ABR_N4))
                    {
                        ABR = ABR_N4;
                    }
                    if (!string.IsNullOrEmpty(NOM_CATE_N4))
                    {
                        NOM_CATE = NOM_CATE_N4;
                    }
                    using (var context = new CoreEntities())
                    {
                        var cat = (from c in context.CATEGORies
                                   where c.ID_CATE == ID_CAT
                                   select c).Single();
                        cat.NAM_CATE = NOM_CATE;
                        cat.ABR_CATE = ABR;
                        cat.ACR_CATE = ABR;
                        cat.DATE_END = DateTime.Now;
                        context.Entry(cat).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                    return Content("OK");
                }
            }
            else
            {
                if (string.IsNullOrEmpty(NEW_CATE) && string.IsNullOrEmpty(NEW_ABRV))
                {
                    string ABR = string.Empty, NOM_CATE = string.Empty;

                    ABR_N1 = Convert.ToString(Request.Params["ABR_N1"]).ToString();
                    NOM_CATE_N1 = Convert.ToString(Request.Params["NOM_CATE_N1"]).ToString();
                    if (!string.IsNullOrEmpty(ABR_N1))
                    {
                        ABR = ABR_N1;
                    }
                    if (!string.IsNullOrEmpty(NOM_CATE_N1))
                    {
                        NOM_CATE = NOM_CATE_N1;
                    }
                    ABR_N2 = Convert.ToString(Request.Params["ABR_N2"]).ToString();
                    NOM_CATE_N2 = Convert.ToString(Request.Params["NOM_CATE_N2"]).ToString();
                    if (!string.IsNullOrEmpty(ABR_N2))
                    {
                        ABR = ABR_N2;
                    }
                    if (!string.IsNullOrEmpty(NOM_CATE_N2))
                    {
                        NOM_CATE = NOM_CATE_N2;
                    }
                    ABR_N3 = Convert.ToString(Request.Params["ABR_N3"]).ToString();
                    NOM_CATE_N3 = Convert.ToString(Request.Params["NOM_CATE_N3"]).ToString();
                    if (!string.IsNullOrEmpty(ABR_N3))
                    {
                        ABR = ABR_N3;
                    }
                    if (!string.IsNullOrEmpty(NOM_CATE_N3))
                    {
                        NOM_CATE = NOM_CATE_N3;
                    }
                    ABR_N4 = Convert.ToString(Request.Params["ABR_N4"]).ToString();
                    NOM_CATE_N4 = Convert.ToString(Request.Params["NOM_CATE_N4"]).ToString();
                    if (!string.IsNullOrEmpty(ABR_N4))
                    {
                        ABR = ABR_N4;
                    }
                    if (!string.IsNullOrEmpty(NOM_CATE_N4))
                    {
                        NOM_CATE = NOM_CATE_N4;
                    }
                    //CATEGORY cat = new CATEGORY();
                    using (var context = new CoreEntities())
                    {
                        var cat = (from c in context.CATEGORies
                                   where c.ID_CATE == ID_CAT
                                   select c).Single();
                        //cat.ID_CATE = result.ID_CATE;
                        cat.NAM_CATE = NOM_CATE;
                        cat.ABR_CATE = ABR;
                        cat.ACR_CATE = ABR;
                        cat.DATE_END = DateTime.Now;
                        cat.ID_TYPE_TICK = IdTipoTicket;
                        context.Entry(cat).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                    return Content("OK");
                }
            }
            STAR_DATE = DateTime.Now;
            END_DATE = DateTime.Now;
            if (nivel1 == 1)
            {
                using (CoreEntities context = new CoreEntities())
                {
                    //Registrar en CATEGORY
                    CATEGORY cat = new CATEGORY();
                    cat.NAM_CATE = NEW_CATE;
                    cat.ACR_CATE = NEW_ABRV;
                    cat.ABR_CATE = NEW_ABRV;
                    cat.VIG_CATE = true;
                    cat.DEF_CATE = true;
                    cat.NIV_CATE = 1;
                    cat.DATE_START = STAR_DATE;
                    cat.DATE_END = END_DATE;
                    cat.ACCO_USR = UserId;
                    dbc.CATEGORies.Add(cat);
                    dbc.SaveChanges();

                    //Registrar en ACCOUNT_CATEGORY
                    var query = context.CATEGORies.OrderByDescending(u => u.ID_CATE).FirstOrDefault();
                    ACCOUNT_CATEGORY acc_cat = new ACCOUNT_CATEGORY();
                    acc_cat.ID_ACCO = ID_ACCO;
                    acc_cat.ID_CATE = query.ID_CATE;
                    acc_cat.VIG_CATE = 1;
                    acc_cat.VIG_ACCO_CATE = true;
                    acc_cat.IS_LEVE_TWO = true;
                    dbc.ACCOUNT_CATEGORY.Add(acc_cat);
                    dbc.SaveChanges();
                }
                return Content("OK");
            }
            else if (ID_CAT != 0 && ID_ACCO != 0)
            {
                try
                {
                    using (CoreEntities context = new CoreEntities())
                    {
                        //Registrar en CATEGORY
                        CATEGORY cat = new CATEGORY();
                        cat.NAM_CATE = NEW_CATE;
                        cat.ACR_CATE = NEW_ABRV;
                        cat.ABR_CATE = NEW_ABRV;
                        cat.VIG_CATE = true;
                        cat.DEF_CATE = true;
                        cat.ID_CATE_PARE = ID_CAT;
                        if (NIV_CAT != 0 && NIV_CAT != 4)
                            cat.NIV_CATE = NIV_CAT + 1;
                        cat.DATE_START = STAR_DATE;
                        cat.DATE_END = END_DATE;
                        cat.ACCO_USR = UserId;
                        cat.ID_TYPE_TICK = IdTipoTicket;
                        dbc.CATEGORies.Add(cat);
                        dbc.SaveChanges();

                        //Registrar en ACCOUNT_CATEGORY
                        var query = context.CATEGORies.OrderByDescending(u => u.ID_CATE).FirstOrDefault();
                        ACCOUNT_CATEGORY acc_cat = new ACCOUNT_CATEGORY();
                        acc_cat.ID_ACCO = ID_ACCO;
                        acc_cat.ID_CATE = query.ID_CATE;
                        acc_cat.VIG_CATE = 1;
                        acc_cat.VIG_ACCO_CATE = true;
                        acc_cat.IS_LEVE_TWO = true;
                        dbc.ACCOUNT_CATEGORY.Add(acc_cat);
                        dbc.SaveChanges();
                    }
                }
                catch (DbEntityValidationException e)
                {
                    var newException = new FormattedDbEntityValidationException(e);
                    throw newException;
                }
                return Content("OK");
            }
            else
            {
                return Content("Error");
            }
        }

        public ActionResult EditarCategoria()
        {
            string ABR = string.Empty, NOM_CATE = string.Empty;

            int ID_ACCO = 0,
                ID_CAT_N1 = 0, ID_CAT_N2 = 0, ID_CAT_N3 = 0, ID_CAT_N4 = 0, ID_CAT_N5 = 0, ID_CAT_N6 = 0, ID_CAT_NFinal = 0,
                IdTipoTicket = 0,
                existe = 0;

            int NivelPadre = 0;
            int SLANV1 = 0, SLANV2 = 0, SLANV3 = 0, SLANV4 = 0, SLANV5 = 0, SLANV6 = 0;

            string ABR_N1 = string.Empty, NOM_CATE_N1 = string.Empty,
                ABR_N2 = string.Empty, NOM_CATE_N2 = string.Empty,
                ABR_N3 = string.Empty, NOM_CATE_N3 = string.Empty,
                ABR_N4 = string.Empty, NOM_CATE_N4 = string.Empty,
                ABR_N5 = string.Empty, NOM_CATE_N5 = string.Empty,
                ABR_N6 = string.Empty, NOM_CATE_N6 = string.Empty,
                TipoTicket = string.Empty;
            bool Error = false;

            //Obtener ID_CATE, abreviatura y nombres
            if (Int32.TryParse((Request.Params["ID_ACCO"].ToString()), out ID_ACCO) == false) { Error = true; }
            if (Int32.TryParse((Request.Params["ID_CAT_N1"].ToString()), out ID_CAT_N1) == false) { Error = true; }
            if (Int32.TryParse((Request.Params["ID_CAT_N2"].ToString()), out ID_CAT_N2) == false) { Error = true; }
            if (Int32.TryParse((Request.Params["ID_CAT_N3"].ToString()), out ID_CAT_N3) == false) { Error = true; }
            if (Int32.TryParse((Request.Params["ID_CAT_N4"].ToString()), out ID_CAT_N4) == false) { Error = true; }
            if (Int32.TryParse((Request.Params["ID_CAT_N5"].ToString()), out ID_CAT_N5) == false) { Error = true; }
            if (Int32.TryParse((Request.Params["ID_CAT_N6"].ToString()), out ID_CAT_N6) == false) { Error = true; }
            if (Int32.TryParse((Request.Params["IdTipoTicket"].ToString()), out IdTipoTicket) == false) { Error = true; }

            ABR_N1 = Convert.ToString(Request.Params["ABR_N1"]).ToString();
            NOM_CATE_N1 = Convert.ToString(Request.Params["NOM_CATE_N1"]).ToString();
            if (!Int32.TryParse(Request.Form["SLANV1"], out SLANV1)){}

            ABR_N2 = Convert.ToString(Request.Params["ABR_N2"]).ToString();
            NOM_CATE_N2 = Convert.ToString(Request.Params["NOM_CATE_N2"]).ToString();
            if (!Int32.TryParse(Request.Form["SLANV2"], out SLANV2)) { }

            ABR_N3 = Convert.ToString(Request.Params["ABR_N3"]).ToString();
            NOM_CATE_N3 = Convert.ToString(Request.Params["NOM_CATE_N3"]).ToString();
            if (!Int32.TryParse(Request.Form["SLANV3"], out SLANV3)) { }

            ABR_N4 = Convert.ToString(Request.Params["ABR_N4"]).ToString();
            NOM_CATE_N4 = Convert.ToString(Request.Params["NOM_CATE_N4"]).ToString();
            if (!Int32.TryParse(Request.Form["SLANV4"], out SLANV4)) { }

            ABR_N5 = Convert.ToString(Request.Params["ABR_N5"]).ToString();
            NOM_CATE_N5 = Convert.ToString(Request.Params["NOM_CATE_N5"]).ToString();
            if (!Int32.TryParse(Request.Form["SLANV5"], out SLANV5)) { }

            ABR_N6 = Convert.ToString(Request.Params["ABR_N6"]).ToString();
            NOM_CATE_N6 = Convert.ToString(Request.Params["NOM_CATE_N6"]).ToString();
            if (!Int32.TryParse(Request.Form["SLANV6"], out SLANV6)) { }

            //Verificar que las abreviaturas sean menores que 5
            if (ABR_N1.Length > 5 || ABR_N2.Length > 5 || ABR_N3.Length > 5 || ABR_N4.Length > 5 || ABR_N5.Length > 5 || ABR_N6.Length > 5)
            {
                return Content("ERROR_ABR");
            }
            //Tipo de Gestion de cambio obtener
            string cbTipoCambioTicket = Convert.ToString(Request.Params["cbTipoCambioTicket"]);

            using (var context = new CoreEntities())
            {
                //Primer nivel obligatorio
                int estado = 0;
                var cat = (from c in context.CATEGORies
                           where c.ID_CATE == ID_CAT_N1
                           select c).Single();

                if (cat.NAM_CATE.ToLower() != NOM_CATE_N1.ToLower())
                {
                    var catePadreN1 = (from r in dbc.ACCOUNT_CATEGORY.Where(x => x.ID_ACCO == ID_ACCO)
                                       join categoria in dbc.CATEGORies on r.ID_CATE equals categoria.ID_CATE
                                       where categoria.NIV_CATE == 1 && categoria.NAM_CATE == NOM_CATE_N1 && r.VIG_ACCO_CATE == true && r.VIG_CATE == 1
                                       select r).ToList();
                    existe = catePadreN1.Count();
                    if (existe > 0)
                        return Content(NOM_CATE_N1);
                }
                cat.NAM_CATE = NOM_CATE_N1;
                cat.ABR_CATE = ABR_N1;
                cat.ACR_CATE = ABR_N1;
                if (SLANV1 == 0) { cat.NivelSla = null; } else { cat.NivelSla = SLANV1; }
                cat.DATE_END = DateTime.Now;
                context.Entry(cat).State = EntityState.Modified;
                context.SaveChanges();

                //Segundo nivel obligatorio
                cat = (from c in context.CATEGORies
                       where c.ID_CATE == ID_CAT_N2
                       select c).Single();

                if (cat.NAM_CATE.ToLower() != NOM_CATE_N2.ToLower())
                {
                    var catePadreN2 = (from r in dbc.ACCOUNT_CATEGORY.Where(x => x.ID_ACCO == ID_ACCO)
                                       join categoria in dbc.CATEGORies on r.ID_CATE equals categoria.ID_CATE
                                       where categoria.NIV_CATE == 2 && categoria.ID_CATE_PARE == ID_CAT_N1 && categoria.NAM_CATE == NOM_CATE_N2
                                       select r).ToList();
                    existe = catePadreN2.Count();
                    if (existe > 0)
                        return Content(NOM_CATE_N2);
                }
                cat.NAM_CATE = NOM_CATE_N2;
                cat.ABR_CATE = ABR_N2;
                cat.ACR_CATE = ABR_N2;
                if (SLANV2 == 0) { cat.NivelSla = null; } else { cat.NivelSla = SLANV2; }
                cat.DATE_END = DateTime.Now;
                context.Entry(cat).State = EntityState.Modified;
                context.SaveChanges();

                //Tercer nivel obligatorio
                cat = (from c in context.CATEGORies
                       where c.ID_CATE == ID_CAT_N3
                       select c).Single();

                if (cat.NAM_CATE.ToLower() != NOM_CATE_N3.ToLower())
                {
                    var catePadreN3 = (from r in dbc.ACCOUNT_CATEGORY.Where(x => x.ID_ACCO == ID_ACCO)
                                       join categoria in dbc.CATEGORies on r.ID_CATE equals categoria.ID_CATE
                                       where categoria.NIV_CATE == 3 && categoria.ID_CATE_PARE == ID_CAT_N2 && categoria.NAM_CATE == NOM_CATE_N3
                                       select r).ToList();
                    existe = catePadreN3.Count();
                    if (existe > 0)
                        return Content(NOM_CATE_N3);
                }
                cat.NAM_CATE = NOM_CATE_N3;
                cat.ABR_CATE = ABR_N3;
                cat.ACR_CATE = ABR_N3;
                if (SLANV3 == 0) { cat.NivelSla = null; } else { cat.NivelSla = SLANV3; }
                cat.DATE_END = DateTime.Now;
                context.Entry(cat).State = EntityState.Modified;
                context.SaveChanges();

                //SOLO Buenaventura

                if(ID_ACCO == 60)
                {
                    //4 nivel SLA
                    cat = (from c in context.CATEGORies
                           where c.ID_CATE == ID_CAT_N4
                           select c).Single();

                    if (cat.NAM_CATE.ToLower() != NOM_CATE_N4.ToLower())
                    {
                        var catePadreN4 = (from r in dbc.ACCOUNT_CATEGORY.Where(x => x.ID_ACCO == ID_ACCO)
                                           join categoria in dbc.CATEGORies on r.ID_CATE equals categoria.ID_CATE
                                           where categoria.NIV_CATE == 3 && categoria.ID_CATE_PARE == ID_CAT_N3 && categoria.NAM_CATE == NOM_CATE_N4
                                           select r).ToList();
                        existe = catePadreN4.Count();
                        if (existe > 0)
                            return Content(NOM_CATE_N4);
                    }
                    cat.NAM_CATE = NOM_CATE_N4;
                    cat.ABR_CATE = ABR_N4;
                    cat.ACR_CATE = ABR_N4;
                    if (SLANV4 == 0) { cat.NivelSla = null; } else { cat.NivelSla = SLANV4; }
                    cat.DATE_END = DateTime.Now;
                    context.Entry(cat).State = EntityState.Modified;
                    context.SaveChanges();
                }

                if (ID_CAT_N4 == 0)
                {
                    // Añadir categoria 4
                    if (NOM_CATE_N4 != "" && ABR_N4 != "")
                    {
                        NivelPadre = 3;

                        int flagExiste = 0;
                        var catePadre = (from r in dbc.ACCOUNT_CATEGORY.Where(x => x.ID_ACCO == ID_ACCO)
                                         join categoria in dbc.CATEGORies on r.ID_CATE equals categoria.ID_CATE
                                         where categoria.NIV_CATE == 4 && categoria.ID_CATE_PARE == ID_CAT_N3 && categoria.NAM_CATE == NOM_CATE_N4
                                         select r).ToList();

                        flagExiste = catePadre.Count();

                        int UserId = Convert.ToInt32(Session["UserId"]);

                        if (flagExiste == 0)
                        {
                            var Categoria = new CATEGORY();
                            Categoria.NAM_CATE = NOM_CATE_N4;
                            Categoria.ACR_CATE = ABR_N4;
                            Categoria.ABR_CATE = ABR_N4;
                            if (SLANV4 == 0) { Categoria.NivelSla = null; } else { Categoria.NivelSla = SLANV4; }
                            Categoria.VIG_CATE = true;
                            Categoria.DEF_CATE = true;

                            Categoria.ID_CATE_PARE = ID_CAT_N3;

                            Categoria.NIV_CATE = 4;
                            Categoria.DATE_START = DateTime.Now;
                            Categoria.DATE_END = DateTime.Now;
                            Categoria.ACCO_USR = UserId;

                            Categoria.ID_TYPE_TICK = IdTipoTicket;

                            if (cbTipoCambioTicket != "" && cbTipoCambioTicket != "null")
                            {
                                Categoria.IdTipoGestionCambio = Convert.ToInt32(Request.Params["cbTipoCambioTicket"]);
                            }
                            else
                            {
                                Categoria.IdTipoGestionCambio = null;
                            }

                            if (Convert.ToString(Request.Params["cbTicketProblema"]) == "1")
                            {
                                Categoria.AplicaTicketProblema = true;
                            }
                            else
                            {
                                Categoria.AplicaTicketProblema = false;
                            }

                            if (Convert.ToString(Request.Params["cbxGestionActivos"]) == "1")
                            {
                                Categoria.AplicaGestionActivos = true;
                            }
                            else
                            {
                                Categoria.AplicaGestionActivos = false;
                            }

                            if (ID_ACCO == 56 || ID_ACCO == 57 || ID_ACCO == 58)
                            {
                                if (Convert.ToString(Request.Params["cbAplicaNotificacion"]) == "1")
                                {
                                    Categoria.AplicaNotificacion = true;
                                }
                                else
                                {
                                    Categoria.AplicaNotificacion = false;
                                }

                                if (Convert.ToString(Request.Params["cbTieneTarea"]) == "1")
                                {
                                    Categoria.TieneTarea = true;
                                }
                                else
                                {
                                    Categoria.TieneTarea = false;
                                }
                            }

                            if (Convert.ToString(Request.Params["cbxEstadoEdit"]) == "1")
                            {
                                estado = 1;
                            }
                            Categoria.VIG_CATE = Convert.ToBoolean(estado);

                            dbc.CATEGORies.Add(Categoria);
                            dbc.SaveChanges();

                            ACCOUNT_CATEGORY acc_categoria = new ACCOUNT_CATEGORY();
                            acc_categoria.ID_ACCO = ID_ACCO;
                            acc_categoria.ID_CATE = Categoria.ID_CATE;
                            acc_categoria.VIG_CATE = estado;
                            acc_categoria.VIG_ACCO_CATE = Categoria.VIG_CATE;
                            acc_categoria.IS_LEVE_TWO = true;
                            dbc.ACCOUNT_CATEGORY.Add(acc_categoria);
                            dbc.SaveChanges();

                            //Se añade categoria 5 si hay
                            if (NOM_CATE_N5 != "" && ABR_N5 != "")
                            {
                                var Categoria5 = new CATEGORY();
                                Categoria5.NAM_CATE = NOM_CATE_N5;
                                Categoria5.ACR_CATE = ABR_N5;
                                Categoria5.ABR_CATE = ABR_N5;
                                if (SLANV5 == 0) { Categoria5.NivelSla = null; } else { Categoria5.NivelSla = SLANV5; }
                                Categoria5.VIG_CATE = true;
                                Categoria5.DEF_CATE = true;

                                Categoria5.ID_CATE_PARE = Categoria.ID_CATE;

                                Categoria5.NIV_CATE = 5;
                                Categoria5.DATE_START = DateTime.Now;
                                Categoria5.DATE_END = DateTime.Now;
                                Categoria5.ACCO_USR = UserId;

                                Categoria5.ID_TYPE_TICK = IdTipoTicket;

                                if (cbTipoCambioTicket != "" && cbTipoCambioTicket != "null")
                                {
                                    Categoria5.IdTipoGestionCambio = Convert.ToInt32(Request.Params["cbTipoCambioTicket"]);
                                }
                                else
                                {
                                    Categoria5.IdTipoGestionCambio = null;
                                }

                                if (Convert.ToString(Request.Params["cbTicketProblema"]) == "1")
                                {
                                    Categoria5.AplicaTicketProblema = true;
                                }
                                else
                                {
                                    Categoria5.AplicaTicketProblema = false;
                                }

                                if (Convert.ToString(Request.Params["cbxGestionActivos"]) == "1")
                                {
                                    Categoria5.AplicaGestionActivos = true;
                                }
                                else
                                {
                                    Categoria5.AplicaGestionActivos = false;
                                }

                                if (ID_ACCO == 56 || ID_ACCO == 57 || ID_ACCO == 58)
                                {
                                    if (Convert.ToString(Request.Params["cbAplicaNotificacion"]) == "1")
                                    {
                                        Categoria5.AplicaNotificacion = true;
                                    }
                                    else
                                    {
                                        Categoria5.AplicaNotificacion = false;
                                    }

                                    if (Convert.ToString(Request.Params["cbTieneTarea"]) == "1")
                                    {
                                        Categoria5.TieneTarea = true;
                                    }
                                    else
                                    {
                                        Categoria5.TieneTarea = false;
                                    }
                                }

                                if (Convert.ToString(Request.Params["cbxEstadoEdit"]) == "1")
                                {
                                    estado = 1;
                                }
                                Categoria5.VIG_CATE = Convert.ToBoolean(estado);

                                dbc.CATEGORies.Add(Categoria5);
                                dbc.SaveChanges();

                                ACCOUNT_CATEGORY acc_categoria5 = new ACCOUNT_CATEGORY();
                                acc_categoria5.ID_ACCO = ID_ACCO;
                                acc_categoria5.ID_CATE = Categoria5.ID_CATE;
                                acc_categoria5.VIG_CATE = estado;
                                acc_categoria5.VIG_ACCO_CATE = Categoria5.VIG_CATE;
                                acc_categoria5.IS_LEVE_TWO = true;
                                dbc.ACCOUNT_CATEGORY.Add(acc_categoria5);
                                dbc.SaveChanges();

                                //Se añade categoria 6 si hay
                                if (NOM_CATE_N6 != "" && ABR_N6 != "")
                                {
                                    var Categoria6 = new CATEGORY();
                                    Categoria6.NAM_CATE = NOM_CATE_N6;
                                    Categoria6.ACR_CATE = ABR_N6;
                                    Categoria6.ABR_CATE = ABR_N6;
                                    if (SLANV6 == 0) { Categoria6.NivelSla = null; } else { Categoria6.NivelSla = SLANV6; }
                                    Categoria6.VIG_CATE = true;
                                    Categoria6.DEF_CATE = true;

                                    Categoria6.ID_CATE_PARE = Categoria5.ID_CATE;

                                    Categoria6.NIV_CATE = 5;
                                    Categoria6.DATE_START = DateTime.Now;
                                    Categoria6.DATE_END = DateTime.Now;
                                    Categoria6.ACCO_USR = UserId;

                                    Categoria6.ID_TYPE_TICK = IdTipoTicket;

                                    if (cbTipoCambioTicket != "" && cbTipoCambioTicket != "null")
                                    {
                                        Categoria6.IdTipoGestionCambio = Convert.ToInt32(Request.Params["cbTipoCambioTicket"]);
                                    }
                                    else
                                    {
                                        Categoria6.IdTipoGestionCambio = null;
                                    }

                                    if (Convert.ToString(Request.Params["cbTicketProblema"]) == "1")
                                    {
                                        Categoria6.AplicaTicketProblema = true;
                                    }
                                    else
                                    {
                                        Categoria6.AplicaTicketProblema = false;
                                    }

                                    if (Convert.ToString(Request.Params["cbxGestionActivos"]) == "1")
                                    {
                                        Categoria6.AplicaGestionActivos = true;
                                    }
                                    else
                                    {
                                        Categoria6.AplicaGestionActivos = false;
                                    }

                                    if (ID_ACCO == 56 || ID_ACCO == 57 || ID_ACCO == 58)
                                    {
                                        if (Convert.ToString(Request.Params["cbAplicaNotificacion"]) == "1")
                                        {
                                            Categoria6.AplicaNotificacion = true;
                                        }
                                        else
                                        {
                                            Categoria6.AplicaNotificacion = false;
                                        }

                                        if (Convert.ToString(Request.Params["cbTieneTarea"]) == "1")
                                        {
                                            Categoria6.TieneTarea = true;
                                        }
                                        else
                                        {
                                            Categoria6.TieneTarea = false;
                                        }
                                    }

                                    if (Convert.ToString(Request.Params["cbxEstadoEdit"]) == "1")
                                    {
                                        estado = 1;
                                    }
                                    Categoria6.VIG_CATE = Convert.ToBoolean(estado);

                                    dbc.CATEGORies.Add(Categoria6);
                                    dbc.SaveChanges();

                                    ACCOUNT_CATEGORY acc_categoria6 = new ACCOUNT_CATEGORY();
                                    acc_categoria6.ID_ACCO = ID_ACCO;
                                    acc_categoria6.ID_CATE = Categoria5.ID_CATE;
                                    acc_categoria6.VIG_CATE = estado;
                                    acc_categoria6.VIG_ACCO_CATE = Categoria6.VIG_CATE;
                                    acc_categoria6.IS_LEVE_TWO = true;
                                    dbc.ACCOUNT_CATEGORY.Add(acc_categoria6);
                                    dbc.SaveChanges();
                                }

                            }
                        }
                        // existe N4
                        else
                        {
                            return Content(NOM_CATE_N4);
                        }
                    }
                    else
                    {
                        //Añade modificaciones hasta el nivel 3
                        if (cbTipoCambioTicket != "" && cbTipoCambioTicket != "null")
                        {
                            cat.IdTipoGestionCambio = Convert.ToInt32(Request.Params["cbTipoCambioTicket"]);
                        }
                        else
                        {
                            cat.IdTipoGestionCambio = null;
                        }

                        cat.ID_TYPE_TICK = IdTipoTicket;

                        if (Convert.ToString(Request.Params["cbTicketProblema"]) == "1")
                        {
                            cat.AplicaTicketProblema = true;
                        }
                        else
                        {
                            cat.AplicaTicketProblema = false;
                        }
                        if (Convert.ToString(Request.Params["cbxGestionActivos"]) == "1")
                        {
                            cat.AplicaGestionActivos = true;
                        }
                        else
                        {
                            cat.AplicaGestionActivos = false;
                        }

                        if (ID_ACCO == 56 || ID_ACCO == 57 || ID_ACCO == 58)
                        {
                            if (Convert.ToString(Request.Params["cbAplicaNotificacion"]) == "1")
                            {
                                cat.AplicaNotificacion = true;
                            }
                            else
                            {
                                cat.AplicaNotificacion = false;
                            }

                            if (Convert.ToString(Request.Params["cbTieneTarea"]) == "1")
                            {
                                cat.TieneTarea = true;
                            }
                            else
                            {
                                cat.TieneTarea = false;
                            }
                        }

                        if (Convert.ToString(Request.Params["cbxEstadoEdit"]) == "1")
                        {
                            estado = 1;
                        }
                        cat.VIG_CATE = Convert.ToBoolean(estado);
                        context.Entry(cat).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                }
                else if (ID_CAT_N5 == 0)
                {
                    //Añadir categoria 5
                    if (NOM_CATE_N5 != "" && ABR_N5 != "")
                    {
                        NivelPadre = 4;

                        int flagExiste = 0;
                        var catePadre = (from r in dbc.ACCOUNT_CATEGORY.Where(x => x.ID_ACCO == ID_ACCO)
                                         join categoria in dbc.CATEGORies on r.ID_CATE equals categoria.ID_CATE
                                         where categoria.NIV_CATE == 5 && categoria.ID_CATE_PARE == ID_CAT_N4 && categoria.NAM_CATE == NOM_CATE_N5
                                         select r).ToList();

                        flagExiste = catePadre.Count();

                        int UserId = Convert.ToInt32(Session["UserId"]);

                        if (flagExiste == 0)
                        {
                            var Categoria = new CATEGORY();
                            Categoria.NAM_CATE = NOM_CATE_N5;
                            Categoria.ACR_CATE = ABR_N5;
                            Categoria.ABR_CATE = ABR_N5;
                            Categoria.VIG_CATE = true;
                            Categoria.DEF_CATE = true;

                            Categoria.ID_CATE_PARE = ID_CAT_N4;

                            Categoria.NIV_CATE = 5;
                            Categoria.DATE_START = DateTime.Now;
                            Categoria.DATE_END = DateTime.Now;
                            Categoria.ACCO_USR = UserId;

                            Categoria.ID_TYPE_TICK = IdTipoTicket;

                            if (cbTipoCambioTicket != "" && cbTipoCambioTicket != "null")
                            {
                                Categoria.IdTipoGestionCambio = Convert.ToInt32(Request.Params["cbTipoCambioTicket"]);
                            }
                            else
                            {
                                Categoria.IdTipoGestionCambio = null;
                            }

                            if (Convert.ToString(Request.Params["cbTicketProblema"]) == "1")
                            {
                                Categoria.AplicaTicketProblema = true;
                            }
                            else
                            {
                                Categoria.AplicaTicketProblema = false;
                            }

                            if (Convert.ToString(Request.Params["cbxGestionActivos"]) == "1")
                            {
                                Categoria.AplicaGestionActivos = true;
                            }
                            else
                            {
                                Categoria.AplicaGestionActivos = false;
                            }

                            if (ID_ACCO == 56 || ID_ACCO == 57 || ID_ACCO == 58)
                            {
                                if (Convert.ToString(Request.Params["cbAplicaNotificacion"]) == "1")
                                {
                                    Categoria.AplicaNotificacion = true;
                                }
                                else
                                {
                                    Categoria.AplicaNotificacion = false;
                                }

                                if (Convert.ToString(Request.Params["cbTieneTarea"]) == "1")
                                {
                                    Categoria.TieneTarea = true;
                                }
                                else
                                {
                                    Categoria.TieneTarea = false;
                                }
                            }

                            if (Convert.ToString(Request.Params["cbxEstadoEdit"]) == "1")
                            {
                                estado = 1;
                            }
                            Categoria.VIG_CATE = Convert.ToBoolean(estado);
                            dbc.CATEGORies.Add(Categoria);
                            dbc.SaveChanges();

                            ACCOUNT_CATEGORY acc_categoria = new ACCOUNT_CATEGORY();
                            acc_categoria.ID_ACCO = ID_ACCO;
                            acc_categoria.ID_CATE = Categoria.ID_CATE;
                            acc_categoria.VIG_CATE = estado;
                            acc_categoria.VIG_ACCO_CATE = Categoria.VIG_CATE;
                            acc_categoria.IS_LEVE_TWO = true;
                            dbc.ACCOUNT_CATEGORY.Add(acc_categoria);
                            dbc.SaveChanges();

                            if (NOM_CATE_N6 != "" && ABR_N6 != "")
                            {
                                var Categoria6 = new CATEGORY();
                                Categoria6.NAM_CATE = NOM_CATE_N6;
                                Categoria6.ACR_CATE = ABR_N6;
                                Categoria6.ABR_CATE = ABR_N6;
                                Categoria6.VIG_CATE = true;
                                Categoria6.DEF_CATE = true;

                                Categoria6.ID_CATE_PARE = Categoria.ID_CATE;

                                Categoria6.NIV_CATE = 6;
                                Categoria6.DATE_START = DateTime.Now;
                                Categoria6.DATE_END = DateTime.Now;
                                Categoria6.ACCO_USR = UserId;

                                Categoria6.ID_TYPE_TICK = IdTipoTicket;

                                if (cbTipoCambioTicket != "" && cbTipoCambioTicket != "null")
                                {
                                    Categoria6.IdTipoGestionCambio = Convert.ToInt32(Request.Params["cbTipoCambioTicket"]);
                                }
                                else
                                {
                                    Categoria6.IdTipoGestionCambio = null;
                                }

                                if (Convert.ToString(Request.Params["cbTicketProblema"]) == "1")
                                {
                                    Categoria6.AplicaTicketProblema = true;
                                }
                                else
                                {
                                    Categoria6.AplicaTicketProblema = false;
                                }

                                if (Convert.ToString(Request.Params["cbxGestionActivos"]) == "1")
                                {
                                    Categoria6.AplicaGestionActivos = true;
                                }
                                else
                                {
                                    Categoria6.AplicaGestionActivos = false;
                                }

                                if (ID_ACCO == 56 || ID_ACCO == 57 || ID_ACCO == 58)
                                {
                                    if (Convert.ToString(Request.Params["cbAplicaNotificacion"]) == "1")
                                    {
                                        Categoria6.AplicaNotificacion = true;
                                    }
                                    else
                                    {
                                        Categoria6.AplicaNotificacion = false;
                                    }

                                    if (Convert.ToString(Request.Params["cbTieneTarea"]) == "1")
                                    {
                                        Categoria6.TieneTarea = true;
                                    }
                                    else
                                    {
                                        Categoria6.TieneTarea = false;
                                    }
                                }

                                if (Convert.ToString(Request.Params["cbxEstadoEdit"]) == "1")
                                {
                                    estado = 1;
                                }
                                Categoria6.VIG_CATE = Convert.ToBoolean(estado);
                                dbc.CATEGORies.Add(Categoria6);
                                dbc.SaveChanges();

                                ACCOUNT_CATEGORY acc_categoria6 = new ACCOUNT_CATEGORY();
                                acc_categoria6.ID_ACCO = ID_ACCO;
                                acc_categoria6.ID_CATE = Categoria6.ID_CATE;
                                acc_categoria6.VIG_CATE = estado;
                                acc_categoria6.VIG_ACCO_CATE = Categoria6.VIG_CATE;
                                acc_categoria6.IS_LEVE_TWO = true;
                                dbc.ACCOUNT_CATEGORY.Add(acc_categoria6);
                                dbc.SaveChanges();
                            }
                        }
                        else
                        {
                            return Content(NOM_CATE_N5);
                        }

                    }
                    else
                    {
                        cat = (from c in context.CATEGORies
                               where c.ID_CATE == ID_CAT_N4
                               select c).Single();

                        if (cat.NAM_CATE.ToLower() != NOM_CATE_N4.ToLower())
                        {
                            var catePadreN4 = (from r in dbc.ACCOUNT_CATEGORY.Where(x => x.ID_ACCO == ID_ACCO)
                                               join categoria in dbc.CATEGORies on r.ID_CATE equals categoria.ID_CATE
                                               where categoria.NIV_CATE == 4 && categoria.ID_CATE_PARE == ID_CAT_N3 && categoria.NAM_CATE == NOM_CATE_N4
                                               select r).ToList();
                            existe = catePadreN4.Count();
                            if (existe > 0)
                                return Content(NOM_CATE_N4);
                        }
                        cat.NAM_CATE = NOM_CATE_N4;
                        cat.ABR_CATE = ABR_N4;
                        cat.ACR_CATE = ABR_N4;
                        cat.DATE_END = DateTime.Now;
                        context.Entry(cat).State = EntityState.Modified;
                        context.SaveChanges();
                        //Añade modificaciones hasta el nivel 4
                        if (cbTipoCambioTicket != "" && cbTipoCambioTicket != "null")
                        {
                            cat.IdTipoGestionCambio = Convert.ToInt32(Request.Params["cbTipoCambioTicket"]);
                        }
                        else
                        {
                            cat.IdTipoGestionCambio = null;
                        }

                        cat.ID_TYPE_TICK = IdTipoTicket;

                        if (Convert.ToString(Request.Params["cbTicketProblema"]) == "1")
                        {
                            cat.AplicaTicketProblema = true;
                        }
                        else
                        {
                            cat.AplicaTicketProblema = false;
                        }
                        if (Convert.ToString(Request.Params["cbxGestionActivos"]) == "1")
                        {
                            cat.AplicaGestionActivos = true;
                        }
                        else
                        {
                            cat.AplicaGestionActivos = false;
                        }

                        if (ID_ACCO == 56 || ID_ACCO == 57 || ID_ACCO == 58)
                        {
                            if (Convert.ToString(Request.Params["cbAplicaNotificacion"]) == "1")
                            {
                                cat.AplicaNotificacion = true;
                            }
                            else
                            {
                                cat.AplicaNotificacion = false;
                            }

                            if (Convert.ToString(Request.Params["cbTieneTarea"]) == "1")
                            {
                                cat.TieneTarea = true;
                            }
                            else
                            {
                                cat.TieneTarea = false;
                            }
                        }

                        if (Convert.ToString(Request.Params["cbxEstadoEdit"]) == "1")
                        {
                            estado = 1;
                        }
                        cat.VIG_CATE = Convert.ToBoolean(estado);
                        context.Entry(cat).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                }
                else if (ID_CAT_N6 == 0)
                {
                    if (NOM_CATE_N6 != "" && ABR_N6 != "")
                    {
                        NivelPadre = 5;

                        int flagExiste = 0;
                        var catePadre = (from r in dbc.ACCOUNT_CATEGORY.Where(x => x.ID_ACCO == ID_ACCO)
                                         join categoria in dbc.CATEGORies on r.ID_CATE equals categoria.ID_CATE
                                         where categoria.NIV_CATE == 6 && categoria.ID_CATE_PARE == ID_CAT_N5 && categoria.NAM_CATE == NOM_CATE_N6
                                         select r).ToList();

                        flagExiste = catePadre.Count();

                        int UserId = Convert.ToInt32(Session["UserId"]);

                        if (flagExiste == 0)
                        {
                            var Categoria = new CATEGORY();
                            Categoria.NAM_CATE = NOM_CATE_N6;
                            Categoria.ACR_CATE = ABR_N6;
                            Categoria.ABR_CATE = ABR_N6;
                            Categoria.VIG_CATE = true;
                            Categoria.DEF_CATE = true;

                            Categoria.ID_CATE_PARE = ID_CAT_N5;

                            Categoria.NIV_CATE = 6;
                            Categoria.DATE_START = DateTime.Now;
                            Categoria.DATE_END = DateTime.Now;
                            Categoria.ACCO_USR = UserId;

                            Categoria.ID_TYPE_TICK = IdTipoTicket;

                            if (cbTipoCambioTicket != "" && cbTipoCambioTicket != "null")
                            {
                                Categoria.IdTipoGestionCambio = Convert.ToInt32(Request.Params["cbTipoCambioTicket"]);
                            }
                            else
                            {
                                Categoria.IdTipoGestionCambio = null;
                            }

                            if (Convert.ToString(Request.Params["cbTicketProblema"]) == "1")
                            {
                                Categoria.AplicaTicketProblema = true;
                            }
                            else
                            {
                                Categoria.AplicaTicketProblema = false;
                            }

                            if (Convert.ToString(Request.Params["cbxGestionActivos"]) == "1")
                            {
                                Categoria.AplicaGestionActivos = true;
                            }
                            else
                            {
                                Categoria.AplicaGestionActivos = false;
                            }

                            if (ID_ACCO == 56 || ID_ACCO == 57 || ID_ACCO == 58)
                            {
                                if (Convert.ToString(Request.Params["cbAplicaNotificacion"]) == "1")
                                {
                                    Categoria.AplicaNotificacion = true;
                                }
                                else
                                {
                                    Categoria.AplicaNotificacion = false;
                                }

                                if (Convert.ToString(Request.Params["cbTieneTarea"]) == "1")
                                {
                                    Categoria.TieneTarea = true;
                                }
                                else
                                {
                                    Categoria.TieneTarea = false;
                                }
                            }

                            if (Convert.ToString(Request.Params["cbxEstadoEdit"]) == "1")
                            {
                                estado = 1;
                            }
                            Categoria.VIG_CATE = Convert.ToBoolean(estado);
                            dbc.CATEGORies.Add(Categoria);
                            dbc.SaveChanges();

                            ACCOUNT_CATEGORY acc_categoria = new ACCOUNT_CATEGORY();
                            acc_categoria.ID_ACCO = ID_ACCO;
                            acc_categoria.ID_CATE = Categoria.ID_CATE;
                            acc_categoria.VIG_CATE = estado;
                            acc_categoria.VIG_ACCO_CATE = Categoria.VIG_CATE;
                            acc_categoria.IS_LEVE_TWO = true;
                            dbc.ACCOUNT_CATEGORY.Add(acc_categoria);
                            dbc.SaveChanges();
                        }
                        else
                        {
                            return Content(NOM_CATE_N6);
                        }

                    }
                    else
                    {
                        cat = (from c in context.CATEGORies
                               where c.ID_CATE == ID_CAT_N5
                               select c).Single();

                        if (cat.NAM_CATE.ToLower() != NOM_CATE_N5.ToLower())
                        {
                            var catePadreN5 = (from r in dbc.ACCOUNT_CATEGORY.Where(x => x.ID_ACCO == ID_ACCO)
                                               join categoria in dbc.CATEGORies on r.ID_CATE equals categoria.ID_CATE
                                               where categoria.NIV_CATE == 5 && categoria.ID_CATE_PARE == ID_CAT_N4 && categoria.NAM_CATE == NOM_CATE_N5
                                               select r).ToList();
                            existe = catePadreN5.Count();
                            if (existe > 0)
                                return Content(NOM_CATE_N5);
                        }
                        cat.NAM_CATE = NOM_CATE_N5;
                        cat.ABR_CATE = ABR_N5;
                        cat.ACR_CATE = ABR_N5;
                        cat.DATE_END = DateTime.Now;
                        context.Entry(cat).State = EntityState.Modified;
                        context.SaveChanges();

                        if (cbTipoCambioTicket != "" && cbTipoCambioTicket != "null")
                        {
                            cat.IdTipoGestionCambio = Convert.ToInt32(Request.Params["cbTipoCambioTicket"]);
                        }
                        else
                        {
                            cat.IdTipoGestionCambio = null;
                        }

                        cat.ID_TYPE_TICK = IdTipoTicket;

                        if (Convert.ToString(Request.Params["cbTicketProblema"]) == "1")
                        {
                            cat.AplicaTicketProblema = true;
                        }
                        else
                        {
                            cat.AplicaTicketProblema = false;
                        }
                        if (Convert.ToString(Request.Params["cbxGestionActivos"]) == "1")
                        {
                            cat.AplicaGestionActivos = true;
                        }
                        else
                        {
                            cat.AplicaGestionActivos = false;
                        }

                        if (ID_ACCO == 56 || ID_ACCO == 57 || ID_ACCO == 58)
                        {
                            if (Convert.ToString(Request.Params["cbAplicaNotificacion"]) == "1")
                            {
                                cat.AplicaNotificacion = true;
                            }
                            else
                            {
                                cat.AplicaNotificacion = false;
                            }

                            if (Convert.ToString(Request.Params["cbTieneTarea"]) == "1")
                            {
                                cat.TieneTarea = true;
                            }
                            else
                            {
                                cat.TieneTarea = false;
                            }
                        }

                        if (Convert.ToString(Request.Params["cbxEstadoEdit"]) == "1")
                        {
                            estado = 1;
                        }
                        cat.VIG_CATE = Convert.ToBoolean(estado);
                        context.Entry(cat).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                }
                else
                {
                    cat = (from c in context.CATEGORies
                           where c.ID_CATE == ID_CAT_N6
                           select c).Single();

                    if (cat.NAM_CATE.ToLower() != NOM_CATE_N6.ToLower())
                    {
                        var catePadreN6 = (from r in dbc.ACCOUNT_CATEGORY.Where(x => x.ID_ACCO == ID_ACCO)
                                           join categoria in dbc.CATEGORies on r.ID_CATE equals categoria.ID_CATE
                                           where categoria.NIV_CATE == 6 && categoria.ID_CATE_PARE == ID_CAT_N5 && categoria.NAM_CATE == NOM_CATE_N6
                                           select r).ToList();
                        existe = catePadreN6.Count();
                        if (existe > 0)
                            return Content(NOM_CATE_N6);
                    }
                    cat.NAM_CATE = NOM_CATE_N6;
                    cat.ABR_CATE = ABR_N6;
                    cat.ACR_CATE = ABR_N6;
                    cat.DATE_END = DateTime.Now;
                    context.Entry(cat).State = EntityState.Modified;
                    context.SaveChanges();

                    if (cbTipoCambioTicket != "" && cbTipoCambioTicket != "null")
                    {
                        cat.IdTipoGestionCambio = Convert.ToInt32(Request.Params["cbTipoCambioTicket"]);
                    }
                    else
                    {
                        cat.IdTipoGestionCambio = null;
                    }

                    cat.ID_TYPE_TICK = IdTipoTicket;

                    if (Convert.ToString(Request.Params["cbTicketProblema"]) == "1")
                    {
                        cat.AplicaTicketProblema = true;
                    }
                    else
                    {
                        cat.AplicaTicketProblema = false;
                    }
                    if (Convert.ToString(Request.Params["cbxGestionActivos"]) == "1")
                    {
                        cat.AplicaGestionActivos = true;
                    }
                    else
                    {
                        cat.AplicaGestionActivos = false;
                    }

                    if (ID_ACCO == 56 || ID_ACCO == 57 || ID_ACCO == 58)
                    {
                        if (Convert.ToString(Request.Params["cbAplicaNotificacion"]) == "1")
                        {
                            cat.AplicaNotificacion = true;
                        }
                        else
                        {
                            cat.AplicaNotificacion = false;
                        }

                        if (Convert.ToString(Request.Params["cbTieneTarea"]) == "1")
                        {
                            cat.TieneTarea = true;
                        }
                        else
                        {
                            cat.TieneTarea = false;
                        }
                    }

                    if (Convert.ToString(Request.Params["cbxEstadoEdit"]) == "1")
                    {
                        estado = 1;
                    }
                    cat.VIG_CATE = Convert.ToBoolean(estado);
                    context.Entry(cat).State = EntityState.Modified;
                    context.SaveChanges();
                }


                #region Coment
                //Cuarto nivel opcional
                //if (ID_CAT_N4 != 0)
                //{
                //    cat = (from c in context.CATEGORies
                //           where c.ID_CATE == ID_CAT_N4
                //           select c).Single();
                //    cat.NAM_CATE = NOM_CATE_N4;
                //    cat.ABR_CATE = ABR_N4;
                //    cat.ACR_CATE = ABR_N4;
                //    cat.DATE_END = DateTime.Now;
                //    context.Entry(cat).State = EntityState.Modified;
                //    context.SaveChanges();

                //    //Quinto nivel opcional
                //    if (ID_CAT_N5 != 0)
                //    {
                //        cat = (from c in context.CATEGORies
                //               where c.ID_CATE == ID_CAT_N5
                //               select c).Single();
                //        cat.NAM_CATE = NOM_CATE_N5;
                //        cat.ABR_CATE = ABR_N5;
                //        cat.ACR_CATE = ABR_N5;
                //        cat.DATE_END = DateTime.Now;
                //        context.Entry(cat).State = EntityState.Modified;
                //        context.SaveChanges();

                //        //Sexto nivel opcional
                //        if (ID_CAT_N6 != 0)
                //        {
                //            cat = (from c in context.CATEGORies
                //                   where c.ID_CATE == ID_CAT_N6
                //                   select c).Single();
                //            cat.NAM_CATE = NOM_CATE_N4;
                //            cat.ABR_CATE = ABR_N6;
                //            cat.ACR_CATE = ABR_N6;
                //            cat.DATE_END = DateTime.Now;
                //            context.Entry(cat).State = EntityState.Modified;
                //            context.SaveChanges();
                //            cat.ID_TYPE_TICK = IdTipoTicket;

                //            if (cbTipoCambioTicket != "" && cbTipoCambioTicket != "null")
                //            {
                //                cat.IdTipoGestionCambio = Convert.ToInt32(Request.Params["cbTipoCambioTicket"]);
                //            }
                //            else
                //            {
                //                cat.IdTipoGestionCambio = null;
                //            }
                //            cat.ID_TYPE_TICK = IdTipoTicket;
                //            //string q = Convert.ToString(Request.Params["cbTicketProblema"]);
                //            if (Convert.ToString(Request.Params["cbTicketProblema"]) == "1")
                //            {
                //                cat.AplicaTicketProblema = true;
                //            }
                //            else
                //            {
                //                cat.AplicaTicketProblema = false;
                //            }
                //            if (Convert.ToString(Request.Params["cbxGestionActivos"]) == "1")
                //            {
                //                cat.AplicaGestionActivos = true;
                //            }
                //            else
                //            {
                //                cat.AplicaGestionActivos = false;
                //            }
                //            if (Convert.ToString(Request.Params["cbxEstadoEdit"]) == "1")
                //            {
                //                estado = 1;
                //            }
                //            cat.VIG_CATE = Convert.ToBoolean(estado);
                //            context.Entry(cat).State = EntityState.Modified;
                //            context.SaveChanges();
                //        }
                //        else
                //        {
                //            //No hay categoria 6

                //            //Se requiere agregar una categoria 6
                //            int NivelCategoria = 6;

                //            if (ABR_N4.Length > 5)
                //            {
                //                return Content("ERROR ABREVIATURA DE LA CATEGORIA COMO MAX. ES 5");
                //            }

                //            int NivelCategoriaPadre = NivelCategoria - 1;

                //            //Verificamos que no exista la categoria
                //            var catePadre = (from r in dbc.ACCOUNT_CATEGORY.Where(x => x.ID_ACCO == ID_ACCO)
                //                             join categoria in dbc.CATEGORies on r.ID_CATE equals categoria.ID_CATE
                //                             where categoria.NIV_CATE == NivelCategoria && categoria.ID_CATE_PARE == ID_CAT_N5 && categoria.NAM_CATE == NOM_CATE_N4 && r.VIG_ACCO_CATE == true && r.VIG_CATE == 1
                //                             select r).ToList();
                //            int flagExiste = catePadre.Count();

                //            //Busacamos el idPadre en caso no exista referencia ID_CATE_N5
                //            if (NOM_CATE_N5 != "" && ABR_N5 != "")
                //            {
                //                var catePadreN5 = (from r in dbc.ACCOUNT_CATEGORY.Where(x => x.ID_ACCO == ID_ACCO)
                //                                   join categoria in dbc.CATEGORies on r.ID_CATE equals categoria.ID_CATE
                //                                   where categoria.NIV_CATE == 5 && categoria.NAM_CATE == NOM_CATE_N4 && r.VIG_ACCO_CATE == true && r.VIG_CATE == 1
                //                                   select r).ToList();
                //            }
                //            int flagNoCategoriaPadre = 0;


                //            int UserId = Convert.ToInt32(Session["UserId"]);

                //            if (flagExiste == 0)
                //            {
                //                var Categoria = new CATEGORY();
                //                Categoria.NAM_CATE = NOM_CATE_N4;
                //                Categoria.ACR_CATE = ABR_N4;
                //                Categoria.ABR_CATE = ABR_N4;
                //                Categoria.VIG_CATE = true;
                //                Categoria.DEF_CATE = true;
                //                Categoria.ID_CATE_PARE = ID_CAT_N3;
                //                Categoria.NIV_CATE = NivelCategoria;
                //                Categoria.DATE_START = DateTime.Now;
                //                Categoria.DATE_END = DateTime.Now;
                //                Categoria.ACCO_USR = UserId;

                //                Categoria.ID_TYPE_TICK = IdTipoTicket;

                //                if (cbTipoCambioTicket != "" && cbTipoCambioTicket != "null")
                //                {
                //                    Categoria.IdTipoGestionCambio = Convert.ToInt32(Request.Params["cbTipoCambioTicket"]);
                //                }

                //                if (Convert.ToString(Request.Params["cbTicketProblema"]) == "1")
                //                {
                //                    Categoria.AplicaTicketProblema = true;
                //                }
                //                else
                //                {
                //                    Categoria.AplicaTicketProblema = false;
                //                }
                //                if (Convert.ToString(Request.Params["cbxGestionActivos"]) == "1")
                //                {
                //                    Categoria.AplicaGestionActivos = true;
                //                }
                //                else
                //                {
                //                    Categoria.AplicaGestionActivos = false;
                //                }

                //                if (Convert.ToString(Request.Params["cbxEstadoEdit"]) == "1")
                //                {
                //                    estado = 1;
                //                }
                //                Categoria.VIG_CATE = Convert.ToBoolean(estado);

                //                dbc.CATEGORies.Add(Categoria);
                //                dbc.SaveChanges();

                //                ACCOUNT_CATEGORY acc_cat = new ACCOUNT_CATEGORY();
                //                acc_cat.ID_ACCO = ID_ACCO;
                //                acc_cat.ID_CATE = Categoria.ID_CATE;
                //                acc_cat.VIG_CATE = 1;
                //                acc_cat.VIG_ACCO_CATE = true;
                //                acc_cat.IS_LEVE_TWO = true;
                //                dbc.ACCOUNT_CATEGORY.Add(acc_cat);
                //                dbc.SaveChanges();
                //                //Se verifica los datos de la categoria 6
                //                if (cbTipoCambioTicket != "" && cbTipoCambioTicket != "null")
                //                {
                //                    cat.IdTipoGestionCambio = Convert.ToInt32(Request.Params["cbTipoCambioTicket"]);
                //                }
                //                else
                //                {
                //                    cat.IdTipoGestionCambio = null;
                //                }

                //                cat.ID_TYPE_TICK = IdTipoTicket;

                //                if (Convert.ToString(Request.Params["cbTicketProblema"]) == "1")
                //                {
                //                    cat.AplicaTicketProblema = true;
                //                }
                //                else
                //                {
                //                    cat.AplicaTicketProblema = false;
                //                }
                //                if (Convert.ToString(Request.Params["cbxGestionActivos"]) == "1")
                //                {
                //                    cat.AplicaGestionActivos = true;
                //                }
                //                else
                //                {
                //                    cat.AplicaGestionActivos = false;
                //                }

                //                if (Convert.ToString(Request.Params["cbxEstadoEdit"]) == "1")
                //                {
                //                    estado = 1;
                //                }
                //                cat.VIG_CATE = Convert.ToBoolean(estado);
                //                context.Entry(cat).State = EntityState.Modified;
                //                context.SaveChanges();

                //            }
                //        }
                //        else
                //        {
                //            if (NOM_CATE_N4 != "")
                //            {
                //                int NivelCategoria = 4;

                //                if (ABR_N4.Length > 5)
                //                {
                //                    return Content("ERROR ABREVIATURA DE LA CATEGORIA 4 COMO MAX. ES 5");
                //                }
                //                int NivelCategoriaPadre = NivelCategoria - 1;
                //                var catePadre = (from r in dbc.ACCOUNT_CATEGORY.Where(x => x.ID_ACCO == ID_ACCO)
                //                                 join categoria in dbc.CATEGORies on r.ID_CATE equals cat.ID_CATE
                //                                 where cat.NIV_CATE == NivelCategoria && cat.ID_CATE_PARE == ID_CAT_N3 && cat.NAM_CATE == NOM_CATE_N4 && r.VIG_ACCO_CATE == true && r.VIG_CATE == 1
                //                                 select r).ToList();

                //                int flag = catePadre.Count();
                //                int UserId = Convert.ToInt32(Session["UserId"]);

                //                if (flag == 0)
                //                {
                //                    var Categoria = new CATEGORY();
                //                    Categoria.NAM_CATE = NOM_CATE_N4;
                //                    Categoria.ACR_CATE = ABR_N4;
                //                    Categoria.ABR_CATE = ABR_N4;
                //                    Categoria.VIG_CATE = true;
                //                    Categoria.DEF_CATE = true;
                //                    Categoria.ID_CATE_PARE = ID_CAT_N3;
                //                    Categoria.NIV_CATE = NivelCategoria;
                //                    Categoria.DATE_START = DateTime.Now;
                //                    Categoria.DATE_END = DateTime.Now;
                //                    Categoria.ACCO_USR = UserId;

                //                    Categoria.ID_TYPE_TICK = IdTipoTicket;

                //                    if (cbTipoCambioTicket != "" && cbTipoCambioTicket != "null")
                //                    {
                //                        Categoria.IdTipoGestionCambio = Convert.ToInt32(Request.Params["cbTipoCambioTicket"]);
                //                    }

                //                    if (Convert.ToString(Request.Params["cbTicketProblema"]) == "1")
                //                    {
                //                        Categoria.AplicaTicketProblema = true;
                //                    }
                //                    else
                //                    {
                //                        Categoria.AplicaTicketProblema = false;
                //                    }
                //                    if (Convert.ToString(Request.Params["cbxGestionActivos"]) == "1")
                //                    {
                //                        Categoria.AplicaGestionActivos = true;
                //                    }
                //                    else
                //                    {
                //                        Categoria.AplicaGestionActivos = false;
                //                    }

                //                    if (Convert.ToString(Request.Params["cbxEstadoEdit"]) == "1")
                //                    {
                //                        estado = 1;
                //                    }
                //                    Categoria.VIG_CATE = Convert.ToBoolean(estado);

                //                    dbc.CATEGORies.Add(Categoria);
                //                    dbc.SaveChanges();

                //                    ACCOUNT_CATEGORY acc_cat = new ACCOUNT_CATEGORY();
                //                    acc_cat.ID_ACCO = ID_ACCO;
                //                    acc_cat.ID_CATE = Categoria.ID_CATE;
                //                    acc_cat.VIG_CATE = 1;
                //                    acc_cat.VIG_ACCO_CATE = true;
                //                    acc_cat.IS_LEVE_TWO = true;
                //                    dbc.ACCOUNT_CATEGORY.Add(acc_cat);
                //                    dbc.SaveChanges();
                //                }
                //                else
                //                {
                //                    return Content("ERROR");
                //                }
                //            }
                //            else
                //            {
                //                if (cbTipoCambioTicket != "" && cbTipoCambioTicket != "null")
                //                {
                //                    cat.IdTipoGestionCambio = Convert.ToInt32(Request.Params["cbTipoCambioTicket"]);
                //                }
                //                else
                //                {
                //                    cat.IdTipoGestionCambio = null;
                //                }

                //                cat.ID_TYPE_TICK = IdTipoTicket;

                //                if (Convert.ToString(Request.Params["cbTicketProblema"]) == "1")
                //                {
                //                    cat.AplicaTicketProblema = true;
                //                }
                //                else
                //                {
                //                    cat.AplicaTicketProblema = false;
                //                }
                //                if (Convert.ToString(Request.Params["cbxGestionActivos"]) == "1")
                //                {
                //                    cat.AplicaGestionActivos = true;
                //                }
                //                else
                //                {
                //                    cat.AplicaGestionActivos = false;
                //                }

                //                if (Convert.ToString(Request.Params["cbxEstadoEdit"]) == "1")
                //                {
                //                    estado = 1;
                //                }
                //                cat.VIG_CATE = Convert.ToBoolean(estado);
                //                context.Entry(cat).State = EntityState.Modified;
                //                context.SaveChanges();
                //            }
                //        }
                //    }
                //    else
                //    {
                //        if (NOM_CATE_N4 != "")
                //        {
                //            int NivelCategoria = 4;

                //            if (ABR_N4.Length > 5)
                //            {
                //                return Content("ERROR ABREVIATURA DE LA CATEGORIA 4 COMO MAX. ES 5");
                //            }
                //            int NivelCategoriaPadre = NivelCategoria - 1;
                //            var catePadre = (from r in dbc.ACCOUNT_CATEGORY.Where(x => x.ID_ACCO == ID_ACCO)
                //                             join categoria in dbc.CATEGORies on r.ID_CATE equals cat.ID_CATE
                //                             where cat.NIV_CATE == NivelCategoria && cat.ID_CATE_PARE == ID_CAT_N3 && cat.NAM_CATE == NOM_CATE_N4 && r.VIG_ACCO_CATE == true && r.VIG_CATE == 1
                //                             select r).ToList();

                //            int flag = catePadre.Count();
                //            int UserId = Convert.ToInt32(Session["UserId"]);

                //            if (flag == 0)
                //            {
                //                var Categoria = new CATEGORY();
                //                Categoria.NAM_CATE = NOM_CATE_N4;
                //                Categoria.ACR_CATE = ABR_N4;
                //                Categoria.ABR_CATE = ABR_N4;
                //                Categoria.VIG_CATE = true;
                //                Categoria.DEF_CATE = true;
                //                Categoria.ID_CATE_PARE = ID_CAT_N3;
                //                Categoria.NIV_CATE = NivelCategoria;
                //                Categoria.DATE_START = DateTime.Now;
                //                Categoria.DATE_END = DateTime.Now;
                //                Categoria.ACCO_USR = UserId;

                //                Categoria.ID_TYPE_TICK = IdTipoTicket;

                //                if (cbTipoCambioTicket != "" && cbTipoCambioTicket != "null")
                //                {
                //                    Categoria.IdTipoGestionCambio = Convert.ToInt32(Request.Params["cbTipoCambioTicket"]);
                //                }

                //                if (Convert.ToString(Request.Params["cbTicketProblema"]) == "1")
                //                {
                //                    Categoria.AplicaTicketProblema = true;
                //                }
                //                else
                //                {
                //                    Categoria.AplicaTicketProblema = false;
                //                }
                //                if (Convert.ToString(Request.Params["cbxGestionActivos"]) == "1")
                //                {
                //                    Categoria.AplicaGestionActivos = true;
                //                }
                //                else
                //                {
                //                    Categoria.AplicaGestionActivos = false;
                //                }

                //                if (Convert.ToString(Request.Params["cbxEstadoEdit"]) == "1")
                //                {
                //                    estado = 1;
                //                }
                //                Categoria.VIG_CATE = Convert.ToBoolean(estado);

                //                dbc.CATEGORies.Add(Categoria);
                //                dbc.SaveChanges();

                //                ACCOUNT_CATEGORY acc_cat = new ACCOUNT_CATEGORY();
                //                acc_cat.ID_ACCO = ID_ACCO;
                //                acc_cat.ID_CATE = Categoria.ID_CATE;
                //                acc_cat.VIG_CATE = 1;
                //                acc_cat.VIG_ACCO_CATE = true;
                //                acc_cat.IS_LEVE_TWO = true;
                //                dbc.ACCOUNT_CATEGORY.Add(acc_cat);
                //                dbc.SaveChanges();
                //            }
                //            else
                //            {
                //                return Content("ERROR");
                //            }
                //        }
                //        else
                //        {
                //            if (cbTipoCambioTicket != "" && cbTipoCambioTicket != "null")
                //            {
                //                cat.IdTipoGestionCambio = Convert.ToInt32(Request.Params["cbTipoCambioTicket"]);
                //            }
                //            else
                //            {
                //                cat.IdTipoGestionCambio = null;
                //            }

                //            cat.ID_TYPE_TICK = IdTipoTicket;

                //            if (Convert.ToString(Request.Params["cbTicketProblema"]) == "1")
                //            {
                //                cat.AplicaTicketProblema = true;
                //            }
                //            else
                //            {
                //                cat.AplicaTicketProblema = false;
                //            }
                //            if (Convert.ToString(Request.Params["cbxGestionActivos"]) == "1")
                //            {
                //                cat.AplicaGestionActivos = true;
                //            }
                //            else
                //            {
                //                cat.AplicaGestionActivos = false;
                //            }

                //            if (Convert.ToString(Request.Params["cbxEstadoEdit"]) == "1")
                //            {
                //                estado = 1;
                //            }
                //            cat.VIG_CATE = Convert.ToBoolean(estado);
                //            context.Entry(cat).State = EntityState.Modified;
                //            context.SaveChanges();
                //        }

                //    }

                //} 
                #endregion

                try
                {
                    ID_CAT_NFinal = ID_CAT_N6 != 0 ? ID_CAT_N6 : ID_CAT_N5 != 0 ? ID_CAT_N5 : ID_CAT_N4 != 0 ? ID_CAT_N4 : ID_CAT_N3;
                    //Edición en ACCOUNT_CATEGORY
                    var aCategory = dbc.ACCOUNT_CATEGORY.Where(x => x.ID_CATE == ID_CAT_NFinal && x.ID_ACCO == ID_ACCO).Single();
                    aCategory.VIG_CATE = estado;
                    aCategory.VIG_ACCO_CATE = Convert.ToBoolean(estado);
                    dbc.Entry(aCategory).State = EntityState.Modified;
                    dbc.SaveChanges();
                }
                catch (Exception e)
                {
                    return Content("ERROR");
                }
            }

            return Content("OK");
            //}
            //catch
            //{
            //    return Content("ERROR");
            //}
        }



        /*Usado para Lecciones Aprendidas*/
        public ActionResult ListCategoryLessonLearned(int ID_ACCO, int ID_CATE)
        {
            string i1 = string.Empty;
            i1 = "0";
            var query = dbc.ACCOUNT_CATEGORY
                        .Join(dbc.CATEGORies, ac => ac.ID_CATE, c => c.ID_CATE, (ac, c) => new { c.NAM_CATE, c.ID_CATE, c.ID_CATE_PARE, c.ABR_CATE, c.NIV_CATE, ac.ID_ACCO })
                        .Where(c => c.ID_ACCO == ID_ACCO);

            if (ID_CATE == 0)
            {
                query = query.Where(c => c.ID_CATE_PARE == null);
            }
            else
            {
                query = query.Where(c => c.ID_CATE_PARE == ID_CATE);
            }

            var result = (from c in query.ToList()
                          select new
                          {
                              NAM_CATE_1 = c.NAM_CATE,
                              c.ID_CATE,
                              NAM_CATE = c.NAM_CATE,
                              ABR_CATE = c.ABR_CATE,
                              ID_CATE_PARE = c.ID_CATE_PARE,
                              NIV_CATE = c.NIV_CATE,
                              ID_ACCO = c.ID_ACCO
                          });

            return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }
        /*
         public ActionResult ListCategoryLessonLearned(int ID_ACCO, int ID_CATE)
         {
             var temas = domainTema.CargarTemasByAccoCate(ID_ACCO, ID_CATE);
             return View();
         }
         */
        /**/

        //public ActionResult ListarPriority()
        //{
        //    var result = dbc.ListPrioritySLA().ToList();

        //    return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult ListCategory(int ID_ACCO = 0)
        {
            int ID_CATE = 0;

            string NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][0][field]"]), i1;

            if (!String.IsNullOrEmpty(NAM_PAR))
            {
                if (NAM_PAR == "ID_CATE")
                {
                    i1 = "0";
                }
                else
                {
                    i1 = "1";
                }

                ID_CATE = Convert.ToInt32(Request.QueryString[("filter[filters][" + i1 + "][value]")]);
            }
            try
            {
                List<catListarCategoria_Result> query = dbc.catListarCategoria(ID_ACCO, ID_CATE).ToList();

                return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult ListCategoryBnv()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int ID_CATE = 0;

            string NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][0][field]"]), i1;

            if (!String.IsNullOrEmpty(NAM_PAR))
            {
                if (NAM_PAR == "ID_CATE")
                {
                    i1 = "0";
                }
                else
                {
                    i1 = "1";
                }

                ID_CATE = Convert.ToInt32(Request.QueryString[("filter[filters][" + i1 + "][value]")]);
            }
            try
            {
                List<catListarCategoria_Result> query = dbc.catListarCategoria(ID_ACCO, ID_CATE).ToList();

                return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult ListaPorCategoryEditTicketBnv(int ID_ACCO = 0, int tipo = 0, int ID_CATE=0)
        {
            try
            {
                List<catListarCategoriaEditarBnv_Result> query = dbc.catListarCategoriaEditarBnv(ID_ACCO, ID_CATE, tipo).ToList();

                return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult ListCategory2(int ID_ACCO = 0)
        {
            int ID_CATE = 0;
            string NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][0][field]"]), i1, text = "";

            if (!String.IsNullOrEmpty(NAM_PAR))
            {
                if (NAM_PAR == "ID_CATE")
                {
                    i1 = "0";
                }
                else
                {
                    i1 = "1";
                    text = Convert.ToString(Request.QueryString[("filter[filters][0][value]")]);
                }

                ID_CATE = Convert.ToInt32(Request.QueryString[("filter[filters][" + i1 + "][value]")]);
            }
            try
            {
                List<catListarCategoria_Result> query = dbc.catListarCategoria(ID_ACCO, ID_CATE).ToList();
                var result = (from c in query.ToList()
                              select new
                              {
                                  ID_CATE2 = c.ID_CATE,
                                  NAM_CATE2 = c.NAM_CATE
                              }).Where(x => x.NAM_CATE2.ToLower().Contains(text.ToLower())).ToList();

                return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult ListCategory3(int ID_ACCO = 0)
      {
            int ID_CATE = 0;

            string NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][0][field]"]), i1, text = "";

            if (!String.IsNullOrEmpty(NAM_PAR))
            {
                if (NAM_PAR == "ID_CATE2")
                {
                    i1 = "0";
                }
                else
                {
                    i1 = "1";
                    text = Convert.ToString(Request.QueryString[("filter[filters][0][value]")]);
                }

                ID_CATE = Convert.ToInt32(Request.QueryString[("filter[filters][" + i1 + "][value]")]);
            }
            try
            {
                List<catListarCategoria_Result> query = dbc.catListarCategoria(ID_ACCO, ID_CATE).ToList();
                var result = (from c in query.ToList()
                              select new
                              {
                                  ID_CATE3 = c.ID_CATE,
                                  NAM_CATE3 = c.NAM_CATE
                              }).Where(x => x.NAM_CATE3.ToLower().Contains(text.ToLower())).ToList();

                return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult MostrarGarantiaBNV(int cat1, int cat2, int cat3, int cat4)
        {
            int val = 0;

            try
            {
                System.Diagnostics.Debug.WriteLine($"cat1: {cat1}, cat2: {cat2}, cat3: {cat3}, cat4: {cat4}");

                var result = dbc.ComparativaCategoryBNV(cat1, cat2, cat3, cat4).FirstOrDefault();

                if (result.HasValue)
                {
                    val = result.Value;
                }

                return Json(new { resultado = val }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }



        public ActionResult ListCategory4(int ID_ACCO = 0)
        {
            int ID_CATE = 0;

            string NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][0][field]"]), i1, text = "";

            if (!String.IsNullOrEmpty(NAM_PAR))
            {
                if (NAM_PAR == "ID_CATE3")
                {
                    i1 = "0";
                }
                else
                {
                    i1 = "1";
                    text = Convert.ToString(Request.QueryString[("filter[filters][0][value]")]);
                }

                ID_CATE = Convert.ToInt32(Request.QueryString[("filter[filters][" + i1 + "][value]")]);
            }
            try
            {
                List<catListarCategoria_Result> query = dbc.catListarCategoria(ID_ACCO, ID_CATE).ToList();
                var result = (from c in query.ToList()
                              select new
                              {
                                  ID_CATE4 = c.ID_CATE,
                                  NAM_CATE4 = c.NAM_CATE,
                                  ID_TYPE_TICK = c.ID_TYPE_TICK
                              }).Where(x => x.NAM_CATE4.ToLower().Contains(text.ToLower())).ToList();

                return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Listar Categoria 5
        public ActionResult ListCategory5(int ID_ACCO = 0)
        {
            int ID_CATE = 0;

            string NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][0][field]"]), i1, text = "";

            if (!String.IsNullOrEmpty(NAM_PAR))
            {
                if (NAM_PAR == "ID_CATE4")
                {
                    i1 = "0";
                }
                else
                {
                    i1 = "1";
                    text = Convert.ToString(Request.QueryString[("filter[filters][0][value]")]);
                }

                ID_CATE = Convert.ToInt32(Request.QueryString[("filter[filters][" + i1 + "][value]")]);
            }
            try
            {
                List<catListarCategoria_Result> query = dbc.catListarCategoria(ID_ACCO, ID_CATE).ToList();
                var result = (from c in query.ToList()
                              select new
                              {
                                  ID_CATE5 = c.ID_CATE,
                                  NAM_CATE5 = c.NAM_CATE,
                                  ID_TYPE_TICK = c.ID_TYPE_TICK
                              }).Where(x => x.NAM_CATE5.ToLower().Contains(text.ToLower())).ToList();

                return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Listar Categoria 6
        public ActionResult ListCategory6(int ID_ACCO = 0)
        {
            int ID_CATE = 0;

            string NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][0][field]"]), i1, text = "";

            if (!String.IsNullOrEmpty(NAM_PAR))
            {
                if (NAM_PAR == "ID_CATE5")
                {
                    i1 = "0";
                }
                else
                {
                    i1 = "1";
                    text = Convert.ToString(Request.QueryString[("filter[filters][0][value]")]);
                }

                ID_CATE = Convert.ToInt32(Request.QueryString[("filter[filters][" + i1 + "][value]")]);
            }
            try
            {
                List<catListarCategoria_Result> query = dbc.catListarCategoria(ID_ACCO, ID_CATE).ToList();
                var result = (from c in query.ToList()
                              select new
                              {
                                  ID_CATE6 = c.ID_CATE,
                                  NAM_CATE6 = c.NAM_CATE,
                                  ID_TYPE_TICK = c.ID_TYPE_TICK
                              }).Where(x => x.NAM_CATE6.ToLower().Contains(text.ToLower())).ToList();

                return Json(new { Data = result, Count = result.Count() }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult ListarCategorias(int id, string q, string page)
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
            try
            {
                var result = dbc.ListarCategoria(ID_ACCO, id, termino).ToList();

                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult ListarTodoCategoria(int ID_ACCO = 0)
        {
            int ID_CATE = 0;

            string NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][0][field]"]), i1;

            if (!String.IsNullOrEmpty(NAM_PAR))
            {
                if (NAM_PAR == "ID_CATE")
                {
                    i1 = "0";
                }
                else
                {
                    i1 = "1";
                }

                ID_CATE = Convert.ToInt32(Request.QueryString[("filter[filters][" + i1 + "][value]")]);
            }
            var query = dbc.ACCOUNT_CATEGORY
                        .Join(dbc.CATEGORies, ac => ac.ID_CATE, c => c.ID_CATE, (ac, c) => new { c.NAM_CATE, c.ID_CATE, c.ID_CATE_PARE, c.ABR_CATE, c.NIV_CATE, ac.ID_ACCO, ac.VIG_CATE, c.ID_TYPE_TICK })
                        .Where(c => c.ID_ACCO == ID_ACCO && c.VIG_CATE == 1);

            if (ID_CATE == 0)
            {
                query = query.Where(c => c.ID_CATE_PARE == null);
            }
            else
            {
                query = query.Where(c => c.ID_CATE_PARE == ID_CATE);
            }

            var result = (from c in query.ToList()
                          select new
                          {
                              NAM_CATE_1 = c.NAM_CATE,
                              c.ID_CATE,
                              NAM_CATE = c.NAM_CATE,
                              ABR_CATE = c.ABR_CATE,
                              ID_CATE_PARE = c.ID_CATE_PARE,
                              NIV_CATE = c.NIV_CATE,
                              ID_ACCO = c.ID_ACCO,
                              ID_TYPE_TICK = c.ID_TYPE_TICK == null ? 0 : c.ID_TYPE_TICK
                          }).OrderBy(x => x.NAM_CATE);

            return Json(new { Data = result, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuscarCategoria()
        {
            int count = 0, ID_ACCOUNT_CAT = 0, ID_CATE_N1 = 0, ID_CATE_N2 = 0, ID_CATE_N3 = 0, ID_CATE_N4 = 0, ID_CATE_N5 = 0, ID_CATE_N6 = 0;
            //  int skip = 0; // ya no se usara 
            //   int take =15;  // ya no se usara
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int Estado = 0;
            int tipoTicket = 0;

            string prueba = Request.Params["Estado"].ToString();
            string keyword = Convert.ToString(Request.Params["keyword"].ToString());

            using (var context = new CoreEntities())
            {
                if (Int32.TryParse(Convert.ToString(Request.Params["ID_ACCO_CAT"]), out ID_ACCOUNT_CAT) == false)
                {
                    ID_ACCOUNT_CAT = 0;
                }
                if (Int32.TryParse(Convert.ToString(Request.Params["ID_CAT_N1"]), out ID_CATE_N1) == false)
                {
                    ID_CATE_N1 = 0;
                }
                if (Int32.TryParse(Convert.ToString(Request.Params["ID_CAT_N2"]), out ID_CATE_N2) == false)
                {
                    ID_CATE_N2 = 0;
                }
                if (Int32.TryParse(Convert.ToString(Request.Params["ID_CAT_N3"]), out ID_CATE_N3) == false)
                {
                    ID_CATE_N3 = 0;
                }
                if (Int32.TryParse(Convert.ToString(Request.Params["ID_CAT_N4"]), out ID_CATE_N4) == false)
                {
                    ID_CATE_N4 = 0;
                }
                if (Int32.TryParse(Convert.ToString(Request.Params["ID_CAT_N5"]), out ID_CATE_N5) == false)
                {
                    ID_CATE_N5 = 0;
                }
                if (Int32.TryParse(Convert.ToString(Request.Params["ID_CAT_N6"]), out ID_CATE_N6) == false)
                {
                    ID_CATE_N6 = 0;
                }
                if (Request.Params["Estado"].ToString() == "")
                {
                    Estado = 0;
                }
                else
                {
                    Estado = Convert.ToInt32(Request.Params["Estado"]);
                }
                if (Request.Params["ID_TYPE_TICK"].ToString() == "")
                {
                    tipoTicket = 0;
                }
                else
                {
                    tipoTicket = Convert.ToInt32(Request.Params["ID_TYPE_TICK"]);
                }

                //List<ListaCategoriaBuscar_Result> resultado = dbc.ListaCategoriaBuscar(ID_ACCOUNT_CAT, ID_CATE_N1, ID_CATE_N2, ID_CATE_N3, ID_CATE_N4, ID_CATE_N5, ID_CATE_N6, keyword, Estado, tipoTicket).ToList();

                var resultado = dbc.Database.SqlQuery<CategoriaResult>(
                                    "exec [Categoria].[ListaCategoriaBuscar] @IdCuenta, @Categoria1, @Categoria2, @Categoria3, @Categoria4, @Categoria5, @Categoria6, @PalabraClave, @Estado, @IdTypeTicket",
                                    new SqlParameter("@IdCuenta", ID_ACCOUNT_CAT),
                                    new SqlParameter("@Categoria1", ID_CATE_N1),
                                    new SqlParameter("@Categoria2", ID_CATE_N2),
                                    new SqlParameter("@Categoria3", ID_CATE_N3),
                                    new SqlParameter("@Categoria4", ID_CATE_N4),
                                    new SqlParameter("@Categoria5", ID_CATE_N5),
                                    new SqlParameter("@Categoria6", ID_CATE_N6),
                                    new SqlParameter("@PalabraClave", keyword),
                                    new SqlParameter("@Estado", Estado),
                                    new SqlParameter("@IdTypeTicket", tipoTicket)
                                ).ToList();

                //List<categ>

                count = resultado.Count();

                // var lista = resultado.Skip(skip).Take(count); // no es necesario

                var json = Json(new { Data = resultado }, JsonRequestBehavior.AllowGet);
                json.MaxJsonLength = int.MaxValue;
                return json;
                //if (Int32.TryParse(Convert.ToString(Request.Params["ID_ACCO_CAT"]), out ID_ACCOUNT_CAT) == true)
                //{
                //    var resultfind = (from ac in context.ACCOUNT_CATEGORY.ToList()
                //                        join c in context.CATEGORies on ac.ID_CATE equals c.ID_CATE
                //                        join a in context.ACCOUNTs on ac.ID_ACCO equals a.ID_ACCO
                //                        join ce in dbe.CLASS_ENTITY on c.ACCO_USR equals ce.UserId into ce_a
                //                        from xf in ce_a.DefaultIfEmpty()
                //                        where ac.VIG_CATE == 1 && ac.ID_ACCO == ID_ACCOUNT_CAT
                //                        select new
                //                        {
                //                            ID_CATE = ac.ID_CATE,
                //                            NAM_CATE = c.NAM_CATE,
                //                            ABR_CATE = c.ABR_CATE,
                //                            NIV_CATE = c.NIV_CATE,
                //                            ID_CATE_PARE = c.ID_CATE_PARE,
                //                            ID_ACCO = a.ID_ACCO,
                //                            NAM_ACCO = a.NAM_ACCO,
                //                            VIG_CATE = (ac.VIG_CATE == 1 ? "Activo" : "Inactivo"),
                //                            VIG_ACCO_CATE = ac.VIG_ACCO_CATE,
                //                            DATE_START = (c.DATE_START == null ? "" : string.Format("{0:dddd dd MMMM yyyy}", c.DATE_START)),
                //                            DATE_END = (c.DATE_END == null ? "" : string.Format("{0:dddd dd MMMM yyyy}", c.DATE_END)),
                //                            ACCO_USR = (xf == null ? string.Empty : xf.FIR_NAME + " " + xf.LAS_NAME)
                //                        }).OrderByDescending(x => x.ID_CATE).ToList();

                //    if (Int32.TryParse(Convert.ToString(Request.Params["ID_CAT_N1"]), out ID_CATE_N1) == true)
                //    {
                //        resultfind = (from r in resultfind
                //                      join cat in context.CATEGORies on r.ID_CATE equals cat.ID_CATE
                //                      where cat.ID_CATE_PARE == ID_CATE_N1
                //                      select r).ToList();
                //    }
                //    if (Int32.TryParse(Convert.ToString(Request.Params["ID_CAT_N2"]), out ID_CATE_N2) == true)
                //    {
                //        resultfind = resultfind.Where(x => x.ID_CATE_PARE == ID_CATE_N2 && x.ID_ACCO == ID_ACCOUNT_CAT).ToList();
                //    }
                //    if (Int32.TryParse(Convert.ToString(Request.Params["ID_CAT_N3"]), out ID_CATE_N3) == true)
                //    {
                //        resultfind = resultfind.Where(x => x.ID_CATE_PARE == ID_CATE_N3 && x.ID_ACCO == ID_ACCOUNT_CAT).ToList();
                //    }
                //    if (Int32.TryParse(Convert.ToString(Request.Params["ID_CAT_N4"]), out ID_CATE_N4) == true)
                //    {
                //        resultfind = resultfind.Where(x => x.ID_CATE == ID_CATE_N4 && x.ID_ACCO == ID_ACCOUNT_CAT).ToList();
                //    }
                //    if (!String.IsNullOrEmpty(keyword))
                //    {
                //        resultfind = resultfind.Where(x => x.NAM_CATE.ToLower().Contains(keyword.ToLower()) && x.ID_ACCO == ID_ACCOUNT_CAT).ToList();
                //    }
                //    var resultado = resultfind.Skip(skip).Take(take).ToList().OrderBy(x => x.NAM_CATE);
                //    count = resultfind.Count();
                //    return Json(new { Data = resultado, Count = count }, JsonRequestBehavior.AllowGet);
                //}
                //else
                //{
                //    var resultfind = (from ac in context.ACCOUNT_CATEGORY.ToList()
                //                      join c in context.CATEGORies on ac.ID_CATE equals c.ID_CATE
                //                      join a in context.ACCOUNTs on ac.ID_ACCO equals a.ID_ACCO
                //                      join ce in dbe.CLASS_ENTITY on c.ACCO_USR equals ce.UserId into ce_a
                //                      from xf in ce_a.DefaultIfEmpty()
                //                      where ac.VIG_CATE == 1 && ac.ID_ACCO==ID_ACCO
                //                      select new
                //                      {
                //                          ID_CATE = ac.ID_CATE,
                //                          NAM_CATE = c.NAM_CATE,
                //                          ABR_CATE = c.ABR_CATE,
                //                          NIV_CATE = c.NIV_CATE,
                //                          ID_CATE_PARE = c.ID_CATE_PARE,
                //                          ID_ACCO = a.ID_ACCO,
                //                          NAM_ACCO = a.NAM_ACCO,
                //                          VIG_CATE = (ac.VIG_CATE == 1 ? "Activo" : "Inactivo"),
                //                          VIG_ACCO_CATE = ac.VIG_ACCO_CATE,
                //                          DATE_START = (c.DATE_START == null ? "" : string.Format("{0:dddd dd MMMM yyyy}", c.DATE_START)),
                //                          DATE_END = (c.DATE_END == null ? "" : string.Format("{0:dddd dd MMMM yyyy}", c.DATE_END)),
                //                          ACCO_USR = (xf == null ? string.Empty : xf.FIR_NAME + " " + xf.LAS_NAME)
                //                      }).OrderByDescending(x => x.ID_CATE).ToList();
                //    if (Int32.TryParse(Convert.ToString(Request.Params["ID_CAT_N1"]), out ID_CATE_N1) == false)
                //    {
                //        resultfind = (from r in resultfind
                //                      join cat in context.CATEGORies on r.ID_CATE equals cat.ID_CATE
                //                      where cat.ID_CATE_PARE == ID_CATE_N1
                //                      select r).ToList();
                //    }
                //    if (Int32.TryParse(Convert.ToString(Request.Params["ID_CAT_N2"]), out ID_CATE_N2) == false)
                //    {
                //        resultfind = resultfind.Where(x => x.ID_CATE_PARE == ID_CATE_N2 && x.ID_ACCO == ID_ACCO).ToList();
                //    }
                //    if (Int32.TryParse(Convert.ToString(Request.Params["ID_CAT_N3"]), out ID_CATE_N3) == false)
                //    {
                //        resultfind = resultfind.Where(x => x.ID_CATE_PARE == ID_CATE_N3 && x.ID_ACCO == ID_ACCO).ToList();
                //    }
                //    if (Int32.TryParse(Convert.ToString(Request.Params["ID_CAT_N4"]), out ID_CATE_N4) == false)
                //    {
                //        resultfind = resultfind.Where(x => x.ID_CATE == ID_CATE_N4 && x.ID_ACCO == ID_ACCO).ToList();
                //    }
                //    if (!String.IsNullOrEmpty(keyword))
                //    {
                //        resultfind = resultfind.Where(x => x.NAM_CATE.ToLower().Contains(keyword.ToLower()) && x.ID_ACCO == ID_ACCO).ToList();
                //    }
                //var resultado = resultfind.Skip(skip).Take(take).ToList().OrderBy(x => x.NAM_CATE);
                //count = resultfind.Count();
                //return Json(new { Data = resultado, Count = count }, JsonRequestBehavior.AllowGet);                   
                //}
            }
        }

        public ActionResult ExportarCategoria()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int ID_ACCOUNT_CAT = 0, ID_CATE_N1 = 0, ID_CATE_N2 = 0, ID_CATE_N3 = 0, ID_CATE_N4 = 0, ID_CATE_N5 = 0, ID_CATE_N6 = 0;
            string keyword = Convert.ToString(Request.Params["keyword"].ToString());
            int Estado = 0;
            int tipoTicket = 0;
            if (Request.Params["ID_ACCO_CAT"] != "0")
            {
                if (Int32.TryParse(Convert.ToString(Request.Params["ID_ACCO_CAT"]), out ID_ACCOUNT_CAT) == false)
                {
                    ID_ACCOUNT_CAT = 0;
                }
            }
            //if (Request.Params["ID_CATE_N1"] != "" || Request.Params["ID_CATE_N1"] == "0")
            //    ID_CATE_N1 = Convert.ToInt32(Request.Params["ID_CATE_N1"]);
            //if (Request.Params["ID_CATE_N2"] != "" || Request.Params["ID_CATE_N2"] == "0")
            //    ID_CATE_N2 = Convert.ToInt32(Request.Params["ID_CATE_N2"]);
            //if (Request.Params["ID_CATE_N3"] != "" || Request.Params["ID_CATE_N3"] == "0")
            //    ID_CATE_N3 = Convert.ToInt32(Request.Params["ID_CATE_N3"]);
            //if (Request.Params["ID_CATE_N4"] != "" || Request.Params["ID_CATE_N4"] == "0")
            //ID_CATE_N4 = Convert.ToInt32(Request.Params["ID_CATE_N4"]);
            if (Int32.TryParse(Convert.ToString(Request.Params["ID_CAT_N1"]), out ID_CATE_N1) == false) { ID_CATE_N1 = 0; }
            if (Int32.TryParse(Convert.ToString(Request.Params["ID_CAT_N2"]), out ID_CATE_N2) == false) { ID_CATE_N2 = 0; }
            if (Int32.TryParse(Convert.ToString(Request.Params["ID_CAT_N3"]), out ID_CATE_N3) == false) { ID_CATE_N3 = 0; }
            if (Int32.TryParse(Convert.ToString(Request.Params["ID_CAT_N4"]), out ID_CATE_N4) == false) { ID_CATE_N4 = 0; }
            if (Int32.TryParse(Convert.ToString(Request.Params["ID_CAT_N5"]), out ID_CATE_N5) == false) { ID_CATE_N5 = 0; }
            if (Int32.TryParse(Convert.ToString(Request.Params["ID_CAT_N6"]), out ID_CATE_N6) == false) { ID_CATE_N6 = 0; }
            if (Request.Params["Estado"].ToString() == "")
            {
                Estado = 0;
            }
            else
            {
                Estado = Convert.ToInt32(Request.Params["Estado"]);
            }
            if (Request.Params["ID_TYPE_TICK"].ToString() == "")
            {
                tipoTicket = 0;
            }
            else
            {
                tipoTicket = Convert.ToInt32(Request.Params["ID_TYPE_TICK"]);
            }
            var nomCuenta = dbc.ACCOUNTs.FirstOrDefault(x => x.ID_ACCO == ID_ACCOUNT_CAT).NAM_ACCO;
            using (var context = new CoreEntities())
            {
                //List<ExportarCategoria_Result> resultado = context.ExportarCategoria(ID_ACCOUNT_CAT, ID_CATE_N1, ID_CATE_N2, ID_CATE_N3, ID_CATE_N4).ToList();
                //List<ListaCategoriaBuscar_Result> resultado = dbc.ListaCategoriaBuscar(ID_ACCOUNT_CAT, ID_CATE_N1, ID_CATE_N2, ID_CATE_N3, ID_CATE_N4, ID_CATE_N5, ID_CATE_N6, keyword, Estado, tipoTicket).ToList();
                var resultado = dbc.Database.SqlQuery<CategoriaResult>(
                                    "exec [Categoria].[ListaCategoriaBuscar] @IdCuenta, @Categoria1, @Categoria2, @Categoria3, @Categoria4, @Categoria5, @Categoria6, @PalabraClave, @Estado, @IdTypeTicket",
                                    new SqlParameter("@IdCuenta", ID_ACCOUNT_CAT),
                                    new SqlParameter("@Categoria1", ID_CATE_N1),
                                    new SqlParameter("@Categoria2", ID_CATE_N2),
                                    new SqlParameter("@Categoria3", ID_CATE_N3),
                                    new SqlParameter("@Categoria4", ID_CATE_N4),
                                    new SqlParameter("@Categoria5", ID_CATE_N5),
                                    new SqlParameter("@Categoria6", ID_CATE_N6),
                                    new SqlParameter("@PalabraClave", keyword),
                                    new SqlParameter("@Estado", Estado),
                                    new SqlParameter("@IdTypeTicket", tipoTicket)
                                ).ToList();

                StringBuilder sb = new StringBuilder();
                int i = 0;
                sb.Append("<meta charset='utf-8'>");
                sb.Append("<style type='text/css'>");
                sb.Append(".tg  {border-collapse:collapse;border-spacing:0;}");
                sb.Append(".tg td{font-family:Arial, sans-serif;font-size:14px;padding:10px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;color:#444;background-color:#F7FDFA;border-top-width:1px;border-bottom-width:1px;}");
                sb.Append(".tg th{font-family:Arial, sans-serif;font-size:14px;font-weight:normal;padding:10px 5px;border-style:solid;border-width:0px;overflow:hidden;word-break:normal;border-color:#999;color:#fff;background-color:#26ADE4;border-top-width:1px;border-bottom-width:1px;}");
                sb.Append("</style>");
                sb.Append("<table class='tg'>");
                sb.Append("<tr>");
                sb.Append("<th class='tg-031e'>CATEGORIA 1</td>");
                sb.Append("<th class='tg-031e'>ABR CATE 1</td>");
                sb.Append("<th class='tg-031e'>CATEGORIA 2</td>");
                sb.Append("<th class='tg-031e'>ABR CATE 2</td>");
                sb.Append("<th class='tg-031e'>CATEGORIA 3</td>");
                sb.Append("<th class='tg-031e'>ABR CATE 3</td>");
                sb.Append("<th class='tg-031e'>CATEGORIA 4</td>");
                sb.Append("<th class='tg-031e'>ABR CATE 4</td>");
                sb.Append("<th class='tg-031e'>CATEGORIA 5</td>");
                sb.Append("<th class='tg-031e'>ABR CATE 5</td>");
                sb.Append("<th class='tg-031e'>CATEGORIA 6</td>");
                sb.Append("<th class='tg-031e'>ABR CATE 6</td>");
                sb.Append("<th class='tg-031e'>TIPO TICKET</td>");
                sb.Append("<th class='tg-031e'>CUENTA</td>");
                sb.Append("<th class='tg-031e'>TIPO DE GESTIÓN DE CAMBIO (TICKET)</td>");
                sb.Append("<th class='tg-031e'>GESTIÓN DE ACTIVOS</td>");
                sb.Append("<th class='tg-031e'>TICKET PROBLEMA</td>");
                sb.Append("<th class='tg-031e'>APLICA NOTIFICACIÓN</td>");
                sb.Append("<th class='tg-031e'>TIENE TAREA</td>");
                sb.Append("<th class='tg-031e'>ESTADO</td>");
                sb.Append("</tr>");

                foreach (var dr in resultado)
                {
                    sb.Append("<tr>");
                    sb.Append("<td class='tg-031e'>" + dr.Categoria1 + "</td>");
                    sb.Append("<td class='tg-031e'>" + dr.AbreCategoria1 + "</td>");
                    sb.Append("<td class='tg-031e'>" + dr.Categoria2 + "</td>");
                    sb.Append("<td class='tg-031e'>" + dr.AbreCategoria2 + "</td>");
                    sb.Append("<td class='tg-031e'>" + dr.Categoria3 + "</td>");
                    sb.Append("<td class='tg-031e'>" + dr.AbreCategoria3 + "</td>");
                    sb.Append("<td class='tg-031e'>" + dr.Categoria4 + "</td>");
                    sb.Append("<td class='tg-031e'>" + dr.AbreCategoria4 + "</td>");
                    sb.Append("<td class='tg-031e'>" + dr.Categoria5 + "</td>");
                    sb.Append("<td class='tg-031e'>" + dr.AbreCategoria5 + "</td>");
                    sb.Append("<td class='tg-031e'>" + dr.Categoria6 + "</td>");
                    sb.Append("<td class='tg-031e'>" + dr.AbreCategoria6 + "</td>");
                    sb.Append("<td class='tg-031e'>" + dr.TipoTicket + "</td>");
                    sb.Append("<td class='tg-031e'>" + dr.Cuenta + "</td>");
                    sb.Append("<td class='tg-031e'>" + dr.TipoGestionCambio + "</td>");
                    sb.Append("<td class='tg-031e'>" + dr.GestionActivos + "</td>");
                    sb.Append("<td class='tg-031e'>" + dr.TicketProblema + "</td>");
                    sb.Append("<td class='tg-031e'>" + dr.AplicaNotificacion + "</td>");
                    sb.Append("<td class='tg-031e'>" + dr.TieneTarea + "</td>");
                    sb.Append("<td class='tg-031e'>" + dr.Estado + "</td>");
                    sb.Append("</tr>");
                }
                sb.Append("</table>");

                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/vnd.ms-excel";
                Response.Charset = "";
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                Response.AddHeader("Content-Disposition", "attachment;filename=Categorias-" + nomCuenta + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
                Response.Write(sb.ToString());
                Response.Flush();
                Response.End();

                return View();
            }
        }
        public ActionResult FindCategory()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            int count = 0;
            int skip = Convert.ToInt32(Request.Params["skip"].ToString());
            int take = Convert.ToInt32(Request.Params["take"].ToString());
            var context = new CoreEntities();

            var listCategory = (from a in context.CATEGORies.ToList()
                                join ac in context.ACCOUNT_CATEGORY on a.ID_CATE equals ac.ID_CATE
                                join c in context.ACCOUNTs on ac.ID_ACCO equals c.ID_ACCO
                                join ce in dbe.CLASS_ENTITY on a.ACCO_USR equals ce.UserId into ce_a
                                from xf in ce_a.DefaultIfEmpty()
                                where ac.VIG_CATE == 1 && ac.VIG_ACCO_CATE == true && ac.ID_ACCO == ID_ACCO
                                orderby a.DATE_START descending
                                select new
                                {
                                    ID_CATE = ac.ID_CATE,
                                    NAM_CATE = a.NAM_CATE,
                                    ABR_CATE = (a.ABR_CATE == "" ? "-" : a.ABR_CATE),
                                    NIV_CATE = a.NIV_CATE,
                                    ID_CATE_PARE = a.ID_CATE_PARE,
                                    //NOM_CATE_PARE = (from cat in context.CATEGORies.Where(x => x.ID_CATE == a.ID_CATE_PARE) select new { NOM_CATE_PARE = cat.NAM_CATE}).ToList().ToString(),
                                    ID_ACCO = c.ID_ACCO,
                                    NAM_ACCO = c.NAM_ACCO,
                                    VIG_CATE = (ac.VIG_CATE == 1 ? "Activo" : "Inactivo"),
                                    VIG_ACCO_CATE = ac.VIG_ACCO_CATE,
                                    DATE_START = (a.DATE_START == null ? "-" : string.Format("{0:yyyy-MM-dd}", a.DATE_START)),
                                    DATE_END = (a.DATE_END == null ? "-" : string.Format("{0:dddd dd MMMM yyyy}", a.DATE_END)),
                                    ACCO_USR = (xf == null ? "-" : xf.FIR_NAME + " " + xf.LAS_NAME)
                                }).OrderBy(x => x.NAM_CATE);
            var resultfind = listCategory.ToList();
            count = listCategory.Count();
            var resultado = resultfind.Skip(skip).Take(take).ToList();
            return Json(new { Data = resultado, Count = count }, JsonRequestBehavior.AllowGet);


        }
        public ActionResult FindRoles()
        {
            return View();
        }
        public ActionResult ExportarRoles()
        {
            int ID_ROLE = 0;
            int ID_ENTI = 0;
            using (var context = new EntityEntities())
            {
                var query1 = (from wr in context.webpages_Roles
                              select new
                              {
                                  ROLE_ID = wr.RoleId,
                                  ROLE_NAME = wr.RoleName,
                              });
                if (Int32.TryParse(Convert.ToString(Request.Params["RoleID"]), out ID_ROLE) == true)
                {
                    query1 = query1.Where(x => x.ROLE_ID == ID_ROLE);
                }
                var query2 = (from wui in context.webpages_UsersInRoles.ToList()
                              join wr in query1 on wui.RoleId equals wr.ROLE_ID
                              join ce in context.CLASS_ENTITY on wui.UserId equals ce.UserId
                              join pe in context.PERSON_ENTITY on ce.ID_ENTI equals pe.ID_ENTI2
                              join area in context.AREAs on pe.ID_AREA equals area.ID_AREA
                              where ce.VIG_ENTI == true && pe.ID_PERS_STAT == 1
                              select new
                              {
                                  UserId = wui.UserId,
                                  ROLE_ID = wr.ROLE_ID,
                                  ROLE_NAME = wr.ROLE_NAME,
                                  ID_FOTO = Convert.ToString(pe.ID_FOTO),
                                  JOB_TITLE = pe.CAR_PERS == null ? "" : pe.CAR_PERS,
                                  NAM_PERSON = ce.FIR_NAME.ToLower() + " " + ce.LAS_NAME.ToLower() +
                                      (ce.MOT_NAME == null ? "" : " " + ce.MOT_NAME.ToUpper().Substring(0, 1) + "."),
                                  NOM_AREA = area.NOM_AREA,
                                  ID_PERS_ENTI = pe.ID_PERS_ENTI
                              });

                if (Int32.TryParse(Convert.ToString(Request.Params["ID_ENTI"]), out ID_ENTI) == true)
                {
                    query2 = query2.Where(x => x.ID_PERS_ENTI == ID_ENTI);
                }
                var resultfind = query2.ToList();
                var grid = new GridView();
                grid.DataSource = resultfind;
                grid.DataBind();

                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=FindRoles" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
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
        public ActionResult FindRolxStaff()
        {
            int ID_ROLE = 0;
            int ID_ENTI = 0;
            int ID_QUEU_FIND = 0;
            using (var context = new EntityEntities())
            {
                var query1 = (from wr in context.webpages_Roles
                              select new
                              {
                                  ROLE_ID = wr.RoleId,
                                  ROLE_NAME = wr.RoleName,
                              });
                if (Int32.TryParse(Convert.ToString(Request.Params["RoleID"]), out ID_ROLE) == true)
                {
                    query1 = query1.Where(x => x.ROLE_ID == ID_ROLE);
                }
                var query2 = (from wui in context.webpages_UsersInRoles.ToList()
                              join wr in query1 on wui.RoleId equals wr.ROLE_ID
                              join ce in context.CLASS_ENTITY on wui.UserId equals ce.UserId
                              join pe in context.PERSON_ENTITY on ce.ID_ENTI equals pe.ID_ENTI2
                              join area in context.AREAs on pe.ID_AREA equals area.ID_AREA
                              where ce.VIG_ENTI == true && pe.ID_PERS_STAT == 1
                              select new
                              {
                                  UserId = wui.UserId,
                                  ROLE_ID = wr.ROLE_ID,
                                  ROLE_NAME = wr.ROLE_NAME,
                                  ID_FOTO = Convert.ToString(pe.ID_FOTO),
                                  JOB_TITLE = pe.CAR_PERS == null ? "" : pe.CAR_PERS,
                                  NAM_PERSON = ce.FIR_NAME.ToUpper() + " " + ce.LAS_NAME.ToUpper() +
                                      (ce.MOT_NAME == null ? "" : " " + ce.MOT_NAME.ToUpper().Substring(0, 1) + "."),
                                  ID_AREA = area.ID_AREA,
                                  ID_AREA_PARENT = area.ID_AREA_PARENT,
                                  NOM_AREA = area.NOM_AREA,
                                  ID_PERS_ENTI = pe.ID_PERS_ENTI
                              });
                if (Int32.TryParse(Convert.ToString(Request.Params["ID_QUEU_FIND"]), out ID_QUEU_FIND) == true)
                {
                    query2 = query2.Where(x => x.ID_AREA == ID_QUEU_FIND);
                }

                if (Int32.TryParse(Convert.ToString(Request.Params["ID_PERS_ENTI_ASSI_FIND"]), out ID_ENTI) == true)
                {
                    query2 = query2.Where(x => x.ID_PERS_ENTI == ID_ENTI);
                }

                query2 = query2.OrderBy(x => x.UserId).ThenBy(x => x.ROLE_ID).ThenBy(x => x.ROLE_NAME);
                var query3 = (from q2 in query2
                              select new
                              {
                                  q2.UserId,
                              }).Distinct();
                int tt = query3.Count();
                int skip = Convert.ToInt32(Request.Params["skip"].ToString());
                int take = Convert.ToInt32(Request.Params["take"].ToString());

                query2 = (from q3 in query3.Skip(skip).Take(take).ToList()
                          join q2 in query2 on q3.UserId equals q2.UserId
                          select q2).ToList();

                return Json(new { Data = query2, Count = tt }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public ActionResult ManagingRoles()
        {
            bool EDIT_ROLES = false;
            string[] rolesArray = Roles.GetRolesForUser(Convert.ToString(Session["UserName"]));
            foreach (string rol in rolesArray)
            {
                if (rol == "ADMINISTRADOR")
                {
                    EDIT_ROLES = true; //permitiendo al acceso
                }
            }
            if (EDIT_ROLES == true)
            {
                return View();
            }
            else
            {
                return Content("<h2><b>You are not authorized for this section</b></h2>");
            }

        }

        public ActionResult ListStaff()
        {
            var query = (from ce in dbe.CLASS_ENTITY.Where(x => x.ID_ENTI == 9)
                         join pe in dbe.PERSON_ENTITY on ce.ID_ENTI equals pe.ID_ENTI2
                         join pc in dbe.PERSON_CONTRACT on pe.ID_PERS_ENTI equals pc.ID_PERS_ENTI
                         where pc.VIG_CONT == true && pc.LAS_CONT == true
                         select new
                         {
                             ID_PERS_ENTI = pe.ID_PERS_ENTI,
                             ID_ENTI = ce.ID_ENTI,
                             FIR_NAME = ce.FIR_NAME.ToUpper() + " " + ce.LAS_NAME.ToUpper(),
                             ID_FOTO = pe.ID_FOTO,
                         });

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListRoles()
        {
            var query = (from r in dbe.webpages_Roles
                         select new
                         {
                             ROLE_ID = r.RoleId,
                             ROLE_NAME = r.RoleName,
                         });

            return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListAreas()
        {
            int ID_ACCO = Convert.ToInt32(Session["ID_ACCO"]);
            using (var context = new EntityEntities())
            {
                var query = (from a in context.AREAs.ToList()
                             where a.ID_ACCO == ID_ACCO && a.VIG_AREA == true
                             select new
                             {
                                 ID_AREA = a.ID_AREA,
                                 NOM_AREA = a.NOM_AREA
                             }).ToList();
                return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ListEmpleados()
        {
            string NAM_PAR = Convert.ToString(Request.QueryString["filter[filters][0][field]"]), i1;

            if (NAM_PAR == "ID_AREA")
            {
                i1 = "0";
            }
            else
            {
                i1 = "1";
            }

            int ID_AREA = Convert.ToInt32(Request.QueryString[("filter[filters][" + i1 + "][value]")]);

            using (var context = new EntityEntities())
            {
                var query = (from ce in context.CLASS_ENTITY.ToList()
                             join pe in context.PERSON_ENTITY on ce.ID_ENTI equals pe.ID_ENTI2
                             join a in context.AREAs on pe.ID_AREA equals a.ID_AREA
                             join pc in context.PERSON_CONTRACT on pe.ID_PERS_ENTI equals pc.ID_PERS_ENTI
                             where a.ID_AREA == ID_AREA && pc.VIG_CONT == true && pe.VIG_PERS_ENTI == true
                             select new
                             {
                                 ID_PERS_ENTI = pe.ID_PERS_ENTI,
                                 NAM_USER = ce.FIR_NAME + ' ' + ce.LAS_NAME,
                             }).ToList();
                return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult RolesByPerson(int id = 0)
        {
            int count = 0;

            var query = (from wui in dbe.webpages_UsersInRoles.Where(x => x.UserId == id)
                         join wpr in dbe.webpages_Roles on wui.RoleId equals wpr.RoleId
                         select new
                         {
                             UserId = wui.UserId,
                             ROLE_ID = wpr.RoleId,
                             ROLE_NAME = wpr.RoleName,
                         }).OrderBy(x => x.ROLE_NAME);
            var resultfind = query.ToList();
            count = resultfind.Count();
            return Json(new { Data = resultfind, Count = count }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult LoadRolesByPerson(int id = 0)
        {
            try
            {
                int ID_ENTI = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == id).ID_ENTI2.Value;
                int UserId = dbe.CLASS_ENTITY.Single(x => x.ID_ENTI == ID_ENTI).UserId.Value;

                var query = (from wui in dbe.webpages_UsersInRoles.Where(x => x.UserId == UserId)
                             join wpr in dbe.webpages_Roles on wui.RoleId equals wpr.RoleId
                             select new
                             {
                                 UserId = wui.UserId,
                                 ROLE_ID = wpr.RoleId,
                                 ROLE_NAME = wpr.RoleName,
                             }).OrderBy(x => x.ROLE_NAME);

                Organization org = new Organization();
                var Organ = org.chart_staff(id);

                return Json(new { Data = query, Orga = Organ }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { Data = "", Count = 0 }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveRoles()
        {
            try
            {
                int UserId = Convert.ToInt32(Session["UserId"]);
                int ID_PERS_ENTI = Convert.ToInt32(Request.Params["idpersenti"]);
                int ID_ROLE = Convert.ToInt32(Request.Params["idrole"]);

                int ID_ENTI = dbe.PERSON_ENTITY.Single(x => x.ID_PERS_ENTI == ID_PERS_ENTI).ID_ENTI2.Value;
                int UserId_Affected = dbe.CLASS_ENTITY.Single(x => x.ID_ENTI == ID_ENTI).UserId.Value;

                var query = dbe.webpages_UsersInRoles.Where(x => x.UserId == UserId_Affected && x.RoleId == ID_ROLE);

                if (query.Count() == 0)
                {
                    webpages_UsersInRoles ur = new webpages_UsersInRoles();
                    ur.RoleId = ID_ROLE;
                    ur.UserId = UserId_Affected;

                    dbe.webpages_UsersInRoles.Add(ur);
                    dbe.SaveChanges();

                    LOG_TABLE_ROLES ltr = new LOG_TABLE_ROLES();

                    ltr.ID_PERS_ENTI_Affected = ID_PERS_ENTI;
                    ltr.UserId = UserId;
                    ltr.UserId_Affected = UserId_Affected;
                    ltr.ID_ACTI_TABL_ROLE = 1;
                    ltr.RoleId_Affected = ID_ROLE;
                    ltr.CREATED = DateTime.Now;

                    dbe.LOG_TABLE_ROLES.Add(ltr);
                    dbe.SaveChanges();

                    return Content("OK");
                }
                else
                {
                    return Content("EXISTE");
                }
            }
            catch
            {
                return Content("ERROR");
            }
        }

        public ActionResult RemoveRoleByUser()
        {
            try
            {
                int UserId = Convert.ToInt32(Session["UserId"]);

                int RoleId = Convert.ToInt32(Request.Params["RoleId"]);
                int UserId_Affected = Convert.ToInt32(Request.Params["UserId"]);
                int ID_PERS_ENTI_Affected = Convert.ToInt32(Request.Params["idpersenti"]);

                webpages_UsersInRoles ur = dbe.webpages_UsersInRoles.Single(x => x.UserId == UserId_Affected && x.RoleId == RoleId);

                dbe.webpages_UsersInRoles.Remove(ur);
                dbe.SaveChanges();

                LOG_TABLE_ROLES ltr = new LOG_TABLE_ROLES();

                ltr.ID_PERS_ENTI_Affected = ID_PERS_ENTI_Affected;
                ltr.UserId = UserId;
                ltr.UserId_Affected = UserId_Affected;
                ltr.ID_ACTI_TABL_ROLE = 3;
                ltr.RoleId_Affected = RoleId;
                ltr.CREATED = DateTime.Now;

                dbe.LOG_TABLE_ROLES.Add(ltr);
                dbe.SaveChanges();

                return Content("OK");
            }
            catch
            {
                return Content("ERROR");
            }

        }

        [Authorize]
        public ActionResult ProgramacionMasiva()
        {
            bool Acceso = false;
            string[] rolesArray = Roles.GetRolesForUser(Convert.ToString(Session["UserName"]));
            foreach (string rol in rolesArray)
            {
                if (rol == "ADMINISTRADOR" || rol == "ADMIN SERVICE DESK")
                {
                    Acceso = true; //permitiendo al acceso
                }
            }
            if (Acceso == true)
            {
                return View();
            }
            else
            {
                return Content("<h3><b>Necesitas autorización para esta sección.</b></h3>");
            }

        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Programar(string cuentas, string programa)
        {
            int UserId = Convert.ToInt32(Session["UserId"]);
            try
            {
                dbc.ProgramacionMasiva(cuentas, programa, UserId);
            }
            catch (Exception ex)
            {
                return Content("ERROR");
            }

            //return Json(new { Data = "OK" }, JsonRequestBehavior.AllowGet);
            return Content("OK");
        }
        [Authorize]
        public ActionResult CrearCategoria()
        {
            return View();
        }
        public ActionResult ListarCuentas(string q, string page)
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

            var result = dbc.ListarCuentas(termino).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);

        }


        //public ActionResult ListarPriorityBNV()
        //{
        //    var result = dbc.ListPriorityTicketBNV();

        //    return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult ListarCuentas_Kendo()
        {
            var result = dbc.ListarCuentas("").ToList();

            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult ListarNroCategorias(string q, string page, int id,int idAcco)
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

            var result = dbc.ListarNroCategorias(id, termino, idAcco).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ListarCategoriasxNivel(string q, string page, int id, int id1, int id2, string idTipoTicket)
        {
            string termino = "";


            int typeTicket = Convert.ToInt32(idTipoTicket);
            

            if (Request.QueryString["q"] == null)
            {
                termino = "";
            }
            else
            {
                termino = Request.QueryString["q"].ToString();
            }

            var result = dbc.ListarCategoriasxNivel(id, id1, id2, termino, typeTicket).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarPrioridadCambio(string q, string page, int id)
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

            var result = dbc.GestionCambioListarTipo(id, termino).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListCategoryxTipoTicket(int ID_ACCO = 0, int IdTipoTicket = 0)
        {
            int ID_CATE = 0;

            try
            {

                List<catListarCategoriaxTipoTicket_Result> query = dbc.catListarCategoriaxTipoTicket(ID_ACCO, ID_CATE, IdTipoTicket).ToList();//

                return Json(new { Data = query, Count = query.Count() }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }





        public ActionResult GuardarCategoria()
        {

            //Variables
            int Cuenta = Convert.ToInt32(Request.Params["cbCuenta"]);
            int NivelCategoria = Convert.ToInt32(Request.Params["cbNivelCategoria"]);
            string NombreCategoria = Convert.ToString(Request.Params["cbNombreCategoria"]);
            string Abreviatura = Convert.ToString(Request.Params["cbAbreviatura"]);
            int SLACategory = Convert.ToInt32(Request.Params["cbSlAsociado"]);




            if (Cuenta == 0 || NivelCategoria == 0 || NombreCategoria == "" || Abreviatura == "")
            {
                return Content("<script type='text/javascript'> function init() {" +
                         "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Ingrese todos los campos obligatorios','2');}window.onload=init;</script>");
            }
            if (Abreviatura.Length > 5)
            {
                return Content("<script type='text/javascript'> function init() {" +
                         "if(top.MensajeConfirmacion) top.MensajeConfirmacion('La abreviatura debe ser máximo 5 dígitos.','2');}window.onload=init;</script>");
            }
            int NivelCategoriaPadre = NivelCategoria - 1;
            for (int i = 1; i <= NivelCategoriaPadre; i++)
            {
                int cbCategoria = Convert.ToInt32(Request.Params["cbCategoria" + i]);
                if (cbCategoria == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                           "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Ingrese todos los campos obligatorios','2');}window.onload=init;</script>");
                }
            }

            int cbTipoTicket = 0, cbTipoCambioTicket = 0;
            cbTipoTicket = Convert.ToInt32(Request.Params["cboTypeTicket"]);
            if (NivelCategoria == 3 || NivelCategoria == 4 || NivelCategoria == 5 || NivelCategoria == 6)
            {
               
                cbTipoCambioTicket = Convert.ToInt32(Request.Params["cbTipoCambioTicket"]);
                if (cbTipoTicket == 0)
                {
                    return Content("<script type='text/javascript'> function init() {" +
                              "if(top.MensajeConfirmacion) top.MensajeConfirmacion('Ingrese todos los campos obligatorios','2');}window.onload=init;</script>");
                }
            }

            int flag = 0;
            if (NivelCategoria == 1)
            {
                var cate = (from r in dbc.ACCOUNT_CATEGORY.Where(x => x.ID_ACCO == Cuenta)
                            join cat in dbc.CATEGORies on r.ID_CATE equals cat.ID_CATE
                            where cat.NIV_CATE == NivelCategoria && cat.NAM_CATE == NombreCategoria && r.VIG_ACCO_CATE == true && r.VIG_CATE == 1
                            select r).ToList();
                flag = cate.Count();
            }
            else if (NivelCategoria != 1)
            {
                int IdCategoriaPadre = Convert.ToInt32(Request.Params["cbCategoria" + NivelCategoriaPadre]);
                var catePadre = (from r in dbc.ACCOUNT_CATEGORY.Where(x => x.ID_ACCO == Cuenta)
                                 join cat in dbc.CATEGORies on r.ID_CATE equals cat.ID_CATE
                                 where cat.NIV_CATE == NivelCategoria && cat.ID_CATE_PARE == IdCategoriaPadre && cat.NAM_CATE == NombreCategoria
                                 select r).ToList();
                flag = catePadre.Count();
            }

            int UserId = Convert.ToInt32(Session["UserId"]);
            if (flag == 0)
            {
                var Categoria = new CATEGORY();
                Categoria.NAM_CATE = NombreCategoria;
                Categoria.ACR_CATE = Abreviatura;
                Categoria.ID_TYPE_TICK = cbTipoTicket;
                Categoria.ABR_CATE = Abreviatura;
                Categoria.VIG_CATE = true;
                Categoria.DEF_CATE = true;
                Categoria.NivelSla = SLACategory;

                if (NivelCategoria != 1)
                {
                    Categoria.ID_CATE_PARE = Convert.ToInt32(Request.Params["cbCategoria" + NivelCategoriaPadre]);
                }
                Categoria.NIV_CATE = NivelCategoria;
                Categoria.DATE_START = DateTime.Now;
                Categoria.DATE_END = DateTime.Now;
                Categoria.ACCO_USR = UserId;

                if (NivelCategoria == 3 || NivelCategoria == 4 || NivelCategoria == 5 || NivelCategoria == 6)
                {
                    Categoria.ID_TYPE_TICK = cbTipoTicket;
                    if (cbTipoCambioTicket != 0)
                    {
                        Categoria.IdTipoGestionCambio = cbTipoCambioTicket;
                    }
                    string prue = Convert.ToString(Request.Params["cbTicketProblema"]);
                    string gActivo = Convert.ToString(Request.Params["cbxGestionActivos"]);

                    if (Convert.ToString(Request.Params["cbTicketProblema"]) == "1")
                    {
                        Categoria.AplicaTicketProblema = true;
                    }
                    else
                    {
                        Categoria.AplicaTicketProblema = false;
                    }

                    if (Convert.ToString(Request.Params["cbxGestionActivos"]) == "1")
                    {
                        Categoria.AplicaGestionActivos = true;
                    }
                    else
                    {
                        Categoria.AplicaGestionActivos = false;
                    }

                    if (Cuenta == 56 || Cuenta == 57 || Cuenta == 58)
                    {
                        if (Convert.ToString(Request.Params["cbAplicaNotificacion"]) == "1")
                        {
                            Categoria.AplicaNotificacion = true;
                        }

                        if (Convert.ToString(Request.Params["cbTieneTarea"]) == "1")
                        {
                            Categoria.TieneTarea = true;
                        }
                    }
                }
                dbc.CATEGORies.Add(Categoria);
                dbc.SaveChanges();

                ACCOUNT_CATEGORY acc_cat = new ACCOUNT_CATEGORY();
                acc_cat.ID_ACCO = Cuenta;
                acc_cat.ID_CATE = Categoria.ID_CATE;
                acc_cat.VIG_CATE = 1;
                acc_cat.VIG_ACCO_CATE = true;
                acc_cat.IS_LEVE_TWO = true;
                dbc.ACCOUNT_CATEGORY.Add(acc_cat);
                dbc.SaveChanges();
            }
            else
            {
                return Content("<script type='text/javascript'> function init() {" +
                              "if(top.MensajeConfirmacion) top.MensajeConfirmacion('La categoria " + NombreCategoria + " ya existe.','2');}window.onload=init;</script>");
            }

            return Content("<script type='text/javascript'> function init() {" +
                        "if(top.MensajeConfirmacion) top.MensajeConfirmacion('El registro ha sido guardado.','1');}window.onload=init;</script>");
        }

        public ActionResult ListarCuentasCate(string q, string page)
        {
            string termino = "";
            int UserId = Convert.ToInt32(Session["UserId"].ToString());

            if (Request.QueryString["q"] == null)
            {
                termino = "";
            }
            else
            {
                termino = Request.QueryString["q"].ToString();
            }

            var result = dbc.ListarCuentaxUsuario(UserId, termino).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        

       

        public class CategoriaResult
        {
            public int IdCategoria1 { get; set; }
            public string Categoria1 { get; set; }
            public string AbreCategoria1 { get; set; }
            public int IdPadre1 { get; set; }
            public int IdCategoria2 { get; set; }
            public string Categoria2 { get; set; }
            public string AbreCategoria2 { get; set; }
            public int IdPadre2 { get; set; }
            public int IdCategoria3 { get; set; }
            public string Categoria3 { get; set; }
            public string AbreCategoria3 { get; set; }
            public int IdPadre3 { get; set; }
            public int IdCategoria4 { get; set; }
            public string Categoria4 { get; set; }
            public string AbreCategoria4 { get; set; }
            public int IdPadre4 { get; set; }
            public int IdCategoria5 { get; set; }
            public string Categoria5 { get; set; }
            public string AbreCategoria5 { get; set; }
            public int IdPadre5 { get; set; }
            public int IdCategoria6 { get; set; }
            public string Categoria6 { get; set; }
            public string AbreCategoria6 { get; set; }
            public int IdPadre6 { get; set; }
            public string TipoTicket { get; set; }
            public int IdCuenta { get; set; }
            public string Cuenta { get; set; }
            public string Estado { get; set; }
            public string TipoGestionCambio { get; set; }
            public string TicketProblema { get; set; }
            public string GestionActivos { get; set; }
            public string TieneTarea { get; set; }
            public string AplicaNotificacion { get; set; }
        }

    }
}
public class FormattedDbEntityValidationException : Exception
{
    public FormattedDbEntityValidationException(DbEntityValidationException innerException) :
        base(null, innerException)
    {
    }

    public override string Message
    {
        get
        {
            var innerException = InnerException as DbEntityValidationException;
            if (innerException != null)
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine();
                sb.AppendLine();
                foreach (var eve in innerException.EntityValidationErrors)
                {
                    sb.AppendLine(string.Format("- Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().FullName, eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        sb.AppendLine(string.Format("-- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                            ve.PropertyName,
                            eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                            ve.ErrorMessage));
                    }
                }
                sb.AppendLine();

                return sb.ToString();
            }

            return base.Message;
        }

    }

   


}
