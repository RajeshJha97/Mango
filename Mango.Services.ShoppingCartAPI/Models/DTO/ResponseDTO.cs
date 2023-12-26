namespace Mango.Services.ShoppingCartAPI.Models.DTO
{
    public class ResponseDTO
    {
        public object? Result { get; set; }
        public bool IsSuccess { get; set; }=false;
        public string Message { get; set; }
    }
}
