using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Q3
{
    public class User
    {
        public Guid UID { get; set; }
        public String UserName { get; set; }
        public bool IsAdmin { get; set; }
        public String Name { get; set; }
        public String IDNumber { get; set; }
        public String Password { get; set; }
        public int NumberOfAttempts { get; set; } //if user exists in db using same IDNumber he/she has attended to this exam before
    }
}
