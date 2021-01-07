namespace QBCodePay
{
    partial class QBCodePay
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnGets = new System.Windows.Forms.Button();
            this.btnPuts = new System.Windows.Forms.Button();
            this.txtEndPint = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnPost = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnGets
            // 
            this.btnGets.Font = new System.Drawing.Font("Yu Gothic UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnGets.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnGets.Location = new System.Drawing.Point(154, 125);
            this.btnGets.Name = "btnGets";
            this.btnGets.Size = new System.Drawing.Size(140, 32);
            this.btnGets.TabIndex = 2;
            this.btnGets.Text = "GET";
            this.btnGets.UseVisualStyleBackColor = true;
            this.btnGets.Click += new System.EventHandler(this.btnGets_Click);
            // 
            // btnPuts
            // 
            this.btnPuts.Font = new System.Drawing.Font("Yu Gothic UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnPuts.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.btnPuts.Location = new System.Drawing.Point(154, 163);
            this.btnPuts.Name = "btnPuts";
            this.btnPuts.Size = new System.Drawing.Size(140, 32);
            this.btnPuts.TabIndex = 3;
            this.btnPuts.Text = "PUT";
            this.btnPuts.UseVisualStyleBackColor = true;
            this.btnPuts.Click += new System.EventHandler(this.btnPuts_Click);
            // 
            // txtEndPint
            // 
            this.txtEndPint.Location = new System.Drawing.Point(12, 42);
            this.txtEndPint.Name = "txtEndPint";
            this.txtEndPint.Size = new System.Drawing.Size(456, 23);
            this.txtEndPint.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Yu Gothic UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "EndPoint";
            // 
            // btnPost
            // 
            this.btnPost.Font = new System.Drawing.Font("Yu Gothic UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnPost.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.btnPost.Location = new System.Drawing.Point(154, 87);
            this.btnPost.Name = "btnPost";
            this.btnPost.Size = new System.Drawing.Size(140, 32);
            this.btnPost.TabIndex = 1;
            this.btnPost.Text = "POST";
            this.btnPost.UseVisualStyleBackColor = true;
            // 
            // QBCodePay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(480, 218);
            this.Controls.Add(this.btnPost);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtEndPint);
            this.Controls.Add(this.btnPuts);
            this.Controls.Add(this.btnGets);
            this.Name = "QBCodePay";
            this.Text = "QBCodePay";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnGets;
        private System.Windows.Forms.Button btnPuts;
        private System.Windows.Forms.TextBox txtEndPint;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnPost;
    }
}

