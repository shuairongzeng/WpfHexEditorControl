using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfHexaEditor;

namespace WpfHexEditor.Sample.BinaryFilesDifference
{
    public class MainWindowViewModel : BindableBase
    {
        private string _firstFileName;
        private string _secondFileName;
        private HexEditor _firstFileEditor;
        private HexEditor _secondFileEditor;
        private ObservableCollection<BlockListItem> _differences;
        private bool _canPatch;
        private bool _canNavigatePrevious;
        private bool _canNavigateNext;
        private double _comparisonProgress;

        public string FirstFileName
        {
            get => _firstFileName;
            set => SetProperty(ref _firstFileName, value);
        }

        public string SecondFileName
        {
            get => _secondFileName;
            set => SetProperty(ref _secondFileName, value);
        }

        public HexEditor FirstFileEditor
        {
            get => _firstFileEditor;
            set => SetProperty(ref _firstFileEditor, value);
        }

        public HexEditor SecondFileEditor
        {
            get => _secondFileEditor;
            set => SetProperty(ref _secondFileEditor, value);
        }

        public ObservableCollection<BlockListItem> Differences
        {
            get => _differences;
            set => SetProperty(ref _differences, value);
        }

        public bool CanPatch
        {
            get => _canPatch;
            set => SetProperty(ref _canPatch, value);
        }

        public bool CanNavigatePrevious
        {
            get => _canNavigatePrevious;
            set => SetProperty(ref _canNavigatePrevious, value);
        }

        public bool CanNavigateNext
        {
            get => _canNavigateNext;
            set => SetProperty(ref _canNavigateNext, value);
        }

        public double ComparisonProgress
        {
            get => _comparisonProgress;
            set => SetProperty(ref _comparisonProgress, value);
        }

        public ICommand LoadFirstFileCommand { get; }
        public ICommand LoadSecondFileCommand { get; }
        public ICommand SaveChangesCommand { get; }
        public ICommand InsertByteCommand { get; }
        public ICommand FindDifferencesCommand { get; }
        public ICommand PatchFileCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand NextPageCommand { get; }

        public MainWindowViewModel()
        {
            LoadFirstFileCommand = new DelegateCommand(LoadFirstFile);
            LoadSecondFileCommand = new DelegateCommand(LoadSecondFile);
            SaveChangesCommand = new DelegateCommand(SaveChanges);
            InsertByteCommand = new DelegateCommand(InsertByte);
            FindDifferencesCommand = new DelegateCommand(FindDifferences);
            PatchFileCommand = new DelegateCommand(PatchFile);
            PreviousPageCommand = new DelegateCommand(PreviousPage);
            NextPageCommand = new DelegateCommand(NextPage);

            Differences = new ObservableCollection<BlockListItem>();
            FirstFileEditor = new HexEditor();
            SecondFileEditor = new HexEditor();
        }

        private void LoadFirstFile()
        {
            // TODO: 实现加载第一个文件的逻辑
        }

        private void LoadSecondFile()
        {
            // TODO: 实现加载第二个文件的逻辑
        }

        private void SaveChanges()
        {
            // TODO: 实现保存更改的逻辑
        }

        private void InsertByte()
        {
            // TODO: 实现插入字节的逻辑
        }

        private void FindDifferences()
        {
            // TODO: 实现查找差异的逻辑
        }

        private void PatchFile()
        {
            // TODO: 实现修补文件的逻辑
        }

        private void PreviousPage()
        {
            // TODO: 实现上一页的逻辑
        }

        private void NextPage()
        {
            // TODO: 实现下一页的逻辑
        }
    }
}