using System;

namespace LifeLongApi.Dtos.Response {
    public class ApiErrorResponseDto : ApiResponseDto {
        public string Code {get; set;} = "500";
        public bool Success { get; set; } = false;
        public string Message { get; set; } = "An unknown error occurred while processing your request, please contact your system administrator.";
    }
}