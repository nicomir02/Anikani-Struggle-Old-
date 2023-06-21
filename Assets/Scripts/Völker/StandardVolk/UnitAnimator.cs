using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Threading.Tasks;

public class UnitAnimator : NetworkBehaviour
{ 
    [SerializeField] private Sprite[] idleBLUE;
    [SerializeField] private Sprite[] idleRED;
    [SerializeField] private Sprite[] idleGREEN;
    [SerializeField] private Sprite[] idlePURP;

    [SerializeField] private Sprite[] moveForwardBLUE;
    [SerializeField] private Sprite[] moveBackBLUE;

    [SerializeField] private Sprite[] moveForwardRED;
    [SerializeField] private Sprite[] moveBackRED;

    [SerializeField] private Sprite[] moveForwardGREEN;
    [SerializeField] private Sprite[] moveBackGREEN;

    [SerializeField] private Sprite[] moveForwardPURP;
    [SerializeField] private Sprite[] moveBackPURP;

    [SerializeField] private Sprite[] attackBLUE;
    [SerializeField] private Sprite[] attackRED;
    [SerializeField] private Sprite[] attackGREEN;
    [SerializeField] private Sprite[] attackPURP;

    [SerializeField] private Sprite[] backAttackBLUE;
    [SerializeField] private Sprite[] backAttackRED;
    [SerializeField] private Sprite[] backAttackGREEN;
    [SerializeField] private Sprite[] backAttackPURP;

    [SerializeField] private Sprite[] healBLUE;
    [SerializeField] private Sprite[] healRED;
    [SerializeField] private Sprite[] healGREEN;
    [SerializeField] private Sprite[] healPURP;

    [SerializeField] private Sprite[] healUnit;
    [SerializeField] private Sprite[] attackEnemy;
    

    private SpriteRenderer spriteRenderer; 
    private SpriteRenderer fernRenderer;
    private Sprite[] idle;
    private Sprite[] forward;
    private Sprite[] back;
    private Sprite[] attack;
    private Sprite[] backAttack; 
    private Sprite[] healing; 
    private new Sprite[] animation;
    private bool gedreht = false;
    private bool angriff = false;
    private bool laeuft = false; //läuft die coroutine
    bool fertigGelaufen = false;
    private Sprite[] zielAnimationen = null;
    public GameObject fernZiel;
    private Vector3Int fernZielPosition;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        int spielerFarbe = GetComponent<UnitSprite>().id;
        
        chooseColor(spielerFarbe);
    }

    //Versuch den Bug mit unterschiedlichen Spielerfarben zu lösen bei den Clients
    public void chooseColor(int spielerFarbe) {
        if(spielerFarbe == 1) {        //BLAU
            forward = moveForwardBLUE;
            back = moveBackBLUE;
            idle = idleBLUE;
            attack = attackBLUE;
            backAttack = backAttackBLUE;
        } else if(spielerFarbe == 2) { //ROT
            forward = moveForwardRED;
            back = moveBackRED;
            idle = idleRED;
            attack = attackRED;
            backAttack = backAttackRED;
        } else if(spielerFarbe == 3) { //GRÜN
            forward = moveForwardGREEN;
            back = moveBackGREEN;
            idle = idleGREEN;
            attack = attackGREEN;
            backAttack = backAttackGREEN;
        } else {                       //LILA
            forward = moveForwardPURP;
            back = moveBackPURP;
            idle = idlePURP;
            attack = attackPURP;
            backAttack = backAttackPURP;
        }

        if(healUnit != null) {
            if(spielerFarbe == 1) {        //BLAU
                healing = healBLUE;
            } else if(spielerFarbe == 2) { //ROT
                healing = healRED;
            } else if(spielerFarbe == 3) { //GRÜN
                healing = healGREEN;
            } else {                       //LILA
                healing = healPURP;
            }
        }
        animation = idle;
    }


    // Update is called once per frame
    void Update() {
        if(!laeuft){
            fertigGelaufen = false;
            if(!angriff){
                int index = Mathf.FloorToInt(Time.time * 4) % animation.Length; //4 für Geschwindigkeit
                spriteRenderer.sprite = animation[index];
            } else {
                StartCoroutine(animationOneTime(zielAnimationen));
                laeuft = true;
            }  
        }
        if(fertigGelaufen) {
            StopCoroutine("animationOneTime");
            laeuft = false;
        }
    }
    

    async public void warten(int dauer) {
        await Task.Delay(dauer);
    }


    //Für UnitGUIPanel um richtiges Sprite auszuwählen
    public Sprite getSprite(int spielerFarbe, int auswahl) {
        if(spielerFarbe == 1) {        //BLAU
            return idleBLUE[auswahl];
        } else if(spielerFarbe == 2) { //ROT
            return idleRED[auswahl];
        } else if(spielerFarbe == 3) { //GRÜN
            return idleGREEN[auswahl];
        } else if(spielerFarbe == 4){  //LILA
            return idlePURP[auswahl];
        }
        return null;
    }

    //Vom Server changeDirection anstoßen
    [Command(requiresAuthority=false)]
    public void changeDirection(Vector3Int start, Vector3Int ziel, int playerid) {
        RPCchangeDirection(start, ziel, playerid);
    }

    
    [ClientRpc] //richtige Sprites auswählen auf Clients
    public void RPCchangeDirection(Vector3Int start, Vector3Int ziel, int playerid) {
        chooseColor(playerid); //Siehe Methode
        if(start == ziel) {
            animation = idle;
        } else {
            if(start.x > ziel.x) {
                if(gedreht) rumdrehen();
                animation = forward;
            }
            if(start.y > ziel.y) {
                if(!gedreht) rumdrehen();
                animation = forward;
            }
            if(start.x < ziel.x) {
                if(!gedreht) rumdrehen();
                animation = back;
            }
            if(start.y < ziel.y) {
                if(gedreht) rumdrehen();
                animation = back;
            }
        }
    }

    //Sprite rumdrehen
    void rumdrehen() {
        GetComponent<Transform>().Rotate(new Vector3(0f, 180f, 0f));
        RectTransform rectTransform = gameObject.transform.GetChild(0).GetComponent<RectTransform>();
        rectTransform.Rotate(new Vector3(0f, 180f, 0f));
        Vector3 vector = rectTransform.localPosition;
        vector.x = -vector.x;
        rectTransform.localPosition = vector;

        gedreht = !gedreht;
    }

    [ClientRpc]
    public void angreifenN(Vector3Int player, Vector3Int enemy) {

        if(healBLUE != null) {
            zielAnimationen = attackEnemy;
            if(player.x > enemy.x) {
                if(gedreht) rumdrehen();
                animation = attack;
            }
            if(player.y > enemy.y) {
                if(!gedreht) rumdrehen();
                animation = attack;
            }
        } else {        

            //richtig drehen
            if(player.x > enemy.x) {
                if(gedreht) rumdrehen();
                animation = attack;
            }
            if(player.y > enemy.y) {
                if(!gedreht) rumdrehen();
                animation = attack;
            }
            if(player.x < enemy.x) {
                if(!gedreht) rumdrehen();
                animation = backAttack;
            }
            if(player.y < enemy.y) {
                if(gedreht) rumdrehen();
                animation = backAttack;
            }
        }

        enemy.z = 3;
        fernZiel.transform.position = enemy;
        Debug.Log(enemy);

        //Animation einmal abspielen
        angriff = true;
    }

    //enemy ist hier die eigene unit die geheilt wird war zu faul den Namen zu ändern
    public void heilen(Vector3Int player, Vector3Int enemy){

        //richtig drehen
        if(player.x > enemy.x) {
            if(gedreht) rumdrehen();
            animation = healing;
        }
        if(player.y > enemy.y) {
            if(!gedreht) rumdrehen();
            animation = healing;
        }

        if(healBLUE != null) {
            zielAnimationen = healUnit;
        }

        enemy.z = 3;
        fernZiel.transform.position = enemy;

        angriff = true;
    }

    //Hilfsfunktion für angreifenN()
    //spielt die Angriffsanimation genau einmal ab und setzt dann wieder idle
    private IEnumerator animationOneTime(Sprite[] zielAnimation) {
        int laenge = animation.Length;
        for(int i=0; i<laenge; i++) {
            spriteRenderer.sprite = animation[i];
            yield return new WaitForSeconds(0.04f);
        }

        if(zielAnimation != null) {
            Debug.Log(fernZiel);
            Debug.Log(fernZielPosition);
            spawnZiel();
            fernRenderer = fernZiel.GetComponent<SpriteRenderer>();
            
            for(int i=0; i<laenge; i++) {
            fernRenderer.sprite = zielAnimation[i];
            yield return new WaitForSeconds(0.04f);
            }
            destroyZiel();
        }

        animation = idle;
        zielAnimationen = null;
        angriff = false;
        laeuft = false;
    }

    [Command(requiresAuthority = false)]
    public void spawnZiel() {
        GameObject spriteObject = Instantiate(fernZiel, transform.position, transform.rotation);
        NetworkServer.Spawn(spriteObject);
    }
    [Command(requiresAuthority = false)]
    public void destroyZiel() {
        NetworkServer.Destroy(GameObject.Find("Cylinder(Clone)"));
    }



}

