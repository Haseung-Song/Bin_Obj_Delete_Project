using Bin_Obj_Delete_Project.Common;
using Bin_Obj_Delete_Project.Models;
using Bin_Obj_Delete_Project.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Bin_Obj_Delete_Project.Services
{
    public class EnumerateService : CommonProperty
    {
        private readonly MainVM _mainVM;

        /// <summary>
        /// [matchingFileInfoOrNot]
        /// </summary>
        private bool matchingFileInfoOrNot;

        /// <summary>
        /// [matchingFldrName]
        /// </summary>
        private string matchingFldrName;

        /// <summary>
        /// [matchingFileName]
        /// </summary>
        private string matchingFileName;

        /// <summary>
        /// [matchingFldrCreationTime]
        /// </summary>
        private string matchingFldrCreationTime;

        /// <summary>
        /// [matchingFileCreationTime]
        /// </summary>
        private string matchingFileCreationTime;

        /// <summary>
        /// [matchingFldrCategory]
        /// </summary>
        private string matchingFldrCategory;

        /// <summary>
        /// [matchingFileCategory]
        /// </summary>
        private string matchingFileCategory;

        /// <summary>
        /// [matchingFldrModifiedTime]
        /// </summary>
        private string matchingFldrModifiedTime;

        /// <summary>
        /// [matchingFileModifiedTime]
        /// </summary>
        private string matchingFileModifiedTime;

        /// <summary>
        /// [matchingFldrSize]
        /// </summary>
        private long matchingFldrSize;

        /// <summary>
        /// [matchingFileSize]
        /// </summary>
        private long matchingFileSize;

        /// <summary>
        /// [matchingFldrPath]
        /// </summary>
        private string matchingFldrPath;

        /// <summary>
        /// [matchingFilePath]
        /// </summary>
        private string matchingFilePath;

        public EnumerateService()
        {
            matchingFldrName = string.Empty;
            matchingFileName = string.Empty;
            matchingFldrCreationTime = string.Empty;
            matchingFileCreationTime = string.Empty;
            matchingFldrCategory = string.Empty;
            matchingFileCategory = string.Empty;
            matchingFldrModifiedTime = string.Empty;
            matchingFileModifiedTime = string.Empty;
            matchingFldrSize = 0;
            matchingFileSize = 0;
            matchingFldrPath = string.Empty;
            matchingFilePath = string.Empty;
        }

        public EnumerateService(MainVM mainVM)
        {
            _mainVM = mainVM;
        }

        /// <summary>
        /// [비동기적] 폴더 열거(탐색) 및 작업의 진행률 업데이트 및 UI 도시 (기능)
        /// </summary>
        /// <param name="cancellationToken">작업 취소</param>
        /// <param name="progress">작업 진행률</param>
        /// <returns>작업 완료 후, Task 반환</returns>
        public async Task EnumerateFolders(CancellationToken cancellationToken, IProgress<double> fldrProgress, IProgress<double> fileProgress)
        {
            fldrProgress?.Report(0); // [폴더 진행률: 0]으로 초기화
            fileProgress?.Report(0); // [파일 진행률: 0]으로 초기화
            try
            {
                IEnumerable<string> lstEneumerateFldr = await Task.Run(() => _mainVM.GetEneumerateFldrList());
                int totalFldrs = lstEneumerateFldr.Count();
                int processedFldrs = 0;
                foreach (string dir in lstEneumerateFldr)
                {
                    // 작업 취소 요청 (약 180초) 후 작업 취소 수행
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    DirectoryInfo dirInfo = new DirectoryInfo(dir);
                    matchingFldrName = dirInfo.Name;
                    matchingFldrCreationTime = dirInfo.CreationTime.ToString("yyyy-MM-dd tt HH:mm:ss");
                    matchingFldrCategory = "파일 폴더";
                    matchingFldrModifiedTime = dirInfo.LastWriteTime.ToString("yyyy-MM-dd tt HH:mm:ss");
                    matchingFldrSize = await Task.Run(() => MainVM.GetDirectorySize(dir));
                    matchingFldrPath = dir;
                    matchingFileInfoOrNot = false; // [폴더]로 구분

                    // 1. 필터 키워드를 콤마(',')로 구분 후, 배열로 생성 (FilterFolderName)
                    string[] filterComma1 = string.IsNullOrEmpty(FilterFolderName) ? Array.Empty<string>() : FilterFolderName.Split(',');

                    // Filter 01: 폴더 이름으로 검색(대소문자 구분(X))
                    // 1) [FilterFolderName]이 null이거나 string.Empty 문자열인 경우
                    // 2) [FilterFolderName]이 디렉토리 또는 하위 디렉토리 폴더의 이름과 일치하는 경우
                    //bool folderMatches1 = string.IsNullOrEmpty(FilterFolderName) || dirInfo.Name.Equals(FilterFolderName, StringComparison.OrdinalIgnoreCase);

                    //if (!folderMatches1)
                    //{
                    //    continue;
                    //}

                    // [Filter 01]: 폴더 이름으로 검색(대소문자 구분(X))
                    // 지정한 배열에 정의된 조건과 일치하는지 여부 확인!
                    // 1) [FilterFolderName]이 null이거나 string.Empty 문자열인 경우
                    // 2) 콤마(',')로 구분된 [FilterFolderName]이 디렉토리 또는 하위 디렉토리 폴더의 이름과 일치하는 경우
                    bool folderMatches2 = string.IsNullOrEmpty(FilterFolderName) ||
                         Array.Exists(filterComma1, comma1 => dirInfo.Name.Equals(comma1.Trim(), StringComparison.OrdinalIgnoreCase));

                    // 1), 2)가 아닐 때,
                    if (!folderMatches2)
                    {
                        processedFldrs++;
                        fldrProgress.Report((double)processedFldrs / totalFldrs * 100);
                        continue;
                    }

                    // 1. [FilterFolderName] => 해당 폴더 및 정보를 리스트의 형태로 전시!
                    // 즉, 필터링 (X) => 필터링 없이 전시, 필터링 (O) => 필터링해서 전시!
                    if (string.IsNullOrEmpty(FilterExtensions) && !matchingFileInfoOrNot)
                    {
                        DeleteFolderInfo.Add(new DelMatchingInfo
                        {
                            DelMatchingName = matchingFldrName,
                            DelMatchingCreationTime = matchingFldrCreationTime,
                            DelMatchingCategory = matchingFldrCategory,
                            DelMatchingModifiedTime = matchingFldrModifiedTime,
                            DelMatchingOfSize = matchingFldrSize,
                            DelMatchingPath = matchingFldrPath
                        });

                    }

                    // 2. 필터 키워드를 콤마(',')로 구분 후, 배열로 생성 (FilterExtensions)
                    string[] filterComma2 = string.IsNullOrEmpty(FilterExtensions) ? Array.Empty<string>() : FilterExtensions.Split(',');

                    // [Filter 02]: 파일 확장자로 검색(대소문자 구분(X))
                    // 지정한 배열에 정의된 조건과 일치하는지 여부 확인!
                    // 1) [FilterExtensions]가 null이거나 string.Empty 문자열이 아닌 경우: 빈 배열(null)을 반환
                    // 2) [FilterExtensions]가 null이거나 string.Empty 문자열인 경우: 파일 확장자를 콤마(',')로 구분 반환
                    if (!string.IsNullOrEmpty(FilterExtensions))
                    {
                        IEnumerable<FileInfo> lstEnumerateFilesInfo = await Task.Run(() => dirInfo.EnumerateFiles("*", SearchOption.AllDirectories));
                        int totalFiles = lstEnumerateFilesInfo.Count();
                        int processedFiles = 0;
                        matchingFileInfoOrNot = true; // [파일]로 구분됨!
                        foreach (FileInfo files in lstEnumerateFilesInfo)
                        {
                            // 작업 취소 요청 (약 180초) 후 작업 취소 수행
                            if (cancellationToken.IsCancellationRequested)
                            {
                                return;
                            }

                            // 이미 처리가 된 파일 경로는 무시! (중복 제거)
                            if (uniqueFilePathSet.Contains(files.FullName))
                            {
                                continue;
                            }

                            // 2) 콤마(',')로 구분된 [FilterExtensions]이 파일의 확장명 부분의 문자열과 일치하는 경우 (확장자 비교)
                            if (Array.Exists(filterComma2, comma2 => files.Extension.Equals(comma2.Trim(), StringComparison.OrdinalIgnoreCase)))
                            {
                                matchingFileName = files.Name;
                                matchingFileCreationTime = files.CreationTime.ToString("yyyy-MM-dd tt HH:mm:ss");
                                Dictionary<string, string> extensionCategoryMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                                {
                                    { ".7z", "압축(7Z) 파일" },
                                    { ".aac", "오디오 압축 파일" },
                                    { ".avi", "AVI 비디오 파일" },
                                    { ".baml", "BAML 파일" },
                                    { ".bmp", "비압축 비트맵 이미지 파일" },
                                    { ".cache", "CACHE 파일" },
                                    { ".cfg", "CFG 파일" },
                                    { ".cs", "C# Source File" },
                                    { ".csproj", "C# Project File" },
                                    { ".csv", "Microsoft Excel 쉼표로 구분된 값 파일" },
                                    { ".config", "VisualStudio.config" },
                                    { ".dll", "응용 프로그램 확장" },
                                    { ".doc", "Word 97–2003 문서" },
                                    { ".docx", "Word 문서" },
                                    { ".exe", "응용 프로그램" },
                                    { ".flac", "무손실 오디오 파일" },
                                    { ".gitattributes", "txtfile" },
                                    { ".gitignore", "txtfile" },
                                    { ".gif", "GIF 이미지 파일" },
                                    { ".hwp", "한컴오피스 한글 문서" },
                                    { ".jpeg", "JPEG 이미지 파일" },
                                    { ".jpg", "JPEG 이미지 파일" },
                                    { ".md", "MD 파일" },
                                    { ".mp3", "MP3 오디오 파일" },
                                    { ".mp4", "MP4 비디오 파일" },
                                    { ".nupkg", "NUPKG 파일" },
                                    { ".p7s", "PKCS #7 서명" },
                                    { ".pdb", "Program Debug Database" },
                                    { ".pdf", "Microsoft Edge PDF Document" },
                                    { ".png", "PNG 이미지 파일" },
                                    { ".resx", "Microsoft .NET Managed Resource File" },
                                    { ".resources", "RESOURCES 파일" },
                                    { ".sln", "Visual Studio Solution" },
                                    { ".settings", "Settings-Designer File" },
                                    { ".suo", "Visual Studio Solution User Options" },
                                    { ".svg", "벡터 이미지 파일" },
                                    { ".txt", "텍스트 문서" },
                                    { ".wav", "WAV 오디오 파일" },
                                    { ".xaml", "Windows 태그 파일" },
                                    { ".xml", "xmlfile" },
                                    { ".xlsx", "Excel 통합 문서" },
                                    { ".zip", "압축(ZIP) 파일" }
                                };
                                matchingFileCategory = extensionCategoryMap.TryGetValue(files.Extension, out string category) ? category : "기타 파일";
                                matchingFileModifiedTime = files.LastWriteTime.ToString("yyyy-MM-dd tt HH:mm:ss");
                                matchingFileSize = files.Length;
                                matchingFilePath = files.FullName;
                                _ = uniqueFilePathSet.Add(matchingFilePath); // [중복] 제거

                                // 2. [FilterExtensions] => 해당 파일 및 정보를 리스트의 형태로 전시
                                // 즉, 필터링 (X) => 필터링 없이 전시, 필터링 (O) => 필터링해서 전시
                                if (string.IsNullOrEmpty(FilterFolderName) && matchingFileInfoOrNot)
                                {
                                    DeleteFolderInfo.Add(new DelMatchingInfo
                                    {
                                        DelMatchingName = matchingFileName,
                                        DelMatchingCreationTime = matchingFileCreationTime,
                                        DelMatchingCategory = matchingFileCategory,
                                        DelMatchingModifiedTime = matchingFileModifiedTime,
                                        DelMatchingOfSize = matchingFileSize,
                                        DelMatchingPath = matchingFilePath
                                    });

                                }

                            }

                        }
                        processedFiles++;
                        fileProgress?.Report((double)processedFiles / totalFiles * 100);
                    }
                    processedFldrs++;
                    fldrProgress?.Report((double)processedFldrs / totalFldrs * 100);
                }

            }
            catch (UnauthorizedAccessException ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Window mainWindow = Application.Current.MainWindow; // [MainWindow] 가져오기 (Owner 설정용)
                    _ = MessageBox.Show(mainWindow, $"{ex.Message}", "액세스 거부", MessageBoxButton.OK, MessageBoxImage.Error);
                });
                // 경로에 대한 엑세스 거부 오류.
                Console.WriteLine($"Exception: Access Denied to Directories: {ex.Message}");
            }
            catch (DirectoryNotFoundException ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Window mainWindow = Application.Current.MainWindow; // [MainWindow] 가져오기 (Owner 설정용)
                    _ = MessageBox.Show(mainWindow, $"{ex.Message}", "경로 미존재", MessageBoxButton.OK, MessageBoxImage.Error);
                });
                // 경로를 찾을 수 없음.
                Console.WriteLine($"Exception: Directories Not Found: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Window mainWindow = Application.Current.MainWindow; // [MainWindow] 가져오기 (Owner 설정용)
                    _ = MessageBox.Show(mainWindow, $"{ex.Message}", "액세스 거부", MessageBoxButton.OK, MessageBoxImage.Error);
                });
                // 특정 객체에 대한 엑세스 거부 오류.
                Console.WriteLine($"Exception: Access Denied to the Object: {ex.Message}");
            }
            catch (PathTooLongException ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Window mainWindow = Application.Current.MainWindow; // [MainWindow] 가져오기 (Owner 설정용)
                    _ = MessageBox.Show(mainWindow, $"{ex.Message}", "경로 재설정", MessageBoxButton.OK, MessageBoxImage.Error);
                });
                // 경로가 너무 긴 경우.
                Console.WriteLine($"Exception: Path is Too Long: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: : {ex.Message}");
            }
            finally
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    // DelMatchingInfo 정보 확인: 디버깅으로 확인 가능!!
                    //foreach (DelMatchingInfo item in DeleteFolderInfo)
                    //{
                    //    Console.WriteLine(item.DelMatchingName);
                    //    Console.WriteLine(item.DelMatchingCreationTime);
                    //    Console.WriteLine(item.DelMatchingCategory);
                    //    Console.WriteLine(item.DelMatchingModifiedTime);
                    //    Console.WriteLine(item.DelMatchingOfSize);
                    //    Console.WriteLine(item.DelMatchingPath);
                    //}
                    if (ProgressValue < 100)
                    {
                        fldrProgress?.Report(100); // [진행률: 100] 작업 완료
                        fileProgress?.Report(100); // [진행률: 100] 작업 완료
                    }
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        _mainVM.LstAllData = DeleteFolderInfo.ToList(); // [DeleteFolderInfo] 컬렉션 데이터 리스트화 (완료)
                        ActiveFolderInfo = new ObservableCollection<DelMatchingInfo>(_mainVM.LstAllData.Take(PageRecords)); // 페이지 데이터 로드
                        CurrentPage = 1;
                    });

                }

            }

        }

    }

}
