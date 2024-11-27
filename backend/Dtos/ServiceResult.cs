namespace backend.Dtos
{
    public class ServiceResult<T>
    {
        public bool IsSucces { get; set; }

        public string Message { get; set; }

        public T Data { get; set; }

        public static ServiceResult<T> Success(T data, string message = "")
        {
            return new ServiceResult<T> { IsSucces = true, Data = data, Message = message };
        }

        public static ServiceResult<T> Failure(string message)
        {
            return new ServiceResult<T> { IsSucces = false, Message = message };
        }
    }
}
