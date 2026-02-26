# This script utilizes 'flask', which is licensed under the Apache License 2.0.

from flask import Flask, request, jsonify
from context_provider import retrieve_context
from answer_generator import retrieve_answer
from datetime import datetime

app = Flask(__name__)

file = "C:\\Users\\susan\\Documents\\bachelor-thesis_data\\tests\\laptop\\05_quantization\\testresults.md" #laptop
# file = "C:\\Users\\Utente\\Documents\\repos\\bachelor-thesis_data\\tests\\PC\\05_quantization\\testresults.md" #PC

@app.route('/')
def home():
    return "Chatbot API is running"

@app.route('/answer', methods=['POST'])
def get_answer():
    """
    Endpoint to retrieve the answer
    """
    try:
        data = request.json
        question = data.get('question')
        topic = data.get('topic')
        language = data.get('language')
        model = data.get('model')
        
        if not question:
            return jsonify({"status": "error", "message": "Question is required"}), 400

        if not topic:
            return jsonify({"status": "error", "message": "Topic is required"}), 400
        
        if not language:
            return jsonify({"status": "error", "message": "Language EN or DE is required"}), 400
        
        if not model:
            return jsonify({"status": "error", "message": "Modelname is required"}), 400
        
        context, context_time = retrieve_context(question, topic, language)
        answer, answer_time = retrieve_answer(context, question, language, model)
        with open(file, "a", encoding="utf-8") as f:
            f.write(f"MODEL:    {model}\n"
                    f"LANGUAGE: {language}\n"
                    f"\n QUESTION:\n    {question}\n"
                    f"\nCONTEXT:\n    {context}\n"
                    f"\nANSWER:\n    {answer}\n"
                    f"TIMINGS:\n"
                    f"CONTEXT = {context_time:.2f} s | ANSWER = {answer_time:.2f} s ({model})")
            
        return jsonify({"status": "success", "answer": answer}), 200
    except Exception as e:
        return jsonify({"status": "error", "message": str(e)}), 500
if __name__ == '__main__':
    app.run()