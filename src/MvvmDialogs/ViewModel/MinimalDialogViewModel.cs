using MvvmDialogs.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmDialogs.ViewModel
{
	public class MinimalDialogViewModel : IUserDialogViewModel
	{
		public virtual bool IsModal { get { return true; } }
		public virtual void RequestClose() { this.DialogClosing(this, null); }
		public virtual event EventHandler DialogClosing;
	}
}
