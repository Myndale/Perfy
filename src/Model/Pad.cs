using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perfy.Model
{
	public class Pad
	{
		public bool Component { get; set; }
		public bool HorzPad { get; set; }
		public bool VertPad { get; set; }
		public bool HorzTrace { get; set; }
		public bool VertTrace { get; set; }
		public bool HorzJunction { get; set; }
		public bool VertJunction { get; set; }
	}
}
