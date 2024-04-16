using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using VisitorGroupsUsage.Helpers;
using VisitorGroupsUsage.Models;
using System;
using EPiServer.Authorization;

namespace VisitorGroupsUsage
{
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class InitializeServices : IConfigurableModule
    {
        private static readonly Action<AuthorizationPolicyBuilder> DefaultPolicy = p => p.RequireRole(Roles.Administrators, Roles.WebAdmins, Roles.CmsAdmins);

        void IConfigurableModule.ConfigureContainer(ServiceConfigurationContext context)
        {
            context.Services.AddScoped<IVisitorGroupsUsageService, VisitorGroupsUsageService>();

            context.Services.AddAuthorization(options =>
            {
                options.AddPolicy(Constants.PolicyName, DefaultPolicy);
            });
        }

        void IInitializableModule.Initialize(InitializationEngine context)
        {
        }

        void IInitializableModule.Uninitialize(InitializationEngine context)
        {
        }
    }
}
