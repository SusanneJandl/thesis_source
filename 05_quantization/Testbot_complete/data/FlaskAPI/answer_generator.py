from ollama import generate
from datetime import datetime

def retrieve_answer(context: str, question: str, language: str, model: str) -> str:
    prompt = ""
    starttime = datetime.now()
    if language=="DE":
        prompt = f"Beantworte die Frage: '{question}'.\n Nutze die folgende Information für deine Antwort: '{context}'. Antworte auf Deutsch."
    else:
        prompt = f"Answer the question: '{question}'.\n Use the following information for your answer: '{context}'. Answer in English."

    # model = "smollm2:1.7b"
    # model = "llama3.2:1b"
    # model = "llama3.2:3b-instruct-q8_0"
    # model = "llama3.2:3b-instruct-q5_0"
    # model = "llama3.2:3b-instruct-q4_0"
    # model = "llama3.2:3b-instruct-q3_K_S"
    # model = "llama3.2:3b-instruct-q4_K_S"
    # model = "llama3.2:3b-instruct-q5_K_S"
    # model = "llama3.2:3b"

    response = generate(model, prompt)
    answer = response['response']
    answer_time = (datetime.now() - starttime).total_seconds() 
     
    return answer, answer_time