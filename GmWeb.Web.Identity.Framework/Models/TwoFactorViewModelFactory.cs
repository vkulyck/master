using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BaseProviders = Microsoft.AspNet.Identity;
using BaseIdentities = Microsoft.AspNet.Identity.EntityFramework;
using GmWeb.Logic.Enums;
using GmWeb.Web.Common.Identity;
using GmWeb.Web.Identity.Models.ProviderConfiguration;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using GmWeb.Logic.Interfaces;

namespace GmWeb.Web.Identity.Models
{
    public class TwoFactorViewModelFactory<TUser> : IDisposable
        where TUser : GmIdentity
    {
        public static TwoFactorViewModelFactory<TUser> Create(IdentityFactoryOptions<TwoFactorViewModelFactory<TUser>> options, IOwinContext owinContext)
        {
            return new TwoFactorViewModelFactory<TUser>();
        }

        public IDisposableMapper Mapper => OwinContext.Get<IDisposableMapper>();
        public IOwinContext OwinContext => HttpContext.Current.GetOwinContext();

        public TwoFactorConfigurationViewModel CreateModel(TUser source, InformationType type)
        {
            var intermediate = this.CreateModel(type);
            var srcType = source.GetType();
            var intType = intermediate.GetType();
            var destination = (TwoFactorConfigurationViewModel)Mapper.Map(source, intermediate, srcType, intType);
            return destination;
        }

        public TwoFactorConfigurationViewModel CreateModel(InformationType type)
        {
            switch(type)
            {
                case InformationType.Email:
                    return CreateModel<EmailConfigurationViewModel>();
                case InformationType.SecurityKey:
                    return CreateModel<FidoConfigurationViewModel>();
                case InformationType.GoogleAuthenticator:
                    return CreateModel<GoogleAuthenticatorConfigurationViewModel>();
                case InformationType.Phone:
                    return CreateModel<PhoneConfigurationViewModel>();
                case InformationType.Yubikey:
                    return CreateModel<YubikeyConfigurationViewModel>();
                case InformationType.Password:
                    return CreateModel<PasswordConfigurationViewModel>();
                default:
                    return CreateModel<TwoFactorConfigurationViewModel>();
            }
            throw new NotImplementedException();
        }

        public TModel CreateModel<TModel>()
            where TModel : TwoFactorConfigurationViewModel, new()
        => CreateModel<TModel>(new TwoFactorConfigurationViewModel());
        public TDestination CreateModel<TSource,TDestination>(TSource source)
            where TSource : class, new()
            where TDestination : class, new()
        {
            TDestination model = Mapper.Map<TSource,TDestination>(source);
            switch (source)
            {
                case EmailConfigurationViewModel emailModel:
                    // TODO: specialize as needed
                    return emailModel as TDestination;
                default:
                    return model;
            }
            throw new NotImplementedException();
        }
        public TDestination CreateModel<TDestination>(TwoFactorConfigurationViewModel source)
            where TDestination : class, new()
        {
            TDestination model = Mapper.Map<TDestination>(source);
            switch (source)
            {
                case EmailConfigurationViewModel emailModel:
                    // TODO: specialize as needed
                    return emailModel as TDestination;
                default:
                    return model;
            }
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            
        }
    }
}