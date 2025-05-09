// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Screens;
using osu.Framework.Testing;
using osu.Game.Database;
using osu.Game.Overlays;
using osu.Game.Overlays.Mods;
using osu.Game.Overlays.Toolbar;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Osu.Mods;
using osu.Game.Screens;
using osu.Game.Screens.Footer;
using osu.Game.Screens.Menu;
using osu.Game.Screens.SelectV2;
using osuTK.Input;

namespace osu.Game.Tests.Visual.SongSelectV2
{
    public partial class TestSceneSongSelect : ScreenTestScene
    {
        [Cached]
        private readonly ScreenFooter screenFooter;

        [Cached]
        private readonly OsuLogo logo;

        [Cached(typeof(INotificationOverlay))]
        private readonly INotificationOverlay notificationOverlay = new NotificationOverlay();

        protected override bool UseOnlineAPI => true;

        public TestSceneSongSelect()
        {
            Children = new Drawable[]
            {
                new PopoverContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Children = new Drawable[]
                    {
                        new Toolbar
                        {
                            State = { Value = Visibility.Visible },
                        },
                        screenFooter = new ScreenFooter
                        {
                            OnBack = () => Stack.CurrentScreen.Exit(),
                        },
                        logo = new OsuLogo
                        {
                            Alpha = 0f,
                        },
                    },
                },
            };

            Stack.Padding = new MarginPadding { Top = Toolbar.HEIGHT };
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            RealmDetachedBeatmapStore beatmapStore;

            Dependencies.CacheAs<BeatmapStore>(beatmapStore = new RealmDetachedBeatmapStore());
            Add(beatmapStore);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            Stack.ScreenPushed += updateFooter;
            Stack.ScreenExited += updateFooter;
        }

        #region Footer

        [Test]
        public void TestMods()
        {
            loadSongSelect();

            AddStep("one mod", () => SelectedMods.Value = new List<Mod> { new OsuModHidden() });
            AddStep("two mods", () => SelectedMods.Value = new List<Mod> { new OsuModHidden(), new OsuModHardRock() });
            AddStep("three mods", () => SelectedMods.Value = new List<Mod> { new OsuModHidden(), new OsuModHardRock(), new OsuModDoubleTime() });
            AddStep("four mods", () => SelectedMods.Value = new List<Mod> { new OsuModHidden(), new OsuModHardRock(), new OsuModDoubleTime(), new OsuModClassic() });
            AddStep("five mods", () => SelectedMods.Value = new List<Mod> { new OsuModHidden(), new OsuModHardRock(), new OsuModDoubleTime(), new OsuModClassic(), new OsuModDifficultyAdjust() });

            AddStep("modified", () => SelectedMods.Value = new List<Mod> { new OsuModDoubleTime { SpeedChange = { Value = 1.2 } } });
            AddStep("modified + one", () => SelectedMods.Value = new List<Mod> { new OsuModHidden(), new OsuModDoubleTime { SpeedChange = { Value = 1.2 } } });
            AddStep("modified + two", () => SelectedMods.Value = new List<Mod> { new OsuModHidden(), new OsuModHardRock(), new OsuModDoubleTime { SpeedChange = { Value = 1.2 } } });
            AddStep("modified + three",
                () => SelectedMods.Value = new List<Mod> { new OsuModHidden(), new OsuModHardRock(), new OsuModClassic(), new OsuModDoubleTime { SpeedChange = { Value = 1.2 } } });
            AddStep("modified + four",
                () => SelectedMods.Value = new List<Mod>
                    { new OsuModHidden(), new OsuModHardRock(), new OsuModClassic(), new OsuModDifficultyAdjust(), new OsuModDoubleTime { SpeedChange = { Value = 1.2 } } });

            AddStep("clear mods", () => SelectedMods.Value = Array.Empty<Mod>());
            AddWaitStep("wait", 3);
            AddStep("one mod", () => SelectedMods.Value = new List<Mod> { new OsuModHidden() });

            AddStep("clear mods", () => SelectedMods.Value = Array.Empty<Mod>());
            AddWaitStep("wait", 3);
            AddStep("five mods", () => SelectedMods.Value = new List<Mod> { new OsuModHidden(), new OsuModHardRock(), new OsuModDoubleTime(), new OsuModClassic(), new OsuModDifficultyAdjust() });
        }

        [Test]
        public void TestShowOptions()
        {
            loadSongSelect();

            AddStep("enable options", () =>
            {
                var optionsButton = this.ChildrenOfType<ScreenFooterButton>().Last();

                optionsButton.Enabled.Value = true;
                optionsButton.TriggerClick();
            });
        }

        [Test]
        public void TestState()
        {
            loadSongSelect();

            AddToggleStep("set options enabled state", state => this.ChildrenOfType<ScreenFooterButton>().Last().Enabled.Value = state);
        }

        // add these test cases when functionality is implemented.
        // [Test]
        // public void TestFooterRandom()
        // {
        //     loadSongSelect();
        //
        //     AddStep("press F2", () => InputManager.Key(Key.F2));
        //     AddAssert("next random invoked", () => nextRandomCalled && !previousRandomCalled);
        // }
        //
        // [Test]
        // public void TestFooterRandomViaMouse()
        // {
        //     loadSongSelect();
        //
        //     AddStep("click button", () =>
        //     {
        //         InputManager.MoveMouseTo(randomButton);
        //         InputManager.Click(MouseButton.Left);
        //     });
        //     AddAssert("next random invoked", () => nextRandomCalled && !previousRandomCalled);
        // }
        //
        // [Test]
        // public void TestFooterRewind()
        // {
        //     loadSongSelect();
        //
        //     AddStep("press Shift+F2", () =>
        //     {
        //         InputManager.PressKey(Key.LShift);
        //         InputManager.PressKey(Key.F2);
        //         InputManager.ReleaseKey(Key.F2);
        //         InputManager.ReleaseKey(Key.LShift);
        //     });
        //     AddAssert("previous random invoked", () => previousRandomCalled && !nextRandomCalled);
        // }
        //
        // [Test]
        // public void TestFooterRewindViaShiftMouseLeft()
        // {
        //     loadSongSelect();
        //
        //     AddStep("shift + click button", () =>
        //     {
        //         InputManager.PressKey(Key.LShift);
        //         InputManager.MoveMouseTo(randomButton);
        //         InputManager.Click(MouseButton.Left);
        //         InputManager.ReleaseKey(Key.LShift);
        //     });
        //     AddAssert("previous random invoked", () => previousRandomCalled && !nextRandomCalled);
        // }
        //
        // [Test]
        // public void TestFooterRewindViaMouseRight()
        // {
        //     loadSongSelect();
        //
        //     AddStep("right click button", () =>
        //     {
        //         InputManager.MoveMouseTo(randomButton);
        //         InputManager.Click(MouseButton.Right);
        //     });
        //     AddAssert("previous random invoked", () => previousRandomCalled && !nextRandomCalled);
        // }

        [Test]
        public void TestOverlayPresent()
        {
            loadSongSelect();

            AddStep("Press F1", () =>
            {
                InputManager.MoveMouseTo(this.ChildrenOfType<FooterButtonMods>().Single());
                InputManager.Click(MouseButton.Left);
            });
            AddAssert("Overlay visible", () => this.ChildrenOfType<ModSelectOverlay>().Single().State.Value == Visibility.Visible);
            AddStep("Hide", () => this.ChildrenOfType<ModSelectOverlay>().Single().Hide());
        }

        #endregion

        private void loadSongSelect()
        {
            AddStep("load screen", () => Stack.Push(new SoloSongSelect()));
            AddUntilStep("wait for load", () => Stack.CurrentScreen is Screens.SelectV2.SongSelect songSelect && songSelect.IsLoaded);
        }

        private void updateFooter(IScreen? _, IScreen? newScreen)
        {
            if (newScreen is IOsuScreen osuScreen && osuScreen.ShowFooter)
            {
                screenFooter.Show();
                screenFooter.SetButtons(osuScreen.CreateFooterButtons());
            }
            else
            {
                screenFooter.Hide();
                screenFooter.SetButtons(Array.Empty<ScreenFooterButton>());
            }
        }
    }
}
