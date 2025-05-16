using Microsoft.VisualBasic.FileIO;
using System.Threading.Tasks;

namespace Bin_Obj_Delete_Project.Services
{
    public interface IDeleteService
    {
        Task<bool> DeleteAsync(string dir, bool useRecycleBin);
    }

    public class DeleteService : IDeleteService
    {
        public async Task<bool> DeleteAsync(string dir, bool useRecycleBin)
        {
            return await Task.Run(() =>
            {
                // 해당 디렉토리의 경로가 존재할 때,
                if (FileSystem.DirectoryExists(dir))
                {
                    // 지정한 디렉토리 및 해당 디렉토리의 하위 디렉토리 및 폴더 휴지통 또는 영구적으로 삭제
                    FileSystem.DeleteDirectory(dir, UIOption.OnlyErrorDialogs, useRecycleBin ? RecycleOption.SendToRecycleBin : RecycleOption.DeletePermanently);
                    return true;
                }
                // 해당 파일 경로 존재 시,
                else if (FileSystem.FileExists(dir))
                {
                    // 지정한 파일 휴지통 또는 영구적으로 삭제
                    FileSystem.DeleteFile(dir, UIOption.OnlyErrorDialogs, useRecycleBin ? RecycleOption.SendToRecycleBin : RecycleOption.DeletePermanently);
                    return true;
                }
                return false;
            });

        }

    }

}
