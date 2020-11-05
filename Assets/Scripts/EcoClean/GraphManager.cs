﻿using EcoClean.Domain;
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

        #region Local variables
        private float graphHeight;
        private float nodeContainerStartingWidth;

        private readonly Dictionary<Element, GameObject> lastNodeDict = new Dictionary<Element, GameObject>();
        private readonly List<int> dayLabelsInGraph = new List<int>();
        #endregion Local variables

        #region Serialized variables
        [SerializeField]
        private Sprite nodeSprite;
        [SerializeField]
        private float horizontalMargin;
        [SerializeField]
        private float verticalMargin;
        [SerializeField]
        private float graphHorizontalSpacing = 50f;
        [SerializeField]
        private RectTransform verticalLabelContainer;
        [SerializeField]
        private RectTransform horizontalLabelContainer;
        [SerializeField]
        private RectTransform nodeContainer;
        [SerializeField]
        private RectTransform labelPrefab;
        [SerializeField]
        private RectTransform linePrefab;
        #endregion Serialized variables

        #region Methods
        private void Awake()
        {
            ErrorHandler.AssertNullQuit(Instance, "There is more than one GraphController script running.");
            Instance = this;
            
            ErrorHandler.AssertNullQuit(nodeSprite);

            ErrorHandler.AssertNullQuit(verticalLabelContainer);
            ErrorHandler.AssertNullQuit(horizontalLabelContainer);

            ErrorHandler.AssertNullQuit(nodeContainer);
            SetupNodeContainer();

            ErrorHandler.AssertNullQuit(labelPrefab);
            ErrorHandler.AssertNullQuit(linePrefab);
        }

        private void SetupNodeContainer()
        {
            graphHeight = nodeContainer.sizeDelta.y;
            nodeContainerStartingWidth = nodeContainer.sizeDelta.x;
        }

        private GameObject CreateNode(string name, Vector2 anchoredPosition, Color color)
        {
            float diameter = Config.UI_GRAPH_NODE_DIAMETER;

            // Creating the edge with an Image component
            GameObject node = new GameObject("Node_" + name, typeof(Image));
            node.transform.SetParent(nodeContainer, false);

            // Setting the edge's color
            Image image = node.GetComponent<Image>();
            image.sprite = nodeSprite;
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

        public void RenderGraph(Element element, float value, int day, float maxValue)
        {
            // Find the X index of this node based on the current day simulated.
            // In the case of UpdateIntervals other than 1, this ensures all nodes are placed
            // at exactly the value of graphHorizontalSeparation apart from each other horizontally.
            int nodeIndex = day / GameManager.Instance.graphUpdateIntervalDays;

            // Find the last node of this element type, if there is any.
            lastNodeDict.TryGetValue(element, out GameObject lastNode);

            // Calculating the new node's position in the graph.
            Vector2 position = new Vector2(
                nodeIndex * graphHorizontalSpacing + horizontalMargin,
                (value / maxValue * (graphHeight - (verticalMargin * 2))) + verticalMargin);

            // Instantiating the new node.
            GameObject node = CreateNode(element.Name, position, element.ElementColor);

            // If there was already a node with this element, create an edge between it and the newly created node.
            if (!(lastNode is null))
            {
                CreateEdge(
                    element.Name,
                    lastNode.GetComponent<RectTransform>().anchoredPosition,
                    node.GetComponent<RectTransform>().anchoredPosition,
                    element.ElementColor
                    );
            }

            // If there is no day label regarding this day, instantiate a new one.
            if (!dayLabelsInGraph.Contains(day))
            {

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

            foreach (Transform child in nodeContainer.transform)
            {
                Destroy(child.gameObject);
            }
        }

        private void UpdateGraphWidth(int day)
        {
            nodeContainer.sizeDelta = new Vector2(
                    Mathf.Max(
                        (horizontalMargin * 2) + (graphHorizontalSpacing * (day)),
                        nodeContainerStartingWidth),
                    nodeContainer.sizeDelta.y);
        }
        #endregion Properties
    }
}
