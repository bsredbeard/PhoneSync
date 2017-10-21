using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PhoneSync
{
    public class DeferredCommand : ICommand
    {
        private static readonly Func<bool> YES_I_CAN = () => true;

        private readonly Action<object> _exec;
        private readonly Func<bool> _canExec;
        private readonly List<string> _sourceObjectProperties = new List<string>();
        private bool _canExecCache = false;

        public DeferredCommand(Action<object> exec, Func<bool> canExecute = null)
        {
            _exec = exec;
            if(canExecute == null)
            {
                _canExec = YES_I_CAN;
            } else
            {
                _canExec = canExecute;
            }
            CanExecute(null);
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            var result = _canExec();
            if(result != _canExecCache)
            {
                _canExecCache = result;
                CanExecuteChanged?.Invoke(this, new EventArgs());
            }

            return result;
        }

        public void Execute(object parameter)
        {
            _exec(parameter);
        }

        public void CanExecuteSource(INotifyPropertyChanged sourceObject, string propertyName)
        {
            _sourceObjectProperties.Add(propertyName);
            sourceObject.PropertyChanged += SourcePropertyChanged;
        }

        private void SourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_sourceObjectProperties.Contains(e.PropertyName))
            {
                CanExecute(null);
            }
        }
    }
}
