using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perfy.ViewModel
{
	public class Label : ViewModelBase
	{
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

		private string _Text;
		public string Text
		{
			get { return this._Text; }
			set { this._Text = value; RaisePropertyChanged(() => this.Text); }
		}

		private bool _Rotate;
		public bool Rotate
		{
			get { return this._Rotate; }
			set { this._Rotate = value; RaisePropertyChanged(() => this.Rotate); }
		}
		
		
	}
}
