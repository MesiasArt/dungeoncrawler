using System;using System.Collections.Generic;using UnityEditor;using UnityEngine;
public static class RefugeWelcomeInstaller {
 [Serializable]sealed class Expected {public string[] speakers,texts;}
 [MenuItem("Editor/Narrativa/Instalar bienvenida del refugio")]
 public static string Install(){
  var expected=JsonUtility.FromJson<Expected>(System.IO.File.ReadAllText("Tests/RefugeWelcomeExpected.json"));
  const string root="Assets/Resources/Exploration/";var settings=AssetDatabase.LoadAssetAtPath<RefugeWelcomeSettings>(root+"RefugeWelcome.asset");if(settings==null){settings=ScriptableObject.CreateInstance<RefugeWelcomeSettings>();AssetDatabase.CreateAsset(settings,root+"RefugeWelcome.asset");}
  var profile=AssetDatabase.LoadAssetAtPath<DungeonInteractionSettings>(root+"RefugeWelcomeDialogue.asset");if(profile==null){profile=ScriptableObject.CreateInstance<DungeonInteractionSettings>();AssetDatabase.CreateAsset(profile,root+"RefugeWelcomeDialogue.asset");}
  profile.id="refuge.welcome";profile.once=true;profile.continueLegacy=false;profile.groups.Clear();var lines=new List<WorldDialogueLine>();for(int i=0;i<expected.texts.Length;i++)lines.Add(new WorldDialogueLine(expected.speakers[i],expected.texts[i]));
  var look=lines.Find(l=>l.text.StartsWith("\u00bf\u00a1Qui"));look.presentationCue="refuge.welcome.look-back";look.pauseBefore=2.2f;lines.Find(l=>l.text.StartsWith("De muchas cosas.")).pauseBefore=1.1f;
  profile.actions=new List<DungeonEventAction>{new DungeonEventAction{id="refuge.welcome.dialogue",kind=DungeonEventActionKind.Dialogue,dialogue=lines.ToArray()}};
  settings.triggerCell=new Vector2Int(17,6);settings.dialogue=profile;EditorUtility.SetDirty(settings);EditorUtility.SetDirty(profile);AssetDatabase.SaveAssets();return "33 exact welcome lines installed at X17/Y6; published geometry and NPC assets unchanged";
 }
}
