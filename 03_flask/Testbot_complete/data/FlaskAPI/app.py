# This script utilizes 'flask', which is licensed under the Apache License 2.0.

from flask import Flask, request, jsonify
from chatbot_trigger import run_chatbot
from context_provider import retrieve_context
from context_provider import retrieve_context_qa
from translation import translate_to_de
from translation import translate_to_en
from answer_generator import retrieve_answer
from datetime import datetime

app = Flask(__name__)
file = "C:\\Users\\susan\\Documents\\bachelor-thesis_data\\tests\\laptop\\03_flask\\testresults.md" #laptop
#file = "C:\\Users\\susan\\Documents\\bachelor-thesis_data\\tests\\PC\\03_flask\\testresults.txt" #PC

@app.route('/')
def home():
    return "Chatbot API is running"

@app.route('/startbot', methods=['POST'])
def start_bot():
    """
    Endpoint to trigger the chatbot exe
    """
    try:
        data = request.json
        topic = data.get('topic')

        if not topic:
            return jsonify({"status": "error", "message": "Topic is required"}), 400
        
        run_chatbot(topic)
        return jsonify({"status": "success", "message": "Chatbot started successfully"})
    except Exception as e:
        return jsonify({"status": "error", "message": str(e)}), 500
    
@app.route('/en-de', methods=['POST'])
def get_english_text():
    """
    Endpoint to retrieve the context
    """
    try:
        data = request.json
        english_text = data.get('english_text')
        

        if not english_text:
            return jsonify({"status": "error", "message": "English text is required"}), 400

        german_text = translate_to_de(english_text)
        return jsonify({"status": "success", "context": german_text}), 200
    except Exception as e:
        return jsonify({"status": "error", "message": str(e)}), 500
    
@app.route('/de-en', methods=['POST'])
def get_german_text():
    """
    Endpoint to retrieve the context
    """
    try:
        data = request.json
        german_text = data.get('german_text')
        

        if not german_text:
            return jsonify({"status": "error", "message": "German text is required"}), 400

        english_text = translate_to_en(german_text)
        return jsonify({"status": "success", "context": english_text}), 200
    except Exception as e:
        return jsonify({"status": "error", "message": str(e)}), 500
    
    
@app.route('/content', methods=['POST'])
def get_context():
    """
    Endpoint to retrieve the context
    """
    try:
        data = request.json
        question = data.get('question')
        topic = data.get('topic')
        language = data.get('language')

        if not question:
            return jsonify({"status": "error", "message": "Question is required"}), 400

        if not topic:
            return jsonify({"status": "error", "message": "Topic is required"}), 400
        
        if not language:
            return jsonify({"status": "error", "message": "Language EN or DE is required"}), 400
        

        context = retrieve_context(question, topic, language)
        return jsonify({"status": "success", "context": context}), 200
    except Exception as e:
        return jsonify({"status": "error", "message": str(e)}), 500

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
                    f"\nENGLISH ANSWER:\n    {answer_en}\n")
            if language=="DE":
                f.write(f"\nGERMAN ANSWER:\n    {answer_de}\n")

            if language=="DE":
                f.write(f"TO EN | ")
            f.write(f"CONTEXT | ANSWER EN")
            if language=="DE":
                f.write(f" | TO DE")
            f.write("\n-|-")
            if language=="DE":
                f.write(f"|-|-")
            f.write("\n")
            if language=="DE":
                f.write(f"{de_en_time:.2f} s | ")
            f.write(f"{context_time:.2f} s | {answer_time:.2f} s ({model})")
            if language=="DE":
                f.write(f" | {en_de_time:.2f} s")
            f.close()
            

        return jsonify({"status": "success", "answer": answer}), 200
    except Exception as e:
        return jsonify({"status": "error", "message": str(e)}), 500
    
@app.route('/answer_qa', methods=['POST'])
def get_answer_qa():
    """
    Endpoint to retrieve the context
    """
    try:
        data = request.json
        question = data.get('question')
        topic = data.get('topic')
        language = data.get('language')

        if not question:
            return jsonify({"status": "error", "message": "Question is required"}), 400

        if not topic:
            return jsonify({"status": "error", "message": "Topic is required"}), 400
        
        if not language:
            return jsonify({"status": "error", "message": "Language EN or DE is required"}), 400
        

        answer = retrieve_context_qa(question, topic, language)
        
        return jsonify({"status": "success", "answer": answer}), 200
    except Exception as e:
        return jsonify({"status": "error", "message": str(e)}), 500

if __name__ == '__main__':
    app.run()