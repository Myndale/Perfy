using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perfy.ViewModel
{
	public class Trace : ViewModelBase
	{
		public Node Node {get; set;}
		public PadViewModel Pad { get; set; }

		public Trace(PadViewModel pad)
		{
			this.Pad = pad;
		}

		public void SetNode(Node node)
		{
			this.Node = node;
			node.Traces.Add(this);
		}

		private string _Color = "Transparent";
		public string Color
		{
			get { return this._Color; }
			set { this._Color = value; RaisePropertyChanged(() => this.Color); }
		}
	}
}
