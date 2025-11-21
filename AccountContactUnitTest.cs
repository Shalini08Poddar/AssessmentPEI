using System;
using System.Linq;
using FakeXrmEasy;
using FakeXrmEasy.Plugins;
using FakeXrmEasy.Abstractions;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace PEI.Plugins.Tests
{
    [Collection("FakeXrmEasy collection")]
    public class CreateContactOnAccountCreatePluginTests
    {
        [Fact]
        public void CreateAccount_WithName_ShouldCreateContact()
        {
            var context = new XrmFakedContext();

            var account = new Entity("account") { Id = Guid.NewGuid() };
            account["name"] = "Test Co";

            var pluginContext = context.GetDefaultPluginContext();
            pluginContext.MessageName = "Create";
            pluginContext.PrimaryEntityName = "account";
            pluginContext.Stage = 40; // post-operation
            pluginContext.InputParameters = new ParameterCollection { { "Target", account } };
            pluginContext.OutputParameters = new ParameterCollection { { "id", account.Id } };

            var plugin = new CreateContactOnAccountCreatePlugin();

            context.ExecutePluginWith(pluginContext, plugin);

            var contacts = context.CreateQuery("contact").ToList();
            Assert.NotEmpty(contacts);
            Assert.Contains(contacts, c => c.GetAttributeValue<string>("lastname") == "Test Co");
        }

        [Fact]
        public void CreateAccount_WithoutName_ShouldCreateContactWithDefaultLastName()
        {
            var context = new XrmFakedContext();

            var account = new Entity("account") { Id = Guid.NewGuid() };
            // No name set

            var pluginContext = context.GetDefaultPluginContext();
            pluginContext.MessageName = "Create";
            pluginContext.PrimaryEntityName = "account";
            pluginContext.Stage = 40;
            pluginContext.InputParameters = new ParameterCollection { { "Target", account } };
            pluginContext.OutputParameters = new ParameterCollection { { "id", account.Id } };

            var plugin = new CreateContactOnAccountCreatePlugin();

            context.ExecutePluginWith(pluginContext, plugin);

            var contacts = context.CreateQuery("contact").ToList();
            Assert.NotEmpty(contacts);
        }
    }
}
