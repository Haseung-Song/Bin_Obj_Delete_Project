using System;
using System.Globalization;
using System.Windows.Data;

namespace Bin_Obj_Delete_Project.Converter
{
    public class FormatConverter : IValueConverter
    {
        /// <summary>
        /// [Convert: {long}형 to {string}형 변환]
        /// [디렉토리 크기] 지정된 출력 포맷 함수!
        /// [소수 첫째자리] (1의 자리)까지만 출력!
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is long bytes)
            {
                string[] sizeofKind = { "B", "KB", "MB", "GB", "TB" };
                double len = bytes;
                int i = 0;
                while (len >= 1024 && i < sizeofKind.Length - 1)
                {
                    i++;
                    len /= 1024;
                }
                return len == 0 ? len.ToString("F0") + "바이트" : len.ToString("F1") + sizeofKind[i]; // 값이 {long}형일 경우, 단위에 따른 바이트 값을 출력!
            }
            return ""; // 값이 {long}형이 아닐 경우, 공백을 출력!
        }

        /// <summary>
        /// [ConvertBack: {string}형 to {long}형 역변환]
        /// [추가적으로 학습이 필요한 부분으로 공부하기]
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string strValue)
            {
                string[] sizeofKind = { "B", "KB", "MB", "GB", "TB" };
                double multiplier = 1;
                // 공백 기준으로 [숫자 부분]과 [단위 부분] 분리!
                string[] partofStr = strValue.Trim().Split(' ');
                if (partofStr.Length == 2 && double.TryParse(partofStr[0], out double sizeofValue))
                {
                    string unitofStr = partofStr[1].ToUpperInvariant();
                    for (int i = 0; i < sizeofKind.Length; i++)
                    {
                        if (unitofStr == sizeofKind[i])
                        {
                            multiplier = Math.Pow(1024, i); // 단위에 따라서 [multiplier] 설정 가능
                            break;
                        }

                    }
                    return (long)(sizeofValue * multiplier); // 변환 성공 시, [숫자] * [단위] 기준 {바이트 값} 반환
                }

            }
            return 0L; // 변환 실패 시, {기본값} 반환
        }

    }

}
