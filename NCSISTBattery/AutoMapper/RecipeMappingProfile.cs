using AutoMapper;
using NCSISTBattery.EFModel;
using SharedMod;

namespace NCSISTBattery.AutoMapper
{
    public class RecipeMappingProfile : Profile
    {
        public RecipeMappingProfile()
        {
            CreateMap<CalculationData, CalculationDataDTO>()
                .ForMember(dest => dest.RecipeDTO, opt => opt.MapFrom(src => src.Recipe))
                .ForMember(dest => dest.JigsDTOs, opt => opt.MapFrom(src => src.Jigs));



            CreateMap<Recipe, RecipeDTO>()
                            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                            .ForMember(dest => dest.RecipeContentDTOs, opt => opt.MapFrom(src => src.RecipeContents.OrderBy(x => x.Sequence)));
            CreateMap<RecipeContent, RecipeContentDTO>()
                .ForMember(dest => dest.MaterialName,
                           opt => opt.MapFrom(src => src.Material.Name))
                .ForMember(dest => dest.MaterialCode,
                           opt => opt.MapFrom(src => src.Material.TypeCode))
                .ForMember(dest => dest.Sequence,
                           opt => opt.MapFrom(src => src.Sequence));

            CreateMap<Material, MaterialDTO>()
                            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                            .ForMember(dest => dest.TypeCode, opt => opt.MapFrom(src => src.TypeCode));


            CreateMap<Jig, JigDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.AreaCode, opt => opt.MapFrom(src => src.AreaCode))
                .ForMember(dest => dest.MaterialFree, opt => opt.MapFrom(src => src.MaterialFree))
                .ForMember(dest => dest.MaterialTypeCode, opt => opt.MapFrom(src => src.MaterialTypeCode))
                .ForMember(dest => dest.JigContentDTOs, opt => opt.MapFrom(src => src.JigContent));
            CreateMap<JigContent, JigContentDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.MaterialCode, opt => opt.MapFrom(src => src.MaterialCode))
                .ForMember(dest => dest.Heat, opt => opt.MapFrom(src => src.Heat))
                .ForMember(dest => dest.Thickness, opt => opt.MapFrom(src => src.Thickness));






        }

    }
}
