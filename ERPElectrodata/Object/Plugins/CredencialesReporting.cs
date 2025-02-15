using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERPElectrodata.Object.Plugins
{
    public class CredencialesReporting : Microsoft.Reporting.WebForms.IReportServerCredentials
    {
        string _usuario, _password, _dominio;
        public CredencialesReporting(string userName, string password, string domain)
        {
            _usuario = userName;
            _password = password;
            _dominio = domain;
        }

        public System.Security.Principal.WindowsIdentity ImpersonationUser
        {
            get
            {
                return null;
            }
        }

        public System.Net.ICredentials NetworkCredentials
        {
            get
            {
                return new System.Net.NetworkCredential(_usuario, _password, _dominio);
            }
        }

        public bool GetFormsCredentials(out System.Net.Cookie authCoki, out string userName, out string password, out string authority)
        {
            userName = _usuario;
            password = _password;
            authority = _dominio;
            authCoki = new System.Net.Cookie(".ASPXAUTH", ".ASPXAUTH", "/", "Domain");
            return true;
        }
    }
}