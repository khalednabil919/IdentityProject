using Core.Entities;
using DataTransferObject.Helpers;
using DataTransferObject.RegionDTO.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
    
namespace Core.Interfaces
{
    public interface IRegionService
    {
        public Task<APIResult> Create(Region region);
        public Task<APIResult> Update(Region reg);
        public Task<APIResult> GetAll();
        public Task<APIResult> GetByID(int id);
        public Task<APIResult> Delete(int id);
    }
}
