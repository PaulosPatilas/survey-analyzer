using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey2018.Models
{
    class Answers
    {
        public int Top { get; set; }
        public int Left { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }

        public Results results;

        public  Answers (int _top, int _left, int _height, int _width){
            Top = _top;
            Left = _left;
            Height = _height;
            Width = _width;
        }

        public System.Drawing.Rectangle GetRect
        {
            get { return new System.Drawing.Rectangle(Left ,Top, Width, Height); }
        }
    }
}
