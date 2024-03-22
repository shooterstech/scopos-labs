import requests
import json

"""
Our take on a 'Hello World' project, that returns the number of coffee cups Scopos
engineers have consumed since they started working on Athena. 

We're not kidding, we actually do track this. https://www.scopos.tech/news/general/the-coffee-counter.html
"""

#This is a shared API Key that is rate limited. Visit www.scopos.tech/restapi to learn how to obtain your own.
xApiKey = "GyaHV300my60rs2ylKug5aUgFnYBj6GrU6V1WE33"

#GetCoffee is documented at https://app.swaggerhub.com/apis-docs/Shooters-Technology/api/1.5.4.0#/Scopos%20Data/GetCoffee
coffeeUrl = "https://api.scopos.tech/coffee"

#All Scopos' Rest API calls require the x-api-key included in the header
headers = {'x-api-key' : xApiKey }

#Make the call
getCoffeeRseponse = requests.get( coffeeUrl, headers=headers )

#check that the status is OK
if (getCoffeeRseponse.ok) :

	#All Scopos Rest API calls return json. Use .loads to read it into a python dictionary.
	getCoffeeJson = json.loads( getCoffeeRseponse.content )

	#Retreive the number of coffee cups consumed.
	numberOfCoffeeCups = getCoffeeJson["CupsOfCoffeeConsumed"]

	#Finally print it.
	print( "Scopos engineers have consumed {} cups of coffee since they began working on Athena.".format( numberOfCoffeeCups ) )

else :
	print( "Sadness, we didn't get a response." )