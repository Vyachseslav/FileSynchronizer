using System;
using System.Windows;
using System.Xml;
using System.Xml.Linq;

namespace MoskvinWorkers
{
    public class Sizes
    {
        private WindowSize _winSize; // Объект класса WindowSize.

        /// <summary>
        /// Объект класса WindowSize.
        /// </summary>
        public WindowSize WinSize
        {
            get { return _winSize; }
            set { _winSize = value; }
        }
        
        /// <summary>
        /// Конструктор.
        /// </summary>
        public Sizes()
        {
            WinSize = new WindowSize();
            LoadWindowSize();
        }

        /// <summary>
        /// Сохранить размеры окна последней сесии.
        /// </summary>
        public void SaveWindowSize()
        {
            XDocument xDoc = new XDocument();
            XElement files = new XElement("Syncronizer");

            XElement windowElement = new XElement("Window");
            XAttribute windowElementName = new XAttribute("Name", "MainWindow");
            windowElement.Add(windowElementName);

            XElement height = new XElement("Height", WinSize.Height);
            XElement width = new XElement("Width", WinSize.Width);
            XElement left = new XElement("Left", WinSize.Left);
            XElement top = new XElement("Top", WinSize.Top);
            XElement state = new XElement("State", WinSize.State);

            windowElement.Add(height);
            windowElement.Add(width);
            windowElement.Add(left);
            windowElement.Add(top);
            windowElement.Add(state);

            files.Add(windowElement);
            xDoc.Add(files);
            xDoc.Save("Settings\\Program\\ProgramSize.xml");
        }

        /// <summary>
        /// Загрузить размеры окна предыдущей сесии.
        /// </summary>
        public void LoadWindowSize()
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("Settings\\Program\\ProgramSize.xml");
            
            XmlElement xRoot = xDoc.DocumentElement;// получим корневой элемент
            foreach(XmlNode xNode in xRoot)
            {
                foreach(XmlNode childNode in xNode.ChildNodes)
                {
                    switch(childNode.Name)
                    {
                        case "Height":
                            WinSize.Height = Convert.ToDouble(childNode.InnerText);
                            break;
                        case "Width":
                            WinSize.Width = Convert.ToDouble(childNode.InnerText);
                            break;
                        case "Left":
                            WinSize.Left = Convert.ToDouble(childNode.InnerText);
                            break;
                        case "Top":
                            WinSize.Top = Convert.ToDouble(childNode.InnerText);
                            break;
                        case "State":
                            WinSize.State = GetWindowState(childNode.InnerText);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Получить состояние окна в виде String.
        /// </summary>
        /// <param name="xmlState">Состояние окна в string-типе.</param>
        /// <returns>Возвращает состояние в типе WindowState.</returns>
        private WindowState GetWindowState(string xmlState)
        {
            WindowState state = WindowState.Normal;
            switch (xmlState)
            {
                case "Minimized":
                    state = WindowState.Minimized;
                    break;
                case "Maximized":
                    state = WindowState.Maximized;
                    break;
                default:
                    state = WindowState.Normal;
                    break;
            }
            return state;
        }
    }
}
