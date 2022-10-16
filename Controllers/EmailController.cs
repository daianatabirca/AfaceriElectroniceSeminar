using Microsoft.AspNetCore.Mvc;
using QuickMailer;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net;
using System;
using ProiectMaster.Web.Models;
using MailMessage = ProiectMaster.Web.Models.MailMessage;
using System.Linq.Expressions;
using System.Linq;

namespace ProiectMaster.Web.Controllers
{
    public class EmailController : Controller
    {
        public IActionResult Send()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Send(MailMessage mailMessage)
        {
            try
            {
                List<string> toMailAddress = new List<string>();
                List<string> ccMailAddress = new List<string>();
                List<string> bccMailAddress = new List<string>();
                Email email = new Email();
                toMailAddress = GetValidMail(mailMessage.To);
                string message = "Email confirmation failed.";

                List<Attachment> attachments = new List<Attachment>();
                if (mailMessage.Files != null)
                {
                    foreach (var file in mailMessage.Files)
                    {
                        Attachment attachment = new Attachment(file.OpenReadStream(), file.FileName);
                        attachments.Add(attachment);
                    }
                }
                bool isSend = email.SendEmail(toMailAddress, Credential.Email, Credential.Password, mailMessage.Subject,
                     "Your order has been successfully placed.", ccMailAddress, bccMailAddress, attachments);
                if (isSend)
                {
                    message = "Email confirmation has been sent.";
                    ModelState.Clear();
                }

                ViewBag.Message = message;
                return View();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public List<string> GetValidMail(List<string> mails)
        {
            List<string> validMails = new List<string>();
            Email email = new Email();
            if (mails == null)
            {
                return validMails;
            }
            if (mails.Any())
            {

                foreach (var mail in mails)
                {
                    bool isValid = email.IsValidEmail(mail);
                    if (isValid)
                    {
                        validMails.Add(mail);
                    }
                }
            }

            return validMails;
        }


    }
}
