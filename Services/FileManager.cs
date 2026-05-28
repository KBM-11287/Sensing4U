#pragma warning disable SYSLIB0011
using Sensing4U.Models;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Sensing4U.Services
{
    public static class FileManager
    {
        private static readonly string DataFolder =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");

        static FileManager()
        {
            if (!Directory.Exists(DataFolder))
                Directory.CreateDirectory(DataFolder);
        }

        // ------------------------------------------------------------
        // Load CSV from Data folder
        // ------------------------------------------------------------
        public static bool LoadCsv(string fileName, List<SensorReading> readings, out string message)
        {
            readings.Clear();
            message = "";

            try
            {
                string fullPath = Path.Combine(DataFolder, fileName);

                foreach (var line in File.ReadAllLines(fullPath))
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    var parts = line.Split(',');
                    if (parts.Length < 4) // Expecting 4 columns now: Timestamp, Label, Value, Unit
                        continue;

                    if (!DateTime.TryParse(parts[0], out var ts))
                        continue;

                    var label = parts[1];

                    if (!double.TryParse(parts[2], NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
                        continue;

                    var unit = parts[3];

                        readings.Add(new SensorReading
                    {
                        Timestamp = ts,
                        Label = label,
                        Value = value,
                        Unit = unit
                    });
                }

                message = $"CSV loaded from Data folder: {fileName}";
                return true;
            }
            catch (Exception ex)
            {
                message = $"Error loading CSV: {ex.Message}";
                return false;
            }
        }

        // ------------------------------------------------------------
        // Save CSV to Data folder
        // ------------------------------------------------------------
        public static bool SaveCsv(string fileName, List<SensorReading> readings, out string message)
        {
            try
            {
                string fullPath = Path.Combine(DataFolder, fileName);

                using (var writer = new StreamWriter(fullPath))
                {
                    foreach (var r in readings)
                    {
                        writer.WriteLine($"{r.Timestamp},{r.Label},{r.Value.ToString(null, CultureInfo.InvariantCulture)}, {r.Unit}");
                    }
                }

                message = $"CSV saved to Data folder: {fileName}";
                return true;
            }
            catch (Exception ex)
            {
                message = $"Error saving CSV: {ex.Message}";
                return false;
            }
        }

        // ------------------------------------------------------------
        // Save Binary to Data folder
        // ------------------------------------------------------------
        public static bool SaveBinary(string fileName, List<SensorReading> readings, out string message)
        {
            try
            {
                string fullPath = Path.Combine(DataFolder, fileName);

                using (var fs = new FileStream(fullPath, FileMode.Create))
                {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(fs, readings);
                }

                message = $"Binary file saved to Data folder: {fileName}";
                return true;
            }
            catch (Exception ex)
            {
                message = $"Error saving binary file: {ex.Message}";
                return false;
            }
        }

        // ------------------------------------------------------------
        // Load Binary from Data folder
        // ------------------------------------------------------------
        public static bool LoadBinary(string fileName, List<SensorReading> readings, out string message)
        {
            readings.Clear();
            message = "";

            try
            {
                string fullPath = Path.Combine(DataFolder, fileName);

                using (var fs = new FileStream(fullPath, FileMode.Open))
                {
                    var formatter = new BinaryFormatter();
                    var loaded = formatter.Deserialize(fs) as List<SensorReading>;

                    if (loaded != null)
                    {
                        readings.AddRange(loaded);
                        message = $"Binary file loaded from Data folder: {fileName}";
                        return true;
                    }
                }

                message = "Binary file was empty or invalid.";
                return false;
            }
            catch (Exception ex)
            {
                message = $"Error loading binary file: {ex.Message}";
                return false;
            }
        }
    }
}

