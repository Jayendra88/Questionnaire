using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Q3
{
    class AdminWindowVM
    {
        private ObservableCollection<QuestionVM> qList;

        public ObservableCollection<QuestionVM> QList 
        { 
            get 
            {
                if(qList == null)
                {
                    CRUDManager cm = new CRUDManager();
                    List<QuestionModel> ql = cm.GetAllQuestions();
                    qList = new ObservableCollection<QuestionVM>();
                    if (ql != null) 
                    {
                        for (int i = 0; i < ql.Count; i++) 
                        {
                            QuestionVM qvm = new QuestionVM();
                            qvm.QID = ql[i].QID;
                            qvm.QBody = ql[i].QBody;
                            if (qvm.QBody.Length > 40)
                            {
                                qvm.QHeader = qvm.QBody.Substring(0, 40) + "...";
                            }
                            else qvm.QHeader = qvm.QBody;

                            qvm.HasImages = ql[i].HasImages;
                            qvm.Answers = new ObservableCollection<AnswersVM>();
                            for (int j = 0; j < ql[i].Answers.Count; j++) 
                            {
                                AnswersVM a = new AnswersVM();
                                a.AID = ql[i].Answers[j].AID;
                                a.QID = ql[i].Answers[j].QID;
                                a.AnswerBody = ql[i].Answers[j].AnswerBody;
                                a.IsCorrectAnswer = ql[i].Answers[j].IsCorrectAnswer;
                                qvm.Answers.Add(a);
                            }
                            qvm.Images = new ObservableCollection<ImageModel>();
                            for (int j = 0; j < ql[i].Images.Count; j++)
                            {
                                qvm.Images.Add(ql[i].Images[j]);
                            }
                            this.qList.Add(qvm);
                        }
                    }
                }
                return this.qList; 
            } 
            set 
            { 
                this.qList = value; OnPropertyChanged("QList"); 
            } 
        }



        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
