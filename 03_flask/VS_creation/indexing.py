# indexing.py
# This script utilizes the 'all-mpnet-base-v2' model from Sentence Transformers.
# The model is licensed under the Apache License 2.0.

# This script now utilizes scipy.spatial.cKDTree (instead of hnswlib or annoy)
# to avoid installing extra C++ build tools.

import os
import json
import pickle
from scipy.spatial import cKDTree
from sentence_transformers import SentenceTransformer
from extract_text import extract_text
from utils import chunk_text

topic = "Fantasy"

# Configuration
folder_path = f"C:/Users/susan/Documents/bachelors_thesis_source/VS_Information/{topic}/"
model_name = "C:/Users/susan/Documents/bachelors_thesis_source/03_flask/Testbot_complete/data/AI_Models/paraphrase-multilingual-MiniLM-L12-v2"
model = SentenceTransformer(model_name)
embedding_dim = model.get_sentence_embedding_dimension()

# Read and process documents
all_text_chunks = []
doc_id = 0

# Iterate through all files in the folder
for filename in os.listdir(folder_path):
    filepath = os.path.join(folder_path, filename)
    if os.path.isfile(filepath):
        text = extract_text(filepath)
        if not text.strip():
            continue  # Skip empty text

        # Chunk the text
        chunks = chunk_text(text, chunk_size=700, overlap=200)
        for chunk in chunks:
            all_text_chunks.append((doc_id, filename, chunk))
            doc_id += 1

# If no chunks found, no index will be built
if not all_text_chunks:
    print("No text chunks found. Check your documents and extraction logic.")
    exit()

# Embed all chunks
texts = [item[2] for item in all_text_chunks]
embeddings = model.encode(texts, batch_size=32, convert_to_numpy=True)

# Build cKDTree index
# cKDTree expects a 2D array: shape (num_samples, embedding_dim)
tree = cKDTree(embeddings)

# Create directory for saving if needed
os.makedirs(f"C:/Users/susan/Documents/bachelors_thesis_source/03_flask/Testbot_complete/data/VectorStores/{topic}", exist_ok=True)

# Save the cKDTree using pickle
with open(f"C:/Users/susan/Documents/bachelors_thesis_source/03_flask/Testbot_complete/data/VectorStores/{topic}/{topic}_tree.pkl", "wb") as f:
    pickle.dump(tree, f)

# Save metadata
metadata = {}
for i, (doc_id, file_name, chunk_text_) in enumerate(all_text_chunks):
    metadata[i] = {
        "source_file": file_name,
        "text": chunk_text_
    }

with open(f"C:/Users/susan/Documents/bachelors_thesis_source/03_flask/Testbot_complete/data/VectorStores/{topic}/metadata.json", "w", encoding="utf-8") as f:
    json.dump(metadata, f, ensure_ascii=False, indent=4)

print("Index (cKDTree) and metadata created successfully!")
