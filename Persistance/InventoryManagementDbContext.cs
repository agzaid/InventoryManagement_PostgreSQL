using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Persistance
{
    public class InventoryManagementDbContext : AuditableDbContext
    {
        public InventoryManagementDbContext(DbContextOptions<InventoryManagementDbContext> options):base(options)
        {
        }
        public DbSet<Department> Departments { get; set; }
        public DbSet<HInvTrans> HInvTrans { get; set; }
        public DbSet<InvTrans> InvTrans { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<InvUser> InvUsers { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemBalance> ItemBalance { get; set; }
        public DbSet<ItemCard> ItemCards { get; set; }
        public DbSet<ItemCategory> ItemCategories { get; set; }
        public DbSet<MonthlyBalance> MonthlyBalances { get; set; }
        public DbSet<MonthlyConsum> MonthlyConsums { get; set; }
        public DbSet<OpenBalance> OpenBalances { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<SysMessg> SysMessgs { get; set; }
        public DbSet<EmpEgx> EmpEgx { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Default schema
            modelBuilder.HasDefaultSchema("KWAREHOUSE");

            modelBuilder.Entity<Department>(entity =>
            {
                // Map table
                entity.ToTable("DEPARTMENTS");

                // Primary key
                entity.HasKey(e => e.DepCode);

                // Map properties to exact Oracle columns
                entity.Property(e => e.DepCode).HasColumnName("DEP_CODE");
                entity.Property(e => e.DepDesc).HasColumnName("DEP_DESC");
                entity.Property(e => e.SectorCode).HasColumnName("SECTOR_CODE");
                entity.Property(e => e.GeneralManeg).HasColumnName("GENERAL_MANEG");
                entity.Property(e => e.ManegSupCode).HasColumnName("MANEG_SUPCODE");
            });
            modelBuilder.Entity<Branch>(entity =>
            {
                entity.ToTable("BRANCHES");
                entity.HasKey(e => e.BranchCode);
                entity.Property(e => e.BranchCode).HasColumnName("BRANCH_CODE");
                entity.Property(e => e.BranchDesc).HasColumnName("BRANCH_DESC");
            });
            modelBuilder.Entity<EmpEgx>(entity =>
            {
                entity.ToTable("EMP_EGX");
                entity.HasKey(e => new { e.DepCode, e.EmpCode }); // Composite Key
                entity.Property(e => e.DepCode).HasColumnName("DEP_CODE");
                entity.Property(e => e.EmpCode).HasColumnName("EMP_CODE");
                entity.Property(e => e.EmpName).HasColumnName("EMP_NAME");
                entity.Property(e => e.BranchCode).HasColumnName("BRANCH_CODE");
            });
            modelBuilder.Entity<HInvTrans>(entity =>
            {
                entity.ToTable("H_INV_TRANS", "KWAREHOUSE"); // تحديد اسم الجدول والـ Schema

                // تعريف المفتاح المركب (Composite Key)
                entity.HasKey(e => new { e.StoreCode, e.TrType, e.TrDate, e.TrNum, e.TrSerial, e.ItemCode });

                // ربط الخصائص بأسماء الأعمدة الحقيقية في Oracle
                entity.Property(e => e.StoreCode).HasColumnName("STORE_CODE");
                entity.Property(e => e.TrType).HasColumnName("TR_TYPE");
                entity.Property(e => e.TrDate).HasColumnName("TR_DATE");
                entity.Property(e => e.TrNum).HasColumnName("TR_NUM");
                entity.Property(e => e.TrSerial).HasColumnName("TR_SERIAL");
                entity.Property(e => e.ItemCode).HasColumnName("ITEM_CODE");
                entity.Property(e => e.DepCode).HasColumnName("DEP_CODE");
                entity.Property(e => e.EmpCode).HasColumnName("EMP_CODE");
                entity.Property(e => e.SuplierCode).HasColumnName("SUPLIER_CODE");
                entity.Property(e => e.FromToStore).HasColumnName("FROM_TO_STORE");
                entity.Property(e => e.ItemQnt).HasColumnName("ITEM_QNT");
                entity.Property(e => e.ItemPrice).HasColumnName("ITEM_PRICE");
                entity.Property(e => e.BillNum).HasColumnName("BILL_NUM");
                entity.Property(e => e.TrNum2).HasColumnName("TR_NUM2"); // هذا هو السطر الذي حل مشكلة الخطأ الأخير
                entity.Property(e => e.TrDate2).HasColumnName("TR_DATE2");
                entity.Property(e => e.OrderDate).HasColumnName("ORDER_DATE");
                entity.Property(e => e.DeliverNo).HasColumnName("DELIVER_NO");
                entity.Property(e => e.DeliverDate).HasColumnName("DELIVER_DATE");

                // حل مشكلة المفتاح المركب لجدول الأصناف (كما فعلنا سابقاً)
                entity.HasOne(d => d.Item)
                      .WithMany()
                      .HasForeignKey(d => d.ItemCode)
                      .HasPrincipalKey(i => i.ItemCode);

                entity.HasOne(d => d.Supplier)
                      .WithMany()
                      .HasForeignKey(d => d.SuplierCode);
            });
            modelBuilder.Entity<InvTrans>(entity =>
            {
                entity.ToTable("INV_TRANS");
                entity.HasKey(e => new { e.StoreCode, e.TrType, e.TrDate, e.TrNum, e.TrSerial, e.ItemCode });
                entity.Property(e => e.StoreCode).HasColumnName("STORE_CODE");
                entity.Property(e => e.TrType).HasColumnName("TR_TYPE");
                entity.Property(e => e.TrDate).HasColumnName("TR_DATE");
                entity.Property(e => e.TrNum).HasColumnName("TR_NUM");
                entity.Property(e => e.TrSerial).HasColumnName("TR_SERIAL");
                entity.Property(e => e.ItemCode).HasColumnName("ITEM_CODE");
                entity.Property(e => e.DepCode).HasColumnName("DEP_CODE");
                entity.Property(e => e.EmpCode).HasColumnName("EMP_CODE");
                entity.Property(e => e.SuplierCode).HasColumnName("SUPLIER_CODE");
                entity.Property(e => e.FromToStore).HasColumnName("FROM_TO_STORE");
                entity.Property(e => e.ItemQnt).HasColumnName("ITEM_QNT");
                entity.Property(e => e.ItemPrice).HasColumnName("ITEM_PRICE");
                entity.Property(e => e.BillNum).HasColumnName("BILL_NUM");
                entity.Property(e => e.TrNum2).HasColumnName("TR_NUM2");
                entity.Property(e => e.TrDate2).HasColumnName("TR_DATE2");
                entity.Property(e => e.OrderDate).HasColumnName("ORDER_DATE");
                entity.Property(e => e.DeliverNo).HasColumnName("DELIVER_NO");
                entity.Property(e => e.DeliverDate).HasColumnName("DELIVER_DATE");
                entity.HasOne(d => d.Item)
                       .WithMany() // أو .WithMany(p => p.InvTrans) إذا كانت العلاقة معرفة في الطرفين
                       .HasForeignKey(d => d.ItemCode)
                       .HasPrincipalKey(p => p.ItemCode); // تأكد أن هذا هو اسم المفتاح في جدول الأصناف

                // ربط جدول الموردين
                entity.HasOne(d => d.Supplier)
                      .WithMany()
                      .HasForeignKey(d => d.SuplierCode)
                      .HasPrincipalKey(p => p.SuplierCode);
                entity.HasOne(d => d.Department)
                  .WithMany()
                  .HasForeignKey(d => d.DepCode);

                entity.HasOne(d => d.Employee)
                      .WithMany()
                      .HasForeignKey(d => new { d.DepCode, d.EmpCode });


            });



            modelBuilder.Entity<InvUser>(entity =>
            {
                entity.ToTable("INV_USERS");
                entity.HasKey(e => e.UserCode);

                // --- THE RELATIONSHIP DEFINITION ---
                entity.HasOne(u => u.Employee)           // InvUser has one Employee
                      .WithMany()                         // EmpEgx doesn't necessarily need a list of Users
                      .HasForeignKey(u => u.UserCode)    // The "FK" is UserCode in INV_USERS
                      .HasPrincipalKey(e => e.EmpCode);  // Join it to EmpCode in EMP_EGX
                                                         // ------------------------------------
                entity.Property(e => e.UserCode).HasColumnName("USER_CODE");
                entity.Property(e => e.UserName).HasColumnName("USER_NAME");
                entity.Property(e => e.UserPasswd).HasColumnName("USER_PASSWD");
                entity.Property(e => e.Prog01).HasColumnName("PROG01");
                entity.Property(e => e.Prog02).HasColumnName("PROG02");
                entity.Property(e => e.Prog03).HasColumnName("PROG03");
                entity.Property(e => e.Prog11).HasColumnName("PROG11");
                entity.Property(e => e.Prog12).HasColumnName("PROG12");
                entity.Property(e => e.Prog13).HasColumnName("PROG13");
                entity.Property(e => e.Prog14).HasColumnName("PROG14");
                entity.Property(e => e.Prog21).HasColumnName("PROG21");
                entity.Property(e => e.Prog22).HasColumnName("PROG22");
                entity.Property(e => e.Prog23).HasColumnName("PROG23");
                entity.Property(e => e.Prog24).HasColumnName("PROG24");
                entity.Property(e => e.Prog25).HasColumnName("PROG25");
                entity.Property(e => e.Prog31).HasColumnName("PROG31");
                entity.Property(e => e.Prog32).HasColumnName("PROG32");
                entity.Property(e => e.Prog33).HasColumnName("PROG33");
                entity.Property(e => e.Prog34).HasColumnName("PROG34");
                entity.Property(e => e.Prog35).HasColumnName("PROG35");
                entity.Property(e => e.Prog29).HasColumnName("PROG29");
            });
            modelBuilder.Entity<Item>(entity =>
            {
                entity.ToTable("ITEMS");
                entity.HasKey(e => new { e.CatgryCode, e.ItemCode });
                entity.Property(e => e.ItemCode).HasColumnName("ITEM_CODE").HasMaxLength(5).HasDefaultValueSql("NULL");
                entity.Property(e => e.CatgryCode).HasColumnName("CATGRY_CODE").HasMaxLength(2);
                entity.Property(e => e.ItemDesc).HasColumnName("ITEM_DESC");
                entity.Property(e => e.RecallPrc).HasColumnName("RECALL_PRC");
                entity.Property(e => e.RecallQnt).HasColumnName("RECALL_QNT");
                entity.Property(e => e.Barecode).HasColumnName("BARECODE");
                entity.HasOne(d => d.ItemCategory)      // الصنف له تصنيف واحد
                      .WithMany(p => p.Items)           // التصنيف له أصناف كثيرة
                      .HasForeignKey(d => d.CatgryCode) // المفتاح الأجنبي هو CatgryCode
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ItemBalance
            modelBuilder.Entity<ItemBalance>(entity =>
            {
                entity.ToTable("ITEM_BALANCE");
                entity.HasKey(e => new { e.StoreCode, e.BalDate, e.ItemCode });
                entity.Property(e => e.StoreCode).HasColumnName("STORE_CODE");
                entity.Property(e => e.BalDate).HasColumnName("BAL_DATE");
                entity.Property(e => e.ItemCode).HasColumnName("ITEM_CODE");
                entity.Property(e => e.OpenBal).HasColumnName("OPEN_BAL");
                entity.Property(e => e.ItemIn).HasColumnName("ITEM_IN");
                entity.Property(e => e.ItemOut).HasColumnName("ITEM_OUT");
                entity.Property(e => e.ItemFrom).HasColumnName("ITEM_FROM");
                entity.Property(e => e.ItemTo).HasColumnName("ITEM_TO");
                entity.Property(e => e.ItemBack).HasColumnName("ITEM_BACK");
                entity.Property(e => e.CurrentBal).HasColumnName("CURRENT_BAL");
                entity.Property(e => e.ItemBack2).HasColumnName("ITEM_BACK2");
                entity.Property(e => e.ItemScrap).HasColumnName("ITEM_SCRAP");
            });

            modelBuilder.Entity<ItemCard>(entity =>
            {
                entity.ToTable("ITEM_CARD");
                entity.HasKey(e => new { e.StoreCode, e.ItemCode, e.CardSerial });
                entity.Property(e => e.StoreCode).HasColumnName("STORE_CODE");
                entity.Property(e => e.ItemCode).HasColumnName("ITEM_CODE");
                entity.Property(e => e.ItemDesc).HasColumnName("ITEM_DESC");
                entity.Property(e => e.CardDate).HasColumnName("CARD_DATE");
                entity.Property(e => e.CardMemo).HasColumnName("CARD_MEMO");
                entity.Property(e => e.InNum).HasColumnName("IN_NUM");
                entity.Property(e => e.InQnt).HasColumnName("IN_QNT");
                entity.Property(e => e.InPrice).HasColumnName("IN_PRICE");
                entity.Property(e => e.OutNum).HasColumnName("OUT_NUM");
                entity.Property(e => e.OutQnt).HasColumnName("OUT_QNT");
                entity.Property(e => e.CardBalance).HasColumnName("CARD_BALANCE");
                entity.Property(e => e.CardSerial).HasColumnName("CARD_SERIAL");
            });

            // ITEM_CATEGORY
            modelBuilder.Entity<ItemCategory>(entity =>
            {
                entity.ToTable("ITEM_CATEGORY");
                entity.HasKey(e => e.CatgryCode);
                entity.Property(e => e.CatgryCode).HasColumnName("CATGRY_CODE");
                entity.Property(e => e.CatgryDesc).HasColumnName("CATGRY_DESC");
            });

            // MONTHLY_BALANCE
            modelBuilder.Entity<MonthlyBalance>(entity =>
            {
                entity.ToTable("MONTHLY_BALANCE");
                entity.HasKey(e => new { e.StoreCode, e.BalYear, e.BalMonth, e.ItemCode });
                entity.Property(e => e.StoreCode).HasColumnName("STORE_CODE");
                entity.Property(e => e.BalYear).HasColumnName("BAL_YEAR");
                entity.Property(e => e.BalMonth).HasColumnName("BAL_MONTH");
                entity.Property(e => e.ItemCode).HasColumnName("ITEM_CODE");
                entity.Property(e => e.OpenBal).HasColumnName("OPEN_BAL");
                entity.Property(e => e.ItemIn).HasColumnName("ITEM_IN");
                entity.Property(e => e.ItemOut).HasColumnName("ITEM_OUT");
                entity.Property(e => e.ItemFrom).HasColumnName("ITEM_FROM");
                entity.Property(e => e.ItemTo).HasColumnName("ITEM_TO");
                entity.Property(e => e.ItemBack).HasColumnName("ITEM_BACK");
                entity.Property(e => e.CurrentBal).HasColumnName("CURRENT_BAL");
                entity.Property(e => e.ItemBack2).HasColumnName("ITEM_BACK2");
                entity.Property(e => e.ItemScrap).HasColumnName("ITEM_SCRAP");
            });

            // MONTHLY_CONSUM
            modelBuilder.Entity<MonthlyConsum>(entity =>
            {
                entity.ToTable("MONTHLY_CONSUM");
                entity.HasKey(e => new { e.StoreCode, e.ConsumYear, e.ConsumMonth, e.DepCode, e.ItemCode });
                entity.Property(e => e.StoreCode).HasColumnName("STORE_CODE");
                entity.Property(e => e.ConsumYear).HasColumnName("CONSUM_YEAR");
                entity.Property(e => e.ConsumMonth).HasColumnName("CONSUM_MONTH");
                entity.Property(e => e.DepCode).HasColumnName("DEP_CODE");
                entity.Property(e => e.ItemCode).HasColumnName("ITEM_CODE");
                entity.Property(e => e.ConsumQnt).HasColumnName("CONSUM_QNT");
                entity.Property(e => e.ConsumAvg).HasColumnName("CONSUM_AVG");
            });

            // OPEN_BALANCE
            modelBuilder.Entity<OpenBalance>(entity =>
            {
                entity.ToTable("OPEN_BALANCE");
                entity.HasKey(e => new { e.StoreCode, e.OpenDate, e.ItemCode });
                entity.Property(e => e.StoreCode).HasColumnName("STORE_CODE");
                entity.Property(e => e.OpenDate).HasColumnName("OPEN_DATE");
                entity.Property(e => e.ItemCode).HasColumnName("ITEM_CODE");
                entity.Property(e => e.OpenBal).HasColumnName("OPEN_BAL");
                entity.Property(e => e.RelayFlag).HasColumnName("RELAY_FLAG");
            });

            // STORES
            modelBuilder.Entity<Store>(entity =>
            {
                entity.ToTable("STORES");
                entity.HasKey(e => e.StoreCode);
                entity.Property(e => e.StoreCode).HasColumnName("STORE_CODE");
                entity.Property(e => e.StoreDesc).HasColumnName("STORE_DESC");
                entity.Property(e => e.SysDate).HasColumnName("SYS_DATE");
                entity.Property(e => e.InNum).HasColumnName("IN_NUM");
                entity.Property(e => e.OutNum).HasColumnName("OUT_NUM");
                entity.Property(e => e.ToNum).HasColumnName("TO_NUM");
                entity.Property(e => e.BackNum).HasColumnName("BACK_NUM");
                entity.Property(e => e.SysLock).HasColumnName("SYS_LOCK");
                entity.Property(e => e.BackNum2).HasColumnName("BACK_NUM2");
                entity.Property(e => e.ScrapNum).HasColumnName("SCRAP_NUM");
            });

            // SUPPLIER
            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.ToTable("SUPPLIER");
                entity.HasKey(e => e.SuplierCode);
                entity.Property(e => e.SuplierCode).HasColumnName("SUPLIER_CODE");
                entity.Property(e => e.SuplierDesc).HasColumnName("SUPLIER_DESC");
                entity.Property(e => e.SuplierAddress).HasColumnName("SUPLIER_ADDRESS");
                entity.Property(e => e.SuplierTel).HasColumnName("SUPLIER_TEL");
                entity.Property(e => e.SuplierFax).HasColumnName("SUPLIER_FAX");
                entity.Property(e => e.SuplierEmail).HasColumnName("SUPLIER_EMAIL");
                entity.Property(e => e.SuplierActivity).HasColumnName("SUPLIER_ACTIVITY");
            });

            // SYS_MESSG
            modelBuilder.Entity<SysMessg>(entity =>
            {
                entity.ToTable("SYS_MESSG");
                entity.HasKey(e => e.MsgCode);
                entity.Property(e => e.MsgCode).HasColumnName("MSG_CODE");
                entity.Property(e => e.MsgDesc).HasColumnName("MSG_DESC");
            });

            modelBuilder.HasSequence<long>("SEQ_ITEM_CATEGORY_CODE");

            modelBuilder.Entity<ItemCategory>(entity =>
            {
                entity.HasKey(e => e.CatgryCode);

                // Tell EF to use the sequence for this property
                entity.Property(e => e.CatgryCode)
                      .HasDefaultValueSql("SEQ_ITEM_CATEGORY_CODE.NEXTVAL")
                      .ValueGeneratedOnAdd();
            });
        }

    }
}
