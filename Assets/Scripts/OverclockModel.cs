using System;
// Pure timed session. Clock values are monotonic seconds, never frame counts.
public sealed class OverclockModel {
 public readonly double Started,Duration;
 public readonly int RequiredPresses;
 public int Presses {get;private set;}
 public bool Finished {get;private set;}
 public bool Success {get;private set;}
 double lastTime;
 public float Charge {get{return Math.Min(1,Presses/(float)RequiredPresses);}}
 public double Remaining {get{return Math.Max(0,Duration-(lastTime-Started));}}
 public OverclockModel(double now,double duration,int presses){Started=lastTime=now;Duration=Math.Max(.1,duration);RequiredPresses=Math.Max(1,presses);}
 public void Tick(double now){if(Finished)return;lastTime=Math.Max(lastTime,now);if(lastTime-Started>=Duration){Finished=true;Success=false;}}
 public bool Press(double now){Tick(now);if(Finished)return false;Presses++;if(Presses>=RequiredPresses){Finished=true;Success=true;}return true;}
}
