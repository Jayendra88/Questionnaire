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
using System.Windows.Shapes;

namespace Q3
{
    /// <summary>
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        #region Internal Variable

        List<QuestionModel> Questions;

        #endregion

        public AdminWindow()
        {
            InitializeComponent();
        }

        public AdminWindow(User AdminInfo)
        {
            InitializeComponent();
        }

        private void AddNewQuestionBtnClicked(object sender, RoutedEventArgs e)
        {
            AddNewQuestionGrid.Visibility = Visibility.Visible;
            EditQuestionGrid.Visibility = Visibility.Hidden;
        }

        private void EditQuestionBtnClicked(object sender, RoutedEventArgs e)
        {
            AddNewQuestionGrid.Visibility = Visibility.Hidden;
            EditQuestionGrid.Visibility = Visibility.Visible;
        }

        private void AdminWindowLoaded(object sender, RoutedEventArgs e)
        {
            CRUDManager cm = new CRUDManager();
            Questions = cm.GetAllQuestions();
        }
    }
}
