using System.Collections.Generic;
using osu.Game.Beatmaps;
using osu.Game.Replays.Legacy;
using osu.Game.Rulesets.Replays;
using osu.Game.Rulesets.Replays.Types;

namespace osu.Game.Rulesets.Bosu.Replays
{
    public class BosuReplayFrame : ReplayFrame, IConvertibleReplayFrame
    {
        public List<BosuAction> Actions = new List<BosuAction>();
        public float PositionX;
        public float PositionY;
        public bool Dashing;

        public BosuReplayFrame()
        {
        }

        public BosuReplayFrame(double time, float? positionx = null, float? positiony = null, BosuReplayFrame lastFrame = null, bool dash = false)
            : base(time)
        {
            PositionX = positionx ?? -100;
            PositionY = positiony ?? -100;
            Dashing = dash;


            if (lastFrame != null)
            {
                if (PositionX > lastFrame.PositionX) 
                    lastFrame.Actions.Add(BosuAction.MoveRight);
                else if (PositionX < lastFrame.PositionX) 
                    lastFrame.Actions.Add(BosuAction.MoveLeft);
                
                if (PositionY < lastFrame.PositionY)
                    lastFrame.Actions.Add(BosuAction.Jump);
                else if (PositionY > lastFrame.PositionY)
                    lastFrame.Actions.Add(BosuAction.MoveDown);
                
                if (Dashing)
                    lastFrame.Actions.Add(BosuAction.Dash);
            }
        }

        public void FromLegacy(LegacyReplayFrame currentFrame, IBeatmap beatmap, ReplayFrame lastFrame = null)
        {
            PositionX = currentFrame.Position.X;
            PositionY = currentFrame.Position.Y;

            if (lastFrame is BosuReplayFrame lastBosuFrame)
            {
                if (PositionX > lastBosuFrame.PositionX)
                    lastBosuFrame.Actions.Add(BosuAction.MoveRight);
                else if (PositionX < lastBosuFrame.PositionX)
                    Actions.Add(BosuAction.MoveLeft);
                
                if (PositionY < lastBosuFrame.PositionY)
                    lastBosuFrame.Actions.Add(BosuAction.Jump);
                else if (PositionY > lastBosuFrame.PositionY)
                    Actions.Add(BosuAction.MoveDown);

                if ((PositionX - lastBosuFrame.PositionX) == 100)
                    Actions.Add(BosuAction.Dash);
                else if ((PositionX - lastBosuFrame.PositionX) == -100)
                    lastBosuFrame.Actions.Add(BosuAction.Dash);
                else if ((PositionY - lastBosuFrame.PositionY) == 100)
                    Actions.Add(BosuAction.Dash);
                else if ((PositionY - lastBosuFrame.PositionY) == -100)
                    lastBosuFrame.Actions.Add(BosuAction.Dash);
            }
        }

        public LegacyReplayFrame ToLegacy(IBeatmap beatmap)
        {
            ReplayButtonState state = ReplayButtonState.None;

            if (Actions.Contains(BosuAction.Jump)) state |= ReplayButtonState.Left1;

            return new LegacyReplayFrame(Time, PositionX, PositionY * 2, state);
        }
    }
}
