using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;
using VirtualDungeon;

public class IntroPlayModeTests
{
    const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
    static object Read(object target, string field) { return target.GetType().GetField(field, Private).GetValue(target); }
    static IEnumerator Press(Keyboard keyboard, params Key[] keys)
    {
        InputSystem.QueueStateEvent(keyboard, new KeyboardState(keys));
        yield return null;
        yield return null;
        InputSystem.QueueStateEvent(keyboard, new KeyboardState());
        yield return null;
        yield return null;
    }
    [UnityTest]
    public IEnumerator IntroBlocksGameplayThenRestoresMovement()
    {
        foreach (var old in Object.FindObjectsByType<DungeonPrototype>()) Object.DestroyImmediate(old.gameObject);
        GameStartContext.PrepareNewGame();
        var prototype = new GameObject("Introduction integration test").AddComponent<DungeonPrototype>();
        yield return null;
        var sequence = (IntroSequence)Read(prototype, "introduction");
        Assert.IsTrue(sequence.Active);
        Assert.AreEqual("intro.01", sequence.Current.Key);
        var initialPosition = (Vector2Int)Read(prototype, "position");
        Assert.AreEqual(new Vector2Int(1, 1), initialPosition);
        var previousBackground = InputSystem.settings.backgroundBehavior;
        var previousEditorInput = InputSystem.settings.editorInputBehaviorInPlayMode;
        InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
        InputSystem.settings.editorInputBehaviorInPlayMode = InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
        var keyboard = InputSystem.AddDevice<Keyboard>();
        try
        {
            yield return Press(keyboard, Key.W);
            Assert.AreEqual(initialPosition, Read(prototype, "position"), "Exploration input must be blocked");
            prototype.GetType().GetMethod("Action", Private).Invoke(prototype, new object[] { 0 });
            Assert.IsFalse((bool)Read(prototype, "busy"), "Combat action must be blocked");
            var move = (IEnumerator)prototype.GetType().GetMethod("Move", Private).Invoke(prototype, new object[] { 1, 0 });
            Assert.IsFalse(move.MoveNext(), "Movement coroutine must reject movement during intro");
            yield return Press(keyboard, Key.E);
            Assert.AreEqual("intro.02", sequence.Current.Key, "Interaction should advance exactly one line");
            while (sequence.Active && (sequence.Current == null || sequence.Current.Key != "intro.20"))
                sequence.Tick(1f, true);
            Assert.IsTrue(sequence.Active, "Controls stay locked on last line");
            yield return Press(keyboard, Key.E);
            Assert.IsFalse(sequence.Active);
            Assert.IsNull(sequence.Current, "Dialogue is closed after last confirmation");
            yield return Press(keyboard, Key.W);
            yield return new WaitForSeconds(.3f);
            Assert.AreEqual(new Vector2Int(1, 2), Read(prototype, "position"), "Exploration resumes after intro");
        }
        finally
        {
            InputSystem.RemoveDevice(keyboard);
            InputSystem.settings.backgroundBehavior = previousBackground;
            InputSystem.settings.editorInputBehaviorInPlayMode = previousEditorInput;
            Object.DestroyImmediate(prototype.gameObject);
        }
        GameStartContext.PrepareSavedGame();
        var resumed = new GameObject("Saved game intro test").AddComponent<DungeonPrototype>();
        yield return null;
        Assert.IsFalse(((IntroSequence)Read(resumed, "introduction")).Active, "Saved-game entry must skip intro");
        Object.DestroyImmediate(resumed.gameObject);
    }
}
