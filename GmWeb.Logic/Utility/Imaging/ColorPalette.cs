using System.Collections.Generic;
using System.Linq;

namespace GmWeb.Logic.Utility.Imaging;
// TODO
public class ColorPalette
{
    protected List<VColor> _Colors { get; private set; }
    public IEnumerable<VColor> Colors => this._Colors;
    public ColorPalette(IEnumerable<VColor> colors)
    {
        this._Colors = colors.ToList();
    }

    public ColorPalette Resize(int length)
    {
        for (int i = 1; i < this._Colors.Count; i++)
        {
        }
        return new ColorPalette(this.Colors);
    }
}