using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey2018.Models
{
    class Questions
    {
        public string Name { get; set; }

        public int Type { get; set; }

        public int Pagenum { get; set; }

        public int NumOfBoxes { get; set; }

        public Answers Box1 { get; set; }
        public Answers Box2 { get; set; }
        public Answers Box3 { get; set; }
        public Answers Box4 { get; set; }
        public Answers Box5 { get; set; }


        public Questions(string _name, int _type, int _PageNum, int _NumOfBoxes, Answers _Box1, Answers _Box2, Answers _Box3, Answers _Box4, Answers _Box5)
        {
            Name = _name;
            Type = _type;
            Pagenum = _PageNum;
            NumOfBoxes = _NumOfBoxes;

            Box1 = _Box1;
            Box2 = _Box2;
            Box3 = _Box3;
            Box4 = _Box4;
            Box5 = _Box5;
        }

        public override string ToString()
        {


            return " 1. Διαφωνώ πάρα πολύ: " +
                   "    Checked   :" + Box1.results.CheckedPixels.ToString("000000") +
                   "\n" +
                   " 2. Διαφωνώ αρκετά:  " +
                   "    Checked   :" + Box2.results.CheckedPixels.ToString("000000") +
                   "\n" +
                   " 3. Ούτε συμφωνώ/ διαφωνώ:  " +
                   "    Checked   :" + Box3.results.CheckedPixels.ToString("000000") +
                   "\n" +
                   " 4. Συμφωνώ αρκετά:  " +
                   "    Checked   :" + Box4.results.CheckedPixels.ToString("000000") +
                   "\n" +
                   " 5. Συμφωνώ πάρα πολύ:  " +
                   "    Checked   :" + Box5.results.CheckedPixels.ToString("000000") +
                   "\n";

        }


    }
}
