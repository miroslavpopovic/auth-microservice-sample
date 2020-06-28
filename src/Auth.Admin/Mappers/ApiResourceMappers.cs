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

        public static ApiResourcePropertyModel ToModel(this ApiResourceProperty property)
        {
            return Mapper.Map<ApiResourcePropertyModel>(property);
        }

        public static ApiResourceScopeModel ToModel(this ApiScope scope)
        {
            return Mapper.Map<ApiResourceScopeModel>(scope);
        }

        // TODO: This is not a good mapping
        public static ApiResourceScopeModel ToModel(this ApiResourceScope scope)
        {
            return Mapper.Map<ApiResourceScopeModel>(scope);
        }

        public static ApiResourceSecretModel ToModel(this ApiResourceSecret secret)
        {
            return Mapper.Map<ApiResourceSecretModel>(secret);
        }

        public static ApiResource ToEntity(this ApiResourceModel model)
        {
            return model == null ? null : Mapper.Map<ApiResource>(model);
        }

        public static void ToEntity(this ApiResourceModel model, ApiResource apiResource)
        {
            Mapper.Map(model, apiResource);
        }

        public static ApiScope ToEntity(this ApiResourceScopeModel model)
        {
            return Mapper.Map<ApiScope>(model);
        }

        public static void ToEntity(this ApiResourceScopeModel model, ApiScope scope)
        {
            Mapper.Map(model, scope);
        }
    }
}
