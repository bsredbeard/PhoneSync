using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PhoneSync
{
    /// <summary>
    /// A base class for all ViewModels. Implements <seealso cref="INotifyPropertyChanged"/>
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// An event fired when a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Signal that the specified property has changed
        /// </summary>
        /// <param name="propertyName"></param>
        protected void RaiseChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Signal that all properties have changed.
        /// </summary>
        public void AllChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        }

        /// <summary>
        /// Create a ICommand with the specified action which is always enabled
        /// </summary>
        /// <param name="cmd">The deferred action to execute when the command is called</param>
        /// <returns>An <see cref="ICommand"/> instance.</returns>
        protected ICommand CreateCommand(Action<object> cmd)
        {
            return new DeferredCommand(cmd);
        }

        /// <summary>
        /// Create an ICommand with the specified action which is enabled via the canExecute predicate.
        /// CanExecute is refreshed when the specified property is changed
        /// </summary>
        /// <param name="cmd">The deferred action to execute when the command is called</param>
        /// <param name="canExecute">The predicate function which determines if the command can execute</param>
        /// <param name="sourceProperty">The name of the property that causes the command's CanExecute flag to refresh</param>
        /// <returns>An <see cref="ICommand"/> instance.</returns>
        protected ICommand CreateCommand(Action<object> cmd, Func<bool> canExecute, string sourceProperty)
        {
            var result = new DeferredCommand(cmd, canExecute);
            result.CanExecuteSource(this, sourceProperty);
            return result;
        }
    }
}
