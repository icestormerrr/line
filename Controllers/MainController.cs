using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Line.Models;

namespace Line.Controllers
{
    /// <summary>
    /// Контроллер для управления основной логикой текстового редактора
    /// </summary>
    public class MainController
    {
        /// <summary>
        /// Текущий документ
        /// </summary>
        public TextDocument CurrentDocument { get; private set; }
        
        /// <summary>
        /// Список открытых документов
        /// </summary>
        public List<TextDocument> OpenedDocuments { get; private set; }
        
        /// <summary>
        /// Интервал автосохранения в миллисекундах (по умолчанию 5 минут)
        /// </summary>
        public int AutoSaveInterval { get; set; } = 300000;
        
        /// <summary>
        /// Событие, возникающее при изменении документа
        /// </summary>
        public event EventHandler<DocumentChangedEventArgs> DocumentChanged;
        
        /// <summary>
        /// Событие, возникающее при открытии нового документа
        /// </summary>
        public event EventHandler<DocumentOpenedEventArgs> DocumentOpened;
        
        /// <summary>
        /// Событие, возникающее при сохранении документа
        /// </summary>
        public event EventHandler<DocumentSavedEventArgs> DocumentSaved;
        
        /// <summary>
        /// Конструктор контроллера
        /// </summary>
        public MainController()
        {
            OpenedDocuments = new List<TextDocument>();
            CurrentDocument = new TextDocument();
        }
        
        /// <summary>
        /// Создает новый документ
        /// </summary>
        public void NewDocument()
        {
            CurrentDocument = new TextDocument();
            OpenedDocuments.Add(CurrentDocument);
            OnDocumentOpened(new DocumentOpenedEventArgs(CurrentDocument));
        }
        
        /// <summary>
        /// Открывает документ из файла
        /// </summary>
        /// <param name="filePath">Путь к файлу</param>
        public void OpenDocument(string filePath)
        {
            try
            {
                var document = TextDocument.Open(filePath);
                CurrentDocument = document;
                
                // Проверяем, не открыт ли уже такой документ
                if (!OpenedDocuments.Contains(document))
                {
                    OpenedDocuments.Add(document);
                }
                
                OnDocumentOpened(new DocumentOpenedEventArgs(document));
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при открытии файла: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// Сохраняет текущий документ
        /// </summary>
        public void SaveDocument()
        {
            if (CurrentDocument == null)
                throw new InvalidOperationException("Нет открытого документа");
            
            try
            {
                CurrentDocument.Save();
                OnDocumentSaved(new DocumentSavedEventArgs(CurrentDocument));
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при сохранении файла: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// Сохраняет текущий документ в новый файл
        /// </summary>
        /// <param name="filePath">Путь к новому файлу</param>
        public void SaveDocumentAs(string filePath)
        {
            if (CurrentDocument == null)
                throw new InvalidOperationException("Нет открытого документа");
            
            try
            {
                CurrentDocument.SaveAs(filePath);
                OnDocumentSaved(new DocumentSavedEventArgs(CurrentDocument));
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при сохранении файла: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// Закрывает текущий документ
        /// </summary>
        public void CloseDocument()
        {
            if (CurrentDocument != null)
            {
                OpenedDocuments.Remove(CurrentDocument);
                CurrentDocument = OpenedDocuments.Count > 0 ? OpenedDocuments[OpenedDocuments.Count - 1] : new TextDocument();
            }
        }
        
        /// <summary>
        /// Обновляет содержимое текущего документа
        /// </summary>
        /// <param name="content">Новое содержимое</param>
        public void UpdateDocumentContent(string content)
        {
            if (CurrentDocument == null)
                throw new InvalidOperationException("Нет открытого документа");
            
            string oldContent = CurrentDocument.Content;
            CurrentDocument.Content = content ?? string.Empty;
            
            if (oldContent != CurrentDocument.Content)
            {
                CurrentDocument.IsModified = true;
                OnDocumentChanged(new DocumentChangedEventArgs(CurrentDocument, oldContent, CurrentDocument.Content));
            }
        }
        
        /// <summary>
        /// Выполняет поиск текста в текущем документе
        /// </summary>
        /// <param name="searchText">Текст для поиска</param>
        /// <param name="startIndex">Индекс начала поиска</param>
        /// <param name="matchCase">Учитывать регистр</param>
        /// <param name="searchUp">Искать вверх</param>
        /// <returns>Индекс найденного текста или -1, если не найден</returns>
        public int FindText(string searchText, int startIndex, bool matchCase, bool searchUp)
        {
            if (CurrentDocument == null)
                throw new InvalidOperationException("Нет открытого документа");
            
            if (searchUp)
            {
                return CurrentDocument.FindLast(searchText, startIndex, matchCase);
            }
            else
            {
                return CurrentDocument.Find(searchText, startIndex, matchCase);
            }
        }
        
        /// <summary>
        /// Выполняет замену текста во всем документе
        /// </summary>
        /// <param name="findText">Текст для поиска</param>
        /// <param name="replaceText">Текст для замены</param>
        /// <param name="matchCase">Учитывать регистр</param>
        /// <returns>Количество произведенных замен</returns>
        public int ReplaceAllText(string findText, string replaceText, bool matchCase)
        {
            if (CurrentDocument == null)
                throw new InvalidOperationException("Нет открытого документа");
            
            int count = CurrentDocument.ReplaceAll(findText, replaceText, matchCase);
            
            if (count > 0)
            {
                CurrentDocument.IsModified = true;
                OnDocumentChanged(new DocumentChangedEventArgs(CurrentDocument, "", CurrentDocument.Content));
            }
            
            return count;
        }
        
        /// <summary>
        /// Автоматически сохраняет измененные документы
        /// </summary>
        public void AutoSave()
        {
            foreach (var document in OpenedDocuments.Where(d => d.IsModified && !string.IsNullOrEmpty(d.FilePath)))
            {
                try
                {
                    document.Save();
                    OnDocumentSaved(new DocumentSavedEventArgs(document));
                }
                catch (Exception ex)
                {
                    // Логируем ошибку, но не прерываем процесс автосохранения других документов
                    System.Diagnostics.Debug.WriteLine($"Ошибка автосохранения файла {document.FilePath}: {ex.Message}");
                }
            }
        }
        
        /// <summary>
        /// Вызывает событие DocumentChanged
        /// </summary>
        /// <param name="e">Аргументы события</param>
        protected virtual void OnDocumentChanged(DocumentChangedEventArgs e)
        {
            DocumentChanged?.Invoke(this, e);
        }
        
        /// <summary>
        /// Вызывает событие DocumentOpened
        /// </summary>
        /// <param name="e">Аргументы события</param>
        protected virtual void OnDocumentOpened(DocumentOpenedEventArgs e)
        {
            DocumentOpened?.Invoke(this, e);
        }
        
        /// <summary>
        /// Вызывает событие DocumentSaved
        /// </summary>
        /// <param name="e">Аргументы события</param>
        protected virtual void OnDocumentSaved(DocumentSavedEventArgs e)
        {
            DocumentSaved?.Invoke(this, e);
        }
    }
    
    /// <summary>
    /// Аргументы события изменения документа
    /// </summary>
    public class DocumentChangedEventArgs : EventArgs
    {
        public TextDocument Document { get; }
        public string OldContent { get; }
        public string NewContent { get; }
        
        public DocumentChangedEventArgs(TextDocument document, string oldContent, string newContent)
        {
            Document = document;
            OldContent = oldContent;
            NewContent = newContent;
        }
    }
    
    /// <summary>
    /// Аргументы события открытия документа
    /// </summary>
    public class DocumentOpenedEventArgs : EventArgs
    {
        public TextDocument Document { get; }
        
        public DocumentOpenedEventArgs(TextDocument document)
        {
            Document = document;
        }
    }
    
    /// <summary>
    /// Аргументы события сохранения документа
    /// </summary>
    public class DocumentSavedEventArgs : EventArgs
    {
        public TextDocument Document { get; }
        
        public DocumentSavedEventArgs(TextDocument document)
        {
            Document = document;
        }
    }
}
