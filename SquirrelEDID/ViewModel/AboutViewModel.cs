using SquirrelEDID.Utilities;
using SquirrelEDID.Utilities.Messaging;
using SquirrelEDID.View;
using SquirrelEDID.View.Controls;
using System.Windows;
using System.Windows.Input;

namespace SquirrelEDID.ViewModel
{
    public class AboutViewModel : BaseViewModel
    {
        #region Commands
        private ICommand _backCommand;
        public ICommand BackCommand
        {
            get
            {
                return _backCommand ?? (_backCommand = new RelayCommand(obj => HandleBackExecuted()));
            }
        } 
        #endregion

        #region Methods
        private void HandleBackExecuted()
        {
            Messenger<ApplicationStates>.Invoke(ApplicationStates.Back);
        } 
        #endregion
    }
}
