using System;

namespace NavMvc
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class NavItemAttribute : Attribute
    {
        public string Context { get; private set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string SubNavContext { get; set; }
        public int OrderingHint { get; set; }
        public object RenderContext { get; set; }

        public NavItemAttribute(string context)
        {
            Context = context;
        }
    }
}
