using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perfy.Model
{
	public class Circuit
	{
		public Pad[][] PadArray {get; set;}

		public Circuit()
		{
			this.PadArray = new Pad[24][];
			for (int y = 0; y < 24; y++)
			{
				this.PadArray[y] = new Pad[36];
				for (int x = 0; x < 36; x++)
					this.PadArray[y][x] = new Pad();
			}
		}
	}
}
