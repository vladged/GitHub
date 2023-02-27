namespace PowerBiMonitor_Scheduler
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
            this.CollectUserActivity = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // CollectUserActivity
            // 
            this.CollectUserActivity.Location = new System.Drawing.Point(37, 54);
            this.CollectUserActivity.Name = "CollectUserActivity";
            this.CollectUserActivity.Size = new System.Drawing.Size(138, 23);
            this.CollectUserActivity.TabIndex = 0;
            this.CollectUserActivity.Text = "Collect UserActivity";
            this.CollectUserActivity.UseVisualStyleBackColor = true;
            this.CollectUserActivity.Click += new System.EventHandler(this.CollectUserActivity_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(37, 106);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(138, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Send Test Message";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.SendTestMessage);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.CollectUserActivity);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button CollectUserActivity;
        private System.Windows.Forms.Button button1;
    }
}

