using EcoClean.Domain;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace EcoClean
{
    public class GraphManager : MonoBehaviour
    {
        #region Properties
        public static GraphManager Instance { get; private set; }
        #endregion Properties

        #region Local variables]
        [SerializeField]
        private Sprite nodeSprite;
        [SerializeField]
        private float margin;
        [SerializeField]
        private float graphHorizontalSeparation = 50f;

        private RectTransform nodeContainer;
        private float graphHeight;
        private float nodeContainerStartingWidth;

        private Dictionary<Element, GameObject> lastCircleDict = new Dictionary<Element, GameObject>();
        #endregion Local variables

        #region Methods
        private void Awake()
        {
            ErrorHandler.AssertNullQuit(Instance, "There is more than one GraphController script running.");
            Instance = this;
            
            ErrorHandler.AssertNullQuit(nodeSprite);

            nodeContainer = transform.GetComponent<RectTransform>();
            graphHeight = nodeContainer.sizeDelta.y;
            nodeContainerStartingWidth = nodeContainer.sizeDelta.x;
            ErrorHandler.AssertNullQuit(nodeContainer);
        }

        private GameObject CreateCircle(Vector2 position)
        {
            return CreateCircle("Element", position, Color.white);
        }

        private GameObject CreateCircle(string name, Vector2 anchoredPosition, Color color)
        {
            float diameter = Config.UI_GRAPH_NODE_DIAMETER;

            // Creating the edge with an Image component
            GameObject circle = new GameObject("Node_" + name, typeof(Image));
            circle.transform.SetParent(nodeContainer, false);

            // Setting the edge's color
            Image image = circle.GetComponent<Image>();
            image.sprite = nodeSprite;
            image.color = color;

            // Setting position and rotation
            RectTransform rectTransform = circle.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = new Vector2(diameter, diameter);
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);

            return circle;
        }

        private GameObject CreateEdge(Vector2 pointA, Vector2 pointB)
        {
            return CreateEdge("Element", pointA, pointB, Color.white);
        }
        
        private GameObject CreateEdge(string name, Vector2 pointA, Vector2 pointB, Color color)
        {
            // Creating the edge with an Image component
            GameObject edge = new GameObject("Edge_" + name, typeof(Image));
            edge.transform.SetParent(nodeContainer, false);

            // Setting the edge's color
            Image image = edge.GetComponent<Image>();
            image.color = new Color(color.r, color.g, color.b, Config.UI_GRAPH_EDGE_ALPHA);

            // Calculating values
            float distance = Vector2.Distance(pointA, pointB);
            Vector2 vector = (pointB - pointA).normalized;
            float edgeRotation = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;

            // Setting position and rotation
            RectTransform rectTransform = edge.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);
            rectTransform.sizeDelta = new Vector2(distance, Config.UI_GRAPH_EDGE_THICKNESS);
            rectTransform.anchoredPosition = pointA + (vector * distance / 2);
            rectTransform.localEulerAngles = new Vector3(0, 0, edgeRotation);

            return edge;
        }

        public void RenderGraph(Element element, float value, int day, float maxValue)
        {
            day /= GameManager.Instance.graphUpdateIntervalDays;

            lastCircleDict.TryGetValue(element, out GameObject lastCircle);

            Vector2 position = new Vector2(
                day * graphHorizontalSeparation + margin,
                (value / maxValue) * graphHeight);

            GameObject circle = CreateCircle(element.Name, position, element.ElementColor);

            if (!(lastCircle is null))
            {
                CreateEdge(
                    element.Name,
                    lastCircle.GetComponent<RectTransform>().anchoredPosition,
                    circle.GetComponent<RectTransform>().anchoredPosition,
                    element.ElementColor
                    );
            }

            UpdateGraphWidth(day);

            lastCircleDict[element] = circle;
        }

        public void ResetGraph()
        {
            lastCircleDict.Clear();

            foreach (Transform child in nodeContainer.transform)
            {
                Destroy(child.gameObject);
            }
        }

        private void UpdateGraphWidth(int day)
        {
            nodeContainer.sizeDelta = new Vector2(
                    Mathf.Max(
                        (margin * 2) + (graphHorizontalSeparation * (day)),
                        nodeContainerStartingWidth),
                    nodeContainer.sizeDelta.y);
        }
        #endregion Properties
    }
}
