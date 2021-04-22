using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey2018.Models
{
    public class Results
    {
        public int AreaPixels { get; set; }
        public int CheckedPixels { get; set; }
        public int SilentAreaPixels { get; set; }
        public int SilentAreaCheckedPixels { get; set; }

        public Results()
        {
            AreaPixels = 0;
            CheckedPixels = 0;
            SilentAreaPixels = 0;
            SilentAreaCheckedPixels = 0;
        }
    }
}
