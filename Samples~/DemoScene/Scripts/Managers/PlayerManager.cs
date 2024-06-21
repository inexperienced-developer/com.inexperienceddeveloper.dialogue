
namespace InexperiencedDeveloper.Dialogue.Samples
{
    public class PlayerManager : BaseManager
    {
        public Player Player { get; private set; }

        public override void Init()
        {
            // TODO: Fix this for now let's just find the player
            Player = FindObjectOfType<Player>();
        }

        public override void CleanUp()
        {
        }

        public override void Restart()
        {
        }
    }
}

