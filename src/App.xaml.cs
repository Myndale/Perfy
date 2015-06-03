using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Perfy
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		DateTime start = DateTime.Now;

		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);

			var finish = DateTime.Now;
			Console.WriteLine(String.Format("Elapsed time: {0} ms", (finish - start).TotalMilliseconds));
		}
	}
}
