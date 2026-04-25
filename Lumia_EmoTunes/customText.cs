using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Lumia_EmoTunes
{
    internal class CustomText
    {
        private static PrivateFontCollection _pfc = new PrivateFontCollection();

        public static void LoadFont()
        {
            byte[] fontData = Properties.Resources.Iceberg_Regular;

            IntPtr fontPtr = Marshal.AllocCoTaskMem(fontData.Length);
            Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
            _pfc.AddMemoryFont(fontPtr, fontData.Length);

        }

        public static void ApplyToForm(Control container, float size)
        {
            if (_pfc.Families.Length == 0) return;

            Font newFont = new Font(_pfc.Families[0], size, FontStyle.Regular);
            container.Font = newFont;

            foreach (Control c in container.Controls)
            {
                c.Font = newFont;
                if (c.HasChildren) ApplyToForm(c, size);
            }
        }
    }
}
