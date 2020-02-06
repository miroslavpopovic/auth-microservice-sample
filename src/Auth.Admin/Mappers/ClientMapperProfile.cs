using System;
using System.Collections.Generic;
using System.Linq;
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

            CreateMap<List<ClientRedirectUri>, string>()
                .ConstructUsing(src => string.Join('\n', src.Select(u => u.RedirectUri)))
                .ReverseMap()
                .ConstructUsing(src =>
                    src.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => new ClientRedirectUri {RedirectUri = x}).ToList());

            CreateMap<List<ClientPostLogoutRedirectUri>, string>()
                .ConstructUsing(src => string.Join('\n', src.Select(u => u.PostLogoutRedirectUri)))
                .ReverseMap()
                .ConstructUsing(src =>
                    src.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => new ClientPostLogoutRedirectUri {PostLogoutRedirectUri = x}).ToList());

            CreateMap<List<ClientIdPRestriction>, string>()
                .ConstructUsing(src => string.Join('\n', src.Select(u => u.Provider)))
                .ReverseMap()
                .ConstructUsing(src =>
                    src.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => new ClientIdPRestriction {Provider = x}).ToList());

            CreateMap<List<ClientCorsOrigin>, string>()
                .ConstructUsing(src => string.Join('\n', src.Select(u => u.Origin)))
                .ReverseMap()
                .ConstructUsing(src =>
                    src.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => new ClientCorsOrigin {Origin = x}).ToList());

            //CreateMap<ClientRedirectUri, string>()
            //    .ConstructUsing(src => src.RedirectUri)
            //    .ReverseMap()
            //    .ForMember(dest => dest.RedirectUri, opt => opt.MapFrom(src => src));

            //CreateMap<ClientPostLogoutRedirectUri, string>()
            //    .ConstructUsing(src => src.PostLogoutRedirectUri)
            //    .ReverseMap()
            //    .ForMember(dest => dest.PostLogoutRedirectUri, opt => opt.MapFrom(src => src));

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

            //CreateMap<ClientIdPRestriction, string>()
            //    .ConstructUsing(src => src.Provider)
            //    .ReverseMap()
            //    .ForMember(dest => dest.Provider, opt => opt.MapFrom(src => src));

            //CreateMap<ClientCorsOrigin, string>()
            //    .ConstructUsing(src => src.Origin)
            //    .ReverseMap()
            //    .ForMember(dest => dest.Origin, opt => opt.MapFrom(src => src));

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
