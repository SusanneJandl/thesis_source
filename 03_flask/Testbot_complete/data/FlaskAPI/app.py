# This script utilizes 'flask', which is licensed under the Apache License 2.0.

from flask import Flask, request, jsonify
from chatbot_trigger import run_chatbot
from context_provider import retrieve_context
from context_provider import retrieve_context_qa
from translation import translate_to_de
from translation import translate_to_en
from answer_generator import retrieve_answer
from datetime import datetime
import threading
import time
import statistics
import os
import psutil

file = "C:\\Users\\susan\\Documents\\bachelor-thesis_data\\tests\\laptop\\03_flask\\testresults.md" #laptop
# file = "C:\\Users\\Utente\\Documents\\repos\\bachelor-thesis_data\\tests\\PC\\03_flask\\testresults.md" #PC

class RamTracker:
    def __init__(self):
        self.log_file = file        
        self.samples_flask = []
        self.samples_ollama = []

        self._stop_event = threading.Event()
        self._thread: threading.Thread | None = None

    def start(self):
        if self._thread and self._thread.is_alive():
            return
        self._stop_event.clear()
        self._thread = threading.Thread(target=self._run_sampler, daemon=True)
        self._thread.start()

    def _run_sampler(self):
        flask_proc = psutil.Process(os.getpid())

        while not self._stop_event.is_set():
            # --- Flask process RAM ---
            try:
                rss_flask = flask_proc.memory_info().rss / (1024 * 1024)
                self.samples_flask.append(rss_flask)
            except Exception:
                pass

            # --- Ollama RAM ---
            try:
                rss_ollama = 0.0
                for p in psutil.process_iter(["name"]):
                    try:
                        if p.info["name"] and "ollama" in p.info["name"].lower():
                            rss_ollama += p.memory_info().rss
                    except psutil.Error:
                        pass

                self.samples_ollama.append(rss_ollama / (1024 * 1024))
            except Exception:
                pass

            time.sleep(1.0)

    def stop_and_log(self):
        # Stop thread
        self._stop_event.set()
        if self._thread:
            self._thread.join(timeout=2)
            self._thread = None

        # No samples?
        if not self.samples_flask and not self.samples_ollama:
            return

        def summarize(values):
            return (
                min(values),
                max(values),
                statistics.fmean(values),
            )

        with open(self.log_file, "a", encoding="utf-8") as f:
            if self.samples_flask:
                mn, mx, avg = summarize(self.samples_flask)
                f.write(
                    f"\n\nFLASK RAM USAGE: "
                    f"MIN={mn:.2f} MB | MAX={mx:.2f} MB | AVG={avg:.2f} MB"
                )

            if self.samples_ollama:
                mn, mx, avg = summarize(self.samples_ollama)
                f.write(
                    f"\n\nOLLAMA RAM USAGE: "
                    f"MIN={mn:.2f} MB | MAX={mx:.2f} MB | AVG={avg:.2f} MB"
                )
app = Flask(__name__)

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

    # ram_tracker = RamTracker()
    # ram_tracker.start()

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
                f.write(f"\nGERMAN ANSWER:\n    {answer_de}\n\n")

            f.write(f"TIMINGS:\n")
            if language=="DE":
                f.write(f"TO EN = {de_en_time:.2f} s | ")
            f.write(f"CONTEXT = {context_time:.2f} s | ANSWER EN = {answer_time:.2f} s ({model})")
            if language=="DE":
                f.write(f" | TO DE = {en_de_time:.2f} s")                                                                                              

        return jsonify({"status": "success", "answer": answer}), 200
    except Exception as e:
        return jsonify({"status": "error", "message": str(e)}), 500
    # finally:
        # --- RAM tracking: stop and log once per request ---
        # ram_tracker.stop_and_log()
    
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