using System;
using FakeXrmEasy;
using FakeXrmEasy.Plugins;
using FakeXrmEasy.Abstractions;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace PEI.Plugins.Tests
{
    [Collection("FakeXrmEasy collection")]
    public class ContactDuplicatePreventionPluginTests
    {
        [Fact]
        public void Create_WithDuplicateEmail_ShouldThrowInvalidPluginException()
        {
            var ctx = new XrmFakedContext();
            var existing = new Entity("contact") { Id = Guid.NewGuid() };
            existing["emailaddress1"] = "abc@test.com";
            ctx.Initialize(new[] { existing });

            var target = new Entity("contact");
            target["emailaddress1"] = "abc@test.com";

            var pluginContext = ctx.GetDefaultPluginContext();
            pluginContext.MessageName = "Create";
            pluginContext.PrimaryEntityName = "contact";
            pluginContext.InputParameters = new ParameterCollection { { "Target", target } };

            var plugin = new ContactDuplicatePrevention();

            var ex = Assert.Throws<InvalidPluginExecutionException>(() =>
                ctx.ExecutePluginWith(pluginContext, plugin));

            Assert.Contains("already exists", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void Create_WithUniqueEmail_ShouldNotThrow()
        {
            var ctx = new XrmFakedContext();

            var target = new Entity("contact");
            target["emailaddress1"] = "unique@example.com";

            var pluginContext = ctx.GetDefaultPluginContext();
            pluginContext.MessageName = "Create";
            pluginContext.PrimaryEntityName = "contact";
            pluginContext.InputParameters = new ParameterCollection { { "Target", target } };

            var plugin = new ContactDuplicatePrevention();

            ctx.ExecutePluginWith(pluginContext, plugin);

            Assert.True(true);
        }
    }
}
