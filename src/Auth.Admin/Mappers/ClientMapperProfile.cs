using System;
using System.Collections.Generic;
using System.Linq;
using Auth.Admin.Extensions;
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
                .ReverseMap()
                .ForMember(
                    x => x.RedirectUris,
                    o => o.MapFrom(x => x.RedirectUris.ToList<ClientRedirectUri>(item => item.RedirectUri)))
                .ForMember(
                    x => x.PostLogoutRedirectUris,
                    o => o.MapFrom(
                        x => x.PostLogoutRedirectUris.ToList<ClientPostLogoutRedirectUri>(
                            item => item.PostLogoutRedirectUri)))
                .ForMember(
                    x => x.IdentityProviderRestrictions,
                    o => o.MapFrom(
                        x => x.IdentityProviderRestrictions.ToList<ClientIdPRestriction>(item => item.Provider)))
                .ForMember(
                    x => x.AllowedCorsOrigins,
                    o => o.MapFrom(x => x.AllowedCorsOrigins.ToList<ClientCorsOrigin>(item => item.Origin)));


            CreateMap<ClientGrantType, string>()
                .ConstructUsing(src => src.GrantType)
                .ReverseMap()
                .ForMember(dest => dest.GrantType, opt => opt.MapFrom(src => src));

            CreateMap<List<ClientRedirectUri>, string>()
                .ConstructUsing(src => string.Join(Environment.NewLine, src.Select(u => u.RedirectUri)))
                .ReverseMap();

            CreateMap<List<ClientPostLogoutRedirectUri>, string>()
                .ConstructUsing(src => string.Join(Environment.NewLine, src.Select(u => u.PostLogoutRedirectUri)))
                .ReverseMap();

            CreateMap<List<ClientIdPRestriction>, string>()
                .ConstructUsing(src => string.Join(Environment.NewLine, src.Select(u => u.Provider)))
                .ReverseMap();

            CreateMap<List<ClientCorsOrigin>, string>()
                .ConstructUsing(src => string.Join(Environment.NewLine, src.Select(u => u.Origin)))
                .ReverseMap();

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
