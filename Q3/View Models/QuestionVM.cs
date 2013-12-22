using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Q3
{
    class QuestionVM
    {
        private Guid qID;
        private string qBody;
        private bool hasImages;
        private int qNumber;
        private int selectedAnswer;
        private ObservableCollection<AnswersVM> answers = null;
        private ObservableCollection<ImageModel> images = null;

        public Guid QID { get { return this.qID; } set { this.qID = value; OnPropertyChanged("QID"); } }
        public String QBody { get { return this.qBody; } set { qBody = value; OnPropertyChanged("QBody"); } }
        //public bool HasMultipleAnswers { get; set; }
        public bool HasImages { get { return this.hasImages; } set { hasImages = value; OnPropertyChanged("HasImages"); } }

        public ObservableCollection<AnswersVM> Answers { get { return this.answers; } set { answers = value; OnPropertyChanged("Answers"); } }
        public ObservableCollection<ImageModel> Images { get { return this.images; } set { images = value; OnPropertyChanged("Images"); } }

        public int QNumber { get { return this.qNumber; } set { qNumber = value; OnPropertyChanged("QNumber"); } }
        //public int SelectedAnswer { get; set; }
        public string QHeader { get; set; }

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;


        private ICommand m_ButtonCommand;
        public ICommand ButtonCommand
        {
            get
            {
                return m_ButtonCommand;
            }
            set
            {
                m_ButtonCommand = value;
            }
        }

        public QuestionVM()
        {
            ButtonCommand = new RelayCommand(new Action<object>(ShowMessage));
        }

        public void ShowMessage(object obj)
        {
            MessageBox.Show(obj.ToString());
        }
    }
    class RelayCommand : ICommand
    {
        private Action<object> _action;

        public RelayCommand(Action<object> action)
        {
            _action = action;
        }

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (parameter != null)
            {
                _action(parameter);
            }
            else
            {
                _action("Hello World");
            }
        }

        #endregion
    }
}
