namespace LifeLongApi.Codes {
    public class ServiceResponse<T> {
        public T Data { get; set; }
        public int Code {get; set;} 
        public bool Success { get; set; }

        public string Message { get; set; }

        public void HelperMethod(int code = 200, string message = "", bool success = true){
            Code  = code;
            Message = message;
            Success = success;
        }
    }
}