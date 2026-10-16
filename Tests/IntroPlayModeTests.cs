#if UNITY_INCLUDE_TESTS
using System.Collections;using NUnit.Framework;using UnityEngine.TestTools;using VirtualDungeon;
// Full camera, audio, loading and controls checks: Assets/Scripts/PrologueValidation.cs (Unity MCP).
public sealed class IntroPlayModeTests {
 [UnityTest] public IEnumerator NewGameStartsBlackAndContinuationSkips(){var s=new IntroSequence(GameStartKind.NewGame);Assert.IsTrue(s.Active);Assert.AreEqual(1,s.Darkness);Assert.AreEqual("wake.silence",s.Stage.Key);yield return null;Assert.IsFalse(new IntroSequence(GameStartKind.SavedGame).Active);Assert.IsFalse(new IntroSequence(GameStartKind.SceneContinuation).Active);}
}
#endif
