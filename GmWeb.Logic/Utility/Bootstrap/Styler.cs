using System;
using System.Collections.Generic;
using System.Linq;
using Maths = System.Math;
using System.Text;
using System.Threading.Tasks;

namespace GmWeb.Logic.Utility.Bootstrap
{
    public static class Styler
    {
        public static IEnumerable<string> GenerateClasses(ColWidth intuitiveWidth)
        {
            int size = (int)intuitiveWidth;
            var devices = Enum.GetValues(typeof(Device)).Cast<Device>().ToList();
            foreach(var device in devices)
            {
                if (device == Device.General)
                    continue;
                int deviceSize = Maths.Min(12, Maths.Max((int)device + 1, size + (int)device - 3));
                var cw = (ColWidth)deviceSize;
                var @class = $"col-{device.GetShortName()}-{(int)cw}";
                yield return @class;
            }
        }
    }
}
