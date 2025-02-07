using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bin_Obj_Delete_Project.Services
{
    public interface IFilteringService
    {
        IEnumerable<DirectoryInfo> FilterFolderName(IEnumerable<DirectoryInfo> directories, string[] folderNameFilter);

        IEnumerable<FileInfo> FilterExtensions(IEnumerable<FileInfo> files, string[] extensionsFilter);
    }

    public class FilteringService : IFilteringService
    {
        /// <summary>
        /// [FolderName] 기준으로 폴더 필터링!
        /// </summary>
        /// <param name="directories">필터링할 디렉토리 목록</param>
        /// <param name="folderNameFilter">콤마(',')로 구분된 폴더 이름 필터링 문자열 배열</param>
        /// <returns>필터링된 디렉토리 목록</returns>
        public IEnumerable<DirectoryInfo> FilterFolderName(IEnumerable<DirectoryInfo> directories, string[] filterFolderName)
        {
            if (directories == null || !directories.Any())
            {
                return Enumerable.Empty<DirectoryInfo>();
            }
            if (filterFolderName?.Length == 0)
            {
                return directories;
            }
            filterFolderName = filterFolderName.Select(filter => filter.Trim()).Where(filter => !string.IsNullOrEmpty(filter)).ToArray();
            return directories.Where(dir => filterFolderName.Any(filter => dir.Name.Equals(filter, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// [Extensions] 기준으로 파일 필터링!
        /// </summary>
        /// <param name="files"></param>
        /// <param name="extensionsFilter"></param>
        /// <returns></returns>
        public IEnumerable<FileInfo> FilterExtensions(IEnumerable<FileInfo> files, string[] filterExtensions)
        {
            throw new NotImplementedException();
        }

    }

}
