﻿using Auth.Admin.Models;
using AutoMapper;
using IdentityServer4.EntityFramework.Entities;

namespace Auth.Admin.Mappers
{
    public static class ClientMappers
    {
        static ClientMappers()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<ClientMapperProfile>())
                .CreateMapper();
        }

        internal static IMapper Mapper { get; }

        public static Client ToEntity(this ClientModel model)
        {
            return Mapper.Map<Client>(model);
        }

        public static void ToEntity(this ClientModel model, Client client)
        {
            Mapper.Map(model, client);
        }

        public static ClientModel ToModel(this Client client)
        {
            return Mapper.Map<ClientModel>(client);
        }

        public static ClientPropertyModel ToModel(this ClientProperty clientProperty)
        {
            return Mapper.Map<ClientPropertyModel>(clientProperty);
        }

        public static ClientClaimModel ToModel(this ClientClaim clientClaim)
        {
            return Mapper.Map<ClientClaimModel>(clientClaim);
        }

        public static ClientSecretModel ToModel(this ClientSecret clientSecret)
        {
            return Mapper.Map<ClientSecretModel>(clientSecret);
        }
    }
}
