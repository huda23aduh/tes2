using PayMedia.ApplicationServices.ClientProxy;
using PayMedia.ApplicationServices.Customers.ServiceContracts;
using PayMedia.ApplicationServices.Customers.ServiceContracts.DataContracts;
using PayMedia.ApplicationServices.SharedContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tes2_huda
{
    class Huda_note_class
    {
        public void createNote()
        {
            int reasonkey = 1;
            
            //Create the authentication header, initialize with credentials, and
            //create the service proxy. For details, see the API Developer's
            //Guide and the Reference Library CHM.
            Authentication_class var_auth = new Authentication_class();
            AuthenticationHeader ah = var_auth.getAuthentication_header();
            AsmRepository.SetServiceLocationUrl("http://mncsvasm.mskydev1.local/asm/all/servicelocation.svc");
            var customersConfiguration = AsmRepository.GetServiceProxyCachedOrDefault<ICustomersService>(ah);
            //Instanstiate and initialize a ValidAddress object.
            //For a description of each property, see the API Reference Library CHM.
            Note new_note = new Note();
            new_note.CategoryId = 1;
            //new_note.CategoryKey = "ADMINPOS";
            new_note.CompletionStageId = 1;
            //new_note.CompletionStageKey = NoteCompletionStage.ENTITY_ID.ToString();

            new_note.Body = "tes notes dari huda";

            new_note.CustomerId = 35153;


            Note the_new_note = customersConfiguration.CreateNote(new_note, 0);
            Console.WriteLine("note created. ID = {0}", the_new_note.Id);
            Console.ReadLine();
        }
    }
}
