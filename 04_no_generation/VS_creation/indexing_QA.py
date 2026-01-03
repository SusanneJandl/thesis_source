
import os
import json
import pickle
import numpy as np
from scipy.spatial import cKDTree
from sentence_transformers import SentenceTransformer

# Configuration
topic = "Fantasy_qa"
baseurl = "C:/Users/susan/Documents/bachelors_thesis_source/04_no_generation"
qa_file = f"{baseurl}/VS_Information/{topic}/fantasy_qa.json"  # JSON file containing Q&A pairs
model_name = f"{baseurl}/Testbot_complete/data/AI_Models/paraphrase-multilingual-MiniLM-L12-v2"
model = SentenceTransformer(model_name)
# Load Q&A pairs
with open(qa_file, "r", encoding="utf-8") as f:
    qa_data = json.load(f)

if not qa_data:
    print("No Q&A data found. Check your JSON file.")
    exit()

# Prepare Q&A embeddings
all_qa_sets = []
qa_id = 0

for item in qa_data:
    question = item["question"]
    answer_en = item["answer_en"]
    answer_de = item["answer_de"]
    all_qa_sets.append((qa_id, question, answer_en, answer_de))
    qa_id += 1

# Embed questions
questions = [item[1] for item in all_qa_sets]
question_embeddings = model.encode(questions, batch_size=32, convert_to_numpy=True)

# Build cKDTree for questions
tree = cKDTree(question_embeddings)

# Create directory for saving if needed
os.makedirs(f"{baseurl}/VectorStores/{topic}", exist_ok=True)

# Save the cKDTree index
with open(f"{baseurl}/VectorStores/{topic}/{topic}_tree.pkl", "wb") as f:
    pickle.dump(tree, f)

# Save metadata mapping Q&A pairs
metadata = {i: {"question": q, "answer_en": a_en, "answer_de": a_de} for i, q, a_en, a_de in all_qa_sets}

with open(f"{baseurl}/VectorStores/{topic}/{topic}_metadata.json", "w", encoding="utf-8") as f:
    json.dump(metadata, f, ensure_ascii=False, indent=4)

print("Q&A index (cKDTree) and metadata created successfully!")
