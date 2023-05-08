using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permission.Infrastructure.DataAccess
{
    public class DataBaseContextFactory
    {
        private readonly Action<DbContextOptionsBuilder> _configureDbContext;

        public DataBaseContextFactory(Action<DbContextOptionsBuilder> configureDbContext)
        {
            _configureDbContext = configureDbContext;
        }

        public DataBaseContext CreateDbContext()
        {
            DbContextOptionsBuilder<DataBaseContext> optionsBuilder = new();
            _configureDbContext(optionsBuilder);

            return new DataBaseContext(optionsBuilder.Options);
        }
    }
}
