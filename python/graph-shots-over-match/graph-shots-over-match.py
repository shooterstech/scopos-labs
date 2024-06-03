import requests
import matplotlib
import matplotlib.pyplot as plt
from datetime import datetime, timedelta
import numpy as np
import argparse

"""
This python script graphs the shot scores of an athlete from a single match over time. 
If the time taken between two shots is greater than one minute, the interval is highlighted in red.
The shot data from the match is retrieved using the Scopos public GetResultCourseOfFire API.
More detailed documentation of this API can be found at https://app.swaggerhub.com/apis/Shooters-Technology/api/1.6.2#/Orion%20Match/GetResultCourseOfFire for documentation.

Ensure that the chosen result course of fire is from a electronically scored match that includes the time each shot was fired. 
If the retrieved match has shots that were scored at the same time, an assertion exception will be thrown. 

This script requires the matplotlib library. To install use:
	pip install matplotlib

Usage:
python ./graph-shots-over-match.py -h


"""

ap = argparse.ArgumentParser()
ap.add_argument("-r", "--rcofid", required = True, type=str, help = "The Result Course of Fire ID to graph.")
args = ap.parse_args()

#This is a shared API Key that is rate limited. Visit www.scopos.tech/restapi to learn how to obtain your own.
xApiKey = "GyaHV300my60rs2ylKug5aUgFnYBj6GrU6V1WE33"


athenaAPIUrl = "https://api.orionscoringsystem.com"
getRCOFPath = "/resultcof/{rcofId}"


def MakeAPICall(api_url, query_params = {}):
	headers = {
		"x-api-key": xApiKey
	}
	response_get = requests.get(api_url, headers=headers, params=query_params)
	# Check if the GET request was successful
	if response_get.status_code == 200:
		#Parse API response
	    data_get = response_get.json()
	    return data_get
	else:
	    raise Exception('Failed to retrieve data with GET:', response_get.status_code, response_get.text)


def GetResultCOF(rcofId):
	#Use the Scopos public API to retrieve a ResultCOF detail
	#This detail contains data for each shot taken during a match
	#See https://app.swaggerhub.com/apis/Shooters-Technology/api/1.6.2#/Orion%20Match/GetResultCourseOfFire for documentation
	fullUrl = athenaAPIUrl + getRCOFPath.format(rcofId=rcofId)
	apiResponse = MakeAPICall(fullUrl)
	return apiResponse.get("ResultCOF", {})

#Retrieve the result course of fire detail using the Scopos API
rcof = GetResultCOF(args.rcofid)

#The shots of the rcof are formatted as a dictionary where the key is the sequence number and the value is the shot information
shots = rcof["Shots"]

#Put the shots in an ordered list
shotsInOrder = [shots[str(i)] for i in range(1, len(shots)+1)]

#Retrieve the display name and match name to use for the title of the figure
displayName = rcof["Participant"]["DisplayName"]
matchName = rcof["MatchName"]


#Create a list of the scores in order and a list of the time each shot was scored
scoresInOrder = []
timesInOrder = []
for shot in shotsInOrder:
	scoresInOrder.append(shot["Score"]["D"])
	try:
		timeScored = datetime.strptime(shot["TimeScored"], '%Y-%m-%dT%H:%M:%S.%f')
	except ValueError:
		timeScored = datetime.strptime(shot["TimeScored"], '%Y-%m-%dT%H:%M:%S')
	timesInOrder.append(timeScored)

assert len(timesInOrder) == len(set(timesInOrder)), "Not all shots scored at separate times"

#Convert the list of datetimes to a list of seconds from the start of the match
secondsFromStart = list(map(lambda t: (t - timesInOrder[0]).total_seconds(), timesInOrder))

# Create a figure and axis
fig, ax = plt.subplots(figsize=(18, 5))

# Plot the data
ax.plot(secondsFromStart, scoresInOrder, marker='o', linestyle='-')



# Set the title and labels
ax.set_title(f'Shot Values From {displayName} During {matchName}')
ax.set_xlabel('Seconds From Start')
ax.set_ylabel('Shot Score')

# Set x-axis tick labels to be the actual datetime values
ax.set_xticks(secondsFromStart)

# Add vertical lines from each data point to the x-axis
for date, decimal in zip(secondsFromStart, scoresInOrder):
    ax.vlines(date, 0, decimal, color='gray', linestyle='--', alpha=0.5)

# Rotate date labels for better readability
plt.xticks(rotation=90)

# Set y-axis limits
ax.set_ylim(min(scoresInOrder)-.5, 11)


# Annotate the graph with the time difference between consecutive dates where the difference is greater than 60 seconds
for i in range(1, len(timesInOrder)):
    time_diff = (timesInOrder[i] - timesInOrder[i - 1]).total_seconds()
    if time_diff > 60:
        midpoint_x = (secondsFromStart[i] + secondsFromStart[i - 1]) / 2
        midpoint_y = max(scoresInOrder[i], scoresInOrder[i - 1]) + 0.02	
        
        # Draw the bracket
        ax.annotate('', xy=(secondsFromStart[i], scoresInOrder[i]), xytext=(secondsFromStart[i - 1], scoresInOrder[i - 1]),
                    arrowprops=dict(arrowstyle='-', lw=1.5, color='red'))

        # Add the text above the bracket
        ax.text(midpoint_x, midpoint_y, f'{time_diff:.0f} secs', ha='center', va='bottom', fontsize=8, color='red')



# Adjust layout to prevent clipping of labels
plt.tight_layout()


# Show the plot
plt.show()


