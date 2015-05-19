using MvvmDialogs.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MvvmDialogs.Presenters
{
	public interface IDialogBoxPresenter<T> where T : IDialogViewModel
	{
		void Show(T viewModel, Window parent);
	}
}
