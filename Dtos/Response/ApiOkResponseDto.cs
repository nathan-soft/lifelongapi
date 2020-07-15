namespace LifeLongApi.Dtos.Response {
    public class ApiOkResponseDto : ApiResponseDto {
        public object Data { get; set; }
        public bool Success { get; set; } = true;
    }
}