using System;
using System.ComponentModel;
using System.Configuration;

namespace NavMvc.Configuration
{
    public class NavMvcConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsRequired=true, IsDefaultCollection=true)]
        public NavProviderCollection Providers { get { return this[""] as NavProviderCollection; } set { this[""] = value; } }

        [ConfigurationProperty("inactiveNavBehavior", DefaultValue = InactiveNavBehaviors.AlwaysHide)]
        public InactiveNavBehaviors InactiveNavBehavior { get { return (InactiveNavBehaviors)base["inactiveNavBehavior"]; } set { base["inactiveNavBehavior"] = value; } }
    }

    public class NavProviderCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement() { return new NavProviderCollection(); }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((NavProviderCollection)element).Name;
        }

        [ConfigurationProperty("name", IsKey=true, IsRequired=true)]
        public string Name { get { return base["name"] as string; } set { base["name"] = value; } }

        [TypeConverter(typeof(TypeNameConverter))]
        [ConfigurationProperty("type", IsRequired=true)]
        public Type Type { get { return base["type"] as Type; } set { base["type"] = value; } }
    }

    public enum InactiveNavBehaviors
    {
        AlwaysHide,
        ShowIfActiveChild,
        RedirectToFirstActive,
    }
}
