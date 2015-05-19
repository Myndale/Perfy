using MvvmDialogs.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmDialogs.ViewModel
{
	public class OpenFileDialogViewModel : IDialogViewModel
	{
		public bool Multiselect {get; set;}
		public bool ReadOnlyChecked { get; set; }
		public bool ShowReadOnly { get; set; }
		public string FileName { get; set; }
		public string[] FileNames { get; set; }
		public string Filter { get; set; }
		public string InitialDirectory { get; set; }
		public bool RestoreDirectory { get; set; }
		public string Title { get; set; }
		public bool ValidateNames { get; set; }
		public bool Result { get; set; }

		public bool Show(IDialogViewModelCollection collection)
		{
			collection.Add(this);
			return this.Result;
		}
	}
}
