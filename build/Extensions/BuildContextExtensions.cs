using System;
using System.Linq;

namespace Build.Extensions
{ 
    public static class BuildContextExtensions
    {
    public static string GetEcrRepoName(this BuildContext context) =>
        $"{context.Options.ApplicationPlatfrom}/{context.Options.ApplicationSystem}-{context.Options.ApplicationSubsystem}-{context.Options.ApplicationName}-repo";
    
        public static bool  IsReleasableBranch( this BuildContext context)
        {
            var branchName = context.Options.GitVersion.BranchName;
            var releasableBranches = context.Options.ReleasableBranches;
            return releasableBranches.Any(r => branchName.ToLower().Contains(r.ToLower()));
        }

        public static string ResolveEnvironmentByBranch(this BuildContext context)
        {
            var branchName = context.Options.GitVersion.BranchName;
            var branchKey = context.Options.BranchEnvironmentMap.Keys.FirstOrDefault(k => branchName.ToLower().Contains(k));
            if (branchKey != null  )
            {
              return  context.Options.BranchEnvironmentMap[branchKey] ; 
            }
            return null ;
        }
    }
}
