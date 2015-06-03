using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perfy.ViewModel
{
	public class CircuitElementViewModel : ViewModelBase
	{
		private int _X = 0;
		public int X
		{
			get { return this._X; }
			set { this._X = value; RaisePropertyChanged(() => this.X); PositionUpdated(); }
		}

		private int _Y = 0;
		public int Y
		{
			get { return this._Y; }
			set { this._Y = value; RaisePropertyChanged(() => this.Y); PositionUpdated(); }
		}

		protected virtual void PositionUpdated()
		{
		}
	}
}
