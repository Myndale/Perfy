using GalaSoft.MvvmLight;
using Perfy.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perfy.ViewModel
{
	// a pad is a location on the perfboard consisting of a hole, a junction and two traces (one for each direction)
	public class PadViewModel : ViewModelBase
	{
		public Pad Model { get; set; }

		private int _X;
		public int X
		{
			get { return this._X; }
			set { this._X = value; RaisePropertyChanged(() => this.X); }
		}

		private int _Y;
		public int Y
		{
			get { return this._Y; }
			set { this._Y = value; RaisePropertyChanged(() => this.Y); }
		}

		public bool HorzHole
		{
			get { return this.Model.Hole.Horz; }
			set { this.Model.Hole.Horz = value; RaisePropertyChanged(() => this.HorzHole); }
		}

		public bool VertHole
		{
			get { return this.Model.Hole.Vert; }
			set { this.Model.Hole.Vert = value; RaisePropertyChanged(() => this.VertHole); }
		}

		private string _HoleColor = "Transparent";
		public string HoleColor
		{
			get { return this._HoleColor; }
			set { this._HoleColor = value; RaisePropertyChanged(() => this.HoleColor); }
		}

		public bool HorzJunction
		{
			get { return this.Model.Junction.Horz; }
			set { this.Model.Junction.Horz = value; RaisePropertyChanged(() => this.HorzJunction); }
		}

		public bool VertJunction
		{
			get { return this.Model.Junction.Vert; }
			set { this.Model.Junction.Vert = value; RaisePropertyChanged(() => this.VertJunction); }
		}

		private string _JunctionColor = "Transparent";
		public string JunctionColor
		{
			get { return this._JunctionColor; }
			set { this._JunctionColor = value; RaisePropertyChanged(() => this.JunctionColor); }
		}

		private Trace _HorzTrace;
		public Trace HorzTrace
		{
			get { return this._HorzTrace; }
			set { this._HorzTrace = value; RaisePropertyChanged(() => this.HorzTrace); }
		}

		private Trace _VertTrace;
		public Trace VertTrace
		{
			get { return this._VertTrace; }
			set { this._VertTrace = value; RaisePropertyChanged(() => this.VertTrace); }
		}
		
		public PadViewModel()
		{
			this.Model = new Pad();
			this.HorzTrace = new Trace(this);
			this.VertTrace = new Trace(this);
		}


	}
}
