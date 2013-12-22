using System;
using System.Collections.Generic;
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
using System.Collections.ObjectModel;

namespace Q3.Views
{
    /// <summary>
    /// Interaction logic for AdminAddQuestion.xaml
    /// </summary>
    public partial class AdminAddQuestion : UserControl
    {
        #region Internal Variables

        int AnswersCount = 0;
        int ImagesCount = 0;
        Dictionary<int, string> AnswersDictionary = new Dictionary<int, string>();
        Dictionary<int, string> ImageDictionary = new Dictionary<int, string>();

        #endregion
        
        public AdminAddQuestion()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Open dialog to choose images
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddImagesBtnClicked(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "SelectImage"; // Default file name
            dlg.DefaultExt = ".jpg"; // Default file extension
            dlg.Filter = "JPG Files (*.jpg)|*.jpg|PNG Files (*.png)|*.png|JPEG Files (*.jpeg)|*.jpeg"; // Filter files by extension 

            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                Image image = new Image();
                image.Margin = new Thickness(0, 5, 10, 5);
                image.MaxHeight = 150;
                image.MaxWidth = 150;
                image.Name = "QImage" + ImagesCount.ToString();
                RegisterName(image.Name, image);
                image.Source = new BitmapImage(new Uri(filename));

                ContextMenu contextmenu = new ContextMenu();
                MenuItem delete = new MenuItem();
                delete.Header = "Delete";
                delete.Tag = ImagesCount.ToString();
                delete.Click += AddQRemoveImageCMClicked;
                contextmenu.Items.Add(delete);
                image.ContextMenu = contextmenu;

                ImagesStackPanel.Children.Add(image);
                ImageDictionary.Add(ImagesCount, image.Name);
                ImagesCount++;
            }
        }

        /// <summary>
        /// Add new answer button clicked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddNewAnswerBtnClicked(object sender, RoutedEventArgs e)
        {
            Grid g = new Grid();
            g.Name = "AnswerGrid" + AnswersCount.ToString();
            RegisterName(g.Name, g);
            g.Tag = AnswersCount.ToString();
            g.MinHeight = 30;

            ColumnDefinition cl = new ColumnDefinition { Width = new GridLength(40.0, GridUnitType.Pixel) };
            ColumnDefinition cm = new ColumnDefinition { Width = new GridLength(20.0, GridUnitType.Pixel) };
            ColumnDefinition cr = new ColumnDefinition { };
            g.ColumnDefinitions.Add(cl);
            g.ColumnDefinitions.Add(cm);
            g.ColumnDefinitions.Add(cr);

            Button btn = new Button();
            btn.VerticalAlignment = VerticalAlignment.Center;
            btn.MinHeight = 35;
            btn.Name = "AnswerCancelBtn"+AnswersCount.ToString();
            RegisterName(btn.Name, btn);
            btn.Tag = AnswersCount.ToString();
            btn.Click += AddQARemoveBtnClicked;
            Grid.SetColumn(btn,0);
            g.Children.Add(btn);

            RadioButton rb = new RadioButton();
            rb.Name = "AnswerRadioButton" + AnswersCount.ToString();
            RegisterName(rb.Name, rb);
            rb.Click += AddQRadioBClicked;//
            rb.Tag = AnswersCount.ToString();
            rb.HorizontalAlignment = HorizontalAlignment.Center;
            rb.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumn(rb, 1);
            g.Children.Add(rb);

            RichTextBox textBox = new RichTextBox();
            textBox.Name = "AnswerRTB" + AnswersCount.ToString();
            RegisterName(textBox.Name, textBox);
            textBox.Margin = new Thickness(5);
            textBox.Tag = AnswersCount.ToString();
            textBox.FontSize = 20;
            Grid.SetColumn(textBox,2);
            g.Children.Add(textBox);

            AnswersDictionary.Add(AnswersCount,textBox.Name);
            AnswersCount++;
            AnswersStackPannel.Children.Add(g);
        }

        /// <summary>
        /// save new question to database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveAnswerBtnClicked(object sender, RoutedEventArgs e)
        {
            QuestionContentRTB.SelectAll();

            AdminWindowVM aw = (AdminWindowVM)this.DataContext;

            #region Validate Input Fields

            if (QuestionContentRTB.Selection.Text == "") 
            {
                MessageBox.Show("Question is not defined");
                return;
            }
            if(AnswersDictionary.Count ==0)
            {
                MessageBox.Show("No answers added");
                return;
            }
            if (AnswersDictionary.Count != 0) 
            {
                bool IsanswrsEmpty = true;
                for (int i = 0; i < AnswersCount; i++)
                {
                    if (AnswersDictionary.ContainsKey(i))
                    {
                        try
                        {
                            RichTextBox tb = (RichTextBox)AnswersStackPannel.FindName("AnswerRTB" + i.ToString());
                            tb.SelectAll();
                            if (tb.Selection.Text.Trim() != "")
                            {
                                IsanswrsEmpty = false;
                            }
                        }
                        catch { }
                    }
                }
                if (IsanswrsEmpty) 
                {
                    MessageBox.Show("No answers added");
                    return;
                }
            }
            if (AnswersDictionary.Count != 0)
            {
                bool IsAChecked = false;
                for (int i = 0; i < AnswersCount; i++)
                {
                    if (AnswersDictionary.ContainsKey(i))
                    {
                        try
                        {
                            RadioButton rb = (RadioButton)AnswersStackPannel.FindName("AnswerRadioButton" + i.ToString());
                            if ((bool)rb.IsChecked)
                            {
                                IsAChecked = true;
                            }
                        }
                        catch { }
                    }
                }
                if (!IsAChecked)
                {
                    MessageBox.Show("Select Correct Answer");
                    return;
                }
            }
            #endregion

            QuestionModel NewQuestion = new QuestionModel();
            NewQuestion.Answers = new List<AnswersModel>();
            NewQuestion.Images = new List<ImageModel>();
            NewQuestion.QID = Guid.NewGuid();
            NewQuestion.QBody = QuestionContentRTB.Selection.Text.Trim();
            NewQuestion.HasMultipleAnswers = false;

            if (ImageDictionary.Count != 0) NewQuestion.HasImages = true;

            for (int i = 0; i < AnswersCount; i++) 
            {
                if (AnswersDictionary.ContainsKey(i))
                {
                    
                    RichTextBox tb = (RichTextBox)AnswersStackPannel.FindName("AnswerRTB" + i.ToString());
                    tb.SelectAll();
                    if (tb.Selection.Text.Trim() != "")
                    {
                        AnswersModel answer = new AnswersModel();
                        answer.QID = NewQuestion.QID;
                        answer.AID = Guid.NewGuid();
                        answer.AnswerBody = tb.Selection.Text.Trim();
                        RadioButton rb = (RadioButton)AnswersStackPannel.FindName("AnswerRadioButton" + i.ToString());
                        answer.IsCorrectAnswer = (bool)rb.IsChecked;
                        NewQuestion.Answers.Add(answer);
                    }
                }
            }
            for (int i = 0; i < ImagesCount; i++)
            {
                if (ImageDictionary.ContainsKey(i))
                {
                    try 
                    {
                        Image img = (Image)ImagesStackPanel.FindName("QImage" + i.ToString());
                        BitmapImage bimg = (BitmapImage)img.Source;
                        FileStream fs = new FileStream(bimg.UriSource.LocalPath.ToString(), FileMode.Open, FileAccess.Read);
                        byte[] picbyte = new byte[fs.Length];
                        fs.Read(picbyte, 0, System.Convert.ToInt32(fs.Length));
                        fs.Close();

                        ImageModel imodel = new ImageModel();
                        imodel.QID = NewQuestion.QID;
                        imodel.ImageID = Guid.NewGuid();
                        imodel.Image = picbyte;

                        NewQuestion.Images.Add(imodel);
                    }
                    catch { }
                }
            }

            CRUDManager dm = new CRUDManager();
            if (!dm.AddNewQuestion(NewQuestion))
            {
                MessageBox.Show("Question Save Failed");
                return;
            }
            else 
            {
                aw.QList.Add(ConvertModelToVM(NewQuestion));
                AddNewQuesttionPanelClear();
                MessageBox.Show("Question Successfully Added"); 
            }

           
        }

        /// <summary>
        /// clear all content
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearAnswerBtnClicked(object sender, RoutedEventArgs e)
        {
            AddNewQuesttionPanelClear();
        }

        #region Internal (Private) Methods

        /// <summary>
        /// Clear all Content
        /// </summary>
        private void AddNewQuesttionPanelClear()
        {
            ImagesStackPanel.Children.Clear();
            AnswersStackPannel.Children.Clear();
            AnswersDictionary.Clear();
            ImageDictionary.Clear();
            QuestionContentRTB.SelectAll();
            QuestionContentRTB.Selection.Text = "";
        }

        private QuestionVM ConvertModelToVM(QuestionModel Question)
        {
            if (Question == null) return null;

            QuestionVM qvm = new QuestionVM();
            qvm.QID = Question.QID;
            qvm.QBody = Question.QBody;
            if (qvm.QBody.Length > 40)
            {
                qvm.QHeader = qvm.QBody.Substring(0, 40) + "...";
            }
            else qvm.QHeader = qvm.QBody;

            qvm.HasImages = Question.HasImages;
            qvm.Answers = new ObservableCollection<AnswersVM>();
            for (int j = 0; j < Question.Answers.Count; j++)
            {
                AnswersVM a = new AnswersVM();
                a.AID = Question.Answers[j].AID;
                a.QID = Question.Answers[j].QID;
                a.AnswerBody = Question.Answers[j].AnswerBody;
                a.IsCorrectAnswer = Question.Answers[j].IsCorrectAnswer;
                qvm.Answers.Add(a);
            }
            qvm.Images = new ObservableCollection<ImageModel>();
            for (int j = 0; j < Question.Images.Count; j++)
            {
                qvm.Images.Add(Question.Images[j]);
            }

            return qvm;
        }

        #endregion

        #region Panel Control Events CodeBehind

        /// <summary>
        /// Synchronize all the radio buttons (if one checked others will unchecked)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddQRadioBClicked(object sender, RoutedEventArgs e)
        {
            if (sender != null)
            {
                RadioButton rbtn = sender as RadioButton;
                for (int i = 0; i < AnswersCount; i++)
                {
                    if (AnswersDictionary.ContainsKey(i))
                    {
                        if (rbtn.Tag.ToString() != i.ToString())
                        {
                            ((RadioButton)AnswersStackPannel.FindName("AnswerRadioButton" + i.ToString())).IsChecked = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Remove answer clicked (remove ui element)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddQARemoveBtnClicked(object sender, RoutedEventArgs e)
        {
            if (sender != null)
            {
                Button btn = sender as Button;
                if (AnswersDictionary.ContainsKey(Convert.ToInt32(btn.Tag.ToString().Trim())))
                {
                    Grid grid = (Grid)AnswersStackPannel.FindName("AnswerGrid" + btn.Tag.ToString());
                    AnswersDictionary.Remove(Convert.ToInt32(btn.Tag.ToString().Trim()));
                    AnswersStackPannel.Children.Remove(grid);
                }
            }
        }

        /// <summary>
        /// Remove images
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddQRemoveImageCMClicked(object sender, RoutedEventArgs e)
        {
            if (sender != null)
            {
                MenuItem item = sender as MenuItem;
                if (ImageDictionary.ContainsKey(Convert.ToInt32(item.Tag.ToString().Trim())))
                {
                    Image img = (Image)AnswersStackPannel.FindName("QImage" + item.Tag.ToString());
                    ImageDictionary.Remove(Convert.ToInt32(item.Tag.ToString().Trim()));
                    ImagesStackPanel.Children.Remove(img);
                }
            }
        }

        #endregion
   }
}

