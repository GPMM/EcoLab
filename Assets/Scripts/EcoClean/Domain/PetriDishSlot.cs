using CubicHex;
using EcoClean.Domain;
using UnityEngine;

namespace EcoClean.Domain
{
    public class PetriDishSlot : Hex
    {
        #region Constructors
        public PetriDishSlot(int q, int r, GameObject gameObject) : base(q, r)
        {
            slotGameObject = gameObject;
            pollutantMeshRenderer = slotGameObject.transform.GetChild(0).GetComponent<MeshRenderer>();
            microorganismMeshRenderer = slotGameObject.transform.GetChild(1).GetComponent<MeshRenderer>();
        }
        #endregion

        #region Local variables
        private readonly MeshRenderer pollutantMeshRenderer;
        private readonly MeshRenderer microorganismMeshRenderer;
        private readonly GameObject slotGameObject;

        private float microorganismAmount = 0;
        private float pollutantAmount = 0;
        #endregion

        #region Properties
        public Microorganism Microorganism { get; private set; }
        
        public Pollutant Pollutant { get; private set; }

        public float MicroorganismAmount
        {
            get
            {
                return microorganismAmount;
            }
            private set
            {
                microorganismAmount = Mathf.Clamp(value, 0, Config.SLOT_MAX_MICROORGANISMS);
            }
        }

        public float PollutantAmount
        {
            get
            {
                return pollutantAmount;
            }
            private set
            {
                pollutantAmount = Mathf.Clamp(value, 0, Config.SLOT_MAX_REAGENTS);
            }
        }
        #endregion

        #region Methods
        public void AddMicroorganism(Microorganism microorganism, float amount)
        {
            if (microorganism == this.Microorganism)
            {
                MicroorganismAmount += amount;
            }
            else if (Microorganism is null)
            {
                Microorganism = microorganism;
                MicroorganismAmount = amount;
            }

            SetMicroorganismColor(Microorganism);

            // TODO: This is wrong. Make these functions the same
            if (microorganismAmount <= 0)
            {
                Microorganism = null;
            }
        }

        public void SetMicroorganism(Microorganism microorganism, float amount)
        {
            Microorganism = microorganism;
            MicroorganismAmount = amount;

            SetMicroorganismColor(Microorganism);

            // TODO: Move this entire method indo AddMicroorganism
            if (MicroorganismAmount <= 0)
            {
                Microorganism = null;
            }
        }

        public void AddPollutant(Pollutant pollutant, float amount)
        {
            if (pollutant == Pollutant)
            {
                PollutantAmount += amount;
            }
            else if (Pollutant is null)
            {
                Pollutant = pollutant;
                PollutantAmount = amount;
            }

            SetPollutantColor(Pollutant);

            // TODO: This is wrong. Make these functions the same
            if (PollutantAmount <= 0)
            {
                Pollutant = null;
            }
        }

        public void SetPollutant(Pollutant pollutant, float amount)
        {
            Pollutant = pollutant;
            PollutantAmount = amount;

            SetPollutantColor(Pollutant);

            // TODO: Move this entire method indo AddPollutant
            if (PollutantAmount <= 0)
            {
                Pollutant = null;
            }
        }

        private void SetMicroorganismColor(Microorganism microorganism)
        {
            Color color = new Color(0, 0, 0, 0);

            if (microorganism != null)
            {
                float transparency = MicroorganismAmount / Config.SLOT_MAX_MICROORGANISMS;

                color = new Color(
                    microorganism.elementColor.r,
                    microorganism.elementColor.g,
                    microorganism.elementColor.b,
                    transparency);
            }

            microorganismMeshRenderer.material.color = color;
        }

        private void SetPollutantColor(Pollutant pollutant)
        {
            Color color = new Color(0, 0, 0, 0);

            if (pollutant != null)
            {
                float transparency = PollutantAmount / Config.SLOT_MAX_REAGENTS;

                color = new Color(
                    pollutant.elementColor.r,
                    pollutant.elementColor.g,
                    pollutant.elementColor.b,
                    transparency);
            }

            pollutantMeshRenderer.material.color = color;
        }
        #endregion
    }
}