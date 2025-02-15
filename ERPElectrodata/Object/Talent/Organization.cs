using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ERPElectrodata.Models;


namespace ERPElectrodata.Object.Talent
{
    
    public class Organization
    {
        public EntityEntities dbe = new EntityEntities();

        public OrganizationModel chart_staff(int ID_PERS_ENTI){
            OrganizationModel org = new OrganizationModel();
            

            //org.ID_SUPE = 1;

            var query = dbe.RH_ORGA(ID_PERS_ENTI).ToList();

            //sacar el cargo de la persona
            var cargo = query.Where(x => x.NUMERO == 0).First();
            org.JOB_TITLE = cargo.NAM_CHAR_SPAN;

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
            //Saca ID_CHAR_AREA Área 
            try
            {
                var chartarea = query.Where(x => x.ID_TYPE_CHAR == 2);
                if (chartarea.Count() > 0)
                {
                    org.ID_CHAR_AREA = chartarea.First().ID_CHAR.Value;
                }
                else
                {
                    var charareaue = query.Where(x => x.ID_TYPE_CHAR == 1);
                    org.ID_CHAR_AREA = charareaue.First().ID_CHAR.Value;
                }
            }
            catch
            {
                org.ID_CHAR_AREA = 0;
            }
            //Saca el ID_CHAR_UEN UEN
            try
            {
                var idcharuen = query.Where(x => x.ID_TYPE_CHAR == 1);
                org.ID_CHAR_UEN = idcharuen.First().ID_CHAR.Value;

            }
            catch
            {
                org.ID_CHAR_UEN = 0;
            }

            //Saca  Nombre Área 
            try
            {
                var Namearea = query.Where(x => x.ID_TYPE_CHAR == 2);
                if (Namearea.Count() > 0)
                {
                    org.NAME_AREA = Namearea.First().NAM_CHAR_ENGL;
                }
                else
                {
                    var Namearea2 = query.Where(x => x.ID_TYPE_CHAR == 1);
                    org.NAME_AREA = Namearea2.First().NAM_CHAR_ENGL;
                }
            }
            catch
            {
                org.NAME_AREA = "";
            }
            //Saca el Nombre UEN
            try
            {
                var charuen = query.Where(x => x.ID_TYPE_CHAR == 1);
                org.NAME_UE = charuen.First().NAM_CHAR_ENGL;

            }
            catch
            {
                org.NAME_UE = "";
            } 

            return org;
        }

    }
}