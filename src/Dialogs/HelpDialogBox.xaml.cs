using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Perfy.Dialogs
{
	/// <summary>
	/// Interaction logic for HelpDialogBox.xaml
	/// </summary>
	public partial class HelpDialogBox : Window
	{
		public HelpDialogBox()
		{
			InitializeComponent();
		}

		// tsk tsk, breaks mvvm
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			SubscribeToAllHyperlinks(this.Viewer.Document as FlowDocument);
		}

		#region Activate Hyperlinks in the Rich Text box
		//http://stackoverflow.com/questions/5465667/handle-all-hyperlinks-mouseenter-event-in-a-loaded-loose-flowdocument
		void SubscribeToAllHyperlinks(FlowDocument flowDocument)
		{
			var hyperlinks = GetVisuals(flowDocument).OfType<Hyperlink>();
			foreach (var link in hyperlinks)
				link.RequestNavigate += new System.Windows.Navigation.RequestNavigateEventHandler(link_RequestNavigate);
		}

		public static IEnumerable<DependencyObject> GetVisuals(DependencyObject root)
		{
			foreach (var child in LogicalTreeHelper.GetChildren(root).OfType<DependencyObject>())
			{
				yield return child;
				foreach (var descendants in GetVisuals(child))
					yield return descendants;
			}
		}

		void link_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
		{
			//http://stackoverflow.com/questions/2288999/how-can-i-get-a-flowdocument-hyperlink-to-launch-browser-and-go-to-url-in-a-wpf
			Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
			e.Handled = true;
		}
		#endregion Activate Hyperlinks in the Rich Text box
	}
}
