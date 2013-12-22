using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

namespace Q3
{
    /// <summary>
    /// Interaction logic for Question.xaml
    /// </summary>
    public partial class Question : UserControl
    {
        public Question()
        {
            InitializeComponent();
        }

        private void QuestionLoaded(object sender, RoutedEventArgs e)
        {
            QuestionVM dc = (QuestionVM)this.DataContext;

            for (int i = 0; i < dc.Answers.Count; i++) 
            {
                Grid g = new Grid();
                g.Name = "AnswerGrid" + dc.Answers[i].TagNumber.ToString();
                RegisterName(g.Name, g);
                g.Tag = dc.QNumber.ToString();
                g.MinHeight = 30;

                ColumnDefinition cm = new ColumnDefinition { Width = new GridLength(20.0, GridUnitType.Pixel) };
                ColumnDefinition cr = new ColumnDefinition { };
                g.ColumnDefinitions.Add(cm);
                g.ColumnDefinitions.Add(cr);

                RadioButton rb = new RadioButton();
                rb.Name = "AnswerRadioButton" + dc.Answers[i].TagNumber.ToString();
                RegisterName(rb.Name, rb);
                rb.Click += AddQRadioBClicked;//
                rb.Tag = dc.Answers[i].TagNumber.ToString();
                rb.HorizontalAlignment = HorizontalAlignment.Center;
                rb.VerticalAlignment = VerticalAlignment.Center;
                Grid.SetColumn(rb, 0);
                g.Children.Add(rb);

                TextBlock textBox = new TextBlock();
                textBox.Name = "AnswerTB" + dc.Answers[i].TagNumber.ToString();
                RegisterName(textBox.Name, textBox);
                textBox.Margin = new Thickness(5);
                textBox.TextWrapping = TextWrapping.Wrap;
                textBox.Tag = dc.Answers[i].ToString();
                textBox.Text = dc.Answers[i].AnswerBody;
                textBox.FontSize = 20;
                Grid.SetColumn(textBox, 1);
                g.Children.Add(textBox);

                AnswersSP.Children.Add(g);
            }
            for (int i = 0; i < dc.Images.Count; i++) 
            {
                var bimage = new BitmapImage();
                using (var mem = new MemoryStream(dc.Images[i].Image))
                {
                    mem.Position = 0;
                    bimage.BeginInit();
                    bimage.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                    bimage.CacheOption = BitmapCacheOption.OnLoad;
                    bimage.UriSource = null;
                    bimage.StreamSource = mem;
                    bimage.EndInit();
                }
                bimage.Freeze();

                Image image = new Image();
                image.Margin = new Thickness(0, 5, 10, 5);
                image.MaxHeight = 250;
                image.MaxWidth = 250;
                image.Source = bimage;
                QuestionImagesStackPanel.Children.Add(image);
            }
        }

        private void AddQRadioBClicked(object sender, RoutedEventArgs e)
        {
            if (sender != null)
            {
                QuestionVM dc = (QuestionVM)this.DataContext;
                RadioButton rbtn = sender as RadioButton;
                for (int i = 0; i < dc.Answers.Count(); i++)
                {
                    if (rbtn.Tag.ToString() != i.ToString())
                    {
                        ((RadioButton)AnswersSP.FindName("AnswerRadioButton" + i.ToString())).IsChecked = false;
                        dc.Answers[i].IsSelected = false;
                    }
                    else {dc.Answers[i].IsSelected = true; }
                }
                this.DataContext = dc;
            }
        }
    }
}
