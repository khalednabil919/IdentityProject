using AutoMapper;
using Core.Auth;
using Core.Entities;
using Core.Interfaces;
using DataTransferObject.Helpers;
using DataTransferObject.RegionDTO.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RoboGas.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.WebRequestMethods;

namespace BusinessLogic.Services.Services
{
    public class RegionService: IRegionService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public RegionService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<APIResult> Create(Region region)
        {
            var reg = _unitOfWork.region.Add(region);
            if (_unitOfWork.Complete() > 0)
                return new APIResult { state = true, entity = reg };

            return new APIResult { state = true, message = "Can't Save Region" };
        }

        public async Task<APIResult> Delete(int id)
        {
            var region = _unitOfWork.region.Query().Where(x => x.Id == id).SingleOrDefault();
            
            if (region == null)
                return new APIResult { message = "region NotFound" };

            _unitOfWork.region.Delete(region);

            if (_unitOfWork.Complete() > 0)
                return new APIResult { state = true, entity = region };

            return new APIResult { message = "Failed To Delete Region" };

        }

        public async Task<APIResult> GetAll()
        {
            var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            using (HttpClient client = new HttpClient(httpClientHandler))
            {
                var ContentBody = new Region { Name = "n", Done = true};
                var requestBodyJson = JsonConvert.SerializeObject(ContentBody);
                HttpContent content = new StringContent(requestBodyJson, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync("https://localhost:7164/api/Regions/CreateRegion",content).Result;
                string result = response.Content.ReadAsStringAsync().Result;
                var regionAPIResult = JsonConvert.DeserializeObject<APIResult>(result) ;
                //JObject o = JObject.Parse(regionAPIResult!.entity);
                var r = regionAPIResult.entity;
                var t = r.ToObject<Region>();

                JObject o1 = JObject.Parse(result);
                var t1 = o1.ToObject<APIResult>();
                var o2 = ((JObject)t1.entity).ToObject<Region>();
                    
                //var kq = o.ToObject<APIResult>();
                //var kw = ;
                //var t = 
                //var region = JsonConvert.DeserializeObject<Region>(regionAPIResult!.entity);
                //var data = region!.ToObject<Region>();

            }

            var allRegion =  _unitOfWork.region.Query().ToList();
            return new APIResult { entity = allRegion, state = true };
        }
        public async Task<APIResult> GetByID(int id)
        {
            Region region = _unitOfWork.region.Query().Where(x => x.Id == id).SingleOrDefault();
            if (region == null)
                return new APIResult { state = false, message = "NotFound" };

            return new APIResult { entity = region, state = true };
        }
        public async Task<APIResult> Update(Region reg)
        {
            var region = _unitOfWork.region.Query().Where(x=>x.Id == reg.Id).SingleOrDefault();
            if (region == null)
                return new APIResult { message = "NotFound region" };
            region.Name = reg.Name;
            region.Done = reg.Done;
            if(_unitOfWork.Complete() > 0)
                return new APIResult { state = true, entity = reg };

            return new APIResult { message = "Failed To Update Region"};
        }
    }
}
