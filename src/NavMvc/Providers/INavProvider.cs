using System.Web.Mvc;
using NavMvc.NavItems;

namespace NavMvc.Providers
{
    public interface INavProvider
    {
        NavItem[] GetNavItems(string navContext);
    }
}
