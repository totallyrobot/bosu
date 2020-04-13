using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics;
using osuTK;
using System;
using osuTK.Graphics;
using osu.Framework.Input.Bindings;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Textures;
using osu.Game.Rulesets.Bosu.Configuration;
using osu.Framework.Bindables;
using osu.Framework.Audio.Sample;
using osu.Framework.Audio.Track;
using osu.Game.Rulesets.UI;
using osu.Game.Rulesets.Bosu.Replays;

namespace osu.Game.Rulesets.Bosu.UI.Objects
{
    public class BosuPlayer : CompositeDrawable, IKeyBindingHandler<BosuAction>
    {
        private const double base_speed = 1.0 / 2500;

        [Resolved]
        private TextureStore textures { get; set; }

        private readonly Bindable<PlayerModel> playerModel = new Bindable<PlayerModel>();

        private int horizontalDirection;
        private int verticalDirection;
        
        private float dashDistance = 0.1f;

        public bool isDashing;
        public readonly Container Player;
        private readonly Sprite drawablePlayer;

        public BosuPlayer()
        {
            RelativeSizeAxes = Axes.Both;
            AddInternal(Player = new Container
            {
                Origin = Anchor.Centre,
                RelativePositionAxes = Axes.Both,
                Position = new Vector2(0.5f, 1),
                Size = new Vector2(15),
                Child = drawablePlayer = new Sprite
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both
                }
            });
        }

        [BackgroundDependencyLoader]
        private void load(BosuRulesetConfigManager config, ISampleStore samples)
        {
            config.BindWith(BosuRulesetSetting.PlayerModel, playerModel);

        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            playerModel.BindValueChanged(model =>
            {
              
                        drawablePlayer.Texture = textures.Get("Player/cyan");
                        return;
            
            }, true);

        }

        public Vector2 PlayerPositionInPlayfieldSpace() => Player.Position * BosuPlayfield.BASE_SIZE;

        public Vector2 PlayerDrawSize() => Player.DrawSize;

        public void PlayMissAnimation()
        {
            drawablePlayer.FadeColour(Color4.Red).Then().FadeColour(Color4.White, 1000, Easing.OutQuint);
        }

        public bool OnPressed(BosuAction action)
        {
            switch (action)
            {
                case BosuAction.MoveLeft:
                    horizontalDirection--;
                    return true;

                case BosuAction.MoveRight:
                    horizontalDirection++;
                    return true;

                case BosuAction.Jump:
                    verticalDirection--;
                    return true;

                case BosuAction.MoveDown:
                    verticalDirection++;
                    return true;

                case BosuAction.Dash:
                    isDashing = true;
                    return true;
            }

            return false;
        }

        public void OnReleased(BosuAction action)
        {
            switch (action)
            {
                case BosuAction.MoveLeft:
                    horizontalDirection++;
                    return;

                case BosuAction.MoveRight:
                    horizontalDirection--;
                    return;

                case BosuAction.Jump:
                    verticalDirection++;
                    return;

                case BosuAction.MoveDown:
                    verticalDirection--;
                    return;

                case BosuAction.Dash:
                    isDashing = false;
                    return;
            }
        }

        protected override void Update()
        {
            updateReplayState();

            base.Update();

            if (horizontalDirection != 0) {
                var position = Math.Clamp(Player.X + Math.Sign(horizontalDirection) * Clock.ElapsedFrameTime * base_speed, 0, 1);

                Player.Scale = new Vector2(Math.Abs(Scale.X) * (horizontalDirection > 0 ? 1 : -1), Player.Scale.Y);

                if (position == Player.X)
                    return;

                Player.X = (float)position;
            }

            if (verticalDirection != 0) {
                var position = Math.Clamp(Player.Y + Math.Sign(verticalDirection) * Clock.ElapsedFrameTime * base_speed, 0, 1);

                Player.Scale = new Vector2(Math.Abs(Scale.Y) * (verticalDirection > 0 ? 1 : -1), Player.Scale.X);

                if (position == Player.Y)
                    return;

                Player.Y = (float)position;
            }

        if (isDashing) {
            if (verticalDirection == 1) {
                Player.Y += dashDistance;
            }
            if (verticalDirection == -1) {
                Player.Y -= dashDistance;
            }
            if (horizontalDirection == 1) {
                Player.X += dashDistance;
            }
            if (horizontalDirection == -1) {
                Player.X -= dashDistance;
            }
            isDashing = false;
            return;
        }

        }
        private void updateReplayState()
        {
            var state = (GetContainingInputManager().CurrentState as RulesetInputManagerInputState<BosuAction>)?.LastReplayState as BosuFramedReplayInputHandler.BosuReplayState ?? null;

            if (state != null)
            {
                Player.X = state.Position.Value / BosuPlayfield.BASE_SIZE.X;
            }
        }
    }
}
