using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo
{
    /// <summary>
    /// Extends the ServiceResult class to enable passing a TResult
    /// </summary>
    /// <typeparam name="TReturnType">The data type returned by invoking the service</typeparam>
    public class ServiceResult<TReturnType> : ServiceResult
    {
        public TReturnType Data { get; internal set; }

        public static ServiceResult<TReturnType> Success(TReturnType data)
        {
            return new ServiceResult<TReturnType>()
            {
                Succeeded = true,
                Data = data
            };
        }

        public new static ServiceResult<TReturnType> Failed(params string[] errors)
        {
            var errorList = new List<string>();
            errorList.AddRange(errors);

            return new ServiceResult<TReturnType>()
            {
                Succeeded = false,
                Errors = errorList
            };
        }
    }
}
