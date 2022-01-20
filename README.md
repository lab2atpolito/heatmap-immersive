# heatmap-immersive

Aprire l'unica scena presente "heatmaps"

1) Assegnare alla proprietà "Video Clip" dell'oggetto Video Player, il file video su cui si vuole disegnare la heatmap.
2) All'oggetto "Target" vi è assegnato lo script "QuadScript". Bisogna qui assegnare le seguenti proprietà:
- Data path: l'indirizzo locale del file csv* dove sono contenuti i dati legati all'eye tracking
- Window length: la lunghezza dell'intervallo temporale considerato
- Instant time: l'istante di tempo in cui si vuole calcolare la heatmap.
- Tracking: selezionare se i dati sono riferiti al solo orientamento della testa "Head" o più propriamente alla direzione dello sguardo "Eye" 

Una volta cliccato play, il codice genererà una heatmap per ogni frame a partire dall'instant time e salverà uno screenshot in png in una cartella denominata con il nome del file video.

*Il codice, per così come è stato strutturato, è funzionante se i file csv sono strutturati nella seguente maniera:
SubjectID; Time; Name; X-Position; Y-Position
-1; 0,02; Planar_Gaze_Point; 4032; 1344;
-1; 0,02; Planar_Eye_Point; 2688; 1344;
-1; 0,04; Planar_Gaze_Point; 2531; 1381;
-1; 0,04; Planar_Eye_Point; 4007; 1042;
-1; 0,06; Planar_Gaze_Point; 2531; 1381;

Il campo Name differenzia i dati relativi al tracciamento degli occhi "Planar_Eye_Point" e quelli relativi all'orientamento della testa "Planar_Gaze_Point". Per scegliere l'uno o l'altro bisogna scegliere il valore della variabile Tracking nello script QuadScript assegnato a Target. X e Y Position sono le coordinate in pixel del punto registrato.

All'oggetto Target vi è assegnato un materiale "Video Material", da qui è possibile regolare diversi parametri che variano l'aspetto dell'heatmap:
- I colori utilizzati per disegnare l'heatmap: di default gli "hot-points" sono colorati in rosso, a seguire arancione, giallo e verde.
- Range: i valori di intensità (percentuali) a cui corrispondono i diversi colori.
- Diametro: aumentando questo valore aumenta la dimensione minima di ogni punto disegnato.
- Numero di utenti: il calcolo del colore di ogni pixel dipende dal numero di utenti da cui si sono prelevati i dati.
![image](https://user-images.githubusercontent.com/75683448/150297398-4bc7478b-c266-4376-be9c-1bb34aa99574.png)

