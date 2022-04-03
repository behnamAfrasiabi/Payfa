using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TestPayfa.Models;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace TestPayfa.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private IHostingEnvironment _environment;
        public HomeController(ILogger<HomeController> logger, IHostingEnvironment environment)
        {
            _logger = logger;
            _environment=environment;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.errors="sa";
            LogError("test LogError");
            string redirectPage = "https://test.com/home/PayfaCallback";
            await PayfaPayment(10000, redirectPage, 123);
            return View();
        }
        #region Payfa

        private async Task PayfaPayment(long price, string redirectPage, int paymentId)
        {
            try
            {
                // شماره ترمینال

                string xAPiKey = "123456789";
                string sendEndPoint = "https://payment.payfa.com/v2/api/Transaction/Request";
                string payfaCaalBack = redirectPage;

                vm_PayfaRequest payfaRequest = new vm_PayfaRequest()
                {
                    amount=price.ToString(),
                    invoiceId=paymentId.ToString(),
                    callbackUrl=payfaCaalBack,
                    mobileNumber=""
                };
                using (var client = new System.Net.Http.HttpClient())
                {
                    string payfaRequestString = System.Text.Json.JsonSerializer.Serialize(payfaRequest);
                    var payfaRequestData = new StringContent(payfaRequestString, Encoding.UTF8, "application/json");
                    client.DefaultRequestHeaders.Add("X-API-Key", xAPiKey);
                    try
                    {
                        var response = await client.PostAsync(sendEndPoint, payfaRequestData);
                        string apiResponse = response.Content.ReadAsStringAsync().Result;

                        LogError(apiResponse??"apiResponse is null");
                        var registerResult = JsonConvert.DeserializeObject<vm_PayfaRequestResult>(apiResponse);
                        if (registerResult != null && !string.IsNullOrEmpty(registerResult.statusCode) && registerResult.statusCode == "200")
                        {
                            // ثبت توکن در دیتابیس
                            //UpdatePayment(paymentId, registerResult.paymentId);

                            Response.Redirect(registerResult.paymentUrl);
                        }
                        else
                        {
                            LogError(apiResponse??"apiResponse is null");
                        }
                    }
                    catch (Exception ex2)
                    {
                        LogError(ex2.Message);
                    }

                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewData["message"] = "در حال حاظر امکان اتصال به این درگاه وجود ندارد ";
            }

        }
        // ساخته بشه تا نتیجه خرید به کاربر نمایش بشه view این اکشن باید براش 
        public async Task<ActionResult> PayfaCallback(string paymentId, string isSucceed)
        {
            ViewData["BankName"] = "درگاه پی فا";
            // فراخوانی متد برگشت از درگاه
            await PayfaCallbackService(paymentId, isSucceed);
            return View();
        }


        public async Task PayfaCallbackService(string paymentId, string isSucceed)
        {
            //کدهای برگشت از درگاه زرین پال و اختصاص مبلغ شارژ

            string xAPiKey = "123456789";
            string payfaConfirm = "https://payment.payfa.com/v2/api/Transaction/verify/";
            try
            {
                bool succeed = bool.Parse(isSucceed);
                if (succeed)
                {
                    //پیدا کردن رکورد پرداخت با توجه به توکن ارسالی از پی فا 
                    Payment payment = FindPaymentByPayfaPaymentId(paymentId);

                    vm_PayfaVerify payfaVerify = new vm_PayfaVerify()
                    {
                        paymentId=paymentId
                    };
                    using (var client = new System.Net.Http.HttpClient())
                    {

                        string payfaVerifyString = System.Text.Json.JsonSerializer.Serialize(payfaVerify);
                        var payfaVerifyData = new StringContent(payfaVerifyString, Encoding.UTF8, "application/json");
                        client.DefaultRequestHeaders.Add("X-API-Key", xAPiKey);
                        var response = await client.PostAsync(payfaConfirm+paymentId, null);
                        string apiResponse = response.Content.ReadAsStringAsync().Result;

                        var registerResult = JsonConvert.DeserializeObject<vm_PayfaVerifyResult>(apiResponse);

                        if (registerResult.statusCode == 200)
                        {
                            //اعمال تغییرات بعد از پرداخت موفق
                            //UpdatePayment()

                            // آماده سازی اطلاعات پرداخت جهت نمایش به کاربر

                        }
                        else
                        {
                            //اعمال تغییرات بعد از پرداخت ناموفق
                            //UpdatePayment()
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                LogError(ex.ToString());

                throw ex;
            }
        }

        private Payment FindPaymentByPayfaPaymentId(string payfaToken)
        {
            //پیدا کردن اطلاعات از دیتابیس با توجه به توکن ارسالی
            //Payment payment = _db.Payments.FirstOrDefault(p => p.PayfaToken==payfaToken);

            return new Payment();
        }

        private void LogError(string error)
        {
            string directory = _environment.WebRootPath;
            // Set the file path
            string path = directory + "\\errors.txt";
            // Add the error message to the file
            System.IO.File.AppendAllText(path, $"{DateTime.Now.ToString("o")} [ERR] {error}" + "\r\n");
            ViewBag.errors+=error+ "\r\n";
        }

        #endregion

    }
}
