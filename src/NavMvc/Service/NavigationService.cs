using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using NavMvc.Configuration;
using NavMvc.NavItems;
using NavMvc.Providers;
using System.Web.Routing;

namespace NavMvc.Service
{
    public interface INavigationService {
        /// <summary>
        /// Adds a NavItem to the specified navigation context
        /// </summary>
        /// <param name="navContext">The context this NavItem belongs to</param>
        /// <param name="item">The NavItem to be added</param>
        void AddNavItem(string navContext, NavItem item);

        /// <summary>
        /// Removes a previously added NavItem from the specified navigation context
        /// </summary>
        /// <param name="navContext">The context the NavItem belongs to</param>
        /// <param name="item">The item to be removed</param>
        /// <returns>true on exists/success</returns>
        bool RemoveNavItem(string navContext, NavItem item);

        /// <summary>
        /// Adds a route value to be persisted for the specified context
        /// </summary>
        /// <param name="context">The navigation context this value belongs to</param>
        /// <param name="name">The name of this value (e.g. currentEntityId)</param>
        /// <param name="value">The value</param>
        void AddContextValue(string context, string name, object value);

        /// <summary>
        /// Removes a route value from the specified context
        /// </summary>
        /// <param name="context">The navigation context the value belongs to</param>
        /// <param name="name">The name of the value to be removed</param>
        void RemoveContextValue(string context, string name);

        /// <summary>
        /// Clears all navigation route values for the specified context
        /// </summary>
        /// <param name="context">The context to be purged</param>
        void ClearContextValues(string context);

        /// <summary>
        /// Clears all navigation route values
        /// </summary>
        void ClearAllContextValues();

        /// <summary>
        /// Gathers all nav items for the requested context
        /// </summary>
        /// <param name="navContext">The requested navigation context</param>
        /// <returns>The requested navigation items</returns>
        NavItem[] GetNavItems(string navContext);
    }

    public class NavigationService : INavigationService
    {
        private readonly INavProvider[] _providers;
        private readonly string _navRequestValuesKey = "NavMvcRequestValues" + Guid.NewGuid().ToString().Replace("-", "");

        private static readonly INavigationService _configured = new NavigationService();
        public static INavigationService Configured { get { return _configured; } }

        public NavigationService(INavProvider[] providers)
        {
            _providers = providers;
        }

        public NavigationService() : this(GetConfiguredProviders()) {}

        private static INavProvider[] GetConfiguredProviders()
        {
            return NavMvcConfiguration.Providers;
        }

        private readonly Dictionary<string, List<NavItem>> _navItems = new Dictionary<string, List<NavItem>>();

        /// <summary>
        /// Adds a NavItem to the specified navigation context
        /// </summary>
        /// <param name="navContext">The context this NavItem belongs to</param>
        /// <param name="item">The NavItem to be added</param>
        public void AddNavItem(string navContext, NavItem item)
        {
            if (!_navItems.ContainsKey(navContext)) _navItems.Add(navContext, new List<NavItem>());
            _navItems[navContext].Add(item);
        }

        /// <summary>
        /// Removes a previously added NavItem from the specified navigation context
        /// </summary>
        /// <param name="navContext">The context the NavItem belongs to</param>
        /// <param name="item">The item to be removed</param>
        /// <returns>true on exists/success</returns>
        public bool RemoveNavItem(string navContext, NavItem item)
        {
            if (!_navItems.ContainsKey(navContext)) return false;
            return _navItems[navContext].Remove(item);
        }

        private Dictionary<string, Dictionary<string, object>> GetRouteValues()
        {
            var webContext = HttpContext.Current;
            if (webContext == null) return new Dictionary<string, Dictionary<string, object>>();
            var values = webContext.Items[_navRequestValuesKey]
                as Dictionary<string, Dictionary<string, object>>
                ?? new Dictionary<string, Dictionary<string, object>>();
            webContext.Items[_navRequestValuesKey] = values;
            return values;
        }

        /// <summary>
        /// Adds a route value to be persisted for the specified context
        /// </summary>
        /// <param name="context">The navigation context this value belongs to</param>
        /// <param name="name">The name of this value (e.g. currentEntityId)</param>
        /// <param name="value">The value</param>
        public void AddContextValue(string context, string name, object value)
        {
            var routeValues = GetRouteValues();
            if (!routeValues.ContainsKey(context)) routeValues.Add(context, new Dictionary<string, object>());
            var ctx = routeValues[context];
            if (!ctx.ContainsKey(name)) ctx.Add(name, value);
            else ctx[name] = value;
        }

        /// <summary>
        /// Removes a route value from the specified context
        /// </summary>
        /// <param name="context">The navigation context the value belongs to</param>
        /// <param name="name">The name of the value to be removed</param>
        public void RemoveContextValue(string context, string name)
        {
            var routeValues = GetRouteValues();
            if (routeValues.ContainsKey(context) && routeValues[context].ContainsKey(name))
            {
                routeValues[context].Remove(name);
            }
        }

        /// <summary>
        /// Clears all navigation route values for the specified context
        /// </summary>
        /// <param name="context">The context to be purged</param>
        public void ClearContextValues(string context)
        {
            var routeValues = GetRouteValues();
            if (routeValues.ContainsKey(context))
            {
                routeValues.Remove(context);
            }
        }

        /// <summary>
        /// Clears all navigation route values
        /// </summary>
        public void ClearAllContextValues()
        {
            var routeValues = GetRouteValues();
            routeValues.Clear();
        }

        /// <summary>
        /// Gathers all nav items for the requested context
        /// </summary>
        /// <param name="navContext">The requested navigation context</param>
        /// <returns>The requested navigation items</returns>
        public NavItem[] GetNavItems(string navContext)
        {
            if (string.IsNullOrWhiteSpace(navContext)) return new NavItem[0];
            var items = GatherNavItems(navContext);
            items = FilterByRoles(items);
            PopulateRouteValues(items);
            return items;
        }

        // Gets NavItems from all sources
        private NavItem[] GatherNavItems(string navContext)
        {
            var items = _navItems.ContainsKey(navContext)
                ? _navItems[navContext].ToArray()
                : new NavItem[0];
            items = _providers.Aggregate(items,
                (current, provider) => current
                    .Concat(provider.GetNavItems(navContext) ?? new NavItem[0])
                    .ToArray())
                .Where(i => i != null)
                .Select(i => (NavItem)i.Clone())
                .ToArray();
            foreach (var item in items)
            {
                item.Context = navContext;
            }
            return items;
        }

        // Filters out NavItems that require roles the current user does not have
        private NavItem[] FilterByRoles(NavItem[] items)
        {
            string[] roles;
            try { roles = Roles.GetRolesForUser(); }
            catch { return items; }

            var allowedItems = items.Where(i => MatchRolesCriteria(roles, i.Roles)).ToArray();
            return allowedItems;
        }

        // Splits and matches the requiredRoles string to the available userRoles
        private bool MatchRolesCriteria(IEnumerable<string> userRoles, string requiredRoles)
        {
            return (requiredRoles ?? "").Split(";, ".ToArray(), StringSplitOptions.RemoveEmptyEntries)
                .All(r => userRoles.Contains(r, StringComparer.InvariantCultureIgnoreCase));
        }

        // Clones and populates route values into each NavItem
        private void PopulateRouteValues(NavItem[] items)
        {
            foreach (var item in items)
            {
                var newRoutes = item.RouteValues != null
                    ? new RouteValueDictionary(item.RouteValues)
                    : new RouteValueDictionary();
                var routeValues = GetRouteValues();
                if (routeValues.ContainsKey(item.Context) && routeValues[item.Context] != null)
                {
                    foreach (var value in routeValues[item.Context])
                    {
                        newRoutes[value.Key] = value.Value;
                    }
                }
                item.RouteValues = newRoutes;
            }
        }
    }
}
