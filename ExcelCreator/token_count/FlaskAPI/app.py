# This script utilizes 'flask', which is licensed under the Apache License 2.0.

from flask import Flask, request, jsonify
from token_counter import count_tokens

app = Flask(__name__)

@app.route('/')
def home():
    return "Chatbot API is running"

@app.route('/token', methods=['POST'])
def get_answer():
    """
    Endpoint to count token
    """
    try:
        data = request.json
        answer = data.get('answer')
        
        if not answer:
            return jsonify({"status": "error", "message": "Answer is required"}), 400

        token = count_tokens(answer)
        
        return jsonify({"status": "success", "token": token}), 200
    except Exception as e:
        return jsonify({"status": "error", "message": str(e)}), 500
if __name__ == '__main__':
    app.run()