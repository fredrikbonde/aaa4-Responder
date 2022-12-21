using Cake.Core.Diagnostics;
using Cake.DotNetCoreEf;
using Cake.Frosting;

namespace Build.Tasks
{
    [TaskName("EF DB Migration")]
    public class DbMigrationTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            context.Log.Information("Running EF DB Migration task");
            context.DotNetCoreEfDatabaseUpdate("./path to your packed ORM, check Enforcement Register for a reference");
        }
    }
}
