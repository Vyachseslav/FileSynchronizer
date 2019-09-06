using System;
using System.Windows.Forms;

namespace MoskvinWorkers
{
    public class SelectFiles
    {
        /// <summary>
        /// Открыть диалоговое окно для выбора файлов.
        /// </summary>
        /// <returns></returns>
        public string OpenFileDialog()
        {
            string res = null;

            // Указать файл для обновления.
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document";
            dlg.Title = "Выбрать файл для копирования...";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                string fileName = dlg.FileName; // Получить путь к файлу.
                res = fileName;
            }

            return res;
        }

        /// <summary>
        /// Открыть диалоговое окно для сохранения файла.
        /// </summary>
        /// <returns>Выбранный путь.</returns>
        public string OpenSaveFileDialog(string name)
        {
            string res = null;

            // Указать файл для обновления.
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = name;
            dlg.Title = "Укажите путь для синхронизации";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                string fileName = dlg.FileName; // Получить путь к файлу.
                res = fileName;
            }

            return res;
        }

        /// <summary>
        /// Открыть диалоговое окно для выбора каталога.
        /// </summary>
        /// <returns>Возвращает путь к существующему каталогу.</returns>
        public string OpenFolderDialog()
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.ShowNewFolderButton = true;
                dialog.Description = "Укажите существующий каталог...";                
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                return (result == DialogResult.OK) ? dialog.SelectedPath : null;
            }
        }
    }
}
