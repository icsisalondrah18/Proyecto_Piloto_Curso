using System.Globalization;

namespace Proyecto_Piloto_Curso.Helpers
{
    public static class FormatHelper
    {
        public static string ToUSD(this decimal precio)
        {
            return precio.ToString("C", CultureInfo.GetCultureInfo("en-US"));
        }

        public static string ToUSD(this double precio)
        {
            return precio.ToString("C", CultureInfo.GetCultureInfo("en-US"));
        }

        public static string ToUSD(this int precio)
        {
            return precio.ToString("C", CultureInfo.GetCultureInfo("en-US"));
        }
    }
}