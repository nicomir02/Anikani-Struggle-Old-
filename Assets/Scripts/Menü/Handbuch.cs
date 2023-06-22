using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Handbuch : MonoBehaviour
{
    
    [SerializeField] private GameObject fullpanel;
    //Untermenüs vom Fullpanel
    [SerializeField] private GameObject hotkeypanel;
    [SerializeField] private GameObject tribepanel;
    [SerializeField] private GameObject anikaniLorepanel;
    [SerializeField] private GameObject developerspanel;
    [SerializeField] private GameObject gamePlaypanel;

        //in gamePlayPanel
        [SerializeField] private GameObject goalpanel;
        [SerializeField] private GameObject rulespanel;

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

                //in holyUnitspanel
                [SerializeField] private GameObject holyNahkampfInfoPanel;
                [SerializeField] private GameObject holyFernkampfInfoPanel;
                [SerializeField] private GameObject holySpezialEinheitInfoPanel;    

                //in holyBuildingsPanel
                [SerializeField] private GameObject holyMainBuildingInfoPanel;
                [SerializeField] private GameObject holyWoodCutterInfoPanel;
                [SerializeField] private GameObject holyStoneCutterInfoPanel;
                [SerializeField] private GameObject holyBarracksInfoPanel;  

                //in wandererunitspanel
                [SerializeField] private GameObject wandererNahkampfInfoPanel;
                [SerializeField] private GameObject wandererFernkampfInfoPanel;
                [SerializeField] private GameObject wandererSpezialEinheitInfoPanel;    

                //in wandererBuildingsPanel
                [SerializeField] private GameObject wandererMainBuildingInfoPanel;
                [SerializeField] private GameObject wandererWoodCutterInfoPanel;
                [SerializeField] private GameObject wandererStoneCutterInfoPanel;
                [SerializeField] private GameObject wandererBarracksInfoPanel;

    //Buttons innerhalb der Panels

    [SerializeField] private Button close;  //x Knopf oben rechts
    [SerializeField] private Button start;  //Help Knopf in der Leiste

    //Buttons in der ersten Schicht für die möglichen Obermenüs
    [SerializeField] private Button hotkeyButton;
    [SerializeField] private Button tribeButton;
    [SerializeField] private Button loreButton;
    [SerializeField] private Button developersButton;
    [SerializeField] private Button gamePlayButton;

    //innerhalb gamePlay nöchste Unterschicht
        [SerializeField] private Button gamePlayGoalButton;
        [SerializeField] private Button gamePlayRulesButton;


    //innerhalb Tribes nächste Unterschicht
        [SerializeField] private Button holyButton;

        [SerializeField] private Button WandererButton;

            //inerhalb Holy
            [SerializeField] private Button holyUnitsButton;
            [SerializeField] private Button holyBuildingsButton;
            [SerializeField] private Button holySpecialAbilitiesButton;
            [SerializeField] private Button holyLoreButton;

                //einzelne Units
                [SerializeField] private Button holyNahkampfButton;
                [SerializeField] private Button holyFernkampfButton;
                [SerializeField] private Button holySpecialButton;
            
                //einzelne Buildings
                [SerializeField] private Button holyBarrackeButton;
                [SerializeField] private Button holyHolzButton;
                [SerializeField] private Button holySteinButton;
                [SerializeField] private Button holyMainButton;
            

            //innerhalb Wanderer
            [SerializeField] private Button wandererUnitsButton;
            [SerializeField] private Button wandererBuildingsButton;
            [SerializeField] private Button wandererSpecialAbilitiesButton;
            [SerializeField] private Button wandererLoreButton;

                //einzelne Units
                [SerializeField] private Button wandererNahkampfButton;
                [SerializeField] private Button wandererFernkampfButton;
                [SerializeField] private Button wandererSpecialButton;

                //einzelne Buildings
                [SerializeField] private Button wandererBarrackeButton;
                [SerializeField] private Button wandererHolzButton;
                [SerializeField] private Button wandererSteinButton;
                [SerializeField] private Button wandererMainButton;

            //Gameobjects in Schicht 4
                //Holy Schicht 4 GameObjects
                [SerializeField] private GameObject NahkampfHoly;
                [SerializeField] private GameObject FernkampfHoly;
                [SerializeField] private GameObject  SpezialEinheitHoly;

                [SerializeField] private GameObject MainBuildingHoly;
                [SerializeField] private GameObject WoodCutterHoly;
                [SerializeField] private GameObject StoneCutterHoly;
                [SerializeField] private GameObject BarracksHoly;
                //Wanderer Schicht 4 Gameobjects
                [SerializeField] private GameObject NahkampfWanderer;
                [SerializeField] private GameObject FernkampfWanderer;
                [SerializeField] private GameObject  SpezialEinheitWanderer;

                [SerializeField] private GameObject MainBuildingWanderer;
                [SerializeField] private GameObject WoodCutterWanderer;
                [SerializeField] private GameObject StoneCutterWanderer;
                [SerializeField] private GameObject BarracksWanderer;
            

    // Start is called before the first frame update
    void Start()
    {
        close.onClick.AddListener(onClose);
        start.onClick.AddListener(onStartPanel);
        //Oberste Schicht 1 Panels
        hotkeyButton.onClick.AddListener(hotkeyPanelStart);
        tribeButton.onClick.AddListener(tribePanelStart);
        loreButton.onClick.AddListener(anikaniLorePanelStart);
        developersButton.onClick.AddListener(developersPanelStart);
        gamePlayButton.onClick.AddListener(gamePlayPanelStart);
            //in GamePlay Schicht 2
            gamePlayGoalButton.onClick.AddListener(gamePlayGoalPanelStart);
            gamePlayRulesButton.onClick.AddListener(gamePlayRulesPanelStart);
            //in Tribes Schicht 2
            holyButton.onClick.AddListener(holyPanelStart);
            WandererButton.onClick.AddListener(wandererPanelStart);
                // Schicht 3
                holyUnitsButton.onClick.AddListener(holyUnitsPanelStart);
                wandererUnitsButton.onClick.AddListener(wandererUnitsPanelStart);
                holyBuildingsButton.onClick.AddListener(holyBuildingsPanelStart);
                wandererBuildingsButton.onClick.AddListener(wandererBuildingsPanelStart);
                holySpecialAbilitiesButton.onClick.AddListener(holySpecialAbilitiesPanelStart);
                wandererSpecialAbilitiesButton.onClick.AddListener(wandererSpecialAbilitiesPanelStart);
                holyLoreButton.onClick.AddListener(holyLorePanelStart);
                wandererLoreButton.onClick.AddListener(wandererLorePanelStart);
                    //Schicht 4.
                    //Units
                    
                    holyNahkampfButton.onClick.AddListener(holyNahkampfPanelStart);
                    holyFernkampfButton.onClick.AddListener(holyFernkampfPanelStart);
                    holySpecialButton.onClick.AddListener(holySpezialEinheitPanelStart);
                    wandererNahkampfButton.onClick.AddListener(wandererNahkampfPanelStart);
                    wandererFernkampfButton.onClick.AddListener(wandererFernkampfPanelStart);
                    wandererSpecialButton.onClick.AddListener(wandererSpezialEinheitPanelStart);
                    
                    //Buildings
                    
                    holyBarrackeButton.onClick.AddListener(holyBaracksPanelStart);
                    holyHolzButton.onClick.AddListener(holyWoodCutterPanelStart);
                    holySteinButton.onClick.AddListener(holyStoneCutterPanelStart);
                    holyMainButton.onClick.AddListener(holyMainBuildingPanelStart);
                    wandererBarrackeButton.onClick.AddListener(wandererBaracksPanelStart);
                    wandererHolzButton.onClick.AddListener(wandererWoodCutterPanelStart);
                    wandererSteinButton.onClick.AddListener(wandererStoneCutterPanelStart);
                    wandererMainButton.onClick.AddListener(wandererMainBuildingPanelStart);
    }

    void Update(){
        //old: if(Input.GetKeyDown(KeyCode.H)){
        if(Input.GetButtonDown("Toggle HelpMenu")){
            if(fullpanel.activeSelf){
                onClose();
            } else {
                onStartPanel();
            }
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

    void gamePlayPanelStart() {
        closePanels();
        gamePlaypanel.SetActive(true);
    }     
//Schicht 2 Panels in GamePlay

        void gamePlayGoalPanelStart(){
            gamePlayPanelStart();
            goalpanel.SetActive(true);
        }
        void gamePlayRulesPanelStart(){
            gamePlayPanelStart();
            rulespanel.SetActive(true);
        }


//Schicht 2 Panels in Tribes
        void holyPanelStart() {
            tribePanelStart();
            holypanel.SetActive(true);
        }
        void wandererPanelStart() {
            tribePanelStart();
            wandererpanel.SetActive(true);
        }
//Schicht 3 Panels
    //Holy
        void holyUnitsPanelStart(){
            holyPanelStart();
            holyUnitsPanel.SetActive(true);
            NahkampfHoly.SetActive(true);
            FernkampfHoly.SetActive(true);
            SpezialEinheitHoly.SetActive(true);
        }
        void holyBuildingsPanelStart(){
            holyPanelStart();
            holyBuildingsPanel.SetActive(true);
            MainBuildingHoly.SetActive(true);
            StoneCutterHoly.SetActive(true);
            WoodCutterHoly.SetActive(true);
            BarracksHoly.SetActive(true);
        }
        void holySpecialAbilitiesPanelStart(){
            holyPanelStart();
            holySpecialAbilitiesPanel.SetActive(true);
        }
        void holyLorePanelStart(){
            holyPanelStart();
            holyLorePanel.SetActive(true);
        }
    //Schicht 3 Wanderer
        void wandererUnitsPanelStart(){
            wandererPanelStart();
            wandererUnitsPanel.SetActive(true);
            //Gameobjects aktivieren
            NahkampfWanderer.SetActive(true);
            FernkampfWanderer.SetActive(true);
            SpezialEinheitWanderer.SetActive(true);
        }
        void wandererBuildingsPanelStart(){
            wandererPanelStart();
            wandererBuildingsPanel.SetActive(true);
            //Gameobjects aktivieren
            MainBuildingWanderer.SetActive(true);
            StoneCutterWanderer.SetActive(true);
            WoodCutterWanderer.SetActive(true);
            BarracksWanderer.SetActive(true);
        }
        void wandererSpecialAbilitiesPanelStart(){
            wandererPanelStart();
            wandererSpecialAbilitiesPanel.SetActive(true);
        }
        void wandererLorePanelStart(){
            wandererPanelStart();
            wandererLorePanel.SetActive(true);
        }
//Schicht 4 Panels
    //Holy Units
    
            void holyNahkampfPanelStart(){
                holyUnitsPanelStart();                
                HolyEinheitenDeaktivieren(); //alle außer Nahkampf
                NahkampfHoly.SetActive(true);   //deswegen hier weider aktiviert
                holyNahkampfInfoPanel.SetActive(true);
            }
            void holyFernkampfPanelStart(){
                holyUnitsPanelStart();
                HolyEinheitenDeaktivieren(); //alle außer Fernkampf
                FernkampfHoly.SetActive(true);   //deswegen hier weider aktiviert
                holyFernkampfInfoPanel.SetActive(true);
            }
            void holySpezialEinheitPanelStart(){
                holyUnitsPanelStart();
                HolyEinheitenDeaktivieren(); //alle außer SpezialEinheit
                SpezialEinheitHoly.SetActive(true);   //deswegen hier weider aktiviert               
                holySpezialEinheitInfoPanel.SetActive(true);
            }

    //Schicht 4 Wanderer Units
            void wandererNahkampfPanelStart(){
                wandererUnitsPanelStart();                
                WandererEinheitenDeaktivieren();//alle außer Nahkampf
                NahkampfWanderer.SetActive(true); //deswegen hier weider aktiviert
                wandererNahkampfInfoPanel.SetActive(true);
            }
            void wandererFernkampfPanelStart(){
                wandererUnitsPanelStart();
                WandererEinheitenDeaktivieren();//alle außer Fernkampf
                FernkampfWanderer.SetActive(true); //deswegen hier weider aktiviert
                wandererFernkampfInfoPanel.SetActive(true);
            }
            void wandererSpezialEinheitPanelStart(){
                wandererUnitsPanelStart();
                WandererEinheitenDeaktivieren();//alle außer SpezialEinheit
                SpezialEinheitWanderer.SetActive(true); //deswegen hier weider aktiviert               
                wandererSpezialEinheitInfoPanel.SetActive(true);
            }

        //Schicht 4 Holy Buildings
            void holyBaracksPanelStart(){
                holyBuildingsPanelStart();
                HolyBuildingsDeaktivieren(); //alle außer Barracken
                BarracksHoly.SetActive(true); //deswegen hier wieder aktiviert               
                holyBarracksInfoPanel.SetActive(true);
            }
            void holyWoodCutterPanelStart(){
                holyBuildingsPanelStart();
                HolyBuildingsDeaktivieren(); //alle außer WoodCutter
                WoodCutterHoly.SetActive(true); //deswegen hier wieder aktiviert  
                holyWoodCutterInfoPanel.SetActive(true);
            }
            void holyStoneCutterPanelStart(){
                holyBuildingsPanelStart();
                HolyBuildingsDeaktivieren(); //alle außer StoneCutter
                StoneCutterHoly.SetActive(true); //deswegen hier wieder aktiviert  
                holyStoneCutterInfoPanel.SetActive(true);
            }
            void holyMainBuildingPanelStart(){
                holyBuildingsPanelStart();                
                HolyBuildingsDeaktivieren(); //alle außer MainBuilding
                MainBuildingHoly.SetActive(true); //deswegen hier wieder aktiviert  
                holyMainBuildingInfoPanel.SetActive(true);
            }
        //Schicht 4 Wanderer Buildings
            void wandererBaracksPanelStart(){
                wandererBuildingsPanelStart();
                WandererBuildingsDeaktivieren(); //alle außer Barracken
                BarracksWanderer.SetActive(true); //deswegen hier wieder aktiviert                
                wandererBarracksInfoPanel.SetActive(true);
            }
            void wandererWoodCutterPanelStart(){
                wandererBuildingsPanelStart();
                WandererBuildingsDeaktivieren(); //alle außer WoodCutter
                WoodCutterWanderer.SetActive(true); //deswegen hier wieder aktiviert  
                wandererWoodCutterInfoPanel.SetActive(true);
            }
            void wandererStoneCutterPanelStart(){
                wandererBuildingsPanelStart();
                WandererBuildingsDeaktivieren(); //alle außer StoneCutter
                StoneCutterWanderer.SetActive(true); //deswegen hier wieder aktiviert  
                wandererStoneCutterInfoPanel.SetActive(true);
            }
            void wandererMainBuildingPanelStart(){
                wandererBuildingsPanelStart();                
                WandererBuildingsDeaktivieren(); //alle außer MainBuilding
                MainBuildingWanderer.SetActive(true); //deswegen hier wieder aktiviert
                wandererMainBuildingInfoPanel.SetActive(true);
            }


    //Schließt alles
    void onClose() {
        GameObject.Find("GameManager").GetComponent<PauseMenu>().togglePauseOff();
        closePanels();
        fullpanel.SetActive(false);
    }

    //Schließt alle Untermenü Panels
    void closePanels() {
    //Schicht 1 Menüs/Panels
        hotkeypanel.SetActive(false);
        tribepanel.SetActive(false);
        anikaniLorepanel.SetActive(false);
        developerspanel.SetActive(false);
        gamePlaypanel.SetActive(false);
        //Schicht 2 GamePlay
            goalpanel.SetActive(false);
            rulespanel.SetActive(false);
        //Schicht 2 Tribes
            holypanel.SetActive(false);
            wandererpanel.SetActive(false);
        //Schicht 3
            //Holy
                holyUnitsPanel.SetActive(false);
                holyBuildingsPanel.SetActive(false);
                holySpecialAbilitiesPanel.SetActive(false);
                holyLorePanel.SetActive(false);
            //Wanderer
                wandererUnitsPanel.SetActive(false);
                wandererBuildingsPanel.SetActive(false);
                wandererSpecialAbilitiesPanel.SetActive(false);
                wandererLorePanel.SetActive(false);
        //Schicht 4
            //Holy
                    //Holy Units
                    holyNahkampfInfoPanel.SetActive(false);
                    holyFernkampfInfoPanel.SetActive(false);
                    holySpezialEinheitInfoPanel.SetActive(false);
                    //Holy Buildings
                    holyMainBuildingInfoPanel.SetActive(false);
                    holyWoodCutterInfoPanel.SetActive(false);
                    holyStoneCutterInfoPanel.SetActive(false);
                    holyBarracksInfoPanel.SetActive(false);
            //Wanderer
                    //Wanderer Units
                    wandererNahkampfInfoPanel.SetActive(false);
                    wandererFernkampfInfoPanel.SetActive(false);
                    wandererSpezialEinheitInfoPanel.SetActive(false);
                    //Wanderer Buildings
                    wandererMainBuildingInfoPanel.SetActive(false);
                    wandererWoodCutterInfoPanel.SetActive(false);
                    wandererStoneCutterInfoPanel.SetActive(false);
                    wandererBarracksInfoPanel.SetActive(false);
    }

    //Hilfsmethode
        void HolyEinheitenDeaktivieren(){
            NahkampfHoly.SetActive(false);
            FernkampfHoly.SetActive(false);
            SpezialEinheitHoly.SetActive(false);
        }
        void HolyBuildingsDeaktivieren(){
            MainBuildingHoly.SetActive(false);
            StoneCutterHoly.SetActive(false);
            WoodCutterHoly.SetActive(false);
            BarracksHoly.SetActive(false);
        }
        void WandererEinheitenDeaktivieren(){
            NahkampfWanderer.SetActive(false);
            FernkampfWanderer.SetActive(false);
            SpezialEinheitWanderer.SetActive(false);
        }        
        void WandererBuildingsDeaktivieren(){
            MainBuildingWanderer.SetActive(false);
            StoneCutterWanderer.SetActive(false);
            WoodCutterWanderer.SetActive(false);
            BarracksWanderer.SetActive(false);
        }
}
