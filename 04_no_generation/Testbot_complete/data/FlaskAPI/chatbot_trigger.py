import subprocess
import os

def run_chatbot(topic: str):
    exe_path = "..\\Chatbot\\ChatbotWPF\\bin\\Release\\net8.0-windows\\ChatbotWPF.exe"
    env = os.environ.copy()
    env['TOPIC'] = topic
    subprocess.Popen([exe_path], env=env)