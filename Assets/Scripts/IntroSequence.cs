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

    public enum IntroVisual { Black, Boot, Glimpse, Reveal, World }
    public sealed class IntroLine {
        public readonly string Key,Speaker,Text;
        public readonly float PauseBefore,AutoSeconds;
        public readonly bool InterferenceBefore;
        public readonly IntroVisual Visual;
        public readonly int InterruptAfter;
        public string DisplayText {get{return InterruptAfter>0?Text.Substring(0,System.Math.Min(Text.Length,InterruptAfter)):Text;}}
        public IntroLine(string key,string speaker,string text,float pauseBefore=0,bool interferenceBefore=false,IntroVisual visual=IntroVisual.World,float autoSeconds=0,int interruptAfter=0){Key=key;Speaker=speaker;Text=text;PauseBefore=pauseBefore;InterferenceBefore=interferenceBefore;Visual=visual;AutoSeconds=autoSeconds;InterruptAfter=interruptAfter;}
    }
    // Exact approved source; visual timing does not alter dialogue or gameplay state.
    public sealed class IntroSequence {
        static readonly IntroLine[] lines={
            new IntroLine("wake.silence","","",0,false,IntroVisual.Black,1f),
            new IntroLine("wake.boot.1","SISTEMA","SYSTEM BOOT",0,false,IntroVisual.Boot,1.2f),
            new IntroLine("wake.boot.2","SISTEMA","Ejecutando recovery.exe...",0,false,IntroVisual.Boot,1.9f),
            new IntroLine("wake.boot.3","SISTEMA","Restaurando sesi\u00f3n...",.35f,false,IntroVisual.Boot,1.4f,14),
            new IntroLine("wake.boot.cut","","",0,true,IntroVisual.Black,.4f),
            new IntroLine("wake.dialogue.01","???","\u00bfMe escuchas?",0.0f,false,IntroVisual.Black),
            new IntroLine("wake.dialogue.02","???","Despierta.",0.5f,false,IntroVisual.Black),
            new IntroLine("wake.glimpse","","",0,true,IntroVisual.Glimpse,.34f),
            new IntroLine("wake.glimpse.black","","",0,false,IntroVisual.Black,.35f),
            new IntroLine("wake.dialogue.03","GAMERBOY","Ah...",0.3f,false,IntroVisual.Black),
            new IntroLine("wake.dialogue.04","???","Eso es. \u00bfPuedes o\u00edrme?",0.0f,false,IntroVisual.Black),
            new IntroLine("wake.dialogue.05","GAMERBOY","Hay alguien...",0.0f,false,IntroVisual.Black),
            new IntroLine("wake.transmission.cut","","",0,true,IntroVisual.Black,.2f),
            new IntroLine("wake.reveal","","",0,false,IntroVisual.Reveal,2f),
            new IntroLine("wake.dialogue.06","GAMERBOY","\u00bfQu\u00e9...?",0.0f,false,IntroVisual.World),
            new IntroLine("wake.dialogue.07","GAMERBOY","No conozco este lugar.",0.4f,false,IntroVisual.World),
            new IntroLine("wake.contemplate","","",0,false,IntroVisual.World,1.1f),
            new IntroLine("wake.dialogue.08","GAMERBOY","Yo estaba...",0.5f,false,IntroVisual.World),
            new IntroLine("wake.dialogue.09","GAMERBOY","Estaba con alguien.",0.7f,false,IntroVisual.World),
            new IntroLine("wake.dialogue.10","GAMERBOY","\u00bfPor qu\u00e9 no puedo...?",0.65f,false,IntroVisual.World),
            new IntroLine("wake.dialogue.11","???","\u00bfSigues ah\u00ed?",0.2f,false,IntroVisual.World),
            new IntroLine("wake.dialogue.12","GAMERBOY","S\u00ed. Estoy aqu\u00ed.",0.1f,false,IntroVisual.World),
            new IntroLine("wake.dialogue.13","GAMERBOY","No s\u00e9 d\u00f3nde es aqu\u00ed.",0.2f,false,IntroVisual.World),
            new IntroLine("wake.dialogue.14","???","Est\u00e1s dentro de la Red.",0.3f,false,IntroVisual.World),
            new IntroLine("wake.dialogue.15","GAMERBOY","\u00bfLa Red?",0.2f,false,IntroVisual.World),
            new IntroLine("wake.dialogue.16","GAMERBOY","Espera. \u00bfT\u00fa sabes c\u00f3mo llegu\u00e9?",0.15f,false,IntroVisual.World),
            new IntroLine("wake.signal.break","","",0,true,IntroVisual.World,.2f),
            new IntroLine("wake.dialogue.17","???","La conexi\u00f3n no va a durar.",0.2f,false,IntroVisual.World),
            new IntroLine("wake.dialogue.18","GAMERBOY","No, espera. Hab\u00eda alguien conmigo. No consigo recordar qui\u00e9n.",0.1f,false,IntroVisual.World),
            new IntroLine("wake.dialogue.19","???","Esc\u00fachame...",0.7f,false,IntroVisual.World),
            new IntroLine("wake.dialogue.20","???","No s\u00e9 cu\u00e1nto tiempo podr\u00e9 mantener esta conexi\u00f3n.",0.25f,false,IntroVisual.World),
            new IntroLine("wake.dialogue.21","GAMERBOY","Entonces dime c\u00f3mo salir de aqu\u00ed.",0.15f,false,IntroVisual.World),
            new IntroLine("wake.dialogue.22","???","Hay algo que debes saber...",0.3f,false,IntroVisual.World),
            new IntroLine("wake.dialogue.23","???","Y, sobre todo... no conf\u00edes en\u2014",0.45f,true,IntroVisual.World),
            new IntroLine("wake.disconnected","SISTEMA","CONEXI\u00d3N PERDIDA",.15f,true,IntroVisual.World,1.7f),
            new IntroLine("wake.dialogue.24","GAMERBOY","\u00bfHola?",0.5f,false,IntroVisual.World),
            new IntroLine("wake.dialogue.25","GAMERBOY","No. Espera.",0.75f,false,IntroVisual.World),
            new IntroLine("wake.dialogue.26","GAMERBOY","Tiene que haber alguien aqu\u00ed.",0.45f,false,IntroVisual.World)
        };
        int index;float age;
        public bool Active {get;private set;}
        public bool Waiting {get{return Active&&age<lines[index].PauseBefore;}}
        public bool Interference {get{return Active&&lines[index].InterferenceBefore&&age<lines[index].PauseBefore+.18f;}}
        public IntroLine Stage {get{return Active?lines[index]:null;}}
        public IntroLine Current {get{return Active&&!Waiting?lines[index]:null;}}
        public float LineAge {get{return Active?System.Math.Max(0,age-lines[index].PauseBefore):0;}}
        public float Darkness {get{if(!Active)return 0;switch(Stage.Visual){case IntroVisual.Black:case IntroVisual.Boot:return 1;case IntroVisual.Glimpse:double pulse=System.Math.Sin(System.Math.PI*System.Math.Min(1,LineAge/System.Math.Max(.01f,Stage.AutoSeconds)));return 1-(float)pulse*.85f;case IntroVisual.Reveal:float t=System.Math.Min(1,LineAge/System.Math.Max(.01f,Stage.AutoSeconds));return 1-t*t*(3-2*t);default:return 0;}}}
        public IntroSequence(GameStartKind kind){Active=kind==GameStartKind.NewGame;}
        public void Skip(){Active=false;age=0;}
        public void Tick(float elapsed,bool confirm){if(!Active)return;bool wasWaiting=Waiting;age+=System.Math.Max(0,elapsed);if(wasWaiting)return;var line=lines[index];if(line.AutoSeconds>0){if(LineAge<line.AutoSeconds)return;}else if(!confirm)return;index++;age=0;if(index==lines.Length)Active=false;}
    }
}
