using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QP_Management_System.Repository;
using QP_Management_DataAccessLayer;
using NotesFor.HtmlToOpenXml;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.CustomProperties;
using Aspose.Words;
using OpenXmlPowerTools;
using SautinSoft;
using GemBox.Document;

namespace QP_Management_System.Controllers
{
    public class UserController : Controller
    {
        public ActionResult Index()
        {
            return View(); 
        }

        //Login Page
        #region
        public ActionResult Login()
        {
            if(Session["UserName"]==null)
            {
                return View();
            }
            else
            {
                if(Session["Role"].ToString().ToLower()=="author")
                {
                    return RedirectToAction("Author");
                }
                else if(Session["Role"].ToString().ToLower() == "qp anchor")
                {
                    return RedirectToAction("QPAnchor");
                }
                else if(Session["Role"].ToString().ToLower() == "reviewer")
                {
                    return RedirectToAction("Reviewer");
                }
                else if(Session["Role"].ToString().ToLower() == "quality anchor")
                {
                    return RedirectToAction("QualityAnchor");
                }
                else
                {
                    return View(); 
                } 
            }
        }
        [HttpPost]
        public ActionResult PostLogin(Models.Users user)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    QPMapper<Models.Users, User> mapObj = new QPMapper<Models.Users, QP_Management_DataAccessLayer.User>();
                    var dal = new QP_Repository();
                    string status = null;
                    status = dal.CheckLogin(mapObj.Translate(user));
                    if (status != null)
                    {
                        if (status == "not exists")
                        {
                            Session["user"] = "Couldn't find your account";
                            return View("Login"); 
                        }
                        else
                        {
                            Session["UserName"] = user.UserName;
                            Session["Role"] = status;
                            if (status == "author")
                            {
                                return RedirectToAction("Author");
                            }
                            else if (status == "reviewer")
                            {
                                return RedirectToAction("Reviewer");
                            }
                            else if (status == "quality anchor")
                            {
                                return RedirectToAction("QualityAnchor");
                            }
                            else if (status == "qp anchor")
                            {
                                return RedirectToAction("QPAnchor");
                            }
                            else
                            {
                                Session["password"] = "Wrong Password Try again";
                                return View("Login");
                            }
                        }
                    }
                    else
                    {

                        return View("UnableToLogin");
                    }
                }
                catch (Exception)
                {
                    return View("UnableToLogin");  
                }
            }
            else
            {
                return View();
            }
        }

        public ActionResult LogOut()
        {
            try
            {
                Session.Clear();
                return RedirectToAction("Login"); 
            }
            catch (Exception)
            {
                return View("UnableToLogout");
            }
        }

        #endregion

        //Roles
        #region

        //Author Page
        public ActionResult Author()
        {
            if(Session["UserName"]==null || Session["Role"].ToString().ToLower()!= "author" )
            {
                return View("Authorization");
            }
            else
            {
                try
                {
                    QPMapper<QPMasterPool, Models.QPMasterPool> mapObj = new QPMapper<QPMasterPool, Models.QPMasterPool>();
                    var dal = new QP_Repository();
                    var doc = dal.GetDocumentsAuthor(Session["UserName"].ToString());
                    List<Models.QPMasterPool> documentList = new List<Models.QPMasterPool>();
                    if (doc.Any())
                    {
                        foreach (var item in doc)
                        {
                            documentList.Add(mapObj.Translate(item));
                        }
                    }
                    return View(documentList);
                }
                catch (Exception)
                {
                    return View("DocumentError"); 
                }
            }

        }

        //Reviewer
        public ActionResult Reviewer()
        {
            if (Session["UserName"] == null || Session["Role"].ToString().ToLower() != "reviewer")
            {
                return View("Authorization");
            }
            else
            {
                try
                {
                    QPMapper<QPMasterPool, Models.QPMasterPool> mapObj = new QPMapper<QPMasterPool, Models.QPMasterPool>();
                    var dal = new QP_Repository();
                    var doc = dal.GetDocumentsReviewer(Session["UserName"].ToString());
                    List<Models.QPMasterPool> documentList = new List<Models.QPMasterPool>();
                    if (doc.Any())
                    {
                        foreach (var item in doc)
                        {
                            documentList.Add(mapObj.Translate(item));
                        }
                    }
                    return View(documentList);
                }
                catch (Exception)
                {
                    return View("DocumentError");
                }
            }
        }

        //Quality Anchor
        public ActionResult QualityAnchor()
        {
            if (Session["UserName"] == null || Session["Role"].ToString().ToLower() != "quality anchor")
            {
                return View("Authorization");
            }
            else
            {
                try
                {
                    QPMapper<QPMasterPool, Models.QPMasterPool> mapObj = new QPMapper<QPMasterPool, Models.QPMasterPool>();
                    var dal = new QP_Repository();
                    var doc = dal.GetDocumentsQualityAnchor(Session["UserName"].ToString());
                    List<Models.QPMasterPool> documentList = new List<Models.QPMasterPool>();
                    if (doc.Any())
                    {
                        foreach (var item in doc)
                        {
                            documentList.Add(mapObj.Translate(item));
                        }
                    }
                    return View(documentList);
                }
                catch (Exception)
                {
                    return View("DocumentError");
                }
            }
        }

        //QP-Anchor
        public ActionResult QPAnchor()
        {
            if(Session["UserName"]==null || Session["Role"].ToString().ToLower()!= "qp anchor")
            {
                return View("Authorization");
            }
            else
            {
                var dal = new QP_Repository();
                var assigneddoc = dal.GetAssignedDocuments();
                List<Models.QPMasterPool> docs = new List<Models.QPMasterPool>();
                foreach (var item in assigneddoc)
                {
                    docs.Add(new Models.QPMasterPool()
                    {
                        Author = item.Author,
                        Comments = item.Comments,
                        CreationLog = item.CreationLog,
                        Document = item.Document,
                        DocumentName = item.DocumentName,
                        ModuleId = item.ModuleId,
                        QPDocId = item.QPDocId,
                        QualityAnchor = item.QualityAnchor,
                        Reviewer = item.Reviewer,
                        Status = item.Status,
                        UpdationLog= item.UpdationLog
                    });
                }
                return View(docs); 
            }
        }

        #endregion

        //Upload-Not Used
        #region
        //public ActionResult Upload()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult Upload(Models.QPMasterPool qpObj,HttpPostedFileBase uploadFile)
        //{
        //    qpObj.CreationLog = DateTime.Now;
        //    qpObj.Author = Session["UserName"].ToString();
        //    var dal = new QP_Repository();
        //    QPMapper<Models.QPMasterPool, QPMasterPool> mapObj = new QPMapper<Models.QPMasterPool, QPMasterPool>();
        //    try
        //    {
        //        if(uploadFile!= null)
        //        {
        //            qpObj.Document = new byte[uploadFile.ContentLength];
        //            uploadFile.InputStream.Read(qpObj.Document, 0, uploadFile.ContentLength);
        //            bool status = dal.AddDocument(mapObj.Translate(qpObj));
        //            if(status)
        //            {
        //                return View("Success");
        //            }
        //            else
        //            {
        //                return View("Error");
        //            }
        //        }
        //        else
        //        {
        //            return View();
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return View("Error"); 
        //    }
        //}  

        #endregion

        //Download/Upload-Method
        #region

        //download
        public ActionResult DownloadDoc(string qpDocId)
        {
            try
            {
                var dal = new QP_Repository();
                var docDetails = dal.DocumentDetails(qpDocId);
                return File(docDetails.Document, System.Net.Mime.MediaTypeNames.Application.Octet, docDetails.DocumentName + ".Docx");
            }
            catch (Exception)
            {
                return View("UnableToDownload"); 
            } 
        }

        //Upload-Method 
        public ActionResult ReUpload(Models.QPMasterPool qpDoc)
        {
            return View(qpDoc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult ReUploadDoc(Models.QPMasterPool qpObj, HttpPostedFileBase reUpload)
        {
            qpObj.UpdationLog = DateTime.Now;
            qpObj.Status = "R";
            qpObj.DocumentName = reUpload.FileName;
            var dal = new QP_Repository();
            QPMapper<Models.QPMasterPool, QPMasterPool> mapObj = new QPMapper<Models.QPMasterPool, QPMasterPool>();
            try
            {
                if (reUpload != null)
                {
                
                    qpObj.Document = new byte[reUpload.ContentLength];
                    reUpload.InputStream.Read(qpObj.Document, 0, reUpload.ContentLength);
                    bool status = dal.UpdateDocumentMaster(mapObj.Translate(qpObj));
                    if (status)
                    {
                        return RedirectToAction("Author");                         
                    }
                    else
                    {
                        return View("UnableToUpload");
                    }
                }
                else
                {
                    return View("Invalid File"); 
                }
            }
            catch (Exception)
            {
                return View("UnableToUpload");
            }
        }
        #endregion

        //Create QP - QP Anchor
        #region
        public ActionResult CreateQP()
        {
            QP_ManagementDBContext db = new QP_ManagementDBContext();
            ViewBag.Track = new SelectList(db.Tracks, "TrackId", "TrackName");
            return View(); 
        }

        [HttpPost]
        public ActionResult CreateQP(FormCollection frm)
        {
            try
            {
                QPMapper<Models.QPMasterPool, QPMasterPool> mapObj = new QPMapper<Models.QPMasterPool, QPMasterPool>();
                int trackId = Convert.ToInt32(frm["Track"]);
                int focusAreaId = Convert.ToInt32(frm["FocusArea"]);
                int moduleId = Convert.ToInt32(frm["Module"]);
                byte[] newdoc = null;
                var dal = new QP_Repository();
                Models.QPMasterPool obj = new Models.QPMasterPool();
                obj.Author = frm["Author"];
                obj.Comments = frm["Comments"];
                obj.CreationLog = DateTime.Now;
                obj.Document = newdoc;
                obj.DocumentName = dal.GetDocName(trackId, focusAreaId, moduleId);
                obj.ModuleId = moduleId;
                obj.QPDocId = dal.GetDocId();
                obj.QualityAnchor = frm["QualityAnchor"];
                obj.Reviewer = frm["Reviewer"];
                obj.Status = "A";
                obj.UpdationLog = null;
                bool status = dal.AddDocument(mapObj.Translate(obj));
                if (status)
                {
                    return RedirectToAction("QPAnchor");
                }
                else
                {
                    return View("UnableToCreateQP"); 
                }
            }
            catch (Exception)
            {
                return View("UnableToCreateQP"); 
            }
        }
        #endregion

        //Methods for cascading dropdown
        #region
        
        public JsonResult GetFocusAreas(int id)
        {
            ViewBag.TrackId = id;
            List<SelectListItem> focusArea = new List<SelectListItem>();
            var focusAreaList = this.GetFocusArea(id);
            var focusAreaData = focusAreaList.Select(m => new SelectListItem() { Text = m.FAName, Value = m.FAId.ToString() });
            return Json(focusAreaData, JsonRequestBehavior.AllowGet); 
        }

        public IList<FocusArea> GetFocusArea(int trackId)
        {
            QP_ManagementDBContext db = new QP_ManagementDBContext();
            return db.FocusAreas.Where(f => f.TrackId == trackId).ToList();
        }

        public JsonResult GetModules(string id)
        {
            ViewBag.FocusAreaId = id;
            List<SelectListItem> modules = new List<SelectListItem>();
            var moduleList = this.GetModule(Convert.ToInt32(id));
            var moduleData = moduleList.Select(m => new SelectListItem() { Text = m.ModuleName, Value = m.ModuleId.ToString() });
            return Json(moduleData, JsonRequestBehavior.AllowGet);
        }

        public IList<Module> GetModule(int focusAreaId)
        {
            QP_ManagementDBContext db = new QP_ManagementDBContext();
            return db.Modules.Where(f => f.FAId == focusAreaId).ToList();
        }

        public JsonResult GetAuthors(int id)
        {
            List<SelectListItem> authors = new List<SelectListItem>();
            var authorList = this.GetAuthor(id);
            var authorData = authorList.Select(a => new SelectListItem() { Text = a.UserName, Value = a.UserName });
            return Json(authorData, JsonRequestBehavior.AllowGet);
        }

        public IList<User> GetAuthor(int trackId)
        {
            QP_ManagementDBContext db = new QP_ManagementDBContext();
            return db.Users.Where(a => a.TrackId == trackId && a.RoleId == 1).ToList();
        }
        public JsonResult GetReviewers(int id)
        {
            List<SelectListItem> reviewers = new List<SelectListItem>();
            var reviewerList = this.GetReviewer(id);
            var reviewerData = reviewerList.Select(a => new SelectListItem() { Text = a.UserName, Value = a.UserName });
            return Json(reviewerData, JsonRequestBehavior.AllowGet);
        }

        public IList<User> GetReviewer(int trackId)
        {
            QP_ManagementDBContext db = new QP_ManagementDBContext();
            return db.Users.Where(a => a.TrackId == trackId && a.RoleId == 2).ToList();
        }

        public JsonResult GetQualityAnchors(int id)
        {
            List<SelectListItem> qualityAnchors = new List<SelectListItem>();
            var qualityAnchorList = this.GetQualityAnchor(id);
            var qualityAnchorData = qualityAnchorList.Select(a => new SelectListItem() { Text = a.UserName, Value = a.UserName });
            return Json(qualityAnchorData, JsonRequestBehavior.AllowGet);
        }

        public IList<User> GetQualityAnchor(int trackId)
        {
            QP_ManagementDBContext db = new QP_ManagementDBContext();
            return db.Users.Where(q => q.TrackId == trackId && q.RoleId == 3).ToList();
        }
        #endregion

        //Editor
        #region

        public ActionResult Editor(string qpDocId)
        {
            var dal = new QP_Repository();
            var mdl = new Models.Editor();
            var docDetails = dal.DocumentDetails(qpDocId);
            //var docxPath = Path.GetTempFileName().Replace(".tmp", ".docx");
            //System.IO.File.WriteAllBytes(docxPath, docDetails.Document);
            var htmlPath = Path.GetTempFileName().Replace(".tmp", ".html");
            mdl.DocId = qpDocId;
            try
            {
                RtfToHtml r = new RtfToHtml();
                r.OpenDocx(docDetails.Document);
                r.ToHtml(htmlPath);
                using (StreamReader sr = new StreamReader(htmlPath))
                {
                    String line = sr.ReadToEnd();
                    mdl.HtmlContent = line;
                    return View(mdl);
                }
            }
            catch (Exception)
            {
                return View(mdl);
            }
            
            
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Editor(Models.Editor doc, FormCollection frm)
        {
            var dal = new QP_Repository();
            QPMasterPool qpObj = new QPMasterPool();
            qpObj = dal.DocumentDetails(doc.DocId);
            qpObj.UpdationLog = DateTime.Now;
            qpObj.Status = "R";
            QPMapper<Models.QPMasterPool, QPMasterPool> mapObj = new QPMapper<Models.QPMasterPool, QPMasterPool>();

            try
            {

                var htmlPath = Path.GetTempFileName().Replace(".tmp", ".htm");
                var docxPath = Path.GetTempFileName().Replace(".tmp", ".docx");
                System.IO.File.WriteAllText(htmlPath, doc.HtmlContent);
                ComponentInfo.SetLicense("FREE-LIMITED-KEY");
                ComponentInfo.FreeLimitReached += (sender, e) => e.FreeLimitReachedAction = FreeLimitReachedAction.ContinueAsTrial;
                DocumentModel.Load(htmlPath).Save(docxPath);


                var docbytes = System.IO.File.ReadAllBytes(docxPath);
                qpObj.Document = docbytes;
                bool status = dal.UpdateDocumentMaster(qpObj);

                if (status)
                {
                    return RedirectToAction("Author");
                }
                else
                {
                    return View("UnableToUpload"); 
                }
            }
            catch(Exception)
            {
                return View("UnableToUpload");
            }

        }

        #endregion

        //Misc
        #region
        public ActionResult ViewerEditor()
        {
            return View();
        }

        public ActionResult ViewDoc(string qpDocId)
        {
            return View(); 
        }

        public ActionResult ContactUs()
        {
            return View();
        }

        public ActionResult AboutUs()
        {
            return View();
        }
        #endregion

        //Method to convert
        #region
        //public static byte[] HtmlToWord(String html)
        //{
        //    try
        //    {
        //        //const string filename = "test.docx";
        //        //string html = Properties.Resources.DemoHtml;
        //        //if (File.Exists(filename)) File.Delete(filename);

        //        using (MemoryStream generatedDocument = new MemoryStream())
        //        {
        //            using (WordprocessingDocument package = WordprocessingDocument.Create(generatedDocument, WordprocessingDocumentType.Document))
        //            {
        //                MainDocumentPart mainPart = package.MainDocumentPart;
        //                if (mainPart == null)
        //                {
        //                    mainPart = package.AddMainDocumentPart();
        //                    new DocumentFormat.OpenXml.Wordprocessing.Document(new DocumentFormat.OpenXml.Wordprocessing.Body()).Save(mainPart);
        //                }

        //                NotesFor.HtmlToOpenXml.HtmlConverter converter = new NotesFor.HtmlToOpenXml.HtmlConverter(mainPart);
        //                DocumentFormat.OpenXml.Wordprocessing.Body body = mainPart.Document.Body;

        //                var paragraphs = converter.Parse(html);
        //                for (int i = 0; i < paragraphs.Count; i++)
        //                {
        //                    body.Append(paragraphs[i]);
        //                }

        //                mainPart.Document.Save();
        //            }

        //            return generatedDocument.ToArray(); 
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        #endregion

        //QP-Anchor Functions
        #region
        public ActionResult QPAnchorDownload()
        {
            try
            {
                QPMapper<QPMasterPool, Models.QPMasterPool> mapObj = new QPMapper<QPMasterPool, Models.QPMasterPool>();
                var dal = new QP_Repository();
                var doc = dal.QPAnchorDownload();
                List<Models.QPMasterPool> downloadDoc = new List<Models.QPMasterPool>();
                if (doc.Any())
                {
                    foreach (var item in doc)
                    {
                        downloadDoc.Add(mapObj.Translate(item));
                    }
                }
                return View(downloadDoc);
            }
            catch (Exception)
            {
                return View("DocumentError");
            } 
        }

        public ActionResult QPAnchorDownloadPost(string qpDocId)
        {
            try
            {
                var dal = new QP_Repository();
                var docDetails = dal.DocumentDetails(qpDocId);
                return File(docDetails.Document, System.Net.Mime.MediaTypeNames.Application.Octet, docDetails.DocumentName + ".Docx"); 
            }
            catch (Exception)
            {
                return View("UnableToDownload"); 
            }
        }

        public ActionResult QPAnchorSelect()
        {
            try
            {
                QPMapper<QPMasterPool, Models.QPMasterPool> mapObj = new QPMapper<QPMasterPool, Models.QPMasterPool>();
                var dal = new QP_Repository();
                var doc = dal.QPAnchorSelect();
                List<Models.QPMasterPool> downloadDoc = new List<Models.QPMasterPool>();
                if (doc.Any())
                {
                    foreach (var item in doc)
                    {
                        downloadDoc.Add(mapObj.Translate(item));
                    }
                }
                return View(downloadDoc);
            }
            catch (Exception)
            {
                return View("DocumentError");
            }
        }

        public ActionResult QPAnchorSelectPost(string qpDocId)
        {
            var dal = new QP_Repository();
            bool status = dal.QPAnchorSelectDoc(qpDocId);
            if(status)
            {
                return RedirectToAction("QPAnchor");
            }
            else
            {
                return View("UnableToSelectQP"); 
            }
        }
        #endregion

        //Reviewer Functions
        #region
        public ActionResult ReviewerAccept(string qpDocId)
        {
            QPMapper<Models.QPMasterPool, QPMasterPool> mapObj = new QPMapper<Models.QPMasterPool, QPMasterPool>();
            var dal = new QP_Repository();
            Models.QPMasterPool doc = new Models.QPMasterPool();
            doc.QPDocId = qpDocId;
            doc.Status = "Q";
            doc.UpdationLog = DateTime.Now;
            bool status = dal.ReviewerAccept(mapObj.Translate(doc));
            if(status)
            {
                return RedirectToAction("Reviewer");
            }
            else
            {
                return View("UnableToReviewerAccept");
            }

        }

        public ActionResult ReviewerReject(string qpDocId)
        {
            QPMapper<Models.QPMasterPool, QPMasterPool> mapObj = new QPMapper<Models.QPMasterPool, QPMasterPool>();
            var dal = new QP_Repository();
            Models.QPMasterPool doc = new Models.QPMasterPool();
            doc.QPDocId = qpDocId;
            doc.Status = "A";
            doc.UpdationLog = DateTime.Now;
            bool status = dal.QPReject(mapObj.Translate(doc));
            if (status)
            {
                return RedirectToAction("Reviewer");
            }
            else
            {
                return View("UnableToReviewerReject");
            }
        }

        public ActionResult ReviewerReview(string qpDocId)
        {
            return View(); 
        }

        #endregion

        //QualityAnchor Functions
        #region
        public ActionResult QualityAnchorAccept(string qpDocId)
        {
            QPMapper<Models.QPMasterPool, QPMasterPool> mapObj = new QPMapper<Models.QPMasterPool, QPMasterPool>();
            var dal = new QP_Repository();
            Models.QPMasterPool doc = new Models.QPMasterPool();
            doc.QPDocId = qpDocId;
            doc.Status = "F";
            doc.UpdationLog = DateTime.Now;
            bool status = dal.QualityAnchorAccept(mapObj.Translate(doc));
            if (status)
            {
                return RedirectToAction("QualityAnchor");
            }
            else
            {
                return View("UnableToAcceptQA"); 
            }
        }
        public ActionResult QualityAnchorReject(string qpDocId)
        {
            QPMapper<Models.QPMasterPool, QPMasterPool> mapObj = new QPMapper<Models.QPMasterPool, QPMasterPool>();
            var dal = new QP_Repository();
            Models.QPMasterPool doc = new Models.QPMasterPool();
            doc.QPDocId = qpDocId;
            doc.Status = "A";
            doc.UpdationLog = DateTime.Now;
            bool status = dal.QPReject(mapObj.Translate(doc));
            if (status)
            {
                return RedirectToAction("QualityAnchor");
            }
            else
            {
                return View("UnableToReviewerReject");
            }
        }

        public ActionResult QualityAnchorReview(string qpDocId)
        {
            return View();
        }

        #endregion

        //Adding comments to documents
        #region
        public ActionResult Aspose()
        {
            return View();
        }

        public ActionResult AsposePost()
        {
            try
            {
                // Open an existing document to add comments to a paragraph.
                Aspose.Words.Document doc = new Aspose.Words.Document("C:\\Vinodh\\EditorOut.docx");
                Node[] nodes = doc.GetChildNodes(NodeType.Paragraph, true).ToArray();

                //E.g this is the Paragraph to which comments will added
                Aspose.Words.Paragraph paragraph = (Aspose.Words.Paragraph)nodes[1];

                Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);


                // Create a Comment.
                Aspose.Words.Comment comment = new Aspose.Words.Comment(doc);
                // Insert some text into the comment.
                Aspose.Words.Paragraph commentParagraph = new Aspose.Words.Paragraph(doc);
                commentParagraph.AppendChild(new Aspose.Words.Run(doc, "This is comment after!!!") ); 
                comment.AppendChild(commentParagraph);


                //Move to paragraph where comments will be added
                builder.MoveTo(paragraph);
                // Insert comment
                builder.InsertNode(comment);

                // Save output document.
                doc.Save("C:\\Vinodh\\EditorOut1.docx");
                return View("Success");
            }
            catch (Exception)
            {
                return View("Error"); 
            }

        }

        #endregion

        //View_Profile Functions
        #region


        public ActionResult ViewProfile(Models.Users userModel)
        {
            if(Session["UserName"]==null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                var dal = new QP_Repository();
                QPMapper<User, Models.Users> mapObj = new QPMapper<User, Models.Users>();
                var user = dal.GetUserDetails(Session["UserName"].ToString());
                userModel = mapObj.Translate(user);
                ViewBag.TrackName = dal.GetTrackName(Convert.ToInt32(userModel.TrackId));
                ViewBag.QPAnchor = dal.GetQPAnchor(Convert.ToInt32(userModel.TrackId));
                ViewBag.Count = dal.GetDocumentsForProfile(Session["UserName"].ToString());
                return View(userModel);
            }
            
        }
        


        #endregion

    }
}
