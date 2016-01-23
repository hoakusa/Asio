using Asio.Models;
using IdentitySample.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;


namespace Asio.DAL
{
    public class SchoolContext : DbContext
    {
        public SchoolContext() : base("SchoolContext")
        {
        }

        

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}