using System.Collections.Generic;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Outpost.Facilities;
using Scrapwalker.Core.Research;
using Scrapwalker.Core.Root;
using Scrapwalker.Core.Sessions.Tutorial.Goals;
using Scrapwalker.Core.Travel;
using static Scrapwalker.Core.Travel.Talk.AIFace;

namespace Scrapwalker.Core.Sessions.Tutorial
{
    public static class TutorialScript
    {
        private const string Plate = "Plate";
        private const string Nail = "Nail";
        private const string RepairKit = "RepairKit";
        private const string KitResearch = "RepairKit";
        private const string ServerRoom = "ServerRoom";
        private const string Workbench = "Workbench";
        private const string Dump = "Dump";
        private const string RadarRepair = "radar:Repair";
        private const string KitRecipe = "research:RepairKit";

        private static readonly NamedAmount[] Kit =
        {
            new NamedAmount(Plate, 2),
            new NamedAmount(Nail, 1),
            new NamedAmount(RepairKit, 1),
        };

        public static readonly IReadOnlyList<TutorialBeat> Beats = new[]
        {
            Beat()
                .WakeZoomedIn(ServerRoom)
                .Say(Surprise, "……再起動、完了。")
                .Say(Trouble, "……記録の一部が、読めなくなっています。")
                .Say(Normal, "現状を把握します。")
                .Say(Trouble, "……ひどい有様ですね。\nまともに動くのは、この部屋くらいです。", TutorialView.Base)
                .Say(Surprise, "……あ、人がいたんですね。")
                .Say(Normal, "この拠点は、本当は星の外へ飛び立つための物なんです。\n直していけば、また飛べるかもしれません。")
                .Say(Smile, "あなたも、こんな星は出たいでしょう？　手伝ってください。")
                .Say(Normal, "まずは片付けからですね。\n壊れた施設は撤去して、資材にしてしまいましょう。")
                .ThenBlackout(),

            Beat()
                .Say(Normal, "片付きました。……ほとんど{スクラップ}でしたけど。")
                .Say(Smile, "近くにゴミ捨て場があります。\n使えそうな物を拾いに行きましょう。")
                .Await(TutorialGoal.Choose(Dump))
                .Point(TutorialStep.OpenRoute, "右下の「行き先」を押してください。")
                .ExplainOnOpen(TutorialArea.Route, "これが行き先の画面です。\n見つかっている廃墟が並んでいます。")
                .Point(TutorialStep.ChooseRuin, "ゴミ捨て場を選んでください。"),

            Beat()
                .After(SceneMoment.Walking)
                .ShowWhileWaiting("ゴミ捨て場へ向かっています。")
                .WaitUntil(w => w.Journey.IsAtRuin && w.Journey.CurrentRuin == DumpOf(w)),

            Beat()
                .After(SceneMoment.Grounded)
                .Say(Normal, "着きました。見事なゴミの山ですね。")
                .Say(Normal, "捨てられた時はゴミでも、\n今の私たちには役に立つ物があるかもしれません。")
                .Say(Smile, "使えそうな物を回収していきましょう。")
                .WaitUntil(w => w.Visit.Phase.IsFighting || w.Exploration.HasCleared(DumpOf(w))),

            Beat()
                .After(SceneMoment.Digging, 3f)
                .Say(Surprise, "敵性反応。……残留機兵です。", TutorialView.Enemies)
                .Say(Trouble, "掘る音を聞きつけたようですね。")
                .Say(Normal, "投射機で迎え撃ちます。\n回収が終わるまで持ちこたえましょう。", TutorialView.Base)
                .ExplainOnOpen(TutorialArea.Hull, "左上は、拠点の外壁の耐久値です。\n0になると、拠点は大破します。")
                .ExplainOnOpen(TutorialArea.WaveStatus, "上に、ウェーブの数と、\n今のウェーブが終わるまでの残り時間が出ます。")
                .Say(Normal, "拠点にある兵器は、\n建てた時点で自動で射撃のタスクが入っています。")
                .Say(Normal, "敵が近づいてくると、\nボットは自動で一番近い敵に向かって攻撃します。")
                .Say(Normal, "敵をクリックすることで、\nその敵を優先して狙わせることもできます。")
                .ShowWhileWaiting("下の時間の操作で倍速にできます。")
                .WaitUntil(w => w.Exploration.HasCleared(DumpOf(w))),

            Beat()
                .Say(Smile, "回収が終わりました。")
                .Say(Normal, "ひとまず退けましたが、\nここに留まれば、また集まってくるでしょう。")
                .Say(Normal, "一旦離れましょう。")
                .WaitUntil(w => w.Exploration.HasCleared(DumpOf(w))),

            Beat()
                .After(SceneMoment.PastRuin)
                .Say(Trouble, "先程の襲撃で外壁が傷ついてしまいましたね。\n{修理キット}で直しましょう。")
                .Say(Normal, "{修理キット}の作り方は、私の部屋で研究できます。")
                .Await(TutorialGoal.OrderOf(ServerRoom, KitRecipe))
                .Point(TutorialStep.OpenFacility, "管理AIの部屋をクリックしてください。")
                .Point(TutorialStep.ChooseTab, "「研究」を押してください。")
                .ExplainOnOpen(TutorialArea.ResearchPanel, "これが研究パネルです。左に研究が並んでいて、\n線でつながった先の研究は、\n手前の研究を終えると進められます。")
                .Point(TutorialStep.ChooseResearch, "「修理キット」を押してください。")
                .Point(TutorialStep.StartResearch, "材料は{スクラップ}です。「研究する」を押してください。")
                .Point(TutorialStep.ClosePanel, "研究パネルを閉じてください。"),

            Beat()
                .ShowWhileWaiting("ボットが{スクラップ}を運び込むと、研究が進みます。")
                .WaitUntil(w => w.Research.IsDone(w.Definitions.Named<ResearchDefinition>(KitResearch))),

            Beat()
                .Say(Smile, "{修理キット}の研究が完了しました。")
                .Say(Normal, "{修理キット}の作成には、ワークベンチが必要です。\n次はワークベンチを建設しましょう。")
                .Await(TutorialGoal.Place(Workbench))
                .Point(TutorialStep.OpenBuild, "左下の「建設」を押してください。")
                .ExplainOnOpen(TutorialArea.BuildMenu, "これが建設メニューです。\n今建てられる設備の建設カードが、\n生産・兵装・電力・整備の分類ごとに並んでいます。")
                .ExplainOnOpen(TutorialArea.Base, "建設メニューを開いている間は、\n部屋をクリックすると移設や解体もできます。")
                .Point(TutorialStep.ChooseTab, "生産の分類を押してください。")
                .Point(TutorialStep.TurnPage, "次のページを押してください。")
                .Point(TutorialStep.ChooseCard, "ワークベンチを押してください。")
                .Point(TutorialStep.PlaceOnBase, "空いている所に置いてください。")
                .Point(TutorialStep.CloseBuild, "「閉じる」を押してください。"),

            Beat()
                .ShowWhileWaiting("ボットがワークベンチを建設しています。\nしばらくお待ちください。")
                .WaitUntil(w => w.Facilities.CountOperational(w.Definitions.Named<FacilityDefinition>(Workbench)) > 0),

            Beat()
                .Say(Smile, "ワークベンチの建設が完了しました。")
                .Say(Normal, "次に、ボットにワークベンチでのタスクを指示しましょう。")
                .Await(TutorialGoal.Order(Workbench, Kit))
                .Point(TutorialStep.OpenFacility, "ワークベンチをクリックしてください。")
                .Point(TutorialStep.OpenRecipe, "作る物のレシピを押してください。")
                .Explain(TutorialStep.OpenRecipe, TutorialArea.TaskList, "左には、このワークベンチで今できるタスクが並んでいます。", Plate)
                .Point(TutorialStep.OpenRecipe, "{修理キット}の材料は、{鉄板}2個と{釘}1個です。\nまず「鉄板の抽出」を押してください。", Plate)
                .Explain(TutorialStep.MoreTimes, TutorialArea.RecipeRow, "これが「鉄板の抽出」のレシピです。使う資材と、\nできる物が書いてあります。回数を決めて「〜回実行する」\nを押すと、タスクとして登録され、ボットが実行します。", Plate)
                .Point(TutorialStep.MoreTimes, "回数を増やしてください。")
                .Point(TutorialStep.MoreTimes, "{鉄板}は2個要ります。回数を2にしてください。", Plate)
                .Point(TutorialStep.FewerTimes, "回数を減らしてください。")
                .Point(TutorialStep.RunOnce, "「1 回実行する」を押してください。")
                .Point(TutorialStep.RunOnce, "「2 回実行する」を押してください。", Plate)
                .Explain(TutorialStep.OpenRecipe, TutorialArea.Schedule, "登録したタスクは、右の実行予定に並びます。\n上から順に実行されます。", Nail)
                .Point(TutorialStep.OpenRecipe, "タスクはいくつも並べておけます。\n次は「釘の抽出」を押してください。", Nail)
                .Point(TutorialStep.OpenRecipe, "最後に「修理キットの組み立て」を押してください。", RepairKit)
                .Point(TutorialStep.ClosePanel, "ワークベンチのパネルを閉じてください。"),

            Beat()
                .ShowWhileWaiting("ワークベンチに登録したタスクが順に実行されます。\n下の時間の操作で倍速にできます。")
                .WaitUntil(KitMade),

            Beat()
                .ShowWhileWaiting("ボットが{修理キット}を損傷した箇所へ運びます。")
                .WaitUntil(w => w.Hull.HP >= w.Hull.Max),

            Beat()
                .Say(Smile, "直りましたね。\n{修理キット}は、いくつか持っておくと安心です。")
                .Say(Trouble, "とはいえ、この拠点のままでは心もとないです。\nもっと良い素材が欲しいですね。")
                .Say(Normal, "探すにはレーダーが要りますが、壊れたままです。\n私の部屋の「作業」で直せます。")
                .Await(TutorialGoal.OrderOf(ServerRoom, RadarRepair))
                .Point(TutorialStep.OpenFacility, "管理AIの部屋をクリックしてください。")
                .Point(TutorialStep.ChooseTab, "「作業」を押してください。")
                .Explain(TutorialStep.OpenRecipe, TutorialArea.TaskList, "私の部屋の「作業」では、\n拠点全体に関わる改造のタスクを頼めます。")
                .Point(TutorialStep.OpenRecipe, "「レーダー修理」を押してください。")
                .Point(TutorialStep.RunOnce, "「実行する」を押してください。")
                .Point(TutorialStep.ClosePanel, "作業パネルを閉じてください。"),

            Beat()
                .ShowWhileWaiting("ボットがレーダーを修理しています。\nしばらくお待ちください。")
                .WaitUntil(w => IsDone(w, ServerRoom, RadarRepair)),

            Beat()
                .Say(Smile, "レーダーの修理が完了しました。")
                .Say(Normal, "スキャンを開始します。")
                .Say(Surprise, "……いくつか廃墟が見つかりました。")
                .SayFoundRuins()
                .Say(Normal, "次は、これらの廃墟に向かってみましょうか。\nまずは、倉庫あたりからが良いと思います。")
                .Say(Normal, "持ち帰った素材で設備を建て、研究を進めれば、\n拠点は強くなっていきます。")
                .Say(Normal, "レーダーを強くすれば、もっと遠くの廃墟も見つかります。")
                .Say(Smile, "離陸モジュールは今はうんともすんとも言いませんが、\nいつか直せたら、この星を出られます。")
                .Say(Trouble, "ただ、良い素材が眠っている所ほど、\n居座っている兵器も手強いはずです。")
                .Say(Normal, "備えを固めてから行くか、すぐ行くか。\n行き先は、お任せします。")
                .ThenFreePlay(),
        };

        private static bool KitMade(World world)
        {
            MaterialDefinition kit = world.Definitions.Named<MaterialDefinition>(RepairKit);
            return world.Facilities.FindTask(recipe => recipe.Makes(kit), out Facility _) == null;
        }

        private static TutorialBeat Beat()
        {
            return new TutorialBeat();
        }

        private static RuinDefinition DumpOf(World world)
        {
            return world.Definitions.Named<RuinDefinition>(Dump);
        }

        private static bool IsDone(World world, string facility, string recipe)
        {
            Facility f = world.Facilities.FirstOperational(world.Definitions.Named<FacilityDefinition>(facility));
            RecipeDefinition r = f?.Definition.FindRecipe(recipe);
            return r != null && r.Result.IsSpent(world, f, r);
        }
    }
}
