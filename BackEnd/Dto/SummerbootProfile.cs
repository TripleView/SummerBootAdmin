using AutoMapper;
using SummerBootAdmin.Dto.Dictionary;
using SummerBootAdmin.Model.Dictionary;

namespace SummerBootAdmin.Dto;

public class SummerbootProfile:Profile
{
    public SummerbootProfile()
    {
        CreateMap<AddDictionaryDto, Model.Dictionary.Dictionary>().ReverseMap();
        CreateMap<AddDictionaryItemDto, DictionaryItem>().ReverseMap();
        
    }
}