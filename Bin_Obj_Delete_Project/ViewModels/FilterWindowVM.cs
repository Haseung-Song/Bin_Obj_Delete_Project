namespace Bin_Obj_Delete_Project.ViewModels
{
    public class FilterWindowVM : MainVM
    {
        #region [프로퍼티]

        /// <summary>
        /// [_searchFolderPath]
        /// </summary>
        private string _searchFolderPath;



        #endregion

        #region [OnPropertyChanged]

        /// <summary>
        /// [SearchFolderPath]
        /// </summary>
        public string SearchFolderPath
        {
            get => _searchFolderPath;
            set
            {
                if (_searchFolderPath != value)
                {
                    _searchFolderPath = value;
                    OnPropertyChanged();
                }

            }

        }
        #endregion
    }

}
