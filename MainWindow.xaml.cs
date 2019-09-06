using System;
using System.Windows;
using System.IO;
using System.ComponentModel;
using System.Windows.Documents;

namespace MoskvinWorkers
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Автозагрузка последней сессии

        private bool _loadLastSession = true;

        /// <summary>
        /// Загружать последнюю сессию.
        /// </summary>
        public bool LoadLastSession 
        {
            get { return _loadLastSession; }
            set { _loadLastSession = value; }
        }

        #endregion

        /// <summary>
        /// Список файлов для обновления.
        /// </summary>
        Files MyFiles { get; set; }

#region Свойства - пути к файлам

        /// <summary>
        /// Путь к новому файлу, который нужно копировать.
        /// </summary>
        string PathToNewFile { get; set; }

        /// <summary>
        /// Путь к файлу, который нужно заменить при синхронизации.
        /// </summary>
        string PathToNewSyncFile { get; set; }

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            Sizes windowSizes = new Sizes();
            this.Height = windowSizes.WinSize.Height;
            this.Width = windowSizes.WinSize.Width;
            this.Left = windowSizes.WinSize.Left;
            this.Top = windowSizes.WinSize.Top;
            this.WindowState = windowSizes.WinSize.State;

            this.DataContext = this;

            MyFiles = new Files(LoadLastSession);
            grid.ItemsSource = MyFiles.FileList;

            this.Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            MyFiles.SaveSession(); // Сохранить список файлов.

            Sizes windowSizes = new Sizes();            
            windowSizes.WinSize.Height = this.Height;
            windowSizes.WinSize.Width = this.Width;
            windowSizes.WinSize.Left = this.Left;
            windowSizes.WinSize.Top = this.Top;
            windowSizes.WinSize.State = this.WindowState;
            windowSizes.SaveWindowSize();
        }



        private void btnSelectFile_Click(object sender, RoutedEventArgs e)
        {
            // Указать файл для обновления.
            try
            {
                PathToNewFile = new SelectFiles().OpenFileDialog(); // Открыть диалог для выбора нового файла.
                if (PathToNewFile != null)
                {
                    MyFiles.AddNewFile(PathToNewFile); // Добавить новый файл в список синхронизаций.
                }
            }
            catch { }
        }

        private void btnEditFile_Click(object sender, RoutedEventArgs e)
        {
            // Редактировать файл в списке.
            if (grid.SelectedIndex == -1)
            {
                MessageBox.Show("Выделите строку с удаляемым файлом", "Нет файла для удаления", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int selectedRow = grid.SelectedIndex; // Номер выделенной строки.
            PathToNewFile = String.Empty;
            PathToNewFile = new SelectFiles().OpenFileDialog(); // Открыть диалог для выбора нового файла.
            if (PathToNewFile != String.Empty)
            {
                MyFiles.EditFile(selectedRow, PathToNewFile); // Добавить новый файл в список синхронизаций.
            }
        }



        private void menuAddSyncPath_Click(object sender, RoutedEventArgs e)
        {
            // Добавляем путь к файлу для обновления.
            try
            {
                int selectedRow = grid.SelectedIndex; // Номер выделенной строки.
                PathToNewSyncFile = new SelectFiles().OpenFileDialog(); // Добавляем путь к файлу, который нужно заменить.
                if (PathToNewSyncFile != null)
                {
                    MyFiles.AddPath(selectedRow, PathToNewSyncFile); // Добавляем новый файл в список обновляемых.
                }
            }
            catch { }
        }

        private void menuRemoveFile_Click(object sender, RoutedEventArgs e)
        {
            // Удалить файл из списка.
            int selectedRow = grid.SelectedIndex; // Номер выделенной строки.
            //int selectingRows = grid.SelectedItems.Count; // Число выделенных строк.


            if (selectedRow == -1)
            {
                MessageBox.Show("Выделите строку с удаляемым файлом", "Нет файла для удаления", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MyFiles.RemoveFile(selectedRow);
        }

        private void menuClearList_Click(object sender, RoutedEventArgs e)
        {
            // Очистить список синхронизации.
            if (MessageBox.Show("Вы действительно хотите очистить список синхронизации полностью?", "Вы уверены?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    MyFiles.FileList.Clear();
                    MessageBox.Show("Список синхронизации успешно очищен.", "Очищено", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Не удалось очистить список.\nПричина:\n" + ex.Message, "Очистка не удалась", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private void Empty_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            /*
             * Добавление файла или добавить путь синхронизации, если строка выделена.
             */

            // Указать файл для обновления.
            if(grid.SelectedIndex == -1)
            {
                try
                {
                    PathToNewFile = new SelectFiles().OpenFolderDialog(); // Открыть диалог для выбора нового файла.
                    if (PathToNewFile != null)
                    {
                        MyFiles.AddNewFile(PathToNewFile); // Добавить новый файл в список синхронизаций.
                    }
                }
                catch { }
            }
            else
            {
                // Добавляем путь к файлу для обновления.
                try
                {
                    int selectedRow = grid.SelectedIndex; // Номер выделенной строки.
                    /*if (MyFiles.FileList[selectedRow].FileType == 0) // Это файл.
                    {
                        PathToNewSyncFile = new SelectFiles().OpenSaveFileDialog(MyFiles.FileList[selectedRow].FileName); // Добавляем путь к файлу, который нужно заменить.
                    }
                    else
                    {
                        PathToNewSyncFile = new SelectFiles().OpenFolderDialog(); // Открыть диалог для выбора папки.
                    }*/
                    PathToNewSyncFile = new SelectFiles().OpenFolderDialog(); // Открыть диалог для выбора папки.                    
                    if (PathToNewSyncFile != null)
                    {
                        MyFiles.AddPath(selectedRow, PathToNewSyncFile += "\\" + MyFiles.FileList[selectedRow].FileName); // Добавляем новый файл в список обновляемых.
                    }
                    grid.SelectedIndex = -1; // Снимаем выделение строки.
                }
                catch { }
            }            
        }

        private void Empty_SingleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            grid.SelectedIndex = -1; // Снимаем выделение строки.
        }

        private void btnSync_Click(object sender, RoutedEventArgs e)
        {
            // Нажимаем кнопку "Синхронизировать".
            Syncronizing doSync = new Syncronizing();
            doSync.SyncronizeFileStart(MyFiles);
        }



        private void Hyperlink_SyncFileInfoNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            // Посмотреть информацию о файле.
            int selectedRow = grid.SelectedIndex; // Номер выделенной строки.
            var tb = ((Hyperlink)e.OriginalSource).DataContext; // Получаем путь для синхронизации.

            MyFiles.GetSyncFileInfo(tb.ToString());
        }

        private void Hyperlink_RequestRemoveNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            // Удаляем из списка путей синхронизаций.
            int selectedRow = grid.SelectedIndex; // Номер выделенной строки.
            var tb = ((Hyperlink)e.OriginalSource).DataContext; // Получаем путь для синхронизации.
            MyFiles.RemovePath(selectedRow, tb.ToString());
            grid.UnselectAll();
        }

        private void Hyperlink_RequestEditNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            // Редактируем путь синхронизации.
            int selectedRow = grid.SelectedIndex; // Номер выделенной строки.
            var tb = ((Hyperlink)e.OriginalSource).DataContext; // Получаем путь для синхронизации.

            if (MyFiles.FileList[selectedRow].FileType == 0) // Это файл.
            {
                PathToNewSyncFile = new SelectFiles().OpenFileDialog(); // Открыть диалог для выбора нового файла.
            }
            else
            {
                PathToNewSyncFile = new SelectFiles().OpenFolderDialog(); // Открыть диалог для выбора папки.
            }

            if (PathToNewSyncFile != null)
            {
                MyFiles.EditPath(selectedRow, tb.ToString(), PathToNewSyncFile); // Изменить путь синхронизации.
            }
        }



        private void Hyperlink_FileEditNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            // Редактируем файл.
            if (grid.SelectedIndex == -1)
            {
                MessageBox.Show("Выделите строку с удаляемым файлом", "Нет файла для удаления", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int selectedRow = grid.SelectedIndex; // Номер выделенной строки.

            // Что это за пункт: файл или папка
            if (MyFiles.FileList[selectedRow].FileType == 0) // Это файл.
            {
                PathToNewFile = new SelectFiles().OpenFileDialog(); // Открыть диалог для выбора нового файла.
            }
            else
            {
                PathToNewFile = new SelectFiles().OpenFolderDialog(); // Открыть диалог для выбора папки.
            }
            
            if (PathToNewFile != null)
            {
                MyFiles.EditFile(selectedRow, PathToNewFile); // Добавить новый файл в список синхронизаций.
            }
        }

        private void Hyperlink_FileRemoveNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            // Удалить файл из списка.
            int selectedRow = grid.SelectedIndex; // Номер выделенной строки.
            if (selectedRow == -1)
            {
                MessageBox.Show("Выделите строку с удаляемым файлом", "Нет файла для удаления", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MyFiles.RemoveFile(selectedRow);
            grid.UnselectAll();
        }

        private void Hyperlink_FileInfoNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            // Посмотреть информацию о файле.
            int selectedRow = grid.SelectedIndex; // Номер выделенной строки.
            if (MyFiles.FileList[selectedRow].FileType == 0) // Это файл.
            {
                MyFiles.GetFileInfo(selectedRow);
            }
            else // Это папка.
            {
                MyFiles.GetFolderInfo(selectedRow);
            }
        }



        private void btnAddFolder_Click(object sender, RoutedEventArgs e)
        {
            // Добавить папку.
            if (grid.SelectedIndex == -1)
            {
                try
                {
                    string folderPath = new SelectFiles().OpenFolderDialog();
                    if (folderPath != null)
                    {
                        MyFiles.AddNewFile(folderPath); // Добавить папку в список синхронизаций.
                    }
                }
                catch { }
            }
            else
            {
                try
                {
                    int selectedRow = grid.SelectedIndex; // Номер выделенной строки.
                    string folderPath = new SelectFiles().OpenFolderDialog();
                    if (folderPath != null)
                    {
                        MyFiles.AddPath(selectedRow, folderPath); // Добавляем папку в список обновляемых.
                    }
                }
                catch { }
            }
        }

        private void btnAddFile_Click(object sender, RoutedEventArgs e)
        {
            if (grid.SelectedIndex == -1)
            {
                try
                {
                    PathToNewFile = new SelectFiles().OpenFileDialog(); // Открыть диалог для выбора нового файла.
                    if (PathToNewFile != null)
                    {
                        MyFiles.AddNewFile(PathToNewFile); // Добавить новый файл в список синхронизаций.
                    }
                }
                catch { }
            }
            else
            {
                try
                {
                    int selectedRow = grid.SelectedIndex; // Номер выделенной строки.
                    PathToNewSyncFile = new SelectFiles().OpenFileDialog(); // Добавляем путь к файлу, который нужно заменить.
                    if (PathToNewSyncFile != null)
                    {
                        MyFiles.AddPath(selectedRow, PathToNewSyncFile); // Добавляем новый файл в список обновляемых.
                    }
                }
                catch { }
            }
        }

        private void btnAddFavorits_Click(object sender, RoutedEventArgs e)
        {
            // Добавить файл в избранное.
            MessageBox.Show("Добавили в избранное");
        }


        /*
         *    События выделения и снятия выделения строки в таблице.
         */

        private void DataGridRow_Selected(object sender, RoutedEventArgs e)
        {
            // Выделили строку.
        }

        private void DataGridRow_Unselected(object sender, RoutedEventArgs e)
        {
            // Сняли выделение со строки.
        }



        private void menuAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow about = new AboutWindow();
            about.Owner = this;
            about.Show();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        /*
         *    События для перемещения файлов в программу.
         */

        private void text_PreviewDragEnter(object sender, DragEventArgs e)
        {
            //bool isCorrect = true;
            /*
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true) == true)
            {
                string[] filenames = (string[])e.Data.GetData(DataFormats.FileDrop, true);
                foreach (string filename in filenames)
                {
                    if (File.Exists(filename) == false)
                    {
                        isCorrect = false;
                        break;
                    }
                    FileInfo info = new FileInfo(filename);
                }
            }
            */
            
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            //e.Handled = true;
        }

        private void text_PreviewDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);

                // Assuming you have one file that you care about, pass it off to whatever
                // handling code you have defined.
                foreach (string file in files)
                {
                    FileInfo ff = new FileInfo(file);
                    if (grid.SelectedIndex == -1)
                    {
                        try
                        {
                            PathToNewFile = ff.FullName; // Путь к файлу.
                            if (PathToNewFile != null)
                            {
                                MyFiles.AddNewFile(PathToNewFile); // Добавить новый файл в список синхронизаций.
                            }
                        }
                        catch { }
                    }
                    else
                    {
                        try
                        {
                            int selectedRow = grid.SelectedIndex; // Номер выделенной строки.
                            PathToNewSyncFile = ff.FullName;
                            if (PathToNewSyncFile != null)
                            {
                                MyFiles.AddPath(selectedRow, PathToNewSyncFile); // Добавляем новый файл в список обновляемых.
                            }
                        }
                        catch { }
                    }
                    //text.Text += ff.FullName + "\n"; // Можно вытащить путь.
                }
            }
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FileModel row = (FileModel)grid.SelectedItems[0];
            //FileModel row = (DataRowView)grid.SelectedItems[0] as FileModel;
            MessageBox.Show(row.FileID.ToString());
        }
    }
}
