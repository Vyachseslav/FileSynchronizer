using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace MoskvinWorkers
{
    public class FileModel : INotifyPropertyChanged
    {
        private int _fileId; // порядковый номер файла.
        private int _fileType; // Тип файла (0-один файл, 1-папка с файлами).
        private BitmapImage _fileTypeImg; // Картика по типу файла.
        private string _fileName; // Имя файла.
        private string _filePath; // Путь к файлу.
        private ObservableCollection<string> _pathToCopy; // Список путей, по которым файл будет скопирован.

        /// <summary>
        /// Номер обновляемого файла.
        /// </summary>
        public int FileID
        {
            get
            {
                return _fileId;
            }
            set
            {
                _fileId = value;
                OnPropertyChanged("FileID");
            }
        }

        /// <summary>
        /// Тип файла (0-один файл, 1-папка с файлами).
        /// </summary>
        public int FileType
        {
            get
            {
                return _fileType;
            }
            set
            {
                _fileType = value;
                OnPropertyChanged("FileType");
            }
        }

        /// <summary>
        /// Картика по типу файла.
        /// </summary>
        public BitmapImage FileTypeImage
        {
            get
            {
                return _fileTypeImg;
            }
            set
            {
                _fileTypeImg = value;
                OnPropertyChanged("FileTypeImage");
            }
        }

        /// <summary>
        /// Имя обновляемого файла.
        /// </summary>
        public string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
                OnPropertyChanged("FileName");
            }
        }

        /// <summary>
        /// Путь обновляемого файла.
        /// </summary>
        public string FilePath
        {
            get
            {
                return _filePath;
            }
            set
            {
                _filePath = value;
                OnPropertyChanged("FilePath");
            }
        }

        /// <summary>
        /// Список путей для копирования этого файла.
        /// </summary>
        public ObservableCollection<string> PathToCopy
        {
            get
            {
                return _pathToCopy;
            }
            set
            {
                _pathToCopy = value;
                OnPropertyChanged("PathToCopy");
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
