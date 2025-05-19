#
# Copyright (c) Microsoft. All rights reserved.
# To learn more, please visit the documentation - Quickstart: Azure Content Safety: https://aka.ms/acsstudiodoc
#

import requests

# Replace with your own subscription_key/aad_token and endpoint
# Choose to use key authorization or AAD token authorization
subscription_key = "your_subscription_key"
endpoint = "your_resource_endpoint"
aad_token = "your_aad_token"


# The code to be analyzed
code_to_analyze = """your_code_here"""

# Set up the API request
url = f"{endpoint}/contentsafety/text:detectProtectedMaterialForCode?api-version=2024-09-15-preview"        
headers = {
  "Content-Type": "application/json",
  "Ocp-Apim-Subscription-Key": subscription_key,
  "Authorization": aad_token
}
data = {
  "code": code_to_analyze
}

# Send the API request
response = requests.post(url, headers=headers, json=data)

# Handle the API response
if response.status_code == 200:
    result = response.json()
    print("Analysis result:", result)
else:
    print("Error:", response.status_code, response.text)