using UnityEngine;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;

using UnityEngine.UI;

using System.IO;

namespace exsdk {
  public partial class Exporter {
    // -----------------------------------------
    // DumpFont
    // -----------------------------------------
    JSON_Font DumpFont(Font font) {
      JSON_Font result = new JSON_Font();
      SerializedObject so = new SerializedObject(font);
      so.Update();

      result.texture = Utils.AssetID(font.material.mainTexture);

      // JSON_Font_Chars
      CharacterInfo[] infos = font.characterInfo;
      Texture tex = font.material.mainTexture;
      for (int i = 0; i < infos.Length; i++) {
        JSON_Font_Chars jsonInfo = new JSON_Font_Chars();       
        CharacterInfo info = infos[i];

        jsonInfo.id = info.index;
        jsonInfo.x = info.uvBottomLeft.x * tex.width;
        jsonInfo.y = (1-info.uvBottomLeft.y-info.uv.height) * tex.height;
        jsonInfo.width = info.maxX - info.minX;
        jsonInfo.height = info.maxY - info.minY;
        jsonInfo.xoffset = info.vert.x;
        jsonInfo.yoffset = -info.vert.y + so.FindProperty("m_Ascent").floatValue;
        jsonInfo.xadvance = info.advance;

        result.chars.Add(info.index.ToString(), jsonInfo);
      }

      // JSON_Font_Kerning
      SerializedProperty kernings = so.FindProperty("m_KerningValues");
      int len = kernings.arraySize;
      for (int i = 0; i < len; i++) {
        JSON_Font_Kerning jsonKerning = new JSON_Font_Kerning();
        SerializedProperty kerning = kernings.GetArrayElementAtIndex(i);
        SerializedProperty pairProp = kerning.FindPropertyRelative("first");
        pairProp.Next(true);
        jsonKerning.first = pairProp.intValue;
        pairProp.Next(false);
        jsonKerning.second = pairProp.intValue;
        jsonKerning.amount = (int)kerning.FindPropertyRelative("second").floatValue;

        result.kernings.Add(jsonKerning);
      }

      // JSON_Font_Info      
      result.info.face = font.name;
      result.info.size = font.fontSize;

      // JSON_Font_Commom
      result.common.lineHeight = font.lineHeight;
      result.common.lineBaseHeight = so.FindProperty("m_Ascent").floatValue;
      result.common.scaleW = tex.width;
      result.common.scaleH = tex.height;

      return result;
    }
  }
}
