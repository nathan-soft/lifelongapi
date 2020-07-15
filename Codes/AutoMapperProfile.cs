using AutoMapper;
using LifeLongApi.Codes;
using LifeLongApi.Dtos;
using LifeLongApi.Dtos.Response;
using LifeLongApi.Models;

namespace LifeLongApi.Codes {
    public class AutoMapperProfile : Profile {
        public AutoMapperProfile () {
            CreateMap<AppUser, RegisterDto> ().ReverseMap();
            CreateMap<AppUser, UserDto>().ReverseMap();
                    // .ForSourceMember(src => src.Followers, opt => opt.DoNotValidate())
                    // .ForSourceMember(src => src.Following, opt => opt.DoNotValidate())
                    // .ForMember(dest => dest.UserFieldOfInterests, opt => opt.Ignore());
            CreateMap<AppUser, ApiOkResponseDto>().ReverseMap();
            CreateMap<AppUser, SearchResponseDto>()
                        .ForMember(dest => dest.MenteesCount, opt => opt.MapFrom(src => src.Followers.Count));
            CreateMap<QualificationDto, Qualification>();
            CreateMap<Qualification, QualificationResponseDto>().ReverseMap();
            CreateMap<UserDto, ApiOkResponseDto>().ReverseMap();
            CreateMap<AppRole, RoleDto>().ReverseMap();
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Topic, TopicDto>().ReverseMap();
            CreateMap<Topic, MutualInterestDto>();
            CreateMap<UserFieldOfInterest, UserFieldOfInterestDto>()
                        .ForMember(dest => dest.Username, opt => opt.Ignore())
                        .ForMember(dest => dest.TopicName, opt => opt.MapFrom(src =>src.Topic.Name));
            CreateMap<Follow, FollowResponseDto>()
                        .ForMember(dest => dest.FollowingMentorUsername, opt => opt.MapFrom(src => src.Follower.UserName))
                        .ForMember(dest => dest.MenteeUsername, opt => opt.MapFrom(src => src.User.UserName))
                        .ForMember(dest => dest.MenteeFullname, opt => opt.MapFrom(src => src.User.FirstName + src.User.LastName))
                        .ForMember(dest => dest.TopicName, opt => opt.MapFrom(src => src.Topic.Name));
            CreateMap<Follow, FriendDto>()
                        .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                        .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FirstName
                                                                                    + src.User.LastName))
                        .ForMember(dest => dest.Location, opt => opt.MapFrom(src => $"{src.User.City}.{src.User.State}.{src.User.Country}"));
            CreateMap<WorkExperience, WorkExperienceResponseDto>()
                        .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.UserName))
                        .ForMember(dest => dest.FieldOfInterestId, opt => opt.MapFrom(src => src.TopicId))
                        .ForMember(dest => dest.FieldOfInterest, opt => opt.MapFrom(src => src.Topic.Name));
            CreateMap<WorkExperienceDto, WorkExperience>();
            CreateMap(typeof(ServiceResponse<>), typeof(ApiOkResponseDto));
            CreateMap(typeof(ServiceResponse<>), typeof(UserDto)).ReverseMap();
            CreateMap(typeof(ServiceResponse<>), typeof(ApiErrorResponseDto)).ForSourceMember("Code", c => c.ToString());
            //CreateMap<AddCharacterDto, Character> ();

            var configuration = new MapperConfiguration (cfg =>
                cfg.CreateMap (typeof (ServiceResponse<>), typeof (ApiErrorResponseDto))
            );
            //configuration.CreateMapper()
        }
    }
}