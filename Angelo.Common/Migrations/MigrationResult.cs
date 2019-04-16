using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Common.Migrations
{
    public class MigrationResult
    {
        public MigrationStatus Status { get; set; }
        public string Message { get; set; }

        private MigrationResult()
        {

        }

        public static MigrationResult Failed(string message = null)
        {
            return new MigrationResult
            {
                Status = MigrationStatus.Failed,
                Message = message
            };
        }

        public static MigrationResult Success(string message = null)
        {
            return new MigrationResult
            {
                Status = MigrationStatus.Success,
                Message = message
            };
        }

        public static MigrationResult Skipped(string message = null)
        {
            return new MigrationResult
            {
                Status = MigrationStatus.Skipped,
                Message = message
            };
        }
    }

    public enum MigrationStatus
    {
        Failed = -1,
        Skipped = 0,
        Success = 1
    }
}
