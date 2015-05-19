using GalaSoft.MvvmLight;
using Perfy.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perfy.ViewModel
{
	public class CircuitViewModel : ViewModelBase
	{
		private ObservableCollection<object> _BoardItems = new ObservableCollection<object>();
		public ObservableCollection<object> BoardItems
		{
			get { return this._BoardItems; }
			set { this._BoardItems = value; RaisePropertyChanged(() => this.BoardItems); }
		}

		private PadViewModel[,] PadArray = new PadViewModel[24, 36];

		private Circuit _Circuit;
		public Circuit Circuit
		{
			get {return this._Circuit;}
			set
			{
				if (this._Circuit != value)
				{
					this._Circuit = value;
					for (int y = 0; y < 24; y++)
						for (int x = 0; x < 36; x++)
							this.PadArray[y, x].Model = this._Circuit.PadArray[y][x];
					UpdateNodes();
					Select(null);
				}
			}
		}

		public CircuitViewModel()
		{
			// create the pads and traces
			for (int y = 0; y < 24; y++)
				for (int x = 0; x < 36; x++)
				{
					var pad = new PadViewModel { X = x, Y = y };
					this.BoardItems.Add(pad);
					this.PadArray[y, x] = pad;
				}

			// create the labels
			for (int x = 0; x < 36; x++)
			{
				var label = new Label { X = x + 1, Y = 0, Text = (x + 1).ToString("D2"), Rotate = true};
				this.BoardItems.Add(label);
			}
			for (int y = 0; y < 24; y++)
			{
				var label = new Label { X = 0, Y = y + 1, Text = ((char)((int)'A' + y)).ToString(), Rotate = false};
				this.BoardItems.Add(label);
			}
			
		}

		public void Clear()
		{
			for (int y = 0; y < 24; y++)
				for (int x = 0; x < 36; x++)
				{
					var pad = this.PadArray[y, x];
					pad.HorzHole = false;
					pad.VertHole = false;
					pad.HorzJunction = false;
					pad.VertJunction = false;
				}
			UpdateNodes();
			Select(null);
		}

		public void UpdateNodes()
		{
			// initially all holes, pads and traces have their own unique traces
			for (int y = 0; y < 24; y++)
				for (int x = 0; x < 36; x++)
				{
					var pad = this.PadArray[y, x];
					pad.HorzTrace.SetNode(new Node());
					pad.VertTrace.SetNode(new Node());
				}

			// connect all pads to traces they've been soldered to
			for (int y = 0; y < 24; y++)
				for (int x = 0; x < 36; x++)
				{
					var pad = this.PadArray[y, x];
					if (pad.HorzHole && pad.VertHole)
						Connect(pad.HorzTrace, pad.VertTrace);
				}


			// connect all horizontal segments
			for (int y = 0; y < 24; y++)
				for (int x = 1; x < 36; x++)
				{
					var pad1 = this.PadArray[y, x-1];
					var pad2 = this.PadArray[y, x];
					if (!pad2.HorzJunction)
						Connect(pad1.HorzTrace, pad2.HorzTrace);
				}

			// connect all vertical segments
			for (int y = 1; y < 24; y++)
				for (int x = 0; x < 36; x++)
				{
					var pad1 = this.PadArray[y - 1, x];
					var pad2 = this.PadArray[y, x];
					if (!pad2.VertJunction)
						Connect(pad1.VertTrace, pad2.VertTrace);
				}

		}

		private void Connect(Trace trace1, Trace trace2)
		{
			// make sure they're not already connected
			if (trace1.Node == trace2.Node)
				return;

			// connect them
			foreach (var trace in trace2.Node.Traces.ToList())
				trace.SetNode(trace1.Node);
		}

		public void Select(Node node)
		{
			for (int y = 0; y < 24; y++)
				for (int x = 0; x < 36; x++)
				{
					var pad = this.PadArray[y, x];

					// set the hole color based on whether we're connected to the traces and if they're highlighted
					var holeHorz = (pad.HorzTrace.Node == node) && pad.HorzHole;
					var holeVert = (pad.VertTrace.Node == node) && pad.VertHole;
					if (holeHorz && holeVert)
						pad.HoleColor = "Magenta";
					else if (holeHorz)
						pad.HoleColor = "Red";
					else if (holeVert)
						pad.HoleColor = "Blue";
					else if (pad.HorzHole && pad.VertHole)
						pad.HoleColor = "DarkMagenta";
					else if (pad.HorzHole)
						pad.HoleColor = "DarkRed";
					else if (pad.VertHole)
						pad.HoleColor = "DarkBlue";
					else
						pad.HoleColor = "Gray";

					// set the trace colors based on whether they're connected to the highlight node
					if (pad.HorzTrace.Node == node)
						pad.HorzTrace.Color = "Red";
					else
						pad.HorzTrace.Color = "DarkRed";
					if (pad.VertTrace.Node == node)
						pad.VertTrace.Color = "Blue";
					else
						pad.VertTrace.Color = "DarkBlue";

					// set the junction color based on which trace lines are active and connected to the junction
					var juncHorz = (pad.HorzTrace.Node == node) && !pad.HorzJunction;
					var juncVert = (pad.VertTrace.Node == node) && !pad.VertJunction;
					if (juncHorz && juncVert)
						pad.JunctionColor = "Magenta";
					else if (juncHorz)
						pad.JunctionColor = "Red";
					else if (juncVert)
						pad.JunctionColor = "Blue";
					else if (!pad.HorzJunction && !pad.VertJunction)
						pad.JunctionColor = "DarkMagenta";
					else if (!pad.HorzJunction)
						pad.JunctionColor = "DarkRed";
					else if (!pad.VertJunction)
						pad.JunctionColor = "DarkBlue";
					else
						pad.JunctionColor = "Transparent";
				}
		}

	}

}
