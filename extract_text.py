# extract_text.py
import os

def extract_text_from_txt(filepath):
    with open(filepath, 'r', encoding='utf-8', errors='ignore') as f:
        text = f.read()
    return text.strip()

def extract_text(filepath):
    ext = os.path.splitext(filepath)[1].lower()
    if ext == '.txt':
        return extract_text_from_txt(filepath)
    else:
        # Unsupported file type
        return ""
