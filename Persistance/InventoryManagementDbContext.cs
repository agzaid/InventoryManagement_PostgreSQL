using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistance
{
    public class InventoryManagementDbContext : AuditableDbContext
    {
        public InventoryManagementDbContext(DbContextOptions<InventoryManagementDbContext> options) : base(options)
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


            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("DEPARTMENTS", "KWAREHOUSE"); 

                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.DepCode)
                    .HasColumnName("dep_code")
                    .IsRequired();
                entity.HasIndex(e => e.DepCode).IsUnique();

                entity.Property(e => e.DepDesc).HasColumnName("dep_desc");
                entity.Property(e => e.SectorCode).HasColumnName("sector_code");
                entity.Property(e => e.GeneralManeg).HasColumnName("general_maneg");
                entity.Property(e => e.ManegSupCode).HasColumnName("maneg_supcode");
            });

            modelBuilder.Entity<Branch>(entity =>
            {
                entity.ToTable("branches", "kwarehouse");

                // Map the C# 'Id' property to the database 'id' column
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd(); // This confirms it's the Identity column

                entity.HasKey(e => e.Id);

                entity.Property(e => e.BranchCode)
                    .HasColumnName("branch_code") // Recommendation: Specify length or use "text"
                    .IsRequired();

                entity.HasIndex(e => e.BranchCode).IsUnique();

                entity.Property(e => e.BranchDesc)
                    .HasColumnName("branch_desc");
            });
            modelBuilder.Entity<EmpEgx>(entity =>
            {
                entity.ToTable("EMP_EGX", "KWAREHOUSE"); // Ensure schema is consistent

                // 1. Set the new 'Id' as the Primary Key
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd(); 

                entity.HasIndex(e => new { e.DepCode, e.EmpCode })
                    .IsUnique();

                entity.Property(e => e.DepCode).HasColumnName("dep_code");
                entity.Property(e => e.EmpCode).HasColumnName("emp_code");
                entity.Property(e => e.EmpName).HasColumnName("emp_name");
                entity.Property(e => e.BranchCode).HasColumnName("branch_code");
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
                entity.ToTable("INV_TRANS", "KWAREHOUSE");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.HasIndex(e => new { e.StoreCode, e.TrType, e.TrDate, e.TrNum, e.TrSerial, e.ItemCode })
                    .IsUnique();

                // ... (Keep your property mappings as they are) ...

                // 1. CLEAN RELATIONSHIP: Link using the new EmployeeId
                entity.HasOne(d => d.Employee)
                      .WithMany()
                      .HasForeignKey(d => d.EmployeeId)
                      .OnDelete(DeleteBehavior.Restrict); // Recommended for safety

                // 2. REMOVE the old composite relationship block completely
                // DELETE THESE LINES:
                // entity.HasOne(d => d.Employee)
                //       .WithMany()
                //       .HasForeignKey(d => new { d.DepCode, d.EmpCode });

                // Keep other relationships
                entity.HasOne(d => d.Item)
                      .WithMany()
                      .HasForeignKey(d => d.ItemCode)
                      .HasPrincipalKey(p => p.ItemCode);

                entity.HasOne(d => d.Supplier)
                      .WithMany()
                      .HasForeignKey(d => d.SuplierCode)
                      .HasPrincipalKey(p => p.SuplierCode);

                entity.HasOne(d => d.Department)
                      .WithMany()
                      .HasForeignKey(d => d.DepCode)
                      .HasPrincipalKey(p => p.DepCode);
            });



            modelBuilder.Entity<InvUser>(entity =>
            {
                entity.ToTable("INV_USERS");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.HasIndex(e => e.UserCode)
                    .IsUnique();

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
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.HasIndex(e => new { e.CatgryCode, e.ItemCode })
                    .IsUnique();

                entity.Property(e => e.ItemCode).HasColumnName("ITEM_CODE").HasMaxLength(5).HasDefaultValueSql("NULL");
                entity.Property(e => e.CatgryCode).HasColumnName("CATGRY_CODE").HasMaxLength(2);
                entity.Property(e => e.ItemDesc).HasColumnName("ITEM_DESC");
                entity.Property(e => e.RecallPrc).HasColumnName("RECALL_PRC");
                entity.Property(e => e.RecallQnt).HasColumnName("RECALL_QNT");
                entity.Property(e => e.Barecode).HasColumnName("BARECODE");
                entity.HasOne(d => d.ItemCategory)      // الصنف له تصنيف واحد
                      .WithMany(p => p.Items)           // التصنيف له أصناف كثيرة
                      .HasForeignKey(d => d.CatgryCode) // المفتاح الأجنبي هو CatgryCode
                      .HasPrincipalKey(p => p.CatgryCode) // Join it to CatgryCode in ItemCategory
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ItemBalance
            modelBuilder.Entity<ItemBalance>(entity =>
            {
                entity.ToTable("ITEM_BALANCE");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.HasIndex(e => new { e.StoreCode, e.BalDate, e.ItemCode })
                    .IsUnique();

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
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.HasIndex(e => new { e.StoreCode, e.ItemCode, e.CardSerial })
                    .IsUnique();

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

            // 1. Define the sequence again
            modelBuilder.HasSequence<int>("seq_item_category_code")
                        .StartsAt(1)
                        .IncrementsBy(1);

            modelBuilder.Entity<ItemCategory>(entity =>
            {
                entity.ToTable("item_category"); // Ensure this matches your actual table name
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.HasIndex(e => e.CatgryCode)
                    .IsUnique();

                entity.Property(e => e.CatgryCode)
                .HasColumnName("catgry_code");

                // Add this line specifically to map the description column correctly
                entity.Property(e => e.CatgryDesc)
                      .HasColumnName("catgry_desc");
            });

            // MONTHLY_BALANCE
            modelBuilder.Entity<MonthlyBalance>(entity =>
            {
                entity.ToTable("MONTHLY_BALANCE");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.HasIndex(e => new { e.StoreCode, e.BalYear, e.BalMonth, e.ItemCode })
                    .IsUnique();

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
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.HasIndex(e => new { e.StoreCode, e.ConsumYear, e.ConsumMonth, e.DepCode, e.ItemCode })
                    .IsUnique();

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
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.HasIndex(e => new { e.StoreCode, e.OpenDate, e.ItemCode })
                    .IsUnique();

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
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.HasIndex(e => e.StoreCode)
                    .IsUnique();

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
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.HasIndex(e => e.SuplierCode)
                    .IsUnique();

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

           

            modelBuilder.HasDefaultSchema("KWAREHOUSE");

            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                // Lowercase the Table Name
                var tableName = entity.GetTableName();
                if (!string.IsNullOrEmpty(tableName))
                    entity.SetTableName(tableName.ToLower());

                // Lowercase the Schema Name
                var schema = entity.GetSchema();
                if (!string.IsNullOrEmpty(schema))
                    entity.SetSchema(schema.ToLower());

                // Lowercase all Column Names
                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(property.GetColumnName().ToLower());
                }
            }
        }

    }
}
