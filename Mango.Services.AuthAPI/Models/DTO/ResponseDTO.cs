namespace Mango.Services.AuthAPI.Models.DTO
{
    public class ResponseDTO
    {
        public object? Result { get; set; } = null;
        public bool IsSuccess { get; set; } = false;
        public string Message { get; set; } = "";
    }
}
