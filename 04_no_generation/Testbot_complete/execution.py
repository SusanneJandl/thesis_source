import http.client
import json

conn = http.client.HTTPConnection("localhost", 5000)

questionsEN = [
    "What unusual feature defines the geography of Velbrek?",
    "What item did Sphodebarb carry instead of a wizard's staff, and what was its unique trait?",
    "Who or what are the members of the 'Order of the Thirteen-Shelled Hypothesis'?",
    "How did The Meow gain endless life?",
    "What did Sphodebarb trade to become semi-immortal?",
    "What substance represented a version of death without finality?",
    "How was the conflict between Sphodebarb and The Meow resolved?",
    "Who was Yalp, and why was he important?",
    "What symbolic event marked the final restoration of balance in Velbrek?",
    "What did Sphodebarb and Nink (the cat) choose after the loop was broken?",
    "What unusual feature defines the geography of Velbrek?",
    "What item did Sphodebarb carry instead of a wizard's staff, and what was its unique trait?",
    "Who or what are the members of the 'Order of the Thirteen-Shelled Hypothesis'?",
    "How did The Meow gain endless life?",
    "What did Sphodebarb trade to become semi-immortal?",
    "What substance represented a version of death without finality?",
    "How was the conflict between Sphodebarb and The Meow resolved?",
    "Who was Yalp, and why was he important?",
    "What symbolic event marked the final restoration of balance in Velbrek?",
    "What did Sphodebarb and Nink (the cat) choose after the loop was broken?"
]

questionsDE = [
    "Welches ungewöhnliche Merkmal prägt die Geographie von Velbrek?",
    "Welchen Gegenstand trug Sphodebarb anstelle eines Zauberstabs, und was war seine besondere Eigenschaft?",
    "Wer oder was sind die Mitglieder des 'Ordens der Dreizehn-Schaligen Hypothese'?",
    "Wie erlangte The Meow endloses Leben?",
    "Was tauschte Sphodebarb, um halb-unsterblich zu werden?",
    "Welche Substanz stellte eine Form des Todes ohne Endgültigkeit dar?",
    "Wie wurde der Konflikt zwischen Sphodebarb und The Meow gelöst?",
    "Wer war Yalp, und warum war er wichtig?",
    "Welches symbolische Ereignis markierte die endgültige Wiederherstellung des Gleichgewichts in Velbrek?",
    "Was wählten Sphodebarb und Nink (die Katze), nachdem die Schleife durchbrochen war?",
    "Welches ungewöhnliche Merkmal prägt die Geographie von Velbrek?",
    "Welchen Gegenstand trug Sphodebarb anstelle eines Zauberstabs, und was war seine besondere Eigenschaft?",
    "Wer oder was sind die Mitglieder des 'Ordens der Dreizehn-Schaligen Hypothese'?",
    "Wie erlangte The Meow endloses Leben?",
    "Was tauschte Sphodebarb, um halb-unsterblich zu werden?",
    "Welche Substanz stellte eine Form des Todes ohne Endgültigkeit dar?",
    "Wie wurde der Konflikt zwischen Sphodebarb und The Meow gelöst?",
    "Wer war Yalp, und warum war er wichtig?",
    "Welches symbolische Ereignis markierte die endgültige Wiederherstellung des Gleichgewichts in Velbrek?",
    "Was wählten Sphodebarb und Nink (die Katze), nachdem die Schleife durchbrochen war?"
]

headers = {
    'Cache-Control': 'no-cache',
    'User-Agent': 'PostmanRuntime/7.44.1',
    'Content-Type': 'application/json'
}

for question in questionsEN:
    payload = json.dumps({
        "question": question,
        "topic": "Fantasy",
        "language": "EN"
    })
    conn.request("POST", "/answer", payload, headers)
    res = conn.getresponse()
    res.read()

for question in questionsDE:
    payload = json.dumps({
        "question": question,
        "topic": "Fantasy",
        "language": "DE"
    })
    conn.request("POST", "/answer", payload, headers)
    res = conn.getresponse()
    res.read()    