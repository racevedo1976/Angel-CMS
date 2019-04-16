using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo
{
    /// <summary>
    /// Base ServiceResult class for reporting success or failure 
    /// </summary>
    public class ServiceResult
    {
        /// <summary>
        /// Flag indicating whether the a service call was successful
        /// </summary>
        public bool Succeeded { get; protected set; }

        /// <summary>
        /// A collection of errors supplied by the service upon failure
        /// </summary>
        public IEnumerable<string> Errors { get; protected set; }


        /// <summary>
        /// Services can use this method to create a ServiceResult that succeeded
        /// </summary>
        /// <returns>ServiceResult instance reporting success</returns>
        public static ServiceResult Success()
        {
            return new ServiceResult() { Succeeded = true };
        }

        /// <summary>
        /// Services can use this method to create a ServiceResult that failed
        /// </summary>
        /// <param name="errors">Any errors that occurred</param>
        /// <returns>ServiceResult instance reporting failure</returns>
        public static ServiceResult Failed(params string[] errors)
        {
            var errorList = new List<string>();
            errorList.AddRange(errors);

            return new ServiceResult()
            {
                Succeeded = false,
                Errors = errorList
            };
        }

        /// <summary>
        /// Services can use this method to create a ServiceResult that failed
        /// </summary>
        /// <returns>ServiceResult instance reporting failure</returns>
        public static ServiceResult Failed()
        {
            return new ServiceResult()
            {
                Succeeded = false
            };
        }

        /// <summary>
        /// Converts the result to a formatted message suitable for displaying to a UI
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var status = Succeeded
                ? " succeeded"
                : " failed with the following errors: \n\n";

            return "The operation " + status + String.Join("\n", this.Errors);
        }


        public ServiceResult()
        {
            Succeeded = false;
            Errors = new List<string>();
        }
    }
}
