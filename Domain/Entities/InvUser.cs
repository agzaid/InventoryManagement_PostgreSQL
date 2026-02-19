using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class InvUser
    {
        public int UserCode { get; set; }
        public string UserName { get; set; }
        public string UserPasswd { get; set; }
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


        public virtual EmpEgx Employee { get; set; }
    }
}
