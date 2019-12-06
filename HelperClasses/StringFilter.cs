using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfPhotoViewer.HelperClasses
{
    public class StringFilter
    {
        public static List<string> FilterFiles(string[] filenames)
        {
            List<string> list = new List<string>();
            string[] extensions = { ".jpg", ".jpeg", ".png", ".ico", ".gif", ".tiff", ".bmp" };
            foreach (var file in filenames)
            {
                if (extensions.Any(x => file.ToLower().EndsWith(x)))
                {
                    list.Add(file);
                }
            }
            return list;
        }
    }
}
