using System.ComponentModel;
using System.Windows;

namespace MoskvinWorkers
{
    public class WindowSize : INotifyPropertyChanged
    {
        private double _height; // Высота окна.
        private double _width; // Ширина окна.
        private double _top; // Отступ сверху.
        private double _left; // Отступ слева.
        private WindowState _state; // Состояние окна.

        /// <summary>
        /// Высота окна.
        /// </summary>
        public double Height
        {
            get { return _height; }
            set
            {
                _height = value;
                OnPropertyChanged("Height");
            }
        }

        /// <summary>
        /// Ширина окна.
        /// </summary>
        public double Width
        {
            get { return _width; }
            set
            {
                _width = value;
                OnPropertyChanged("Width");
            }
        }

        /// <summary>
        /// Отступ сверху.
        /// </summary>
        public double Top
        {
            get { return _top; }
            set
            {
                _top = value;
                OnPropertyChanged("Top");
            }
        }

        /// <summary>
        /// Отступ слева.
        /// </summary>
        public double Left
        {
            get { return _left; }
            set
            {
                _left = value;
                OnPropertyChanged("Left");
            }
        }

        /// <summary>
        /// Состояние окна.
        /// </summary>
        public WindowState State
        {
            get { return _state; }
            set
            {
                _state = value;
                OnPropertyChanged("State");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
