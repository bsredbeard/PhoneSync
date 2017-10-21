using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PhoneSync
{
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaiseChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void AllChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        }

        protected ICommand CreateCommand(Action<object> cmd)
        {
            return new DeferredCommand(cmd);
        }

        protected ICommand CreateCommand(Action<object> cmd, Func<bool> canExecute, string sourceProperty)
        {
            var result = new DeferredCommand(cmd, canExecute);
            result.CanExecuteSource(this, sourceProperty);
            return result;
        }
    }
}
