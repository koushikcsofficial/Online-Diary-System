namespace DiarySystemWebApp.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class DiaryDetail
    {
        [Key]
        public Guid Diary_Id { get; set; }

        public Guid Account_Id { get; set; }

        [Required]
        public string Diary_Subject { get; set; }

        [Column(TypeName = "text")]
        [Required]
        public string Diary_Content { get; set; }

        public DateTime Diary_SubmittedAt { get; set; }

        public int Diary_IsAccepted { get; set; }

        public DateTime? Diary_ViewDate { get; set; }

        public Guid? Diary_AcceptedBy { get; set; }

        public virtual AccountDetail SubmitionAccountDetail { get; set; }

        public virtual AccountDetail AcceptanceAccountDetail { get; set; }
    }
}
