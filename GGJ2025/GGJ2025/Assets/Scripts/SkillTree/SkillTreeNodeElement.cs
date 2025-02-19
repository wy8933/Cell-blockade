using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class SkillTreeNodeElement : Node
{
    public string GUID;
    public BuffData buffData;
    public int cost;

    public TextField skillNameField;
    public IntegerField costField;
    public ObjectField buffObjectField;

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

        // Create a text field for editing the node's name
        skillNameField = new TextField("Skill Name");
        skillNameField.RegisterValueChangedCallback(evt => { title = evt.newValue; });
        mainContainer.Add(skillNameField);

        // Create an IntegerField for the cost value
        costField = new IntegerField("Cost");
        costField.RegisterValueChangedCallback(evt => { cost = evt.newValue; });
        mainContainer.Add(costField);

        // Create an ObjectField for selecting a BuffData asset
        buffObjectField = new ObjectField("Buff Data")
        {
            objectType = typeof(BuffData),
            value = null
        };
        buffObjectField.RegisterValueChangedCallback(evt =>
        {
            buffData = evt.newValue as BuffData;
        });
        mainContainer.Add(buffObjectField);

        RefreshExpandedState();
        RefreshPorts();
    }
}