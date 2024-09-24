namespace Bin_Obj_Delete_Project.Models
{
    public class DelMatchingInfo
    {
        #region [DelMatchingInfo] 모델

        /// <summary>
        /// [삭제할 폴더 이름]
        /// </summary>
        public string DelMatchingName { get; set; }

        /// <summary>
        /// [삭제할 폴더 생성 일자]
        /// </summary>
        public string DelMatchingCreationTime { get; set; }

        /// <summary>
        /// [삭제할 폴더 유형]
        /// </summary>
        public string DelMatchingCategory { get; set; }

        /// <summary>
        /// [삭제할 폴더 수정 일자]
        /// </summary>
        public string DelMatchingModifiedTime { get; set; }

        /// <summary>
        /// [삭제할 폴더 크기]
        /// </summary>
        public long DelMatchingOfSize { get; set; }

        /// <summary>
        /// [삭제할 폴더 경로]
        /// </summary>
        public string DelMatchingPath { get; set; }

        #endregion

        /// <summary>
        /// [DeleteFolderInfo 생성자]
        /// </summary>
        public DelMatchingInfo()
        {

        }

    }

}
