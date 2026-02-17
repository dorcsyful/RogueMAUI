using RogueCore.Models;

namespace RogueCore.Input;

public class InputProcessor
{
    private World _world;

    public InputProcessor(World world)
    {
        _world = world;
    }

    public void ProcessAction(Actions action)
    {
        switch (action)
        {
            case Actions.MoveUp:
            case Actions.MoveDown:
            case Actions.MoveLeft:
            case Actions.MoveRight:
                Move(action);
                break;
            case Actions.Interact:
                Interact();
                break;
            case Actions.Attack:
                Attack();
                break;
            case Actions.Pause:
                Pause();
                break;
        }
    }

    private void Move(Actions direction)
    {
        int directionX = 0;
        int directionY = 0;

        switch (direction)
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
        }

        _world.Player.Move(directionX, directionY);
    }

    private void Interact()
    {
        // TODO: Implement interact logic (open doors, pick up items, etc.)
    }

    private void Attack()
    {
        // TODO: Implement attack logic
    }

    private void Pause()
    {
        // TODO: Implement pause logic
    }
}