using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace DiarySystemWebApp.Models
{
    public class BusinessLogics:GeneralLogics
    {
        //Register a user (official/ unofficial)
        public int registerUser(string User_FirstName, string User_LastName, string User_Email, string User_Password, string User_Address, string User_Mobile, int? SecretCode)
        {
            List<string> inputElements = new List<string> { User_FirstName, User_LastName, User_Email, User_Password, User_Address, User_Mobile };
            if (ContainsAnyNullorWhiteSpace(inputElements))
            {
                //No mandetory fields should be left empty. Error occured
                return 0;
            }
            else
            {
                if (ContainsOnlyAlphabets(User_FirstName) && ContainsOnlyAlphabets(User_LastName) && ValidEmail(User_Email) && ContainsOnlyDigits(User_Mobile))
                {
                    if (SecretCode == 7412020)
                    {
                        using (DatabaseContext db = new DatabaseContext())
                        {
                            //if any account already present with the same email id account can't be created
                            if (db.AccountDetails.Any(account => account.User_Email == User_Email))
                            {
                                //Error occured
                                return 2;
                            }
                            else
                            {
                                AccountDetail acc = new AccountDetail();

                                acc.Account_Id = CreateUniqueId();
                                acc.User_FirstName = User_FirstName;
                                acc.User_LastName = User_LastName;
                                acc.User_Email = User_Email.ToLower();
                                acc.User_Password = User_Password;
                                acc.User_Address = User_Address;
                                acc.User_Mobile = User_Mobile;
                                acc.Account_IsOfficial = 1;
                                try
                                {
                                    db.AccountDetails.Add(acc);
                                    db.SaveChanges();

                                    //Successfully created account
                                    return 1;
                                }
                                catch
                                {
                                    //Error occured while creating account
                                    return 3;
                                }

                            }

                        }
                    }
                    else
                    {
                        using (DatabaseContext db = new DatabaseContext())
                        {
                            //if any account already present with the same email id account can't be created
                            if (db.AccountDetails.Any(account => account.User_Email == User_Email))
                            {
                                //Error occured
                                return 2;
                            }
                            else
                            {
                                AccountDetail acc = new AccountDetail();

                                acc.Account_Id = CreateUniqueId();
                                acc.User_FirstName = User_FirstName;
                                acc.User_LastName = User_LastName;
                                acc.User_Email = User_Email.ToLower();
                                acc.User_Password = User_Password;
                                acc.User_Address = User_Address;
                                acc.User_Mobile = User_Mobile;
                                acc.Account_IsOfficial = 0;

                                try
                                {
                                    db.AccountDetails.Add(acc);
                                    db.SaveChanges();

                                    //Successfully created account
                                    return 1;
                                }
                                catch
                                {
                                    //Error occured while creating account
                                    return 3;
                                }
                            }
                        }
                    }
                }
                else
                {
                    //Improper input provided. Account can't be created.
                    return 4;
                }
            }
        }

        //Login a user
        public AccountDetail Login(string email, string password)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                return db.AccountDetails.Where(account => account.User_Email == email.ToLower() && account.User_Password == password).SingleOrDefault();
            }
        }

        //Get account id by email
        public Guid getAccountId(string email)
        {
            using(DatabaseContext db = new DatabaseContext())
            {
                return db.AccountDetails.Where(acc => acc.User_Email == email).SingleOrDefault().Account_Id;
            }
        }

        //Get the list of Diaries of an user
        public List<DiaryDetail> getAllDiaries(string email)
        {
            using(DatabaseContext db = new DatabaseContext())
            {
                return db.DiaryDetails.Include(rec=>rec.SubmitionAccountDetail).Where(rec => rec.SubmitionAccountDetail.User_Email == email).OrderByDescending(rec=>rec.Diary_SubmittedAt).ToList();
            }
        }

        //Get a single Diary of an user
        public DiaryDetail getDiary(Guid Diary_Id)
        {
            using(DatabaseContext db = new DatabaseContext())
            {
                return db.DiaryDetails.Where(rec => rec.Diary_Id == Diary_Id).Include(acc => acc.SubmitionAccountDetail).Include(acc => acc.AcceptanceAccountDetail).SingleOrDefault();
            }
        }

        //method to check if the account registered the provided diary number
        public bool ifThisAccountRegisteredThisDiary(string email,Guid diary_id)
        {
            using(DatabaseContext db = new DatabaseContext())
            {
                return db.AccountDetails.Any(acc => acc.SubmittedDiaryDetails.Any(rec => rec.Diary_Id == diary_id) && acc.User_Email == email);
            }
        }

        //method to check if the account accepted the provided diary number
        public bool ifThisAccountAcceptedThisDiary(string email, Guid diary_id)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                return db.AccountDetails.Any(acc => acc.AcceptedDiaryDetails.Any(rec => rec.Diary_Id == diary_id) && acc.User_Email == email);
            }
        }

        //get the list of diaries which are pending
        public List<DiaryDetail> GetPendingDiaries(string email)
        {
            using(DatabaseContext db = new DatabaseContext())
            {
                return db.DiaryDetails.Where(rec => rec.Diary_IsAccepted == 2 && rec.SubmitionAccountDetail.User_Email != email).Include(rec=>rec.SubmitionAccountDetail).ToList();
            }
        }

        //Register a diary
        public int registerDiary(string email, string Diary_Subject, string Diary_Content)
        {
            if (!String.IsNullOrWhiteSpace(Diary_Subject) && !String.IsNullOrWhiteSpace(Diary_Content))
            {
                using(DatabaseContext db = new DatabaseContext())
                {
                    DiaryDetail diary = new DiaryDetail();

                    diary.Diary_Id = CreateUniqueId();
                    diary.Account_Id = getAccountId(email);
                    diary.Diary_Subject = Diary_Subject;
                    diary.Diary_Content = Diary_Content;
                    diary.Diary_SubmittedAt = CurrentIndianTime();
                    // 2 means pending
                    diary.Diary_IsAccepted = 2;
                    diary.Diary_ViewDate = null;
                    diary.Diary_AcceptedBy = null;

                    try
                    {
                        db.DiaryDetails.Add(diary);
                        db.SaveChanges();
                        //Successfully created the diary
                        return 1;
                    }
                    catch
                    {
                        //Internal error occured while registering the diary.
                        return 2;
                    }
                }
            }
            else
            {
                //Mandetory fields are empty. Error occured.
                return 0;
            }
        }

        //update a diary by normal user
        public int updateDiaryByUser(Guid Diary_id, string Diary_Subject, string Diary_Content)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                DiaryDetail diary = db.DiaryDetails.Find(Diary_id);
                diary.Diary_Subject = Diary_Subject;
                diary.Diary_Content = Diary_Content;
                try
                {
                    db.Entry(diary).State = EntityState.Modified;
                    db.SaveChanges();
                    //successfully updated details
                    return 1;
                }
                catch
                {
                    //internal error occured while updating existing diary
                    return 0;
                }
            }
        }

        //update a diary by official user
        public int updateDiaryByOfficial(string email, Guid diary_Id, int setAcceptance)
        {
            using(DatabaseContext db = new DatabaseContext())
            {
                DiaryDetail diary = db.DiaryDetails.Find(diary_Id);
                diary.Diary_IsAccepted = setAcceptance;
                diary.Diary_ViewDate = CurrentIndianTime();
                diary.Diary_AcceptedBy = getAccountId(email);
                try
                {
                    db.Entry(diary).State = EntityState.Modified;
                    db.SaveChanges();
                    //successfully updated details
                    return 1;
                }
                catch
                {
                    //internal error occured while updating existing diary
                    return 0;
                }
            }
        }

        //Delete a diary
        public int RemoveDiary(Guid Diary_id)
        {
            using(DatabaseContext db = new DatabaseContext())
            {
                try
                {
                    db.Entry(getDiary(Diary_id)).State = EntityState.Deleted;
                    db.SaveChanges();
                    return 1;
                }
                catch
                {
                    //internal error occured
                    return 0;
                }
                
            }
        }
    }
}