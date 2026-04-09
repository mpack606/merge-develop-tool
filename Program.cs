using System.Diagnostics;
/*
 * Cheat sheet
 * dotnet pack
 * dotnet tool update -g --add-source ./dist MergeDevelopTool 
 */


class MergeDevelop
{
    static void Main(string[] args)
    {
        try
        {
            // Check for flags
            bool refresh = false;
            string targetBranch = "develop";

            foreach (var arg in args)
            {
                if (arg == "--help" || arg == "-h")
                {
                    ShowHelp();
                    return;
                }
                
                if (arg == "--refresh" || arg == "-r")
                {
                    refresh = true;
                }
                else if (!arg.StartsWith("-"))
                {
                    targetBranch = arg;
                }
            }

            if (refresh)
            {
                Console.WriteLine("Refreshing current branch (stash -> pull -> stash pop)...");
                try 
                {
                    RunGitCommand("stash --include-untracked");
                }
                catch (Exception)
                {
                    // Stash might fail if there are no changes, but we should continue with pull
                }
                
                RunGitCommand("pull");
                
                try 
                {
                    RunGitCommand("stash pop");
                }
                catch (Exception)
                {
                    // Stash pop might fail if there was nothing to pop or conflicts
                }
                
                Console.WriteLine("✅ Refresh complete.");
                return;
            }
            
            Console.WriteLine("Fetching current branch...");
            string currentBranch = RunGitCommand("rev-parse --abbrev-ref HEAD");
            Console.WriteLine($"Current branch: {currentBranch}");

            if (currentBranch == targetBranch)
            {
                Console.WriteLine($"Already on {targetBranch} branch. Please run this script from a feature branch.");
                Environment.Exit(1);
            }

            Console.WriteLine($"Switching to {targetBranch}...");
            RunGitCommand($"checkout {targetBranch}");

            Console.WriteLine($"Pulling latest changes from {targetBranch}...");
            RunGitCommand("pull");

            Console.WriteLine($"Switching back to {currentBranch}...");
            RunGitCommand($"checkout {currentBranch}");

            Console.WriteLine($"Merging {targetBranch} into {currentBranch}...");
            RunGitCommand($"merge {targetBranch}");

            Console.WriteLine("✅ Merge complete.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
            Environment.Exit(1);
        }
    }

    static void ShowHelp()
    {
        Console.WriteLine("mdev - Merge Development Branch Tool");
        Console.WriteLine("===================================");
        Console.WriteLine();
        Console.WriteLine("Description:");
        Console.WriteLine("  A Git workflow automation tool that merges a target branch");
        Console.WriteLine("  (typically 'develop' or 'main') into your current feature branch.");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  mdev [target-branch] [options]");
        Console.WriteLine("  mdev --help");
        Console.WriteLine("  mdev -h");
        Console.WriteLine();
        Console.WriteLine("Arguments:");
        Console.WriteLine("  target-branch    The branch to merge into current branch (default: 'develop')");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  --help, -h       Show this help message");
        Console.WriteLine("  --refresh, -r    Stash changes, pull latest and pop stash");
    }

    static string RunGitCommand(string arguments)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();

        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();

        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new Exception(error.Trim());
        }

        return output.Trim();
    }
}