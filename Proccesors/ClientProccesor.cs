using SharpHydra.Logging;

namespace SharpHydra.Proccesors;

public class ClientProcessor
    {
        // Client machine identifier (hostname, IP, etc.)
        private string client { get; set; }

        // Where logs are written
        private string logPath { get; set; }

        // List of patterns/profiles to skip during cleanup
        private string[] skipListPattern { get; set; }

        // Logger instance for recording activity/errors
        private Logger logger { get; set; }

        public ClientProcessor(string client, string logPath, string[] skipListPattern, Logger logger)
        {
            this.client = client;
            this.logPath = logPath;
            this.skipListPattern = skipListPattern;
            this.logger = logger;
        }

        /// <summary>
        /// Performs the actual processing of a client:
        ///  - Logs start
        ///  - Enumerates user profiles
        ///  - Checks file activity
        ///  - Estimates space usage
        ///  - Logs completion
        /// </summary>
        public void Process()
        {
            logger.LogTextToFile($"Started Processing {client}");

            // UNC path to client’s Users directory via admin share (requires permissions!)
            string usersRoot = $@"\\{client}\C$\Users"; 
            string[] profiles = Directory.GetDirectories(usersRoot);
            foreach (string profile in profiles)
            {
                Console.WriteLine($"User Profile Found: {profile}");
            }

            var userAges = new List<int>();

            foreach (var profilePath in profiles)
            {
                try
                {
                    // Get all files in the profile folder
                    var profileFiles = Directory.GetFiles(profilePath);
                    Console.WriteLine($"Found {profileFiles.Length} files");

                    // Find the most recently modified file
                    var newestFile = profileFiles
                        .Select(path => new FileInfo(path))
                        .OrderByDescending(f => f.LastWriteTime)
                        .FirstOrDefault();

                    // Calculate "age" in days based on last access time
                    var ageDays = ( DateTime.Now - newestFile.LastAccessTime).Days;
                    userAges.Add(ageDays);
                    Console.WriteLine($"user: {profilePath}, Age: {ageDays}");
                }
                catch (Exception e)
                {
                    // Log but continue processing
                    logger.LogTextToFile(e.Message);
                }

                try
                {
                    // Compute total size of all files under Users
                    long totalBytes = Directory
                        .EnumerateFiles($"\\{client}\\C$\\Users", "*", SearchOption.AllDirectories)
                        .Select(f => new FileInfo(f).Length)
                        .Sum();

                    // Convert bytes to GB
                    double totalSizeGb = totalBytes / Math.Pow(1024, 3);

                    // Log cleanup summary
                    logger.LogTextToFile($"Space freed:  {totalSizeGb} GB on Client : {client}");
                    logger.LogTextToFile($"Finished with {client} : {DateTime.Now.Date}");
                }
                catch (Exception e)
                {
                    logger.LogTextToFile(e.Message);
                }
            }
        }
    }