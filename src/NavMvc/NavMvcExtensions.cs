using System.Linq;
using System.Web.Mvc;
using NavMvc.Engine;
using NavMvc.NavItems;

namespace NavMvc
{
    public static class NavMvcExtensions
    {
        #region GetNavigationFor
        /// <summary>
        /// Retrieves the navigation for the specified context
        /// </summary>
        /// <param name="webContext"></param>
        /// <param name="navContext">The context</param>
        /// <returns>An array of NavItems</returns>
        public static NavItem[] GetNavigationFor(this HtmlHelper webContext, string navContext)
        {
            return GetNavigationFor(webContext.ViewContext.Controller.ControllerContext, navContext);
        }

        /// <summary>
        /// Retrieves the navigation for the specified context
        /// </summary>
        /// <param name="webContext"></param>
        /// <param name="navContext">The context</param>
        /// <returns>An array of NavItems</returns>
        public static T[] GetNavigationFor<T>(this HtmlHelper webContext, string navContext) where T : NavItem
        {
            return GetNavigationFor<T>(webContext.ViewContext.Controller.ControllerContext, navContext);
        }

        /// <summary>
        /// Retrieves the navigation for the specified context
        /// </summary>
        /// <param name="webContext"></param>
        /// <param name="navContext">The context</param>
        /// <returns>An array of NavItems</returns>
        public static NavItem[] GetNavigationFor(this ViewContext webContext, string navContext)
        {
            return GetNavigationFor(webContext.Controller.ControllerContext, navContext);
        }

        /// <summary>
        /// Retrieves the navigation for the specified context
        /// </summary>
        /// <param name="webContext"></param>
        /// <param name="navContext">The context</param>
        /// <returns>An array of NavItems</returns>
        public static T[] GetNavigationFor<T>(this ViewContext webContext, string navContext) where T : NavItem
        {
            return GetNavigationFor<T>(webContext.Controller.ControllerContext, navContext);
        }

        /// <summary>
        /// Retrieves the navigation for the specified context
        /// </summary>
        /// <param name="webContext"></param>
        /// <param name="navContext">The context</param>
        /// <returns>An array of NavItems</returns>
        public static NavItem[] GetNavigationFor(this ControllerBase webContext, string navContext)
        {
            return GetNavigationFor(webContext.ControllerContext, navContext);
        }

        /// <summary>
        /// Retrieves the navigation for the specified context
        /// </summary>
        /// <param name="webContext"></param>
        /// <param name="navContext">The context</param>
        /// <returns>An array of NavItems</returns>
        public static T[] GetNavigationFor<T>(this ControllerBase webContext, string navContext) where T : NavItem
        {
            return GetNavigationFor<T>(webContext.ControllerContext, navContext);
        }

        /// <summary>
        /// Retrieves the navigation for the specified context
        /// </summary>
        /// <param name="webContext"></param>
        /// <param name="navContext">The context</param>
        /// <returns>An array of NavItems</returns>
        public static NavItem[] GetNavigationFor(this ControllerContext webContext, string navContext)
        {
            return GetNavigationFor<NavItem>(webContext, navContext);
        }

        /// <summary>
        /// Retrieves the navigation for the specified context
        /// </summary>
        /// <param name="webContext"></param>
        /// <param name="navContext">The context</param>
        /// <returns>An array of NavItems</returns>
        public static T[] GetNavigationFor<T>(this ControllerContext webContext, string navContext) where T : NavItem
        {
            return NavMvcFactory.Current.GetNavItems(webContext, navContext).OfType<T>().ToArray();
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
            NavMvcFactory.Current.AddContextValue(navContext, name, value);
        }

        /// <summary>
        /// Sets a value for the navigation context
        /// </summary>
        /// <param name="webContext"></param>
        /// <param name="navContext">The context</param>
        /// <param name="name">The name of the value</param>
        /// <param name="value">The value</param>
        public static void SetNavValue(this ViewContext webContext, string navContext, string name, object value)
        {
            NavMvcFactory.Current.AddContextValue(navContext, name, value);
        }

        /// <summary>
        /// Sets a value for the navigation context
        /// </summary>
        /// <param name="webContext"></param>
        /// <param name="navContext">The context</param>
        /// <param name="name">The name of the value</param>
        /// <param name="value">The value</param>
        public static void SetNavValue(this ControllerBase webContext, string navContext, string name, object value)
        {
            NavMvcFactory.Current.AddContextValue(navContext, name, value);
        }

        /// <summary>
        /// Sets a value for the navigation context
        /// </summary>
        /// <param name="webContext"></param>
        /// <param name="navContext">The context</param>
        /// <param name="name">The name of the value</param>
        /// <param name="value">The value</param>
        public static void SetNavValue(this ControllerContext webContext, string navContext, string name, object value)
        {
            NavMvcFactory.Current.AddContextValue(navContext, name, value);
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
            NavMvcFactory.Current.AddNavItem(navContext, item);
        }

        /// <summary>
        /// Sets a value for the navigation context
        /// </summary>
        /// <param name="webContext"></param>
        /// <param name="navContext">The context</param>
        /// <param name="item">The navigation item to be added</param>
        public static void AddNavItem(this ViewContext webContext, string navContext, NavItem item)
        {
            NavMvcFactory.Current.AddNavItem(navContext, item);
        }

        /// <summary>
        /// Sets a value for the navigation context
        /// </summary>
        /// <param name="webContext"></param>
        /// <param name="navContext">The context</param>
        /// <param name="item">The navigation item to be added</param>
        public static void AddNavItem(this ControllerBase webContext, string navContext, NavItem item)
        {
            NavMvcFactory.Current.AddNavItem(navContext, item);
        }

        /// <summary>
        /// Sets a value for the navigation context
        /// </summary>
        /// <param name="webContext"></param>
        /// <param name="navContext">The context</param>
        /// <param name="item">The navigation item to be added</param>
        public static void AddNavItem(this ControllerContext webContext, string navContext, NavItem item)
        {
            NavMvcFactory.Current.AddNavItem(navContext, item);
        }
        #endregion
    }
}
