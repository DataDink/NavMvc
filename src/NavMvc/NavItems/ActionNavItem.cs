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
    }
}
