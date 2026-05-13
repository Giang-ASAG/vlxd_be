namespace VLXD_API.Common
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public Meta? Meta { get; set; }
        public Error? Error { get; set; }

        public static ApiResponse<T> Ok(T data)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Meta = new Meta()
            };
        }
        public static ApiResponse<T> Fail(string code, string message)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Error = new Error
                {
                    Code = code,
                    Message = message
                }
            };
        }
        public static ApiResponse<T> Succes(string code, string message)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Error = new Error
                {
                    Code = code,
                    Message = message
                }
            };
        }
    }
}
