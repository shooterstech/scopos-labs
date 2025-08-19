using Microsoft.VisualBasic;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Scopos.BabelFish.APIClients;
using Scopos.BabelFish.DataModel.Athena.Match;
using Scopos.BabelFish.DataModel.Definitions;
using Scopos.BabelFish.DataModel.OrionMatch;
using Scopos.BabelFish.Requests.OrionMatchAPI;
using Scopos.BabelFish.Responses.OrionMatchAPI;
using Scopos.BabelFish.Runtime;
using System.ComponentModel;

namespace MatchResultsToExcel
{
	/*
	 * Match Results To Excel Form
	 * 
	 * This form allows someone to export a subset of match results to an excel file.
	 * Enter the Match ID, then select the number of results from each Result List to export. 
	 * For example, selecting 3 for Individual - All will export the top three competitors Rank, Display Name, and Score. 
	 */
	public partial class MainForm : Form
	{
		private OrionMatchAPIClient _matchClient;
		private List<Label> _resultLabels;
		private List<ComboBox> _resultBoxes;
		private MatchID _matchId;

		private GetMatchPublicResponse _matchResponse;
		public MainForm()
		{
			InitializeComponent();
			//if you use this code to develop an application, you'll need to request your own x api key
			Initializer.Initialize("GyaHV300my60rs2ylKug5aUgFnYBj6GrU6V1WE33", false);
			_matchClient = new OrionMatchAPIClient();
			_resultLabels = new List<Label>();
			_resultBoxes = new List<ComboBox>();
			ExcelPackage.License.SetNonCommercialOrganization("Scopos Labs");
		}

		private async void buttonFindResults_Click(object sender, EventArgs e) //find result lists and populate panel
		{
			try
			{
				_matchId = new MatchID(textBoxMatchId.Text);
			}
			catch (FormatException)
			{
				MessageBox.Show("Enter a valid MatchID to find result lists.");
				return;

			}
			buttonFindResults.Text = "loading...";
			buttonFindResults.Enabled = false;
			buttonSaveExcel.Enabled = false;

			//clear previous match info
			panel1.Controls.Clear();
			_resultLabels.Clear();
			_resultBoxes.Clear();

			
			try
			{
				//GetMatchDetail API Call https://app.swaggerhub.com/apis-docs/Shooters-Technology/api/1.8#/Orion%20Match/GetMatchDetail 
				_matchResponse = await _matchClient.GetMatchPublicAsync(_matchId);
				if (_matchResponse.StatusCode != System.Net.HttpStatusCode.OK)
				{
					throw new Exception(_matchResponse.ExceptionMessage);
				}

				var resultEvents = _matchResponse.Match.ResultEvents;
				int items = 0;
				for (int i = 0; i < resultEvents.Count; ++i) 
				{
					//each Result Event contains associated Result Lists

					// Create label for Result Event name
					Label eventLbl = new Label();
					eventLbl.Text = resultEvents[i].DisplayName;
					eventLbl.Font = new Font(eventLbl.Font, FontStyle.Bold);
					eventLbl.AutoSize = true;
					eventLbl.Location = new System.Drawing.Point(20, (items++) * 30 + 10);
					panel1.Controls.Add(eventLbl);

					var resultLists = resultEvents[i].ResultLists;
					for (int j = 0; j < resultLists.Count; ++j)
					{
						//for each Result List within the Result Event, create a label and dropdown

						ComboBox rlCmb = new ComboBox();
						rlCmb.DropDownStyle = ComboBoxStyle.DropDownList;
						rlCmb.Location = new System.Drawing.Point(155, (items) * 30);
						rlCmb.Width = 40;

						Label rlLbl = new Label();
						rlLbl.Text = resultLists[j].ResultName;
						rlLbl.AutoSize = true;
						rlLbl.Location = new System.Drawing.Point(25, 5 + (items++) * 30);

						for (int k = 0; k <= 100; k++) //populate dropdown with 1-100
							rlCmb.Items.Add(k);

						rlCmb.SelectedIndex = 0; //default to 0 selected

						//keep track of the created labels and combo boxes
						_resultLabels.Add(rlLbl); 
						_resultBoxes.Add(rlCmb);

						// Add to panel
						panel1.Controls.Add(rlLbl);
						panel1.Controls.Add(rlCmb);

					}

				}

				buttonSaveExcel.Enabled = true; //only enable save button if results were successfully loaded 
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Failed to get results for Match ID {_matchId.ToString()}: {ex.Message}");
				
			}
			finally
			{
				buttonFindResults.Text = "Find Result Lists";
				buttonFindResults.Enabled = true;
			}

		}

		private async void buttonSaveExcel_Click(object sender, EventArgs e) //save to excel
		{

			if (_resultBoxes.All(cmb => cmb.SelectedIndex == 0)) //if all result lists are selected as '0'
			{
				MessageBox.Show("Zero results are selected");
				return;
			}

			buttonSaveExcel.Enabled = false;
			buttonSaveExcel.Text = "Saving...";
			
			try
			{
				string filePath;
				using (var saveFileDialog = new SaveFileDialog())
				{
					saveFileDialog.Title = "Save Match Results";
					saveFileDialog.Filter = "Excel Files|*.xlsx";
					saveFileDialog.FileName = $"{_matchResponse.Match.Name}_MatchResults.xlsx";
					if (saveFileDialog.ShowDialog() == DialogResult.OK)
					{
						filePath = saveFileDialog.FileName;
					}
					else
					{
						return;
					}
				}

				using (var package = new ExcelPackage())
				{
					for (int i = 0; i < _resultLabels.Count; ++i)
					{
						var resultName = _resultLabels[i].Text;
						var numResults = _resultBoxes[i].SelectedIndex; //selected index is equivalent to selected number

						if (numResults == 0)
						{
							continue;
						}
						//GetResultList API Call https://app.swaggerhub.com/apis-docs/Shooters-Technology/api/1.8#/Orion%20Match/GetResultList 
						var request = new GetResultListPublicRequest(_matchId, resultName);
						var resultListResponse = await _matchClient.GetResultListPublicAsync(request);

						var results = resultListResponse.ResultList.Items;
						var eventName = resultListResponse.ResultList.EventName; //The EventName that the ResultList ranks based on (Qualification, Standing, Kneeling, ...)

						// Create a new worksheet for this event
						var ws = package.Workbook.Worksheets.Add(resultName);

						// --- Header Row ---
						ws.Cells[1, 1].Value = "Rank";
						ws.Cells[1, 2].Value = "Display Name";
						ws.Cells[1, 3].Value = "Score";

						using (var headerRange = ws.Cells[1, 1, 1, 3])
						{
							headerRange.Style.Font.Bold = true;
							headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
							headerRange.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
						}


						for (int rInd = 0; rInd < numResults && rInd < results.Count; ++rInd)
						{
							var cellRow = rInd + 2;
							ws.Cells[cellRow, 1].Value = results[rInd].Rank;
							ws.Cells[cellRow, 2].Value = results[rInd].Participant.DisplayName; 
							ws.Cells[cellRow, 3].Value = results[rInd].EventScores[eventName].ScoreFormatted;
						}

						// Auto-fit columns
						ws.Cells[ws.Dimension.Address].AutoFitColumns();
						ws.View.FreezePanes(2, 1); // freeze top row
					}

					// Save file
					package.SaveAs(new FileInfo(filePath));

				}
				MessageBox.Show($"Successfully saved results to {filePath}");
			}
			catch(Exception ex)
			{
				MessageBox.Show($"Failed to save result data: {ex.Message}");
			}
			finally
			{
				buttonSaveExcel.Enabled = true;
				buttonSaveExcel.Text = "Save To Excel";
			}
			

			
		}
	}
}
