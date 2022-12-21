using Cake.Common;
using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Core.IO;
using Cake.Frosting;
using System;
using Cake.Core;
using System.IO;

namespace Build.Tasks
{

    public class ParameterStoreUploadTask : FrostingTask<BuildContext>
    {
        const int Retries = 3;
        public override void Run(BuildContext context)
        {
            var exitCode = 0;
            // AWS SSM is sometimes glitchy, and all we need to do is to run it again, so retries on failure is useful here
            for (int i = 0; i < Retries; i++)
            {
                exitCode = Upload(context);
                if(exitCode == 0)
                    break;
            }

            if (exitCode != 0)
            {
                throw new ArgumentException("Failed to upload parameters");
            }
            else
            {
                var applicationEnvironment = context.Environment.GetEnvironmentVariable("Application__Environment");
                context.Information("Successfully uploaded parameters to SSM: /{0}/{1}/{2}/{3}", applicationEnvironment, context.Options.ApplicationPlatfrom, context.Options.ApplicationSystem, context.Options.ApplicationSubsystem);
            }
        }

        private  int Upload(BuildContext context)
        {
            FilePath parameterUploader = context.Tools.Resolve("ParameterStoreUploader.dll");

            var applicationEnvironment = context.Environment.GetEnvironmentVariable("Application__Environment");
            var region = context.Environment.GetEnvironmentVariable("AWS_DEFAULT_REGION");
            var awsProfile = context.Environment.GetEnvironmentVariable("AWS_PROFILE");
            var awsAccessKey = context.Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
            var awsSecretKey = context.Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");
            var applicationVersion = context.Environment.GetEnvironmentVariable("Application__Version");

            var kmsAlias = context.Environment.GetEnvironmentVariable("Application__UploaderKmsAlias");
            var uploaderPrivateKey = context.Environment.GetEnvironmentVariable("Application__UploaderPrivateKeyString");

            if (string.IsNullOrEmpty(applicationEnvironment))
            {
                throw new Exception("ParameterStoreUploader | Cannot find environment variable 'Application__Environment'");
            }

            if (string.IsNullOrEmpty(region))
            {
                region = context.Environment.GetEnvironmentVariable("AWS_REGION");
                if (string.IsNullOrEmpty(region))
                {
                    throw new Exception("ParameterStoreUploader | Cannot find environment variable 'AWS_DEFAULT_REGION'");
                }
            }

            if (string.IsNullOrEmpty(awsProfile) && string.IsNullOrEmpty(awsAccessKey)) //either using profile or access secret key
            {
                throw new Exception("ParameterStoreUploader | Cannot find either AWS Profile or AWS Access/Secret key");
            }

            if (string.IsNullOrEmpty(kmsAlias))
            {
                context.Information("Cannot find Uploader KMS alias from env var. Set default value");
                kmsAlias = "alias/dev-bic-security-kms-general_purpose_key";
            }

            if (string.IsNullOrEmpty(uploaderPrivateKey))
            {
                context.Information("Cannot find Uploader private key from env var. Set default value");
                uploaderPrivateKey = "no_value";
            }



            var configFolder = context.Options.ArtifactsFolder + "/configs";
            if (!context.DirectoryExists(configFolder))
                throw new DirectoryNotFoundException(configFolder);

            context.Information(configFolder);

            var pab = new ProcessArgumentBuilder()
                 .Append(parameterUploader.FullPath)
                .AppendSwitch("-f", configFolder)
                .AppendSwitch("-e", applicationEnvironment)
                .AppendSwitch("-a", context.Options.ApplicationPlatfrom)
                .AppendSwitch("-n", context.Options.ApplicationSystem)
                .AppendSwitch("--sub-system-name", context.Options.ApplicationSubsystem)
                .AppendSwitch("--system-version", applicationVersion)
                .AppendSwitch("-r", region)
                .AppendSwitch("-i", kmsAlias)
                .AppendSwitch("-p", uploaderPrivateKey);

            if (!string.IsNullOrEmpty(awsAccessKey) && !string.IsNullOrEmpty(awsSecretKey))
            {
                pab.AppendSwitch("-k", awsAccessKey).AppendSwitch("-s", awsSecretKey);
            }
            else
            {
                pab.AppendSwitch("-z", awsProfile);
            }

            context.Information("Executing: {0} {1}", "ParameterStoreUploader.dll", pab.Render());
            var exitCode = context.StartProcess("dotnet", new ProcessSettings { Arguments = pab, RedirectStandardOutput = true });

            return exitCode;
        }
    }
}