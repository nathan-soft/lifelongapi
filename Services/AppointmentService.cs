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
        Task<ServiceResponse<List<AppointmentResponseDto>>> GetMentorAppointmentsAsync(string username, string appointmentStatus);
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
                          + TimeZoneInfo.ConvertTimeFromUtc(appointment.DateAndTime,
                              TimeZoneInfo.FindSystemTimeZoneById(foundMentee.TimeZone)).ToOrdinalWords()
                          + ".</b>"
                          + "Time is : <b>" 
                          + TimeZoneInfo.ConvertTimeFromUtc(appointment.DateAndTime,
                              TimeZoneInfo.FindSystemTimeZoneById(foundMentee.TimeZone)).ToShortTimeString()
                          + "</b>";
            await _notificationService.NewNotificationAsync(foundMentor.Id, foundMentee.Id, message, NotificationType.APPOINTMENT);

            //get all apointments
            var appointments = await _appointmentRepo.GetAllAsync();

            //return newly created resource
            sr.Code = 201;
            sr.Data = _mapper.Map<AppointmentResponseDto>(appointments.OrderByDescending(a => a.Id).First());
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

        public async Task<ServiceResponse<List<AppointmentResponseDto>>> GetMentorAppointmentsAsync(
            string username,
            string appointmentStatus)
        {
            var sr = new ServiceResponse<List<AppointmentResponseDto>>();

            //valid apointment status??
            var validStatus = Enum.TryParse(appointmentStatus.ToUpper(), out AppointmentStatus status);

            if (!validStatus)
            {
                sr.HelperMethod(400, "Invalid appointment status.", false);
                return sr;
            }

            //status must be either pending or missed because these are the statuses known by the "Outside World".
            if (status != AppointmentStatus.PENDING && status != AppointmentStatus.MISSED)
            {
                sr.HelperMethod(400, "Invalid appointment status.", false);
                return sr;
            }

            //verify user exists
            var foundMentor = await _userManager.FindByNameAsync(username);
            if (foundMentor == null)
            {
                sr.HelperMethod(404, "User not found", false);
                return sr;
            }

            // if (string.IsNullOrEmpty(appointmentStatus)) 
            // {
            //     sr.HelperMethod(400, "Appointment status is required.", false);
            //     return sr;
            // }

            var userAppointments = new List<Appointment>();
            userAppointments = await _appointmentRepo.GetMentorAppointmentsByTypeAsync(foundMentor.Id, status);
            

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
            bool isRescheduled = false;
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

            //get appointment
            var appointment = await _appointmentRepo.GetByIdAsync(appointmentId);
            if (appointment.Mentee.UserName != appointmentCreds.MenteeUsername)
            {
                sr.HelperMethod(400, "The 'Mentee Username' provided does not match with the one in db.", false);
                return sr;
            }

            if (appointment.DateAndTime != dateTimeVal.ToUniversalTime())
            {
                if(appointment.Status == AppointmentStatus.MISSED.ToString()){
                    //the appointment was missed and its being rescheduled.
                    isRescheduled = true;
                }else{
                    //appointment has been moved to a new date and time.
                    isPostponed = true;
                    appointmentStatus = AppointmentStatus.POSTPONED.ToString();
                }
            }

            //Set Values
            appointment.Title = appointmentCreds.Title;
            appointment.Description = appointmentCreds.Description;
            appointment.DateAndTime = dateTimeVal.ToUniversalTime();
            appointment.Status = appointmentStatus;
            //save edited record to db.
            await _appointmentRepo.UpdateAsync(appointment);

            string message = "";
            if (isPostponed)
            {
                //notify mentee of the new change.
                message = "Your appointment with {"
                              + foundMentor.FirstName
                              + foundMentor.LastName
                              + "} has been moved to <b>"
                              + TimeZoneInfo.ConvertTimeFromUtc(appointment.DateAndTime,
                                         TimeZoneInfo.FindSystemTimeZoneById(foundMentee.TimeZone)).ToShortDateString()
                              + "</b>";
            }else if(isRescheduled){
                //notify mentee of the new change.
                message = "Your appointment with {"
                              + foundMentor.FirstName
                              + foundMentor.LastName
                              + "} has been rescheduled to <b>"
                              + TimeZoneInfo.ConvertTimeFromUtc(appointment.DateAndTime,
                                         TimeZoneInfo.FindSystemTimeZoneById(foundMentee.TimeZone)).ToShortDateString()
                              + "</b>";
            }

            await _notificationService.NewNotificationAsync(foundMentor.Id,
                                                                foundMentee.Id,
                                                                message,
                                                                NotificationType.APPOINTMENT
                                                                );

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