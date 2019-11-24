using System;
using System.Text;

namespace Main.Utility
{
    public static class TranslateLanguage
    {
        const string GorillaLanguageTable = "ウホ";
        const string NumberLiteralHeader = "ウッホ";

        static readonly StringBuilder _builder = new StringBuilder();

        // 数字をゴリラ語に変換
        public static string NumberToGorilla(int num)
        {
            var numStr = Convert.ToString(num, 2)
                .Replace('0', GorillaLanguageTable[0])
                .Replace('1', GorillaLanguageTable[1]);

            // 数値リテラルヘッダ + 数値(ゴリラ語表現)
            _builder.Clear();
            _builder.Append(NumberLiteralHeader).Append(numStr);
            return _builder.ToString();
        }
    }
}
