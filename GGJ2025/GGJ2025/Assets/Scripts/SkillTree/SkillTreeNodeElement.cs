using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;

public class SkillTreeNodeElement : Node
{
    public string GUID;
    public BuffData buffData;

    public SkillTreeNodeElement()
    {
        this.pickingMode = PickingMode.Position;

        // Create an input port for prerequisites
        Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(string));
        inputPort.portName = "Prerequisite";
        inputContainer.Add(inputPort);

        // Create an output port for connecting to subsequent skills
        Port outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(string));
        outputPort.portName = "Next Skill";
        outputContainer.Add(outputPort);

        // Add a text field for editing the node's name
        TextField nodeNameField = new TextField("Skill Name");
        nodeNameField.RegisterValueChangedCallback(evt => title = evt.newValue);
        mainContainer.Add(nodeNameField);

        // Add an ObjectField to select a BuffData asset
        ObjectField buffField = new ObjectField("Buff Data")
        {
            objectType = typeof(BuffData),
            value = null
        };
        buffField.RegisterValueChangedCallback(evt =>
        {
            buffData = evt.newValue as BuffData;
        });
        mainContainer.Add(buffField);

        // Refresh the UI to reflect changes
        RefreshExpandedState();
        RefreshPorts();
    }
}
