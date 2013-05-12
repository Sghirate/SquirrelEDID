using SquirrelEDID.Utilities;
using SquirrelEDID.Utilities.Messaging;
using System.Windows.Input;

namespace SquirrelEDID.ViewModel
{
    public class WelcomeViewModel : BaseViewModel
    {
        #region Commands
        private ICommand _startCommand;
        public ICommand StartCommand
        {
            get
            {
                return _startCommand ?? (_startCommand = new RelayCommand(HandleStartExecuted));
            }
        }
        private ICommand _aboutCommand;
        public ICommand AboutCommand
        {
            get
            {
                return _aboutCommand ?? (_aboutCommand = new RelayCommand(HandleAboutExecuted));
            }
        }
        #endregion

        #region Methods
        private void HandleStartExecuted(object obj)
        {
            Messenger<ApplicationStates>.Invoke(ApplicationStates.EDID);
        }

        private void HandleAboutExecuted(object obj)
        {
            Messenger<ApplicationStates>.Invoke(ApplicationStates.About);
        } 
        #endregion
    }
}
