# This script utilizes 'flask', which is licensed under the Apache License 2.0.
import psutil
import threading
import time
from flask import Flask, request, jsonify
from context_provider import retrieve_context
from answer_generator import retrieve_answer
from datetime import datetime

class RamTracker:
    def __init__(self):
        self.samples_total = []
        self.samples_python = []
        self.samples_ollama = []
        self._running = False
        self._thread = None

    def start(self):
        if self._running:
            return

        self.samples_total.clear()
        self.samples_python.clear()
        self.samples_ollama.clear()

        self._running = True
        self._thread = threading.Thread(target=self._track)
        self._thread.start()

    def _track(self):
        while self._running:
            try:
                python_mb = self._get_process_memory("python")
                ollama_mb = self._get_process_memory("ollama")

                total_mb = python_mb + ollama_mb

                self.samples_python.append(python_mb)
                self.samples_ollama.append(ollama_mb)
                self.samples_total.append(total_mb)

            except:
                pass

            time.sleep(0.5)

    def stop(self):
        self._running = False
        if self._thread:
            self._thread.join()

    def summarize(self, samples):
        if not samples:
            return (0, 0, 0)

        return (
            int(min(samples)),
            int(max(samples)),
            int(sum(samples) / len(samples))
        )

    def get_results(self):
        return {
            "TOTAL": self.summarize(self.samples_total),
            "PYTHON": self.summarize(self.samples_python),
            "OLLAMA": self.summarize(self.samples_ollama),
        }

    def _get_process_memory(self, name):
        total = 0
        for p in psutil.process_iter(['name', 'memory_info']):
            try:
                if name.lower() in (p.info['name'] or "").lower():
                    total += p.info['memory_info'].rss / (1024 * 1024)
            except:
                pass
        return total
    
def log_ram_usage(f, purpose, stats):
    min_mb, max_mb, avg_mb = stats
    f.write(f"\n{purpose}: MIN={min_mb} MB | MAX={max_mb} MB | AVG={avg_mb} MB")


def log_separator(f):
    f.write("\n\n================================================================\n")

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
        
        tracker = RamTracker()
        tracker.start()

        start_total = datetime.now()

        context, context_time = retrieve_context(question, topic, language)
        answer, answer_time = retrieve_answer(context, question, language, model)

        end_total = datetime.now()
  
        tracker.stop()
        ram_results = tracker.get_results()
        total_time = (end_total - start_total).total_seconds()
        with open(file, "a", encoding="utf-8") as f:
            f.write(f"MODEL:    {model}\n")
            f.write(f"LANGUAGE: {language}\n")

            f.write(f"\nQUESTION:\n    {question}\n")
            f.write(f"\nCONTEXT:\n    {context}\n")
            f.write(f"\nANSWER:\n    {answer}\n")

            f.write("\nTIMINGS:\n")
            f.write(f"CONTEXT = {context_time:.2f} s | ")
            f.write(f"ANSWER  = {answer_time:.2f} s | ")
            f.write(f"TOTAL   = {total_time:.2f} s\n\n")

            f.write("RAM USAGE:")
            # RAM logs (like C#)
            log_ram_usage(f, "TOTAL RAM", ram_results["TOTAL"])
            log_ram_usage(f, "PYTHON RAM", ram_results["PYTHON"])
            log_ram_usage(f, "OLLAMA RAM", ram_results["OLLAMA"])

            log_separator(f)
        return jsonify({"status": "success", "answer": answer}), 200
    except Exception as e:
        return jsonify({"status": "error", "message": str(e)}), 500
if __name__ == '__main__':
    app.run()