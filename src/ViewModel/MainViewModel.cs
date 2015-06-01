using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MvvmDialogs.ViewModels;
using Perfy.Behaviors;
using Perfy.Dialogs;
using Perfy.Model;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Windows;
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
	public class MainViewModel : ViewModelBase, IMouseCaptureProxy
    {
		private IDialogViewModelCollection _Dialogs = new DialogViewModelCollection();
		public IDialogViewModelCollection Dialogs { get { return _Dialogs; } }

		private CircuitViewModel _Board;
		public CircuitViewModel Board
		{
			get { return this._Board; }
			set { this._Board = value; RaisePropertyChanged(() => this.Board); }
		}

		/* todo:
		private PadViewModel CurrentPad = null;
		 * */
		private bool Capturing = false;
		private object CaptureSender;
		public event EventHandler Capture;
		public event EventHandler Release;

		private string LastFilename = null;
		private Point TraceStart;
		private Point TraceEnd;

		private EditMode _EditMode = EditMode.Traces;
		public EditMode EditMode
		{
			get { return this._EditMode; }
			set
			{
				if (this._EditMode != value)
				{
					CancelCurrentTrace();
					this._EditMode = value;
					RaisePropertyChanged(() => this.EditMode);
				}
			}
		}

		private ViewMode _ViewMode = ViewMode.Normal;
		public ViewMode ViewMode
		{
			get { return this._ViewMode; }
			set
			{
				if (this._ViewMode != value)
				{
					this.Board.DeselectHighlights();
					CancelCurrentTrace();
					this._ViewMode = value;
					RaisePropertyChanged(() => this.ViewMode);
				}
			}
		}

		private Perspective _Perspective = Perspective.Front;
		public Perspective Perspective
		{
			get { return this._Perspective; }
			set
			{
				if (this._Perspective != value)
				{
					this.Board.DeselectHighlights();
					CancelCurrentTrace();
					this._Perspective = value;
					RaisePropertyChanged(() => this.Perspective);
				}
			}
		}

		private double _Zoom = 1;
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

		public ICommand SetEditModeCommand { get { return new RelayCommand<EditMode>(OnSetEditMode); } }
		private void OnSetEditMode(EditMode mode)
		{
			CancelCurrentTrace();
			this.EditMode = mode;
		}

		public ICommand UndoCommand { get { return new RelayCommand(OnUndo); } }
		private void OnUndo()
		{
			this.Board.Undo();
		}

		public ICommand RedoCommand { get { return new RelayCommand(OnRedo); } }
		private void OnRedo()
		{
			this.Board.Redo();
		}

		const string CaptionPrefix = "Perfy Circuit Designer";

		private string _Caption = CaptionPrefix;
		public string Caption
		{
			get { return this._Caption; }
			set { this._Caption = value; RaisePropertyChanged(() => this.Caption); }
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

#if DEBUG
			if (args.Key == Key.Escape)
				ExitCommand.Execute(null);
#endif
		}
		
        public MainViewModel()
        {
			this.LastFilename = null;
			this.ZoomLevel = 10;
			this.Board = new CircuitViewModel { Circuit = new Circuit() };
			this.Board.SaveForUndo(true);
		}

		public ICommand NewCommand { get { return new RelayCommand(OnNew); } }
		private void OnNew()
		{
			CancelCurrentTrace();			
			this.LastFilename = null;
			this.ZoomLevel = 10;
			// todo: this.CurrentPad = null;			
			this.Board.Circuit = new Circuit();
			// todo remove UpdateSelections();
			this.Board.SaveForUndo(true);
		}

		public ICommand OpenCommand { get { return new RelayCommand(OnOpen); } }
		private void OnOpen()
		{
			CancelCurrentTrace();
			var dlg = new OpenFileDialogViewModel
			{
				Title = "Load Perfy Layout",
				Filter = "Perfy files (*.pfp)|*.pfp|All files (*.*)|*.*",
				FileName = "*.pfp",
				Multiselect = false
			};

			if (dlg.Show(this.Dialogs))
			{
				this.Caption = CaptionPrefix + " - " + Path.GetFileName(dlg.FileName);
				this.LastFilename = dlg.FileName;
				Load(dlg.FileName);
			}
		}

		public ICommand SaveCommand { get { return new RelayCommand(OnSave); } }
		private void OnSave()
		{
			CancelCurrentTrace();
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
			CancelCurrentTrace();
			try
			{
				var dlg = new SaveFileDialogViewModel
				{
					Title = "Save Perfy Layout",
					Filter = "Perfy files (*.pfp)|*.pfp|All files (*.*)|*.*",
					FileName = "*.pfp"
				};

				if (dlg.Show(this.Dialogs) == System.Windows.Forms.DialogResult.OK)
				{
					this.Caption = CaptionPrefix + " - " + Path.GetFileName(dlg.FileName);
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
			CancelCurrentTrace();
//#if !DEBUG
			if (this.Board.Changed)
			{
				 if (new MessageBoxViewModel("Quit without saving?", "Quit", MessageBoxButton.YesNo, MessageBoxImage.Question).Show(this.Dialogs) != MessageBoxResult.Yes)
					 return;
			}
//#endif
				App.Current.MainWindow.Close();
		}

		public ICommand SummaryCommand { get { return new RelayCommand(SummaryCommand_Execute); } }
		private void SummaryCommand_Execute()
		{
			CancelCurrentTrace();
			var msg = String.Format(
				"Horizontal pad solders:\t{0}\n" +
				"Vertical pad solders:\t\t{1}\n" +
				"Horizontal trace cuts:\t{2}\n" +
				"Vertical trace cuts:\t\t{3}\n" +
				"Total board utilization:\t{4}%",
				this.Board.NumHorzHoles,
				this.Board.NumVertHoles,
				this.Board.NumHorzCuts,
				this.Board.NumVertCuts,
				this.Board.Utilization);
			new MessageBoxViewModel(msg, "Circuit Summary", MessageBoxButton.OK, MessageBoxImage.Information).Show(this.Dialogs);
		}

		public ICommand HelpCommand { get { return new RelayCommand(OnHelp); } }
		private void OnHelp()
		{
			CancelCurrentTrace();
			foreach (var dlg in this.Dialogs)
				if (dlg is HelpDialogViewModel)
					return;

			this.Dialogs.Add(new HelpDialogViewModel());
		}

		public ICommand AboutCommand { get { return new RelayCommand(OnAbout); } }
		private void OnAbout()
		{
			CancelCurrentTrace();
			Assembly assembly = Assembly.GetEntryAssembly();
			var fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
			var version = fvi.FileVersion;
			var copyright = fvi.LegalCopyright;
			new MessageBoxViewModel("Perfy v" + version + " The Perf+ Circuit Editor" + Environment.NewLine + Environment.NewLine + copyright, "About Perfy").Show(this.Dialogs);
		}

		public void Load(string filename)
		{
			XmlSerializer x = new XmlSerializer(typeof(Circuit));
			using (var fs = new FileStream(filename, FileMode.Open))
				this.Board.Circuit = x.Deserialize(fs) as Circuit;
			this.Board.SaveForUndo(true);
		}

		public void Save(string filename)
		{
			XmlSerializer x = new XmlSerializer(typeof(Circuit));
			using (var writer = new StreamWriter(filename))
				x.Serialize(writer, this.Board.Circuit);
		}

		public void OnMouseDown(object sender, MouseCaptureArgs e)
		{
			if (this.ViewMode != ViewMode.Normal)
				return;
			if (this.EditMode == EditMode.Pads)
				OnMouseDownPads(sender, e);
			else if (this.EditMode == EditMode.Traces)
				OnMouseDownTraces(sender, e);			
		}

		public void OnMouseDownPads(object sender, MouseCaptureArgs e)
		{
			int x = (int)(e.X - 1.0);
			int y = (int)(e.Y - 1.0);
			if (!ValidPosition(x, y))
				return;
			if (e.LeftButton)
				this.Board.PadArray[y, x].Component = true;
			else
				this.Board.PadArray[y, x].Component = false;
			this.Board.SaveForUndo();
		}

		public void OnMouseDownTraces(object sender, MouseCaptureArgs e)
		{
			int x = (int)(e.X - 1.0);
			int y = (int)(e.Y - 1.0);
			if (!ValidPosition(x, y))
				return;
			if (this.Capturing)
			{
				if (e.RightButton)
					CancelCurrentTrace();
				else if ((this.TraceStart.X == this.TraceEnd.X) && (this.TraceStart.Y == this.TraceEnd.Y) && e.LeftButton)
					CancelCurrentTrace();
				else
				{
					this.TraceEnd = new Point(x, y);
					var dx = Math.Abs(this.TraceStart.X - this.TraceEnd.X);
					var dy = Math.Abs(this.TraceStart.Y - this.TraceEnd.Y);
					if (dx >= dy)
						this.TraceEnd.Y = this.TraceStart.Y;
					else
						this.TraceEnd.X = this.TraceStart.X;
					this.Board.SetTrace(this.TraceStart, this.TraceEnd);
					this.Board.SaveForUndo();
					this.TraceStart = this.TraceEnd;
					this.TraceEnd = new Point(x, y);
					dx = Math.Abs(this.TraceStart.X - this.TraceEnd.X);
					dy = Math.Abs(this.TraceStart.Y - this.TraceEnd.Y);
					if (dx >= dy)
						this.TraceEnd.Y = this.TraceStart.Y;
					else
						this.TraceEnd.X = this.TraceStart.X;
					this.Board.SetHighlight(this.TraceStart, this.TraceEnd);
				}
			}
			else if (e.RightButton)
				DeleteTraces(x, y);
			else
			{
				this.TraceStart = this.TraceEnd = new Point(x, y);
				this.Board.SetHighlight(this.TraceStart, this.TraceEnd);
				this.Capturing = true;
				this.CaptureSender = sender;
				if (this.Capture != null)
					this.Capture(sender, null);
			}
		}

		public void OnMouseMove(object sender, MouseCaptureArgs e)
		{
			if (this.ViewMode != ViewMode.Normal)
				return;

			var x = (int)(e.X - 1.0);
			var y = (int)(e.Y - 1.0);
			if (!ValidPosition(x, y))
				return;
			if (this.Capturing)
			{
				this.TraceEnd = new Point(x, y);
				var dx = Math.Abs(this.TraceStart.X - this.TraceEnd.X);
				var dy = Math.Abs(this.TraceStart.Y - this.TraceEnd.Y);
				if (dx >= dy)
					this.TraceEnd.Y = this.TraceStart.Y;
				else
					this.TraceEnd.X = this.TraceStart.X;
				this.Board.SetHighlight(this.TraceStart, this.TraceEnd);
			}
			else
				this.Board.HighlightNode(x, y);
		}

		public void OnMouseUp(object sender, MouseCaptureArgs e)
		{
		}

		private void CancelCurrentTrace()
		{
			if (this.Capturing)
			{
				this.Capturing = false;
				this.Board.DeselectHighlights();
				if (this.Release != null)
					this.Release(this.CaptureSender, null);
			}
		}

		private void DeleteTraces(int x, int y)
		{
			if (this.Board.PadArray[y, x].HorzPad)
			{
				this.Board.PadArray[y, x].HorzPad = false;
				this.Board.PadArray[y, x].HorzTrace = false;
				this.Board.PadArray[y, x].HorzJunction = false;
				int tx = x;
				do
				{
					this.Board.PadArray[y, tx].HorzTrace = false;
					this.Board.PadArray[y, tx].HorzJunction = false;
					tx--;
				} while ((tx > 0) && !this.Board.PadArray[y, tx].HorzPad);
				tx = x;
				do
				{
					this.Board.PadArray[y, tx].HorzTrace = false;
					this.Board.PadArray[y, tx].HorzJunction = false;
					tx++;
				} while ((tx < Circuit.WIDTH) && !this.Board.PadArray[y, tx].HorzPad);
				if (tx < Circuit.WIDTH)
					this.Board.PadArray[y, tx].HorzJunction = false;				
			}

			if (this.Board.PadArray[y, x].VertPad)
			{
				this.Board.PadArray[y, x].VertPad = false;
				this.Board.PadArray[y, x].VertTrace = false;
				this.Board.PadArray[y, x].VertJunction = false;
				int ty = y;
				do
				{
					this.Board.PadArray[ty, x].VertTrace = false;
					this.Board.PadArray[ty, x].VertJunction = false;
					ty--;
				} while ((ty > 0) && !this.Board.PadArray[ty, x].VertPad);
				ty = y;
				do
				{
					this.Board.PadArray[ty, x].VertTrace = false;
					this.Board.PadArray[ty, x].VertJunction = false;
					ty++;
				} while ((ty < Circuit.HEIGHT) && !this.Board.PadArray[ty, x].VertPad);
				if (ty < Circuit.HEIGHT)
					this.Board.PadArray[ty, x].VertJunction = false;
			}
			
			this.Board.Clean();
			this.Board.SaveForUndo();
		}

		private bool ValidPosition(int x, int y)
		{
			return (x >= 0) && (x < Circuit.WIDTH) && (y >= 0) && (y < Circuit.HEIGHT);
		}

	}
}