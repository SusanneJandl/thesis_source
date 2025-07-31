# Models
## Test models

- downloaded with ollama
  - llama3.2:3b-instruct-q4_0 (1,9 GB)
  - llama3.2:3b-instruct-q8_0 (3,4 GB)
  - llama3.2:3b-instruct-q5_0 (2,3 GB)


## Model test procedure

- ask 4 questions in English, 5 times each (use in chatbot API)
  - measure response time
  - evaluate correctness of spelling/grammar (wrong/partly correct/incorrect)
  - evaluate correctness of content (wrong/partly correct/incorrect)

- ask 4 questions in German, 5 times each (use in chatbot API)
  - measure response time
  - evaluate correctness of spelling/grammar (wrong/partly correct/incorrect)
  - evaluate correctness of content (wrong/partly correct/incorrect)

- repeat on different devices (Macbook, Thinkpad)

# Chatbot

## Chatbot test procedure

- use chatbot with original model on different devices
  - ATMS machine
  - workstation
  - Thinkpad
  - Macbook

## Plan

- [x] adapt python output to display full answer, question and time
- [ ] test on ATMS machine, store results
- [ ] test on workstation, store results
- [ ] test on Thinkpad, store results
- [ ] test on Macbook, store results

## Prerequisites

- install ollama
- install python
- create chat_env in data
- install requirements in chat_env

## Machines

- ATMS machine
- workstation
- Thinkpad
```
Gerätename	ThinkPadSue
Prozessor	11th Gen Intel(R) Core(TM) i7-1185G7 @ 3.00GHz   3.00 GHz
Installierter RAM	32,0 GB (31,7 GB verwendbar)
Geräte-ID	652330E3-165B-4E6E-9BEA-62D6A7245D11
Produkt-ID	00355-60703-12487-AAOEM
Systemtyp	64-Bit-Betriebssystem, x64-basierter Prozessor
Stift- und Toucheingabe	Für diese Anzeige ist keine Stift- oder Toucheingabe verfügbar.

Edition	Windows 11 Pro
Version	24H2
Installiert am	‎25.‎02.‎2025
Betriebssystembuild	26100.4349
Seriennummer	PC2CZB22

Leistung	Windows Feature Experience Pack 1000.26100.107.0
Hersteller	Lenovo
```
- Macbook
- Werner's dell

## Models for text generation

- llama2.3:1b
- smollm2:1.7b

## languages

- English
- German

## Questions (3 times each)

- 1E - What type of flour is recommended for making fresh pasta, and why is it preferred?
- 1D - Welches Mehl ist empfohlen, um frische Pasta zuzubereiten, und wieso wird es bevorzugt?

- 2E - Why is it important to let the pasta dough rest before rolling it out, and for how long should it rest?
- 2D - Wieso ist es wichtig, den Nudelteig vor dem Ausrollen rasten zu lassen, und wie lange sollte er rasten?

- 3E - What is a soffrito?
- 3D - Was ist ein Soffrito?

- 4E - What role does milk or cream play in the Bolognese sauce, and when is it added?
- 4D - Welche Rolle spielen Milch oder Obers in der Bolognaise-Sauce und wann wird es hinzugefügt?