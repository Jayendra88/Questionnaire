using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Q3
{
    class QuestionModel
    {
        public Guid QID { get; set; }
        public String QBody { get; set; }
        public bool HasMultipleAnswers { get; set; }
        public bool HasImages { get; set; }

        public List<AnswersModel> Answers { get; set; }
        public List<ImageModel> Images { get; set; }

        public int QNumber { get; set; }
        public int SelectedAnswer { get; set; }
    }
}
