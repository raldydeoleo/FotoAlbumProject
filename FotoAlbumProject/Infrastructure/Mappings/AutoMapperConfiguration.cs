using AutoMapper;

namespace PhotoGallery.Infrastructure.Mappings
{
    public class AutoMapperConfiguration
    {
        public static void Configure()
        {
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<DomainToViewModelMappingProfile>();
            });

           /* Mapper.Initialize(cfg => {
                cfg.AddProfile<DomainToViewModelMappingProfile>();
            });*/
        }
    }
}
