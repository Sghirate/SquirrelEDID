using SquirrelEDID.Resources;
using System;
using System.Windows.Markup;

namespace SquirrelEDID.Utilities
{
    [MarkupExtensionReturnType(typeof(string), typeof(string))]
    public class I18NExtension : MarkupExtension
    {
        #region Fields
        string _key; 
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

            return Strings.ResourceManager.GetString(_key) ?? ("#" + _key);
        } 
        #endregion
    }
}
