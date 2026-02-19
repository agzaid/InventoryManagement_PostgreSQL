using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain.Entities
{
    public class HInvTrans
    {
        // أعمدة Not Null (يجب أن لا تكون Nullable في C#)
        public int StoreCode { get; set; }
        public int TrType { get; set; }
        public DateTime TrDate { get; set; }
        public int TrNum { get; set; }
        public int TrSerial { get; set; }
        public string ItemCode { get; set; } // VARCHAR2(5 CHAR) NOT NULL
        public decimal ItemQnt { get; set; }  // NUMBER(8,2) NOT NULL

        // أعمدة يمكن أن تكون Null (Nullable)
        public int? DepCode { get; set; }
        public int? EmpCode { get; set; }
        public int? SuplierCode { get; set; } // لاحظ السباينج SUPLIER_CODE بـ S واحدة كما في جدولك
        public int? FromToStore { get; set; }
        public decimal? ItemPrice { get; set; }
        public int? BillNum { get; set; }
        public int? TrNum2 { get; set; }
        public DateTime? TrDate2 { get; set; }
        public DateTime? OrderDate { get; set; }
        public int? DeliverNo { get; set; }
        public DateTime? DeliverDate { get; set; }

        // --- Navigation Properties ---



        [ForeignKey(nameof(SuplierCode))]
        public virtual Supplier Supplier { get; set; }

        [ForeignKey(nameof(ItemCode))]
        public virtual Item Item { get; set; }
    }
}
