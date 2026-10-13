using System;
using UnityEngine;

// The only time origin is the exact scheduled audio start, never frame delta time.
public sealed class JumpRhythmClock
{
 public double ScheduledStart { get; private set; }
 public double SecondsPerBeat { get; private set; }
 public double Offset { get; private set; }
 public double SongTime { get { return AudioSettings.dspTime-ScheduledStart; } }
 public double BeatPosition { get { return (SongTime-Offset)/SecondsPerBeat; } }
 public int BeatCount { get { return Math.Max(0,(int)Math.Floor(BeatPosition)+1); } }
 public float Phase { get { double b=BeatPosition;return (float)(b-Math.Floor(b)); } }
 // Gameplay elapsed time is independent of the continuously playing combat music.
 public JumpRhythmClock(EnemyJumpSettings settings)
 {
  SecondsPerBeat=60d/Math.Max(1,settings.bpm);Offset=settings.musicOffsetSeconds;
  ScheduledStart=AudioSettings.dspTime+Math.Max(.2,settings.scheduleLeadSeconds);
 }
 public JumpRhythmClock(AudioSource source,AudioClip track,EnemyJumpSettings settings)
 {
  SecondsPerBeat=60d/Math.Max(1,settings.bpm);Offset=settings.musicOffsetSeconds;
  ScheduledStart=AudioSettings.dspTime+Math.Max(.2,settings.scheduleLeadSeconds);
  source.clip=track;source.pitch=1;source.loop=true;source.timeSamples=0;
  source.PlayScheduled(ScheduledStart);
 }
}
