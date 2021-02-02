﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LES.Core.Common.HttpContextUser;
using LES.Core.Extensions.Others;
using LES.Core.IServices;
using LES.Core.Model;
using LES.Core.Model.Models;
using LES.Core.Model.ParamModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LES.Core.Controllers
{
    /// <summary>
    /// 接口管理
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Permissions.Name)]
    public class ModuleController : ControllerBase
    {
        readonly IModuleServices _moduleServices;
        readonly IUser _user;


        public ModuleController(IModuleServices moduleServices, IUser user)
        {
            _moduleServices = moduleServices;
            _user = user;
        }

        /// <summary>
        /// 分页获取接口api
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<MessageModel<PageModel<Modules>>> Get(ModulesParam param)
        {
            if (string.IsNullOrEmpty(param.searchVal) || string.IsNullOrWhiteSpace(param.searchVal))
            {
                param.searchVal = string.Empty;
            }
            var whereConditions = WhereConditionsExtensions.GetWhereConditions<Modules, ModulesParam>(param);
            if (!whereConditions.IsSuccess)
            {
                return new MessageModel<PageModel<Modules>>()
                {
                    msg = whereConditions.ErrMsg,
                    success = false,
                    response = null
                };
            }
            var data = await _moduleServices.QueryPage(whereConditions.data, param.page, param.pageSize, " Id desc ");

            return new MessageModel<PageModel<Modules>>()
            {
                msg = "获取成功",
                success = data.dataCount >= 0,
                response = data
            };

        }

        /// <summary>
        /// 获取全部接口api
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<MessageModel<List<Modules>>> GetAll()
        {
            var data = await _moduleServices.Query(a => (a.IsDeleted != true), " Id desc ");

            return new MessageModel<List<Modules>>()
            {
                msg = "获取成功",
                success = true,
                response = data
            };

        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public string Get(string id)
        {
            return "value";
        }

        /// <summary>
        /// 添加一条接口信息
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        // POST: api/User
        [HttpPost]
        public async Task<MessageModel<string>> Post([FromBody] Modules module)
        {
            var data = new MessageModel<string>();

            module.CreateId = _user.ID;
            module.CreateBy = _user.Name;

            var id = (await _moduleServices.Add(module));
            data.success = id > 0;
            if (data.success)
            {
                data.response = id.ObjToString();
                data.msg = "添加成功";
            }

            return data;
        }

        /// <summary>
        /// 更新接口信息
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        // PUT: api/User/5
        [HttpPut]
        public async Task<MessageModel<string>> Put([FromBody] Modules module)
        {
            var data = new MessageModel<string>();
            if (module != null && module.Id > 0)
            {
                data.success = await _moduleServices.Update(module);
                if (data.success)
                {
                    data.msg = "更新成功";
                    data.response = module?.Id.ObjToString();
                }
            }

            return data;
        }

        /// <summary>
        /// 删除一条接口
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/ApiWithActions/5
        [HttpDelete]
        public async Task<MessageModel<string>> Delete(int id)
        {
            var data = new MessageModel<string>();
            if (id > 0)
            {
                var userDetail = await _moduleServices.QueryById(id);
                userDetail.IsDeleted = true;
                data.success = await _moduleServices.Update(userDetail);
                if (data.success)
                {
                    data.msg = "删除成功";
                    data.response = userDetail?.Id.ObjToString();
                }
            }

            return data;
        }
    }
}
