﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bin_Obj_Delete_Project.Services
{
    public interface IEnumerateService
    {
        IEnumerable<string> GetDirectories(string path);

        IEnumerable<FileInfo> GetFiles(DirectoryInfo dirInfo);

        long GetDirectorySize(string dir);
    }

    public class EnumerateService : IEnumerateService
    {
        public IEnumerable<string> GetDirectories(string path)
        {
            try
            {
                // 모든 하위 디렉토리를 검색하되, 접근이 거부된 디렉토리는 제외!
                return Directory.EnumerateDirectories(path, "*", SearchOption.AllDirectories);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return Enumerable.Empty<string>();
            }

        }

        public IEnumerable<FileInfo> GetFiles(DirectoryInfo dirInfo)
        {
            try
            {
                return dirInfo.EnumerateFiles("*", SearchOption.AllDirectories);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return Enumerable.Empty<FileInfo>();
            }

        }

        /// <summary>
        /// [하위 디렉토리] 파일의 총량 계산 (기능)
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public long GetDirectorySize(string dir)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(dir); // DirectoryInfo 객체 생성
            long sizeofDir = 0; // [총량] 초기화
            try
            {
                // 병렬 옵션 설정
                ParallelOptions options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };

                // 병렬 루프 제공
                _ = Parallel.ForEach(dirInfo.GetFiles("*", SearchOption.AllDirectories), options, (files) =>
                {
                    try
                    {
                        _ = Interlocked.Add(ref sizeofDir, files.Length); // 각 파일의 [크기 계산 및 누적]
                    }
                    catch
                    {
                        /* 파일 접근 오류 무시 */
                    }

                });

            }
            catch
            {
                /* 폴더 접근 오류 무시 */
            }
            return sizeofDir; // 누적된 파일 크기 총합(sizeofDir) 반환
        }

    }

}
