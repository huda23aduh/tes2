using PayMedia.ApplicationServices.SharedContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tes2_huda
{
    class Authentication_class
    {
        public AuthenticationHeader getAuthentication_header()
        {
            //Create the authentication header, initialize with credentials, and
            //create the service proxy.
            //To avoid passing in a clear password, use Token Authentication.
            //For details, see the API Developer's Guide.
            //Pass in a value for ExternalAgent to track the external user who
            //accessed the API. For details, see the API Reference Library CHM.
            AuthenticationHeader ah = new AuthenticationHeader
            {
                //UserName = "ICCAPI",
                //Proof = "api9hsn!",

                UserName = "hdkartika",
                Proof = "ICCMsky2016",
                Dsn = "MSKY-TRA"


                //UserName = "iccapi",
                //Proof = "ap1msky!",
                //Dsn = "MSKY-PROD"
            };

            return ah;
        }
        //http://192.168.176.112/asm/all/servicelocation.svc http://mncsvasm.mskydev1.local/asm/all/servicelocation.svc
        public string var_url_asm = "http://mncsvasm.mskydev1.local/asm/all/servicelocation.svc";
    }
}
