using Bin_Obj_Delete_Project.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Bin_Obj_Delete_Project.Common
{
    public class CommonProperty : INotifyPropertyChanged
    {
        /// <summary>
        /// [AbsolutePath]
        /// </summary>
        public static string AbsolutePath { get; set; }

        /// <summary>
        /// [_IssTheBtnEnabledOrNot]
        /// </summary>
        private bool _IsTheBtnEnabledOrNot;

        /// <summary>
        /// [uniqueFilePathSet]
        /// </summary>
        protected readonly HashSet<string> uniqueFilePathSet;

        /// <summary>
        /// [_folderNameFiltered]
        /// </summary>
        private string _folderNameFiltered;

        /// <summary>
        /// [_extensionsFiltered]
        /// </summary>
        private string _extensionsFiltered;

        /// <summary>
        /// [selectToDelete]
        /// </summary>
        protected List<DelMatchingInfo> selectToDelete;

        /// <summary>
        /// [entireToDelete]
        /// </summary>
        protected List<DelMatchingInfo> entireToDelete;

        /// <summary>
        /// [_selectFolderInfo]
        /// </summary>
        private ObservableCollection<DelMatchingInfo> _selectFolderInfo;

        /// <summary>
        /// [_deleteFolderInfo]
        /// </summary>
        private ObservableCollection<DelMatchingInfo> _deleteFolderInfo;

        /// <summary>
        /// [_activeFolderInfo]
        /// </summary>
        private ObservableCollection<DelMatchingInfo> _activeFolderInfo;

        /// <summary>
        /// [_progressValue]
        /// </summary>
        private double _progressValue;

        /// <summary>
        /// [_currentPage]
        /// </summary>
        private int _currentPage;

        /// <summary>
        /// [_pageRecords]
        /// </summary>
        private int _pageRecords;

        /// <summary>
        /// [_aVisibleLoadingOrNot]
        /// </summary>
        private bool _aVisibleLoadingOrNot;

        /// <summary>
        /// [_aVisibleDestroyOrNot]
        /// </summary>
        private bool _aVisibleDestroyOrNot;

        /// <summary>
        /// [TheBtnEnabledOrNot]
        /// </summary>
        public bool TheBtnEnabledOrNot
        {
            get => _IsTheBtnEnabledOrNot;
            set
            {
                if (_IsTheBtnEnabledOrNot != value)
                {
                    _IsTheBtnEnabledOrNot = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [FilterFolderName]
        /// </summary>
        public string FilterFolderName
        {
            get => _folderNameFiltered;
            set
            {
                if (_folderNameFiltered != value)
                {
                    _folderNameFiltered = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [FilterExtensions]
        /// </summary>
        public string FilterExtensions
        {
            get => _extensionsFiltered;
            set
            {
                if (_extensionsFiltered != value)
                {
                    _extensionsFiltered = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [SelectFolderInfo]
        /// </summary>
        public ObservableCollection<DelMatchingInfo> SelectFolderInfo
        {
            get => _selectFolderInfo;
            set
            {
                if (_selectFolderInfo != value)
                {
                    _selectFolderInfo = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [DeleteFolderInfo]
        /// </summary>
        public ObservableCollection<DelMatchingInfo> DeleteFolderInfo
        {
            get => _deleteFolderInfo;
            set
            {
                if (_deleteFolderInfo != value)
                {
                    _deleteFolderInfo = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [ActiveFolderInfo]
        /// [선택 삭제하기] 시 => [ActiveFolderInfo = SelectFolderInfo]
        /// [일괄 삭제하기] 시 => [ActiveFolderInfo = DeleteFolderInfo]
        /// </summary>
        public ObservableCollection<DelMatchingInfo> ActiveFolderInfo
        {
            get => _activeFolderInfo;
            set
            {
                if (_activeFolderInfo != value)
                {
                    _activeFolderInfo = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [ProgressValue]
        /// </summary>
        public double ProgressValue
        {
            get => _progressValue;
            set
            {
                if (_progressValue != value)
                {
                    _progressValue = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [CurrentPage]
        /// [현재 페이지]
        /// </summary>
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [PageRecords]
        /// [페이지 당 데이터 개수]
        /// </summary>
        public int PageRecords
        {
            get => _pageRecords = 100;
            set
            {
                if (_pageRecords != value)
                {
                    _pageRecords = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [VisibleLoading]
        /// </summary>
        public bool VisibleLoading
        {
            get => _aVisibleLoadingOrNot;
            set
            {
                if (_aVisibleLoadingOrNot != value)
                {
                    _aVisibleLoadingOrNot = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [VisibleDestroy]
        /// </summary>
        public bool VisibleDestroy
        {
            get => _aVisibleDestroyOrNot;
            set
            {
                if (_aVisibleDestroyOrNot != value)
                {
                    _aVisibleDestroyOrNot = value;
                    OnPropertyChanged();
                }

            }

        }

        public CommonProperty()
        {
            VisibleLoading = false;
            VisibleDestroy = false;
            uniqueFilePathSet = new HashSet<string>();
            SelectFolderInfo = new ObservableCollection<DelMatchingInfo>();
            DeleteFolderInfo = new ObservableCollection<DelMatchingInfo>();
            ActiveFolderInfo = new ObservableCollection<DelMatchingInfo>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

}
