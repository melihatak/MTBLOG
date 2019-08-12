using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MTBlog.ViewModel
{
    public class UyeModel
    {
        public System.Guid UyeId { get; set; }

        [Required(ErrorMessage = "Please enter UserName")]
        [Display(Name = "User Name")]
        public string KullaniciAdi { get; set; }

        [Required(ErrorMessage = "Please enter Name and Surname")]
        [Display(Name = "First Name & Last Surname")]
        public string AdSoyad { get; set; }

        [Required(ErrorMessage = "Please enter Email Address")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Please enter Password")]
        [Display(Name = "Password")]
        public string Sifre { get; set; }

        [Required(ErrorMessage = "Please choose a photo")]
        [Display(Name = "Photo")]
        public HttpPostedFileBase Foto { get; set; }

        public Nullable<System.Guid> UyeAdmin { get; set; }
    }
}