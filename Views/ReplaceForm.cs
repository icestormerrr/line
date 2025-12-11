using System;
using System.Drawing;
using System.Windows.Forms;
using Line.Controllers;

namespace Line
{
    public partial class ReplaceForm : Form
    {
        private TextBox textBox;
        private MainController controller;
        private TextBox findTextBox;
        private TextBox replaceTextBox;
        private Button findNextButton;
        private Button replaceButton;
        private Button replaceAllButton;
        private Button cancelButton;
        private CheckBox matchCaseCheckBox;
        
        public ReplaceForm(TextBox textBox, MainController controller)
        {
            InitializeComponent();
            this.textBox = textBox;
            this.controller = controller;
        }
        
        private void InitializeComponent()
        {
            this.findTextBox = new TextBox();
            this.replaceTextBox = new TextBox();
            this.findNextButton = new Button();
            this.replaceButton = new Button();
            this.replaceAllButton = new Button();
            this.cancelButton = new Button();
            this.matchCaseCheckBox = new CheckBox();
            
            this.SuspendLayout();
            
            // 
            // findTextBox
            // 
            this.findTextBox.Location = new Point(100, 15);
            this.findTextBox.Name = "findTextBox";
            this.findTextBox.Size = new Size(180, 23);
            this.findTextBox.TabIndex = 0;
            
            // 
            // replaceTextBox
            // 
            this.replaceTextBox.Location = new Point(100, 45);
            this.replaceTextBox.Name = "replaceTextBox";
            this.replaceTextBox.Size = new Size(180, 23);
            this.replaceTextBox.TabIndex = 1;
            
            // 
            // findNextButton
            // 
            this.findNextButton.Location = new Point(290, 15);
            this.findNextButton.Name = "findNextButton";
            this.findNextButton.Size = new Size(90, 23);
            this.findNextButton.TabIndex = 2;
            this.findNextButton.Text = "Найти далее";
            this.findNextButton.UseVisualStyleBackColor = true;
            this.findNextButton.Click += new EventHandler(this.findNextButton_Click);
            
            // 
            // replaceButton
            // 
            this.replaceButton.Location = new Point(290, 45);
            this.replaceButton.Name = "replaceButton";
            this.replaceButton.Size = new Size(90, 23);
            this.replaceButton.TabIndex = 3;
            this.replaceButton.Text = "Заменить";
            this.replaceButton.UseVisualStyleBackColor = true;
            this.replaceButton.Click += new EventHandler(this.replaceButton_Click);
            
            // 
            // replaceAllButton
            // 
            this.replaceAllButton.Location = new Point(290, 75);
            this.replaceAllButton.Name = "replaceAllButton";
            this.replaceAllButton.Size = new Size(90, 23);
            this.replaceAllButton.TabIndex = 4;
            this.replaceAllButton.Text = "Заменить всё";
            this.replaceAllButton.UseVisualStyleBackColor = true;
            this.replaceAllButton.Click += new EventHandler(this.replaceAllButton_Click);
            
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new Point(290, 105);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new Size(90, 23);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "Отмена";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new EventHandler(this.cancelButton_Click);
            
            // 
            // matchCaseCheckBox
            // 
            this.matchCaseCheckBox.AutoSize = true;
            this.matchCaseCheckBox.Location = new Point(15, 80);
            this.matchCaseCheckBox.Name = "matchCaseCheckBox";
            this.matchCaseCheckBox.Size = new Size(126, 19);
            this.matchCaseCheckBox.TabIndex = 6;
            this.matchCaseCheckBox.Text = "Учитывать регистр";
            this.matchCaseCheckBox.UseVisualStyleBackColor = true;
            
            // 
            // ReplaceForm
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(390, 140);
            this.Controls.Add(this.matchCaseCheckBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.replaceAllButton);
            this.Controls.Add(this.replaceButton);
            this.Controls.Add(this.findNextButton);
            this.Controls.Add(this.replaceTextBox);
            this.Controls.Add(this.findTextBox);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ReplaceForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Заменить";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        
        private void findNextButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(findTextBox.Text))
            {
                MessageBox.Show("Введите текст для поиска.", "Заменить", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            int startIndex = textBox.SelectionStart + textBox.SelectionLength;
            // Для поиска вниз (по умолчанию)
            int index = controller.FindText(findTextBox.Text, startIndex, matchCaseCheckBox.Checked, false);
            
            if (index == -1)
            {
                // Если не найдено, начинаем с начала
                index = controller.FindText(findTextBox.Text, 0, matchCaseCheckBox.Checked, false);
                
                if (index == -1)
                {
                    MessageBox.Show("Текст не найден.", "Заменить", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            
            textBox.Select(index, findTextBox.Text.Length);
            textBox.ScrollToCaret();
            textBox.Focus();
        }
        
        private void replaceButton_Click(object sender, EventArgs e)
        {
            if (textBox.SelectedText == findTextBox.Text || 
                (matchCaseCheckBox.Checked == false && 
                 textBox.SelectedText.Equals(findTextBox.Text, StringComparison.OrdinalIgnoreCase)))
            {
                textBox.SelectedText = replaceTextBox.Text;
                controller.UpdateDocumentContent(textBox.Text);
            }
            
            // Найти следующее вхождение
            findNextButton_Click(sender, e);
        }
        
        private void replaceAllButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(findTextBox.Text))
            {
                MessageBox.Show("Введите текст для поиска.", "Заменить", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            try
            {
                int count = controller.ReplaceAllText(findTextBox.Text, replaceTextBox.Text, matchCaseCheckBox.Checked);
                MessageBox.Show($"Произведено замен: {count}", "Заменить", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при замене текста: {ex.Message}", "Заменить", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
