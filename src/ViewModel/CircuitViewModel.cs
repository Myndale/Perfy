using GalaSoft.MvvmLight;
using Perfy.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Perfy.ViewModel
{
	public class CircuitViewModel : ViewModelBase
	{
		public bool Changed { get; private set; }
		private List<Pad[,]> UndoStack = new List<Pad[,]>();
		private int UndoStackPos = 0;

		// connectivity nodes used for gui highlights and calculating trace cuts
		object[,] HorzNodes = new object[Circuit.HEIGHT, Circuit.WIDTH];
		object[,] VertNodes = new object[Circuit.HEIGHT, Circuit.WIDTH];

		private ObservableCollection<CircuitElementViewModel> _BoardItems = new ObservableCollection<CircuitElementViewModel>();
		public ObservableCollection<CircuitElementViewModel> BoardItems
		{
			get { return this._BoardItems; }
			set { this._BoardItems = value; RaisePropertyChanged(() => this.BoardItems); }
		}

		public PadViewModel[,] PadArray = new PadViewModel[Circuit.HEIGHT, Circuit.WIDTH];
		
		private Circuit _Circuit;
		public Circuit Circuit
		{
			get {return this._Circuit;}
			set
			{
				if (this._Circuit != value)
				{
					this._Circuit = value;

					for (int y = 0; y < Circuit.HEIGHT; y++)
						for (int x = 0; x < Circuit.WIDTH; x++)
							this.PadArray[y, x].Model = value.PadArray[y][x];
				}
			}
		}

		public CircuitViewModel()
		{
			// create the pads
			for (int y = 0; y < Circuit.HEIGHT; y++)
				for (int x = 0; x < Circuit.WIDTH; x++)
				{
					var pad = new PadViewModel { X = x, Y = y };
					this.BoardItems.Add(pad);
					this.PadArray[y, x] = pad;
				}

			// create the labels
			for (int x = 0; x < Circuit.WIDTH; x++)
			{
				this.BoardItems.Add(new LabelViewModel {
					Text = (x + 1).ToString("D2"),
					Angle = 90,
					FrontTranslate = new Point { X = x + 2, Y = 0 },
					FrontScale = new Point { X = 1, Y = 1 },
					RearTranslate = new Point { X = -x - 1, Y = 0 },
					RearScale = new Point { X = -1, Y = 1 }
				});
				this.BoardItems.Add(new LabelViewModel {
					Text = (x + 1).ToString("D2"),
					Angle = -90,
					FrontTranslate = new Point { X = x + 1, Y = Circuit.HEIGHT + 2 },
					FrontScale = new Point { X = 1, Y = 1 },
					RearTranslate = new Point { X = -x - 2, Y = Circuit.HEIGHT + 2 },
					RearScale = new Point { X = -1, Y = 1 }
				});
			}

			for (int y = 0; y < Circuit.HEIGHT; y++)
			{
				this.BoardItems.Add(new LabelViewModel {
					Text = ((char)((int)'A' + y)).ToString(),
					Angle = 0,
					FrontTranslate = new Point { X = 0, Y = y + 0.5 },
					FrontScale = new Point { X = 1, Y = 1 },
					RearTranslate = new Point { X = -1, Y = y + 1 },
					RearScale = new Point { X = -1, Y = 1 }
				});
				this.BoardItems.Add(new LabelViewModel {
					Text = ((char)((int)'A' + y)).ToString(),
					Angle = 0,
					FrontTranslate = new Point { X = Circuit.WIDTH + 1, Y = y + 0.5},
					FrontScale = new Point { X = 1, Y = 1 },
					RearTranslate = new Point { X = -Circuit.WIDTH - 2, Y = y + 1 },
					RearScale = new Point { X = -1, Y = 1 }
				});
			}
			
		}

		public void SetHorzTrace(int x, int y, bool set)
		{
			this.PadArray[y, x].HorzTrace = set;
			if (set)
			{
				// place a horz trace on this pad
				if ((x == 0) || this.PadArray[y, x-1].HorzTrace)
					this.PadArray[y, x].HorzJunction = true;
				if ((x < Circuit.WIDTH-1) && this.PadArray[y, x + 1].HorzTrace)
					this.PadArray[y, x + 1].HorzJunction = true;
			}
			else
			{
				// removing a horz trace from this pad
				this.PadArray[y, x].HorzJunction = false;
				if ((x < Circuit.WIDTH-1) && !this.PadArray[y, x + 1].HorzTrace)
					this.PadArray[y, x + 1].HorzJunction = false;
			}
		}

		public void SetVertTrace(int x, int y, bool set)
		{
			this.PadArray[y, x].VertTrace = set;
			if (set)
			{
				// place a Vert trace on this pad
				if ((y == 0) || this.PadArray[y - 1, x].VertTrace)
					this.PadArray[y, x].VertJunction = true;
				if ((y < Circuit.HEIGHT-1) && this.PadArray[y + 1, x].VertTrace)
					this.PadArray[y + 1, x].VertJunction = true;
			}
			else
			{
				// removing a Vert trace from this pad
				this.PadArray[y, x].VertJunction = false;
				if ((y < Circuit.HEIGHT-1) && !this.PadArray[y + 1, x].VertTrace)
					this.PadArray[y + 1, x].VertJunction = false;
			}
		}

		public void SetHorzHighlight(int x, int y, bool set)
		{
			this.PadArray[y, x].HorzHighlight = set;
			if (set)
			{
				// place a horz Highlight on this pad
				if ((x < 0) || this.PadArray[y, x - 1].HorzHighlight)
					this.PadArray[y, x].HorzJunction = false;
				if ((x < Circuit.WIDTH) && this.PadArray[y, x + 1].HorzHighlight)
					this.PadArray[y, x + 1].HorzJunction = false;
			}
			else
			{
				// removing a horz Highlight from this pad
				this.PadArray[y, x].HorzJunction = true;
				if ((x < Circuit.WIDTH) && !this.PadArray[y, x + 1].HorzHighlight)
					this.PadArray[y, x + 1].HorzJunction = true;
			}
		}

		public void SetHighlight(Point start, Point end)
		{
			DeselectHighlights();
			var firstx = (int)Math.Min(start.X, end.X);
			var lastx = (int)Math.Max(start.X, end.X);
			var firsty = (int)Math.Min(start.Y, end.Y);
			var lasty = (int)Math.Max(start.Y, end.Y);
			int dx = lastx - firstx;
			int dy = lasty - firsty;
			int length = Math.Max(dx, dy);
			if (length > 0)
			{
				dx /= length;
				dy /= length;
			}
			for (int i = 0; i <= length; i++)
			{
				if (dy > 0)
					this.PadArray[firsty, firstx].VertHighlight = true;
				else
					this.PadArray[firsty, firstx].HorzHighlight = true;
				firstx += dx;
				firsty += dy;
			}
		}

		public void DeselectHighlights()
		{
			for (int y = 0; y < Circuit.HEIGHT; y++)
				for (int x = 0; x < Circuit.WIDTH; x++)
				{
					this.PadArray[y, x].HorzHighlight = false;
					this.PadArray[y, x].VertHighlight = false;
				}
		}

		public void SetTrace(Point start, Point end)
		{
			var firstx = (int)Math.Min(start.X, end.X);
			var lastx = (int)Math.Max(start.X, end.X);
			var firsty = (int)Math.Min(start.Y, end.Y);
			var lasty = (int)Math.Max(start.Y, end.Y);
			int dx = lastx - firstx;
			int dy = lasty - firsty;
			int length = Math.Max(dx, dy);
			if (length > 0)
			{
				dx /= length;
				dy /= length;
			}
			if (dy > 0)
			{
				this.PadArray[firsty, firstx].VertPad = true;
				this.PadArray[lasty, lastx].VertPad = true;
			}
			else
			{
				this.PadArray[firsty, firstx].HorzPad = true;
				this.PadArray[lasty, lastx].HorzPad = true;
			}
			firstx += dx;
			firsty += dy;
			for (int i = 1; i <= length; i++)
			{
				if (dy > 0)
				{
					this.PadArray[firsty, firstx].VertTrace = true;
					this.PadArray[firsty, firstx].VertJunction = true;
				}
				else
				{
					this.PadArray[firsty, firstx].HorzTrace = true;
					this.PadArray[firsty, firstx].HorzJunction = true;
				}
				firstx += dx;
				firsty += dy;
			}
		}

		public void Clean()
		{
			// removes any dangling pad connections that are no longer connected to traces (called whenever traces are deleted)
			for (int y = 0; y < Circuit.HEIGHT; y++)
				for (int x = 0; x < Circuit.WIDTH; x++)
				{
					var hasHorzNeighbour = this.PadArray[y, x].HorzJunction;
					hasHorzNeighbour |= (x < Circuit.WIDTH-1) && this.PadArray[y, x + 1].HorzJunction;
					if (!hasHorzNeighbour)
					{
						this.PadArray[y, x].HorzJunction = false;
						this.PadArray[y, x].HorzTrace = false;
						this.PadArray[y, x].HorzPad = false;
					}
					if ((x < Circuit.WIDTH-1) && !this.PadArray[y, x].HorzTrace && !this.PadArray[y, x].HorzPad)
						this.PadArray[y, x + 1].HorzJunction = false;

					var hasVertNeighbour = this.PadArray[y, x].VertJunction;
					hasVertNeighbour |= (y < Circuit.HEIGHT-1) && this.PadArray[y + 1, x].VertJunction;
					if (!hasVertNeighbour)
					{
						this.PadArray[y, x].VertJunction = false;
						this.PadArray[y, x].VertTrace = false;
						this.PadArray[y, x].VertPad = false;
					}
					if ((y < Circuit.HEIGHT-1) && !this.PadArray[y, x].VertTrace && !this.PadArray[y, x].VertPad)
						this.PadArray[y + 1, x].VertJunction = false;
				}
		}

		public void HighlightNode(int nodex, int nodey)
		{
			// highlight the current node, deselect everything else
			var horzNode = this.PadArray[nodey, nodex].HorzPad ? this.HorzNodes[nodey, nodex] : null;
			var vertNode = this.PadArray[nodey, nodex].VertPad ? this.VertNodes[nodey, nodex] : null;
			for (int y = 0; y < Circuit.HEIGHT; y++)
				for (int x = 0; x < Circuit.WIDTH; x++)
				{
					this.PadArray[y, x].HorzHighlighted = (this.HorzNodes[y, x] == horzNode) || (this.HorzNodes[y, x] == vertNode);
					this.PadArray[y, x].VertHighlighted = (this.VertNodes[y, x] == horzNode) || (this.VertNodes[y, x] == vertNode);
				}
		}

		private void ConnectNodes(object node1, object node2)
		{
			for (int y = 0; y < Circuit.HEIGHT; y++)
				for (int x = 0; x < Circuit.WIDTH; x++)
				{
					if (this.HorzNodes[y, x] == node2)
						this.HorzNodes[y, x] = node1;
					if (this.VertNodes[y, x] == node2)
						this.VertNodes[y, x] = node1;
				}
		}

		public void SaveForUndo(bool fresh = false)
		{
			var save = new Pad[Circuit.HEIGHT, Circuit.WIDTH];
			for (int y=0; y<Circuit.HEIGHT; y++)
				for (int x=0; x<Circuit.WIDTH; x++)
				{
					var src = this.PadArray[y, x];
					var dst = save[y, x] = new Pad();
					dst.Component = src.Component;
					dst.HorzPad = src.HorzPad;
					dst.VertPad = src.VertPad;
					dst.HorzTrace = src.HorzTrace;
					dst.VertTrace = src.VertTrace;
					dst.HorzJunction = src.HorzJunction;
					dst.VertJunction = src.VertJunction;
				}
			this.UndoStack = this.UndoStack.Take(this.UndoStackPos).ToList();
			if (CircuitChanged(save))
			{
				this.Changed = true;
				this.UndoStack.Add(save);
				this.UndoStackPos++;
				CalculateNodes();
				GenerateCuts();
			}
			if (fresh)
				this.Changed = false;
		}

		private bool CircuitChanged(Pad[,] circuit1)
		{
			if (this.UndoStack.Count() == 0)
				return true;
			var circuit2 = this.UndoStack.Last();
			for (int y = 0; y < Circuit.HEIGHT; y++)
				for (int x = 0; x < Circuit.WIDTH; x++)
				{
					var pad1 = circuit1[y, x];
					var pad2 = circuit2[y, x];
					if (
						(pad1.Component != pad2.Component) ||
						(pad1.HorzPad != pad2.HorzPad) ||
						(pad1.VertPad != pad2.VertPad) ||
						(pad1.HorzTrace != pad2.HorzTrace) ||
						(pad1.VertTrace != pad2.VertTrace) ||
						(pad1.HorzJunction != pad2.HorzJunction) ||
						(pad1.VertJunction != pad2.VertJunction)
						)
						return true;
				}
			return false;
		}

		public void Undo()
		{
			if (this.UndoStackPos > 1)
				Restore(this.UndoStack[(--this.UndoStackPos)-1]);
		}

		public void Redo()
		{
			if (this.UndoStackPos < UndoStack.Count())
				Restore(this.UndoStack[(++this.UndoStackPos)-1]);
		}

		private void Restore(Pad[,] saved)
		{
			for (int y=0; y<Circuit.HEIGHT; y++)
				for (int x=0; x<Circuit.WIDTH; x++)
				{
					var src = saved[y, x];
					var dst = this.PadArray[y, x];
					dst.Component = src.Component;
					dst.HorzPad = src.HorzPad;
					dst.VertPad = src.VertPad;
					dst.HorzTrace = src.HorzTrace;
					dst.VertTrace = src.VertTrace;
					dst.HorzJunction = src.HorzJunction;
					dst.VertJunction = src.VertJunction;
				}
			CalculateNodes();
			GenerateCuts();
		}

		private void CalculateNodes()
		{
			// give all pads a unique node
			for (int y = 0; y < Circuit.HEIGHT; y++)
				for (int x = 0; x < Circuit.WIDTH; x++)
				{
					this.HorzNodes[y, x] = new object();
					this.VertNodes[y, x] = new object();
				}

			// merge connected nodes
			for (int y = 0; y < Circuit.HEIGHT; y++)
				for (int x = 1; x < Circuit.WIDTH; x++)
					if (this.PadArray[y, x].HorzJunction)
						ConnectNodes(this.HorzNodes[y, x - 1], this.HorzNodes[y, x]);
			for (int y = 1; y < Circuit.HEIGHT; y++)
				for (int x = 0; x < Circuit.WIDTH; x++)
					if (this.PadArray[y, x].VertJunction)
						ConnectNodes(this.VertNodes[y - 1, x], this.VertNodes[y, x]);
			for (int y = 0; y < Circuit.HEIGHT; y++)
				for (int x = 0; x < Circuit.WIDTH; x++)
					if (this.PadArray[y, x].HorzPad && this.PadArray[y, x].VertPad)
						ConnectNodes(this.HorzNodes[y, x], this.VertNodes[y, x]);
		}

		private void GenerateCuts()
		{
			// generate the horizontal cuts
			for (int y = 0; y < Circuit.HEIGHT; y++)
			{
				bool first = true;
				object currentNode = null;
				for (int x = 0; x < Circuit.WIDTH; x++)
				{
					this.PadArray[y, x].HorzCut = false;
					if (this.PadArray[y, x].HorzPad && !this.PadArray[y, x].HorzJunction)
					{
						if (!first && (currentNode != this.HorzNodes[y, x]))
							this.PadArray[y, x].HorzCut = true;
						currentNode = this.HorzNodes[y, x];
						first = false;
					}
				}
			}

			// generate the vertical cuts
			for (int x = 0; x < Circuit.WIDTH; x++)
			{
				bool first = true;
				object currentNode = null;
				for (int y = 0; y < Circuit.HEIGHT; y++)
				{
					this.PadArray[y, x].VertCut = false;
					if (this.PadArray[y, x].VertPad && !this.PadArray[y, x].VertJunction)
					{
						if (!first && (currentNode != this.VertNodes[y,x]))
							this.PadArray[y, x].VertCut = true;
						currentNode = this.VertNodes[y, x];
						first = false;
					}
				}
			}
		}

		public int NumHorzHoles
		{
			get
			{
				int count = 0;
				for (int y=0; y<Circuit.HEIGHT; y++)
					for (int x=0; x<Circuit.WIDTH; x++)
						if (this.PadArray[y, x].HorzPad)
							count++;
				return count;
			}
		}
		public int NumVertHoles
		{
			get
			{
				int count = 0;
				for (int y=0; y<Circuit.HEIGHT; y++)
					for (int x=0; x<Circuit.WIDTH; x++)
						if (this.PadArray[y, x].VertPad)
							count++;
				return count;
			}
		}

		public int NumHorzCuts
		{
			get
			{
				int count = 0;
				for (int y=0; y<Circuit.HEIGHT; y++)
					for (int x=0; x<Circuit.WIDTH; x++)
						if (this.PadArray[y, x].HorzCut)
							count++;
				return count;
			}
		}
		public int NumVertCuts
		{
			get
			{
				int count = 0;
				for (int y=0; y<Circuit.HEIGHT; y++)
					for (int x=0; x<Circuit.WIDTH; x++)
						if (this.PadArray[y, x].VertCut)
							count++;
				return count;
			}
		}

		public int Utilization
		{
			get
			{
				int count = 0;
				for (int y = 0; y < Circuit.HEIGHT; y++)
					for (int x = 0; x < Circuit.WIDTH; x++)
					{
						if (this.PadArray[y, x].HorzTrace)
							count++;
						if (this.PadArray[y, x].VertTrace)
							count++;
					}
				return 100 * count / (2 * Circuit.WIDTH * Circuit.HEIGHT);
			}
		}
	}

}
