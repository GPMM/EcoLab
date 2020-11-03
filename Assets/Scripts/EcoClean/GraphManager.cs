using EcoClean.Domain;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EcoClean
{
    public class GraphManager : MonoBehaviour
    {
        #region Properties
        public static GraphManager Instance { get; private set; }
        #endregion Properties

        #region Local variables
        private float graphHeight;
        private float nodeContainerStartingWidth;

        private readonly Dictionary<Element, GameObject> lastNodeDict = new Dictionary<Element, GameObject>();
        private readonly List<int> dayLabelsInGraph = new List<int>();
        private readonly List<Element> elementsWithLegend = new List<Element>();
        #endregion Local variables

        #region Serialized variables
        [SerializeField]
        private RectTransform nodeContainer;
        [SerializeField]
        private RectTransform verticalLabelContainer;
        [SerializeField]
        private RectTransform horizontalLabelContainer;
        [SerializeField]
        private RectTransform legendContainer;
        [SerializeField]
        private Scrollbar scrollbar;
        [SerializeField]
        private GameObject labelPrefab;
        [SerializeField]
        private GameObject legendPrefab;
        [SerializeField]
        private Sprite nodeImage;
        [SerializeField]
        private Sprite separatorImage;
        [SerializeField]
        private Color separatorColor = Color.grey;
        [SerializeField]
        private float horizontalMargin;
        [SerializeField]
        private float verticalMargin;
        [SerializeField]
        private float graphHorizontalSpacing = 50f;
        [SerializeField]
        private int verticalSeparatorAmount = 5;
        #endregion Serialized variables

        #region Methods
        private void Awake()
        {
            ErrorHandler.AssertExists(Instance, "There is more than one GraphController script running.");
            Instance = this;

            ErrorHandler.AssertNull(nodeImage);
            ErrorHandler.AssertNull(separatorImage);

            ErrorHandler.AssertNull(scrollbar);
            ErrorHandler.AssertNull(verticalLabelContainer);
            ErrorHandler.AssertNull(horizontalLabelContainer);
            ErrorHandler.AssertNull(legendContainer);

            ErrorHandler.AssertNull(nodeContainer);
            SetupNodeContainer();

            ErrorHandler.AssertNull(legendPrefab);

            ErrorHandler.AssertNull(labelPrefab);
            SetupHorizontalLabels();
        }

        private void SetupNodeContainer()
        {
            graphHeight = nodeContainer.sizeDelta.y;
            nodeContainerStartingWidth = nodeContainer.sizeDelta.x;
        }

        private void SetupHorizontalLabels()
        {
            if (verticalSeparatorAmount > 1)
            {
                for (int i = 0; i < verticalSeparatorAmount; i++)
                {
                    int percentage = i * 100 / (verticalSeparatorAmount - 1);
                
                    CreateHorizontalLabel(percentage, MapValueToGraphVerticalCoordinates(i, verticalSeparatorAmount - 1) + 40f);
                }
            }
        }

        private void SetupHorizontalSeparators()
        {
            if (verticalSeparatorAmount > 1)
            {
                for (int i = 0; i < verticalSeparatorAmount; i++)
                {
                    CreateHorizontalSeparator(MapValueToGraphVerticalCoordinates(i, verticalSeparatorAmount - 1));
                }
            }
        }

        #region Element creation
        private GameObject CreateNode(string name, Vector2 anchoredPosition, Color color)
        {
            float diameter = Config.UI_GRAPH_NODE_DIAMETER;

            // Creating the edge with an Image component
            GameObject node = new GameObject("Node_" + name, typeof(Image));
            node.transform.SetParent(nodeContainer, false);

            // Setting the edge's color
            Image image = node.GetComponent<Image>();
            image.sprite = nodeImage;
            image.color = color;

            // Setting position and rotation
            RectTransform rectTransform = node.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = new Vector2(diameter, diameter);
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);

            return node;
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

        private GameObject CreateHorizontalLabel(int number, float position)
        {
            Vector2 positionVector = new Vector2(
                -30f,
                position);

            CreateHorizontalSeparator(position);

            return CreateLabel(number.ToString() + "%", positionVector, horizontalLabelContainer);
        }

        private GameObject CreateVerticalLabel(int day, float position)
        {
            Vector2 positionVector = new Vector2(
                position,
                -30f);

            return CreateLabel(day.ToString(), positionVector, verticalLabelContainer);
        }

        private GameObject CreateLabel(string text, Vector2 position, RectTransform parent)
        {
            GameObject label = Instantiate(labelPrefab, parent);

            RectTransform rectTransform = label.GetComponent<RectTransform>();
            ErrorHandler.AssertNull(rectTransform);

            Text textComponent = label.GetComponent<Text>();
            ErrorHandler.AssertNull(textComponent);

            rectTransform.anchoredPosition = position;

            textComponent.text = text;

            return label;
        }

        private GameObject CreateHorizontalSeparator(float position)
        {
            // Creating the edge with an Image component
            GameObject separator = new GameObject("Separator", typeof(Image));
            separator.transform.SetParent(nodeContainer, false);

            // Setting the edge's color
            Image image = separator.GetComponent<Image>();
            image.sprite = separatorImage;
            image.color = separatorColor;

            // Setting position and rotation
            RectTransform rectTransform = separator.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0, position);
            rectTransform.sizeDelta = new Vector2(1, Config.UI_GRAPH_SEPARATOR_THICKNESS);
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 0);

            return separator;
        }

        private GameObject CreateLegend(Element element)
        {
            GameObject gameObject = Instantiate(legendPrefab, legendContainer);

            Image image = gameObject.GetComponent<Image>();
            image.color = element.elementColor;

            Text text = gameObject.GetComponentInChildren<Text>();
            text.text = element.name;

            return gameObject;
        }

        #endregion Element creation

        private float MapValueToGraphVerticalCoordinates(float value, float maxValue)
        {
            return (value / maxValue * (graphHeight - (verticalMargin * 2))) + verticalMargin;
        }

        public void RenderGraph(Element element, float value, int day, float maxValue)
        {
            // Find the X index of this node based on the current day simulated.
            // In the case of UpdateIntervals other than 1, this ensures all nodes are placed
            // at exactly the value of graphHorizontalSeparation apart from each other horizontally.
            int nodeIndex = day / GameManager.Instance.graphUpdateIntervalDays;

            // Find the last node of this element type, if there is any.
            lastNodeDict.TryGetValue(element, out GameObject lastNode);

            // If this element has no legend in the graph, create a new one.
            if (!elementsWithLegend.Contains(element))
            {
                elementsWithLegend.Add(element);

                CreateLegend(element);
            }

            // Calculating the new node's position in the graph.
            Vector2 position = new Vector2(
                nodeIndex * graphHorizontalSpacing + horizontalMargin,
                MapValueToGraphVerticalCoordinates(value, maxValue));

            // Instantiating the new node.
            GameObject node = CreateNode(element.name, position, element.elementColor);

            // If there was already a node with this element, create an edge between it and the newly created node.
            if (!(lastNode is null))
            {
                CreateEdge(
                    element.name,
                    lastNode.GetComponent<RectTransform>().anchoredPosition,
                    node.GetComponent<RectTransform>().anchoredPosition,
                    element.elementColor
                    );
            }

            // If there is no day label regarding this day, instantiate a new one.
            if (!dayLabelsInGraph.Contains(day))
            {
                CreateVerticalLabel(day, position.x);

                dayLabelsInGraph.Add(day);
            }

            // Refresh the container's width to allow for all new nodes to be shown.
            UpdateGraphWidth(nodeIndex);

            // Update the last created node for the purpose of creating edges.
            lastNodeDict[element] = node;
        }

        // Resets all necessary properties to enable reuse of the graph.
        public void ResetGraph()
        {
            lastNodeDict.Clear();
            dayLabelsInGraph.Clear();
            elementsWithLegend.Clear();

            foreach (Transform child in nodeContainer.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (Transform child in legendContainer.transform)
            {
                Destroy(child.gameObject);
            }

            SetupHorizontalSeparators();
        }

        private void UpdateGraphWidth(int day)
        {
            nodeContainer.sizeDelta = new Vector2(
                    Mathf.Max(
                        (horizontalMargin * 2) + (graphHorizontalSpacing * (day)),
                        nodeContainerStartingWidth),
                    nodeContainer.sizeDelta.y);

            scrollbar.value = 1;
        }
        #endregion Properties
    }
}
