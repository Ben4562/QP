using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QP_Management_System.Repository;
using AutoMapper;
using QP_Management_DataAccessLayer;

namespace QP_Management_System.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            return View(); 
        }

        // GET: User/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: User/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: User/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: User/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: User/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: User/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PostLogin(Models.Users user)
        {
            QPMapper<Models.Users, User> mapObj = new QPMapper<Models.Users, QP_Management_DataAccessLayer.User>(); 
            var dal = new QP_Repository();
            string status = null;
            status = dal.CheckLogin(mapObj.Translate(user));
            if(status!=null)
            {
                Session["UserName"] = user.UserName;
                if(status=="author")
                {
                    Session["Role"] = status;
                    return RedirectToAction("Author");
                }
                else if(status=="reviewer")
                {
                    return RedirectToAction("Reviewer");
                }
                else if(status=="quality anchor")
                {
                    return View();
                }
                else
                {
                    return View("Error"); 
                }
            }
            else if(status=="wrong")
            {
                return View("WrongPassword");
            }
            else
            {
                return View("Error");
            }
            
        }

        public ActionResult Author()
        {
            if(Session["UserName"]==null)
            {
                return RedirectToAction("Login");
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

        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(Models.QPMasterPool qpObj,HttpPostedFileBase uploadFile)
        {
            qpObj.CreationLog = DateTime.Now;
            qpObj.Author = Session["UserName"].ToString();
            var dal = new QP_Repository();
            QPMapper<Models.QPMasterPool, QPMasterPool> mapObj = new QPMapper<Models.QPMasterPool, QPMasterPool>();
            try
            {
                if(uploadFile!= null)
                {
                    qpObj.Document = new byte[uploadFile.ContentLength];
                    uploadFile.InputStream.Read(qpObj.Document, 0, uploadFile.ContentLength);
                    bool status = dal.AddDocument(mapObj.Translate(qpObj));
                    if(status)
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
                    return View();
                }
            }
            catch (Exception)
            {
                return View("Error"); 
            }
        }

        public ActionResult Download()
        {
            QPMapper<QPMasterPool, Models.QPMasterPool> mapObj = new QPMapper<QPMasterPool, Models.QPMasterPool>();
            var dal = new QP_Repository();
            var doc = dal.GetDocuments(Session["UserName"].ToString());
            List<Models.QPMasterPool> documentList = new List<Models.QPMasterPool>();
            if(doc.Any())
            {
                foreach (var item in doc)
                {
                    documentList.Add(mapObj.Translate(item));
                }
            }
            return View(documentList);
        }

        public ActionResult DownloadDoc(string QPDocId)
        {
            var dal = new QP_Repository();
            var docDetails = dal.DocumentDetails(QPDocId);
            return File(docDetails.Document,System.Net.Mime.MediaTypeNames.Application.Rtf,docDetails.DocumentName+".docx");  
        }

    }
}
