using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShoppingStore.Controllers
{
    public class PaymentController : Controller
    {
        // GET: Payment
        public ActionResult PaymentWithPaypal()
        {
            APIContext apicontext = PayPalConfiguration.GetAPIContext();
            try
            {
                string PayerID = Request.Params["PayerID"];

                if (string.IsNullOrEmpty(PayerID))
                {
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "PaymentWithPaypal/PaymentWithPaypal?";

                    var Guid = Convert.ToString((new Random()).Next(100000000));

                    var createPayment = this.CreatePayment(apicontext, baseURI + "guid=" + Guid);

                    var links = createPayment.links.GetEnumerator();

                    string paypalRedirect = null;

                    while (links.MoveNext())
                    {
                        Links ink = links.Current;

                        if (ink.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            paypalRedirect = ink.href;
                        }
                    }
                }
                else
                {
                    var guid = Request.Params["guid"];

                    var exectePayment = ExecutePayment(apicontext, PayerID, Session[guid] as string);

                    if (exectePayment.ToString().ToLower() != "approved")
                    {
                        return View("FailureView");
                    }
                }
            }
            catch (Exception)
            {

                return View("FailureView");
            }

            return View("SuccessView");
        }

        private object ExecutePayment(APIContext apicontext, string payerID, string paymentId)
        {
            var paymentExecution = new PaymentExecution() { payer_id = payerID };
            this.payment = new Payment() { id = payerID };
            return this.payment.Execute(apicontext, paymentExecution);
        }

        private PayPal.Api.Payment payment;

        private Payment CreatePayment(APIContext apicontext, string redirectURI)
        {
            var itemlist = new ItemList(){items = new List<Item>()};

            if (Session["cart"] != null)
            {
                List<Models.Home.Item> cart = (List<Models.Home.Item>)Session["cart"];
                foreach (var item in cart)
                {
                    itemlist.items.Add(new Item() {
                        name = item.Product.ProductName,
                        currency = "TK",
                        price = item.Product.Price.ToString(),
                        quantity = item.Product.Quantity.ToString(),
                        sku = "sku"
                    });
                }

                var payer = new Payer() { payment_method = "paypal" };

                var rediUrl = new RedirectUrls()
                {
                    cancel_url = redirectURI + "&Cancel=true",
                    return_url = redirectURI
                };

                var details = new Details()
                {
                    tax = "1",
                    shipping = "1",
                    subtotal = "1"
                };

                var amount = new Amount()
                {
                    currency = "USD",
                    total = Session["cart"].ToString(),
                    details = details
                };

                var transactionList = new List<Transaction>();
                transactionList.Add(new Transaction()
                {
                    description = "TransactionDescription",
                    invoice_number = "10000000",
                    amount = amount,
                    item_list = itemlist
                });

                this.payment = new Payment()
                {
                    intent = "sale",
                    payer = payer,
                    transactions = transactionList,
                    redirect_urls = rediUrl

                };
            }

            return this.payment.Create(apicontext);
        }
    }
}