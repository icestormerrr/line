using System;
using System.Drawing;
using System.Windows.Forms;
using Line.Controllers;

namespace Line
{
    public partial class FindForm : Form
    {
        private TextBox textBox;
        private MainController controller;
        private TextBox findTextBox;
        private Button findNextButton;
        private Button cancelButton;
        private CheckBox matchCaseCheckBox;
        private GroupBox directionGroupBox;
        private RadioButton upRadioButton;
        private RadioButton downRadioButton;
        
        public FindForm(TextBox textBox, MainController controller)
        {
            InitializeComponent();
            this.textBox = textBox;
            this.controller = controller;
        }
        
        private void InitializeComponent()
        {
            this.findTextBox = new TextBox();
            this.findNextButton = new Button();
            this.cancelButton = new Button();
            this.matchCaseCheckBox = new CheckBox();
            this.directionGroupBox = new GroupBox();
            this.upRadioButton = new RadioButton();
            this.downRadioButton = new RadioButton();
            
            this.directionGroupBox.SuspendLayout();
            this.SuspendLayout();
            
            // 
            // findTextBox
            // 
            this.findTextBox.Location = new Point(80, 15);
            this.findTextBox.Name = "findTextBox";
            this.findTextBox.Size = new Size(200, 23);
            this.findTextBox.TabIndex = 0;
            
            // 
            // findNextButton
            // 
            this.findNextButton.Location = new Point(290, 15);
            this.findNextButton.Name = "findNextButton";
            this.findNextButton.Size = new Size(80, 23);
            this.findNextButton.TabIndex = 1;
            this.findNextButton.Text = "Найти далее";
            this.findNextButton.UseVisualStyleBackColor = true;
            this.findNextButton.Click += new EventHandler(this.findNextButton_Click);
            
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new Point(290, 45);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new Size(80, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Отмена";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new EventHandler(this.cancelButton_Click);
            
            // 
            // matchCaseCheckBox
            // 
            this.matchCaseCheckBox.AutoSize = true;
            this.matchCaseCheckBox.Location = new Point(15, 50);
            this.matchCaseCheckBox.Name = "matchCaseCheckBox";
            this.matchCaseCheckBox.Size = new Size(126, 19);
            this.matchCaseCheckBox.TabIndex = 3;
            this.matchCaseCheckBox.Text = "Учитывать регистр";
            this.matchCaseCheckBox.UseVisualStyleBackColor = true;
            
            // 
            // directionGroupBox
            // 
            this.directionGroupBox.Controls.Add(this.upRadioButton);
            this.directionGroupBox.Controls.Add(this.downRadioButton);
            this.directionGroupBox.Location = new Point(150, 50);
            this.directionGroupBox.Name = "directionGroupBox";
            this.directionGroupBox.Size = new Size(130, 45);
            this.directionGroupBox.TabIndex = 4;
            this.directionGroupBox.TabStop = false;
            this.directionGroupBox.Text = "Направление";
            
            // 
            // upRadioButton
            // 
            this.upRadioButton.AutoSize = true;
            this.upRadioButton.Location = new Point(10, 20);
            this.upRadioButton.Name = "upRadioButton";
            this.upRadioButton.Size = new Size(54, 19);
            this.upRadioButton.TabIndex = 0;
            this.upRadioButton.Text = "Вверх";
            this.upRadioButton.UseVisualStyleBackColor = true;
            
            // 
            // downRadioButton
            // 
            this.downRadioButton.AutoSize = true;
            this.downRadioButton.Checked = true;
            this.downRadioButton.Location = new Point(70, 20);
            this.downRadioButton.Name = "downRadioButton";
            this.downRadioButton.Size = new Size(54, 19);
            this.downRadioButton.TabIndex = 1;
            this.downRadioButton.TabStop = true;
            this.downRadioButton.Text = "Вниз";
            this.downRadioButton.UseVisualStyleBackColor = true;
            
            // 
            // FindForm
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(380, 110);
            this.Controls.Add(this.directionGroupBox);
            this.Controls.Add(this.matchCaseCheckBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.findNextButton);
            this.Controls.Add(this.findTextBox);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FindForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Найти";
            this.directionGroupBox.ResumeLayout(false);
            this.directionGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        
        private void findNextButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(findTextBox.Text))
            {
                MessageBox.Show("Введите текст для поиска.", "Найти", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            int startIndex = textBox.SelectionStart + textBox.SelectionLength;
            bool searchUp = upRadioButton.Checked;
            int index = controller.FindText(findTextBox.Text, startIndex, matchCaseCheckBox.Checked, searchUp);
            
            if (index == -1)
            {
                // Если не найдено, начинаем с начала/конца
                index = controller.FindText(findTextBox.Text, searchUp ? textBox.Text.Length : 0, matchCaseCheckBox.Checked, searchUp);
                
                if (index == -1)
                {
                    MessageBox.Show("Текст не найден.", "Найти", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            
            textBox.Select(index, findTextBox.Text.Length);
            textBox.ScrollToCaret();
            textBox.Focus();
        }
        
        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
