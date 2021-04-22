using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ImageMagick;
using System.Drawing;
using Survey2018.Models;
using Survey2018.Core;
using System.IO;
using Surveys2018.Models;

namespace Surveys2018.Services
{
    class SurveyAnalyzer
    {

        //public const int _CONVERTED_DOC_RESOLUTION = 150; //dpi
        //public const int _DOC_WIDTH = 1240;
        //public const int _DOC_HEIGHT = 1760;
        //public const int _DOC_OFFSET = 300;
        //public const int _DOC_PAGEOFFSET = 50;
        //public const int _DOC_NUMOFPAGES = 3;


        //public const int _QUEST_LEFT_INDICATORE_ARIA_MIN = 60;
        //public const int _QUEST_LEFT_INDICATORE_ARIA_MAX = 100;

        //public const int _QUEST_RIGHT_INDICATORE_ARIA_MIN = 1110;
        //public const int _QUEST_RIGHT_INDICATORE_ARIA_MAX = 1160;


        //public const int A_x = 867;
        //public const int B_x = 915;
        //public const int C_x = 965;
        //public const int D_x = 1015;
        //public const int E_x = 1066;

        //public const int A_y = 40;
        //public const int B_y = 82;
        //public const int C_y = 129;
        //public const int D_y = 180;
        //public const int E_y = 225;

        Variables var = new Variables();

        DataBase _oradb = new DataBase();

        Entity ent = new Entity();
        Document doc = new Document();

        Random rnd = new Random();
        int value;


        //Consents con = new Consents();

        public List<Questions> qList = new List<Questions>();

        public void ProcessDocument(string DocName, string DocType, string SurveyCode  )
        {
            Bitmap source;
            String TemporaryFileName;
            Consents con = new Consents();

            //όρισε τα settings της εικόνας - ανάλυση και background;
            MagickReadSettings settings = new MagickReadSettings();
            settings.Density = new Density(var._CONVERTED_DOC_RESOLUTION);
            //settings.BackgroundColor = new MagickColor(Color.White);

            //Μετατροπή του pdf σε εικόνα 
            using (MagickImageCollection images = new MagickImageCollection())
            {
                // Add all the pages of the pdf file to the collection
                images.Read(DocName, settings);

                IMagickImage vertical = images.AppendVertically();
                //vertical.Transparent(MagickColors.White);
                vertical.Format = MagickFormat.Png;
                source = vertical.ToBitmap();
            }

            if (true)
            {
                TemporaryFileName = System.IO.Path.GetDirectoryName(DocName) + @"\_temp.png";
                source.Save(TemporaryFileName);
            }

            //Βρες του οδηγούς στα ερωτηματολόγια  
            DetectPositionsType_1(source);
            DetectPositionsType_2(source);

            int answer = 1;
            int NumberOfQuest = 0;

            ent.CENT_CODE = Path.GetFileNameWithoutExtension(DocName).Substring(0, 4);  // DateTime.Now.Day + DateTime.Now.Month;   //Θα βρισκεται μεσα στο barcode; Bαζω μονο το 4-ψηφιο ή προσθετω και αλλα ψηφία;
            ent.CENT_TYPE = DocType;

            //insert Document
            doc.CDOC_CSDOC_CODE = SurveyCode;
            doc.CDOC_CENT_CODE = ent.CENT_CODE;
            doc.CDOC_CENT_TYPE = ent.CENT_TYPE;

            con.CreateConsent(doc);

            if (_oradb.GetDocument(doc.CDOC_ID) == null)
            {
                doc.CDOC_ID = Path.GetFileNameWithoutExtension(DocName);

                if (CheckPageOrder(source))
                {
                    doc.CDOC_STATUS = 1;
                }
                else
                {
                    doc.CDOC_STATUS = 0;
                }
                doc.CDOC_BATCHNO = "";
                doc.CDOC_CREATEDDATE = DateTime.Now;
                doc.CDOC_UPDATEDATE = DateTime.Now;
                try
                {
                    _oradb.InsertDocument(doc);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            if (CheckPageOrder(source))
            {
                foreach (var q in qList)
                {
                    string resultsText = string.Empty;

                    NumberOfQuest++;

                    ProcessQuestion(source, q);

                    if (NumberOfQuest <= 52)
                    {

                        if (q.Box1.results.CheckedPixels > q.Box2.results.CheckedPixels && q.Box1.results.CheckedPixels > q.Box3.results.CheckedPixels && q.Box1.results.CheckedPixels > q.Box4.results.CheckedPixels && q.Box1.results.CheckedPixels > q.Box5.results.CheckedPixels)
                        {
                            resultsText = "1. Διαφωνώ πάρα πολύ ";
                            
                        }
                        else if (q.Box2.results.CheckedPixels > q.Box1.results.CheckedPixels && q.Box2.results.CheckedPixels > q.Box3.results.CheckedPixels && q.Box2.results.CheckedPixels > q.Box4.results.CheckedPixels && q.Box2.results.CheckedPixels > q.Box5.results.CheckedPixels)
                        {
                            resultsText = "2. Διαφωνώ αρκετά ";
                            
                        }
                        else if (q.Box3.results.CheckedPixels > q.Box1.results.CheckedPixels && q.Box3.results.CheckedPixels > q.Box2.results.CheckedPixels && q.Box3.results.CheckedPixels > q.Box4.results.CheckedPixels && q.Box3.results.CheckedPixels > q.Box5.results.CheckedPixels)
                        {

                            resultsText = "3. Ούτε Συμφωνώ / Ούτε διαφωνώ ";

                        }
                        else if (q.Box4.results.CheckedPixels > q.Box1.results.CheckedPixels && q.Box4.results.CheckedPixels > q.Box2.results.CheckedPixels && q.Box4.results.CheckedPixels > q.Box3.results.CheckedPixels && q.Box4.results.CheckedPixels > q.Box5.results.CheckedPixels)
                        {

                            resultsText = "4. Συμφωνώ αρκετά: ";

                        }
                        else
                        {
                            resultsText = "5. Συμφωνώ πάρα πολύ: ";

                        }
                        //Debug.Print(q.ToString());αψαβ
                    }
                    else if (NumberOfQuest == 54)
                    {
                        if (q.Box1.results.CheckedPixels > q.Box2.results.CheckedPixels)
                        {
                            resultsText = "1. Άνδρας ";
                        }
                        else
                        {
                            resultsText = "2. Γυναίκα ";
                        }

                    }
                    else if (NumberOfQuest == 55)
                    {
                        if (q.Box1.results.CheckedPixels > q.Box2.results.CheckedPixels && q.Box1.results.CheckedPixels > q.Box3.results.CheckedPixels && q.Box1.results.CheckedPixels > q.Box4.results.CheckedPixels && q.Box1.results.CheckedPixels > q.Box5.results.CheckedPixels)
                        {
                            resultsText = "1. 18 - 25 ";
                        }
                        else if (q.Box2.results.CheckedPixels > q.Box1.results.CheckedPixels && q.Box2.results.CheckedPixels > q.Box3.results.CheckedPixels && q.Box2.results.CheckedPixels > q.Box4.results.CheckedPixels && q.Box2.results.CheckedPixels > q.Box5.results.CheckedPixels)
                        {
                            resultsText = "2. 26 - 35 ";
                        }
                        else if (q.Box3.results.CheckedPixels > q.Box1.results.CheckedPixels && q.Box3.results.CheckedPixels > q.Box2.results.CheckedPixels && q.Box3.results.CheckedPixels > q.Box4.results.CheckedPixels && q.Box3.results.CheckedPixels > q.Box5.results.CheckedPixels)
                        {
                            resultsText = "3. 36 - 45 ";
                        }
                        else if (q.Box4.results.CheckedPixels > q.Box1.results.CheckedPixels && q.Box4.results.CheckedPixels > q.Box2.results.CheckedPixels && q.Box4.results.CheckedPixels > q.Box3.results.CheckedPixels && q.Box4.results.CheckedPixels > q.Box5.results.CheckedPixels)
                        {
                            resultsText = "4. 46 - 55 ";
                        }
                        else
                        {
                            resultsText = "5. 56 και άνω ";
                        }
                    }
                    else if (NumberOfQuest == 56)
                    {
                        if (q.Box1.results.CheckedPixels > q.Box2.results.CheckedPixels && q.Box1.results.CheckedPixels > q.Box3.results.CheckedPixels && q.Box1.results.CheckedPixels > q.Box4.results.CheckedPixels && q.Box1.results.CheckedPixels > q.Box5.results.CheckedPixels)
                        {
                            resultsText = "1. 0 - 2 ";
                        }
                        else if (q.Box2.results.CheckedPixels > q.Box1.results.CheckedPixels && q.Box2.results.CheckedPixels > q.Box3.results.CheckedPixels && q.Box2.results.CheckedPixels > q.Box4.results.CheckedPixels && q.Box2.results.CheckedPixels > q.Box5.results.CheckedPixels)
                        {
                            resultsText = "2. 2 - 5 ";
                        }
                        else if (q.Box3.results.CheckedPixels > q.Box1.results.CheckedPixels && q.Box3.results.CheckedPixels > q.Box2.results.CheckedPixels && q.Box3.results.CheckedPixels > q.Box4.results.CheckedPixels && q.Box3.results.CheckedPixels > q.Box5.results.CheckedPixels)
                        {
                            resultsText = "3. 5 - 10 ";
                        }
                        else if (q.Box4.results.CheckedPixels > q.Box1.results.CheckedPixels && q.Box4.results.CheckedPixels > q.Box2.results.CheckedPixels && q.Box4.results.CheckedPixels > q.Box3.results.CheckedPixels && q.Box4.results.CheckedPixels > q.Box5.results.CheckedPixels)
                        {
                            resultsText = "4. 10 -20 ";
                        }
                        else
                        {
                            resultsText = "5. 20 και άνω ";
                        }
                    }
                    else if (NumberOfQuest == 57)
                    {
                        if (q.Box1.results.CheckedPixels > q.Box2.results.CheckedPixels && q.Box1.results.CheckedPixels > q.Box3.results.CheckedPixels)
                        {
                            resultsText = "1. Οκτάωρη ";
                        }
                        else if (q.Box2.results.CheckedPixels > q.Box1.results.CheckedPixels && q.Box2.results.CheckedPixels > q.Box3.results.CheckedPixels)
                        {
                            resultsText = "1. Εξάωρη ";
                        }
                        else
                        {
                            resultsText = "1. Τετράωρη ";
                        }
                    }
                    else if (NumberOfQuest == 58)
                    {
                        if (q.Box1.results.CheckedPixels > q.Box2.results.CheckedPixels)
                        {
                            resultsText = "1. Γενικός Διευθυντής/ Διευθυντής/ Περιφερειακός Διευθυντής/ Υποδιευθυντής Καταστήματος/ Διευθυντής Καταστήματος/ Προϊστάμενος Τμήματος ";
                        }
                        else
                        {
                            resultsText = "2. Υπάλληλος Καταστήματος/ Υπάλληλος Γενικής Διεύθυνσης Εφοδιαστικής Αλυσίδας/ Υπάλληλος Κεντρικών Γραφείων ";
                        }
                    }

                    LoadText(DocName, answer, q, resultsText);

                    answer++;
                }
            }

            //insert documentdetails
            try
            {
                con.ProcessConsent(qList, source, DocName);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static void LoadText(string filename, int Num, Questions q, string resultsText)
        {

            filename += ".txt";

            if (!File.Exists(filename))
            {
                using (StreamWriter sw = File.CreateText(filename))
                {
                    sw.WriteLine("");
                }
            }

            using (StreamWriter sw = File.AppendText(filename))
            {
                sw.WriteLine("Info for answer: " + Num);
                sw.WriteLine(resultsText + "\n");
                sw.WriteLine(q.ToString());
            }
        }

        private void ProcessQuestion(Bitmap sourceBmp, Questions q)
        {
            if (q.NumOfBoxes >= 1)
            {
                q.Box1.results = CalculateBpixels(sourceBmp, q.Box1.GetRect);
            }
            else
            {
                q.Box1.results = new Results();
            }

            if (q.NumOfBoxes >= 2)
            {
                q.Box2.results = CalculateBpixels(sourceBmp, q.Box2.GetRect);
            }
            else
            {
                q.Box2.results = new Results();
            }

            if (q.NumOfBoxes >= 3)
            {
                q.Box3.results = CalculateBpixels(sourceBmp, q.Box3.GetRect);
            }
            else
            {
                q.Box3.results = new Results();
            }


            if (q.NumOfBoxes >= 4)
            {
                q.Box4.results = CalculateBpixels(sourceBmp, q.Box4.GetRect);
            }
            else
            {
                q.Box4.results = new Results();
            }


            if (q.NumOfBoxes >= 5)
            {
                q.Box5.results = CalculateBpixels(sourceBmp, q.Box5.GetRect);
            }
            else
            {
                q.Box5.results = new Results();
            }
        }

        private void DetectPositionsType_1(Bitmap sourceBmp)
        {

            int x, y;
            int tmp;
            int quest;
            int PageNum=1;
            DataBase _db = new DataBase();

            quest = 1;

            for (y = var._DOC_OFFSET+ var._DOC_PAGEOFFSET; y <= var._DOC_HEIGHT * var._DOC_NUMOFPAGES; y++)
            {
                //Βρες τον αριθμό σελίδας 
                PageNum = (y / var._DOC_HEIGHT)+1;


                for (x = var._QUEST_RIGHT_INDICATORE_ARIA_MIN; x <= var._QUEST_RIGHT_INDICATORE_ARIA_MAX; x++)
                {
                    tmp = (sourceBmp.GetPixel(x, y).R + sourceBmp.GetPixel(x, y).G + sourceBmp.GetPixel(x, y).B) / 3;

                    if (tmp < 200)
                    {
                        Rectangle area = new Rectangle(x - 4, y - 4, 8, 8);

                        if (CheckArea(sourceBmp, area) && quest <= 53)
                        {

                            Questions q = new Questions(quest.ToString(), 1, (PageNum + 1), _db.GetNumOfAnswers(quest), new Answers(y - 20, var.A_x - 25, 50, 50),
                                                            new Answers(y - 20, var.B_x - 25, 50, 50),
                                                            new Answers(y - 20, var.C_x - 25, 50, 50),
                                                            new Answers(y - 20, var.D_x - 25, 50, 50),
                                                            new Answers(y - 20, var.E_x - 25, 50, 50));

                            qList.Add(q);
                            x = var._QUEST_RIGHT_INDICATORE_ARIA_MAX;
                            y = y + 30;
                            quest++;
                        }
                    }
                }

                //Αν έχεις φτάσει στο τέλος της σελίδας, προχώρισε τον άξονα y στην επόμενη, συνυπολλογίζοντας το offset 
                if (y >= PageNum * (var._DOC_HEIGHT - var._DOC_PAGEOFFSET))
                {
                    y = ((PageNum)* var._DOC_HEIGHT) + var._DOC_PAGEOFFSET;
                }
            }
        }

        private void DetectPositionsType_2(Bitmap sourceBmp)
        {
            int x, y;
            int tmp;
            int quest;

            DataBase _db = new DataBase();

            quest = 54;
            
            for (y = (var._DOC_NUMOFPAGES -1)* var._DOC_HEIGHT; y <= (var._DOC_NUMOFPAGES * var._DOC_HEIGHT) - var._DOC_PAGEOFFSET; y++)
            {
                for (x = var._QUEST_LEFT_INDICATORE_ARIA_MIN; x <= var._QUEST_LEFT_INDICATORE_ARIA_MAX; x++)
                {
                    tmp = (sourceBmp.GetPixel(x, y).R + sourceBmp.GetPixel(x, y).G + sourceBmp.GetPixel(x, y).B) / 3;

                    if (tmp < 200)
                    {
                        Rectangle area = new Rectangle(x - 4, y - 4, 8, 8);
                        if (CheckArea(sourceBmp, area))
                        {

                            //Debug.Print("quest:" + quest.ToString() + " x =" + area.X.ToString() + "  y=" + area.Y.ToString());

                            int PageNum;

                            //Βρες τον αριθμό σελίδας 
                            PageNum = y / var._DOC_HEIGHT;

                            Questions q = new Questions(quest.ToString(), 2, PageNum, _db.GetNumOfAnswers(quest),
                                                            new Answers(y + var.A_y, 110, 38, 38),
                                                            new Answers(y + var.B_y, 110, 38, 38),
                                                            new Answers(y + var.C_y, 110, 38, 38),
                                                            new Answers(y + var.D_y, 110, 38, 38),
                                                            new Answers(y + var.E_y, 110, 38, 38));

                            qList.Add(q);
                            y = y + 30;
                            x = var._QUEST_LEFT_INDICATORE_ARIA_MAX;
                            quest++;
                        }
                    }
                }
            }
        }

        public bool AnswerValidation(Bitmap sourceBmp, Questions q)
        {
            // int x = q.Box1.Left;
            int y = q.Box1.Top;
            int A_1 = 0;
            int A_2 = 0;
            int A_3 = 0;
            int A_4 = 0;
            int A_5 = 0;
            int repeat = 0;
            bool Flag = false;


            Rectangle area_1 = new Rectangle(var.A_x - 5, y + 15, 15, 15);
            A_1 = CalculatePixels(sourceBmp, area_1);
            // Console.WriteLine(A_1);

            Rectangle area_2 = new Rectangle(var.B_x - 5, y + 15, 15, 15);
            A_2 = CalculatePixels(sourceBmp, area_2);
            // Console.WriteLine(A_2);

            Rectangle area_3 = new Rectangle(var.C_x - 5, y + 15, 15, 15);
            A_3 = CalculatePixels(sourceBmp, area_3);
            // Console.WriteLine(A_3);

            Rectangle area_4 = new Rectangle(var.D_x - 5, y + 15, 15, 15);
            A_4 = CalculatePixels(sourceBmp, area_4);
            // Console.WriteLine(A_4);

            Rectangle area_5 = new Rectangle(var.E_x - 5, y + 15, 15, 15);
            A_5 = CalculatePixels(sourceBmp, area_5);
            // Console.WriteLine(A_5);

            Bitmap cImg1 = sourceBmp.Clone(area_1, sourceBmp.PixelFormat);
            Bitmap cImg2 = sourceBmp.Clone(area_2, sourceBmp.PixelFormat);
            Bitmap cImg3 = sourceBmp.Clone(area_3, sourceBmp.PixelFormat);
            Bitmap cImg4 = sourceBmp.Clone(area_4, sourceBmp.PixelFormat);
            Bitmap cImg5 = sourceBmp.Clone(area_5, sourceBmp.PixelFormat);

            cImg1.Save(@"C:\temp\test_1.bmp");
            cImg2.Save(@"C:\temp\test_2.bmp");
            cImg3.Save(@"C:\temp\test_3.bmp");
            cImg4.Save(@"C:\temp\test_4.bmp");
            cImg5.Save(@"C:\temp\test_5.bmp");

            if (A_1 >= A_2 && A_1 >= A_3 && A_1 >= A_4 && A_1 >= A_5)
            {
                if (A_1 < 85 && repeat == 0)
                {
                    // Debug.Print("Η ερώτηση " + quest + " δεν απαντήθηκε");
                    Flag = true;
                    repeat++;
                }
            }

            else if (A_2 >= A_1 && A_2 >= A_3 && A_2 >= A_4 && A_2 >= A_5)
            {
                if (A_2 < 85 && repeat == 0)
                {
                    // Debug.Print("Η ερώτηση " + quest + " δεν απαντήθηκε");
                    Flag = true;
                    repeat++;
                }
            }

            else if (A_3 >= A_2 && A_1 <= A_3 && A_3 >= A_4 && A_3 >= A_5)
            {
                if (A_3 < 85 && repeat == 0)
                {
                    // Debug.Print("Η ερώτηση " + quest + " δεν απαντήθηκε");
                    Flag = true;
                    repeat++;
                }
            }

            else if (A_4 >= A_2 && A_4 >= A_3 && A_1 <= A_4 && A_4 >= A_5)
            {
                if (A_4 < 85 && repeat == 0)
                {
                    Flag = true;
                    // Debug.Print("Η ερώτηση " + quest + " δεν απαντήθηκε");
                    repeat++;
                }
            }
            else if (A_5 >= A_2 && A_5 >= A_3 && A_5 >= A_4 && A_5 >= A_1)
            {
                if (A_5 < 85 && repeat == 0)
                {
                    // Debug.Print("Η ερώτηση " + quest + " δεν απαντήθηκε");
                    Flag = true;
                    repeat++;
                }
            }

            if (A_1 > 85 && A_2 > 85 || A_1 > 85 && A_3 > 85 || A_1 > 85 && A_4 > 85 || A_1 > 85 && A_5 > 85)
            {
                // Debug.Print("Η ερώτηση " + quest + " έχει δύο ή περισσότερες απαντήσεις");
                Flag = true;
            }
            else if (A_2 > 85 && A_1 > 85 || A_2 > 85 && A_3 > 85 || A_2 > 85 && A_4 > 85 || A_2 > 85 && A_5 > 85)
            {
                // Debug.Print("Η ερώτηση " + quest + " έχει δύο ή περισσότερες απαντήσεις");
                Flag = true;
            }
            else if (A_3 > 85 && A_2 > 85 || A_3 > 85 && A_1 > 85 || A_3 > 85 && A_4 > 85 || A_3 > 85 && A_5 > 85)
            {
                // Debug.Print("Η ερώτηση " + quest + " έχει δύο ή περισσότερες απαντήσεις");
                Flag = true;
            }
            else if (A_4 > 85 && A_2 > 85 || A_4 > 85 && A_3 > 85 || A_4 > 85 && A_1 > 85 || A_4 > 85 && A_5 > 85)
            {
                // Debug.Print("Η ερώτηση " + quest + " έχει δύο ή περισσότερες απαντήσεις");
                Flag = true;
            }
            else if (A_5 > 85 && A_2 > 85 || A_5 > 85 && A_3 > 85 || A_5 > 85 && A_4 > 85 || A_5 > 85 && A_1 > 85)
            {
                // Debug.Print("Η ερώτηση " + quest + " έχει δύο ή περισσότερες απαντήσεις");
                Flag = true;
            }

            repeat = 0;

            return Flag;
        }

        private bool CheckArea(Bitmap sourceBmp, Rectangle area)
        {
            int tmp;
            int count = 0;

            for (int _y = area.Y; _y <= area.Y + area.Height; _y++)
            {
                for (int _x = area.X; _x <= area.X + area.Width; _x++)
                {
                    tmp = (sourceBmp.GetPixel(_x, _y).R + sourceBmp.GetPixel(_x, _y).G + sourceBmp.GetPixel(_x, _y).B) / 3;
                    if (tmp > 200)
                    {
                        count++;
                        if (count > 10)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private int CalculatePixels(Bitmap sourceBmp, Rectangle area)
        {
            int tmp;

            int r = 0;

            for (int _y = area.Y; _y <= area.Y + area.Height; _y++)
            {
                for (int _x = area.X; _x <= area.X + area.Width; _x++)
                {
                    tmp = (sourceBmp.GetPixel(_x, _y).R + sourceBmp.GetPixel(_x, _y).G + sourceBmp.GetPixel(_x, _y).B) / 3;
                    if (tmp < 200)
                    {
                        r = r + 1;
                    }
                }
            }
            return r;
        }

        private Results CalculateBpixels(Bitmap sourceBmp, Rectangle area)
        {
            int tmp;

            Results r = new Results();

            for (int _y = area.Y; _y <= area.Y + area.Height; _y++)
            {
                for (int _x = area.X; _x <= area.X + area.Width; _x++)
                {
                    tmp = (sourceBmp.GetPixel(_x, _y).R + sourceBmp.GetPixel(_x, _y).G + sourceBmp.GetPixel(_x, _y).B) / 3;
                    if (tmp < 200)
                    {
                        r.CheckedPixels = r.CheckedPixels + 1;
                    }
                }
            }
            return r;
        }

        private bool CheckPageOrder(Bitmap sourceBmp)
        {
            int counter_1 = 0;
            int counter_2 = 0;
            int counter_3 = 0;
            int _y, _x, tmp;

            for (_y = var._DOC_OFFSET + var._DOC_PAGEOFFSET; _y <= var._DOC_HEIGHT - var._DOC_PAGEOFFSET; _y++)
            {
                for (_x = var._QUEST_RIGHT_INDICATORE_ARIA_MIN; _x <= var._QUEST_RIGHT_INDICATORE_ARIA_MAX; _x++)
                {
                    tmp = (sourceBmp.GetPixel(_x, _y).R + sourceBmp.GetPixel(_x, _y).G + sourceBmp.GetPixel(_x, _y).B) / 3;
                    if (tmp < 200)
                    {
                        Rectangle area = new Rectangle(_x, _y, 8, 8);
                        if (CheckArea(sourceBmp, area))
                        {
                            counter_1++;
                            _x = 1160;
                            _y = _y + 25;
                        }
                    }
                }
            }

            for (_y = 1810; _y <= 3400; _y++)
            {
                for (_x = var._QUEST_RIGHT_INDICATORE_ARIA_MIN; _x <= var._QUEST_RIGHT_INDICATORE_ARIA_MAX; _x++)
                {
                    tmp = (sourceBmp.GetPixel(_x, _y).R + sourceBmp.GetPixel(_x, _y).G + sourceBmp.GetPixel(_x, _y).B) / 3;
                    if (tmp < 200)
                    {
                        Rectangle area = new Rectangle(_x, _y, 8, 8);
                        if (CheckArea(sourceBmp, area))
                        {
                            counter_2++;
                            _x = 1160;
                            _y = _y + 25;
                        }

                    }
                }
            }

            for (_y = 3560; _y <= 3760; _y++)
            {
                for (_x = var._QUEST_RIGHT_INDICATORE_ARIA_MIN; _x <= var._QUEST_RIGHT_INDICATORE_ARIA_MAX; _x++)
                {
                    tmp = (sourceBmp.GetPixel(_x, _y).R + sourceBmp.GetPixel(_x, _y).G + sourceBmp.GetPixel(_x, _y).B) / 3;
                    if (tmp < 200)
                    {
                        Rectangle area = new Rectangle(_x, _y, 8, 8);
                        if (CheckArea(sourceBmp, area))
                        {
                            counter_3++;
                            _x = 1160;
                            _y = _y + 25;
                        }
                    }
                }
            }

            if (counter_1 != 21 || counter_2 != 28 || counter_3 != 3)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool FindText(Bitmap sourceBmp)
        {
            int tmp;
            int x;
            int y = 3850;
            int black = 0;
            bool flag = false;
            for (y = 3850; y <= 3930; y = y + 30)
            {
                for (x = 100; x <= 1000; x++)
                {
                    tmp = (sourceBmp.GetPixel(x, y).R + sourceBmp.GetPixel(x, y).G + sourceBmp.GetPixel(x, y).B) / 3;
                    if (tmp < 200)
                    {
                        black++;
                    }
                }
            }
            if (black >= 50)
            {
                flag = true;
            }
            return flag;

        }

        public void MakeTextPng(Bitmap sourceBmp, string DocName)
        {

            Rectangle Text = new Rectangle(95, 3780, 1020, 220);
            Bitmap CImg = sourceBmp.Clone(Text, sourceBmp.PixelFormat);
            CImg.Save(DocName + ".png");

        }

    }
}