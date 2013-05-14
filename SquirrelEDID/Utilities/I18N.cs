using SquirrelEDID.Resources;
using System;
using System.Collections.Generic;
using System.Windows.Markup;

namespace SquirrelEDID.Utilities
{
    [MarkupExtensionReturnType(typeof(string), typeof(string))]
    public class I18NExtension : MarkupExtension
    {
        #region Fields
        private static HashSet<string> _missing = new HashSet<string>();
        string _key; 
        #endregion

        #region Properties
        public static HashSet<string> Missing { get { return _missing; } }
        #endregion

        #region Constructors
        public I18NExtension(string key)
        {
            _key = key;
        } 
        #endregion

        #region Methods
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (string.IsNullOrEmpty(_key))
                return "#";

            string str = Strings.ResourceManager.GetString(_key);
            if (String.IsNullOrEmpty(str))
            {
                _missing.Add(_key);
                str = "#" + _key;
            }
            return str;
        } 
        #endregion
    }
}
