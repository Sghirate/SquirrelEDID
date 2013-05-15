using SquirrelEDID.Model;
using SquirrelEDID.Resources;
using SquirrelEDID.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Windows.Input;

namespace SquirrelEDID.ViewModel
{
    public class BaseViewModel : NotifyPropertyChanged
    {
        protected string LocalString(string key, object args = null)
        {
            string msg = Strings.ResourceManager.GetString(key);
            if (String.IsNullOrEmpty(msg))
                msg = "#" + key;
            if (msg.Contains("{0}"))
                msg = String.Format(msg, args);
            msg += "\r\n";
            return msg;
        }
    }
}
