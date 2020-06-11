namespace StockToDatabase
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
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.helloWorldLabel = new System.Windows.Forms.Label();
            this.fromDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.toDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.clearDatabaseButton = new System.Windows.Forms.Button();
            this.fromLabel = new System.Windows.Forms.Label();
            this.toLabel = new System.Windows.Forms.Label();
            this.timeIntervalGroupBox = new System.Windows.Forms.GroupBox();
            this.folderButton = new System.Windows.Forms.Button();
            this.scanDatabaseButton = new System.Windows.Forms.Button();
            this.directoryGroupBox = new System.Windows.Forms.GroupBox();
            this.xmlProposalTexBox = new System.Windows.Forms.TextBox();
            this.timeIntervalGroupBox.SuspendLayout();
            this.directoryGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(116, 383);
            this.linkLabel1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(314, 13);
            this.linkLabel1.TabIndex = 0;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Click here for information about StockReader and StockAnalyzer!";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // helloWorldLabel
            // 
            this.helloWorldLabel.AutoSize = true;
            this.helloWorldLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helloWorldLabel.Location = new System.Drawing.Point(202, 19);
            this.helloWorldLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.helloWorldLabel.Name = "helloWorldLabel";
            this.helloWorldLabel.Size = new System.Drawing.Size(184, 26);
            this.helloWorldLabel.TabIndex = 3;
            this.helloWorldLabel.Text = "StockToDatabase";
            this.helloWorldLabel.Click += new System.EventHandler(this.helloWorldLabel_Click);
            // 
            // fromDateTimePicker
            // 
            this.fromDateTimePicker.Location = new System.Drawing.Point(45, 21);
            this.fromDateTimePicker.MinDate = new System.DateTime(2010, 5, 6, 0, 0, 0, 0);
            this.fromDateTimePicker.Name = "fromDateTimePicker";
            this.fromDateTimePicker.Size = new System.Drawing.Size(149, 20);
            this.fromDateTimePicker.TabIndex = 4;
            this.fromDateTimePicker.Value = new System.DateTime(2013, 7, 28, 0, 0, 0, 0);
            // 
            // toDateTimePicker
            // 
            this.toDateTimePicker.Location = new System.Drawing.Point(45, 59);
            this.toDateTimePicker.MinDate = new System.DateTime(2010, 5, 6, 0, 0, 0, 0);
            this.toDateTimePicker.Name = "toDateTimePicker";
            this.toDateTimePicker.Size = new System.Drawing.Size(149, 20);
            this.toDateTimePicker.TabIndex = 5;
            this.toDateTimePicker.Value = new System.DateTime(2013, 7, 28, 0, 0, 0, 0);
            this.toDateTimePicker.ValueChanged += new System.EventHandler(this.toDateTimePicker_ValueChanged);
            // 
            // clearDatabaseButton
            // 
            this.clearDatabaseButton.Location = new System.Drawing.Point(12, 275);
            this.clearDatabaseButton.Name = "clearDatabaseButton";
            this.clearDatabaseButton.Size = new System.Drawing.Size(91, 28);
            this.clearDatabaseButton.TabIndex = 6;
            this.clearDatabaseButton.Text = "Clear Database";
            this.clearDatabaseButton.UseVisualStyleBackColor = true;
            this.clearDatabaseButton.Click += new System.EventHandler(this.button2_Click);
            // 
            // fromLabel
            // 
            this.fromLabel.AutoSize = true;
            this.fromLabel.Location = new System.Drawing.Point(9, 27);
            this.fromLabel.Name = "fromLabel";
            this.fromLabel.Size = new System.Drawing.Size(30, 13);
            this.fromLabel.TabIndex = 7;
            this.fromLabel.Text = "From";
            this.fromLabel.Click += new System.EventHandler(this.label1_Click);
            // 
            // toLabel
            // 
            this.toLabel.AutoSize = true;
            this.toLabel.Location = new System.Drawing.Point(23, 65);
            this.toLabel.Name = "toLabel";
            this.toLabel.Size = new System.Drawing.Size(16, 13);
            this.toLabel.TabIndex = 8;
            this.toLabel.Text = "to";
            this.toLabel.Click += new System.EventHandler(this.label2_Click);
            // 
            // timeIntervalGroupBox
            // 
            this.timeIntervalGroupBox.Controls.Add(this.fromLabel);
            this.timeIntervalGroupBox.Controls.Add(this.toLabel);
            this.timeIntervalGroupBox.Controls.Add(this.fromDateTimePicker);
            this.timeIntervalGroupBox.Controls.Add(this.toDateTimePicker);
            this.timeIntervalGroupBox.Location = new System.Drawing.Point(12, 109);
            this.timeIntervalGroupBox.Name = "timeIntervalGroupBox";
            this.timeIntervalGroupBox.Size = new System.Drawing.Size(271, 94);
            this.timeIntervalGroupBox.TabIndex = 9;
            this.timeIntervalGroupBox.TabStop = false;
            this.timeIntervalGroupBox.Text = "Time Interval to Scan";
            // 
            // folderButton
            // 
            this.folderButton.Location = new System.Drawing.Point(6, 22);
            this.folderButton.Name = "folderButton";
            this.folderButton.Size = new System.Drawing.Size(240, 27);
            this.folderButton.TabIndex = 10;
            this.folderButton.Text = "Test";
            this.folderButton.UseVisualStyleBackColor = true;
            this.folderButton.Click += new System.EventHandler(this.folderButton_Click);
            // 
            // scanDatabaseButton
            // 
            this.scanDatabaseButton.Location = new System.Drawing.Point(109, 275);
            this.scanDatabaseButton.Name = "scanDatabaseButton";
            this.scanDatabaseButton.Size = new System.Drawing.Size(113, 28);
            this.scanDatabaseButton.TabIndex = 11;
            this.scanDatabaseButton.Text = "Scan to Database";
            this.scanDatabaseButton.UseVisualStyleBackColor = true;
            this.scanDatabaseButton.Click += new System.EventHandler(this.button4_Click);
            // 
            // directoryGroupBox
            // 
            this.directoryGroupBox.Controls.Add(this.folderButton);
            this.directoryGroupBox.Location = new System.Drawing.Point(12, 210);
            this.directoryGroupBox.Name = "directoryGroupBox";
            this.directoryGroupBox.Size = new System.Drawing.Size(271, 55);
            this.directoryGroupBox.TabIndex = 12;
            this.directoryGroupBox.TabStop = false;
            this.directoryGroupBox.Text = "Current Directory";
            // 
            // xmlProposalTexBox
            // 
            this.xmlProposalTexBox.Location = new System.Drawing.Point(316, 103);
            this.xmlProposalTexBox.Multiline = true;
            this.xmlProposalTexBox.Name = "xmlProposalTexBox";
            this.xmlProposalTexBox.Size = new System.Drawing.Size(281, 259);
            this.xmlProposalTexBox.TabIndex = 13;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(654, 453);
            this.Controls.Add(this.xmlProposalTexBox);
            this.Controls.Add(this.directoryGroupBox);
            this.Controls.Add(this.scanDatabaseButton);
            this.Controls.Add(this.timeIntervalGroupBox);
            this.Controls.Add(this.clearDatabaseButton);
            this.Controls.Add(this.helloWorldLabel);
            this.Controls.Add(this.linkLabel1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.timeIntervalGroupBox.ResumeLayout(false);
            this.timeIntervalGroupBox.PerformLayout();
            this.directoryGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label helloWorldLabel;
        private System.Windows.Forms.DateTimePicker fromDateTimePicker;
        private System.Windows.Forms.DateTimePicker toDateTimePicker;
        private System.Windows.Forms.Button clearDatabaseButton;
        private System.Windows.Forms.Label fromLabel;
        private System.Windows.Forms.Label toLabel;
        private System.Windows.Forms.GroupBox timeIntervalGroupBox;
        private System.Windows.Forms.Button folderButton;
        private System.Windows.Forms.Button scanDatabaseButton;
        private System.Windows.Forms.GroupBox directoryGroupBox;
        private System.Windows.Forms.TextBox xmlProposalTexBox;
    }
}

