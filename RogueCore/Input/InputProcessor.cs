namespace RogueCore.Input;

public class InputProcessor
{
    
    public void Move(Actions expression)
    {
        int directionX = 0;
        int directionY = 0;
        switch (expression)
        {
            case Actions.MoveDown:
                directionY = 1;
                break;
            case Actions.MoveUp:
                directionY = -1;
                break;
            case Actions.MoveLeft:
                directionX = -1;
                break;
            case Actions.MoveRight:
                directionX = 1;
                break;
            default:
                break;
        }
        
        
    }
}