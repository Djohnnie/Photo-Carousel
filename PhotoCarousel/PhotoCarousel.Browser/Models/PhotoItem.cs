using System;
using PhotoCarousel.Browser.ViewModels;
using PhotoCarousel.Enums;

namespace PhotoCarousel.Browser.Models
{
    internal class PhotoItem : ViewModelBase
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Rating Rating { get; set; }


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


        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }
    }
}