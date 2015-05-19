using MvvmDialogs.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Perfy.Dialogs
{
	public class SaveFileDialogViewModel : IDialogViewModel
	{
		public string FileName { get; set; }
		public string[] FileNames { get; set; }
		public string Filter { get; set; }
		public string InitialDirectory { get; set; }
		public bool RestoreDirectory { get; set; }
		public string Title { get; set; }
		public bool ValidateNames { get; set; }
		public DialogResult Result { get; set; }

		public DialogResult Show(IDialogViewModelCollection collection)
		{
			collection.Add(this);
			return this.Result;
		}
	}
}
