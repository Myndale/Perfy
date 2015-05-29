using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perfy.Model
{
	public class Circuit
	{
		public const int WIDTH = 36;
		public const int HEIGHT = 24;

		public Pad[][] PadArray { get; set; }
		
		public Circuit()
		{
			this.PadArray = new Pad[Circuit.HEIGHT][];
			for (int y = 0; y < Circuit.HEIGHT; y++)
			{
				this.PadArray[y] = new Pad[Circuit.WIDTH];
				for (int x = 0; x < Circuit.WIDTH; x++)
					this.PadArray[y][x] = new Pad();
			}
		}

	}
}
