namespace DiarySystemWebApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class AccountDetail
    {
        public AccountDetail()
        {
            SubmittedDiaryDetails = new HashSet<DiaryDetail>();
            AcceptedDiaryDetails = new HashSet<DiaryDetail>();
        }

        [Key]
        public Guid Account_Id { get; set; }

        [Required]
        public string User_Email { get; set; }

        [Required]
        public string User_Password { get; set; }

        [Required]
        public string User_FirstName { get; set; }

        [Required]
        public string User_LastName { get; set; }

        [Required]
        public string User_Address { get; set; }

        [Required]
        [StringLength(13)]
        public string User_Mobile { get; set; }

        public byte Account_IsOfficial { get; set; }

        public virtual ICollection<DiaryDetail> SubmittedDiaryDetails { get; set; }

        public virtual ICollection<DiaryDetail> AcceptedDiaryDetails { get; set; }
    }
}
