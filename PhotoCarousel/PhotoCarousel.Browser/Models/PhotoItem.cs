using System;
using PhotoCarousel.Browser.ViewModels;

namespace PhotoCarousel.Browser.Models
{
    internal class PhotoItem : ViewModelBase
    {
        public Guid Id { get; set; }
        public string Name { get; set; }


        private byte[] _bitmap;

        public byte[] Bitmap
        {
            get => _bitmap;
            set
            {
                _bitmap = value; 
                OnPropertyChanged();
            }
        }
    }
}