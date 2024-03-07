using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AmaZone.Models
{
    public enum Role
    {
        User,
        Admin
    }
    public class User
    {
        [Key]
        public int id { get; set; }
        [Display(Name = "Username:")]
        [Required(ErrorMessage = "Field Required")]
        public string username { get; set; }
        [Display(Name = "Password:")]
        [Required(ErrorMessage = "Field Required")]
        [DataType(DataType.Password)]

        public string password { get; set; }
        [Display(Name = "Full Name:")]
        [Required(ErrorMessage = "Field Required")]

        public string name { get; set; }

        [Display(Name = "CF o Partita IVA:")]
        [Required(ErrorMessage = "Field Required")]
        [RegularExpression("^([A-Z]{6}[0-9LMNPQRSTUV]{2}[ABCDEHLMPRST]{1}[0-9LMNPQRSTUV]{2}[A-Z]{1}[0-9LMNPQRSTUV]{3}[A-Z]{1})$|([0-9]{11})$",ErrorMessage = "Insert a valid code")]
        public string UserCode { get; set; }
        public Role role = Role.User;

        public Role ToRole(int role)
        {
            switch (role)
            {
                case 2:
                    return Role.Admin;
                default:
                    return Role.User;
            }
        }

        public string ToRoleString(int role)
        {
            switch (role)
            {
                case 2:
                    return "admin";
                default:
                    return "user";
            }
        }



        public int ParseRole(Role role)
        {
            switch (role)
            {
                case Role.Admin:
                    return 2;
                default:
                    return 1;
            }
        }
    }
}