using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using WpfPhotoViewer.HelperClasses;
namespace WpfPhotoViewer
{
    class HomeViewModel : INotifyPropertyChanged, IDragDropHandler
    {
        private static readonly HomeViewModel instance = new HomeViewModel();
        public RelayCommand ReturnToListCommand { get; set; }
        public RelayCommand ShowLeftPictureCommand { get; set; }
        public RelayCommand ShowRightPictureCommand { get; set; }
        public RelayCommand SelectPictureCommand { get; set; }
        public RelayCommand ClearListViewCommand { get; set; }
        public RelayCommand SetBlurCommand { get; set; }

        public static HomeViewModel Instance { get { return instance; } }

        public ObservableCollection<ImageData> ImageDataCollection { get; set; }

        public HomeViewModel()
        {
            ListVisible = true;
            ImageDataCollection = new ObservableCollection<ImageData>();
            ReturnToListCommand = new RelayCommand(ShowList);
            ShowLeftPictureCommand = new RelayCommand(ShowLeftPicture);
            ShowRightPictureCommand = new RelayCommand(ShowRightPicture);
            SelectPictureCommand = new RelayCommand(SelectPicture);
            ClearListViewCommand = new RelayCommand(ClearListView);
            SetBlurCommand = new RelayCommand(SetBlur);
        }

        private int blurRadius;
        public int BlurRadius
        {
            get { return blurRadius; }
            set
            {
                blurRadius = value;
                NotifyPropertyChanged("BlurRadius");
            }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyPropertyChanged("Name");
            }
        }

        private ImageSource imageSource;
        public ImageSource ImageSource
        {
            get { return imageSource; }
            set
            {
                imageSource = value;
                NotifyPropertyChanged("ImageSource");
            }
        }

        private ImageData imageDataSelectedItem;
        public ImageData ImageDataSelectedItem
        {
            get { return imageDataSelectedItem; }
            set
            {
                imageDataSelectedItem = value;
                NotifyPropertyChanged("ImageDataSelectedItem");
            }
        }

        private bool listVisible;
        public bool ListVisible
        {
            get { return listVisible; }
            set
            {
                listVisible = value;
                NotifyPropertyChanged("ListVisible");
            }
        }

        public void ShowList(object obj)
        {
            ListVisible = true;
        }

        async public void SelectPicture(object obj)
        {
            if (ImageDataSelectedItem == null) return;
            ImageSource = await Task.Run(() => ImageLoader.Loadfile(ImageDataSelectedItem.FileName));
            SetCountString();
            ListVisible = false;
        }

        async public void ShowLeftPicture(object obj)
        {
            var index = ImageDataCollection.IndexOf(ImageDataSelectedItem);
            if (index == 0) return;
            index--;
            ImageDataSelectedItem = ImageDataCollection[index];
            ImageSource = await Task.Run(() => ImageLoader.Loadfile(ImageDataSelectedItem.FileName));
            SetCountString();
        }

        async public void ShowRightPicture(object obj)
        {
            var index = ImageDataCollection.IndexOf(ImageDataSelectedItem);
            if (index == ImageDataCollection.Count - 1) return;
            index++;
            ImageDataSelectedItem = ImageDataCollection[index];
            ImageSource = await Task.Run(() => ImageLoader.Loadfile(ImageDataSelectedItem.FileName));
            SetCountString();
        }

        public void ClearListView(object obj)
        {
            ImageDataCollection.Clear();
            ListVisible = true;
        }

        public void SetBlur(object obj)
        {
            int i = Convert.ToInt32(obj);
            BlurRadius = i;
        }

        public async Task LoadImages()
        {
            foreach (var item in ImageDataCollection)
            {
                await item.Load();
            }
        }

        public async Task LoadImages(List<string> list)
        {
            foreach (var file in list)
            {
                var item = new ImageData { FileName = file };
                ImageDataCollection.Add(item);
                await item.Load();
            }
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

        public bool CanDrop(IDataObject dropObject, IEnumerable dropTarget)
        {
            return true;
        }

        public async void OnDrop(IDataObject dropObject, IEnumerable dropTarget)
        {
            string[] filenames = (string[])dropObject.GetData(DataFormats.FileDrop, true);
            var list = StringFilter.FilterFiles(filenames);
            await LoadImages(list);
        }

        public void SetCountString()
        {
            var i = ImageDataCollection.IndexOf(ImageDataSelectedItem);
            Name = String.Format("picture {0} selected of {1} pictures", ++i, ImageDataCollection.Count);
        }
    }
}
