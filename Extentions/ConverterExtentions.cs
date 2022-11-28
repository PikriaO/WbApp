using System;

namespace WbApp.Extentions
{
  public  static class ConverterExtentions
    {
        public static int? ToNullableInt(this object data)
        {
            return data is DBNull ? (int?)null : Convert.ToInt32(data);

        }
        public static decimal? ToNullableDecimal(this object data)
        {
            return data is DBNull ? (decimal?)null : Convert.ToDecimal(data);
        }
        public static double? ToNullableDouble(this object data)
        {
            return data is DBNull ? (double?)null : Convert.ToDouble(data);
        }
        public static string ToNullableString(this object data)
        {
            return data is DBNull ? null : Convert.ToString(data);
        }
        public static DateTime? ToNullableDateTime(this object data)
        {
            return data is DBNull ? (DateTime?)null : Convert.ToDateTime(data);
        }
    }
}
