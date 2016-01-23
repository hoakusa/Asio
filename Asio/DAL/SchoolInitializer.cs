using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Asio.Models;
using IdentitySample.Models;

namespace Asio.DAL
{
    public class SchoolInitializer : DropCreateDatabaseIfModelChanges<SchoolContext>
    {
        protected override void Seed(SchoolContext context)
        {

            
        }
    }
}