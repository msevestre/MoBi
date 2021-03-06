using FluentNHibernate.Cfg;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using OSPSuite.Core.Extensions;
using OSPSuite.Infrastructure.Services;

namespace MoBi.Core.Serialization.ORM
{
   public class SessionFactoryProvider : ISessionFactoryProvider
   {
      public ISessionFactory InitalizeSessionFactoryFor(string dataSource)
      {
         var cfg = createSqlLiteConfigurationFor(dataSource);
         //Create schema for database
         new SchemaExport(cfg).Execute(false, true, false);

         return cfg.BuildSessionFactory();
      }

      public ISessionFactory OpenSessionFactoryFor(string dataSource)
      {
         return createSqlLiteConfigurationFor(dataSource).BuildSessionFactory();
      }

      public SchemaExport GetSchemaExport(string dataSource)
      {
         var cfg = createSqlLiteConfigurationFor(dataSource);
         //Create schema for database
         return new SchemaExport(cfg);
      }

      private Configuration createSqlLiteConfigurationFor(string dataSource)
      {
         var configuration = new Configuration();
         var path = dataSource.ToUNCPath();
         configuration.SetProperty("connection.provider", "NHibernate.Connection.DriverConnectionProvider");
         configuration.SetProperty("connection.driver_class", "NHibernate.Driver.SQLite20Driver");
         configuration.SetProperty("dialect", "NHibernate.Dialect.SQLiteDialect");
         configuration.SetProperty("query.substitutions", "true=1;false=0");
         configuration.SetProperty("show_sql", "false");
         configuration.SetProperty("connection.connection_string", string.Format("Data Source={0};Version=3;New=False;Compress=True;", path));

         return Fluently.Configure(configuration)
            .Mappings(cfg => cfg.FluentMappings.AddFromAssemblyOf<SessionFactoryProvider>()).BuildConfiguration();
      }
   }
}