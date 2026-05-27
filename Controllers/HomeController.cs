using Microsoft.AspNetCore.Mvc;
using Referralcode.Data;
using Referralcode.Models;
using Referralcode.ViewModels;
using System.Diagnostics;

namespace Referralcode.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, AppDbContext context, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 先比對資料庫是否存在該帳號密碼
                var account = _context.SystemAccounts.FirstOrDefault(a => a.Username == model.Username && a.Password == model.Password);

                // 從設定檔讀取預設管理員帳號與加密的密碼
                var adminUsername = _configuration["AdminCredentials:Username"];
                var adminEncryptedPassword = _configuration["AdminCredentials:EncryptedPassword"];
                var adminDecryptedPassword = string.IsNullOrEmpty(adminEncryptedPassword) ? "" : Referralcode.Helpers.EncryptionHelper.Decrypt(adminEncryptedPassword);

                // 驗證是否為預設管理員
                bool isAdmin = model.Username == adminUsername && model.Password == adminDecryptedPassword;

                if (account != null || isAdmin)
                {
                    // 登入成功，導向推薦碼查詢首頁
                    return RedirectToAction("Index", "Referral");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "帳號或密碼錯誤");
                }
            }
            return View(model);
        }

        // 這裡將原本的 Privacy 改成帳號管理平台
        public IActionResult Privacy()
        {
            var accounts = _context.SystemAccounts.OrderByDescending(a => a.CreatedAt).ToList();
            return View(accounts);
        }

        [HttpPost]
        public IActionResult AddAccount(string username)
        {
            if (!string.IsNullOrWhiteSpace(username))
            {
                // 檢查是否已經有同名的帳號
                if (!_context.SystemAccounts.Any(a => a.Username == username))
                {
                    _context.SystemAccounts.Add(new SystemAccount
                    {
                        Username = username,
                        Password = "123456" // 預設密碼 123456
                    });
                    _context.SaveChanges();
                    TempData["SuccessMessage"] = $"帳號 {username} 新增成功！";
                }
                else
                {
                    TempData["ErrorMessage"] = $"帳號 {username} 已經存在。";
                }
            }
            return RedirectToAction("Privacy");
        }

        [HttpPost]
        public IActionResult DeleteAccount(int id)
        {
            var account = _context.SystemAccounts.Find(id);
            if (account != null)
            {
                _context.SystemAccounts.Remove(account);
                _context.SaveChanges();
                TempData["SuccessMessage"] = $"帳號 {account.Username} 已刪除。";
            }
            return RedirectToAction("Privacy");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
