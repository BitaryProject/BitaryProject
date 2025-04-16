using System;
using System.Collections.Generic;

namespace Shared.SecurityModels
{
    /// <summary>
    /// Generic API response wrapper
    /// </summary>
    /// <typeparam name="T">Type of data being returned</typeparam>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        public ApiResponse() { }

        public ApiResponse(T data, bool success = true, string message = null)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        public ApiResponse(string errorMessage)
        {
            Success = false;
            Message = errorMessage;
            Errors.Add(errorMessage);
        }

        public ApiResponse(List<string> errors)
        {
            Success = false;
            Message = "One or more errors occurred";
            Errors = errors;
        }
    }

    /// <summary>
    /// DTO version of ApiResponse for mapping
    /// </summary>
    /// <typeparam name="T">Type of data being returned</typeparam>
    public class ApiResponseDTO<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
} 