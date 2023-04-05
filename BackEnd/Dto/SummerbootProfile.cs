using AutoMapper;
using SummerBootAdmin.Dto.Department;
using SummerBootAdmin.Dto.Dictionary;
using SummerBootAdmin.Dto.Role;
using SummerBootAdmin.Dto.User;
using SummerBootAdmin.Model.Dictionary;

namespace SummerBootAdmin.Dto;

public class SummerbootProfile:Profile
{
    public SummerbootProfile()
    {
        CreateMap<AddDictionaryDto, Model.Dictionary.Dictionary>().ReverseMap();
        CreateMap<AddDictionaryItemDto, DictionaryItem>().ReverseMap();
        CreateMap<AddDepartmentDto, Model.Department.Department>().ReverseMap();
        CreateMap<AddRoleDto, Model.Role.Role>().ReverseMap();
        CreateMap<AddUserDto, Model.User.User>().ReverseMap();
    }
}