using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using NavMvc.Configuration;
using NavMvc.NavItems;
using NavMvc.Providers;

namespace NavMvc.Engine
{
    public class NavMvcFactory
    {
        #region Singleton
        private static readonly NavMvcFactory Singleton = new NavMvcFactory();
        public static NavMvcFactory Current { get { return Singleton; } }
        private NavMvcFactory() { }
        #endregion

        #region Configuration
        private List<INavProvider> _providers = GetProviders();
        private InactiveNavBehaviors _inactiveNavBehavior = GetNavBehavior();
        private static List<INavProvider> GetProviders()
        {
            var configuration = ConfigurationManager.GetSection("navProviders") as NavMvcConfigurationSection;
            if (configuration == null) return new List<INavProvider>();
            var providerconfigs = configuration.Providers;
            if (providerconfigs == null) return new List<INavProvider>();
            var types = providerconfigs.Cast<NavProviderCollection>().Select(c => c.Type).Where(t => t != null).ToArray();
            var providers = types.Select(t => Activator.CreateInstance(t) as INavProvider).Where(p => p != null).ToList();
            return providers;
        }
        private static InactiveNavBehaviors GetNavBehavior()
        {
            var configuration = ConfigurationManager.GetSection("navProviders") as NavMvcConfigurationSection;
            if (configuration == null) return InactiveNavBehaviors.AlwaysHide;
            return configuration.InactiveNavBehavior;
        }
        #endregion

        #region Factory Items
        private readonly Dictionary<string, List<NavItem>> _factoryItems = new Dictionary<string, List<NavItem>>();

        /// <summary>
        /// Adds a NavItem to the specified navigation context
        /// </summary>
        /// <param name="navContext">The context this NavItem belongs to</param>
        /// <param name="item">The NavItem to be added</param>
        public void AddNavItem(string navContext, NavItem item)
        {
            if (!_factoryItems.ContainsKey(navContext)) _factoryItems.Add(navContext, new List<NavItem>());
            _factoryItems[navContext].Add(item);
        }

        /// <summary>
        /// Remove all items previously added to the specified navigation context
        /// </summary>
        /// <param name="navContext">The context to be purged</param>
        /// <returns>true on exists/success</returns>
        public bool RemoveContext(string navContext)
        {
            return _factoryItems.Remove(navContext);
        }

        /// <summary>
        /// Removes a previously added NavItem from the specified navigation context
        /// </summary>
        /// <param name="navContext">The context the NavItem belongs to</param>
        /// <param name="item">The item to be removed</param>
        /// <returns>true on exists/success</returns>
        public bool RemoveNavItem(string navContext, NavItem item)
        {
            if (!_factoryItems.ContainsKey(navContext)) return false;
            return _factoryItems[navContext].Remove(item);
        }
        #endregion

        #region Factory Values
        private Dictionary<string, Dictionary<string, object>> GetRouteValues(ControllerContext webContext)
        {
            if (webContext == null || webContext.HttpContext == null || webContext.HttpContext.Items == null) return null;
            var values = webContext.HttpContext.Items["NavMvcContextValues"] 
                as Dictionary<string, Dictionary<string, object>>
                ?? new Dictionary<string, Dictionary<string, object>>();
            webContext.HttpContext.Items["NavMvcContextValues"] = values;
            return values;
        }

        /// <summary>
        /// Adds a route value to be persisted for the specified context
        /// </summary>
        /// <param name="webContext">The current web context</param>
        /// <param name="context">The navigation context this value belongs to</param>
        /// <param name="name">The name of this value (e.g. currentEntityId)</param>
        /// <param name="value">The value</param>
        public void AddContextValue(ControllerContext webContext, string context, string name, object value)
        {
            var routeValues = GetRouteValues(webContext);
            if (!routeValues.ContainsKey(context)) routeValues.Add(context, new Dictionary<string, object>());
            var ctx = routeValues[context];
            if (!ctx.ContainsKey(name)) ctx.Add(name, value);
            else ctx[name] = value;
        }

        /// <summary>
        /// Removes a route value from the specified context
        /// </summary>
        /// <param name="webContext">The current web context</param>
        /// <param name="context">The navigation context the value belongs to</param>
        /// <param name="name">The name of the value to be removed</param>
        public void RemoveContextValue(ControllerContext webContext, string context, string name)
        {
            var routeValues = GetRouteValues(webContext);
            if (routeValues.ContainsKey(context) && routeValues[context].ContainsKey(name))
            {
                routeValues[context].Remove(name);
            }
        }

        /// <summary>
        /// Clears all navigation route values for the specified context
        /// </summary>
        /// <param name="webContext">The current web context</param>
        /// <param name="context">The context to be purged</param>
        public void ClearContextValues(ControllerContext webContext, string context)
        {
            var routeValues = GetRouteValues(webContext);
            if (routeValues.ContainsKey(context))
            {
                routeValues.Remove(context);
            }
        }

        /// <summary>
        /// Clears all navigation route values
        /// </summary>
        /// <param name="webContext">The current web context</param>
        public void ClearAllContextValues(ControllerContext webContext)
        {
            var routeValues = GetRouteValues(webContext);
            routeValues.Clear();
        }
        #endregion

        #region Retrieval
        /// <summary>
        /// Gathers all nav items for the requested context
        /// </summary>
        /// <param name="webContext">The current controller context</param>
        /// <param name="navContext">The requested navigation context</param>
        /// <returns>The requested navigation items</returns>
        public NavItem[] GetNavItems(ControllerContext webContext, string navContext)
        {
            var items = GatherNavItems(webContext, navContext);
            var itemsWithChildren = items.ToDictionary(
                i => i, i => !string.IsNullOrWhiteSpace(i.SubNavContext)
                    ? GatherSubNavItems(webContext, i.SubNavContext) : null);

            itemsWithChildren = FilterByRoles(itemsWithChildren);
            DetectActiveNavigations(webContext, itemsWithChildren);
            PopulateRouteValues(webContext, itemsWithChildren);
            return itemsWithChildren.Keys.OrderBy(i => i.OrderingHint).ToArray();
        }

        // Gets NavItems from all sources
        private NavItem[] GatherNavItems(ControllerContext webContext, string navContext)
        {
            var items = _factoryItems.ContainsKey(navContext)
                ? _factoryItems[navContext].ToArray()
                : new NavItem[0];
            items = _providers.Aggregate(items,
                (current, provider) => current
                    .Concat(provider.GetNavItems(webContext, navContext) ?? new NavItem[0])
                    .ToArray())
                .Where(i => i != null)
                .Select(i => (NavItem)i.Clone())
                .ToArray();
            foreach (var item in items) {
                item.Context = navContext;
            }
            return items;
        }

        // Filters out NavItems that require roles the current user does not have
        private Dictionary<NavItem, NavItem[]> FilterByRoles(Dictionary<NavItem, NavItem[]> items)
        {
            string[] roles;
            try { roles = Roles.GetRolesForUser(); }
            catch { return items; }

            var allowedItems = new Dictionary<NavItem, NavItem[]>(items);
            var disallowed = items.Where(i => !MatchRolesCriteria(roles, i.Key.Roles)).ToList();

            // Handle "disabled" navs based on the config setting
            disallowed.ForEach(v => allowedItems.Remove(v.Key));
            if (_inactiveNavBehavior != InactiveNavBehaviors.AlwaysHide) {
                var disabled = disallowed // We want to keep items that have "allowed" children
                    .Where(i => i.Value != null && i.Value.Any(s => MatchRolesCriteria(roles, s.Roles)))
                    .Select(i =>
                    {
                        NavItem newItem;
                        switch (_inactiveNavBehavior)
                        {
                            case InactiveNavBehaviors.RedirectToFirstActive:
                                newItem = (NavItem)i.Value.First(s => MatchRolesCriteria(roles, s.Roles)).Clone();
                                newItem.Context = i.Key.Context;
                                newItem.Title = i.Key.Title;
                                newItem.Description = i.Key.Description;
                                newItem.OrderingHint = i.Key.OrderingHint;
                                newItem.IsActive = i.Key.IsActive;
                                newItem.SubNavContext = i.Key.SubNavContext;
                                break;
                            case InactiveNavBehaviors.ShowIfActiveChild:
                            default: // Make an inactive nav item
                                newItem = new NavItem
                                {
                                    Context = i.Key.Context,
                                    Title = i.Key.Title,
                                    Description = i.Key.Description,
                                    Roles = i.Key.Roles,
                                    OrderingHint = i.Key.OrderingHint,
                                    SubNavContext = i.Key.SubNavContext,
                                    RouteValues = i.Key.RouteValues,
                                };
                                break;
                        }
                        return new KeyValuePair<NavItem, NavItem[]>(newItem, i.Value);
                    }).ToList();
                disabled.ForEach(v => allowedItems.Add(v.Key, v.Value));
            }
            return allowedItems;
        }

        // This is totally cheating TODO: Dissassociate ActionNavItem from the NavMvcFactory
        private void DetectActiveNavigations(ControllerContext webContext, Dictionary<NavItem, NavItem[]> items)
        {
            var currentAction = webContext.RouteData.Values["action"] as string;
            var currentController = webContext.RouteData.Values["controller"] as string;
            foreach (var branch in items) {
                var branchItems = new[] {branch.Key};
                if (branch.Value != null) branchItems = branchItems.Concat(branch.Value).ToArray();
                branch.Key.IsActive = branchItems.OfType<ActionNavItem>().Any(a => a != null
                    && (a.Action ?? "").Equals(currentAction, StringComparison.InvariantCultureIgnoreCase)
                    && (a.Controller ?? "").Equals(currentController, StringComparison.InvariantCultureIgnoreCase));
            }
        }

        // Splits and matches the requiredRoles string to the available userRoles
        private bool MatchRolesCriteria(IEnumerable<string> userRoles, string requiredRoles)
        {
            return (requiredRoles ?? "").Split(";, ".ToArray(), StringSplitOptions.RemoveEmptyEntries)
                .All(r => userRoles.Contains(r, StringComparer.InvariantCultureIgnoreCase));
        }

        // Gathers a flattened collection of child nav items for the context
        private NavItem[] GatherSubNavItems(ControllerContext webContext, string subContext, List<string> parentContexts = null)
        {
            parentContexts = parentContexts ?? new List<string>();
            if (parentContexts.Contains(subContext)) return new NavItem[0];
            parentContexts.Add(subContext);
            var navs = GatherNavItems(webContext, subContext).ToList();
            foreach (var item in navs.Where(n => !string.IsNullOrWhiteSpace(n.SubNavContext)).ToArray()) {
                navs.AddRange(GatherSubNavItems(webContext, item.SubNavContext, parentContexts.ToList()));
            }
            return navs.ToArray();
        }

        // Clones and populates route values into each NavItem
        private void PopulateRouteValues(ControllerContext webContext, Dictionary<NavItem, NavItem[]> items)
        {
            foreach (var item in items) {
                var newRoutes = item.Key.RouteValues != null
                    ? new RouteValueDictionary(item.Key.RouteValues)
                    : new RouteValueDictionary();
                var routeValues = GetRouteValues(webContext);
                if (routeValues.ContainsKey(item.Key.Context) && routeValues[item.Key.Context] != null)
                {
                    foreach (var value in routeValues[item.Key.Context])
                    {
                        newRoutes[value.Key] = value.Value;
                    }
                }
                item.Key.RouteValues = newRoutes;
            }
        }
        #endregion
    }
}
