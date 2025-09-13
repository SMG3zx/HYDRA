using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharpHydra.Configuration;

 public class Config
    {
        // Application version number
        [JsonPropertyName("hydraVersion")] public double Version { get; set; }

        // Flag that determines whether to show a startup message
        [JsonPropertyName("displayStartupMessage")]
        public bool DisplayStartupMessage { get; set; }

        // Last date this config file was modified
        [JsonPropertyName("lastModifiedDate")] public DateTime LastModifiedDate { get; set; }

        // Age-related threshold setting (e.g., days since last file access)
        [JsonPropertyName("ageSetting")] public int AgeSetting { get; set; }

        // Throttle limit for controlling processing concurrency/throughput
        [JsonPropertyName("throttleLimit")] public int ThrottleLimit { get; set; }

        // Paths to files containing lists of clients (default: empty array to avoid null refs)
        [JsonPropertyName("clientsPath")] public string ClientsPath { get; set; }

        // Path where logs will be written (default: empty string if not set)
        [JsonPropertyName("logPath")] public string LogPath { get; set; } = string.Empty;

        // List of profiles that should be skipped when processing clients
        [JsonPropertyName("skipProfiles")] public string[] SkipProfiles { get; set; } = Array.Empty<string>();


        /// <summary>
        /// Loads configuration values from a JSON file on disk.
        /// Throws exceptions if file is missing or JSON is invalid.
        /// </summary>
        public void LoadConfig()
        {
            // Hard-coded config file path (could be made configurable later)
            const string configPath = "HYDRA.json";

            // Fail fast if config file is missing
            if (!File.Exists(configPath))
                throw new FileNotFoundException("Configuration file not found.", configPath);

            // Read the JSON content into a string
            string configJson = File.ReadAllText(configPath);

            // Configure deserialization options:
            //  - Ignore case for property names
            //  - Allow trailing commas
            //  - Skip over comments in the JSON
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip
                // Note: could add converters here for enums, custom types, etc.
            };

            Config? deserializedConfig;
            try
            {
                // Attempt to deserialize JSON into a Config object
                deserializedConfig = JsonSerializer.Deserialize<Config>(configJson, options);
            }
            catch (JsonException ex)
            {
                // Wrap JSON errors with a clearer message
                throw new InvalidOperationException("Configuration JSON is invalid.", ex);
            }

            // If deserialization returns null, treat it as a failure
            if (deserializedConfig is null)
                throw new InvalidOperationException("Failed to deserialize configuration file.");

            // Copy all deserialized values into the current object instance
            this.Version = deserializedConfig.Version;
            this.DisplayStartupMessage = deserializedConfig.DisplayStartupMessage;
            this.LastModifiedDate = deserializedConfig.LastModifiedDate;
            this.AgeSetting = deserializedConfig.AgeSetting;
            this.ThrottleLimit = deserializedConfig.ThrottleLimit;
            this.ClientsPath = deserializedConfig.ClientsPath;
            this.LogPath = deserializedConfig.LogPath;
            this.SkipProfiles = deserializedConfig.SkipProfiles;
        }

        /// <summary>
        /// Saves the current configuration back to the JSON file.
        /// Writes the JSON in indented/pretty format.
        /// </summary>
        public void SaveConfig()
        {
            var options = new JsonSerializerOptions();
            options.WriteIndented = true; // Human-readable JSON

            // Serialize current Config instance (this) into JSON
            string serializedConfig = JsonSerializer.Serialize<Config>(this, options);

            // Write back to the same hard-coded file path
            File.WriteAllText("D:\\CSharp\\SharpHYDRA\\HYDRA.json", serializedConfig);
        }
    }