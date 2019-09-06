using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace MoskvinWorkers
{
    public class Files
    {
        private string _lastSessionPath = "Settings\\LastSession\\LastSession.xml";
        
        /// <summary>
        /// Путь к файлу с последней сесии.
        /// </summary>
        public string LastSessionPath
        {
            get { return _lastSessionPath; }
        }
        
        /// <summary>
        /// Коллекция файлов.
        /// </summary>
        public ObservableCollection<FileModel> FileList { get; set; }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="IsLastSession">Параметр загрузки пердыдущей сесии.</param>
        public Files(bool IsLastSession)
        {
            FileList = new ObservableCollection<FileModel>(); // Создать коллекцию файлов.
            
            if (IsLastSession)
            {
                LoadLastSession();
            }
        }

        /// <summary>
        /// Добавить файл в список.
        /// </summary>
        /// <param name="path">Путь к файлу.</param>
        public void AddNewFile(string path)
        {
            if (!IsRepeateFile(path))
            {
                try
                {
                    FileList.Add(new FileModel()
                    {
                        FileID = FileList.Count + 1,
                        FileType = IsFile(path),
                        FileName = GetFileName(path),
                        FilePath = path,
                        FileTypeImage = GetImageFile(IsFile(path)),
                        PathToCopy = new ObservableCollection<string>()
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Вы не можете добавить этот файл в список, так как\nон фигурирует в списке файлов.", "Такой файл уже есть", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Удалить файл из списка.
        /// </summary>
        /// <param name="fileID">Номер файла в списке.</param>
        public void RemoveFile(int fileID)
        {
            try
            {
                FileList.RemoveAt(fileID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Изменить выбранный файл в списке.
        /// </summary>
        /// <param name="fileId">Номер редактируемого файла.</param>
        /// <param name="newPath">Новый путь к файлу.</param>
        public void EditFile(int fileId, string newPath)
        {
            if (!IsRepeateFile(newPath))
            {
                try
                {
                    FileList[fileId].FileName = GetFileName(newPath);
                    FileList[fileId].FilePath = newPath;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Вы не можете добавить этот файл в список, так как\nон уже есть в списке.\nПожалуйста, выберите другой файл.", "Такой файл уже есть",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Добавить путь для копирования.
        /// </summary>
        /// <param name="fileId">Номер файла, к которому добавляется путь.</param>
        /// <param name="path">Добавляемый путь для копирования.</param>
        public void AddPath(int fileId, string path)
        {
            if (FileList.Count == 0)
            {
                MessageBox.Show("Вы не можете добавить этот путь, так как список не заполнен.\nУкажите хотя бы один файл", "Нет файлов для заполнения",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (!IsRepeateSyncPath(fileId, path))
                {
                    try
                    {
                        FileList[fileId].PathToCopy.Add(path); // Добавили путь к обновляемому файлу.
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Вы не можете добавить этот путь, так как\nон уже в списке.", "Такой путь уже есть",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Удалить путь для копирования.
        /// </summary>
        /// <param name="fileId">Номер файла, в котором удаляется путь.</param>
        /// <param name="path">Удаляемый путь для копирования.</param>
        public void RemovePath(int fileId, string path)
        {
            for (int i = 0; i < FileList[fileId].PathToCopy.Count; i++)
            {
                if (FileList[fileId].PathToCopy[i].Contains(path.ToString()))
                {
                    FileList[fileId].PathToCopy.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Редактировать путь для копирования.
        /// </summary>
        /// <param name="fileId">Номер файла, в котором редактируется путь.</param>
        /// <param name="oldPath">Редактируемый путь для копирования.</param>
        /// <param name="newPath">Новый путь для копирования.</param>
        public void EditPath(int fileId, string oldPath, string newPath)
        {
            if (IsRepeateSyncPath(fileId, newPath))
            {
                MessageBox.Show("Вы не можете добавить этот путь, так как\nон уже в списке.", "Такой путь уже есть",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            for (int i = 0; i < FileList[fileId].PathToCopy.Count; i++)
            {
                if (FileList[fileId].PathToCopy[i].Contains(oldPath))
                {
                    FileList[fileId].PathToCopy[i] = newPath;
                }
            }
        }

        /// <summary>
        /// Сохранить эту сессию.
        /// </summary>
        public void SaveSession()
        {
            XDocument xDoc = new XDocument();
            XElement files = new XElement("Files");

            foreach (FileModel file in FileList)
            {
                XElement fileElement = new XElement("File");
                XAttribute fileElementName = new XAttribute("Name", file.FilePath);
                fileElement.Add(fileElementName);

                XElement syncPathes = new XElement("Syncronizes");
                foreach (string path in file.PathToCopy)
                {
                    XElement fileSyncPath = new XElement("SyncPath", path);
                    syncPathes.Add(fileSyncPath);
                }
                fileElement.Add(syncPathes);
                files.Add(fileElement);
            }

            xDoc.Add(files);
            xDoc.Save(LastSessionPath);
        }

        /// <summary>
        /// Загрузить последнюю сессию.
        /// </summary>
        private void LoadLastSession()
        {
            FileModel file;

            try
            {
                XDocument xDoc = XDocument.Load(LastSessionPath);
                foreach (XElement fileElement in xDoc.Element("Files").Elements("File"))
                {
                    file = new FileModel();
                    XAttribute fileName = fileElement.Attribute("Name"); // Получаем имя файла.
                    file.FileID = FileList.Count + 1;                    
                    file.FileName = GetFileName(fileName.Value);
                    file.FilePath = fileName.Value;
                    file.FileType = IsFile(fileName.Value);
                    file.FileTypeImage = GetImageFile(file.FileType);
                    file.PathToCopy = new ObservableCollection<string>();

                    foreach (XElement syncElement in fileElement.Elements("Syncronizes"))
                    {
                        foreach (string path in syncElement.Elements("SyncPath"))
                        {
                            file.PathToCopy.Add(path);
                        }
                    }
                    FileList.Add(file); // Добавляем в список файлов.
                }
            }
            catch
            {
                MessageBox.Show("Не найден файл с последней сесии. Программа будет загружена по умолчанию.", 
                    "Нет последней сесии", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Exclamation);
            }
        }

        /// <summary>
        /// Задать картинку типа.
        /// </summary>
        /// <param name="type">Код типа.</param>
        /// <returns>Возвращает иконку типа файла.</returns>
        private BitmapImage GetImageFile(int type)
        {
            if (type == 0)
            {
                return new BitmapImage(new Uri(@"pack://application:,,,/Image/png/Doc.png"));
            }
            else
            {
                return new BitmapImage(new Uri(@"pack://application:,,,/Image/png/folder.ico"));
            }
        }

        /// <summary>
        /// Получить имя файла.
        /// </summary>
        /// <param name="path">Путь к файлу, который будет синхронизироваться.</param>
        /// <returns>Возвращает имя файла без путя.</returns>
        private string GetFileName(string path)
        {
            string[] words = path.Split('\\');
            return words[words.Length - 1];
        }

        /// <summary>
        /// Проверить на существование файла в коллекции.
        /// </summary>
        private bool IsRepeateFile(string filePath)
        {
            /*
             * Проверяется путь к файлу в списке.
             * Если такой путь уже есть в списке файлов, то возвращается true и файл не добавляется.
             * Иначе будет возвращено false и файл добавляется.
             */

            bool result = false;
            foreach (FileModel file in FileList)
            {
                if (file.FilePath.Contains(filePath))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Проверить путь на существование в списке синзронизационных путей.
        /// </summary>
        private bool IsRepeateSyncPath(int fileID, string filePath)
        {
            bool result = false;

            if (FileList[fileID].PathToCopy.Contains(filePath))
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Получить информацию об одном из путей синхронизации.
        /// </summary>
        /// <param name="path">Путь к файлу.</param>
        public void GetSyncFileInfo(string path)
        {
            string result = null;
            if (IsFile(path) == 0) // Это файл.
            {
                FileInfo fileInfo = new FileInfo(path);
                DateTime timeOfWriting = fileInfo.LastWriteTime; // Время последнего сохраненного измения.
                result = String.Format("Дата изменения: {0}\nВремя изменения: {1}\nРазмер: {2} кб",
                    timeOfWriting.ToShortDateString(),
                    timeOfWriting.ToShortTimeString(),
                    fileInfo.Length / 1024);
            }
            else
            {
                DirectoryInfo folderInfo = new DirectoryInfo(path);
                DateTime timeOfWriteting = folderInfo.LastWriteTime; // Время последнего сохраненного измения.

                result = String.Format("Расположение: {2}\nДата изменения: {0}\nВремя изменения: {1}",
                    timeOfWriteting.ToShortDateString(),
                    timeOfWriteting.ToShortTimeString(),
                    path);
            }            
            MessageBox.Show(result, "Информация о файле", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Получить информацию файле из списка.
        /// </summary>
        /// <param name="path">Путь к файлу.</param>
        public void GetFileInfo(int fileId)
        {
            FileInfo fileInfo = new FileInfo(FileList[fileId].FilePath);
            DateTime timeOfWriting = fileInfo.LastWriteTime; // Время последнего сохраненного измения.
            string path = FileList[fileId].FilePath;
            string result = String.Format("Расположение: {3}\nДата изменения: {0}\nВремя изменения: {1}\nРазмер: {2} кб", timeOfWriting.ToShortDateString(), timeOfWriting.ToShortTimeString(), fileInfo.Length/1024, path);
            MessageBox.Show(result, "Информация о файле", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Получить информацию папке из списка.
        /// </summary>
        public void GetFolderInfo(int fileId)
        {
            DirectoryInfo folderInfo = new DirectoryInfo(FileList[fileId].FilePath);
            DateTime timeOfWriteting = folderInfo.LastWriteTime; // Время последнего сохраненного измения.

            string path = FileList[fileId].FilePath;
            string result = String.Format("Расположение: {2}\nДата изменения: {0}\nВремя изменения: {1}",
                timeOfWriteting.ToShortDateString(),
                timeOfWriteting.ToShortTimeString(),
                path);
            MessageBox.Show(result, "Информация о файле", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Определить тип: файл или папка.
        /// </summary>
        /// <returns>0-файл\n1-папка.</returns>
        private int IsFile(string path)
        {
            return (File.Exists(path)) ? 0 : 1;
        }
    }
}
