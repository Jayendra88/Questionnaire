using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Q3
{
    class UserWindowVM
    {
        private ObservableCollection<QuestionVM> qList;

        public ObservableCollection<QuestionVM> QList { get { return this.qList; } set { this.qList = value; OnPropertyChanged("QList"); } }



        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
