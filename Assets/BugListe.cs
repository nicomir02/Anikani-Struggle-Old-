//BugListe, Verbesserungen und zukünftig zu bearbeitende Sachen

//Hohe Priorität

    //letzter verbleibender Spieler bekommt nicht Win Screen angezeigt [Nico]
    //Mehr Sprites [Nikolai]
    //wanderer programmierung dahinter [Alex?]
    //Name nur von Host, nicht von clients [Alex]
    //Spielernamensliste durch Tab anzeigen lassen können [Alex]
    //Beenden Button im Hauptmenü klappt nicht [Katharina, Nico, Alex?]
    //eigene Einheiten nicht auf zerstörte Gebäude bewegbar [Nico]

//Mittlere Priorität

    //Tutorial oder Handbuch [Alex & Nico?]

    //Gegnerische Einheiten können durch feindliche Gebäude durchgehen //nicht durch gegnerische truppen und gebäude bewegen können(ishealth healthmanager als hilfe?) || Unit dürfte schon einmal funkt. [Alex & Nico?]

    //Sounds [Nikolai, Alex, Katharina, Nico]
    //Code kommentieren [Alex, Nico]

    //Animation von Sword Attack Knight besser ausschneiden und einfügen [Katharina?]
    //Angriffsanimation implementierung [Katharina]
    //animationen der engel unsynchron machen [Katharina]

    //Select Unit funkt. nur richtig beim Host [Nico]

    //In Leiste ist schrift unterschiedlich groß [Alex]

    //angegriffene truppen sollen sich im Nahkampf wehren!! [Alex & Nico?]
    //Truppen brauchen mehr Statistiken als Angriff + leben + bewegungsreichweite + angriffsreichweite [Was denn noch außer wehren?]

    //IP Speicherung um benutzte IPS wieder zu wervenden [Alex]
    //Speicherung des eigenen Namens bei nöchstem SPiel [Alex]

    //neben lebensbalken verbleibende schritte als kleine notiz/zahl [Alex & Nico?]

    //knopf um durch truppen durchzuklicken und diese für bewegung direkt auszuwählen [Nico]

    //bestimmter abstand zwischen hauptgebäuden der verschiedenen Spieler und eventuelle Eingrenzung wo SPieler ihr HQ setzen dürfen [Nico]

    //gebäude können von Engeln noch geheilt werden [Ausprobieren]

//Niedrige Priorität

    //generischer flächenschaden bei angriff(für heilung, angriffe, angriffe über zeit(curse, poison)) [erstmal niedrigere Priorität, Wanderer Implementierung ist wichtiger]

    //markierte Felder in die Bewegung dieser Truppe möglich ist // Markierung wo kann die Truppe hin
    //knopf für verbleibende rekrutierungszeiten der Baracken

    //Wenn Engel heilen will und schon volles Leben hat, dann soll nicht Bewegung runter gehen

    //showarea nicht immer geupdated(hauptgebäude setzen, spieler ausgeschieden)
    //Angriff schräg 1 Block links 1 block rechts = 2 blöcke deshalb geht da kein angriff, ändern?
    //mindest Spieler anzahl auf 2 setzen damit man nicht alleine auf ready drücken kann(später, da zum testen noch hilfreich)

    //Menü mit existierenden Spielern
    //disqualifyPlayer methode noch im Building Manager Skript(ändern)
    //Methoden in Hilfsskripte und so umlagern und allgemein Skripte säubern
    //sehr lange latency/wartezeit auf seite des clients bei online-modus über hamachi(bleibt laut dozent) //scheinbar nicht mehr so??
    //manchmal verschwinden Einheiten auf Feldern(im multiplayer)
    

    
    //hauptgebäude soll auch angreifen
    //Ladebildschirm mit Katharina
    //manchmal bleiben Einheiten halb angelickt(graues feld unter ihnen), obwohl nicht mehr angeklickt eigentlich
    //Partikeleffekte für Attacken(z.b. Heilung des Engels auf Einheit)

    //kamerabewegung durch ziehen der maus in die ecken
    
    //cheats


    //Kontinuierlich schauen (Balancing):
    //Kostenüberlegungen für Truppen und Gebäude
    //balancen von angriff und leben
    //balancen von truppenrekrutierung

    //Fragen:
    //engel über Einheiten fliegen?

    //List of Fixed Bugs: (noch vervollständigen durch history aus gitHub)
    //escape taste aus Building und TruppenManager
    //engel bewegungsanimation
    //Engelsanimation ausschneiden damit Engel fliegt, animationen der engel hoch und runter wie bei blau
    //Einheiten wenn man nicht dran ist kann man anfangen zu trainieren
    //beim unitprefab im unit script sind die farbsprites unnötig da sie im unitanimator skript zu finden sind
    //heilung des engels funktioniert nicht (geheilte truppe nicht mehr anklickbar)
    //angriffe möglich obwohl nicht in angrifssdistanz-> kein schaden aber aktion durchgeführt und keine bewegungen mehr
    //engel über kleine wasserstellen fliegen
    //durch taste, Gebäude ausblenden lassen