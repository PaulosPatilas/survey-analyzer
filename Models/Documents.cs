using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Survey2018.Models
{
    public class Document
    {
        public string CDOC_CSDOC_CODE { get; set; }
        public string CDOC_ID { get; set; }
        public DateTime? CDOC_CREATEDDATE { get; set; }
        public DateTime? CDOC_UPDATEDATE { get; set; }
        public int CDOC_STATUS { get; set; }
        public string CDOC_BATCHNO { get; set; }
        public string CDOC_CENT_CODE { get; set; }
        public string CDOC_CENT_TYPE { get; set; }

        public IEnumerable <DocumentDetails> Details;

        public List <SysQuestions> DocQuestions;

        public SysDocuments sysDocument;

    }
}
