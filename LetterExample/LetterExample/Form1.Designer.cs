namespace LetterExample
{
    partial class LetterExample
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
            this.btnGenerateLetter = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnGenerateLetter
            // 
            this.btnGenerateLetter.Location = new System.Drawing.Point(119, 162);
            this.btnGenerateLetter.Name = "btnGenerateLetter";
            this.btnGenerateLetter.Size = new System.Drawing.Size(109, 23);
            this.btnGenerateLetter.TabIndex = 0;
            this.btnGenerateLetter.Text = "Generate letter";
            this.btnGenerateLetter.UseVisualStyleBackColor = true;
            this.btnGenerateLetter.Click += new System.EventHandler(this.BtnGenerateLetter_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(347, 261);
            this.Controls.Add(this.btnGenerateLetter);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnGenerateLetter;
    }
}

