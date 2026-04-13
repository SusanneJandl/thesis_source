import tiktoken

def count_tokens(text: str) -> int:
    enc = tiktoken.get_encoding("cl100k_base")
    token = len(enc.encode(text))
    return f" ({token} T)"