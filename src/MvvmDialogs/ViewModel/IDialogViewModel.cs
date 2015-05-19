using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmDialogs.ViewModels
{
	public interface IDialogViewModel
	{
	}

	public interface IDialogViewModelCollection : ICollection<IDialogViewModel>
	{
	}

	public class DialogViewModelCollection : ObservableCollection<IDialogViewModel>, IDialogViewModelCollection
	{
	}
}
