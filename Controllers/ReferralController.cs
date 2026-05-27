using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Referralcode.Data;
using Referralcode.Models;
using Referralcode.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Referralcode.Controllers
{
    [Authorize]
    public class ReferralController : Controller
    {
        private readonly AppDbContext _context;

        public ReferralController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Referral/Index (查詢頁面)
        public async Task<IActionResult> Index(ReferralQueryViewModel vm)
        {
            var query = _context.ReferralApplications.AsQueryable();

            if (vm.QueryType == "MG")
            {
                query = query.Where(x => x.ProjectType == "A");

                if (vm.SearchStartDate.HasValue)
                {
                    query = query.Where(x => x.StartDate >= vm.SearchStartDate.Value);
                }
                if (vm.SearchEndDate.HasValue)
                {
                    query = query.Where(x => x.EndDate <= vm.SearchEndDate.Value);
                }
                if (!string.IsNullOrEmpty(vm.SearchReferralCode))
                {
                    query = query.Where(x => x.PartnerReferralCode.Contains(vm.SearchReferralCode));
                }
            }
            else if (vm.QueryType == "BCBE")
            {
                query = query.Where(x => x.ProjectType == "B");

                // 處理 BC / BE 複選
                var projectCodes = new List<string>();
                if (vm.IncludeBC) projectCodes.Add("金融特約商-BC");
                if (vm.IncludeBE) projectCodes.Add("異業合作-BE");
                
                if (projectCodes.Any())
                {
                    query = query.Where(x => projectCodes.Contains(x.ProjectCode));
                }
                else
                {
                    // 若都沒勾選，則回傳空結果
                    query = query.Where(x => false);
                }

                if (vm.SearchStartDate.HasValue)
                {
                    query = query.Where(x => x.StartDate >= vm.SearchStartDate.Value);
                }
                if (vm.SearchEndDate.HasValue)
                {
                    query = query.Where(x => x.EndDate <= vm.SearchEndDate.Value);
                }
                if (!string.IsNullOrEmpty(vm.SearchPartnerId))
                {
                    query = query.Where(x => x.PartnerId.Contains(vm.SearchPartnerId));
                }
                if (!string.IsNullOrEmpty(vm.SearchReferralCode))
                {
                    query = query.Where(x => x.PartnerReferralCode.Contains(vm.SearchReferralCode));
                }
            }

            vm.Results = await query.OrderByDescending(x => x.ProjectType).ThenBy(x => x.BranchName).ThenBy(x => x.StartDate).ToListAsync();
            return View(vm);
        }

        // GET: Referral/Index2 (查詢頁面副本)
        public async Task<IActionResult> Index2(ReferralQueryViewModel vm)
        {
            var query = _context.ReferralApplications.AsQueryable();

            if (vm.QueryType == "MG")
            {
                query = query.Where(x => x.ProjectType == "A");

                if (vm.SearchStartDate.HasValue)
                {
                    query = query.Where(x => x.StartDate >= vm.SearchStartDate.Value);
                }
                if (vm.SearchEndDate.HasValue)
                {
                    query = query.Where(x => x.EndDate <= vm.SearchEndDate.Value);
                }
                if (!string.IsNullOrEmpty(vm.SearchReferralCode))
                {
                    query = query.Where(x => x.PartnerReferralCode.Contains(vm.SearchReferralCode));
                }
            }
            else if (vm.QueryType == "BCBE")
            {
                query = query.Where(x => x.ProjectType == "B");

                // 處理 BC / BE 複選
                var projectCodes = new List<string>();
                if (vm.IncludeBC) projectCodes.Add("金融特約商-BC");
                if (vm.IncludeBE) projectCodes.Add("異業合作-BE");
                
                if (projectCodes.Any())
                {
                    query = query.Where(x => projectCodes.Contains(x.ProjectCode));
                }
                else
                {
                    // 若都沒勾選，則回傳空結果
                    query = query.Where(x => false);
                }

                if (vm.SearchStartDate.HasValue)
                {
                    query = query.Where(x => x.StartDate >= vm.SearchStartDate.Value);
                }
                if (vm.SearchEndDate.HasValue)
                {
                    query = query.Where(x => x.EndDate <= vm.SearchEndDate.Value);
                }
                if (!string.IsNullOrEmpty(vm.SearchPartnerId))
                {
                    query = query.Where(x => x.PartnerId.Contains(vm.SearchPartnerId));
                }
                if (!string.IsNullOrEmpty(vm.SearchReferralCode))
                {
                    query = query.Where(x => x.PartnerReferralCode.Contains(vm.SearchReferralCode));
                }
            }

            vm.Results = await query.OrderByDescending(x => x.ProjectType).ThenBy(x => x.BranchName).ThenBy(x => x.StartDate).ToListAsync();
            return View(vm);
        }

        // POST: Referral/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int[] selectedIds, string QueryType)
        {
            if (selectedIds != null && selectedIds.Length > 0)
            {
                var itemsToDelete = await _context.ReferralApplications
                    .Where(x => selectedIds.Contains(x.Id))
                    .ToListAsync();

                if (itemsToDelete.Any())
                {
                    _context.ReferralApplications.RemoveRange(itemsToDelete);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"成功刪除 {itemsToDelete.Count} 筆資料。";
                }
            }

            // 重導回 Index2 並保留原本的查詢頁籤類型
            return RedirectToAction(nameof(Index2), new { QueryType });
        }

        // GET: Referral/Create
        public IActionResult Create()
        {
            // 預設模型值
            var model = new ReferralApplication
            {
                ProjectType = "A",
                ProjectCode = "MGM-MG"
            };
            return View(model);
        }

        // POST: Referral/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReferralApplication model)
        {
            if (ModelState.IsValid)
            {
                // 如果是 Type B，檢查推薦碼是否重複
                if (model.ProjectType == "B" && !string.IsNullOrEmpty(model.PartnerReferralCode))
                {
                    bool isDuplicate = await _context.ReferralApplications
                        .AnyAsync(x => x.PartnerReferralCode == model.PartnerReferralCode);
                        
                    if (isDuplicate)
                    {
                        ModelState.AddModelError("PartnerReferralCode", "推薦碼重複，請使用其他推薦碼");
                        return View(model);
                    }
                }

                // 如果是 Type A，確保 ProjectCode 寫死
                if (model.ProjectType == "A")
                {
                    model.ProjectCode = "MGM-MG";
                    // 為了成功寫入資料庫，先讓必填且暫時用不到的欄位為 null (因為前端沒傳過來且已 disabled)
                    // (EF Core 會處理)
                }

                _context.Add(model);
                await _context.SaveChangesAsync();
                
                // Type A 自動產生推薦碼邏輯：MG + 6碼(左側補零ID)
                if (model.ProjectType == "A")
                {
                    model.PartnerReferralCode = $"MG{model.Id:D6}";
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }

                TempData["SuccessMessage"] = $"專案類型 {model.ProjectType} ({model.ProjectCode}) 已成功申請！{(model.ProjectType == "A" ? " 系統已自動產生推薦碼: " + model.PartnerReferralCode : "")}";
                return RedirectToAction(nameof(Create));
            }
            return View(model);
        }

        // GET: Referral/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var referralApplication = await _context.ReferralApplications.FindAsync(id);
            if (referralApplication == null)
            {
                return NotFound();
            }
            return View(referralApplication);
        }

        // POST: Referral/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ReferralApplication model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingModel = await _context.ReferralApplications.FindAsync(id);
                    if (existingModel == null)
                    {
                        return NotFound();
                    }

                    // 如果是 Type B，檢查推薦碼是否重複 (排除自己)
                    if (existingModel.ProjectType == "B" && !string.IsNullOrEmpty(model.PartnerReferralCode))
                    {
                        bool isDuplicate = await _context.ReferralApplications
                            .AnyAsync(x => x.Id != id && x.PartnerReferralCode == model.PartnerReferralCode);
                            
                        if (isDuplicate)
                        {
                            ModelState.AddModelError("PartnerReferralCode", "推薦碼重複，請使用其他推薦碼");
                            // 為了在返回視圖時能正確顯示欄位，需確保 model 帶有原有的資料
                            model.ProjectType = existingModel.ProjectType;
                            model.ProjectCode = existingModel.ProjectCode;
                            model.BranchName = existingModel.BranchName;
                            model.PartnerName = existingModel.PartnerName;
                            model.PartnerId = existingModel.PartnerId;
                            model.DiscountCondition = existingModel.DiscountCondition;
                            return View(model);
                        }
                    }

                    // Update fields
                    existingModel.StartDate = model.StartDate;
                    existingModel.EndDate = model.EndDate;

                    if (existingModel.ProjectType == "B")
                    {
                        existingModel.ProjectCode = model.ProjectCode;
                        existingModel.BranchName = model.BranchName;
                        existingModel.PartnerName = model.PartnerName;
                        existingModel.PartnerId = model.PartnerId;
                        existingModel.PartnerReferralCode = model.PartnerReferralCode;
                        existingModel.DiscountCondition = model.DiscountCondition;
                    }

                    _context.Update(existingModel);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "推薦碼資料已成功更新！";
                    
                    // 重導回正確的頁籤
                    string queryType = existingModel.ProjectType == "A" ? "MG" : "BCBE";
                    return RedirectToAction(nameof(Index2), new { QueryType = queryType });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReferralApplicationExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(model);
        }

        private bool ReferralApplicationExists(int id)
        {
            return _context.ReferralApplications.Any(e => e.Id == id);
        }
    }
}
