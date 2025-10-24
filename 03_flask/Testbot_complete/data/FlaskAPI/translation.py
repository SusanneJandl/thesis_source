from transformers import AutoTokenizer, AutoModelForSeq2SeqLM

def translate_to_de(english_text: str) -> str:
    model_name = "../AI_Models/opus-mt-en-de"
    tokenizer = AutoTokenizer.from_pretrained(model_name)
    model = AutoModelForSeq2SeqLM.from_pretrained(model_name)
    inputs = tokenizer(english_text, return_tensors="pt")

    translated_tokens = model.generate(**inputs)

    german_text = tokenizer.decode(translated_tokens[0], skip_special_tokens=True)
    return german_text

def translate_to_en(german_text: str) -> str:
    model_name = "../AI_Models/opus-mt-de-en"
    tokenizer = AutoTokenizer.from_pretrained(model_name)
    model = AutoModelForSeq2SeqLM.from_pretrained(model_name)
    inputs = tokenizer(german_text, return_tensors="pt")

    translated_tokens = model.generate(**inputs)

    english_text = tokenizer.decode(translated_tokens[0], skip_special_tokens=True)
    return english_text
