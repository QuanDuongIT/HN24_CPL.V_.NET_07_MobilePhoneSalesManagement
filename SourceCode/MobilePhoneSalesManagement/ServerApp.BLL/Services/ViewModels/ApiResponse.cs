using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp.BLL.Services.ViewModels
{
    public class ApiResponse<T>
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        // Constructor mặc định
        public ApiResponse()
        {
        }

        // Constructor với thông báo và dữ liệu
        public ApiResponse(int statusCode, string message, T data)
        {
            StatusCode = statusCode;
            Message = message;
            Data = data;
        }

        // Constructor khi chỉ có thông báo và mã trạng thái
        public ApiResponse(int statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
            Data = default; // Không có dữ liệu trả về
        }
    }

}
