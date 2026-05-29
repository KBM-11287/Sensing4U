using Sensing4U.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sensing4U.Services
{
    public class DataProcessor
    {
        public static DataProcessor? Instance;

        // List of datasets (each dataset is a 2D array)
        private readonly List<SensorReading[,]> datasets;
        private readonly List<string> datasetNames;

        private int currentIndex = -1;

        private DataProcessor()
        {
            datasets = new List<SensorReading[,]>();
            datasetNames = new List<string>();
        }

        public static DataProcessor GetInstance()
        {
            if (Instance == null)
                Instance = new DataProcessor();

            return Instance;
        }

        // ------------------------------------------------------------
        // Add a dataset from a List<SensorReading> (List -> 2D array)
        // ------------------------------------------------------------
        public void AddDataset(string name, List<SensorReading> readings)
        {
            if (readings == null || readings.Count == 0)
                return;

            var array = new SensorReading[readings.Count, 1];
            for (int i = 0; i < readings.Count; i++)
                array[i, 0] = readings[i];

            datasets.Add(array);
            datasetNames.Add(name);

            // If this is the first dataset, select it
            if (currentIndex == -1)
                currentIndex = 0;
        }

        // ------------------------------------------------------------
        // Get current dataset (2D array)
        // ------------------------------------------------------------
        public SensorReading[,]? GetCurrentDataset()
        {
            if (currentIndex < 0 || currentIndex >= datasets.Count)
                return null;

            return datasets[currentIndex];
        }

        // Convert current dataset to List<SensorReading> for UI binding
        public List<SensorReading> GetCurrentDatasetAsList()
        {
            var result = new List<SensorReading>();
            var array = GetCurrentDataset();
            if (array == null) return result;

            int rows = array.GetLength(0);
            for (int i = 0; i < rows; i++)
                result.Add(array[i, 0]);

            return result;
        }

        // ------------------------------------------------------------
        // Dataset navigation
        // ------------------------------------------------------------
        public bool MoveNext()
        {
            if (currentIndex < datasets.Count - 1)
            {
                currentIndex++;
                return true;
            }
            return false;
        }

        public bool MovePrevious()
        {
            if (currentIndex > 0)
            {
                currentIndex--;
                return true;
            }
            return false;
        }

        public int GetCurrentIndex() => currentIndex;

        public List<string> GetDatasetNames() => new List<string>(datasetNames);

        public string? GetCurrentDatasetName()
        {
            if (currentIndex < 0 || currentIndex >= datasetNames.Count)
                return null;
            return datasetNames[currentIndex];
        }

        // ------------------------------------------------------------
        // Sorting current dataset
        // ------------------------------------------------------------
        public void SortCurrentDataset(string sortBy, bool descending = false)
        {
            var array = GetCurrentDataset();
            if (array == null) return;

            int rows = array.GetLength(0);
            var list = new List<SensorReading>();
            for (int i = 0; i < rows; i++)
                list.Add(array[i, 0]);

            switch (sortBy.ToLower())
            {
                case "timestamp":
                    list = descending
                        ? list.OrderByDescending(r => r.Timestamp).ToList()
                        : list.OrderBy(r => r.Timestamp).ToList();
                    break;

                case "label":
                    list = descending
                        ? list.OrderByDescending(r => r.Label).ToList()
                        : list.OrderBy(r => r.Label).ToList();
                    break;

                case "value":
                    list = descending
                        ? list.OrderByDescending(r => r.Value).ToList()
                        : list.OrderBy(r => r.Value).ToList();
                    break;
            }

            var sortedArray = new SensorReading[list.Count, 1];
            for (int i = 0; i < list.Count; i++)
                sortedArray[i, 0] = list[i];

            datasets[currentIndex] = sortedArray;
        }

        // ------------------------------------------------------------
        // Optimized: Calculate Average without re-allocating a full List
        // ------------------------------------------------------------
        public double CalculateAverageCurrent()
        {
            var array = GetCurrentDataset();
            if (array == null) return 0;

            int rows = array.GetLength(0);
            if (rows == 0) return 0;

            double sum = 0;
            // Iterate directly through the 2D array 
            for (int i = 0; i < rows; i++)
            {
                sum += array[i, 0].Value;
            }

            return sum / rows;
        }

        // ------------------------------------------------------------
        // Optimized: Binary Search directly on the Array
        // ------------------------------------------------------------
        public SensorReading? BinarySearchCurrent(string label)
        {
            var array = GetCurrentDataset();
            if (array == null) return null;

            int rows = array.GetLength(0);


            // Optimization: If the array is already sorted, search directly
            int left = 0;
            int right = rows - 1;

            while (left <= right)
            {
                int mid = left + (right - left) / 2; // Prevents potential overflow
                int cmp = string.Compare(array[mid, 0].Label, label, StringComparison.OrdinalIgnoreCase);

                if (cmp == 0) return array[mid, 0];
                if (cmp < 0) left = mid + 1;
                else right = mid - 1;
            }
             
            return null;
        }
    }
}
