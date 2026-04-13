# This script utilizes 'flask', which is licensed under the Apache License 2.0.

from flask import Flask, request, jsonify
from context_provider import retrieve_context
from translation import translate_to_en
from answer_generator import retrieve_answer
from datetime import datetime
from token_counter import count_tokens

app = Flask(__name__)

file = "C:\\Users\\susan\\Documents\\bachelor-thesis_data\\tests\\laptop_auto\\03_flask\\testresults.md" #laptop
# file = "C:\\Users\\Utente\\Documents\\repos\\bachelor-thesis_data\\tests\\PC\\03_flask\\testresults.md" #PC

@app.route('/')
def home():
    return "Chatbot API is running"

@app.route('/answer', methods=['POST'])
def get_answer():
    """
    Endpoint to retrieve the answer
    """
    de_en_time = 0

    try:
        data = request.json
        question = data.get('question')
        topic = data.get('topic')
        language = data.get('language')
        history = data.get('history')

        if not question:
            return jsonify({"status": "error", "message": "Question is required"}), 400

        if not topic:
            return jsonify({"status": "error", "message": "Topic is required"}), 400
        
        if not language:
            return jsonify({"status": "error", "message": "Language EN or DE is required"}), 400
        
        if language=="DE":
            starttime = datetime.now()
            question_de = question
            question = translate_to_en(question_de)
            de_en_time = (datetime.now() - starttime).total_seconds()
            
        context, context_time = retrieve_context(question, topic, language)
        answer, answer_de, answer_en, answer_time, en_de_time, model = retrieve_answer(context, question, language, history)
        with open(file, "a", encoding="utf-8") as f:
            if language=="DE":
                f.write(f"\nGERMAN QUESTION:\n    {question_de}\n")
            f.write(f"\nENGLISH QUESTION:\n    {question}\n"
                    f"\nCONTEXT:\n    {context}\n"
                    f"\nHISTORY: \n    {history}\n"
                    f"\nENGLISH ANSWER:\n    {answer_en + count_tokens(answer_en)}\n")
            if language=="DE":
                f.write(f"\nGERMAN ANSWER:\n    {answer_de + count_tokens(answer_de)}\n\n")

            f.write(f"TIMINGS:\n")
            if language=="DE":
                f.write(f"TO EN = {de_en_time:.2f} s | ")
            f.write(f"CONTEXT = {context_time:.2f} s | ANSWER EN = {answer_time:.2f} s ({model})")
            if language=="DE":
                f.write(f" | TO DE = {en_de_time:.2f} s")                                                                                              

        return jsonify({"status": "success", "answer": answer}), 200
    except Exception as e:
        return jsonify({"status": "error", "message": str(e)}), 500
if __name__ == '__main__':
    app.run()