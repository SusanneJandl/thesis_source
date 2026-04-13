# This script utilizes 'flask', which is licensed under the Apache License 2.0.

from flask import Flask, request, jsonify
from context_provider import retrieve_context_qa
from translation import translate_to_en
from datetime import datetime

file = "C:\\Users\\susan\\Documents\\bachelor-thesis_data\\tests\\laptop_auto\\04_no_generation\\testresults.md" #laptop
# file = "C:\\Users\\Utente\\Documents\\repos\\bachelor-thesis_data\\tests\\PC\\04_no_generation\\testresults.md" #PC

app = Flask(__name__)

@app.route('/')
def home():
    return "Chatbot API is running"
    
@app.route('/answer_qa', methods=['POST'])
def get_answer_qa():
    """
    Endpoint to retrieve the context
    """
    try:
        data = request.json
        question = data.get('question')
        topic = data.get('topic')
        accuracy = data.get('accuracy')
        language = data.get('language')

        if not question:
            return jsonify({"status": "error", "message": "Question is required"}), 400

        if not topic:
            return jsonify({"status": "error", "message": "Topic is required"}), 400
        
        if not accuracy:
            return jsonify({"status": "error", "message": "Accuracy is required"}), 400
        
        if not language:
            return jsonify({"status": "error", "message": "Language EN or DE is required"}), 400
        
        if language=="DE":
            starttime = datetime.now()
            question_de = question
            question = translate_to_en(question_de)
            de_en_time = (datetime.now() - starttime).total_seconds()

        answer, best_match, answer_time = retrieve_context_qa(question, topic, accuracy, language)

        with open(file, "a", encoding="utf-8") as f:
            if language=="DE":
                f.write(f"\nGERMAN QUESTION:\n    {question_de}\n")
            f.write(f"\nENGLISH QUESTION:\n    {question}\n"
                    f"\nBEST MATCH:\n    {best_match}\n"
                    f"\n ANSWER:\n    {answer}\n")
            f.write(f"TIMINGS:\n")
            if language=="DE":
                f.write(f"TO EN = {de_en_time:.2f} s | ")
            f.write(f"ANSWER = {answer_time:.2f} s"
                    f"\n================================================================\n")
        
        return jsonify({"status": "success", "answer": answer}), 200
    except Exception as e:
        return jsonify({"status": "error", "message": str(e)}), 500

if __name__ == '__main__':
    app.run()