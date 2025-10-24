@echo off
REM Step 1: Activate Python environment
echo Activating python environment...
call .\data\chat_env\Scripts\activate

REM Step 2: Start FlaskAPI
echo Starting FlaskAPI...
cd .\data\FlaskAPI\
start cmd /k "python app.py"

REM Step 3: Start Ollama
echo Starting Ollama...
start cmd /k "ollama serve"

REM Step 4: Start Chatbot
echo Starting Chatbot...
cd ..\UI\
start ChatbotWPF.exe

echo Chatbot started successfully
exit