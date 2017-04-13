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
                    return RedirectToAction("");
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
                QPMapper<Models.Users, User> mapObj = new QPMapper<Models.Users, QP_Management_DataAccessLayer.User>();
                var dal = new QP_Repository();
                string status = null;
                status = dal.CheckLogin(mapObj.Translate(user));
                if (status != null)
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
                        return View("WrongPassword");
                    }
                }
                else
                {
                    return View("Error");
                }
            }
            else
            {
                return View();
            }
        }

        public ActionResult LogOut()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        #endregion


        //Author Page
        public ActionResult Author()
        {
            if(Session["UserName"]==null || Session["Role"].ToString().ToLower()!= "author" )
            {
                return RedirectToAction("Login");
            }
            else
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

        }


        //QP-Anchor
        public ActionResult QPAnchor()
        {
            if(Session["UserName"]==null || Session["Role"].ToString().ToLower()!= "qp anchor")
            {
                return RedirectToAction("Login");
            }
            else
            {
                return View();
            }
        }

        
        //Quality Anchor
        public ActionResult QualityAnchor()
        {
            if (Session["UserName"] == null )
            {
                return RedirectToAction("Login");
            }
            else
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
        }

        //Reviewer
        #region
        public ActionResult Reviewer()
        {
            if (Session["UserName"] == null || Session["Role"].ToString().ToLower() != "reviewer")
            {
                return RedirectToAction("Login");
            }
            else
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
        }
        #endregion

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


        //Download-Method
        public ActionResult DownloadDoc(string qpDocId)
        {
            var dal = new QP_Repository();
            var docDetails = dal.DocumentDetails(qpDocId);
            return File(docDetails.Document,System.Net.Mime.MediaTypeNames.Application.Octet,docDetails.DocumentName+".Docx");  
        }

        //Upload-Method 
        #region
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

                        return View("Success");
                        
                    }
                    else
                    {
                        return View("Error");
                    }
                }
                else
                {
                    return View("Error");
                }
            }
            catch (Exception)
            {
                return View("Error");
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
            obj.DocumentName = dal.GetDocName(trackId,focusAreaId,moduleId);
            obj.ModuleId = moduleId;
            obj.QPDocId = dal.GetDocId();
            obj.QualityAnchor = frm["QualityAnchor"];
            obj.Reviewer = frm["Reviewer"];
            obj.Status = "A";
            obj.UpdationLog = null;
            bool status = dal.AddDocument(mapObj.Translate(obj));
            if(status)
            {
                return View("Success");
            }
            else
            {
                return View("Error");
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
        public ActionResult Editor()
        {
            return View(); 
        }

        [HttpPost]
        [ValidateInput(false)]
        public FileResult Editor(Models.Editor doc)
        {
            return File(HtmlToWord(doc.HtmlContent), "application/vnd.openxmlformats-officedocument.wordprocessingml.document"); 
        }

        #endregion

        //Misc
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

        //Method to convert
        #region
        public static byte[] HtmlToWord(String html)
        {
            //const string filename = "test.docx";
            // string html = Properties.Resources.DemoHtml;

            //if (File.Exists(filename)) File.Delete(filename);

            using (MemoryStream generatedDocument = new MemoryStream())
            {
                using (WordprocessingDocument package = WordprocessingDocument.Create(generatedDocument, WordprocessingDocumentType.Document) )
                {
                    MainDocumentPart mainPart = package.MainDocumentPart;
                    if (mainPart == null)
                    {
                        mainPart = package.AddMainDocumentPart();
                        new Document(new Body()).Save(mainPart);
                    }

                    HtmlConverter converter = new HtmlConverter(mainPart);
                    Body body = mainPart.Document.Body;

                    var paragraphs = converter.Parse(html);
                    for (int i = 0; i < paragraphs.Count; i++)
                    {
                        body.Append(paragraphs[i]);
                    }

                    mainPart.Document.Save();
                }

                return generatedDocument.ToArray();
            }
        }
        #endregion


    }
}
