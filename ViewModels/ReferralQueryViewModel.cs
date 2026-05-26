using System.ComponentModel.DataAnnotations;
using Referralcode.Models;

namespace Referralcode.ViewModels
{
    public class ReferralQueryViewModel
    {
        // 查詢條件
        public string QueryType { get; set; } = "MG"; // "MG" 或 "BCBE"
        
        // BC/BE 專屬條件
        public bool IncludeBC { get; set; } = true;
        public bool IncludeBE { get; set; } = true;
        
        // 共用條件
        public DateTime? SearchStartDate { get; set; }
        public DateTime? SearchEndDate { get; set; }
        public string? SearchPartnerId { get; set; }
        public string? SearchReferralCode { get; set; }

        // 查詢結果
        public List<ReferralApplication> Results { get; set; } = new List<ReferralApplication>();
    }
}
