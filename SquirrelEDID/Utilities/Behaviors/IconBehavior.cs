using System;
using System.Windows.Documents;
using System.Windows.Interactivity;

namespace SquirrelEDID.Utilities.Behaviors
{
    public class IconBehavior : Behavior<Glyphs>
    {
        private static Uri _fontUri = new Uri("pack://application:,,,/Resources/SquirrelEDID.ttf");

        protected override void OnAttached()
        {
            AssociatedObject.FontUri = _fontUri;
            base.OnAttached();
        }
    }
}
