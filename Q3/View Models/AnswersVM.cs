using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Q3
{
    class AnswersVM
    {
        private Guid qID;
        private Guid aID;
        private bool isCorrect;
        private string aBody;
        private string tag;
        private bool isSelected;

        public Guid QID { get; set; }
        public Guid AID { get; set; }
        public bool IsCorrectAnswer { get; set; }
        public String AnswerBody { get; set; }
        public string TagNumber { get; set; }
        public bool IsSelected { get { return this.isSelected; } set { this.isSelected = value; } }

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;


        //private ICommand m_ButtonCommand;
        //public ICommand ButtonCommand
        //{
        //    get
        //    {
        //        return m_ButtonCommand;
        //    }
        //    set
        //    {
        //        m_ButtonCommand = value;
        //    }
        //}
        //public AnswersVM()
        //{
        //    ButtonCommand = new RelayCommand(new Action<object>(ShowMessage));
        //}

        //public void ShowMessage(object obj)
        //{
        //    IsSelected = false;
        //    //MessageBox.Show(obj.ToString());
        //}
    }
    
}
