using AutoMapper;
using Core.Auth;
using DataTransferObject.AuthDTO.Request;
using DataTransferObject.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Helper
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterRequest, User>();
            CreateMap<AddUserByAdminRequest, User>();
            CreateMap<EditUserByAdminRequest, User>();
        }
    }
}
