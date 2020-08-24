using AutoMapper;
using LifeLongApi.Codes;
using LifeLongApi.Dtos;
using LifeLongApi.Dtos.Response;
using LifeLongApi.Models;

namespace LifeLongApi.Codes
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AppUser, RegisterDto>().ReverseMap();
            CreateMap<AppUser, UserDto>().ReverseMap();
            // .ForSourceMember(src => src.Followers, opt => opt.DoNotValidate())
            // .ForSourceMember(src => src.Following, opt => opt.DoNotValidate())
            // .ForMember(dest => dest.UserFieldOfInterests, opt => opt.Ignore());
            CreateMap<AppUser, ApiOkResponseDto>().ReverseMap();
            CreateMap<AppUser, SearchResponseDto>()
                        .ForMember(dest => dest.MenteesCount, opt => opt.MapFrom(src => src.Mentees.Count));
            CreateMap<Appointment, AppointmentResponseDto>()
                .ForPath(dest => dest.Mentee.FullName,
                           opt => opt.MapFrom(src => $"{src.Mentee.FirstName} {src.Mentee.FirstName}"))
                .ForPath(dest => dest.Mentee.Username, opt => opt.MapFrom(src => src.Mentee.UserName));
            CreateMap<AppointmentDto, Appointment>();
            CreateMap<QualificationDto, Qualification>();
            CreateMap<Qualification, QualificationResponseDto>().ReverseMap();
            CreateMap<Notification, NotificationResponseDto>()
                .ForPath(dest => dest.Creator.Username, opt => opt.MapFrom(src => src.CreatedBy.UserName))
                .ForPath(dest => dest.Creator.FullName,
                           opt => opt.MapFrom(src => $"{src.CreatedBy.FirstName} { src.CreatedBy.FirstName}"));
            CreateMap<UserDto, ApiOkResponseDto>().ReverseMap();
            CreateMap<AppRole, RoleDto>().ReverseMap();
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Topic, TopicDto>().ReverseMap();
            CreateMap<Topic, MutualInterestDto>();
            CreateMap<UserFieldOfInterest, UserFieldOfInterestDto>()
                        .ForMember(dest => dest.Username, opt => opt.Ignore())
                        .ForMember(dest => dest.TopicName, opt => opt.MapFrom(src => src.Topic.Name));
            CreateMap<Follow, UnAttendedRequestForMentorDto>()
                .ForPath(dest => dest.User.Username, opt => opt.MapFrom(src => src.Mentee.UserName))
                .ForPath(dest => dest.User.FullName, opt => opt.MapFrom(src => $"{src.Mentee.FirstName} {src.Mentee.LastName}"))
                .ForMember(dest => dest.TopicName, opt => opt.MapFrom(src => src.Topic.Name));
            CreateMap<Follow, UnAttendedRequestForMenteeDto>()
                .ForPath(dest => dest.User.Username, opt => opt.MapFrom(src => src.Mentor.UserName))
                .ForPath(dest => dest.User.FullName, opt => opt.MapFrom(src => $"{src.Mentor.FirstName} {src.Mentor.LastName}"))
                .ForMember(dest => dest.TopicName, opt => opt.MapFrom(src => src.Topic.Name));
            CreateMap<Follow, MentorFriendDto>()
                .ForPath(dest => dest.User.Username, opt => opt.MapFrom(src => src.Mentee.UserName))
                .ForPath(dest => dest.User.FullName, opt => opt.MapFrom(src => $"{src.Mentee.FirstName} {src.Mentee.LastName}"))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => $"{src.Mentee.City}.{src.Mentee.State}.{src.Mentee.Country}"));
            CreateMap<Follow, MenteeFriendDto>()
                .ForPath(dest => dest.User.Username, opt => opt.MapFrom(src => src.Mentor.UserName))
                .ForPath(dest => dest.User.FullName,
                         opt => opt.MapFrom(src => $"{src.Mentor.FirstName} {src.Mentor.LastName}"))
                .ForMember(dest => dest.Location,
                           opt => opt.MapFrom(src => $"{src.Mentor.City}.{src.Mentor.State}.{src.Mentor.Country}"));
            CreateMap<WorkExperience, WorkExperienceResponseDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.FieldOfInterestId, opt => opt.MapFrom(src => src.TopicId))
                .ForMember(dest => dest.FieldOfInterest, opt => opt.MapFrom(src => src.Topic.Name));
            CreateMap<WorkExperienceDto, WorkExperience>();
            CreateMap(typeof(ServiceResponse<>), typeof(ApiOkResponseDto));
            CreateMap(typeof(ServiceResponse<>), typeof(UserDto)).ReverseMap();
            CreateMap(typeof(ServiceResponse<>), typeof(ApiErrorResponseDto)).ForSourceMember("Code", c => c.ToString());
            //CreateMap<AddCharacterDto, Character> ();

            var configuration = new MapperConfiguration(cfg =>
               cfg.CreateMap(typeof(ServiceResponse<>), typeof(ApiErrorResponseDto))
            );
            //configuration.CreateMapper()
        }
    }
}