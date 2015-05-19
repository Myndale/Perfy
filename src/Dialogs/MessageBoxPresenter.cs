using MvvmDialogs.Presenters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Perfy.Dialogs
{
	public class MessageBoxPresenter : IDialogBoxPresenter<MessageBoxViewModel>
	{
		public void Show(MessageBoxViewModel vm, Window parent)
		{
			vm.Result = MessageBox.Show(parent, vm.Message, vm.Caption, vm.Buttons, vm.Image);
		}
	}
}
