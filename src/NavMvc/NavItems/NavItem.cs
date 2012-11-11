using System;
using System.Web.Routing;

namespace NavMvc.NavItems
{
    /// <summary>
    /// The most basic information used to describe a navigation item
    /// </summary>
    public class NavItem : ICloneable
    {
        /// <summary>
        /// The context this NavItem belongs to
        /// </summary>
        public string Context { get; set; }
        /// <summary>
        /// The child context of this NavItem
        /// </summary>
        public string SubNavContext { get; set; }
        /// <summary>
        /// The roles required for this NavItem
        /// </summary>
        public string Roles { get; set; }
        /// <summary>
        /// The title or header for this NavItem
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// The description or additional text for this NavItem
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// The ordering hint for this NavItem
        /// </summary>
        public int OrderingHint { get; set; }
        /// <summary>
        /// The route values for this nav item in the current context
        /// </summary>
        public RouteValueDictionary RouteValues { get; set; }
        /// <summary>
        /// NavItems are cloned before delivery to help prevent member cross-talk
        /// </summary>
        /// <summary>
        /// True if this NavItem or one of its children's Action and Controller match the current routing information.
        /// </summary>
        public bool IsActive { get; set; }
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
