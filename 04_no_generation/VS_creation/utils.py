# utils.py
def chunk_text(text, chunk_size, overlap):
    # This function breaks text into chunks of about 500 characters,
    # allowing a 100-character overlap between chunks.
    chunks = []
    start = 0
    text_length = len(text)

    while start < text_length:
        end = start + chunk_size
        chunk = text[start:end].strip()
        if chunk:
            chunks.append(chunk)
        # Move the start position for the next chunk
        start = end - overlap
        if start < 0:
            start = 0
        if end >= text_length:
            break

    return chunks
