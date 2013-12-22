using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Q3
{
    /// <summary>
    /// Interaction logic for UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        #region Internal Variable

        List<Question> QuestionList = new List<Question>();
        int Marks = 0;
        string UserName;
        string IDCardNo;

        #endregion

        public UserWindow()
        {
            InitializeComponent();
        }

        public UserWindow(string name, string id)
        {
            InitializeComponent();
            this.UserName = name;
            this.IDCardNo = id;
        }

        private void UserWindowLoaded(object sender, RoutedEventArgs e)
        {
            

            NameTB.Text = this.UserName;
            IDNumbrTB.Text = this.IDCardNo;

            CRUDManager cm = new CRUDManager();
            //ttbCountDown.TimeSpan = cm.GetTimeSpan();
            int number = cm.GetNumberOfQuestions();
            List<QuestionModel> qm = cm.GetAllQuestions();
            int[] temp = { };
            if (qm.Count > 0)
            {
                Random rnd = new Random();
                temp = Enumerable.Range(0, qm.Count).OrderBy(i => rnd.Next()).ToArray();
            }

            int[] count = { number, temp.Length };

            for (int i = 0; i < count.Min(); i++)
            {
                int qIndex = temp[i];
                QuestionVM qvModel = new QuestionVM();
                qvModel.Answers = new ObservableCollection<AnswersVM>();
                qvModel.Images = new ObservableCollection<ImageModel>();
                qvModel.QNumber = i+1;
                qvModel.QBody = qm[qIndex].QBody;
                for (int j = 0; j < qm[qIndex].Answers.Count; j++) 
                {
                    AnswersVM a = new AnswersVM();
                    a.AnswerBody = qm[qIndex].Answers[j].AnswerBody;
                    a.IsCorrectAnswer = qm[qIndex].Answers[j].IsCorrectAnswer;
                    a.TagNumber = j.ToString();

                    qvModel.Answers.Add(a);
                }
                for (int j = 0; j < qm[qIndex].Images.Count; j++)
                {
                    qvModel.Images.Add(qm[qIndex].Images[j]);
                }

                Question qView = new Question();
                qView.DataContext = qvModel;
                QuestionListSP.Children.Add(qView);
                QuestionList.Add(qView);
            }

        }

        private void BClicked(object sender, RoutedEventArgs e)
        {
            ttbCountDown.IsStarted = false;
            SendForMarking();
        }

        private void StartQuisButtonClicked(object sender, RoutedEventArgs e)
        {
            QuisStartScreen.Visibility = Visibility.Hidden;
            QPanelGrid.Visibility = Visibility.Visible;

            ttbCountDown.Reset();
            ttbCountDown.TimeSpan = TimeSpan.FromHours(1);
            ttbCountDown.IsStarted = true;

        }

        private void ttbCountDown_OnCountDownComplete(object sender, EventArgs e)
        {
            SendForMarking();
        }

        #region Internal Methods

        private void SendForMarking()
        {
            for (int i = 0; i < QuestionList.Count; i++)
            {
                QuestionVM q = (QuestionVM)QuestionList[i].DataContext;
                for (int j = 0; j < q.Answers.Count; j++)
                {
                    if (q.Answers[j].IsCorrectAnswer && q.Answers[j].IsSelected)
                    {
                        Marks++;
                    }
                }
            }
            QuisStartScreen.Visibility = Visibility.Visible;
            StartQuisBtn.Visibility = Visibility.Hidden;
            MarksTB.Visibility = Visibility.Visible;
            QPanelGrid.Visibility = Visibility.Hidden;

            MarksTB.Text = "Marks = " + Marks + "/" + QuestionList.Count;

        }

        #endregion
    }
}
