using Perfy.ViewModel;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Perfy.View
{
	// Really should be using proper MVVM here but we need the extra performance. Might be able to clean it up later.

	public partial class CellControl : UserControl
	{
		const double ThinTrace = 2;
		const double MediumTrace = 5;
		const double ThickTrace = 10;

		public MainViewModel MainViewModel
		{
			get { return (MainViewModel)GetValue(MainViewModelProperty); }
			set { SetValue(MainViewModelProperty, value); }
		}

		public static readonly DependencyProperty MainViewModelProperty =
			DependencyProperty.Register("MainViewModel", typeof(MainViewModel), typeof(CellControl), new PropertyMetadata(null, OnMainViewModelChanged));

		public static readonly DependencyProperty BoundDataContextProperty = DependencyProperty.Register(
			"BoundDataContext",
			typeof(object),
			typeof(CellControl),
			new PropertyMetadata(null, OnBoundDataContextChanged));

		static private Brush NormalPadBrush = (Brush)Application.Current.Resources["NormalPadBrush"];
		static private Brush NormalComponentBrush = (Brush)Application.Current.Resources["NormalComponentBrush"];
		static private Brush NormalHorzBrush = (Brush)Application.Current.Resources["NormalHorzBrush"];
		static private Brush NormalVertBrush = (Brush)Application.Current.Resources["NormalVertBrush"];
		static private Brush NormalHorzVertBrush = (Brush)Application.Current.Resources["NormalHorzVertBrush"];
		static private Brush HighlightedComponentBrush = (Brush)Application.Current.Resources["HighlightedComponentBrush"];
		static private Brush HighlightedHorzBrush = (Brush)Application.Current.Resources["HighlightedHorzBrush"];
		static private Brush HighlightedVertBrush = (Brush)Application.Current.Resources["HighlightedVertBrush"];
		static private Brush HighlightedHorzVertBrush = (Brush)Application.Current.Resources["HighlightedHorzVertBrush"];
		static private Brush HighlightBrush = (Brush)Application.Current.Resources["HighlightBrush"];
		static private Brush BackgroundBrush = (Brush)Application.Current.Resources["BackgroundBrush"];
		
		public CellControl()
		{
			InitializeComponent();
			this.SetBinding(BoundDataContextProperty, new Binding());
		}

		private static void OnMainViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var control = d as CellControl;
			var pad = e.OldValue as MainViewModel;
			if (pad != null)
				pad.PropertyChanged -= control.MainViewModelChanged;
			pad = e.NewValue as MainViewModel;
			if (pad != null)
				pad.PropertyChanged += control.MainViewModelChanged;
			control.UpdateControl();
		}

		void MainViewModelChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if ((e.PropertyName == "ViewMode") || (e.PropertyName == "Perspective"))
				UpdateControl();
		}

		private static void OnBoundDataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var control = d as CellControl;
			var pad = e.OldValue as PadViewModel;
			if (pad != null)
				pad.PropertyChanged -= control.BoundDataContextChanged;
			pad = e.NewValue as PadViewModel;
			if (pad != null)
				pad.PropertyChanged += control.BoundDataContextChanged;
			control.UpdateControl();
		}

		void BoundDataContextChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			UpdateControl();
		}

		private void UpdateControl()
		{
			var mvm = this.MainViewModel;
			if (mvm.ViewMode == ViewMode.Normal)
				UpdateNormal();
			else if (mvm.ViewMode == ViewMode.Cuts)
			{
				if (mvm.Perspective == Perspective.Front)
					UpdateHorzCuts();
				else
					UpdateVertCuts();
			}
			else if (mvm.ViewMode == ViewMode.Pads)
			{
				if (mvm.Perspective == Perspective.Front)
					UpdateHorzPads();
				else
					UpdateVertPads();
			}
			else if (mvm.ViewMode == ViewMode.Template)
			{
				if (mvm.Perspective == Perspective.Front)
					UpdateHorzTemplate();
				else
					UpdateVertTemplate();
			}
		}

		private void UpdateNormal()
		{
			var mvm = this.MainViewModel;
			var pad = this.DataContext as PadViewModel;
			if (pad == null)
				return;

			this.HorzGuide.Fill = NormalPadBrush;			
			this.HorzGuide.Visibility = Visibility.Visible;
			this.HorzGuide.Height = ThinTrace;
			this.VertGuide.Visibility = Visibility.Visible;
			this.VertGuide.Width = ThinTrace;
			this.VertGuide.Fill = NormalPadBrush;

			this.HorzConn.Visibility = pad.HorzPad ? Visibility.Visible : Visibility.Hidden;
			this.HorzConn.Fill = pad.HorzHighlighted ? HighlightedHorzBrush : NormalHorzBrush;
			this.HorzConn.StrokeThickness = 0;
			this.VertConn.Visibility = pad.VertPad ? Visibility.Visible : Visibility.Hidden;
			this.VertConn.Fill = pad.VertHighlighted ? HighlightedVertBrush : NormalVertBrush;
			this.VertConn.StrokeThickness = 0;
			this.HorzVertConn.Visibility = (pad.HorzPad && pad.VertPad) ? Visibility.Visible : Visibility.Hidden;
			this.HorzVertConn.Fill = pad.HorzHighlighted ? HighlightedHorzVertBrush : NormalHorzVertBrush;			

			this.MainPad.Visibility = Visibility.Visible;
			this.PadHole.Fill = BackgroundBrush;
			this.PadHole.StrokeThickness = 0;
			if (pad.Component)
			{
				if (pad.HorzPad && pad.HorzHighlighted)
					this.MainPad.Fill = HighlightedComponentBrush;
				else if (pad.VertPad && pad.VertHighlighted)
					this.MainPad.Fill = HighlightedComponentBrush;
				else
					this.MainPad.Fill = NormalComponentBrush;
			}
			else
			{
				if (pad.HorzPad && pad.VertPad)
					this.MainPad.Fill = pad.HorzHighlighted ? HighlightedHorzVertBrush : NormalHorzVertBrush;
				else if (pad.HorzPad)
					this.MainPad.Fill = pad.HorzHighlighted ? HighlightedHorzBrush : NormalHorzBrush;
				else if (pad.VertPad)
					this.MainPad.Fill = pad.VertHighlighted ? HighlightedVertBrush : NormalVertBrush;
				else
					this.MainPad.Fill = NormalPadBrush;
			}

			this.HorzCut.Visibility = Visibility.Hidden;
			this.VertCut.Visibility = Visibility.Hidden;

			this.HorzTrace.Visibility = pad.HorzTrace ? Visibility.Visible : Visibility.Hidden;
			this.HorzTrace.Fill = pad.HorzHighlighted ? HighlightedHorzBrush : NormalHorzBrush;
			this.HorzJunction.Visibility = pad.HorzJunction ? Visibility.Visible : Visibility.Hidden;
			this.HorzJunction.Fill = pad.HorzHighlighted ? HighlightedHorzBrush : NormalHorzBrush;
			this.VertTrace.Visibility = pad.VertTrace ? Visibility.Visible : Visibility.Hidden;
			this.VertTrace.Fill = pad.VertHighlighted ? HighlightedVertBrush : NormalVertBrush;
			this.VertJunction.Visibility = pad.VertJunction ? Visibility.Visible : Visibility.Hidden;
			this.VertJunction.Fill = pad.VertHighlighted ? HighlightedVertBrush : NormalVertBrush;

			this.Junction.Visibility = (pad.HorzJunction && pad.VertJunction) ? Visibility.Visible : Visibility.Hidden;
			if (pad.HorzHighlighted && pad.VertHighlighted)
				this.Junction.Fill = HighlightedHorzVertBrush;
			else if (pad.HorzHighlighted)
				this.Junction.Fill = HighlightedHorzBrush;
			else if (pad.VertHighlighted)
				this.Junction.Fill = HighlightedVertBrush;
			else
				this.Junction.Fill = NormalHorzVertBrush;

			this.HorzHighlight.Visibility = pad.HorzHighlight ? Visibility.Visible : Visibility.Hidden;
			this.VertHighlight.Visibility = pad.VertHighlight ? Visibility.Visible : Visibility.Hidden;
		}

		private void UpdateHorzCuts()
		{
			var mvm = this.MainViewModel;
			var pad = this.DataContext as PadViewModel;
			if (pad == null)
				return;

			this.HorzGuide.Visibility = Visibility.Visible;
			this.HorzGuide.Height = ThickTrace;
			this.HorzGuide.Fill = NormalPadBrush;			
			this.VertGuide.Visibility = Visibility.Visible;
			this.VertGuide.Width = ThinTrace;
			this.VertGuide.Fill = NormalPadBrush;			

			this.HorzConn.Visibility = Visibility.Hidden;
			this.VertConn.Visibility = Visibility.Hidden;
			this.HorzVertConn.Visibility = Visibility.Hidden;

			this.MainPad.Visibility = Visibility.Visible;
			this.PadHole.Fill = BackgroundBrush;
			this.PadHole.StrokeThickness = 0;
			if (pad.Component)
				this.MainPad.Fill = NormalComponentBrush;
			else
				this.MainPad.Fill = NormalPadBrush;

			this.HorzCut.Visibility = pad.HorzCut ? Visibility.Visible : Visibility.Hidden;
			this.HorzCut.StrokeThickness = ThinTrace;
			this.HorzCut.Fill = HighlightedHorzBrush;
			this.VertCut.Visibility = Visibility.Hidden;

			this.HorzTrace.Visibility = Visibility.Hidden;
			this.HorzJunction.Visibility = Visibility.Hidden;
			this.VertTrace.Visibility = Visibility.Hidden;
			this.VertJunction.Visibility = Visibility.Hidden;

			this.Junction.Visibility = Visibility.Hidden;

			this.HorzHighlight.Visibility = Visibility.Hidden;
			this.VertHighlight.Visibility = Visibility.Hidden;
		}

		private void UpdateVertCuts()
		{
			var mvm = this.MainViewModel;
			var pad = this.DataContext as PadViewModel;
			if (pad == null)
				return;

			this.HorzGuide.Visibility = Visibility.Visible;
			this.HorzGuide.Height = ThinTrace;
			this.HorzGuide.Fill = NormalPadBrush;
			this.VertGuide.Visibility = Visibility.Visible;
			this.VertGuide.Width = ThickTrace;
			this.VertGuide.Fill = NormalPadBrush;

			this.HorzConn.Visibility = Visibility.Hidden;
			this.VertConn.Visibility = Visibility.Hidden;
			this.HorzVertConn.Visibility = Visibility.Hidden;

			this.MainPad.Visibility = Visibility.Visible;
			this.PadHole.Fill = BackgroundBrush;
			this.PadHole.StrokeThickness = 0;
			if (pad.Component)
				this.MainPad.Fill = NormalComponentBrush;
			else
				this.MainPad.Fill = NormalPadBrush;

			this.HorzCut.Visibility = Visibility.Hidden;
			this.VertCut.Visibility = pad.VertCut ? Visibility.Visible : Visibility.Hidden;
			this.VertCut.Fill = HighlightedVertBrush;
			this.VertCut.StrokeThickness = ThinTrace;

			this.HorzTrace.Visibility = Visibility.Hidden;
			this.HorzJunction.Visibility = Visibility.Hidden;
			this.VertTrace.Visibility = Visibility.Hidden;
			this.VertJunction.Visibility = Visibility.Hidden;

			this.Junction.Visibility = Visibility.Hidden;

			this.HorzHighlight.Visibility = Visibility.Hidden;
			this.VertHighlight.Visibility = Visibility.Hidden;
		}

		private void UpdateHorzPads()
		{
			var mvm = this.MainViewModel;
			var pad = this.DataContext as PadViewModel;
			if (pad == null)
				return;

			this.HorzGuide.Visibility = Visibility.Visible;
			this.HorzGuide.Height = ThinTrace;
			this.HorzGuide.Fill = NormalPadBrush;
			this.VertGuide.Visibility = Visibility.Visible;
			this.VertGuide.Width = ThinTrace;
			this.VertGuide.Fill = NormalPadBrush;

			this.HorzConn.Visibility = pad.HorzPad ? Visibility.Visible : Visibility.Hidden;
			this.HorzConn.StrokeThickness = 0;
			this.HorzConn.Fill = NormalHorzBrush;
			this.VertConn.Visibility = Visibility.Hidden;
			this.HorzVertConn.Visibility = Visibility.Hidden;

			this.MainPad.Visibility = Visibility.Visible;
			this.PadHole.Fill = BackgroundBrush;
			this.PadHole.StrokeThickness = 0;
			if (pad.Component)
				this.MainPad.Fill = NormalComponentBrush;
			else
			{
				if (pad.HorzPad)
					this.MainPad.Fill = NormalHorzBrush;
				else
					this.MainPad.Fill = NormalPadBrush;
			}

			this.HorzCut.Visibility = Visibility.Hidden;
			this.VertCut.Visibility = Visibility.Hidden;

			this.HorzTrace.Visibility = Visibility.Hidden;
			this.HorzJunction.Visibility = Visibility.Hidden;
			this.VertTrace.Visibility = Visibility.Hidden;
			this.VertJunction.Visibility = Visibility.Hidden;

			this.Junction.Visibility = Visibility.Hidden;

			this.HorzHighlight.Visibility = Visibility.Hidden;
			this.VertHighlight.Visibility = Visibility.Hidden;
		}

		private void UpdateVertPads()
		{
			var mvm = this.MainViewModel;
			var pad = this.DataContext as PadViewModel;
			if (pad == null)
				return;

			this.HorzGuide.Visibility = Visibility.Visible;
			this.HorzGuide.Height = ThinTrace;
			this.HorzGuide.Fill = NormalPadBrush;
			this.VertGuide.Visibility = Visibility.Visible;
			this.VertGuide.Width = ThinTrace;
			this.VertGuide.Fill = NormalPadBrush;

			this.HorzConn.Visibility = Visibility.Hidden;
			this.VertConn.Visibility = pad.VertPad ? Visibility.Visible : Visibility.Hidden;
			this.VertConn.Fill = NormalVertBrush;
			this.VertConn.StrokeThickness = 0;
			this.HorzVertConn.Visibility = Visibility.Hidden;

			this.MainPad.Visibility = Visibility.Visible;
			this.PadHole.Fill = BackgroundBrush;
			this.PadHole.StrokeThickness = 0;
			if (pad.Component)
				this.MainPad.Fill = NormalComponentBrush;
			else
			{
				if (pad.VertPad)
					this.MainPad.Fill = NormalVertBrush;
				else
					this.MainPad.Fill = NormalPadBrush;
			}

			this.HorzCut.Visibility = Visibility.Hidden;
			this.VertCut.Visibility = Visibility.Hidden;

			this.HorzTrace.Visibility = Visibility.Hidden;
			this.HorzJunction.Visibility = Visibility.Hidden;
			this.VertTrace.Visibility = Visibility.Hidden;
			this.VertJunction.Visibility = Visibility.Hidden;

			this.Junction.Visibility = Visibility.Hidden;

			this.HorzHighlight.Visibility = Visibility.Hidden;
			this.VertHighlight.Visibility = Visibility.Hidden;
		}

		private void UpdateHorzTemplate()
		{
			var mvm = this.MainViewModel;
			var pad = this.DataContext as PadViewModel;
			if (pad == null)
				return;

			this.HorzGuide.Visibility = Visibility.Visible;
			this.HorzGuide.Height = ThinTrace;
			this.HorzGuide.Fill = Brushes.Black;
			this.VertGuide.Visibility = Visibility.Visible;
			this.VertGuide.Width = ThinTrace;
			this.VertGuide.Fill = Brushes.Black;

			this.HorzConn.Visibility = pad.HorzPad ? Visibility.Visible : Visibility.Hidden;
			this.HorzConn.Fill = Brushes.White;
			this.HorzConn.Stroke = Brushes.Black;
			this.HorzConn.StrokeThickness = MediumTrace;
			this.VertConn.Visibility = Visibility.Hidden;
			this.HorzVertConn.Visibility = Visibility.Hidden;

			this.MainPad.Visibility = Visibility.Hidden;
			this.PadHole.Fill = Brushes.White;
			if (pad.Component)
				this.PadHole.StrokeThickness = MediumTrace;
			else
				this.PadHole.StrokeThickness = 0;
			
			this.HorzCut.Visibility = pad.HorzCut ? Visibility.Visible : Visibility.Hidden;
			this.HorzCut.Fill = Brushes.White;
			this.HorzCut.StrokeThickness = MediumTrace;
			this.VertCut.Visibility = Visibility.Hidden;

			this.HorzTrace.Visibility = Visibility.Hidden;
			this.HorzJunction.Visibility = Visibility.Hidden;
			this.VertTrace.Visibility = Visibility.Hidden;
			this.VertJunction.Visibility = Visibility.Hidden;

			this.Junction.Visibility = Visibility.Hidden;

			this.HorzHighlight.Visibility = Visibility.Hidden;
			this.VertHighlight.Visibility = Visibility.Hidden;
		}

		private void UpdateVertTemplate()
		{
			var mvm = this.MainViewModel;
			var pad = this.DataContext as PadViewModel;
			if (pad == null)
				return;

			this.HorzGuide.Visibility = Visibility.Visible;
			this.HorzGuide.Height = ThinTrace;
			this.HorzGuide.Fill = Brushes.Black;
			this.VertGuide.Visibility = Visibility.Visible;
			this.VertGuide.Width = ThinTrace;
			this.VertGuide.Fill = Brushes.Black;

			this.HorzConn.Visibility = Visibility.Hidden;			
			this.VertConn.Visibility = pad.VertPad ? Visibility.Visible : Visibility.Hidden;
			this.VertConn.Fill = Brushes.White;
			this.VertConn.Stroke = Brushes.Black;
			this.VertConn.StrokeThickness = MediumTrace;
			this.HorzVertConn.Visibility = Visibility.Hidden;

			this.MainPad.Visibility = Visibility.Hidden;
			this.PadHole.Fill = Brushes.White;
			if (pad.Component)
				this.PadHole.StrokeThickness = MediumTrace;
			else
				this.PadHole.StrokeThickness = 0;

			this.HorzCut.Visibility = Visibility.Hidden;
			this.VertCut.Visibility = pad.VertCut ? Visibility.Visible : Visibility.Hidden;
			this.VertCut.Fill = Brushes.White;
			this.VertCut.StrokeThickness = MediumTrace;

			this.HorzTrace.Visibility = Visibility.Hidden;
			this.HorzJunction.Visibility = Visibility.Hidden;
			this.VertTrace.Visibility = Visibility.Hidden;
			this.VertJunction.Visibility = Visibility.Hidden;

			this.Junction.Visibility = Visibility.Hidden;

			this.HorzHighlight.Visibility = Visibility.Hidden;
			this.VertHighlight.Visibility = Visibility.Hidden;
		}

	}
}
