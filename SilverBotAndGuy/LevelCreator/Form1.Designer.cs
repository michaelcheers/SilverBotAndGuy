namespace LevelCreator
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.saveFile = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.Load = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.tryButton = new System.Windows.Forms.Button();
            this.Prove = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(15, 14);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(1017, 488);
            this.textBox1.TabIndex = 0;
            // 
            // saveFile
            // 
            this.saveFile.Location = new System.Drawing.Point(1041, 14);
            this.saveFile.Name = "saveFile";
            this.saveFile.Size = new System.Drawing.Size(87, 25);
            this.saveFile.TabIndex = 1;
            this.saveFile.Text = "Save";
            this.saveFile.UseVisualStyleBackColor = true;
            this.saveFile.Click += new System.EventHandler(this.saveFile_Click);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "sbalvl";
            this.saveFileDialog1.FileName = "level.sbalvl";
            this.saveFileDialog1.Title = "level.sbalvl";
            // 
            // Load
            // 
            this.Load.Location = new System.Drawing.Point(1041, 45);
            this.Load.Name = "Load";
            this.Load.Size = new System.Drawing.Size(87, 23);
            this.Load.TabIndex = 2;
            this.Load.Text = "Load";
            this.Load.UseVisualStyleBackColor = true;
            this.Load.Click += new System.EventHandler(this.Load_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "sbalvl";
            this.openFileDialog1.FileName = "level.sbalvl";
            // 
            // tryButton
            // 
            this.tryButton.Location = new System.Drawing.Point(1041, 74);
            this.tryButton.Name = "tryButton";
            this.tryButton.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tryButton.Size = new System.Drawing.Size(87, 23);
            this.tryButton.TabIndex = 3;
            this.tryButton.Text = "Try";
            this.tryButton.UseVisualStyleBackColor = true;
            this.tryButton.Click += new System.EventHandler(this.tryButton_Click);
            // 
            // Prove
            // 
            this.Prove.Location = new System.Drawing.Point(1041, 103);
            this.Prove.Name = "Prove";
            this.Prove.Size = new System.Drawing.Size(123, 23);
            this.Prove.TabIndex = 4;
            this.Prove.Text = "Prove Possible";
            this.Prove.UseVisualStyleBackColor = true;
            this.Prove.Click += new System.EventHandler(this.Prove_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1041, 132);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(155, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "Remove All Solutions";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 515);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.Prove);
            this.Controls.Add(this.tryButton);
            this.Controls.Add(this.Load);
            this.Controls.Add(this.saveFile);
            this.Controls.Add(this.textBox1);
            this.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button saveFile;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private new System.Windows.Forms.Button Load;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button tryButton;
        private System.Windows.Forms.Button Prove;
        private System.Windows.Forms.Button button1;
    }
}

