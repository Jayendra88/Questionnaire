using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Q3
{
    class AnswersModel
    {
        public Guid QID { get; set; }
        public Guid AID { get; set; }
        public bool IsCorrectAnswer { get; set; }
        public String AnswerBody { get; set; }
        public string TagNumber { get; set; }
        public bool IsSelected { get; set; }
    }
}
