using System;
using System.Collections.Generic;
using System.IO;
using GlobExpressions;
using static Bullseye.Targets;
using static SimpleExec.Command;

const string clean = "clean";
const string build = "build";
const string test = "test";
const string format = "format";
const string publish = "publish";

Target(clean,
    ForEach("publish", "**/bin", "**/obj"),
    dir =>
    {
        IEnumerable<string> GetDirectories(string d)
        {
            return Glob.Directories(".", d);
        }

        void RemoveDirectory(string d)
        {
            if (Directory.Exists(d))
            {
                Console.WriteLine($"Cleaning {d}");
                Directory.Delete(d, true);
            }
        }

        foreach (var d in GetDirectories(dir))
        {
            RemoveDirectory(d);
        }
    });


Target(format, () =>
{
    Run("dotnet", "tool restore");
    Run("dotnet", "format");
});

Target(build, DependsOn(format), () => Run("dotnet", "build . -c Release"));

Target(test, DependsOn(build),
    () =>
    {
        IEnumerable<string> GetFiles(string d)
        {
            return Glob.Files(".", d);
        }

        foreach (var file in GetFiles("tests/**/*.csproj"))
        {
            Run("dotnet", $"test {file} -c Release --no-restore --no-build --verbosity=normal");
        }
    });

Target(publish, DependsOn(test),
    ForEach("src/Conduit"),
    project =>
    {
        Run("dotnet",
            $"publish {project} -c Release -f net6.0 -o ./publish --no-restore --no-build --verbosity=normal");
    });

Target("default", DependsOn(publish), () => Console.WriteLine("Done!"));
await RunTargetsAndExitAsync(args);
