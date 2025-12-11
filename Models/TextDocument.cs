using System;
using System.IO;

namespace Line.Models
{
    /// <summary>
    /// Модель для работы с текстовым документом
    /// </summary>
    public class TextDocument
    {
        /// <summary>
        /// Путь к файлу
        /// </summary>
        public string FilePath { get; set; }
        
        /// <summary>
        /// Содержимое документа
        /// </summary>
        public string Content { get; set; }
        
        /// <summary>
        /// Флаг, указывающий, был ли документ изменен
        /// </summary>
        public bool IsModified { get; set; }
        
        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public TextDocument()
        {
            FilePath = string.Empty;
            Content = string.Empty;
            IsModified = false;
        }
        
        /// <summary>
        /// Конструктор с параметрами
        /// </summary>
        /// <param name="filePath">Путь к файлу</param>
        /// <param name="content">Содержимое документа</param>
        public TextDocument(string filePath, string content)
        {
            FilePath = filePath ?? string.Empty;
            Content = content ?? string.Empty;
            IsModified = false;
        }
        
        /// <summary>
        /// Открывает документ из файла
        /// </summary>
        /// <param name="filePath">Путь к файлу</param>
        /// <returns>Экземпляр TextDocument</returns>
        public static TextDocument Open(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("Путь к файлу не может быть пустым", nameof(filePath));
            
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Файл не найден", filePath);
            
            try
            {
                string content = File.ReadAllText(filePath);
                return new TextDocument(filePath, content);
            }
            catch (Exception ex)
            {
                throw new IOException($"Ошибка при открытии файла: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// Сохраняет документ в файл
        /// </summary>
        public void Save()
        {
            if (string.IsNullOrEmpty(FilePath))
                throw new InvalidOperationException("Путь к файлу не задан");
            
            try
            {
                File.WriteAllText(FilePath, Content);
                IsModified = false;
            }
            catch (Exception ex)
            {
                throw new IOException($"Ошибка при сохранении файла: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// Сохраняет документ в новый файл
        /// </summary>
        /// <param name="filePath">Путь к новому файлу</param>
        public void SaveAs(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("Путь к файлу не может быть пустым", nameof(filePath));
            
            try
            {
                File.WriteAllText(filePath, Content);
                FilePath = filePath;
                IsModified = false;
            }
            catch (Exception ex)
            {
                throw new IOException($"Ошибка при сохранении файла: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// Поиск текста в документе
        /// </summary>
        /// <param name="searchText">Текст для поиска</param>
        /// <param name="startIndex">Индекс начала поиска</param>
        /// <param name="matchCase">Учитывать регистр</param>
        /// <returns>Индекс найденного текста или -1, если не найден</returns>
        public int Find(string searchText, int startIndex, bool matchCase)
        {
            if (string.IsNullOrEmpty(searchText))
                return -1;
            
            if (startIndex < 0 || startIndex > Content.Length)
                return -1;
            
            StringComparison comparison = matchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            return Content.IndexOf(searchText, startIndex, comparison);
        }
        
        /// <summary>
        /// Поиск текста с конца документа
        /// </summary>
        /// <param name="searchText">Текст для поиска</param>
        /// <param name="startIndex">Индекс начала поиска</param>
        /// <param name="matchCase">Учитывать регистр</param>
        /// <returns>Индекс найденного текста или -1, если не найден</returns>
        public int FindLast(string searchText, int startIndex, bool matchCase)
        {
            if (string.IsNullOrEmpty(searchText))
                return -1;
            
            if (startIndex < 0 || startIndex > Content.Length)
                return -1;
            
            string textToSearch = Content.Substring(0, startIndex);
            StringComparison comparison = matchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            return textToSearch.LastIndexOf(searchText, comparison);
        }
        
        /// <summary>
        /// Замена текста в документе
        /// </summary>
        /// <param name="findText">Текст для поиска</param>
        /// <param name="replaceText">Текст для замены</param>
        /// <param name="matchCase">Учитывать регистр</param>
        /// <returns>Количество произведенных замен</returns>
        public int ReplaceAll(string findText, string replaceText, bool matchCase)
        {
            if (string.IsNullOrEmpty(findText))
                return 0;
            
            int count = 0;
            replaceText = replaceText ?? string.Empty;
            
            if (matchCase)
            {
                // Простая замена с учетом регистра
                string oldContent = Content;
                Content = Content.Replace(findText, replaceText);
                count = (oldContent.Length - Content.Length + replaceText.Length * (oldContent.Split(new string[] { findText }, StringSplitOptions.None).Length - 1)) / replaceText.Length;
            }
            else
            {
                // Замена без учета регистра
                int index = 0;
                while ((index = Content.IndexOf(findText, index, StringComparison.OrdinalIgnoreCase)) != -1)
                {
                    Content = Content.Remove(index, findText.Length).Insert(index, replaceText);
                    index += replaceText.Length;
                    count++;
                }
            }
            
            if (count > 0)
                IsModified = true;
                
            return count;
        }
    }
}
