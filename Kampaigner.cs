using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using KSPPluginFramework;

namespace Kampaigner
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    class KampaignerMainMenu : MonoBehaviour
    {
        public void Start()
        {
            var game = HighLogic.CurrentGame;
            ProtoScenarioModule psm = game.scenarios.Find(s => s.moduleName == typeof(KampaignerProgram).Name);
            if (psm == null)
            {
                GameScenes[] desiredScenes = new GameScenes[5] { GameScenes.SPACECENTER, GameScenes.EDITOR, GameScenes.FLIGHT, GameScenes.TRACKSTATION, GameScenes.SPH };
                psm = game.AddProtoScenarioModule(typeof(KampaignerProgram), desiredScenes);
            }
        }
    }
    public class KampaignerProgram : ScenarioModule
    {
        [KSPField(isPersistant = true)]
        public string aSavedValue = "the default value";


        public override void OnSave(ConfigNode node)
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                Debug.Log("KampaignerProgram: Saving in FLIGHT scene");
                // We need to go through all parts and store the flight data
                ConfigNode partData = node.AddNode("KAMPAIGNER_PARTDATA");
                partData.AddValue("raw_rnd_level", 0);
                partData.AddValue("flightdata_surface", 100);
                partData.AddValue("final_reliability", 90.5);
                aSavedValue = "Modified and resumed from the persistent save";
            }
            Debug.Log("KampaignerProgram: Scenario Saved");
        }
    }
    
    public class FlightDataRecorder : PartModule
    {
        // Flight data is recorded for unique Planet + Situation EG:
        // kerbin_flyinghigh
        // mun_landed
        // The flightData key is always in the format "planet_situation", all lowercase.
        Dictionary<string, float> flightData = new Dictionary<string, float>();

        private bool isRecordingFlightData = false;

        public override void OnLoad(ConfigNode node)
        {
            print("FlightDataRecorder: OnLoad");
            print("FlightDataRecorder: " + node.ToString());
        }

        public override void OnStart(StartState state)
        {
            print("FlightDataRecorder: onStart()");
            print("FlightDataRecorder: State = " + state);
        }
        public override void OnUpdate()
        {
            //print("FlightDataRecorder: onUpdate");
        }
        public override void OnSave(ConfigNode node)
        {
            print("FlightDataRecorder: OnSave");
            print("FlightDataRecorder: foo = " + node.GetValue("foo"));
            print("FlightDataRecorder: num = " + node.GetValue("num"));
        }
        public override void OnFixedUpdate()
        {
            if (!isRecordingFlightData)
                return;
            string mainBody = this.vessel.mainBody.name.ToLower();
            string situation = this.vessel.situation.ToString().ToLower();
            string flightDataSituation = mainBody + "_" + situation;
            print("FlightDataRecorder: Recording flight data for " + flightDataSituation);
            this.part.RequestResource("FlightData", -1);
        }
        public override void OnActive()
        {
            print("FlightDataRecorder: onActive");
            isRecordingFlightData = true;
        }
        public override void OnInactive()
        {
            print("FlightDataRecorder: onInactive");
            isRecordingFlightData = false;
        }
    }
}
