using AutoMapper;
using TeklifAlani.Core.DTOs;
using TeklifAlani.Core.DTOs.ApplicationUserDTO;
using TeklifAlani.Core.Identity;
using TeklifAlani.Core.Models;


namespace TeklifAlani.Core.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Supplier, CreateSupplierDTO>().ReverseMap();

            CreateMap<CreateApplicationUserDTO, ApplicationUser>()
                .ForMember(dest => dest.Suppliers, opt => opt.MapFrom(src => src.Suppliers)) // Suppliers listesi eşleme
                .ReverseMap()
                .ForMember(dest => dest.Suppliers, opt => opt.MapFrom(src => src.Suppliers)); // Suppliers listesi eşleme
        }
    }
}
