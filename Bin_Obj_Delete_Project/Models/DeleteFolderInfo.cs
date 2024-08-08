using Bin_Obj_Delete_Project.ViewModels;

namespace Bin_Obj_Delete_Project.Models
{
    public class DeleteFolderInfo : MainVM
    {
        /// <summary>
        /// [삭제할 폴더 경로]
        /// </summary>
        public string DelFolderPath { get; set; }

        /// <summary>
        /// [삭제할 폴더 이름]
        /// </summary>
        public string DelFolderName { get; set; }

        /// <summary>
        /// [삭제할 폴더 생성 일자]
        /// </summary>
        public string DelFolderCreationTime { get; set; }

        /// <summary>
        /// [삭제할 폴더 접근 일자]
        /// </summary>
        public string DelFolderAccessedTime { get; set; }

        /// <summary>
        /// [삭제할 폴더 수정 일자]
        /// </summary>
        public string DelFolderModifiedTime { get; set; }

        /// <summary>
        /// [삭제할 폴더 유형]
        /// </summary>
        public string DelFolderCategory { get; set; }

        /// <summary>
        /// [삭제할 폴더 크기]
        /// </summary>
        public string DelFolderSize { get; set; }

        /// <summary>
        /// [DeleteFolderInfo 생성자]
        /// </summary>
        public DeleteFolderInfo() { }

    }

}
