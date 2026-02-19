using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.Interfaces.Models
{
    public class InvUserDto
    {
        [Required(ErrorMessage = "يرجى اختيار اسم المستخدم")]
        public int? UserCode { get; set; }
        [Required(ErrorMessage = "يرجى إدخال اسم المستخدم")]
        [StringLength(10, ErrorMessage = "اسم المستخدم يجب ألا يتجاوز 10 أحرف")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [StringLength(16, MinimumLength = 6, ErrorMessage = "كلمة المرور يجب أن تكون بين 6 و 16 حرف")]
        public string UserPasswd { get; set; }
        public string FullNameArabic { get; set; }
        public string Prog01 { get; set; }
        public string Prog02 { get; set; }
        public string Prog03 { get; set; }
        public string Prog11 { get; set; }
        public string Prog12 { get; set; }
        public string Prog13 { get; set; }
        public string Prog14 { get; set; }
        public string Prog21 { get; set; }
        public string Prog22 { get; set; }
        public string Prog23 { get; set; }
        public string Prog24 { get; set; }
        public string Prog25 { get; set; }
        public string Prog31 { get; set; }
        public string Prog32 { get; set; }
        public string Prog33 { get; set; }
        public string Prog34 { get; set; }
        public string Prog35 { get; set; }
        public string Prog29 { get; set; }
    }
}
