using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Perfy.ViewModel
{
	public class LabelViewModel : CircuitElementViewModel
	{
		private string _Text;
		public string Text
		{
			get { return this._Text; }
			set { this._Text = value; RaisePropertyChanged(() => this.Text); }
		}

		private int _Angle;
		public int Angle
		{
			get { return this._Angle; }
			set { this._Angle = value; RaisePropertyChanged(() => this.Angle); }
		}

		private Point _FrontTranslate;
		public Point FrontTranslate
		{
			get { return this._FrontTranslate; }
			set { this._FrontTranslate = value; RaisePropertyChanged(() => this.FrontTranslate); }
		}

		private Point _FrontScale;
		public Point FrontScale
		{
			get { return this._FrontScale; }
			set { this._FrontScale = value; RaisePropertyChanged(() => this.FrontScale); }
		}

		private Point _RearTranslate;
		public Point RearTranslate
		{
			get { return this._RearTranslate; }
			set { this._RearTranslate = value; RaisePropertyChanged(() => this.RearTranslate); }
		}

		private Point _RearScale;
		public Point RearScale
		{
			get { return this._RearScale; }
			set { this._RearScale = value; RaisePropertyChanged(() => this.RearScale); }
		}
		
		
		
	}
}
