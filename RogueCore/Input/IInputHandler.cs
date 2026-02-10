namespace RogueCore.Input;

public interface IInputHandler
{
    Actions HandleInput(object platformInputEventArgs);

    //For handling continuous movement
    bool IsActionPressed(Actions action);
}