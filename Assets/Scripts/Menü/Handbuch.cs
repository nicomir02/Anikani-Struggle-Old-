using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Handbuch : MonoBehaviour
{
    [SerializeField] private GameObject fullpanel;
    [SerializeField] private GameObject hotkeypanel;
    [SerializeField] private GameObject tribepanel;
    [SerializeField] private GameObject anikaniLorepanel;
    [SerializeField] private GameObject developerspanel;

    //in tribepanel
    [SerializeField] private GameObject holypanel;
    [SerializeField] private GameObject wandererpanel;
    //in holy panel
    [SerializeField] private GameObject holyUnitsPanel;
    [SerializeField] private GameObject holyBuildingsPanel;
    [SerializeField] private GameObject holySpecialAbilitiesPanel;
    [SerializeField] private GameObject holyLorePanel;
    //in wanderer Panel
    [SerializeField] private GameObject wandererUnitsPanel;
    [SerializeField] private GameObject wandererBuildingsPanel;
    [SerializeField] private GameObject wandererSpecialAbilitiesPanel;
    [SerializeField] private GameObject wandererLorePanel;

    [SerializeField] private Button close;
    [SerializeField] private Button start;

    [SerializeField] private Button hotkeyButton;
    [SerializeField] private Button tribeButton;
    [SerializeField] private Button loreButton;
    [SerializeField] private Button developersButton;

    //innerhalb Tribes nächste Unterschicht
    [SerializeField] private Button holyButton;

    [SerializeField] private Button WandererButton;

        //inerhalb Holy
        [SerializeField] private Button holyUnitsButton;

            //einzelne Units
            [SerializeField] private Button holyNahkampfButton;
            [SerializeField] private Button holyFernkampfButton;
            [SerializeField] private Button holySpecialButton;
        [SerializeField] private Button holyBuildingsButton;
            //einzelne Buildings
            [SerializeField] private Button holyBarrackeButton;
            [SerializeField] private Button holyHolzButton;
            [SerializeField] private Button holySteinButton;
            [SerializeField] private Button holyMainButton;
        [SerializeField] private Button holySpecialAbilitiesButton;
        [SerializeField] private Button holyLoreButton;

        //innerhalb Wanderer
        [SerializeField] private Button wandererUnitsButton;

            //einzelne Units
            [SerializeField] private Button wandererNahkampfButton;
            [SerializeField] private Button wandererFernkampfButton;
            [SerializeField] private Button wandererSpecialButton;

        [SerializeField] private Button wandererBuildingsButton;
            //einzelne Buildings
            [SerializeField] private Button wandererBarrackeButton;
            [SerializeField] private Button wandererHolzButton;
            [SerializeField] private Button wandererSteinButton;
            [SerializeField] private Button wandererMainButton;
        [SerializeField] private Button wandererSpecialAbilitiesButton;
        [SerializeField] private Button wandererLoreButton;

    // Start is called before the first frame update
    void Start()
    {
        close.onClick.AddListener(onClose);
        start.onClick.AddListener(onStartPanel);
        //Oberste Schicht 1 Panels
        hotkeyButton.onClick.AddListener(hotkeyPanelStart);
        tribeButton.onClick.AddListener(tribePanelStart);
        //loreButton.onClick.AddListener(anikaniLorePanelStart);
        //developersButton.onClick.AddListener(developersPanelStart);
            //Schicht 2 Panels(vieleicht in Methoden unterordnen und Listener starten und stoppen?)
            holyButton.onClick.AddListener(holyPanelStart);
            WandererButton.onClick.AddListener(wandererPanelStart);
            //noch methoden für folgende einbauen Schicht 3
                holyUnitsButton.onClick.AddListener(holyUnitsPanelStart);
                wandererUnitsButton.onClick.AddListener(wandererUnitsPanelStart);
                holyBuildingsButton.onClick.AddListener(holyBuildingsPanelStart);
                wandererBuildingsButton.onClick.AddListener(wandererBuildingsPanelStart);
                holySpecialAbilitiesButton.onClick.AddListener(holySpecialAbilitiesPanelStart);
                wandererSpecialAbilitiesButton.onClick.AddListener(wandererSpecialAbilitiesPanelStart);
                holyLoreButton.onClick.AddListener(holyLorePanelStart);
                wandererLoreButton.onClick.AddListener(wandererLorePanelStart);
                    //noch Schicht 4. methoden auch einbauen
                    //Units
                    /*
                    holyNahkampfButton.onClick.AddListener(holyNahkampfPanelStart);
                    holyFernkampfButton.onClick.AddListener(holyFernkampfPanelStart);
                    holySpecialButton.onClick.AddListener(holySpezialEinheitPanelStart);
                    wandererNahkampfButton.onClick.AddListener(wandererNahkampfPanelStart);
                    wandererFernkampfButton.onClick.AddListener(wandererFernkampfPanelStart);
                    wandererSpecialButton.onClick.AddListener(wandererSpezialEinheitPanelStart);
                    */
                    //Buildings
                    /*
                    holyBarrackeButton.onClick.AddListener(holyBaracksPanelStart);
                    holyHolzButton.onClick.AddListener(holyHolzPanelStart);
                    holySteinButton.onClick.AddListener(holySteinPanelStart);
                    holyMainButton.onClick.AddListener(holyMainPanelStart);
                    wandererBarrackeButton.onClick.AddListener(wandererBaracksPanelStart);
                    wandererHolzButton.onClick.AddListener(wandererHolzPanelStart);
                    wandererSteinButton.onClick.AddListener(wandererSteinPanelStart);
                    wandererMainButton.onClick.AddListener(wandererMainPanelStart);*/
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.H)){
            onClose();
        }
    }

    void onStartPanel() {
        fullpanel.SetActive(true);
        GameObject.Find("GameManager").GetComponent<PauseMenu>().togglePauseOn();
    }

    void hotkeyPanelStart() {
        closePanels();
        hotkeypanel.SetActive(true);
    }

    void tribePanelStart() {
        closePanels();
        tribepanel.SetActive(true);
    }

    void anikaniLorePanelStart() {
        closePanels();
        anikaniLorepanel.SetActive(true);
    }
    void developersPanelStart() {
        closePanels();
        developerspanel.SetActive(true);
    }    
//Schicht 2 Panels
        void holyPanelStart() {
            closePanels();
            holypanel.SetActive(true);
        }
        void wandererPanelStart() {
            closePanels();
            wandererpanel.SetActive(true);
        }
//Schicht 3 Panels
    //Holy
        void holyUnitsPanelStart(){
            closePanels();
            holyUnitsPanel.SetActive(true);
        }
        void holyBuildingsPanelStart(){
            closePanels();
            holyBuildingsPanel.SetActive(true);
        }
        void holySpecialAbilitiesPanelStart(){
            closePanels();
            holySpecialAbilitiesPanel.SetActive(true);
        }
        void holyLorePanelStart(){
            closePanels();
            holyLorePanel.SetActive(true);
        }
    //Schicht 3 Wanderer
        void wandererUnitsPanelStart(){
            closePanels();
            wandererUnitsPanel.SetActive(true);
        }
        void wandererBuildingsPanelStart(){
            closePanels();
            wandererBuildingsPanel.SetActive(true);
        }
        void wandererSpecialAbilitiesPanelStart(){
            closePanels();
            wandererSpecialAbilitiesPanel.SetActive(true);
        }
        void wandererLorePanelStart(){
            closePanels();
            wandererLorePanel.SetActive(true);
        }
//Schicht 4 Panels
    //Holy Units
    /*
            void holyNahkampfPanelStart(){
                closePanels();
                holyNahkampfPanel.SetActive(true);
            }
            void holyFernkampfPanelStart(){
                closePanels();
                holyFernkampfPanel.SetActive(true);
            }
            void holySpezialEinheitPanelStart(){
                closePanels();
                holySpezialEinheitPanel.SetActive(true);
            }

    //Schicht 4 Wanderer Units
            void wandererNahkampfPanelStart(){
                closePanels();
                wandererNahkampfPanel.SetActive(true);
            }
            void wandererFernkampfPanelStart(){
                closePanels();
                wandererFernkampfPanel.SetActive(true);
            }
            void wandererSpezialEinheitPanelStart(){
                closePanels();
                wandererSpezialEinheitPanel.SetActive(true);
            }*/

        //Schicht 4 Holy Buildings
            



    //Schließt alles
    void onClose() {
        GameObject.Find("GameManager").GetComponent<PauseMenu>().togglePauseOff();
        closePanels();
        fullpanel.SetActive(false);
    }

    //Schließt alle Untermenü Panels
    void closePanels() {
        hotkeypanel.SetActive(false);
        tribepanel.SetActive(false);
        //anikaniLorepanel.SetActive(false);
        //developerspanel.SetActive(false);
        //Schicht 2
            holypanel.SetActive(false);
            wandererpanel.SetActive(false);
        //Schicht 3
                holyUnitsPanel.SetActive(false);
                wandererUnitsPanel.SetActive(false);
                holyBuildingsPanel.SetActive(false);
                wandererBuildingsPanel.SetActive(false);
                holySpecialAbilitiesPanel.SetActive(false);
                wandererSpecialAbilitiesPanel.SetActive(false);
                holyLorePanel.SetActive(false);
                wandererLorePanel.SetActive(false);

        //Schicht 4
    }
}
