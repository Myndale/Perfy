using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Perfy.Behaviors
{
	public interface IMouseCaptureProxy
	{
		event EventHandler Capture;
		event EventHandler Release;

		void OnMouseDown(object sender, MouseCaptureArgs e);
		void OnMouseMove(object sender, MouseCaptureArgs e);
		void OnMouseUp(object sender, MouseCaptureArgs e);
	}

	public class MouseCaptureArgs
	{
		public double X {get; set;}
		public double Y { get; set; }
		public bool LeftButton { get; set; }
		public bool RightButton { get; set; }
	}
}
