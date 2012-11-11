using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using NavMvc.NavItems;

namespace NavMvc.Providers
{
    public class ReflectionNavProvider : INavProvider
    {
        private Dictionary<string, List<NavItem>> _navItems;
 
        public NavItem[] GetNavItems(ControllerContext webContext, string navContext)
        {
            if (_navItems == null) _navItems = ReflectNavItems(webContext);
            if (!_navItems.ContainsKey(navContext)) return new NavItem[0];
            return _navItems[navContext].ToArray();
        }

        private Dictionary<string, List<NavItem>> ReflectNavItems(ControllerContext context)
        {
            var assembly = context.Controller.GetType().Assembly;
            var controllers = assembly.GetTypes().Where(t => typeof (IController).IsAssignableFrom(t)).ToArray();
            var actions = controllers.SelectMany(c => c.GetMethods());
            var attributes = actions
                .SelectMany(method => method.GetCustomAttributes(false)
                    .OfType<NavItemAttribute>()
                    .Where(a => a != null)
                    .Select(attr => new {
                        action = method,
                        attr = attr,
                        roles = ExtractRoles(method),
                    })).ToArray();

            var items = attributes.Select<dynamic, NavItem>(info => 
                new ReflectionNavItem {
                    Roles = info.roles,
                    Context = info.attr.Context ?? "",
                    SubNavContext = info.attr.SubNavContext ?? "",
                    Action = info.action.Name,
                    Controller = Regex.Replace(info.action.DeclaringType.Name, "Controller$", "", RegexOptions.IgnoreCase),
                    Title = info.attr.Title ?? "",
                    Description = info.attr.Description ?? "",
                    OrderingHint = info.attr.OrderingHint,
                    RenderContext = info.attr.RenderContext,
            });
            return items
                .GroupBy(i => i.Context)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        private string ExtractRoles(MethodInfo method)
        {
            var roles = method.GetCustomAttributes(false)
                .Concat(method.DeclaringType.GetCustomAttributes(false))
                .OfType<AuthorizeAttribute>()
                .Where(a => a != null)
                .SelectMany(r => r.Roles.Split(";, ".ToArray(), StringSplitOptions.RemoveEmptyEntries))
                .Select(r => r.Trim())
                .Distinct(StringComparer.InvariantCultureIgnoreCase)
                .ToArray();
            return string.Join(" ", roles);
        }
    }
}
