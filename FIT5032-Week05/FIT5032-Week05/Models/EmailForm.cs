using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FIT5032_Week05.Models
{
    public class EmailForm
    {
            [Required(ErrorMessage = "Please Enter Name"), Display(Name = "Your name")]
            public string FromName { get; set; }
            [Display(Name = "Send to email"), EmailAddress(ErrorMessage = "InVailid Email Address")]
            public string ToEmail { get; set; }
            public string Message { get; set; }
            public HttpPostedFileBase Upload { get; set; }
    }
}