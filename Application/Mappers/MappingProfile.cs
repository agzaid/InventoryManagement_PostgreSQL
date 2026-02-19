using Application.Interfaces.Models;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // From Oracle Entity to DTO
            CreateMap<InvUser, InvUserDto>().
                ForMember(dest => dest.FullNameArabic,
                       opt => opt.MapFrom(src => src.Employee != null ? src.Employee.EmpName : string.Empty))
                .ReverseMap(); // Allows mapping DTO back to Entity

            CreateMap<Department, DepartmentDto>().ReverseMap();
            CreateMap<EmpEgx, EgxEmployeeDto>().ReverseMap();
            CreateMap<Store, StoreDto>().ReverseMap();
            CreateMap<InvTrans, InvTransDto>().ReverseMap();
            CreateMap<InvTrans, HInvTrans>();
            CreateMap<HInvTrans, HInvTransDto>().ReverseMap();
            CreateMap<ItemBalance, ItemBalanceDto>().ReverseMap();
            CreateMap<MonthlyConsum, MonthlyConsumDto>().ReverseMap();
            CreateMap<MonthlyBalance, MonthlyBalanceDto>().ReverseMap();
            CreateMap<ItemCategory, ItemCategoryDto>().ReverseMap();

            CreateMap<Item, ItemDto>()
                     .ForMember(dest => dest.CatgryCode, opt => opt.MapFrom(src => src.ItemCategory.CatgryCode))
                     // جلب الوصف النصي من جدول التصنيفات المرتبط
                     //.ForMember(dest => dest.CatgryDesc, opt => opt.MapFrom(src => src.ItemCategory.CatgryDesc))
                     .ReverseMap();

            // 2. ماب للتصنيفات إذا احتجت عرضها منفصلة
            CreateMap<ItemCategory, CategoryDto>().ReverseMap();
            CreateMap<Supplier, SupplierDto>().ReverseMap();
        }
    }
}
