using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Humanizer;
using LifeLongApi.Codes;
using LifeLongApi.Data.Repositories;
using LifeLongApi.Dtos;
using LifeLongApi.Dtos.Response;
using LifeLongApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using static LifeLongApi.Codes.AppHelper;

namespace LifeLongApi.Services
{
    public interface IAppointmentService
    {
        Task<ServiceResponse<AppointmentResponseDto>> AddAppointmentAsync(AppointmentDto appointmentCreds);
        Task<ServiceResponse<AppointmentResponseDto>> DeleteAppointmentAsync(int appointmentId);
        Task<ServiceResponse<AppointmentResponseDto>> EditAppointmentAsync(int appointmentId, AppointmentDto appointmentCreds);
        Task<ServiceResponse<AppointmentResponseDto>> GetAppointmentAsync(int appointmentId);
        Task<ServiceResponse<List<AppointmentResponseDto>>> GetMentorsAppointmentsAsync(string username, AppointmentStatus status = AppointmentStatus.ALL);
    }

    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepo;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        public AppointmentService(IAppointmentRepository appointmentRepo,
                                  IMapper mapper,
                                  INotificationService notificationService,
                                  UserManager<AppUser> userManager)
        {
            _appointmentRepo = appointmentRepo;
            _mapper = mapper;
            _notificationService = notificationService;
            _userManager = userManager;
        }

        public async Task<ServiceResponse<AppointmentResponseDto>> AddAppointmentAsync(AppointmentDto appointmentCreds)
        {
            var sr = new ServiceResponse<AppointmentResponseDto>();
            //verify mentor and mentee exists
            var foundMentor = await _userManager.FindByNameAsync(appointmentCreds.MentorUsername);
            var foundMentee = await _userManager.FindByNameAsync(appointmentCreds.MenteeUsername);

            if (foundMentor == null || foundMentee == null)
            {
                sr.HelperMethod(404, "User not found", false);
                return sr;
            }

            var dateAndTime = $"{appointmentCreds.Date} {appointmentCreds.Time}";
            DateTime dateTimeVal;
            if (!IsValidDate(dateAndTime, out dateTimeVal))
            {
                sr.HelperMethod(400, "Invalid date or time format.", false);
                return sr;
            }


            //convert to Appointment entity.
            var appointment = _mapper.Map<Appointment>(appointmentCreds);
            //Set Values.
            appointment.MentorId = foundMentor.Id;
            appointment.MenteeId = foundMentee.Id;
            appointment.DateAndTime = dateTimeVal.ToUniversalTime();
            appointment.Status = AppHelper.AppointmentStatus.PENDING.ToString();
            //insert and save record to db.
            await _appointmentRepo.InsertAsync(appointment);

            //send notification to mentee
            var message = "You have a new appointment with {"
                          + foundMentor.FirstName
                          + foundMentor.LastName
                          + "} on <b>"
                          + TimeZoneInfo.ConvertTime(appointment.DateAndTime,
                                                     TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time")).ToOrdinalWords()
                          + "</b>"
                          + "time is : <b>" 
                          + TimeZoneInfo.ConvertTime(appointment.DateAndTime,
                                                     TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time")).ToShortTimeString()
                          + "</b>";
            await _notificationService.NewNotificationAsync(foundMentor.Id, foundMentee.Id, message, NotificationType.APPOINTMENT);

            //get all apointments
            var appointments = await _appointmentRepo.GetAllAsync();

            //return newly created resource
            sr.Code = 201;
            sr.Data = _mapper.Map<AppointmentResponseDto>(appointments.OrderByDescending(a => a.Id).Last());
            sr.Success = true;

            return sr;
        }

        public async Task<ServiceResponse<AppointmentResponseDto>> GetAppointmentAsync(int appointmentId)
        {
            var sr = new ServiceResponse<AppointmentResponseDto>();

            var appointment = await _appointmentRepo.GetByIdAsync(appointmentId);

            if (appointment == null)
            {
                sr.HelperMethod(404, "Appointment not found", false);
                return sr;
            }

            sr.Code = StatusCodes.Status200OK;
            sr.Data = _mapper.Map<AppointmentResponseDto>(appointment);
            sr.Success = true;
            return sr;
        }

        public async Task<ServiceResponse<List<AppointmentResponseDto>>> GetMentorsAppointmentsAsync(string username,
            AppointmentStatus status = AppointmentStatus.ALL)
        {
            var sr = new ServiceResponse<List<AppointmentResponseDto>>();
            //verify user exists
            var foundMentor = await _userManager.FindByNameAsync(username);
            if (foundMentor == null)
            {
                sr.HelperMethod(404, "User not found", false);
                return sr;
            }

            var userAppointments = new List<Appointment>();

            if (status.ToString() == AppointmentStatus.ALL.ToString())
            {
                userAppointments = await _appointmentRepo.GetAllMentorAppointmentsAsync(foundMentor.Id);
            }
            else
            {
                userAppointments = await _appointmentRepo.GetMentorAppointmentsByTypeAsync(foundMentor.Id, status);
            }

            sr.Code = StatusCodes.Status200OK;
            sr.Data = _mapper.Map<List<AppointmentResponseDto>>(userAppointments);
            sr.Success = true;
            return sr;
        }
        
        public async Task<ServiceResponse<AppointmentResponseDto>> EditAppointmentAsync(int appointmentId,
                                                                                        AppointmentDto appointmentCreds)
        {
            var sr = new ServiceResponse<AppointmentResponseDto>();
            bool isPostponed = false;
            string appointmentStatus = AppointmentStatus.PENDING.ToString();

            //verify mentor and mentee exists
            var foundMentor = await _userManager.FindByNameAsync(appointmentCreds.MentorUsername);
            var foundMentee = await _userManager.FindByNameAsync(appointmentCreds.MenteeUsername);
            if (foundMentor == null || foundMentee == null)
            {
                sr.HelperMethod(404, "User not found", false);
                return sr;
            }

            var dateAndTime = $"{appointmentCreds.Date} {appointmentCreds.Time}";
            DateTime dateTimeVal;
            if (!IsValidDate(dateAndTime, out dateTimeVal))
            {
                sr.HelperMethod(400, "Invalid date or time format.", false);
                return sr;
            }

            var appointment = await _appointmentRepo.GetByIdAsync(appointmentId);

            if (appointment.DateAndTime != dateTimeVal.ToUniversalTime())
            {
                //appointment has been moved to a new date and time.
                isPostponed = true;
                appointmentStatus = AppointmentStatus.POSTPONED.ToString();
            }

            appointment = _mapper.Map<Appointment>(appointmentCreds);
            //Set Values
            appointment.Id = appointmentId;
            appointment.MentorId = foundMentor.Id;
            appointment.MenteeId = foundMentee.Id;
            appointment.DateAndTime = dateTimeVal.ToUniversalTime();
            appointment.Status = appointmentStatus;
            //save edited record to db.
            await _appointmentRepo.UpdateAsync(appointment);

            if (isPostponed)
            {
                //notify mentee of the new change.
                var message = "Your appointment with {"
                              + foundMentor.FirstName
                              + foundMentor.LastName
                              + "} has been moved to <b>"
                              + TimeZoneInfo.ConvertTime(appointment.DateAndTime,
                                         TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time")).ToShortDateString()
                              + "</b>";
                await _notificationService.NewNotificationAsync(foundMentor.Id,
                                                                foundMentee.Id,
                                                                message,
                                                                NotificationType.APPOINTMENT
                                                                );
            }


            //return edited resource
            sr.Code = 200;
            sr.Data = _mapper.Map<AppointmentResponseDto>(appointment);
            sr.Success = true;
            return sr;
        }

        public async Task<ServiceResponse<AppointmentResponseDto>> DeleteAppointmentAsync(int appointmentId)
        {
            var sr = new ServiceResponse<AppointmentResponseDto>();

            var appointment = await _appointmentRepo.GetByIdAsync(appointmentId);
            if (appointment == null)
            {
                sr.HelperMethod(404, "No appointment found", false);
                return sr;
            }

            //delete record.
            await _appointmentRepo.DeleteAsync(appointment);

            //return newly created resource
            sr.Code = 200;
            sr.Message = "Appointment was successfully deleted.";
            sr.Success = true;
            return sr;
        }

        private static bool IsValidDate(string dateAndTime, out DateTime dateTimeVal)
        {
            bool validDate = DateTime.TryParseExact(dateAndTime,
                                                    "dd-MM-yyyy hh:mm tt",
                                                    CultureInfo.InvariantCulture,
                                                    DateTimeStyles.None,
                                                    out dateTimeVal);
            if (validDate)
                return true;
            else
                return false;
        }

    }
}