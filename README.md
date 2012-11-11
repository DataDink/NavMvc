NavMvc (0.0.1)
=====================================

by [Mark Nelson](http://www.markonthenet.com/)

This is a .NET MVC framework package that helps organize and orchestrate navigation.

What it is for:
---------------
* Simple to complicated navigation solutions
* Full-page navigations // No additional configurations required
* Single Page Applications (SPAs) // Additional configuration required

What it does:
-------------
Out of the box NavMvc is an attribute-based navigation system that requires very little work to get Actions plugged into menus and nav bars.
NavMvc can also be extended to support nearly any navigation requirement with minimal additional coding and configuration.

Out of the box:
---------------
* Wiring up standard full-page navigations/menus is very simple:

*Decorating Actions:*
```c#
public class SomeController : Controller
{
	[NavItem("MainNav", Title="Some Action")]
	public ActionResult SomeAction()
	{
		return View();
	}
}
```

*Rendering Navigations/Menus*
```html
<div id="primary-nav">
	@Html.Partial("_MvcNavigation", "MainNav");
</div>
```

###See the wiki for more info