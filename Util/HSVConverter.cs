using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Util
{
    public class HSVConverter
    {
        public static string FromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            ushort v = Convert.ToUInt16(value);
            ushort p = Convert.ToUInt16(value * (1 - saturation));
            ushort q = Convert.ToUInt16(value * (1 - f * saturation));
            ushort t = Convert.ToUInt16(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return RGBToString(v, t, p);
            else if (hi == 1)
                return RGBToString(q, v, p);
            else if (hi == 2)
                return RGBToString(p, v, t);
            else if (hi == 3)
                return RGBToString(p, q, v);
            else if (hi == 4)
                return RGBToString(t, p, v);
            else
                return RGBToString(v, p, q);
        }

        private static string RGBToString(ushort r, ushort g, ushort b)
        {
            string c = "#";

            c += r.ToString("X2");
            c += g.ToString("X2");
            c += b.ToString("X2");

            return c;
        }
    }
}
