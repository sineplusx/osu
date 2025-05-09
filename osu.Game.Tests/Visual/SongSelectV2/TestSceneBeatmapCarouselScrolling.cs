// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using NUnit.Framework;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Testing;
using osu.Game.Screens.Select.Filter;
using osu.Game.Screens.SelectV2;

namespace osu.Game.Tests.Visual.SongSelectV2
{
    [TestFixture]
    public partial class TestSceneBeatmapCarouselScrolling : BeatmapCarouselTestScene
    {
        [SetUpSteps]
        public void SetUpSteps()
        {
            RemoveAllBeatmaps();
            CreateCarousel();

            SortBy(SortMode.Artist);

            AddBeatmaps(10);
            WaitForDrawablePanels();
        }

        [Test]
        public void TestScrollPositionMaintainedOnAddSecondSelected()
        {
            Quad positionBefore = default;

            AddStep("select middle beatmap", () => Carousel.CurrentSelection = BeatmapSets.ElementAt(BeatmapSets.Count - 2).Beatmaps.First());
            AddStep("scroll to selected item", () => Scroll.ScrollTo(Scroll.ChildrenOfType<PanelBeatmap>().Single(p => p.Selected.Value)));

            WaitForScrolling();

            AddStep("save selected screen position", () => positionBefore = Carousel.ChildrenOfType<PanelBeatmap>().FirstOrDefault(p => p.Selected.Value)!.ScreenSpaceDrawQuad);

            RemoveFirstBeatmap();
            WaitForFiltering();

            AddAssert("select screen position unchanged", () => Carousel.ChildrenOfType<PanelBeatmap>().Single(p => p.Selected.Value).ScreenSpaceDrawQuad,
                () => Is.EqualTo(positionBefore));
        }

        [Test]
        public void TestScrollPositionMaintainedOnAddLastSelected()
        {
            Quad positionBefore = default;

            AddStep("scroll to last item", () => Scroll.ScrollToEnd(false));

            AddStep("select last beatmap", () => Carousel.CurrentSelection = BeatmapSets.Last().Beatmaps.Last());

            WaitForScrolling();

            AddStep("save selected screen position", () => positionBefore = Carousel.ChildrenOfType<PanelBeatmap>().FirstOrDefault(p => p.Selected.Value)!.ScreenSpaceDrawQuad);

            RemoveFirstBeatmap();
            WaitForFiltering();
            AddAssert("select screen position unchanged", () => Carousel.ChildrenOfType<PanelBeatmap>().Single(p => p.Selected.Value).ScreenSpaceDrawQuad,
                () => Is.EqualTo(positionBefore));
        }
    }
}
