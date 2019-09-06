using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace MoskvinWorkers
{
    public class Syncronizing
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        public Syncronizing()
        {

        }

        /// <summary>
        /// Начать синхронизацию списка файлов.
        /// </summary>
        /// <param name="MyFiles">Список файлов.</param>
        public void SyncronizeFileStart(Files MyFiles)
        {
            if (MyFiles.FileList.Count == 0)
            {
                MessageBox.Show("Список файлов пуст!", "Не удалось", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Синхронизировать. 
            foreach (FileModel file in MyFiles.FileList)
            {
                if (file.PathToCopy == null) { continue; }

                if (file.FileType == 0) // Это один файл.
                {
                    foreach (string path in file.PathToCopy)
                    {
                        CopyFile(file.FilePath, path, true);
                    }
                }
                else // Это папка.
                {
                    foreach (string path in file.PathToCopy)
                    {
                        try
                        {
                            DirectoryCopy(file.FilePath, path, true);
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show(String.Format("Папку {0} не удалось скопировать.\nПричина:\n{1}", file.FileName, ex.Message),
                                "Не удалось скопировать директорию", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }                
            }
            MessageBox.Show("Синхронизация завершена", "Выполнено", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Скопировать файл.
        /// </summary>
        /// <param name="oldPath">Старый путь.</param>
        /// <param name="newPath">Новый путь.</param>
        private void CopyFile(string oldPath, string newPath, bool canRewriteFile)
        {
            try
            {
                File.Copy(oldPath, newPath, canRewriteFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Не удалось скопировать файл", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Скопировать папку.
        /// </summary>
        /// <param name="sourceDirName">Старый путь.</param>
        /// <param name="destDirName">Новый путь.</param>
        /// <param name="copySubDirs">Разрешать копирование поддиректорий.</param>
        private void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            /* 
             * Эта функция взята со страницы 
             * https://msdn.microsoft.com/ru-ru/library/bb762914(v=vs.110).aspx
             */

            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, true);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}
