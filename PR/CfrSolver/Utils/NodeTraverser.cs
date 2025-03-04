using System.Xml.Linq;
using CfrSolver.Model;

namespace CfrSolver.Utils
{
    public static class NodeTraverser
    {
        public static void TraverseToXml(this Node node, XElement xElement)
        {
            xElement.Add(
                new XAttribute("player", node.Pos),
                new XAttribute("action", node.Action.ToShortString()),
                new XAttribute("round", node.Round)
            );

            if (node.PayOff != 0)
            {
                xElement.Add(new XAttribute("payoff", node.PayOff));
            }

            foreach (var nodeChild in node.Children)
            {
                XElement newxElement = new XElement("Node");
                TraverseToXml(nodeChild, newxElement);
                xElement.Add(newxElement);
            }
        }

        public static void TraverseWithAction(this Node node, Action<Node, string> nodeAction, string action = "")
        {
            var shortActionName = node.Action.ToShortString();
            var actionName = action == "" ? shortActionName : action + "," + shortActionName;
            nodeAction(node, actionName);
            foreach (var nodeChild in node.Children)
            {
                TraverseWithAction(nodeChild, nodeAction, actionName);
            }
        }
    }
}