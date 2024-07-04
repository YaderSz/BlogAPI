using SharedModels;
using SharedModels.Dto;
using AutoMapper;
namespace BlogAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig() {

            CreateMap<Autor, AutorDto>().ReverseMap();
            CreateMap<Autor, AutorCreateDto>().ReverseMap();
            CreateMap<Autor, AutorUpdateDto>().ReverseMap();
            CreateMap<Publicacion, PublicacionDto>().ReverseMap();
            CreateMap<Publicacion, PublicacionCreateDto>().ReverseMap();
            CreateMap<Publicacion, PublicacionUpdateDto>().ReverseMap();

        }
    }
}
