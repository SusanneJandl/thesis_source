@echo off
REM -------------------------------
REM Step 1: Activate Python environment
REM -------------------------------
echo Activating python environment...
call ".\data\chat_env\Scripts\activate"

REM -------------------------------
REM Step 2: Start Flask API with Waitress (MINIMIZED)
REM -------------------------------
echo Starting FlaskAPI with Waitress...
cd .\data\FlaskAPI\
start /min cmd /k "waitress-serve --host=0.0.0.0 --port=5000 app:app"

REM -------------------------------
REM Step 3: Start Ollama (MINIMIZED)
REM -------------------------------
echo Starting Ollama...
start /min cmd /k "ollama serve"

REM -------------------------------
REM Step 4: Start Chatbot UI
REM -------------------------------
echo Starting Chatbot...
cd ..\UI\
start "" ChatbotWPF.exe

echo Chatbot started successfully.
exit
