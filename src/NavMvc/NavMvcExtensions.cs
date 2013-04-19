using System.Collections.Generic;
using System.Linq;
using NavMvc.NavItems;
using NavMvc.Service;

namespace System.Web.Mvc
{
    public static class NavMvcExtensions
    {
        private static readonly INavigationService Service = NavigationService.Configured;

        #region GetNavigationFor
        /// <summary>
        /// Retrieves the navigation for the specified context
        /// </summary>
        /// <param name="context"></param>
        /// <param name="navContext">The context</param>
        /// <returns>An array of NavItems</returns>
        public static NavItem[] GetNavigationFor(this HtmlHelper context, string navContext)
        {
            var navigation = Service.GetNavItems(navContext);
            MarkActive(context.ViewContext.Controller.ControllerContext, navigation);
            return navigation;
        }

        /// <summary>
        /// Retrieves the navigation for the specified context
        /// </summary>
        /// <param name="context"></param>
        /// <param name="navContext">The context</param>
        /// <returns>An array of NavItems</returns>
        public static T[] GetNavigationFor<T>(this HtmlHelper context, string navContext) where T : NavItem
        {
            return GetNavigationFor(context, navContext).OfType<T>().ToArray();
        }

        private static bool MarkActive(ControllerContext context, IEnumerable<NavItem> items)
        {
            if (items == null) return false;
            var active = false;
            var action = context.RouteData.Values["action"] as string;
            var controller = context.RouteData.Values["controller"] as string;
            foreach (var item in items) {
                var actionItem = item as ActionNavItem;
                if (actionItem == null) continue;
                var isActive = actionItem.Action.Equals(action, StringComparison.InvariantCultureIgnoreCase)
                    && actionItem.Controller.Equals(controller, StringComparison.InvariantCultureIgnoreCase);
                actionItem.IsActive = isActive || MarkActive(context, Service.GetNavItems(item.SubNavContext));
                active = active || actionItem.IsActive;
            }
            return active;
        }
        #endregion

        #region SetNavValue
        /// <summary>
        /// Sets a value for the navigation context
        /// </summary>
        /// <param name="webContext"></param>
        /// <param name="navContext">The context</param>
        /// <param name="name">The name of the value</param>
        /// <param name="value">The value</param>
        public static void SetNavValue(this HtmlHelper webContext, string navContext, string name, object value)
        {
            Service.AddContextValue(navContext, name, value);
        }
        #endregion

        #region AddNavItem
        /// <summary>
        /// Sets a value for the navigation context
        /// </summary>
        /// <param name="webContext"></param>
        /// <param name="navContext">The context</param>
        /// <param name="item">The navigation item to be added</param>
        public static void AddNavItem(this HtmlHelper webContext, string navContext, NavItem item)
        {
            Service.AddNavItem(navContext, item);
        }
        #endregion
    }
}
