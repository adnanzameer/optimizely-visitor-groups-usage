using EPiServer.Core;
using EPiServer.Personalization.VisitorGroups;
using System.Collections.Generic;

namespace VisitorGroupsUsage.Helpers
{
    public interface IVisitorGroupsUsageService
    {
        List<KeyValuePair<VisitorGroup, List<IContent>>> GetVisitorGroupsUsage();
    }
}
