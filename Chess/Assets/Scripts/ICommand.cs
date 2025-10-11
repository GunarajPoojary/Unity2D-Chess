public interface ICommand
{
    void Execute();
    void Undo();
    void Redo();
}
public class MovePieceCommand : ICommand
{
    public void Execute()
    {

    }

    public void Redo()
    {

    }

    public void Undo()
    {

    }
}