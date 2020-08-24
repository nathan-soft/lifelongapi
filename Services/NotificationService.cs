using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using LifeLongApi.Codes;
using LifeLongApi.Data.Repositories;
using LifeLongApi.Dtos.Response;
using LifeLongApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using static LifeLongApi.Codes.AppHelper;

namespace LifeLongApi.Services
{
    public interface INotificationService
    {
        Task<ServiceResponse<string>> DeleteNotificationAsync(int notificationId);
        Task<ServiceResponse<NotificationResponseDto>> MarkNotificationAsSeenAsync(int notificationId);
        Task NewNotificationAsync(int createdBy, int createdFor, string message, NotificationType type);
    }

    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepo;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;

        public NotificationService(INotificationRepository notificationRepo, IMapper mapper)
        {
            _notificationRepo = notificationRepo;
            _mapper = mapper;
        }

        public async Task NewNotificationAsync(
            int createdBy,
            int createdFor,
            string message,
            NotificationType type)
        {
            var notification = new Notification
            {
                CreatedById = createdBy,
                CreatedForId = createdFor,
                Message = message,
                Type = type.ToString(),
                IsSeen = false
            };

            await _notificationRepo.InsertAsync(notification);
        }

        public async Task<ServiceResponse<List<NotificationResponseDto>>> GetUserNotificationsAsync(string username)
        {
            var sr = new ServiceResponse<List<NotificationResponseDto>>();
            //verify user exists
            var foundUser = await _userManager.FindByNameAsync(username);
            if (foundUser == null)
            {
                sr.HelperMethod(404, "User not found", false);
                return sr;
            }

            var userWorkExperiences = await _notificationRepo.GetNotificationsForUserAsync(foundUser.Id);

            sr.Code = StatusCodes.Status200OK;
            sr.Data = _mapper.Map<List<NotificationResponseDto>>(userWorkExperiences);
            sr.Success = true;
            return sr;
        }

        public async Task<ServiceResponse<NotificationResponseDto>> MarkNotificationAsSeenAsync(int notificationId)
        {
            var sr = new ServiceResponse<NotificationResponseDto>();

            var notification = await _notificationRepo.GetByIdAsync(notificationId);
            if (notification == null)
            {
                sr.HelperMethod(404, "Notification not found", false);
                return sr;
            }

            notification.IsSeen = true;
            await _notificationRepo.UpdateAsync(notification);
            //return newly created resource
            sr.Code = 201;
            sr.Data = _mapper.Map<NotificationResponseDto>(notification);
            sr.Success = true;
            return sr;
        }

        public async Task<ServiceResponse<string>> DeleteNotificationAsync(int notificationId)
        {
            var sr = new ServiceResponse<string>();

            var notification = await _notificationRepo.GetByIdAsync(notificationId);

            if (notification == null)
            {
                sr.HelperMethod(404, "Notification not found", false);
                return sr;
            }

            await _notificationRepo.DeleteAsync(notification);
            //return newly created resource
            sr.Code = 200;
            sr.Message = "Notification Deleted!";
            sr.Success = true;
            return sr;
        }

    }
}