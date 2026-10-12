# Jump rhythm calibration

Only HACK -> JUMP uses the scheduled musical clock. Arcade Showdown is played unchanged, at pitch 1, starting at a future DSP timestamp. The normal combat source pauses during Jump and resumes afterwards. Other hacks keep their current music behavior.

Select Assets/Resources/Combat/EnemyJumpSettings.asset in the Inspector. New sessions snapshot these settings; restart Jump after editing calibration.

- BPM: initial estimate 145, not a measured beat map.
- Music Offset Seconds: where beat zero falls in the original track, in seconds. Positive values move both the pulse and enemy arrival grid later.
- Judgement Offset Seconds: latency calibration. Positive allows landings later than the visual/music grid without shifting enemy movement.
- Perfect Window Seconds: +/- 0.065 seconds initially.
- Good Window Seconds: +/- 0.125 seconds initially. Outside this window or missing/colliding with an enemy is MISS.
- Beats Per Enemy: 2 initially; First Enemy Beat: 8, with extra full beat intervals added if the entry travel requires it.
- Schedule Lead Seconds: 0.5, allowing streaming audio to prepare before playback.

Enemy positions derive from their absolute planned arrival time; frame rate, combo and jump timing do not move their target beat. Existing speed/spacing variation is ignored in rhythm mode. High rebound velocity is matched to the configured beat interval within the existing arena's jump-height limit. Very slow BPM or long beat intervals can exceed that limit; reduce Beats Per Enemy for those configurations.

SPACE and the JUMP button remain manual. A landing is judged independently of the extra landing press; pressing near/just after contact retains the existing high rebound, otherwise the small rebound remains. Each accepted stomp still adds 2 damage, with floating points, accumulated total and the existing defeat fall.

The small cyan pulse, sweep and BEAT counter use the same DSP origin as the song. BPM is approximate and offset defaults to zero: calibrate against the audible track on the target audio device. No time stretching, editing or replacement of the MP3 occurs.

Validation: Unity MCP Play Mode simulations cover actual PERFECT/GOOD/MISS landings, 18 complete chains across 30/60/240 FPS, offsets and gravity configurations, no-input failure, scheduled playback of Arcade Showdown at pitch 1, paused shared music and restoration after Jump. Legacy Jump tests remain available separately.

Playability update: the manual ground jump now reaches enemy-top height exactly two beats after launch by default (Ground Jump Beats is configurable). Exact ballistic integration at 120 Hz carries the remainder of each contact step into the rebound, and post-contact presses retain that same contact timestamp. Input advances to current DSP time before applying the jump. Sprite feet/head match the collision height; a brief SALTA cue identifies the launch beat. The preserved landing-press grace defaults to 140 ms. A live Hongo run with queued SPACE input achieved ten PERFECT stomps and 20 damage.

Final live input check after the taller opening jump and input-event timestamp fix: Hongo, ten PERFECT stomps, 20 damage, landing error approximately 2.8 ms with no accumulated drift. Space events were queued through Input System in Play Mode; enemy positions and combo were not injected.
