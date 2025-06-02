
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace FileDifferenceViewer
{
    public partial class MainWindow : Window
    {
        private string _filePathA;
        private string _filePathB;
        private ObservableCollection<Difference> _differences = new ObservableCollection<Difference>();

        public MainWindow()
        {
            InitializeComponent();
            DifferencesGrid.ItemsSource = _differences;
        }

        private void SelectFileA_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                _filePathA = openFileDialog.FileName;
                TextBoxFileA.Text = _filePathA;
            }
        }

        private void SelectFileB_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                _filePathB = openFileDialog.FileName;
                TextBoxFileB.Text = _filePathB;
            }
        }

        private async void CompareFiles_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_filePathA) || string.IsNullOrEmpty(_filePathB))
            {
                MessageBox.Show("请先选择要对比的两个文件。", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            StatusTextBlock.Text = "正在对比文件...";
            _differences.Clear();

            try
            {
                byte[] fileABytes = await File.ReadAllBytesAsync(_filePathA);
                byte[] fileBBytes = await File.ReadAllBytesAsync(_filePathB);

                long lengthA = fileABytes.Length;
                long lengthB = fileBBytes.Length;
                long maxLength = Math.Max(lengthA, lengthB);

                long i = 0;
                while (i < maxLength)
                {
                    if (i >= lengthA || i >= lengthB || fileABytes[i] != fileBBytes[i])
                    {
                        long startOffset = i;
                        long endOffset = i;
                        StringBuilder valuesA = new StringBuilder();
                        StringBuilder valuesB = new StringBuilder();

                        // 查找连续的差异
                        while (endOffset < maxLength &&
                               (endOffset >= lengthA || endOffset >= lengthB || fileABytes[endOffset] != fileBBytes[endOffset]))
                        {
                            if (endOffset < lengthA)
                            {
                                valuesA.Append(fileABytes[endOffset].ToString("X2"));
                            }
                            else
                            {
                                valuesA.Append("--");
                            }

                            if (endOffset < lengthB)
                            {
                                valuesB.Append(fileBBytes[endOffset].ToString("X2"));
                            }
                            else
                            {
                                valuesB.Append("--");
                            }

                            if (endOffset < maxLength - 1 && // 不是最后一个差异
                                (endOffset + 1 < lengthA && endOffset + 1 < lengthB && fileABytes[endOffset + 1] != fileBBytes[endOffset + 1]) ||
                                (endOffset + 1 < lengthA && endOffset + 1 >= lengthB) ||
                                (endOffset + 1 >= lengthA && endOffset + 1 < lengthB)
                               )
                            {
                                valuesA.Append(" "); // 添加分隔符
                                valuesB.Append(" "); // 添加分隔符
                            }
                            endOffset++;
                        }
                        endOffset--; // 回退到最后一个不同的字节

                        _differences.Add(new Difference
                        {
                            StartOffset = startOffset,
                            EndOffset = endOffset,
                            ValueA = valuesA.ToString(),
                            ValueB = valuesB.ToString()
                        });
                        i = endOffset + 1; // 跳过已处理的连续差异
                    }
                    else
                    {
                        i++;
                    }
                }

                StatusTextBlock.Text = $"对比完成，共发现 {_differences.Count} 处连续差异。";
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"对比出错：{ex.Message}";
                MessageBox.Show($"对比文件时发生错误：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveDifferences_Click(object sender, RoutedEventArgs e)
        {
            if (_differences.Count == 0)
            {
                MessageBox.Show("没有发现任何差异，无法保存。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "差异文件 (*.diff)|*.diff|所有文件 (*.*)|*.*";
            saveFileDialog.DefaultExt = ".diff";
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                    {
                        writer.WriteLine("起始位置,结束位置,a.exe差异值,b.exe差异值");
                        foreach (var diff in _differences)
                        {
                            writer.WriteLine($"{diff.StartOffset},{diff.EndOffset},{diff.ValueA},{diff.ValueB}");
                        }
                    }
                    MessageBox.Show($"差异已保存到：{saveFileDialog.FileName}", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"保存差异文件时发生错误：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void OpenDifferencesFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "差异文件 (*.diff)|*.diff|所有文件 (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                _differences.Clear();
                try
                {
                    string[] lines = await File.ReadAllLinesAsync(openFileDialog.FileName);
                    if (lines.Length > 1)
                    {
                        for (int i = 1; i < lines.Length; i++)
                        {
                            string[] parts = lines[i].Split(',');
                            if (parts.Length == 4 &&
                                long.TryParse(parts[0], out long startOffset) &&
                                long.TryParse(parts[1], out long endOffset))
                            {
                                _differences.Add(new Difference
                                {
                                    StartOffset = startOffset,
                                    EndOffset = endOffset,
                                    ValueA = parts[2],
                                    ValueB = parts[3]
                                });
                            }
                            else if (parts.Length == 4) // 尝试兼容旧格式，如果需要
                            {
                                _differences.Add(new Difference
                                {
                                    StartOffset = i - 1,
                                    EndOffset = i - 1,
                                    ValueA = parts[0],
                                    ValueB = parts[1]
                                });
                            }
                        }
                    }
                    StatusTextBlock.Text = $"已加载差异文件：{openFileDialog.FileName}，共 {_differences.Count} 处连续差异。";
                }
                catch (Exception ex)
                {
                    StatusTextBlock.Text = $"加载差异文件时发生错误：{ex.Message}";
                    MessageBox.Show($"加载差异文件时发生错误：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

    }

    public class Difference
    {
        public long StartOffset { get; set; } // 使用 long 以支持更大的文件
        public long EndOffset { get; set; }
        public string ValueA { get; set; } // 可以考虑存储起始和结束的差异值，或者一个范围
        public string ValueB { get; set; }
    }
}