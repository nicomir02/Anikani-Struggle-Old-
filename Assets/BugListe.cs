//BugListe, Verbesserungen und zukünftig zu bearbeitende Sachen

//Hohe Priorität

    //Unit Syncro doesnt work
    //Beim ausscheiden von einem Spieler werden Units nicht despawned
    //letzter verbleibender Spieler bekommt nicht Win Screen angezeigt
    //grobe skizzen/sprites für verschiedene Völker um mal ein umfangreicheres spiel aufzubauen 
    //wanderer programmierung dahinter
    //Name nur von Host, nicht von clients
    //Spielernamensliste durch Tab anzeigen lassen können(Alex)
    //Beenden Button im Hauptmenü klappt nicht
    

//Mittlere Priorität

    //Gegnerische Einheiten können durch feindliche Gebäude durchgehen

    //Sounds
    //Code kommentieren [Alex, Nico]
    //Animation von Sword Attack Knight besser ausschneiden und einfügen
    //Engelsanimation ausschneiden damit Engel fliegt

    //Einheiten wenn man dran ist kann man anfangen zu trainieren

    //Select Unit funkt. nur richtig beim Host

    //In Leiste ist schrift unterschiedlich groß

    //Kostenüberlegungen für Truppen und Gebäude
    //angegriffene truppen sollen sich im Nahkampf wehren!!
    //Truppen brauchen mehr Statistiken als Angriff + leben + bewegungsreichweite + angriffsreichweite
    //generischer flächenschaden bei angriff(für heilung, angriffe, angriffe über zeit(curse, poison))
    //IP Speicherung um benutzte IPS wieder zu wervenden(Alex)
    //Speicherung des eigenen Namens bei nöchstem SPiel(Alex)

//Niedrige Priorität

    //Wenn Engel heilen will und schon volles Leben hat, dann soll nicht Bewegung runter gehen

    //showarea nicht immer geupdated(hauptgebäude setzen, spieler ausgeschieden)
    //Angriff schräg 1 Block links 1 block rechts = 2 blöcke deshalb geht da kein angriff, ändern?
    //mindest Spieler anzahl auf 2 setzen damit man nicht alleine auf ready drücken kann(später, da zum testen noch hilfreich)

    //Menü mit existierenden Spielern
    //disqualifyPlayer methode noch im Building Manager Skript(ändern)
    //Methoden in Hilfsskripte und so umlagern und allgemein Skripte säubern
    //sehr lange latency/wartezeit auf seite des clients bei online-modus über hamachi(bleibt laut dozent)
    //manchmal verschwinden Einheiten auf Feldern(im multiplayer)
    //beim unitprefab im unit script sind die farbsprites unnötig da sie im unitanimator skript zu finden sind
    //heilung des engels funktioniert nicht (geheilte truppe nicht mehr anklickbar)
    //hauptgebäude soll auch angreifen
    //Ladebildschirm mit Katharina
    //manchmal bleiben Einheiten halb angelickt(graues feld unter ihnen), obwohl nicht mehr angeklickt eigentlich
    //Partikeleffekte für Attacken(z.b. Heilung des Engels auf Einheit)
    
    
    //cheats

    //beim spielen noch rausgefunden: noch unsortiert nach Dringlichkeit(muss noch gemacht werden)

    //animationen der engel hoch und runter wie bei blau
    //animationen der engel unsynchron
    //angriffe möglich obwohl nicht in angrifssdistanz-> kein schaden aber aktion durchgeführt und keine bewegungen mehr
    //nicht durch gegnerische truppen und gebäude bewegen können(ishealth healthmanager als hilfe?)
    //engel über kleine wasserstellen fliegen? über einheiten?
    //balancen von angriff und leben
    //balancen von truppenrekrutierung
    //neben lebensbalken verbleibende schritte als kleine notiz/zahl
    //knopf für verbleibende rekrutierungszeiten der Baracken
    //knopf um durch truppen durchzuklicken und diese für bewegung direkt auszuwählen
    //kamerabewegung durch ziehen der maus in die ecken
    //bestimmter abstand zwischen hauptgebäuden der verschiedenen Spieler und eventuelle Eingrenzung wo SPieler ihr HQ setzen dürfen
    //markierte Felder in die Bewegung dieser Truppe möglich ist
    //durch taste, Gebäude ausblenden lassen
    //knights mehr schaden und engel weniger leben haben
    //eigene Einheiten nicht auf zerstörte Gebäude bewegbar
    //gebäude können von Engeln noch geheilt werden







    //List of Fixed Bugs: (noch vervollständigen durch history aus gitHub)
    //escape taste aus Building und TruppenManager
    //engel bewegungsanimation
    