using System;
using VirtualDungeon;

// Standalone regression tests, independent of the Unity editor and external packages.
public static class IntroSequenceTests
{
    static void Check(bool condition, string name)
    {
        if (!condition) throw new Exception(name);
    }
    public static void Main()
    {
        GameStartContext.PrepareNewGame();
        Check(GameStartContext.Consume() == GameStartKind.NewGame, "New game selected");
        Check(GameStartContext.Consume() == GameStartKind.SceneContinuation, "Scene changes do not replay intro");
        GameStartContext.PrepareSavedGame();
        Check(!new IntroSequence(GameStartContext.Consume()).Active, "Saved games skip intro");
        Check(!new IntroSequence(GameStartKind.SceneContinuation).Active, "Continuations skip intro");
        GameStartContext.PrepareNewGame();
        var sequence = new IntroSequence(GameStartContext.Consume());
        Check(sequence.Active && sequence.Current.Key == "intro.01", "New game locks controls on first line");
        sequence.Tick(100f, false);
        Check(sequence.Current.Key == "intro.01", "Time never advances a visible line");
        for (int i = 0; i < 4; i++) sequence.Tick(0f, true);
        Check(sequence.Active && sequence.Waiting && sequence.Current == null, "Mystery pause retains lock");
        sequence.Tick(.3f, true);
        Check(sequence.Waiting, "Cannot skip pause by confirming");
        sequence.Tick(1f, true);
        Check(sequence.Current.Key == "intro.05", "Pause-ending press is not buffered");
        for (int i = 0; i < 11; i++) sequence.Tick(0f, true);
        Check(sequence.Interference && sequence.Waiting, "Glitch before connection loss");
        sequence.Tick(1f, false);
        Check(sequence.Current.Speaker == "SISTEMA" && sequence.Current.Text == "CONNECTION LOST", "Connection lost follows glitch");
        sequence.Tick(0f, true);
        Check(sequence.Waiting && !sequence.Interference, "Silence after communication stops");
        sequence.Tick(1f, false);
        for (int i = 0; i < 3; i++) sequence.Tick(0f, true);
        Check(sequence.Active && sequence.Current.Key == "intro.20", "Last dialogue still locks controls");
        sequence.Tick(0f, true);
        Check(!sequence.Active && sequence.Current == null, "Last confirmation releases lock and closes text");
        sequence.Tick(10f, true);
        Check(!sequence.Active, "Completed intro stays closed");
        Console.WriteLine("PASS: new game, saved game, scene continuation, pauses, interference, final control release.");
    }
}
