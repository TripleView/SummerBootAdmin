using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlParser.Net.Ast;
using SummerBoot.Cache;
using SummerBoot.Core;
using SummerBoot.Repository;
using SummerBootAdmin.Dto;
using SummerBootAdmin.Dto.CodeGenerator;
using SummerBootAdmin.Dto.Dictionary;
using SummerBootAdmin.Dto.Menu;
using SummerBootAdmin.Model.CodeGenerator;
using SummerBootAdmin.Repository.CodeGenerator;
using System.Collections.Generic;
using System.Text;

namespace SummerBootAdmin.Controllers.CodeGenerator;

[Authorize(Policy = "urlPolicy")]
[ApiController]
[Route("api/[controller]/[action]")]
public class CodeGeneratorSettingController : ControllerBase
{
    private readonly IConfiguration configuration;

    public CodeGeneratorSettingController(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    [HttpGet]
    public async Task<ApiResult<List<DirectoryTreeItem>>> TreeList()
    {
        var baseDir = configuration.GetSection("baseDir").Value;
        var baseDirInfo = new DirectoryInfo(baseDir);
        var i = 0;
        var result = GetDirectoryTree(baseDirInfo, ref i);

        return ApiResult<List<DirectoryTreeItem>>.Ok(result.Children);
    }

    private DirectoryTreeItem GetDirectoryTree(DirectoryInfo directoryInfo, ref int i)
    {
        var result = new DirectoryTreeItem()
        {
            Id = i++,
            Name = directoryInfo.Name,
            FullName = directoryInfo.FullName,
            Children = new List<DirectoryTreeItem>()
        };
        var subDirectories = directoryInfo.GetDirectories();
        if (!subDirectories.Any())
        {
            return result;
        }

        foreach (var subDirectory in subDirectories)
        {
            var item = GetDirectoryTree(subDirectory, ref i);
            result.Children.Add(item);
        }

        return result;
    }
}