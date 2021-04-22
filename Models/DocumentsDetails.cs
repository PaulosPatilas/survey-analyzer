using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Survey2018.Models
{
    public class DocumentDetails
    {
        public string CDDT_CSDDT_CODE { get; set; }
        public string CDDT_CDOC_ID { get; set; }
        public int CDDT_QUESTION { get; set; }
        public int CDDT_VALUE { get; set; }
        public int CDDT_PixelsA { get; set; }
        public int CDDT_PixelsB { get; set; }
        public int CDDT_PixelsC { get; set; }
        public int CDDT_PixelsD { get; set; }
        public int CDDT_PixelsE { get; set; }
    }
}
