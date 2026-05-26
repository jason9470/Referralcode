using System.ComponentModel.DataAnnotations;

namespace Referralcode.Models
{
    public class ReferralApplication
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "專案類型")]
        public string ProjectType { get; set; } = "A";

        [Display(Name = "專案代碼")]
        public string? ProjectCode { get; set; }

        [Display(Name = "專案起迄日 (起)")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "專案起迄日 (迄)")]
        public DateTime? EndDate { get; set; }

        // --- 新增 Type B 專用欄位 ---
        [Display(Name = "分公司名稱")]
        public string? BranchName { get; set; }

        [Display(Name = "合作廠商名稱")]
        public string? PartnerName { get; set; }

        [Display(Name = "廠商統編")]
        public string? PartnerId { get; set; }

        [Display(Name = "推薦碼")]
        public string? PartnerReferralCode { get; set; }

        [Display(Name = "折讓條件")]
        public string? DiscountCondition { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
