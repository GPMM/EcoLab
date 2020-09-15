using CubicHex;
using EcoClean.Domain;
using EcoClean.TimeManaging;
using EcoClean.TimeManaging.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

namespace EcoClean
{
    public class GameManager : MonoBehaviour
    {
        #region Local variables

        #region Time
        private float secondsToNextTick = 0;
        private int currentTimeStepMultiplierIndex = 0;
        private bool paused = true;
        private bool simulationIsStarted = false;

        private TextMeshProUGUI timeScaleText;
        #endregion Time

        #region Selection
        private string selectedElement = "";
        private ElementType selectedElementType = ElementType.NONE;
        #endregion Selection

        private Camera MainCamera;
        private Animation AnimButton;
        private Animation AnimCamera;
        private Animation AnimGraph;
        #endregion Local variables

        #region Serialized variables
        public GameObject GraphPanel;
        public GameObject ButtonPrefab;
        public GameObject ButtonPanel;
        public GameObject ButtonPlay;
        public GameObject ButtonPause;
        public GameObject TimeScale;

        #region Properties
        public float BaseSecondsPerTick = 3;
        public int graphUpdateIntervalDays = 1;
        #endregion Properties

        #endregion Serialized variables

        #region Properties
        public HexMap hexMap { get; private set; }
        public static GameManager Instance { get; private set; }
        #endregion Properties

        #region Methods

        #region Unity methods
        private void Awake()
        {
            ErrorHandler.AssertExists(Instance, "There is more than one GameLogic script running.");
            Instance = this;
            
            GameObject hexesGameObject = GameObject.Find("Hexes");
            ErrorHandler.AssertNull(hexesGameObject, "Hexes object not found! Cannot proceed execution.");
            
            hexMap = hexesGameObject.GetComponent<HexMap>();
            ErrorHandler.AssertNull(hexMap, "HexMap script not found in GameManager object!");
            
            MainCamera = Camera.main;
            ErrorHandler.AssertNull(MainCamera, "MainCamera object not found! Cannot proceed execution!");
            
            AnimCamera = MainCamera.GetComponent<Animation>();
            ErrorHandler.AssertNull(AnimCamera, "Main Camera Animation component not found! Cannot proceed execution!");
            
            ErrorHandler.AssertNull(GraphPanel, "GraphPanel object not found! Cannot proceed execution!");
            
            AnimGraph = GraphPanel.GetComponent<Animation>();
            ErrorHandler.AssertNull(AnimGraph, "Graph Panel Animation component not found! Cannot proceed execution!");
            
            ErrorHandler.AssertNull(ButtonPanel, "ButtonPanel object component not found! Cannot proceed execution!");
            
            AnimButton = ButtonPanel.GetComponent<Animation>();
            ErrorHandler.AssertNull(AnimButton, "Button Panel Animation component not found! Cannot proceed execution!");
            
            ErrorHandler.AssertNull(ButtonPrefab, "Button prefab not found! Cannot proceed execution!");
            
            ErrorHandler.AssertNull(TimeScale, "Time Scale object not found! Cannot proceed execution!");
            
            timeScaleText = TimeScale.GetComponent<TextMeshProUGUI>();
            
            ErrorHandler.AssertNull(timeScaleText, "Time Scale TextMeshPro not found! Cannot proceed execution!");
        }

        private void Start()
        {
            // Start the simulation paused
            UITimePause();

            BuildButtons();
        }

        private void Update()
        {
            try
            {
                HandleControls();
            }
            catch (Exception e)
            {
                ErrorHandler.LogError("Runtime error during the control handling step: " + e.StackTrace, e);
            }

            if (!paused)
            {
                HandleTick();
            }
        }
        #endregion Unity methods

        #region Update() methods
        /// <summary>
        /// Handles every keybind and how they interact with the game.
        /// 
        /// Does not account for GUI input
        /// </summary>
        private void HandleControls()
        {
            if (Input.GetMouseButtonDown(1))
            {
                PROTOSaveJSON();
            }

            if (!simulationIsStarted && Input.GetMouseButton(0))
            {
                Hex mouseHex = Hex.FindHexAtMousePosition();

                if (mouseHex != null)
                {
                    List<Hex> petriDishSlots;

                    switch (selectedElementType)
                    {
                        case ElementType.MICROORGANISM:
                            petriDishSlots = hexMap.GetHexesWithin(new Hex(mouseHex.Q, mouseHex.R), 1).ToList();

                            Dictionary<string, Microorganism> microorganisms = Repository.GetMicroorganismsDictionary();

                            foreach (Hex hex in petriDishSlots)
                            {
                                PetriDishSlot petriDishSlot = (PetriDishSlot)hex;

                                petriDishSlot.AddMicroorganism(microorganisms[selectedElement], 0.1f);
                            }

                            break;
                        case ElementType.POLLUTANT:

                            petriDishSlots = hexMap.GetHexesWithin(new Hex(mouseHex.Q, mouseHex.R), 3).ToList();

                            Dictionary<string, Pollutant> pollutants = Repository.GetPollutantsDictionary();

                            foreach (Hex hex in petriDishSlots)
                            {
                                PetriDishSlot petriDishSlot = (PetriDishSlot)hex;

                                petriDishSlot.AddPollutant(pollutants[selectedElement], 1.5f);
                            }

                            break;
                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Handles timing, game speed calculations, and calls the mechanics that happen every tick.
        /// </summary>
        private void HandleTick()
        {
            if (secondsToNextTick <= 0)
            {
                // Execute the next tick.
                //try
                //{
                FeedingPhase();
                //}
                //catch (Exception e)
                //{
                //    ErrorHandler.LogError("RUNTIME ERROR DURING THE FEEDING PHASE: " + e.Message);
                //}

                //try
                //{
                BinaryDivisionPhase();
                //}
                //catch (Exception e)
                //{
                //    ErrorHandler.LogError("RUNTIME ERROR DURING THE BINARY DIVISION PHASE: " + e.Message);
                //}

                // Reset the tick timer.
                secondsToNextTick = Config.SECONDS_PER_TICK;
            }
            else
            {
                // Progress the timer to the next tick, according to the current game speed.
                secondsToNextTick -= UnityEngine.Time.deltaTime * Config.TIME_STEP_MULTIPLIERS[currentTimeStepMultiplierIndex];
            }
        }
        #endregion Update() methods

        #region Feeding phase methods
        /// <summary>
        /// Allows the microorganisms to consume pollutants in their Hexes.
        /// </summary>
        private void FeedingPhase()
        {
            Dictionary<Consumption, float> consumptionPerMicroorganism = Tick.GetEmptyConsumptionPerMicroorganism();

            foreach (PetriDishSlot petriDishSlot in hexMap.AllHexes)
            {
                if (petriDishSlot.Microorganism != null)
                {
                    float amountConsumed = Feed(petriDishSlot);

                    if (!(petriDishSlot.Microorganism is null) && !(petriDishSlot.Pollutant is null))
                    {
                        Consumption consumption = new Consumption(petriDishSlot.Microorganism, petriDishSlot.Pollutant);

                        consumptionPerMicroorganism[consumption] += amountConsumed;
                    }
                }
            }

            bool updateGraph = (TimeManager.NextTick % graphUpdateIntervalDays) == 0;

            TimeManager.CalculateNewTick(consumptionPerMicroorganism, updateGraph);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="petriDishSlot">The tile slot to be simulated</param>
        /// <returns>The amount consumed by the microorganism</returns>
        private float Feed(PetriDishSlot petriDishSlot)
        {
            // The return value to be calculated as how much of the pollutant the microorganism has consumed.
            float amountConsumed = 0;

            if (petriDishSlot == null)
            {
                ErrorHandler.LogError("Method Feed() was called with a null argument.", new ArgumentNullException("petriDishSlot"));
            }

            // Performs the basic energy decay step. The microorganism will lose its base energy consumption.
            Decay(petriDishSlot);

            if (petriDishSlot.Microorganism != null && petriDishSlot.Pollutant != null)
            {
                float reaction = Repository.GetReaction(
                    petriDishSlot.Microorganism,
                    petriDishSlot.Pollutant);

                // Generates a random deviation multiplier from the expected reaction,
                // ranging from -1.0f to 1.0f.
                float deviation = (UnityEngine.Random.value - 0.5f) * 2;

                reaction += deviation * Config.MICROORGANISM_CONSUMPTION_RANDOM_VARIATION;

                amountConsumed = Mathf.Min(
                    Config.SLOT_MAX_MICROORGANISMS - petriDishSlot.MicroorganismAmount,
                    Mathf.Min(petriDishSlot.PollutantAmount, reaction));

                petriDishSlot.SetMicroorganism(
                    petriDishSlot.Microorganism,
                    petriDishSlot.MicroorganismAmount + amountConsumed);

                // Takes away a Difference amount of energy from the pollutant, regardless of whether
                // it is positive or negative. It will be negative when the Microorganism reacts to
                // the Pollutant by losing energy. The Pollutant will be not be consumed if it is
                // poisonous to the microorganism.
                petriDishSlot.SetPollutant(
                    petriDishSlot.Pollutant,
                    petriDishSlot.PollutantAmount - amountConsumed);
            }

            return amountConsumed;
        }

        private void Decay(PetriDishSlot petriDishSlot)
        {
            // TODO: Prototype microorganism-pollutant consumption method.
            petriDishSlot.SetMicroorganism(
                       petriDishSlot.Microorganism,
                       petriDishSlot.MicroorganismAmount - petriDishSlot.Microorganism.PassiveEnergyLoss);
        }
        #endregion Feeding phase methods

        #region Binary division phase methods
        /// <summary>
        /// Calculates which microorganisms should divide and how they do so.
        /// </summary>
        private void BinaryDivisionPhase()
        {
            foreach (PetriDishSlot petriDishSlot in hexMap.AllHexes)
            {
                if (petriDishSlot.MicroorganismAmount >= Config.MICROORGANISMS_TO_TRIGGER_DIVISION)
                {
                    DivideMicroorganism(petriDishSlot);
                }
            }
        }

        private void DivideMicroorganism(PetriDishSlot petriDishSlot)
        {
            // TODO: Prototype binary division procedure.

            int[] intRange = Enumerable.Range(0, 6).ToArray();

            intRange = Utils.RandomizeArray(intRange);

            for (int i = 0; i < 6; i++)
            {
                int j = intRange[i];

                Hex hex = Hex.Neighbour(petriDishSlot, j);

                PetriDishSlot neighbour = (PetriDishSlot)hexMap.GetHexAt(hex);

                if (neighbour != null)
                {
                    if (neighbour.Microorganism == null)
                    {
                        PerformDivisionToHex(petriDishSlot, neighbour);

                        return;
                    }
                }
            }

            // TODO: If no target around was found, we can count how many divisions were limited by space.
        }

        private void PerformDivisionToHex(PetriDishSlot origin, PetriDishSlot target)
        {
            target.AddMicroorganism(origin.Microorganism, origin.MicroorganismAmount / 2);

            origin.SetMicroorganism(
                origin.Microorganism,
                origin.MicroorganismAmount / 2);
        }
        #endregion Binary division phase methods

        #region UI methods
        private void BuildButtons()
        {
            List<Microorganism> microorganisms = Repository.GetMicroorganisms().ToList();
            List<Pollutant> pollutants = Repository.GetPollutants().ToList();

            foreach (Microorganism microorganism in microorganisms)
            {
                GameObject button = Instantiate(
                    ButtonPrefab,
                    ButtonPanel.transform);

                button.GetComponentInChildren<Text>().text = microorganism.name;

                Image image = button.GetComponent<Image>();
                image.color = Color.Lerp(image.color, microorganism.elementColor, Config.UI_COLOR_BLEND);

                button.GetComponent<Button>().onClick.AddListener(() => UISelectMicroorganism(microorganism.name));
            }

            foreach (Pollutant pollutant in pollutants)
            {
                GameObject button = Instantiate(
                    ButtonPrefab,
                    ButtonPanel.transform);

                button.GetComponentInChildren<Text>().text = pollutant.name;

                Image image = button.GetComponent<Image>();
                image.color = Color.Lerp(image.color, pollutant.elementColor, 0.5f);

                button.GetComponent<Button>().onClick.AddListener(() => UISelectPollutant(pollutant.name));
            }
        }

        private void SetTimeScaleText()
        {
            if (paused)
            {
                timeScaleText.text = "x0";
            }
            else
            {
                timeScaleText.text = string.Concat("x", Config.TIME_STEP_MULTIPLIERS[currentTimeStepMultiplierIndex]);
            }
        }

        private void AnimStartSimulation()
        {
            AnimButton.Play("ButtonPanelHide");
            AnimCamera.Play("CameraNarrow");
            AnimGraph.Play("GraphShow");
        }

        private void AnimResetSimulation()
        {
            AnimButton.Play("ButtonPanelShow");
            AnimCamera.Play("CameraWide");
            AnimGraph.Play("GraphHide");
        }

        public void UISelectMicroorganism(string microorganism)
        {
            selectedElement = microorganism;
            selectedElementType = ElementType.MICROORGANISM;
        }

        public void UISelectPollutant(string pollutant)
        {
            selectedElement = pollutant;
            selectedElementType = ElementType.POLLUTANT;
        }

        public void UITimeScaleIncrease()
        {
            // Increase the time scale to the next higher
            currentTimeStepMultiplierIndex++;

            // But limit it to the possible values in the array
            currentTimeStepMultiplierIndex = Mathf.Clamp(currentTimeStepMultiplierIndex, 0, Config.TIME_STEP_MULTIPLIERS.Length - 1);

            // Refresh the time scale text on screen
            SetTimeScaleText();
        }

        public void UITimeScaleDecrease()
        {
            // Decrease the time scale to the next lower
            currentTimeStepMultiplierIndex--;

            // But limit it to the possible values in the array
            currentTimeStepMultiplierIndex = Mathf.Clamp(currentTimeStepMultiplierIndex, 0, Config.TIME_STEP_MULTIPLIERS.Length - 1);

            // Refresh the time scale text on screen
            SetTimeScaleText();
        }

        public void UITimePause()
        {
            // Switch active play/pause buttons
            ButtonPlay.SetActive(true);
            ButtonPause.SetActive(false);

            // Stop the simulator from running ticks
            paused = true;

            // Update the visual time scale
            SetTimeScaleText();
        }

        public void UITimePlay()
        {
            // Show graph to analize simulation throughout its progress
            if (simulationIsStarted == false)
            {
                AnimStartSimulation();
                TimeManager.StartNewSimulation(hexMap);
            }

            // Start running again at the default time increment
            currentTimeStepMultiplierIndex = Config.TIME_STEP_DEFAULT;

            // Set the simulation to "started" state, and lock further modification to the petri dish
            simulationIsStarted = true;

            // TODO: hide microorganism/pollutant UI 

            // Switch active play/pause buttons
            ButtonPlay.SetActive(false);
            ButtonPause.SetActive(true);

            // Set simulator to run ticks
            paused = false;

            // Update the visual time scale
            SetTimeScaleText();
        }

        public void UITimeReset()
        {
            if (DialogConfirm("Você tem certeza de que deseja reiniciar a simulação? Todo o progresso não exportado será perdido."))
            {
                if (simulationIsStarted)
                {
                    AnimResetSimulation();
                }

                simulationIsStarted = false;

                UITimePause();
                hexMap.Generate();
            }
        }

        private bool DialogConfirm(string message)
        {
            // TODO: Display dialog box to user
            return true;
        }
        #endregion UI methods

        private void PROTOSaveJSON()
        {
            Tick tick = TimeManager.CurrentSimulation.ticks.Last();

            Metadata metadata = new Metadata(MetadataManager.Instance.UserId, tick);

            string json = JsonConvert.SerializeObject(metadata, Formatting.Indented);

            Debug.Log(json);
        }

        #endregion Methods
    }
}