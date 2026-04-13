from ollama import generate
from datetime import datetime
from token_counter import count_tokens

def retrieve_answer(context: str, question: str, language: str, model: str) -> str:
    prompt = ""
    starttime = datetime.now()
    if language=="DE":
        prompt = f"Beantworte die Frage: '{question}'.\n Nutze die folgende Information für deine Antwort: '{context}'. Antworte auf Deutsch."
    else:
        prompt = f"Answer the question: '{question}'.\n Use the following information for your answer: '{context}'. Answer in English."

    response = generate(model, prompt)
    answer = response['response']
    answer_time = (datetime.now() - starttime).total_seconds() 
    answer += count_tokens(answer)
     
    return answer, answer_time