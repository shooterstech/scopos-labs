namespace MatchResultsToExcel
{
	partial class Form1
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
			button1 = new Button();
			label1 = new Label();
			textBox1 = new TextBox();
			panel1 = new Panel();
			button2 = new Button();
			SuspendLayout();
			// 
			// button1
			// 
			button1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			button1.Location = new Point(242, 24);
			button1.Name = "button1";
			button1.Size = new Size(123, 23);
			button1.TabIndex = 0;
			button1.Text = "Find Result Lists";
			button1.UseVisualStyleBackColor = true;
			button1.Click += button1_Click;
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
			// textBox1
			// 
			textBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			textBox1.Location = new Point(100, 24);
			textBox1.Name = "textBox1";
			textBox1.Size = new Size(136, 23);
			textBox1.TabIndex = 2;
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
			// button2
			// 
			button2.Anchor = AnchorStyles.Bottom;
			button2.Enabled = false;
			button2.Location = new Point(133, 559);
			button2.Name = "button2";
			button2.Size = new Size(113, 26);
			button2.TabIndex = 4;
			button2.Text = "Save To Excel";
			button2.UseVisualStyleBackColor = true;
			button2.Click += button2_Click;
			// 
			// Form1
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(379, 594);
			Controls.Add(button2);
			Controls.Add(panel1);
			Controls.Add(textBox1);
			Controls.Add(label1);
			Controls.Add(button1);
			Name = "Form1";
			Text = "Match Results To Excel";
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private Button button1;
		private Label label1;
		private TextBox textBox1;
		private Panel panel1;
		private Button button2;
	}
}
