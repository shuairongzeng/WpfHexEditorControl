//////////////////////////////////////////////
// Apache 2.0  - 2021
// Author : Derek Tremblay (derektremblay666@gmail.com)
//
//
// NOT A TRUE PROJECT! IT'S JUST A SAMPLE FOR TESTING THE HEXEDITOR IN VARIOUS SITUATIONS... 
//////////////////////////////////////////////

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using WpfHexaEditor;
using WpfHexaEditor.Core;
using WpfHexaEditor.Core.Bytes;
using WpfHexaEditor.Core.EventArguments;
using WpfHexaEditor.Core.MethodExtention;

namespace WpfHexEditor.Sample.BinaryFilesDifference
{
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Used to catch internal change for cath potential infinite loop
        /// </summary>
        private bool _internalChange;
        List<ByteDifference> _differences;
        ObservableCollection<BlockListItem> _blockListItem = new();

        public MainWindow()
        {
            InitializeComponent();
            FileDiffBlockList.ItemsSource = _blockListItem; // 绑定数据源
        }

        #region Various controls events
        private void FirstHexEditor_Click(object sender, RoutedEventArgs e) => OpenFile(FirstFile);

        private void SecondHexEditor_Click(object sender, RoutedEventArgs e) => OpenFile(SecondFile);

        private void FindDifferenceButton_Click(object sender, RoutedEventArgs e)
        {
            if (FirstFile.FileName == string.Empty || SecondFile.FileName == string.Empty)
            {
                MessageBox.Show("LOAD TWO FILE!!", "HexEditor sample", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            Application.Current.MainWindow.Cursor = Cursors.Wait;
            FindDifference();
            Application.Current.MainWindow.Cursor = null;
        }


        private void FileDiffBytesList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (_internalChange) return;
            if (FileDiffBytesList.SelectedItem is not ByteDifferenceListItem byteDifferenceItem) return;

            _internalChange = true;
            FirstFile.SetPosition(byteDifferenceItem.ByteDiff.BytePositionInStream, 1);
            SecondFile.SetPosition(byteDifferenceItem.ByteDiff.BytePositionInStream, 1);
            _internalChange = false;
        }

        private void PatchButton_Click(object sender, RoutedEventArgs e)
        {
            if (_differences is null) return;

            SecondFile.With(c =>
            {
                c.ReadOnlyMode = false;

                foreach (BlockListItem itm in FileDiffBlockList.ItemsSource)
                {
                    var diffList = _differences.Where(d => d.BytePositionInStream >= itm.CustomBlock.StartOffset &&
                                                           d.BytePositionInStream <= itm.CustomBlock.StopOffset);

                    foreach (var byteDiff in diffList)
                        c.ModifyByte(byteDiff.Destination, byteDiff.BytePositionInStream);

                    itm.PatchBlockButton.IsEnabled = false;
                }

                c.ReadOnlyMode = true;
            });
        }

        private void BlockItemProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //UpdateListofBlockItem();
        }

        private void SaveChangeButton_Click(object sender, RoutedEventArgs e)
        {
            SecondFile.With(c =>
            {
                c.ReadOnlyMode = false;
                c.SubmitChanges();
                c.ReadOnlyMode = true;
            });

            ClearUI();
        }

        #endregion

        #region Various methods

        private void OpenFile(HexEditor hexEditor)
        {
            ClearUI();

            #region Create file dialog
            var fileDialog = new OpenFileDialog
            {
                Multiselect = true,
                CheckFileExists = true
            };

            if (fileDialog.ShowDialog() == null || !File.Exists(fileDialog.FileName)) return;
            #endregion

            hexEditor.FileName = fileDialog.FileName;
        }

        /// <summary>
        /// Clear the difference in various control
        /// </summary>
        private void ClearUI()
        {
            FileDiffBytesList.Items.Clear();
            FirstFile.ClearCustomBackgroundBlock();
            SecondFile.ClearCustomBackgroundBlock();
            SecondFile.ClearAllChange();
            PatchButton.IsEnabled = false;
            _blockListItem.Clear();
            _differences = null;
        }

        /// <summary>
        /// Update the list of block item
        /// </summary>
        //private void UpdateListofBlockItem()
        //{
        //    // 计算需要显示的项范围
        //    var startIndex = (int)BlockItemProgress.Value; // 当前滚动条位置
        //    var visibleItemCount = (int)(FileDiffBlockList.ActualHeight / new BlockListItem().Height); // 可见项数量
        //    var endIndex = Math.Min(startIndex + visibleItemCount, _blockListItem.Count); // 结束索引

        //    // 创建一个新的临时集合，只包含当前需要显示的项
        //    var visibleItems = _blockListItem.Skip(startIndex).Take(endIndex - startIndex).ToList();

        //    // 更新 ItemsControl 的 ItemsSource
        //    FileDiffBlockList.ItemsSource = visibleItems;
        //}

        #region 源代码逻辑
        /// <summary>
        /// Find the difference of the two loaded file and add to lists the results
        /// </summary>
        //private void FindDifference()
        //{
        //    ClearUI();

        //    if (FirstFile.FileName == string.Empty || SecondFile.FileName == string.Empty) return;

        //    FileDiffBlockList.Children.Clear();

        //    //load the difference
        //    _differences = FirstFile.Compare(SecondFile).ToList();

        //    //Load list of difference
        //    var cbb = new CustomBackgroundBlock();
        //    var j = 0;

        //    foreach (var byteDifference in _differences)
        //    {
        //        //create or update custom background block
        //        if (j == 0)
        //            cbb = new CustomBackgroundBlock(byteDifference.BytePositionInStream, ++j, RandomBrushes.PickBrush());
        //        else
        //            cbb.Length = ++j;

        //        if (!_differences.Any(c => c.BytePositionInStream == byteDifference.BytePositionInStream + 1))
        //        {
        //            j = 0;

        //            new BlockListItem(cbb).With(c =>
        //            {
        //                c.PatchButtonClick += BlockItem_PatchButtonClick;
        //                c.Click += BlockItem_Click;
        //                _blockListItem.Add(c);
        //            });

        //            //add to hexeditor
        //            FirstFile.CustomBackgroundBlockItems.Add(cbb);
        //            SecondFile.CustomBackgroundBlockItems.Add(cbb);
        //        }
        //    }

        //    //Update progressbar
        //    BlockItemProgress.Maximum = _blockListItem.Count();
        //    UpdateListofBlockItem();

        //    //refresh editor
        //    FirstFile.RefreshView();
        //    SecondFile.RefreshView();

        //    //Enable patch button
        //    PatchButton.IsEnabled = true;
        //}

        #endregion

        #region 新代码逻辑

        // 是否正在加载数据
        private bool _isLoading = false;
        private static readonly double ItemHeight = new BlockListItem().Height;
        private static int batches = 0;
        private static int batchSize = 20;
        private static int index = 0;
        private CancellationTokenSource _cts;
        Progress<int> progress;

        private async void FindDifference()
        {
            ClearUI();
            if (FirstFile.FileName == string.Empty || SecondFile.FileName == string.Empty) return;

            // 清空绑定的集合
            _blockListItem.Clear();

            _cts = new CancellationTokenSource();

            try
            {
                // 开始异步处理
                progress = new Progress<int>(percent =>
                {
                    // 更新进度条（如果需要）
                    ComparisonProgressBar.Value = percent;
                });

                // 在后台线程执行比较和分批次处理
                _differences = await Task.Run(() =>
                    FirstFile.Compare(SecondFile).ToList(), _cts.Token);

                await ProcessDifferencesInBatches(_differences, batchSize, _cts.Token, progress);

                // 最终UI更新
                Dispatcher.Invoke(() =>
                {
                    PatchButton.IsEnabled = true;
                    FirstFile.RefreshView();
                    SecondFile.RefreshView();
                    PreButton.IsEnabled= true;
                    NextButton.IsEnabled = true;
                });
            }
            catch (OperationCanceledException)
            {
                // 处理取消操作
            }
            finally
            {
            //    _cts.Dispose();
              //  _cts = null;
            }
        }

  
        private void FileDiffScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (_isLoading || _differences == null || _differences.Count == 0)
                return;

            // 获取滚动视图的高度和滚动位置
            var scrollViewer = sender as ScrollViewer;
            var viewportHeight = scrollViewer.ViewportHeight; // 可视区域高度
            var verticalOffset = scrollViewer.VerticalOffset; // 当前滚动位置

            // 计算当前可视区域的起始索引和结束索引
            var startIndex = (int)(verticalOffset / ItemHeight);
            var endIndex = (int)((verticalOffset + viewportHeight) / ItemHeight);

            // 限制索引范围
            startIndex = Math.Max(0, startIndex);
            endIndex = Math.Min(endIndex, _differences.Count - 1);

            // 动态加载当前可视区域的数据
            LoadVisibleItems(startIndex, endIndex);
        }

        private void LoadVisibleItems(int startIndex, int endIndex)
        {
            _isLoading = true;

            try
            {
                // 清空当前集合
                _blockListItem.Clear();

                // 提取当前可视区域的数据
                var visibleItems = _differences.Skip(startIndex).Take(endIndex - startIndex + 1)
                    .Select(diff => new BlockListItem(new CustomBackgroundBlock(
                        diff.BytePositionInStream,
                        1,
                        RandomBrushes.PickBrush())))
                    .ToList();

                // 添加到集合中
                foreach (var item in visibleItems)
                {
                    _blockListItem.Add(item);
                }
            }
            finally
            {
                _isLoading = false;
            }
        }


       /// <summary>
       /// 上一页
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
        private async void PreButton_Click(object sender, RoutedEventArgs e)
        {
            index--;
            await GetPageData(_differences, batchSize, progress, index, _cts.Token);
        }

        /// <summary>
        /// 下一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void NextButton_Click(object sender, RoutedEventArgs e)
        {
            index++;
            await GetPageData(_differences, batchSize, progress, index, _cts.Token);
        }

   
        private async Task ProcessDifferencesInBatches(
     List<ByteDifference> differences,
     int batchSize,
     CancellationToken token,
     IProgress<int> progress)
        {
            var total = differences.Count;
              batches = (int)Math.Ceiling(total / (double)batchSize);

            for (int i = 0; i < 1; i++)
            {
                await GetPageData(differences, batchSize, progress, i, token);
            }
        }

        private async Task GetPageData(List<ByteDifference> differences, int batchSize, IProgress<int> progress, int i, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            // 获取当前批次数据
            var batch = differences.Skip(i * batchSize).Take(batchSize).ToList();

            // 处理并更新UI
            await Dispatcher.InvokeAsync(() =>
            {
                //// 保存滚动条位置
                //var currentScrollPosition = BlockItemProgress.Value;

                // 处理批次数据
                ProcessBatch(batch);
                //UpdateUIAfterBatch();

                // 恢复滚动条位置
                //BlockItemProgress.Value = currentScrollPosition;


            }, DispatcherPriority.Normal, token);

            // 报告进度
            progress?.Report((i + 1) * 100 / batches);

            // 模拟UI更新间隔
            await Task.Delay(10, token);
        }

        private void ProcessBatch(List<ByteDifference> batch)
        {
            var cbb = new CustomBackgroundBlock();
            var j = 0;

            foreach (var byteDifference in batch)
            {
                if (j == 0)
                    cbb = new CustomBackgroundBlock(byteDifference.BytePositionInStream, ++j, RandomBrushes.PickBrush());
                else
                    cbb.Length = ++j;

                if (!_differences.Any(c => c.BytePositionInStream == byteDifference.BytePositionInStream + 1))
                {
                    j = 0;
                    var blockItem = new BlockListItem(cbb);
                    blockItem.PatchButtonClick += BlockItem_PatchButtonClick;
                    blockItem.Click += BlockItem_Click;

                    // 添加到集合中
                    _blockListItem.Add(blockItem);

                    FirstFile.CustomBackgroundBlockItems.Add(cbb);
                    SecondFile.CustomBackgroundBlockItems.Add(cbb);
                }
            }
        }
        //private void UpdateUIAfterBatch()
        //{
        //    // 分批次更新列表
        //    var currentCount = _blockListItem.Count;
        //    BlockItemProgress.Maximum = currentCount;
        //    UpdateListofBlockItem();
        //}
        private void UpdateUIAfterBatch()
        {
            // 保存当前滚动条位置
            //var currentScrollPosition = BlockItemProgress.Value;

            //// 更新滚动条的最大值
            //BlockItemProgress.Maximum = _blockListItem.Count;

            //// 恢复滚动条位置
            //BlockItemProgress.Value = currentScrollPosition;
        }

        #endregion
        /// <summary>
        /// Update view when item is clicked
        /// </summary>
        private void BlockItem_Click(object sender, EventArgs e)
        {
            if (_internalChange) return;
            if (sender is not BlockListItem blockitm) return;
            if (_differences is null) return;

            //Clear UI
            FileDiffBytesList.Items.Clear();

            _internalChange = true;
            FirstFile.SetPosition(blockitm.CustomBlock.StartOffset, 1);
            SecondFile.SetPosition(blockitm.CustomBlock.StartOffset, 1);
            _internalChange = false;

            //Load list of byte difference
            foreach (var byteDifference in _differences
                .Where(c => c.BytePositionInStream >= blockitm.CustomBlock.StartOffset &&
                            c.BytePositionInStream <= blockitm.CustomBlock.StopOffset))
            {
                byteDifference.Color = blockitm.CustomBlock.Color;
                FileDiffBytesList.Items.Add(new ByteDifferenceListItem(byteDifference));
            }
        }

        /// <summary>
        /// Patch the selected block in the second file
        /// </summary>
        private void BlockItem_PatchButtonClick(object sender, EventArgs e)
        {
            if (sender is not BlockListItem itm) return;
            if (_differences is null) return;

            SecondFile.With(c =>
            {
                c.ReadOnlyMode = false;

                var diffList = _differences.Where(d => d.BytePositionInStream >= itm.CustomBlock.StartOffset &&
                                                       d.BytePositionInStream <= itm.CustomBlock.StopOffset);

                foreach (var byteDiff in diffList)
                    c.ModifyByte(byteDiff.Origine, byteDiff.BytePositionInStream);

                c.ReadOnlyMode = true;
            });

            itm.PatchBlockButton.IsEnabled = false;
        }
        #endregion

        #region Synchronise the two hexeditor
        private void FirstFile_VerticalScrollBarChanged(object sender, ByteEventArgs e)
        {
            if (_internalChange) return;

            _internalChange = true;
            SecondFile.SetPosition(e.BytePositionInStream);
            _internalChange = false;
        }

        private void SecondFile_VerticalScrollBarChanged(object sender, ByteEventArgs e)
        {
            if (_internalChange) return;

            _internalChange = true;
            FirstFile.SetPosition(e.BytePositionInStream);
            _internalChange = false;
        }
        #endregion

        private void InsertByteFirstFileButton_Click(object sender, RoutedEventArgs e)
        {
            FirstFile.InsertByte((byte)0xFF, FirstFile.SelectionStart);
            FirstFile.RefreshView();
            ClearUI();
        }

        private void InsertByteSecondFileButton_Click(object sender, RoutedEventArgs e)
        {
            SecondFile.InsertByte((byte)0xFF, SecondFile.SelectionStart);
            SecondFile.RefreshView();
            ClearUI();
        }
    }
}
