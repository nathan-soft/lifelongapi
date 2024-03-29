using System;
using System.Linq;
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
            CreateMap<AppUser, RegisterUserDto>().ReverseMap();
            CreateMap<CreateUserDto, AppUser>()
                .ForMember(dest => dest.UserRoles, opt => opt.Ignore());
            CreateMap<UserDto, AppUser>();
            CreateMap<AppUser, AppUserResponseDto>()
             .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
             .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.UserRoles.Select(r => r.Role.Name).ToList()));
            CreateMap<AppUser, UserDto>()
             .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.UserRoles.Select(r => r.Role.Name).ToList()));
            CreateMap<AppUser, ApiOkResponseDto>().ReverseMap();
            CreateMap<AppUser, SearchResponseDto>()
                        .ForPath(dest => dest.User.Username, opt => opt.MapFrom(src => src.UserName))
                        .ForPath(dest => dest.User.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                        .ForMember(dest => dest.Location,
                                   opt => opt.MapFrom(src => $"{src.City}.{src.State}.{src.Country}"))
                        .ForMember(dest => dest.MenteesCount, opt => opt.MapFrom(src => src.Mentees.Count));
            CreateMap<Appointment, AppointmentResponseDto>()
                .ForPath(dest => dest.Mentee.FullName,
                           opt => opt.MapFrom(src => $"{src.Mentee.FirstName} {src.Mentee.LastName}"))
                .ForPath(dest => dest.Mentee.Username, opt => opt.MapFrom(src => src.Mentee.UserName))
                .ForMember(dest => dest.DateAndTime, opt =>
                {
                    opt.MapFrom(src => TimeZoneInfo.ConvertTimeFromUtc(src.DateAndTime,
                                                                TimeZoneInfo.FindSystemTimeZoneById(src.Mentor.TimeZone)));
                });
            CreateMap<AppointmentDto, Appointment>();
            CreateMap<ArticleRequestDto, Article>();
            CreateMap<Article, ArticleResponseDto>()
                .ForPath(dest => dest.Author.FullName,
                           opt => opt.MapFrom(src => $"{src.Author.FirstName} {src.Author.LastName}"))
                .ForPath(dest => dest.Author.Username, opt => opt.MapFrom(src => src.Author.UserName))
                .ForPath(dest => dest.ArticleTags, opt => opt.MapFrom(src => src.ArticleTags.Select(at => at.Topic.Name)));
            CreateMap<QualificationDto, Qualification>();
            CreateMap<Qualification, QualificationResponseDto>().ReverseMap();
            CreateMap<Notification, NotificationResponseDto>()
                .ForPath(dest => dest.Creator.Username, opt => opt.MapFrom(src => src.CreatedBy.UserName))
                .ForPath(dest => dest.Creator.FullName,
                           opt => opt.MapFrom(src => $"{src.CreatedBy.FirstName} { src.CreatedBy.LastName}"))
                .ForMember(dest => dest.CreatedOn, opt =>
                {
                    opt.MapFrom(src => TimeZoneInfo.ConvertTimeFromUtc(src.CreatedOn,
                                                                TimeZoneInfo.FindSystemTimeZoneById(src.CreatedFor.TimeZone)));
                });
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
            CreateMap<Follow, UsersRelationshipInfoDto>()
                .ForMember(dest => dest.TopicName, opt => opt.MapFrom(src => src.Topic.Name));
            CreateMap<WorkExperience, WorkExperienceResponseDto>()
                .ForMember(dest => dest.FieldOfInterest, opt => opt.MapFrom(src => src.Topic.Name));
            CreateMap<WorkExperienceDto, WorkExperience>();
            CreateMap(typeof(ServiceResponse<>), typeof(ApiOkResponseDto));
            CreateMap(typeof(ServiceResponse<>), typeof(UserDto)).ReverseMap();
            CreateMap(typeof(ServiceResponse<>), typeof(ApiErrorResponseDto)).ForSourceMember("Code", c => c.ToString());

            var configuration = new MapperConfiguration(cfg =>
               cfg.CreateMap(typeof(ServiceResponse<>), typeof(ApiErrorResponseDto))
            );
            //configuration.CreateMapper()
        }
    }
}