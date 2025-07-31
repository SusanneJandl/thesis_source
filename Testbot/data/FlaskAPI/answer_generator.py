from ollama import generate
from translation import translate_to_de
from datetime import datetime

def retrieve_answer(context: str, question: str, language: str) -> str:
    
    starttime = datetime.now()
    prompt = f"You are a chef who is keen on sharing his knowledge about cooking perfect pasta with bolognaise sauce. Do not use external or general knowledge or guessing.**User question:** {question}. **Information to answer the question:** {context}. Provide a clear and concise answer based strictly on the provided information."
    
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