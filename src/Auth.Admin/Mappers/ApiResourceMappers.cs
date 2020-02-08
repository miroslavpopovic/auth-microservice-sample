using Auth.Admin.Models;
using AutoMapper;
using IdentityServer4.EntityFramework.Entities;

namespace Auth.Admin.Mappers
{
    public static class ApiResourceMappers
    {
        static ApiResourceMappers()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<ApiResourceMapperProfile>())
                .CreateMapper();
        }

        private static IMapper Mapper { get; }

        public static ApiResourceModel ToModel(this ApiResource resource)
        {
            return resource == null ? null : Mapper.Map<ApiResourceModel>(resource);
        }

        public static ApiResource ToEntity(this ApiResourceModel model)
        {
            return model == null ? null : Mapper.Map<ApiResource>(model);
        }

        public static void ToEntity(this ApiResourceModel model, ApiResource apiResource)
        {
            Mapper.Map(model, apiResource);
        }
    }
}
