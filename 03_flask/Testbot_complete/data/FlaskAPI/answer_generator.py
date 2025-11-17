from ollama import generate
from translation import translate_to_de
from datetime import datetime

def retrieve_answer(context: str, question: str, language: str, history: list) -> str:
    print(f"\n\nHISTORY: {history}\n\n")
    prompt = ""
    starttime = datetime.now()
    if history == None or history == []:
        prompt = f"Answer the question in the according language: '{question}'.\n Use the following information for your answer: '{context}'. Answer in English."
    else:
        prompt = f"Answer the question in the according language: '{question}'.\n Take into account the previous conversation: '{history}' and use the following information for your answer: '{context}'. Answer in English."

    # model = "smollm2:1.7b"
    # model = "llama3.2:1b"
    # model = "llama3.2:3b-instruct-q8_0"
    # model = "llama3.2:3b-instruct-q5_0"
    # model = "llama3.2:3b-instruct-q4_0"
    # model = "llama3.2:3b-instruct-q3_K_S"
    # model = "llama3.2:3b-instruct-q4_K_S"
    # model = "llama3.2:3b-instruct-q5_K_S"
    model = "llama3.2:3b"

    response = generate(model, prompt)
    answer_en = response['response']
    answer_time = (datetime.now() - starttime).total_seconds()
    answer_de = ""
    answer = ""
    en_de_time = 0
    if language=="DE": 
        starttime = datetime.now()
        answer_de = translate_to_de(answer_en)
        en_de_time = (datetime.now() - starttime).total_seconds()
        answer = answer_de
    if language=="EN":
        answer = answer_en    
     
    return answer, answer_de, answer_en, answer_time, en_de_time, model