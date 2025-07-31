import http.client
import json

conn = http.client.HTTPConnection("localhost", 5000)

questionsEN = [
    "How to prepare spaghetti Bolognaise?",
    "What type of flour is recommended for making fresh pasta, and why is it preferred?",
    "Why is it important to let the pasta dough rest before rolling it out, and for how long should it rest?",
    "What is a soffrito?",
    "What role does milk or cream play in the Bolognese sauce, and when is it added?",
    "What type of flour is recommended for making fresh pasta, and why is it preferred?",
    "Why is it important to let the pasta dough rest before rolling it out, and for how long should it rest?",
    "What is a soffrito?",
    "What role does milk or cream play in the Bolognese sauce, and when is it added?",
    "What type of flour is recommended for making fresh pasta, and why is it preferred?",
    "Why is it important to let the pasta dough rest before rolling it out, and for how long should it rest?",
    "What is a soffrito?",
    "What role does milk or cream play in the Bolognese sauce, and when is it added?",
    "What type of flour is recommended for making fresh pasta, and why is it preferred?",
    "Why is it important to let the pasta dough rest before rolling it out, and for how long should it rest?",
    "What is a soffrito?",
    "What role does milk or cream play in the Bolognese sauce, and when is it added?",
    "What type of flour is recommended for making fresh pasta, and why is it preferred?",
    "Why is it important to let the pasta dough rest before rolling it out, and for how long should it rest?",
    "What is a soffrito?",
    "What role does milk or cream play in the Bolognese sauce, and when is it added?"
]

questionsDE = [
    "Wie bereitet man Spaghetti Bolognaise zu?",
    "Welches Mehl ist empfohlen, um frische Pasta zuzubereiten, und wieso wird es bevorzugt?",
    "Wieso ist es wichtig, den Nudelteig vor dem Ausrollen rasten zu lassen, und wie lange sollte er rasten?",
    "Was ist ein Soffrito?",
    "Welche Rolle spielen Milch oder Obers in der Bolognaise-Sauce und wann wird es hinzugefügt?",
    "Welches Mehl ist empfohlen, um frische Pasta zuzubereiten, und wieso wird es bevorzugt?",
    "Wieso ist es wichtig, den Nudelteig vor dem Ausrollen rasten zu lassen, und wie lange sollte er rasten?",
    "Was ist ein Soffrito?",
    "Welche Rolle spielen Milch oder Obers in der Bolognaise-Sauce und wann wird es hinzugefügt?",
    "Welches Mehl ist empfohlen, um frische Pasta zuzubereiten, und wieso wird es bevorzugt?",
    "Wieso ist es wichtig, den Nudelteig vor dem Ausrollen rasten zu lassen, und wie lange sollte er rasten?",
    "Was ist ein Soffrito?",
    "Welche Rolle spielen Milch oder Obers in der Bolognaise-Sauce und wann wird es hinzugefügt?",
    "Welches Mehl ist empfohlen, um frische Pasta zuzubereiten, und wieso wird es bevorzugt?",
    "Wieso ist es wichtig, den Nudelteig vor dem Ausrollen rasten zu lassen, und wie lange sollte er rasten?",
    "Was ist ein Soffrito?",
    "Welche Rolle spielen Milch oder Obers in der Bolognaise-Sauce und wann wird es hinzugefügt?",
    "Welches Mehl ist empfohlen, um frische Pasta zuzubereiten, und wieso wird es bevorzugt?",
    "Wieso ist es wichtig, den Nudelteig vor dem Ausrollen rasten zu lassen, und wie lange sollte er rasten?",
    "Was ist ein Soffrito?",
    "Welche Rolle spielen Milch oder Obers in der Bolognaise-Sauce und wann wird es hinzugefügt?",
]

headers = {
    'Cache-Control': 'no-cache',
    'User-Agent': 'PostmanRuntime/7.44.1',
    'Content-Type': 'application/json'
}

for question in questionsEN:
    payload = json.dumps({
        "question": question,
        "topic": "Bolognaise",
        "language": "EN"
    })
    conn.request("POST", "/answer", payload, headers)
    res = conn.getresponse()
    res.read()

for question in questionsDE:
    payload = json.dumps({
        "question": question,
        "topic": "Bolognaise",
        "language": "DE"
    })
    conn.request("POST", "/answer", payload, headers)
    res = conn.getresponse()
    res.read()    