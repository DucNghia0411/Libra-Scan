using ScanProject.Commands;
using System;
using System.Windows.Input;


namespace ScanProject.ViewModels
{
    public abstract class WorkspaceViewModel : ViewModelBase
    {
        private RelayCommand _closeCommand;

        public ICommand CloseCommand
        {
            get { return _closeCommand ?? (_closeCommand = new RelayCommand(param => OnRequestClose())); }
        }

        public string Title { get; set; }

        public event EventHandler RequestClose;

        public virtual void OnRequestClose()
        {
            EventHandler handler = RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}