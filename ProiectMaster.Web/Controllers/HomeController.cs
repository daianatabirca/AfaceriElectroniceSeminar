using iTextSharp.text;
using iTextSharp.text.log;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProiectMaster.DataAccess;
using ProiectMaster.Models.Entites;
using ProiectMaster.Models.Interfaces;
using ProiectMaster.Web.Models;
using IronPdf;
using ProiectMaster.Web.Views.Shared;
using QuickMailer;
using System;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Web;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using MailMessage = ProiectMaster.Web.Models.MailMessage;
using Grpc.Core;
using System.Net.Mime;
using ProiectMaster.Models.DTOs.VM;
using iTextSharp.text.html.simpleparser;
using Microsoft.AspNetCore.Components.RenderTree;
using System.Threading;

namespace ProiectMaster.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService productService;
        private readonly IOrderService orderService;
        protected readonly MagazinVirtualContext _db;

        private readonly ILogger<HomeController> _logger;
        private IViewRenderService _viewRender;

        public HomeController(IProductService productService, IOrderService orderService,  MagazinVirtualContext db, ILogger<HomeController> logger, IViewRenderService viewRender)
        {
            this.productService = productService;
            this.orderService = orderService;
            this._db = db;

            _logger = logger;
            _viewRender = viewRender;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var products = productService.GetAllProducts();
            return View(products);
        }

        [HttpGet]
        [Route("Details/{id}")]
        public IActionResult Details(int id)
        {
            var product = productService.GetProduct(id);
            return View(product);
        }

        [HttpPost]
        [Route("Add/{id}")]
        public IActionResult Add(int id, int qty)
        {
            var products = this._db.Products;
            Dictionary<int, int> shopList;

            if (HttpContext.Session.Get<Dictionary<int, int>>(SessionHelper.ShoppingCart) == null)
                shopList = new Dictionary<int, int>();
            else
                shopList = HttpContext.Session.Get<Dictionary<int, int>>(SessionHelper.ShoppingCart);

            foreach (Product p in products)
            {
                if (p.Id == id && qty > 0)
                {
                    if (shopList.ContainsKey(p.Id))
                    {
                        shopList[p.Id] = qty;
                    }
                    else
                    {
                        shopList.Add(p.Id, qty);
                    }
                }
            }

            HttpContext.Session.Set(SessionHelper.ShoppingCart, shopList);

            return RedirectToAction("Index", "Home", productService.GetAllProducts());
        }

        [ActionName("RemoveFromCart")]
        public IActionResult Remove(int id)
        {
            var shopList = HttpContext.Session.Get<Dictionary<int, int>>(SessionHelper.ShoppingCart);

            if (shopList == null)
                return RedirectToAction("Cart", "Home", productService.GetAllProducts());

            if (shopList.ContainsKey(id))
                shopList.Remove(id);

            HttpContext.Session.Set(SessionHelper.ShoppingCart, shopList);

            return RedirectToAction("Cart", "Home", productService.GetAllProducts());
        }

        public IActionResult Cart()
        {
            List<Product> cartProd = new List<Product>();
            var shopList = HttpContext.Session.Get<Dictionary<int, int>>(SessionHelper.ShoppingCart);
            if (shopList == null)
            {
                shopList = new Dictionary<int, int>();
            }
            var products = this._db.Products;

            foreach (Product p in products)
            {
                if (shopList.ContainsKey(p.Id))
                    cartProd.Add(p);
            }
            return View(cartProd);
        }

        public IActionResult Checkout()
        {
            List<Product> cartProd = new List<Product>();
            var shopList = HttpContext.Session.Get<Dictionary<int, int>>(SessionHelper.ShoppingCart);
            if (shopList == null)
            {
                shopList = new Dictionary<int, int>();
            }
            var products = this._db.Products;

            foreach (Product p in products)
            {
                if (shopList.ContainsKey(p.Id))
                    cartProd.Add(p);
            }

            return View(cartProd);
        }

        public IActionResult SendEmail()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SendEmail(Order order)
        {
            order.OrderDate = DateTime.Now;

            OrderVM ord = new OrderVM();
            ord.OrderDate = order.OrderDate;
            ord.CustomerPhoneNumber = order.CustomerPhoneNumber;
            ord.CustomerName = order.CustomerName;
            ord.CustomerEmail = order.CustomerEmail;
            ord.CustomerAddress = order.CustomerAddress;
            orderService.AddOrder(ord);

            try
            {
                Email email = new Email();
                string message = "Email confirmation failed.";

                List<Attachment> attachments = new List<Attachment>();
                string file = "C:/Users/daian/OneDrive/Desktop/MasterAfaceriElectronice/Seminar 3/ProiectMaster/ProiectMaster.Web/Utils/Invoice.xls";
                Attachment attachment = new Attachment(file, MediaTypeNames.Application.Octet);
                ContentDisposition disposition = attachment.ContentDisposition;
                disposition.CreationDate = System.IO.File.GetCreationTime(file);
                disposition.ModificationDate = System.IO.File.GetLastWriteTime(file);
                disposition.ReadDate = System.IO.File.GetLastAccessTime(file);
                attachments.Add(attachment);

                bool isSend = email.SendEmail(order.CustomerEmail, Credential.Email, Credential.Password, "#" + DateTime.Now.ToString("yyyyMMddHHmmss") + " Invoice",
                     "Your order has been successfully placed." + Environment.NewLine + "Name: " + ord.CustomerName + Environment.NewLine + "Address: " 
                     + ord.CustomerAddress, attachments);
                if (isSend)
                {
                    message = "Email confirmation has been sent.";
                    ModelState.Clear();
                }

                ViewBag.Message = message;

                if(ViewBag.Message == "Email confirmation has been sent.")
                {
                    var shopList = HttpContext.Session.Get<Dictionary<int, int>>(SessionHelper.ShoppingCart);

                    shopList.Clear();

                    HttpContext.Session.Set(SessionHelper.ShoppingCart, shopList);

                    return RedirectToAction("Index", "Home", productService.GetAllProducts());
                }
                else return View();
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
