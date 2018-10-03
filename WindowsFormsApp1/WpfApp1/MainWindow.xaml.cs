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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            hideAll();
        }

        private void hideAll()
        {
            foreach (RowDefinition row in mConditionsInputGrid.RowDefinitions)
            {
                row.Height = new GridLength(0);
            }
        }

        private void mConditionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var aNewSelectedIndices = from ListBoxItem aNewSelectedItem in e.AddedItems
                                      select mConditionList.Items.IndexOf(aNewSelectedItem);
            var aNewUnselectedIndices = from ListBoxItem aNewUnselectedItem in e.RemovedItems
                                        select mConditionList.Items.IndexOf(aNewUnselectedItem);
            adjustConditionsPanel(aNewSelectedIndices, aNewUnselectedIndices);
        }

        private void adjustConditionsPanel(IEnumerable<int> iNewSelectedItems, IEnumerable<int> iNewUnselectedItems)
        {
            foreach (var s in iNewSelectedItems)
            {
                mConditionsInputGrid.RowDefinitions[s].Height = GridLength.Auto;
            }

            foreach (var u in iNewUnselectedItems)
            {
                mConditionsInputGrid.RowDefinitions[u].Height = new GridLength(0);
            }
        }
    }
}
