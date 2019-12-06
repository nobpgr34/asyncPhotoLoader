using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WpfPhotoViewer
{
    class ImageLoader
    {
        public static BitmapFrame Loadfile(string filename)
        {
            using (var fileStream = new FileStream(
                filename, FileMode.Open, FileAccess.Read))
            {
                var b = BitmapFrame.Create(
                     fileStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                return b;
            }
        }
    }
}
