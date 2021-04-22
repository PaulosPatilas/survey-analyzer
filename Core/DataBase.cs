using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Survey2018.Models;
using System.Configuration;
using System.Data;
using System.Data.OleDb;

namespace Survey2018.Core
{
    class DataBase
    {
        string myOraDbConn;

        public DataBase()
        {
            myOraDbConn = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        }

        #region Kαταστήματα
        public List<Branches> GetBranches()
        {
            String SQLSTR = "";

            SQLSTR = "SELECT * " +
                "FROM Branches " +
                "WHERE BRA_CMP_Code = 'MET' AND BRA_Type <> 9";
            using (IDbConnection db = new OleDbConnection(myOraDbConn))
            {
                return db.Query<Branches>(SQLSTR).ToList();
            }

        }
        public int GetEmployees(string Bra_Code)
        {
            String SQLSTR = "";

            SQLSTR = "SELECT count(*) FROM Employees " +
                      "WHERE EMP_BRA_Code = ?Bra_Code? AND EMP_CMP_Code = 'MET' AND EMP_RetireDate is null ";

            using (IDbConnection db = new OleDbConnection(myOraDbConn))
            {
                return db.Query<int>(SQLSTR, new { Bra_Code = Bra_Code }).FirstOrDefault();
            }
        }
        public String GetBranchName(String Bra_Code)
        {
            String SQLSTR = "";
            SQLSTR = "SELECT FROM Branches.BRA_Name " +
                "WHERE EMP_BRA_Code = ?Bra_Code? AND EMP_CMP_Code = 'MET' AND EMP_RetireDate is null ";
            using (IDbConnection db = new OleDbConnection(myOraDbConn))
            {
                return db.Query<String>(SQLSTR, new { BRA_Code = Bra_Code }).FirstOrDefault();
            }
        }
        #endregion

        #region Documents

        public List<SysQuestions> GetSysQuestions(String SQ_SDOC_Code)
        {
            String SQLSTR = "";

            SQLSTR = "SELECT * " +
                        "FROM SysQuestions " +
                    "WHERE SysQuestions.SQ_SDOC_Code = ?SQ_SDOC_Code? ";

            using (IDbConnection db = new OleDbConnection(myOraDbConn))
            {
                return db.Query<SysQuestions>(SQLSTR, new { SQ_SDOC_Code = SQ_SDOC_Code }).ToList();
            }
        }

        public Document GetDocument(String CDOC_ID)
        {
            String SQLSTR = "";

            SQLSTR = "SELECT * " +
                        "FROM Document " +
                    "WHERE Document.CDOC_ID = ?CDOC_ID? ";

            using (IDbConnection db = new OleDbConnection(myOraDbConn))
            {
                return db.Query<Document>(SQLSTR, new { CDOC_ID = CDOC_ID }).FirstOrDefault();
            }
        }


        public List<Document> GetDocuments(string CDOC_BATCHNO)
        {
            String SQLSTR = "";

            SQLSTR = "SELECT * " +
                        "FROM Document " +
                    "WHERE CDOC_BATCHNO = ?CDOC_BATCHNO? ";

            using (IDbConnection db = new OleDbConnection(myOraDbConn))
            {
                return db.Query<Document>(SQLSTR, new { CDOC_BATCHNO = CDOC_BATCHNO }).ToList();
            }
        }
  
        public bool InsertDocument(Document Doc)
        {
            String SQLSTR = "";

            SQLSTR = "INSERT INTO Document (CDOC_CSDOC_CODE, CDOC_ID, CDOC_CREATEDDATE, CDOC_UPDATEDATE,  CDOC_STATUS, CDOC_BATCHNO, CDOC_CENT_CODE, CDOC_CENT_TYPE)  ON EXISTING UPDATE  VALUES" +
                "(?CDOC_CSDOC_CODE?, ?CDOC_ID?, ?CDOC_CREATEDDATE?, ?CDOC_UPDATEDATE?,  ?CDOC_STATUS? , ?CDOC_BATCHNO?, ?CDOC_CENT_CODE?, ?CDOC_CENT_TYPE?)";

            using (IDbConnection db = new OleDbConnection(myOraDbConn))
            {
                db.Execute(SQLSTR, new
                {
                    CDOC_CSDOC_CODE = Doc.CDOC_CSDOC_CODE,
                    CDOC_ID = Doc.CDOC_ID,
                    CDOC_CREATEDDATE = Doc.CDOC_CREATEDDATE,
                    CDOC_UPDATEDATE = Doc.CDOC_UPDATEDATE,
                    CDOC_STATUS = Doc.CDOC_STATUS,
                    CDOC_BATCHNO = Doc.CDOC_BATCHNO,
                    CDOC_CENT_CODE = Doc.CDOC_CENT_CODE,
                    CDOC_CENT_TYPE = Doc.CDOC_CENT_TYPE
                });

            }

            return true;
        }

        public bool UpdateDocument(Document Doc)
        {
            String SQLSTR = "";

            SQLSTR = "UPDATE Document SET " +
                        "Document.CDOC_UPDATEDATE = ?CDOC_UPDATEDATE?, " +
                        "Document.CDOC_STATUS = ?CDOC_STATUS?," +
                        "Document.CDOC_BATCHNO = ?CDOC_BATCHNO?, " +
                        "Document.CDOC_CENT_TYPE = ?CDOC_CENT_TYPE?," +
                        "Document.CDOC_CENT_CODE = ?CDOC_CENT_CODE? " +
                        "WHERE Document.CDOC_ID = ?CDOC_ID? ";

            using (IDbConnection db = new OleDbConnection(myOraDbConn))
            {
                db.Execute(SQLSTR, new
                {
                    CDOC_CSDOC_CODE = Doc.CDOC_CSDOC_CODE,
                    CDOC_ID = Doc.CDOC_ID,
                    CDOC_CREATEDDATE = Doc.CDOC_CREATEDDATE,
                    CDOC_UPDATEDATE = Doc.CDOC_UPDATEDATE,
                    CDOC_STATUS = Doc.CDOC_STATUS,
                    CDOC_BATCHNO = Doc.CDOC_BATCHNO,
                    CDOC_CENT_TYPE = Doc.CDOC_CENT_TYPE,
                    CDOC_CENT_CODE = Doc.CDOC_CENT_CODE
                });
            }
            return true;

        }

        #endregion

        #region DocumentDetails

        public IEnumerable<DocumentDetails> GetDocumentsDetails(String CDDT_CDOC_ID)
        {
            String SQLSTR = "";

            SQLSTR = " SELECT * "
                + "FROM DocumentDetails" +
                " WHERE DocumentDetails.CDDT_CDOC_ID = ?CDDT_CDOC_ID?";

            using (IDbConnection db = new OleDbConnection(myOraDbConn))
            {
                return db.Query<DocumentDetails>(SQLSTR, new { CDDT_CDOC_ID = CDDT_CDOC_ID }).ToList();
            }
        }

        public bool InsertDocumentsDetails(DocumentDetails DOC)
        {
            String SQLSTR = "";

            SQLSTR = "INSERT INTO DocumentDetails(CDDT_CSDDT_CODE, CDDT_CDOC_ID, CDDT_QUESTION, CDDT_VALUE , CDDT_PixelsA, CDDT_PixelsB, CDDT_PixelsC, CDDT_PixelsD, CDDT_PixelsE) ON EXISTING UPDATE  VALUES " +
                "(?CDDT_CSDDT_CODE?, ?CDDT_CDOC_ID?, ?CDDT_QUESTION?, ?CDDT_VALUE?, ?CDDT_PixelsA?, ?CDDT_PixelsB?, ?CDDT_PixelsC?, ?CDDT_PixelsD?, ?CDDT_PixelsE?)";

            using (IDbConnection db = new OleDbConnection(myOraDbConn))
            {
                db.Execute(SQLSTR, new
                {
                    CDDT_CSDDT_CODE = DOC.CDDT_CSDDT_CODE,
                    CDDT_CDOC_ID = DOC.CDDT_CDOC_ID,
                    CDDT_QUESTION = DOC.CDDT_QUESTION,
                    CDDT_VALUE = DOC.CDDT_VALUE,
                    CDDT_PixelsA = DOC.CDDT_PixelsA,
                    CDDT_PixelsB = DOC.CDDT_PixelsB,
                    CDDT_PixelsC = DOC.CDDT_PixelsC,
                    CDDT_PixelsD = DOC.CDDT_PixelsD,
                    CDDT_PixelsE = DOC.CDDT_PixelsE
                });
            }
            return true;
        }

        #endregion

        #region Entity

        public Entity GetEntity(String CENT_CODE)
        {
            String SQLSTR = "";

            SQLSTR = " SELECT * FROM ENTITY WHERE ENTITY.CENT_CODE = ?CENT_CODE? ";

            using (IDbConnection db = new OleDbConnection(myOraDbConn))
            {
                return db.Query<Entity>(SQLSTR, new { CENT_CODE = CENT_CODE }).FirstOrDefault();
            }
        }

        public bool InsertEntity(Entity ENT)
        {
            String SQLSTR = "";

            SQLSTR = "INSERT INTO ENTITY(CENT_CODE, CENT_NAME, CENT_TYPE) VALUES " +
                "(?CENT_CODE?, ?CENT_NAME?, ?CENT_TYPE?)";

            using (IDbConnection db = new OleDbConnection(myOraDbConn))
            {
                db.Execute(SQLSTR, new { CENT_CODE = ENT.CENT_CODE, CENT_NAME = ENT.CENT_NAME, CENT_TYPE = ENT.CENT_TYPE });
            }

            return true;
        }

        public bool UpdateEntity(Entity ENT)
        {
            String SQLSTR = "";

            SQLSTR = "UPDATE ENTITY SET " +
                "ENTITY.CENT_NAME = ?CENT_NAME?, " +
                "ENTITY.CENT_TYPE = ?CENT_TYPE? " +
                "WHERE ENTITY.CENT_CODE = ?CENT_CODE?";

            using (IDbConnection db = new OleDbConnection(myOraDbConn))
            {
                db.Execute(SQLSTR, new { CENT_CODE = ENT.CENT_CODE, CENT_NAME = ENT.CENT_NAME, CENT_TYPE = ENT.CENT_TYPE });
            }

            return true;
        }
        #endregion

        #region SysDocument
        public SysDocuments GetSysDocuments(String CSDOC_CODE)
        {
            String SQLSTR = "";

            SQLSTR = "SELECT * FROM SysDocuments WHERE SysDocuments.CSDOC_CODE = ?CSDOC_CODE? ";

            using (IDbConnection db = new OleDbConnection(myOraDbConn))
            {
                return db.Query<SysDocuments>(SQLSTR, new { CSDOC_CODE = CSDOC_CODE }).FirstOrDefault();
            }
        }

        public SysDocuments GetActiveSysDocuments()
        {
            String SQLSTR = "";

            SQLSTR = "SELECT * FROM SysDocuments WHERE CSDOC_ACTIVE = 1";

            using (IDbConnection db = new OleDbConnection(myOraDbConn))
            {
                return db.Query<SysDocuments>(SQLSTR).FirstOrDefault();
            }
        }


        #endregion

        public string GetMaxBatchNo()
        {
            string SQLSTR = "";

            SQLSTR = "SELECT Isnull(Max(CDOC_BATCHNO),'000001') " +
                        "FROM Document ";

            using (IDbConnection db = new OleDbConnection(myOraDbConn))
            {
                return db.Query<string>(SQLSTR).FirstOrDefault();
            }
        }

        public int GetNumOfAnswers(int AnswNum)
        {


            switch (AnswNum)
            {
                case 54:
                    return 2;
                    break;
                case 57:
                    return 3;
                    break;
                case 58:
                    return 2;
                    break;
                case 53:
                    return 0;
                    break;
                default:
                    return 5;
                    break;
            }


        }
    }
}