using Cake.Core;
using Cake.Frosting;
using Build.Models;
using System;

namespace Build
{
    public partial class BuildContext : FrostingContext
    {
        public Options Options { get; set; }

        public BuildContext(ICakeContext context) : base(context)
        {
            Options = context.GetConfigurationModel();
        } 
    }
}
