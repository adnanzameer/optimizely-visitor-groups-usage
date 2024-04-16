using EPiServer.Framework.Localization;
using EPiServer.Shell.Navigation;
using System.Collections.Generic;
using VisitorGroupsUsage.Models;

namespace VisitorGroupsUsage
{
    [MenuProvider]
    public class CustomAdminMenuProvider : IMenuProvider
    {
        public IEnumerable<MenuItem> GetMenuItems()
        {
            var urlMenuItem1 = new UrlMenuItem(LocalizationService.Current.GetString("/plugins/visitorgroupsusage/displayname"),
                "/global/cms/visitorgroupsusage", 
                "/VisitorGroupsUsage/Index")
            {
                SortIndex = 100,
                AuthorizationPolicy = Constants.PolicyName
            };

            return new List<MenuItem>
            {
                urlMenuItem1
            };
        }
    }
}
