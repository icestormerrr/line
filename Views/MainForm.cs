using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.IO;
using Line.Controllers;
using Line.Models;

namespace Line
{
    public partial class MainForm : Form
    {
        private MainController controller;
        private System.Windows.Forms.Timer autoSaveTimer;
        private PrintDocument printDocument;
        
        public MainForm()
        {
            InitializeComponent();
            InitializeController();
            UpdateTitle();
            
            // Инициализация таймера автосохранения (каждые 5 минут)
            autoSaveTimer = new System.Windows.Forms.Timer();
            autoSaveTimer.Interval = 300000; // 5 минут в миллисекундах
            autoSaveTimer.Tick += AutoSaveTimer_Tick;
            autoSaveTimer.Start();
            
            // Инициализация компонента печати
            printDocument = new PrintDocument();
            printDocument.PrintPage += PrintDocument_PrintPage;
        }
        
        private void InitializeController()
        {
            controller = new MainController();
            controller.DocumentChanged += Controller_DocumentChanged;
            controller.DocumentOpened += Controller_DocumentOpened;
            controller.DocumentSaved += Controller_DocumentSaved;
        }
        
        private void Controller_DocumentChanged(object sender, DocumentChangedEventArgs e)
        {
            textBox.Text = e.NewContent;
            UpdateTitle();
        }
        
        private void Controller_DocumentOpened(object sender, DocumentOpenedEventArgs e)
        {
            textBox.Text = e.Document.Content;
            UpdateTitle();
        }
        
        private void Controller_DocumentSaved(object sender, DocumentSavedEventArgs e)
        {
            UpdateTitle();
        }
        
        private void UpdateTitle()
        {
            if (controller.CurrentDocument != null)
            {
                string fileName = string.IsNullOrEmpty(controller.CurrentDocument.FilePath) ? "Новый файл" : Path.GetFileName(controller.CurrentDocument.FilePath);
                this.Text = $"{fileName} - Текстовый редактор";
                if (controller.CurrentDocument.IsModified)
                {
                    this.Text = "*" + this.Text;
                }
            }
        }
        
        // Обработчики событий меню
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (controller.CurrentDocument.IsModified)
            {
                DialogResult result = MessageBox.Show("Сохранить изменения перед созданием нового файла?", "Подтверждение", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {
                    SaveFile();
                }
                else if (result == DialogResult.Cancel)
                {
                    return;
                }
            }
            
            controller.NewDocument();
            textBox.Clear();
        }
        
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (controller.CurrentDocument.IsModified)
            {
                DialogResult result = MessageBox.Show("Сохранить изменения перед открытием другого файла?", "Подтверждение", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {
                    SaveFile();
                }
                else if (result == DialogResult.Cancel)
                {
                    return;
                }
            }
            
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
            
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    controller.OpenDocument(openFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при открытии файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile();
        }
        
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileAs();
        }
        
        private void SaveFile()
        {
            if (string.IsNullOrEmpty(controller.CurrentDocument.FilePath))
            {
                SaveFileAs();
            }
            else
            {
                try
                {
                    controller.UpdateDocumentContent(textBox.Text);
                    controller.SaveDocument();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        
        private void SaveFileAs()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
            
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    controller.UpdateDocumentContent(textBox.Text);
                    controller.SaveDocumentAs(saveFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (textBox.CanUndo)
            {
                textBox.Undo();
            }
        }
        
        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // В .NET 10 TextBox не имеет встроенного метода Redo()
            // Для реализации повтора действий используем альтернативный подход
            // В данном случае просто показываем сообщение, что функция недоступна
            MessageBox.Show("Функция повтора действий временно недоступна.", "Повторить", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        
        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox.Cut();
        }
        
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox.Copy();
        }
        
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox.Paste();
        }
        
        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox.SelectAll();
        }
        
        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FindForm findForm = new FindForm(textBox, controller);
            findForm.Show();
        }
        
        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReplaceForm replaceForm = new ReplaceForm(textBox, controller);
            replaceForm.Show();
        }
        
        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FontDialog fontDialog = new FontDialog();
            fontDialog.Font = textBox.Font;
            
            if (fontDialog.ShowDialog() == DialogResult.OK)
            {
                textBox.Font = fontDialog.Font;
            }
        }
        
        private void colorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.Color = textBox.ForeColor;
            
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                textBox.ForeColor = colorDialog.Color;
            }
        }
        
        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = printDocument;
            
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    printDocument.Print();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при печати: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        
        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            // Получаем контент для печати
            string content = textBox.Text;
            
            // Настройки печати
            Font font = textBox.Font;
            Brush brush = Brushes.Black;
            float lineHeight = font.GetHeight(e.Graphics ?? throw new ArgumentNullException(nameof(e.Graphics)));
            float leftMargin = e.MarginBounds.Left;
            float topMargin = e.MarginBounds.Top;
            
            // Разбиваем текст на строки
            string[] lines = content.Split('\n');
            
            float yPos = topMargin;
            int lineIndex = 0;
            
            // Печатаем строки, пока не закончится место на странице
            while (lineIndex < lines.Length && yPos < e.MarginBounds.Bottom)
            {
                e.Graphics.DrawString(lines[lineIndex], font, brush, leftMargin, yPos);
                yPos += lineHeight;
                lineIndex++;
            }
            
            // Если еще есть строки для печати, установим HasMorePages в true
            e.HasMorePages = lineIndex < lines.Length;
        }
        
        private void AutoSaveTimer_Tick(object sender, EventArgs e)
        {
            controller.AutoSave();
        }
        
        private void textBox_TextChanged(object sender, EventArgs e)
        {
            controller.UpdateDocumentContent(textBox.Text);
        }
        
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (controller.CurrentDocument.IsModified)
            {
                DialogResult result = MessageBox.Show("Сохранить изменения перед закрытием?", "Подтверждение", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {
                    SaveFile();
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
            
            autoSaveTimer.Stop();
            base.OnFormClosing(e);
        }
    }
}
