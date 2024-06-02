using Autofac;
using BusinessLogic.Services.Auth;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using RoboGas.Core.Interfaces;
using RoboGas.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class InfrastructureModule:Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>()
                .InstancePerLifetimeScope();

        }
    }
}
