using System;
using Microsoft.Xrm.Sdk;

namespace PEI.Plugins
{
    public class CreateContactOnAccountCreatePlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext)); //to obtain execution context from the service provider
            var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory)); //to obtain organization service reference that allows us to interact with CRM data and perform CRUD operations
            var service = serviceFactory.CreateOrganizationService(context.UserId);

            if (context.MessageName != "Create") return; // to run only on Create of Contact record
            if (!context.InputParameters.Contains("Target") || !(context.InputParameters["Target"] is Entity account)) return;

            if (account.LogicalName != "account") return; //check if logical name is account

            Guid accountId = account.Id; // to get the GUID of the created account if available in output parameters (post-Create)
            if (accountId == Guid.Empty && context.OutputParameters.Contains("id"))
            {
                accountId = (Guid)context.OutputParameters["id"];
            }

            var accountName = account.Contains("name") ? account.GetAttributeValue<string>("name") : null; //if Account Name contains value, then fetch that value

            var contact = new Entity("contact");
            contact["firstname"] = "Default";
            contact["lastname"] = string.IsNullOrWhiteSpace(accountName) ? "Account Record" : accountName; //Last Name is mandatory field

            if (accountId != Guid.Empty)
            {
                contact["parentcustomerid"] = new EntityReference("account", accountId); //to set parentcustomerid (lookup to account)
            }

            var contactId = service.Create(contact);
        }
    }
}
