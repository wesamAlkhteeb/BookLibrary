using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models.Configuration
{
    public class DatabaseSettings
    {
        public required string ConnectionString { get; set; }
    }
}
