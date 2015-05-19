using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MvvmDialogs.ViewModels;
using Perfy.Dialogs;
using Perfy.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml.Serialization;

namespace Perfy.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
		private IDialogViewModelCollection _Dialogs = new DialogViewModelCollection();
		public IDialogViewModelCollection Dialogs { get { return _Dialogs; } }

		private CircuitViewModel _Board;
		public CircuitViewModel Board
		{
			get { return this._Board; }
			set { this._Board = value; RaisePropertyChanged(() => this.Board); }
		}

		private PadViewModel CurrentPad = null;
		private string LastFilename = null;

		private double _Zoom;
		public double Zoom
		{
			get { return this._Zoom; }
			set { this._Zoom = value; RaisePropertyChanged(() => this.Zoom); }
		}

		private double _ZoomLevel;
		private double ZoomLevel
		{
			get { return this._ZoomLevel; }
			set
			{
				this._ZoomLevel = value;
				this.Zoom = Math.Pow(Math.Sqrt(2), value);
			}
		}

		private bool _HorzLayer = true;
		public bool HorzLayer
		{
			get { return this._HorzLayer; }
			set { this._HorzLayer = value; RaisePropertyChanged(() => this.HorzLayer); }
		}

		private bool _VertLayer;
		public bool VertLayer
		{
			get { return this._VertLayer; }
			set { this._VertLayer = value; RaisePropertyChanged(() => this.VertLayer); }
		}

		private bool _BothLayers;
		public bool BothLayers
		{
			get { return this._BothLayers; }
			set { this._BothLayers = value; RaisePropertyChanged(() => this.BothLayers); }
		}

		public ICommand ZoomInCommand { get { return new RelayCommand(OnZoomIn); } }
		private void OnZoomIn()
		{
			this.ZoomLevel++;
			if (this.ZoomLevel > 16)
				this.ZoomLevel = 16;
		}

		public ICommand ZoomOutCommand { get { return new RelayCommand(OnZoomOut); } }
		private void OnZoomOut()
		{
			this.ZoomLevel--;
			if (this.ZoomLevel < 1)
				this.ZoomLevel = 1;
		}

		public ICommand HoleSelectedCommand { get { return new RelayCommand<PadViewModel>(OnHoleSelected); } }
		private void OnHoleSelected(PadViewModel pad)
		{
			// todo: checking the mouse button states here is an MVVM violation, needs to be fixed.
			if (Mouse.LeftButton == MouseButtonState.Pressed)
			{
				pad.HorzHole |= (this.HorzLayer | this.BothLayers);
				pad.VertHole |= (this.VertLayer | this.BothLayers);
			}
			else if (Mouse.RightButton == MouseButtonState.Pressed)
			{
				pad.HorzHole &= !(this.HorzLayer | this.BothLayers);
				pad.VertHole &= !(this.VertLayer | this.BothLayers);
			}

			this.Board.UpdateNodes();
			UpdateSelections();
		}

		public ICommand HoleOverCommand { get { return new RelayCommand<PadViewModel>(OnHoleOver); } }
		private void OnHoleOver(PadViewModel pad)
		{
			this.CurrentPad = pad;
			UpdateSelections();
		}

		private void UpdateSelections()
		{
			if (this.CurrentPad == null)
			{
				this.Board.Select(null);
				return;
			}
			var node = this.CurrentPad.HorzTrace.Node;
			if (this.CurrentPad.HorzHole)
				this.Board.Select(this.CurrentPad.HorzTrace.Node);
			else if (this.CurrentPad.VertHole)
				this.Board.Select(this.CurrentPad.VertTrace.Node);
			else
				this.Board.Select(null);
		}

		public ICommand JunctionSelectedCommand { get { return new RelayCommand<PadViewModel>(OnJunctionSelected); } }
		private void OnJunctionSelected(PadViewModel pad)
		{
			// todo: checking the mouse button states here is an MVVM violation, needs to be fixed.
			if (Mouse.LeftButton == MouseButtonState.Pressed)
			{
				pad.HorzJunction &= !(this.HorzLayer | this.BothLayers);
				pad.VertJunction &= !(this.VertLayer | this.BothLayers);
				
			}
			else if (Mouse.RightButton == MouseButtonState.Pressed)
			{
				pad.HorzJunction |= (this.HorzLayer | this.BothLayers);
				pad.VertJunction |= (this.VertLayer | this.BothLayers);
			}

			this.Board.UpdateNodes();
			UpdateSelections();
		}

		public ICommand PreviewKeyDownCommand { get { return new RelayCommand<System.Windows.Input.KeyEventArgs>(OnPreviewKeyDown); } }
		private void OnPreviewKeyDown(System.Windows.Input.KeyEventArgs args)
		{
			if (args.Key == Key.Add)
			{
				ZoomInCommand.Execute(null);
				args.Handled = true;
			}
			else if (args.Key == Key.Subtract)
			{
				ZoomOutCommand.Execute(null);
				args.Handled = true;
			}
		}
		
        public MainViewModel()
        {
			this.LastFilename = null;
			this.ZoomLevel = 10;
			this.Board = new CircuitViewModel { Circuit = new Circuit() };
			UpdateSelections();
		}

		public ICommand NewCommand { get { return new RelayCommand(OnNew); } }
		private void OnNew()
		{
			this.LastFilename = null;
			this.ZoomLevel = 10;
			this.CurrentPad = null;
			this.Board.Circuit = new Circuit();
			UpdateSelections();
		}

		public ICommand OpenCommand { get { return new RelayCommand(OnOpen); } }
		private void OnOpen()
		{
			var dlg = new OpenFileDialogViewModel
			{
				Title = "Load Perfy Layout",
				Filter = "Perfy files (*.pfp)|*.pfp|All files (*.*)|*.*",
				FileName = "*.pfp",
				Multiselect = false
			};

			if (dlg.Show(this.Dialogs))
			{
				this.LastFilename = dlg.FileName;
				Load(dlg.FileName);
			}
		}

		public ICommand SaveCommand { get { return new RelayCommand(OnSave); } }
		private void OnSave()
		{
			if (String.IsNullOrEmpty(this.LastFilename))
			{
				OnSaveAs();
				return;
			}

			try
			{
				Save(this.LastFilename);
			}
			catch (Exception e)
			{
				new MessageBoxViewModel("Error saving perfy file: " + e.Message, "Error").Show(this.Dialogs);
			}
		}

		public ICommand SaveAsCommand { get { return new RelayCommand(OnSaveAs); } }
		private void OnSaveAs()
		{
			try
			{
				var dlg = new SaveFileDialogViewModel
				{
					Title = "Save Perfy Layout",
					Filter = "Perfy files (*.pfp)|*.pfp|All files (*.*)|*.*",
					FileName = "*.pfp"
				};

				if (dlg.Show(this.Dialogs) == DialogResult.OK)
				{
					this.LastFilename = dlg.FileName;
					Save(dlg.FileName);
				}
			}
			catch (Exception e)
			{
				new MessageBoxViewModel("Error saving perfy file: " + e.Message, "Error").Show(this.Dialogs);
			}
		}

		public ICommand ExitCommand { get { return new RelayCommand(ExitCommand_Execute); } }
		private void ExitCommand_Execute()
		{
			if (new MessageBoxViewModel("Are you sure you want to quit?", "Quit", MessageBoxButton.YesNo, MessageBoxImage.Question).Show(this.Dialogs) == MessageBoxResult.Yes)
				App.Current.MainWindow.Close();
		}

		public ICommand HelpCommand { get { return new RelayCommand(OnHelp); } }
		private void OnHelp()
		{
			foreach (var dlg in this.Dialogs)
				if (dlg is HelpDialogViewModel)
					return;

			this.Dialogs.Add(new HelpDialogViewModel());
		}

		public ICommand AboutCommand { get { return new RelayCommand(OnAbout); } }
		private void OnAbout()
		{
			Assembly assembly = Assembly.GetEntryAssembly();
			FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
			var version = fvi.FileVersion;
			var copyright = fvi.LegalCopyright;
			new MessageBoxViewModel("Perfy v" + version + " The Perf+ Circuit Editor" + Environment.NewLine + Environment.NewLine + copyright, "About Perfy").Show(this.Dialogs);
		}

		public void Load(string filename)
		{
			XmlSerializer x = new XmlSerializer(typeof(Circuit));
			using (var fs = new FileStream(filename, FileMode.Open))
				this.Board.Circuit = x.Deserialize(fs) as Circuit;
		}

		public void Save(string filename)
		{
			XmlSerializer x = new XmlSerializer(typeof(Circuit));
			using (var writer = new StreamWriter(filename))
				x.Serialize(writer, this.Board.Circuit);
		}

	}
}