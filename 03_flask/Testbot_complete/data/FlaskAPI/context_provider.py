# context_provider.py
# This script utilizes the 'all-mpnet-base-v2' model from Sentence Transformers.
# The model is licensed under the Apache License 2.0.

# This script now utilizes scipy.spatial.cKDTree for retrieval
# to avoid installing extra C++ build tools.

import json
import pickle
from scipy.spatial import cKDTree
from sentence_transformers import SentenceTransformer
from translation import translate_to_en
from datetime import datetime

model_name = '../AI_Models/paraphrase-multilingual-MiniLM-L12-v2'
model = SentenceTransformer(model_name)
embedding_dim = model.get_sentence_embedding_dimension()

def retrieve_context(question: str, topic: str, language: str) -> list:
    """
    Gets context from the cKDTree-based vector store based on the question.
    Returns a list of texts.
    """
    
    starttime = datetime.now()
    # Load the cKDTree
    with open(f"../VectorStores/{topic}/{topic}_tree.pkl", "rb") as f:
        tree: cKDTree = pickle.load(f)

    # Load metadata
    with open(f"../VectorStores/{topic}/metadata.json", "r", encoding="utf-8") as f:
        metadata = json.load(f)

    # Get the embedding for the query
    query_embedding = model.encode([question], convert_to_numpy=True)  # shape: (1, embedding_dim)

    # Number of results to retrieve
    k = 3

    # Query cKDTree
    # 'query' returns (distances, indices)
    distances, indices = tree.query(query_embedding, k=k)

    # Note: if k=1, you'll get a single integer index; if k>1, you'll get an array
    # cKDTree returns shape (k,) or (1,k). We'll handle both cases by standardizing arrays.
    if k == 1:
        # single item
        indices = [indices]   # make it a list
        distances = [distances]

    # cKDTree returns arrays of shape (1, k) if the query has shape (1, embedding_dim)
    # we only care about the first row (since there's only one query).
    indices = indices[0] if len(indices.shape) > 1 else indices
    distances = distances[0] if len(distances.shape) > 1 else distances

    # Gather the texts from metadata
    contexts = []
    for idx, dist in zip(indices, distances):
        doc_info = metadata[str(idx)]
        contexts.append(doc_info['text'])
    context_time = (datetime.now()-starttime).total_seconds()
    return contexts, context_time

def retrieve_context_qa(question: str, topic: str, language: str) -> list:
    """
    Gets context from the cKDTree-based vector store based on the question.
    Returns a list of texts.
    """
    similarity_threshold = 5 # lower for higher similarity
    
    # Load the cKDTree
    with open(f"../VectorStores/{topic}/{topic}_tree.pkl", "rb") as f:
        qa_tree = pickle.load(f)

    # Load metadata
    with open(f"../VectorStores/{topic}/{topic}_metadata.json", "r", encoding="utf-8") as f:
        qa_metadata = json.load(f)

    # Get the embedding for the query
    query_embedding = model.encode([question], convert_to_numpy=True)  # shape: (1, embedding_dim)

    distance, closest_idx = qa_tree.query(query_embedding, k=1)

    if distance < similarity_threshold:  # Good match found
        result = qa_metadata.get(str(closest_idx[0]), {"question": "No match found", "answer": "No relevant answer found."})
        print(f"\nBest Match: {result['question']}")
        print(f"Answer: {result['answer']}")
        return result["answer"]
    
    print("\nNo strong match found. Try rephrasing.")
    return "No relevant answer found."