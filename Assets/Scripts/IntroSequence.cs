namespace VirtualDungeon
{
    public enum GameStartKind { NewGame, SavedGame, SceneContinuation }

    // The menu/save loader must select the start kind BEFORE loading the game scene.
    // Consuming it prevents replaying the intro on subsequent scene loads.
    public static class GameStartContext
    {
        static GameStartKind next = GameStartKind.NewGame;
        public static void PrepareNewGame() { next = GameStartKind.NewGame; }
        public static void PrepareSavedGame() { next = GameStartKind.SavedGame; }
        public static GameStartKind Consume()
        {
            GameStartKind result = next;
            next = GameStartKind.SceneContinuation;
            return result;
        }
    }

    public sealed class IntroLine
    {
        public readonly string Key;
        public readonly string Speaker;
        public readonly string Text;
        public readonly float PauseBefore;
        public readonly bool InterferenceBefore;
        public IntroLine(string key, string speaker, string text, float pauseBefore = 0f, bool interferenceBefore = false)
        {
            Key = key; Speaker = speaker; Text = text;
            PauseBefore = pauseBefore; InterferenceBefore = interferenceBefore;
        }
    }

    // Stable keys keep the script ready for a future localization table.
    // There is no language system in the current project: Spanish is the source text.
    public sealed class IntroSequence
    {
        static readonly IntroLine[] lines = {
            new IntroLine("intro.01", "GAMERBOY", "Ugh... ¿Qué pasó?"),
            new IntroLine("intro.02", "GAMERBOY", "¿Dónde demonios estoy?"),
            new IntroLine("intro.03", "GAMERBOY", "Espera... ¿Esto es un videojuego?"),
            new IntroLine("intro.04", "GAMERBOY", "No recuerdo haber iniciado ninguno..."),
            new IntroLine("intro.05", "???", "...¿Puedes escucharme?", .8f),
            new IntroLine("intro.06", "GAMERBOY", "¿Eh? ¿Quién dijo eso?"),
            new IntroLine("intro.07", "???", "¡Funcionó! ¡La transferencia fue un éxito!"),
            new IntroLine("intro.08", "GAMERBOY", "¿Transferencia? ¿De qué estás hablando?"),
            new IntroLine("intro.09", "???", "No hay tiempo para explicaciones. Necesitamos tu ayuda."),
            new IntroLine("intro.10", "GAMERBOY", "¿Ayuda? ¡Ni siquiera sé dónde estoy!"),
            new IntroLine("intro.11", "???", "Estás dentro de la Red. Este mundo está muriendo."),
            new IntroLine("intro.12", "GAMERBOY", "¿Dentro de Internet? ¡¿Qué?!"),
            new IntroLine("intro.13", "???", "Busca a los Guardianes. Ellos tienen las respuestas."),
            new IntroLine("intro.14", "GAMERBOY", "¡Espera! ¿Qué guardianes? ¿Cómo salgo de aquí?"),
            new IntroLine("intro.15", "???", "Y, sobre todo... no confíes en..."),
            new IntroLine("intro.16", "SISTEMA", "CONNECTION LOST", .75f, true),
            new IntroLine("intro.17", "GAMERBOY", "¿Hola? ¿Sigues ahí?", .4f),
            new IntroLine("intro.18", "GAMERBOY", "...Genial."),
            new IntroLine("intro.19", "GAMERBOY", "Atrapado en Internet."),
            new IntroLine("intro.20", "GAMERBOY", "Espero que al menos haya un botón para salir.")
        };
        int index;
        float pauseRemaining;
        public bool Active { get; private set; }
        public bool Waiting { get { return Active && pauseRemaining > 0f; } }
        public bool Interference { get { return Waiting && lines[index].InterferenceBefore; } }
        public IntroLine Current { get { return Active && !Waiting ? lines[index] : null; } }
        public IntroSequence(GameStartKind kind) { Active = kind == GameStartKind.NewGame; }

        public void Skip()
        {
            Active=false; pauseRemaining=0;
        }

        // Time only ends pauses. Each visible line always needs a fresh confirmation.
        // A press made during a pause is discarded, including on its final frame.
        public void Tick(float elapsed, bool confirm)
        {
            if (!Active) return;
            if (Waiting)
            {
                pauseRemaining = System.Math.Max(0f, pauseRemaining - System.Math.Max(0f, elapsed));
                return;
            }
            if (!confirm) return;
            index++;
            if (index == lines.Length) { Active = false; return; }
            pauseRemaining = lines[index].PauseBefore;
        }
    }
}
