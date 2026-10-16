using System;using System.Collections.Generic;using VirtualDungeon;
public static class IntroSequenceTests {
 static int checks;static void Check(bool ok,string name){if(!ok)throw new Exception(name);checks++;}
 public static void Main(){
  GameStartContext.PrepareNewGame();Check(GameStartContext.Consume()==GameStartKind.NewGame,"New game context");Check(GameStartContext.Consume()==GameStartKind.SceneContinuation,"Context consumed once");GameStartContext.PrepareSavedGame();Check(!new IntroSequence(GameStartContext.Consume()).Active,"Saved context skips intro");Check(!new IntroSequence(GameStartKind.SceneContinuation).Active,"Scene continuation skips intro");
  foreach(int fps in new[]{60,120}){var s=new IntroSequence(GameStartKind.NewGame);Check(s.Darkness==1&&s.Stage.Key=="wake.silence","New game begins fully black");s.Tick(.1f,true);Check(s.Stage.Key=="wake.silence","Confirm cannot cut initial silence");var dialogue=new List<string>();bool boot=false,cut=false,glimpse=false,reveal=false;int guard=0;
   while(s.Active&&guard++<20000){var line=s.Stage;if(line.Visual==IntroVisual.Boot)boot=true;if(line.InterruptAfter>0)cut=line.DisplayText.Length<line.Text.Length;if(line.Visual==IntroVisual.Glimpse)glimpse=true;if(line.Visual==IntroVisual.Reveal){reveal=true;Check(s.Darkness>=0&&s.Darkness<=1,"Fade alpha valid");}
    if(line.AutoSeconds>0||s.Waiting){s.Tick(1f/fps,true);continue;}
    if(line.Speaker=="???"||line.Speaker=="GAMERBOY")dialogue.Add(line.Text);string before=line.Key;s.Tick(10,false);Check(s.Stage.Key==before,"Visible dialogue requires confirmation");s.Tick(0,true);
   }
   Check(!s.Active&&guard<20000,"Sequence finishes at "+fps+" FPS");Check(boot&&cut&&glimpse&&reveal,"All presentation phases present");Check(dialogue.Count==26,"All 26 supplied dialogue lines present");Check(dialogue[0]=="\u00bfMe escuchas?"&&dialogue[25]=="Tiene que haber alguien aqu\u00ed.","First and last lines exact");Check(!string.Join(" ",dialogue).Contains("Guardian"),"No Guardian disclosure");s.Tick(100,true);Check(!s.Active,"Completed sequence stays closed");
  }
  var pause=new IntroSequence(GameStartKind.NewGame);while(pause.Stage.Key!="wake.dialogue.02"){pause.Tick(100,true);if(pause.Current!=null&&pause.Stage.AutoSeconds==0)pause.Tick(0,true);}pause.Tick(1,true);Check(pause.Stage.Key=="wake.dialogue.02","Pause-ending confirmation discarded");pause.Skip();Check(!pause.Active&&pause.Darkness==0,"Skip releases black cover");Console.WriteLine("PASS "+checks+" sequence checks including 60/120 FPS");
 }
}
