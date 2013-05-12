using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquirrelEDID.ViewModel
{
    public enum ProgrammerStates
    {
        None,
        ReadSuccess,
        ReadFailed
    }

    public class PromptProgrammerViewModel : BaseViewModel
    {
        #region Fields
        private ProgrammerStates _state; 
        #endregion

        #region Properties
        public ProgrammerStates State
        {
            get { return _state; }
            set
            {
                if (_state == value)
                    return;

                _state = value;
                OnPropertyChanged("State");
            }
        } 
        #endregion

        #region Constructors
        public PromptProgrammerViewModel()
        {
            _state = ProgrammerStates.None;
        } 
        #endregion
    }
}
