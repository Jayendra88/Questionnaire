using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Q3
{
    class ImageModel
    {
        public Guid ImageID { get; set; }
        public Guid QID { get; set; }
        public byte[] Image { get; set; }
    }
}
