using Auth.Admin.Models;
using AutoMapper;
using IdentityServer4.EntityFramework.Entities;

namespace Auth.Admin.Mappers
{
    public class ClientMapperProfile : Profile
    {
        public ClientMapperProfile()
        {
            CreateMap<Client, ClientModel>(MemberList.Destination)
                .ForMember(dest => dest.ProtocolType, opt => opt.Condition(srs => srs != null))
                .ReverseMap();

            //CreateMap<SelectItem, SelectItemDto>(MemberList.Destination)
            //    .ReverseMap();

            CreateMap<ClientGrantType, string>()
                .ConstructUsing(src => src.GrantType)
                .ReverseMap()
                .ForMember(dest => dest.GrantType, opt => opt.MapFrom(src => src));

            CreateMap<ClientRedirectUri, string>()
                .ConstructUsing(src => src.RedirectUri)
                .ReverseMap()
                .ForMember(dest => dest.RedirectUri, opt => opt.MapFrom(src => src));

            CreateMap<ClientPostLogoutRedirectUri, string>()
                .ConstructUsing(src => src.PostLogoutRedirectUri)
                .ReverseMap()
                .ForMember(dest => dest.PostLogoutRedirectUri, opt => opt.MapFrom(src => src));

            CreateMap<ClientScope, string>()
                .ConstructUsing(src => src.Scope)
                .ReverseMap()
                .ForMember(dest => dest.Scope, opt => opt.MapFrom(src => src));

            CreateMap<ClientSecret, ClientSecretModel>(MemberList.Destination)
                .ForMember(dest => dest.Type, opt => opt.Condition(srs => srs != null))
                .ReverseMap();

            CreateMap<ClientClaim, ClientClaimModel>(MemberList.None)
                .ConstructUsing(src => new ClientClaimModel { Type = src.Type, Value = src.Value })
                .ReverseMap();

            CreateMap<ClientIdPRestriction, string>()
                .ConstructUsing(src => src.Provider)
                .ReverseMap()
                .ForMember(dest => dest.Provider, opt => opt.MapFrom(src => src));

            CreateMap<ClientCorsOrigin, string>()
                .ConstructUsing(src => src.Origin)
                .ReverseMap()
                .ForMember(dest => dest.Origin, opt => opt.MapFrom(src => src));

            CreateMap<ClientProperty, ClientProperty>(MemberList.Destination)
                .ReverseMap();

            ////PagedLists
            //CreateMap<PagedList<ClientSecret>, ClientSecretsDto>(MemberList.Destination)
            //    .ForMember(x => x.ClientSecrets, opt => opt.MapFrom(src => src.Data));

            //CreateMap<PagedList<ClientClaim>, ClientClaimsDto>(MemberList.Destination)
            //    .ForMember(x => x.ClientClaims, opt => opt.MapFrom(src => src.Data));

            //CreateMap<PagedList<ClientProperty>, ClientPropertiesDto>(MemberList.Destination)
            //    .ForMember(x => x.ClientProperties, opt => opt.MapFrom(src => src.Data));

            //CreateMap<PagedList<Client>, ClientsDto>(MemberList.Destination)
            //    .ForMember(x => x.Clients, opt => opt.MapFrom(src => src.Data));
        }
    }
}
