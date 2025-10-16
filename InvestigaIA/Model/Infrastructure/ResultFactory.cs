using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestigaIA.Model.Infrastructure
{
    public static class ResultFactory<T>
    {


        public static GenericResult<T> Success(T value, string message) => new GenericResult<T>() 
        { 
        IsSuccess = true,
        Result = value,
        Error = null,
        Message = message
        };



        public static GenericResult<T> Failure(T value, string message) => new GenericResult<T>()
        {
            Result = value,
            IsSuccess = false,
            Error = null,
            Message = message
        };

        public static GenericResult<T> Failure(T value, string message, int statusCode) => new GenericResult<T>()
        {
            Result = value,
            IsSuccess = false,
            Error = statusCode,
            Message = message
        };


    }


    public class GenericResult<T>
    {
        public T? Result { get; init; }
        public bool IsSuccess { get; init; }
        
        [Range(100, 599)]
        public int? Error { get; init; }

        public string? Message { get; init; }





    }


}
