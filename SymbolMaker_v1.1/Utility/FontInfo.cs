using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymbolMaker
{
    public class FontInfo
    {
        public string FamilyName { get; set; }
        public float Size { get; set; }
        public FontStyle Style { get; set; }


        public Font ToFont()
        {
            return new Font(FamilyName, Size, Style);
        }

        public static FontInfo FromFont(Font font)
        {
            return new FontInfo
            {
                FamilyName = font.FontFamily.Name,
                Size = font.Size,
                Style = font.Style
            };
        }
    }
}
