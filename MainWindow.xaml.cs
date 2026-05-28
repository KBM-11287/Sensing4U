using Sensing4U.Models;
using Sensing4U.Services;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Sensing4U
{
    public partial class MainWindow : Window
    {
        private readonly List<SensorReading> _tempList = new();
        private double _lower = 10;
        private double _upper = 50;

        public MainWindow()
        {
            InitializeComponent();
            RefreshUI();
        }

        private void SetStatus(string msg)
        {
            txtStatus.Text = msg;
        }

        private void RefreshUI()
        {
            var list = DataProcessor.Instance.GetCurrentDatasetAsList();
            SensorGrid.ItemsSource = list;

            double avg = DataProcessor.Instance.CalculateAverageCurrent();
            txtAverage.Text = avg.ToString("0.00");

            SetStatus("Dataset refreshed");
        }

        // LOAD CSV
        private void LoadCsv_Click(object sender, RoutedEventArgs e)
        {
            if (FileManager.LoadCsv("dataset.csv", _tempList, out string msg))
            {
                DataProcessor.Instance.AddDataset("CSV Dataset", _tempList  );
                RefreshUI();
            }

            SetStatus(msg);
        }

        // LOAD BINARY
        private void LoadBin_Click(object sender, RoutedEventArgs e)
        {
            if (FileManager.LoadBinary("dataset.bin", _tempList, out string msg))
            {
                DataProcessor.Instance.AddDataset("Binary Dataset", _tempList);
                RefreshUI();
            }

            SetStatus(msg);
        }

        // SAVE CSV
        private void SaveCsv_Click(object sender, RoutedEventArgs e)
        {
            var list = DataProcessor.Instance.GetCurrentDatasetAsList();

            if (FileManager.SaveCsv("saved.csv", list, out string msg))
                SetStatus("CSV saved successfully");
            else
                SetStatus(msg);
        }

        // SAVE BINARY
        private void SaveBin_Click(object sender, RoutedEventArgs e)
        {
            var list = DataProcessor.Instance.GetCurrentDatasetAsList();

            if (FileManager.SaveBinary("saved.bin", list, out string msg))
                SetStatus("Binary saved successfully");
            else
                SetStatus(msg);
        }

        // SEARCH
        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            string label = txtSearch.Text.Trim();
            if (string.IsNullOrWhiteSpace(label))
            {
                SetStatus("Enter a label to search");
                return;
            }

            var result = DataProcessor.Instance.BinarySearchCurrent(label);

            if (result != null)
                SetStatus($"Found: {result.Label} = {result.Value} {result.Unit}");
            else
                SetStatus("Label not found");
        }

        // UPDATE BOUNDS
        private void UpdateThresholds_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(txtLower.Text, out double lo))
                _lower = lo;

            if (double.TryParse(txtUpper.Text, out double hi))
                _upper = hi;

            RefreshUI();
            SetStatus("Bounds updated");
        }

        // ROW COLOURING
        private void SensorGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (e.Row.Item is not SensorReading reading)
                return;

            if (reading.Value < _lower)
                e.Row.Background = Brushes.LightBlue;
            else if (reading.Value > _upper)
                e.Row.Background = Brushes.LightCoral;
            else
                e.Row.Background = Brushes.LightGreen;
        }

        // NAVIGATION
        private void BtnPrev_Click(object sender, RoutedEventArgs e)
        {
            DataProcessor.Instance.MovePrevious();
            RefreshUI();
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            DataProcessor.Instance.MoveNext();
            RefreshUI();
        }
    }
}
