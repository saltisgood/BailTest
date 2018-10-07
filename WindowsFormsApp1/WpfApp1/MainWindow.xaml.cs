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

            setup();
        }

        private void setup()
        {
            var aDefaultBailCond = BailCond.CreateDefault();
            for (int j = 0; j < aDefaultBailCond.Conditions.Count; ++j)
            {
                var aRowDef = new RowDefinition
                {
                    Height = new GridLength(0)
                };
                mConditionsInputGrid.RowDefinitions.Add(aRowDef);
            }

            int i = 0;
            foreach (var aCond in aDefaultBailCond.Conditions)
            {
                mConditionList.Items.Add(new ListBoxItem { Content = aCond.ShortText });

                var aPanel = new WrapPanel
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Margin = new Thickness(0),
                    VerticalAlignment = VerticalAlignment.Top
                };
                mConditionsInputGrid.Children.Add(aPanel);
                Grid.SetRow(aPanel, i);

                foreach (var aTextBlock in aCond.TextBlocks)
                {
                    UIElement aTextElem;
                    if (!aTextBlock.Editable)
                    {
                        aTextElem = new Label
                        {
                            Margin = new Thickness(10),
                            Content = aTextBlock.Value
                        };
                    }
                    else
                    {
                        aTextElem = new TextBox
                        {
                            Height = 23,
                            TextWrapping = TextWrapping.Wrap,
                            Width = 120
                        };
                    }
                    aPanel.Children.Add(aTextElem);
                }

                ++i;
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
