using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QP_Management_DataAccessLayer
{
    public class QP_Repository
    {
        public QP_ManagementDBContext Context { get; set; }

        public QP_Repository()
        {
            Context = new QP_ManagementDBContext();
        }

        public string CheckLogin(User usr)
        {
            string status = null;
            try
            {
                var userDetails = (from u in Context.Users where u.UserName == usr.UserName select u).FirstOrDefault<User>();
                if(userDetails!=null)
                {
                    if(userDetails.UserPassword==usr.UserPassword)
                    {
                        if(userDetails.RoleId==1)
                        {
                            status = "author";
                        }
                        else if(userDetails.RoleId==2)
                        {
                            status = "reviewer";
                        }
                        else if(userDetails.RoleId==3)
                        {
                            status = "quality anchor"; 
                        }
                        else if(userDetails.RoleId==4)
                        {
                            status = "qp anchor";
                        }
                    }
                    else
                    {
                        status = "wrong";
                    }
                }
                else
                {
                    status = "not exists";
                }
                
            }
            catch (Exception)
            {

                status = null;
            }
            return status;
        }

        public bool AddDocument(QPMasterPool qpObj)
        {
            bool status = false;
            try
            {
                Context.QPMasterPools.Add(qpObj);
                Context.SaveChanges();
                status = true;
            }
            catch (Exception)
            {
                status = false;
                
            }
            return status;
        }

        public List<QPMasterPool> GetDocumentsAuthor(string author)
        {
            List<QPMasterPool> doc = null;
        
            try
            {
                
                
                    doc = (from d in Context.QPMasterPools where d.Author == author select d).ToList<QPMasterPool>();
                
              
            }
            catch (Exception)
            {

                doc = null;
            }
            return doc;
        }

        public List<QPMasterPool> GetDocumentsQualityAnchor(string qualityanchor)
        {
            List<QPMasterPool> doc = null;
            try
            {
                doc = (from d in Context.QPMasterPools where d.QualityAnchor == qualityanchor select d).ToList<QPMasterPool>();
            }
            catch (Exception)
            {

                doc = null;
            }
            return doc;
        }

        //public byte[] DownloadDocument(string QPDocId)
        //{
        //    byte[] doc = null;
        //    try
        //    {
        //        doc = (from d in Context.QPMasterPools where d.QPDocId == QPDocId select d.Document).FirstOrDefault();
        //    }
        //    catch (Exception)
        //    {

        //        doc = null;
        //    }
        //    return doc;
        //}

        public QPMasterPool DocumentDetails(string qpDocId)
        {
            QPMasterPool doc = null;
            try
            {
                doc = (from d in Context.QPMasterPools where d.QPDocId == qpDocId select d).FirstOrDefault();
            }
            catch (Exception)
            {

                doc = null;
            }
            return doc;
        }

        public bool UpdateDocumentMaster(QPMasterPool doc)
        {
            bool status = false;
            try
            {
                var oldDoc = (from d in Context.QPMasterPools where d.QPDocId == doc.QPDocId select d).FirstOrDefault<QPMasterPool>();
                oldDoc.Document = doc.Document;
                oldDoc.Comments = doc.Comments;
                oldDoc.UpdationLog = doc.UpdationLog;
                oldDoc.Status = doc.Status;
                Context.SaveChanges();
                status = true;
            }
            catch (Exception)
            {
                status = false;
            }
            return status;
        }

        public string GetDocName(int trackId, int faId, int moduleId)
        {
            string docName = null;
            try
            {
                var trackName = (from t in Context.Tracks where t.TrackId == trackId select t.TrackName).FirstOrDefault();
                var focusAreaName = (from f in Context.FocusAreas where f.FAId == faId select f.FAName).FirstOrDefault();
                var moduleName = (from m in Context.Modules where m.ModuleId == moduleId select m.ModuleName).FirstOrDefault();
                docName = trackName+"-" + focusAreaName+ "-" + moduleName;
            }
            catch (Exception)
            {

                docName = null;
            }
            return docName;
        }

        public string GetDocId()
        {
            string docId = null;
            try
            {
                docId = Context.Database.SqlQuery<string>("select dbo.ufn_GenerateDocId()").FirstOrDefault();
            }
            catch (Exception)
            {

                docId = null;
            }
            return docId;
        }
    }
}
