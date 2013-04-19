using System;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using NavMvc.Providers;

namespace NavMvc.Configuration
{
    public class NavMvcConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("", IsRequired=true, IsDefaultCollection=true)]
        public NavProviderCollection ProviderTypes { get { return this[""] as NavProviderCollection; } }

        private static NavMvcConfiguration _current;
        public static NavMvcConfiguration Current { get { return _current ?? (_current = (NavMvcConfiguration) ConfigurationManager.GetSection("navmvc")); } }

        private static INavProvider[] _providers;
        public static INavProvider[] Providers
        {
            get
            {
                return _providers 
                    ?? (_providers = Current.ProviderTypes.OfType<NavProvider>()
                    .Select(t => Activator.CreateInstance(t.Type) as INavProvider)
                    .Where(p => p != null)
                    .ToArray());
            }
        }
    }

    [ConfigurationCollection(typeof(NavProvider), AddItemName="add")]
    public class NavProviderCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new NavProvider();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((NavProvider)element).Name;
        }
    }

    public class NavProvider : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey=true, IsRequired=true)]
        public string Name { get { return base["name"] as string; } set { base["name"] = value; } }

        [TypeConverter(typeof(TypeNameConverter))]
        [ConfigurationProperty("type", IsRequired=true)]
        public Type Type { get { return base["type"] as Type; } set { base["type"] = value; } }
    }
}
