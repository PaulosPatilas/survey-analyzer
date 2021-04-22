using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForge.Imaging;
using AForge;
using ImageMagick;
using System.Drawing;
using Survey2018.Models;
using Survey2018.Core;
using System.IO;
using System.Configuration;

namespace Surveys2018.Services
{
    class Consents
    {
        DataBase _db = new DataBase();
        private Document _doc = new Document();

        SurveyAnalyzer service = new SurveyAnalyzer();

        int Answer1 = 0;
        int Answer2 = 0;
        int Answer3 = 0;
        int Answer4 = 0;
        int Answer5 = 0;

        public void CreateConsent(Document doc)
        {
            _doc = doc;
            _doc.sysDocument = LoadSysDocData(doc);
        }

        public void ProcessConsent(List<Questions> questions, Bitmap sourceBmp, string DocName)
        {
            List<DocumentDetails> docDetails = new List<DocumentDetails>();

            int QUEST = 0;

            _doc.DocQuestions = _db.GetSysQuestions(_doc.CDOC_CSDOC_CODE);

            foreach (var quest in _doc.DocQuestions)
            {

                DocumentDetails dt = new DocumentDetails();
                dt.CDDT_CDOC_ID = _doc.CDOC_ID;
                dt.CDDT_CSDDT_CODE = _doc.CDOC_CSDOC_CODE;

                if (quest.SQ_Code == 54)
                {
                    var x = "";
                }

                Questions q = questions.Single(s => s.Name == quest.SQ_Code.ToString());

                QUEST++;

                if (q == null)
                {
                    // Logger.CreateLogEnty(_db, _doc.CDOC_SESSIONID, _doc.CDOC_ID, "D" + sdt.CSDDT_NUMBER, 100, string.Format("Δεν βέθηκε απάντηση στην ερώτηση Νο.{0}", dt.CDDT_CSDDT_NUMBER));
                    throw new Exception(string.Format("Δεν βέθηκε απάντηση στην ερώτηση Νο.{0:000000}", quest.SQ_Code));
                }

                Answer1 = q.Box1.results.CheckedPixels;
                Answer2 = q.Box2.results.CheckedPixels;
                Answer3 = q.Box3.results.CheckedPixels;
                Answer4 = q.Box4.results.CheckedPixels;
                Answer5 = q.Box5.results.CheckedPixels;

                dt.CDDT_PixelsA = Answer1;
                dt.CDDT_PixelsB = Answer2;
                dt.CDDT_PixelsC = Answer3;
                dt.CDDT_PixelsD = Answer4;
                dt.CDDT_PixelsE = Answer5;

                dt.CDDT_QUESTION = QUEST;

                if (quest.SQ_Type == 3)
                {
                    if (service.FindText(sourceBmp))
                    {
                        dt.CDDT_VALUE = 1;
                        service.MakeTextPng(sourceBmp, DocName);
                    }
                    else
                    {
                        dt.CDDT_VALUE = 0;
                    }
                }

                else
                {
                    if (quest.SQ_Type == 1)
                    {
                        if (Answer1 > Answer2 && Answer1 > Answer3 && Answer1 > Answer4 && Answer1 > Answer5)
                        {
                            if (service.AnswerValidation(sourceBmp, q))
                            {
                                dt.CDDT_VALUE = 0;
                            }
                            else
                            {
                                dt.CDDT_VALUE = 1;
                            }
                        }
                        else if (Answer2 > Answer1 && Answer2 > Answer3 && Answer2 > Answer4 && Answer2 > Answer5)
                        {

                            if (service.AnswerValidation(sourceBmp, q))
                            {
                                dt.CDDT_VALUE = 0;
                            }
                            else
                            {
                                dt.CDDT_VALUE = 2;
                            }

                        }
                        else if (Answer3 > Answer2 && Answer1 < Answer3 && Answer3 > Answer4 && Answer3 > Answer5)
                        {


                            if (service.AnswerValidation(sourceBmp, q))
                            {
                                dt.CDDT_VALUE = 0;
                            }
                            else
                            {
                                dt.CDDT_VALUE = 3;
                            }

                        }
                        else if (Answer4 > Answer2 && Answer4 > Answer3 && Answer4 > Answer1 && Answer4 > Answer5)
                        {

                            if (service.AnswerValidation(sourceBmp, q))
                            {
                                dt.CDDT_VALUE = 0;
                            }
                            else
                            {
                                dt.CDDT_VALUE = 4;
                            }

                        }
                        else
                        {

                            if (service.AnswerValidation(sourceBmp, q))
                            {
                                dt.CDDT_VALUE = 0;
                            }
                            else
                            {
                                dt.CDDT_VALUE = 5;
                            }

                        }
                    }
                    else if (quest.SQ_Type == 2)
                    {
                        if (Answer1 > Answer2 && Answer1 > Answer3 && Answer1 > Answer4 && Answer1 > Answer5)
                        {
                            dt.CDDT_VALUE = 1;

                        }
                        else if (Answer2 > Answer1 && Answer2 > Answer3 && Answer2 > Answer4 && Answer2 > Answer5)
                        {
                            dt.CDDT_VALUE = 2;

                        }
                        else if (Answer3 > Answer2 && Answer1 < Answer3 && Answer3 > Answer4 && Answer3 > Answer5)
                        {

                            dt.CDDT_VALUE = 3;

                        }
                        else if (Answer4 > Answer2 && Answer4 > Answer3 && Answer4 > Answer1 && Answer4 > Answer5)
                        {

                            dt.CDDT_VALUE = 4;

                        }
                        else if (Answer5 > Answer2 && Answer5 > Answer3 && Answer5 > Answer1 && Answer5 > Answer4)
                        {
                            dt.CDDT_VALUE = 5;

                        }
                    }
                }

                //Insert Document Details 
                _db.InsertDocumentsDetails(dt);

                docDetails.Add(dt);
            }

            _doc.Details = docDetails;
        }

        //private ENTITY SyncEntity(string type, string Code)
        //{
        //    ENTITY ent = new ENTITY();


        //    ent.CENT_TYPE = type;
        //    ent.CENT_CODE = Code;

        //    if (ent.CENT_TYPE == "Κ")
        //    {
        //        String globalCode;

        //        globalCode = ent.CENT_CODE;

        //        globalCode = "200" + Code.Substring(0, 1) + 0 + Code.Substring(1, 5);

        //        Stores str = _db.GetStores(globalCode);

        //        if (str == null)
        //        {
        //            return null;
        //        }

        //        ent.CENT_NAME = str.STR_Name;

        //        ent.CENT_TYPE = str.STR_Type;

        //        _db.InsertEntity(ent);

        //        return _db.GetENTITY(ent.CENT_CODE);
        //    }
        //    else
        //    {
        //        return null;
        //    }

        //}

        //public bool ValidateConsent()
        //{

        //    _doc.Entity = _db.GetENTITY(_doc.CDOC_CENT_CODE);

        //    if (_doc.Entity == null)
        //    {
        //        _doc.Entity = SyncEntity(_doc.CDOC_CENT_TYPE, _doc.CDOC_CENT_CODE);
        //    }

        //    if (_doc.Entity == null)
        //    {
        //        throw new Exception(string.Format("Δεν βρέθηκε η οντότητα με κωδικό {0}", _doc.CDOC_CENT_CODE));
        //    }

        //    return true;
        //}

        public bool MoveDocument(Boolean hasErrors)
        {
            string filename;
            string sourcePath;
            string destinationPath;

            filename = _doc.CDOC_ID + ".pdf";
            sourcePath = @ConfigurationManager.AppSettings["DOCUMENTS_PATH"];

            if (hasErrors)
            {
                destinationPath = @ConfigurationManager.AppSettings["ERROR_PATH"];
            }
            else
            {
                destinationPath = @ConfigurationManager.AppSettings["DATA_PATH"] + _doc.CDOC_BATCHNO + @"\";
            }


            if (!Directory.Exists(destinationPath))
            {
                Directory.CreateDirectory(destinationPath);
            }

            File.Move(sourcePath + filename, destinationPath + filename);

            return true;

        }

        //public bool Save(string LoadId, string DocName)
        //{
        //   ValidateConsent();
        //    _db.SaveDocument(_doc, DocName);
        //    //MoveDocument(false);

        //    //    Logger.CreateLogEnty(_db, _doc.Doc_Id, "H", 0, string.Format("Επιτυχής Ανάγνωση {0}", _doc.Doc_Id));
        //    return true;
        //}

        private SysDocuments LoadSysDocData(Document doc)
        {
            SysDocuments sysDocDetail = new SysDocuments();

            sysDocDetail = _db.GetSysDocuments(doc.CDOC_CSDOC_CODE);

            if (sysDocDetail is null)
            {
                throw new System.ArgumentException("Δεν βρέθηκε έντυπο με κωδικό {1}", doc.CDOC_CSDOC_CODE);
            }

            return sysDocDetail;
        }

        //public Document LoadDocument(string Id)
        //{
        //    Document doc = new Document();

        //    doc = _db.GetDocument(Id);
        //    doc.Entity = _db.GetENTITY(doc.Doc_Ent_Code);
        //    doc.Details = _db.GetDocumentsDetails(Id);

        //    foreach (var d in doc.Details)
        //    {
        //        doc.sysDocument = _db.GetSysDocuments(d.DCDT_SDOC_Code);
        //    }


        //    return doc;
        //}
    }
}