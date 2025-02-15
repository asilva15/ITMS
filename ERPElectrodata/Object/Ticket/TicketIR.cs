using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ERPElectrodata.Models;

namespace ERPElectrodata.Object.Ticket
{
    public class TicketIR
    {
        public CoreEntities dbc = new CoreEntities();
        public String ExpirationTime(int id,int hou_prio){
            
            DateTime Fecha = DateTime.MinValue;
            int TimeTrans = 0, time = 0;
            string stop = "Stopped";

            try
            {
                var t = dbc.TICKETs.Single(x=>x.ID_TICK == id);
                
                if(t.ID_PRIO.Value == 5){
                    //return DateTime.MinValue;
                    return "Planning";
                }
                else
                {
                    if(t.ID_STAT_END.Value == 1 || t.ID_STAT_END.Value == 3)
                    {
                        try
                        {
                            Fecha = Convert.ToDateTime(dbc.DETAILS_TICKET.Where(x => x.ID_TICK == id)
                               .Where(x => x.ID_STAT != null)
                               .Where(x=>x.ID_TYPE_DETA_TICK == 3)
                               .OrderByDescending(x => x.CREATE_DETA_INCI)
                               .Take(1).Single().CREATE_DETA_INCI);
                        }
                        catch
                        {
                            Fecha = t.FEC_TICK.Value;
                        }

                        TimeTrans = Convert.ToInt32(t.MINUTES);

                        int fm = (hou_prio * 60) - (Convert.ToInt32(TimeTrans) + (DateTime.Now.Subtract(Fecha).Days * 24 * 60) + (DateTime.Now.Subtract(Fecha).Hours * 60) + DateTime.Now.Subtract(Fecha).Minutes);

                        int day = fm / 1440;
                        int hour = (fm - day * 1440) / 60;
                        int minutes = fm - day * 1440 - hour * 60;
                        //int segundos = 60;

                        return Convert.ToString(day) + (day == 1 ? " day" : " days") + " | " + Convert.ToString(hour) + ":" + ("00"+Convert.ToString(minutes)).Substring((Convert.ToString(minutes) + "00").Length-2, 2)
                            //+ Convert.ToString(segundos) 
                            + " Running";
                        //return Convert.ToString(fm / 1440) + ":" + Convert.ToString((fm % 1440)/60) + ":" + Convert.ToString((fm % 60))+":60" + ":Running";

                    }
                    else if(t.ID_STAT_END.Value == 5){
                        try
                        {
                            try
                            {
                                Fecha = Convert.ToDateTime(dbc.DETAILS_TICKET.Where(x => x.ID_TICK == id)
                                    .Where(x => x.ID_STAT == 5)
                                    .Where(x=>x.ID_TYPE_DETA_TICK == 3)
                                    .OrderByDescending(x => x.CREATE_DETA_INCI)
                                    .Take(1).Single().FEC_SCHE);
                            }
                            catch
                            {
                                Fecha = t.FEC_INI_TICK.Value;
                            }

                            if (Fecha == DateTime.MinValue)
                            {
                                Fecha = t.FEC_INI_TICK.Value;
                            }

                            TimeTrans = Convert.ToInt32(t.MINUTES);

                            if (Fecha > DateTime.Now)
                            {
                                time = TimeTrans;
                            }
                            else
                            {
                                stop = "Running";
                                time = (Convert.ToInt32(TimeTrans) + (DateTime.Now.Subtract(Fecha).Days * 24 * 60) + (DateTime.Now.Subtract(Fecha).Hours * 60) + DateTime.Now.Subtract(Fecha).Minutes);
                            }

                            int fm = (hou_prio * 60) - time;

                            int day = fm / 1440;
                            int hour = (fm - day * 1440) / 60;
                            int minutes = fm - day * 1440 - hour * 60;
                            int segundos = 60;

                            return Convert.ToString(day) + (day == 1 ? "day" : "days") + " | " + Convert.ToString(hour) + ":" + (Convert.ToString(minutes) + "00").Substring(("00" + Convert.ToString(minutes)).Length - 2, 2) +
                                //":" + Convert.ToString(segundos) +
                                " Stopped";
                            //return Convert.ToString(fm / 60) + ":" + Convert.ToString((fm % 60)) + " HH:mm Stopped";
                            //return Convert.ToString(Convert.ToInt32(hou_prio) - Convert.ToInt32(time)) + " Hours "+stop;
                        }
                        catch
                        {
                            return stop;
                        }
                    }
                    else
                    {
                        return stop;
                    }

                }
            }
            catch
            {

            }
            return stop;
        }
    }
}