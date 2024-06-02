using Autofac;
using AutoMapper;
using BusinessLogic.Services.Auth;
using BusinessLogic.Services.Services;
using Core.Interfaces;
using BusinessLogic.MiddleWares;

namespace BusinessLogic
{
    public class BusinessLogicModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AuthService>().As<IAuthService>()
                .InstancePerLifetimeScope(); 

            builder.RegisterType<RegionService>().As<IRegionService>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ValidateToken>();

        }
    }
}
