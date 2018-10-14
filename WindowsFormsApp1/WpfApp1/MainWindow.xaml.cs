﻿using System;
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
using System.Windows.Media.Animation;
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
        private static TimeSpan DefaultAnimationDuration
        {
            get
            {
                return TimeSpan.FromMilliseconds(200);
            }
        }

        private readonly List<WrapPanel> mConditionPanels;
        private readonly BailCond mBailCond;

        public MainWindow()
        {
            InitializeComponent();

            mConditionPanels = new List<WrapPanel>();
            mBailCond = BailCond.CreateDefault();
            Setup();
        }

        private void Setup()
        {
            for (int j = 0; j < mBailCond.Conditions.Count; ++j)
            {
                var aRowDef = new RowDefinition
                {
                    Height = GridLength.Auto
                };
                mConditionsInputGrid.RowDefinitions.Add(aRowDef);
            }

            BailCond.GenderFormat aGender = BailCond.GenderFormat.Female;

            int i = 0;
            foreach (var aCond in mBailCond.Conditions)
            {
                mConditionList.Items.Add(new ListBoxItem { Content = aCond.ShortText });

                var aPanel = new WrapPanel
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Margin = new Thickness(0),
                    Opacity = 0.0,
                    VerticalAlignment = VerticalAlignment.Top,
                    Visibility = Visibility.Visible
                };

                // Draw the elements higher in the grid last so they're on the top.
                // This lets us always have the elements loaded but hidden so we always know their height
                // which makes the animation easier.
                WrapPanel.SetZIndex(aPanel, mBailCond.Conditions.Count - i);
                // As soon as the panel is loaded we immediately hide it behind the above row. Since we
                // know the actual height at that point it's the best time to do it. Since it starts out
                // as transparent you can't see it anyway.
                aPanel.Loaded += APanel_Loaded;
                mConditionsInputGrid.Children.Add(aPanel);
                Grid.SetRow(aPanel, i);
                mConditionPanels.Add(aPanel);

                foreach (var aTextBlock in aCond.TextBlocks)
                {
                    UIElement aTextElem;
                    if (!aTextBlock.Editable)
                    {
                        aTextElem = new Label
                        {
                            Margin = new Thickness(10),
                            Content = String.Format(aTextBlock.Value, aGender)
                        };
                    }
                    else
                    {
                        int aMinSize = aTextBlock.MinSize != 0 ? aTextBlock.MinSize : 120;

                        // Values are just placeholders
                        aTextElem = new TextBox
                        {
                            Height = 23,
                            TextWrapping = TextWrapping.Wrap,
                            Width = aMinSize
                        };
                    }
                    aPanel.Children.Add(aTextElem);
                }

                ++i;
            }
        }

        private void APanel_Loaded(object sender, RoutedEventArgs e)
        {
            var aPanel = (WrapPanel)sender;
            aPanel.Margin = new Thickness(0, -aPanel.ActualHeight, 0, 0);
        }

        private void mConditionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AdjustConditionsPanel(e.AddedItems, true);
            AdjustConditionsPanel(e.RemovedItems, false);
        }

        private void AdjustConditionsPanel(System.Collections.IList iItems, bool iShow)
        {
            var aIndices = from ListBoxItem aItem in iItems select mConditionList.Items.IndexOf(aItem);
            foreach (int aIdx in aIndices)
                ShowHideRow(aIdx, iShow);
        }

        private void AdjustConditionsPanel(IEnumerable<int> iNewSelectedItems, IEnumerable<int> iNewUnselectedItems)
        {
            foreach (var s in iNewSelectedItems)
            {
                ShowHideRow(s, true);
            }

            foreach (var u in iNewUnselectedItems)
            {
                ShowHideRow(u, false);
            }
        }

        private void ShowHideRow(int iRowIdx, bool iShow)
        {
            // The show/hide transition fades in/out and moves the row down/up

            mBailCond.Conditions[iRowIdx].Enabled = iShow;
            var aPanel = mConditionPanels[iRowIdx];

            // Fade
            DoubleAnimation aDoubleAnim = new DoubleAnimation
            {
                From = iShow ? 0.0 : 1.0,
                To = iShow ? 1.0 : 0.0,
                Duration = new Duration(DefaultAnimationDuration)
            };

            var aStoryBoard = new Storyboard();
            aStoryBoard.Children.Add(aDoubleAnim);
            Storyboard.SetTarget(aDoubleAnim, aPanel);
            Storyboard.SetTargetProperty(aDoubleAnim, new PropertyPath(WrapPanel.OpacityProperty));

            // Move
            var aThickAnim = new ThicknessAnimation
            {
                From = new Thickness(0, iShow ? -aPanel.ActualHeight : 0, 0, 0),
                To = new Thickness(0, iShow ? 0 : -aPanel.ActualHeight, 0, 0),
                Duration = new Duration(DefaultAnimationDuration) // May look better with a slightly faster transition...
            };

            aStoryBoard.Children.Add(aThickAnim);
            Storyboard.SetTarget(aThickAnim, aPanel);
            Storyboard.SetTargetProperty(aThickAnim, new PropertyPath(WrapPanel.MarginProperty));

            aStoryBoard.Begin(this);
        }
    }
}
