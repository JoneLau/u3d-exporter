using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace exsdk {
  // =========================
  // GLTF_Base
  // =========================

  [System.Serializable]
  public class GLTF_Base {
    public Dictionary<string,object> extensions;
    public Dictionary<string,object> extras;

    public bool ShouldSerializeextensions () {
      return extensions != null && extensions.Count != 0;
    }

    public bool ShouldSerializeextras () {
      return extras != null && extras.Count != 0;
    }
  }

  // =========================
  // GLTF
  // =========================

  [System.Serializable]
  public class GLTF : GLTF_Base {
    public GLTF_Asset asset;
    public string scene;
    public List<string> extensionsUsed;
    public List<GLTF_Accessor> accessors;
    public List<GLTF_Animation> animations;
    public List<GLTF_Buffer> buffers;
    public List<GLTF_BufferView> bufferViews;
    public List<GLTF_Camera> cameras;
    public List<GLTF_Image> images;
    public List<GLTF_Material> materials;
    public List<GLTF_Mesh> meshes;
    public List<GLTF_Node> nodes;
    public List<GLTF_Program> programs;
    public List<GLTF_Sampler> samplers;
    public List<GLTF_Scene> scenes;
    public List<GLTF_Shader> shaders;
    public List<GLTF_Skin> skins;
    public List<GLTF_Technique> techniques;
    public List<GLTF_Texture> textures;

    public bool ShouldSerializeasset () {
      return asset != null;
    }

    public bool ShouldSerializescene () {
      return string.IsNullOrEmpty(scene) == false;
    }

    public bool ShouldSerializeextensionsUsed () {
      return extensionsUsed != null && extensionsUsed.Count != 0;
    }

    public bool ShouldSerializeaccessors () {
      return accessors != null && accessors.Count != 0;
    }

    public bool ShouldSerializebuffers () {
      return buffers != null && buffers.Count != 0;
    }

    public bool ShouldSerializebufferViews () {
      return bufferViews != null && bufferViews.Count != 0;
    }

    public bool ShouldSerializecameras () {
      return cameras != null && cameras.Count != 0;
    }

    public bool ShouldSerializematerials () {
      return materials != null && materials.Count != 0;
    }

    public bool ShouldSerializemeshes () {
      return meshes != null && meshes.Count != 0;
    }

    public bool ShouldSerializenodes () {
      return nodes != null && nodes.Count != 0;
    }

    public bool ShouldSerializeprograms () {
      return programs != null && programs.Count != 0;
    }

    public bool ShouldSerializescenes () {
      return scenes != null && scenes.Count != 0;
    }

    public bool ShouldSerializeshaders () {
      return shaders != null && shaders.Count != 0;
    }

    public bool ShouldSerializetechniques () {
      return techniques != null && techniques.Count != 0;
    }

    public bool ShouldSerializesamplers () {
      return samplers != null && samplers.Count != 0;
    }

    public bool ShouldSerializetextures () {
      return textures != null && textures.Count != 0;
    }

    public bool ShouldSerializeimages () {
      return images != null && images.Count != 0;
    }

    public bool ShouldSerializeanimations () {
      return animations != null && animations.Count != 0;
    }

    public bool ShouldSerializeskins () {
      return skins != null && skins.Count != 0;
    }
  }

  // =========================
  // GLTF_Asset
  // =========================

  [System.Serializable]
  public class GLTF_Asset : GLTF_Base {
    public string copyright;
    public string generator;
    public string version = "1.0.0";
    public string minVersion;

    public bool ShouldSerializecopyright () {
      return string.IsNullOrEmpty(copyright) == false;
    }

    public bool ShouldSerializegenerator () {
      return string.IsNullOrEmpty(generator) == false;
    }

    public bool ShouldSerializeminVersion () {
      return string.IsNullOrEmpty(minVersion) == false;
    }
  }

  // =========================
  // GLTF_Profile
  // =========================

  [System.Serializable]
  public class GLTF_Profile : GLTF_Base {
    public string api;
    public string version;
  }

  // =========================
  // GLTF_Scene
  // =========================

  [System.Serializable]
  public class GLTF_Scene : GLTF_Base {
    public string name;
    public List<string> nodes;
  }

  // =========================
  // GLTF_Node
  // =========================

  [System.Serializable]
  public class GLTF_Node : GLTF_Base {
    public string name;
    public string jointName;
    public List<string> children;
    public float[] matrix = new float[16] {1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1};
    public float[] translation = new float[3] {0,0,0};
    public float[] rotation = new float[4] {0,0,0,1};
    public float[] scale = new float[3] {1,1,1};
    public List<string> meshes;
    public List<string> skeletons;
    public string camera;
    public string skin;

    // DELME
    // [Newtonsoft.Json.JsonIgnore] public string id {
    //   get {
    //     return Utils.ID(this._gameObject);
    //   }
    // }

    public bool ShouldSerializename () {
      return string.IsNullOrEmpty(name) == false;
    }

    public bool ShouldSerializejointName () {
      return string.IsNullOrEmpty(jointName) == false;
    }

    public bool ShouldSerializeskin () {
      return string.IsNullOrEmpty(skin) == false;
    }

    public bool ShouldSerializechildren () {
      return children != null && children.Count != 0;
    }

    public bool ShouldSerializematrix () {
      return
        matrix[0]  != 1 || matrix[1]  != 0 || matrix[2]  != 0 || matrix[3]  != 0 ||
        matrix[4]  != 0 || matrix[5]  != 1 || matrix[6]  != 0 || matrix[7]  != 0 ||
        matrix[8]  != 0 || matrix[9]  != 0 || matrix[10] != 1 || matrix[11] != 0 ||
        matrix[12] != 0 || matrix[13] != 0 || matrix[14] != 0 || matrix[15] != 1
        ;
    }

    public bool ShouldSerializetranslation () {
      return translation[0] != 0 || translation[1] != 0 || translation[2] != 0;
    }

    public bool ShouldSerializerotation () {
      return rotation[0] != 0 || rotation[1] != 0 || rotation[2] != 0 || rotation[3] != 1;
    }

    public bool ShouldSerializescale () {
      return scale[0] != 1 || scale[1] != 1 || scale[2] != 1;
    }

    public bool ShouldSerializemeshes () {
      return meshes != null && meshes.Count != 0;
    }

    public bool ShouldSerializeskeletons () {
      return skeletons != null && skeletons.Count != 0;
    }

    public bool ShouldSerializecamera () {
      return string.IsNullOrEmpty(camera) == false;
    }
  }

  // =========================
  // GLTF_Mesh
  // =========================

  [System.Serializable]
  public class GLTF_Mesh : GLTF_Base {
    public string name;
    public List<GLTF_Primitive> primitives;
    // public List<int> weights;

    public bool ShouldSerializename () {
      return string.IsNullOrEmpty(name) == false;
    }

    public bool ShouldSerializeprimitives () {
      return primitives != null && primitives.Count != 0;
    }
  }

  // =========================
  // GLTF_Skin
  // =========================

  [System.Serializable]
  public class GLTF_Skin : GLTF_Base {
    public string name;
    public float[] bindShapeMatrix = new float[16] {1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1};
    public string inverseBindMatrices;
    public string[] jointNames;

    public bool ShouldSerializename () {
      return string.IsNullOrEmpty(name) == false;
    }

    public bool ShouldSerializebindShapeMatrix () {
      return
        bindShapeMatrix[0]  != 1 || bindShapeMatrix[1]  != 0 || bindShapeMatrix[2]  != 0 || bindShapeMatrix[3]  != 0 ||
        bindShapeMatrix[4]  != 0 || bindShapeMatrix[5]  != 1 || bindShapeMatrix[6]  != 0 || bindShapeMatrix[7]  != 0 ||
        bindShapeMatrix[8]  != 0 || bindShapeMatrix[9]  != 0 || bindShapeMatrix[10] != 1 || bindShapeMatrix[11] != 0 ||
        bindShapeMatrix[12] != 0 || bindShapeMatrix[13] != 0 || bindShapeMatrix[14] != 0 || bindShapeMatrix[15] != 1
        ;
    }
  }

  // =========================
  // GLTF_Animation
  // =========================

  [System.Serializable]
  public class GLTF_Animation : GLTF_Base {
    public string name;
    public List<GLTF_AnimChannel> channels;
    public Dictionary<string,string> parameters;
    public Dictionary<string,GLTF_AnimSampler> samplers;

    public bool ShouldSerializechannels () {
      return channels != null && channels.Count != 0;
    }

    public bool ShouldSerializeparameters () {
      return parameters != null && parameters.Count != 0;
    }

    public bool ShouldSerializesamplers () {
      return samplers != null && samplers.Count != 0;
    }
  }

  // =========================
  // GLTF_AnimChannel
  // =========================

  [System.Serializable]
  public class GLTF_AnimChannel : GLTF_Base {
    public string sampler;
    public GLTF_AnimTarget target;
  }

  // =========================
  // GLTF_AnimTarget
  // =========================

  [System.Serializable]
  public class GLTF_AnimTarget : GLTF_Base {
    public string id;
    public string path;
  }

  // =========================
  // GLTF_AnimSampler
  // =========================

  [System.Serializable]
  public class GLTF_AnimSampler : GLTF_Base {
    public string input;
    public string interpolation;
    public string output;

    public bool ShouldSerializeinterpolation () {
      return string.IsNullOrEmpty(interpolation) == false;
    }
  }

  // =========================
  // GLTF_Primitive
  // =========================

  [System.Serializable]
  public class GLTF_Primitive : GLTF_Base {
    public Dictionary<string,int> attributes;
    public int indices = -1;
    public int material = -1;
    public int mode = 4;
    // TODO: public List<int> targets;

    public bool ShouldSerializeattributes () {
      return attributes != null && attributes.Count != 0;
    }

    public bool ShouldSerializeindices () {
      return indices != -1;
    }

    public bool ShouldSerializematerial () {
      return material != -1;
    }

    public bool ShouldSerializemode () {
      return mode != 4;
    }
  }

  // =========================
  // GLTF_Accessor
  // =========================

  [System.Serializable]
  public class GLTF_Accessor : GLTF_Base {
    public string name;
    public int bufferView;
    public int byteOffset = 0;
    public int componentType;
    public bool normalized = false;
    public int count;
    public string type;
    public object[] min;
    public object[] max;
    // public object sparse;

    public bool ShouldSerializename () {
      return string.IsNullOrEmpty(name) == false;
    }

    public bool ShouldSerializenormalized () {
      return normalized != false;
    }

    public bool ShouldSerializemin () {
      return min != null && min.Length != 0;
    }

    public bool ShouldSerializemax () {
      return max != null && max.Length != 0;
    }
  }

  // =========================
  // GLTF_BufferView
  // =========================

  [System.Serializable]
  public class GLTF_BufferView : GLTF_Base {
    public string name;
    public int buffer;
    public int byteOffset = 0;
    public int byteLength;
    public int byteStride;
    public int target = -1;

    public bool ShouldSerializename () {
      return string.IsNullOrEmpty(name) == false;
    }

    public bool ShouldSerializebyteLength () {
      return byteLength != 0;
    }

    public bool ShouldSerializebyteStride () {
      return byteStride != 0;
    }

    public bool ShouldSerializetarget () {
      return target != -1;
    }
  }

  // =========================
  // GLTF_Buffer
  // =========================

  [System.Serializable]
  public class GLTF_Buffer : GLTF_Base {
    public string name;
    public string uri;
    public int byteLength = 0;

    public bool ShouldSerializename () {
      return string.IsNullOrEmpty(name) == false;
    }

    public bool ShouldSerializebyteLength () {
      return byteLength != 0;
    }
  }

  // =========================
  // GLTF_Material
  // =========================

  [System.Serializable]
  public class GLTF_Material : GLTF_Base {
    public string name;
    public string technique;
    public Dictionary<string,object> values;

    public bool ShouldSerializename () {
      return string.IsNullOrEmpty(name) == false;
    }

    public bool ShouldSerializetechnique () {
      return string.IsNullOrEmpty(technique) == false;
    }

    public bool ShouldSerializevalues () {
      return values != null && values.Count != 0;
    }
  }

  // =========================
  // GLTF_Technique
  // =========================

  [System.Serializable]
  public class GLTF_Technique : GLTF_Base {
    public string program = "default";
    public string name;
    public Dictionary<string,string> attributes;
    public Dictionary<string,GLTF_Parameter> parameters;
    public Dictionary<string,string> uniforms;
    public GLTF_States states;

    public bool ShouldSerializename () {
      return string.IsNullOrEmpty(name) == false;
    }

    public bool ShouldSerializeattributes () {
      return attributes != null && attributes.Count != 0;
    }

    public bool ShouldSerializeuniforms () {
      return uniforms != null && uniforms.Count != 0;
    }

    public bool ShouldSerializestates () {
      return states != null;
    }
  }

  // =========================
  // GLTF_Parameter
  // =========================

  [System.Serializable]
  public class GLTF_Parameter : GLTF_Base {
    public int count = -1;
    public string node;
    public int type;
    public string semantic;
    public object value;

    public bool ShouldSerializetecount () {
      return count > 0;
    }

    public bool ShouldSerializetenode () {
      return string.IsNullOrEmpty(node) == false;
    }

    public bool ShouldSerializetesemantic () {
      return string.IsNullOrEmpty(semantic) == false;
    }

    public bool ShouldSerializetevalule () {
      return value != null;
    }
  }

  // =========================
  // GLTF_States
  // =========================

  [System.Serializable]
  public class GLTF_States : GLTF_Base {
    public int[] enable;
    public GLTF_Functions functions;

    public bool ShouldSerializeenable () {
      return enable != null && enable.Length != 0;
    }

    public bool ShouldSerializefunctions () {
      return functions != null;
    }
  }

  // =========================
  // GLTF_Functions
  // =========================

  [System.Serializable]
  public class GLTF_Functions : GLTF_Base {
    public float[] blendColor = new float[4] { 0, 0, 0, 0 };
    public int[] blendEquationSeparate = new int[2] {
      (int)BlendEquation.FUNC_ADD, (int)BlendEquation.FUNC_ADD
    };
    public int[] blendFuncSeparate = new int[4] {
      (int)BlendMode.ONE, (int)BlendMode.ZERO, (int)BlendMode.ONE, (int)BlendMode.ZERO,
    };
    public bool[] colorMask = new bool[4] { true, true, true, true };
    public int[] cullFace = new int[1] { (int)CullFace.BACK };
    public int[] depthFunc = new int[1] { (int)DepthFunc.LESS };
    public bool[] depthMask = new bool[1] { true };
    public float[] depthRange = new float[2] { 0, 1 };
    public int[] frontFace = new int[1] { (int)FrontFace.CCW };
    public float[] lineWidth = new float[1] { 1 };
    public float[] polygonOffset = new float[2] { 0, 0 };
    public float[] scissor = new float[4] { 0, 0, 0, 0 };

    public bool ShouldSerializeblendColor () {
      return
        blendColor[0] != 0 ||
        blendColor[1] != 0 ||
        blendColor[2] != 0 ||
        blendColor[3] != 0
        ;
    }

    public bool ShouldSerializeblendEquationSeparate () {
      return
        blendEquationSeparate[0] != (int)BlendEquation.FUNC_ADD ||
        blendEquationSeparate[1] != (int)BlendEquation.FUNC_ADD
        ;
    }

    public bool ShouldSerializecolorMask () {
      return
        colorMask[0] != true ||
        colorMask[1] != true ||
        colorMask[2] != true ||
        colorMask[3] != true
        ;
    }

    public bool ShouldSerializecullFace () {
      return cullFace[0] != (int)CullFace.BACK;
    }

    public bool ShouldSerializedepthFunc () {
      return depthFunc[0] != (int)DepthFunc.LESS;
    }

    public bool ShouldSerializedepthMask () {
      return depthMask[0] != true;
    }

    public bool ShouldSerializedepthRange () {
      return depthRange[0] != 0 || depthRange[1] != 1;
    }

    public bool ShouldSerializefrontFace () {
      return frontFace[0] != (int)FrontFace.CCW;
    }

    public bool ShouldSerializelineWidth () {
      return lineWidth[0] != 1;
    }

    public bool ShouldSerializepolygonOffset () {
      return
        polygonOffset[0] != 0 ||
        polygonOffset[1] != 1;
    }

    public bool ShouldSerializescissor () {
      return
        scissor[0] != 0 ||
        scissor[1] != 0 ||
        scissor[2] != 0 ||
        scissor[3] != 0
        ;
    }
  }

  // =========================
  // GLTF_Program
  // =========================

  [System.Serializable]
  public class GLTF_Program : GLTF_Base {
    public string name;
    public List<string> attributes;
    public string fragmentShader = "shader_default";
    public string vertexShader = "shader_default";

    public bool ShouldSerializename () {
      return string.IsNullOrEmpty(name) == false;
    }

    public bool ShouldSerializeteattributes () {
      return attributes != null && attributes.Count != 0;
    }
  }

  // =========================
  // GLTF_Shader
  // =========================

  [System.Serializable]
  public class GLTF_Shader : GLTF_Base {
    public string name;
    public string uri;
    public int type;

    public bool ShouldSerializename () {
      return string.IsNullOrEmpty(name) == false;
    }
  }

  // =========================
  // GLTF_Texture
  // =========================

  [System.Serializable]
  public class GLTF_Texture : GLTF_Base {
    public string name;
    public int format = (int)TextureFormat.RGBA;
    public int internalFormat = (int)TextureFormat.RGBA;
    public string sampler;
    public string source;
    public int target = (int)TextureTarget.TEXTURE_2D;
    public int type = (int)TexturePixelType.UNSIGNED_BYTE;

    public bool ShouldSerializename () {
      return string.IsNullOrEmpty(name) == false;
    }

    public bool ShouldSerializeformat () {
      return format != (int)TextureFormat.RGBA;
    }

    public bool ShouldSerializeinternalFormat () {
      return internalFormat != (int)TextureFormat.RGBA;
    }

    public bool ShouldSerializetarget () {
      return target != (int)TextureTarget.TEXTURE_2D;
    }

    public bool ShouldSerializetype () {
      return type != (int)TexturePixelType.UNSIGNED_BYTE;
    }
  }

  // =========================
  // GLTF_Sampler
  // =========================

  [System.Serializable]
  public class GLTF_Sampler : GLTF_Base {
    public string name;
    public int magFilter = (int)FilterMode.LINEAR;
    public int minFilter = (int)FilterMode.NEAREST_MIPMAP_LINEAR;
    public int wrapS = (int)WrapMode.REPEAT;
    public int wrapT = (int)WrapMode.REPEAT;

    public bool ShouldSerializename () {
      return string.IsNullOrEmpty(name) == false;
    }

    public bool ShouldSerializemagFilter () {
      return magFilter != (int)FilterMode.LINEAR;
    }

    public bool ShouldSerializeminFilter () {
      return minFilter != (int)FilterMode.NEAREST_MIPMAP_LINEAR;
    }

    public bool ShouldSerializewrapS () {
      return wrapS != (int)WrapMode.REPEAT;
    }

    public bool ShouldSerializewrapT () {
      return wrapT != (int)WrapMode.REPEAT;
    }
  }

  // =========================
  // GLTF_Image
  // =========================

  [System.Serializable]
  public class GLTF_Image : GLTF_Base {
    public string name;
    public string uri;

    public bool ShouldSerializename () {
      return string.IsNullOrEmpty(name) == false;
    }
  }

  // =========================
  // GLTF_Orthographic
  // =========================

  [System.Serializable]
  public class GLTF_Orthographic : GLTF_Base {
    public float xmag;
    public float ymag;
    public float zfar;
    public float znear;
  }

  // =========================
  // GLTF_Perspective
  // =========================

  [System.Serializable]
  public class GLTF_Perspective : GLTF_Base {
    public float aspectRatio = 0;
    public float yfov;
    public float zfar;
    public float znear;

    public bool ShouldSerializeaspectRatio () {
      return aspectRatio > 0;
    }
  }

  // =========================
  // GLTF_Camera
  // =========================

  [System.Serializable]
  public class GLTF_Camera : GLTF_Base {
    public string name;
    public string type = "perspective";
    public GLTF_Orthographic orthographic;
    public GLTF_Perspective perspective;

    public bool ShouldSerializename () {
      return string.IsNullOrEmpty(name) == false;
    }

    public bool ShouldSerializeorthographic () {
      return orthographic != null;
    }

    public bool ShouldSerializeorthperspective () {
      return perspective != null;
    }
  }
}
