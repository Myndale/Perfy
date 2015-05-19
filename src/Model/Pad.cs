using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perfy.Model
{
	public class Pad
	{
		public Joint Hole {get; set;}
		public Joint Junction {get; set;}

		public Pad()
		{
			this.Hole = new Joint { Horz = false, Vert = false };
			this.Junction = new Joint { Horz = false, Vert = false };
		}
	}
}
