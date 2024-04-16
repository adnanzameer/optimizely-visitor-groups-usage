using System.Collections.Generic;
using System.Linq;
using EPiServer.Core;
using EPiServer.Personalization.VisitorGroups;
using VisitorGroupsUsage.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VisitorGroupsUsage.Models;
using VisitorGroupsUsage.ViewModels;

namespace VisitorGroupsUsage.Controllers
{
    [Authorize(Policy = Constants.PolicyName)]
    [Route("[controller]")]
    public class VisitorGroupsUsageController : Controller
    {
        private readonly IVisitorGroupsUsageService _visitorGroupsUsageService;

        public VisitorGroupsUsageController(IVisitorGroupsUsageService visitorGroupsUsageService)
        {
            _visitorGroupsUsageService = visitorGroupsUsageService;
        }

        [Route("[action]")]
        public IActionResult Index(int? page)
        {
            var model = new VisitorGroupsUsageViewModel
            {
                Results = new List<KeyValuePair<VisitorGroup, List<IContent>>>(),
                QueryString = HttpContext.Request.QueryString.ToString(),
                PageNumber = page != null && page.Value != 0 ? page.Value : 1
            };

            var usages = _visitorGroupsUsageService.GetVisitorGroupsUsage();

            if (usages != null && usages.Any())
            {
                model.TotalItemsCount = usages.Count;

                model.TotalUnusedVisitorGroupsCount = usages.Count(x => x.Value.Count != 0);

                var skip = (model.PageNumber - 1) * model.PageSize;

                var results = usages.Skip(skip).Take(model.PageSize).ToList();
                model.Results = results;
            }

            return View(model);
        }
    }
}