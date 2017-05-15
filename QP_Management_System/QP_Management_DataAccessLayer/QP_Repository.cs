using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        #region

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
            catch (Exception e)
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


        public List<QPMasterPool> GetDocumentsReviewer(string reviewer)
        {
            List<QPMasterPool> doc = null;
            try
            {
                doc = (from d in Context.QPMasterPools where d.Reviewer == reviewer select d).ToList<QPMasterPool>();
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
                oldDoc.DocumentName = doc.DocumentName;
                oldDoc.UpdationLog = doc.UpdationLog;
                oldDoc.Status = doc.Status;
                Context.SaveChanges();
                bool status2 = this.UpdateDocumentVersion(doc);
                if(status2)
                {
                    status = true;
                }
                else
                {
                    status = false;
                }
            }
            catch (Exception e)
            {
                status = false;
            }
            return status;
        }

        public bool UpdateDocumentVersion(QPMasterPool doc)
        {
            QPVersion version = new QPVersion();
            string versionId;
            bool status = false;
            try
            {
                var oldDoc = (from d in Context.QPMasterPools where d.QPDocId == doc.QPDocId select d).FirstOrDefault<QPMasterPool>();
                var ver = (from v in Context.QPVersions where v.DocId == oldDoc.QPDocId select v.VersionId).ToList();
                version.Comments = oldDoc.Comments;
                version.CreationLog = oldDoc.CreationLog;
                version.DocId = oldDoc.QPDocId;
                version.Document = oldDoc.Document;
                version.DocumentName = oldDoc.DocumentName;
                version.UpdationLog = oldDoc.UpdationLog;
                //-------------------------------------
                if(ver.Count==0)
                {
                    versionId = Regex.Match(oldDoc.QPDocId, @"\d+").Value;
                    versionId += ".1";
                    version.VersionId = versionId;
                }
                else
                {
                    int length=0;
                    foreach (var item in ver)
                    {
                        if(item.Length>length)
                        {
                            length = item.Length;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    ver.RemoveAll(x => x.Length < length);
                    string vers = ver.Max();
                    string[] parts = vers.Split('.');
                    int i1 = int.Parse(parts[0]);
                    int i2 = int.Parse(parts[1]);
                    i2 += 1;
                    vers = i1.ToString()+"." + i2.ToString();
                    versionId = vers;
                    version.VersionId = versionId;
                }
                //----------------------------------------
                Context.QPVersions.Add(version);
                Context.SaveChanges();
                status = true;
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        // raise a new exception nesting
                        // the current instance as InnerException
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;
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

        public List<QPMasterPool> GetAssignedDocuments()
        {
            List<QPMasterPool> assigneddoc = new List<QPMasterPool>();
            try
            {
                assigneddoc = (from ad in Context.QPMasterPools select ad).ToList<QPMasterPool>();
            }
            catch (Exception)
            {
                assigneddoc = null;
            }
            return assigneddoc;
        }

        public List<QPMasterPool> QPAnchorDownload()
        {
            List<QPMasterPool> doc = new List<QPMasterPool>();
            try
            {
                doc = (from d in Context.QPMasterPools where d.Status == "F" select d).ToList<QPMasterPool>();
            }
            catch (Exception)
            {
                doc = null;
            }
            return doc;
        }

        public List<QPMasterPool> QPAnchorSelect()
        {
            List<QPMasterPool> doc = new List<QPMasterPool>();
            try
            {
                doc = (from d in Context.QPMasterPools where d.Status == "F" select d).ToList<QPMasterPool>();
            }
            catch (Exception)
            {
                doc = null;
            }
            return doc;
        }


        public bool QPAnchorSelectDoc(string qpDocId)
        {
            QPMasterPool doc = new QPMasterPool();
            bool status = false;
            try
            {
                doc = (from d in Context.QPMasterPools where d.QPDocId==qpDocId select d).FirstOrDefault();
                Context.QPMasterPools.Remove(doc);
                Context.SaveChanges();
                status = true;
            }
            catch (Exception)
            {
                status = false;
            }
            return status;
        }

        public bool ReviewerAccept(QPMasterPool qpObj)
        {
            bool status = false;
            QPMasterPool doc = new QPMasterPool();
            try
            {
                doc = (from d in Context.QPMasterPools where d.QPDocId == qpObj.QPDocId select d).FirstOrDefault();
                doc.Status = qpObj.Status;
                doc.UpdationLog = qpObj.UpdationLog;
                Context.SaveChanges();
                status = true;
            }
            catch (Exception)
            {
                status = false;
            }
            return status;
        }

        public bool QPReject(QPMasterPool qpObj)
        {
            bool status = false;
            QPMasterPool doc = new QPMasterPool();
            try
            {
                doc = (from d in Context.QPMasterPools where d.QPDocId == qpObj.QPDocId select d).FirstOrDefault();
                doc.Status = qpObj.Status;
                doc.UpdationLog = qpObj.UpdationLog;
                Context.SaveChanges();
                status = true;
            }
            catch (Exception)
            {
                status = false;
            }
            return status;
        }

        public bool QualityAnchorAccept(QPMasterPool qpObj)
        {
            bool status = false;
            QPMasterPool doc = new QPMasterPool();
            try
            {
                doc = (from d in Context.QPMasterPools where d.QPDocId == qpObj.QPDocId select d).FirstOrDefault();
                doc.Status = qpObj.Status;
                doc.UpdationLog = qpObj.UpdationLog;
                Context.SaveChanges();
                status = true;
            }
            catch (Exception)
            {
                status = false;
            }
            return status;
        }

        public User GetUserDetails(string userId)
        {
            User userDetails = new User();
            try
            {
                userDetails = (from u in Context.Users where u.UserName == userId select u).FirstOrDefault();
            }
            catch (Exception)
            {
                userDetails = null;
            }
            return userDetails;
        }

        public string GetTrackName(int trackId)
        {
            string trackName;
            try
            {
                trackName = (from t in Context.Tracks where t.TrackId == trackId select t.TrackName).FirstOrDefault();
            }
            catch (Exception)
            {
                trackName = null;
            }
            return trackName;
        }


        public string GetQPAnchor(int trackId)
        {
            string qpAnchor;
            try
            {
                qpAnchor = (from q in Context.Users where q.TrackId == trackId && q.RoleId == 4 select q.UserName).FirstOrDefault();
            }
            catch (Exception)
            {
                qpAnchor = null;
            }
            return qpAnchor;
        }

        public int GetDocumentsForProfile(string userName)
        {
            List<QPMasterPool> docs = new List<QPMasterPool>();
            try
            {
                docs = (from d in Context.QPMasterPools where d.Author == userName || d.Reviewer == userName || d.QualityAnchor == userName select d).ToList();
            }
            catch (Exception)
            {
                docs = null;
            }
            return docs.Count;
        }

        public string GetModuleName(int moduleId)
        {
            string moduleName;
            try
            {
                moduleName = (from m in Context.Modules where m.ModuleId == moduleId select m.ModuleName).FirstOrDefault();
            }
            catch (Exception)
            {
                moduleName = null;
            }
            return moduleName;
        }

        public List<QPVersion> GetVersions(string qpDocId)
        {
            List<QPVersion> versions = new List<QPVersion>();
            try
            {
                versions = (from v in Context.QPVersions where v.DocId == qpDocId orderby v.UpdationLog select v).ToList();
            }
            catch (Exception)
            {
                versions = null;
            }
            return versions;
        }

        public QPVersion VersionDetails(string versionId)
        {
            QPVersion ver = new QPVersion();
            try
            {
                ver = (from v in Context.QPVersions where v.VersionId == versionId select v ).FirstOrDefault();
            }
            catch (Exception)
            {
                ver = null;
            }
            return ver;
        }
        #endregion


    }
}
