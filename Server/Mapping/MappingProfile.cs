using AutoMapper;
using BlazorWithApi.Shared.Models;

namespace BlazorWithApi.Server.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Map from MealSchedule to MealScheduleDto
        CreateMap<MealSchedule, MealScheduleDto>()
            .ForMember(dest => dest.EmployeeName, opt => 
                opt.MapFrom(src => src.Employee != null ? src.Employee.EmployeeName : null));
            
        // Map from MealScheduleDto to MealSchedule
        CreateMap<MealScheduleDto, MealSchedule>();
        
        // Map from Employee to Employee DTO if needed
        CreateMap<Employee, EmployeeDto>();
    }
}
