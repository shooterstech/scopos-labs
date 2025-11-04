namespace MatchResultsToExcel
{
	partial class MainForm
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
			buttonFindResults = new Button();
			label1 = new Label();
			textBoxMatchId = new TextBox();
			panel1 = new Panel();
			buttonSaveExcel = new Button();
			SuspendLayout();
			// 
			// buttonFindResults
			// 
			buttonFindResults.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			buttonFindResults.Location = new Point(242, 24);
			buttonFindResults.Name = "buttonFindResults";
			buttonFindResults.Size = new Size(123, 23);
			buttonFindResults.TabIndex = 0;
			buttonFindResults.Text = "Find Result Lists";
			buttonFindResults.UseVisualStyleBackColor = true;
			buttonFindResults.Click += buttonFindResults_Click;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.FlatStyle = FlatStyle.Flat;
			label1.Font = new Font("Segoe UI", 9F);
			label1.Location = new Point(12, 27);
			label1.Name = "label1";
			label1.Size = new Size(82, 15);
			label1.TabIndex = 1;
			label1.Text = "Enter MatchID";
			// 
			// textBoxMatchId
			// 
			textBoxMatchId.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			textBoxMatchId.Location = new Point(100, 24);
			textBoxMatchId.Name = "textBoxMatchId";
			textBoxMatchId.Size = new Size(136, 23);
			textBoxMatchId.TabIndex = 2;
			// 
			// panel1
			// 
			panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			panel1.AutoScroll = true;
			panel1.Location = new Point(12, 53);
			panel1.Name = "panel1";
			panel1.Size = new Size(353, 492);
			panel1.TabIndex = 3;
			// 
			// buttonSaveExcel
			// 
			buttonSaveExcel.Anchor = AnchorStyles.Bottom;
			buttonSaveExcel.Enabled = false;
			buttonSaveExcel.Location = new Point(133, 559);
			buttonSaveExcel.Name = "buttonSaveExcel";
			buttonSaveExcel.Size = new Size(113, 26);
			buttonSaveExcel.TabIndex = 4;
			buttonSaveExcel.Text = "Save To Excel";
			buttonSaveExcel.UseVisualStyleBackColor = true;
			buttonSaveExcel.Click += buttonSaveExcel_Click;
			// 
			// MainForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(379, 594);
			Controls.Add(buttonSaveExcel);
			Controls.Add(panel1);
			Controls.Add(textBoxMatchId);
			Controls.Add(label1);
			Controls.Add(buttonFindResults);
			Name = "MainForm";
			Text = "Match Results To Excel";
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private Button buttonFindResults;
		private Label label1;
		private TextBox textBoxMatchId;
		private Panel panel1;
		private Button buttonSaveExcel;
	}
}
