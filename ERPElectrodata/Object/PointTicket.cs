using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ERPElectrodata.Models;
using System.Web.Mvc;

namespace ERPElectrodata.Object
{
    public class PointTicket
    {
        public CoreEntities db = new CoreEntities();
        public EntityEntities dbe = new EntityEntities();
        public string UpdatePointsByTicket(TICKET ticket, int id, int bandera)
        {
                
            var query = db.POINTS_ACTION.Single(pa => pa.ID_POIN_ACTI == 1);

            //int ID_PERS_ENTI = id;
            var point = new POINT();

            //-------------INSERTA EN POINT CUANDO SE CREA UN TICKET --------------

            point.ID_PERS_ENTI = id;
            point.ID_TICK = ticket.ID_TICK;
            point.ID_POIN_ACTI = query.ID_POIN_ACTI;
            point.VAL_POIN = query.VAL_POIN_ACTI;
            point.CREATE_DATE = DateTime.Now;
            db.POINTs.Add(point);
            db.SaveChanges();

            //-------------INSERTA EN POINT CUANDO SE CREA UN TICKET RESUELTO--------------
            if (ticket.ID_STAT == 4)
            {
                var querypoint = db.POINTS_ACTION.Single(pa => pa.ID_POIN_ACTI == 3);

                point.ID_PERS_ENTI = id;
                point.ID_TICK = ticket.ID_TICK;
                point.ID_POIN_ACTI = querypoint.ID_POIN_ACTI;
                point.VAL_POIN = querypoint.VAL_POIN_ACTI;
                point.CREATE_DATE = DateTime.Now;
                db.POINTs.Add(point);
                db.SaveChanges();

               //-----------------DENTRO DEL SLA---------------------
                var querypointsla = db.POINTS_ACTION.Single(pa => pa.ID_POIN_ACTI == 8);

                point.ID_PERS_ENTI = id;
                point.ID_TICK =ticket.ID_TICK;               
                point.ID_POIN_ACTI = querypointsla.ID_POIN_ACTI;
                point.VAL_POIN = querypointsla.VAL_POIN_ACTI;
                point.CREATE_DATE = DateTime.Now;
                db.POINTs.Add(point);
                db.SaveChanges();

                //----POR CREAR TICKETS RESUELTOS-------------------------

                if (ticket.ID_PERS_ENTI_ASSI == id)
                {
                    var querypointsolve = db.POINTS_ACTION.Single(pa => pa.ID_POIN_ACTI == 2);

                    point.ID_PERS_ENTI = id;
                    point.ID_TICK = ticket.ID_TICK;
                    point.ID_POIN_ACTI = querypointsolve.ID_POIN_ACTI;
                    point.VAL_POIN = querypointsolve.VAL_POIN_ACTI;
                    point.CREATE_DATE = DateTime.Now;
                    db.POINTs.Add(point);
                    db.SaveChanges();
                }

            }

            //------------------ASIGNA A DIFERENTE AL QUE SE LOGUEA-----------------

            if (ticket.ID_PERS_ENTI_ASSI != id)
            {
                var querypoint = db.POINTS_ACTION.Single(pa => pa.ID_POIN_ACTI == 4);

                point.ID_PERS_ENTI = ticket.ID_PERS_ENTI_ASSI;
                point.ID_TICK = ticket.ID_TICK;
                point.ID_POIN_ACTI = querypoint.ID_POIN_ACTI;
                point.VAL_POIN = querypoint.VAL_POIN_ACTI;
                point.CREATE_DATE = DateTime.Now;
                db.POINTs.Add(point);
                db.SaveChanges();

            }

            //------------------SE ASIGNA ASI MISMO-----------------

            if (ticket.ID_PERS_ENTI_ASSI == id)
            {
                var querypoint = db.POINTS_ACTION.Single(pa => pa.ID_POIN_ACTI == 7);

                point.ID_PERS_ENTI = id;
                point.ID_TICK = ticket.ID_TICK;
                point.ID_POIN_ACTI = querypoint.ID_POIN_ACTI;
                point.VAL_POIN = querypoint.VAL_POIN_ACTI;
                point.CREATE_DATE = DateTime.Now;
                db.POINTs.Add(point);
                db.SaveChanges();

            }

            //-----------------CUANDO HAY ADJUNTOS---------------------

            if (bandera == 1)
            {
                var querypoint = db.POINTS_ACTION.Single(pa => pa.ID_POIN_ACTI == 6);

                point.ID_PERS_ENTI = id;
                point.ID_TICK = ticket.ID_TICK;
                point.ID_POIN_ACTI = querypoint.ID_POIN_ACTI;
                point.VAL_POIN = querypoint.VAL_POIN_ACTI;
                point.CREATE_DATE = DateTime.Now;
                db.POINTs.Add(point);
                db.SaveChanges();
            }
             
            //-----------------CUANDO SE CREA UN TICKET SCHEDULED---------------------

            //if (ticket.ID_STAT == 5)
            //{
            //    var querypointsch = db.POINTS_ACTION.Single(pa => pa.ID_POIN_ACTI == 11);

            //    point.ID_PERS_ENTI = id;
            //    point.ID_TICK = ticket.ID_TICK;
            //    point.ID_POIN_ACTI = querypointsch.ID_POIN_ACTI;
            //    point.VAL_POIN = querypointsch.VAL_POIN_ACTI;
            //    point.CREATE_DATE = DateTime.Now;
            //    db.POINTs.Add(point);
            //    db.SaveChanges();


            //}
                        
            return "OK";
        }
        public string UpdatePointByDetailsTicket(DETAILS_TICKET details_ticket, int id, int bandera)
        {

            //--------------------ESTADO RESUELTO ------------------

            //--------------------Resolve incident=3 ---- RESOLVED=4-------------------
            //int ID_PERS_ENTI = id;
            var point = new POINT();

            var query = db.TICKETs.Single(t => t.ID_TICK == details_ticket.ID_TICK);

            //-----------------CUANDO SE SCHEDULED EL TICKET---------------------

            if (details_ticket.ID_TYPE_DETA_TICK == 3 && details_ticket.ID_STAT == 5)
            {
                var querypointsch = db.POINTS_ACTION.Single(pa => pa.ID_POIN_ACTI == 11);

                point.ID_PERS_ENTI = id;
                point.ID_TICK = details_ticket.ID_TICK;
                point.ID_DETA_TICK = details_ticket.ID_DETA_TICK;
                point.ID_POIN_ACTI = querypointsch.ID_POIN_ACTI;
                point.VAL_POIN = querypointsch.VAL_POIN_ACTI;
                point.CREATE_DATE = DateTime.Now;
                db.POINTs.Add(point);
                db.SaveChanges();

            }

            //-----------------CUANDO SE HACE UN LOG COMMENT---------------------

            if (details_ticket.ID_TYPE_DETA_TICK == 1)
            {
                var querypointslc = db.POINTS_ACTION.Single(pa => pa.ID_POIN_ACTI == 12);

                point.ID_PERS_ENTI = id;
                point.ID_TICK = details_ticket.ID_TICK;
                point.ID_DETA_TICK = details_ticket.ID_DETA_TICK;
                point.ID_POIN_ACTI = querypointslc.ID_POIN_ACTI;
                point.VAL_POIN = querypointslc.VAL_POIN_ACTI;
                point.CREATE_DATE = DateTime.Now;
                db.POINTs.Add(point);
                db.SaveChanges();

            }

            //-----------------CUANDO RESUELVE EL TICKET---------------------

            if (details_ticket.ID_TYPE_DETA_TICK == 3 && details_ticket.ID_STAT == 4)
            {
                var querypoint = db.POINTS_ACTION.Single(pa => pa.ID_POIN_ACTI == 3);

                point.ID_PERS_ENTI = id;
                point.ID_TICK = details_ticket.ID_TICK;
                point.ID_DETA_TICK = details_ticket.ID_DETA_TICK;
                point.ID_POIN_ACTI = querypoint.ID_POIN_ACTI;
                point.VAL_POIN = querypoint.VAL_POIN_ACTI;
                point.CREATE_DATE = DateTime.Now;
                db.POINTs.Add(point);
                db.SaveChanges();

                //-----------------------------DENTRO DEL SLA--------------------------------

                int slainpoit = db.POINTs.Where(x => x.ID_TICK == query.ID_TICK).Where(y => y.ID_POIN_ACTI == 9).Count();

                if (query.MINUTES != null && slainpoit == 0)
                {
                    var tiempo = (from t in db.TICKETs.Where(x => x.ID_TICK == details_ticket.ID_TICK)
                                  join p in db.PRIORITies on t.ID_PRIO equals p.ID_PRIO
                                  select new
                                  {
                                      p.ID_PRIO,
                                      p.HOU_PRIO,
                                  }).First();

                    if (query.MINUTES <= (tiempo.HOU_PRIO * 60) && query.ID_PRIO != 5)
                    {
                        var querypointsla = db.POINTS_ACTION.Single(pa => pa.ID_POIN_ACTI == 8);

                        point.ID_PERS_ENTI = id;
                        point.ID_TICK = details_ticket.ID_TICK;
                        point.ID_DETA_TICK = details_ticket.ID_DETA_TICK;
                        point.ID_POIN_ACTI = querypointsla.ID_POIN_ACTI;
                        point.VAL_POIN = querypointsla.VAL_POIN_ACTI;
                        point.CREATE_DATE = DateTime.Now;
                        db.POINTs.Add(point);
                        db.SaveChanges();
                    }
                    else
                    {
                        var querypointsla = db.POINTS_ACTION.Single(pa => pa.ID_POIN_ACTI == 9);

                        if (query.ID_PRIO != 5)
                        {
                            point.ID_PERS_ENTI = id;
                            point.ID_TICK = details_ticket.ID_TICK;
                            point.ID_DETA_TICK = details_ticket.ID_DETA_TICK;
                            point.ID_POIN_ACTI = querypointsla.ID_POIN_ACTI;
                            point.VAL_POIN = querypointsla.VAL_POIN_ACTI;
                            point.CREATE_DATE = DateTime.Now;
                            db.POINTs.Add(point);
                            db.SaveChanges();
                        }
                    }
                }

                //-----------------CUANDO RESUELVE EL TICKET SI SE ASIGNADO EL MISMO ---------------------

                if (query.UserId == details_ticket.UserId)
                {
                    var querypointsolve = db.POINTS_ACTION.Single(pa => pa.ID_POIN_ACTI == 2);

                    point.ID_PERS_ENTI = id;
                    point.ID_TICK = details_ticket.ID_TICK;
                    point.ID_DETA_TICK = details_ticket.ID_DETA_TICK;
                    point.ID_POIN_ACTI = querypointsolve.ID_POIN_ACTI;
                    point.VAL_POIN = querypointsolve.VAL_POIN_ACTI;
                    point.CREATE_DATE = DateTime.Now;
                    db.POINTs.Add(point);
                    db.SaveChanges();
                }
            }

            //-----------------CUANDO ES TRANSFERIDO ---SE ASIGNA A OTRO---------------------

            if (details_ticket.ID_TYPE_DETA_TICK == 2 && details_ticket.ID_PERS_ENTI != null)
            {
                var querypointtra = db.POINTS_ACTION.Single(pa => pa.ID_POIN_ACTI == 4);

                point.ID_PERS_ENTI = details_ticket.ID_PERS_ENTI;
                point.ID_TICK = details_ticket.ID_TICK;
                point.ID_DETA_TICK = details_ticket.ID_DETA_TICK;
                point.ID_POIN_ACTI = querypointtra.ID_POIN_ACTI;
                point.VAL_POIN = querypointtra.VAL_POIN_ACTI;
                point.CREATE_DATE = DateTime.Now;
                db.POINTs.Add(point);
                db.SaveChanges();
            }

            //-----------------CUANDO HAY ADJUNTOS---------------------

            if (bandera == 1)
            {
                var querypointatt = db.POINTS_ACTION.Single(pa => pa.ID_POIN_ACTI == 6);

                point.ID_PERS_ENTI = id;
                point.ID_TICK = details_ticket.ID_TICK;
                point.ID_DETA_TICK = details_ticket.ID_DETA_TICK;
                point.ID_POIN_ACTI = querypointatt.ID_POIN_ACTI;
                point.VAL_POIN = querypointatt.VAL_POIN_ACTI;
                point.CREATE_DATE = DateTime.Now;
                db.POINTs.Add(point);
                db.SaveChanges();
            }

            return "OK";
        }
        public string UpdatePointByDetailsTicketReject(DETAILS_TICKET details_ticket, int id)
        {
                var point = new POINT();

                //var query = db.TICKETs.Single(t => t.ID_TICK == details_ticket.ID_TICK);   
        
                if (details_ticket.ID_TYPE_DETA_TICK != 1)
                {
                    var querypointrej = db.POINTS_ACTION.Single(pa => pa.ID_POIN_ACTI == 10);

                    point.ID_PERS_ENTI = id;
                    point.ID_TICK = details_ticket.ID_TICK;
                    point.ID_DETA_TICK = details_ticket.ID_DETA_TICK;
                    point.ID_POIN_ACTI = querypointrej.ID_POIN_ACTI;
                    point.VAL_POIN = querypointrej.VAL_POIN_ACTI;
                    point.CREATE_DATE = DateTime.Now;
                    db.POINTs.Add(point);
                    db.SaveChanges();
                }
                else
                {
                    var querypointslc = db.POINTS_ACTION.Single(pa => pa.ID_POIN_ACTI == 12);

                    point.ID_PERS_ENTI = id;
                    point.ID_TICK = details_ticket.ID_TICK;
                    point.ID_DETA_TICK = details_ticket.ID_DETA_TICK;
                    point.ID_POIN_ACTI = querypointslc.ID_POIN_ACTI;
                    point.VAL_POIN = querypointslc.VAL_POIN_ACTI;
                    point.CREATE_DATE = DateTime.Now;
                    db.POINTs.Add(point);
                    db.SaveChanges();
                }

            return "OK";
        }
    }
}