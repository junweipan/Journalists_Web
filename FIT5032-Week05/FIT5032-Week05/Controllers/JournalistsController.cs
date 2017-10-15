using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FIT5032_Week05.Models;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace FIT5032_Week05.Controllers
{
    public class JournalistsController : Controller
    {
        private Week05Entities db = new Week05Entities();
        //register get
        public ActionResult Register()
        {
            return View();
        }
        //Upload image
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            String userEmail = Session["userEmail"].ToString();
            Journalist journalist = db.Journalists.Where(j => j.Email == userEmail).FirstOrDefault();
            var path = "";
            String trueName = Path.GetFileName(file.FileName);//image.png
            if (file != null)
            {
                if (file.ContentLength>0)
                {
                    //check image format
                    if (Path.GetExtension(file.FileName).ToLower().Equals(".jpg")
                        || Path.GetExtension(file.FileName).ToLower().Equals(".gif")
                        || Path.GetExtension(file.FileName).ToLower().Equals(".png")
                        || Path.GetExtension(file.FileName).ToLower().Equals(".jpeg"))
                    {
                        path = Path.Combine(Server.MapPath("~/Content/Images"), file.FileName);
                        file.SaveAs(path);
                        ViewBag.Image = trueName;
                        ViewBag.Message = "File upload seccess";
                        //TODO
                        return View("Details",journalist);
                    }
                }
            }
            ViewBag.Message = "File upload fail";
            return View("Details", journalist);
        }
        //register post
        [HttpPost]
        public ActionResult Register(Journalist journalist)
        {
            if (ModelState.IsValid)
            {
                db.Journalists.Add(journalist);
                db.SaveChanges();
                ModelState.Clear();
                ViewBag.Message = journalist.JName + "Register successfully";
                return RedirectToAction("Index");
            }
            
            return View();
        }
        // GET: Journalists
        public ActionResult Index()
        {
            return View(db.Journalists.ToList());
        }

        // GET: Journalists/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Journalist journalist = db.Journalists.Find(id);
            if (journalist == null)
            {
                return HttpNotFound();
            }
            return View(journalist);
        }
        //log in get
        public ActionResult Login()
        {
            return View();
        }
        //lot in post
        [HttpPost]
        public ActionResult Login(Journalist journalist)
        {
            var user = db.Journalists.Where(j => j.Email == journalist.Email && j.Password == journalist.Password).FirstOrDefault();
            if (user != null)
            {
                Session["userEmail"] = journalist.Email.ToString();
            
                return RedirectToAction("LoggedIn");
            }
            else
            {
                ModelState.AddModelError("","User or password is wrong.");
            }
            return View();
        }
        public ActionResult LoggedIn()
        {
            if (Session["userEmail"] != null)  
            {
                String userEmail = Session["userEmail"].ToString();
                Journalist journalist = db.Journalists.Where(j => j.Email == userEmail).FirstOrDefault();
                ViewBag.LogInMessage = Session["userEmail"] + " Log in successfully";
                return View(journalist);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        // GET: Journalists/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Journalists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "JID,JName,DOB,Address,Phone,Email,Password")] Journalist journalist)
        {
            if (ModelState.IsValid)
            {
                db.Journalists.Add(journalist);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(journalist);
        }

        // GET: Journalists/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Journalist journalist = db.Journalists.Find(id);
            if (journalist == null)
            {
                return HttpNotFound();
            }
            return View(journalist);
        }
        //GET: send individual email to a journalist
        public ActionResult Email(int? id)
        {
            Journalist journalist = db.Journalists.Find(id);
            String toEmail = journalist.Email;
            ViewBag.ToEmail = toEmail;
            return View();
        }
        //Get send all journalists an eamil
        //Emails
        public ActionResult Emails()
        {
            return View();
        }
        //email test method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Email(EmailForm email)
        {
            if (ModelState.IsValid)
            {         
                var fromEmail = new MailAddress("newleaves4loop@gmail.com", "FIT 5032 !!!");
                var toEmail = new MailAddress(email.ToEmail);
                var fromEmailPassword = "bone1992ls"; // Replace with actual password

                string subject = "You have a Email from Sky!";
                string body = email.Message;
                var message = new MailMessage(fromEmail, toEmail);
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;
                if (email.Upload != null && email.Upload.ContentLength > 0)
                {
                    message.Attachments.Add(new Attachment(email.Upload.InputStream, System.IO.Path.GetFileName(email.Upload.FileName)));
                }
                using (var smtp = new SmtpClient())
                {

                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                    smtp.Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword);
                    smtp.Timeout = 20000;
                   
                    await smtp.SendMailAsync(message);
                    return RedirectToAction("Sent");
                }
            }
            return View(email);
        }
        public ActionResult Sent()
        {
            return View();
        }
        //Post send Individual email
       
        // POST: Journalists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

            //sent all journalist emails
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Emails(EmailForm email)
        {
            if (ModelState.IsValid)
            {
                var body = "";
                var message = new MailMessage();
                IEnumerable<Journalist> list = db.Journalists.ToList();               
                foreach (Journalist j in list)
                {
                    message.To.Add(new MailAddress(j.Email));  // replace with valid value 
                }           
                message.From = new MailAddress("newleaves4loop@gmail.com");  // replace with valid value
                message.Subject = "Your email subject";
                message.Body = string.Format(body);
                message.IsBodyHtml = true;
                if (email.Upload != null && email.Upload.ContentLength > 0)
                {
                    message.Attachments.Add(new Attachment(email.Upload.InputStream, System.IO.Path.GetFileName(email.Upload.FileName)));
                }
                using (var smtp = new SmtpClient())
                {
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                    var fromEmail = new MailAddress("newleaves4loop@gmail.com");
                    var fromEmailPassword = "bone1992ls"; // Replace with actual password
                    smtp.Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword);
                    smtp.Timeout = 20000;

                    await smtp.SendMailAsync(message);
                    return RedirectToAction("Sent");
                }
            }
            return View(email);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "JID,JName,DOB,Address,Phone,Email,Password")] Journalist journalist)
        {
            if (ModelState.IsValid)
            {
                db.Entry(journalist).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(journalist);
        }

        // GET: Journalists/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Journalist journalist = db.Journalists.Find(id);
            if (journalist == null)
            {
                return HttpNotFound();
            }
            return View(journalist);
        }

        // POST: Journalists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Journalist journalist = db.Journalists.Find(id);
            db.Journalists.Remove(journalist);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
