using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfPhotoViewer
{
    public class ImageData : INotifyPropertyChanged
    {
        public string FileName { get; set; }
        private string _Title = string.Empty;
        public string Title
        {
            get { return _Title; }
            set
            {
                _Title = value;
                NotifyPropertyChanged("Title");
            }
        }

        private ImageSource imageDataItem;
        public ImageSource ImageDataItem
        {
            get { return imageDataItem; }
            set
            {
                imageDataItem = value;
                NotifyPropertyChanged("ImageDataItem");
            }
        }

        public async Task Load()
        {
            Title = Path.GetFileNameWithoutExtension(FileName);
            BitmapFrame data = await Task.Run(() => ImageLoader.Loadfile(FileName));
            if (data.Thumbnail == null )
            {
                ImageDataItem = data;
                return;
            }
            ImageDataItem = data.Thumbnail;
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this,
                        new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
