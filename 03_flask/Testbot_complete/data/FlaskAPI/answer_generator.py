from ollama import generate
from translation import translate_to_de
from datetime import datetime

def retrieve_answer(context: str, question: str, language: str, history: list) -> str:
    print(f"\n\nHISTORY: {history}\n\n")
    prompt = ""
    starttime = datetime.now()
    if history == None or history == []:
        prompt = f"Answer the question in the according language: '{question}'.\n Use the following information for your answer: '{context}'"
    else:
        prompt = f"Answer the question in the according language: '{question}'.\n Take into account the previous conversation: '{history}' and use the following information for your answer: '{context}'"

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
    time = (datetime.now() - starttime).total_seconds()
    print(f"\n{answer_en}\n\nRetrieving answer with model {model} took {time} seconds\n\n")  
    
    if language=="DE": 
        starttime = datetime.now()
        answer = translate_to_de(answer_en)
        time = (datetime.now() - starttime).total_seconds()
        print(f"\n{answer}\n\nTranslation to German took {time} seconds\n\n")  
    if language=="EN":
        answer = answer_en    
     
    return answer