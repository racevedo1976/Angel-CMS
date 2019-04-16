using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Angelo.Common.Migrations
{
    public interface IAppMigration
    {
        string Id { get; }

        string Migration { get; }

     
        Task<MigrationResult> ExecuteAsync();
    }
}
