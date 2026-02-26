import json
import pickle
from scipy.spatial import cKDTree
from sentence_transformers import SentenceTransformer
from translation import translate_to_en
from datetime import datetime

model_name = '../AI_Models/paraphrase-multilingual-MiniLM-L12-v2'
model = SentenceTransformer(model_name)
embedding_dim = model.get_sentence_embedding_dimension()

def retrieve_context_qa(question: str, topic: str, accuracy: int, language: str) -> list:
    """
    Gets context from the cKDTree-based vector store based on the question.
    Returns a list of texts.
    """
    starttime = datetime.now()
    similarity_threshold = accuracy # lower for higher similarity
    baseurl = "C:/Users/susan/Documents/bachelors_thesis_source/04_no_generation"

    # Load the cKDTree
    with open(f"{baseurl}/Testbot_complete/data/VectorStores/{topic}/{topic}_tree.pkl", "rb") as f:
        qa_tree = pickle.load(f)

    # Load metadata
    with open(f"{baseurl}/Testbot_complete/data/VectorStores/{topic}/{topic}_metadata.json", "r", encoding="utf-8") as f:
        qa_metadata = json.load(f)

    # Get the embedding for the query
    query_embedding = model.encode([question], convert_to_numpy=True)  # shape: (1, embedding_dim)

    distance, closest_idx = qa_tree.query(query_embedding, k=1)

    answer = ""
    best_match = ""

    if distance < similarity_threshold:  # Good match found
        result = qa_metadata.get(str(closest_idx[0]), {"question": "No match found", "answer_en": "No relevant answer found.", "answer_de": "Keine relevante Antwort gefunden."})
        best_match = result['question']
        if language == "EN":
            answer = result['answer_en']
        if language == "DE":
            answer = result['answer_de']
    else:
        answer = "No relevant answer found."
    answer_time = (datetime.now()-starttime).total_seconds()
    return answer, best_match, answer_time