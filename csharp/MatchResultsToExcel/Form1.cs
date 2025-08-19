using Microsoft.VisualBasic;
using Scopos.BabelFish.APIClients;
using Scopos.BabelFish.DataModel.OrionMatch;
using Scopos.BabelFish.Runtime;

namespace MatchResultsToExcel
{
	public partial class Form1 : Form
	{
		private int controlCount = 0;
		private OrionMatchAPIClient _matchClient;
		 
		public Form1()
		{
			InitializeComponent();
			//if you use this code to develop an application, you'll need to request your own x api key
			Initializer.Initialize("GyaHV300my60rs2ylKug5aUgFnYBj6GrU6V1WE33", false); 
			_matchClient = new OrionMatchAPIClient();
		}

		private async void button1_Click(object sender, EventArgs e) //find result lists and populate panel
		{
			MatchID matchId;
			try
			{
				matchId = new MatchID(textBox1.Text);
			}
			catch (FormatException)
			{
				MessageBox.Show("Enter a valid MatchID to find result lists.");
				return;

			}
			button1.Enabled = false;
			button1.Text = "loading...";
			panel1.Controls.Clear();
			


			

			var matchDetailResponse = await _matchClient.GetMatchPublicAsync(matchId);

			var resultEvents = matchDetailResponse.Match.ResultEvents;
			int yPos = 30;
			int yInc = 40;
			int items = 0;
			for (int i = 0; i < resultEvents.Count; ++i)
			{
				// Create Label For Event Name
				Label eventLbl = new Label();
				eventLbl.Text = resultEvents[i].DisplayName;
				eventLbl.Font = new Font(eventLbl.Font, FontStyle.Bold);
				eventLbl.AutoSize = true;
				eventLbl.Location = new System.Drawing.Point(20, (items++) * 30 +10);
				panel1.Controls.Add(eventLbl);

				var resultLists = resultEvents[i].ResultLists;
				for(int j = 0; j < resultLists.Count; ++j)
				{
					//for each result list within the event, create dropdown

					// Create ComboBox
					ComboBox rlCmb = new ComboBox();
					rlCmb.DropDownStyle = ComboBoxStyle.DropDownList;
					rlCmb.Location = new System.Drawing.Point(155, (items) * 30);
					rlCmb.Width = 40;
					

					// Create Label
					Label rlLbl = new Label();
					rlLbl.Text = resultLists[j].ResultName;
					rlLbl.AutoSize = true;
					rlLbl.Location = new System.Drawing.Point(25,  5+ (items++) * 30);

					

					for (int k = 0; k <= 100; k++)
						rlCmb.Items.Add(k);
					rlCmb.SelectedIndex = 0;

					// Add to panel (not the form)
					panel1.Controls.Add(rlLbl);
					panel1.Controls.Add(rlCmb);

				}

			}


			/*
			controlCount++;

			// Create Label
			Label lbl = new Label();
			lbl.Text = "Select number " + controlCount + ":";
			lbl.AutoSize = true;
			lbl.Location = new System.Drawing.Point(20, 30 + (controlCount - 1) * 40);

			// Create ComboBox
			ComboBox cmb = new ComboBox();
			cmb.DropDownStyle = ComboBoxStyle.DropDownList;
			cmb.Location = new System.Drawing.Point(150, 25 + (controlCount - 1) * 40);
			cmb.Width = 80;

			// Fill with numbers 0–10
			for (int i = 0; i <= 10; i++)
				cmb.Items.Add(i);

			// Add to panel (not the form)
			panel1.Controls.Add(lbl);
			panel1.Controls.Add(cmb);
			*/


			button1.Text = "Find Result Lists";
			button1.Enabled = true;
			

		}

		private void button2_Click(object sender, EventArgs e) //save to excel
		{

		}
	}
}
