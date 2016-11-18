using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FBCode.Models;
using System.Configuration;
using Stripe;
using Newtonsoft.Json.Linq;

namespace FBCode.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext ctx = new ApplicationDbContext();
       
        public ActionResult Index()
        {
            var x = ctx.Cards.Where(y => y.Email == System.Web.HttpContext.Current.User.Identity.Name);
            ViewBag.PublishableKey = ConfigurationManager.AppSettings["StripePublishableKey"];

            return View(x);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        [HttpPost]
        public ActionResult Index(string jsonCardInformation)
        {

            JObject jObject = JObject.Parse(jsonCardInformation);
            string key = ConfigurationManager.AppSettings["StripeSecretKey"];
            var myCustomer = new StripeCustomerCreateOptions();
            myCustomer.Email = System.Web.HttpContext.Current.User.Identity.Name;
            myCustomer.Description = "Testing a Card";
            myCustomer.SourceToken = jObject["tokenId"].ToString();
            var customerService = new StripeCustomerService();
            StripeRequestOptions requestOption = new StripeRequestOptions() { ApiKey = key };
            StripeCustomer stripeCustomer = new StripeCustomer();
            stripeCustomer = customerService.Create(myCustomer, requestOption);
            CardInfo newCustomer = new CardInfo()
            {
                CustomerId = stripeCustomer.Id,
                Email = System.Web.HttpContext.Current.User.Identity.Name,
                Name = jObject["name"].ToString(),
                CardType = jObject["cardBrand"].ToString(),
                LastFour = jObject["lastFour"].ToString()
            };
            ctx.Cards.Add(newCustomer);
            ctx.SaveChanges();
            var x = ctx.Cards.Where(y => y.Email == System.Web.HttpContext.Current.User.Identity.Name);
            return View(x);
        }
   

    }
}