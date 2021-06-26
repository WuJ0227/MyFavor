using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyFavor.Model;
using MyFavor.WebApi.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFavor.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MyFavorController : ControllerBase
    {
        [HttpGet]
        public string GetMyFavors()
        {
            return DAL.Instance.GetMyFavor();
        }
        [HttpPost]
        public void AddMyFavor([FromBody] int[] data)
        {
            DAL.Instance.AddMyFavor(data);
        }
        [HttpPost]
        public void RemoveMyFavors([FromBody] int[] data)
        {
            DAL.Instance.RemoveMyFavor(data);
        }
        [HttpGet]
        public string GetMyFavorDescription()
        {
            return DAL.Instance.GetMyFavorDescription();
        }
    }
}
