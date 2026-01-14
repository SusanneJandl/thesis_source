# langflow_prompt/prompt.py


import requests
from typing import Optional

def query_langflow(message: str) -> Optional[str]:
    """
    Send a message to the Langflow backend and retrieve the extracted text.
    :param message: The input message to send.
    :return: Extracted text if successful, None otherwise.
    """
    url = "http://127.0.0.1:7888/api/v1/run/ff53b3b5-c9db-468f-abc6-5d34899b34dc"
    headers = {"Content-Type": "application/json"}
    data = {"input_value": message}

    try:
        response = requests.post(url, headers=headers, json=data)
        response.raise_for_status()  # Raise an exception for HTTP errors
        result = response.json()
        return extract_text(result)
    except requests.exceptions.RequestException as e:
        print(f"Request failed: {e}")
        return None

def extract_text(response_json: dict) -> Optional[str]:
    """
    Safely extract the text from the JSON response.
    :param response_json: The JSON response from the Langflow backend.
    :return: Extracted text if present, None otherwise.
    """
    try:
        text = response_json["outputs"][0]["outputs"][0]["results"]["message"]["data"]["text"]
        return text
    except (KeyError, IndexError) as e:
        print(f"Error extracting text: {e}")
        return None
