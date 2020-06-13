using CubicHex;
using EcoClean.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace EcoClean
{
    public class GameLogic : MonoBehaviour
    {
        #region Local variables

        HexMap hexMap;

        #region Time
        private float secondsToNextTick = 0;
        private int currentTimeStepMultiplierIndex = 2;
        #endregion Time

        #region Selection
        private string selectedElement = "";
        private ElementTypes selectedElementType = ElementTypes.NONE;
        private enum ElementTypes
        {
            NONE,
            MICROORGANISM,
            POLLUTANT
        }
        #endregion Selection

        #endregion Local variables

        #region Serialized variables
        public GameObject Canvas;
        public GameObject ButtonPanel;
        public GameObject Button;
        #endregion

        #region Properties
        public float BaseSecondsPerTick = 3;
        #endregion

        #region Methods

        #region Unity methods
        private void Awake()
        {
            GameObject hexesGameObject = GameObject.Find("Hexes");

            if (hexesGameObject == null)
            {
                ErrorHandler.LogError("Hexes object not found! Cannot procceed execution.");
            }

            hexMap = hexesGameObject.GetComponent<HexMap>();

            if (hexMap == null)
            {
                ErrorHandler.LogError("HexMap script not found in GameManager object!");
            }

            if (Canvas == null)
            {
                ErrorHandler.LogError("Canvas object not found! Cannot procceed execution.");
            }

            if (ButtonPanel == null)
            {
                ErrorHandler.LogError("ButtonPanel object not found! Cannot procceed execution.");
            }

            if (Button == null)
            {
                ErrorHandler.LogError("Button prefab not found! Cannot procceed execution.");
            }
        }

        private void Start()
        {
            List<Microorganism> microorganisms = Repository.GetMicroorganisms().ToList();
            List<Pollutant> pollutants = Repository.GetPollutants().ToList();

            foreach (Microorganism microorganism in microorganisms)
            {
                GameObject button = Instantiate(
                    Button,
                    Canvas.transform);

                button.GetComponentInChildren<Text>().text = microorganism.Name;

                button.GetComponent<Button>().onClick.AddListener(() => UISelectMicroorganism(microorganism.Name));
            }

            foreach (Pollutant pollutant in pollutants)
            {
                GameObject button = Instantiate(
                    Button,
                    Canvas.transform);

                button.GetComponentInChildren<Text>().text = pollutant.Name;

                button.GetComponent<Button>().onClick.AddListener(() => UISelectPollutant(pollutant.Name));
            }
        }

        private void Update()
        {
            try
            {
                HandleControls();
            }
            catch
            {
                ErrorHandler.LogError("Runtime error during the control handling step");
            }

            HandleTick();
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
            if (Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                currentTimeStepMultiplierIndex++;

                currentTimeStepMultiplierIndex = Mathf.Clamp(currentTimeStepMultiplierIndex, 0, Config.TIME_STEP_MULTIPLIERS.Length - 1);
            }
            if (Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                currentTimeStepMultiplierIndex--;

                currentTimeStepMultiplierIndex = Mathf.Clamp(currentTimeStepMultiplierIndex, 0, Config.TIME_STEP_MULTIPLIERS.Length - 1);
            }

            if (Input.GetMouseButton(0))
            {
                Hex mouseHex = Hex.FindHexAtMousePosition();

                if (mouseHex != null)
                {
                    List<Hex> petriDishSlots = null;

                    switch (selectedElementType)
                    {
                        case ElementTypes.MICROORGANISM:
                            petriDishSlots = hexMap.GetHexesWithin(new Hex(mouseHex.Q, mouseHex.R), 1).ToList();

                            foreach (Hex hex in petriDishSlots)
                            {
                                PetriDishSlot petriDishSlot = (PetriDishSlot)hex;

                                Dictionary<string, Microorganism> microorganisms = Repository.GetMicroorganismsDictionary();

                                petriDishSlot.AddMicroorganism(microorganisms[selectedElement], 0.1f);
                            }

                            break;
                        case ElementTypes.POLLUTANT:

                            petriDishSlots = hexMap.GetHexesWithin(new Hex(mouseHex.Q, mouseHex.R), 3).ToList();

                            foreach (Hex hex in petriDishSlots)
                            {
                                PetriDishSlot petriDishSlot = (PetriDishSlot)hex;

                                Dictionary<string, Pollutant> pollutants = Repository.GetPollutantsDictionary();

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
                secondsToNextTick -= Time.deltaTime * Config.TIME_STEP_MULTIPLIERS[currentTimeStepMultiplierIndex];
            }
        }
        #endregion Update() methods

        #region Feeding phase methods
        /// <summary>
        /// Allows the microorganisms to consume pollutants in their Hexes.
        /// </summary>
        private void FeedingPhase()
        {
            foreach (PetriDishSlot petriDishSlot in hexMap.AllHexes)
            {
                if (petriDishSlot.Microorganism != null)
                {
                    Feed(petriDishSlot);
                }
            }
        }

        private void Feed(PetriDishSlot petriDishSlot)
        {
            if (petriDishSlot == null)
            {
                throw new ArgumentNullException("petriDishSlot");
            }

            Decay(petriDishSlot);

            if (petriDishSlot.Microorganism != null && petriDishSlot.Pollutant != null)
            {
                float reaction = Repository.GetReaction(
                    petriDishSlot.Microorganism.Name,
                    petriDishSlot.Pollutant.Name);

                // Generates a random deviation multiplier from the expected reaction,
                // ranging from -1.0f to 1.0f.
                float deviation = (UnityEngine.Random.value - 0.5f) * 2;

                reaction += deviation * Config.MICROORGANISM_CONSUMPTION_RANDOM_VARIATION;

                float difference = Mathf.Min(
                    Config.SLOT_MAX_MICROORGANISMS - petriDishSlot.MicroorganismAmount,
                    Mathf.Min(petriDishSlot.PollutantAmount, reaction));

                petriDishSlot.SetMicroorganism(
                    petriDishSlot.Microorganism,
                    petriDishSlot.MicroorganismAmount + difference);

                // Takes away a Difference amount of energy from the pollutant, regardless of whether
                // it is positive or negative. It will be negative when the Microorganism reacts to
                // the Pollutant by losing energy. The Pollutant will be not be consumed if it is
                // poisonous to the microorganism.
                petriDishSlot.SetPollutant(
                    petriDishSlot.Pollutant,
                    petriDishSlot.PollutantAmount - Mathf.Clamp(difference, 0, Mathf.Infinity));
            }
        }

        private void Decay (PetriDishSlot petriDishSlot)
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

            // If no target around was found, find the nearest empty tile to the dividing one,
            // to find the path of least resistance to push the other microorganisms that way.
            //List<Hex> path = Pathfinder.FindHexPathByBreadth(
            //    petriDishSlot, 
            //    (hex) => 
            //    {
            //        return (hex as PetriDishSlot).MicroorganismAmount == 0;
            //    });

            //if (path != null)
            //{
            //    for (int i = 0; i < path.Count - 1; i++)
            //    {
            //        PetriDishSlot target = (PetriDishSlot)path[i];
            //        PetriDishSlot origin = (PetriDishSlot)path[i + 1];

            //        target.SetMicroorganism(origin.Microorganism, origin.MicroorganismAmount);
            //        origin.SetMicroorganism(null, 0);
            //    }
            //}
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
        public void UISelectMicroorganism(string microorganism)
        {
            selectedElement = microorganism;
            selectedElementType = ElementTypes.MICROORGANISM;
        }

        public void UISelectPollutant(string pollutant)
        {
            selectedElement = pollutant;
            selectedElementType = ElementTypes.POLLUTANT;
        }
        #endregion UI methods

        #endregion Methods
    }
}