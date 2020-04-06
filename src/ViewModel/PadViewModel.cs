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
	public class PadViewModel : CircuitElementViewModel
	{
		private Pad _Model;
		public Pad Model
		{
			get { return this._Model; }
			set
			{
				this._Model = value;
				RaisePropertyChanged(() => this.Model);
				RaisePropertyChanged(() => this.HorzPad);
				RaisePropertyChanged(() => this.VertPad);
				RaisePropertyChanged(() => this.HorzTrace);
				RaisePropertyChanged(() => this.VertTrace);
				RaisePropertyChanged(() => this.HorzJunction);
				RaisePropertyChanged(() => this.VertJunction);
				RaisePropertyChanged(() => this.Component);
			}
		}

		public bool Component
		{
			get { return this.Model.Component; }
			set { this.Model.Component = value; RaisePropertyChanged(() => this.Component); }
		}

		public bool HorzPad
		{
			get { return this.Model.HorzPad; }
			set { this.Model.HorzPad = value; RaisePropertyChanged(() => this.HorzPad); }
		}

		public bool VertPad
		{
			get { return this.Model.VertPad; }
			set { this.Model.VertPad = value; RaisePropertyChanged(() => this.VertPad); }
		}

		public bool HorzTrace
		{
			get { return this.Model.HorzTrace; }
			set { this.Model.HorzTrace = value; RaisePropertyChanged(() => this.HorzTrace); }
		}

		public bool VertTrace
		{
			get { return this.Model.VertTrace; }
			set { this.Model.VertTrace = value; RaisePropertyChanged(() => this.VertTrace); }
		}

		public bool HorzJunction
		{
			get { return this.Model.HorzJunction; }
			set { this.Model.HorzJunction = value; RaisePropertyChanged(() => this.HorzJunction); }
		}

		public bool VertJunction
		{
			get { return this.Model.VertJunction; }
			set { this.Model.VertJunction = value; RaisePropertyChanged(() => this.VertJunction); }
		}

		private bool _HorzHighlighted;
		public bool HorzHighlighted
		{
			get { return this._HorzHighlighted; }
			set { this._HorzHighlighted = value; RaisePropertyChanged(() => this.HorzHighlighted); }
		}

		private bool _VertHighlighted;
		public bool VertHighlighted
		{
			get { return this._VertHighlighted; }
			set { this._VertHighlighted = value; RaisePropertyChanged(() => this.VertHighlighted); }
		}

		private bool _HorzHighlight = false;
		public bool HorzHighlight
		{
			get { return this._HorzHighlight; }
			set { this._HorzHighlight = value; RaisePropertyChanged(() => this.HorzHighlight); }
		}

		private bool _VertHighlight = false;
		public bool VertHighlight
		{
			get { return this._VertHighlight; }
			set { this._VertHighlight = value; RaisePropertyChanged(() => this.VertHighlight); }
		}

		private bool _HorzCut = false;
		public bool HorzCut
		{
			get { return this._HorzCut; }
			set { this._HorzCut = value; RaisePropertyChanged(() => this.HorzCut); }
		}

		private bool _VertCut = false;
		public bool VertCut
		{
			get { return this._VertCut; }
			set { this._VertCut = value; RaisePropertyChanged(() => this.VertCut); }
		}

		private string _ToolTip;
		public string ToolTip
		{
			get { return this._ToolTip; }
			set { this._ToolTip = value; RaisePropertyChanged(() => this.ToolTip); }
		}
		

		protected override void PositionUpdated()
		{
			base.PositionUpdated();
			this.ToolTip = String.Format("{0},{1}", (char)('A' + this.Y), this.X+1);
		}
		
	}
}
