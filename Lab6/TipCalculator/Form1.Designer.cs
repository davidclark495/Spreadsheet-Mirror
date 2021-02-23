
namespace TipCalculator
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
            this.BillLabel = new System.Windows.Forms.Label();
            this.CalcButton = new System.Windows.Forms.Button();
            this.BillText = new System.Windows.Forms.TextBox();
            this.TipAmountText = new System.Windows.Forms.TextBox();
            this.TipPercentText = new System.Windows.Forms.TextBox();
            this.TipPercentLabel = new System.Windows.Forms.Label();
            this.TipAmountLabel = new System.Windows.Forms.Label();
            this.TotalAmountLabel = new System.Windows.Forms.Label();
            this.TotalAmountText = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // BillLabel
            // 
            this.BillLabel.AutoSize = true;
            this.BillLabel.Location = new System.Drawing.Point(246, 99);
            this.BillLabel.Name = "BillLabel";
            this.BillLabel.Size = new System.Drawing.Size(23, 13);
            this.BillLabel.TabIndex = 0;
            this.BillLabel.Text = "Bill:";
            this.BillLabel.Click += new System.EventHandler(this.TotalLabel_Click);
            // 
            // CalcButton
            // 
            this.CalcButton.Enabled = false;
            this.CalcButton.Location = new System.Drawing.Point(289, 198);
            this.CalcButton.Name = "CalcButton";
            this.CalcButton.Size = new System.Drawing.Size(75, 23);
            this.CalcButton.TabIndex = 3;
            this.CalcButton.Text = "Calculate";
            this.CalcButton.UseVisualStyleBackColor = true;
            this.CalcButton.Click += new System.EventHandler(this.CalcButton_Click);
            // 
            // BillText
            // 
            this.BillText.Location = new System.Drawing.Point(335, 92);
            this.BillText.Name = "BillText";
            this.BillText.Size = new System.Drawing.Size(100, 20);
            this.BillText.TabIndex = 4;
            this.BillText.TextChanged += new System.EventHandler(this.BillText_TextChanged);
            // 
            // TipAmountText
            // 
            this.TipAmountText.Location = new System.Drawing.Point(335, 248);
            this.TipAmountText.Name = "TipAmountText";
            this.TipAmountText.ReadOnly = true;
            this.TipAmountText.Size = new System.Drawing.Size(100, 20);
            this.TipAmountText.TabIndex = 5;
            // 
            // TipPercentText
            // 
            this.TipPercentText.Location = new System.Drawing.Point(335, 153);
            this.TipPercentText.Name = "TipPercentText";
            this.TipPercentText.Size = new System.Drawing.Size(100, 20);
            this.TipPercentText.TabIndex = 6;
            this.TipPercentText.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.TipPercentText.TextChanged += new System.EventHandler(this.TipPercentText_TextChanged);
            // 
            // TipPercentLabel
            // 
            this.TipPercentLabel.AutoSize = true;
            this.TipPercentLabel.Location = new System.Drawing.Point(246, 153);
            this.TipPercentLabel.Name = "TipPercentLabel";
            this.TipPercentLabel.Size = new System.Drawing.Size(36, 13);
            this.TipPercentLabel.TabIndex = 7;
            this.TipPercentLabel.Text = "Tip %:";
            // 
            // TipAmountLabel
            // 
            this.TipAmountLabel.AutoSize = true;
            this.TipAmountLabel.Location = new System.Drawing.Point(216, 251);
            this.TipAmountLabel.Name = "TipAmountLabel";
            this.TipAmountLabel.Size = new System.Drawing.Size(64, 13);
            this.TipAmountLabel.TabIndex = 8;
            this.TipAmountLabel.Text = "Tip Amount:";
            // 
            // TotalAmountLabel
            // 
            this.TotalAmountLabel.AutoSize = true;
            this.TotalAmountLabel.Location = new System.Drawing.Point(216, 299);
            this.TotalAmountLabel.Name = "TotalAmountLabel";
            this.TotalAmountLabel.Size = new System.Drawing.Size(73, 13);
            this.TotalAmountLabel.TabIndex = 9;
            this.TotalAmountLabel.Text = "Total Amount:";
            this.TotalAmountLabel.Click += new System.EventHandler(this.label1_Click);
            // 
            // TotalAmountText
            // 
            this.TotalAmountText.Location = new System.Drawing.Point(335, 292);
            this.TotalAmountText.Name = "TotalAmountText";
            this.TotalAmountText.ReadOnly = true;
            this.TotalAmountText.Size = new System.Drawing.Size(100, 20);
            this.TotalAmountText.TabIndex = 10;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(316, 95);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(13, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "$";
            this.label1.Click += new System.EventHandler(this.label1_Click_1);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(441, 156);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(15, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "%";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(316, 251);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(13, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "$";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(316, 292);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(13, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "$";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TotalAmountText);
            this.Controls.Add(this.TotalAmountLabel);
            this.Controls.Add(this.TipAmountLabel);
            this.Controls.Add(this.TipPercentLabel);
            this.Controls.Add(this.TipPercentText);
            this.Controls.Add(this.TipAmountText);
            this.Controls.Add(this.BillText);
            this.Controls.Add(this.CalcButton);
            this.Controls.Add(this.BillLabel);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label BillLabel;
        private System.Windows.Forms.Button CalcButton;
        private System.Windows.Forms.TextBox BillText;
        private System.Windows.Forms.TextBox TipAmountText;
        private System.Windows.Forms.TextBox TipPercentText;
        private System.Windows.Forms.Label TipPercentLabel;
        private System.Windows.Forms.Label TipAmountLabel;
        private System.Windows.Forms.Label TotalAmountLabel;
        private System.Windows.Forms.TextBox TotalAmountText;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}

