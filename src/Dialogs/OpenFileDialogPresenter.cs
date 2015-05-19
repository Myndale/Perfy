using Microsoft.Win32;
using MvvmDialogs.Presenters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Perfy.Dialogs
{
	public class OpenFileDialogPresenter : IDialogBoxPresenter<OpenFileDialogViewModel>
	{
		public void Show(OpenFileDialogViewModel vm, Window parent)
		{
			var dlg = new OpenFileDialog();

			dlg.Multiselect = vm.Multiselect;
			dlg.ReadOnlyChecked = vm.ReadOnlyChecked;
			dlg.ShowReadOnly = vm.ShowReadOnly;
			dlg.FileName = vm.FileName;
			dlg.Filter = vm.Filter;
			dlg.InitialDirectory = vm.InitialDirectory ?? "";
			dlg.RestoreDirectory = vm.RestoreDirectory;
			dlg.Title = vm.Title ?? "";
			dlg.ValidateNames = vm.ValidateNames;

			var result = dlg.ShowDialog(parent);
			vm.Result = (result != null) && result.Value;

			vm.Multiselect = dlg.Multiselect;
			vm.ReadOnlyChecked = dlg.ReadOnlyChecked;
			vm.ShowReadOnly = dlg.ShowReadOnly;
			vm.FileName = dlg.FileName;
			vm.FileNames = dlg.FileNames;
			vm.Filter = dlg.Filter;
			vm.InitialDirectory = dlg.InitialDirectory;
			vm.RestoreDirectory = dlg.RestoreDirectory;
			vm.SafeFileName = dlg.SafeFileName;
			vm.SafeFileNames = dlg.SafeFileNames;
			vm.Title = dlg.Title;
			vm.ValidateNames = dlg.ValidateNames;
		}
	}
}
