NavMvc (Down and Dirty)

For detailed information please visit: https://github.com/DataDink/NavMvc
For questions, comments, non-hate mail: http://www.markonthenet.com/

 * How to add a main navigation bar to your site using NavMvc:

	<html> <!-- This is your pretend site -->
		<body>
			<header>
				<div id="main-nav"> <!-- Watch closely: here it is... -->
					@Html.RenderPartial("_MvcNavigation", "MainNav");
				</div>
			</header>
			<section id="main-content">
				@RenderBody();
			</section>
			<footer>
			</footer>
		</body>
	</html>

 * How to add a button to your new main navigation bar

		public class SomeController : Controller
		{
			[NavItem("MainNav", Title = "Home")] // This attribute - that's it
			public ActionResult Index()
			{
				return View();
			}
		}


 * How to style your new navigation bar (really poorly)

		.navigation {
			display: block;
			list-style: none;
			padding: 0px;
			margin: 0px;
		}

		.navigation > li {
			display: inline-block;
			padding: 0px;
			margin: 5px;
		}

		.navigation > li > a,
		.navigation > li > span {
			display: block;
			width: 100%;
			height: 100%;
		}

		#main-nav {
			width: 500px;
			margin: auto;
		}