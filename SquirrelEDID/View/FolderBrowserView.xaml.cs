using SquirrelEDID.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SquirrelEDID.View
{
    /// <summary>
    /// Interaction logic for FolderBrowserView.xaml
    /// </summary>
    public partial class FolderBrowserView : UserControl
    {
        public FolderBrowserView()
        {
            InitializeComponent();
        }

        private void TreeView_Selected(object sender, RoutedEventArgs e)
        {
            if (!(e.OriginalSource is TreeViewItem))
                return;
            TreeViewItem tvm = (TreeViewItem)e.OriginalSource;

            if (DataContext == null || !(DataContext is FolderBrowserViewModel))
                return;

            FolderBrowserViewModel vm = (FolderBrowserViewModel)DataContext;

            vm.SelectCommand.Execute(tvm.DataContext);
        }

        private void TreeView_Expanded(object sender, RoutedEventArgs e)
        {
            if (!(e.OriginalSource is TreeViewItem))
                return;
            TreeViewItem tvm = (TreeViewItem)e.OriginalSource;

            if (DataContext == null || !(DataContext is FolderBrowserViewModel))
                return;

            FolderBrowserViewModel vm = (FolderBrowserViewModel)DataContext;

            vm.ExpandCommand.Execute(tvm.DataContext);
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer sv = (ScrollViewer)sender;
            sv.ScrollToVerticalOffset(sv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}
