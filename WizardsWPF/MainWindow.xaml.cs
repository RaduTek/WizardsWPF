using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WizardsWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        WizardHelper helper;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            helper = new WizardHelper(PageList)
            {
                LabelHost = PageLabelList,
                BackButton = BackButton,
                NextButton = NextButton,
                CancelButton = CancelButton,
                FinishButton = FinishButton,
                AllowToGoNext = true,
                DisableBackOnFinish = true
            };

            helper.SelectedPageChanged += Helper_SelectedPageChanged;
        }

        private void Helper_SelectedPageChanged(object sender, SelectedPageChangedEventArgs e)
        {
            //Console.WriteLine(e.PreviousIndex.ToString() + ' ' + e.CurrentIndex.ToString());

            helper.AllowToGoNext = helper.AllowToGoBack = e.CurrentIndex != 2;
            if (e.CurrentIndex == 2)
                StartWorkTimer();
        }

        DispatcherTimer t;

        private void StartWorkTimer()
        {
            t = new DispatcherTimer();
            t.Interval = TimeSpan.FromMilliseconds(25);
            t.Tick += T_Tick;
            t.Start();

        }

        private void T_Tick(object sender, EventArgs e)
        {
            if (Progress1.Value < Progress1.Maximum)
                Progress1.Value++;
            else
            {
                t.Stop();
                helper.SelectedPage++;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            helper.GoBack();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            helper.GoNext();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to cancel?", Title, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                Close();
        }

        private void FinishButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
