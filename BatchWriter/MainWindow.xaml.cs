using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BatchWriter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            _viewModel = (ViewModel)RootControl.DataContext;

            Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            _viewModel.Kill();
        }

        private ViewModel _viewModel;
        private bool _selectClick;

        private void ListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                _viewModel.Command_RemoveSteamWorkshopID.Execute(SteamWorkshopIDList.SelectedItem);
            }
        }

        //private void PreservedBox_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Delete)
        //    {
        //        _viewModel.PreservedAddons.Remove((string)PreservedAddonsList.SelectedItem);
        //    }
        //}

        private void FrameworkElement_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1)
            {
                if (_selectClick) foreach (AddonItem item in _viewModel.Addons) item.IsSelected = false;
                else foreach (AddonItem item in _viewModel.Addons) item.IsSelected = true;

                _selectClick = !_selectClick;
            }
            else e.Handled = false;
        }


        private Boolean AutoScroll = true;

        private void SteamOutputScrollViewer_ScrollChanged(Object sender, ScrollChangedEventArgs e)
        {
            // User scroll event : set or unset auto-scroll mode
            if (e.ExtentHeightChange == 0)
            {   // Content unchanged : user scroll event
                if (SteamOutputScrollViewer.VerticalOffset == SteamOutputScrollViewer.ScrollableHeight)
                {   // Scroll bar is in bottom
                    // Set auto-scroll mode
                    AutoScroll = true;
                }
                else
                {   // Scroll bar isn't in bottom
                    // Unset auto-scroll mode
                    AutoScroll = false;
                }
            }

            // Content scroll event : auto-scroll eventually
            if (AutoScroll && e.ExtentHeightChange != 0)
            {   // Content changed and auto-scroll mode set
                // Autoscroll
                SteamOutputScrollViewer.ScrollToVerticalOffset(SteamOutputScrollViewer.ExtentHeight);
            }
        }
    }
}
