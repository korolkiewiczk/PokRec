using System;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters.Binary;
using CfrSolver.Model;
using CfrSolver;
using System.Text.Json;
using System.Collections.Concurrent;
using System.Text.Json.Serialization;
using System.Linq;
using System.Collections.Generic;
using static CfrSolver.Model.Node;

namespace Agent.CfrSolver.Graphgen
{
    public class TrainingDataSerializer
    {
        private const string NodesFileName = "nodes.json";
        private const string ConfigFileName = "config.json";
        private const string MetadataFileName = "metadata.json";
        
        public const string Folder = "traindata";
        public const string Extension = ".traindata";
        
        [Flags]
        public enum DeserializeFlags
        {
            None = 0,
            Nodes = 1,
            Config = 2,
            Metadata = 4,
            All = Nodes | Config | Metadata
        }

        public class TrainingMetadata
        {
            public int Iterations { get; set; }
        }

        public static void SaveTrainingData(string tableName, Node rootNode, NodeGenConfig config, int iterations)
        {
            
            var fileName = $"{Folder}\\{tableName}{Extension}";

            // Create a temporary directory for our files
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);

            try
            {
                // Save node tree
                using (var fs = File.Create(Path.Combine(tempDir, NodesFileName)))
                {
                    var options = new JsonSerializerOptions 
                    { 
                        IncludeFields = true,
                        WriteIndented = false,
                        ReferenceHandler = ReferenceHandler.Preserve
                    };
                    options.Converters.Add(new ConcurrentDictionaryConverter<int, NodeData>());
                    System.Text.Json.JsonSerializer.Serialize(fs, rootNode, options);
                }

                // Save config
                File.WriteAllText(
                    Path.Combine(tempDir, ConfigFileName), 
                    JsonConvert.SerializeObject(config, Formatting.Indented)
                );

                // Save metadata
                var metadata = new TrainingMetadata { Iterations = iterations };
                File.WriteAllText(
                    Path.Combine(tempDir, MetadataFileName),
                    JsonConvert.SerializeObject(metadata, Formatting.Indented)
                );

                if (!Directory.Exists(Folder))
                {
                    Directory.CreateDirectory(Folder);
                }
                // Create zip archive
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                ZipFile.CreateFromDirectory(tempDir, fileName);
            }
            finally
            {
                // Cleanup
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                }
            }
        }

        public static (Node RootNode, NodeGenConfig Config, TrainingMetadata Metadata) LoadTrainingData(
            string fileName, 
            DeserializeFlags flags = DeserializeFlags.All)
        {
            if (!fileName.Contains(Folder) && !fileName.Contains(Extension))
            {
                fileName = Path.Combine(Folder, fileName + Extension);
            }
            Node rootNode = null;
            NodeGenConfig config = null;
            TrainingMetadata metadata = null;

            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);

            try
            {
                ZipFile.ExtractToDirectory(fileName, tempDir);

                if (flags.HasFlag(DeserializeFlags.Nodes))
                {
                    var nodesPath = Path.Combine(tempDir, NodesFileName);
                    if (File.Exists(nodesPath))
                    {
                        var options = new JsonSerializerOptions 
                        { 
                            IncludeFields = true,
                            WriteIndented = false,
                            ReferenceHandler = ReferenceHandler.Preserve
                        };
                        options.Converters.Add(new ConcurrentDictionaryConverter<int, NodeData>());
                        var jsonString = File.ReadAllText(nodesPath);
                        rootNode = System.Text.Json.JsonSerializer.Deserialize<Node>(jsonString, options);
                    }
                }

                if (flags.HasFlag(DeserializeFlags.Config))
                {
                    var configPath = Path.Combine(tempDir, ConfigFileName);
                    if (File.Exists(configPath))
                    {
                        var jsonString = File.ReadAllText(configPath);
                        config = JsonConvert.DeserializeObject<NodeGenConfig>(jsonString);
                    }
                }

                if (flags.HasFlag(DeserializeFlags.Metadata))
                {
                    var metadataPath = Path.Combine(tempDir, MetadataFileName);
                    if (File.Exists(metadataPath))
                    {
                        var jsonString = File.ReadAllText(metadataPath);
                        metadata = JsonConvert.DeserializeObject<TrainingMetadata>(jsonString);
                    }
                }

                return (rootNode, config, metadata);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                }
            }
        }

        public class ConcurrentDictionaryConverter<TKey, TValue> : System.Text.Json.Serialization.JsonConverter<ConcurrentDictionary<TKey, TValue>>
        {
            public override ConcurrentDictionary<TKey, TValue> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var dictionary = System.Text.Json.JsonSerializer.Deserialize<Dictionary<TKey, TValue>>(ref reader, options);
                return new ConcurrentDictionary<TKey, TValue>(dictionary);
            }

            public override void Write(Utf8JsonWriter writer, ConcurrentDictionary<TKey, TValue> value, JsonSerializerOptions options)
            {
                System.Text.Json.JsonSerializer.Serialize(writer, value.ToDictionary(kvp => kvp.Key, kvp => kvp.Value), options);
            }
        }
    }
} 