using Humanizer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Shared.Utilities
{
    public static class StringUtilities
    {
        public static List<string> Decombined(this string src)
        {
            return Decombined(src, Constant.SPLITTER);
        }
        
        public static List<string> Decombined(this string src, string splitter)
        {
            if (string.IsNullOrEmpty(src))
            {
                return new List<string>();
            }

            return src.Split(new string[] { splitter }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public static string Combined<T>(this IEnumerable<T> source)
        {
            if (source == null || !source.Any())
            {
                return string.Empty;
            }

            return string.Join(Constant.SPLITTER, source);
        }


        private static Regex RemoveQuestionCharacter = new Regex("[?]|[#]|[=]", RegexOptions.Compiled);
        public static string ToLinkName(this string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return name;
            }
            name = RemoveSign4VietnameseString(name);
            name = RemoveQuestionCharacter.Replace(name, "");
            return string.Join("-", name.ToLower().Split(' ').Select(d => d.Trim()));
        }
        
        public static string RemoveSign4VietnameseString(this string str)
        {
            if(string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            for (int i = 1; i < VietnameseSigns.Length; i++)
            {
                for (int j = 0; j < VietnameseSigns[i].Length; j++)
                    str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
            }
            return str;
        }

        public static List<Guid> ToListGuid(this string src, string splitter = ",")
        {
            if (string.IsNullOrEmpty(src))
            {
                return new List<Guid>();
            }

            return src.Split(splitter, System.StringSplitOptions.RemoveEmptyEntries)
                      .Select(d =>
                      {
                          if (Guid.TryParse(d, out Guid guid))
                          {
                              return guid as Guid?;
                          }
                          return null;
                      })
                      .Where(d => d.HasValue)
                      .Select(d => d.Value)
                      .ToList();
        }

        private static readonly string[] VietnameseSigns = new string[]
        {

            "aAeEoOuUiIdDyY",

            "áàạảãâấầậẩẫăắằặẳẵ",

            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",

            "éèẹẻẽêếềệểễ",

            "ÉÈẸẺẼÊẾỀỆỂỄ",

            "óòọỏõôốồộổỗơớờợởỡ",

            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",

            "úùụủũưứừựửữ",

            "ÚÙỤỦŨƯỨỪỰỬỮ",

            "íìịỉĩ",

            "ÍÌỊỈĨ",

            "đ",

            "Đ",

            "ýỳỵỷỹ",

            "ÝỲỴỶỸ"
        };

        public static string ToPluralize(this string src) => src.Humanize().Pluralize();

        public static List<T> Decombined<T>(this string removeOldImages, Func<string, T> func)
        {
            var stringArr = removeOldImages.Decombined();
           
            return stringArr.Select(d => func(d)).ToList();
        }

        public static string DisplayConcurency(this decimal? src, int numberDecimalDigits = 0)
        {
            if (!src.HasValue)
            {
                return string.Empty;
            }

            NumberFormatInfo setPrecision = new NumberFormatInfo();
            setPrecision.NumberDecimalDigits = numberDecimalDigits;
            return src.Value.ToString("N", setPrecision);
        }

        public static string GetEnumDescription(this Enum value)
        {
            System.Reflection.FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
    }
}
