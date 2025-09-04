namespace Bin_Obj_Delete_Project.Models
{
    public class DelMatchingInfo
    {
        #region [DelMatchingInfo] 모델

        /// <summary>
        /// [삭제할 폴더/파일 이름]
        /// </summary>
        public string DelMatchingName { get; set; }

        /// <summary>
        /// [삭제할 폴더/파일 생성 일자]
        /// </summary>
        public string DelMatchingCreationTime { get; set; }

        /// <summary>
        /// [삭제할 폴더/파일 유형]
        /// </summary>
        public string DelMatchingCategory { get; set; }

        /// <summary>
        /// [삭제할 폴더/파일 수정 일자]
        /// </summary>
        public string DelMatchingModifiedTime { get; set; }

        /// <summary>
        /// [삭제할 폴더/파일 크기]
        /// </summary>
        public long DelMatchingOfSize { get; set; }

        /// <summary>
        /// [삭제할 폴더/파일 경로]
        /// </summary>
        public string DelMatchingPath { get; set; }

        /// <summary>
        /// [정렬할 폴더/파일 순서] - 초기 로드 순서!
        /// </summary>
        public int OriginalIndex { get; set; }

        #endregion

        /// <summary>
        /// [DeleteFolderInfo 생성자]
        /// </summary>
        public DelMatchingInfo()
        {

        }

    }

}
