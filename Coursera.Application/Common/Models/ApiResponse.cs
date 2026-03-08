using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Common.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T?  Data { get; set; }
        public ApiResponse()
        {
            Success = true;
        }

        public ApiResponse(T? data, string? message = null)
        {
            Success = true;
            Message = message;
            Data = data;
        }
        public ApiResponse(string message)
        {
            Success = false;
            Message = message;
        }
    }
}
