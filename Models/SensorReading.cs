using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sensing4U.Models
{
    public class SensorReading
    {
        public DateTime Timestamp { get; set; } // When the reading was taken
        public string Label { get; set; } // The type of reading, e.g., "Temperature", "Humidity", "LightLevel"
        public double? Value { get; set; } // numeric value, nullable in case of non-numeric readings
        public string Unit { get; set; } // e.g., "°C", "%", "Lux"
    }
}
