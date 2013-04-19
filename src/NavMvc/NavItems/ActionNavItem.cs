namespace NavMvc.NavItems
{
    public class ActionNavItem : NavItem
    {
        /// <summary>
        /// The Action for this ActionNavItem
        /// </summary>
        public string Action { get; set; }
        /// <summary>
        /// The Controller for this ActionNavItem
        /// </summary>
        public string Controller { get; set; }
        /// <summary>
        /// True if this item or one of its descendants contains matching action/controller values to the current route info
        /// </summary>
        public bool IsActive { get; set; }
    }
}
