using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Referralcode.Data;
using Referralcode.Models;
using Referralcode.ViewModels;
using System.Diagnostics;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

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
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Referral");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool isAuthSuccess = false;
                string errorMessage = "帳號或密碼錯誤";

                // 先比對資料庫是否存在該帳號密碼
                var account = await _context.SystemAccounts.FirstOrDefaultAsync(a => a.Username == model.Username);

                if(account != null)
                {
                    // 根據正式環境邏輯，將帳號前方的0去掉
                    string formattedAccount = model.Username.TrimStart('0');

                    // 執行 AD 驗證
                    var adServer = _configuration["AdSettings:AdServer"];
                    var adDomainName = _configuration["AdSettings:AdDomainName"];

                    if (!string.IsNullOrEmpty(adServer) && !string.IsNullOrEmpty(adDomainName))
                    {
                        var g_objAuth = new Referralcode.Services.AuthLib();
                        g_objAuth.setAdPath("LDAP://" + adServer);
                        g_objAuth.setDomainName(adDomainName);
                        g_objAuth.setUsrId(formattedAccount.Trim());
                        g_objAuth.setUsrPwd(model.Password.Trim());

                        string adResult = g_objAuth.getLdapAuthRes();
                        if (adResult == "Success")
                        {
                            isAuthSuccess = true;
                        }
                        else
                        {
                            errorMessage = adResult.Replace("\n", "").Replace("\r", "");
                        }
                    }
                }
                else
                {
                    // 從設定檔讀取預設管理員帳號與加密的密碼
                    var adminUsername = _configuration["AdminCredentials:Username"];
                    var adminEncryptedPassword = _configuration["AdminCredentials:EncryptedPassword"];
                    var adminDecryptedPassword = string.IsNullOrEmpty(adminEncryptedPassword) ? "" : Referralcode.Helpers.EncryptionHelper.Decrypt(adminEncryptedPassword);

                    // 驗證是否為預設管理員
                    bool isAdmin = model.Username == adminUsername && model.Password == adminDecryptedPassword;
                    if (isAdmin)
                        isAuthSuccess = true;
                }

                if (isAuthSuccess)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, model.Username)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    // 登入成功，導向推薦碼查詢首頁
                    return RedirectToAction("Index", "Referral");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, errorMessage);
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Home");
        }

        // 這裡將原本的 Privacy 改成帳號管理平台
        [Authorize]
        public async Task<IActionResult> Privacy()
        {
            var accounts = await _context.SystemAccounts.AsNoTracking().OrderByDescending(a => a.CreatedAt).ToListAsync();
            return View(accounts);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddAccount(string username)
        {
            if (!string.IsNullOrWhiteSpace(username))
            {
                // 檢查是否已經有同名的帳號
                if (!await _context.SystemAccounts.AnyAsync(a => a.Username == username))
                {
                    _context.SystemAccounts.Add(new SystemAccount
                    {
                        Username = username.Trim(),
                    });
                    await _context.SaveChangesAsync();
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
        [Authorize]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var account = await _context.SystemAccounts.FindAsync(id);
            if (account != null)
            {
                _context.SystemAccounts.Remove(account);
                await _context.SaveChangesAsync();
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
