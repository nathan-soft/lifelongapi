namespace LifeLongApi.Dtos.Response {
    public class TokenDto : ApiResponseDto {
        public string access_token { get; set; }
        public string expiration { get; set; }
    }
}