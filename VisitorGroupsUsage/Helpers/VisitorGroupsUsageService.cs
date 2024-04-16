using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EPiServer.Cms.Shell;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Personalization.VisitorGroups;
using EPiServer.Personalization;
using EPiServer.Personalization.VisitorGroups.Internal;
using EPiServer.SpecializedProperties;

namespace VisitorGroupsUsage.Helpers
{
    public class VisitorGroupsUsageService : IVisitorGroupsUsageService
    {
        private readonly IContentLoader _contentLoader;
        private readonly ILanguageBranchRepository _languageBranchRepository;
        private readonly List<Tuple<string, IContent>> _visitorGroupsOnContent = new();

        public VisitorGroupsUsageService(IContentLoader contentLoader, ILanguageBranchRepository languageBranchRepository)
        {
            _contentLoader = contentLoader;
            _languageBranchRepository = languageBranchRepository;
        }

        public List<KeyValuePair<VisitorGroup, List<IContent>>> GetVisitorGroupsUsage()
        {
            var contentReferences = _contentLoader.GetDescendents(ContentReference.StartPage).ToList();
            if (!contentReferences.Contains(ContentReference.StartPage))
            {
                contentReferences.Add(ContentReference.StartPage);
            }

            var languages = _languageBranchRepository.ListEnabled();

            foreach (var language in languages)
            {
                foreach (var contentReference in contentReferences)
                {
                    var content = _contentLoader.Get<IContent>(contentReference, new CultureInfo(language.LanguageID));
                    if (content != null)
                    {
                        GetVisitorGroupsRecursive(content, language.LanguageID);
                    }
                }
            }

            var visitorGroups = new VisitorGroupStore().List();
            Dictionary<VisitorGroup, List<IContent>> visitorGroupsUsages = new();

            foreach (var visitor in visitorGroups)
            {
                var matches = _visitorGroupsOnContent.Where(x => x.Item1.Equals(visitor.Id.ToString(), StringComparison.OrdinalIgnoreCase)).ToList();

                if (matches.Any())
                {
                    var content = new List<IContent>();
                    foreach (var variable in matches)
                    {
                        if (content.Count(x => x.ContentLink.ID == variable.Item2.ContentLink.ID && x.LanguageBranch().Equals(variable.Item2.LanguageBranch(), StringComparison.OrdinalIgnoreCase)) == 0)
                        {
                            content.Add(variable.Item2);
                        }
                    }

                    visitorGroupsUsages.Add(visitor, content);
                }
                else
                {
                    visitorGroupsUsages.Add(visitor, new List<IContent>());
                }
            }

            return visitorGroupsUsages.OrderByDescending(x => x.Value.Count).ThenBy(x => x.Key.Name).ToList();
        }

        private void GetVisitorGroupsRecursive(IContent currentContent, string language)
        {
            foreach (var propertyData in currentContent.Property)
            {
                if (propertyData is IPersonalizedRoles personalizedRoles)
                {
                    foreach (var role in personalizedRoles.GetRoles())
                    {
                        AddVisitorGroup(role, currentContent);
                    }
                }

                if (propertyData is PropertyContentArea && propertyData.Value is ContentArea contentArea)
                {
                    foreach (var contentAreaItem in contentArea.Items)
                    {
                        var allowedRoles = contentAreaItem.AllowedRoles;
                        if (allowedRoles != null)
                        {
                            foreach (var role in allowedRoles)
                            {
                                AddVisitorGroup(role, currentContent);
                            }
                        }

                        if (!ContentReference.IsNullOrEmpty(contentAreaItem.ContentLink))
                        {
                            var content = _contentLoader.Get<IContent>(contentAreaItem.ContentLink, new CultureInfo(language));
                            if (content != null)
                            {
                                GetVisitorGroupsRecursive(content, language);
                            }
                        }
                    }
                }
            }

        }

        private void AddVisitorGroup(string group, IContent content)
        {
            _visitorGroupsOnContent.Add(new Tuple<string, IContent>(group, content));
        }

    }
}
