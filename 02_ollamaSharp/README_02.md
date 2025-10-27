## Use C# app with langflow and ollama

## Prerequisites for this project

- install Ollama from [https://ollama.com](https://ollama.com/download)
- start ollama
  - `ollama serve`
  - ollama will run on localhost:11434
- pull models
  - `ollama pull llama3.2:3b`
  - `ollama pull all-minilm:22m`

### installation

- download and install Visual Studio from [https://visualstudio.microsoft.com/de/downloads/](https://visualstudio.microsoft.com/de/downloads) (or other IDE for C#)
- install Python from [https://www.python.org](https://www.python.org/downloads/) if necessary
  - version 3.12.7 used in this project (for langflow installation python 3.10 or 3.12 is required)
- create a virtual environment
  - `python -m venv langflow_env`
- activate virtual environment
  - `.\langflow_env\Scripts\activate`
- navigate to `.\03_chat_with_widget` and install requirements
  - `pip install -r requirements.txt`

### run langflow UI

- activate virtual environment
  - `.\langflow_env\Scripts\activate`
- run `python -m langflow run`
- go to `localhost:7860`

### add flows

- on the top left click `Add Flow` and add a new flow
  ![add flow](../img/Fig_langflowUpload.png)
- select all json files in `.\03_chat_with_widget\FlowIDs` and click `open`
- open the flows and adapt filepaths in Faiss and Directory cards

### run langflow in background

- to specify a port and run in background: `python -m langflow run --port 7888 --backend-only`

### start chatbot

- open `.\02_chat_with_langflow_vs\ollama_langflow\ollama_langflow.sln` with Visual Studio
  - build the solution
  - a python environment will be created in bin folder
  - copy `.\02_chat_with_langflow_vs\langflow_prompt` folder to `.\02_chat_with_langflow_vs\ollama_langflow\bin\Debug\net8.0\python-3.11.0-embed-amd64\Lib\site-packages\`.
  - run the solution