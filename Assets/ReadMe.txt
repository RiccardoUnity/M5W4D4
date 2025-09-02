PENSIERI DELLO SVILUPPATORE (Note utili per capire cos'ho fatto)

La parte più complessa e sostanziosa di questo progetto è la FSM. All'inizio doveva essere leggermente più complessa di quella vista durante il corso, poi la cosa mi è sfuggita di mano. Mi ha preso a tal punto che mi ci sono voluti 12 giorni per finirla (contando la correzione di bug, qualche giorno in più).

Sostanzialmente mi sono ispirato ad un sistema decisionale simile a quello che succede in natura. Per decidere lo stato mi servono degli input dall'ambiente o dall'agente stesso, e per prendere questi mi servono dei sensi(input esterni) o delle funzioni interne apposta(input interni).
I sensi che ho implementato sono la vista e l'udito, entrambi riferiscono ad uno script "SenseBrain" che elabora gli input in base all'importanza (per esempio pesi dati da un'ordine su una lista o dallo scorrere del tempo). I risultati saranno o un Character o/e un punto Vector3. Queste informazioni possono essere prese dal controller della FSM per decidere quale stato usare per quel momento. Le transizioni di uno stato possono essere bypassate in qualunque momento da una transizione dell'anyState.

L'udito è un senso passivo, perciò deve registrare le informazioni che gli vengono passate, non cercarle attivamente dall'ambiente. Quando c'è un suono da sentire, lo script Noise chiama tutti i metodi Listen di EnemyHear in un certo raggio e gli passa delle informazioni (degno di nota è l'informazione dell'intensità).
La vista è un senso attivo (ha la priorità sull'udito) e l'ho strutturata in modo leggermente diverso dal solito. Non volevo usare una serie di Raycast (o Linecast) per cercare tutti i target possibili nel cono di visione (l'ho fatto solo più tardi per debuggare con i Gizoms), mi sono venuti in mente casi in cui avrei potuto perderne qualcuno o rischiare di calcolare lo stesso target 2 o più volte. Ho optato per filtrare i colliders di un OverlapSphere attraverso un Linecast per sapere se erano direttamente visibili e ho filtrato i restanti per il coseno.
L'estensione dei sensi cambia in base allo stato in cui ci si trova in quel momento. Se non hai un target definito allora non sei concentrato su qualcosa perciò hai un campo visivo più ampio, ma ridotto. Se stai inseguendo qualcuno allora non togli lo sguardo dal bersaglio, il campo visivo è più ridotto, ma più profondo. Anche qui mi sono ispirato alla natura (differenze tra i sensi delle prede e dei predatori).

Un altro aspetto fondamentale di questa FSM (o di questo gioco in sé) è che, a inizio partita, ogni Enemy non sa distinguere il Player o gli Enemy tra tutti i Character in scena, e, man mano che si incontrano, si passano informazioni su chi sono e se hanno incontrato un character ostile. Per dare profondità allo scambio di informazioni, ogni Enemy ha un id univoco che ogni Enemy può verificare, attraverso il Singleton dedicato, per sapere se è un id valido. L'unico a non avere un id è il Player. Quando un Enemy chatta con il Player compare un'interfaccia utente domanda-risposta. Le domande e le risposte non hanno un sistema di verifica e possono essere scritte tramite degli ScriptableObject. L'ultima domanda è quella fondamentale: viene chiesto al Player di fornire un id. Qui il Player sbaglierà (ogni 10 Enemy in scena sono 1% in più di dare un id giusto, e non c'è modo di procurarselo in game, ma solo tramite debug dell'inspector in editor). Sicuramente ci sono diverse meccaniche da rivedere per rendere l'esperienza più … sensata, ma era più per sperimentare.

Per limitare e rendere più controllato l'uso di certi metodi pubblici, anche in un'ottica futura di lavoro di gruppo, ho cercato di limitarne l'accesso inserendo degli if che richiedono parametri già noti, così che solo certi componenti possano accedere a tali funzioni. Probabilmente in futuro potrei applicare una cosa del tipo chiave pubblica e chiave privata.

I character sono stati realizzati e animati completamente in Blender. In Unity usano gli stessi passaggi (tranne l'avatar) che si usano con i modelli e le animazioni esportate da Mixamo. Volevo provare qualcosa di nuovo.

Sicuramente se dovessi rifarlo lo ottimizzerei meglio in certi punti e ne rifarei degli altri.



--Difetti--

Per come sono stati messi gli Enemy a inizio partita se stanno parlando tra di loro ignoreranno il player perchè non lo riconoscono come player, perciò non lo inseguiranno. Solo dopo che uno ci ha parlato, o che ha ricevuto l'informazione da un altro enemy, solo allora lo inseguono.

Quando si passa da uno stato ad un altro, cambia anche l'angolo, perciò possono verificarsi dei loop tra stati se il player e l'enemy stanno fermi.