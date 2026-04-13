import http.client
import json

conn = http.client.HTTPConnection("127.0.0.1", 5000)

questionsEN = [
    "What unusual feature defines the geography of Velbrek?",
    "What item did Sphodebarb carry instead of a wizard's staff, and what was its unique trait?",
    "Who or what are the members of the \"Order of the Thirteen-Shelled Hypothesis\"?",
    "How did The Meow gain endless life?",
    "What substance represented a version of death without finality?",
    "How was the conflict between Sphodebarb and The Meow resolved?",
    "Who was Yalp, and why was he important?",
    "What did Sphodebarb and Nink (the cat) choose after the loop was broken?",
    "What unusual feature defines the geography of Velbrek?",
    "What item did Sphodebarb carry instead of a wizard's staff, and what was its unique trait?",
    "Who or what are the members of the \"Order of the Thirteen-Shelled Hypothesis\"?",
    "How did The Meow gain endless life?",
    "What substance represented a version of death without finality?",
    "How was the conflict between Sphodebarb and The Meow resolved?",
    "Who was Yalp, and why was he important?",
    "What did Sphodebarb and Nink (the cat) choose after the loop was broken?"
]

questionsDE = [
    "Welche ungewöhnliche Eigenschaft definiert die Geographie von Velbrek?",
    "Welchen Gegenstand trug Sphodebarb anstelle eines Zauberstabs, und was war seine besondere Eigenschaft?",
    "Wer oder was sind die Mitglieder des \"Ordens der Dreizehn-Schaligen Hypothese\"?",
    "Wie erlangte The Meow endloses Leben?",
    "Welche Substanz stellte eine Form des Todes ohne Endgültigkeit dar?",
    "Wie wurde der Konflikt zwischen Sphodebarb und The Meow gelöst?",
    "Wer war Yalp, und warum war er wichtig?",
    "Wofür entschieden sich Sphodebarb und Nink (die Katze), nachdem die Schleife durchbrochen war?",
    "Welche ungewöhnliche Eigenschaft definiert die Geografie von Velbrek?",
    "Welchen Gegenstand trug Sphodebarb anstelle eines Zauberstabs, und was war seine besondere Eigenschaft?",
    "Wer oder was sind die Mitglieder des \"Ordens der Dreizehn-Schaligen Hypothese\"?",
    "Wie erlangte The Meow endloses Leben?",
    "Welche Substanz stellte eine Form des Todes ohne Endgültigkeit dar?",
    "Wie wurde der Konflikt zwischen Sphodebarb und The Meow gelöst?",
    "Wer war Yalp, und warum war er wichtig?",
    "Wofür entschieden sich Sphodebarb und Nink (die Katze), nachdem die Schleife durchbrochen war?"
]

model = "llama3.1:8b-instruct-q4_0"

headers = {
    'Cache-Control': 'no-cache',
    'User-Agent': 'PostmanRuntime/7.44.1',
    'Content-Type': 'application/json'
}

for question in questionsEN:
    payload = json.dumps({
        "question": question,
        "topic": "Fantasy",
        "language": "EN",
        "model": model
    })
    conn.request("POST", "/answer", payload, headers)
    res = conn.getresponse()
    res.read()
    if res.status != 200:
        print(f"Question: {question} | error: {res.status}")
    else:    
        print(f"Question: {question} | done")

for question in questionsDE:
    payload = json.dumps({
        "question": question,
        "topic": "Fantasy",
        "language": "DE",
        "model": model
    })
    conn.request("POST", "/answer", payload, headers)
    res = conn.getresponse()
    res.read()
    if res.status != 200:
        print(f"Question: {question} | error: {res.status}")
    else:    
        print(f"Question: {question} | done")
