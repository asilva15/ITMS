using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ConsoleNotification.Models;

namespace ConsoleNotification.Plugins
{
    
    public class Organization
    {
        public EntityEntities dbe = new EntityEntities();

        public OrganizationModel chart_staff(int ID_PERS_ENTI){
            OrganizationModel org = new OrganizationModel();           

            //org.ID_SUPE = 1;

            var query = dbe.RH_ORGA(ID_PERS_ENTI).ToList();

            //sacar el cargo en español de la persona
            var person = query.Where(x => x.NUMERO == 0).First();
            org.JOB_TITLE = person.NAM_CHAR_SPAN;            

            //sacar supervisor
            try{
                var boss = query.Where(x => x.NUMERO != 0)
                    .Where(x=>x.ID_TYPE_CHAR == 3).First();

                org.ID_BOSS = boss.ID_PERS_ENTI.Value;
            }
            catch{
                org.ID_BOSS = ID_PERS_ENTI;
            }           

            //sacar jefe UEN
            try
            {
                var uen = query.Where(x => x.ID_TYPE_CHAR == 1).First();
                var boss_uen = query.Where(x => x.ID_CHAR_PARE == uen.ID_CHAR.Value)
                                    .Where(x=>x.ID_TYPE_CHAR == 3).First();

                org.ID_BOSS_UEN = boss_uen.ID_PERS_ENTI.Value;
            }
            catch
            {
                org.ID_BOSS_UEN = ID_PERS_ENTI;
            }

            //Saca Área 
            try
            {
                var chartarea = query.Where(x=>x.ID_TYPE_CHAR==2);
                if (chartarea.Count() > 0)
                {
                    org.ID_CHAR_AREA = chartarea.First().ID_CHAR.Value;
                }
                else
                {
                    var charareaue = query.Where(x=>x.ID_TYPE_CHAR==1);
                    org.ID_CHAR_AREA = charareaue.First().ID_CHAR.Value;
                }
            }
            catch
            {
                org.ID_CHAR_AREA = 0;
            }
            //Saca el UEN
            try
            {
                var idcharuen = query.Where(x => x.ID_TYPE_CHAR == 1);
                org.ID_CHAR_UEN = idcharuen.First().ID_CHAR.Value;               

            }
            catch
            {
                org.ID_CHAR_UEN = 0;
            }           
            return org;
        }

    }
}