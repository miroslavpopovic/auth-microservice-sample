using System.Collections.Generic;
using Auth.Admin.Models;
using AutoMapper;
using IdentityServer4.EntityFramework.Entities;

namespace Auth.Admin.Mappers
{
    public static class IdentityResourceMappers
    {
        static IdentityResourceMappers()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<IdentityResourceMapperProfile>())
                .CreateMapper();
        }

        private static IMapper Mapper { get; }


        public static IdentityResourceModel ToModel(this IdentityResource resource)
        {
            return resource == null ? null : Mapper.Map<IdentityResourceModel>(resource);
        }

        public static List<IdentityResourceModel> ToModel(this List<IdentityResource> resource)
        {
            return resource == null ? null : Mapper.Map<List<IdentityResourceModel>>(resource);
        }

        public static IdentityResource ToEntity(this IdentityResourceModel resource)
        {
            return resource == null ? null : Mapper.Map<IdentityResource>(resource);
        }

        public static List<IdentityResource> ToEntity(this List<IdentityResourceModel> resource)
        {
            return resource == null ? null : Mapper.Map<List<IdentityResource>>(resource);
        }
    }
}
